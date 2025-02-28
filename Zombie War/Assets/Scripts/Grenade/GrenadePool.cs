using UnityEngine;

namespace ZombieWar.Grenade
{
    public class GrenadePool : ObjectPoolTemplate<Grenade>
    {
        protected override Grenade CreateObject()
        {
            GameObject grenade = Instantiate(m_prefab.gameObject);
            grenade.transform.SetParent(transform);
            grenade.GetComponent<Grenade>().Pool = this;
            return grenade.GetComponent<Grenade>();
        }

        protected override void OnGetFromPool(Grenade grenade)
        {
            grenade.gameObject.SetActive(true);
            grenade.ResetState();
        }
    }
}
