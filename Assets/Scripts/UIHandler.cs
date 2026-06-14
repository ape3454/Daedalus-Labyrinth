using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    private VisualElement m_HealthBox;
    private List<VisualElement> m_Hearts;
    [SerializeField]
    private StyleBackground fullHeart, emptyHeart;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_HealthBox = uiDocument.rootVisualElement.Q<VisualElement>("Health");
        m_Hearts = m_HealthBox.Children().ToList();
    }

    public void SetHealthValue(int health)
    {
        for (int i = 0; i < m_Hearts.Count; i++)
        {
            if (i < health)
            {
                m_Hearts[i].style.backgroundImage = fullHeart;
            }
            else
            {
                m_Hearts[i].style.backgroundImage = emptyHeart;
            }
        }
    }
}
