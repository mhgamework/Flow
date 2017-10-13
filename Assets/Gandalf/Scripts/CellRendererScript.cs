using System.Linq;
using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class CellRendererScript : MonoBehaviour
    {
        public Transform GroundNoMagic;
        public Transform GroundMagic;
        private MagicChargeService magicChargeService;

        public void Start()
        {
            magicChargeService = GandalfDIScript.Instance.Get<MagicChargeService>();
        }

        public void Update()
        {
            var hasMagic = magicChargeService.HasMagic(Cell);
            GroundMagic.gameObject.SetActive(hasMagic);
            GroundNoMagic.gameObject.SetActive(!hasMagic);
        }


        public Cell Cell { get; set; }
    }
}