using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Pipeline.Standard;
using Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline.PlayerCharacter;
using UnityEngine;
using UnityEngine.Analytics;
using Image = UnityEngine.UI.Image;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline
{
    public class GameManager : MonoBehaviour
    {
        public GameObject clearedLevelText;
        public string gameIsolationType;
        public GameObject pipelineManagerObject;
        public Image progressCircleImage;
        
        public static int uniqueAreasAmount;
        public static int foundAreas;
        public static bool gameHasStarted;

        public static DataPack analyitcsData { get; private set; }
        public static HashSet<Color> landMarkAreaColors;
        public static HashSet<string> landMarkAreaSilhouettes;
        public static int casual;

        private void Start()
        {
            StartCoroutine(nameof(StartNewGame));
        }

        private void Update()
        {
            if (gameHasStarted)
            {
                if (foundAreas == uniqueAreasAmount)
                {
                    clearedLevelText.SetActive(true);
                }

                analyitcsData.timeAll += Time.deltaTime;
            }
        }

        public IEnumerator StartNewGame()
        {
            
            analyitcsData = new DataPack()
            {
                type = gameIsolationType
            };
            landMarkAreaColors = new HashSet<Color>();
            landMarkAreaSilhouettes = new HashSet<string>();
            
            StandardPipelineManager manager = pipelineManagerObject.GetComponent<StandardPipelineManager>();
            ConnectedAreaTrigger.SetAllToFalse();
            manager.Seed = Environment.TickCount;
            analyitcsData.seed = manager.Seed;
            manager.Setup();
            yield return StartCoroutine(manager.Generate());

            //assign progress canvas/image to every trigger
            ConnectedAreaTrigger[] triggers = pipelineManagerObject.GetComponentsInChildren<ConnectedAreaTrigger>();
            foreach (ConnectedAreaTrigger trigger in triggers)
            {
                trigger.progressCircleImage = progressCircleImage;
            }

            gameHasStarted = true;

           
            foundAreas = 0;
        }

        public static void CleanUpForNewGame()
        {
            analyitcsData.colorsAmount = landMarkAreaColors.Count;
            analyitcsData.silhouettesAmount = landMarkAreaSilhouettes.Count;
            analyitcsData.landmarkTypesAmount = uniqueAreasAmount;
            analyitcsData.casual = casual;
            SendAnalyticsData();
            //reset some variables for new data
            landMarkAreaColors = new HashSet<Color>();
            landMarkAreaSilhouettes = new HashSet<string>();
            foundAreas = -1;
            uniqueAreasAmount = -2;
            gameHasStarted = false;
        }

        public static void SendAnalyticsData()
        {
            AnalyticsResult result = Analytics.CustomEvent("playRunData",
                new Dictionary<string, object>() {{"d", analyitcsData.ToCsvString()}});
            Debug.Log(analyitcsData.ToCsvString());
            Debug.Log(result);
        }
    }
}