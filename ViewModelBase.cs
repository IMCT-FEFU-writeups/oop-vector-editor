using ReactiveUI;
using System.Collections.Generic;
using System;
using Avalonia.Media;
using System.Reactive;
using Avalonia.Controls;

namespace vector_editor
{
    public class ViewModelBase : ReactiveObject
    {
        private Canvas _canvas;
        private Elements _selectedElement;
        public ViewModelBase (Canvas canvas){
            _canvas = canvas;
            _selectedElement = canvas.selectedElement;
            NewLineCommand = ReactiveCommand.Create(NewLine);
            DeleteLineCommand = ReactiveCommand.Create(DeleteLine);
            TransformCommand = ReactiveCommand.Create(Transform);
            RotateCommand = ReactiveCommand.Create(Rotate);
            ScaleCommand = ReactiveCommand.Create(Scale);
            DoneCommand = ReactiveCommand.Create(Done);
            
            // Subscribe to canvas property changes
            this.WhenAnyValue(x => x._canvas.selectedElement)
                .Subscribe(value => {
                    _selectedElement = value;
                    this.RaisePropertyChanged(nameof(SelectedElement));
                });
        }

        public ReactiveCommand<Unit, Unit> NewLineCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteLineCommand { get; }
        public ReactiveCommand<Unit, Unit> TransformCommand { get; }
        public ReactiveCommand<Unit, Unit> RotateCommand { get; }
        public ReactiveCommand<Unit, Unit> ScaleCommand { get; }
        public ReactiveCommand<Unit, Unit> DoneCommand { get; }

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

        private void Transform()
        {
            _canvas.selectedEffect = Effects.transform;
        }

        private void Rotate()
        {
            _canvas.selectedEffect = Effects.rotate;
        }

        private void Scale()
        {
            _canvas.selectedEffect = Effects.scale;
        }

        private void Done()
        {
            _canvas.selectedEffect = null;
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

        public void NotifyDrawnElementsChanged()
        {
            SelectedIndex = _canvas.DrawnElementsList().Count - 1;
            this.RaisePropertyChanged(nameof(DrawnElements));
            this.RaisePropertyChanged(nameof(SelectedIndex));
        }
    }
}