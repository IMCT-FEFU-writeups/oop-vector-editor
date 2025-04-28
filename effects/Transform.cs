using System.Diagnostics;
using Avalonia;

public class Transform : Effect
{
    public override void transform(Point from, Point to, Element element)
    {
        double dx = from.X - to.X;
        double dy = from.Y - to.Y;
        Debug.Print("d {0}, {1} {2}", dx, dy, element.points.Count);
        for (int i = 0; i < element.points.Count; ++i) {
            element.points[i] = new Point(element.points[i].X - dx, element.points[i].Y - dy);
        }
    }
}