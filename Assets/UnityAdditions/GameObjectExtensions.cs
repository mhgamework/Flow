using System.Linq;
using UnityEngine;

namespace Assets
{
    public static class GameObjectExtensions
    {
        public static T GetOrCreateComponent<T>(this Component b) where T : Component
        {
            var ret = b.GetComponent<T>();
            if (ret == null)
                ret = b.gameObject.AddComponent<T>();

            return ret;
        }


        public static Bounds GetTotalMeshRendererBounds(this GameObject b)
        {
            var bounds = b.GetComponentsInChildren<MeshRenderer>()
                 .Select(r => r.bounds)
                 .Aggregate((el, acc) =>
                 {
                     acc.Encapsulate(el);
                     return acc;
                 });
            return bounds;
        }
    }


}