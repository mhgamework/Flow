using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class rust_test : MonoBehaviour {

    [StructLayout(LayoutKind.Sequential)]
    struct Triangle
    {
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]
        public XYZ[] p;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct XYZ
    {
        public float X;
        public float Y;
        public float Z;
    }

    [DllImport("rust_test")]
    [return: MarshalAs(UnmanagedType.LPArray, SizeConst = 16)]
    private static extern Triangle[] marching_cubes();

    [DllImport("rust_test")]
    private static extern Int32 test();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    Debug.Log(test());
	    var t = marching_cubes();
	}
}
