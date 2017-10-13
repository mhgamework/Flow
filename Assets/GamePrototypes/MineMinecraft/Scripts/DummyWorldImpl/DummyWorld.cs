using System;
using System.Collections.Generic;
using System.Linq;
using Assets.MineMinecraft;
using Assets.MineMinecraft.MathUtils;
using UnityEngine;
using Object = UnityEngine.Object;

public class DummyWorld : IWorld
{
    private readonly Transform dynamicObjectsContainer;
    private Dictionary<Vector3, BlockData> blocks = new Dictionary<Vector3, BlockData>();

    public static Vector3[] CubeSides = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward, -Vector3.right, -Vector3.up, -Vector3.forward };


    public DummyWorld(Transform dynamicObjectsContainer)
    {
        this.dynamicObjectsContainer = dynamicObjectsContainer;
    }

    public Transform getBlockTransform(IBlock block)
    {
        return GetBlockData(block).Model;
    }

    private BlockData GetBlockData(IBlock block)
    {
        var bd = blocks[block.Position];
        if (bd == null || bd.Block != block) throw new InvalidOperationException();
        return bd;
    }
    private BlockData GetBlockData(Vector3 v)
    {
        if (!blocks.ContainsKey(v)) return null;
        var bd = blocks[v];
        return bd;
    }

    public IBlock GetBlockAt(Vector3 v)
    {
        if (!blocks.ContainsKey(v)) return null;
        return blocks[v.Round()].Block;
    }

    public void SetBlockAt(Vector3 v, IBlock block)
    {
        //Console.WriteLine("WORLD: Set block at " + v + " - " + block);
        v = v.Round();
        var oldbd = GetBlockData(v);
        if (oldbd != null)
        {
            oldbd.Block.OnDestroy();
            oldbd.DestroyModel();

        }
        blocks.Remove(v);
        if (block != null)
        {
            var bd = new BlockData();
            blocks[v] = bd;
            bd.Block = block;
            block.Position = v;
            block.OnCreate();


        }
        if (!updatingPaused)
            // Update the block, and its neighbours (incase they are now hidden or visible)
            updateBlockModelAndNeighbours(v);
    }

    private bool updatingPaused = false;
    public void DoWithoutModelUpdate(Action act)
    {
        if (updatingPaused) throw new InvalidOperationException("Already in DoWithoutModelUpdate");
        updatingPaused = true;
        act();
        updatingPaused = false;
    }

    public void UpdateAllModels()
    {
        foreach (var val in blocks.Values)
        {
            UpdateBlockModel(val.Block.Position);
        }
    }

    private void updateBlockModelAndNeighbours(Vector3 v)
    {
        UpdateBlockModel(v);
        for (int i = 0; i < CubeSides.Length; i++)
        {
            UpdateBlockModel(v + CubeSides[i]);
        }
    }

    public void InvalidateBlockModel(IBlock block)
    {
        var bd = GetBlockData(block);

        if (bd.Model != null)
            bd.DestroyModel();

        UpdateBlockModel(bd.Block.Position);

    }

    public WorldRaycastResult Raycast(Ray ray)
    {
        //Use physics raycasting for now.

        var hits = Physics.RaycastAll(ray, 8).OrderBy(h => h.distance);
        foreach (var hit in hits)
        {
            if (hit.collider.GetComponent<BlockInteractScript>() == null) continue; // Hacky , should be a better purposed script
            var ret = new WorldRaycastResult();
            ret.IsHit = true;
            ret.HitPoint = hit.point;
            ret.Block = GetBlockAt(hit.collider.transform.position);

            if (ret.Block != null) return ret;
        }


        return WorldRaycastResult.Empty;

    }


    private void UpdateBlockModel(Vector3 pos)
    {
        BlockData bd;
        if (!blocks.TryGetValue(pos, out bd)) return;
        if (isHidden(pos)) bd.DestroyModel();
        else
        {
            // Assume unchanged, if the model needs refresh, the invalidatelbockmodel should've taken care of the removal
            if (bd.Model != null)
                return;

            var block = bd.Block;

            bd.Model = Object.Instantiate(block.GetModel());
            bd.Model.gameObject.isStatic = true; // Pretty sure this doesnt work at runtime, but what the heck
            bd.Model.SetParent(dynamicObjectsContainer);
            bd.Model.position = block.Position;
        }
    }

    private bool isHidden(Vector3 pos)
    {
        for (int i = 0; i < CubeSides.Length; i++)
        {
            if (!blocks.ContainsKey(pos + CubeSides[i]))
                return false;
        }
        return true;
    }

    private class BlockData
    {
        public IBlock Block;
        public Transform Model;


        public void DestroyModel()
        {
            if (Model == null) return;
            GameObject.Destroy(Model.gameObject);
            Model = null;
        }

    }

}