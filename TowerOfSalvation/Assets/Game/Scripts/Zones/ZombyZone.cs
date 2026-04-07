public class ZombyZone : CharactersZone
{
    public int zombyQuantity = 1;
    private void Awake()
    {
        Initialize(null);
    }

    private void Start()
    {
        SpawnCharacters();
    }

    public void SpawnCharacters()
    {
        for (int i = 0; i < zombyQuantity; i++)
        {
            var newCharacterModel = G.instance.charactersLevelsSettings.GetRandomZomby(GameData.GetCurrentDay());
            var newWeaponModel = G.instance.itemsLevelsSettings.GetRandomZombyWeapon(GameData.GetCurrentDay());
            newCharacterModel.itemSlot.item = newWeaponModel;
            G.instance.entityFactory.CreateCharacter(newCharacterModel, this);
        }
    }
}