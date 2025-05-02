using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using vector_editor;

public abstract class Element {
    public virtual string _name { get; } = "Abstract element";
    public string Name { get {
        return '#' + id.ToString() + ' ' + _name + ' ' + ColorHelper.ToDisplayName(color);
    }}
    public int id;

    public List<Point> points = [];
    public Color color;
    public Color fillColor;

    public Element (Color color, int id) {
        this.color = color;
        this.id = id;
    }

    public abstract Elements ElementType { get; }

    public virtual void Save(BinaryWriter writer) {
        writer.Write(id);
        writer.Write(points.Count);
        foreach (var point in points) {
            writer.Write(point.X);
            writer.Write(point.Y);
        }
        writer.Write(color.ToUint32());
        writer.Write(fillColor.ToUint32());
    }

    public virtual void Load(BinaryReader reader) {
        id = reader.ReadInt32();
        int pointCount = reader.ReadInt32();
        points.Clear();
        for (int i = 0; i < pointCount; i++) {
            double x = reader.ReadDouble();
            double y = reader.ReadDouble();
            points.Add(new Point(x, y));
        }
        color = Color.FromUInt32(reader.ReadUInt32());
        fillColor = Color.FromUInt32(reader.ReadUInt32());
    }


    public void DrawPoint(Bitmap renderTarget, DrawingContext ctx, Point pt, Pen pen)
    {
        if (pt.X >= 0 && pt.X < renderTarget.Size.Width && pt.Y >= 0 && pt.Y < renderTarget.Size.Height)
        {
            ctx.DrawRectangle(null, pen, new Rect(pt.X, pt.Y, 1, 1));
        }
    }


    public abstract void Render(Bitmap renderTarget, DrawingContext ctx);
    public abstract void AddNewPoint(int x, int y);
    public abstract void MoveLastPoint(int x, int y);

    public Point Center() {
        double sumX = 0;
        double sumY = 0;
        foreach (var point in points) {
            sumX += point.X;
            sumY += point.Y;
        }

        return new Point(sumX / points.Count, sumY / points.Count);
    }

}