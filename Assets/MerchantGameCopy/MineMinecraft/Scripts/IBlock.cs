using System;
using UnityEngine;

namespace Assets.MineMinecraft
{
    public interface IBlock
    {
        /// <summary>
        /// Setter only used by world
        /// </summary>
        Vector3 Position { get; set; }
        Transform GetModel();
        event Action ModelChanged;
        /// <summary>
        /// Called when block enters the world
        /// </summary>
        void OnCreate();
        /// <summary>
        /// Called when block leaves the world
        /// </summary>
        void OnDestroy();
        void OnInteract(Vector3 localMousePoint, IPlayer player);
    }
}