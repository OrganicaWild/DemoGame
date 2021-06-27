﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline
{
    [RequireComponent(typeof(Collider))]
    public class ConnectedAreaTrigger : MonoBehaviour
    {
        public int partOfGroupX;
        public static Dictionary<int, bool> groupsActivated = new Dictionary<int, bool>();

        public GameObject toSpawn;
        public Vector3 spawnPoint;
        public float secondsToWait;
        private float timeSinceActivated;
        private bool spawnedThing;
        public Image progressCircleImage;

        private void Start()
        {
            if (!groupsActivated.ContainsKey(partOfGroupX))
            {
                groupsActivated.Add(partOfGroupX, false);
            }
            
        }

        private void OnTriggerStay(Collider other)
        {
            var fillPercent = timeSinceActivated / secondsToWait;

            progressCircleImage.fillAmount = fillPercent;
            timeSinceActivated += Time.deltaTime;

            if (timeSinceActivated > secondsToWait)
            {
                if (!groupsActivated[partOfGroupX])
                {
                    progressCircleImage.fillAmount = 0;
                    groupsActivated[partOfGroupX] = true;
                    Instantiate(toSpawn, spawnPoint, Quaternion.identity);
                    spawnedThing = true;
                    GameManager.foundAreas++;
                }
                else if (spawnedThing != true)
                {
                    Debug.Log("failed Activate");
                    GameManager.analyitcsData.failedActivations++;
                    spawnedThing = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            progressCircleImage.fillAmount = 0;
            if (timeSinceActivated < secondsToWait)
            {
                groupsActivated[partOfGroupX] = false;
                timeSinceActivated = 0;
            }
        }

        private void OnDestroy()
        {
        }

        public void SetImage(Image image)
        {
            progressCircleImage = image;
        }

        public static void SetAllToFalse()
        {
            groupsActivated.Clear();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0, 0, .5f);
            if (spawnPoint != Vector3.zero)
            {
                Gizmos.DrawCube(spawnPoint, Vector3.one);
#if UNITY_EDITOR
                Handles.Label(spawnPoint + new Vector3(0, 1, 0), $"{partOfGroupX}", new GUIStyle() {fontSize = 32});
#endif
            }
        }
    }
}