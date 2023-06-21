using Phoenix.Editor.GameProject;
using Phoenix.Editor.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Phoenix.Editor.GameDev
{
    public partial class NewScriptDialog : Window
    {
        private static readonly string _cppCode = @"#include ""{0}.h""

                                                    namespace {1}
                                                    {{
	                                                    REGISTER_SCRIPT({0});

                                                        void {0}::begin_play()
	                                                    {{

	                                                    }}

	                                                    void {0}::update(float dt)
	                                                    {{

	                                                    }}
                                                    }}";
        private static readonly string _hCode = @"#pragma once

                                                    namespace {1}
                                                    {{
	                                                    class {0} :public phoenix::script::entity_script
	                                                    {{
	                                                    public:
		                                                    constexpr explicit {0}(phoenix::game_entity::entity entity) :phoenix::script::entity_script{{entity}} {{}}
                                                            void begin_play() override;
		                                                    void update(float dt) override;
                                                        private:
	                                                    }};
                                                    }}";
        public NewScriptDialog()
        {
            InitializeComponent();
        }

        bool Validate()
        {
            bool isValid = false;
            var name = scriptName.Text.Trim();
            var path = scriptPath.Text.Trim();
            string errorMsg = string.Empty;

            if (string.IsNullOrEmpty(name))
                errorMsg = "Type in a script name.";
            else if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 || name.Any(x => char.IsWhiteSpace(x)))
                errorMsg = "Invalid character(s) used in script name.";
            if (string.IsNullOrEmpty(path))
                errorMsg = "Select a valid script folder.";
            else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                errorMsg = "Invalid character(s) used in path.";
            else if (!Path.GetFullPath(Path.Combine(Project.Current.Path, path)).Contains(Path.Combine(Project.Current.Path, @"Scripts\")))
                errorMsg = "Script must be added to (a sub folder) Scripts.";
            else if (File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.cpp"))) || File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.h"))))
                errorMsg = $"Script {name} already exists in this folder.";
            else
                isValid = true;
            if (!isValid)
                messageTextBlock.Foreground = FindResource("Editor.RedBrush") as Brush;
            else
                messageTextBlock.Foreground = FindResource("Editor.FontBrush") as Brush;

            messageTextBlock.Text = errorMsg;
            return isValid;
        }

        private void OnScriptName_TexBox_TextChange(object sender, TextChangedEventArgs e)
        {
            if (!Validate()) return;
            var name = scriptName.Text.Trim();
            messageTextBlock.Text = $"{name}.h and {name}.cpp will be added to {Project.Current.Name}";
        }

        private void OnScriptPath_TexBox_TextChange(object sender, TextChangedEventArgs e)
        {
            Validate();
        }

        private async void OnCreate_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;
            IsEnabled = false;

            try
            {
                await Task.Run(() => CreateScript(name, path, solution, projectName));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to create script {scriptName.Text}");
            }
        }

        private void CreateScript(string name, string path, string solution, string projectName)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var cpp = Path.GetFullPath(Path.Combine(path, $"{name}.cpp"));
            var h = Path.GetFullPath(Path.Combine(path, $"{name}.h"));

            using (var sw = File.CreateText(cpp))
            {

            }
            using (var sw = File.CreateText(h))
            {

            }
        }
    }
}
