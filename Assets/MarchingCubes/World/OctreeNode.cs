using MHGameWork.TheWizards.DualContouring.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;

namespace Assets.MarchingCubes.VoxelWorldMVP.Octrees
{
    public class OctreeNode : IOctreeNode<OctreeNode>
    {
        public OctreeNode[] Children { get; set; }
        public int Depth { get; set; }
        public Point3 LowerLeft { get; set; }
        public int Size { get; set; }

        public UniformVoxelData VoxelData { get; set; }

        public bool Modified { get; set; }

        public void Destroy()
        {
        }

        public void Initialize(OctreeNode parent)
        {
        }
    }
}
