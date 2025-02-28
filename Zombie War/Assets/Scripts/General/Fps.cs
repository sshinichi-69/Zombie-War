using UnityEngine;
using UnityEngine.UI;

public class Fps : MonoBehaviour
{
    private Text m_fpsText;
    private float m_updateTime = 0;

    private void Start()
    {
        m_fpsText = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time >= m_updateTime)
        {
            int fps = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            m_fpsText.text = fps.ToString();
            m_updateTime += 0.5f;
        }
    }
}
