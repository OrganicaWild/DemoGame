using UnityEngine;

namespace GameScripts
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
