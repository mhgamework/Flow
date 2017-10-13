using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Reusable
{
    public abstract class EuclidianAStarAlgorithm<T> : AStarAlgorithm<T>
    {
        protected abstract Vector3 GetPosition(T p);

        public override float exact_cost(T start, T neighbour)
        {
            var startPos = GetPosition(start);
            var neighbourPos = GetPosition(neighbour);
            return (startPos - neighbourPos).magnitude; // Make it direction dependent
        }


        public override float heuristic_cost_estimate(T start, T goal)
        {
            return (GetPosition(goal) - GetPosition(start)).magnitude;
        }
    }
    public abstract class AStarAlgorithm<T>
    {
        public List<T> CalculatePath(T start, T goal)
        {
            // The set of nodes already evaluated
            var closedSet = new HashSet<T>();

            // The set of currently discovered nodes that are not evaluated yet.
            // Initially, only the start node is known.
            var openSet = new HashSet<T>();
            openSet.Add(start);


            // For each node, which node it can most efficiently be reached from.
            // If a node can be reached from many nodes, cameFrom will eventually contain the
            // most efficient previous step.
            var cameFrom = new Dictionary<T, T>();

            // For each node, the cost of getting from the start node to that node.
            var gScore = new Dictionary<T, float>();
            //gScore:= map with default value of Infinity

            // The cost of going from start to start is zero.
            gScore[start] = 0;

            // For each node, the total cost of getting from the start node to the goal
            // by passing by that node. That value is partly known, partly heuristic.

            var fScore = new Dictionary<T, float>(); // fScore:= map with default value of Infinity


            // For the first node, that value is completely heuristic.
            fScore[start] = heuristic_cost_estimate(start, goal);

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(f => fScore.GetOrDefault(f, float.PositiveInfinity)).First();
                // := the node in openSet having the lowest fScore[] value
                if (current.Equals(goal))
                    return reconstruct_path(cameFrom, current);

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var n in GetNeighbours(current, goal))
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

        public abstract IEnumerable<T> GetNeighbours(T current, T finalGoal);

        public abstract float exact_cost(T start, T neighbour);

        public abstract float heuristic_cost_estimate(T start, T goal);



        public List<T> reconstruct_path(Dictionary<T, T> cameFrom, T current)
        {
            var total_path = new List<T>();
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