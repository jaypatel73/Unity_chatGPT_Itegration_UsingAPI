# Unity ChatGPT Integration Project

This Unity project integrates ChatGPT using OpenAI API keys. The project includes two scripts that demonstrate how to interact with ChatGPT: one script handles a single question, and the other handles multiple questions with timing functionalities. The purpose is to observe the response generation and measure the time taken by each script.

## Features

- **Single Question Script**: Sends a single question to ChatGPT and measures the response time.
- **Multiple Questions Script**: Sends multiple questions sequentially, maintaining context, and times each response.
- Both scripts are located in the `Assets` folder of the Unity project.

## Scripts Overview

### Single Question Script: `ChatGPTConversation.cs`

- Located in the `Assets` folder.
- Sends a single predefined question to ChatGPT.
- Measures the time taken for the response.
- Handles rate limits and retries up to 5 times if needed.

### Multiple Questions Script: `ChatGPTConversationMultipleQuestions.cs`

- Located in the `Assets` folder.
- Sends a list of questions to ChatGPT, one at a time.
- Maintains context throughout the conversation.
- Times each question and logs the total time taken for all responses.

## Installation

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/jaypatel73/Unity_chatGPT_Itegration_UsingAPI.git
   
2. **Open in Unity**:

   - Open the cloned project in Unity.

3. **Add API Key**:

   - Replace `API_KEY` in the scripts with your OpenAI API key.
