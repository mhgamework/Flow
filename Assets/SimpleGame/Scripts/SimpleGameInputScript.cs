﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Flow.WardDrawing;
using Assets.MarchingCubes;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.SimpleGame.Scripts;
using MHGameWork.TheWizards;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SimpleGameInputScript : MonoBehaviour
{
    public Rigidbody ShootPrefab;
    public Vector3 ShootStartOffset;
    public float ShootStartVelocity = 1;
    public Transform Player;

    public VoxelRenderingEngineScript Renderer;
    public FirstPersonController FirstPersonController;

    public float DigSize = 3;


    public Transform CubeGhost;

    public Color DirtColor;
    public Color StoneColor;
    public Color WoodColor;

    public WardDrawInputScript WardDrawInput;

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PlayerScript.Instance.ToggleAirSpellCasting();
            if (PlayerScript.Instance.AirSpellCasting)
            {

            }
        }

        FirstPersonController.enabled = !PlayerScript.Instance.AirSpellCasting;
        if (PlayerScript.Instance.AirSpellCasting)
        {
            Cursor.lockState = CursorLockMode.None; // Doesnt work in editor: CursorLockMode.Confined;
            Cursor.visible = true;
            updateGhost(false);
        }
        else
        {
            updateGhost(true);
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
                    Dig(-1, new VoxelMaterial(c));

                }

            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                ShootSphere();
            }
        }




        if (Input.GetKeyDown(KeyCode.KeypadPlus)) DayNightCycleScript.Instance.ChangeTimeRelative(0.1f);
        if (Input.GetKeyDown(KeyCode.KeypadMinus)) DayNightCycleScript.Instance.ChangeTimeRelative(-0.1f);

     
        if (Input.GetKeyDown(KeyCode.R))
        {
            Assets.SimpleGame.Scripts.PlayerScript.Instance.Respawn();
        }

        if (Input.mouseScrollDelta.y < 0)
            HotbarScript.Instance.SelectNext();
        if (Input.mouseScrollDelta.y > 0)
            HotbarScript.Instance.SelectPrevious();
    }

    private void updateGhost(bool enabled)
    {
        RaycastHit raycastHit;
        var res = Raycast(out raycastHit);
        res = res && enabled;
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

        if (mode > 0)
        {
            raycastHit.point -= raycastHit.normal * DigSize;
        }

        var b = worldToDigCubeCell(raycastHit);

        var w = Renderer.GetWorld();

        var f = 1.1f;
        //if (mode > 0) f = 0.9f;
        var box = new Box(b.extents.x*f, b.extents.y * f, b.extents.z * f);
        var t = new Translation(box, b.center);

        w.RunKernel1by1(b.min.ToPoint3Rounded() - new DirectX11.Point3(2, 2, 2), b.max.ToPoint3Rounded() + new DirectX11.Point3(2, 2, 2), (data, p) =>
            {
                if (mode > 0)
                {
                    var cubeD = t.Sdf(p);
                    var newDensity = Mathf.Max(data.Density, -cubeD);

                    data.Density = newDensity;


                }
                else
                {
                    var cubeD = t.Sdf(p);
                    var newDensity = Mathf.Min(data.Density, cubeD);
                    if (newDensity != data.Density)
                        data.Material = c;

                    data.Density = newDensity;
                }

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
