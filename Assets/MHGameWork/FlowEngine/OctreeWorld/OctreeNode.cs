using Assets.MHGameWork.FlowEngine.Models;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;

namespace Assets.MHGameWork.FlowEngine.OctreeWorld
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
