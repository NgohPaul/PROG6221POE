Cybersecurity Awareness Chatbot

Overview

The Cybersecurity Awareness Chatbot is a console based AI assistant built in C# that helps users understand basic cybersecurity concepts. It answers user questions using Google’s Gemini AI model and provides friendly, informative responses on a wide range of cybersecurity topics.

This chatbot is designed as an educational tool to promote awareness about online safety, secure practices, and cybersecurity threats.

---

Features

- Voice Greeting – Plays a friendly audio message to greet the user at startup.
- AI-Powered Responses – Uses Gemini (Google Generative Language API) to respond to user questions intelligently.
- Color coded Console UI – Neatly formatted and color coded responses to improve readability.
- Smart Error Handling – Politely responds when the question is vague or not related to cybersecurity.
- Continuous Interaction – Allows users to ask multiple questions until they type "exit".

---

How It Works

1. When the program runs, it displays ASCII art and plays a greeting.
2. The user is prompted to enter their name.
3. The chatbot starts a Q&A session about cybersecurity.
4. User input is sent to the Gemini API.
5. The chatbot receives a response and displays it in blue/green text.
6. If the question is unrelated or unclear, the bot prompts the user to rephrase.
7. Typing "exit" ends the session.

---

Technologies Used

- C# (.NET Console App)
- Google Gemini API (for AI responses)
- Newtonsoft.Json (for JSON parsing)
- System.Media (for playing audio)
- Console Color and Formatting (for improved UX)

---

Setup Instructions

1. Clone this repository or copy the source code into a C# Console App in Visual Studio.
2. Install Dependencies:
   - Use NuGet to install `Newtonsoft.Json`
3. Replace API Key:
   - In the `Program.cs` file, replace the placeholder `YOUR_API_KEY` with your actual Google Gemini API key.
	- this needs to be done every 30 days because i am currently using the free play which only lasts for that duration of time.
4. Audio File:
   - Place the greeting `.wav` file in a `Resources` folder.
   - Update the file path in `PlayVoiceGreeting()` if needed.
5. Run the application.

---

Sample Questions You Can Ask

- What is phishing?
- How can I create a strong password?
- What is two-factor authentication?
- How do firewalls protect my data?

---

Note

This chatbot is focused only on cybersecurity topics. If a user asks a question unrelated to cybersecurity, the chatbot will respond with a friendly message encouraging them to rephrase the question.

---

Author

Developed by Paul Ngoh

---



