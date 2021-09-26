using Mirror;
using RTS.GameManagement;
using TMPro;
using UnityEngine;

namespace RTS.UI
{
    public class GameOverDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverDisplayParent;
        [SerializeField] private TMP_Text winnerNameText;
        
        private void Awake()
        {
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        public void LeaveGame()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }

        private void ClientHandleGameOver(string winner)
        {
            gameOverDisplayParent.SetActive(true);
            winnerNameText.text = $"{winner} Has Won!";
        }
    }
}