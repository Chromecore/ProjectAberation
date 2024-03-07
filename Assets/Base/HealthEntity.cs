using Sirenix.OdinInspector;
using UnityEngine;

namespace Chromecore
{
    public abstract class HealthEntity : MonoBehaviour
    {
        [Title("Health")]
        [SerializeField] public bool isInvincible;
        [SerializeField, LabelText("Health"), ShowIf("@!isInvincible")] protected float maxHealth;
		[SerializeField, Unit(Units.Second)] protected float delayKillTime;

		public event DefaultCallback healEvent;
		public event DefaultCallback damageEvent;
		public event DefaultCallback killEvent;
		public event DefaultCallback destroyEvent;

		public float health { get; private set; }
		protected bool dead { get; private set; }

		private void Awake()
		{
			ResetHealth();
		}

		public virtual void Damage(float amount)
        {
			if (amount <= 0 || dead || isInvincible) return;

			health -= amount;
			damageEvent?.Invoke();
			if (health <= 0)
			{
				Kill();
			}
		}

        public virtual void Heal(float amount)
        {
            if (amount <= 0 || dead) return;

            health += amount;
            healEvent?.Invoke();
			if (health > maxHealth) health = maxHealth;
		}

        public virtual void Kill()
        {
			dead = true;
			health = 0;
			killEvent?.Invoke();
		}

		protected void InvokeDestroyEvent()
		{
			destroyEvent?.Invoke();
		}

		public virtual void ResetHealth()
		{
			health = maxHealth;
			dead = false;
		}
	}
}