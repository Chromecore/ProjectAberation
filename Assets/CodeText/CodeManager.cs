using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chromecore
{
    public enum CodeType {
        None,
        NoJump,
        DoubleEnemies,
        LAST,
    }

    public class CodeManager : MonoBehaviour
    {
        [SerializeField, Unit(Units.Second)] private float newCodeTime; 
        [SerializeField, Unit(Units.Second)] private float showWarningTime; 
        
        [Title("References")]
        [SerializeField, Required] private CodeText codeText;
        [SerializeField, Required] private GameObject codeSpawnText;

        private CodeType currentType;

        private void Awake() 
        {
            currentType = CodeType.None;
            codeText.Toggle(false);
            codeSpawnText.SetActive(false);
            StartCoroutine(CodeHandler());
        }

        private IEnumerator CodeHandler()
        {
            while(true){
                yield return new WaitForSeconds(newCodeTime);
                currentType = (CodeType)Random.Range(0, (int)CodeType.LAST);
                ResetCodes();
                SetNewCode();
            }
        }

        private IEnumerator ShowWarning()
        {
            codeSpawnText.SetActive(true);
            yield return new WaitForSeconds(showWarningTime);
            codeSpawnText.SetActive(false);
        }

        private void SetNewCode()
        {
            if (GameManager.gamePaused) return;
            string code = "";

            switch (currentType)
            {
                case CodeType.NoJump:
                    code = "While(true){\n" +
                           "    canJump = false;\n" +
                           "}";
                    break;
                case CodeType.DoubleEnemies:
                    code = "SpawnEnemies(500);";
                    break;
                default:
                    return;
            }

            codeText.transform.position = GetRandomPosition();
            codeText.Toggle(true);
            codeText.SetCode(code);
            StartCoroutine(ShowWarning());
            ApplyCode();
        }

        private void ResetCodes(){
            GameManager.instance.noJumpCode = false;
            GameManager.instance.doubleEnemiesCode = false;
        }

        private void ApplyCode()
        {
            switch (currentType)
            {
                case CodeType.NoJump:
                    GameManager.instance.noJumpCode = true;
                    break;
                case CodeType.DoubleEnemies:
                    GameManager.instance.doubleEnemiesCode = true;
                    break;
                default:
                    return;
            }
        }

        private Vector2 GetRandomPosition()
        {
            return new Vector2(
                Random.Range(GameManager.instance.roomMinX, GameManager.instance.roomMaxX),
                Random.Range(GameManager.instance.roomMinY, GameManager.instance.roomMaxY)
                );
        }

        public void DestroyCurrentCode()
        {
            codeText.Toggle(false);
            codeText.SetCode("");
            ResetCodes();
        }

        private void OnDestroy() 
        {
            ResetCodes();
        }
    }
}