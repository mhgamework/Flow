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
        private readonly Point3 offset;
        private readonly Point3 size;

        public ArrayHermiteGrid(Array3D<VoxelMaterial> gridPoints, Point3 offset, Point3 size)
        {
            this.gridPoints = gridPoints;
            this.offset = offset;
            this.size = size;
        }

        public override bool GetSign(Point3 pos)
        {
            return gridPoints.Get(pos+offset) != null;
        }

        public override Point3 Dimensions
        {
            get
            {
                return size; //gridPoints.Size - new Point3(1,1,1);
            }
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