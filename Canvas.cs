using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;


namespace vector_editor {
    public enum Effects {
        transform,
        rotate,
        scale
    }

    public class Canvas {
        public List<Element> drawnElements = [];
        private int lastId = 1;
        private Element? _activeElement;
        private int _activeElementIndex = 0;
        public int activeElement {
            set {
                _activeElement = drawnElements[drawnElements.Count - 1];
                _activeElementIndex = value;
            }
            get {
                return _activeElementIndex;
            }
        }
        private Elements _selectedElement = Elements.points;
        public Elements selectedElement {
            get { return _selectedElement; }
            set {
                Debug.Print(value.ToString());
                if (selectedElement == value) {
                    return;
                }
                _selectedElement = value;
                CreateNewLine();
            }
        }
        private Color _color = Colors.Black;
        public Color color {
            get { return _color; }
            set {
                _color = value;
                if (_activeElement != null) {
                    _activeElement.color = value;
                }
            }
        }

        private Color _fillColor = Colors.Black;
        public Color fillColor {
            get { return _fillColor; }
            set {
                _fillColor = value;
                if (_activeElement != null) {
                    _activeElement.fillColor = value;
                }
            }
        }

        public Effects? selectedEffect;

        public int width = 1;
        public int height = 1;

        public Canvas() {
            CreateNewLine();
        }

        public Bitmap Render() {
            var renderTarget = new RenderTargetBitmap(new PixelSize(width, height));
            using (var ctx = renderTarget.CreateDrawingContext())
            {
                var backgroundBrush = new SolidColorBrush(Colors.White);
                ctx.FillRectangle(backgroundBrush, new Rect(0, 0, width, height));

                foreach (var element in drawnElements) {
                    element.Render(renderTarget, ctx);
                }
            }
            return renderTarget;
        }

        public void OnMouseDown(int x, int y) {
            if (selectedEffect != null) {
                return;
            }
            _activeElement?.AddNewPoint(x, y);
        }

        public void OnMouseMove(Point now, Point prev) {
            if (selectedEffect == null) {
                _activeElement?.MoveLastPoint((int) now.X + 100, (int) now.Y);
            } else {
                ApplyEffect(prev, now);
            }
        }

        public void CreateNewLine() {
            switch (selectedElement) {
                case Elements.points:
                    _activeElement = new Points(_color, lastId++);
                    break;
                case Elements.polygonal:
                    _activeElement = new Polygonial(_color, lastId++);
                    break;
                case Elements.bezier:
                    _activeElement = new Bezier(_color, lastId++);
                    break;
                case Elements.polygon:
                    _activeElement = new Polygon(_color, _fillColor, lastId++);
                    break;
                case Elements.ellipse:
                _activeElement = new Ellipse(_color, _fillColor, lastId++);
                    break;
                default:
                    break;
            }
            if (_activeElement == null) {
                throw new Exception();
            }

            drawnElements.Add(_activeElement);
        }

        public List<String> DrawnElementsList() {
            List<String> list = [];
            foreach (var element in drawnElements) {
                list.Add(element.Name);
            }
            return list;
        }

        private void ApplyEffect(Point from, Point to) {
            Effect? effect;
            switch (selectedEffect) {
                case Effects.transform:
                    effect = new Transform();
                    break;
                case Effects.rotate:
                    effect = new Rotate();
                    break;
                case Effects.scale:
                    effect = new Scale();
                    break;
                default:
                throw new Exception();
            }

            if (effect != null && _activeElement != null) {
                effect.transform(from, to, _activeElement);
            }
        }
    }
}
