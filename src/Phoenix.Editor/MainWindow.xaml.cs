using Phoenix.Editor.GameProject;
using System.Windows;

namespace Phoenix.Editor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWinodowLoaded;
        }

        private void OnMainWinodowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWinodowLoaded;
            OpenProjectBrowserDialog();
        }

        private void OpenProjectBrowserDialog()
        {
            var projectBrowser = new ProjectBrowserDialog();
            if (projectBrowser.ShowDialog() == false)
                Application.Current.Shutdown();
            else
            {

            }
        }
    }
}
