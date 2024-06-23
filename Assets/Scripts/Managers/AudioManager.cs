using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip[] audioClips;
        private readonly Dictionary<string, AudioClip> _audioDictionary = new Dictionary<string, AudioClip>();
        private AudioSource _audioSource;

        #region const

        public const string HIT = "61_Hit_03";
        public const string DIE = "21_Debuff_01";
        
        public const string SHORT_SWORD_ATTACK = "03_Step_grass_03";
        public const string SHORT_SWORD_DASH = "03_Claw_03";
        public const string SHORT_SWORD_CLOAKING = "26_Swim_Submerged_02";
        
        public const string SWORD_ATTACK = "35_Miss_Evade_02";
        public const string SWORD_DASH = "25_Wind_01";
        public const string SWORD_HEAL = "13_Ice_explosion_01";
        
        public const string SPEAR_ATTACK = "56_Attack_03";
        public const string SPEAR_SPEED_UP = "48_Speed_up_02";
        public const string SPEAR_SPIN = "04_Fire_explosion_04_medium";
        
        #endregion
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            foreach(var clip in audioClips) _audioDictionary.TryAdd(clip.name, clip);
        }

        public void PlayAudio(string audioName)
        {
            if (!_audioDictionary.TryGetValue(audioName, out var clip)) return;
            _audioSource.Stop();
            _audioSource.PlayOneShot(clip);
        }
    }
}