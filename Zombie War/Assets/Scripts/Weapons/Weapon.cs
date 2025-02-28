using UnityEngine;
using ZombieWar.Monsters;
using ZombieWar.Particle;

namespace ZombieWar.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] float m_range;
        [SerializeField] int m_damage;
        [SerializeField] float m_shootSpeed;
        [SerializeField] float m_explodeRange;
        LayerMask m_hitLayerMask;

        [SerializeField] Color m_weaponIconColor;
        [SerializeField] ParticlePool m_weaponParticlePool;

        private float m_shootCd = 0;

        private void Awake()
        {
            m_hitLayerMask = transform.parent.GetComponent<WeaponManager>().HitLayerMask;
        }

        private void FixedUpdate()
        {
            m_shootCd -= Time.fixedDeltaTime;
            if (m_shootCd < 0)
            {
                m_shootCd = 0;
            }
        }

        public bool Shoot(Vector3 soldierPosition, Vector3 shootDirection)
        {
            if (m_shootCd <= 0) {
                m_shootCd = 1 / m_shootSpeed;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, shootDirection, out hit, m_range, m_hitLayerMask))
                {
                    InitParticle(hit.point);
                    if (m_explodeRange > 0)
                    {
                        Collider[] hitMonsters = Physics.OverlapSphere(hit.point, m_explodeRange, m_hitLayerMask);
                        foreach (Collider collider in hitMonsters)
                        {
                            DealDamage(collider.gameObject);
                        }
                    }
                    else
                    {
                        DealDamage(hit.collider.gameObject);
                    }
                }
                else
                {
                    if (m_explodeRange > 0)
                    {
                        Vector3 explodePos = transform.position + shootDirection.normalized * m_range;

                        InitParticle(explodePos);
                        Collider[] hitMonsters = Physics.OverlapSphere(explodePos, m_explodeRange, m_hitLayerMask);
                        foreach (Collider collider in hitMonsters)
                        {
                            DealDamage(collider.gameObject);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private void DealDamage(GameObject gameObject)
        {
            Monster monster = gameObject.GetComponent<Monster>();
            if (monster != null) {
                monster.TakeDamage(m_damage);
            }
        }

        private void InitParticle(Vector3 pos)
        {
            Particle.Particle particle = m_weaponParticlePool.GenerateObject();
            particle.transform.position = pos;
            particle.transform.rotation = transform.rotation;
            particle.transform.localScale = Vector3.one * (m_explodeRange > 0.1f ? m_explodeRange : 0.1f);
        }

        public Color WeaponIconColor { get { return m_weaponIconColor; } }
    }
}
