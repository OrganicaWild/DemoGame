using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipeLineSteps;
using UnityEngine;

[LandmarksPlacedGuarantee]
public class SimpleAreaStep : PipelineStep
{
    public override Type[] RequiredGuarantees => new Type[0];

    public override GameWorld Apply(GameWorld world)
    {

        //Area outer = new Area(new OwRectangle(Vector2.zero, Vector2.one * 10), "area");
        
        Area outer = new Area(new OwCircle(Vector2.zero, 50f, 50));
        Area inner = new Area(new OwCircle(Vector2.zero, 10f, 20), "test");
        Area inner1 = new Area(new OwCircle(new Vector2(0, 12), 10f, 20), "test");
        Landmark landmark = new Landmark(new OwPoint(Vector2.zero));
        Landmark landmark1 = new Landmark(new OwPoint(new Vector2(0,12)));
        inner.AddChild(landmark);
        inner1.AddChild(landmark1);
        outer.AddChild(inner);
        outer.AddChild(inner1);

        return world = new GameWorld(outer);;
    }
}