using System;
using System.Collections.Generic;
using Assets.TheWizards.Mathematics;
using MHGameWork.TheWizards.Engine.Raycasting;
using UnityEngine;

namespace MHGameWork.TheWizards.Scattered._Engine
{
    /// <summary>
    /// Warning: this is a good idea, but is untested and i think this does not work.
    /// </summary>
    public static class RaycastExtensions
    {
        public static RaycastResult Raycast<T>(this IEnumerable<T> e, Func<T, Bounds> getLocalBoundingbox, Func<T, Matrix4x4> getTransform, Ray ray) where T : class
        {
            var closest = new RaycastResult();
            var newResult = new RaycastResult();

            //IWorldSelectableProvider closestProvider = null;
            //Selectable closestSelectable = null;  

            foreach (var s in e)
            {
                var transformation = getTransform(s);
                var localRay =ray.Transform(Matrix4x4.Inverse(transformation));
                float dist;
                if (getLocalBoundingbox(s).IntersectRay(localRay, out dist))
                {
                    var localPoint = localRay.GetPoint(dist);
                    var point = transformation.MultiplyPoint3x4(localPoint);
                    dist = Vector3.Distance(ray.origin, point);
                }

                newResult.Set(dist, s);

                if (newResult.IsCloser(closest))
                {
                    newResult.CopyTo(closest);
                }

            }

            return closest;
        }

        public static T Raycast<T>(this IEnumerable<T> e, Func<T, Ray, float?> intersect, Ray ray) where T : class
        {
            var closest = RaycastDetail(e, intersect, ray);
            return closest.IsHit ? (T)closest.Object : null;
        }
        public static RaycastResult RaycastDetail<T>(this IEnumerable<T> e, Func<T, Ray, float?> intersect, Ray ray) where T : class
        {
            var closest = new RaycastResult();
            var newResult = new RaycastResult();

            //IWorldSelectableProvider closestProvider = null;
            //Selectable closestSelectable = null;

            foreach (var s in e)
            {
                var dist = intersect(s, ray);

                newResult.Set(dist, s);

                if (newResult.IsCloser(closest))
                {
                    newResult.CopyTo(closest);
                }

            }

            return closest;
        }
    }
}