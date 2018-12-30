using System;
using System.Linq;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MHGameWork.FlowGame.UI
{
    public class FlowGameUiScript : MonoBehaviour
    {
        [SerializeField]
        private Text activeModeText;
        [SerializeField]
        private Text helpText;
        [SerializeField]
        private Text resourcesText;

        private IFlowGameUiProvider provider;

        public void Start()
        {
            provider = new ExampleFlowGameUiProvider();
            var buttonActions = provider.ButtonDescriptions.Select(f => f.ButtonName + " " + f.ActionDescription);
            helpText.text = string.Join("\n", new[] {"Help:"}.Concat(buttonActions).ToArray());

        }

        public void Update()
        {
            //TODO: check for gc problems
            activeModeText.text = "Currently:\n" + provider.ActiveModeName;
            resourcesText.text =string.Join("   -   ", provider.ResourceCounts.Select(f => f.Amount + "/"+f.Max + " " + f.Name).ToArray());
        }
    }
}