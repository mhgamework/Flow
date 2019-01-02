using Assets.MHGameWork.FlowEngine._Cleanup;
using Assets.MHGameWork.FlowGame.DI;
using UnityEngine;

namespace Assets.SimpleGame.Scripts.Enemies
{
    /**
     * Disable gameobject until voxel engine ready
     */
    public class SleepUntilVoxelsLoadedScript : MonoBehaviour
    {
        public void Start()
        {
            SleepUntilVoxelsLoadedSingleton.Instance.Register(this);
            gameObject.SetActive(false);
        }

        public bool TryWakeup()
        {
            if (!FlowGameServiceProvider.Instance.GetService<VoxelRenderingEngineScript>()
                .IsChunkLoadedAt(transform.position)) return false;

            gameObject.SetActive(true);
            enabled = false;
            return true;
        }
    }
}