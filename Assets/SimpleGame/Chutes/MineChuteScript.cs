﻿using System;
using System.Collections.Generic;
using Assets.UnityAdditions;
using UnityEngine;

namespace Assets.SimpleGame.Chutes
{
    public class MineChuteScript : MonoBehaviour
    {
        [SerializeField] private float productionInterval = 1;
        [SerializeField] private ChuteTransportPointScript outputPoint;
        [SerializeField] private string itemType;

        public void Start()
        {

//            outputPoint.IsOutput = true;
        }

        private void OnEnable()
        {
            Debug.Log("Start miner");

            this.StartCoroutine(mineRoutine);

        }



        public void Update()
        {

        }

        IEnumerable<YieldInstruction> mineRoutine()
        {
            Debug.Log("Start miner routine");
            yield return null;
            for (; ; )
            {
                Debug.Log("Emit miner " + gameObject.name);
                if (outputPoint.Chute != null)
                    outputPoint.EmitIfFreeSpace(ChuteItemType.GetById(itemType));
                
                yield return new WaitForSeconds(productionInterval);
            }
        }
    }
}