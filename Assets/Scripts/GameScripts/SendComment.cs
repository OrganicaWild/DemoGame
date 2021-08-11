using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

namespace GameScripts
{
    public class SendComment : MonoBehaviour
    {
        public static int seed;
        public static string type;
        public InputField inputField;

        // Start is called before the first frame update
        public void Send()
        {
            string comment = inputField.text;
            if (comment.Length > 0)
            {
                AnalyticsResult result =Analytics.CustomEvent("levelComment", new Dictionary<string, object>()
                {
                    {"seed", seed},
                    {"type", type},
                    {"c", comment}
                });
                Debug.Log(result);
                inputField.text = "";
            }
        }
    }
}