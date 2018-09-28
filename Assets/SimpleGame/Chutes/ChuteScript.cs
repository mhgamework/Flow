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
        [SerializeField] private ChuteScript initialChute;

        private float totalChuteLength;

        private List<ChuteItemScript> items;





        public void Start()
        {
            initialChute = this;
            items = new List<ChuteItemScript>();
            chuteItemTemplate.gameObject.SetActive(false);
            this.StartCoroutine(start);
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

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                var acceleration = g;
                var initialVelocity = acceleration * Time.deltaTime;
                var pushAwayForce = -Vector3.Dot(initialVelocity * Vector3.down, calculateItemNormal(item.Position)) * calculateItemNormal(item.Position);
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
                Debug.DrawLine(pos,pos+normal,Color.green);
                Debug.DrawLine(pos,pos+forward*0.1f* item.Direction, Color.blue);
                Debug.DrawLine(pos + forward * 0.1f* item.Direction, pos+forward*(0.1f+speed*0.2f)* item.Direction, new Color(0.5f,0.5f,1));
                item.transform.position =  calculateItemPos(item.Position);
                item.transform.localRotation = Quaternion.AngleAxis(Mathf.Rad2Deg * (item.Position / itemRadius), Vector3.Cross(normal,forward));
            }
        }

        private ChuteItemScript getNextItem(int i)
        {
            if (i + 1 < items.Count) return items[i + 1];
            return null;
        }

        private ChuteItemScript getPrevItem(int i)
        {
            if (i == 0) return null;
            return items[i-1];
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
            item.Position = posmid - itemDistance/2f;
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

            return getLocalChutePos(chute,pos);
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

        private void spawn()
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

            items.Insert(0, item);

            item.Position = 0;
            item.StartY = calculateItemPos(item.Position).y;
        }

        public void EmitIfFreeSpace()
        {
            if (items.Any() && !(items.First().Position > itemDistance * 1.2f)) return;

            spawn();

        }
    }
}