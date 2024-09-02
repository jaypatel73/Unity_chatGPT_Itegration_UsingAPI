[System.Serializable]
public class OpenAIResponse
{
    public Choice[] choices { get; set; }
}

[System.Serializable]
public class Choice
{
    public Message message { get; set; }
}

[System.Serializable]
public class Message
{
    public string content { get; set; }
}
