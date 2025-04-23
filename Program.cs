using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CybersecurityAwarenessChatbot
{
    class Program
    {
        static readonly string apiKey = "AIzaSyCUy2kyzzkLBkxlbeBoNAiAvYl66-I8RgU";
        static readonly string geminiEndpoint = $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-pro-002:generateContent?key={apiKey}";

        static async Task Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Assistant";
            Console.ForegroundColor = ConsoleColor.Cyan;
            DisplayAsciiArt();
            PlayVoiceGreeting();

            Console.ResetColor();
            Console.WriteLine("\n---------------------------------------------------");
            Console.Write("Hello! What is your name? ");
            string userName = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(userName))
            {
                Console.Write("Name cannot be empty. Please enter your name: ");
                userName = Console.ReadLine();
            }

            Console.WriteLine($"Nice to meet you, {userName}!");
            Console.WriteLine("Ask me a cybersecurity-related question or type 'exit' to leave.");
            Console.WriteLine("---------------------------------------------------");

            while (true)
            {
                Console.Write("\nYou: ");
                string userInput = Console.ReadLine().ToLower();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Bot: I didn’t quite understand that. Could you rephrase?");
                    continue;
                }

                if (userInput == "exit")
                {
                    Console.WriteLine("Bot: Stay safe online, goodbye!");
                    break;
                }

                // Get the response from Gemini AI for all input
                Console.Write("Bot: Thinking...\n");
                string aiResponse = await GetGeminiResponse(userInput);
                Console.WriteLine("Bot: " + aiResponse);
            }
        }

        static void DisplayAsciiArt()
        {
            Console.WriteLine(@"
  ____            _ _       
 |  _ \ __ _ _   _| |
 | |_) / _` | | | | |
 |  __/ (_| | |_| | |
 |_|   \__,_|\__,_|_|        
     ____        _   
    | __ )  ___ | |_ 
    |  _ \ / _ \| __|
    | |_) | (_) | |_ 
    |____/ \___/ \__|                            
            ");
        }

        static void PlayVoiceGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer(@"Resources\[Friendly Women]Hello......line.wav");
                player.Load();
                player.PlaySync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing audio: " + ex.Message);
            }
        }

        static async Task<string> GetGeminiResponse(string userQuestion)
        {
            using (HttpClient client = new HttpClient())
            {
                string prompt = $"You are a helpful cybersecurity assistant. Answer the following question clearly and simply:\n\n{userQuestion}";

                var body = new
                {
                    contents = new[] {
                        new {
                            parts = new[] {
                                new { text = prompt }
                            }
                        }
                    }
                };

                string json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(geminiEndpoint, content);
                string result = await response.Content.ReadAsStringAsync();

                try
                {
                    JObject data = JObject.Parse(result);

                    if (data["candidates"] != null && data["candidates"].HasValues)
                    {
                        return data["candidates"][0]["content"]["parts"][0]["text"]?.ToString() ?? "I couldn’t find a clear answer.";
                    }
                    else if (data["error"] != null)
                    {
                        return $"API Error: {data["error"]["message"]}";
                    }
                    else
                    {
                        return "I'm not sure how to answer that.";
                    }
                }
                catch (Exception ex)
                {
                    return $"Error parsing response: {ex.Message}";
                }
            }
        }
    }
}
