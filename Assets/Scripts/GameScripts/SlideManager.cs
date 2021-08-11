using UnityEngine;

namespace GameScripts
{
    public class SlideManager : MonoBehaviour
    {
        public GameObject[] slides;
        private int currentSlide = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                NextSlide();
            }
        }

        public void NextSlide()
        {
            if (currentSlide < slides.Length-1)
            {
                slides[currentSlide].SetActive(false);
                slides[currentSlide + 1].SetActive(true);
                currentSlide++;
            }
        }
    }
}