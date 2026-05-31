using UnityEngine;
using Zenject;
namespace Game.Runtime.Core.Connections
{
    public class ConnectionStatusPanelView : MonoBehaviour, IConnectionStatusPanelView
    {
        [SerializeField] private Animator animator;
        private IConnectionService _connectionService;

        [Inject]
        private void Construct(IConnectionService service)
        {
            _connectionService = service;
        }


        public void Init()
        {
            _connectionService.OnConnectionChanged += OnConnectionChangedHandler;
        }

        private void OnConnectionChangedHandler(bool status)
        {
            if (status)
            {
                animator.SetTrigger("Ok");
            }
            else
            {
                animator.SetTrigger("Start");

            }
        }
    }
}
