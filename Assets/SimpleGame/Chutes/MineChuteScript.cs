using System.Collections.Generic;
using Assets.UnityAdditions;
using UnityEngine;

namespace Assets.SimpleGame.Chutes
{
    public class MineChuteScript : MonoBehaviour
    {
        [SerializeField] private float productionInterval = 1;
        [SerializeField] private ChuteScript outputChute;

        public void Start()
        {
            this.StartCoroutine(mineRoutine);
        }

        public void Update()
        {

        }

        IEnumerable<YieldInstruction> mineRoutine()
        {
            yield return null;
            for (; ; )
            {
                outputChute.EmitIfFreeSpace();
                yield return new WaitForSeconds(productionInterval);
            }
        }
    }
}