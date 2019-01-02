using System.Collections.Generic;
using System.Linq;
using Assets.MHGameWork.FlowGame.Domain;
using Assets.MHGameWork.FlowGame.PlayerStating;
using Assets.MHGameWork.FlowGame.UI;

namespace Assets.MHGameWork.FlowGame.PlayerInput
{
    public class WiredFlowGameUiProvider : IFlowGameUiProvider
    {
        private FlowGamePlayerInput playerInput;
        private readonly PlayerGlobalResourcesRepository playerGlobalResourcesRepository;

        public WiredFlowGameUiProvider(FlowGamePlayerInput playerInput,PlayerGlobalResourcesRepository playerGlobalResourcesRepository)
        {
            this.playerInput = playerInput;
            this.playerGlobalResourcesRepository = playerGlobalResourcesRepository;
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
            get
            {
                var playerInputModeButtons = playerInput.Modes.Select(f => new FlowGameUiButtonDescription(f.Key.ToString(), f.ModeName));
                return
                    new[]
                        {
                            new FlowGameUiButtonDescription(playerInput.InteractKey.ToString(),"Interact"),
                            new FlowGameUiButtonDescription(playerInput.DisableActiveModeKey.ToString(),"clear active mode"),
                        }
                        .Concat(playerInputModeButtons)
                        .ToList();
            }
        }

        public IEnumerable<FlowGameUiResourceCount> ResourceCounts
        {
            get
            {
                return new[]
                {
                    getResourceCountDesc(ResourceTypeFactory.MagicCrystals),
                    getResourceCountDesc(ResourceTypeFactory.Rock),
                    getResourceCountDesc(ResourceTypeFactory.Firestone),
                };
            }
        }

        private FlowGameUiResourceCount getResourceCountDesc( ResourceType resourceType)
        {
            return new FlowGameUiResourceCount(
                playerGlobalResourcesRepository.GetResourceAmount(resourceType),
                playerGlobalResourcesRepository.GetMaxResourceAmount(resourceType),
                resourceType.Name);
        }
    }
}