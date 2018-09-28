using System;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.BuilderSystem
{
    public class CubeBuildableScript : BaseBuilderInteractable
    {

        public override void OnBuilt(Vector3 position, Vector3 normal, Vector3 lookDir)
        {
            var gridSize = 1;

            {
                var max = normal.MaxComponent();
                var absN = new Vector3(Mathf.Abs(normal.x), Mathf.Abs(normal.y), Mathf.Abs(normal.z));
                if (absN.x > absN.y && absN.x > absN.z)
                    normal = new Vector3(Mathf.Sign(normal.x), 0, 0);
                else if (absN.y > absN.z)
                    normal = new Vector3(0, Mathf.Sign(normal.y), 0);
                else
                    normal = new Vector3(0, 0, Mathf.Sign(normal.z));

            }

            if (normal.magnitude < 0.01) throw new Exception("Should no be possible, normal should be at least sqrt(2)");
            normal.Normalize();
            Debug.DrawLine(position, position + normal, Color.blue, 3);

            position = (position + normal * 0.01f).ToFloored().ToVector3();
            position = position + Vector3.one * 0.5f - normal * 0.5f;

            transform.position = position;
            transform.up = normal;





        }
    }
}