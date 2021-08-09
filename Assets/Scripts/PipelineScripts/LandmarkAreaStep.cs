using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.PipeLineSteps;
using Framework.Poisson_Disk_Sampling;
using GameScripts;
using Polybool.Net.Objects;
using UnityEngine;

namespace PipelineScripts
{
    [LandmarksPlacedGuarantee]
    public class LandmarkAreaStep : PipelineStep
    {
        [Range(0, 1)] public float landMarkIsAreaPercentage;
        public int areaXTimes = 2;
        public int maxCircleSize;
        public int minCircleSize;
        public int circleResolution;
        public float radiusP;
        public int sizeP;
        public int rejectionP;
        public int safetyMaxTries;

        public decimal epsilon = 0.0000000000000001m;

        public override Type[] RequiredGuarantees => new Type[] {typeof(LandmarksPlacedGuarantee)};

        public override GameWorld Apply(GameWorld world)
        {
            Epsilon.Eps = epsilon;

            IEnumerable<Area> areas =
                world.Root.GetAllChildrenOfType<Area>();

            List<Area> areasWithLandmarks =
                areas.Where(area => area.HasAnyChildrenOfType<Landmark>() && area.Type != "start" && area.Type != "end")
                    .ToList();
            int areasWithLandmarksSum = (int) (areasWithLandmarks.Count() * landMarkIsAreaPercentage);
            int pairs = areasWithLandmarksSum / areaXTimes;

            GameManager.uniqueAreasAmount = pairs;

            //create pairs / triples / quadruples etc..
            for (int pair = 0; pair < pairs; pair++)
            {
                //create unique Landmark
                Area[] chosenAreas = GetConnectedChosenAreas(areasWithLandmarks);
                Landmark[] chosenLandmarks = new Landmark[areaXTimes];
                OwPolygon[] movedCircle = new OwPolygon[areaXTimes];

                for (int index = 0; index < areaXTimes; index++)
                {
                    List<Landmark> allLandmarksInArea = chosenAreas[index].GetAllChildrenOfType<Landmark>().ToList();
                    int ind = (int) (random.NextDouble() * (allLandmarksInArea.Count - 1));
                    chosenLandmarks[index] =
                        allLandmarksInArea[ind];
                }

                OwPolygon uniqueShape = GetUniqueShape(chosenAreas, chosenLandmarks);

                for (int index = 0; index < areaXTimes; index++)
                {
                    movedCircle[index] = new OwPolygon(uniqueShape.representation);
                    movedCircle[index].MovePolygon(chosenLandmarks[index].GetShape().GetCentroid());
                }

                for (int index = 0; index < areaXTimes; index++)
                {
                    Landmark chosenLandmarkI = chosenLandmarks[index];
                    OwPolygon movedCircleI = movedCircle[index];
                    chosenLandmarkI.SetShape(movedCircleI);
                    chosenLandmarkI.Type = $"landmarkPair{pair}";
                    areasWithLandmarks.Remove(chosenAreas[index]);

                    List<Landmark> allLandmarksInArea = chosenAreas[index].GetAllChildrenOfType<Landmark>().ToList();
                    allLandmarksInArea.Remove(chosenLandmarkI);

                    foreach (Landmark otherLandmark in allLandmarksInArea)
                    {
                        if (otherLandmark.GetShape() is OwPoint)
                        {
                            if (PolygonPointInteractor.Use().Contains(movedCircleI, otherLandmark.GetShape() as OwPoint))
                            {
                                chosenAreas[index].RemoveChild(otherLandmark);
                            }
                        }
                    }
                }
            }

            return world;
        }

        private Area[] GetConnectedChosenAreas(
            List<Area> areasWithLandmarks)
        {
            int[] areaIndices = new int[areaXTimes];

            if (areasWithLandmarks.Count == areaXTimes)
            {
                for (int i = 0; i < areaXTimes; i++)
                {
                    areaIndices[i] = i;
                }
            }
            else
            {
                do
                {
                    for (int i = 0; i < areaXTimes; i++)
                    {
                        areaIndices[i] = (int) (random.NextDouble() * (areasWithLandmarks.Count - 1));
                    }
                } while (areaIndices.Distinct().Count() != areaIndices.Length);
            }

            Area[] chosenAreas = new Area[areaXTimes];
            for (int i = 0; i < areaXTimes; i++)
            {
                chosenAreas[i] = areasWithLandmarks[areaIndices[i]];
            }

            return chosenAreas;
        }

        private OwPolygon GetUniqueShape(Area[] chosenAreas, Landmark[] chosenLandmarks)
        {
            IEnumerable<Vector2> points = PoissonDiskSampling.GeneratePoints(radiusP, sizeP, sizeP, rejectionP, random);
            OwPolygon baseCircle = new OwCircle(Vector2.zero, 1f, circleResolution);

            foreach (Vector2 point in points)
            {
                int circleSize = (int) (random.NextDouble() * (maxCircleSize - minCircleSize) + minCircleSize);
                OwCircle circle = new OwCircle(point, circleSize, circleResolution);
                baseCircle = PolygonPolygonInteractor.Use().Union(baseCircle, circle);

                //bool[] inside = new bool[areaXTimes];


                //if all areas can contain this point at given landmark, move forward with this shape
                /*if (inside.All(x => x))
            {*/
                //baseCircle = newCircle;
                /*}*/
            }

            for (int i = 0; i < areaXTimes; i++)
            {
                Vector2 landmarkPos = chosenLandmarks[i].GetShape().GetCentroid();
                baseCircle.MovePolygon(landmarkPos);
                baseCircle = PolygonPolygonInteractor.Use()
                    .Intersection(chosenAreas[i].GetShape(), baseCircle);
            }

            return baseCircle;
        }
    }
}