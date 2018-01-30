using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    public class DayNightCycleScript : Singleton<DayNightCycleScript>
    {
        public Vector3 RotationAxis;
        public float TimePerRevolution;

        public float DayStart;
        public float DayEnd;

        public float TimeOfDay;
        public void Start()
        {

        }
        public void Update()
        {
            ChangeTimeRelative(Time.deltaTime / TimePerRevolution);
            transform.rotation = Quaternion.AngleAxis(TimeOfDay * 360, RotationAxis);
        }


        public bool IsDay()
        {
            if (DayEnd < DayStart)
                return TimeOfDay < DayEnd || TimeOfDay > DayStart;
            else
                return TimeOfDay > DayStart && TimeOfDay < DayEnd ;
        }

        public void ChangeTimeRelative(float delta)
        {
            TimeOfDay = (TimeOfDay + delta) % 1;
        }
    }
}