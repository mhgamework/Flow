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

    private Vector3 playerStartPos;

    public Transform CubeGhost;

    // Use this for initialization
    void Start()
    {
        playerStartPos = Player.position;
    }

    // Update is called once per frame
    void Update()
    {
        updateGhost();
        if (Input.GetMouseButtonDown(0))
            Dig(1);
        if (Input.GetMouseButtonDown(1))
            Dig(-1);
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShootSphere();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Player.position = playerStartPos;
        }
    }

    private void updateGhost()
    {
        RaycastHit raycastHit;
        var res = Raycast(out raycastHit);
        CubeGhost.gameObject.SetActive(res);
        if (!res) return;
        var b = worldToDigCubeCell(raycastHit);

        CubeGhost.position = b.center;
        CubeGhost.localScale = b.extents * 2;


    }

    private void ShootSphere()
    {
        var s = Instantiate(ShootPrefab, transform);
        s.transform.position = Player.position + ShootStartOffset;

        s.velocity = Player.forward * ShootStartVelocity;

    }

    private void Dig(int mode)
    {
        RaycastHit raycastHit;
        var res = Raycast(out raycastHit);
        if (!res) return;


        var b = worldToDigCubeCell(raycastHit);

        var w = Renderer.GetWorld();

        w.RunKernel1by1(b.min.ToPoint3Rounded(), b.max.ToPoint3Rounded(), (data, p) =>
        {
            data.Density = mode;
            return data;
        }, Time.frameCount);

    }

    private Bounds worldToDigCubeCell(RaycastHit raycastHit)
    {
        
        var pos = raycastHit.point;
        pos = DigSize * (((pos + Vector3.one * Renderer.RenderScale) / DigSize).ToFloored().ToVector3() + 0.5f * Vector3.one) - Vector3.one * Renderer.RenderScale;
        var min = pos - Vector3.one * DigSize / 2f;
        var max = pos + Vector3.one * DigSize / 2f;

        var b = new Bounds();
        b.SetMinMax(Renderer.ToVoxelSpace(min).ToFloored(), Renderer.ToVoxelSpace(max).ToCeiled());
        return b;
    }

    private static bool Raycast(out RaycastHit raycastHit)
    {
        return Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out raycastHit, 100);
    }
}
