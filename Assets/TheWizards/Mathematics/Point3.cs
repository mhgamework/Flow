﻿using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DirectX11
{
    /// <summary>
    /// This struct represents a discrete vector
    /// </summary>
    [Serializable]
    public struct Point3
    {
        public int X;
        public int Y;
        public int Z;



        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// This uses a vector3 as a point3, note that this uses Math.round to convert the coords
        /// </summary>
        /// <param name="v"></param>
        public Point3(Vector3 v)
        {
            X = (int)Math.Round(v.x);
            Y = (int)Math.Round(v.y);
            Z = (int)Math.Round(v.z);
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new ArgumentException("index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new ArgumentException("index");
                }
            }
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        internal static Point3 Floor(Vector3 vector3)
        {
            return new Point3((int)Math.Floor(vector3.x), (int)Math.Floor(vector3.y), (int)Math.Floor(vector3.z));
        }
        internal static Point3 Ceil(Vector3 vector3)
        {
            return new Point3((int)Math.Ceiling(vector3.x), (int)Math.Ceiling(vector3.y), (int)Math.Ceiling(vector3.z));
        }

        public static implicit operator Vector3(Point3 p)
        {
            return p.ToVector3();
        }
        public static Boolean operator ==(Point3 p, Point3 p2)
        {
            return p.hasSameValue(p2);
        }
        public static Boolean operator !=(Point3 p, Point3 p2)
        {
            return !p.hasSameValue(p2);
        }

        public Boolean hasSameValue(Point3 pos)
        {
            if (pos.X == X && pos.Y == Y && pos.Z == Z)
                return true;
            return false;
        }
        public static Point3 operator -(Point3 p, Point3 p2)
        {
            return new Point3(p.X - p2.X, p.Y - p2.Y, p.Z - p2.Z);
        }
        public static Point3 operator +(Point3 p, Point3 p2)
        {
            return new Point3(p.X + p2.X, p.Y + p2.Y, p.Z + p2.Z);
        }
        public static Point3 operator -(Point3 p)
        {
            return new Point3(-p.X, -p.Y, -p.Z);
        }
        public static Point3 operator *(Point3 p, int num)
        {
            return new Point3(p.X * num, p.Y * num, p.Z * num);
        }
        public static Point3 operator /(Point3 p, int num)
        {
            return new Point3(p.X / num, p.Y / num, p.Z / num);
        }

        /// <summary>
        /// Component wise multiplication
        /// </summary>
        /// <returns></returns>
        public Point3 Multiply(Point3 other)
        {
            return new Point3(X * other.X, Y * other.Y, Z * other.Z);
        }

        public override string ToString()
        {
            return string.Format("X: {0},Y: {1},Z: {2}", X, Y, Z);
        }

        public static Point3 UnitX()
        {
            return new Point3(1, 0, 0);
        }
        public static Point3 UnitY()
        {
            return new Point3(0, 1, 0);
        }
        public static Point3 UnitZ()
        {
            return new Point3(0, 0, 1);
        }

        public bool Equals(Point3 other)
        {
            return other.X == X && other.Y == Y && other.Z == Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Point3)) return false;
            return Equals((Point3)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X;
                result = (result * 397) ^ Y;
                result = (result * 397) ^ Z;
                return result;
            }
        }

        public int[] ToArray()
        {
            return new[] { X, Y, Z };
        }
        public static Point3 FromArray(int[] arr)
        {
            return new Point3(arr[0], arr[1], arr[2]);
        }

        public static void ForEach(Point3 maxExclusive, Action<Point3> act)
        {
            for (int x = 0; x < maxExclusive.X; x++)
            {
                for (int y = 0; y < maxExclusive.Y; y++)
                {
                    for (int z = 0; z < maxExclusive.Z; z++)
                    {
                        act(new Point3(x, y, z));
                    }
                }
            }
        }
    }
}
