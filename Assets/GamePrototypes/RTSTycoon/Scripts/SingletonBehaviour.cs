using UnityEngine;

namespace Assets
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        public static T Instance()
        {
            if (instance != null) return instance;
            var ret = FindObjectOfType<T>();
            if (!ret) Debug.LogError("No instance of RenderToTextureScript found in scene!");
            instance = ret;
            return ret;
        }
    }
}