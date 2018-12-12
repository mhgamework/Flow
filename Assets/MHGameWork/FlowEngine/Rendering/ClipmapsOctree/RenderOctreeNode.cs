using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine.Rendering.AsyncCPURenderer;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;

namespace Assets.MHGameWork.FlowEngine.Rendering.ClipmapsOctree
{
    public class RenderOctreeNode : IOctreeNode<RenderOctreeNode>
    {
        public RenderOctreeNode[] Children { get; set; }
        public int Depth { get; set; }
        public Point3 LowerLeft { get; set; }
        public int Size { get; set; }
        public VoxelChunkRendererScript RenderObject { get; set; }
        public OctreeNode DataNode { get; set; }
        public bool ShouldRender { get; set; }
        public int LastRenderFrame { get; set; }

        public ClipmapsOctreeService clipmapsOctreeService;

        public void Destroy()
        {
            clipmapsOctreeService.OnDestroyRenderNode(this);
        }

        public void Initialize(RenderOctreeNode parent)
        {
        }
    }
}
