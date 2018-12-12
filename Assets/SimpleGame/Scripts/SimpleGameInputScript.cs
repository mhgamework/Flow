using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.SdfModeling;
using Assets.MHGameWork.FlowEngine._Cleanup;
using Assets.SimpleGame;
using Assets.SimpleGame.BuilderSystem;
using Assets.SimpleGame.Items;
using Assets.SimpleGame.Multiplayer;
using Assets.SimpleGame.Multiplayer.Players;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.Tools;
using Assets.SimpleGame.VoxelEngine;
using Assets.SimpleGame.WardDrawing;
using Assets.SimpleGame.Wards;
using Assets.SimpleGame._UtilsToMove;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class SimpleGameInputScript : MonoBehaviour
{
    public Rigidbody ShootPrefab;
    public Vector3 ShootStartOffset;
    public float ShootStartVelocity = 1;



    public MagicParticleSpellItem tempMagicParticleSpellItem;

    public float DigSize = 3;


    public Transform CubeGhost;

    public Color DirtColor;
    public Color StoneColor;
    public Color WoodColor;

    public List<AbstractWardSpell> Spells;
    public GameObject LightPrefab;


    private Transform Player;
    private VoxelRenderingEngineScript Renderer;
    private FirstPersonController FirstPersonController;
    private WardDrawingModeScript WardDrawingModeScript;

    private DigTool digTool;

    [SerializeField]
    private DigToolGizmoScript DigToolGizmoPrefab;

    private PlayerScript playerScript;
    [SerializeField] private BuildSystemInputTool buildSystemInputTool;


    public void Initialize(LocalPlayerScript localPlayer, VoxelRenderingEngineScript voxelRenderer, WardDrawingModeScript wardDrawingModeScript)
    {
        this.Player = localPlayer.transform;
        playerScript = localPlayer.GetPlayer();
        this.Renderer = voxelRenderer;
        this.FirstPersonController = localPlayer.GetFirstPersonController();
        this.WardDrawingModeScript = wardDrawingModeScript;

        WardDrawingModeScript.SetWards(Spells.Select(s => s.Ward).ToList());

        WardDrawingModeScript.OnCorrectWard += OnCorrectWard;

        buildSystemInputTool.Init();

    }



   

    private void OnCorrectWard(Ward obj)
    {
        var spell = Spells.FirstOrDefault(s => s.Ward == obj);
        if (spell == null) return;
        spell.Cast(getPlayer());
        updateWardDrawingState();
    }

    private static PlayerScript getPlayer()
    {
        return LocalPlayerScript.Instance.GetPlayer();
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            getPlayer().ToggleAirSpellCasting();
            updateWardDrawingState();

        }

        doTool();

        if (Input.GetKeyDown(KeyCode.KeypadPlus)) DayNightCycleScript.Instance.ChangeTimeRelative(0.1f);
        if (Input.GetKeyDown(KeyCode.KeypadMinus)) DayNightCycleScript.Instance.ChangeTimeRelative(-0.1f);


        if (Input.GetKeyDown(KeyCode.R))
        {
            getPlayer().Respawn();
        }

        if (Input.mouseScrollDelta.y < 0)
            HotbarScript.Instance.SelectNext();
        if (Input.mouseScrollDelta.y > 0)
            HotbarScript.Instance.SelectPrevious();


        if (Input.GetKeyDown(KeyCode.F))
        {
            ShootSphere();
        }

        getPlayer().GetComponent<PlayerGrenadeScript>().SetFireGrenadeDown(Input.GetKey(KeyCode.F));
        if (Input.GetKeyDown(KeyCode.G))
            getPlayer().GetComponent<PlayerPushScript>().DoStartCastPushSpell();
        if (Input.GetKeyUp(KeyCode.G))
            getPlayer().GetComponent<PlayerPushScript>().DoStopCastPushSpell();
    }


    void doTool()
    {

        if (getPlayer().AirSpellCasting)
        {
            updateGhost(false);
            if (digTool != null)
            {
                digTool.Stop();
                digTool = null;
            }
            return;
        }

        if (isResourceOfTypeSelectedOnHotbar("digtool"))
        {
            if (digTool == null)
            {
                digTool = new DigTool(new VoxelWorldEditingHelper(Renderer, Renderer.GetWorld()), DigToolGizmoPrefab, playerScript);
                StartCoroutine(digTool.Start().GetEnumerator());
            }

            return;
        }
        else
        {
            if (digTool != null)
            {
                digTool.Stop();
                digTool = null;
            }

        }

        if (isResourceOfTypeSelectedOnHotbar("magicprojectile"))
        {
            // Hacky!
            var spellItem = tempMagicParticleSpellItem;
            spellItem.UpdateTool(getPlayer());
            updateGhost(false);
            return;
        }

        var selectedInventoryItem = HotbarScript.Instance.GetSelectedInventoryItem();
        if (selectedInventoryItem.Amount > 0 && buildSystemInputTool.tryTempHackySetToolActive(selectedInventoryItem.ResourceType))
        {
            buildSystemInputTool.DoUpdate();

            return;
        }

        {
            updateGhost(true);
            updateToolDig();
        }

    }

    private static bool isResourceOfTypeSelectedOnHotbar(string type)
    {
        return HotbarScript.Instance.GetSelectedInventoryItem().ResourceType == type && HotbarScript.Instance.GetSelectedInventoryItem().Amount > 0;
    }

    private void updateToolDig()
    {
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
                    getPlayer().StoreItems("stone", 1);
                }
                else if (mats.Any(v => v.color == WoodColor))
                {
                    getPlayer().StoreItems("wood", 1);
                }
                else
                {
                    getPlayer().StoreItems("dirt", 1);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var pl = getPlayer();


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

    }

    private void updateWardDrawingState()
    {
        WardDrawingModeScript.SetModeEnabled(getPlayer().AirSpellCasting);
        FirstPersonController.enabled = !getPlayer().AirSpellCasting;

        if (getPlayer().AirSpellCasting)
        {
            var camTransform = FirstPersonController.GetComponentInChildren<Camera>().transform;
            WardDrawingModeScript.SetPlane(camTransform.position + camTransform.forward * 1, -camTransform.forward);
        }
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
        var box = new Box(b.extents.x * f, b.extents.y * f, b.extents.z * f);
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
