using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.SimpleGame.Wards
{
    public class EntityScript : MonoBehaviour
    {
        private List<ActiveEffect> StatusEffects = new List<ActiveEffect>();

        public bool Invisible;
        public float SpeedMultiplier = 1;

        private void OnEnable()
        {

        }

        public void Update()
        {
            for (var i = 0; i < StatusEffects.Count; i++)
            {
                var s = StatusEffects[i];
                s.Effect.Update(this);
                if (!(s.StartTime + s.Effect.Duration > Time.timeSinceLevelLoad))
                {
                    s.Effect.EndEffect(this);
                    StatusEffects.Remove(s);
                    i--; // was removed, so redo
                }
            }
        }

        public void ApplyEffect(AbstractStatusEffect effect)
        {
            var existing = StatusEffects.FirstOrDefault(f => f.Effect == effect);
            if (existing != null)
            {
                existing.StartTime = Time.timeSinceLevelLoad;
                return;
            }
            StatusEffects.Add(new ActiveEffect { Effect = effect, StartTime = Time.timeSinceLevelLoad });
            effect.StartEffect(this);
        }



        [Serializable]
        class ActiveEffect
        {
            public AbstractStatusEffect Effect;
            public float StartTime;
        }
    }
}