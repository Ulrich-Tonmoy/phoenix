using Phoenix.Editor.GameProject;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Phoenix.Editor.Editors
{
    public partial class WorldEditorView : UserControl
    {
        public WorldEditorView()
        {
            InitializeComponent();
            Loaded += OnWorldEditorViewLoaded;
        }

        private void OnWorldEditorViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnWorldEditorViewLoaded;
            Focus();
            ((INotifyCollectionChanged)Project.UndoRedo.UndoList).CollectionChanged += (s, e) => Focus();
        }
    }
}
