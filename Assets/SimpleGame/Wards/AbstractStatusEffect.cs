using System;
using UnityEngine;

namespace Assets.SimpleGame.Wards
{
    [Serializable]
    public abstract class AbstractStatusEffect : ScriptableObject
    {
        public float Duration;
        public abstract void StartEffect(EntityScript entity);
        public abstract void EndEffect(EntityScript entity);

        public abstract void Update(EntityScript entityScript);
    }
}