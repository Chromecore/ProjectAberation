using Sirenix.OdinInspector;
using UnityEngine;

namespace Chromecore
{
	[SelectionBase, RequireComponent(typeof(EnemyHealth), typeof(ContextSteering))]
    public class EnemyAI : MonoBehaviour
    {
        [Title("Attributes")]
        [SerializeField] public int ID;
        [SerializeField] public float speed;
        [SerializeField] public float points;
        [SerializeField] public float attackDamage;

        [Title("Spawning")]
        [SerializeField] public int maxCount;

        [Title("References")]
		[SerializeField, Required] private EnemyHealth enemyHealth;
		[SerializeField, Required] private SpriteRenderer spriteRenderer;
        [SerializeField, Required] protected ContextSteering contextSteering;

        public Transform target { protected get; set; }

		private void Reset()
		{
			enemyHealth = GetComponent<EnemyHealth>();
            contextSteering = GetComponent<ContextSteering>();
		}

        protected virtual void Start() 
        {
            enemyHealth.killEvent += ResetEnemy;
            enemyHealth.killEvent += AddPoints;

            ResetEnemy();
        }

        private void Update() 
        {
            HandleMovement();
        }

        protected virtual void HandleMovement()
        {
            Vector2 moveDirection = (target.position - transform.position).normalized;
            moveDirection = contextSteering.Steer(moveDirection + (Vector2)transform.position);
            const float speedScaler = 0.01f;
            transform.position += (Vector3)moveDirection * speed * speedScaler;
        }

        public void AddPoints()
        {
            GameManager.instance.score += points;
        }

		public void ResetEnemy()
        {
            enemyHealth.ResetHealth();
		}
	}
}