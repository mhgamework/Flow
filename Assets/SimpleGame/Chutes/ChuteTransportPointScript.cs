using System;
using Assets.SimpleGame.BuilderSystem;
using UnityEngine;

namespace Assets.SimpleGame.Chutes
{
    public class ChuteTransportPointScript : MonoBehaviour, IBuilderInteractable
    {
        public ChuteScript Chute;



        //        public ChuteScript ChuteB;
        //        public bool IsInput;
        //        public bool IsOutput;


        //        public bool CanStart()
        //        {
        //            return !IsInput;
        //        }
        //
        //        public bool CanEnd()
        //        {
        //            return !IsOutput;
        //        }

        //        public void ConnectTo(ChuteTransportPointScript endPoint)
        //        {
        //            if (!CanStart()) throw new Exception();
        //            if (!endPoint.CanEnd()) throw new Exception();
        //
        //        }
        //
        //        public void Disconnect()
        //        {
        ////            if (ChuteA != null)
        ////            {
        ////                ChuteA.Destroy();
        ////            }
        ////
        ////            if (ChuteB != null)
        ////            {
        ////                ChuteB.Destroy();
        ////            }
        //        }


        public void OnUnBuilt()
        {
        }

        public void OnBuilt(Vector3 position, Vector3 normal, Vector3 lookDir)
        {
        }

        public void EmitIfFreeSpace(ChuteItemType type)
        {
            if (Chute == null)
            {
                Debug.LogError("No chute connected, cant emit", this);
                return;
            }
            Chute.EmitIfFreeSpace(this, type);
        }

        public ChuteItemScript TakeItem()
        {
            if (Chute == null)
            {
                return null;
            }
            return Chute.TakeItemAtTransportPoint(this);
        }

        public bool HasItem()
        {
            return HasItem(t => true);
        }
        public bool HasItem(Func<ChuteItemType, bool> predicate)
        {
            if (Chute == null) return false;
            var get = Chute.GetItemAtTransportPoint(this) ;
            if (get == null) return false;
            return predicate(get.Type);
        }


        public bool IsFree
        {
            get
            {
                if (Chute == null) return false;
                return Chute.HasFreeSpace(this);
            }
        }



    }
}