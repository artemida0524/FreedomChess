using Game.Runtime.Core.Auth;
using Game.Runtime.Core.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Runtime.Gameplay
{
    public class FriendButtonView : MonoBehaviour
    {
        [SerializeField] private GameObject friendRequest;
        [SerializeField] private TextMeshProUGUI countText;

        [SerializeField] private GameObject friendRequest2;
        [SerializeField] private TextMeshProUGUI countText2;

        [SerializeField] private FriendsPanel panel;
        [SerializeField] private Button button;
        [SerializeField] private GameObject buttonView;

        private FriendsProvider _friendsProvider;
        private IPlayerAuthRepository _playerAuthRepository;

        private bool _isDestroyed;

        [Inject]
        private void Construct(FriendsProvider friendsProvider, IPlayerAuthRepository playerAuthRepository)
        {
            _friendsProvider = friendsProvider;
            _playerAuthRepository = playerAuthRepository;
        }

        public void Init()
        {
            if (_friendsProvider == null) return;

            Changed();

            _friendsProvider.OnRequestsChanged += Changed;

            if (button != null)
                button.onClick.AddListener(OnButtonClick);

            if (_playerAuthRepository != null)
                _playerAuthRepository.OnLoggedOut += OnLoggedOut;
        }

        private void OnButtonClick()
        {
            if (panel != null)
                panel.SetActive(true);
        }

        private void OnLoggedOut(string obj)
        {
            Unsubscribe();
        }

        public void SetActive(bool active)
        {
            if (buttonView != null)
                buttonView.SetActive(active);
        }

        private void Changed()
        {
            if (_isDestroyed || _friendsProvider == null)
                return;

            int count = _friendsProvider.Requests?.Count ?? 0;

            bool hasRequests = count > 0;

            if (friendRequest != null)
                friendRequest.SetActive(hasRequests);

            if (friendRequest2 != null)
                friendRequest2.SetActive(hasRequests);

            if (hasRequests)
            {
                if (countText != null)
                    countText.text = count.ToString();

                if (countText2 != null)
                    countText2.text = count.ToString();
            }
        }

        private void Unsubscribe()
        {
            if (_friendsProvider != null)
                _friendsProvider.OnRequestsChanged -= Changed;

            if (_playerAuthRepository != null)
                _playerAuthRepository.OnLoggedOut -= OnLoggedOut;

            if (button != null)
                button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            _isDestroyed = true;
            Unsubscribe();
        }
    }
}