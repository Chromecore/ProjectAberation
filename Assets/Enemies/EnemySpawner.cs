using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chromecore
{
    public class EnemySpawner : MonoBehaviour
    {
        [Title("Spawning")]
        [SerializeField] private bool spawningEnabled;
        [SerializeField, Unit(Units.Second)] private float lengthOfSpawnTimeCurve;
        [SerializeField] private AnimationCurve spawnTimeCurve;

        [Title("References")]
        [SerializeField, Required, AssetsOnly, AssetList(AssetNamePrefix = "Enemy")] 
        private List<EnemyAI> enemyPrefabs;
        [SerializeField, Required] private Transform player;

        private Dictionary<int, Queue<EnemyAI>> enemyPool;
        private Dictionary<int, int> enemyCount;
		
        private float spawnTime;
		private float timeSinceLastSpawn;

		private void Awake()
		{
			enemyPool = new();
            enemyCount = new();
		}

		private void Update()
		{
            if (GameManager.gamePaused) return;
            if(!spawningEnabled) return;

            spawnTime = Time.timeSinceLevelLoad > spawnTimeCurve.keys.Last().time * lengthOfSpawnTimeCurve ?
                spawnTimeCurve.Evaluate(spawnTimeCurve.keys.Last().time) :
                spawnTimeCurve.Evaluate(Time.timeSinceLevelLoad / lengthOfSpawnTimeCurve);
            if(GameManager.instance.doubleEnemiesCode) spawnTime /= 2;

			timeSinceLastSpawn += Time.deltaTime;
			if(timeSinceLastSpawn >= spawnTime)
			{
				timeSinceLastSpawn = 0;
				SpawnEnemy();
			}
		}

		private void SpawnEnemy()
        {
            // find all possible enemies
            List<EnemyAI> enemySelectionList = enemyPrefabs.FindAll((enemyAI) => 
                {
                    return GetEnemyCount(enemyAI.ID) < enemyAI.maxCount;
                });
            
            // set enemy data
            EnemyAI enemyAI = enemySelectionList[Random.Range(0, enemySelectionList.Count)];
            ChangeEnemyCount(enemyAI.ID, 1);

            // get enemy
			EnemyAI newEnemyAI = GetEnemy(enemyAI);

			// set position
			newEnemyAI.transform.position = GetRandomPosition();
		}

        private Vector2 GetRandomPosition()
        {
            return new Vector2(
                Random.Range(GameManager.instance.roomMinX, GameManager.instance.roomMaxX),
                Random.Range(GameManager.instance.roomMinY, GameManager.instance.roomMaxY)
                );
        }

        private void ChangeEnemyCount(int ID, int value)
        {
            if(enemyCount.ContainsKey(ID))
            {
                enemyCount[ID] += value;
            }
            else
            {
                enemyCount.Add(ID, 1);
            }
        }

        private int GetEnemyCount(int ID)
        {
            if(enemyCount.ContainsKey(ID))
            {
                return enemyCount[ID];
            }
            else
            {
                enemyCount.Add(ID, 0);
                return 0;
            }
        }

		private EnemyAI GetEnemy(EnemyAI enemyAI)
		{
			EnemyAI newEnemyAI;
			if (enemyPool.ContainsKey(enemyAI.ID) && enemyPool[enemyAI.ID].Count > 0)
			{
				newEnemyAI = enemyPool[enemyAI.ID].Dequeue();
				newEnemyAI.gameObject.SetActive(true);
                newEnemyAI.ResetEnemy();
			}
			else
			{
				newEnemyAI = Instantiate(enemyAI, transform);
				newEnemyAI.target = player;
				newEnemyAI.GetComponent<EnemyHealth>().destroyEvent += () => { EnemyDied(newEnemyAI); };
			}
			return newEnemyAI;
		}

		private void EnemyDied(EnemyAI enemyAI)
		{
            // pool enemy
            if(!enemyPool.ContainsKey(enemyAI.ID))
            {
                enemyPool.Add(enemyAI.ID, new());
            }
			enemyPool[enemyAI.ID].Enqueue(enemyAI);
            ChangeEnemyCount(enemyAI.ID, -1);
			enemyAI.gameObject.SetActive(false);
		}
	}
}