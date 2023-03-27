using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UploadToDB : MonoBehaviour
{
    //The local address of the server - URI (Uniform Resource Identifier)
    private string URI = "http://localhost:3000/";

    public Text leaderboardText;
    private IEnumerator SendRequest(string requestURI, Action<string> postProcessResult)
    {
        //Sends a Web Request - HTTP Get "/createPlayer"
        UnityWebRequest request = UnityWebRequest.Get(requestURI);

        //Handles the send Request
        var handler = request.SendWebRequest();

        //Tracks the responseTime
        float responseTime = 0.0f;

        //While awaiting a response
        while (!handler.isDone)
        {
            //Adds the time since the last frame
            responseTime += Time.deltaTime;

            //Cancels the request if the responseTime exceeds 10 seconds
            if (responseTime > 10.0f)
            {
                Debug.Log("Cancel request - ResponseTime exceeds 10 seconds");
                break;
            }

            //Wait for the next frame and continue execution from this line
            yield return null;
        }

        //If the Request succesfully received a Response
        if (request.result == UnityWebRequest.Result.Success)
        {
            //here we call our post-process function that we passed as a parameter
            postProcessResult(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error - No response received");
        }
    }

    //------------Methods for the various interactions we want with the DB----//

    //CREATE player
    public void CreatePlayer(string name, string password, int score)
    {
        //Sends a Web Request - HTTP Get "/createPlayer"
        string requestURL = $"{URI}createPlayer?name={name}&password={password}&score={score}";

        StartCoroutine(SendRequest(requestURL, PrintInLog));
    }

    //READ leaderboard
    public void GetHighScores()
    {
        //Sends a Web Request - HTTP Get "/leaderboard"
        string requestURL = $"{URI}leaderboard";

        leaderboardText.gameObject.SetActive(true);

        //Writes that we are waiting for the database
        leaderboardText.text = "Loading...";

        StartCoroutine(SendRequest(requestURL, PrintLeaderboard));
    }

    //-------------------Methods for doing post processing of the data-----//
    private void PrintInLog(string text)
    {
        Debug.Log("Received response: ");
        Debug.Log(text);
    }

    private void PrintLeaderboard(string text)
    {
        List<LeaderItem> items = JsonConvert.DeserializeObject<List<LeaderItem>>(text);
        string tx = "Leaderboard:\n";
        foreach (LeaderItem li in items)
        {
            tx += $"{li.name}: {li.score}\n";
        }
        leaderboardText.text = tx;
    }
}

public class LeaderItem
{
    public string name;
    public int score;
}
