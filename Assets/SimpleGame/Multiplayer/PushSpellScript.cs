﻿using Assets.UnityAdditions;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame.Multiplayer
{
    public class PushSpellScript : MonoBehaviour
    {
        private ParticleSystem chargeParticleSystem;
        public float PushStrenght = 20;
        public float PushStrenghtY = 3;
        public void Start()
        {
            chargeParticleSystem = transform.GetComponentInChildren<ParticleSystem>("Charge");
        }

        private int CastFrames = 0;

        public void Update()
        {
            CastFrames = Mathf.Max(0, CastFrames - 1);

            if (chargeParticleSystem.isEmitting && CastFrames == 0)
                //if (CastFrames == 0)
                chargeParticleSystem.Stop();
            if (!chargeParticleSystem.isEmitting && CastFrames > 0)
                chargeParticleSystem.Play();


        }

        public void Cast()
        {
            CastFrames = 5;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (CastFrames == 0) return;
            var fps = other.GetComponent<MultiplayerPlayerScript>();
            if (fps == null) return;


            var dir = (fps.transform.position - transform.position).normalized * PushStrenght + Vector3.up * PushStrenghtY;

            fps.ApplyPushSpell(dir);
        }

        private void OnTriggerExit(Collider other)
        {

        }



    }
}