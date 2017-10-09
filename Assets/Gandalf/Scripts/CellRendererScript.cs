using System.Linq;
using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class CellRendererScript : MonoBehaviour
    {
        public Transform GroundNoMagic;
        public Transform GroundMagic;

        public void Start()
        {
            
        }

        public void Update()
        {
            GroundMagic.gameObject.SetActive(Cell.HasMagic());
            GroundNoMagic.gameObject.SetActive(!Cell.HasMagic());
        }


        public Cell Cell { get; set; }
    }
}