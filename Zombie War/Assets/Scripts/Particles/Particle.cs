using System.Collections;
using UnityEngine;

namespace ZombieWar.Particle
{
    public class Particle : MonoBehaviour
    {
        [SerializeField] private float m_time;
        private ParticlePool m_pool;

        public void ResetState()
        {
            GetComponent<ParticleSystem>().Play();
        }

        private void OnEnable()
        {
            StartCoroutine(DestroyCountDown());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator DestroyCountDown()
        {
            yield return new WaitForSeconds(m_time);
            m_pool.DestroyObject(this);
        }

        public ParticlePool Pool { get { return m_pool; } set { m_pool = value; } }
    }
}
