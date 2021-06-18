using System;
using System.Collections.Generic;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;
using UnityEngine;
using UnityEngine.Analytics;

[MainPathsInAreasGuaranteed]
public class PathLengthCounter : PipelineStep
{
    // Start is called before the first frame update
    public override Type[] RequiredGuarantees => new[] {typeof(MainPathsInAreasGuaranteed)};

    public override GameWorld Apply(GameWorld world)
    {
        IEnumerable<IGameWorldObject> children = world.Root.GetChildrenInChildren();
        float sum = 0;

        foreach (IGameWorldObject child in children)
        {
            if (child is MainPath)
            {
                float length = ((OwLine) child.Shape).Length();
                sum += length;
            }
        }

        Debug.Log($"PathLengthSum: {sum}");
        AnalyticsResult result = Analytics.CustomEvent("GesamtePfadlänge", new Dictionary<string, object>()
        {
            {"InGameUnits", sum}
        });
        Debug.Log(result);

        return world;
    }
}