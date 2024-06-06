using System;
using System.Collections;
using Bases;
using Photon.Pun;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public enum GameState
    {
        COUNT_DOWN_STATE,
        GAME_PLAY_STATE,
        GAME_FINAL_STATE,
        GAME_END_STATE
    }
    public class GameSceneManager : MonoBehaviour
    {
        public GameState CurrentGameState { get; private set; } = GameState.COUNT_DOWN_STATE;

        public float zoomDuration = 2f;
        [Header("Objects")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform spawnPoints;
        [SerializeField] private GamePlayUI ui;

        private PlayerController _player;
        private int _playerLifeCount = 3;
        private int _playerKillCount = 0;
        
        private void Start()
        {
            _player = NetworkManager.Instance.InstantiatePlayer(spawnPoints).GetComponent<PlayerController>();
            _player.SetPause(true);

            _player.PlayerDie += PlayerDied;
            _player.AliveTextSetting += (text) => { ui.aliveText.text = text; };
            _player.GetComponentInChildren<Weapon>().UseSkill += ui.OnUseSkill;
            NetworkManager.Instance.OnPlayerKillOther += PlayerKill;
            NetworkManager.Instance.OnGameEnd += ui.GameEnd;
            NetworkManager.Instance.OnGameEnd += s => { CurrentGameState = GameState.GAME_END_STATE; };
            NetworkManager.Instance.OnEnemyLeftRoom += OnEnemyLeftRoom;
            
            CountDownState();
        }

        #region CountDownState

        private void CountDownState() => StartCoroutine(Zoom());

        private IEnumerator Zoom()
        {
            float elapsedTime = 0f;
            mainCamera.orthographicSize = 1f;

            while (elapsedTime < zoomDuration)
            {
                mainCamera.orthographicSize = Mathf.Lerp(1f, 5f, elapsedTime / zoomDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            mainCamera.orthographicSize = 5f;
            yield return StartCoroutine(CountDown());
        }

        private IEnumerator CountDown()
        {
            for (int i = 5; i > 0; --i)
            {
                ui.countDownText.text = $"{i}";
                yield return new WaitForSeconds(1f);
            }

            ui.countDownText.text = "";
            _player.SetPause(false);

            CurrentGameState = GameState.GAME_PLAY_STATE;
        }
        
        
        #endregion

        #region GamePlay

        private void PlayerDied()
        {
            ui.PlayerLifeDecrease(--_playerLifeCount);
            NetworkManager.Instance.PlayerKillOther();
        }
        
        private void PlayerKill()
        {
            ui.PlayerKillIncrease(_playerKillCount++);

            if (_playerKillCount == 4) GameEndByKillOther();
        }

        #endregion

        #region GameEndState

        private void GameEndByKillOther()
        {
            if (CurrentGameState == GameState.GAME_END_STATE) return;
            CurrentGameState = GameState.GAME_END_STATE;
            
            ui.GameEnd("WIN");
            NetworkManager.Instance.GameEndAnnounce("LOSS");

            Time.timeScale = 0f;
        }

        private void OnEnemyLeftRoom()
        {
            if (CurrentGameState == GameState.GAME_END_STATE) return;
            CurrentGameState = GameState.GAME_END_STATE;
            
            ui.GameEnd("Enemy Run");
        }

        #endregion
    }
}