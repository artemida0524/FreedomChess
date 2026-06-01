////using Cysharp.Threading.Tasks;
////using Firebase.Auth;
////using Firebase.Database;
////using Firebase.Firestore;
////using Game.Runtime.Core.Auth;
////using System;
////using System.Collections.Generic;
////using System.Threading;
////using Unity.VisualScripting;
////using UnityEngine;
////using Zenject;


////namespace Game.Runtime.Core.Player
////{
////    public class FriendsProvider
////    {
////        public Dictionary<string, FriendData> Requests { get; private set; } = new();

////        public event Action OnFriendsChanged;
////        public event Action OnRequestsChagned;


////        private FirebaseAuth _auth;
////        private FirebaseDatabase _database;
////        private FirebaseFirestore _firestore;

////        private bool _part2 = false;
////        private string _userId;
////        private IPlayerAuthRepository _playerAuthRepository;

////        private Dictionary<string,Listener> _requestListener = new();

////        [Inject]
////        private void Construct(IPlayerAuthRepository playerAuthRepository)
////        {
////            _playerAuthRepository = playerAuthRepository;
////        }

////        public async UniTask Init()
////        {
////            _auth = FirebaseAuth.DefaultInstance;
////            _database = FirebaseDatabase.DefaultInstance;
////            _firestore = FirebaseFirestore.DefaultInstance;

////            if (_auth.CurrentUser == null)
////            {
////                Debug.LogError("User is null");
////                return;
////            }

////            _userId = _auth.CurrentUser.UserId;

////            _database.GetReference($"friends/{_userId}").ValueChanged += FriendsProvider_ValueChanged;
////            _database.GetReference($"friendRequests/{_userId}").ValueChanged += FriendRequests_ValueChanged;

////            _playerAuthRepository.OnLoggedOut += HandleLoggedOut;

////            await UniTask.WaitForSeconds(2);
////        }
////        private void HandleLoggedOut()
////{
////    _database.GetReference($"friends/{_userId}").ValueChanged -= FriendsProvider_ValueChanged;
////    _database.GetReference($"friendRequests/{_userId}").ValueChanged -= FriendRequests_ValueChanged;

////    foreach (var listener in _requestListener.Values)
////        listener.Dispose();

////    _requestListener.Clear();
////    Requests.Clear();
////}

////        private void RequestsFriendChanged(FriendData friend)
////        {
////            if(!Requests.ContainsKey(friend.name))
////            {
////                Requests.Add(friend.name, friend);
////            }
////            else
////            {
////                Requests[friend.name] = friend;
////            }
////            OnRequestsChagned?.Invoke();
////        }

////        private async void FriendRequests_ValueChanged(object sender, ValueChangedEventArgs e)
////        {
////            if (!e.Snapshot.Exists)
////            {
////                Debug.Log("No Requests found");
////                return;
////            }

////            foreach (DataSnapshot offer in e.Snapshot.Children)
////            {
////                if (!_requestListener.TryGetValue(offer.Key, out Listener listener1))
////                {
////                    Listener listener = new Listener(_database.GetReference($"publicPlayerData/{offer.Key}"), RequestsFriendChanged);
////                    _requestListener.Add(offer.Key, listener);
////                }
////                else
////                {
////                    listener1.Dispose();
////                    _requestListener.Remove(offer.Key);
////                }
////            }

////            OnRequestsChagned?.Invoke();
////        }

////        private void FriendsProvider_ValueChanged(object sender, ValueChangedEventArgs e)
////        {
////            if (!e.Snapshot.Exists)
////            {
////                Debug.Log("No friends found");
////                return;
////            }

////            foreach (DataSnapshot friendSnap in e.Snapshot.Children)
////            {
////                Debug.Log(friendSnap.Key);
////            }
////            OnFriendsChanged?.Invoke();
////        }

////        public async UniTask<bool> SendFriendOffer(string friendId)
////        {

////            try
////            {
////                await _database.GetReference($"friendRequests/{friendId}/{_auth.CurrentUser.UserId}").SetValueAsync("");

////                return true;
////            }
////            catch (Exception)
////            {
////                return false;
////            }


