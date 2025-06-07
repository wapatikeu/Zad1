using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    private static char[] availableLetters;
    private static readonly HashSet<string> usedWords = new HashSet<string>();
    private static string initialWord;
    private const int MinWordLength = 8;
    private const int MaxWordLength = 30;
    private static string language = "ru";

    static void Main(string[] args)
    {
        ChooseLanguage();
        PlayGame();
    }

    private static void ChooseLanguage()
    {
        Console.WriteLine("Выберите язык/Choose language:");
        Console.WriteLine("1 - русский");
        Console.WriteLine("2 - english");

        string choice = Console.ReadLine();
        language = choice == "2" ? "en" : "ru";
    }

    private static string GetLocalizedMessage(string ruMessage, string enMessage)
    {
        return language == "en" ? enMessage : ruMessage;
    }

    private static string GetUserInput(string prompt)
    {
        Console.WriteLine(prompt);
        return Console.ReadLine()?.Trim().ToLower();
    }

    private static void ShowMessage(string message)
    {
        Console.WriteLine($"\n{message}");
    }

    static string InputInitialWord()
    {
        while (true)
        {
            string prompt = GetLocalizedMessage(
                $"Введите начальное слово ({MinWordLength}-{MaxWordLength} букв):",
                $"Enter initial word ({MinWordLength}-{MaxWordLength} letters):");

            string word = GetUserInput(prompt);

            if (word.Length < MinWordLength || word.Length > MaxWordLength)
            {
                ShowMessage(GetLocalizedMessage(
                    $"Слово должно быть от {MinWordLength} до {MaxWordLength} символов!",
                    $"Word must be between {MinWordLength} and {MaxWordLength} characters!"));
                continue;
            }

            if (!word.All(char.IsLetter))
            {
                ShowMessage(GetLocalizedMessage(
                    "Слово должно содержать только буквы!",
                    "Word must contain only letters!"));
                continue;
            }

            return word;
        }
    }

    static void PlayGame()
    {
        initialWord = InputInitialWord();
        availableLetters = initialWord.ToCharArray();
        bool gameOver = false;
        int currentPlayer = 1;
        var timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);

        ShowMessage(GetLocalizedMessage(
            $"Начальное слово: {initialWord}",
            $"Initial word: {initialWord}"));

        while (!gameOver)
        {
            string input = GetUserInput(GetLocalizedMessage(
                $"Игрок {currentPlayer}, ваш ход. Введите слово:",
                $"Player {currentPlayer}, your turn. Enter a word:"));

            timer.Change(30000, Timeout.Infinite);
            timer.Change(Timeout.Infinite, Timeout.Infinite);

            if (string.IsNullOrEmpty(input))
            {
                gameOver = true;
                ShowMessage(GetLocalizedMessage(
                    $"Игрок {currentPlayer} не ввел слово и проиграл!",
                    $"Player {currentPlayer} didn't enter a word and lost!"));
                break;
            }

            if (ValidateWord(input))
            {
                usedWords.Add(input);
                SubtractLetters(input);
                currentPlayer = currentPlayer == 1 ? 2 : 1;
            }
            else
            {
                gameOver = true;
                ShowMessage(GetLocalizedMessage(
                    $"Игрок {currentPlayer} ввел недопустимое слово и проиграл!",
                    $"Player {currentPlayer} entered an invalid word and lost!"));
            }
        }
    }

    static bool ValidateWord(string word)
    {
        if (word == initialWord)
        {
            ShowMessage(GetLocalizedMessage(
                "Нельзя использовать начальное слово!",
                "You can't use the initial word!"));
            return false;
        }

        if (usedWords.Contains(word))
        {
            ShowMessage(GetLocalizedMessage(
                "Это слово уже было использовано!",
                "This word has already been used!"));
            return false;
        }

        var tempLetters = availableLetters.ToArray();
        foreach (char c in word)
        {
            int index = Array.IndexOf(tempLetters, c);
            if (index == -1)
            {
                ShowMessage(GetLocalizedMessage(
                    $"Буква '{c}' недоступна!",
                    $"Letter '{c}' is not available!"));
                return false;
            }
            tempLetters[index] = '\0';
        }

        return true;
    }

    static void SubtractLetters(string word)
    {
        foreach (char c in word)
        {
            int index = Array.IndexOf(availableLetters, c);
            if (index != -1)
            {
                availableLetters[index] = '\0';
            }
        }
    }

    static void TimerCallback(object state)
    {
        ShowMessage(GetLocalizedMessage(
            "Время вышло! Нажмите Enter для продолжения...",
            "Time's up! Press Enter to continue..."));
    }
}