using Assets.Flow.WardDrawing;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame.WardDrawing
{
    public class WardDrawingSceneScript : MonoBehaviour
    {
        public WardDrawInputScript wardDrawInputScript;

        bool drawMode = false;
        public Camera camera;

        public float Distance = 7;

        public FirstPersonController firstPersonController;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                drawMode = !drawMode;

                wardDrawInputScript.gameObject.SetActive(drawMode);
                firstPersonController.enabled = !drawMode;
                if (drawMode)
                {
                    Cursor.lockState = CursorLockMode.None; // Doesnt work in editor: CursorLockMode.Confined;
                    Cursor.visible = true;
                    wardDrawInputScript.SetPlane(camera.transform.position + camera.transform.forward * Distance,
                        -camera.transform.forward);

                }
            }
        }
    }
}