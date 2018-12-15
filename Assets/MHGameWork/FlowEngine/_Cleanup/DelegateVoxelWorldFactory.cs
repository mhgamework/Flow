using System;
using Assets.MHGameWork.FlowEngine.OctreeWorld;

namespace Assets.MHGameWork.FlowEngine._Cleanup
{
    public class DelegateVoxelWorldFactory : IVoxelWorldFactory
    {
        private readonly Func<OctreeVoxelWorld> worldFactory;

        public DelegateVoxelWorldFactory(Func<OctreeVoxelWorld> worldFactory)
        {
            this.worldFactory = worldFactory;
        }
        public OctreeVoxelWorld CreateNewWorld()
        {
            return worldFactory();
        }
    }
}