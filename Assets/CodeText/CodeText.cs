using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Chromecore
{
    [RequireComponent(typeof(TMP_Text))]
    public class CodeText : MonoBehaviour 
    {
        [SerializeField] private LayerMask damageMask;

        [Title("References")]
        [SerializeField, Required] private CodeManager codeManager;
        [SerializeField, Required] private TMP_Text codeText;

        private void Reset() {
            codeText = GetComponent<TMP_Text>();
        }

        public void Toggle(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetCode(string text)
        {
            codeText.text = text;
        }

        private void OnParticleCollision(GameObject other)
		{
			if ((damageMask & (1 << other.layer)) != 0)
			{
                codeManager.DestroyCurrentCode();
			}
		}
    }
}