using System.Collections;
using Bases;
using Interface;
using UnityEngine;

namespace Skill
{
    public class ShortSword : Weapon
    {
        private bool _isCloaking = false;
        public GameObject leftSword;
        public GameObject rightSword;
        
        protected override void Skill0()
        {
            animator.SetTrigger(ATTACK);
        }

        protected override void Skill1()
        {
            _player.Dash(20, 0.2f);
        }

        protected override void Skill2()
        {
            if (_isCloaking) return;
            StartCoroutine(Cloaking(5f));
        }

        private IEnumerator Cloaking(float duration)
        {
            _isCloaking = true;
            var t = 0f;
            _player.Cloaking(true);
            leftSword.SetActive(false);
            rightSword.SetActive(false);
            
            while (t < duration)
            {
                t += Time.deltaTime;
                yield return null;
            }
            _player.Cloaking(false);
            leftSword.SetActive(true);
            rightSword.SetActive(true);
            _isCloaking = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other.gameObject != _player.gameObject)
            {
                other.GetComponent<IDamageable>()?.Damaged(0.5f, transform.position);
            }
        }
    }
}