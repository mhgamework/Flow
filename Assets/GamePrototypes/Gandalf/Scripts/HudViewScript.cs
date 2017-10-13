using System.Linq;
using Assets.Gandalf.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gandalf.Scripts
{
    public class HudViewScript : MonoBehaviour
    {
        public Text StateText;
        public Text HoveredCellText;
        private UIControllerScript uiControllerScript;
        private GridUserInputScript gridUserInputScript;

        public RectTransform ActionButtonsPanel;

        public void Start()
        {
            uiControllerScript = UIControllerScript.Instance;
            gridUserInputScript = GridUserInputScript.Instance;
            var buttons = ActionButtonsPanel.GetComponentsInChildren<Button>(true);
            foreach (var b in buttons)
            {
                b.onClick.AddListener(() => onClickAction(b.GetComponentInChildren<Text>().text));
            }
        }

      

        private IUISelectable lastSelected;

        public void Update()
        {
            StateText.text = uiControllerScript.State.ToString();

            updateInfoBox();
        }

        private void updateInfoBox()
        {
            var selected = uiControllerScript.Selected;
            //if (lastSelected == selected) return;
            lastSelected = selected;
            if (selected == null)
            {
                HoveredCellText.text = "Nothing selected";

                foreach (var b in ActionButtonsPanel.GetComponentsInChildren<Button>(true))
                    b.gameObject.SetActive(false);
                    return;
            }

            var statsText = selected.Values.Aggregate("", (acc, e) => acc + e.Key + ": " + e.Value + "\n");

                HoveredCellText.text =
                    "== " + selected.Name +
                    "\n" +
                    "[Items]\n" +
                    selected.Items.ToItemString() +
                    "\n" +
                    "[Stats]\n" +
                    statsText;

            var buttons = ActionButtonsPanel.GetComponentsInChildren<Button>(true);
            var actions = selected.Actions.ToArray();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (actions.Length <= i)
                    buttons[i].gameObject.SetActive(false);
                else
                {
                    buttons[i].gameObject.SetActive(true);
                    buttons[i].GetComponentInChildren<Text>().text = actions[i];
                }
            }
        }

        private void onClickAction(string action)
        {
            lastSelected.DoAction(action);
        }
    }
}