using System;
using Assets.SimpleGame.Scripts;
using UnityEngine;

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