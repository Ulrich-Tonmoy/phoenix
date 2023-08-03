﻿using Phoenix.Editor.Common;
using Phoenix.Editor.Components;
using Phoenix.Editor.DllWrapper;
using Phoenix.Editor.GameDev;
using Phoenix.Editor.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Phoenix.Editor.GameProject
{
    enum BuildConfiguration
    {
        Debug,
        DebugEditor,
        Release,
        ReleaseEditor
    }

    [DataContract(Name = "Game")]
    class Project : ViewModelBase
    {
        public static string Extension { get; } = ".phoenix";
        [DataMember]
        public string Name { get; private set; } = "New Project";
        [DataMember]
        public string Path { get; private set; }
        public string FullPath => $@"{Path}{Name}{Extension}";
        public string Solution => $@"{Path}{Name}.sln";
        public string AssetPath => $@"{Path}Assets\";

        private static readonly string[] _buildConfigurationNames = new string[] { "Debug", "DebugEditor", "Release", "ReleaseEditor" };
        private int _buildConfig;
        [DataMember]
        public int BuildConfig
        {
            get => _buildConfig;
            set
            {
                if (_buildConfig != value)
                {
                    _buildConfig = value;
                    OnPropertyChanged(nameof(BuildConfig));
                }
            }
        }

        public BuildConfiguration StandAloneBuildConfig => BuildConfig == 0 ? BuildConfiguration.Debug : BuildConfiguration.Release;
        public BuildConfiguration DllBuildConfig => BuildConfig == 0 ? BuildConfiguration.DebugEditor : BuildConfiguration.ReleaseEditor;

        private string[] _availableScripts;

        public string[] AvailableScripts
        {
            get => _availableScripts;
            set
            {
                if (_availableScripts != value)
                {
                    _availableScripts = value;
                    OnPropertyChanged(nameof(AvailableScripts));
                }
            }
        }


        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }
        private Scene _activeScene;
        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                if (_activeScene != value)
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }
        public static Project Current => Application.Current.MainWindow.DataContext as Project;
        public static UndoRedo UndoRedo { get; } = new UndoRedo();
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveSceneCommand { get; private set; }
        public ICommand BuildCommand { get; private set; }
        public ICommand DebugStartCommand { get; private set; }
        public ICommand DebugStartWithoutDebuggingCommand { get; private set; }
        public ICommand DebugStopCommand { get; private set; }

        private static string GetConfigurationName(BuildConfiguration config) => _buildConfigurationNames[(int)config];

        public Project(string name, string path)
        {
            Name = name;
            Path = path;
            OnDeserialized(new StreamingContext());
        }

        [OnDeserialized]
        private async void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);
            Debug.Assert(ActiveScene != null);

            await BuildScriptDll(false);

            SetCommands();
        }

        private void SetCommands()
        {
            AddSceneCommand = new RelayCommand<object>(x =>
            {
                AddScene($"New Scene {_scenes.Count}");
                var newScene = _scenes.Last();
                var sceneIndex = _scenes.Count - 1;
                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => _scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"
                    ));
            });
            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                var sceneIndex = _scenes.IndexOf(x);
                RemoveScene(x);
                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveScene(x),
                    $"Remove {x.Name}"
                    ));
            }, x => !x.IsActive);

            UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo(), x => UndoRedo.UndoList.Any());
            RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo(), x => UndoRedo.RedoList.Any());
            SaveCommand = new RelayCommand<object>(x => Save(this));
            BuildCommand = new RelayCommand<bool>(async x => await BuildScriptDll(x), x => !VisualStudio.IsDebugging() && VisualStudio.BuildCompleted);
            DebugStartCommand = new RelayCommand<object>(async x => await RunGame(true), x => !VisualStudio.IsDebugging() && VisualStudio.BuildCompleted);
            DebugStartWithoutDebuggingCommand = new RelayCommand<object>(async x => await RunGame(false), x => !VisualStudio.IsDebugging() && VisualStudio.BuildCompleted);
            DebugStopCommand = new RelayCommand<object>(async x => await StopGame(), x => VisualStudio.IsDebugging());

            OnPropertyChanged(nameof(AddSceneCommand));
            OnPropertyChanged(nameof(RemoveSceneCommand));
            OnPropertyChanged(nameof(UndoCommand));
            OnPropertyChanged(nameof(RedoCommand));
            OnPropertyChanged(nameof(SaveCommand));
            OnPropertyChanged(nameof(BuildCommand));
            OnPropertyChanged(nameof(DebugStartCommand));
            OnPropertyChanged(nameof(DebugStartWithoutDebuggingCommand));
            OnPropertyChanged(nameof(DebugStopCommand));
        }

        private void AddScene(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }

        private void RemoveScene(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public void Unload()
        {
            UnloadScriptDll();
            VisualStudio.CloseVisualStudio();
            UndoRedo.Reset();
        }

        public static void Save(Project project)
        {
            Serializer.ToFile(project, project.FullPath);
            Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
        }

        private async Task BuildScriptDll(bool showWindow = true)
        {
            try
            {
                UnloadScriptDll();
                await Task.Run(() => VisualStudio.BuildSolution(this, GetConfigurationName(DllBuildConfig), showWindow));
                if (VisualStudio.BuildSucceeded)
                {
                    LoadScriptDll();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private void SaveToBinary()
        {
            string configName = GetConfigurationName(StandAloneBuildConfig);
            var bin = $@"{Path}x64\{configName}\data.bin";

            using (var bw = new BinaryWriter(File.Open(bin, FileMode.Create, FileAccess.Write)))
            {
                bw.Write(ActiveScene.GameEntities.Count);
                foreach (var entity in ActiveScene.GameEntities)
                {
                    bw.Write(0);
                    bw.Write(entity.Components.Count);
                    foreach (var component in entity.Components)
                    {
                        bw.Write((int)component.ToEnumType());
                        component.WriteToBinary(bw);
                    }
                }
            }
        }

        private async Task RunGame(bool debug)
        {
            string configName = GetConfigurationName(StandAloneBuildConfig);
            await Task.Run(() => VisualStudio.BuildSolution(this, configName, debug));
            if (VisualStudio.BuildSucceeded)
            {
                SaveToBinary();
                await Task.Run(() => VisualStudio.Run(this, configName, debug));
            }
        }

        private async Task StopGame() => await Task.Run(() => VisualStudio.Stop());

        private void LoadScriptDll()
        {
            var configName = GetConfigurationName(DllBuildConfig);
            var dll = $@"{Path}x64\{configName}\{Name}.dll";
            AvailableScripts = null;
            if (File.Exists(dll) && EngineAPI.LoadScriptDll(dll) != 0)
            {
                AvailableScripts = EngineAPI.GetScriptNames();
                ActiveScene.GameEntities.Where(x => x.GetComponent<Script>() != null).ToList().ForEach(x => x.IsActive = true);
                Logger.Log(MessageType.Info, "Script DLL loaded successfully.");
            }
            else
                Logger.Log(MessageType.Error, "Failed to load Script DLL files. Try to build the project first.");
        }

        private void UnloadScriptDll()
        {
            ActiveScene.GameEntities.Where(x => x.GetComponent<Script>() != null).ToList().ForEach(x => x.IsActive = false);
            if (EngineAPI.UnloadScriptDll() != 0)
            {
                Logger.Log(MessageType.Info, "Script DLL unloaded.");
                AvailableScripts = null;
            }
        }
    }
}