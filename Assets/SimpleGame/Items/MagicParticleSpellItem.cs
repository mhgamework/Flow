using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.Wards;
using UnityEngine;

namespace Assets.SimpleGame.Items
{
    [CreateAssetMenu(menuName ="Flow/Items/MagicParticleSpell")]
    public class MagicParticleSpellItem : ScriptableObject
    {
        public float MaxCenteringDistance = 20;

        public Vector3 ArmsOffset;
        public Vector3 CenterOffset;
        public MagicProjectileScript ProjectilePrefab;

        public void UpdateTool(PlayerScript player)
        {
            if (Input.GetMouseButtonDown(0) && player.GetNumItems("magicprojectile") > 0)
            {
                player.TakeItems("magicprojectile", 1);
                fireParticle(CenterOffset - ArmsOffset,player);
            }

            if (Input.GetMouseButtonDown(1) && player.GetNumItems("magicprojectile") > 0)
            {
                player.TakeItems("magicprojectile", 1);
                fireParticle(CenterOffset + ArmsOffset, player);

            }
        }

        private void fireParticle(Vector3 start, PlayerScript player)
        {
        

            start = player.GetCameraTransform().rotation * start;
            var worldStart = player.GetPlayerPosition() + start;

            var projectile = Object.Instantiate(ProjectilePrefab);
            var dir= player.GetCameraTransform().forward;

            RaycastHit hit;
            var ray = new Ray(player.GetPlayerPosition() + CenterOffset,player.GetCameraTransform().forward);
            if (Physics.Raycast(ray, out hit, MaxCenteringDistance))
            {
                dir = (hit.point - ray.origin).normalized;
            }
            projectile.Fire(worldStart,dir);

        }
    }
}