using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Flow
{
    public class Wizard : Singleton<Wizard>
    {
        public float MaxMagic;
        public List<MagicAmount> MagicAmounts = new List<MagicAmount>();
        public List<MagicAmount> PrevMagicAmounts = new List<MagicAmount>();
        public List<MagicAmount> MagicChangeRates = new List<MagicAmount>();
        private MagicGrid magicGrid;

        public float AbsorbSpeed = 1;
        public float LifeConsumptionSpeed = 0.5f;
        public float ChangeRateUpdateInterval = 1;

        public MagicGridCell CurrentCell { get; private set; }

        public void Start()
        {
            magicGrid = MagicGrid.Instance;
            MagicAmounts.AddRange(Enum.GetValues(typeof(MagicType)).Cast<MagicType>().Select(k => new MagicAmount { Type = k }));
            PrevMagicAmounts.AddRange(Enum.GetValues(typeof(MagicType)).Cast<MagicType>().Select(k => new MagicAmount { Type = k }));
            MagicChangeRates.AddRange(Enum.GetValues(typeof(MagicType)).Cast<MagicType>().Select(k => new MagicAmount { Type = k }));
            StartCoroutine(updateChangeRates().GetEnumerator());
        }

        public IEnumerable<YieldInstruction> updateChangeRates()
        {
            for (;;)
            {
                yield return new WaitForSeconds(ChangeRateUpdateInterval);
                for (int i = 0; i < MagicAmounts.Count; i++) // Assume same order!
                {
                    MagicChangeRates[i].Amount = (MagicAmounts[i].Amount - PrevMagicAmounts[i].Amount) / ChangeRateUpdateInterval;
                    PrevMagicAmounts[i].Amount = MagicAmounts[i].Amount;
                }

            }

        }
        public void Update()
        {
            CurrentCell = magicGrid.GetCellFromWorldPos(transform.position);

            var lifeIncrease = -CurrentCell.ChangeMagic(MagicType.Life, -AbsorbSpeed * Time.deltaTime);

            MagicAmounts.First(k => k.Type == MagicType.Life).Amount =
                Mathf.Clamp(MagicAmounts.First(k => k.Type == MagicType.Life).Amount + lifeIncrease - LifeConsumptionSpeed * Time.deltaTime, 0, MaxMagic);


        }

        [Serializable]
        public class MagicAmount
        {
            public MagicType Type;
            public float Amount;
        }

        public float GetMagicAmount(MagicType magicType)
        {
            return MagicAmounts.First(k => k.Type == magicType).Amount;

        }

        public float GetMagicChangeRate(MagicType magicType)
        {
            return MagicChangeRates.First(k => k.Type == magicType).Amount;
        }
    }
}