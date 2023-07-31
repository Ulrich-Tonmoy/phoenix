﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace Phoenix.Editor.Editors
{
    public partial class GeometryView : UserControl
    {
        private Point _clickedPosition;
        private bool _capturedLeft;
        private bool _capturedRight;

        public GeometryView()
        {
            InitializeComponent();
            DataContextChanged += (s, e) => SetGeometry();
        }

        private void SetGeometry(int index = -1)
        {
            if (!(DataContext is MeshRenderer vm)) return;

            if (vm.Meshes.Any() && viewport.Children.Count == 2)
                viewport.Children.RemoveAt(1);

            var meshIndex = 0;
            var modelGroup = new Model3DGroup();

            foreach (var mesh in vm.Meshes)
            {
                if (index != -1 && meshIndex != index)
                {
                    ++meshIndex;
                    continue;
                }

                var mesh3D = new MeshGeometry3D()
                {
                    Positions = mesh.Positions,
                    Normals = mesh.Normals,
                    TriangleIndices = mesh.Indices,
                    TextureCoordinates = mesh.UVs
                };

                var diffuese = new DiffuseMaterial(mesh.Diffuse);
                var specular = new SpecularMaterial(mesh.Specular, 50);
                var matGroup = new MaterialGroup();
                matGroup.Children.Add(diffuese);
                matGroup.Children.Add(specular);

                var model = new GeometryModel3D(mesh3D, matGroup);
                modelGroup.Children.Add(model);

                var binding = new Binding(nameof(mesh.Diffuse)) { Source = mesh };
                BindingOperations.SetBinding(diffuese, DiffuseMaterial.BrushProperty, binding);

                if (meshIndex == index) break;
            }
            var visual = new ModelVisual3D() { Content = modelGroup };
            viewport.Children.Add(visual);
        }

        private void OnGrid_Mouse_LBD(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _clickedPosition = e.GetPosition(this);
            _capturedLeft = true;
            Mouse.Capture(sender as UIElement);
        }

        private void OnGrid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_capturedLeft && !_capturedRight) return;

            Point pos = e.GetPosition(this);
            Vector d = pos - _clickedPosition;

            if (_capturedLeft && !_capturedRight)
            {
                MoveCamera(d.X, d.Y, 0);
            }
            else if (!_capturedLeft && _capturedRight)
            {
                MeshRenderer vm = DataContext as MeshRenderer;
                Point3D cp = vm.CameraPosition;
                double yOffset = d.Y * 0.001 * Math.Sqrt(cp.X * cp.X + cp.Z * cp.Z);
                vm.CameraTarget = new Point3D(vm.CameraTarget.X, vm.CameraTarget.Y + yOffset, vm.CameraTarget.Z);
            }

            _clickedPosition = pos;
        }

        private void OnGrid_Mouse_LBU(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _capturedLeft = false;
            if (!_capturedRight) Mouse.Capture(null);
        }

        private void OnGrid_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            MoveCamera(0, 0, Math.Sign(e.Delta));
        }

        private void OnGrid_Mouse_RBD(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _clickedPosition = e.GetPosition(this);
            _capturedRight = true;
            Mouse.Capture(sender as UIElement);
        }

        private void OnGrid_Mouse_RBU(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _capturedRight = false;
            if (!_capturedLeft) Mouse.Capture(null);
        }

        private void MoveCamera(double dx, double dy, int dz)
        {
            MeshRenderer vm = DataContext as MeshRenderer;
            Vector3D v = new Vector3D(vm.CameraPosition.X, vm.CameraPosition.Y, vm.CameraPosition.Z);

            double r = v.Length;
            double theta = Math.Acos(v.Y / r);
            double pi = Math.Atan2(-v.Z, v.X);

            theta -= dy * 0.01;
            pi -= dx * 0.01;
            r *= 1.0 - 0.1 * dz;

            theta = Math.Clamp(theta, 0.0001, Math.PI - 0.0001);

            v.X = r * Math.Sin(theta) * Math.Cos(pi);
            v.Y = r * Math.Cos(theta);
            v.Z = -r * Math.Sin(theta) * Math.Sin(pi);

            vm.CameraPosition = new Point3D(v.X, v.Y, v.Z);
        }
    }
}
