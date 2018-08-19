using System;
using System.Linq;
using UnityEngine;

namespace Assets.SimpleGame.Multiplayer.Players
{
    /// <summary>
    /// Component on player that loads in a player model from a prefab and adds it to the player's game object
    /// Also holds the camera
    /// </summary>
    public class PlayerModelScript : MonoBehaviour
    {
        public Transform PlayerModelPrefab;

        private Transform playerModel;

        public void Start()
        {
        }

        public void initialize()
        {
            if (playerModel != null) return;
            playerModel = Instantiate(PlayerModelPrefab, transform);
        }
        public Transform GetHead()
        {
            return transform.GetComponentsInChildren<Transform>().First(n => n.name == "Head");
        }

        public void SetColorOfPlayerModel(Color color)
        {
            var capsule = transform.GetComponentsInChildren<Transform>().First(n => n.name == "Capsule");
            changeUnityModelColor(capsule, color);
        }
        private void changeUnityModelColor(Transform capsule, Color color)
        {
            capsule.GetComponent<MeshRenderer>().material.color = color;
        }



    }
}