using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.SimpleGame.Scripts;
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


    public Transform CubeGhost;

    public Color DirtColor;
    public Color StoneColor;
    public Color WoodColor;

    // Use this for initialization
    void Start()
    {
 

    }

    // Update is called once per frame
    void Update()
    {
        updateGhost();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            var res = Raycast(out raycastHit);
            if (res)
            {
                var b = worldToDigCubeCell(raycastHit);

                var w = Renderer.GetWorld();

                var mats = new List<VoxelMaterial>();

                w.RunKernel1by1(b.min.ToPoint3Rounded(), b.max.ToPoint3Rounded(), (data, p) =>
                {
                    mats.Add(data.Material);
                    return data;
                }, Time.frameCount);


                Dig(1);

                if (mats.Any(v => v.color == StoneColor))
                {
                    Assets.SimpleGame.Scripts.PlayerScript.Instance.StoreItems("stone", 1);
                }
                else if (mats.Any(v => v.color == WoodColor))
                {
                    Assets.SimpleGame.Scripts.PlayerScript.Instance.StoreItems("wood", 1);
                }
                else
                {
                    Assets.SimpleGame.Scripts.PlayerScript.Instance.StoreItems("dirt", 1);
                }

            }

        }
        if (Input.GetMouseButtonDown(1))
        {
            var pl = Assets.SimpleGame.Scripts.PlayerScript.Instance;


            var selectedItem = HotbarScript.Instance.GetSelectedInventoryItem();
            if (!selectedItem.IsEmpty)
            {
                Color c;
                if (selectedItem.ResourceType == "stone")
                    c = StoneColor;
                else if (selectedItem.ResourceType == "wood")
                    c = WoodColor;
                else
                    c = DirtColor;

                pl.TakeItems(selectedItem.ResourceType, 1);
                Dig(-1,new VoxelMaterial(c));

            }

        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShootSphere();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Assets.SimpleGame.Scripts.PlayerScript.Instance.Respawn();
        }

        if (Input.mouseScrollDelta.y < 0)
            HotbarScript.Instance.SelectNext();
        if (Input.mouseScrollDelta.y > 0)
            HotbarScript.Instance.SelectPrevious();
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

    private void Dig(int mode, VoxelMaterial c = null)
    {
        RaycastHit raycastHit;
        var res = Raycast(out raycastHit);
        if (!res) return;


        var b = worldToDigCubeCell(raycastHit);

        var w = Renderer.GetWorld();

        w.RunKernel1by1(b.min.ToPoint3Rounded(), b.max.ToPoint3Rounded(), (data, p) =>
        {
            data.Density = mode;
            if (c != null)
                data.Material = c;
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
