using System.Linq;
using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class WizardScript : MonoBehaviour
    {
        private Wizard wizard;
        private GridUserInputScript inputScript;
        private TilePlaceHelper tilePlaceHelper;

        public void Start()
        {
            inputScript = GridUserInputScript.Instance;
            wizard = GandalfDIScript.Instance.Get<Wizard>();
            tilePlaceHelper = GandalfDIScript.Instance.Get<TilePlaceHelper>();

            wizard.TeleportTo(tilePlaceHelper.GetCell(transform));

        }
        public void Update()
        {
            if (inputScript.ClickedCell != null)
            {
                Debug.Log("WizardMove");
                if (wizard.GetMovementOptions().Contains(inputScript.ClickedCell))
                {
                    wizard.MoveTo(inputScript.ClickedCell);
                }

            }

            tilePlaceHelper.ToTransform(transform, wizard.CurrentCell);
        }
    }
}