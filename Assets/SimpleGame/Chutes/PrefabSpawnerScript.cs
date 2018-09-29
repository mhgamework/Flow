using UnityEngine;

namespace Assets.SimpleGame.Chutes
{

    public class PrefabSpawnerScript : MonoBehaviour
    {
        [SerializeField] private Transform prefab;
        public void Start()
        {
            var obj = Instantiate(prefab, transform);
            obj.SetParent(transform.parent, true);

            gameObject.SetActive(false);

        }
    }
}