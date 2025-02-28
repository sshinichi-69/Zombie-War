using UnityEngine;

namespace ZombieWar.Particle
{
    public class ParticlePool : ObjectPoolTemplate<Particle>
    {
        protected override Particle CreateObject()
        {
            GameObject particle = Instantiate(m_prefab.gameObject);
            particle.transform.SetParent(transform);
            particle.GetComponent<Particle>().Pool = this;
            return particle.GetComponent<Particle>();
        }

        protected override void OnGetFromPool(Particle particle)
        {
            particle.gameObject.SetActive(true);
            particle.ResetState();
        }
    }
}
