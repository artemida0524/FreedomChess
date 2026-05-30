
using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Runtime.Boot
{

	public class Bootstrap : MonoBehaviour
	{
        private void Start()
        {
            Application.targetFrameRate = 60;
            SceneManager.LoadScene(1);
        }
    }

}