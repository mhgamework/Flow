using System.Collections.Generic;
using Assets.MHGameWork.FlowGame.ModeBasedInput;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.PlayerInput
{
    /// <summary>
    /// Converts player input (keypresses,mouse) into flow game actions.
    /// Can probably be split or generalized?
    /// </summary>
    public class FlowGamePlayerInput
    {
        private ModeBasedInputSystem modeBasedInputSystem;

        public KeyCode DisableActiveModeKey { get; set; }

        public List<InputModeInfo> Modes { get; private set; }

        public FlowGamePlayerInput(ModeBasedInputSystem modeBasedInputSystem)
        {
            this.modeBasedInputSystem = modeBasedInputSystem;
            DisableActiveModeKey = KeyCode.Alpha0;
            Modes = new List<InputModeInfo>();
        }

        public void BindInputModeToKey(KeyCode key, IInputMode mode)
        {
            Modes.Add(new InputModeInfo(key, mode));
        }

        public void Update()
        {
            for (int i = 0; i < Modes.Count; i++)
            {
                if (Input.GetKeyDown(Modes[i].Key))
                    modeBasedInputSystem.SwitchActiveMode(Modes[i].Mode);
            }

            if (Input.GetKeyDown(DisableActiveModeKey))
            {
                modeBasedInputSystem.DisableActiveMode();
            }
        }

        public class InputModeInfo
        {
            public KeyCode Key;
            public IInputMode Mode;

            public InputModeInfo(KeyCode key, IInputMode mode)
            {
                Key = key;
                Mode = mode;
            }
        }
    }
}