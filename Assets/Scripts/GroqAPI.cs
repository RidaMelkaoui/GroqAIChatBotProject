using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
public class GroqAPI : MonoBehaviour
{
    private string apiKey = "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"; // Replace with your Groq API key
    private string url = "https://api.groq.com/openai/v1/chat/completions";
    [SerializeField] private TMP_InputField userInputField;
    [SerializeField] private TMP_Text userText;
    [SerializeField] private TMP_Text responseText;
    [SerializeField] private int maxResponseLength = 50;
    [SerializeField] private int maxResponseForGroq =50;


    private void Awake()
    {
        Show();
    }


    public void OnSendButtonClicked()
    {
        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            StartCoroutine(PostRequest(userMessage));
            Hide();
        }
    }

    // Coroutine to handle the POST request
    private IEnumerator PostRequest(string userMessage)
    {
        // Create the JSON payload with an instruction to limit the response length
        string instruction = $"Please limit your response to {maxResponseForGroq} characters.";
        string jsonPayload = "{\"messages\": [{\"role\": \"user\", \"content\": \"" + userMessage + " " + instruction + "\"}], \"model\": \"llama3-8b-8192\"}";

        // Create a new UnityWebRequest
        UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Send the request and wait for a response
        yield return webRequest.SendWebRequest();

        // Check for errors
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + webRequest.error);
            responseText.text = "Error: " + webRequest.error;
        }
        else
        {
            // Process the response
            string responseString = webRequest.downloadHandler.text;
            Debug.Log("Response: " + responseString);

            // Deserialize the JSON response to extract the AI's message content
            GroqResponse.ChatCompletion chatCompletion = JsonUtility.FromJson<GroqResponse.ChatCompletion>(responseString);
            string aiResponse = chatCompletion.choices[0].message.content;

            // Limit the response to the maximum length and display it in the UI
            aiResponse = aiResponse.Length > maxResponseLength ? aiResponse.Substring(0, maxResponseLength) : aiResponse;
            responseText.text = aiResponse;
        }
    }

    private void Show()
    {
        userText.gameObject.SetActive(true);
    }

    private void Hide()
    {
        userText.gameObject.SetActive(false);
    }
}
