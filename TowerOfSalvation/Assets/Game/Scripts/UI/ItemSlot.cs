using System;

[Serializable]
public class ItemSlot
{
    public ID id;
    public ItemModel item;

    public event Action<ItemSlot> OnSlotChanged;

    public ItemSlot(ItemModel item = null)
    {
        id = new ID();

        this.item = item;
    }
}