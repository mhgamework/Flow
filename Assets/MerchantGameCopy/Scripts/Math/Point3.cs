//using System;
//using UnityEngine;

//[Serializable]
//public struct Point3
//{
//    public int x;
//    public int y;
//    public int z;

//    public Point3(int x, int y, int z)
//    {
//        this.x = x;
//        this.y = y;
//        this.z = z;
//    }

//    public static Point3 Floor(Vector3 v)
//    {
//        return new Point3()
//        {
//            x = Mathf.FloorToInt(v.x),
//            y = Mathf.FloorToInt(v.y),
//            z = Mathf.FloorToInt(v.z),
//        };
//    }

//    public bool Equals(Point3 other)
//    {
//        return x == other.x && y == other.y && z == other.z;
//    }

//    public override bool Equals(object obj)
//    {
//        if (ReferenceEquals(null, obj)) return false;
//        return obj is Point3 && Equals((Point3)obj);
//    }

//    public override int GetHashCode()
//    {
//        unchecked
//        {
//            var hashCode = x;
//            hashCode = (hashCode * 397) ^ y;
//            hashCode = (hashCode * 397) ^ z;
//            return hashCode;
//        }
//    }

//    public static bool operator ==(Point3 a, Point3 b)
//    {
//        return a.Equals(b);
//    }

//    public static bool operator !=(Point3 a, Point3 b)
//    {
//        return !(a == b);
//    }

//    public static Point3 operator +(Point3 a, Point3 b)
//    {
//        return new Point3(a.x + b.x, a.y + b.y, a.z + b.z);
//    }
//}