using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Diagnostics;

public class ChatGPTConversation : MonoBehaviour
{
    private const string API_KEY = "";
    private const string API_URL = "https://api.openai.com/v1/chat/completions";
    private string question = "Hi, I am not feeling well today. Can you help me please?";
    private const int maxRetries = 5;
    private const float retryDelay = 2f; // Delay in seconds before retrying

    void Start()
    {
        StartCoroutine(GetResponse(question));
    }

    IEnumerator GetResponse(string prompt, int retries = 0)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var jsonData = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant " +
                        "specialized in providing medical advice." },
                new { role = "user", content = prompt }
            }
        };

        string jsonString = JsonConvert.SerializeObject(jsonData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {API_KEY}");
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            stopwatch.Stop();
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            if (request.result == UnityWebRequest.Result.Success)
            {
                OpenAIResponse jsonResponse = JsonConvert.DeserializeObject<OpenAIResponse>(request.downloadHandler.text);
                string answer = jsonResponse.choices[0].message.content.Trim();
                UnityEngine.Debug.Log($"Question: {prompt}\nAnswer: {answer}\nTime taken: {elapsedMilliseconds} ms");
            }
            else
            {
                UnityEngine.Debug.LogError("Error: " + request.error);
                UnityEngine.Debug.LogError("Response: " + request.downloadHandler.text);

                if (request.responseCode == 429 && retries < maxRetries)
                {
                    UnityEngine.Debug.LogError("Rate limit exceeded. Retrying...");
                    yield return new WaitForSeconds(retryDelay);
                    yield return GetResponse(prompt, retries + 1);
                }
                else if (request.responseCode == 402)
                {
                    UnityEngine.Debug.LogError("Insufficient quota. Please check your plan and billing details.");
                }
                else
                {
                    UnityEngine.Debug.LogError("Error: " + request.error);
                }
            }
        }
    }

    [System.Serializable]
    public class OpenAIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
}
