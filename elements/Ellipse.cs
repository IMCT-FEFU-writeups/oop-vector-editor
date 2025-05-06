using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using vector_editor; // Added to access Elements enum

public class Ellipse : Element
{
    public override string _name { get; } = "Ellipse";

    // Implement ElementType property using the enum
    public override Elements ElementType => Elements.ellipse;

    public Ellipse(Color color, Color fillColor, int id) : base(color, id)
    {
        this.fillColor = fillColor;
    }

    public override void AddNewPoint(int x, int y)
    {
        points.Add(new Point(x, y));
    }

    public override void MoveLastPoint(int x, int y)
    {
        if (points.Count == 0)
            return;

        points[^1] = new Point(x, y);
    }

    public override void Render(Bitmap renderTarget, DrawingContext ctx)
    {
        if (points.Count < 2)
            return;

        var pen = new Pen(new SolidColorBrush(color), 2);
        var fillPen = new Pen(new SolidColorBrush(fillColor), 2);
        Point p1 = points[0];
        Point p2 = points[^1];

        double centerX = (p1.X + p2.X) / 2;
        double centerY = (p1.Y + p2.Y) / 2;
        double radiusX = Math.Abs(p2.X - p1.X) / 2;
        double radiusY = Math.Abs(p2.Y - p1.Y) / 2;

        FillEllipse(renderTarget, ctx, new Point(centerX, centerY), radiusX, radiusY, fillPen);
        DrawEllipseBorder(renderTarget, ctx, new Point(centerX, centerY), radiusX, radiusY, pen);
    }

    private void FillEllipse(Bitmap renderTarget, DrawingContext ctx, Point center, double radiusX, double radiusY, Pen pen)
    {
        int minX = (int)(center.X - radiusX);
        int maxX = (int)(center.X + radiusX);
        int minY = (int)(center.Y - radiusY);
        int maxY = (int)(center.Y + radiusY);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                double dx = (x - center.X) / radiusX;
                double dy = (y - center.Y) / radiusY;
                if (dx * dx + dy * dy <= 1.0)
                {
                    DrawPoint(renderTarget, ctx, new Point(x, y), pen);
                }
            }
        }
    }

    private void DrawEllipseBorder(Bitmap renderTarget, DrawingContext ctx, Point center, double radiusX, double radiusY, Pen pen)
    {
        int steps = 360; // More steps = smoother ellipse
        double angleStep = 2 * Math.PI / steps;

        Point? previous = null;

        for (int i = 0; i <= steps; i++)
        {
            double angle = i * angleStep;
            double x = center.X + radiusX * Math.Cos(angle);
            double y = center.Y + radiusY * Math.Sin(angle);

            Point current = new Point(Math.Round(x), Math.Round(y));

            if (previous != null)
            {
                DrawLine(renderTarget, ctx, previous.Value, current, pen);
            }

            previous = current;
        }
    }

    private void DrawLine(Bitmap renderTarget, DrawingContext ctx, Point p1, Point p2, Pen pen)
    {
        double x1 = p1.X;
        double y1 = p1.Y;
        double x2 = p2.X;
        double y2 = p2.Y;

        double dx = x2 - x1;
        double dy = y2 - y1;

        int steps = (int)Math.Max(Math.Abs(dx), Math.Abs(dy));

        if (steps == 0)
        {
            DrawPoint(renderTarget, ctx, p1, pen);
            return;
        }

        double xInc = dx / steps;
        double yInc = dy / steps;

        double x = x1;
        double y = y1;

        for (int i = 0; i <= steps; i++)
        {
            DrawPoint(renderTarget, ctx, new Point(Math.Round(x), Math.Round(y)), pen);
            x += xInc;
            y += yInc;
        }
    }
public override string ToSvg()
    {
        if (points.Count < 2)
        {
            return string.Empty;
        }

        Point p1 = points[0];
        Point p2 = points[^1];

        double cx = (p1.X + p2.X) / 2;
        double cy = (p1.Y + p2.Y) / 2;
        double rx = Math.Abs(p2.X - p1.X) / 2;
        double ry = Math.Abs(p2.Y - p1.Y) / 2;

        if (rx <= 0 || ry <= 0) 
        {
            return string.Empty;
        }

        string strokeColorHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        string fillColorHex = "none";
        if (fillColor.A != 0)
        {
            fillColorHex = $"#{fillColor.R:X2}{fillColor.G:X2}{fillColor.B:X2}";
        }

        int thickness = 2;

        return $"<ellipse cx=\"{cx}\" cy=\"{cy}\" rx=\"{rx}\" ry=\"{ry}\" stroke=\"{strokeColorHex}\" stroke-width=\"{thickness}\" fill=\"{fillColorHex}\" />";
    }
}
