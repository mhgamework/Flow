using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.World;
using UnityEngine;

namespace Assets.MarchingCubes.Scenes
{
    public class SDFRenderingTestScript : MonoBehaviour
    {
        public VoxelRenderingEngineScript VoxelRenderingEngine;


        private IEditableVoxelWorld world;
        private SDFWorldEditingService editingService;




        void OnEnable()
        {
            Debug.Log("Enabling");
            world = VoxelRenderingEngine.GetWorld();
            editingService = new SDFWorldEditingService();
            CreateScene();
        }

        public void Update()
        {

        }

        private void CreateScene()
        {
            //createSDFPrimitives(new Vector3(0, 0, 0));

            createPerlinNoise(new Vector3(0, 0, 0));

            createSDFWithNoise(new Vector3(100, 0, 0));

            createLocalityPrincipleDemo(new Vector3(150, 0, 0));


        }

        private void createSDFPrimitives(Vector3 vector3)
        {
            // Imperative

            var s2 = new Ball(vector3 + new Vector3(0, 0, 0), 5);

            editingService.AddSDFObject(world, s2, new Bounds(vector3 + new Vector3(0, 0, 0), new Vector3(1, 1, 1) * 10), new VoxelMaterial(Color.red), 20);


            for (int i = 0; i < 5; i++)
            {
                var sphere = new Ball(vector3 + new Vector3(30, 30, 30)*i, 20);

                editingService.AddSDFObject(world, sphere, new Bounds(vector3 + new Vector3(30, 30, 30)*i, new Vector3(1, 1, 1) * 20 * 2 * 1.5f), new VoxelMaterial(Color.red), 20);

            }

        }
        private void createPerlinNoise(Vector3 vector3)
        {

        }


        private void createLocalityPrincipleDemo(Vector3 vector3)
        {
        }

        private void createSDFWithNoise(Vector3 vector3)
        {
        }

       

    }
}