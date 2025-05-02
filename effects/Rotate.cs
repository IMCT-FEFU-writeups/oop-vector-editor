using System;
using System.Diagnostics;
using Avalonia;

public class Rotate : Effect
{
    public override void transform(Point from, Point to, Element element)
    {
        var pivot = element.Center();
        double angle = Math.Atan2(to.Y - pivot.Y, to.X - pivot.X) - Math.Atan2(from.Y - pivot.Y, from.X - pivot.X);

        for (int i = 0; i < element.points.Count; ++i)
        {
            Point p = element.points[i];

            double x = p.X - pivot.X;
            double y = p.Y - pivot.Y;

            double rotatedX = x * Math.Cos(angle) - y * Math.Sin(angle);
            double rotatedY = x * Math.Sin(angle) + y * Math.Cos(angle);

            element.points[i] = new Point(rotatedX + pivot.X, rotatedY + pivot.Y);
        }
    }
}
