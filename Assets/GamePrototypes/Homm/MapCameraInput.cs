using UnityEngine;

namespace Assets.Homm
{
    /// <summary>
    /// TODO: transform mouse delta to world coords so movement feels natural
    /// </summary>
    public class MapCameraInput : MonoBehaviour
    {

        public float ScrollSpeed = 1;
        /// <summary>
        /// For when user leaves screen
        /// </summary>
        public float MaxDeltaThreshold = 10;

        public void Setup()
        {
            lastMousePos = Input.mousePosition;
        }

        private Vector3 lastMousePos;
        public void Update()
        {
            if (Input.GetMouseButton(2))
            {
                var delta = (Input.mousePosition - lastMousePos) * ScrollSpeed;
                if (delta.magnitude < MaxDeltaThreshold)
                {
                transform.position += new Vector3(-delta.x, 0, -delta.y);

                }
            }
            lastMousePos = Input.mousePosition;
        }
    }
}