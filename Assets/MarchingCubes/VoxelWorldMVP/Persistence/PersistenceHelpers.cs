using UnityEngine;

namespace Assets.MarchingCubes.Persistence
{
    public static class PersistenceHelpers
    {
        public static Vector3 ToVector3(this float[] data)
        {
            return new Vector3(data[0], data[1], data[2]);
        }
        public static float[] ToArray(this Vector3 data)
        {
            return new[] { data[0], data[1], data[2] };
        }
    }
}