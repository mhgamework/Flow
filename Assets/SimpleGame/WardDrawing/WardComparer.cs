﻿using DirectX11;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.SimpleGame.WardDrawing
{
    public class WardComparer
    {
        public List<PartialMatch> Match(List<List<Point3>> x, List<List<Point3>> target)
        {
            x = normalizeShape(x);
            target = normalizeShape(target);
            var offsets = new List<Point3>();
            foreach (var l in x)
            {
                // Use each line as starting point
                // Ignoring rotations


                foreach (var k in target)
                {
                    Point3 offset;
                    if (!matchLine(l, k, out offset)) continue;
                    offsets.Add(offset);
                }
            }

            var ret = new List<PartialMatch>();

            foreach (var offset in offsets)
            {
                var pMatch = new PartialMatch();
                var matches = new List<List<Point3>>();
                // Rate the offset
                foreach (var l in x)
                {
                    if (!hasMatch(l, target, offset)) continue;
                    matches.Add(l);
                }
                //Was: pMatch.ExactMatch = matches.Count == x.Count;
                pMatch.ExactMatch = matches.Count == target.Count;
                pMatch.MatchingXLines = matches;
                pMatch.X = x;
                pMatch.Target = target;
                ret.Add(pMatch);

            }

            return ret;
        }

        private static List<List<Point3>> normalizeShape(List<List<Point3>> x)
        {
            x = x.Where(k => k.Count > 1).ToList();

            return x;

        }

        private bool hasMatch(List<Point3> x, List<List<Point3>> target, Point3 offset)
        {
            foreach (var l in target)
            {
                if (matchLineWithOffset(x, l, offset)) return true;
            }
            return false;
        }

        private bool matchLineWithOffset(List<Point3> x, List<Point3> targetLine, Point3 offset)
        {
            if (x.Count != targetLine.Count) return false;
            if (isLoop(x) && isLoop(targetLine))
            {
                int loopOffset = 0;
                // Loop matching is gonna be hard
                foreach (var l in targetLine)
                {
                    var failed = false;
                    // Assume l matches on x
                    for (int i = 0; i < (x.Count-1); i++)
                    {
                        if (match(x[i] + offset, targetLine[(i + loopOffset) % (targetLine.Count-1)])) continue;
                        failed = true;
                        break;

                    }
                    if (!failed) return true;
                    loopOffset++;
                }
                // Try reverse direction!
                foreach (var l in targetLine)
                {
                    var failed = false;
                    // Assume l matches on x
                    for (int i = 0, j = targetLine.Count - 1; i < x.Count; i++, j--)
                    {
                        if (match(x[i] + offset, targetLine[(j + loopOffset) % (targetLine.Count - 1)])) continue;
                        failed = true;
                        break;

                    }
                    if (!failed) return true;
                    loopOffset++;
                }
                return false;
            }

            if (match(x[0] + offset, targetLine[0]))
            {
                for (int i = 0; i < x.Count; i++)
                {
                    if (!match(x[i] + offset, targetLine[i])) return false;

                }
                return true;
            }
            if (match(x[0] + offset, targetLine[targetLine.Count - 1]))
            {
                // Reverse mode
                for (int i = 0, j = targetLine.Count - 1; i < x.Count; i++, j--)
                {
                    if (!match(x[i] + offset, targetLine[j])) return false;
                }
                return true;

            }
            return false;
        }


        private bool matchLine(List<Point3> x, List<Point3> target, out Point3 offset)
        {
            offset = target[0] - x[0];

            var loopX = isLoop(x);
            var loopY = isLoop(target);
            if (loopX != loopY) return false;
            if (loopX)
            {
                // Loop, we cant know which point of x matches to which point of target
                // But since we are looking for identical loops, choose a random starting point in x
                //  then try to match to every point in target
                foreach (var p in target)
                {
                    offset = p - x[0];
                    if (matchLineWithOffset(x, target, offset)) return true; // TODO should matchwithline understand looping?

                }
            }

            if (matchLineWithOffset(x, target, offset)) return true;
            // Try reverse
            offset = target.Last() - x[0];

            if (matchLineWithOffset(x, target, offset)) return true;

            return false;
        }

        private bool match(Point3 x, Point3 y)
        {
            return (x.ToVector3() - y.ToVector3()).sqrMagnitude < 0.001;
        }

        private bool isLoop(List<Point3> x)
        {
            return match(x[0], x[x.Count - 1]);
        }


        public class PartialMatch
        {
            public List<List<Point3>> X;
            public List<List<Point3>> Target;
            public Vector3 Offset;
            public List<List<Point3>> MatchingXLines;
            public bool ExactMatch;
        }
    }
}