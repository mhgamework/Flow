using UnityEngine;

namespace Assets.SimpleGame.PowerBeams
{
    public class MirrorDiscScript : MonoBehaviour, IBeamPowerReceiver
    {
        [SerializeField] private BeamScript beamPrefab;

        private BeamScript currentIncomingBeam;
        private BeamScript outgoingBeam;

        public void Start()
        {
            outgoingBeam = Instantiate(beamPrefab, transform);
        }

        public void SetForwardDirection(Vector3 forward)
        {

        }

        public void Update()
        {
            if (currentIncomingBeam != null)
            {
                if (!outgoingBeam.gameObject.activeSelf)
                    outgoingBeam.gameObject.SetActive(true);

                var incomingDir = currentIncomingBeam.GetBeamDirection();
                var normal = transform.forward;
                var hitPoint = currentIncomingBeam.GetEndHitPoint();
                var outGoingDir = Vector3.Dot(normal, -incomingDir) * normal * 2 + incomingDir;

                outgoingBeam.SetPosition(hitPoint, outGoingDir);
            }
            else
            {
                outgoingBeam.Disable();
                if (outgoingBeam.gameObject.activeSelf)
                    outgoingBeam.gameObject.SetActive(false);
            }
        }

        public void OnBeamRemoved(BeamScript beamScript)
        {
            if (currentIncomingBeam != beamScript) return;// Only accept one beam
            currentIncomingBeam = null;
        }

        public void OnBeamHit(BeamScript beamScript)
        {
            if (currentIncomingBeam != null) return;// Only accept one beam
            currentIncomingBeam = beamScript;
        }

        public void ReceivePacket(BeamScript beamScript)
        {
            if (currentIncomingBeam != beamScript) return; // Ignore other beams
            outgoingBeam.EmitPacket();

        }
    }
}