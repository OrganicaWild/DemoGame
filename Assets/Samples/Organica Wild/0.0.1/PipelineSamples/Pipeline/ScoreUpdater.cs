using UnityEngine;
using UnityEngine.UI;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline
{
    public class ScoreUpdater : MonoBehaviour
    {
        public Text scoreText;

        private int prevScore = -1;

        // Update is called once per frame
        void Update()
        {
            int totalAreas = GameManager.uniqueAreasAmount;
            int foundAreas = GameManager.foundAreas;

           
        }
    }
}