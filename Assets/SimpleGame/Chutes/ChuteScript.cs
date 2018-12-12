using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Reusable.Utils;
using Assets.SimpleGame.Wards;
using Assets.UnityAdditions;
using Boo.Lang.Runtime;
using UnityEngine;

namespace Assets.SimpleGame.Chutes
{
    public class ChuteScript : MonoBehaviour
    {
        [SerializeField] private Transform chuteBody;
        [SerializeField] private ChuteItemScript chuteItemTemplate;
        [SerializeField] private float itemSpeed = 1;
        [SerializeField] private float itemDistance = 0.6f;
        [SerializeField] private float length = 5;
        [SerializeField] private float g = 9.81f;
        [SerializeField] private float itemRadius = 0.5f;
        [SerializeField] private float collisionEnergyLoss = 1f;
        [SerializeField] private float collisionEnergyLossThreshold = 1f;

        [SerializeField] private ChuteScript nextChute;
        private ChuteScript initialChute;

        private float totalChuteLength;

        private List<ChuteItemScript> items;

        public ChuteTransportPointScript startPoint;
        public ChuteTransportPointScript endPoint;






        public void Start()
        {
            initialChute = this;
            items = new List<ChuteItemScript>();
            chuteItemTemplate.gameObject.SetActive(false);
            nextChute = null;
            if (startPoint != null && endPoint != null)
                SetupChute(startPoint, endPoint);

            this.StartCoroutine(start);
        }

        public void SetupChute(ChuteTransportPointScript start, ChuteTransportPointScript end)
        {
            transform.position = start.transform.position;
            chuteBody.localScale = new Vector3(1, 1, Vector3.Distance(end.transform.position, start.transform.position));
            transform.LookAt(end.transform.position);

            // Create chute
            startPoint = start;
            endPoint = end;
            start.Chute = this;
            end.Chute = this;
            //            if (start.ChuteA != null) start.ChuteA.ConnectNextChute(this);
            //            if (end.ChuteB != null) this.ConnectNextChute(end.ChuteB);
        }

        public void Destroy()
        {
            //TODO:
            //            if (startPoint != null )
            //            {
            //                if (startPoint.ChuteA != null) startPoint.ChuteA.nextChute = null;
            //                startPoint.ChuteB = null;
            //            }
            //
            //            if (endPoint != null)
            //            {
            //                if (endPoint.ChuteB != null) endPoint.ChuteB.initialChute = endPoint.ChuteB;
            //                endPoint.ChuteA = null;
            //            }
            Destroy(gameObject);
        }


        public void Update()
        {
            //TODO: Hacky for later
            if (nextChute != null) nextChute.initialChute = initialChute;

            if (initialChute != this) return;

            ChuteScript count = initialChute;
            totalChuteLength = 0;
            while (count != null)
            {
                totalChuteLength += count.getChuteLength();
                count = count.nextChute;
            }

            SimulateMarbles();
        }

        private void SimulateMarbles()
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                var acceleration = g;
                var initialVelocity = acceleration * Time.deltaTime;
                var pushAwayForce = -Vector3.Dot(initialVelocity * Vector3.down, calculateItemNormal(item.Position)) *
                                    calculateItemNormal(item.Position);
                var initialVelocityProjected = (initialVelocity * Vector3.down + pushAwayForce).magnitude;

                var speed = calculateItemSpeed(item);
                if (speed < initialVelocityProjected)
                {
                    // Estimate dir

                    //speed = initialVelocity;
                }

                if (speed < 0.1f)
                {
                    speed = initialVelocity; //0.1f;
                    item.Direction = -Mathf.Sign(calculateItemForward(item.Position).y);
                }


                item.Position += speed * item.Direction * Time.deltaTime;
                if (item.Position > totalChuteLength) item.Position = totalChuteLength;

                var prev = getPrevItem(i);
                var next = getNextItem(i);
                if (next) collide(item, next);
                if (prev) collide(prev, item);


