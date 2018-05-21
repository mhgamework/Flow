using System;
using System.Collections.Generic;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.Reusable.Utils;
using Assets.SimpleGame._UtilsToMove;
using UnityEngine;

namespace Assets.SimpleGame.Tools
{
    public class DigTool
    {
        private VoxelWorldEditingHelper editor;
        private DigToolGizmoScript GizmoPrefab;

        private bool stopped = false;
        private float diggingProgress = 0;
        private float DigTime = 0.5f;
        private float DigRadius = 1f;

        public DigTool(VoxelWorldEditingHelper editor, DigToolGizmoScript gizmoPrefab)
        {
            this.editor = editor;
            GizmoPrefab = gizmoPrefab;
        }

        public IEnumerable<YieldInstruction> Start()
        {
            stopped = false;
            var gizmo = GameObject.Instantiate(GizmoPrefab);
            gizmo.Hide();

            while (!stopped)
            {
                var point = editor.RaycastPlayerCursor();
                if (!point.HasValue)
                {
                    gizmo.Hide();
                    yield return null;
                    continue;
                }
                gizmo.Show(point.Value, false);

                diggingProgress = 0;
                yield return null;
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
                    editor.Subtract(obj);
                }

                yield return null;
            }

            GameObject.Destroy(gizmo);
        }



        private void updateGizmo(DigToolGizmoScript gizmo, Vector3? point)
        {


        }


        public void Stop()
        {
            stopped = true;
        }
    }
}