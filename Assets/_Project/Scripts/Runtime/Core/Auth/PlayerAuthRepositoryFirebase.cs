using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Game.Core;
using Game.Core.Player;
using Game.Runtime.Core.Connections;
using Google;
using System;
using UnityEngine;
using Zenject;

namespace Game.Runtime.Core.Auth
{
    public class PlayerAuthRepositoryFirebase : IPlayerAuthRepository
    {
        private const string PLAYER_LOGIN_CONFIG = "PlayerLogInConfig";
        private IConnectionService _connectionChecker;
        private bool _isConnected;


        private GoogleSignInWithFirebase googleSignInWithFirebase;

        public event Action OnLoggedOut;

        public bool IsLoggedIn { get; private set; } = false;

        public string Email
        {
            get
            {
                if (FirebaseAuth.DefaultInstance.CurrentUser != null)
                {
                    return FirebaseAuth.DefaultInstance.CurrentUser.Email;
                }
                return string.Empty;
            }
        }

        [Inject]
        private void Construct(IConnectionService connectionChecker)
        {
            _connectionChecker = connectionChecker;
        }

        public async UniTask Init()
        {
            _connectionChecker.OnConnectionChanged += OnConnectionChangedHandler;
            _isConnected = _connectionChecker.IsConnected;
            PlayerLogInConfig config = GetPlayerLoginConfigLocal();
            try
            {
                await FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
            }
            catch (Exception)
            {
                Debug.Log("No user cached");
                IsLoggedIn = false;
                return;
            }
            if (config != null)
            {
                if (config.IsLogginedIn)
                {
                    IsLoggedIn = true;
                    return;
                }
            }
            IsLoggedIn = false;
        }




        public PlayerCacheResult IsPlayerCached()
        {
            if (FirebaseAuth.DefaultInstance.CurrentUser != null)
            {
                return PlayerCacheResult.Found;
            }
            return PlayerCacheResult.NotFound;
        }

        public async UniTask<AuthError> CreatePlayerAsync(string email, string password)
        {
            if (!_isConnected)
            {
                Debug.Log("Connection failed");
                return AuthError.NoInternetConnection;
            }

            FirebaseAuth firebaseAuth = FirebaseAuth.DefaultInstance;
            try
            {
                AuthResult authResult = await firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);


                if (authResult.User != null)
                {
                    SetPlayerLogInConfigLocal(true);
                    Debug.Log("login success");
                    IsLoggedIn = true;
                    return AuthError.None;
                }
            }
            catch (FirebaseException e)
            {
                AuthError authError = (AuthError)e.ErrorCode;
                Debug.Log($"Authentication failed: {authError}");
                return authError;
            }

            return AuthError.Unimplemented;
        }

        public async UniTask<AuthError> LogInAsync()
        {
            if (!_isConnected)
            {
                Debug.Log("Connection failed");
                return AuthError.NoInternetConnection;
            }

            //try
            //{
            //    await FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
            //}
            //catch (Exception)
            //{
            //    return AuthError.UserNotFound;
            //}

            bool reloadResult = await ReloadAsync();
            if (!reloadResult)
            {
                return AuthError.UserNotFound;
            }

            PlayerCacheResult playerExistenceResult = IsPlayerCached();

            if (playerExistenceResult != PlayerCacheResult.Found)
            {
                Debug.Log("Player not exist");
                return AuthError.UserNotCached;
            }

            bool emailVerified = await IsEmailVerifiedAsync();
            if (!emailVerified)
            {
                return AuthError.UnverifiedEmail;
            }

            SetPlayerLogInConfigLocal(true);
            return AuthError.None;
        }

