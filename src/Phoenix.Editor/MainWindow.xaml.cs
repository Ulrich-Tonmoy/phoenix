using Phoenix.Editor.GameProject;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Phoenix.Editor
{
    public partial class MainWindow : Window
    {
        public static string PhoenixPath { get; private set; } = @"F:\TEST\phoenix\src";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWinodowLoaded;
            Closing += OnMainWiindowClosing;
        }

        private void OnMainWinodowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWinodowLoaded;
            GetEnginePath();
            OpenProjectBrowserDialog();
        }

        private void GetEnginePath()
        {
            var phoenixPath = Environment.GetEnvironmentVariable("PHOENIX_ENGINE", EnvironmentVariableTarget.User);
            if (phoenixPath == null || !Directory.Exists(Path.Combine(phoenixPath, @"Phoenix.Core\EngineAPI")))
            {
                var dlg = new EnginePathDialog();
                if (dlg.ShowDialog() == true)
                {
                    PhoenixPath = dlg.PhoenixPath;
                    Environment.SetEnvironmentVariable("PHOENIX_ENGINE", PhoenixPath.ToUpper(), EnvironmentVariableTarget.User);
                }
                else
                {
                    // TODO: uncomment this for release
                    //Application.Current.Shutdown();
                }
            }
            else
            {
                PhoenixPath = phoenixPath;
            }
        }

        private void OnMainWiindowClosing(object sender, CancelEventArgs e)
        {
            Closing -= OnMainWiindowClosing;
            Project.Current?.Unload();
        }

        private void OpenProjectBrowserDialog()
        {
            var projectBrowser = new ProjectBrowserDialog();
            if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext is null)
                Application.Current.Shutdown();
            else
            {
                Project.Current?.Unload();
                DataContext = projectBrowser.DataContext;
            }
        }
    }
}
