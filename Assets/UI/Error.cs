using UnityEngine;
using Sirenix.OdinInspector;

namespace Chromecore
{
    public class Error : MonoBehaviour 
    {
        [SerializeField] private LayerMask damageMask;
        [SerializeField, Required] private MenuUI menuUI;

        private void OnParticleCollision(GameObject other)
		{
			if ((damageMask & (1 << other.layer)) != 0)
			{
                menuUI.ErrorHit();
			}
		}
    }
}