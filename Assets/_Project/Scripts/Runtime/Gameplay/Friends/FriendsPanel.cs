using Firebase.Auth;
using Firebase.Database;
using Game.Runtime.Core.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Runtime.Gameplay
{
    public class FriendsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject background;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject container;
        [SerializeField] private GameObject scrollRect;
        [SerializeField] private GameObject noFriends;
        [SerializeField] private GameObject loading;
        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private Button friendsButton;
        [SerializeField] private Button onlineButton;
        [SerializeField] private Button requestsButton;



        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private Button searchButton;


        [SerializeField] private AddFriendItem addFriendItem;
        [SerializeField] private RequestFriendItem requestFriendItem;
        [SerializeField] private SimpleFriendItem simpleFriendItem;

        private IconStatManager _iconStatManager;


        private FriendsProvider _friendsProvider;
        private FirebaseDatabase _database;
        [Inject]
        private void Construct(FriendsProvider friendsProvider, IconStatManager iconStatManager)
        {
            _friendsProvider = friendsProvider;
            _iconStatManager = iconStatManager;
        }

        public void Init()
        {
            _database = FirebaseDatabase.DefaultInstance;
            closeButton.onClick.AddListener(() => SetActive(false));
            searchButton.onClick.AddListener(OnSearchClicked);
            friendsButton.onClick.AddListener(OpenFriendPanel);
            onlineButton.onClick.AddListener(OpenOnlinePanel);
            requestsButton.onClick.AddListener(OpenRequestsPanel);
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                background.SetActive(true);
                scrollRect.SetActive(false);
                noFriends.SetActive(false);
                loading.SetActive(false);
                searchInput.text = string.Empty;
                canvasGroup.interactable = true;
                OpenFriendPanel();
                return;
            }
            background.SetActive(false);
        }

        private async void OnSearchClicked()
        {
            if (string.IsNullOrEmpty(searchInput.text))
            {
                return;
            }
            if (searchInput.text == FirebaseAuth.DefaultInstance.CurrentUser.UserId)
            {
                return;
            }
            canvasGroup.interactable = false;
            loading.SetActive(true);
            scrollRect.SetActive(false);
            noFriends.SetActive(false);
            foreach (Transform item in container.transform)
            {
                Destroy(item.gameObject);
            }
            DataSnapshot snapshot = null;
            try
            {
                snapshot = await _database.GetReference($"publicPlayerData/{searchInput.text}").GetValueAsync();
            }
            catch (Exception)
            {
                loading.SetActive(false);
                canvasGroup.interactable = true;
                return;
            }

            if (!snapshot.Exists)
            {
                loading.SetActive(false);
                OpenFriendPanel();
                canvasGroup.interactable = true;
                return;
            }

            FriendData friendData = JsonUtility.FromJson<FriendData>(snapshot.GetRawJsonValue());

            if (!_friendsProvider.Friends.TryGetValue(searchInput.text, out FriendData myFriendData))
            {
                AddFriendItem instance = Instantiate(addFriendItem, container.transform);
                instance.Init(friendData, searchInput.text, _iconStatManager);
                instance.OnClicked += OnAddFriendClicked;
            }
            loading.SetActive(false);
            scrollRect.SetActive(true);
            canvasGroup.interactable = true;
        }

        private async void OnAddFriendClicked(string userId)
        {
            await _friendsProvider.SendFriendOffer(userId);
            Debug.Log("Friend request sent");
        }

        private void OpenFriendPanel()
        {
            loading.SetActive(false);
            foreach (Transform item in container.transform)
            {
                Destroy(item.gameObject);
            }
            if (_friendsProvider.Friends.Count > 0)
            {
                scrollRect.SetActive(true);
                foreach (Transform item in container.transform)
                {
                    Destroy(item.gameObject);
                }
                foreach (var item in _friendsProvider.Friends)
                {
                    SimpleFriendItem instance = Instantiate(simpleFriendItem, container.transform);
                    instance.Init(item.Value, searchInput.text, _iconStatManager);

                    
                }
            }
            else
            {
                noFriends.SetActive(true);
                scrollRect.SetActive(false);
            }
        }

        private void OpenOnlinePanel()
        {

        }

        private void OpenRequestsPanel()
        {
            noFriends.SetActive(false);

            if (_friendsProvider.Requests.Count > 0)
            {
                scrollRect.SetActive(true);
                foreach (Transform item in container.transform)
                {
                    Destroy(item.gameObject);
                }

                foreach (var item in _friendsProvider.Requests)
                {
                    RequestFriendItem instance = Instantiate(requestFriendItem, container.transform);
                    instance.Init(item.Value, item.Key, _iconStatManager);
                    instance.AcceptButtonClicked += async (userId) =>
                    {
                        try
                        {
                            await _friendsProvider.AcceptFriendOffer(userId);
                            Debug.Log("Friend request accepted");
                            Destroy(instance.gameObject);
                            canvasGroup.interactable = false;
                        }
                        catch (Exception)
                        {

                        }
                        finally
                        {
                            canvasGroup.interactable = true;
                        }
                    };
                    instance.DiscardButtonClicked += async (userId) =>
                    {
                        try
                        {
                            await _friendsProvider.DeclineFriendOffer(userId);
                            Debug.Log("Friend request declined");
                            Destroy(instance.gameObject);
                            canvasGroup.interactable = true;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        finally { canvasGroup.interactable = false; }
                    };
                }
            }
            else
            {
                scrollRect.SetActive(false);

            }

            loading.SetActive(false);
            searchInput.text = string.Empty;
        }
    }
}