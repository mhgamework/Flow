using UnityEngine;

namespace Assets.SimpleGame.PowerBeams
{
    public class MirrorDiscScript : MonoBehaviour, IBeamPowerReceiver
    {
        [SerializeField] private BeamScript beamPrefab;
        [SerializeField] private float userRotateSpeed = 20;

        private BeamScript currentIncomingBeam;
        private BeamScript outgoingBeam;

        private float rotY;
        private float rotX;

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

            transform.localRotation = Quaternion.Euler(rotX, rotY, 0);
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

        public void PushRotate(Vector3 resultPosition, Vector3 resultNormal, int direction)
        {
            var y =Vector3.Dot(transform.up, (resultPosition - transform.position).normalized);
            var x =Vector3.Dot(transform.right, (resultPosition - transform.position).normalized);
            if (Mathf.Abs(y) > Mathf.Abs(x))
            {
                var partDir = -Mathf.Sign(Vector3.Dot(Vector3.Cross((resultPosition - transform.position).normalized, resultNormal), transform.right));

                // Rotate x
                rotX += (direction * Time.deltaTime * userRotateSpeed * partDir);
            }
            else
            {
                var partDir = -Mathf.Sign(Vector3.Dot(Vector3.Cross((resultPosition-transform.position).normalized, resultNormal), transform.up));

                rotY += (direction * Time.deltaTime * userRotateSpeed * partDir);

            }
        }
    }
}