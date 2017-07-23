using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Flow
{
    public class MagicGridCell
    {
        private const int MaxMagicContents = 1000;
        public List<Wizard.MagicAmount> MagicAmounts = new List<Wizard.MagicAmount>();

        public float ChangeMagic(MagicType magicType, float delta)
        {
            var amount = GetMagicAmountInternal(magicType);

            var oldAmount = amount.Amount;
            amount.Amount = Mathf.Clamp(amount.Amount + delta, 0, MaxMagicContents);

            return amount.Amount - oldAmount;
        }

        public float GetMagicAmount(MagicType life)
        {
            return GetMagicAmountInternal(life).Amount;
        }
        public Wizard.MagicAmount GetMagicAmountInternal(MagicType life)
        {
            var amounts = MagicAmounts.FirstOrDefault();

            if (amounts == null)
            {
                amounts = new Wizard.MagicAmount { Type = life, Amount = 0 };
                MagicAmounts.Add(amounts);
            }
            return amounts;
        }
    }
}