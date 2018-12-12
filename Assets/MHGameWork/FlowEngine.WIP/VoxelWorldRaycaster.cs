﻿using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Responsible for raycasting the voxel world
    /// Current implementation relies on phyics raycasting of the renderables
    /// Assumes colliders are created for the voxel chunks
    /// Also interesects non voxels at this point
    /// </summary>
    public class VoxelWorldRaycaster
    {
        public RaycastHit? raycast()
        {

            //var ray = Camera.main.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0));// new Ray(RayPosition, RayDirection.normalized);
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hitInfo;
            if (!Physics.Raycast(ray, out hitInfo, float.MaxValue, int.MaxValue, QueryTriggerInteraction.Ignore)) return null;

            return hitInfo;

        }
    }
}