using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player's inventory, holding a list of items and the amount of gold the player has
/// </summary>
public class Inventory : MonoBehaviour
{
    public List<Item> inventory = new List<Item>();
    public int gold = 200;

    /// <summary>
    /// Buy an item if there's enough money
    /// </summary>
    /// <param name="item"></param>
    public void BuyItem(Item item)
    {
        if (item.cost <= gold)
        {
            inventory.Add(item);
            gold -= item.cost;
        }
        else
        {
            Debug.Log("not enough gold to buy " + item.name);
        }
    }

    /// <summary>
    /// Save the inventory when the game is closed
    /// </summary>
    private void OnApplicationQuit()
    {
        //Save the inventory
        DBManager.SaveInventory(inventory);
    }

}
