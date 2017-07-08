using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Homm.UI
{
    public class BuyUnitsDialog : Singleton<BuyUnitsDialog>
    {
        public Text TxtAvailable;
        public Text TxtRecruit;
        public Text TxtBuyButton;
        public Text TxtTotalPrice;
        public Button BtnBuy;
        public Button BtnCancel;
        public Slider Slider;

        public Transform Container;

        Action back = null;
        private UnitCamp unitCamp;

        public void Start()
        {
            BtnBuy.onClick.AddListener(onBuy);
            BtnCancel.onClick.AddListener(onCancel);
            Slider.onValueChanged.AddListener(f => updateView());
            Container.gameObject.SetActive(false);
        }

        private void onCancel()
        {
            Container.gameObject.SetActive(false);
            var call = back; // So can be reshown in callback!
            back = null;
            call();
        }

        private void onBuy()
        {
            unitCamp.BuyUnits((int)Slider.value);
            updateView();
        }

        private void updateView()
        {
            TxtAvailable.text = unitCamp.AvailableUnits.ToString();
            Slider.maxValue = unitCamp.GetMaxUnitBuyCount();
            TxtRecruit.text = Slider.value.ToString();
            TxtBuyButton.text = "Buy " + Slider.value + " units.";
            TxtTotalPrice.text = unitCamp.UnitCrystalPrice * (int)Slider.value + " crystals";

        }

        public IEnumerable<YieldInstruction> ShowCoroutine(UnitCamp camp)
        {
            unitCamp = camp;

            var set = false;
            if (back != null) throw new Exception("Already showing dialog!");

            back = () => set = true;
            Container.gameObject.SetActive(true);
            updateView();

            while (!set)
                yield return null;
        }
    }
}