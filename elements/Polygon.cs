using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using vector_editor; // Added to access Elements enum

public class Polygon : Polygonial
{
    public override string _name { get; } = "Polygon";

    // Implement ElementType property using the enum
    public override Elements ElementType => Elements.polygon;

    public Polygon(Color color, Color _fillColor, int id) : base(color, id)
    {
        fillColor = _fillColor;
    }

    public override void Render(Bitmap renderTarget, DrawingContext ctx)
    {
        var pen = new Pen(new SolidColorBrush(fillColor), 1);
        int n = points.Count;
        if (n < 3)
            return;

        double minY = double.MaxValue;
        double maxY = double.MinValue;
        foreach (var pt in points)
        {
            if (pt.Y < minY) minY = pt.Y;
            if (pt.Y > maxY) maxY = pt.Y;
        }

        int yStart = (int)Math.Ceiling(minY);
        int yEnd = (int)Math.Floor(maxY);

        for (int y = yStart; y <= yEnd; y++)
        {
            var intersections = new List<double>();

            for (int i = 0; i < n; i++)
            {
                Point p1 = points[i];
                Point p2 = points[(i + 1) % n];

                if ((p1.Y <= y && p2.Y > y) || (p2.Y <= y && p1.Y > y))
                {
                    double x = p1.X + (y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y);
                    intersections.Add(x);
                }
            }

            if (intersections.Count < 2)
                continue;

            intersections.Sort();

            for (int i = 0; i < intersections.Count; i += 2)
            {
                int xStart = (int)Math.Ceiling(intersections[i]);
                int xEnd = (int)Math.Floor(intersections[i + 1]);

                for (int x = xStart; x <= xEnd; x++)
                {
                    DrawPoint(renderTarget, ctx, new Point(x, y), pen);
                }
            }
        }
        base.Render(renderTarget, ctx, true);
    }
}
