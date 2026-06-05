using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using System;
using Unity.VisualScripting;
using UnityEngine;


namespace Game.Runtime.Core.Player
{
    public class PlayerProvider
    {
        public string Name { get; private set; }
        public string Id { get; private set; }
        public int Elo { get; private set; }

        public int Icon { get; private set; }

        public event Action OnPlayerDataChanged;

        private FirebaseAuth _auth;
        private FirebaseDatabase _database;
        private FirebaseFirestore _firestore;


        public async UniTask Init()
        {
            _auth = FirebaseAuth.DefaultInstance;
            _database = FirebaseDatabase.DefaultInstance;
            _firestore = FirebaseFirestore.DefaultInstance;

            DataSnapshot snap = await _database.GetReference($"playerConfigs/{_auth.CurrentUser.UserId}").GetValueAsync();

            if (!snap.Exists)
            {
                PlayerConfig config = new PlayerConfig() { name = $"Player{UnityEngine.Random.Range(1000, 100000)}", icon = 1, elo = 1000 };

                await _database.GetReference($"playerConfigs/{_auth.CurrentUser.UserId}").SetRawJsonValueAsync(JsonUtility.ToJson(config));
                await _database.GetReference($"publicPlayerData/{_auth.CurrentUser.UserId}").SetRawJsonValueAsync(JsonUtility.ToJson(config));

                Name = config.name;
                Icon = config.icon;
                Elo = config.elo;
            }
            else
            {
                PlayerConfig playerConfig = JsonUtility.FromJson<PlayerConfig>(snap.GetRawJsonValue());
                Name = playerConfig.name;
                Icon = playerConfig.icon;
                Elo = playerConfig.elo;
            }

            Id = _auth.CurrentUser.UserId;



            await _database.GetReference($"publicPlayerData/{_auth.CurrentUser.UserId}/online").SetValueAsync(true);
            await _database.GetReference($"publicPlayerData/{_auth.CurrentUser.UserId}/online").OnDisconnect().SetValue(false);

        }

        public async UniTask SetName(string name)
        {
            await _database.GetReference($"playerConfigs/{_auth.CurrentUser.UserId}/name").SetValueAsync(name);
            await _database.GetReference($"publicPlayerData/{_auth.CurrentUser.UserId}/name").SetValueAsync(name);

            Name = name;

            OnPlayerDataChanged?.Invoke();
        }

        public async UniTask SetIcon(int icon)
        {
            await _database.GetReference($"playerConfigs/{_auth.CurrentUser.UserId}/icon").SetValueAsync(icon);
            await _database.GetReference($"publicPlayerData/{_auth.CurrentUser.UserId}/icon").SetValueAsync(icon);
            Icon = icon;
            OnPlayerDataChanged?.Invoke();

        }

        public async UniTask SetElo(int elo)
        {
            await _database.GetReference($"playerConfigs/{_auth.CurrentUser.UserId}/elo").SetValueAsync(elo);
            await _database.GetReference($"publicPlayerData/{_auth.CurrentUser.UserId}/elo").SetValueAsync(elo);
            Elo = elo;
            OnPlayerDataChanged?.Invoke();
        }

        [Serializable]
        private class PlayerConfig
        {
            public string name;
            public int icon;
            public int elo;
        }
    }
}