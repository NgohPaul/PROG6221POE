using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberBotWPF
{
    public class QuizBank
    { 
        public static List<QuizQuestion> Questions = new List<QuizQuestion>
        {
            new QuizQuestion {
                Question = "What should you do if you receive an email asking for your password?",
                Options = new[] { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                CorrectOptionIndex = 2,
                Explanation = "Correct! Reporting phishing emails helps prevent scams."
            },
            new QuizQuestion {
                Question = "True or False: Using the same password for multiple sites is safe.",
                Options = new[] { "True", "False" },
                CorrectOptionIndex = 1,
                Explanation = "False. Always use unique passwords to limit risk."
            },
            new QuizQuestion {
                Question = "Which is the strongest password?",
                Options = new[] { "Password123", "Qwerty2023", "JohnSmith2022", "x$4T!9@zQ" },
                CorrectOptionIndex = 3,
                Explanation = "Correct! Complex passwords with symbols and numbers are strongest."
            },
            new QuizQuestion {
                Question = "Phishing attempts often try to:",
                Options = new[] { "Improve your PC performance", "Steal sensitive information", "Install helpful apps", "Boost your internet speed" },
                CorrectOptionIndex = 1,
                Explanation = "Correct! Phishing is used to steal data like passwords or credit cards."
            },
            new QuizQuestion {
                Question = "What is two-factor authentication (2FA)?",
                Options = new[] { "A backup password", "A way to reset your password", "A second form of login verification", "A fingerprint login only" },
                CorrectOptionIndex = 2,
                Explanation = "Correct! 2FA adds an extra layer of security to your login process."
            },
            new QuizQuestion {
                Question = "True or False: You should click all links in emails from your bank.",
                Options = new[] { "True", "False" },
                CorrectOptionIndex = 1,
                Explanation = "False. Always verify the source before clicking email links."
            },
            new QuizQuestion {
                Question = "What’s a sign of a phishing website?",
                Options = new[] { "HTTPS in the URL", "Misspelled domain names", "Contact info listed", "Secure padlock icon" },
                CorrectOptionIndex = 1,
                Explanation = "Correct! Phishing sites often use misspelled or fake domains."
            },

            new QuizQuestion {
                Question = "How often should you update your passwords?",
                Options = new[] { "Every 5 years", "Once a month", "Only when hacked", "Every few months" },
                CorrectOptionIndex = 3,
                Explanation = "Correct! Updating passwords regularly reduces security risks."
            },
            new QuizQuestion {
                Question = "Which of the following is a secure practice?",
                Options = new[] { "Using public Wi-Fi for banking", "Saving passwords in plain text", "Using a password manager", "Sharing passwords with friends" },
                CorrectOptionIndex = 2,
                Explanation = "Correct! Password managers securely store your credentials."
            },
            new QuizQuestion {
                Question = "Social engineering is:",
                Options = new[] { "A programming language", "A way hackers manipulate people", "A type of firewall", "A security software" },
                CorrectOptionIndex = 1,
                Explanation = "Correct! Social engineering tricks people into revealing confidential info."
            }
        };
    }
}

