using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline.Standard.PipeLineSteps;
using Framework.Util;
using UnityEngine;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline
{
    [MainPathsInAreasGuaranteed]
    public class WorldMSTMainPathStep : PipelineStep
    {
        public override Type[] RequiredGuarantees => new Type[] {typeof(LandmarksPlacedGuarantee)};
        public override GameWorld Apply(GameWorld world)
        {
            var allAreas = world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>();

            var areaCenters = new List<OwPoint>();

            foreach (AreaTypeAssignmentStep.TypedArea typedArea in allAreas)
            {
                //add mst inside of each area
                var allLandmarksOfArea = typedArea.GetAllChildrenOfType<Landmark>();
                var allLandmarkCenters = allLandmarksOfArea.Select(x => new OwPoint(x.Shape.GetCentroid()));

                var mst = MinimumSpanningTree.ByDistance(allLandmarkCenters.ToList());

                foreach (OwLine line in mst)
                {
                    typedArea.AddChild(new MainPath(line));
                }

                Vector2 sum = new Vector2();
                sum = allLandmarkCenters.Aggregate(sum, (current, allLandmarkCenter) => current + allLandmarkCenter.Position);

                Vector2 avg = sum / allLandmarkCenters.Count();
                
                areaCenters.Add(new OwPoint(avg));
            }

            var worldMst = MinimumSpanningTree.ByDistance(areaCenters);

            foreach (OwLine owLine in worldMst)
            {
                
            }

            return world;
        }
    }
}