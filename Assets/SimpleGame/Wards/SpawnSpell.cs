using System;
using Assets.Flow;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.WardDrawing;
using DirectX11;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame.Wards
{
    [CreateAssetMenu(fileName = "SpawnSpell", menuName = "Flow/SpawnSpell")]
    public class SpawnSpell : AbstractWardSpell
    {
        public string WardShape;

        public GameObject Prefab;

        public SpawnSpell()
        {


        }
        private void OnEnable()
        {
            if (WardShape == "light")
                Ward = Ward.Create(new Point3(0, 1, 0), new Point3(1, 0, 0), new Point3(0, -1, 0), new Point3(-1, 0, 0), new Point3(0, 1, 0));
            else if (WardShape == "explosion")
                Ward = Ward.Create(
                    Ward.CreateLine(new Point3(1, 1, 0), new Point3(-1, -1, 0)),
                    Ward.CreateLine(new Point3(1, -1, 0), new Point3(-1, 1, 0)));
            else
                Ward = Ward.Create(new Point3(0, 1, 0), new Point3(1, 0, 0), new Point3(0, -1, 0), new Point3(-1, 0, 0), new Point3(0, 1, 0));
        }

        public override void Cast(PlayerScript player)
        {
            var camTransform = player.GetCameraTransform();

            var inst = Instantiate(Prefab, camTransform.position + camTransform.forward * 1, camTransform.rotation);
            inst.GetComponentInChildren<MeshWardViewScript>().SetShape(Ward.Shape, inst.transform.localToWorldMatrix);
            PlayerScript.Instance.AirSpellCasting = false;
        }

    }
}