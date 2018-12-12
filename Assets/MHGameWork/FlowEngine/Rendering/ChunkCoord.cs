using System.Collections.Generic;
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

        //public bool Equals(ChunkCoord other)
        //{
        //    return LowerLeft.Equals(other.LowerLeft) && Depth == other.Depth;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    return obj is ChunkCoord && Equals((ChunkCoord)obj);
        //}

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        return (LowerLeft.GetHashCode() * 397) ^ Depth;
        //    }
        //}

        public override string ToString()
        {
            return string.Format("LowerLeft: {0}, Depth: {1}", LowerLeft, Depth);
        }

        public class Comparer : IEqualityComparer<ChunkCoord>
        {
            public bool Equals(ChunkCoord x, ChunkCoord y)
            {
                return x.LowerLeft.Equals(y.LowerLeft) && x.Depth == y.Depth;
            }

            public int GetHashCode(ChunkCoord obj)
            {
                unchecked
                {
                    return (obj.LowerLeft.GetHashCode() * 397) ^ obj.Depth;
                }
            }
        }
    }
}