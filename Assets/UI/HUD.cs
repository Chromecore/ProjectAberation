using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Chromecore
{
    public class HUD : MonoBehaviour
    {
        [Title("UI")]
        [SerializeField, Required] private TMP_Text scoreText;
        [SerializeField, Required] private TMP_Text healthText;

        [Title("References")]
        [SerializeField, Required] private PlayerHealth playerHealth;

        private void Start() {
            playerHealth.damageEvent += DisplayPlayerHealth;
            playerHealth.healEvent += DisplayPlayerHealth;
            DisplayPlayerHealth();
            DisplayScore();
        }

        private void Update() {
            DisplayScore();
        }

        private void DisplayPlayerHealth(){
            healthText.text = playerHealth.health.ToString();
        }

        private void DisplayScore(){
            scoreText.text = GameManager.instance.score.ToString();
        }
    }
}