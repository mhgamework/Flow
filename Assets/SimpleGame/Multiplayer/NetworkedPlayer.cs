using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.ThirdPerson;
using Object = System.Object;

namespace Assets.SimpleGame.Multiplayer
{
    [Obsolete]
    public class NetworkedPlayer : NetworkBehaviour
    {
        [SyncVar] public int Score = 0;

        public void Start()
        {
            Debug.Log("Start networked palyer");
        }
        public override void OnStartLocalPlayer()
        {
            Debug.Log("OnStartLocalPlayer");

            GetComponent<ThirdPersonUserControl>().enabled = true;
            //GetComponent<ThirdPersonCharacter>().enabled = true;
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
        }


        public GameObject bulletPrefab;
        public float bulletSpeed;
        [Command]
        void CmdDoFire(float lifeTime)
        {
            GameObject bullet = (GameObject)Instantiate(
                bulletPrefab,
                transform.position + transform.forward+transform.up,
                Quaternion.identity);

            var bullet2D = bullet.GetComponent<Rigidbody>();
            bullet2D.velocity = transform.forward * bulletSpeed;
            Destroy(bullet, lifeTime);

            NetworkServer.Spawn(bullet);
        }

        void Update()
        {
            if (!isLocalPlayer)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                CmdDoFire(3.0f);
            }

        }
    }
}
