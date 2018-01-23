using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    public class DayNightCycleScript : MonoBehaviour
    {
        public Vector3 RotationAxis;
        public float DayDuration;
        public void Start()
        {
            
        }
        public void Update()
        {
            transform.Rotate(RotationAxis, 360 / DayDuration * Time.deltaTime);
        }
    }
}