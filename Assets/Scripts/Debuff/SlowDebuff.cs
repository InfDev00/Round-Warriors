using UnityEngine;

namespace Debuff
{
    public class SlowDebuff : Bases.Debuff
    {
        private float _slowSpeed;
        private float _originSpeed;

        public void Init(float duration, float slowSpeed, float originSpeed)
        {
            _originSpeed = originSpeed;
            _slowSpeed = slowSpeed;
            base.Init(duration);
        }
        
        public override void AttachDebuff()
        {
            _player.moveSpeed = _slowSpeed;
        }

        public override void DetachDebuff()
        {
            _player.moveSpeed = _originSpeed;
        }
    }
}