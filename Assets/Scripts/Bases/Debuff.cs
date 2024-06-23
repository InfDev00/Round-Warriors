using System;
using UnityEngine;

namespace Bases
{
    public abstract class Debuff : MonoBehaviour
    {
        protected PlayerController _player;
        public float _debuffDuration = 0.1f;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        protected void Init(float duration)
        {
            _debuffDuration = duration;
            AttachDebuff();
        }

        private void FixedUpdate()
        {
            _debuffDuration -= Time.fixedDeltaTime;
            
            if(_player.transform.position.x > 50) Destroy(this);
            if(_debuffDuration <= 0) Destroy(this);
        }

        private void OnDestroy()
        {
            DetachDebuff();
        }

        public abstract void AttachDebuff();

        public abstract void DetachDebuff();
    }
}