using GameScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline
{
    public class LoadSceneOnTriggerEnter : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (GameManager.foundAreas >= GameManager.uniqueAreasAmount)
            {
                GameManager.CleanUpForNewGame();
                SceneManager.LoadScene("IntermediaryScene");
            }
        }
    }
}