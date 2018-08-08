using System.Linq;
using Assets.SimpleGame.Multiplayer.Players;
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

        private PlayerGrenadeScript playerGrenadeScript;

        private Vector3 spawnLocation;

        [SyncVar]
        private Color playerColor;

        public void Start()
        {
            if (isServer)
                SetPlayerColor(MultiplayerGameManager.Instance.GetFreePlayerColor());
            else
                setModelColor(playerColor);
            var head = transform.GetComponentsInChildren<Transform>().First(n => n.name == "Head");

            playerGrenadeScript = GetComponent<PlayerGrenadeScript>();
            playerGrenadeScript.SetPlayerTransform(head);

            Debug.Log("Start networked palyer");
            if (!isLocalPlayer)
            {
                GetComponent<FirstPersonController>().enabled = false;
                GetComponentInChildren<Camera>().enabled = false;
            }


            pushSpellScript = Instantiate(PushSpellScriptPrefab, head);
            spawnLocation = transform.position;
        }

        private void SetModelColor(Transform capsule, Color color)
        {
            capsule.GetComponent<MeshRenderer>().material.color = color;
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


            playerGrenadeScript.SetFireGrenadeDown(Input.GetMouseButton(1));


            checkOutOfMap();
        }

        private void checkOutOfMap()
        {
            if (transform.position.y < -100)
            {
                OnFallOfWorld();
            }
        }

        [ClientRpc]
        public void RpcPush(Vector3 push)
        {
            GetComponent<FirstPersonController>().PushedVelocity += push;
        }

        public void ApplyPushAway(Vector3 pushOrigin, float strength, float strenghtY, float amount)
        {
            if (!isServer) return;

            var dir = (transform.position - pushOrigin).normalized * strength + Vector3.up * strenghtY;
            RpcPush(dir * amount);
        }

        public void OnFallOfWorld()
        {
            if (!isLocalPlayer) return;
            transform.position = spawnLocation;
            GetComponent<FirstPersonController>().PushedVelocity = new Vector3();

        }

        public Color GetPlayerColor()
        {
            return playerColor;
        }

        public void SetPlayerColor(Color color)
        {
            playerColor = color;
            RpcSetPlayerColor(color);
        }

        [ClientRpc]
        public void RpcSetPlayerColor(Color color)
        {
            playerColor = color;
            setModelColor(color);
        }

        private void setModelColor(Color color)
        {
            var capsule = transform.GetComponentsInChildren<Transform>().First(n => n.name == "Capsule");
            SetModelColor(capsule, color);
        }
    }
}