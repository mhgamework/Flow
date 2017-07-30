using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Flow.WardDrawing;

public class WardMatcherTest
{

    private Vector3 a = new Vector3(0, 0, 0);
    private Vector3 b = new Vector3(1, 1, 1);
    private Vector3 aOff = new Vector3(50, 50, 50);
    private Vector3 bOff = new Vector3(51, 51, 51);
    private Vector3 c = new Vector3(-2, 2, 2);
    private Vector3 d = new Vector3(3, 3, 3);
    [Test]
    public void TestMatchSimpleLine()
    {
        var matcher = new WardComparer();

        var x = create(create(a, b));
        var target = create(create(a, b));


        var matches = matcher.Match(x, target);

        Assert.IsNotEmpty(matches);

        Assert.IsTrue(matches[0].ExactMatch);

    }
    [Test]
    public void TestMatchSimpleWrongLine()
    {
        var matcher = new WardComparer();

        var x = create(create(a, c));
        var target = create(create(a, b));


        var matches = matcher.Match(x, target);

        Assert.IsEmpty(matches);
    }

    [Test]
    public void TestMatchSimpleLine_Reversed()
    {
        var matcher = new WardComparer();

        var x = create(create(b, a));
        var target = create(create(a, b));

        var matches = matcher.Match(x, target);

        Assert.IsNotEmpty(matches);

        Assert.IsTrue(matches[0].ExactMatch);

    }

    [Test]
    public void TestMatchSimpleLineOffset()
    {
        var matcher = new WardComparer();

        var x = create(create(aOff, bOff));
        var target = create(create(a, b));


        var matches = matcher.Match(x, target);

        Assert.IsNotEmpty(matches);

        Assert.IsTrue(matches[0].ExactMatch);

    }
    [Test]
    public void TestMatchSimpleLineOffsetReversed()
    {
        var matcher = new WardComparer();

        var x = create(create(bOff, aOff));
        var target = create(create(a, b));


        var matches = matcher.Match(x, target);

        Assert.IsNotEmpty(matches);

        Assert.IsTrue(matches[0].ExactMatch);

    }

    [Test]
    public void TestMatchTwoLines()
    {

        var matcher = new WardComparer();

        var x = create(create(a, b), create(a, c));
        var target = create(create(a, b), create(a, c));


        var matches = matcher.Match(x, target);

        Assert.IsNotEmpty(matches);

        Assert.IsTrue(matches[0].ExactMatch);

    }
    [Test]
    public void TestMatchTwoLines_OneCorrect()
    {
        var matcher = new WardComparer();

        var x = create(create(a, d), create(a, c));
        var target = create(create(a, c), create(a, b));


        var matches = matcher.Match(x, target);

        Assert.IsNotEmpty(matches);

        Assert.IsFalse(matches[0].ExactMatch);
        Assert.AreEqual(1, matches[0].MatchingXLines.Count);

    }

    [Test]
    public void TestMatchLoopWithDifferentStartingPoint()
    {
        var matcher = new WardComparer();

        var x = create(create(b, c, a, b));
        var target = create(create(a, b, c, a));


        var matches = matcher.Match(x, target);

        Assert.IsNotEmpty(matches);

        Assert.IsTrue(matches[0].ExactMatch);

    }
    [Test]
    public void TestMatchLoopWithDifferentStartingPointReverse()
    {
        var matcher = new WardComparer();

        var x = create(create(b,a,c,b));
        var target = create(create(a, b, c, a));


        var matches = matcher.Match(x, target);

        Assert.IsNotEmpty(matches);

        Assert.IsTrue(matches[0].ExactMatch);

    }

    private List<Vector3> create(params Vector3[] line)
    {
        return line.ToList();
    }
    private List<List<Vector3>> create(params List<Vector3>[] lines)
    {
        return lines.ToList();
    }

    //// A UnityTest behaves like a coroutine in PlayMode
    //// and allows you to yield null to skip a frame in EditMode
    //[UnityTest]
    //public IEnumerator WardMatcherTestWithEnumeratorPasses()
    //{
    //    // Use the Assert class to test conditions.
    //    // yield to skip a frame
    //    yield return null;
    //}
}
