using System;
using System.IO;
using NAudio.Wave;
using System.Collections.Generic;
using System.Linq;

public class InfoGuardCombined
{
    public string UserName { get; set; }
    private Dictionary<string, Dictionary<Sentiment, List<string>>> sentimentAwareResponses =
        new Dictionary<string, Dictionary<Sentiment, List<string>>>()
        {
            { "password", new Dictionary<Sentiment, List<string>>()
                {
                    { Sentiment.Neutral, new List<string> {
                        "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
                        "A strong password typically includes a mix of uppercase and lowercase letters, numbers, and symbols.",
                        "Consider using a password manager to securely store and generate complex passwords."
                    }},
                    { Sentiment.Worried, new List<string> {
                        "Feeling worried about password security is understandable. Let's focus on creating strong ones.",
                        "It's wise to be concerned about password safety. I can give you some tips to ease your worries."
                    }}
                    // Add other sentiments if needed
                }
            },
            { "scam", new Dictionary<Sentiment, List<string>>()
                {
                    { Sentiment.Neutral, new List<string> {
                        "Be wary of online scams that may try to trick you into giving away personal information or money.",
                        "If something sounds too good to be true, it probably is a scam.",
                        "Never share sensitive information with unverified sources."
                    }},
                    { Sentiment.Worried, new List<string> {
                        "It's understandable to feel worried about online scams. They can be very convincing. Let's discuss some ways to identify and avoid them.",
                        "Being concerned about scams is a good sign! I can share some tips to help you feel more secure."
                    }}
                    // Add other sentiments if needed
                }
            },
            { "privacy", new Dictionary<Sentiment, List<string>>()
                {
                    { Sentiment.Neutral, new List<string> {
                        "Review your privacy settings on online accounts to control who sees your information.",
                        "Be mindful of the information you share online.",
                        "Consider using privacy-focused tools and services."
                    }},
                    { Sentiment.Worried, new List<string> {
                        "It's natural to be worried about online privacy. There are steps you can take to protect your personal information.",
                        "Concerns about privacy are valid. Let's explore some ways to enhance your online privacy."
                    }}
                    // Add other sentiments if needed
                }
            },
            { "phishing", new Dictionary<Sentiment, List<string>>()
                {
                    { Sentiment.Neutral, new List<string> {
                        "Phishing attempts often come in the form of emails or messages that look legitimate but are designed to steal your information.",
                        "Never click on suspicious links or open attachments from unknown senders.",
                        "Verify the sender's authenticity through official channels if you are unsure."
                    }},
                    { Sentiment.Worried, new List<string> {
                        "Phishing can be a real concern. I can provide some guidance on how to spot and avoid phishing attempts.",
                        "Feeling uneasy about phishing is understandable. Let me share some crucial tips."
                    }}
                    // Add other sentiments if needed
                }
            },
            { "malware", new Dictionary<Sentiment, List<string>>()
                {
                    { Sentiment.Neutral, new List<string> {
                        "Malware is malicious software that can harm your device or steal your data. Install reputable antivirus software and keep it updated.",
                        "Be cautious when downloading files or installing software from the internet.",
                        "Avoid clicking on suspicious ads or pop-ups."
                    }},
                    { Sentiment.Worried, new List<string> {
                        "Worrying about malware is reasonable. Protecting your devices is important. Let's talk about how to do that.",
                        "It's wise to be concerned about malware. I can give you some advice on prevention."
                    }}
                    // Add other sentiments if needed
                }
            },
            { "online safety", new Dictionary<Sentiment, List<string>>()
                {
                    { Sentiment.Neutral, new List<string> {
                        "Practice safe browsing habits by avoiding suspicious websites and using secure connections (HTTPS).",
                        "Keep your software and operating system updated to patch security vulnerabilities.",
                        "Be aware of social engineering tactics that criminals use to manipulate you."
                    }},
                    { Sentiment.Worried, new List<string> {
                        "Feeling worried about overall online safety is common. Let's break it down into manageable steps.",
                        "It's good that you're thinking about online safety. I can offer some fundamental guidelines."
                    }}
                    // Add other sentiments if needed
                }
            },
            { "personal information", new Dictionary<Sentiment, List<string>>()
                {
                    { Sentiment.Neutral, new List<string> {
                        "Be cautious about sharing personal information online. Only provide it to trusted sources when necessary.",
                        "Securely store any sensitive personal data and avoid keeping it unnecessarily.",
                        "Be aware of data breaches and take steps to protect your accounts if your information may have been compromised."
                    }},
                    { Sentiment.Worried, new List<string> {
                        "Concerns about protecting personal information online are valid. Let's discuss how to minimize risks.",
                        "It's important to be careful with your personal information. I can share some best practices."
                    }}
                    // Add other sentiments if needed
                }
            },
            { "how are you", new Dictionary<Sentiment, List<string>>()
                {
                    { Sentiment.Neutral, new List<string> { "I'm doing well, thank you! Ready to help you with cybersecurity." } }
                }
            },
            { "what's your purpose", new Dictionary<Sentiment, List<string>>()
                {
                    { Sentiment.Neutral, new List<string> { "My purpose is to educate you about cybersecurity best practices." } }
                }
            },
            { "what can i ask you about", new Dictionary<Sentiment, List<string>>()
                {
                    { Sentiment.Neutral, new List<string> { "You can ask me about:\n- Password safety\n- Phishing\n- Safe browsing\n- And other cybersecurity topics!" } }
                }
            }
            // Add more cybersecurity-related keywords and sentiment-aware responses
        };