                if (item.Position > length) item.Position = length;
                var pos = calculateItemPos(item.Position);
                var normal = calculateItemNormal(item.Position);
                var forward = calculateItemForward(item.Position);
                //Debug.DrawLine(pos, pos + normal, Color.green);
                //Debug.DrawLine(pos, pos + forward * 0.1f * item.Direction, Color.blue);
                //Debug.DrawLine(pos + forward * 0.1f * item.Direction, pos + forward * (0.1f + speed * 0.2f) * item.Direction, new Color(0.5f, 0.5f, 1));
                //Debug.DrawLine(pos, pos + Vector3.Cross(normal, forward), Color.red);
                item.transform.position = calculateItemPos(item.Position);
                item.transform.rotation =
                    Quaternion.AngleAxis(Mathf.Rad2Deg * (item.Position / itemRadius), Vector3.Cross(normal, forward));


                //                if (item.Position > getChuteLength() - 0.01f && item.Direction > 0)
                //                {
                //                    // Moved outside
                //                    tryEmitToTransportPoint(item, endPoint);
                //                }
                //                else if (item.Position < 0.01f && item.Direction < 0)
                //                {
                //                    tryEmitToTransportPoint(item, startPoint);
                //                }
            }
        }

        public ChuteItemScript GetItemAtTransportPoint(ChuteTransportPointScript point)
        {
            if (items.Count == 0) return null;
            var first = items[0];
            var last = items[items.Count - 1];
            if (point == endPoint)
            {
                if (last.Position > getChuteLength() - 0.01f) return last;
            }

            if (point == startPoint)
            {
                if (first.Position < 0.01f) return first;
            }

            return null;
        }

        public ChuteItemScript TakeItemAtTransportPoint(ChuteTransportPointScript point)
        {
            var get = GetItemAtTransportPoint(point);
            if (get == null) return null;
            items.Remove(get);
            return get;
        }




        private ChuteItemScript getNextItem(int i)
        {
            if (i + 1 < items.Count) return items[i + 1];
            return null;
        }

        private ChuteItemScript getPrevItem(int i)
        {
            if (i == 0) return null;
            return items[i - 1];
        }

        private void collide(ChuteItemScript item, ChuteItemScript next)
        {
            if (!(item.Position + itemDistance > next.Position)) return;

            item.Direction = -1;
            next.Direction = 1;
            var diff = calculateItemSpeed(item) * item.Direction -
                       calculateItemSpeed(next) * next.Direction;
            var collisionEnergy = 1 / 2f * diff * diff;
            var energy = item.StartY + next.StartY;
            if (collisionEnergy > collisionEnergyLossThreshold)
                energy -= collisionEnergy * collisionEnergyLoss; // Now we have a unit problem. We want to lose some kinetic energy, but what are the correct units for energy?
            //TODO: this should be only kinetic energy loss, now it is losing potential energy, thus breaking the laws of nature!!

            item.StartY = Mathf.Max(energy / 2, calculateItemPos(item.Position).y);
            next.StartY = Mathf.Max(energy / 2, calculateItemPos(next.Position).y);

            var posmid = (next.Position + item.Position) / 2f;

            //TODO
            // One option, causes jitter
            item.Position = posmid - itemDistance / 2f;
            next.Position = posmid + itemDistance / 2f;

            //Second option causes: causes the last item in a chain to push elements back wierdly
            //next.Position = item.Position - itemDistance;
        }

        private float calculateItemSpeed(ChuteItemScript item)
        {
            return Mathf.Sqrt(Mathf.Max(0, 2 * g * (item.StartY - calculateItemPos(item.Position).y)));
        }

        private Vector3 calculateItemForward(float pos)
        {
            //TODO: gonna be expensivo
            // copy pasta
            ChuteScript chute = initialChute;
            while (pos > chute.getChuteLength())
            {
                if (chute.nextChute == null)
                    return chute.getChuteForward();
                pos -= chute.getChuteLength();
                chute = chute.nextChute;
            }


            return chute.getChuteForward();
            //if (pos < chuteBody.localScale.z) return transform.up;
            //if (pos > chuteBody.localScale.z * 2) return -transform.up + Vector3.up * transform.up.y * 2;
            //return Vector3.up;
        }

        private Vector3 getChuteForward()
        {
            return transform.forward;
        }

        private Vector3 calculateItemNormal(float pos)
        {
            //TODO: gonna be expensivo
            // copy pasta
            ChuteScript chute = initialChute;
            while (pos > chute.getChuteLength())
            {
                if (chute.nextChute == null)
                    return chute.getChuteNormal();
                pos -= chute.getChuteLength();
                chute = chute.nextChute;
            }


            return chute.getChuteNormal();
            //if (pos < chuteBody.localScale.z) return transform.up;
            //if (pos > chuteBody.localScale.z * 2) return -transform.up + Vector3.up * transform.up.y * 2;
            //return Vector3.up;
        }

        private Vector3 getChuteNormal()
        {
            return transform.up;
        }

        private Vector3 calculateItemPos(float pos)
        {
            //TODO: gonna be expensivo
            // copy pasta
            ChuteScript chute = initialChute;
            while (pos > chute.getChuteLength())
            {
                if (chute.nextChute == null)
                    return getLocalChutePos(chute, chute.getChuteLength());
                pos -= chute.getChuteLength();
                chute = chute.nextChute;
            }

            return getLocalChutePos(chute, pos);
            //return Mathf.Clamp(pos, 0, getChuteLength()) * transform.forward;
            //return Mathf.Clamp(pos, 0, chuteBody.localScale.z) * transform.forward +
            //       Mathf.Clamp(pos - chuteBody.localScale.z, 0, chuteBody.localScale.z) * Vector3.forward +
            //       Mathf.Max(pos - chuteBody.localScale.z * 2, 0) * (-transform.forward + Vector3.forward * transform.forward.z * 2);
        }

        private static Vector3 getLocalChutePos(ChuteScript chute, float pos)
        {
            //NOTE: chuteItemTemplate is here intentionally
            return chute.chuteItemTemplate.transform.position + Mathf.Clamp(pos, 0, chute.getChuteLength()) * chute.transform.forward;
        }

        private float getChuteLength()
        {
            return chuteBody.localScale.z;
        }

        private IEnumerable<YieldInstruction> start()
        {
            //spawn();

            //for (; ; )
            //{
            //    yield return new WaitForSeconds(1);
            //    if (items.First().Position > itemDistance * 2)
            //        spawn();
            //}
            yield return null;
        }

        private void spawn(ChuteTransportPointScript point,ChuteItemType type)
        {
            if (initialChute == null)
            {
                Debug.LogWarning("Chute not initialized (temp implementation), dropping spawned resource");
                return;
            }

            if (initialChute != this)
            {
                throw new RuntimeException("Can only spawn on the initial chute");
            }
            var item = Instantiate(chuteItemTemplate, transform);
            item.gameObject.SetActive(true);
            item.InitType(type);

            Insert(item, point);
            item.StartY = calculateItemPos(item.Position).y;

        }

        public bool TryInsertItem(ChuteItemScript item, ChuteTransportPointScript point)
        {
            if (!HasFreeSpace(point)) return false;

            Insert(item, point);
            return true;
        }

        public bool HasFreeSpace(ChuteTransportPointScript point)
        {

            var f = itemDistance * 1.2f;
            if (items.Count == 0 && getChuteLength() >= f) return true;

            if (point == startPoint)
            {
                return ((items.First().Position > f));

            }
            else if (point == endPoint)
            {
                return ((items.Last().Position < getChuteLength() - f));

            }
            else
            {
                throw new Exception("Cannot emit on not-chute connected point");
            }
        }
        private void Insert(ChuteItemScript item, ChuteTransportPointScript point)
        {
            if (point == startPoint)
            {
                items.Insert(0, item);
                item.Position = 0;
            }
            else if (point == endPoint)
            {
                items.Add(item);
                item.Position = getChuteLength();

            }
            else
            {
                throw new Exception("Cannot emit on not-chute connected point");
            }
        }

        public void EmitIfFreeSpace(ChuteTransportPointScript outputPoint, ChuteItemType type)
        {
            if (!HasFreeSpace(outputPoint)) return;

            spawn(outputPoint,type);

        }

//        public void ConnectNextChute(ChuteScript chute)
//        {
//            // Prevent loops
//            var curr = chute.nextChute;
//            for (int i = 0; i < 1000; i++)
//            {
//                if (curr == null) break;
//                if (curr == this) throw new Exception("chute loop not allowd!");
//                curr = curr.nextChute;
//            }
//            // No loop, set it
//            nextChute = chute;
//        }


    }
}