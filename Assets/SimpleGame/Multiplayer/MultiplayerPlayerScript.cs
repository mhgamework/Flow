using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Assets.SimpleGame.Multiplayer
{
    public class MultiplayerPlayerScript : NetworkBehaviour
    {
        //[SyncVar] public int Score = 0;

        private PushSpellScript pushSpellScript;
        public PushSpellScript PushSpellScriptPrefab;

        [SyncVar] private bool casting;

        public void Start()
        {
            Debug.Log("Start networked palyer");
            if (!isLocalPlayer)
            {
                GetComponent<FirstPersonController>().enabled = false;
                GetComponentInChildren<Camera>().enabled = false;
            }


            pushSpellScript = Instantiate(PushSpellScriptPrefab, transform.GetComponentsInChildren<Transform>().First(n => n.name == "Head"));
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

        [Command]
        void CmdStartCast()
        {
            casting = true;

        }
        [Command]
        void CmdStopCast()
        {
            casting = false;

        }

        void Update()
        {
            if (casting) pushSpellScript.Cast();

            if (!isLocalPlayer)
                return;

            if (Input.GetMouseButton(0))
            {
                CmdStartCast();
                //CmdPushSpell();
                //CmdDoFire(3.0f);
            }
            else
            {
                CmdStopCast();
            }


        }

        [ClientRpc]
        public void RpcPush(Vector3 push)
        {
            GetComponent<FirstPersonController>().PushedVelocity += push;
        }
        public void ApplyPushSpell(Vector3 dir)
        {
            if (!isServer) return;
            RpcPush(dir * Time.deltaTime);
        }

        public void OnFallOfWorld()
        {
            transform.position = new Vector3();
            GetComponent<FirstPersonController>().PushedVelocity  = new Vector3();

        }
    }
}