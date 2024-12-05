using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Unity.Profiling.LowLevel;

public class OpenAIScoreInterpreter : MonoBehaviour
{
    public enum LevelName
    {
        PUW,
        GB,
        LS,
        TM,
    };
    [SerializeField]
    private LevelName level;


    private const string ApiKey = "";  // Replace with your actual OpenAI API key
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

    public string[] pdfFiles = { "Assets/Resources/BIS_11.pdf", "Assets/Resources/PersonalityBigFiveInventory.pdf", "Assets/Resources/UPPSGerman.pdf" };
    public string dataFilePath = "Assets/Resources/Scripts/PUW/PUWData.cs";

    // Load the content of PDFs and C# data file
    public static OpenAIScoreInterpreter Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private async void Start()
    {

    }

    private async Task<string> ExtractTextFromPdfs(string[] filePaths)
    {
        StringBuilder content = new StringBuilder();

        foreach (string filePath in filePaths)
        {
            if (File.Exists(filePath))
            {
                content.Append(File.ReadAllText(filePath));
            }
            else
            {
                Debug.LogWarning($"File not found: {filePath}");
            }
        }
        return content.ToString();
    }

    private Dictionary<string, float> ExtractDataFromCSharp(string filePath)
    {
        var data = new Dictionary<string, float>();
        string pattern = @"PUWInputData\.Add\(""([^""]+)"",\s*(\d+\.?\d*)\);";

        if (File.Exists(filePath))
        {
            string text = File.ReadAllText(filePath);
            foreach (Match match in Regex.Matches(text, pattern))
            {
                string key = match.Groups[1].Value;
                float value = float.Parse(match.Groups[2].Value);
                data[key] = value;
            }
        }
        else
        {
            Debug.LogWarning($"File not found: {filePath}");
        }
        return data;
    }

