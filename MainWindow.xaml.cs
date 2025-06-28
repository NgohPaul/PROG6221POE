using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Media;
using System.Linq;
using System.Media;

namespace CyberBotWPF
{
    public partial class MainWindow : Window
    {
        private List<LogEntry> activityLog = new List<LogEntry>();
        private readonly string apiKey = "AIzaSyAQT9iiyhs2Jvgiw6iXjUo6bPq9RuCtXyY";
        private readonly string geminiEndpoint;

        private string conversationState = "Idle";
        private int quizIndex = 0;
        private int correctAnswers = 0;
        private bool isQuizActive = false;

        private TaskItem currentTaskInProgress;
        private List<TaskItem> tasks = new List<TaskItem>();

        private TaskConversationState taskState = TaskConversationState.Idle;
        private TaskItem taskBeingCreated = null;

        private string userName = "";


        public MainWindow()
        {
            InitializeComponent();
            geminiEndpoint = $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-pro-002:generateContent?key={apiKey}";
            PlayStartupSound();
            PromptForUserName();
        }
        private void AddToLog(string description)
        {
            activityLog.Add(new LogEntry
            {
                Timestamp = DateTime.Now,
                Description = description
            });

            // Optional: Keep only latest 50 entries
            if (activityLog.Count > 50)
                activityLog.RemoveAt(0);
        }
        private void PromptForUserName()
        {
            userName = Microsoft.VisualBasic.Interaction.InputBox(
                "Welcome! Please enter your name:",
                "Cybersecurity Bot – Greeting",
                "Your name here");

            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = "User"; // fallback
            }

            DisplayResponse($"👋 Hello {userName}! I am a cybersecurity chat bot. Ask me anything about cybersecurity, or type 'exit' to quit.");
            AddToLog($"User greeted as '{userName}'");
        }


