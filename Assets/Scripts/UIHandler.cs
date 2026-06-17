using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Center.Common.Analytics;
using Unity.VisualScripting;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }
    UIDocument uiDocument;

    private TemplateContainer m_HealthContainer;
    private VisualElement m_HealthBox;
    private List<VisualElement> m_Hearts;
    [SerializeField]
    private StyleBackground fullHeart, emptyHeart;

    private TemplateContainer m_InventoryBox;
    private VisualElement m_Inventory;
    private VisualElement m_CoinProgress;
    private TemplateContainer m_CoinBox;
    private List<VisualElement> m_CoinFragments;
    private VisualElement m_Sword;
    private VisualElement m_SwordImage;

    private TemplateContainer m_InteractionBox;
    private VisualElement m_Interaction;
    private Image m_InteractionImage;
    private Label m_InteractionLabel;
    [SerializeField]
    private Texture2D interactTalk, interactChange, interactInspect;
    bool interactionVisible;
    float fadeSpeed = 0.3f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
    }

    IEnumerator FadeOpacity(float duration, VisualElement element, bool fadein = true)
    {
        float startOpacity = element.style.opacity.value;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            element.style.opacity = Mathf.Lerp(startOpacity, (fadein) ? 1f : 0f, timeElapsed / duration);
            yield return null;
        }
        element.style.opacity = (fadein) ? 1f : 0f;
        element.visible = fadein;
    }

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();

        m_HealthContainer = uiDocument.rootVisualElement.Q<TemplateContainer>("HealthBox");
        m_HealthBox = m_HealthContainer.Q<VisualElement>("Health");
        m_Hearts = m_HealthBox.Children().ToList();

        m_InventoryBox = uiDocument.rootVisualElement.Q<TemplateContainer>("InventoryBox");
        m_Inventory = m_InventoryBox.Q<VisualElement>("Inventory");

        m_CoinProgress = m_Inventory.Q<VisualElement>("CoinProgress");
        m_CoinBox = m_CoinProgress.Q<TemplateContainer>("Coin");
        m_CoinFragments = m_CoinBox.Children().ToList();

        m_Sword = m_Inventory.Q<VisualElement>("Sword");
        m_SwordImage = m_Sword.Q<VisualElement>("SwordImage");

        m_InteractionBox = uiDocument.rootVisualElement.Q<TemplateContainer>("InteractionBox");
        m_Interaction = m_InteractionBox.Q<VisualElement>("Interaction");
        m_InteractionImage = m_Interaction.Q<Image>("InteractionImage");
        m_InteractionLabel = m_Interaction.Q<Label>("InteractionLabel");
    }

    public void UIReset()
    {
        m_Inventory.visible = false;
        m_CoinBox.visible = false;
        m_Sword.visible = false;
        m_SwordImage.visible = false;
        ElementSetVisible("coin_TopLeft", false);
        ElementSetVisible("coin_TopRight", false);
        ElementSetVisible("coin_BottomRight", false);
        ElementSetVisible("coin_BottomLeft", false);
        m_Interaction.visible = false;
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

    public void ElementSetVisible(string name, bool visible=true)
    {
        if (name.Substring(0, 4) == "coin")
        {
            m_CoinFragments.Find(y => y.name == name).visible = visible;
        }
        else
        {
            switch (name)
            {
                case "Inventory":
                    m_Inventory.visible = visible;
                    break;
                case "sword":
                    m_Sword.visible = visible;
                    m_SwordImage.visible = visible;
                    break;
                case "Interaction":
                    if (m_Interaction.visible != visible)
                    {
                        if (visible) m_Interaction.visible = visible;
                        Color elementColour = new Color(m_Interaction.style.color.value.r, m_Interaction.style.color.value.g, m_Interaction.style.color.value.b, (visible) ? 0f : 1f);
                        Color newColour = new Color(m_Interaction.style.color.value.r, m_Interaction.style.color.value.g, m_Interaction.style.color.value.b, (visible) ? 1f : 0f);
                        StartCoroutine(FadeOpacity(fadeSpeed, m_Interaction, visible));
                    }
                    break;
            }
        }
    }

    public void SetInteraction(string interaction, string label)
    {
        switch (interaction)
        {
            case "Talk":
                m_InteractionImage.image = interactTalk;
                m_InteractionLabel.text = label;
                break;
            case "Change":
                m_InteractionImage.image = interactChange;
                m_InteractionLabel.text = label;
                break;
            case "Inspect":
                m_InteractionImage.image = interactInspect;
                m_InteractionLabel.text = label;
                break;
        }
    }
}
