using UnityEngine;

namespace Assets.MineMinecraft
{
    public class WorldRaycastResult
    {
        public bool IsHit { get; set; }
        public IBlock Block { get; set; }
        public Vector3 HitPoint { get; set; }
        public static WorldRaycastResult Empty
        {
            get { return new WorldRaycastResult() { IsHit = false }; }
        }
    }
}