using MHGameWork.TheWizards.DualContouring.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP.Octrees
{
    public class RenderOctreeNode : IOctreeNode<RenderOctreeNode>
    {
        public RenderOctreeNode[] Children { get; set; }
        public int Depth { get; set; }
        public Point3 LowerLeft { get; set; }
        public int Size { get; set; }
        public VoxelChunkRenderer RenderObject { get; set; }


        public void Destroy()
        {
            DestroyRenderObject();
        }

        public void DestroyRenderObject()
        {
            if (RenderObject != null)
            {
                GameObject.Destroy(RenderObject.gameObject);
            }
            RenderObject = null;
        }

        public void Initialize(RenderOctreeNode parent)
        {
        }
    }
}
