using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ZombieWar.Monsters
{
    public class MonsterManager : MonoBehaviour
    {
        public int N_MAX_ENEMIES = 500;
        [SerializeField] public int N_MAX_BOSSES = 0;

        private static MonsterManager m_instance;

        private int m_nEnemyNotGenerateLeft;
        private int m_nBossNotGenerateLeft;

        int m_levelIdx;

        [SerializeField] GameObject m_bossPrefab;
        [SerializeField] MonsterPool m_monsterPool;

        [SerializeField] private Text m_enemyAliveLeftText;

        private int fieldTop, fieldBottom, fieldLeft, fieldRight;
        private const float fieldPadding = 10f;

        [SerializeField] Transform fieldTransform;
        private void Awake()
        {
            m_levelIdx = SceneManager.GetActiveScene().buildIndex;
            m_nEnemyNotGenerateLeft = N_MAX_ENEMIES;
            m_nBossNotGenerateLeft = N_MAX_BOSSES;

            fieldTop = Mathf.RoundToInt(fieldTransform.localScale.z / 2);
            fieldBottom = -fieldTop;
            fieldRight = Mathf.RoundToInt(fieldTransform.localScale.x / 2);
            fieldLeft = -fieldRight;

            if (m_instance == null)
            {
                m_instance = FindObjectOfType<MonsterManager>();
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void OnMonsterDie(Monster monster)
        {
            SetEnemyLeftText();
            if (transform.childCount == 0 && GetTotalEnemyLeft() <= 0)
            {
                GameManager.Instance.Win();
            }
        }

        public void SetEnemyLeftText()
        {
            m_enemyAliveLeftText.text = (transform.childCount + GetTotalEnemyLeft()).ToString();
        }

        public void GenerateEnemies(int time)
        {
            if (m_nEnemyNotGenerateLeft > 0)
            {
                int nMonster = (int)Mathf.Log(time, 1.614f);
                switch (m_levelIdx)
                {
                    case 0:
                        nMonster = nMonster <= m_nEnemyNotGenerateLeft ? nMonster : m_nEnemyNotGenerateLeft;
                        StartCoroutine(GenerateMonsters(nMonster));
                        break;
                    case 1:
                        if (nMonster < m_nEnemyNotGenerateLeft)
                        {
                            StartCoroutine(GenerateMonsters(nMonster));
                        }
                        else
                        {
                            StartCoroutine(GenerateMonsters(m_nEnemyNotGenerateLeft));
                            GenerateBoss();
                        }
                        break;
                }
            }
        }

        public Vector3 CalcMonsterGeneratedPos()
        {
            System.Random rnd = new System.Random();
            int side = rnd.Next(0, 4);
            switch (side)
            {
                case 0:
                    return new Vector3(rnd.Next(fieldLeft, fieldRight), 0, fieldTop + fieldPadding);
                case 1:
                    return new Vector3(rnd.Next(fieldLeft, fieldRight), 0, fieldBottom - fieldPadding);
                case 2:
                    return new Vector3(fieldLeft - fieldPadding, 0, rnd.Next(fieldBottom, fieldTop));
                case 3:
                    return new Vector3(fieldRight + fieldPadding, 0, rnd.Next(fieldBottom, fieldTop));
            }
            return Vector3.zero;
        }

        IEnumerator GenerateMonsters(int nMonster)
        {
            while (nMonster > 0)
            {
                GenerateMonster();
                nMonster--;
                yield return new WaitForSeconds(0.05f);
            }
        }

        void GenerateMonster()
        {
            GameObject monster = m_monsterPool.GenerateObject().gameObject;
            monster.transform.SetParent(transform);
            m_nEnemyNotGenerateLeft--;
        }

        void GenerateBoss()
        {
            Vector3 pos = CalcMonsterGeneratedPos();
            GameObject boss = Instantiate(m_bossPrefab, pos, Quaternion.identity);
            boss.transform.parent = transform;
            m_nBossNotGenerateLeft--;
        }

        int GetTotalEnemyLeft()
        {
            return m_nEnemyNotGenerateLeft + m_nBossNotGenerateLeft;
        }

        public static MonsterManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<MonsterManager>();
                }
                return m_instance;
            }
        }
    }
}
