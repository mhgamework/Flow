using System.Collections.Generic;
using System.Linq;
using Assets.MHGameWork.FlowGame.ModeBasedInput;
using Assets.MHGameWork.FlowGame.PlayerInputting.Interacting;
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
        private readonly FlowGameInteractionSystem flowGameInteractionSystem;

        public KeyCode DisableActiveModeKey { get; set; }
        public KeyCode InteractKey { get; set; }

        public List<InputModeInfo> Modes { get; private set; }

        public InputModeInfo ActiveMode
        {
            get
            {
                return Modes.FirstOrDefault(f => f.Mode == modeBasedInputSystem.ActiveMode);
            }
        }

        public FlowGamePlayerInput(ModeBasedInputSystem modeBasedInputSystem,FlowGameInteractionSystem flowGameInteractionSystem)
        {
            this.modeBasedInputSystem = modeBasedInputSystem;
            this.flowGameInteractionSystem = flowGameInteractionSystem;
            DisableActiveModeKey = KeyCode.Alpha0;
            InteractKey = KeyCode.E;
            Modes = new List<InputModeInfo>();
        }

        public void BindInputModeToKey(KeyCode key, IInputMode mode,string name)
        {
            Modes.Add(new InputModeInfo(key, mode,name));
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

            if (Input.GetKeyDown(InteractKey))
            {
                flowGameInteractionSystem.TriggerPlayerInteract();
            }
        }

        public class InputModeInfo
        {
            public KeyCode Key;
            public IInputMode Mode;
            public string ModeName ;


            public InputModeInfo(KeyCode key, IInputMode mode, string modeName)
            {
                Key = key;
                Mode = mode;
                ModeName = modeName;
            }
        }
    }
}