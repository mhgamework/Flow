using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Flow
{
    public class MinerGoblin : MonoBehaviour, IInteractable
    {
        public TextPopup TextPopup;

        public float WaterMagic = 0;
        public float WaterMagicMax = 10;
        public float LifeTime = 10;
        public float MoveSpeed = 1;
        private Coroutine coroutine;
        public string ResourceType;
        public int ResourceAmount;

        public float MineDuration = 3;

        public float DropoffDuration = 1;

        public Storage Storage;

        public void Start()
        {
        }

        public void Update()
        {
            WaterMagic = Mathf.Max(0, WaterMagic - Time.deltaTime * (WaterMagicMax / LifeTime));
            if (WaterMagic <= 0 && coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            if (WaterMagic > 0 && coroutine == null)
                coroutine = StartCoroutine(UpdateState().GetEnumerator());

        }

        public void OnUnfocus(Player p)
        {

        }

        public void OnFocus(Player p)
        {
        }


        public IEnumerable<YieldInstruction> UpdateState()
        {
            for (;;)
            {
                if (ResourceAmount <= 0)
                {
                    var resource = findNearestResource();
                    if (resource == null) { yield return new WaitForSeconds(0.5f); continue; }

                    while (!moveToPoint(resource.transform.position))
                        yield return new WaitForSeconds(0);

                    yield return new WaitForSeconds(MineDuration);
                    ResourceType = resource.Type;
                    ResourceAmount += 1;
                }
                else
                {
                    while (!moveToPoint(Storage.transform.position))
                        yield return new WaitForSeconds(0);

                    yield return new WaitForSeconds(DropoffDuration);
                    Storage.AddResources(ResourceType, 1);
                    ResourceAmount -= 1;
                }

            }


        }

        private bool moveToPoint(Vector3 position)
        {
            var diff = position - transform.position;
            var elapsed = Mathf.Min(Time.deltaTime * MoveSpeed, diff.magnitude);

            transform.position += diff.normalized * elapsed;

            return diff.magnitude < 0.01;
        }

        private MagicCrystals findNearestResource()
        {
            return FindObjectsOfType<MagicCrystals>().Where(c => c.Type != "clay").OrderBy(c => Vector3.Distance(c.transform.position, transform.position)).FirstOrDefault();
        }


        public void TryInteract(Player p, Vector3 point)
        {
            TextPopup.SetText("Water: " + WaterMagic);
            if (Input.GetMouseButton(0))
            {
                var delta = Mathf.Min(Time.deltaTime, p.WaterMagic);
                p.WaterMagic -= delta;
                WaterMagic += delta;

            }
        }
    }
}