using Assets.UnityAdditions;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.SimpleGame.Multiplayer.Players
{
    /// <summary>
    /// This is a workaround in unity to make the part that handles player movement plug-and-playable and modular
    /// THis prefabs structure will be overlaid on the player gameobject and the player gameobject's model
    /// </summary>
    public class PlayerMovementPartPrefabScript : MonoBehaviour
    {

        public void ApplyToPlayerGameObject(GameObject playerGameObject, Transform headForCamera)
        {
            // Dirty copying/hacking
            var netTrans = playerGameObject.AddComponent<NetworkTransform>();
            var child = playerGameObject.AddComponent<NetworkTransformChild>();

            netTrans.sendInterval = GetComponent<NetworkTransform>().sendInterval;
            netTrans.transformSyncMode = GetComponent<NetworkTransform>().transformSyncMode;
            netTrans.snapThreshold = GetComponent<NetworkTransform>().snapThreshold;
            netTrans.movementTheshold = GetComponent<NetworkTransform>().movementTheshold;
            netTrans.interpolateMovement = GetComponent<NetworkTransform>().interpolateMovement;
            netTrans.syncRotationAxis = GetComponent<NetworkTransform>().syncRotationAxis;
            netTrans.interpolateRotation = GetComponent<NetworkTransform>().interpolateRotation;
            netTrans.rotationSyncCompression = GetComponent<NetworkTransform>().rotationSyncCompression;

            child.target = headForCamera;
            child.movementThreshold = GetComponent<NetworkTransformChild>().movementThreshold;
            child.interpolateMovement = GetComponent<NetworkTransformChild>().interpolateMovement;
            child.interpolateRotation = GetComponent<NetworkTransformChild>().interpolateRotation;
            child.syncRotationAxis = GetComponent<NetworkTransformChild>().syncRotationAxis;
            child.rotationSyncCompression = GetComponent<NetworkTransformChild>().rotationSyncCompression;


            var audioSource = playerGameObject.AddComponent<AudioSource>();
            var characterControlelr = playerGameObject.AddComponent<CharacterController>();
            var controller = playerGameObject.AddComponent<FirstPersonController>();

            //TODO: probably easy and better to add camera as a child object of the head
            var cam = headForCamera.gameObject.AddComponent<Camera>();
            var audiolistener = headForCamera.gameObject.AddComponent<AudioListener>();

            //            var body = playerGameObject.AddComponent<Rigidbody>();
            //            body.mass = GetComponent<Rigidbody>().mass;
            //            body.drag = GetComponent<Rigidbody>().drag;
            //            body.angularDrag = GetComponent<Rigidbody>().angularDrag;
            //            body.useGravity = GetComponent<Rigidbody>().useGravity;
            //            body.isKinematic = GetComponent<Rigidbody>().isKinematic;
            //            body.interpolation = GetComponent<Rigidbody>().interpolation;
            //            body.collisionDetectionMode = GetComponent<Rigidbody>().collisionDetectionMode;
            //            body.constraints = GetComponent<Rigidbody>().constraints;

        }
    }
}