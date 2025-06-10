//using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;

namespace FrameWork.GameSettings
{
    public class DisplayNamePanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroupController _controller;
        [SerializeField] private TextMeshProUGUI _text;

        //private PhotonView _photonView;
        private Transform _mainCamera;
        private bool _isVisible;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);

            //_photonView = GetComponentInParent<PhotonView>();
            _mainCamera = Camera.main.transform;

            DisplayNameChanged();
            DisplayNameVisibleChanged(GameSettingsManager.DisplayNameVisible);

            GameSettingsManager.DisplayNameChanged += DisplayNameChanged;
            GameSettingsManager.DisplayNameVisibleChanged += DisplayNameVisibleChanged;
        }

        private void OnDestroy()
        {
            GameSettingsManager.DisplayNameChanged -= DisplayNameChanged;
            GameSettingsManager.DisplayNameVisibleChanged -= DisplayNameVisibleChanged;
        }

        // 싱글플레이의 경우
        private void DisplayNameChanged(string value)
        {
            _text.text = value;
        }

        // 멀티플레이의 경우
        private void DisplayNameChanged()
        {
            //if (PhotonNetwork.OfflineMode)
            //{
            //    _text.text = PhotonNetwork.LocalPlayer.NickName;
            //}
            //else
            //{
            //    _text.text = _photonView.Owner.NickName;
            //}
        }

        private void DisplayNameVisibleChanged(bool isVisible)
        {
            _isVisible = isVisible;

            if (isVisible)
            {
                _controller.Show(true);
            }
            else
            {
                _controller.Hide(true);
            }
        }

        private void Update()
        {
            if (_mainCamera == null) return;
            if (!_isVisible) return;

            transform.LookAt(transform.position + _mainCamera.rotation * Vector3.forward, _mainCamera.rotation * Vector3.up);
        }
    }
}