    private List<string> phishingTips = new List<string>()
    {
        "Be cautious of emails asking for personal information.",
        "Scammers often disguise themselves as trusted organizations.",
        "Check the sender's email address carefully for any inconsistencies.",
        "Never click on links in suspicious emails.",
        "If in doubt, contact the organization directly through a verified channel."
    };

    private Dictionary<string, string> memory = new Dictionary<string, string>();
    private Dictionary<string, Sentiment> sentimentHistory = new Dictionary<string, Sentiment>();
    private Dictionary<string, string> lastResponse = new Dictionary<string, string>(); // To avoid immediate repetition

    private enum Sentiment { Worried, Curious, Frustrated, Neutral }

    public void Run()
    {
        PlayVoiceGreeting();
        DisplayAsciiArt();
        GreetUser();
        HandleUserInteraction();
    }

    private void PlayVoiceGreeting()
    {
        try
        {
            string soundFilePath = "greeting.wav";

            if (File.Exists(soundFilePath))
            {
                try
                {
                    using (var audioFile = new AudioFileReader(soundFilePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
                catch (Exception audioEx)
                {
                    Console.WriteLine($"Error during audio playback: {audioEx.Message}");
                }

            }
            else
            {
                Console.WriteLine("Warning: Voice greeting file not found. Place 'greeting.wav' in the application's directory.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing audio file: {ex.Message}");
        }
    }

    private void DisplayAsciiArt()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
    /\_/\
  ( o.o )
    > ^ <  InfoGuard
  /  _  \
 | (_) |  Meow! Let's stay safe online!
 \_____/
");
        Console.ResetColor();
    }

    private void GreetUser()
    {
        Console.Write("Please enter your name: ");
        UserName = Console.ReadLine();

        if (string.IsNullOrEmpty(UserName))
        {
            UserName = "User";
        }

        Console.WriteLine($"\nHello, {UserName}! Welcome to InfoGuard.");
    }

    private void HandleUserInteraction()
    {
        string previousInput = null;
        while (true)
        {
            Console.Write($"\nAsk me a cybersecurity question, {UserName} (or type 'exit' to quit): ");
            string userInput = Console.ReadLine();

            if (userInput.ToLower() == "exit")
            {
                Console.WriteLine("Goodbye! Stay safe online.");
                break;
            }

            string response = GetResponse(userInput, previousInput);
            Console.WriteLine(response);
            previousInput = userInput;
        }
    }

    private string GetResponse(string userInput, string previousInput)
    {
        userInput = userInput.ToLower();
        Sentiment currentSentiment = DetectSentiment(userInput);
        if (!string.IsNullOrEmpty(UserName))
        {
            sentimentHistory[UserName] = currentSentiment;
        }

        // Memory Recall
        if (userInput.StartsWith("remember my name is"))
        {
            string remainingText = userInput.Substring("remember my name is".Length).Trim();
            if (!string.IsNullOrEmpty(remainingText))
            {
                UserName = remainingText;
                memory["name"] = remainingText;
                return $"Okay, I'll remember your name is {remainingText}.";
            }
            else
            {
                return "Please provide the name you want me to remember.";
            }
        }
        else if (userInput.StartsWith("remember my favorite cybersecurity topic is"))
        {
            string remainingText = userInput.Substring("remember my favorite cybersecurity topic is".Length).Trim();
            if (!string.IsNullOrEmpty(remainingText))
            {
                string topic = remainingText;
                memory["favorite_topic"] = topic;
                return $"Got it! I'll remember that your favorite cybersecurity topic is {topic}.";
            }
            else
            {
                return "Please provide your favorite cybersecurity topic.";
            }
        }
        else if (userInput.Contains("what do you remember about me"))
        {
            if (memory.Count > 0)
            {
                string memoryRecall = "I remember ";
                if (memory.ContainsKey("name"))
                {
                    memoryRecall += $"your name is {memory["name"]}";
                    if (memory.ContainsKey("favorite_topic"))
                    {
                        memoryRecall += $" and your favorite cybersecurity topic is {memory["favorite_topic"]}.";
                    }
                    else
                    {
                        memoryRecall += ".";
                    }
                }
                else if (memory.ContainsKey("favorite_topic"))
                {
                    memoryRecall += $"your favorite cybersecurity topic is {memory["favorite_topic"]}.";
                }
                return memoryRecall;
            }
            else
            {
                return "I don't remember anything specific about you yet.";
            }
        }
        else if (previousInput != null && userInput.Contains("tell me more about") && ContainsKeyword(previousInput, sentimentAwareResponses.Keys.ToArray()))
        {
            string keyword = GetKeyword(previousInput, sentimentAwareResponses.Keys.ToArray());
            if (sentimentAwareResponses.ContainsKey(keyword) && sentimentAwareResponses[keyword].ContainsKey(Sentiment.Neutral))
            {
                var availableResponses = sentimentAwareResponses[keyword][Sentiment.Neutral].Where(r => r != GetLastResponse(keyword));
                if (availableResponses.Any())
                {
                    string response = availableResponses.ElementAt(new Random().Next(availableResponses.Count()));
                    UpdateLastResponse(keyword, response);
                    return response;
                }
                else
                {
                    return $"I've already shared some information about {keyword}. Is there anything else specific you'd like to know?";
                }
            }
        }

        // Sentiment-aware Keyword Responses
        foreach (var keyword in sentimentAwareResponses.Keys)
        {
            if (userInput.Contains(keyword))
            {
                if (sentimentAwareResponses[keyword].ContainsKey(currentSentiment) && sentimentAwareResponses[keyword][currentSentiment].Any())
                {
                    string response = sentimentAwareResponses[keyword][currentSentiment].ElementAt(new Random().Next(sentimentAwareResponses[keyword][currentSentiment].Count()));
                    UpdateLastResponse(keyword, response);
                    return response;
                }
                else if (sentimentAwareResponses[keyword].ContainsKey(Sentiment.Neutral) && sentimentAwareResponses[keyword][Sentiment.Neutral].Any())
                {
                    string response = sentimentAwareResponses[keyword][Sentiment.Neutral].ElementAt(new Random().Next(sentimentAwareResponses[keyword][Sentiment.Neutral].Count()));
                    UpdateLastResponse(keyword, response);
                    return response;
                }
            }
        }

        if (userInput.Contains("give me a phishing tip"))
        {
            string tip = GetRandomPhishingTip();
            UpdateLastResponse("phishing_tip", tip);
            return tip;
        }

        // Error Handling and Edge Cases
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return "Please enter a question.";
        }
        else
        {
            return "I didn't quite understand that. Could you rephrase?";
        }
    }

    private Sentiment DetectSentiment(string text)
    {
        text = text.ToLower();
        if (text.Contains("worried") || text.Contains("concerned") || text.Contains("anxious") || text.Contains("uneasy"))
        {
            return Sentiment.Worried;
        }
        else if (text.Contains("curious") || text.Contains("interested to know") || text.Contains("tell me more") || text.Contains("what is"))
        {
            return Sentiment.Curious;
        }
        else if (text.Contains("frustrated") || text.Contains("confused") || text.Contains("don't understand") || text.Contains("this is hard"))
        {
            return Sentiment.Frustrated;
        }
        return Sentiment.Neutral;
    }

    private string GetRandomPhishingTip()
    {
        if (phishingTips.Count > 0)
        {
            return phishingTips[new Random().Next(phishingTips.Count)];
        }
        return "Here's a tip: Be cautious of unsolicited emails.";
    }

    private bool ContainsKeyword(string text, string[] keywords)
    {
        text = text.ToLower();
        return keywords.Any(k => text.Contains(k));
    }

    private string GetKeyword(string text, string[] keywords)
    {
        text = text.ToLower();
        return keywords.FirstOrDefault(k => text.Contains(k));
    }

    private string GetLastResponse(string key)
    {
        return lastResponse.ContainsKey(key) ? lastResponse[key] : null;
    }

    private void UpdateLastResponse(string key, string response)
    {
        lastResponse[key] = response;
    }

    public static void Main(string[] args)
    {
        InfoGuardCombined infoGuard = new InfoGuardCombined();
        infoGuard.Run();
    }
}
