using Assets.MarchingCubes.Domain;
using Assets.MarchingCubes.VoxelWorldMVP;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.Flow.FireSpirit
{
    public class FireSpiritScript : MonoBehaviour
    {
        public Material unityMaterial;
        private VoxelMaterial material = new VoxelMaterial { color = Color.red };

        public float SampleInterval = 2f;
        public float Size = 3f;
        public int MovementRadius = 5;
        public float MovementSpeed = 0.5f;
        private DynamicVoxelEntity dynamicVoxelEntity;

        public void Start()
        {
            var dynamicRenderingService = DynamicRenderingService.Instance;
            // Must do this first
            dynamicRenderingService.AddMaterial(Color.red, unityMaterial);

            dynamicVoxelEntity = dynamicRenderingService.CreateEntity(sdf, Size, pos(), SampleInterval);

        }

        public void Update()
        {
            dynamicVoxelEntity.Center = pos();
            dynamicVoxelEntity.SampleInterval = SampleInterval;
            dynamicVoxelEntity.Size = Size;

        }

     

        private VoxelData sdf(Vector3 point, float samplingDistance)
        {
            var dista = (pos() - point).magnitude - Size*0.7f;
            var distb = ((pos() + new Vector3(0, Size * 0.7f, 0)) - point).magnitude - Size * 0.3f;
            var dist = Mathf.Min(dista, distb);

            return new VoxelData(dist, material);
        }

        private Vector3 pos()
        {
            return transform.position + new Vector3(Mathf.Cos(Time.timeSinceLevelLoad * MovementSpeed), Mathf.Sin(Time.timeSinceLevelLoad * MovementSpeed), 0) * MovementRadius;
            return new Vector3(Mathf.Cos(Time.timeSinceLevelLoad * MovementSpeed), Mathf.Sin(Time.timeSinceLevelLoad * MovementSpeed), 0) * MovementRadius;
        }

    }
}