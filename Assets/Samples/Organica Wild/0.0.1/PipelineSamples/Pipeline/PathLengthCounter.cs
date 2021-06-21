using System;
using System.Collections.Generic;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline
{
    [MainPathsInAreasGuaranteed]
    public class PathLengthCounter : PipelineStep
    {
        // Start is called before the first frame update
        public override Type[] RequiredGuarantees => new[] {typeof(MainPathsInAreasGuaranteed)};

        public override GameWorld Apply(GameWorld world)
        {
            IEnumerable<IGameWorldObject> children = world.Root.GetChildrenInChildren();
            float sum = 0;
            int i = 0;

            foreach (IGameWorldObject child in children)
            {
                if (child is MainPath)
                {
                    float length = ((OwLine) child.Shape).Length();
                    sum += length;
                    i++;
                }
            }

            GameManager.analyitcsData.pathSum = sum;
            GameManager.analyitcsData.pathAverage = sum / i;
        
            return world;
        }
    }
}