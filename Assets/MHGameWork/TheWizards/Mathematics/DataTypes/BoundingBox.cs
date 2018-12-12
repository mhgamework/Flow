using UnityEngine;

namespace Assets.TheWizards.Mathematics.DataTypes
{
    public class BoundingBox
    {
        private readonly Vector3 _min;
        private readonly Vector3 _max;

        public BoundingBox(Vector3 min, Vector3 max)
        {
            _min = min;
            _max = max;
        }

        public Bounds unity()
        {
            var ret = new Bounds();
            ret.SetMinMax(_min,_max);
            return ret;
        }
    }
}