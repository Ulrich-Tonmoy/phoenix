using Phoenix.Editor.GameProject;
using System.ComponentModel;
using System.Windows;

namespace Phoenix.Editor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWinodowLoaded;
            Closing += OnMainWiindowClosing;
        }

        private void OnMainWinodowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWinodowLoaded;
            OpenProjectBrowserDialog();
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
