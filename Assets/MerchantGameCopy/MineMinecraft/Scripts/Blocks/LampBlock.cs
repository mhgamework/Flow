using Assets.MineMinecraft.Blocks;
using Assets.MineMinecraft.DummyWorldImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MineMinecraft.Scripts.Blocks
{
    class LampBlock : UnityBlock
    {
        private Boolean lightOn = true;
        public Material materialOff;
        public Material materialOn;
        public Transform modelPrefab;

        public LampBlock()
        {
        }


        private void switchLight()
        {
            var world = DummyWorldManager.Instance().World;
            var meshRender = world.getBlockTransform(this).GetComponent<MeshRenderer>();
            var light = world.getBlockTransform(this).GetComponentInChildren<Light>();
            if (lightOn)
            {
                meshRender.material = materialOff; 
                light.enabled = false;
            }
            else
            {
                meshRender.material = materialOn; 
                light.enabled = true;
            }
            lightOn = !lightOn;
        }

        public override Transform GetModel()
        {
            return modelPrefab;
        }

        public override void OnInteract(Vector3 localMousePoint, IPlayer player)
        {
            switchLight();
        }

        public override string TypeName { get { return "lamp"; } }
    }
}
