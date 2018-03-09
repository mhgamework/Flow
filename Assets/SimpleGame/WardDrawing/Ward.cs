using System.Collections.Generic;
using System.Linq;
using DirectX11;

namespace Assets.SimpleGame.WardDrawing
{
    public class Ward
    {
        public List<List<Point3>> Shape { get; private set; }

        public Ward(List<List<Point3>> shape)
        {
            Shape = shape;
        }

        public static Ward Create(params Point3[] line)
        {
            return Create(new[] { line });
        }

        public static Ward Create(params Point3[][] lines)
        {
            return new Ward(lines.Select(CreateLine).ToList());
        }

        public static List<Point3> CreateLine(params Point3[] points)
        {
            return points.ToList();
        }
    }
}