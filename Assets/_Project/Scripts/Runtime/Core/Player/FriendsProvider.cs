using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using Game.Runtime.Core.Auth;
using System;
using UnityEngine;
using Zenject;


namespace Game.Runtime.Core.Player
{
    public class FriendsProvider
    {

        private FirebaseAuth _auth;
        private FirebaseDatabase _database;
        private FirebaseFirestore _firestore;

        private bool _part2 = false;

        private IPlayerAuthRepository _playerAuthRepository;

        [Inject]
        private void Construct(IPlayerAuthRepository playerAuthRepository)
        {
            _playerAuthRepository = playerAuthRepository;
        }

        public async UniTask Init()
        {
            _auth = FirebaseAuth.DefaultInstance;
            _database = FirebaseDatabase.DefaultInstance;
            _firestore = FirebaseFirestore.DefaultInstance;

            //DataSnapshot snap = await _database.GetReference($"friends/{_auth.CurrentUser.UserId}").GetValueAsync();

            //if (!snap.Exists)
            //{
            //	Debug.Log("No friends found");
            //	return;
            //}

            //         foreach (DataSnapshot friendSnap in snap.Children)
            //{
            //	Debug.Log(friendSnap.Key);
            //         }

            //DataSnapshot offerSnap = await _database.GetReference($"friendOffers/{_auth.CurrentUser.UserId}").GetValueAsync();

            //if (!offerSnap.Exists)
            //{
            //    Debug.Log("No offers found");
            //    return;
            //}

            //foreach (DataSnapshot offer in offerSnap.Children)
            //{
            //    Debug.Log(offer.Key);
            //}
            _database.GetReference($"friends/{_auth.CurrentUser.UserId}").ValueChanged += FriendsProvider_ValueChanged;
            _database.GetReference($"friendOffers/{_auth.CurrentUser.UserId}").ValueChanged += FriendOffers_ValueChanged;

            if (_part2)
            {
                _playerAuthRepository.OnLoggedOut += () =>
        {
            _database.GetReference($"friends/{_auth.CurrentUser.UserId}").ValueChanged -= FriendsProvider_ValueChanged;
            _database.GetReference($"friendOffers/{_auth.CurrentUser.UserId}").ValueChanged -= FriendOffers_ValueChanged;
            _part2 = true;
        };
            }
            await UniTask.WaitForSeconds(2);
        }

        private void FriendOffers_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (!e.Snapshot.Exists)
            {
                Debug.Log("No offers found");
                return;
            }

            foreach (DataSnapshot offer in e.Snapshot.Children)
            {
                Debug.Log(offer.Key);
            }
        }

        private void FriendsProvider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (!e.Snapshot.Exists)
            {
                Debug.Log("No friends found");
                return;
            }

            foreach (DataSnapshot friendSnap in e.Snapshot.Children)
            {
                Debug.Log(friendSnap.Key);
            }
        }

        public async UniTask<bool> SendFriendOffer(string friendId)
        {

            try
            {
                await _database.GetReference($"friendOffers/{friendId}/{_auth.CurrentUser.UserId}").SetValueAsync("");

                return true;
            }
            catch (Exception)
            {

                return false;
            }


        }

        public async UniTask<bool> AcceptFriendOffer(string friendId)
        {
            try
            {
                DataSnapshot dataSnapshot = await _database.GetReference($"friendOffers/{_auth.CurrentUser.UserId}/{friendId}").GetValueAsync();

                if (!dataSnapshot.Exists)
                {
                    return false;
                }

                await _database.GetReference($"friends/{_auth.CurrentUser.UserId}/{friendId}").SetValueAsync("");
                await _database.GetReference($"friends/{friendId}/{_auth.CurrentUser.UserId}").SetValueAsync("");
                await _database.GetReference($"friendOffers/{_auth.CurrentUser.UserId}/{friendId}").RemoveValueAsync();
                

                return true;
            }
            catch (Exception)
            {
                return false;
            }




        }

    }
}