using System;
using System.Linq;
using Assets.SimpleGame.Multiplayer.Players;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Component for on the player script that implements MP pushing spell
    /// </summary>
    public class PlayerPushScript : NetworkBehaviour
    {
        private PushSpellScript pushSpellScript;
        public PushSpellScript PushSpellScriptPrefab;

        [SyncVar] private bool casting;


        private bool init = false;
        public void Start()
        {
            initialize();
        }

        public void initialize()
        {
            if (init) return;
            init = true;

            var model = GetComponent<PlayerModelScript>();
            model.initialize();
            var head = model.GetHead();
            pushSpellScript = Instantiate(PushSpellScriptPrefab, head);
        }

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
            if (!init) return;
            if (casting) pushSpellScript.Cast();
        }


        public void DoStartCastPushSpell()
        {
            CmdStartCast();

        }

        public void DoStopCastPushSpell()
        {
            CmdStopCast();
        }

        public void ResetOnPlayerDeath()
        {
            GetComponent<FirstPersonController>().PushedVelocity = new Vector3();
        }
    }
}