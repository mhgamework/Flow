using System.Collections;
using System.Collections.Generic;
using Assets.MarchingCubes;
using Assets.MarchingCubes.VoxelWorldMVP;
using UnityEngine;

public class SimpleGameInputScript : MonoBehaviour {
    public Rigidbody ShootPrefab;
    public Vector3 ShootStartOffset;
    public float ShootStartVelocity = 1;
    public Transform Player;

    public VoxelWorldGenerator World;

    public float DigSize = 3;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButtonDown(0))
	    {
	        Dig();
	    }
	    if (Input.GetKeyDown(KeyCode.F))
	    {
	        ShootSphere();
	    }
	}

    private void ShootSphere()
    {
        var s = Instantiate(ShootPrefab, transform);
        s.transform.position = Player.position + ShootStartOffset;

        s.velocity = Player.forward * ShootStartVelocity;

    }

    private void Dig()
    {
        RaycastHit raycastHit;
        var res = Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out raycastHit, 100);
        if (!res) return;


    }
}
