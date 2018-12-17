using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample
{
    public struct SdfDataColor
    {
        public float Distance;
        public Color Color;

        public SdfDataColor(float distance, Color color)
        {
            Distance = distance;
            Color = color;
        }
    }

    public interface ISuggestedSdfInterfaceColor : ISuggestedSdfInterface<SdfDataColor>
    {

    }
    public interface ISuggestedSdfInterface<T> 
    {
        void GetSdf(Vector3 pos, out T outData);
        void SetDirtyHandler(IDirtyHandler handler);
    }

    public interface IDirtyHandler
    {
        void MarkRegionDirty(Bounds b);
    }
}