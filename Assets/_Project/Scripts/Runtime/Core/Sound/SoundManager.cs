using System;
using UnityEngine;

namespace Game.Core.Sounds
{
	public class SoundManager : MonoBehaviour
	{
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private AudioClip click;
        [SerializeField] private AudioClip move;
        [SerializeField] private AudioClip take;
        [SerializeField] private AudioClip request;
        [SerializeField] private AudioClip startBattle;

		public static SoundManager Instance { get; private set; }


        private void Awake()
        { 
            Instance = this;
        }

        public void Click()
        {
            audioSource.PlayOneShot(click);
        }


        public void Move()
        {
            audioSource.PlayOneShot(move);
        }
        public void Take()
        {
            audioSource.PlayOneShot(take);
        
        }

        public void Request()
        {
            audioSource.PlayOneShot(request);
        }

        public void StartBattle()
        {
            audioSource.PlayOneShot(startBattle);

        }
    }

}