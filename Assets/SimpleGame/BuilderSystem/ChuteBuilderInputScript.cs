using System.Collections.Generic;
using System.Net;
using Assets.SimpleGame.Chutes;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.BuilderSystem
{
    public class ChuteBuilderInputScript : MonoBehaviour
    {
        [SerializeField] private ChuteScript chutePrefab;

        private Vector3 startPoint;

        private Vector3 curPos;
        private List<Vector3> chuteList = new List<Vector3>();



        public void OnClick(ChuteTransportPointScript chutePoint)
        {
            //            if (startPoint == null && chutePoint.CanStart())
            //                startPoint = chutePoint;
            //            else if (startPoint != null)
            //            {
            //                if (chutePoint == startPoint) return;
            //                if (chutePoint.CanEnd())
            //                {
            //                    var chute = Instantiate(chutePrefab, SimpleGameSystemScript.Instance.Get<DynamicObjectsContainerScript>().transform);
            //
            //                    startPoint.ChuteB = chute;
            //                    chutePoint.ChuteA = chute;
            //
            //
            //                    chute.SetupChute(startPoint, chutePoint);
            //
            //
            //                }
            //                startPoint = null;
            //            }
        }

        public void StopBuilding()
        {
        }

        public void DoBuildUpdate(ChuteTransportPointScript chutePoint)
        {
            //            if (Input.GetMouseButtonDown(0))
            //            {
            //                this.OnClick(chutePoint);
            //            }
            //
            //            if (Input.GetMouseButtonDown(1))
            //            {
            //                if (chutePoint.ChuteA != null)
            //                    chutePoint.ChuteA.Destroy();
            //                if (chutePoint.ChuteB != null)
            //                    chutePoint.ChuteB.Destroy();
            //            }
        }

        public void DoUpdate(Vector3 resultPosition, Vector3 resultNormal, Vector3 resultLookDir, BuildSystemInputTool.MouseButton resultMouseButton, IBuilderInteractable resultInteractable)
        {
            curPos = resultPosition + resultNormal.normalized * 0.5f;
            Debug.DrawLine(curPos, curPos + resultNormal.normalized, Color.red);
            Debug.Log("FFF " + curPos);
            if (Input.GetMouseButtonDown(0))
            {
                chuteList.Add(curPos);
            }

            for (int i = 0; i < chuteList.Count - 1; i++)
            {
                Debug.DrawLine(chuteList[i], chuteList[i + 1], Color.red);

            }
        }


    }
}