////        }

////        public async UniTask<bool> AcceptFriendOffer(string friendId)
////        {
////            try
////            {
////                DataSnapshot dataSnapshot = await _database.GetReference($"friendRequests/{_auth.CurrentUser.UserId}/{friendId}").GetValueAsync();

////                if (!dataSnapshot.Exists)
////                {
////                    return false;
////                }

////                await _database.GetReference($"friends/{_auth.CurrentUser.UserId}/{friendId}").SetValueAsync("");
////                await _database.GetReference($"friends/{friendId}/{_auth.CurrentUser.UserId}").SetValueAsync("");
////                await _database.GetReference($"friendRequests/{_auth.CurrentUser.UserId}/{friendId}").RemoveValueAsync();


////                return true;
////            }
////            catch (Exception)
////            {
////                return false;
////            }
////        }

////        private class Listener : IDisposable
////        {
////            private readonly DatabaseReference _reference;
////            private readonly Action<FriendData> _changed;

////            public Listener(DatabaseReference reference, Action<FriendData> changed)
////            {
////                _reference = reference;
////                _changed = changed;

////                _reference.ValueChanged += Reference_ValueChanged;
////            }

////            private void Reference_ValueChanged(object sender, ValueChangedEventArgs e)
////            {
////                if(!e.Snapshot.Exists)
////                {
////                    Debug.LogError("No friend data found");
////                    return;
////                }

////                FriendData friendData = JsonUtility.FromJson<FriendData>(e.Snapshot.GetRawJsonValue());

////                _changed(friendData);
////            }

////            public void Dispose()
////            {
////                _reference.ValueChanged -= Reference_ValueChanged;
////            }
////        }

////    }

////    [Serializable]
////    public class FriendData
////    {
////        public int elo;
////        public int icon;
////        public string name;
////        public bool online;
////    }

////}




//using Cysharp.Threading.Tasks;
//using Firebase.Auth;
//using Firebase.Database;
//using Game.Runtime.Core.Auth;
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using Zenject;

//namespace Game.Runtime.Core.Player
//{
//    public class FriendsProvider :  IDisposable
//    {
//        public IReadOnlyDictionary<string, FriendData> Requests => _requests;

//        public event Action OnFriendsChanged;
//        public event Action OnRequestsChanged;

//        private FirebaseAuth _auth;
//        private FirebaseDatabase _database;

//        private string _userId;

//        private readonly Dictionary<string, FriendData> _requests = new();
//        private readonly Dictionary<string, Listener> _requestListeners = new();

//        private DatabaseReference _friendsReference;
//        private DatabaseReference _requestsReference;

//        private IPlayerAuthRepository _playerAuthRepository;

//        [Inject]
//        private void Construct(IPlayerAuthRepository playerAuthRepository)
//        {
//            _playerAuthRepository = playerAuthRepository;
//        }

//        public UniTask Init()
//        {
//            _auth = FirebaseAuth.DefaultInstance;
//            _database = FirebaseDatabase.DefaultInstance;

//            if (_auth.CurrentUser == null)
//            {
//                Debug.LogError("FriendsProvider: User is null");
//                return UniTask.CompletedTask;
//            }

//            _userId = _auth.CurrentUser.UserId;

//            _friendsReference = _database.GetReference($"friends/{_userId}");
//            _requestsReference = _database.GetReference($"friendRequests/{_userId}");

//            _friendsReference.ValueChanged += Friends_ValueChanged;
//            _requestsReference.ValueChanged += Requests_ValueChanged;

//            _playerAuthRepository.OnLoggedOut += HandleLoggedOut;

//            return UniTask.CompletedTask;
//        }

//        private void HandleLoggedOut(string uid)
//        {
//            Dispose();
//        }

//        private void Friends_ValueChanged(object sender, ValueChangedEventArgs e)
//        {
//            if (e.DatabaseError != null)
//            {
//                Debug.LogException(e.DatabaseError.ToException());
//                return;
//            }

//            OnFriendsChanged?.Invoke();
//        }

//        private void Requests_ValueChanged(object sender, ValueChangedEventArgs e)
//        {
//            if (e.DatabaseError != null)
//            {
//                Debug.LogException(e.DatabaseError.ToException());
//                return;
//            }

