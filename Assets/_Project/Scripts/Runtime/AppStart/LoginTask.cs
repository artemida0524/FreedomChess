using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Game.Core.Player;
using Game.Runtime.AppStart.Views;
using Game.Runtime.Core.Auth;
using Game.Runtime.Core.Tasks;
using System;
using UnityEngine;
using Zenject;
using AuthError = Game.Runtime.Core.Auth.AuthError;


namespace Game.Runtime.AppStart.Tasks
{
    public class LoginTask : ITask<AuthError>
    {
        public string Name => nameof(LoginTask);
        public float Progress { get; private set; } = 0f;

        public event Action<float> OnProgressChanged;

        private IPlayerAuthRepository _playerAuthRepository;
        private ISignInPanelView _signInPanel;
        //private IGameLogSpawner _logSpawner;

        private bool _isInitialized = false;
        private bool _continue = false;

        [Inject]
        private void Construct(
            IPlayerAuthRepository playerDataRepository
            //ISignInPanelView signInPanel
            //IGameLogSpawner logSpawner
            )
        {
            _playerAuthRepository = playerDataRepository;
            //_signInPanel = signInPanel;
            //_logSpawner = logSpawner;
        }

        public LoginTask(
            ISignInPanelView signInPanel)
        {

            _signInPanel = signInPanel;
        }

        private void Init()
        {
            _signInPanel.OnSignInButtonClicked += OnSignInButtonClickedHandler;
            _signInPanel.OnSignUpButtonClicked += OnSignUpButtonClickedHandler;

            _signInPanel.OnGoogleSignInClicked += OnGoogleSignInClickedHandler;
            _signInPanel.OnResetPasswordButtonClicked += OnResetPasswordButtonClickedHandler;

            _signInPanel.OnGoBackToExistentClicked += OnGoBackToExistentClickedHandler;

            _isInitialized = true;
        }


        public async UniTask<AuthError> Execute()
        {
            if (!_isInitialized)
            {
                Init();
            }

            UpdateProgress(0.1f);

            if (_playerAuthRepository.IsLoggedIn)
            {
                return await LogInFound();
            }

            return await LogInNotFound();
        }


        private void UpdateProgress(float progress)
        {
            Progress = progress;
            OnProgressChanged?.Invoke(Progress);
        }


        private async UniTask<AuthError> LogInFound()
        {
            AuthError authError = await _playerAuthRepository.LogInAsync();
            UpdateProgress(1f);

            if (authError != AuthError.None)
            {
                _playerAuthRepository.LogOut();
            }
            return authError;
        }

        private async UniTask<AuthError> LogInNotFound()
        {
            _signInPanel.Open();

            if (_playerAuthRepository.IsPlayerCached() == PlayerCacheResult.Found)
            {
                _signInPanel.ShowGoBackToSignedPanel(_playerAuthRepository.Email);
            }

            await UniTask.WaitUntil(() => _continue);
            if (_playerAuthRepository.IsLoggedIn)
            {
                UpdateProgress(1f);
            }


            return AuthError.None;
        }

        private async void OnSignInButtonClickedHandler()
        {
            _signInPanel.SetInteractable(false);
            _signInPanel.SetLoading(true);

            AuthError authError = await _playerAuthRepository.LogInAsync(_signInPanel.Email, _signInPanel.Password);


            switch (authError)
            {
                case AuthError.None:
                    bool emailVerified = await _playerAuthRepository.IsEmailVerifiedAsync();
                    if (!emailVerified)
                    {
                        //_logSpawner.Log($"AuthError.UnverifiedEmail", TableKey.AppStartInfoLogs);
                        _signInPanel.DisableContinueAs();
                    }
                    else
                    {
                        _continue = true;
                        _signInPanel.Close();
                    }

                    break;

                default:
                    //_logSpawner.Log($"{nameof(AuthError)}.{authError.ToString()}", TableKey.AppStartInfoLogs);
                    break;

            }

            _signInPanel.SetInteractable(true);
            _signInPanel.SetLoading(false);
        }

        private async void OnSignUpButtonClickedHandler()
        {
            if (_signInPanel.Password != _signInPanel.ConfirmPassword)
            {
                //_logSpawner.Log("PasswordsDontMatch", TableKey.AppStartInfoLogs);
                return;
            }

            _signInPanel.SetInteractable(false);
            _signInPanel.SetLoading(true);

            AuthError authError = await _playerAuthRepository.CreatePlayerAsync(_signInPanel.Email, _signInPanel.Password);

            switch (authError)
            {
                case AuthError.None:
                    await _playerAuthRepository.SendEmailVerificationAsync();
                    //_logSpawner.Log("CreatedEmail", TableKey.AppStartInfoLogs);
                    _signInPanel.DisableContinueAs();

                    break;

                default:

                    //_logSpawner.Log($"{nameof(AuthError)}.{authError.ToString()}", TableKey.AppStartInfoLogs);


                    break;
            }

            _signInPanel.SetInteractable(true);
            _signInPanel.SetLoading(false);
        }

        private async void OnResetPasswordButtonClickedHandler()
        {
            _signInPanel.SetInteractable(false);
            _signInPanel.SetLoading(true);

            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            if (string.IsNullOrEmpty(_signInPanel.Email))
            {
                //_logSpawner.Log("EnterEmailForReset", TableKey.AppStartInfoLogs);

                _signInPanel.SetInteractable(true);
                _signInPanel.SetLoading(false);

                return;
            }
            bool result = await _playerAuthRepository.ResetPassword(_signInPanel.Email);

            if (result)
            {
                //_logSpawner.Log("ResetPasswordSended", TableKey.AppStartInfoLogs);
            }
            else
            {
                //_logSpawner.Log("ResetPasswordGoWrong");
            }

            _signInPanel.SetInteractable(true);
            _signInPanel.SetLoading(false);


        }

        private async void OnGoogleSignInClickedHandler()
        {
            _signInPanel.SetInteractable(false);
            _signInPanel.SetLoading(true);
            try
            {

                AuthError authError = await _playerAuthRepository.SignInWithGoogle();

                switch (authError)
                {
                    case AuthError.None:
                        _continue = true;
                        _signInPanel.Close();

                        break;
                    default:

                        //_logSpawner.Log($"{nameof(AuthError)}.{authError.ToString()}", TableKey.AppStartInfoLogs);

                        
                        break;
                }

            }
            catch (Exception e)
            {
                //_logSpawner.Log(e.Message);
            }
            finally
            {
                _signInPanel.SetInteractable(true);
                _signInPanel.SetLoading(false);
            }
        }

        private async void OnGoBackToExistentClickedHandler()
        {
            

            _signInPanel.SetInteractable(false);
            _signInPanel.SetLoading(true);

            try
            {
                AuthError authError = await _playerAuthRepository.LogInAsync();

                if (authError != AuthError.None)
                {

                    //_logSpawner.Log($"{nameof(AuthError)}.{authError.ToString()}", TableKey.AppStartInfoLogs);

                    return;
                }

                _signInPanel.Close();
                _continue = true;
            }
            catch (FirebaseException e)
            {
                //_logSpawner.Log($"{nameof(AuthError)}.{((AuthError)e.ErrorCode)}", TableKey.AppStartInfoLogs);
            }
            finally
            {
                _signInPanel.SetInteractable(true);
                _signInPanel.SetLoading(false);
            }
        }

    }

}