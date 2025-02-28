using UnityEngine;
using ZombieWar.UI;

namespace ZombieWar.Grenade
{
    public class GrenadeManager : MonoBehaviour
    {
        [SerializeField] private float cd = 10f;

        private float restCd = 0f;
        private Vector3 m_inputGrenadeDirection;

        [SerializeField] private GrenadePool m_grenadePool;
        [SerializeField] private ReleaseJoystick m_grenadeJoystick;

        void Update()
        {
            if (restCd > 0f)
            {
                restCd -= Time.deltaTime;
                if (restCd <= 0f)
                {
                    m_grenadeJoystick.SetToReadyState();
                }
            }
            else
            {
                // bomb
                Vector3 bombDirection = new Vector3(m_grenadeJoystick.Horizontal, 0, m_grenadeJoystick.Vertical);
                m_inputGrenadeDirection = bombDirection.normalized;
                if (bombDirection != Vector3.zero)
                {
                    restCd = cd;
                    m_grenadeJoystick.SetToCdState();
                    Throw();
                }
            }
        }

        public void Throw()
        {
            GameObject grenade = m_grenadePool.GenerateObject().gameObject;
            grenade.transform.position = transform.position;
            grenade.transform.LookAt(grenade.transform.position + m_inputGrenadeDirection);
        }
    }
}
