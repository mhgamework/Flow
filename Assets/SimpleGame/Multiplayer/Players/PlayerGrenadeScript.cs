using UnityEngine;
using UnityEngine.Networking;

namespace Assets.SimpleGame.Multiplayer.Players
{
    public class PlayerGrenadeScript : NetworkBehaviour
    {
        private bool fireGrenadeDown;


        private bool charging = false;
        private float chargeTime = -1;

        public float MinGrenadeSpeed = 1;
        public float MaxGrenadeSpeed = 10;
        public float TimeToMaxCharge = 3;
        public float StartOffset = 1;
        public BombScript GrenadePrefab;

        private Transform playerTransform;

        public void SetPlayerTransform(Transform playerTransform)
        {
            this.playerTransform = playerTransform;

        }

        [ClientCallback]
        public void Update()
        {
            if (fireGrenadeDown)
            {
                if (!charging)
                {
                    charging = true;
                    chargeTime = Time.realtimeSinceStartup;
                }
            }
            else
            {
                if (charging)
                {
                    charging = false;
                    CmdFireGrenade(playerTransform.forward, calculateFireSpeed(Time.realtimeSinceStartup - chargeTime));
                }
            }
        }

        [Command]
        private void CmdFireGrenade(Vector3 dir, float speed)
        {
            var grenade = Instantiate(GrenadePrefab, playerTransform.position + dir * StartOffset,Quaternion.identity);
            grenade.GetComponent<Rigidbody>().velocity = dir * speed;
            grenade.Arm();
            grenade.DetonateOnImpact = true;
            NetworkServer.Spawn(grenade.gameObject);

        }

        private float calculateFireSpeed(float timeInSeconds)
        {
            return Mathf.Lerp(MinGrenadeSpeed, MaxGrenadeSpeed, Mathf.Clamp01(timeInSeconds / TimeToMaxCharge));
        }



        public void SetFireGrenadeDown(bool fireGrenadeDown)
        {
            this.fireGrenadeDown = fireGrenadeDown;
        }



    }
}