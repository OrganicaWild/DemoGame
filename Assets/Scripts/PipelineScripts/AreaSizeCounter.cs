using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using g3;
using GameScripts;


namespace PipelineScripts
{
    [AreasPlacedGuarantee]
    public class AreaSizeCounter : PipelineStep
    {
        public override GameWorld Apply(GameWorld world)
        {
            IEnumerable<IGameWorldObject> childrenInChildren = world.Root.GetChildren();
            List<IGameWorldObject> areas = childrenInChildren.Where(x =>
                x.GetType() == typeof(Area) || x.GetType() == typeof(Area)).ToList();

            double sum = 0.0;

            int index = 0;
            foreach (Area area in areas)
            {
                double size  = area.GetShape().GetArea();
                sum += size;
                index++;
            }

            //index is correct value for division because it gets increment after last area
            double average = sum / index;

            GameManager.analyitcsData.areaSizeAverage = average;

            return world;
        }

        public override Type[] RequiredGuarantees => new[] {typeof(AreasPlacedGuarantee)};
    }
}