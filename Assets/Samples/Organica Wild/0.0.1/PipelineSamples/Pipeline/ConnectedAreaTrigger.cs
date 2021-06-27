using System;
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
        public GameObject toSpawn;
        public Vector3 spawnPoint;
        public float secondsToWait;
        public Image progressCircleImage;

        private float timeWaiting;
        private bool activated;
        private bool standingInside;
        
        private void OnTriggerEnter(Collider other)
        {
            standingInside = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (activated)
            {
                progressCircleImage.fillAmount = 1;
                return;
            }
            
            float fillPercent = timeWaiting / secondsToWait;
            progressCircleImage.fillAmount = fillPercent;
            timeWaiting += Time.deltaTime;
            
            //found
            if (timeWaiting > secondsToWait)
            {
                if (!GameManager.activatedGroups[partOfGroupX])
                {
                    progressCircleImage.fillAmount = fillPercent;
                    GameManager.activatedGroups[partOfGroupX] = true;
                    Instantiate(toSpawn, spawnPoint, Quaternion.identity);
                    activated = true;
                    GameManager.foundAreas++;
                }
                else
                {
                    //false activation
                    if (!activated && standingInside)
                    {
                        Debug.Log($"failed activation of {partOfGroupX}");
                        GameManager.analyitcsData.failedActivations++;
                        standingInside = false;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            standingInside = false;
            timeWaiting = 0;
            progressCircleImage.fillAmount = 0;
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