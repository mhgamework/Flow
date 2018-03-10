using UnityEngine;

namespace Assets.SimpleGame.Wards
{
    [CreateAssetMenu(fileName = "HastEffect", menuName = "Flow/Effects/Haste")]
    public class HasteStatusEffect : AbstractStatusEffect
    {
        public float SpeedMultiplier;

        public override void StartEffect(EntityScript entity)
        {
            entity.SpeedMultiplier = SpeedMultiplier;
        }

        public override void EndEffect(EntityScript entity)
        {
            entity.SpeedMultiplier = 1;

        }

        public override void Update(EntityScript entityScript)
        {
        }
    }
}