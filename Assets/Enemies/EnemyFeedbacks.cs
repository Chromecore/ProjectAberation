using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Chromecore
{
    public class EnemyFeedbacks : MonoBehaviour
    {
		[Title("Feedbacks")]
		[SerializeField] private MMF_Player damageFeedback;

        [Title("References")]
		[SerializeField, Required] private ParticleSystem killParticles;
		[SerializeField, Required] private ParticleSystem spawnParticles;
		[SerializeField, Required] private SpriteRenderer spriteRenderer;
		[SerializeField, Required] private EnemyHealth enemyHealth;

		private void Start()
		{
			enemyHealth.damageEvent += () => { damageFeedback?.PlayFeedbacks(); };
			enemyHealth.killEvent += () => { StartCoroutine(Kill()); };
			spawnParticles.Play();
		}

		private IEnumerator Kill()
		{
			spriteRenderer.enabled = false;
			killParticles.Play();
			yield return new WaitForSeconds(killParticles.main.duration);
            spriteRenderer.enabled = true;
		}
	}
}