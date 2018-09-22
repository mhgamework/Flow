using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.UnityAdditions
{
    static public class MethodExtensionForMonoBehaviourTransform
    {
        /// <summary>
        /// Gets or add a component. Usage example:
        /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
        /// </summary>
        static public T GetOrAddComponent<T>(this Component child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.gameObject.AddComponent<T>();
            }
            return result;
        }

        public static IEnumerable<T> GetChildren<T>(this Transform target) where T : Component
        {
            for (int i = 0; i < target.childCount; i++)
            {
                var c = target.GetChild(i).GetComponent<T>();
                if (c != null) yield return c;
            }
        }
        //Breadth-first search
        public static T GetComponentInChildren<T>(this Transform aParent, string aName) where T : Component
        {
            var ret = aParent.GetComponentsInChildren<T>().FirstOrDefault(f => f.name == aName);
            if (ret == null)
                throw new Exception("Cannot find child component name " + aName + " and type " + typeof(T).Name);
            return ret;
        }

        //Breadth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static Coroutine StartCoroutine(this MonoBehaviour mono, Func<IEnumerable<YieldInstruction>> coroutineFunc)
        {
            return mono.StartCoroutine(coroutineFunc().GetEnumerator());
        }
    }
}
