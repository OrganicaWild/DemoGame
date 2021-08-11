using System;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Standard.ThemeApplicator.Recipe;
using Framework.Util;
using GameScripts;
using UnityEngine;
using Random = System.Random;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline
{
    [CreateAssetMenu(fileName = "PairedAreaSilhouetteRecipe", menuName = "Pipeline/PairedAreaSilhouetteRecipe",
        order = 0)]
    public class PairedAreaSilhouetteRecipe : GameWorldObjectRecipe
    {
        public Material[] floorMaterials;
        public GameObject[] silhouetteDefiningPrefabs;
        public int maxPrefabs;
        public int radius;

        public GameObject[] flowerPrefabs;

        public int rings;
        public float minRadius;
        public float maxRadius;
        public int ringResolution;
        public float staticScaleFactor;

        public GameObject toSpawn;
        public float secondsToSpawn;

        public override GameObject Cook(IGameWorldObject individual)
        {
            if (individual.GetShape() is OwPolygon shapePolygon)
            {
                Random localRandom = new Random(individual.Identifier.Sum(x => x));
                var number = int.Parse(individual.Identifier.Replace("landmarkPair", ""));

                string arrayContents = "";
                foreach (GameObject silhouetteDefiningPrefab in silhouetteDefiningPrefabs)
                {
                    arrayContents += silhouetteDefiningPrefab.name;
                }

                GameManager.landMarkAreaSilhouettes.Add(arrayContents);

                GameObject mesh;
                try
                {
                    /*Material floorMaterial = floorMaterials[(int) (localRandom.NextDouble() * (floorMaterials.Length - 1))];*/
                    if (floorMaterials.Length == 1)
                    {
                        mesh = GameObjectCreation.CombineMeshesFromPolygon(shapePolygon, floorMaterials[0]);
                    }
                    else
                    {
                        Material floorMaterial = floorMaterials[number + 1];
                        Color color = floorMaterial.color;
                        GameManager.landMarkAreaColors.Add(color);
                        mesh = GameObjectCreation.CombineMeshesFromPolygon(shapePolygon, floorMaterial);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    mesh = new GameObject();
                }

                Rect boundingBox = shapePolygon.GetBoundingBox();
                BoxCollider collider = mesh.AddComponent<BoxCollider>();
                collider.isTrigger = true;
                collider.center = (new Vector3(boundingBox.center.x, 0, boundingBox.center.y));
                collider.size = new Vector3(boundingBox.width, 20, boundingBox.height);

                ConnectedAreaTrigger connectedAreaTrigger = mesh.AddComponent<ConnectedAreaTrigger>();
                string groupString = individual.Identifier.Replace("landmarkPair", "");
                connectedAreaTrigger.partOfGroupX = int.Parse(groupString);
                connectedAreaTrigger.toSpawn = toSpawn;
                connectedAreaTrigger.spawnPoint = new Vector3(boundingBox.center.x, 2, boundingBox.center.y);
                connectedAreaTrigger.secondsToWait = secondsToSpawn;

                int numberOfDefiningPrefabs = (int) (localRandom.NextDouble() * (maxPrefabs - 1) + 1);

                if (numberOfDefiningPrefabs < 3)
                {
                    Vector2 randomVector = Vector2Extensions.GetRandomNormalizedVector(localRandom);
                    randomVector *= radius;

                    if (numberOfDefiningPrefabs == 1)
                    {
                        GameObject prefab =
                            silhouetteDefiningPrefabs[
                                (int) (localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                        GameObject instantiated =
                            GameObjectCreation.InstantiatePrefab(prefab, shapePolygon.GetCentroid() + randomVector);
                        instantiated.transform.parent = mesh.transform;
                    }
                    else
                    {
                        //maxPrefabs = 2
                        GameObject prefab =
                            silhouetteDefiningPrefabs[
                                (int) (localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                        GameObject instantiated =
                            GameObjectCreation.InstantiatePrefab(prefab, shapePolygon.GetCentroid() + randomVector);
                        instantiated.transform.parent = mesh.transform;

                        GameObject prefab1 =
                            silhouetteDefiningPrefabs[
                                (int) (localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                        GameObject instantiated1 =
                            GameObjectCreation.InstantiatePrefab(prefab1, shapePolygon.GetCentroid() - randomVector);
                        instantiated1.transform.parent = mesh.transform;
                    }
                }
                else
                {
                    OwCircle circle = new OwCircle(shapePolygon.GetCentroid(), radius, numberOfDefiningPrefabs);

                    foreach (Vector2 point in circle.GetPoints())
                    {
                        GameObject prefab =
                            silhouetteDefiningPrefabs[
                                (int) (localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                        GameObject instantiated = GameObjectCreation.InstantiatePrefab(prefab, point);
                        instantiated.transform.parent = mesh.transform;
                    }
                }

                float radiusPerRing = (maxRadius - minRadius) / rings;

                for (int i = 1; i <= rings; i++)
                {
                    OwCircle circle = new OwCircle(shapePolygon.GetCentroid(), minRadius + radiusPerRing * i,
                        ringResolution);
                    float scale = (maxRadius - i * radiusPerRing) * staticScaleFactor;
                    //float scale = 1;

                    foreach (Vector2 point in circle.GetPoints())
                    {
                        Vector2 offset =
                            (Vector2Extensions.GetRandomNormalizedVector(localRandom) - new Vector2(0.5f, 0.5f)) /
                            (maxRadius - i * radiusPerRing);
                        Vector2 position = point;
                        position += offset;

                        GameObject prefab = flowerPrefabs[(int) (localRandom.NextDouble() * flowerPrefabs.Length)];
                        GameObject instantiated =
                            GameObjectCreation.InstantiatePrefab(prefab,
                                position);
                        instantiated.transform.parent = mesh.transform;
                        instantiated.transform.localScale = prefab.transform.localScale * scale;
                    }
                }

                return mesh;
            }

            return new GameObject();
        }
    }
}