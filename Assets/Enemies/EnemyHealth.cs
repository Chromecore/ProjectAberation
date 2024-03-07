using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Chromecore
{
	[RequireComponent(typeof(Collider2D))]
    public class EnemyHealth : HealthEntity
	{
		[Title("Enemy Health")]
		[SerializeField] private LayerMask damageMask;

		[Title("References")]
		[SerializeField, Required] private Collider2D mainCollider;

		private void Reset()
		{
			mainCollider = GetComponent<Collider2D>();
		}

		private void OnParticleCollision(GameObject other)
		{
			if ((damageMask & (1 << other.layer)) != 0)
			{
                float damage = 1;
                float.TryParse(other.tag.Split(":")[1], out damage);
				Damage(damage);
			}
		}

		public override void Kill()
		{
			base.Kill();
			mainCollider.enabled = false;
			StartCoroutine(DestroyEnemy());
		}

		public IEnumerator DestroyEnemy()
		{
			yield return new WaitForSeconds(delayKillTime);
			InvokeDestroyEvent();
		}

        public override void ResetHealth()
        {
            base.ResetHealth();
            mainCollider.enabled = true;
        }
	}
}