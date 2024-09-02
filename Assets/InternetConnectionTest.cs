using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class InternetConnectionTest : MonoBehaviour
{
    private const string TEST_URL = "https://jsonplaceholder.typicode.com/posts/1";

    void Start()
    {
        StartCoroutine(CheckInternetConnection(TEST_URL));
    }

    IEnumerator CheckInternetConnection(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Connection successful. Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}