        public async UniTask<AuthError> LogInAsync(string email, string password)
        {
            if (!_isConnected)
            {
                Debug.Log("Connection failed");
                return AuthError.NoInternetConnection;
            }
            FirebaseAuth firebaseAuth = FirebaseAuth.DefaultInstance;
            AuthResult authResult = null;
            try
            {
                authResult = await firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);
                if (authResult.User != null)
                {
                    SetPlayerLogInConfigLocal(true);
                    Debug.Log("login player success");
                    IsLoggedIn = true;
                    return AuthError.None;
                }
            }
            catch (FirebaseException e)
            {
                AuthError authError = (AuthError)e.ErrorCode;
                Debug.Log($"Authentication failed: {authError}");
                return authError;
            }
            Debug.Log("login player failed");
            await ReloadAsync();
            return AuthError.Unimplemented;
        }

        public async UniTask<bool> IsEmailVerifiedAsync()
        {
            if (!_isConnected)
            {
                Debug.Log("inter connect");
                return false;
            }
            try
            {
                await FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified;
        }

        public async UniTask<EmailVerificationType> SendEmailVerificationAsync()
        {
            if (!_isConnected)
            {
                Debug.Log("no internet");
            }
            await ReloadAsync();
            bool emailVerified = await IsEmailVerifiedAsync();
            if (!emailVerified)
            {
                await FirebaseAuth.DefaultInstance.CurrentUser.SendEmailVerificationAsync();
                return EmailVerificationType.Sended;
            }
            return EmailVerificationType.AlreadyVerified;
        }

        public async UniTask<bool> ResetPassword(string email)
        {
            if (!_isConnected)
            {
                return false;
            }
            try
            {
                await FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(email);

            }
            catch (FirebaseException)
            {
                return false;
            }
            return true;
        }


        public async UniTask<AuthError> SignInWithGoogle()
        {
            if (!_isConnected)
            {
                return AuthError.NoInternetConnection;
            }
            if (googleSignInWithFirebase == null)
            {
                googleSignInWithFirebase = new GoogleSignInWithFirebase();
            }

            Credential user = await googleSignInWithFirebase.GetGoogleCredentialAsync();

            if (user == null)
            {
                return AuthError.InvalidCustomToken;
            }

            FirebaseUser firebaseUser = await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(user);

            if (firebaseUser.Email == null)
            {
                return AuthError.InvalidEmail;
            }
            IsLoggedIn = true;
            SetPlayerLogInConfigLocal(true);
            return AuthError.None;
        }

        public void LogOut()
        {
            //FirebaseAuth.DefaultInstance.SignOut();
            try
            {
                GoogleSignIn.DefaultInstance.SignOut();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            SetPlayerLogInConfigLocal(false);
            OnLoggedOut?.Invoke();
            IsLoggedIn = false;
        }

        private async UniTask<bool> ReloadAsync()
        {
            try
            {
                await FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
                await FirebaseAuth.DefaultInstance.CurrentUser.TokenAsync(true);
                return true;

            }
            catch (Exception)
            {
                LogOut();
                Debug.Log("Reloaded go wrong");
                return false;
            }
        }

        private async void OnConnectionChangedHandler(bool obj)
        {
            _isConnected = obj;

            if (_isConnected)
            {
                try
                {
                    await FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
                }
                catch (Exception)
                {
                    LogOut();
                    Debug.Log("Reloaded go wrong");
                }
            }
        }

        private PlayerLogInConfig GetPlayerLoginConfigLocal()
        {
            if (PlayerPrefs.HasKey(PLAYER_LOGIN_CONFIG))
            {
                string json = PlayerPrefs.GetString(PLAYER_LOGIN_CONFIG);
                PlayerLogInConfig playerLoginConfig = JsonUtility.FromJson<PlayerLogInConfig>(json);
                return playerLoginConfig;
            }
            return null;
        }

        private void SetPlayerLogInConfigLocal(bool isLoggedIn)
        {
            string json = JsonUtility.ToJson(new PlayerLogInConfig() { IsLogginedIn = isLoggedIn });
            PlayerPrefs.SetString(PLAYER_LOGIN_CONFIG, json);
        }


        [Serializable]
        private class PlayerLogInConfig
        {
            public bool IsLogginedIn;
        }
    }
}