using UnityEngine;

namespace ZombieWar.Cameras
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] private GameObject m_soldier;
        [SerializeField] private Transform m_fieldTransform;

        private float m_fieldTop, m_fieldBottom, m_fieldLeft, m_fieldRight = 0;

        private void Awake()
        {
            Camera cam = Camera.main;
            Vector3 padding = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 21f));
            float horizontalPadding = padding.x - 3f;
            float verticalPadding = padding.z - 3f;

            m_fieldTop = m_fieldTransform.localScale.z / 2 - verticalPadding;
            m_fieldBottom = -m_fieldTransform.localScale.z / 2 + verticalPadding;
            m_fieldLeft = -m_fieldTransform.localScale.x / 2 + horizontalPadding;
            m_fieldRight = m_fieldTransform.localScale.x / 2 - horizontalPadding;
        }

        void LateUpdate()
        {
            if (m_soldier != null)
            {
                float xPos = m_soldier.transform.position.x;
                float zPos = m_soldier.transform.position.z;
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
                transform.position = new Vector3(xPos, m_soldier.transform.position.y, zPos);
            }
        }
    }
}
