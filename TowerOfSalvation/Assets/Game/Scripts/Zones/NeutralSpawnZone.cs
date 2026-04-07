using UnityEngine;

public class NeutralSpawnZone : CharactersZone
{
    public void TeleportCharacters()
    {
        for (int i = 0; i < G.instance.charactersLevelsSettings.teleportCharactersQuantity; i++)
        {
            var newCharacterModel = G.instance.charactersLevelsSettings.GetRandomHuman(GameData.GetCurrentDay());
            var newWeaponModel = G.instance.itemsLevelsSettings.GetRandomHumanWeapon(GameData.GetCurrentDay());
            newCharacterModel.itemSlot.item = newWeaponModel;
            G.instance.entityFactory.CreateCharacter(newCharacterModel, this);
        }
    }
}