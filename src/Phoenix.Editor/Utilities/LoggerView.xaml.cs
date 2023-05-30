using System.Windows.Controls;

namespace Phoenix.Editor.Utilities
{
    public partial class LoggerView : UserControl
    {
        public LoggerView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                Logger.Log(MessageType.Info, "Info Message");
                Logger.Log(MessageType.Warning, "Warning Message");
                Logger.Log(MessageType.Error, "Error Message");
            };
        }

        private void OnClear_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Logger.Clear();
        }

        private void OnMsgFilter_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var filter = 0x0;
            if (toggleInfo.IsChecked == true) filter |= (int)MessageType.Info;
            if (toggleWarn.IsChecked == true) filter |= (int)MessageType.Warning;
            if (toggleError.IsChecked == true) filter |= (int)MessageType.Error;
            Logger.SetMessageFilter(filter);
        }
    }
}