    private string PreparePrompt(string pdfText, Dictionary<string, float> dataDict)
    {
        var prompt = new StringBuilder();
        switch (level)
        {
            case LevelName.LS:
                {
                    // Adding the default starting text for the prompt
                    prompt.AppendLine("The following analysis should consider multiple facets for a better understanding:");
                    /*prompt.AppendLine();
                    prompt.AppendLine("Context from PDF: ");
                    prompt.AppendLine(pdfText);
                    prompt.AppendLine();*/

                    // Include strategy if available
                    prompt.AppendLine("Important Focus point: ");
                    prompt.AppendLine("The defined strategy emphasizes the importance of the statement below as a key consideration");
                    prompt.AppendLine($"strategy: {ParticipantSettings.Instance.getStrategy()}");
                    prompt.AppendLine();

                    prompt.AppendLine("You are given in addition a set of tagged measurements, which should be interpreted together with the important focus point and mapped to psychological scores based on UPPS, Big Five Inventory, and BIS-11 assessments.");
                    prompt.AppendLine("Present each score as an integer within the specified range, as shown below:");
                    prompt.AppendLine();
                    prompt.AppendLine("// UPPS Scores");
                    prompt.AppendLine("Urgency: [11-44]");
                    prompt.AppendLine("Lack of premeditation: [10-40]");
                    prompt.AppendLine("Lack of perseverance: [10-40]");
                    prompt.AppendLine("Sensation seeking: [12-48]");
                    prompt.AppendLine();
                    prompt.AppendLine("// Big Five Traits");
                    prompt.AppendLine("Extraversion: [8-40]");
                    prompt.AppendLine("Introversion: [40-Extraversion]");
                    prompt.AppendLine("Agreeableness: [9-45]");
                    prompt.AppendLine("Antagonism: [45-Agreeableness]");
                    prompt.AppendLine("Conscientiousness: [9-45]");
                    prompt.AppendLine("Lack of direction: [45-Conscientiousness]");
                    prompt.AppendLine("Neuroticism: [8-40]");
                    prompt.AppendLine("Emotional stability: [40-Neuroticism]");
                    prompt.AppendLine("Openness: [10-50]");
                    prompt.AppendLine("Closedness to experience: [50-Openness]");
                    prompt.AppendLine();
                    prompt.AppendLine("// BIS-11 Scores");
                    prompt.AppendLine("Attention score: [0-20]");
                    prompt.AppendLine("Cognitive Instability score: [0-12]");
                    prompt.AppendLine("Motor Scores: [0-28]");
                    prompt.AppendLine("Perseverance scores: [0-16]");
                    prompt.AppendLine("Self-Control scores: [0-24]");
                    prompt.AppendLine("Cognitive Complexity scores: [0-20]");
                    prompt.AppendLine();
                    prompt.AppendLine("After presenting the scores, provide a detailed interpretation of the individual's personality and tendencies, including behavioral predictions and solutions in the following scenarios:");
                    prompt.AppendLine("Social settings");
                    prompt.AppendLine("Sad situations");
                    prompt.AppendLine("Stressful situations");
                    prompt.AppendLine("Peaceful situations");
                    prompt.AppendLine("Situations under pressure");
                    prompt.AppendLine("Overwhelming situations");
                    prompt.AppendLine("Complex situations");
                    prompt.AppendLine("Reward-triggering environments");
                    prompt.AppendLine("Risk-prone environments");
                    prompt.AppendLine();
                    prompt.AppendLine("Additionally, offer coping strategies and practical recommendations to help the individual manage challenges in these areas. Do not repeat any parts of the response.");
                    prompt.AppendLine();
                    prompt.AppendLine("Given the following tagged measurements:");
                    // Add details from dataDict to provide a more specific prompt
                    foreach (var entry in dataDict)
                    {
                        if (entry.Key != "strategy") // Avoid repeating the strategy key
                        {
                            prompt.AppendLine($"- {entry.Key}: {entry.Value:0.00}");
                        }
                    }
                    return prompt.ToString();
                }

            default:
                {
                    prompt.AppendLine("You are given a set of tagged measurements, which should be interpreted and mapped to psychological scores based on UPPS, Big Five Inventory, and BIS-11 assessments.");
                    prompt.AppendLine("Present each score as an integer within the specified range, as shown below:");
                    prompt.AppendLine();
                    prompt.AppendLine("// UPPS Scores");
                    prompt.AppendLine("Urgency: [11-44]");
                    prompt.AppendLine("Lack of premeditation: [10-40]");
                    prompt.AppendLine("Lack of perseverance: [10-40]");
                    prompt.AppendLine("Sensation seeking: [12-48]");
                    prompt.AppendLine();
                    prompt.AppendLine("// Big Five Traits");
                    prompt.AppendLine("Extraversion: [8-40]");
                    prompt.AppendLine("Introversion: [40-Extraversion]");
                    prompt.AppendLine("Agreeableness: [9-45]");
                    prompt.AppendLine("Antagonism: [45-Agreeableness]");
                    prompt.AppendLine("Conscientiousness: [9-45]");
                    prompt.AppendLine("Lack of direction: [45-Conscientiousness]");
                    prompt.AppendLine("Neuroticism: [8-40]");
                    prompt.AppendLine("Emotional stability: [40-Neuroticism]");
                    prompt.AppendLine("Openness: [10-50]");
                    prompt.AppendLine("Closedness to experience: [50-Openness]");
                    prompt.AppendLine();
                    prompt.AppendLine("// BIS-11 Scores");
                    prompt.AppendLine("Attention score: [0-20]");
                    prompt.AppendLine("Cognitive Instability score: [0-12]");
                    prompt.AppendLine("Motor Scores: [0-28]");
                    prompt.AppendLine("Perseverance scores: [0-16]");
                    prompt.AppendLine("Self-Control scores: [0-24]");
                    prompt.AppendLine("Cognitive Complexity scores: [0-20]");
                    prompt.AppendLine();
                    prompt.AppendLine("After presenting the scores, provide a detailed interpretation of the individual's personality and tendencies, including behavioral predictions and solutions in the following scenarios:");
                    prompt.AppendLine("Social settings");
                    prompt.AppendLine("Sad situations");
                    prompt.AppendLine("Stressful situations");
                    prompt.AppendLine("Peaceful situations");
                    prompt.AppendLine("Situations under pressure");
                    prompt.AppendLine("Overwhelming situations");
                    prompt.AppendLine("Complex situations");
                    prompt.AppendLine("Reward-triggering environments");
                    prompt.AppendLine("Risk-prone environments");
                    prompt.AppendLine();
                    prompt.AppendLine("Additionally, offer coping strategies and practical recommendations to help the individual manage challenges in these areas. Do not repeat any parts of the response.");
                    prompt.AppendLine();
                    prompt.AppendLine("Given the following tagged measurements:");

                    foreach (var entry in dataDict)
                    {
                        prompt.AppendLine($"{entry.Key}: {entry.Value}");
                    }
                    return prompt.ToString();
                }

        }
    }

    public async Task<string> GenerateInterpretation()
    {
        string pdfContent = await ExtractTextFromPdfs(pdfFiles);
        Dictionary<string, float> inputData = await ParticipantSettings.Instance.GeneratePUWInputData(); //ExtractDataFromCSharp(dataFilePath);
        string prompt = PreparePrompt(pdfContent, inputData);
        Debug.Log($"Promt: {prompt}");
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            var requestContent = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 750,
                temperature = 0.3
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");

            try
            {
                Debug.Log("Sending request to OpenAI API...");
                HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();
                Debug.Log($"Received response from OpenAI API: {responseString}");

                JObject responseJson = JObject.Parse(responseString);

                if (responseJson != null && responseJson["choices"] is JArray choices && choices.Count > 0)
                {
                    // Extract the "message" and then "content" within it
                    var message = choices[0]["message"] as JObject;
                    if (message != null && message["content"] != null)
                    {
                        var messageContent = message["content"].ToString().Trim();
                        Debug.Log("Interpretation generated successfully.");
                        return messageContent;
                    }
                    else
                    {
                        Debug.LogError("Missing 'content' field in 'message'.");
                    }
                }
                else
                {
                    Debug.LogError("Unexpected response structure or missing 'choices' key.");
                }
                return null;
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"HTTP request error: {e.Message}");
                return null;
            }
            catch (JsonException e)
            {
                Debug.LogError($"JSON parsing error: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error generating interpretation: {e.Message}");
                return null;
            }
        }
    }
}
