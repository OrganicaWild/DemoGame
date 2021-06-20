using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class LoadScene : MonoBehaviour
{
    public string[] sceneNames;
    private List<int> order = new List<int>();

    private void Start()
    {
        if (order.Count == 0)
        {
            order = new List<int>();
            for (int i = 0; i < sceneNames.Length; i++)
            {
                order.Add(i);
            }
            
            
            Random rng = new Random();
            order = order.OrderBy(a => rng.NextDouble()).ToList();
            
        }

        string nameOfSceneToLoad =  sceneNames[order.First()];
        order.RemoveAt(0);

        SceneManager.LoadScene(nameOfSceneToLoad);
    }
}