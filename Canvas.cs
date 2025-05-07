using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;


namespace vector_editor {
    public enum Effects {
        transform,
        rotate,
        scale,
        none
    }

    public class Canvas {
        public List<Element> drawnElements = [];
        private int lastId = 1;
        private Element? _activeElement;
        private int _activeElementIndex = 0;
        public int activeElement {
            set {
                _activeElement = drawnElements[value];
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

        public Effects selectedEffect = Effects.none;

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
            if (selectedEffect != Effects.none) {
                return;
            }
            _activeElement?.AddNewPoint(x + 35, y + 30);
        }

        public void OnMouseMove(Point now, Point prev) {
            if (selectedEffect == Effects.none) {
                _activeElement?.MoveLastPoint((int) now.X + 35, (int) now.Y + 30);
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
                    effect = null;
                    break;
            }

            if (effect != null && _activeElement != null) {
                effect.transform(from, to, _activeElement);
            }
        }

        public void ClearCanvas() {
            drawnElements.Clear();
            lastId = 1;
            _activeElement = null;
            _activeElementIndex = 0;
            CreateNewLine(); 
        }

        public void SaveDrawnElements(string filePath) {
            try {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fs)) {
                    writer.Write(drawnElements.Count);
                    foreach (var element in drawnElements) {
                        writer.Write((int)element.ElementType);
                        element.Save(writer);
                    }
                }
                Debug.Print($"Elements saved to {filePath}");
            } catch (Exception ex) {
                Debug.Print($"Error saving elements: {ex.Message}");
            }
        }

        public void LoadDrawnElements(string filePath) {
            if (!File.Exists(filePath)) {
                Debug.Print($"File not found: {filePath}");
                return;
            }

            try {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs)) {
                    int count = reader.ReadInt32();
                    drawnElements.Clear();
                    for (int i = 0; i < count; i++) {
                        Elements elementType = (Elements)reader.ReadInt32(); // Read element type
                        Element? element = null;

                        // Create instance based on element type
                        switch (elementType) {
                            case Elements.points:
                                element = new Points(Colors.Black, 0); // Placeholder color and id
                                break;
                            case Elements.polygonal:
                                element = new Polygonial(Colors.Black, 0); // Placeholder color and id
                                break;
                            case Elements.bezier:
                                element = new Bezier(Colors.Black, 0); // Placeholder color and id
                                break;
                            case Elements.polygon:
                                element = new Polygon(Colors.Black, Colors.Black, 0); // Placeholder colors and id
                                break;
                            case Elements.ellipse:
                                element = new Ellipse(Colors.Black, Colors.Black, 0); // Placeholder colors and id
                                break;
                            default:
                                Debug.Print($"Unknown element type: {elementType}");
                                continue; // Skip unknown element
                        }

                        if (element != null) {
                            element.Load(reader); // Call virtual Load method
                            drawnElements.Add(element);
                        }
                    }
                    Debug.Print($"Elements loaded from {filePath}");
                    // After loading, update lastId to be greater than any loaded element's id
                    lastId = drawnElements.Count > 0 ? drawnElements.Max(e => e.id) + 1 : 1;
                }
            } catch (Exception ex) {
                Debug.Print($"Error loading elements: {ex.Message}");
            }
        }
    public void ExportToSvg(string filePath) {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<svg width=\"{width}\" height=\"{height}\" xmlns=\"http://www.w3.org/2000/svg\">");
        sb.AppendLine($"  <rect width=\"100%\" height=\"100%\" fill=\"white\"/>");

        foreach (var element in drawnElements) {
            sb.AppendLine($"  {element.ToSvg()}");
        }

        sb.AppendLine("</svg>");

        try {
            File.WriteAllText(filePath, sb.ToString());
            Debug.Print($"SVG exported to {filePath}");
        } catch (Exception ex) {
            Debug.Print($"Error exporting SVG: {ex.Message}");
        }
    }

    }
}
