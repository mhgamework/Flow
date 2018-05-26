using System;
using System.Collections.Generic;
using Assets.MarchingCubes.Domain;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.World;
using Assets.Reusable.Utils;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame._UtilsToMove;
using UnityEngine;

namespace Assets.SimpleGame.Tools
{
    public class DigTool
    {
        private VoxelWorldEditingHelper editor;
        private DigToolGizmoScript GizmoPrefab;
        private readonly PlayerScript playerScript;

        private bool stopped = false;
        private float diggingProgress = 0;
        private float DigTime = 0.5f;
        private float DigRadius = 1f;

        private float DigMaxDistance = 20f;

        private SDFWorldEditingService.Counts counts = new SDFWorldEditingService.Counts();

        public DigTool(VoxelWorldEditingHelper editor, DigToolGizmoScript gizmoPrefab, PlayerScript playerScript)
        {
            this.editor = editor;
            GizmoPrefab = gizmoPrefab;
            this.playerScript = playerScript;
        }

        public IEnumerable<YieldInstruction> Start()
        {
            stopped = false;
            var gizmo = GameObject.Instantiate(GizmoPrefab);
            gizmo.Hide();

            while (!stopped)
            {
                var point = editor.RaycastPlayerCursor();
                if (!point.HasValue || Vector3.Distance( point.Value, playerScript.GetPlayerPosition()) > DigMaxDistance)
                {
                    gizmo.Hide();
                    yield return null;
                    continue;
                }
                gizmo.Show(point.Value, false);

                diggingProgress = 0;
                yield return null;

                if (Input.GetMouseButton(1))
                {
                    editor.Smooth(point.Value, DigRadius, counts);
                    yield return null;
                    continue;
                }


                while (Input.GetMouseButton(0) && diggingProgress < DigTime)
                {
                    var temp = editor.RaycastPlayerCursor();
                    if (temp.HasValue && Vector3.Distance(temp.Value, point.Value) > DigRadius / 2)
                        point = temp;


                    diggingProgress += Time.deltaTime;
                    gizmo.Show(point.Value, true);
                    yield return null;

                }

                if (diggingProgress >= DigTime)
                {
                    var obj = new Ball(point.Value, DigRadius);
                    //counts.Clear();
                    editor.Subtract(obj, counts);


                    Debug.Log(counts);

                    for (int i = 0; i < 5; i++)
                    {
                        editor.Smooth(point.Value, DigRadius, counts);
                    }

                    foreach (var am in counts.Amounts)
                    {
                        var type = am.Material.color == Color.red ? "iron" : am.Material.color == Color.yellow ? "gold" : "rock";
                        var amount = Mathf.FloorToInt(am.Amount);
                        if (amount < 0) continue;
                        counts.Change(am.Material, -amount);
                        playerScript.StoreItems(type, amount);
                    }
                }

                yield return null;
            }

            GameObject.Destroy(gizmo.gameObject);
        }



        public void Stop()
        {
            stopped = true;

        }
    }
}