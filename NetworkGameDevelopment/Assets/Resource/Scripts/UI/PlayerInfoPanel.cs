using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resource.Scripts.UI
{
    public class PlayerInfoPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private Button _kickButt;
        [SerializeField] private Image _readyStatusImg, _PlayerColorImg;

        public event Action<ulong> onKickClicked;
        private ulong _clientId;

        // methods to populate information

        private void OnEnable()
        {
            _kickButt.onClick.AddListener(ButtKickClicked);
        }

        public void SetPlayerName(ulong playerName)
        {
            _clientId = playerName;
            _playerName.text = playerName.ToString();
        }

        private void ButtKickClicked()
        {
            onKickClicked?.Invoke(_clientId);
        }

        public void SetKickActivate(bool isOn)
        {
            _kickButt.gameObject.SetActive(isOn);
        }

        public void SetReadyStatus(bool isReady)
        {
            if (isReady)
            {
                _readyStatusImg.color = Color.green;
            }
            else
            {
                _readyStatusImg.color = Color.red;
            }
        }

        public void SetPlayerColor(Color selectedColor)
        {
            _PlayerColorImg.color = selectedColor;
        }
    }
}

