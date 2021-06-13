using System;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.Standard.ThemeApplicator.Recipe;
using Framework.Util;
using UnityEngine;
using Random = System.Random;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples
{
    [CreateAssetMenu(fileName = "PairedAreaSilhouetteRecipe", menuName = "Pipeline/PairedAreaSilhouetteRecipe", order = 0)]
    public class PairedAreaSilhouetteRecipe : GameWorldObjectRecipe
    {
        public Material floorMaterial;
        public GameObject[] silhouetteDefiningPrefabs;
        public int maxPrefabs;
        public int radius;
        
        public GameObject[] flowerPrefabs;
        public Vector2Int size;
        public Vector2 distance;
        public float minScale;
        public float maxScale;
        
        public override GameObject Cook(IGameWorldObject individual)
        {
            OwPolygon polygon = individual.Shape as OwPolygon;
            GameObject mesh;
            try
            {
                mesh = GameObjectCreation.CombineMeshesFromPolygon(polygon, floorMaterial);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mesh = new GameObject();
            }

            Random localRandom = new Random(individual.Type.Sum(x => x));

            var numberOfDefiningPrefabs = (int) (localRandom.NextDouble() * maxPrefabs);  
            
            if (numberOfDefiningPrefabs < 3)
            {
                if (numberOfDefiningPrefabs == 1)
                {
                    var prefab = silhouetteDefiningPrefabs[(int)(localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                    var instantiated = GameObjectCreation.InstantiatePrefab(prefab, polygon.GetCentroid());
                    instantiated.transform.parent = mesh.transform;
                }
                else
                {
                    //maxPrefabs = 2
                    var randomVector = Vector2Extensions.GetRandomNormalizedVector(localRandom);
                    randomVector *= radius;
                    
                    var prefab = silhouetteDefiningPrefabs[(int)(localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                    var instantiated = GameObjectCreation.InstantiatePrefab(prefab, polygon.GetCentroid() + randomVector);
                    instantiated.transform.parent = mesh.transform;
                    
                    var prefab1 = silhouetteDefiningPrefabs[(int)(localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                    var instantiated1 = GameObjectCreation.InstantiatePrefab(prefab1, polygon.GetCentroid() - randomVector);
                    instantiated1.transform.parent = mesh.transform;
                }
            }
            else
            {
                var circle = new OwCircle(polygon.GetCentroid(), radius, numberOfDefiningPrefabs);

                foreach (var point in circle.GetPoints())
                {
                    var prefab = silhouetteDefiningPrefabs[(int)(localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                    var instantiated = GameObjectCreation.InstantiatePrefab(prefab, point);
                    instantiated.transform.parent = mesh.transform;
                }
            }
            
            float[,] noiseMap =
                PerlinNoise.GenerateNoiseMap(localRandom, size.x, size.y, 20, 6, 2, 1, Vector2.zero);
            Vector2 center = individual.Shape.GetCentroid();

            Vector2 start = center - size / 2;
            Vector2 xStep = new Vector2(distance.x, 0);
            Vector2 yStep = new Vector2(0, distance.y);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    float noiseValue = noiseMap[x, y];
                    
                    Vector2 pos = start + x * xStep + y * yStep;
                    Vector2 offset = Vector2Extensions.GetRandomNormalizedVector(localRandom) / distance;
                    pos += offset;

                    bool isContained =
                        PolygonPointInteractor.Use().Contains(polygon, new OwPoint(pos));

                    if (isContained )//&& noiseValue > 0.5)
                    {
                        float scale = (float) (localRandom.NextDouble() * (maxScale - minScale) + minScale);
                        
                        GameObject instantiated =
                            GameObjectCreation.InstantiatePrefab(flowerPrefabs[(int) (localRandom.NextDouble() * flowerPrefabs.Length)],
                                pos);
                        instantiated.transform.parent = mesh.transform;
                        instantiated.transform.localScale = new Vector3(scale, scale, scale);
                    }
                }
            }
            
            return mesh;
        }
    }
}