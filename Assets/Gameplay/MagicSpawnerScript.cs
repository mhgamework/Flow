using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Gameplay
{
    public class MagicSpawnerScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public string MagicType;
        public float MagicGrowSpeed = 0.1f;
        public float EmissionSize = 1;
        public float MaxBuffered = 3;

        public float MagicEmitted = 0;

        public Transform EmittedMagicRenderable;


        private bool absorbing = false;
        public float AbsorbSpeed = 1;

        private bool targeted = false;

        public void Start()
        {
            oriColor = EmittedMagicRenderable.GetComponentInChildren<Renderer>().material.color;

            StartCoroutine(Emit().GetEnumerator());

        }

        public void Update()
        {
            if (absorbing)
            {
                if (!Input.GetMouseButton(0) || !LocalPlayerSingleton.Instance.InInteractRange(transform))
                {
                    absorbing = false;
                    return;
                }
                var amount = Time.deltaTime * AbsorbSpeed;
                amount = Mathf.Min(amount, MagicEmitted);
                LocalPlayerSingleton.Instance.Inventory.Add(MagicType, amount);
                MagicEmitted -= amount;
                UpdateEmittedMagicRenderable();
            }
            if (targeted)
            {
                if (LocalPlayerSingleton.Instance.InInteractRange(transform))
                    setHighlight();
                else
                    setNonHighlight();

            }
        }


        public IEnumerable<YieldInstruction> Emit()
        {
            for (; ; )
            {
                UpdateEmittedMagicRenderable();

                var nextEmit = EmissionSize / MagicGrowSpeed;
                yield return new WaitForSeconds(nextEmit);
                MagicEmitted = Mathf.Min(MaxBuffered, MagicEmitted + EmissionSize);
                UpdateEmittedMagicRenderable();
            }
        }

        private void UpdateEmittedMagicRenderable()
        {
            EmittedMagicRenderable.localScale = new Vector3(1, 1, 1) * MagicEmitted;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                absorbing = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("PointerUp");
        }

        private Color oriColor;
        public void OnPointerEnter(PointerEventData eventData)
        {
            targeted = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targeted = false;
            setNonHighlight();
        }
        private void setHighlight()
        {
            var renderer = EmittedMagicRenderable.GetComponentInChildren<Renderer>();
            renderer.material.color = Color.Lerp(oriColor, Color.white, 0.5f);
        }

        void setNonHighlight()
        {
            var renderer = EmittedMagicRenderable.GetComponentInChildren<Renderer>();
            renderer.material.color = oriColor;
        }
    }
}