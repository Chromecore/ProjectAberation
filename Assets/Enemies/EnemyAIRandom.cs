using UnityEngine;

namespace Chromecore
{
    public class EnemyAIRandom : EnemyAI
    {
        private Vector2 targetPosition;

        protected override void Start() 
        {
            base.Start();
            GetRandomPosition();
        }

        protected override void HandleMovement()
        {
            Vector2 selfPosition = transform.position;
            Vector2 moveDirection = (targetPosition - selfPosition).normalized;
            moveDirection = contextSteering.Steer(moveDirection + selfPosition);
            const float speedScaler = 0.01f;
            transform.position += (Vector3)moveDirection * speed * speedScaler;
        }

        private void GetRandomPosition()
        {
            targetPosition = new Vector2(
                Random.Range(GameManager.instance.roomMinX, GameManager.instance.roomMaxX), 
                Random.Range(GameManager.instance.roomMinY, GameManager.instance.roomMaxY));
        }
    }
}