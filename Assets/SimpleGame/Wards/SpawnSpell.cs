using System;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.WardDrawing;
using UnityEngine;

namespace Assets.SimpleGame.Wards
{
    [CreateAssetMenu(fileName = "SpawnSpell", menuName = "Flow/SpawnSpell")]
    public class SpawnSpell : AbstractWardSpell
    {

        public GameObject Prefab;

        public override void Cast(PlayerScript player)
        {
            var camTransform = player.GetCameraTransform();

            var inst = Instantiate(Prefab, camTransform.position + camTransform.forward * 1, camTransform.rotation);
            inst.GetComponentInChildren<MeshWardViewScript>().SetShape(Ward.Shape, inst.transform.localToWorldMatrix);
            LocalPlayerScript.Instance.GetPlayer().AirSpellCasting = false;
        }

    }
}