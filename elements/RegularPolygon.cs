using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using vector_editor;

public class RegularPolygon : Polygon
{
    private int vertexCount;
    private Point center;
    private double radius;

    public override string _name
    {
        get
        {
            return $"{this.vertexCount} vertices";
        }
    }

    public override Elements ElementType => Elements.polygon;

    public RegularPolygon(Color color, Color fillColor, int id, int vertexCount) : base(color, fillColor, id)
    {
        if (vertexCount < 3)
            throw new ArgumentException("A polygon must have at least 3 vertices.", nameof(vertexCount));

        this.vertexCount = vertexCount;
        points = new System.Collections.Generic.List<Point>(vertexCount);
        center = new Point(0, 0);
        radius = 0;
    }

    public override void AddNewPoint(int x, int y)
    {
        if (points.Count == 0)
        {
            center = new Point(x, y);
            points.Clear();
            for (int i = 0; i < vertexCount; i++)
            {
                points.Add(center);
            }
        }
        else if (points.Count == vertexCount)
        {
            Point newPoint = new Point(x, y);
            radius = Distance(center, newPoint);
            CalculateVertices();
        }
    }

    public override void MoveLastPoint(int x, int y)
    {
        if (points.Count == 0)
            return;

        Point newPoint = new Point(x, y);
        radius = Distance(center, newPoint);
        CalculateVertices();
    }

    private void CalculateVertices()
    {
        points.Clear();
        double angleStep = 2 * Math.PI / vertexCount;
        double startAngle = -Math.PI / 2; 

        for (int i = 0; i < vertexCount; i++)
        {
            double angle = startAngle + i * angleStep;
            double px = center.X + radius * Math.Cos(angle);
            double py = center.Y + radius * Math.Sin(angle);
            points.Add(new Point(px, py));
        }
    }

    private double Distance(Point p1, Point p2)
    {
        double dx = p2.X - p1.X;
        double dy = p2.Y - p1.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}