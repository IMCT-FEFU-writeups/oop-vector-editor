using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using vector_editor; // Added to access Elements enum

public class Points : Element
{
    public override string _name { get; } = "Points";

    public override Elements ElementType => Elements.points;

    public Points(Color color, int id) : base(color, id)
    {
    }

    public override void Render(Bitmap renderTarget, DrawingContext ctx)
    {
        var pen = new Pen(new SolidColorBrush(color), 1);
        int n = points.Count;
        for (int i = 0; i < n; i++) {
            DrawPoint(renderTarget, ctx, points[i], pen);
        }
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
public override string ToSvg()
    {
        System.Text.StringBuilder svgBuilder = new System.Text.StringBuilder();
        string colorHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        double radius = 2;

        foreach (var point in points)
        {
            svgBuilder.Append($"<circle cx=\"{point.X}\" cy=\"{point.Y}\" r=\"{radius}\" fill=\"{colorHex}\" />\n");
        }

        return svgBuilder.ToString();
    }
}
