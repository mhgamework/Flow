using System.Collections.Generic;
using Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen;
using DirectX11;

namespace Assets.MHGameWork.FlowEngine.SdfWorldGeneration
{
    /// <summary>
    /// Represents a world that is represented by a collection of IVoxelObject's
    /// </summary>
    public interface ISdfObjectWorld
    {
        void GetObjectsInRange(Point3 min, Point3 max, List<IVoxelObject> outList);

    }
}