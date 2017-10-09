using Assets.Gandalf.Domain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Gandalf.Scripts
{
    public class GoblinScript : MonoBehaviour, IPointerClickHandler
    {
        private UIControllerScript uiControllerScript;
        public Goblin Goblin { get; set; }

        public void Start()
        {
            StartCoroutine(Goblin.SimulateMovement().GetEnumerator());
            uiControllerScript = UIControllerScript.Instance;

        }

        public void Update()
        {
            transform.position = Goblin.CalculateRenderingPosition(0);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                uiControllerScript.OnGoblinRightClick(this);
        }
    }
}