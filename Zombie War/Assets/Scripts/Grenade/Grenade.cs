using System.Collections;
using UnityEngine;
using ZombieWar.Monsters;

namespace ZombieWar.Grenade
{
    public class Grenade : MonoBehaviour
    {
        private float speed = 10f;
        private float maxRange = 24f;
        private float radius = 10f;
        private int damage = 4;

        private float distanceMove = 0f;
        private bool isRolling = true;

        private GrenadePool m_pool;

        [SerializeField] private LayerMask m_monsterLayerMask;
        [SerializeField] private GameObject m_mesh;
        [SerializeField] private ParticleSystem m_explosion;

        void FixedUpdate()
        {
            if (isRolling)
            {
                transform.Translate(speed * Time.fixedDeltaTime * Vector3.forward);
                distanceMove += speed * Time.fixedDeltaTime;
                if (distanceMove >= maxRange)
                {
                    Explode();
                }
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isRolling)
            {
                string collisionTag = collision.gameObject.tag;
                if (collisionTag == "Enemy" || collisionTag == "Barrier")
                {
                    Explode();
                }
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            if (isRolling)
            {
                string collisionTag = collision.gameObject.tag;
                if (collisionTag == "Enemy" || collisionTag == "Barrier")
                {
                    Explode();
                }
            }
        }

        public void ResetState()
        {
            isRolling = true;
            m_explosion.gameObject.SetActive(false);
            m_mesh.SetActive(true);
        }

        private void Explode()
        {
            if (isRolling)
            {
                isRolling = false;
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius, m_monsterLayerMask);
                foreach (Collider collider in colliders)
                {
                    Monster monster = collider.GetComponent<Monster>();
                    if (monster != null)
                    {
                        monster.TakeDamage(damage);
                    }
                }
                StartCoroutine(PlayExplosion());
            }
        }

        private IEnumerator PlayExplosion()
        {
            m_mesh.SetActive(false);
            m_explosion.gameObject.SetActive(true);
            m_explosion.Play();
            yield return new WaitForSeconds(2);
            m_pool.DestroyObject(this);
        }

        public GrenadePool Pool { get { return m_pool; } set { m_pool = value; } }
    }
}
