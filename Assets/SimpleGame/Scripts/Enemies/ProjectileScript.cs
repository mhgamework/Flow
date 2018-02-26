using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.SdfModeling;
using Assets.SimpleGame.Voxel;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Scripts.Enemies
{
    public class ProjectileScript : MonoBehaviour
    {
        public float Speed = 5;
        public float ProjectileDamage = 20;
        public float ProjectileExplosionRadius = 3;
        public VoxelRenderingEngineScript VoxelRenderer;


        public void Launch(Vector3 position, Vector3 direction)
        {
            transform.position = position;
            GetComponent<Rigidbody>().position = position;
            GetComponent<Rigidbody>().velocity = direction * Speed;

            gameObject.SetActive(true);
        }

        public void OnCollisionEnter(Collision collision)
        {
            var player = collision.collider.GetComponentInParent<PlayerScript>();
            if (player != null) player.TakeDamage(ProjectileDamage);
            DestroyTerrain();
            Destroy(gameObject);
        }


        public void LaunchAtTargetPredicted(Vector3 start, Vector3 target, Vector3 targetVelocity)
        {
            // Correct target
            target = FirstOrderIntercept(start, Vector3.zero, Speed, target, targetVelocity);

            var dir = (target - start).normalized;

            Debug.DrawLine(start, target, Color.red, 2f);
            Debug.DrawLine(target, target + targetVelocity, Color.blue, 2f);


            Launch(start, dir);
        }
        //first-order intercept using absolute target position
        public static Vector3 FirstOrderIntercept
        (
            Vector3 shooterPosition,
            Vector3 shooterVelocity,
            float shotSpeed,
            Vector3 targetPosition,
            Vector3 targetVelocity
        )
        {
            Vector3 targetRelativePosition = targetPosition - shooterPosition;
            Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
            float t = FirstOrderInterceptTime
            (
                shotSpeed,
                targetRelativePosition,
                targetRelativeVelocity
            );
            return targetPosition + t * (targetRelativeVelocity);
        }
        //first-order intercept using relative target position
        public static float FirstOrderInterceptTime
        (
            float shotSpeed,
            Vector3 targetRelativePosition,
            Vector3 targetRelativeVelocity
        )
        {
            float velocitySquared = targetRelativeVelocity.sqrMagnitude;
            if (velocitySquared < 0.001f)
                return 0f;

            float a = velocitySquared - shotSpeed * shotSpeed;

            //handle similar velocities
            if (Mathf.Abs(a) < 0.001f)
            {
                float t = -targetRelativePosition.sqrMagnitude /
                          (
                              2f * Vector3.Dot
                              (
                                  targetRelativeVelocity,
                                  targetRelativePosition
                              )
                          );
                return Mathf.Max(t, 0f); //don't shoot back in time
            }

            float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
            float c = targetRelativePosition.sqrMagnitude;
            float determinant = b * b - 4f * a * c;

            if (determinant > 0f)
            { //determinant > 0; two intercept paths (most common)
                float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
                if (t1 > 0f)
                {
                    if (t2 > 0f)
                        return Mathf.Min(t1, t2); //both are positive
                    else
                        return t1; //only t1 is positive
                }
                else
                    return Mathf.Max(t2, 0f); //don't shoot back in time
            }
            else if (determinant < 0f) //determinant < 0; no intercept path
                return 0f;
            else //determinant = 0; one intercept path, pretty much never happens
                return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
        }


        private void DestroyTerrain()
        {
            if (VoxelRenderer.GetWorld() == null) return;
            var s = new Ball(transform.position, ProjectileExplosionRadius);

            var b = new Bounds();
            b.SetMinMax((this.transform.position - Vector3.one * ProjectileExplosionRadius), (this.transform.position + Vector3.one * ProjectileExplosionRadius));

            VoxelEditingFunctions.Subtract(VoxelRenderer.GetWorld(), s, b);
        }


    }
}