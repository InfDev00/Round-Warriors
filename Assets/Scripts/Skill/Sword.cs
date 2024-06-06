using System;
using System.Collections;
using Bases;
using Debuff;
using Interface;
using Unity.VisualScripting;
using UnityEngine;

namespace Skill
{
    public class Sword : Weapon
    {
        [SerializeField] private float healingDuration;
        private bool _isHealing;

        protected override void Skill0()
        {
            animator.SetTrigger(ATTACK);
        }

        protected override void Skill1()
        {
            _player.Dash(10, 0.6f);
        }

        protected override void Skill2()
        {
            if (_isHealing) return;
            StartCoroutine(IHeal());
        }
        
        private IEnumerator IHeal()
        {
            _player.SetPause(true);
            _isHealing = true;
            
            animator.SetBool(HEALING, true);
            var healingTime  = 0f;

            while (healingTime < healingDuration)
            {
                healingTime += 1f;
                _player.Healing(1.5f);
                yield return new WaitForSeconds(1f);
            }
            animator.SetBool(HEALING, false);
            _player.SetPause(false);
            _isHealing = false;
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