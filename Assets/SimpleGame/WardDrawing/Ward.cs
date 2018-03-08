using System.Collections.Generic;
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
    }
}