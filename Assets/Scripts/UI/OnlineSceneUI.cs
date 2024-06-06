using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class OnlineSceneUI : MonoBehaviour
    {
        [Header("Button")] 
        public Button gameStartButton;
        public Button backToInitButton;
        [Space(25f)] 
        public Button prevButton;
        public Button nextButton;

        [Header("Image")] 
        public Transform imageGroup;
        private GameObject[] _images;
        private int _idx;
        
        [Header("Text")] 
        public TextMeshProUGUI className;

        [Header("InputField")] 
        public TMP_InputField playerNameInput;

        private readonly string[] _classNameArray = {"Sword", "Spear", "Short Sword"};
        
        private void Awake()
        {
            _images = new GameObject[imageGroup.childCount];

            for (int i = 0; i < imageGroup.childCount; ++i)
            {
                _images[i] = imageGroup.GetChild(i).gameObject;
                _images[i].SetActive(false);
            }
            _images[_idx].SetActive(true);

            className.text = _classNameArray[_idx];
            
            nextButton.onClick.AddListener(OnNextButtonClick);
            prevButton.onClick.AddListener(OnPrevButtonClick);
            backToInitButton.onClick.AddListener(OnBackToInitButtonClick);
            gameStartButton.onClick.AddListener(OnGameStartButtonClick);
        }
        
        private void OnNextButtonClick() => ChangeIdx(1);
        private void OnPrevButtonClick() => ChangeIdx(-1);
        private static void OnBackToInitButtonClick() => SceneManager.LoadScene("1. Init");

        private void OnGameStartButtonClick()
        {
            if (string.IsNullOrEmpty(playerNameInput.text))
            {
                StartCoroutine(InputFieldShake(10, 0.5f));
                return;
            }
            
            SceneManager.LoadScene("3. Loading");
            NetworkManager.Instance.ConnectToRoom(playerNameInput.text, _idx);
        }

        private IEnumerator InputFieldShake(float shakeAmount, float shakeTime)
        {
            var origin = playerNameInput.GetComponent<RectTransform>().anchoredPosition;
            float timer = 0;
            while (timer <= shakeTime)
            {
                var shake = Random.insideUnitCircle * shakeAmount;
                playerNameInput.GetComponent<RectTransform>().anchoredPosition = origin + new Vector2(shake.x, shake.y);
                timer += Time.deltaTime;
                yield return null;
            }

            playerNameInput.GetComponent<RectTransform>().anchoredPosition = origin;
        }
        private void ChangeIdx(int add)
        {
            var len = _images.Length;
            _images[_idx].SetActive(false);
            
            _idx += add;
            if (_idx < 0) _idx += len;
            else if (_idx >= len) _idx -= len;
            
            _images[_idx].SetActive(true);
            className.text = _classNameArray[_idx];
        }
    }
}