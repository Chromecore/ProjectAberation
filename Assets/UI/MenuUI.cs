using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chromecore
{
    public class MenuUI : MonoBehaviour
    {
        [Title("UI Elements")]
        [SerializeField, Required] private GameObject titleText;
        [SerializeField, Required] private GameObject controlsText;
        [SerializeField, Required] private GameObject errorText;
        [SerializeField, Required] private TMP_Text scoreText;

        private bool controlsOpen;
        private bool errorOpen;

        private void Start() 
        {
            scoreText.text = GameManager.score.ToString();
        }

        public void ErrorHit()
        {
            LoadGame();
        }

        public void ControlsButtonHit()
        {
            controlsOpen = !controlsOpen;
            controlsText.SetActive(controlsOpen);
            titleText.SetActive(!controlsOpen && !errorOpen);
            errorText.SetActive(!controlsOpen && errorOpen);
        }

        public void TitleHit()
        {
            errorOpen = true;
            titleText.SetActive(false);
            errorText.SetActive(true);
            scoreText.gameObject.SetActive(false);
        }

        private void LoadGame()
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
}