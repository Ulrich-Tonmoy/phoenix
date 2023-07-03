using Phoenix.Editor.Components;
using Phoenix.Editor.GameProject;
using Phoenix.Editor.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace Phoenix.Editor.Editors
{
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChanged = false;

        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoader;
        }

        private void OnTransformViewLoader(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTransformViewLoader;
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
        }

        private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector, Action<(Transform transform, Vector3)> forEachAction)
        {
            if (!(DataContext is MSTransform vm))
            {
                _undoAction = null;
                _propertyChanged = false;
                return null;
            }

            var selection = vm.SelectedComponents.Select(x => selector(x)).ToList();
            return new Action(() =>
            {
                selection.ForEach(x => forEachAction(x));
                (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
            });
        }

        private Action GetPositionAction() => GetAction(x => (x, x.Position), x => x.transform.Position = x.Item2);
        private Action GetRotationAction() => GetAction(x => (x, x.Rotation), x => x.transform.Rotation = x.Item2);
        private Action GetScaleAction() => GetAction(x => (x, x.Scale), x => x.transform.Scale = x.Item2);

        private void RecordActions(Action redoAction, string name)
        {
            if (_propertyChanged)
            {
                Debug.Assert(_undoAction != null);
                _propertyChanged = false;
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }

        private void OnPosition_VectorBox_PreviewMouse_LBD(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetPositionAction();
        }
        private void OnPosition_VectorBox_PreviewMouse_LBU(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var redoAction = GetPositionAction();
            RecordActions(redoAction, "Position changed");
        }

        private void OnRotation_VectorBox_PreviewMouse_LBD(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetRotationAction();
        }
        private void OnRotation_VectorBox_PreviewMouse_LBU(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var redoAction = GetRotationAction();
            RecordActions(redoAction, "Rotation changed");
        }

        private void OnScale_VectorBox_PreviewMouse_LBD(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetScaleAction();
        }
        private void OnScale_VectorBox_PreviewMouse_LBU(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var redoAction = GetScaleAction();
            RecordActions(redoAction, "Scale changed");
        }

        private void OnPosition_VectorBox_LostkeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnPosition_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }

        private void OnRotation_VectorBox_LostkeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnRotation_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }

        private void OnScale_VectorBox_LostkeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnScale_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }
    }
}
