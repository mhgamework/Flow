using System.Collections.Generic;
using System.Linq;
using DirectX11;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame.WardDrawing
{
    public class WardDrawingSceneScript : MonoBehaviour
    {
        bool drawMode;
        public Camera camera;

        public float Distance = 7;

        public FirstPersonController firstPersonController;

        public List<Vector3> Ward1;
        public List<Vector3> Ward2;

        public GameObject Ward1Prefab;

        public WardDrawingModeScript wardDrawingMode;

        private List<Ward> wards;

        public void Start()
        {
            updateState();
            wards = new[] { Ward1, Ward2 }.Select(WardDrawingUtils.UnflattenShape).Select(k => new Ward(k)).ToList();
            wardDrawingMode.SetWards(wards);

            wardDrawingMode.OnCorrectWard += w =>
            {
                var index = wards.IndexOf(w);
                Debug.Log("Ward! " + index);

                if (index == 0)
                {
                    var inst = Instantiate(Ward1Prefab, camera.transform.position + camera.transform.forward * Distance,
                        camera.transform.rotation, transform);
                    inst.GetComponentInChildren<MeshWardViewScript>().SetShape(wards[0].Shape, inst.transform.localToWorldMatrix);
                    drawMode = false;
                    updateState();
                }

            };
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                drawMode = !drawMode;

                updateState();
            }
        }

        private void updateState()
        {
            wardDrawingMode.SetPlane(camera.transform.position + camera.transform.forward * Distance, -camera.transform.forward);

            wardDrawingMode.SetModeEnabled(drawMode);
            firstPersonController.enabled = !drawMode;

        }
    }
}