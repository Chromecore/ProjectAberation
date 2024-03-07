using Sirenix.OdinInspector;
using UnityEngine;

namespace Chromecore
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHealth : HealthEntity
	{
        [Title("Player Health")]
		[SerializeField] private LayerMask damageMask;
        [SerializeField, Unit(Units.Second)] private float iTime;

        [Title("References")]
		[SerializeField, Required] private Collider2D mainCollider;

        private float timeSinceLastHit;

        private void Reset() 
        {
            mainCollider = GetComponent<Collider2D>();
        }

        private void OnCollisionStay2D(Collision2D other) 
        {
            if (GameManager.gamePaused) return;
            bool isDamageInMask = (damageMask & (1 << other.gameObject.layer)) != 0;
            if (isDamageInMask && timeSinceLastHit >= iTime)
			{
                timeSinceLastHit = 0;
                float damage = 1;
                if(other.gameObject.TryGetComponent<EnemyAI>(out EnemyAI enemyAI))
                {
                    damage = enemyAI.attackDamage;
                }
				Damage(damage);
                isInvincible = true;
			}
        }

        private void Update() 
        {
            if (GameManager.gamePaused) return;
            timeSinceLastHit += Time.deltaTime;
            if(timeSinceLastHit >= iTime) isInvincible = false;
        }

        public override void Kill()
        {
            base.Kill();
            GameManager.instance.PlayerDeath();
        }
    }
}