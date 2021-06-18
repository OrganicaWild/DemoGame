using System;
using System.Collections.Generic;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using g3;
using UnityEngine;
using UnityEngine.Analytics;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples
{
    [AreasPlacedGuarantee]
    public class AreaSizeCounter : PipelineStep
    {
        public override GameWorld Apply(GameWorld world)
        {
            IEnumerable<Area> ares = world.Root.GetAllChildrenOfType<Area>();

            double sum = 0.0;
            Dictionary<string, object> eventValues = new Dictionary<string, object>();

            int index = 0;
            foreach (Area area in ares)
            {
                OwPolygon shapePolygon = area.Shape as OwPolygon;
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
            eventValues.Add("Durchschnitt", average);
            eventValues.Add("Summe", sum);

            AnalyticsResult result = Analytics.CustomEvent("GebietsGrösse", eventValues);
            Debug.Log(result);
            
            return world;
        }

        public override Type[] RequiredGuarantees => new[] {typeof(AreasPlacedGuarantee)};
    }
}