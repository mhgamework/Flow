using System.Linq;
using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class WizardScript : MonoBehaviour
    {
        private Wizard wizard;
        private TilePlaceHelper tilePlaceHelper;

        public void Start()
        {
            wizard = GandalfDIScript.Instance.Get<Wizard>();
            tilePlaceHelper = GandalfDIScript.Instance.Get<TilePlaceHelper>();

            wizard.TeleportTo(tilePlaceHelper.GetCell(transform));

        }
        public void Update()
        {
            tilePlaceHelper.ToTransform(transform, wizard.CurrentCell);
        }
    }
}