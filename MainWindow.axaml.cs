using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using ReactiveUI;

using Classic.Avalonia;
using Classic.CommonControls.Dialogs;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace vector_editor {
    public partial class MainWindow : Window
    {
        private Canvas _canvas = new Canvas();
        private bool _isPointerDown = false;
        private Point _lastPoint;

        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new ViewModelBase(_canvas);
            DataContext = viewModel;

            // Subscribe to all property changes in the ViewModel
            viewModel.WhenAnyValue(
                x => x.SelectedIndex,
                x => x.SelectedElement,
                x => x.Color,
                x => x.FillColor,
                x => x.DrawnElements
            ).Subscribe(_ => Render());

            AddHandler(SizeChangedEvent, OnWindowSizeChanged, handledEventsToo: true);

            OutputImage.AddHandler(PointerPressedEvent, MouseDownHandler, handledEventsToo: true);
            OutputImage.AddHandler(PointerReleasedEvent, MouseUpHandler, handledEventsToo: true);
            OutputImage.AddHandler(PointerMovedEvent, MouseMovedHandler, handledEventsToo: true);
        }

        private void Render() {
            OutputImage.Source = _canvas.Render();
        }

        private void OnWindowSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            _canvas.width = (int)e.NewSize.Width;
            _canvas.height = (int)e.NewSize.Height;
            Render();
        }

        private void MouseDownHandler(object? sender, PointerEventArgs e)
        {
            _isPointerDown = true;
            var point = e.GetCurrentPoint(sender as Control);
            _canvas.OnMouseDown((int) point.Position.X + 100, (int) point.Position.Y);
            Render();
        }

        private void MouseUpHandler(object? sender, PointerEventArgs e)
        {
            _isPointerDown = false;
        }

        private void MouseMovedHandler(object? sender, PointerEventArgs e) {
            var position = e.GetCurrentPoint(sender as Control).Position;
            if (_isPointerDown) {
                _canvas.OnMouseMove(position, _lastPoint);
                Render();
            }
            _lastPoint = position;
        }
        private void CellRightClick(object? sender, PointerPressedEventArgs e)
        {
            if (sender is Button button && button.Background is ImmutableSolidColorBrush brush)
            {
                if (DataContext is ViewModelBase viewModel)
                {
                    viewModel.FillColor = brush.Color;
                }
            }
        }

        private void CellLeftClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Background is ImmutableSolidColorBrush brush)
            {
                if (DataContext is ViewModelBase viewModel)
                {
                    viewModel.Color = brush.Color;
                }
            }
        }

        private async void AboutMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            await AboutDialog.ShowDialog(this, new AboutDialogOptions()
            {
                Title = "Avalonia Internet Explorer",
                Copyright = "Copyleft (R) 2024 bandysc",
            });
        }
    }
}
