using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame.WardDrawing
{
    public class WardDrawingModeScript : MonoBehaviour
    {
        public WardDrawInputScript wardDrawInputScript;

        private WardComparer comparer = new WardComparer();

        private List<Ward> wards = new List<Ward>();
        public event Action<Ward> OnCorrectWard;

        public void Start()
        {
        }

        public void Update()
        {
            foreach (var w in wards)
            {
                if (comparer.Match(wardDrawInputScript.CurrentShapeLocal, w.Shape).Any(k => k.ExactMatch))
                {
                    if (OnCorrectWard != null)
                        OnCorrectWard.Invoke(w);
                }
            }
        }


        public void SetWards(List<Ward> wards)
        {
            this.wards = wards;
        }

        public void SetPlane(Vector3 point, Vector3 normal)
        {
            wardDrawInputScript.SetPlane(point, normal);
        }

        public void SetModeEnabled(bool enabled)
        {
            gameObject.SetActive(enabled);
            wardDrawInputScript.gameObject.SetActive(enabled);
            if (enabled)
            {
                Cursor.lockState = CursorLockMode.None; // Doesnt work in editor: CursorLockMode.Confined;
                Cursor.visible = true;
            }

        }
    }
}