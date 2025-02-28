using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using ZombieWar.Player;
using ZombieWar.Stat;

namespace ZombieWar.Monsters
{
    public class Monster : MonoBehaviour
    {
        private float m_speed = 1.5f;
        private int m_hp = 2;
        private int m_damage = 10;
        private float m_gravityVelocity = 2f;
        private float m_attackSpeed = 1;
        private float m_attackTime = 0f;
        private bool m_isDead = false;

        private MonsterPool m_pool;

        Transform soldierTransform;
        private Material m_material;
        private NavMeshAgent m_agent;
        private CharacterController m_controller;
        AudioSource m_audioSource;

        [SerializeField] private VisualEffect m_takeDamageEffect;
        [SerializeField] private GameObject m_mesh;
        [SerializeField] MonsterStat m_data;
        private void Awake()
        {
            m_speed = m_data.speed;
            m_hp = m_data.hp;
            m_damage = m_data.damage;
            m_gravityVelocity = m_data.gravityVelocity;
            m_attackSpeed = m_data.attackSpeed;

            var soldier = FindObjectOfType<Soldier>();
            if (soldier != null)
            {
                soldierTransform = FindObjectOfType<Soldier>().transform;
            }

            m_takeDamageEffect.Stop();

            m_agent = gameObject.GetComponent<NavMeshAgent>();
            m_agent.speed = 1;

            m_material = m_mesh.GetComponent<Renderer>().material;
            m_controller = GetComponent<CharacterController>();
            m_audioSource = GetComponent<AudioSource>();

            m_attackTime = m_attackSpeed;
        }

        private void Start()
        {
            StartCoroutine(Move());
        }

        void Update()
        {
            if (m_isDead)
            {
                float fade = m_material.GetFloat("_Fade");
                fade -= Time.deltaTime;
                if (fade <= 0f)
                {
                    if (m_pool == null)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        m_pool.DestroyObject(this);
                    }
                }
                else
                {
                    m_material.SetFloat("_Fade", fade);
                }
            }
            else
            {
                if (m_attackTime < m_attackSpeed)
                {
                    m_attackTime += Time.deltaTime;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!m_isDead) {
                Fall();
            }
        }

        void Fall()
        {
            m_gravityVelocity += m_data.gravity * Time.fixedDeltaTime;
            if (m_controller.isGrounded)
            {
                m_gravityVelocity = 2f;
            }
            else
            {
                m_controller.Move(m_gravityVelocity * Time.fixedDeltaTime * Vector3.down);
            }
        }

        IEnumerator Move()
        {
            m_agent.SetDestination(soldierTransform.position);
            int maxFps = 30;
            int minDelayTime = 2;
            int fps = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            float delayTime = fps >= maxFps - minDelayTime ? minDelayTime : maxFps - fps;
            float distance = (soldierTransform.position - transform.position).magnitude;
            if (distance < 50f)
            {
                delayTime /= 4f;
            }
            yield return new WaitForSeconds(delayTime);
            if (!m_isDead && soldierTransform != null)
            {
                StartCoroutine(Move());
            }
        }

        public void OnCollisionStay(Collision collision)
        {
            if (collision.collider.tag == "Player")
            {
                if (m_attackTime >= m_attackSpeed)
                {
                    AttackSoldier(collision.gameObject.GetComponent<Soldier>());
                    m_attackTime = 0f;
                }
            }
        }

        public void AttackSoldier(Soldier soldier)
        {
            if (soldier != null)
            {
                soldier.TakeDamage(m_damage);
            }
        }

        public void TakeDamage(int damage)
        {
            m_hp -= damage;
            if (m_hp <= 0)
            {
                Die();
            }
            else
            {
                m_takeDamageEffect.Play();
                StartCoroutine(StopTakeDamageEffect());
            }
        }

        public void ResetState()
        {
            m_speed = m_data.speed;
            m_hp = m_data.hp;
            m_damage = m_data.damage;
            m_gravityVelocity = m_data.gravityVelocity;
            m_attackSpeed = m_data.attackSpeed;

            m_isDead = false;

            m_takeDamageEffect.Stop();
            m_material.SetFloat("_Fade", 1f);

            m_attackTime = m_attackSpeed;
            m_agent.enabled = true;
            StartCoroutine(Move());
        }

        public void Disable(Vector3 startPos)
        {
            m_agent.Warp(startPos);
            m_agent.enabled = false;
            StopAllCoroutines();
        }

        private void Die()
        {
            m_audioSource.Play();
            transform.SetParent(m_pool == null ? null : m_pool.transform);
            MonsterManager.Instance.OnMonsterDie(this);
            m_isDead = true;
            m_controller.enabled = false;
        }

        private IEnumerator StopTakeDamageEffect()
        {
            yield return new WaitForSeconds(0.5f);
            m_takeDamageEffect.Stop();
        }

        public bool IsDead { get { return m_isDead; } }
        public MonsterPool Pool { get { return m_pool; } set { m_pool = value; } }
    }
}
