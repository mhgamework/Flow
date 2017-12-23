using System.Collections;
using System.Collections.Generic;
using Assets.MarchingCubes;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.VoxelWorldMVP;
using MHGameWork.TheWizards;
using UnityEngine;

public class SimpleGameInputScript : MonoBehaviour
{
    public Rigidbody ShootPrefab;
    public Vector3 ShootStartOffset;
    public float ShootStartVelocity = 1;
    public Transform Player;

    public VoxelRenderingEngineScript Renderer;

    public float DigSize = 3;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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

        var w = Renderer.GetWorld();

        var min = raycastHit.point - Vector3.one * DigSize / 2f;
        var max = raycastHit.point + Vector3.one * DigSize / 2f;

        w.RunKernel1by1(Renderer.ToVoxelSpace(min).ToFloored(), Renderer.ToVoxelSpace(max).ToCeiled(), (data, p) =>
        {
            data.Density = -1;
            return data;
        }, Time.frameCount);

    }
}
