using System;
using UnityEngine;

namespace Assets.MineMinecraft.Scripts.Blocks
{
    public abstract class UnityBlock: MonoBehaviour, IBlock, IBlockTypeDefinition
    {
        public Vector3 Position { get; set; }
        public abstract Transform GetModel();

        public event Action ModelChanged;
        public virtual void OnCreate()
        {
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void OnInteract(Vector3 localMousePoint, IPlayer player)
        {
        }

        public abstract string TypeName { get; }

        public IBlock CreateBlockInstance()
        {
            return Instantiate(this);
        }
    }
}