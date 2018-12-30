using System.Collections.Generic;

namespace Assets.MHGameWork.FlowGame.UI
{
    public interface IFlowGameUiProvider
    {
        string ActiveModeName { get; }
        IEnumerable<FlowGameUiButtonDescription> ButtonDescriptions { get; }
        IEnumerable<FlowGameUiResourceCount> ResourceCounts { get; }
    }

    public class ExampleFlowGameUiProvider : IFlowGameUiProvider
    {
        public string ActiveModeName
        {
            get { return "Placing Gnomes"; }
        }

        public IEnumerable<FlowGameUiButtonDescription> ButtonDescriptions
        {
            get
            {
                return new[]
                {
                    new FlowGameUiButtonDescription("R", "does this"),
                    new FlowGameUiButtonDescription("B", "does something else"),
                    new FlowGameUiButtonDescription("C", "does nothing at all"),
                };
            }
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

    public class FlowGameUiButtonDescription
    {
        public string ButtonName;
        public string ActionDescription;

        public FlowGameUiButtonDescription(string buttonName, string actionDescription)
        {
            ButtonName = buttonName;
            ActionDescription = actionDescription;
        }
    }

    public class FlowGameUiResourceCount
    {
        public int Amount;
        public int Max;
        public string Name;

        public FlowGameUiResourceCount(int amount, int max, string name)
        {
            Amount = amount;
            Max = max;
            Name = name;
        }
    }
}