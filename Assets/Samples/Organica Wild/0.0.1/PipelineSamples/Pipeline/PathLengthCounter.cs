using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Util.Miscellanous;
using UnityEngine;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline
{
    [MainPathsInAreasGuaranteed]
    public class PathLengthCounter : PipelineStep
    {
        // Start is called before the first frame update
        public override Type[] RequiredGuarantees => new[] {typeof(MainPathsInAreasGuaranteed)};

        public override GameWorld Apply(GameWorld world)
        {
            List<Area> areas = world.Root.GetAllChildrenOfType<Area>().ToList();
            int verticesCount = areas.Count;
            int[,] adjMatrix = new int[verticesCount, verticesCount];

            //initialize all connections as really long
            for (int j = 0; j < verticesCount; j++)
            {
                for (int k = 0; k < verticesCount; k++)
                {
                    if (k == j)
                    {
                        adjMatrix[j, k] = 0;
                    }
                    else
                    {
                        adjMatrix[j, k] = FloydWarshall.INF;
                    }
                }
            }

            //set all existing connections
            for (int index = 0; index < areas.Count; index++)
            {
                Area area = areas[index];
                IEnumerable<AreaConnection> connections = area.GetAllChildrenOfType<AreaConnection>();
                foreach (AreaConnection areaConnection in connections)
                {
                    Area target = areaConnection.Target;
                    int targetIndex = areas.IndexOf(target);
                    float distanceToConnection =
                        (areaConnection.Shape.GetCentroid() - area.Shape.GetCentroid()).magnitude;
                    float distanceFromConnection =
                        (areaConnection.Shape.GetCentroid() - target.Shape.GetCentroid()).magnitude;
                    float distance = distanceFromConnection + distanceToConnection;
                    adjMatrix[index, targetIndex] = (int) Math.Floor(distance);
                    adjMatrix[targetIndex, index] = (int) Math.Floor(distance);
                }
            }

            int[,] distances = FloydWarshall.Execute(adjMatrix, verticesCount);

            IEnumerable<int> allWithAreaLandmarkIndexes = areas
                .Where(area => area.GetAllChildrenOfType<Landmark>().Any(landmark => landmark.Shape is OwPolygon))
                .Select(area => areas.IndexOf(area));
            
            int sumOfDistances = 0;
            int number = 0;
            foreach (int first in allWithAreaLandmarkIndexes)
            {
                foreach (int second in allWithAreaLandmarkIndexes)
                {
                    if (first != second)
                    {
                        int distanceBetween = distances[first, second];
                        sumOfDistances += distanceBetween;
                        number++;
                    }
                }
            }

            float averageOfDistances = sumOfDistances / (float) number;
            Debug.Log(averageOfDistances);
            GameManager.analyitcsData.averageAreaDistance = averageOfDistances;

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