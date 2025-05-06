using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using vector_editor;

public class Polygonial : Element
{
    public override string _name { get; } = "Polygonial";

    public override Elements ElementType => Elements.polygonal;

    public Polygonial(Color color, int id) : base(color, id)
    {
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
        Render(renderTarget, ctx, false);
    }

    public void Render(Bitmap renderTarget, DrawingContext ctx, bool loop = false)
    {
        var pen = new Pen(new SolidColorBrush(color), 2);

        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLine(renderTarget, ctx, points[i], points[i + 1], pen);
        }
        if (loop && points.Count > 1) {
            DrawLine(renderTarget, ctx, points[0], points[points.Count - 1], pen);
        }
    }

    public void DrawLine(Bitmap renderTarget, DrawingContext ctx, Point p1, Point p2, Pen pen)
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

        System.Text.StringBuilder svgPoints = new System.Text.StringBuilder();
        foreach (var point in points)
        {
            svgPoints.Append($"{point.X},{point.Y} ");
        }

        string colorHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        int thickness = 2; 

        return $"<polyline points=\"{svgPoints.ToString().TrimEnd()}\" stroke=\"{colorHex}\" stroke-width=\"{thickness}\" fill=\"none\" />";
    }
}
