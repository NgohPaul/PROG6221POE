using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace CybersecurityAwarenessChatbot
{
    class Program
    {
        static readonly string apiKey = "AIzaSyAQT9iiyhs2Jvgiw6iXjUo6bPq9RuCtXyY"; 
        static readonly string geminiEndpoint = $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-pro-002:generateContent?key={apiKey}";

        static async Task Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Assistant";
            Console.ForegroundColor = ConsoleColor.Cyan;
            DisplayHeader();
            DisplayAsciiArt();
            PlayVoiceGreeting();

            Console.ResetColor();
            Console.Write("\n Please enter your name: ");
            string userName = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(userName))
            {
                Console.Write("Name cannot be empty. Please enter your name: ");
                userName = Console.ReadLine();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n Hello, {userName}! I'm your cybersecurity assistant.");
            Console.WriteLine(" Ask me anything about cybersecurity, or type 'exit' to quit.");
            Console.ResetColor();

            Console.WriteLine("\n──────────────────────────────────────────────");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nYou: ");
                Console.ResetColor();
                string userInput = Console.ReadLine().ToLower();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bot: I didn’t quite understand that. Could you rephrase?");
                    Console.ResetColor();
                    continue;
                }

                if (userInput == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n Bot: Stay safe online, goodbye!");
                    Console.ResetColor();
                    break;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(" Bot: Thinking");
                AnimateDots();
                Console.ResetColor();

                string aiResponse = await GetGeminiResponse(userInput);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" Bot: " + aiResponse);
                Console.ResetColor();

                Console.WriteLine("\n──────────────────────────────────────────────");
            }
        }

        static void DisplayHeader()
        {
            Console.WriteLine("╔═══════════════════════════════════════════════╗");
            Console.WriteLine("║         CYBERSECURITY AWARENESS BOT          ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");
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

        static void AnimateDots()
        {
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(400);
                Console.Write(".");
            }
            Console.WriteLine();
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error playing audio: " + ex.Message);
                Console.ResetColor();
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

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(geminiEndpoint, content);
                string result = await response.Content.ReadAsStringAsync();

                try
                {
                    JObject data = JObject.Parse(result);

                    if (data["candidates"] != null && data["candidates"].HasValues)
                    {
                        string answer = data["candidates"][0]["content"]["parts"][0]["text"]?.ToString();

                        if (string.IsNullOrWhiteSpace(answer) ||
                            answer.ToLower().Contains("i'm not sure") ||
                            answer.ToLower().Contains("i don't understand") ||
                            answer.ToLower().Contains("i couldn’t find") ||
                            answer.Length < 30)
                        {
                            return "I don't really understand. Try rephrasing with a cybersecurity-related question.";
                        }

                        return answer;
                    }
                    else if (data["error"] != null)
                    {
                        return $"API Error: {data["error"]["message"]}";
                    }
                    else
                    {
                        return "I don't really understand. Try rephrasing with a cybersecurity-related question.";
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
