using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Immutable;
using ReactiveUI;
using Classic.CommonControls.Dialogs;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.IO;
using System.Collections.Generic; 
using System.Threading.Tasks;

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

            SaveMenuItem.Click += SaveMenuItem_Click;
            OpenMenuItem.Click += OpenMenuItem_Click;
            ExportSvgMenuItem.Click += ExportSvgMenuItem_Click;
            ExitMenuItem.Click += ExitMenuItem_Click;
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
                Title = "Vector editor",
                Copyright = "Copyleft (or copymiddle, i don't care really)",
            });
        }

        private async void SaveMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filters.Add(new FileDialogFilter() { Name = "Vector Editor Files", Extensions = { "vec" } });
            var result = await saveFileDialog.ShowAsync(this);

            if (result != null && DataContext is ViewModelBase viewModel)
            {
                viewModel.SaveCommand.Execute(result);
            }
        }

        private async void OpenMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filters.Add(new FileDialogFilter() { Name = "Vector Editor Files", Extensions = { "vec" } });
            var result = await openFileDialog.ShowAsync(this);

            if (result != null && result.Length > 0 && DataContext is ViewModelBase viewModel)
            {
                viewModel.LoadCommand.Execute(result[0]);
            }
        }

        private async void ExportSvgMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filters.Add(new FileDialogFilter() { Name = "Scalable vector graphics", Extensions = { "svg" } });
            var result = await saveFileDialog.ShowAsync(this);

            if (result != null && DataContext is ViewModelBase viewModel)
            {
                _canvas.ExportToSvg(result);
            }
        }

        private void ExitMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
