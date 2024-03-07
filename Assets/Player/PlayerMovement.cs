using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Threading;

namespace Chromecore
{
	[SelectionBase, RequireComponent(typeof(Rigidbody2D))]
	public class PlayerMovement : MonoBehaviour
	{
		[Title("Movement")]
		[SerializeField] private float movementSpeed;
		[SerializeField] private float movementSpeedInAir;
		[SerializeField] private float rotateSmoothing;

		[Title("Jump")]
		[SerializeField] private float jumpHeight;
		[SerializeField] private float jumpTimeIntervals;
		[SerializeField, Unit(Units.Second)] private float maxJumpBufferTime;
		[SerializeField] private AnimationCurve jumpCurve;

		[Title("Dash")]
		[SerializeField] private float dashSpeed;
		[SerializeField] private float dashTimeIntervals;
		[SerializeField] private AnimationCurve dashCurve;

		[Title("Knockback")]
		[SerializeField] private float knockbackDecay;
		[SerializeField] private float knockbackLength;
		[SerializeField] private float knockbackAirPower;

		[Title("Input")]
		[SerializeField] private float controllerDeadZone;

		[Title("References")]
		[SerializeField, Required] private Rigidbody2D body;
		[SerializeField, Required] private Collider2D playerCollider;
		[SerializeField, Required] private Transform shadowSprite;
		[SerializeField, Required] private Camera mainCamera;

		// look
		private Vector2 look;
		private bool isMouseInput;

		// move
		private Vector2 move;
		private Vector2 knockback;
		private float groundToAir = 50;

		// jump
		private float jumpLandY;
		private float jumpDisplacement;
		private bool isJumping;
		private bool isJumpBuffered;
		private float jumpBufferedTime;

		// callbacks
		public event DefaultCallback jumpEvent;
		public event DefaultCallback landEvent;

		private void Reset()
		{
			body = GetComponent<Rigidbody2D>();
			playerCollider = GetComponent<Collider2D>();
		}

		private void Start()
		{
			InputHandler.Instance.playerActions.Jump.performed += HandleJumpCallback;
			InputHandler.Instance.playerActions.Look.performed += ctx => HandleLookInput(ctx.ReadValue<Vector2>(), ctx);
		}

        private void OnDestroy() 
        {
            InputHandler.Instance.playerActions.Jump.performed -= HandleJumpCallback;
			InputHandler.Instance.playerActions.Look.performed -= ctx => HandleLookInput(ctx.ReadValue<Vector2>(), ctx);
        }

		private void Update()
		{
            if (GameManager.gamePaused)
            {
                body.velocity = Vector2.zero;
                return;
            }
			move = InputHandler.Instance.playerActions.Movement.ReadValue<Vector2>();
			HandleJumpBuffer();
		}

		private void HandleJumpBuffer()
		{
			if (isJumpBuffered)
			{
				if (!isJumping) HandleJump();

				jumpBufferedTime += Time.deltaTime;
				if (jumpBufferedTime > maxJumpBufferTime) isJumpBuffered = false;
			}
		}

		private void HandleJumpCallback(InputAction.CallbackContext _)
		{
            if (GameManager.gamePaused) return;
            if(GameManager.instance.noJumpCode) return;
			HandleJump();
		}

        private void HandleJump()
		{
			if (isJumping)
			{
				isJumpBuffered = true;
				jumpBufferedTime = 0;
				return;
			}
			isJumpBuffered = false;
			StartCoroutine(Jump());
		}

		private IEnumerator Jump()
		{
			isJumping = true;
			jumpEvent?.Invoke();
			ToggleCollisions(false);

			jumpLandY = transform.position.y;
			float time = 0;
			do
			{
				time += jumpTimeIntervals;
				jumpDisplacement = jumpCurve.Evaluate(time) * jumpHeight;
				yield return new WaitForFixedUpdate();
			} while (time <= 1 && jumpDisplacement != 0);
			jumpDisplacement = 0;

			ToggleCollisions(true);
			isJumping = false;
			landEvent?.Invoke();
		}

		private void ToggleCollisions(bool toggle)
		{
			playerCollider.enabled = toggle;
		}

		private void FixedUpdate()
		{
            if (GameManager.gamePaused) return;
			HandleLook();
			HandleMovement();
		}

		private void HandleMovement()
		{
			// speed
			float currentSpeed = isJumping ? movementSpeedInAir : movementSpeed;
			currentSpeed *= isJumping ? 1 : groundToAir;

			// update the jump land y
			jumpLandY += move.y * currentSpeed * Time.fixedDeltaTime;

			// jump
			Vector2 jumpVector = body.position;
			jumpVector.y = jumpLandY + jumpDisplacement;
			
			// main movement
			Vector2 moveVector = Vector2.zero;
			moveVector += move * currentSpeed * Time.fixedDeltaTime;

			if (isJumping)
			{
				moveVector = body.position;
				moveVector.y = jumpLandY + jumpDisplacement;
				moveVector += move * currentSpeed * Time.fixedDeltaTime;
				body.MovePosition(moveVector + knockback / groundToAir * knockbackAirPower);
			}
			else
			{
				body.velocity = moveVector + Vector2.up * jumpDisplacement + knockback;
			}

			// set shadow sprite position
			Vector3 shadowSpritePosition = transform.position;
			shadowSpritePosition.y -= jumpDisplacement;
			shadowSprite.position = shadowSpritePosition;
		}

		private void HandleLookInput(Vector2 look, InputAction.CallbackContext ctx)
		{
			this.look = look;
			isMouseInput = ctx.control.parent.ToString().Contains("Mouse");
		}

		private void HandleLook()
		{
			if (look.sqrMagnitude == 0) return;
			if (isMouseInput)
			{
				// rotate the player to face the mouse
				Vector2 mousePos = mainCamera.ScreenToWorldPoint(look);
				Vector2 lookDirection = mousePos - body.position;
				float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
				body.rotation = angle;
			}
			else
			{
				// rotate with the stick
				body.rotation = Vector2.SignedAngle(Vector2.up, look);
			}
		}

		public void AddKnockback(float amount)
		{
			StartCoroutine(Knockback(amount));
		}

		private IEnumerator Knockback(float amount)
		{
			Vector2 back = -transform.up;
			knockback = back * amount;
			for (int i = 0; i < knockbackLength; i++)
			{
				yield return new WaitForFixedUpdate();
				if (knockback.sqrMagnitude > 0) knockback -= -knockback * knockbackDecay;
			}
			knockback = Vector3.zero;
		}
	}
}