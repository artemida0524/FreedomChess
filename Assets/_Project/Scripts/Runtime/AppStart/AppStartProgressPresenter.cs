using DG.Tweening;
using DG.Tweening.Core;
using Game.Runtime.AppStart.StartupFlow;
using Game.Runtime.AppStart.Views;

using Game.Runtime.Core.Tasks;
using UnityEngine;
using Zenject;

namespace Game.Runtime.AppStart
{
    public class AppStartProgressPresenter : MonoBehaviour
    {
        public AppStartPanelView _appStartPanelView;
        public AppStartupFlow _startupFlow;

        //private IAppStartPanelView _appStartPanelView;
        //private IAppStartupFlow _startupFlow;


        private ITaskInfo _currentTaskInfo;

        private float _taskProgress = 0f;
        private Tween _taskTween;

        private float _globalProgress = 0f;
        private Tween _globalTween;


        //[Inject]
        //private void Construct(
        //    IAppStartPanelView appStartPanelView,
        //    IAppStartupFlow startupFlow

        //    )
        //{
        //    _appStartPanelView = appStartPanelView;
        //    _startupFlow = startupFlow;

        //}

        public void Init()
        {
            _startupFlow.OnTaskStarted += OnTaskStartedHandler;
            _startupFlow.OnProgressChanged += OnGlobalProgressChangedHandler;
        }

        private async void OnTaskStartedHandler(ITaskInfo info)
        {
            if (_currentTaskInfo != null)
            {
                _currentTaskInfo.OnProgressChanged -= OnProgressChangedHandler;
            }

            //info.OnProgressChanged += OnProgressChangedHandler;
            //_currentTaskInfo = info;
            ////_appStartPanelView.SetTaskValue(0f);
            //OnProgressChangedHandler(0f);

            //string result = await _localizationService.GetAsync(info.Name, TableKey.AppStartTasks);
            //if (string.IsNullOrEmpty(result))
            //{
            //    _appStartPanelView.SetTaskName($"Key for {info.Name} not found");
            //    return;
            //}
            _appStartPanelView.SetTaskName(info.Name);
        }

        private void OnGlobalProgressChangedHandler(float value)
        {
            ChangeProgress(() => _globalProgress, newValue => { _appStartPanelView.SetGlobalValue(newValue); _globalProgress = newValue; }, value, 0.3f, _globalTween);
        }

        private void OnProgressChangedHandler(float value)
        {
            ChangeProgress(() => _taskProgress, newValue => { _appStartPanelView.SetTaskValue(newValue); _taskProgress = newValue; }, value, 0.3f, _taskTween);
        }

        private void ChangeProgress(DOGetter<float> beginValue, DOSetter<float> setter, float endValue, float duration, Tween tween)
        {
            tween?.Kill();
            tween = DOTween.To(beginValue, setter, endValue, duration).SetLink(gameObject);
        }
    }

}
