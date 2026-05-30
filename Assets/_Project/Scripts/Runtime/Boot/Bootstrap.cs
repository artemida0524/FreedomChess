
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace Game.Runtime.Boot
{

	public class Bootstrap : MonoBehaviour
	{
        private async void Start()
        {
            Application.targetFrameRate = 60;
        }
    }

}