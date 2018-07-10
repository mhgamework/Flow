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
    public class NetworkedPlayer : NetworkBehaviour
    {
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

    }
}
