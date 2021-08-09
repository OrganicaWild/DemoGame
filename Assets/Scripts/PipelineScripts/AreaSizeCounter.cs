using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Standard.PipeLineSteps;
using g3;
using UnityEngine;
using UnityEngine.Analytics;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline
{
    [AreasPlacedGuarantee]
    public class AreaSizeCounter : PipelineStep
    {
        public override GameWorld Apply(GameWorld world)
        {
            var childrenInChildren = world.Root.GetChildren();
            var areas = childrenInChildren.Where(x =>
                x.GetType() == typeof(Area) || x.GetType() == typeof(Area)).ToList();

            double sum = 0.0;
            Dictionary<string, object> eventValues = new Dictionary<string, object>();

            int index = 0;
            foreach (Area area in areas)
            {
                OwPolygon shapePolygon = area.GetShape();
                List<GeneralPolygon2d> polygons = shapePolygon.Getg3Polygon();
                foreach (GeneralPolygon2d generalPolygon2d in polygons)
                {
                    double size = generalPolygon2d.Area;
                    eventValues.Add($"area{index}size", size);
                    sum += size;
                    index++;
                }
            }

            //index is correct value for division because it gets increment after last area
            double average = sum / index;

            GameManager.analyitcsData.areaSizeAverage = average;

            return world;
        }

        public override Type[] RequiredGuarantees => new[] {typeof(AreasPlacedGuarantee)};
    }
}