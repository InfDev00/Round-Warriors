using System;
using Bases;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        public string[] playerPrefabNames;
        private static string _currentPlayerPrefabName;

        private const int MAX_PLAYER_PER_ROOM = 2;
        
        #region Event

        public Action OnConnectToMasterSuccess = null;
        public Action OnPlayerKillOther = null;
        public Action<string> OnGameEnd = null;
        public Action OnEnemyLeftRoom = null;

        #endregion
        
        private void Awake()
        {
            CreateSingleton(this);

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "1";
        }
        
        #region Server

        public static void ConnectToMaster() => PhotonNetwork.ConnectUsingSettings();
        public override void OnConnectedToMaster() => OnConnectToMasterSuccess?.Invoke();
        
        #endregion
        
        #region Room

        public void ConnectToRoom(string nickname, int idx)
        {
            PhotonNetwork.LocalPlayer.NickName = nickname;
            _currentPlayerPrefabName = playerPrefabNames[idx];
            
            PhotonNetwork.JoinRandomOrCreateRoom(roomOptions:new RoomOptions{MaxPlayers = MAX_PLAYER_PER_ROOM});
        }

        public override void OnJoinedRoom() => photonView.RPC(nameof(PlayerJoined), RpcTarget.MasterClient);

        [PunRPC]
        private void PlayerJoined()
        {
            if (PhotonNetwork.PlayerList.Length == MAX_PLAYER_PER_ROOM) PhotonNetwork.LoadLevel("4. Game");
        }

        public static void LeaveRoom() => PhotonNetwork.LeaveRoom();
        public override void OnLeftRoom() => SceneManager.LoadScene("1. Init");

        #endregion

        #region GamePlay

        public static string GetNickname(bool mine)
        {
            int idx = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            if(!mine) idx = -1 * (idx - 1);
            return PhotonNetwork.PlayerList[idx].NickName;
        }

        public GameObject InstantiatePlayer(Transform spawnPoints)
        {
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            var spawnPoint = spawnPoints.GetChild(playerIndex);

            var player =
                PhotonNetwork.Instantiate(_currentPlayerPrefabName, spawnPoint.position, spawnPoint.rotation, 0);
            
            return player;
        }

        public void PlayerKillOther() => photonView.RPC(nameof(KillOther), RpcTarget.Others);
        [PunRPC]
        private void KillOther() => OnPlayerKillOther?.Invoke();

        public void GameEndAnnounce(string text) => photonView.RPC(nameof(GameEnd), RpcTarget.Others, text);
        [PunRPC]
        private void GameEnd(string text) => OnGameEnd?.Invoke(text);

        public override void OnPlayerLeftRoom(Player otherPlayer) => OnEnemyLeftRoom?.Invoke();

        #endregion
    }
}