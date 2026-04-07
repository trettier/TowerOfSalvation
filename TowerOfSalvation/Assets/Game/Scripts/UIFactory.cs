using UnityEngine;

public class UIFactory : MonoBehaviour
{
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private GameObject expirienceBarPrefab;
    [SerializeField] private Transform worldCanvas;

    [SerializeField] private GameObject characterPanel;
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private GameObject effectPanel;
    [SerializeField] private GameObject effectIconPrefab;

    public void CreateHealthBar(Transform character, HealthPoints healthPoints)
    {
        var bar = Instantiate(healthBarPrefab, character.position, Quaternion.identity, worldCanvas).GetComponent<HealthBarRender>();

        bar.Initialize(character, healthPoints);        
    }

    public void CreateExpirienceBar(Transform character, HealthPoints healthPoints, Level level)
    {
        var bar = Instantiate(expirienceBarPrefab, character.position, Quaternion.identity, worldCanvas).GetComponent<ExpirienceBarRender>();

        bar.Initialize(character, level, healthPoints);        
    }

    public CharacterPanel CreateCharacterPanel(CharacterPresenter character)
    {
        CharacterPanel panel = Instantiate(characterPanel, character.view.transform.position, Quaternion.identity, worldCanvas).GetComponent<CharacterPanel>();



        panel.Initialize(character);

        return panel;
    }

    public void CreateItemPanel(ItemPresenter item)
    {
        ItemPanel panel = Instantiate(characterPanel, item.view.transform.position, Quaternion.identity, worldCanvas).GetComponent<ItemPanel>();

        panel.Initialize(item);

    }

    public void CreateEffectIcon(Effect effect, Transform container)
    {
        EffectIcon icon = Instantiate(effectIconPrefab, Vector3.zero, Quaternion.identity, container).GetComponent<EffectIcon>();
    }

    public void CreateEffectPanel(Effect effect, Vector3 postion)
    {
        EffectPanel icon = Instantiate(effectPanel, postion, Quaternion.identity, worldCanvas).GetComponent<EffectPanel>();

    }

    public void CreateCharacterLevelUpChoice(CharacterPresenter character, CharacterEffect effect, CharacterEffect effect2, CharacterEffect effect3)
    {

    }
}

public class LevelUpManager
{
    private CharacterPresenter character;

    private EffectPanel effect1;
    private EffectPanel effect2;
    private EffectPanel effect3;

    public void StartLevelUp(CharacterPresenter character, EffectPanel e1, EffectPanel e2, EffectPanel e3)
    {
        this.character = character;

        effect1 = e1;
        effect2 = e2;
        effect3 = e3;

        effect1.button.clicked += () => OnChoice(effect1);
        effect2.button.clicked += () => OnChoice(effect2);
        effect3.button.clicked += () => OnChoice(effect3);
    }

    private void OnChoice(EffectPanel effect)
    {
        character.ApplyEffect((CharacterEffect)effect.effect);

        effect1.button.clicked -= () => OnChoice(effect1);
        effect2.button.clicked -= () => OnChoice(effect2);
        effect3.button.clicked -= () => OnChoice(effect3);

        GameObject.Destroy(effect1.gameObject);
        GameObject.Destroy(effect2.gameObject);
        GameObject.Destroy(effect3.gameObject);
    }

}
