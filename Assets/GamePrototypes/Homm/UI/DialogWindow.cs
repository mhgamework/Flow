using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Homm.UI
{
    public class DialogWindow : Singleton<DialogWindow>
    {
        public Text Description;
        public Text Resource;
        public Button Ok;
        public Transform Container;

        Action back = null;

        public void Start()
        {
            Ok.onClick.AddListener(onOk);
            Container.gameObject.SetActive(false);
        }

        private void onOk()
        {
            Container.gameObject.SetActive(false);
            var call = back; // So can be reshown in callback!
            back = null;
            call();
            
        }

        public void Show(string desc,string resource,  Action callback)
        {
            if (back != null) throw new Exception("Already showing dialog!");
            Description.text = desc;
            Resource.text = resource;

            back = callback;

            Container.gameObject.SetActive(true);
        }

        public IEnumerable<YieldInstruction> ShowCoroutine(string desc, string resource)
        {
            var set = false;

            Show(desc, resource, () => set = true);

            while (!set)
                yield return null;
        }
    }
}