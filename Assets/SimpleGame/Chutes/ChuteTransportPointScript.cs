using System;
using Assets.SimpleGame.BuilderSystem;
using UnityEngine;

namespace Assets.SimpleGame.Chutes
{
    public class ChuteTransportPointScript :MonoBehaviour, IBuilderInteractable
    {
        public ChuteScript ChuteA;
        public ChuteScript ChuteB;
        public bool IsInput;
        public bool IsOutput;


        public bool CanStart()
        {
            return !IsInput;
        }

        public bool CanEnd()
        {
            return !IsOutput;
        }

        public void ConnectTo(ChuteTransportPointScript endPoint)
        {
            if (!CanStart()) throw new Exception();
            if (!endPoint.CanEnd()) throw new Exception();

        }

        public void Disconnect()
        {
//            if (ChuteA != null)
//            {
//                ChuteA.Destroy();
//            }
//
//            if (ChuteB != null)
//            {
//                ChuteB.Destroy();
//            }
        }


        public void OnUnBuilt()
        {
            
        }

        public void OnBuilt(Vector3 position, Vector3 normal, Vector3 lookDir)
        {
        }
    }
}