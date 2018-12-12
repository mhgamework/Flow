using System;
using UnityEngine;

namespace Assets.SimpleGame.Chutes
{
    public class ChuteItemScript : MonoBehaviour
    {
        public float StartY;
        public float Position;
        public float Direction;
        public ChuteItemType Type;

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void InitType(ChuteItemType type)
        {
            this.Type = type;
            GetComponent<MeshRenderer>().material.color = type.Color;
        }
    }
}