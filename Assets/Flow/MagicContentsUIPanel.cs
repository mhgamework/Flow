using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Flow
{
    public class MagicContentsUIPanel : MonoBehaviour
    {
        public MagicType MagicType;
        private Wizard wizard;
        private Text text;

        public void Start()
        {
            wizard = Wizard.Instance;
            text = GetComponentInChildren<Text>();
        }

        public void Update()
        {

            var amount = wizard.GetMagicAmount(MagicType);
            var rate = wizard.GetMagicChangeRate(MagicType);

            text.text = MagicType.ToString() + "\n" 
                + amount.ToString("#0.0") + "/" + wizard.MaxMagic + "\n" + 
                rate.ToString("#0.0") + " / sec";
        }
    }
}
