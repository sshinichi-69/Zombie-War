using UnityEngine;
using UnityEngine.UI;
using ZombieWar.UI;
using ZombieWar.Weapons;

namespace ZombieWar.Player
{
    public class Soldier : MonoBehaviour
    {
        CharacterController m_controller;
        AudioSource m_audioSource;

        private int m_speed = 2;
        private int m_hp;
        private float m_gravityVelocity;

        const int k_maxHp = 100;
        const float k_gravity = 9.81f * 2;

        [SerializeField] private WeaponManager m_weaponManager;

        const float k_touchMoveSensitive = 0.2f;
        private float m_fieldTop, m_fieldBottom, m_fieldLeft, m_fieldRight = 0;

        [SerializeField] private Transform m_meshTransform;
        [SerializeField] private Joystick m_moveJoystick;
        [SerializeField] private HoldJoystick m_shootJoystick;
        [SerializeField] private Animator m_animator;
        [SerializeField] Slider m_hpBarUi;
        [SerializeField] private ParticleSystem m_gunParticle;
        [SerializeField] private Transform m_fieldTransform;

        private void Awake()
        {
            m_controller = GetComponent<CharacterController>();
            m_audioSource = GetComponent<AudioSource>();

            m_hp = k_maxHp;

            m_hpBarUi.maxValue = k_maxHp;
            m_hpBarUi.value = m_hpBarUi.maxValue;

            m_fieldTop = m_fieldTransform.localScale.z / 2;
            m_fieldBottom = -m_fieldTransform.localScale.z / 2;
            m_fieldLeft = -m_fieldTransform.localScale.x / 2;
            m_fieldRight = m_fieldTransform.localScale.x / 2;
        }

        // Update is called once per frame
        void Update()
        {
            Move();
            Shoot();
        }

        private void FixedUpdate()
        {
            m_gravityVelocity += k_gravity * Time.fixedDeltaTime;
            if (m_controller.isGrounded)
            {
                m_gravityVelocity = 2f;
            }
            else
            {
                m_controller.Move(m_gravityVelocity * Time.fixedDeltaTime * Vector3.down);
            }
        }

        public void TakeDamage(int damage)
        {
            m_hp -= damage;
            SetHpBarUi();
            if (m_hp <= 0)
            {
                GameManager.Instance.Loss();
                Destroy(gameObject);
            }
            else
            {
                GameManager.Instance.ShowSoldierTakeDamageEffect((float)damage / k_maxHp);
            }
        }

        private void Move()
        {
            Vector3 moveDirection = new Vector3(m_moveJoystick.Horizontal, 0, m_moveJoystick.Vertical);
            if (Mathf.Abs(m_moveJoystick.Horizontal) >= k_touchMoveSensitive || Mathf.Abs(m_moveJoystick.Vertical) >= k_touchMoveSensitive)
            {
                Vector3 inputMovement = moveDirection.normalized;
                float inputEulerRotation = Vector3.Angle(Vector3.forward, moveDirection);
                inputEulerRotation = moveDirection.x > 0 ? inputEulerRotation : -inputEulerRotation;
                MoveController(inputMovement, inputEulerRotation);
                m_animator.SetFloat("Speed", m_speed);
            }
            else
            {
                m_animator.SetFloat("Speed", 0);
            }
        }

        private void MoveController(Vector3 movement, float eulerRotation)
        {
            if (movement != Vector3.zero)
            {
                Vector3 newPos = transform.position + m_speed * Time.deltaTime * movement;
                float xPos = newPos.x;
                float zPos = newPos.z;
                if (xPos < m_fieldLeft)
                {
                    xPos = m_fieldLeft;
                }
                else if (xPos > m_fieldRight)
                {
                    xPos = m_fieldRight;
                }
                if (zPos < m_fieldBottom)
                {
                    zPos = m_fieldBottom;
                }
                else if (zPos > m_fieldTop)
                {
                    zPos = m_fieldTop;
                }
                Vector3 correctPos = new Vector3(xPos, transform.position.y, zPos);
                m_controller.Move(correctPos - transform.position);

                m_meshTransform.LookAt(transform.position + movement);
            }
        }

        private void Shoot()
        {
            Vector3 shootDirection = new Vector3(m_shootJoystick.Horizontal, 0, m_shootJoystick.Vertical);
            Vector3 inputShootDirection = shootDirection.normalized;
            if (inputShootDirection != Vector3.zero)
            {
                m_meshTransform.LookAt(transform.position + inputShootDirection);
                if (m_weaponManager.Shoot(transform.position, inputShootDirection))
                {
                    m_animator.SetTrigger("Shoot");
                    m_audioSource.Play();
                    m_gunParticle.Play();
                }
            }
        }

        private void SetHpBarUi()
        {
            m_hpBarUi.value = m_hp > 0 ? m_hp : 0;
        }
    }
}
