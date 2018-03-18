using Assets.MarchingCubes.SdfModeling;
using UnityEngine;

namespace Assets.MarchingCubes.Scenes
{
    public class SDFRenderingTestScript : MonoBehaviour
    {
        public void Update()
        {

        }

        private void CreateScene()
        {
            createSDFPrimitives(new Vector3(0,0,0));

            createPerlinNoise(new Vector3(50,0,0) );

            createSDFWithNoise(new Vector3(100,0,0));

            createLocalityPrincipleDemo(new Vector3(150, 0, 0));


        }

        private void createSDFPrimitives(Vector3 vector3)
        {
            // Imperative
            var sphere = new Ball(vector3, 5);

            unionWithWorld(sphere);

        }

        private void createLocalityPrincipleDemo(Vector3 vector3)
        {
        }

        private void createSDFWithNoise(Vector3 vector3)
        {
        }

        private void createPerlinNoise(Vector3 vector3)
        {
        }

      
    }
}