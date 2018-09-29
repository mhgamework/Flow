using System.Net;
using Assets.SimpleGame.Chutes;
using UnityEngine;

namespace Assets.SimpleGame.BuilderSystem
{
    public class ChuteBuilderInputScript : MonoBehaviour
    {
        [SerializeField] private ChuteScript chutePrefab;

        private ChuteTransportPointScript startPoint;

        public void OnClick(ChuteTransportPointScript chutePoint)
        {
            if (startPoint == null && chutePoint.CanStart())
                startPoint = chutePoint;
            else if (startPoint != null)
            {
                if (chutePoint == startPoint) return;
                if (chutePoint.CanEnd())
                {
                    var chute = Instantiate(chutePrefab, SimpleGameSystemScript.Instance.Get<DynamicObjectsContainerScript>().transform);

                    startPoint.ChuteB = chute;
                    chutePoint.ChuteA = chute;


                    chute.SetupChute(startPoint, chutePoint);


                }
                startPoint = null;
            }
        }

        public void StopBuilding()
        {
        }

        public void DoBuildUpdate(ChuteTransportPointScript chutePoint)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.OnClick(chutePoint);
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (chutePoint.ChuteA != null)
                    chutePoint.ChuteA.Destroy();
                if (chutePoint.ChuteB != null)
                    chutePoint.ChuteB.Destroy();
            }
        }
    }
}