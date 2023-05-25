using System.Windows;
using System.Windows.Controls;

namespace Phoenix.Editor.GameProject
{
    public partial class OpenProjectView : UserControl
    {
        public OpenProjectView()
        {
            InitializeComponent();
        }

        private void OnOpen_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenSelectedProject();
        }
        private void OnListBoxItem_DoubleClick(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenSelectedProject();
        }

        private void OpenSelectedProject()
        {
            var project = OpenProject.Open(projectsListBox.SelectedItem as ProjectData);
            var dialogResult = false;
            var win = Window.GetWindow(this);
            if (project is not null)
            {
                dialogResult = true;
                win.DataContext = project;
            }
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
