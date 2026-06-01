using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Firestore;
using Game.Runtime.AppStart.Tasks;
using Game.Runtime.AppStart.Views;
//using Game.Runtime.Core.Localizations;
using Game.Runtime.Core.Connections;
using Game.Runtime.Core.Player;
using Game.Runtime.Core.Tasks;
using System;
using UnityEngine;
using Zenject;
using AuthError = Game.Runtime.Core.Auth.AuthError;

namespace Game.Runtime.AppStart.StartupFlow
{

    public class PreloadTask : ITaskInfo
    {
        public string Name => "Preload";

        public float Progress { get => 40; }

        public event Action<float> OnProgressChanged;
    }

    public class AppStartupFlow : MonoBehaviour, IAppStartupFlow
    {
        public SignInPanelView signInPanelView;

        public event Action<ITaskInfo> OnTaskStarted;
        public event Action<float> OnProgressChanged;

        private ITaskFactory _taskFactory;
        //private IGameLogSpawner _logSpawner;
        private IConnectionService _connectionService;
        private PlayerProvider _playerProvider;
        private FriendsProvider _friendsProvider;
        //private ITask<DependencyStatus> _preloadTask;
        private ITask<AuthError> _loginTask;
        //private ITask<InitDependenciesResult> _initDependencies;

        private bool _exitButtonClicked = false;

        [Inject]
        private void Construct(
            //ITaskFactory taskFactory,
            //IGameLogSpawner logSpawner,
            IConnectionService connectionService,
            PlayerProvider playerProvider,
            FriendsProvider friendsProvider
            )
        {
            //_taskFactory = taskFactory;
            //_logSpawner = logSpawner;
            _connectionService = connectionService;
            _playerProvider = playerProvider;
            _friendsProvider = friendsProvider;
        }
        

        public void Init()
        {
            //_preloadTask = _taskFactory.Create<PreloadTask>();
            //_loginTask = _taskFactory.Create<LoginTask>();
            //_initDependencies = _taskFactory.Create<InitDependenciesTask>();
        }


        public async UniTask RunAsync()
        {
            await RunStartupSequenceAsync();
        }

        private async UniTask RunStartupSequenceAsync()
        {

            //await RepeatUntilSuccess(
            //     () => TaskAsync(_preloadTask),
            //    DependencyStatus.Available);

            //OnProgressChanged?.Invoke(0.2f);

            //await CheckInternetConnection(5);

            //await RepeatUntilSuccess(
            //     () => TaskAsync(_loginTask),
            //    AuthError.None);
            //OnProgressChanged?.Invoke(0.6f);
            //await CheckInternetConnection(5);


            //await RepeatUntilSuccess(
            //    () => TaskAsync(_initDependencies),
            //    InitDependenciesResult.Successed);
            //OnProgressChanged?.Invoke(1f); 

            OnTaskStarted?.Invoke(new PreloadTask());
            _ = FirebaseApp.DefaultInstance;

            _ = Firebase.Auth.FirebaseAuth.DefaultInstance; 
            _ = FirebaseFirestore.DefaultInstance;

            Firebase.DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (status != Firebase.DependencyStatus.Available)
            {
                UnityEngine.Debug.LogError($"Could not resolve all Firebase dependencies: {status}");
                return;
            }
            FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
            FirebaseFirestore.DefaultInstance.Settings.PersistenceEnabled = false;

            GoogleSignInWithFirebase.Init();

            _connectionService.Init();

            bool success = await WaitForInternet(10);
            if (!success)
            {
                await RunStartupSequenceAsync();
            }
            OnProgressChanged?.Invoke(0.2f);


            _loginTask = new LoginTask(signInPanelView);
            ProjectContext.Instance.Container.Inject(_loginTask);
            await RepeatUntilSuccess(
                 () => TaskAsync(_loginTask),
                AuthError.None);
            OnProgressChanged?.Invoke(0.6f);

            _friendsProvider.Dispose();
            await _playerProvider.Init();
            await _friendsProvider.Init();

            //await _friendsProvider.AcceptFriendOffer("bwE1xS7XpxNSqh5bOycu6lrg9UH2");
        }

        private async UniTask RepeatUntilSuccess<T>(
            Func<UniTask<T>> taskFunc,
            T successValue)
            where T : IComparable
        {
            while (true)
            {
                T result = await taskFunc();

                if (result.CompareTo(successValue) == 0)
                    break;
                //Debug.LogWarning($"{typeof(T).Name}.{result.ToString()}");
                //IGameLogger gameLogger = _logSpawner.Log($"{typeof(T).Name}.{result.ToString()}", TableKey.AppStartInfoLogs);

                //gameLogger.OnExitButtonClicked += OnExitButtonClicked;
                //await WaitForOkButton();
                await UniTask.Delay(1000);
            }

        }

        private async UniTask WaitForOkButton()
        {
            _exitButtonClicked = false;
            await UniTask.WaitUntil(() => _exitButtonClicked);
        }

        private void OnExitButtonClicked()
        {
            //_exitButtonClicked = true;
            //gameLogger.OnExitButtonClicked -= OnExitButtonClicked;
        }

        private async UniTask<TSource> TaskAsync<TSource>(ITask<TSource> task)
        {
            OnTaskStarted?.Invoke(task);
            return await task.Execute();
        }


        public async UniTask CheckInternetConnection(float timeoutSeconds)
        {
            while (true)
            {
                bool status = await WaitForInternet(timeoutSeconds);

                if (status)
                {
                    return;
                }

                //_logSpawner.Log($"{DependencyStatus.NoInternetConnection}", TableKey.AppStartInfoLogs);
                await WaitForOkButton();
            }
        }

        private async UniTask<bool> WaitForInternet(float timeoutSeconds)
        {
            int index = await UniTask.WhenAny(
                UniTask.Delay(TimeSpan.FromSeconds(timeoutSeconds)), // 0
                UniTask.WaitUntil(() => _connectionService.IsConnected) // 1
                );
            return index == 1;
        }

    }
}
