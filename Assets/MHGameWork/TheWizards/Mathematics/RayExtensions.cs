using UnityEngine;

namespace Assets.TheWizards.Mathematics
{
    public static class RayExtensions
    {
        public static Ray Transform(this Ray ray, Matrix4x4 transform)
        {
            return new Ray(transform.MultiplyPoint(ray.origin), transform.MultiplyVector(ray.direction));
        }
    }
}