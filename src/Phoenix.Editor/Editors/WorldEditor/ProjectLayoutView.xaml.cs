using Phoenix.Editor.Components;
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

        private void OnAddGameEntity_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as Button;
            var vm = btn.DataContext as Scene;
            vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Object" });
        }

        private void OnGameEntiities_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var entiry = (sender as ListBox).SelectedItem;
            GameEntityView.Instance.DataContext = entiry;
        }
    }
}
