using Firebase.Database;
using System;
using UnityEngine;
namespace Game.Runtime.Core.Connections
{

    public class ConnectionServiceFirebase : IConnectionService
    {
        public bool IsConnected => _isConnected;

        public event Action<bool> OnConnectionChanged;

        private DatabaseReference _db;
        private bool _isConnected;

        private bool _initialized = false;

        public void Init()
        {
            if (_initialized)
            {
                return;
            }

            _db = FirebaseDatabase
                .DefaultInstance
                .GetReference(".info/connected");

            _db.ValueChanged += ConnectionChangedHandler;
            _initialized = true;
        }

        private void ConnectionChangedHandler(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError($"Firebase connection error: {args.DatabaseError.Message}");
                SetConnectionState(false);
                return;
            }

            bool connected = false;

            if (args.Snapshot?.Value is bool value)
            {
                connected = value;
            }

            SetConnectionState(connected);

        }

        private void SetConnectionState(bool state)
        {
            if (_isConnected == state)
            {
                return;
            }

            _isConnected = state;
            OnConnectionChanged?.Invoke(_isConnected);
        }
    }

}