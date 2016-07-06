using System;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.VoxelEngine
{
    public class ArrayHermiteGrid : AbstractHermiteGrid
    {
        private readonly Array3D<VoxelMaterial> gridPoints;

        public ArrayHermiteGrid(Array3D<VoxelMaterial> gridPoints)
        {
            this.gridPoints = gridPoints;
        }

        public override bool GetSign(Point3 pos)
        {
            return gridPoints.Get(pos) != null;
        }

        public override Point3 Dimensions
        {
            get { return gridPoints.Size - new Point3(1,1,1); }
        }

        public override Vector4 getEdgeData(Point3 cube, int edgeId)
        {
            return new Vector4(1, 0, 0, 0.5f); // always in middle
        }

        public override DCVoxelMaterial GetMaterial(Point3 cube)
        {
            return new DCVoxelMaterial();
        }
    }
}