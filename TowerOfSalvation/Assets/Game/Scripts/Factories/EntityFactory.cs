using UnityEngine;

public class EntityFactory : Singleton<EntityFactory>
{
    public CharacterView CreateCharacter(CharacterModel model, CharactersZone zone, Vector3 position = default)
    {
        CharacterView view = Instantiate(model.prefab).GetComponent<CharacterView>();
        view.Initialize(model);

        zone.SpawnCharacter(view, position);

        return view;
    }

    public ItemView CreateWeapon(WeaponModel model, CharacterView character)
    {
        ItemView view = Instantiate(model.prefab, character.rightHand).GetComponentInChildren<ItemView>();
        view.Initialize(model, character);

        return view;
    }

    public CharactersZone CreateZone(ZoneData data, Vector3 position)
    {
        CharactersZone zone = Instantiate(data.prefab, position, Quaternion.identity).GetComponent<CharactersZone>();
        zone.Initialize(data);

        return zone;
    }

    public Projectile CreateProjectile(ItemView owner, ProjectileModel model, Vector3 position, Vector3 direction, Vector3 offset)
    {
        Quaternion angle = Quaternion.identity; // should be gotten from direction
        Projectile projectile = Instantiate(model.prefab, position, angle).GetComponent<Projectile>();
        projectile.Initilize(model, direction, offset, owner);

        return projectile;
    }

}