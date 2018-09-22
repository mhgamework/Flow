using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.UnityAdditions;

namespace Assets.SimpleGame.PowerBeams
{
    public class PowerTapScript : MonoBehaviour
    {
        public float EmissionInterval = 3;
        public Vector3 EmitPoint;
        public BeamScript BeamPrefab;

        private BeamScript beam;

        public void Start()
        {
            beam = Instantiate(BeamPrefab,transform);
            beam.SetPosition(transform.position,Vector3.up);
            
            this.StartCoroutine(startEmit);
        }

        public IEnumerable<YieldInstruction> startEmit()
        {
            for (;;)
            {
                yield return new WaitForSeconds(EmissionInterval);
                emit();
            }
        }

        private void emit()
        {
            beam.EmitPacket();
        }
    }
}