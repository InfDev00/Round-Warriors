using System;
using System.Collections;
using System.Net.Mime;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GamePlayUI : MonoBehaviour
    {
        public float gamePlayTime = 300;
        private float _initTime;
        
        [Header("Image")] 
        public Image clock;

        public GameObject countObj;
        private Image[] killCount = new Image[3];
        private Image[] lifeCount = new Image[3];

        public Image[] skillIcon;
        private readonly Image[] _skillCoolImage = new Image[3];
        private readonly TextMeshProUGUI[] _skillCoolTimes = new TextMeshProUGUI[3];
        
        [Header("Text")]
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI aliveText;
        public TextMeshProUGUI countDownText;

        [Header("Ending")] 
        public GameObject endingGroup;
        private TextMeshProUGUI _endingText;
        private Button _endingButton;


        private void Awake()
        {
            timeText.text = TimeSetting((int)gamePlayTime);
            _initTime = gamePlayTime;

            int i = 0;
            foreach (var obj in countObj.GetComponentsInChildren<Image>())
            {
                if (i < 3) killCount[i] = obj;
                else lifeCount[i - 3] = obj;
                i++;
            }
            
            for (i = 0; i < 3; ++i)
            {
                _skillCoolImage[i] = skillIcon[i].transform.GetChild(0).GetComponent<Image>();
                _skillCoolTimes[i] = skillIcon[i].GetComponentInChildren<TextMeshProUGUI>();
                
                _skillCoolImage[i].gameObject.SetActive(false);

                killCount[i].color = Color.black;
                lifeCount[i].color = Color.white;
            }

            _endingText = endingGroup.GetComponentInChildren<TextMeshProUGUI>();
            _endingButton = endingGroup.GetComponentInChildren<Button>();
            _endingButton.onClick.AddListener(BackToMain);
            endingGroup.SetActive(false);
            aliveText.text = "";
            countDownText.text = "";
        }

        private void FixedUpdate()
        {
            gamePlayTime -= Time.fixedDeltaTime;
            timeText.text = TimeSetting((int)gamePlayTime);

            clock.fillAmount = (gamePlayTime / _initTime);
        }

        private static string TimeSetting(int sec)
        {
            var min = sec / 60;
            sec = sec % 60;
            return $"{min:D2}:{sec:D2}";
        }

        public void PlayerLifeDecrease(int idx)
        {
            if (idx is < 0 or >= 3) return;
            lifeCount[idx].color = Color.black;
        }

        public void PlayerKillIncrease(int idx)
        {
            if (idx is < 0 or >= 3) return;
            
            if (idx == 2) foreach(var cnt in killCount) cnt.color = Color.red;
            else killCount[idx].color = Color.white;
            
        }
        
        public void OnUseSkill(int idx, float coolTime) => StartCoroutine(ISkillUse(idx, coolTime));

        IEnumerator ISkillUse(int idx, float coolTime)
        {
            _skillCoolImage[idx].gameObject.SetActive(true);
            var initCoolTime = coolTime;
            
            while (coolTime > 0f)
            {
                var intCoolTime = (int)coolTime;
                _skillCoolImage[idx].fillAmount = coolTime / initCoolTime;
                _skillCoolTimes[idx].text = $"{intCoolTime + 1}";
                
                coolTime -= Time.deltaTime;
                yield return null;
            }
            
            _skillCoolImage[idx].gameObject.SetActive(false);
        }

        public void GameEnd(string text)
        {
            _endingText.text = text;
            endingGroup.SetActive(true);
        }

        private void BackToMain()
        {
            Time.timeScale = 1f;
            _endingButton.interactable = false;
            NetworkManager.LeaveRoom();
        }
    }
}