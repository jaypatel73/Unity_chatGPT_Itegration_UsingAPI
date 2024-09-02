using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Diagnostics;

public class ChatGPTConversationMultipleQuestions : MonoBehaviour
{
    private const string API_KEY = "";
    private const string API_URL = "https://api.openai.com/v1/chat/completions";
    private List<string> questions = new List<string>
    {
        "Hi, I am not feeling well today. Can you help me please?",
        "I have a headache and feel tired. What should I do?",
        "Is it okay to take ibuprofen for a headache?",
        "I also have a slight fever. Should I see a doctor?",
        "Can you suggest any home remedies for fever?",
        "What symptoms should I watch out for if it gets worse?",
        "How much water should I drink if I have a fever?",
        "Is rest important when feeling unwell?",
        "Should I avoid certain foods while I am sick?",
        "How can I boost my immune system to recover faster?"
    };
    private const int maxRetries = 5;
    private const float retryDelay = 2f; // Delay in seconds before retrying

    void Start()
    {
        StartCoroutine(AskQuestions(questions));
    }

    IEnumerator AskQuestions(List<string> prompts)
    {
        Stopwatch totalStopwatch = new Stopwatch();
        totalStopwatch.Start();

        List<object> conversation = new List<object>
        {
            new { role = "system", content = "You are a helpful assistant specialized in providing medical advice." }
        };

        for (int i = 0; i < prompts.Count; i++)
        {
            string prompt = prompts[i];
            conversation.Add(new { role = "user", content = prompt });

            yield return GetResponse(conversation, prompt, i + 1);
        }

        totalStopwatch.Stop();
        long totalElapsedMilliseconds = totalStopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"Total time taken for all questions: {totalElapsedMilliseconds} ms");
    }

    IEnumerator GetResponse(List<object> conversation, string prompt, int questionNumber, int retries = 0)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var jsonData = new
        {
            model = "gpt-3.5-turbo",
            messages = conversation
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
                UnityEngine.Debug.Log($"Question {questionNumber}: {prompt}\nAnswer: {answer}\nTime taken: {elapsedMilliseconds} ms");

                // Add the assistant's response to the conversation to maintain context
                conversation.Add(new { role = "assistant", content = answer });
            }
            else
            {
                UnityEngine.Debug.LogError("Error: " + request.error);
                UnityEngine.Debug.LogError("Response: " + request.downloadHandler.text);

                if (request.responseCode == 429 && retries < maxRetries)
                {
                    UnityEngine.Debug.LogError("Rate limit exceeded. Retrying...");
                    yield return new WaitForSeconds(retryDelay);
                    yield return GetResponse(conversation, prompt, questionNumber, retries + 1);
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
}
