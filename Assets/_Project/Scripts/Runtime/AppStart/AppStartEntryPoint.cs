using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime.AppStart.StartupFlow;
using Game.Runtime.AppStart.Views;
using Game.Runtime.Background;
using Game.Runtime.Core.Auth;
using Game.Runtime.Core.Connections;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Runtime.AppStart
{
	public class AppStartEntryPoint : MonoBehaviour
	{
        [SerializeField] private BackgroundView _backgroundView;
        [SerializeField] private SignInPanelView _signInPanelView;
        [SerializeField] private AppStartupFlow flow;
        [SerializeField] private AppStartProgressPresenter presenter;

        private IPlayerAuthRepository _auth;
        private IConnectionService _connection;
        private IconStatManager _iconStatManager;


        [Inject]
        private void Constract(IConnectionService connection ,IPlayerAuthRepository auth, IconStatManager iconStatManager)
        {
            _connection = connection;
            _auth = auth;
            _iconStatManager = iconStatManager;
        }

        private async void Start()
        {
            Application.targetFrameRate = 60;
            PlayerPrefs.DeleteAll();
            _iconStatManager.Init();
            _backgroundView.Show();

            _signInPanelView.Init();
            presenter.Init();

            await _auth.Init();

            flow.Init();
            await flow.RunAsync();

            SceneManager.LoadScene(1);

            //_connection.Init();

            //bool internetAvailable = await WaitForInternet(5);

            //if (!internetAvailable)
            //{
            //    Debug.Log("no inet");
            //    return;
            //}

            //await _auth.Init();



            //await _auth.LogInAsync("asdf@adsfad.com", "asdfasdf");
        }

        private async UniTask<bool> WaitForInternet(float timeoutSeconds)
        {
            int index = await UniTask.WhenAny(
                UniTask.Delay(TimeSpan.FromSeconds(timeoutSeconds)), // 0
                UniTask.WaitUntil(() => _connection.IsConnected) // 1
                );
            return index == 1;
        }
    }

}