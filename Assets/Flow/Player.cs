using System;
using System.Linq;
using MHGameWork.TheWizards.DualContouring;
using UnityEngine;

namespace Assets.Flow
{
    public class Player : MonoBehaviour
    {
        public float FireMagic;
        public float WaterMagic;
        public float Clay;

        public float MaxMagic = 10;
        public float MaxClay = 3;

        private IInteractable currentTarget = null;
        private Collider currentTargetCollider = null;
        public float MagicExtractionSpeed = 1;
        public float ClayExtractionSpeed = 1f / 3;

        public MinerGoblin MinerGoblin;
        public Storage Storage;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                tryPlaceMinerGoblin();

            }
            /*else if (Input.GetKeyDown(KeyCode.Alpha2))
                tryPlaceStorage();*/
            /*else if (Input.GetKeyDown(KeyCode.Alpha2))
                Place*/

            if (Camera.current == null) return;
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f))
            {
                IInteractable interactable = hit.collider.GetComponent(typeof(IInteractable)) as IInteractable;
                if (interactable == null)
                    clearRaycast(hit.collider);
                else
                    updateRaycast(hit.collider, interactable);

                if (currentTarget != null)
                    currentTarget.TryInteract(this, hit.point);
            }
      
        }

        //private void tryPlaceStorage()
        //{
        //    if (Clay < 1) return;
        //    Clay--;

        //    Vector3 pos;
        //    if (!getTargetedPlacePosition(out pos)) return;

           
        //}

        private void tryPlaceMinerGoblin()
        {
            if (Clay < 1) return;

            Vector3 pos ;
            if (!getTargetedPlacePosition(out pos)) return;

            Clay--;


            var storage = Instantiate(Storage);

            storage.transform.position = pos;


            var goblin = Instantiate(MinerGoblin);

            goblin.transform.position = pos;
            goblin.Storage = storage;


        }

        private bool getTargetedPlacePosition(out Vector3 pos)
        {
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            var ret = Physics.Raycast(ray, out hit, 10f);
            pos = new Vector3(0,0,0);
            if (ret)
                pos = hit.point;

            return ret;
        }


        private void updateRaycast(Collider collider, IInteractable interactable)
        {
            if (currentTarget == interactable) return;
            clearRaycast(collider);
            interactable.OnFocus(this);
            collider.GetComponents(typeof(ISecondaryInteractable)).Cast<ISecondaryInteractable>().ForEach(i => i.OnFocus(this));

            currentTarget = interactable;
            currentTargetCollider = collider;
            Debug.Log("Interacting with " + interactable);
        }

        private void clearRaycast(Collider collider)
        {
            if (currentTarget == null) return;
            Debug.Log("Stopped interacting with: " + currentTarget);
            currentTarget.OnUnfocus(this);
            currentTargetCollider.GetComponents(typeof(ISecondaryInteractable)).Cast<ISecondaryInteractable>().ForEach(i => i.OnUnfocus(this));
            currentTarget = null;
            currentTargetCollider = null;


        }
    }
}