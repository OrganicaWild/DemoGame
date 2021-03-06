using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Pipeline.Standard;

using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace GameScripts
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
        private bool readyToPlay;
        public static int uniqueLevels;

        public static DataPack analyitcsData { get; private set; }
        public static HashSet<Color> landMarkAreaColors;
        public static HashSet<string> landMarkAreaSilhouettes;
        public static bool[] activatedGroups;
        public static int casual;

        public GameObject scoreboardText;
        public Text scoreText;
        public GameObject startBoard;
        public GameObject commentInput;
        public Camera startCamera;
        public GameObject goButton;
        public GameObject loadingText;
        public Text progression;
        public static bool isFirst;

        private void Start()
        {
            progression.text = $"{uniqueLevels - IntermediarySceneManager.order.Count}/{uniqueLevels}";
            startBoard.SetActive(true);

            if (isFirst)
            {
                commentInput.SetActive(false);
                isFirst = false;
            }

            StartCoroutine(nameof(StartNewGame));
        }

        private void Update()
        {
            if (gameHasStarted)
            {
                Destroy(startCamera);
                scoreText.text = $"{foundAreas}/{uniqueAreasAmount} \nOrtspaare gefunden/ location pairs found\n";

                if (foundAreas == uniqueAreasAmount)
                {
                    clearedLevelText.SetActive(true);
                }

                analyitcsData.timeAll += Time.deltaTime;
            }
            else if (readyToPlay)
            {
                loadingText.SetActive(false);
                goButton.SetActive(true);
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

            if (manager.Seed == 0)
            {
                manager.Seed = Environment.TickCount;
                analyitcsData.seed = manager.Seed;
            }
            else
            {
                analyitcsData.seed = manager.Seed;
            }

            do
            {
                manager.Setup();
                yield return StartCoroutine(manager.Generate());
                
                //if no error --> continue, else set new seed and try again
                if (!StandardPipelineRunner.EncounteredError) continue;
                
                manager.Seed = Environment.TickCount;
                analyitcsData.seed = manager.Seed;
                
            } while (StandardPipelineRunner.EncounteredError);

           
          

            //assign progress canvas/image to every trigger
            ConnectedAreaTrigger[] triggers = pipelineManagerObject.GetComponentsInChildren<ConnectedAreaTrigger>();
            foreach (ConnectedAreaTrigger trigger in triggers)
            {
                trigger.progressCircleImage = progressCircleImage;
            }

            readyToPlay = true;
            foundAreas = 0;
            activatedGroups = new bool[uniqueAreasAmount];

            yield return null;
        }

        public static void CleanUpForNewGame()
        {
            analyitcsData.colorsAmount = landMarkAreaColors.Count;
            analyitcsData.silhouettesAmount = landMarkAreaSilhouettes.Count;
            analyitcsData.landmarkTypesAmount = uniqueAreasAmount;
            analyitcsData.casual = casual;
            SendComment.seed = analyitcsData.seed;
            SendComment.type = analyitcsData.type;
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

        public void StartGame()
        {
            readyToPlay = false;
            gameHasStarted = true;
            startBoard.SetActive(false);
            scoreboardText.SetActive(true);
        }
    }
}