using Phoenix.Editor.GameProject;
using System.Windows.Controls;

namespace Phoenix.Editor.Editors
{
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddScene_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = DataContext as Project;
            vm.AddScene("New Scene" + vm.Scenes.Count);
        }
    }
}
