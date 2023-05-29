using Phoenix.Editor.Components;
using Phoenix.Editor.GameProject;
using Phoenix.Editor.Utilities;
using System.Linq;
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
            GameEntityView.Instance.DataContext = null;
            var listBox = sender as ListBox;
            if (e.AddedItems.Count > 0)
            {
                GameEntityView.Instance.DataContext = listBox.SelectedItem;
            }
            var newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList();
            var prevSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

            Project.UndoRedo.Add(new UndoRedoAction(
                () =>
                {
                    listBox.UnselectAll();
                    prevSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                },
                () =>
                {
                    listBox.UnselectAll();
                    newSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                },
                "Selection chnaged"
                ));
        }
    }
}
