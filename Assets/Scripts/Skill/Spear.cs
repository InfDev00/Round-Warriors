using System.Collections;
using Bases;
using Debuff;
using Interface;
using Unity.VisualScripting;
using UnityEngine;

namespace Skill
{
    public class Spear : Weapon
    {
        private float _playerSpeed;
        private bool _isBuff = false;
        private bool _isSpin = false;
        [SerializeField] private float buffDuration = 5f;
        [SerializeField] private float spinDuration = 5f;
        
        private void Start() => _playerSpeed = _player.moveSpeed;

        protected override void Skill0()
        {
            animator.SetTrigger(ATTACK);
        }

        protected override void Skill1()
        {
            if (!_isBuff) StartCoroutine(ISpeadUp());
        }

        protected override void Skill2()
        {
            if (!_isSpin) StartCoroutine(ISpin());
        }
        
        private IEnumerator ISpeadUp()
        {
            _isBuff = true;
            _player.particle.SetActive(true);
            var healingTime  = 0f;
            _player.moveSpeed *= 2;
            
            while (healingTime < buffDuration)
            {
                healingTime += Time.deltaTime;
                yield return null;
            }
            _player.moveSpeed /= 2;
            _isBuff = false;
            _player.particle.SetActive(false);
        }
        
        private IEnumerator ISpin()
        {
            _isSpin = true;
            var parent = transform.parent;
            
            transform.SetParent(_player.transform);
            animator.SetBool(SPIN, true);
            float wallTime  = 0f;

            while (wallTime < spinDuration)
            {
                wallTime += Time.deltaTime;
                yield return null;
            }
            animator.SetBool(SPIN, false);
            transform.SetParent(parent);
            _isSpin = false;
        }

        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && other.gameObject != _player.gameObject)
            {
                if (_isSpin) other.GetComponent<IDamageable>()?.Damaged(1f, transform.position);
                else
                {
                    other.GetComponent<IDamageable>()?.Damaged(1f, transform.position, false);
                    var slowDebuff = other.AddComponent<SlowDebuff>();
                    slowDebuff.Init(3f, 1f, _playerSpeed);
                }
            }
        }
    }
}