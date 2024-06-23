using System;
using System.Collections;
using Bases;
using Interface;
using Managers;
using UnityEngine;

namespace Skill
{
    public class ShortSword : Weapon
    {
        private bool _isCloaking = false;
        public GameObject leftSword;
        public GameObject rightSword;
        private Coroutine _cloak;

        protected override void Skill0()
        {
            audioManager.PlayAudio(AudioManager.SHORT_SWORD_ATTACK);
            animator.SetTrigger(ATTACK);
            if (_isCloaking)
            {
                if (_cloak != null) StopCoroutine(_cloak);
                _player.Cloaking(false);
                _isCloaking = false;
            }
        }

        protected override void Skill1()
        {
            audioManager.PlayAudio(AudioManager.SHORT_SWORD_DASH);
            _player.Dash(20, 0.2f, _isCloaking);
        }

        protected override void Skill2()
        {
            if (_isCloaking) return;
            audioManager.PlayAudio(AudioManager.SHORT_SWORD_CLOAKING);
            _cloak = StartCoroutine(Cloaking(5f));
        }

        private IEnumerator Cloaking(float duration)
        {
            _isCloaking = true;
            var t = 0f;
            _player.Cloaking(true);
            
            while (t < duration)
            {
                t += Time.deltaTime;
                yield return null;
            }
            _player.Cloaking(false);
            _isCloaking = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other.gameObject != _player.gameObject)
            {
                other.GetComponent<IDamageable>()?.Damaged(1f, transform.position);
            }
        }
    }
}