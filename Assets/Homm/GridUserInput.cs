using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Homm
{
    public class GridUserInput : MonoBehaviour, IPointerClickHandler
    {
        public Transform TargetCell;
        public Transform HeroPathCell;
        public Transform HeroFigure;

        private Point3 SelectedGridcell;

        private Grid grid;

        private List<Transform> PathCells = new List<Transform>();

        private Point3 HeroPos = new Point3();

        public void Update()
        {
            TargetCell.transform.position = toCellCenter(SelectedGridcell);
            grid = HommMain.Instance.Grid;

            HeroFigure.transform.position = toCellCenter(HeroPos);

            var path = CalculatePath(HeroPos, SelectedGridcell);
            path.RemoveAt(path.Count - 1);
            for (int i = 0; i < Math.Max(path.Count, PathCells.Count); i++)
            {
                if (i >= path.Count)
                {
                    PathCells[i].gameObject.SetActive(false);
                    continue;
                }
                if (i >= PathCells.Count)
                {
                    PathCells.Add(Instantiate(HeroPathCell));
                }
                PathCells[i].gameObject.SetActive(true);
                PathCells[i].position = toCellCenter(path[i]);
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked!");
            var pos = eventData.pointerCurrentRaycast.worldPosition;
            var targetCell = pointToCell(pos);
            if (eventData.clickCount > 1) Debug.Log("igoring dblclick");
            else if (SelectedGridcell == targetCell)
                HeroPos = targetCell;

            SelectedGridcell = targetCell;
        }

        public Point3 pointToCell(Vector3 worldPos)
        {
            return worldPos.ToFloored();
        }

        private Vector3 toCellCenter(Point3 selectedGridcell)
        {
            return selectedGridcell.ToVector3() + new Vector3(0.5f, 0, 0.5f);
        }


        private List<Point3> CalculatePath(Point3 start, Point3 goal)
        {
            // The set of nodes already evaluated
            var closedSet = new HashSet<Point3>();

            // The set of currently discovered nodes that are not evaluated yet.
            // Initially, only the start node is known.
            var openSet = new HashSet<Point3>();
            openSet.Add(start);


            // For each node, which node it can most efficiently be reached from.
            // If a node can be reached from many nodes, cameFrom will eventually contain the
            // most efficient previous step.
            var cameFrom = new Dictionary<Point3, Point3>();

            // For each node, the cost of getting from the start node to that node.
            var gScore = new Dictionary<Point3, float>();
            //gScore:= map with default value of Infinity

            // The cost of going from start to start is zero.
            gScore[start] = 0;

            // For each node, the total cost of getting from the start node to the goal
            // by passing by that node. That value is partly known, partly heuristic.

            var fScore = new Dictionary<Point3, float>(); // fScore:= map with default value of Infinity


            // For the first node, that value is completely heuristic.
            fScore[start] = heuristic_cost_estimate(start, goal);

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(f => fScore.GetOrDefault(f, float.PositiveInfinity)).First();
                // := the node in openSet having the lowest fScore[] value
                if (current == goal)
                    return reconstruct_path(cameFrom, current);

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var n in getNeighbours(current))
                {
                    if (closedSet.Contains(n)) continue; // Ignore the neighbor which is already evaluated.

                    if (!openSet.Contains(n))
                        openSet.Add(n); // Discover a new node

                    // The distance from start to a neighbor
                    var tentative_gScore = gScore[current] + exact_cost(current, n);
                    if (tentative_gScore >= gScore.GetOrDefault(n, float.PositiveInfinity))
                        continue; // This is not a better path.

                    // This path is the best until now. Record it!
                    cameFrom[n] = current;
                    gScore[n] = tentative_gScore;
                    fScore[n] = gScore[n] + heuristic_cost_estimate(n, goal);
                }
            }
            return null;
        }

        private Point3[] points = new Point3[]
        {
            new Point3(-1, 0, -1),
            new Point3(0, 0, -1),
            new Point3(1, 0, -1),
            new Point3(-1, 0, 0),
            new Point3(1, 0, 0),
            new Point3(-1, 0, 1),
            new Point3(0, 0, 1),
            new Point3(1, 0, 1),
        };

        private IEnumerable<Point3> getNeighbours(Point3 current)
        {
            for (int i = 0; i < points.Length; i++)
            {
                yield return current + points[i];
            }
        }

        public float exact_cost(Point3 start, Point3 neighbour)
        {
            return (start - neighbour).ToVector3().magnitude;
        }

        public float heuristic_cost_estimate(Point3 start, Point3 goal)
        {
            return (goal - start).ToVector3().magnitude;
        }

        public List<Point3> reconstruct_path(Dictionary<Point3, Point3> cameFrom, Point3 current)
        {
            var total_path = new List<Point3>();
            total_path.Add(current);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                total_path.Add(current);
            }
            return total_path;
        }
    }
}