using Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline;
using UnityEngine;

namespace Demo.Pipeline
{
    public class ActivateAllChildrenUponGameManagerStart : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (GameManager.gameHasStarted)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }
}
