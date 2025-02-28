using UnityEngine;

namespace ZombieWar.Monsters
{
    public class MonsterPool : ObjectPoolTemplate<Monster>
    {
        protected override Monster CreateObject()
        {
            Vector3 monsterPosition = MonsterManager.Instance.CalcMonsterGeneratedPos();
            GameObject monster = Instantiate(m_prefab.gameObject, monsterPosition, Quaternion.identity);
            monster.transform.SetParent(transform);
            monster.GetComponent<Monster>().Pool = this;
            return monster.GetComponent<Monster>();
        }

        protected override void OnGetFromPool(Monster monster)
        {
            monster.gameObject.SetActive(true);
            monster.ResetState();
        }

        protected override void OnReleaseToPool(Monster monster)
        {
            monster.Disable(MonsterManager.Instance.CalcMonsterGeneratedPos());
            monster.transform.SetParent(transform);
            monster.gameObject.SetActive(false);
        }
    }
}
