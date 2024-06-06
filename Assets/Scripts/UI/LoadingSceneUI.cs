using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LoadingSceneUI : MonoBehaviour
    {
        public TextMeshProUGUI loadingText;
        
        private void Start()
        {
            StartCoroutine(LoadingText());
            
            NetworkManager.Instance.OnConnectToMasterSuccess += () => { SceneManager.LoadScene("1. Init"); };
            NetworkManager.ConnectToMaster();
        }

        private IEnumerator LoadingText()
        {
            loadingText.text = "LOADING.";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = "LOADING..";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = "LOADING...";
            yield return new WaitForSeconds(0.5f);
        }
    }
}