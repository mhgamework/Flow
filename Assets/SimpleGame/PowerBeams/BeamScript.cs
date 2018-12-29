using System.Collections.Generic;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.PowerBeams
{
    public class BeamScript : MonoBehaviour
    {

        [SerializeField] private float beamGrowSpeed = 5;
        [SerializeField] private float maxLength = 20;
        [SerializeField] private float energyPacketSpeed = 10;

        [SerializeField] private Transform beamModel;
        [SerializeField] private EnergyPacketScript energyPacketTemplate;


        private float currentLength = 0;
        private float desiredLength = 0;
        private RaycastHit? targetHitInfo;

        private Collider currentHittingCollider;
        private IBeamPowerReceiver currentPowerReceiver;

        private List<EnergyPacketScript> energyPackets;

        void Start()
        {
            energyPacketTemplate.gameObject.SetActive(false);
            energyPackets = new List<EnergyPacketScript>();
        }

        public void SetPosition(Vector3 start, Vector3 direction)
        {
            transform.position = start;
            transform.LookAt(start + direction);
        }


        private void updateBeamRaycast()
        {
            RaycastHit hitInfo;

            var result = Physics.Raycast(transform.position, transform.forward, out hitInfo, maxLength,int.MaxValue,QueryTriggerInteraction.Ignore);
            
            if (!result)
            {
                desiredLength = maxLength;
                targetHitInfo = null;
            }
            else
            {
                desiredLength = hitInfo.distance;
                targetHitInfo = hitInfo;
            }
        }

        void updateBeamEnd()
        {
            updateBeamRaycast();

            currentLength = Mathf.Min(currentLength+ Time.deltaTime * beamGrowSpeed, desiredLength);

            beamModel.localScale = beamModel.localScale.ChangeZ(currentLength);

            if (currentLength == desiredLength)
            {
                updateHitTarget(targetHitInfo);
            }
            else
            {
                updateHitTarget(null);
            }
        }

        private void updateHitTarget(RaycastHit? target)
        {
            var newCollider = target.HasValue ? target.Value.collider : null;
            if (currentHittingCollider == newCollider) return; // Same collider, do nothing
            currentHittingCollider = newCollider;

            var newReceiver = extractPowerReceiver(newCollider);
            if (newReceiver == currentPowerReceiver) return;
            // Different receiver!
            disengageCurrentReceiver();
            engageNewReceiver(newReceiver);

        }

        private IBeamPowerReceiver extractPowerReceiver(Collider newCollider)
        {
            if (newCollider == null) return null;
            return newCollider.GetComponentInParent<IBeamPowerReceiver>();
        }

        private void engageNewReceiver(IBeamPowerReceiver newReceiver)
        {
            currentPowerReceiver = newReceiver;
            if (newReceiver == null) return;
            newReceiver.OnBeamHit(this);

        }

        private void disengageCurrentReceiver()
        {
            if (currentPowerReceiver == null) return;
            
            currentPowerReceiver.OnBeamRemoved(this);
            currentPowerReceiver = null;

        }


        public void Update()
        {
            updateBeamEnd();
            UpdateEnergyPacketPositions();
        }

        public Vector3 GetBeamDirection()
        {
            return transform.forward;
        }

        public Vector3 GetEndHitPoint()
        {
            //TODO: if not at desired length calling this method should not be allowed!
            return transform.position + transform.forward * currentLength;
        }

        public void Disable()
        {
            disengageCurrentReceiver();
            currentHittingCollider = null;
            // Drop all packets
            for (int i = 0; i < energyPackets.Count; i++)
            {
                Destroy(energyPackets[i].gameObject);
            }
            energyPackets.Clear();
        }

        public void EmitPacket()
        {
            var packet = Instantiate(energyPacketTemplate,transform);
            packet.gameObject.SetActive(true);
            energyPackets.Add(packet);
        }

        public void UpdateEnergyPacketPositions()
        {
            for (int i = 0; i < energyPackets.Count; i++)
            {
                // packets that are (behind) the current target are dropped
                // This can happen when the beam moves and hits something thats closer than its current length
                // this is performed before moving the packets, so we know which packets are behind the target vs moved "into" the target
                var packet = energyPackets[i];
                if (packet.pos > currentLength)
                {
                    //Drop packet
                    Destroy(packet.gameObject);
                    energyPackets.RemoveAt(i);
                    i--;
                    continue;
                }


                packet.pos += energyPacketSpeed * Time.deltaTime;
                packet.transform.position = transform.position + transform.forward * packet.pos;
                if (packet.pos > currentLength)
                {
                    pushPacketToTarget();
                    Destroy(packet.gameObject);
                    energyPackets.RemoveAt(i);
                    i--;
                    continue;

                }

            }
        }

        private void pushPacketToTarget()
        {
            if (currentPowerReceiver == null)
            {
                // Drop packet. TODO: Explode :)?
                return;
            }

            currentPowerReceiver.ReceivePacket(this);
        }
    }
}