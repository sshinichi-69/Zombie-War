using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZombieWar.Monsters;

namespace ZombieWar
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager m_instance;

        private AudioSource m_audioSource;

        private int m_time = 0;
        private bool m_isPlaying = true;

        const float k_minRedVignetteRadiusSoldierTakeDamageEffect = 0.5f;
        const float k_maxRedVignetteRadiusSoldierTakeDamageEffect = -0.25f;
        const float k_whiteVignetteRadiusSoldierTakeDamageEffect = 1f;

        [SerializeField] private Text m_timeText;
        [SerializeField] private Text m_gameStateText;
        [SerializeField] private AudioClip m_winAudio;

        private bool m_isShowSoldierTakeDamageEffect = false;

        [SerializeField] private Material m_soldierTakeDamageEffectMaterial;

        void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();

            SetTimeText();

            if (m_instance == null)
            {
                m_instance = this;
            }
        }

        private void Start()
        {
            ShowSoldierTakeDamageEffect(k_whiteVignetteRadiusSoldierTakeDamageEffect);
            StartCoroutine(Timer());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                    activity.Call<bool>("moveTaskToBack", true);
                }
                else
                {
                    Application.Quit();
                }
            }
            HidingSoldierTakeDamageEffect();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void Win()
        {
            if (!m_isPlaying) return;
            m_isPlaying = false;
            StopAllCoroutines();
            m_gameStateText.text = "YOU WIN!!!";
            m_audioSource.clip = m_winAudio;
            m_audioSource.Play();
            StartCoroutine(LoadNextScene());
        }

        public void Loss()
        {
            if (!m_isPlaying) return;
            m_isPlaying = false;
            StopAllCoroutines();
            m_gameStateText.text = "YOU LOSE!!!";
            m_audioSource.Stop();
            StartCoroutine(Reload());
        }

        IEnumerator LoadNextScene()
        {
            yield return new WaitForSeconds(10);
            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCount)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        IEnumerator Reload()
        {
            yield return new WaitForSeconds(10);
            SceneManager.LoadScene(0);
        }

        public void ShowSoldierTakeDamageEffect(float intensity)
        {
            float vignetteRadius = Mathf.Lerp(
                k_minRedVignetteRadiusSoldierTakeDamageEffect,
                k_maxRedVignetteRadiusSoldierTakeDamageEffect,
                intensity
            );
            m_soldierTakeDamageEffectMaterial.SetFloat("_VignetteRadius", vignetteRadius);
            m_isShowSoldierTakeDamageEffect = true;
        }

        public void HidingSoldierTakeDamageEffect()
        {
            if (m_isShowSoldierTakeDamageEffect)
            {
                float vignetteRadius = m_soldierTakeDamageEffectMaterial.GetFloat("_VignetteRadius") + Time.deltaTime;
                m_soldierTakeDamageEffectMaterial.SetFloat("_VignetteRadius", vignetteRadius);
                if (vignetteRadius >= k_whiteVignetteRadiusSoldierTakeDamageEffect)
                {
                    m_isShowSoldierTakeDamageEffect = false;
                }
            }
        }

        IEnumerator Timer()
        {
            while (m_isPlaying)
            {
                yield return new WaitForSeconds(1);
                m_time++;
                SetTimeText();
                MonsterManager.Instance.GenerateEnemies(m_time);
            }
        }

        void SetTimeText()
        {
            m_timeText.text = IntToStringTimeNumber(m_time / 60) + ":" + IntToStringTimeNumber(m_time % 60);
        }

        string IntToStringTimeNumber(int number)
        {
            string result = number.ToString();
            return number < 10 ? "0" + result : result;
        }

        public static GameManager Instance {
            get {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<GameManager>();
                }
                return m_instance;
            }
        }
    }
}
