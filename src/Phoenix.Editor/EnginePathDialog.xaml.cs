using System.IO;
using System.Windows;

namespace Phoenix.Editor
{
    public partial class EnginePathDialog : Window
    {
        public string PhoenixPath { get; private set; }

        public EnginePathDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void OnSave_Button_Click(object sender, RoutedEventArgs e)
        {
            var path = pathTextBox.Text.Trim();
            messageTextBlock.Text = string.Empty;
            if (string.IsNullOrEmpty(path))
                messageTextBlock.Text = "Invalid Path.";
            else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                messageTextBlock.Text = "Invalid character(s) used in path.";
            else if (!Directory.Exists(Path.Combine(path, @"Phoenix.Core/EngineAPI/")))
                messageTextBlock.Text = $"Unable to find the engine in '{path}'.";
            if (string.IsNullOrEmpty(messageTextBlock.Text))
            {
                if (!Path.EndsInDirectorySeparator(path)) path += @"/";
                PhoenixPath = path;
                DialogResult = true;
                Close();
            }
        }
    }
}
