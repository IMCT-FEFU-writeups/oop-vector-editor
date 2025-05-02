using ReactiveUI;
using System.Collections.Generic;
using System;
using Avalonia.Media;
using System.Reactive;
using System.Windows.Input; // Added for ICommand

namespace vector_editor
{
    public enum MouseButton
    {
        Left,
        Right
    }

    public class ViewModelBase : ReactiveObject
    {
        private Canvas _canvas;
        private Elements _selectedElement;
        private Effects _selectedEffect;
        public ViewModelBase (Canvas canvas){
            _canvas = canvas;
            _selectedElement = canvas.selectedElement;
            _selectedEffect = canvas.selectedEffect;
            NewLineCommand = ReactiveCommand.Create(NewLine);
            DeleteLineCommand = ReactiveCommand.Create(DeleteLine);
            HandleColorClickCommand = ReactiveCommand.Create<(Color color, MouseButton button)>(HandleColorClick);

            SaveCommand = ReactiveCommand.Create<string>(SaveCanvas);
            LoadCommand = ReactiveCommand.Create<string>(LoadCanvas);
            ClearCanvasCommand = ReactiveCommand.Create(ClearCanvas);


            this.WhenAnyValue(x => x._canvas.selectedElement)
                .Subscribe(value => {
                    _selectedElement = value;
                    this.RaisePropertyChanged(nameof(SelectedElement));
                });
        }

        public ReactiveCommand<Unit, Unit> NewLineCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteLineCommand { get; }
        public ReactiveCommand<(Color color, MouseButton button), Unit> HandleColorClickCommand { get; }

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearCanvasCommand { get; }


        private void NewLine()
        {
            _canvas.CreateNewLine();
            NotifyDrawnElementsChanged();
        }

        private void DeleteLine()
        {
            _canvas.drawnElements.RemoveAt(SelectedIndex);
            NotifyDrawnElementsChanged();
        }

        private void SaveCanvas(string filePath)
        {
            _canvas.SaveDrawnElements(filePath);
        }

        private void LoadCanvas(string filePath)
        {
            _canvas.LoadDrawnElements(filePath);
            NotifyDrawnElementsChanged();
        }

        private void ClearCanvas()
        {
            _canvas.ClearCanvas();
            NotifyDrawnElementsChanged();
        }


        public int SelectedIndex
        {
            get => _canvas.activeElement;
            set {
                _canvas.activeElement = value;
                this.RaisePropertyChanged(nameof(SelectedIndex));
            }
        }

        public Elements SelectedElement
        {
            get => _selectedElement;
            set {
                Console.WriteLine(value);
                _selectedElement = value;
                _canvas.selectedElement = value;
                this.RaisePropertyChanged(nameof(SelectedElement));
                NotifyDrawnElementsChanged();
            }
        }

        public Effects SelectedEffect
        {
            get => _selectedEffect;
            set {
                Console.WriteLine(value);
                _selectedEffect = value;
                _canvas.selectedEffect = value;
                this.RaisePropertyChanged(nameof(SelectedEffect));
                NotifyDrawnElementsChanged();
            }
        }


        public List<string> DrawnElements
        {
            get => _canvas.DrawnElementsList();
        }

        public Color Color
        {
            get => _canvas.color;
            set {
                _canvas.color = value;
                this.RaisePropertyChanged(nameof(Color));
                NotifyDrawnElementsChanged();
            }
        }
        public Color FillColor
        {
            get => _canvas.fillColor;
            set {
                _canvas.fillColor = value;
                this.RaisePropertyChanged(nameof(FillColor));
                NotifyDrawnElementsChanged();
            }
        }
        private void HandleColorClick((Color color, MouseButton button) info)
        {
            if (info.button == MouseButton.Left)
            {
                Color = info.color;
            }
            else if (info.button == MouseButton.Right)
            {
                FillColor = info.color;
            }
        }

        public void NotifyDrawnElementsChanged()
        {
            SelectedIndex = _canvas.DrawnElementsList().Count - 1;
            this.RaisePropertyChanged(nameof(DrawnElements));
            this.RaisePropertyChanged(nameof(SelectedIndex));
        }
    }
}
