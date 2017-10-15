using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gameplay;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.MarchingCubes
{
    public class PlayerMovementScript : MonoBehaviour
    {
        public float StartAfter;
        public float Acceleration;
        public float Deceleration;
        public float MinDecelerationVelocity;

        private float lastPressed = float.MinValue;
        private Rigidbody rigidbody;
        private CharacterController characterController;
        private FirstPersonController firstPersonController;

        public bool PlayerMagicModeEnabled = false;

        private LocalPlayerSingleton localPlayer;

        public void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            characterController = GetComponent<CharacterController>();
            firstPersonController = GetComponent<FirstPersonController>();
            if (PlayerMagicModeEnabled)
                localPlayer = LocalPlayerSingleton.Instance;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                lastPressed = Time.timeSinceLevelLoad;
            }
            if (Input.GetKey(KeyCode.Space) && Time.timeSinceLevelLoad - lastPressed > StartAfter)
                if (!PlayerMagicModeEnabled || localPlayer.TryUseFlyMagic())

                {
                    firstPersonController.AddMoveDir(Vector3.up * Acceleration * Time.deltaTime);
                    //rigidbody.velocity += Vector3.up * Acceleration * Time.deltaTime;
                }

            if (!Input.GetKey(KeyCode.Space) && firstPersonController.GetVelocity().y > MinDecelerationVelocity)
            {
                firstPersonController.AddMoveDir(-Vector3.up * Deceleration * Time.deltaTime);
            }
        }
    }
}