//            HashSet<string> activeRequestIds = new();

//            if (e.Snapshot.Exists)
//            {
//                foreach (DataSnapshot request in e.Snapshot.Children)
//                {
//                    string friendId = request.Key;
//                    activeRequestIds.Add(friendId);

//                    if (!_requestListeners.ContainsKey(friendId))
//                    {
//                        var listener = new Listener(
//                            _database.GetReference($"publicPlayerData/{friendId}"),
//                            friendData => OnFriendDataChanged(friendId, friendData));

//                        _requestListeners.Add(friendId, listener);
//                    }
//                }
//            }

//            List<string> removedIds = new();

//            foreach (var pair in _requestListeners)
//            {
//                if (!activeRequestIds.Contains(pair.Key))
//                {
//                    pair.Value.Dispose();
//                    removedIds.Add(pair.Key);
//                }
//            }

//            foreach (string id in removedIds)
//            {
//                _requestListeners.Remove(id);
//                _requests.Remove(id);
//            }

//            OnRequestsChanged?.Invoke();
//        }

//        private void OnFriendDataChanged(string friendId, FriendData friendData)
//        {
//            _requests[friendId] = friendData;
//            OnRequestsChanged?.Invoke();
//        }

//        public async UniTask<bool> SendFriendOffer(string friendId)
//        {
//            try
//            {
//                await _database
//                    .GetReference($"friendRequests/{friendId}/{_userId}")
//                    .SetValueAsync(true);

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Debug.LogException(ex);
//                return false;
//            }
//        }

//        public async UniTask<bool> AcceptFriendOffer(string friendId)
//        {
//            try
//            {
//                DataSnapshot snapshot = await _database
//                    .GetReference($"friendRequests/{_userId}/{friendId}")
//                    .GetValueAsync();

//                if (!snapshot.Exists)
//                    return false;

//                Dictionary<string, object> updates = new()
//                {
//                    [$"friends/{_userId}/{friendId}"] = true,
//                    [$"friends/{friendId}/{_userId}"] = true,
//                    [$"friendRequests/{_userId}/{friendId}"] = null
//                };

//                await _database.RootReference.UpdateChildrenAsync(updates);

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Debug.LogException(ex);
//                return false;
//            }
//        }

//        public async UniTask<bool> DeclineFriendOffer(string friendId)
//        {
//            try
//            {
//                await _database
//                    .GetReference($"friendRequests/{_userId}/{friendId}")
//                    .RemoveValueAsync();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Debug.LogException(ex);
//                return false;
//            }
//        }

//        public async UniTask<bool> RemoveFriend(string friendId)
//        {
//            try
//            {
//                Dictionary<string, object> updates = new()
//                {
//                    [$"friends/{_userId}/{friendId}"] = null,
//                    [$"friends/{friendId}/{_userId}"] = null
//                };

//                await _database.RootReference.UpdateChildrenAsync(updates);

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Debug.LogException(ex);
//                return false;
//            }
//        }

//        public void Dispose()
//        {
//            if (_friendsReference != null)
//                _friendsReference.ValueChanged -= Friends_ValueChanged;

//            if (_requestsReference != null)
//                _requestsReference.ValueChanged -= Requests_ValueChanged;

//            foreach (var listener in _requestListeners.Values)
//                listener.Dispose();

//            _requestListeners.Clear();
//            _requests.Clear();

//            if (_playerAuthRepository != null)
//                _playerAuthRepository.OnLoggedOut -= HandleLoggedOut;
//        }

//        private class Listener : IDisposable
//        {
//            private readonly DatabaseReference _reference;
//            private readonly Action<FriendData> _onChanged;

//            public Listener(
//                DatabaseReference reference,
//                Action<FriendData> onChanged)
//            {
//                _reference = reference;
//                _onChanged = onChanged;

//                _reference.ValueChanged += Reference_ValueChanged;
//            }

//            private void Reference_ValueChanged(
//                object sender,
//                ValueChangedEventArgs e)
//            {
//                if (e.DatabaseError != null)
//                {
//                    Debug.LogException(e.DatabaseError.ToException());
//                    return;
//                }

