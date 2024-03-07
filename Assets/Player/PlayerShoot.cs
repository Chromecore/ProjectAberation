using Sirenix.OdinInspector;
using UnityEngine;

namespace Chromecore
{
	[RequireComponent(typeof(PlayerMovement))]
    public class PlayerShoot : MonoBehaviour
    {
		[Title("Shoot")]
		[SerializeField, Unit(Units.Second)] private float shootDelay;
		[SerializeField, Unit(Units.Second)] private float shoot2Delay;
		[SerializeField] private float shoot2Knockback;

		[Title("References")]
		[SerializeField, Required] private ParticleSystem shootParticles;
		[SerializeField, Required] private ParticleSystem shoot2Particles;
		[SerializeField, Required] private PlayerMovement playerMovement;

		private bool canShoot = true;
		private float timeSinceLastShot;
		private bool currentShoot = true;

		public event DefaultCallback shootEvent;

		private void Reset()
		{
			playerMovement = GetComponent<PlayerMovement>();
		}

		private void Start()
		{
			InputHandler.Instance.playerActions.SwitchWeapon.performed += _ => SwitchWeapon();

			playerMovement.jumpEvent += () => { canShoot = false; };
			playerMovement.landEvent += () => { canShoot = true; };
		}

		private void Update()
		{
			if (InputHandler.Instance.playerActions.Shoot.IsPressed())
			{
				if (currentShoot) HandleShoot(shootParticles, shootDelay, 0);
				else
				{
					HandleShoot(shoot2Particles, shoot2Delay, shoot2Knockback);
				}
			}
			timeSinceLastShot += Time.deltaTime;
		}

		private void HandleShoot(ParticleSystem shootParticles, float shootDelay, float shootKnockback)
		{
			if (!canShoot) return;
			if (timeSinceLastShot >= shootDelay)
			{
				playerMovement.AddKnockback(shootKnockback);
				Shoot(shootParticles);
			}
		}

		private void Shoot(ParticleSystem shootParticles)
		{
            AudioPlayer.instance.PlaySound(Sound.shoot);
			shootEvent?.Invoke();
			timeSinceLastShot = 0;
			shootParticles.Play();
		}

		private void SwitchWeapon()
		{
			timeSinceLastShot = shootDelay;
			currentShoot = !currentShoot;
		}
	}
}