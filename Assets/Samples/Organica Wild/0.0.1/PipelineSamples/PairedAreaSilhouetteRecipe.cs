using System;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Standard.ThemeApplicator.Recipe;
using Framework.Util;
using UnityEngine;
using Random = System.Random;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples
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
            Random localRandom = new Random(individual.Type.Sum(x => x));
            var number = int.Parse(individual.Type.Replace("landmarkPair", ""));

            OwPolygon polygon = individual.Shape as OwPolygon;
            if (polygon == null)
            {
                return new GameObject();
            }

            GameObject mesh;
            try
            {
                /*Material floorMaterial = floorMaterials[(int) (localRandom.NextDouble() * (floorMaterials.Length - 1))];*/
                if (floorMaterials.Length == 1)
                {
                    mesh = GameObjectCreation.CombineMeshesFromPolygon(polygon, floorMaterials[0]);
                }
                else
                {
                    Material floorMaterial = floorMaterials[number + 1];
                    mesh = GameObjectCreation.CombineMeshesFromPolygon(polygon, floorMaterial);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mesh = new GameObject();
            }

            Rect boundingBox = polygon.GetBoundingBox();
            BoxCollider collider = mesh.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = (new Vector3(boundingBox.center.x, 0, boundingBox.center.y));
            collider.size = new Vector3(boundingBox.width, 20, boundingBox.height);

            ConnectedAreaTrigger connectedAreaTrigger = mesh.AddComponent<ConnectedAreaTrigger>();
            string groupString = individual.Type.Replace("landmarkPair", "");
            connectedAreaTrigger.partOfGroupX = int.Parse(groupString);
            connectedAreaTrigger.toSpawn = toSpawn;
            connectedAreaTrigger.spawnPoint = new Vector3(boundingBox.center.x, 2, boundingBox.center.y);
            connectedAreaTrigger.secondsToWait = secondsToSpawn;

            int numberOfDefiningPrefabs = (int) (localRandom.NextDouble() * (maxPrefabs - 1) + 1);
            Debug.Log(numberOfDefiningPrefabs);
            if (numberOfDefiningPrefabs < 3)
            {
                if (numberOfDefiningPrefabs == 1)
                {
                    GameObject prefab =
                        silhouetteDefiningPrefabs[
                            (int) (localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                    GameObject instantiated = GameObjectCreation.InstantiatePrefab(prefab, polygon.GetCentroid());
                    instantiated.transform.parent = mesh.transform;
                }
                else
                {
                    //maxPrefabs = 2
                    Vector2 randomVector = Vector2Extensions.GetRandomNormalizedVector(localRandom);
                    randomVector *= radius;

                    GameObject prefab =
                        silhouetteDefiningPrefabs[
                            (int) (localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                    GameObject instantiated =
                        GameObjectCreation.InstantiatePrefab(prefab, polygon.GetCentroid() + randomVector);
                    instantiated.transform.parent = mesh.transform;

                    GameObject prefab1 =
                        silhouetteDefiningPrefabs[
                            (int) (localRandom.NextDouble() * (silhouetteDefiningPrefabs.Length - 1))];
                    GameObject instantiated1 =
                        GameObjectCreation.InstantiatePrefab(prefab1, polygon.GetCentroid() - randomVector);
                    instantiated1.transform.parent = mesh.transform;
                }
            }
            else
            {
                OwCircle circle = new OwCircle(polygon.GetCentroid(), radius, numberOfDefiningPrefabs);

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
                OwCircle circle = new OwCircle(polygon.GetCentroid(), minRadius + radiusPerRing * i, ringResolution);
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
    }
}