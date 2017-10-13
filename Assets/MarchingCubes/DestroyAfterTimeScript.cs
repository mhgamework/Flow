using UnityEngine;

namespace Assets.MarchingCubes
{
    public class DestroyAfterTimeScript : MonoBehaviour
    {
        public float DestroyAfter = 5f;

        public void Update()
        {
            DestroyAfter -= Time.deltaTime;
            if (DestroyAfter < 0)
                Destroy(gameObject);
        }
    }
}