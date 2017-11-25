using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MarchingCubes
{
    public class TransformRotator :MonoBehaviour
    {
        public Vector3 RotationEuler;
        public float Speed;

        public void Update()
        {
            transform.Rotate(RotationEuler * Speed);
        }
    }
}
