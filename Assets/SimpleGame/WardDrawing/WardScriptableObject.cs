using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SimpleGame.WardDrawing
{
    [Serializable]
    public class WardScriptableObject : ScriptableObject
    {
        public List<Vector3> Points;
    }
}