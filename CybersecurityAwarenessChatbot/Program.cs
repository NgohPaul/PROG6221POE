using System;
using System.Media;

namespace CybersecurityAwarenessChatbot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Assistant";
            Console.ForegroundColor = ConsoleColor.Cyan;
            DisplayAsciiArt();
            PlayVoiceGreeting(); // Voice

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
            Console.WriteLine("Examples: how can I browse safely, what is password safety?,what is phishing ?");
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

                switch (userInput)
                {
                                                
                                        
                    case "what is password safety?":
                        Console.WriteLine("Bot: Password safety means using strong, unique passwords for each of your accounts. A strong password should be at least 12 characters long and include a mix of upper- and lowercase letters, numbers, and special characters. Avoid using personal info like your name or birthdate. Consider using a password manager to generate and store secure passwords.");
                        break;

                    case "what is phishing?":
                        Console.WriteLine("Bot: Phishing is a cyberattack where scammers send fake messages (often emails or texts) pretending to be from trusted sources to trick you into revealing personal or financial information. Look out for urgent language, suspicious links, or unknown senders. Always verify links and don't share personal details unless you're 100% sure it's legitimate.");
                        break;

                    case "how can i browse safely?":
                        Console.WriteLine("Bot: To browse safely:\n- Always check that websites use HTTPS (you'll see a padlock icon in the address bar).\n- Don’t click on suspicious popups or ads.\n- Keep your browser and antivirus software updated.\n- Use a secure, privacy-focused browser.\n- Don’t download files from untrusted sites.\n- Use strong passwords and two-factor authentication where possible.");
                        break;

                    default:
                        Console.WriteLine("Bot: I didn’t quite understand that. Could you rephrase?");
                        break;
                }

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
                //  Sound Path
                SoundPlayer player = new SoundPlayer(@"Resources\[Friendly Women]Hello......line.wav");
                player.Load();
                player.PlaySync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing audio: " + ex.Message);
            }
        }
    }
}
