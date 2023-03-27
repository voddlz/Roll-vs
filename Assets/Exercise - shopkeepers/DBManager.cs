using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that centralize all the interactions with the DB. 
/// I thought it was cleaner than having some of this code in Shopkeeper, some in Inventory, etc.
/// </summary>
public static class DBManager
{
    //the path for the db (maybe issues with non-windows system)
    private static string dbName = "URI=file:" + Application.streamingAssetsPath + @"\shop.db";

    /// <summary>
    /// Retrieves the items available at a specific shopkeeper and creates a list of Items
    /// </summary>
    /// <param name="shopkeeperName"></param>
    /// <returns></returns>
    public static List<Item> GetItems(string shopkeeperName)
    {
        List<Item> items = new List<Item>();

        using (SqliteConnection connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                //this time we're asking to select all thee rows in the players table
                string query = "SELECT items.name, items.cost " +
                    "FROM shopkeepers " +
                    "JOIN itemOwnership " +
                    "ON shopkeepers.id = itemOwnership.ownerID " +
                    "JOIN items " +
                    "ON items.id = itemOwnership.itemID " +
                    "WHERE shopkeepers.name = '{0}'";

                command.CommandText = string.Format(query, shopkeeperName);

                //instead of calling ExecuteNonQuery, this time we use ExecuteReader
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    //while reader still has lines 
                    while (reader.Read())
                    {
                        //we print the values returned from the db.
                        //we can access each field like it was a dictionary
                        Debug.Log("Name: " + reader["name"] + " Cost: " + reader["cost"]);
                        items.Add(new Item(reader["name"].ToString(), int.Parse(reader["cost"].ToString())));
                    }
                }
            }
        }
        return items;
    }

    /// <summary>
    /// Saves the list passed in input as the player's inventory in the db
    /// After I wrote the code, I realized that if I'd saved the item id in Item this would have been much simpler, you can try to fix it :) 
    /// </summary>
    /// <param name="inventory"></param>
    public static void SaveInventory(List<Item> inventory)
    {
        //creates a connection object
        using (SqliteConnection connection = new SqliteConnection(dbName))
        {
            //open the connection
            connection.Open();

            //create a command object
            using (SqliteCommand command = connection.CreateCommand())
            {
                //in case we have a player inventory in the db, we delete the table
                command.CommandText = "DROP TABLE IF EXISTS inventory";
                command.ExecuteNonQuery();

                //we recreate the table
                command.CommandText = "CREATE TABLE IF NOT EXISTS inventory (itemID INT)";
                command.ExecuteNonQuery();

                //for each item we get the id of the item and put it in the inventory table
                foreach (Item item in inventory)
                {
                    //get the item id
                    string itemID;
                    string query = "SELECT id FROM items WHERE name = '{0}'";
                    command.CommandText = string.Format(query, item.name);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        //while reader still has lines 
                        reader.Read();
                        itemID = reader["id"].ToString();
                    }
                    //save the item id into the inventory
                    query = "INSERT INTO inventory (itemID) VALUES ('{0}')";
                    command.CommandText = string.Format(query, itemID);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

}
