using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chromecore
{
    public class GameManager : MonoBehaviour
    {
        // singleton
		public static GameManager instance;

		private void Awake()
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

        public float roomMinX;
        public float roomMaxX;
        public float roomMinY;
        public float roomMaxY;
        [HideInInspector] public float score;

        public void Reset()
        {
            SceneManager.LoadScene(0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Vector3 size = new Vector3(roomMaxX - roomMinX, roomMaxY - roomMinY);
            Vector3 center = new Vector3((roomMaxX - roomMinX)/2, (roomMaxY - roomMinY)/2);
            Gizmos.DrawWireCube(center, size);
        }
    }
}