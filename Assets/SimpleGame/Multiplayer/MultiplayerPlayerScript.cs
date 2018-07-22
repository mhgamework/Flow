using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Assets.SimpleGame.Multiplayer
{
    public class MultiplayerPlayerScript : NetworkBehaviour
    {
        //[SyncVar] public int Score = 0;
        public float PushStrenght = 20;
        public float PushStrenghtY = 3;
        public void Start()
        {
            Debug.Log("Start networked palyer");
            if (!isLocalPlayer)
            {
                GetComponent<FirstPersonController>().enabled = false;
                GetComponentInChildren<Camera>().enabled = false;
            }
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("OnStartLocalPlayer");
            GetComponent<FirstPersonController>().enabled = true;
            GetComponentInChildren<Camera>().enabled = true;
        }


        //public GameObject bulletPrefab;
        //public float bulletSpeed;
        //[Command]
        //void CmdDoFire(float lifeTime)
        //{
        //    GameObject bullet = (GameObject)Instantiate(
        //        bulletPrefab,
        //        transform.position + transform.forward + transform.up,
        //        Quaternion.identity);

        //    var bullet2D = bullet.GetComponent<Rigidbody>();
        //    bullet2D.velocity = transform.forward * bulletSpeed;
        //    Destroy(bullet, lifeTime);

        //    NetworkServer.Spawn(bullet);
        //}

        void Update()
        {
            if (!isLocalPlayer)
                return;

            if (Input.GetMouseButton(0))
            {
                var obj = GameObject.Find("Dummy");

                var pos = transform.position;
                var targetPos = obj.transform.position;

                var dir = (targetPos - pos).normalized * PushStrenght + Vector3.up * PushStrenghtY;

                obj.GetComponent<FirstPersonController>().PushedVelocity += dir * Time.deltaTime;
                //CmdDoFire(3.0f);
            }

        }

    }
}