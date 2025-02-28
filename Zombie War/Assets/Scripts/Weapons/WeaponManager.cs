using UnityEngine;
using UnityEngine.UI;

namespace ZombieWar.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        private int m_weaponIndex = 0;

        [SerializeField] private LayerMask m_hitLayerMask;

        [SerializeField] private GameObject m_weaponSwitchButton;

        public void SwitchWeapon()
        {
            m_weaponIndex++;
            if (m_weaponIndex >= transform.childCount)
            {
                m_weaponIndex = 0;
            }
            m_weaponSwitchButton.GetComponent<Image>().color = transform.GetChild(m_weaponIndex).GetComponent<Weapon>().WeaponIconColor;
        }

        public bool Shoot(Vector3 soldierPosition, Vector3 shootDirection)
        {
            return transform.GetChild(m_weaponIndex).GetComponent<Weapon>().Shoot(soldierPosition, shootDirection);
        }

        public LayerMask HitLayerMask { get { return m_hitLayerMask; } }
    }
}
