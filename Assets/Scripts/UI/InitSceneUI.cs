using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class InitSceneUI : MonoBehaviour
    {
        [Header("Button")]
        public Button enterGameButton;
        
        private void Awake()
        {
            enterGameButton.onClick.AddListener(OnOnlineButtonClick);
            StartCoroutine(ButtonBlink());
        }

        private IEnumerator ButtonBlink()
        { 
            while (true)
            {
                enterGameButton.image.color = new Color(0.5f, 0.5f, 0.5f);
                yield return new WaitForSeconds(1f);
                enterGameButton.image.color = Color.white;
                yield return new WaitForSeconds(1f);
            }
        }
        
        private void OnOnlineButtonClick()
        {
            enterGameButton.interactable = false;
            SceneManager.LoadScene("2. Online");
        }
    }
}