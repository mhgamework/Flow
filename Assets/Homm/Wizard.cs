using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using UnityEngine;

namespace Assets.Homm
{
    public class Wizard : MonoBehaviour
    {
        public Point3 SelectedTargetLocation;
        public Point3 Location;
        public int Movement = 8;
        public int MovementLeft = 8;
        private Grid grid;

        public List<Point3> WalkPath;

        public void Start()
        {
        }

        public void Update()
        {
            if (grid == null)
            {
                grid = HommMain.Instance.Grid;
                MoveTo(Location);
            }

        }

        private void MoveTo(Point3 location)
        {
            Location = location;
            transform.position = grid.toCellCenter(location);

        }

        public IEnumerable<YieldInstruction> MoveStep(float stepInterval)
        {
            while (WalkPath != null && MovementLeft != 0)
            {
                MovementLeft -= 1;

                if (WalkPath.Count == 2)
                {
                    // Last cell, check if interactable
                    var cell = grid.Get(WalkPath[1]);
                    if (cell.Interactables.Any())
                    {
                        if (cell.Interactables.Count() > 1)
                            throw new Exception("Dont know what to do with multiple interactables!");

                        var inter = cell.Interactables.First();

                        foreach (var i in inter.Interact())
                            yield return i;

                        WalkPath = null;
                        continue;

                    }
                }

                //if (grid.Get(WalkPath[1]).IsOccupied)
                //{
                //    foreach (var f in grid.Get(WalkPath[1]).Interactables.First().Interact())
                //        yield return f;

                //    WalkPath = null;
                //    continue;

                //}

                MoveTo(WalkPath[1]);
                WalkPath.RemoveAt(0);
                if (WalkPath.Count == 1) WalkPath = null;

                yield return new WaitForSeconds(stepInterval);
            }

        }

        public void SetTargetLocation(Point3 targetCell)
        {
            SelectedTargetLocation = targetCell;
            updateWalkPath();
        }

        private void updateWalkPath()
        {
            if (Location == SelectedTargetLocation)
            {
                WalkPath = null;
                return;
            }
            WalkPath = CalculatePath(Location, SelectedTargetLocation);
            if (WalkPath != null)
                WalkPath.Reverse();

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

                foreach (var n in getNeighbours(current, goal))
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
            new Point3(-1, 0, 0),
            new Point3(0, 0, 1),
            new Point3(1, 0, 0),
            new Point3(0, 0,-1),
        };

        //private Point3[] points2 = new Point3[]
        //{
        //    new Point3(-1, 0, -1),
        //    new Point3(0, 0, -1),
        //    new Point3(1, 0, -1),
        //    new Point3(-1, 0, 0),
        //    new Point3(1, 0, 0),
        //    new Point3(-1, 0, 1),
        //    new Point3(0, 0, 1),
        //    new Point3(1, 0, 1),
        //};

        private bool[] tempNeighbours = new bool[4];

        private IEnumerable<Point3> getNeighbours(Point3 current, Point3 finalGoal)
        {
            for (int i = 0; i < points.Length; i++)
            {
                var n = current + points[i];
                var walkable = (grid.Get(n).IsWalkable && !grid.Get(n).IsOccupied) || (n == finalGoal && grid.Get(n).IsOccupied);
                tempNeighbours[i] = walkable;
                if (walkable)
                    yield return n;
            }
            for (int i = 0; i < points.Length; i++)
            {
                var next = (i + 1) % 4;
                if (tempNeighbours[i] && tempNeighbours[next])
                {
                    // We can walk both sides, so we can attempt diagonal walk
                    var n = current + points[i] + points[next];

                    var walkable = (grid.Get(n).IsWalkable && !grid.Get(n).IsOccupied) || (n == finalGoal && grid.Get(n).IsOccupied);
                    if (walkable)
                        yield return n;

                }
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