//                if (!e.Snapshot.Exists)
//                    return;

//                string json = e.Snapshot.GetRawJsonValue();

//                if (string.IsNullOrEmpty(json))
//                    return;

//                FriendData friendData =
//                    JsonUtility.FromJson<FriendData>(json);

//                _onChanged?.Invoke(friendData);
//            }

//            public void Dispose()
//            {
//                _reference.ValueChanged -= Reference_ValueChanged;
//            }
//        }
//    }

//    [Serializable]
//    public class FriendData
//    {
//        public int elo;
//        public int icon;
//        public string name;
//        public bool online;
//    }
//}


using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Game.Runtime.Core.Auth;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Runtime.Core.Player
{
    public class FriendsProvider : IDisposable
    {
        public IReadOnlyDictionary<string, FriendData> Friends => _friends;
        public IReadOnlyDictionary<string, FriendData> Requests => _requests;

        public event Action OnFriendsChanged;
        public event Action OnRequestsChanged;

        private FirebaseAuth _auth;
        private FirebaseDatabase _database;

        private string _userId;

        private readonly Dictionary<string, FriendData> _friends = new();
        private readonly Dictionary<string, FriendData> _requests = new();

        private readonly Dictionary<string, Listener> _friendListeners = new();
        private readonly Dictionary<string, Listener> _requestListeners = new();

        private DatabaseReference _friendsReference;
        private DatabaseReference _requestsReference;

        private IPlayerAuthRepository _playerAuthRepository;

        [Inject]
        private void Construct(IPlayerAuthRepository playerAuthRepository)
        {
            _playerAuthRepository = playerAuthRepository;
        }

        public UniTask Init()
        {
            _auth = FirebaseAuth.DefaultInstance;
            _database = FirebaseDatabase.DefaultInstance;

            if (_auth.CurrentUser == null)
            {
                Debug.LogError("FriendsProvider: User is null");
                return UniTask.CompletedTask;
            }

            _userId = _auth.CurrentUser.UserId;

            _friendsReference = _database.GetReference($"friends/{_userId}");
            _requestsReference = _database.GetReference($"friendRequests/{_userId}");

            _friendsReference.ValueChanged += Friends_ValueChanged;
            _requestsReference.ValueChanged += Requests_ValueChanged;

            _playerAuthRepository.OnLoggedOut += HandleLoggedOut;

            return UniTask.CompletedTask;
        }

        private void HandleLoggedOut(string uid)
        {
            Dispose();
        }

        private void Friends_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.DatabaseError != null)
            {
                Debug.LogException(e.DatabaseError.ToException());
                return;
            }

            HashSet<string> activeFriendIds = new();

            if (e.Snapshot.Exists)
            {
                foreach (DataSnapshot friendSnap in e.Snapshot.Children)
                {
                    string friendId = friendSnap.Key;
                    activeFriendIds.Add(friendId);

                    if (!_friendListeners.ContainsKey(friendId))
                    {
                        var listener = new Listener(
                            _database.GetReference($"publicPlayerData/{friendId}"),
                            friendData => OnFriendDataChanged(friendId, friendData, isRequest: false));

                        _friendListeners.Add(friendId, listener);
                    }
                }
            }

            List<string> removedIds = new();

            foreach (var pair in _friendListeners)
            {
                if (!activeFriendIds.Contains(pair.Key))
                {
                    pair.Value.Dispose();
                    removedIds.Add(pair.Key);
                }
            }

            foreach (string id in removedIds)
            {
                _friendListeners.Remove(id);
                _friends.Remove(id);
            }

            OnFriendsChanged?.Invoke();
        }

        private void Requests_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.DatabaseError != null)
            {
                Debug.LogException(e.DatabaseError.ToException());
                return;
            }

            HashSet<string> activeRequestIds = new();

            if (e.Snapshot.Exists)
            {
                foreach (DataSnapshot request in e.Snapshot.Children)
                {
                    string friendId = request.Key;
                    activeRequestIds.Add(friendId);

                    if (!_requestListeners.ContainsKey(friendId))
                    {
                        var listener = new Listener(
                            _database.GetReference($"publicPlayerData/{friendId}"),
                            friendData => OnFriendDataChanged(friendId, friendData, isRequest: true));

                        _requestListeners.Add(friendId, listener);
                    }
                }
            }

            List<string> removedIds = new();

            foreach (var pair in _requestListeners)
            {
                if (!activeRequestIds.Contains(pair.Key))
                {
                    pair.Value.Dispose();
                    removedIds.Add(pair.Key);
                }
            }

            foreach (string id in removedIds)
            {
                _requestListeners.Remove(id);
                _requests.Remove(id);
            }

            OnRequestsChanged?.Invoke();
        }

        private void OnFriendDataChanged(string friendId, FriendData friendData, bool isRequest)
        {
            if (isRequest)
            {
                _requests[friendId] = friendData;
                OnRequestsChanged?.Invoke();
            }
            else
            {
                _friends[friendId] = friendData;
                OnFriendsChanged?.Invoke();
            }
        }

        public async UniTask<bool> SendFriendOffer(string friendId)
        {
            try
            {
                await _database
                    .GetReference($"friendRequests/{friendId}/{_userId}")
                    .SetValueAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public async UniTask<bool> AcceptFriendOffer(string friendId)
        {
            try
            {
                DataSnapshot snapshot = await _database
                    .GetReference($"friendRequests/{_userId}/{friendId}")
                    .GetValueAsync();

                if (!snapshot.Exists)
                    return false;

                Debug.Log(_userId + ": " + friendId);

                await _database.GetReference($"friends/{_userId}/{friendId}").SetValueAsync(true);
                await _database.GetReference($"friends/{friendId}/{_userId}").SetValueAsync(true);
                await _database.GetReference($"friendRequests/{_userId}/{friendId}").RemoveValueAsync();

                Debug.Log("sended");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public async UniTask<bool> DeclineFriendOffer(string friendId)
        {
            try
            {
                await _database
                    .GetReference($"friendRequests/{_userId}/{friendId}")
                    .RemoveValueAsync();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public async UniTask<bool> RemoveFriend(string friendId)
        {
            try
            {
                Dictionary<string, object> updates = new()
                {
                    [$"friends/{_userId}/{friendId}"] = null,
                    [$"friends/{friendId}/{_userId}"] = null
                };

                await _database.RootReference.UpdateChildrenAsync(updates);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public void Dispose()
        {
            if (_friendsReference != null)
                _friendsReference.ValueChanged -= Friends_ValueChanged;

            if (_requestsReference != null)
                _requestsReference.ValueChanged -= Requests_ValueChanged;

            foreach (var listener in _friendListeners.Values)
                listener.Dispose();

            foreach (var listener in _requestListeners.Values)
                listener.Dispose();

            _friendListeners.Clear();
            _requestListeners.Clear();
            _friends.Clear();
            _requests.Clear();

            if (_playerAuthRepository != null)
                _playerAuthRepository.OnLoggedOut -= HandleLoggedOut;
        }

        private class Listener : IDisposable
        {
            private readonly DatabaseReference _reference;
            private readonly Action<FriendData> _onChanged;

            public Listener(DatabaseReference reference, Action<FriendData> onChanged)
            {
                _reference = reference;
                _onChanged = onChanged;
                _reference.ValueChanged += Reference_ValueChanged;
            }

            private void Reference_ValueChanged(object sender, ValueChangedEventArgs e)
            {
                if (e.DatabaseError != null)
                {
                    Debug.LogException(e.DatabaseError.ToException());
                    return;
                }

                if (!e.Snapshot.Exists)
                    return;

                string json = e.Snapshot.GetRawJsonValue();
                if (string.IsNullOrEmpty(json))
                    return;

                FriendData friendData = JsonUtility.FromJson<FriendData>(json);
                _onChanged?.Invoke(friendData);
            }

            public void Dispose()
            {
                _reference.ValueChanged -= Reference_ValueChanged;
            }
        }
    }

    [Serializable]
    public class FriendData
    {
        public int elo;
        public int icon;
        public string name;
        public bool online;
    }
}