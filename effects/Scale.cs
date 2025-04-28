using System;
using Avalonia;

public class Scale : Effect
{
    public override void transform(Point from, Point to, Element element)
    {
        var pivot = element.Center();
        double distanceFrom = Math.Sqrt(Math.Pow(from.X - pivot.X, 2) + Math.Pow(from.Y - pivot.Y, 2));
        double distanceTo = Math.Sqrt(Math.Pow(to.X - pivot.X, 2) + Math.Pow(to.Y - pivot.Y, 2));

        if (distanceFrom == 0) return;

        double scale = distanceTo / distanceFrom;


        for (int i = 0; i < element.points.Count; ++i)
        {
            Point p = element.points[i];

            double x = p.X - pivot.X;
            double y = p.Y - pivot.Y;

            double scaledX = x * scale;
            double scaledY = y * scale;

            element.points[i] = new Point(scaledX + pivot.X, scaledY + pivot.Y);
        }
    }
}
