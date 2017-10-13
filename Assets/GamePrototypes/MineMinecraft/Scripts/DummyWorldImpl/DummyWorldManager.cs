using System;
using Assets.MineMinecraft.Blocks;
using UnityEngine;

namespace Assets.MineMinecraft.DummyWorldImpl
{
    public class DummyWorldManager : SingletonBehaviour<DummyWorldManager>
    {
        public Transform BlockPrefab;


        private DummyWorld world;


        public IWorld World
        {
            get { return world; }
        }

        public void Start()
        {
            // Set transform to identity, since we use this as simpleblock container and they should map to voxel coordinates without transform
            transform.position = new Vector3();
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            world = new DummyWorld(transform);

            world.DoWithoutModelUpdate(genWorld);
            world.UpdateAllModels();
        }

        private void genWorld()
        {
            int size = 50;
            int ground = 30;
            for (int x = 0; x < size; x++)
            {
                Console.WriteLine("Setup x-plane: " + x);
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        if (x + y + z < ground*3
                            || y < ground)
                            world.SetBlockAt(new Vector3(x, y, z), new SimpleBlock(BlockPrefab));
                    }
                }
            }
        }

        public void Update()
        {

        }
    }
}