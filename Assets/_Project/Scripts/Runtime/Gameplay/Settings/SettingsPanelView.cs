using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Game.Runtime.Core.Auth;
using Game.Runtime.Core.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Game.Runtime.Gameplay
{
    public class SettingsPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Icons icons;

        [SerializeField] private Button exitButton;


        [SerializeField] private Button nameButton;
        [SerializeField] private Button eloButton;

        [SerializeField] private Button uidButton;
        [SerializeField] private TextMeshProUGUI uidText;

        [SerializeField] private Button iconButton;

        [SerializeField] private GameObject inputs;

        [SerializeField] private GameObject namePanel;
        [SerializeField] private TMP_InputField nameText;
        [SerializeField] private Button nameEnter;

        [SerializeField] private GameObject eloPanel;
        [SerializeField] private TMP_InputField eloText;
        [SerializeField] private Button eloEnter;


        [SerializeField] private Button logoutButton;

        private PlayerProvider _playerProvider;
        private IPlayerAuthRepository _playerRepository;

        [Inject]
        private void Construct(PlayerProvider playerProvider, IPlayerAuthRepository playerAuthRepository)
        {
            _playerProvider = playerProvider;
            _playerRepository = playerAuthRepository;
        }

        public void SetActive(bool isActive)
        {
            panel.SetActive(isActive);
            if (isActive)
            {
                inputs.SetActive(false);
                namePanel.SetActive(false);
                eloPanel.SetActive(false);
            }
        }

        public void Init()
        {
            icons.Init();
            exitButton.onClick.AddListener(() => SetActive(false));
            uidButton.onClick.AddListener(() => GUIUtility.systemCopyBuffer = FirebaseAuth.DefaultInstance.CurrentUser.UserId);
            uidText.text = $"uid: {FirebaseAuth.DefaultInstance.CurrentUser.UserId}";

            nameButton.onClick.AddListener(() =>
            {
                inputs.SetActive(true);
                namePanel.SetActive(true);
                eloPanel.SetActive(false);
            });

            eloButton.onClick.AddListener(() =>
            {
                inputs.SetActive(true);
                namePanel.SetActive(false);
                eloPanel.SetActive(true);
            });

            nameEnter.onClick.AddListener(async () =>
            {

                if (string.IsNullOrEmpty(nameText.text))
                {
                    return;
                }

                canvasGroup.interactable = false;
                
                await _playerProvider.SetName(nameText.text);

                canvasGroup.interactable = true;

                inputs.SetActive(false);
            });

            eloEnter.onClick.AddListener(async () =>
            {

                if (string.IsNullOrEmpty(eloText.text))
                {
                    return;
                }

                if(!int.TryParse(eloText.text, out int elo))
                {
                    return;
                }

                canvasGroup.interactable = false;

                await _playerProvider.SetElo(elo);

                canvasGroup.interactable = true;

                inputs.SetActive(false);
            });
            
            iconButton.onClick.AddListener(() => icons.SetActive(true));
            icons.iconClicked += async iconStat =>
            {
                canvasGroup.interactable = false;
                await _playerProvider.SetIcon(iconStat.iconId);
                canvasGroup.interactable = true;
                inputs.SetActive(false);
                icons.SetActive(false);
            };
            logoutButton.onClick.AddListener(() => 
            {
                _playerRepository.LogOut();
                SceneManager.LoadScene(0);
            });
        }
        
        
    }

}
