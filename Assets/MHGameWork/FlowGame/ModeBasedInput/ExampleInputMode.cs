using UnityEngine;

namespace Assets.MHGameWork.FlowGame.ModeBasedInput
{
    public class ExampleInputMode : IInputMode
    {
        public void Start()
        {
            Debug.Log("Start example input mode");
        }

        public void Stop()
        {
            Debug.Log("Stop example input mode");
        }
    }
}