using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chromecore
{
    public class PlayerFeedbacks : MonoBehaviour
    {
		[Title("Feedbacks")]
		[SerializeField] private MMF_Player shootFeedback;
		[SerializeField] private MMF_Player jumpFeedback;
        [SerializeField] private MMF_Player landFeedback;
        [SerializeField] private MMF_Player damageFeedback;
        [SerializeField] private MMF_Player healFeedback;
        [SerializeField] private MMF_Player killFeedback;

        [Title("References")]
		[SerializeField, Required] private PlayerMovement playerMovement;
		[SerializeField, Required] private PlayerShoot playerShoot;
		[SerializeField, Required] private PlayerHealth playerHealth;

		private void Start()
		{
			playerShoot.shootEvent += () => { shootFeedback?.PlayFeedbacks(); };
			playerMovement.jumpEvent += () => { jumpFeedback?.PlayFeedbacks(); };
			playerMovement.landEvent += () => { landFeedback?.PlayFeedbacks(); };
            playerHealth.damageEvent += () => { damageFeedback?.PlayFeedbacks(); };
            playerHealth.healEvent += () => { healFeedback?.PlayFeedbacks(); };
            playerHealth.killEvent += () => { killFeedback?.PlayFeedbacks(); };
		}
	}
}