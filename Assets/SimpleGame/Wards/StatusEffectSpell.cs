using System;
using Assets.Flow;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.WardDrawing;
using DirectX11;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame.Wards
{
    [CreateAssetMenu(fileName = "SpawnSpell", menuName = "Flow/StatusEffectSpell")]
    public class StatusEffectSpell : AbstractWardSpell
    {

        public AbstractStatusEffect StatusEffect;

        public override void Cast(PlayerScript player)
        {
            player.Entity.ApplyEffect(StatusEffect);
            player.AirSpellCasting = false;
        }

    }
}