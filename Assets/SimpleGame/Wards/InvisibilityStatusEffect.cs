using Assets.SimpleGame.WardDrawing;
using UnityEngine;

namespace Assets.SimpleGame.Wards
{
    [CreateAssetMenu(fileName = "StatusEffect", menuName = "Flow/Effects/Invisibility")]
    public class InvisibilityStatusEffect : AbstractStatusEffect
    {

        public override void StartEffect(EntityScript entity)
        {
            entity.Invisible = true;
        }

        public override void EndEffect(EntityScript entity)
        {
            entity.Invisible = false;
        }

        public override void Update(EntityScript entityScript)
        {
        }
    }
}