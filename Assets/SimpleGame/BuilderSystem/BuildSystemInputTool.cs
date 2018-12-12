using System.Collections.Generic;
using Assets.SimpleGame.Chutes;
using Assets.SimpleGame.PowerBeams;
using UnityEngine;

namespace Assets.SimpleGame.BuilderSystem
{
    public class BuildSystemInputTool : MonoBehaviour
    {
        [SerializeField] private BaseBuilderInteractable boxPrefab;
        [SerializeField] private BaseBuilderInteractable mirrorPrefab;
        [SerializeField] private BaseBuilderInteractable chuteNodePrefab;
        [SerializeField] private BaseBuilderInteractable elevatorCellPrefab;
        [SerializeField] private BaseBuilderInteractable elevatorBasePrefab;


        [SerializeField] private ChuteBuilderInputScript chuteBuilderInput;

        private Transform transformForNewObjects;
        [SerializeField] int maxRaycastDistance = 20;

        private BaseBuilderInteractable prefab = null;

        private string activeBuilder = null;

        private Dictionary<string, BaseBuilderInteractable> builders;
        void Start()
        {
        }


        public void Init()
        {
            transformForNewObjects = SimpleGameSystemScript.Instance.Get<DynamicObjectsContainerScript>().transform;
            builders = new Dictionary<string, BaseBuilderInteractable>();
            builders.Add("mirrorBuilding", mirrorPrefab);
            builders.Add("boxBuilding", boxPrefab);
            builders.Add("chuteNode", chuteNodePrefab);
            builders.Add("elevatorCell", elevatorCellPrefab);
            builders.Add("elevatorBase", elevatorBasePrefab);
            builders.Add("wrench", null);
        }

        public bool tryTempHackySetToolActive(string resourceType)
        {
            if (!builders.ContainsKey(resourceType)) return false;
            prefab = builders[resourceType];
            activeBuilder = resourceType;
            return true;
        }


        public void OnClick(Vector3 position, Vector3 normal, Vector3 lookDir, MouseButton button, IBuilderInteractable interactable)
        {
            if (isWrench())
            {
                return;
            }


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

      
        private bool isWrench()
        {
            return (activeBuilder == "wrench");
        }

        public void OnHover(Vector3 position, Vector3 normal, Vector3 lookDir, IBuilderInteractable interactable)
        {
        }


        public void DoUpdate()
        {
            var result = raycast();
            if (result == null) return;

            if (isWrench())
            {
                chuteBuilderInput.DoUpdate(result.position, result.normal, result.lookDir, result.MouseButton, result.interactable);
            }
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


            if (isWrench() && result.interactable != null)
            {
                var mirrorDisc = (result.interactable as MonoBehaviour).GetComponentInChildren<MirrorDiscScript>();
                var chutePoint = (result.interactable as MonoBehaviour).GetComponentInChildren<ChuteTransportPointScript>();
                if (mirrorDisc != null)
                {
                    if (Input.GetMouseButton(0))
                    {
                        mirrorDisc.PushRotate(result.position, result.normal, 1);
                    }

                    if (Input.GetMouseButton(1))
                    {
                        mirrorDisc.PushRotate(result.position, result.normal, -1);
                    }
                }

                if (chutePoint != null)
                {

                    chuteBuilderInput.DoBuildUpdate(chutePoint);

                }

            }

            if (!isWrench())
            {
                chuteBuilderInput.StopBuilding();
            }

        }

        private BuildRaycastInfo raycast()
        {
            RaycastHit hitInfo;
            if (!Physics.Raycast(Camera.main.transform.position + Camera.main.transform.forward * 0.5f, Camera.main.transform.forward, out hitInfo, maxRaycastDistance, int.MaxValue, QueryTriggerInteraction.Ignore))
                return null;

            Debug.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal, Color.green);

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