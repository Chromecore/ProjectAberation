using UnityEngine;

namespace Chromecore
{
    public class ContextSteering : MonoBehaviour
    {
		[SerializeField] private LayerMask layerMask;
		[SerializeField] private int angle = 30;
		[SerializeField] private float checkDistance = 1f;

		private WeightedRay[] rays;
		private WeightedRay lowestWeightRay;
		private Vector2 combineDirection = Vector3.zero;
		private Vector2 moveDirection = Vector3.zero;

		private void Awake()
		{
			rays = new WeightedRay[360 / angle];
			for (int i = 0; i < rays.Length; i++)
			{
				float x = Mathf.Cos(i * angle * Mathf.PI / 180f);
				float y = Mathf.Sin(i * angle * Mathf.PI / 180f);
				rays[i] = new WeightedRay();
				rays[i].ray = new Ray2D(transform.position, new Vector2(x, y));
			}

			lowestWeightRay = new WeightedRay();
			lowestWeightRay.weight = Mathf.NegativeInfinity;
		}

		public Vector2 Steer(Vector2 target)
		{
			// cast rays
			combineDirection = Vector2.zero;
			Vector2 playerDir = (target - (Vector2)transform.position).normalized;
			WeightedRay highestWeightedRay = lowestWeightRay;
			for (int i = 0; i < rays.Length; i++)
			{
				rays[i].ray.origin = transform.position;
				rays[i].weight = Mathf.Clamp(Vector3.Dot(playerDir, rays[i].ray.direction), 0, 1);

				RaycastHit hit;
				Physics.Raycast(rays[i].ray.origin, rays[i].ray.direction, out hit, checkDistance, layerMask);
				if (hit.collider)
				{
					rays[i].weight = -1 + hit.distance / checkDistance;
				}

				rays[i].weight = Mathf.Clamp(rays[i].weight, -1, 1);

				if(rays[i].weight > highestWeightedRay.weight)
				{
					highestWeightedRay = rays[i];
				}

				// find the combine direction of the rays
				if (rays[i].weight >= 0)
				{
					combineDirection += rays[i].ray.direction * rays[i].weight;
				}
			}
			moveDirection = highestWeightedRay.ray.direction;
			moveDirection.Normalize();

			return moveDirection;
		}

		private void OnDrawGizmos() // todo remove on finish
		{
			if (!Application.isPlaying) return;

			Gizmos.color = Color.black;
			Gizmos.DrawWireSphere(transform.position, 0.25f);

			foreach (WeightedRay weightedRay in rays)
			{
				Gizmos.color = weightedRay.weight < 0 ? Color.red : Color.green;

				Gizmos.DrawRay(weightedRay.ray.origin + weightedRay.ray.direction * 0.25f, weightedRay.ray.direction * Mathf.Abs(weightedRay.weight) * 0.25f);
			}

			Gizmos.color = Color.blue;
			Gizmos.DrawRay((Vector2)transform.position + moveDirection * 0.25f, moveDirection * 0.25f);
		}

		class WeightedRay
		{
			public Ray2D ray;
			public float weight = 0;
		}
	}
}