namespace Assets.MHGameWork.FlowGame.ModeBasedInput
{
    public class ModeBasedInputSystem
    {
        private IInputMode activeMode;

        public IInputMode ActiveMode
        {
            get { return activeMode; }
        }


        public void SwitchActiveMode(IInputMode mode)
        {
            if (activeMode == mode) return;

            if (activeMode != null)
                activeMode.Stop();
            activeMode = mode;
            if (activeMode != null)
                activeMode.Start();
        }

        public void DisableActiveMode()
        {
            SwitchActiveMode(null);
        }

    }
}