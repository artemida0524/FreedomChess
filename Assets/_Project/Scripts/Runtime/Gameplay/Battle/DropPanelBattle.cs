using DG.Tweening;
using Game.Core.Sounds;
using Game.Runtime.Core.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Game.Runtime.Gameplay
{
    public class DropPanelBattle : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private RectTransform panel;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI eloText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button discardButton;
        [SerializeField] private RectMask2D rectMask2D;

        [SerializeField] private float duration = 0.25f;
        private float autoCloseTime = 6f;

        public event Action<string, string> OnAcceptClicked;
        public event Action<string, string> OnDiscardClicked;

        private IconStatManager _iconStatManager;

        private Tween tween;
        private Tween autoCloseTween;

        public string UserId { get; private set; }

        private bool _canDrop = true;

        [Inject]
        private void Construct(IconStatManager iconStatManager)
        {
            _iconStatManager = iconStatManager;
        }


        public void Drop(FriendData friendData, string userId, string battleId)
        {
            if(!_canDrop)
                return;

            Debug.Log(userId + ": " + battleId);
            panel.gameObject.SetActive(true);

            icon.sprite = _iconStatManager.GetIconStatById(friendData.icon).icon;
            nameText.text = friendData.name;
            eloText.text = friendData.elo.ToString();

            UserId = userId;

            acceptButton.onClick.RemoveAllListeners();
            discardButton.onClick.RemoveAllListeners();

            acceptButton.onClick.AddListener(() => OnAcceptClicked?.Invoke(userId, battleId));
            discardButton.onClick.AddListener(() => OnDiscardClicked?.Invoke(userId, battleId));

            Show();
            RestartAutoClose();
        }

        private void Show()
        {
            if (tween != null && tween.IsActive())
                tween.Kill();

            rectMask2D.softness = new Vector2Int(10000, 10000);

            tween = DOTween.To(
                    () => new Vector2(rectMask2D.softness.x, rectMask2D.softness.y),
                    x => rectMask2D.softness = new Vector2Int((int)x.x, (int)x.y),
                    Vector2.zero,
                    duration
                )
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject);

            SoundManager.Instance.Request();
        }

        public void Hide()
        {
            if (tween != null && tween.IsActive())
                tween.Kill();

            if (autoCloseTween != null && autoCloseTween.IsActive())
                autoCloseTween.Kill();

            panel.gameObject.SetActive(false);
        }

        private void RestartAutoClose()
        {
            if (autoCloseTween != null && autoCloseTween.IsActive())
                autoCloseTween.Kill();

            autoCloseTween = DOVirtual.DelayedCall(autoCloseTime, Hide)
                .SetLink(gameObject);
        }

        public void SetActive(bool v)
        {
            _canDrop = v;
        }
    }
}