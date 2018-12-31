using System.Collections.Generic;
using System.Linq;
using Assets.MHGameWork.FlowGame.UI;

namespace Assets.MHGameWork.FlowGame.PlayerInput
{
    public class WiredFlowGameUiProvider : IFlowGameUiProvider
    {
        private FlowGamePlayerInput playerInput;

        public WiredFlowGameUiProvider(FlowGamePlayerInput playerInput)
        {
            this.playerInput = playerInput;
        }

        public string ActiveModeName
        {
            get
            {
                if (playerInput.ActiveMode == null) return "No active mode";
                return playerInput.ActiveMode.ModeName;
            }
        }
        public IEnumerable<FlowGameUiButtonDescription> ButtonDescriptions
        {
            get { return playerInput.Modes.Select(f => new FlowGameUiButtonDescription(f.Key.ToString(), f.ModeName)); }

        }

        public IEnumerable<FlowGameUiResourceCount> ResourceCounts
        {
            get
            {
                return new[]
                {
                    new FlowGameUiResourceCount(10,100,"Magic crystals"),
                    new FlowGameUiResourceCount(20,50,"Rock"),
                    new FlowGameUiResourceCount(0,10,"Firestone"),
                };
            }
        }
    }
}