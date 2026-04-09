using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour, IPointerExitHandler
{
    public Transform effectsContainer;

    public CharacterPresenter character;

    public event Action Exit;

    [SerializeField] private Button _levelUpButton;

    public void Initialize(CharacterPresenter character)
    {
        this.character = character;
        character.ShowInPanel();

        foreach (var effect in character.model.effectsData)
        {
            G.instance.uiFactory.CreateEffectIcon(effect, effectsContainer);
        }

        if (character.model.level.availableUpgrades != 0)
        {
            _levelUpButton.gameObject.SetActive(true);
        }
        else
        {
            _levelUpButton.gameObject.SetActive(false);

        }

        //_levelUpButton.clicked += UpgradeLevel;
    }

    private void Update()
    {
        if (character != null)
            transform.position = character.view.transform.position;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Exit?.Invoke();
        Destroy(gameObject);
    }

    public void HideTooltip()
    {
        Debug.Log("hide");
        OnPointerExit(null);
    }

    public void UpgradeLevel()
    {
        character.LevelUp();
    }

    private void OnDestroy()
    {
        if (character != null)
            character.HideFromPanel();
        //_levelUpButton.clicked -= UpgradeLevel;
    }
}