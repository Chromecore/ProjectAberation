using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chromecore
{
    public class GameManager : MonoBehaviour
    {
        // singleton
		public static GameManager instance;
        private void SetupSingleton()
        {
            if (instance != null)
			{
				Destroy(gameObject);
				return;
			}
			instance = this;

			int numGameManagers = FindObjectsOfType<GameManager>().Length;

			if (numGameManagers > 1)
			{
				Destroy(gameObject);
			}
        }

        [Title("XP")]
        [SerializeField, Required] private Transform xPCollector;
        [SerializeField, Required] private ParticleSystem xpParticlePrefab;
        [SerializeField, Required] private Transform xpParent;

        [Title("Room")]
        public float roomMinX;
        public float roomMaxX;
        public float roomMinY;
        public float roomMaxY;

        [Title("Game Over")]
        [SerializeField, Unit(Units.Second)] private float timeToLoadMenu;
        [SerializeField, Required] private GameObject deathText;

        public static float score;
        public static bool gamePaused;
        [HideInInspector] public bool noJumpCode;
        [HideInInspector] public bool doubleEnemiesCode;
        private Queue<ParticleSystem> xpParticlePool;

        public void PlayerDeath()
        {
            gamePaused = true;
            deathText.SetActive(true);
            StartCoroutine(LoadMainMenu());
        }

        private IEnumerator LoadMainMenu(){
            yield return new WaitForSeconds(timeToLoadMenu);
            gamePaused = false;
            SceneManager.LoadScene(0);
        }

        private void Awake()
		{
            gamePaused = false;
			SetupSingleton();
            score = 0;
            xpParticlePool = new();
            deathText.SetActive(false);
		}

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Vector3 size = new Vector3(roomMaxX - roomMinX, roomMaxY - roomMinY);
            Vector3 center = new Vector3((roomMaxX - roomMinX)/2, (roomMaxY - roomMinY)/2);
            center.x += roomMinX;
            center.y += roomMinY;
            Gizmos.DrawWireCube(center, size);
        }

        public void SpawnXPParticle(Vector2 position, int amount)
        {
            // get particle system
            ParticleSystem xpParticle;
            if (xpParticlePool.Count > 0)
            {
                xpParticle = xpParticlePool.Dequeue();
                xpParticle.Clear();
            }
            else
            {
                xpParticle = Instantiate(xpParticlePrefab, xpParent);
                xpParticle.trigger.AddCollider(xPCollector);
            }

            // set position
            xpParticle.transform.position = position;

            // set amount
            ParticleSystem.Burst burst = xpParticle.emission.GetBurst(0);
            burst.count = amount;
            xpParticle.emission.SetBurst(0, burst);

            // start particles
            xpParticle.gameObject.SetActive(true);
            xpParticle.Play();
            StartCoroutine(ReturnToXPPool(xpParticle));
        }

        private IEnumerator ReturnToXPPool(ParticleSystem xpParticle)
        {
            yield return new WaitForSeconds(xpParticle.main.startLifetime.constantMax);
            xpParticle.Stop();
            xpParticle.gameObject.SetActive(false);
            xpParticlePool.Enqueue(xpParticle);
        }
    }
}