        private void PlayStartupSound()
{
    try
    {
        // Combine to build the path to Resources\Hello.wav
        string relativePath = System.IO.Path.Combine("Resources", "Hello.wav");
        string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

        // Ensure the file exists before trying to play it
        if (File.Exists(fullPath))
        {
            SoundPlayer player = new SoundPlayer(fullPath);
            player.Play();  // Use .PlaySync() if you want it to block
        }
        else
        {
            MessageBox.Show($"❌ Audio file not found:\n{fullPath}", "Missing Audio", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Error playing audio:\n" + ex.Message, "Playback Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

        private void CloseLog_Click(object sender, RoutedEventArgs e)
        {
            ActivityLogContainer.Visibility = Visibility.Collapsed;
        }



        public enum TaskConversationState
        {
            Idle,
            AwaitingTitle,
            AwaitingDescription,
            AwaitingReminderConfirmation,
            AwaitingReminderDate
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string userInput = txtUser.Text.Trim();
            txtUser.Clear();

            if (!string.IsNullOrEmpty(userInput))
            {
                DisplayResponse($"You: {userInput}");
                _ = HandleUserInput(userInput);
            }
        }

        private void DisplayResponse(string response)
        {
            txtOutput.Text += $": {response}\n";
        }

        private void AddTask(TaskItem task)
        {
            tasks.Add(task);
            currentTaskInProgress = null;
           
        }

        //Handles the users input
        private async Task HandleUserInput(string input)
        {
            input = input.ToLower().Trim();

            // === NLP Keyword Triggers ===
            bool isTaskRelated = input.Contains("add") || input.Contains("task") || input.Contains("remind") || input.Contains("reminder");
            bool isQuizRequested = input.Contains("quiz");
            bool isActivityLogRequest = input.Contains("show activity log") || input.Contains("what have you done");

            // === Show Activity Log ===
            if (isActivityLogRequest)
            {
                var recentEntries = activityLog.Skip(Math.Max(0, activityLog.Count - 10)).Reverse().ToList();

                if (recentEntries.Count == 0)
                {
                    txtActivityLog.Text = "No activity yet.";
                }
                else
                {
                    txtActivityLog.Text = "Here's a summary of recent actions:\n\n" +
                        string.Join("\n", recentEntries.Select((e, i) => $"{i + 1}. {e.Description} ({e.Timestamp:yyyy-MM-dd HH:mm})"));
                }

                ActivityLogContainer.Visibility = Visibility.Visible;
                DisplayResponse("📝 Activity log displayed.");
                AddToLog("Activity log viewed by user.");

                // Optional: Auto-hide after delay
                await Task.Delay(30000);
                ActivityLogContainer.Visibility = Visibility.Collapsed;

                return;
            }

            // === Start Quiz Flow ===
            if (isQuizRequested)
            {
                StartQuiz();
                AddToLog("Quiz started.");
                return;
            }

            // === Start Task Flow ===
            if (isTaskRelated && taskState == TaskConversationState.Idle)
            {
                taskState = TaskConversationState.AwaitingTitle;
                taskBeingCreated = new TaskItem();
                DisplayResponse("Let's add a task! What is the title?");
                AddToLog("Task creation started.");
                btnComplete.Visibility = Visibility.Visible;
                return;
            }

            // === Continue Task Creation Flow ===
            switch (taskState)
            {
                case TaskConversationState.AwaitingTitle:
                    taskBeingCreated.Title = input;
                    txtTask.Text = $"Task Title: {taskBeingCreated.Title}";
                    taskState = TaskConversationState.AwaitingDescription;
                    DisplayResponse("Great. What is the description?");
                    AddToLog($"Task added: '{taskBeingCreated.Title}'");
                    return;

                case TaskConversationState.AwaitingDescription:
                    taskBeingCreated.Description = input;
                    txtTaskDes.Text = $"Description: {taskBeingCreated.Description}";
                    taskState = TaskConversationState.AwaitingReminderConfirmation;
                    DisplayResponse("Would you like to set a reminder? (yes/no)");
                    AddToLog($"Task description added: '{taskBeingCreated.Description}'");
                    return;

                case TaskConversationState.AwaitingReminderConfirmation:
                    if (input.Contains("yes"))
                    {
                        taskState = TaskConversationState.AwaitingReminderDate;
                        DisplayResponse("Please enter the date for the reminder (yyyy-MM-dd):");
                    }
                    else
                    {
                        taskBeingCreated.Reminder = null;
                        SaveTaskToFile(taskBeingCreated);
                        DisplayResponse($"✅ Task '{taskBeingCreated.Title}' saved without a reminder.");
                        AddToLog($"Task saved without reminder: '{taskBeingCreated.Title}'");
                        taskBeingCreated = null;
                        taskState = TaskConversationState.Idle;
                    }
                    return;

                case TaskConversationState.AwaitingReminderDate:
                    if (DateTime.TryParse(input, out DateTime reminderDate))
                    {
                        taskBeingCreated.Reminder = reminderDate;
                        txtTaskDate.Text = $"Reminder Date: {reminderDate:yyyy-MM-dd}";
                        SaveTaskToFile(taskBeingCreated);
                        DisplayResponse($"✅ Task '{taskBeingCreated.Title}' saved with reminder on {reminderDate:yyyy-MM-dd}.");
                        AddToLog($"Reminder set for '{taskBeingCreated.Title}' on {reminderDate:yyyy-MM-dd}");
                        taskBeingCreated = null;
                        taskState = TaskConversationState.Idle;
                    }
                    else
                    {
                        DisplayResponse("❌ Invalid date format. Please use yyyy-MM-dd:");
                    }
                    return;
            }

            // === Fallback to Gemini API for general questions ===
            string botReply = await GetGeminiResponse(input);
            DisplayResponse(botReply);
            AddToLog($"User asked: '{input}' | Bot responded.");
        }



        private async Task<string> GetGeminiResponse(string userQuestion)
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

                try
                {
                    HttpResponseMessage response = await client.PostAsync(geminiEndpoint, content);
                    string result = await response.Content.ReadAsStringAsync();
                    JObject data = JObject.Parse(result);

                    if (data["candidates"] != null && data["candidates"].HasValues)
                    {
                        string answer = data["candidates"][0]["content"]["parts"][0]["text"]?.ToString();
                        return string.IsNullOrWhiteSpace(answer) || answer.Length < 30
                            ? "Try rephrasing with a clearer cybersecurity question."
                            : answer;
                    }
                    else if (data["error"] != null)
                    {
                        return $"API Error: {data["error"]["message"]}";
                    }
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }

                return "Hmm... I couldn't understand that. Try asking another cybersecurity question.";
            }
        }

        private void SaveTaskToFile(TaskItem task)
        {
            string path = "tasks.txt";
            string taskLine = $"Title: {task.Title}\nDescription: {task.Description}";
            if (task.Reminder.HasValue)
            {
                taskLine += $"\nReminder: {task.Reminder.Value:yyyy-MM-dd}";
            }
            taskLine += "\n-------------------------\n";

            File.AppendAllText(path, taskLine);
        }

        private void txtUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        // === QUIZ LOGIC ===
        private void StartQuiz()
        {
            AddToLog("Quiz started");

            quizIndex = 0;
            correctAnswers = 0;
            isQuizActive = true;

            QuizPanel.Visibility = Visibility.Visible;
            txtQuizScore.Visibility = Visibility.Visible; // Show score when quiz starts
            txtQuizScore.Text = $" Live Score: {correctAnswers} / {QuizBank.Questions.Count}";

            ShowNextQuizQuestion();
        }


        private void ShowNextQuizQuestion()
        {
            QuizOptionsPanel.Children.Clear();
            txtQuizFeedback.Text = "";

            if (quizIndex >= QuizBank.Questions.Count)
            {
                EndQuiz();
                return;
            }

            var question = QuizBank.Questions[quizIndex];
            txtQuizQuestion.Text = question.Question;

            for (int i = 0; i < question.Options.Length; i++)
            {
                var btn = new Button
                {
                    Content = question.Options[i],
                    Margin = new Thickness(0, 5, 0, 0),
                    Tag = i
                };
                btn.Click += QuizOption_Click;
                QuizOptionsPanel.Children.Add(btn);
            }
        }

        private void QuizOption_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = (int)((Button)sender).Tag;
            var question = QuizBank.Questions[quizIndex];

            if (selectedIndex == question.CorrectOptionIndex)
            {
                correctAnswers++;
                txtQuizFeedback.Text = "✅ Correct! " + question.Explanation;
                txtQuizFeedback.Foreground = Brushes.Green;
            }
            else
            {
                string correctAnswer = question.Options[question.CorrectOptionIndex];
                txtQuizFeedback.Text = $"❌ Incorrect. The correct answer is: '{correctAnswer}'. {question.Explanation}";
                txtQuizFeedback.Foreground = Brushes.Red;
            }

            txtQuizScore.Text = $"Score: {correctAnswers} / {QuizBank.Questions.Count}";

            quizIndex++;
            Task.Delay(1500).ContinueWith(_ => Dispatcher.Invoke(ShowNextQuizQuestion));
        }

        private void EndQuiz()
        {
            AddToLog($"Quiz completed. Final score: {correctAnswers} / {QuizBank.Questions.Count}");

            QuizPanel.Visibility = Visibility.Collapsed;
            txtQuizScore.Visibility = Visibility.Collapsed; // Hide score when quiz ends
            isQuizActive = false;

            string resultMsg = correctAnswers >= 7
                ? "Great job! You're a cybersecurity pro!"
                : "Keep learning to stay safe online!";

            DisplayResponse($"Quiz complete! Score: {correctAnswers}/{QuizBank.Questions.Count}. {resultMsg}");
        }

        
            private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            txtTask.Text = "TO DO LIST!";
            txtTask.FontWeight = FontWeights.Bold;

            txtTaskDes.Text = "";
            txtTaskDate.Text = "";

            btnComplete.Visibility = Visibility.Collapsed;
        }

    }
}

    public class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Reminder { get; set; }
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Options { get; set; }
        public int CorrectOptionIndex { get; set; }
        public string Explanation { get; set; }

    }
   

