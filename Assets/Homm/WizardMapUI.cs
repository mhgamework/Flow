using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using UnityEngine;

namespace Assets.Homm
{
    public class WizardMapUI : MonoBehaviour
    {
        public Wizard Wizard;

        public Transform TargetCell;
        public Transform HeroPathCell;
        public Transform HeroPathCell_LaterTurns;


        private Grid grid;

        private List<Transform> PathCells = new List<Transform>();
        private List<Transform> PathCells_LaterTurns = new List<Transform>();



        public void Start()
        {
        }

        public void Update()
        {
            if (grid == null)
                grid = HommMain.Instance.Grid;

            TargetCell.transform.position = grid.toCellCenter(Wizard.SelectedTargetLocation);
            applyOverlayOffset(TargetCell.transform);

            UpdateWalkpathRendering(Wizard.WalkPath, Wizard.MovementLeft); // Add 1 for first bullet
        }

        private void UpdateWalkpathRendering(List<Point3> path, int stepsThisTurn)
        {
            if (path == null)
            {
                Debug.Log("No path!");
                path = new List<Point3>();
            }

            var numThisTurn = Math.Min(path.Count, stepsThisTurn);

            int iGreen = 0;
            int iGray = 0;

            foreach (var p in PathCells) p.gameObject.SetActive(false);
            foreach (var p in PathCells_LaterTurns) p.gameObject.SetActive(false);

            for (int i = 1; i < path.Count - 1; i++)
            {
                if (i <= stepsThisTurn)
                {
                    if (iGreen >= PathCells.Count)
                    {
                        PathCells.Add(Instantiate(HeroPathCell));
                    }
                    PathCells[iGreen].gameObject.SetActive(true);
                    PathCells[iGreen].position = grid.toCellCenter(path[i]);
                    iGreen++;
                }
                else
                {
                    if (iGray >= PathCells_LaterTurns.Count)
                    {
                        PathCells_LaterTurns.Add(Instantiate(HeroPathCell_LaterTurns));
                    }
                    PathCells_LaterTurns[iGray].gameObject.SetActive(true);
                    PathCells_LaterTurns[iGray].position = grid.toCellCenter(path[i]);
                    iGray++;
                }
            }


            foreach (var p in PathCells.Concat(PathCells_LaterTurns))
            {
                applyOverlayOffset(p);
            }

        }

        private static void applyOverlayOffset(Transform p)
        {
            p.transform.position += -Camera.main.transform.forward * 5;
        }
    }
}