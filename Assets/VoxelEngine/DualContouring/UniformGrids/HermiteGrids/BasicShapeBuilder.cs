
//using UnityEngine;

//namespace MHGameWork.TheWizards.DualContouring
//{
//    /// <summary>
//    /// Constructs basic shapes
//    /// </summary>
//    public class BasicShapeBuilder
//    {
//        /// <summary>
//        /// Returns a hermite grid of size+1 with a cube of 'size' voxels centered in the middle
//        /// </summary>
//        public HermiteDataGrid CreateCube(int size)
//        {
//            return HermiteDataGrid.FromIntersectableGeometry(size + 1, size + 1, Matrix4x4.Scale( Vector3.one*(0.5f * size)) * Matrix4x4.TRS(Vector3.one*(0.5f + size / 2f),Quaternion.identity, Vector3.one),
//                                                            new IntersectableCube());
//        }
//        /// <summary>
//        /// Returns a hermite grid of size+1 with a sphere of 'size' voxels centered in the middle
//        /// </summary>
//        public HermiteDataGrid CreateSphere(int size)
//        {
//            return HermiteDataGrid.FromIntersectableGeometry(size + 1, size + 1, Matrix4x4.Scale(Vector3.one*(0.5f * size)) * Matrix4x4.TRS(Vector3.one * (0.5f + size / 2f), Quaternion.identity, Vector3.one),
//                                                            new IntersectableSphere());
//        }
//    }
//}