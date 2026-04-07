using UnityEngine;

public class TeleportationZone : CharactersZone
{
    public ResourceInformation cost;

    public override void Initialize(ZoneData data)
    {
        base.Initialize(data);
    }

    public void TeleportCharacters()
    {
        Clear();

        for (int i = 0; i < G.instance.charactersLevelsSettings.teleportCharactersQuantity; i++)
        {
            CoroutineHolder.instance.AfterSeconds((i + 1) * 0.2f, () =>
            {
                var newCharacterModel = G.instance.charactersLevelsSettings.GetRandomHuman(GameData.GetCurrentDay());
                var newWeaponModel = G.instance.itemsLevelsSettings.GetRandomHumanWeapon(GameData.GetCurrentDay());
                newCharacterModel.itemSlot.item = newWeaponModel;
                G.instance.entityFactory.CreateCharacter(newCharacterModel, this);
            });
        }
    }

    public override bool TryTakeAwayCharacter(CharacterView view)
    {
        bool enoughCurrency = G.instance.resourceService.TrySpend(cost.resourceType, cost.quantity);

        if (!enoughCurrency) 
            return false;

        RemoveCharacter(view);
        return true;
    }
}