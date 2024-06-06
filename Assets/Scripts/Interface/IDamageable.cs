using UnityEngine;

namespace Interface
{
    public interface IDamageable
    {
        public void Damaged(float damage, Vector3 position, bool isKnockBack=true);
    }
}