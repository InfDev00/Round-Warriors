using System;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bases
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected AudioManager audioManager;
        protected PlayerController _player;
        protected const string ATTACK = "Attack";
        protected const string HEALING = "Healing";
        protected const string SPIN = "Spin";

        public float[] skillCoolTimes;
        private readonly float[] _skillCools = new float[3];

        #region EventHandler

        public Action<int, float> UseSkill = null;

        #endregion

        private void Awake()
        {
            for (int i = 0; i < 3; ++i) _skillCools[i] = 100;
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < 3; ++i) _skillCools[i] += Time.fixedDeltaTime;
        }

        public void ExecuteSkill(int idx)
        {
            if (_skillCools[idx] < skillCoolTimes[idx]) return;
            
            UseSkill?.Invoke(idx, skillCoolTimes[idx]);
            switch (idx)
            {
                case 0:
                    Skill0();
                    break;
                case 1:
                    Skill1();
                    break;
                case 2:
                    Skill2();
                    break;
            }

            _skillCools[idx] = 0;
        }

        protected abstract void Skill0();
        protected abstract void Skill1();
        protected abstract void Skill2();

        public void SetPlayer(PlayerController player) => _player = player;
    }
}