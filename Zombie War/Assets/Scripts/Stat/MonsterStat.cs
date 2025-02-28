using UnityEngine;

namespace ZombieWar.Stat
{
    [CreateAssetMenu(fileName = "New Zombie", menuName = "Stats/Zombie")]
    public class MonsterStat : ScriptableObject
    {
        public float speed = 1.5f;
        public int hp = 2;
        public int damage = 10;
        public readonly float gravityVelocity = 2f;

        public readonly float attackSpeed = 1;
        public readonly float gravity = 9.81f * 2;
    }
}
