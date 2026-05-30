using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.AppStart.Views
{

    public class SignInPanelView : MonoBehaviour, ISignInPanelView
    {

        [Header("Sign In Panel")]
        [SerializeField] private TMP_InputField emailSignIn;
        [SerializeField] private TMP_InputField passwordSignIn;
        [SerializeField] private Button signInButton;
        [SerializeField] private Button googleSignIn;
        [Space(5)]
        [SerializeField] private Button dontHaveAccountButton; // "Don't have an account? Sign up"
        [SerializeField] private Button resetPasswordButton;   // "Forgot password?"

        [Header("Sign Up Panel")]
        [SerializeField] private TMP_InputField emailSignUp;
        [SerializeField] private TMP_InputField passwordSignUp;
        [SerializeField] private TMP_InputField confirmPasswordSignUp;
        [SerializeField] private Button signUpButton;
        [Space(5)]
        [SerializeField] private Button backFromSignUpButton;

        [Header("UI Panels")]
        [SerializeField] private GameObject signInPanel;
        [SerializeField] private GameObject signUpPanel;
        [SerializeField] private GoBacktoExistentPanelView goBackToExistentPanel;

        [Header("Visuals & Effects")]
        [SerializeField] private GameObject panelObject;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject loading;


        public string Email
        {
            get
            {
                return currentPanelState == PanelState.SignIn ? emailSignIn.text : emailSignUp.text;
            }
        }

        public string Password
        {
            get
            {
                return currentPanelState == PanelState.SignIn ? passwordSignIn.text : passwordSignUp.text;
            }
        }

        public string ConfirmPassword => confirmPasswordSignUp.text;

        public event Action OnSignInButtonClicked;
        public event Action OnGoogleSignInClicked;
        public event Action OnSignUpButtonClicked;
        public event Action OnResetPasswordButtonClicked;
        public event Action OnGoBackToExistentClicked;

        private PanelState currentPanelState = PanelState.SignIn;

        public void Init()
        {
            signInButton.onClick.AddListener(() => OnSignInButtonClicked?.Invoke());
            googleSignIn.onClick.AddListener(() => OnGoogleSignInClicked?.Invoke());
            signUpButton.onClick.AddListener(() => OnSignUpButtonClicked?.Invoke());
            resetPasswordButton.onClick.AddListener(() => OnResetPasswordButtonClicked?.Invoke());

            dontHaveAccountButton.onClick.AddListener(() =>
            {
                signUpPanel.SetActive(true);
                signInPanel.SetActive(false);

                currentPanelState = PanelState.SignUp;
            });

            backFromSignUpButton.onClick.AddListener(() =>
            {
                signUpPanel.SetActive(false);
                signInPanel.SetActive(true);

                currentPanelState = PanelState.SignIn;
            });

            goBackToExistentPanel.OnGoBackToExistentClicked += () => OnGoBackToExistentClicked?.Invoke();

        }

        public void Open()
        {
            panelObject.SetActive(true);
        }
        public void Close()
        {
            panelObject.SetActive(false);
        }

        public void ShowGoBackToSignedPanel(string email)
        {
            goBackToExistentPanel.gameObject.SetActive(true);
            goBackToExistentPanel.Init(email);
        }

        public void DisableContinueAs()
        {
            goBackToExistentPanel.gameObject.SetActive(false);
        }

        public void SetInteractable(bool interactable)
        {
            canvasGroup.interactable = interactable;
        }

        public void SetLoading(bool isLoading)
        {
            loading.SetActive(isLoading);
        }



        private enum PanelState
        {
            SignIn,
            SignUp
        }
    }
}
