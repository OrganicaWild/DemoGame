using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace GameScripts
{
    public class IntermediarySceneManager : MonoBehaviour
    {
        public string[] sceneNames;
        public static List<int> order = new List<int>();
        public bool instant;

        private void Start()
        {
            if(instant) NextScene();
            GameManager.uniqueLevels = sceneNames.Length;
        }

        public void NextScene()
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
                GameManager.uniqueLevels = order.Count;
            }

            string nameOfSceneToLoad =  sceneNames[order.First()];
            order.RemoveAt(0);

            SceneManager.LoadScene(nameOfSceneToLoad);
        }

        public void SetCasualAndNextScene(int casual)
        {
            GameManager.casual = casual;
            GameManager.isFirst = true;
            NextScene();
        }
        
    }
}