using UnityEngine;

namespace Assets.SimpleGame.BuilderSystem
{
    public class BuildSystemInputTool : MonoBehaviour
    {
        [SerializeField] private BaseBuilderInteractable prefab;
        private Transform transformForNewObjects;
        [SerializeField] int maxRaycastDistance = 20;

        void Start()
        {
        }

        public void Init()
        {
            transformForNewObjects = SimpleGameSystemScript.Instance.Get<DynamicObjectsContainerScript>().transform;

        }

        public void OnClick(Vector3 position, Vector3 normal, Vector3 lookDir, MouseButton button, IBuilderInteractable interactable)
        {
            var obj = Instantiate(prefab, transformForNewObjects);
            if (button == MouseButton.Left)
            {
                obj.OnBuilt(position, normal, lookDir);
            }

            if (button == MouseButton.Right)
            {
                if (interactable == null) return;
                interactable.OnUnBuilt();
            }
        }

        public void OnHover(Vector3 position, Vector3 normal, Vector3 lookDir, IBuilderInteractable interactable)
        {
        }

        public void DoUpdate()
        {
            var result = raycast();
            if (result == null) return;
            if (Input.GetMouseButtonDown(0))
            {
                result.MouseButton = MouseButton.Left;
                OnClick(result.position, result.normal, result.lookDir, result.MouseButton, result.interactable);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                result.MouseButton = MouseButton.Right;
                OnClick(result.position, result.normal, result.lookDir, result.MouseButton, result.interactable);
            }

        }

        private BuildRaycastInfo raycast()
        {
            RaycastHit hitInfo;
            if (!Physics.Raycast(Camera.main.transform.position+ Camera.main.transform.forward*0.5f, Camera.main.transform.forward, out hitInfo, maxRaycastDistance,int.MaxValue,QueryTriggerInteraction.Ignore))
                return null;

            Debug.DrawLine(hitInfo.point,hitInfo.point + hitInfo.normal,Color.green);

            return new BuildRaycastInfo()
            {
                interactable = hitInfo.collider.GetComponentInParent<IBuilderInteractable>(),
                position = hitInfo.point,
                normal = hitInfo.normal,
                lookDir = Camera.main.transform.forward

            };

        }

        private class BuildRaycastInfo
        {
            public IBuilderInteractable interactable;
            public Vector3 position;
            public Vector3 normal;
            public Vector3 lookDir;
            public MouseButton MouseButton;
        }

        public void OnToolEnable()
        {
        }

        public void OnToolDisable()
        {
        }


        public enum MouseButton
        {
            Left,
            Right
        }
    }
}