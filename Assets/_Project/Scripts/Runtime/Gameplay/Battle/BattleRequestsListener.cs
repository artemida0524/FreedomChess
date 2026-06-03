//using Firebase.Auth;
//using Firebase.Database;
//using Game.Runtime.Core.Player;
//using System;
//using UnityEngine;

//namespace Game.Runtime.Gameplay
//{
//	public class BattleListener : MonoBehaviour
//	{
//		[SerializeField] private DropPanelBattle dropPanelBattle;

//        private FirebaseDatabase _database;
//		private FirebaseAuth _auth;

//        private DatabaseReference _requestsReference;
//        public void Init()
//		{
//            _database = FirebaseDatabase.DefaultInstance;
//			_auth = FirebaseAuth.DefaultInstance;

//            _requestsReference = _database.GetReference($"battleRequests/{_auth.CurrentUser.UserId}");
//            _requestsReference.ChildAdded += OnBattleRequestAdded;
//            dropPanelBattle.OnClicked += OnBattleRequestClicked;
//        }

//        private void OnBattleRequestClicked(string obj)
//        {
//            Debug.Log("play");
//        }

//        private async void OnBattleRequestAdded(object sender, ChildChangedEventArgs e)
//        {
//            DataSnapshot dataSnapshot = await _database.GetReference($"publicPlayerData/{e.Snapshot.Key}").GetValueAsync();
//            FriendData friendData = JsonUtility.FromJson<FriendData>(dataSnapshot.GetRawJsonValue());

//            dropPanelBattle.Drop(friendData, e.Snapshot.Key);
//        }

//        private void OnDestroy()
//        {
//            _requestsReference.ChildAdded -= OnBattleRequestAdded;
//            dropPanelBattle.OnClicked -= OnBattleRequestClicked;
//        }
//    }

//	public class BattleManager : MonoBehaviour
//	{

//	}
//}
using Firebase.Auth;
using Firebase.Database;
using Game.Runtime.Core.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace Game.Runtime.Gameplay
{
    public class BattleRequestsListener : MonoBehaviour
    {
        [SerializeField] private DropPanelBattle dropPanelBattle;

        private FirebaseDatabase _database;
        private FirebaseAuth _auth;
        private DatabaseReference _requestsReference;

        private bool _ignoreInitialEvents = true;
        private bool _ignoreRequests = false;

        public static BattleRequestsListener Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void Init()
        {
            _database = FirebaseDatabase.DefaultInstance;
            _auth = FirebaseAuth.DefaultInstance;

            _requestsReference = _database.GetReference($"battleRequests/{_auth.CurrentUser.UserId}");
            _requestsReference.ChildChanged += OnBattleRequestAdded;
            _requestsReference.ChildAdded += OnBattleRequestAdded;

            dropPanelBattle.OnAcceptClicked += OnBattleRequestClicked;
            dropPanelBattle.OnDiscardClicked += DropPanelBattle_OnDiscardClicked;

            StartCoroutine(DisableInitialIgnore());
        }

        private async void DropPanelBattle_OnDiscardClicked(string enemyId, string battleId)
        {
            await _database.GetReference($"battle/{battleId}/enemyPlayerResponse").SetValueAsync("no");
            dropPanelBattle.Hide();
            Debug.Log("hide");
        }

        public void SetActive(bool active)
        {
            _ignoreRequests = active;
        }


        private IEnumerator DisableInitialIgnore()
        {
            yield return new WaitForSeconds(1f);
            _ignoreInitialEvents = false;
        }

        private async void OnBattleRequestClicked(string userId, string battleId)
        {
            await _database.GetReference($"battle/{battleId}/enemyPlayerResponse").SetValueAsync("yes");
            BattleManager.Instance.StartBattle(userId, battleId);

            dropPanelBattle.Hide();
            
        }

        private async void OnBattleRequestAdded(object sender, ChildChangedEventArgs e)
        {
            if (_ignoreInitialEvents || _ignoreRequests)
                return;

            DataSnapshot dataSnapshot = await _database
                .GetReference($"publicPlayerData/{e.Snapshot.Key}")
                .GetValueAsync();

            if (!dataSnapshot.Exists)
                return;

            FriendData friendData =
                JsonUtility.FromJson<FriendData>(dataSnapshot.GetRawJsonValue());

            dropPanelBattle.Drop(friendData, e.Snapshot.Key, e.Snapshot.Child("battleId").GetValue(false).ToString());
        }

        private void OnDestroy()
        {
            if (_requestsReference != null)
            {
                _requestsReference.ChildChanged -= OnBattleRequestAdded;
                _requestsReference.ChildAdded -= OnBattleRequestAdded;
            }

            dropPanelBattle.OnAcceptClicked -= OnBattleRequestClicked;
        }
    }

    

}