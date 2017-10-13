using System;
using UnityEngine;

namespace Assets.MineMinecraft.Blocks
{
    public class SimpleBlock :IBlock
    {
        private readonly Transform modelPrefab;

        public SimpleBlock(Transform modelPrefab)
        {
            this.modelPrefab = modelPrefab;
        }

        public Vector3 Position { get; set; }
        public Transform GetModel()
        {
            return modelPrefab;
        }

        public event Action ModelChanged;
        public void OnCreate()
        {
        }

        public void OnDestroy()
        {
        }

        public virtual void OnInteract(Vector3 localMousePoint, IPlayer player)
        {
        }
    }
}