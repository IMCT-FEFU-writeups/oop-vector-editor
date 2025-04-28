using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

public class Bezier: Element {
    public override string _name { get; } = "Bezier";

    public Bezier(Color color, int id) : base(color, id)
    {
    }

    private Point Lerp(Point a, Point b, double t)
    {
        return new Point(Convert.ToInt32(a.X * (1 - t) + b.X * t), Convert.ToInt32(a.Y * (1 - t) + b.Y * t));
    }

    override public void Render(Bitmap renderTarget, DrawingContext ctx)  {
        var pen = new Pen(new SolidColorBrush(color), 2);

        for (int i = 1; i < points.Count - 3; i += 3)
        {
            for (double t = 0; t <= 1; t += 0.001)
            {
                Point a = Lerp(points[i], points[i + 1], t);
                Point b = Lerp(points[i + 1], points[i + 2], t);
                Point c = Lerp(points[i + 2], points[i + 3], t);
                Point ab = Lerp(a, b, t);
                Point bc = Lerp(b, c, t);
                Point abc = Lerp(ab, bc, t);

                DrawPoint(renderTarget, ctx, abc, pen);
            }
        }
    }

    public override void AddNewPoint(int x, int y)
    {
        points.Add(new Point(x, y));
        points.Add(new Point(x, y));
        points.Add(new Point(x, y));
    }

    public override void MoveLastPoint(int x, int y)
    {
        Point p = points[^2];
        points[^1] = new Point(x, y);
        points[^3] = new Point(p.X - (x - p.X), p.Y - (y - p.Y));
    }
}