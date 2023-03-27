using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shopkeeper : MonoBehaviour
{
    // a reference to the canvas, so you can manipulate it
    public GameObject shopCanvas;

    // the name of the shopkeeper
    public string shopkeeperName = "Aaron";

    // the list of items the shopkeeper has in stock (which you should retrieve/reconstruct from the db 
    public List<Item> items;

    // a reference to the player's inventory
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

   

    /// <summary>
    /// Method called when the shop is activated by the player, here you want to 
    /// 1 activate the shop canvas
    /// 2 get the items from the db
    /// 3 display the info on the shop canvas
    /// </summary>
    public void ActivateShop()
    {
        // activate the shop canvas
        shopCanvas.SetActive(true);
        // ask the DBManager to get us the items based on the shopkeeper's name
        items = DBManager.GetItems(shopkeeperName);

        // update the UI
        for (int i = 0; i<items.Count; i++)
        {
            // find the gameobject that is the parent of the UI elements (text + button)
            GameObject itemUI = shopCanvas.transform.GetChild(i).gameObject;
            // find the text and update it 
            TMP_Text text = itemUI.transform.GetChild(0).GetComponent<TMP_Text>();
            text.text = items[i].name + ", " + items[i].cost;
            // find the button, and then this becomes tricky: we add a new event to OnClick. Basically we add a function to be called when the button is pressed
            Button button = itemUI.transform.GetChild(1).GetComponent<Button>();
            //Note that when adding delegates we need to "capture" the value by saving a reference in a variable
            //using delegate { SellItem(items[i]); will fail 
            Item currentItem = items[i];
            button.onClick.AddListener( delegate { inventory.BuyItem(currentItem); });
            // I also changed the text to "buy" to make it more obvious that it's an active button (you could also use the enable/disable property of buttons)
            TMP_Text buttonText = itemUI.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
            buttonText.text = "Buy";
        }
    }

    /// <summary>
    /// Deactivating the shop is easier, we can just deactivate the shop canvas.
    /// </summary>
    public void DeactivateShop()
    {
        shopCanvas.SetActive(false);
    }

}
