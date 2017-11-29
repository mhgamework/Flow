using DirectX11;

namespace Assets.MarchingCubes.Rendering.AsyncCPURenderer
{
    public struct ChunkCoord
    {
        public Point3 LowerLeft;
        public int Depth;

        public ChunkCoord(Point3 lowerLeft, int depth)
        {
            LowerLeft = lowerLeft;
            Depth = depth;
        }

        public bool Equals(ChunkCoord other)
        {
            return LowerLeft.Equals(other.LowerLeft) && Depth == other.Depth;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ChunkCoord && Equals((ChunkCoord) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (LowerLeft.GetHashCode() * 397) ^ Depth;
            }
        }

        public override string ToString()
        {
            return string.Format("LowerLeft: {0}, Depth: {1}", LowerLeft, Depth);
        }
    }
}