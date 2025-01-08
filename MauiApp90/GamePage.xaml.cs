using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;
using System.Text;
using Microsoft.Maui.Graphics;

namespace MauiApp90
{
    public partial class GamePage : ContentPage
    {
        private List<string> letterCombinations = new List<string> { "ar", "ex", "ing", "ly", "un", "in" };


        private int timeLeft = 30; // Start at 30 seconds for each turn
        private const int minTime = 5; // Minimum time allowed
        private const int timeReduction = 2; // Time to reduce after each correct guess

        private System.Timers.Timer timer;

        private Random random = new Random();
        private string currentLetters;
        private List<string> validWords = new List<string>(); // List of valid words
        private List<string> guessedWords = new List<string>();



        private int playerCount; // The number of players
        private int[] playerLives;
        private int[] playerScores;
        private int currentPlayerIndex = 0; // Track which player's turn it is
        private bool gameOver = false;


        private readonly IAudioManager audioManager;
        private IAudioPlayer tickPlayer;


        public GamePage(int playercount, IAudioManager audioManager)
        {
            InitializeComponent();
            playerCount = playercount;
            playerLives = new int[playerCount];
            playerScores = new int[playerCount];
            this.audioManager = audioManager;

            // Initialize lives and scores
            for (int i = 0; i < playerCount; i++)
            {
                playerLives[i] = 3; // Each player starts with 3 lives
                playerScores[i] = 0; // Each player starts with 0 score
            }

            LoadValidWords(); // Load words when the page is initialized
            StartGame();      // Start the game

            // Hide Player 2 UI components if only 1 player is selected
            if (playerCount == 1)
            {
                Player2LivesLabel.IsVisible = false;
                Player2ScoreLabel.IsVisible = false;
            }
        }

        // Method to load valid words from the embedded text file
        private void LoadValidWords()
        {
            try
            {
                var assembly = typeof(GamePage).Assembly;
                var resourceName = "MauiApp90.maui.txt"; // Ensure this is the correct resource name

                // Read the embedded file
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    // Split the content by new lines and trim spaces
                    validWords = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(word => word.Trim().ToLower())
                                         .ToList();
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load valid words file: " + ex.Message, "OK");
            }
        }

        // Start a new game
        private void StartGame()
        {
            if (gameOver) return;

            currentLetters = letterCombinations[random.Next(letterCombinations.Count)];

            // Update LetterLabel safely on the main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LetterLabel.Text = currentLetters;
                 PlayerWordLabel.IsVisible = true;
            });

            StartTimer(); // Start the countdown timer

            // Update the UI with current lives and scores
            UpdateUI();
        }

        // Start the timer
        private void StartTimer()
        {
            // If it's the first time or after the bomb explodes, set a random timer value between 10 and 30 seconds
            if (timeLeft == 0)
            {
                timeLeft = random.Next(10, 31); // Random time between 10 and 30 seconds
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerLabel.Text = $"{timeLeft}";
            });

            // Create the timer if it's not already created or dispose of the old one
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }

            // Create a new timer that ticks every second
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }



        private async void PlayTickSound()
        {
            try
            {
                tickPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("ticking.mp3"));
                tickPlayer.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing tick sound: {ex.Message}");
            }
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (timeLeft > 0 && !gameOver)
            {
                timeLeft--; // Decrease the timer

                // Stop the previous ticking sound if it exists
                if (tickPlayer != null)
                {
                    tickPlayer.Stop();
                    tickPlayer.Dispose(); // Dispose the old player to release resources
                }

                // Play the ticking sound
                tickPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("ticking.mp3"));
                tickPlayer.Play();

                // Safely update the TimerLabel on the main thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TimerLabel.Text = $"{timeLeft}";
                });
            }
            else if (timeLeft == 0 && !gameOver)
            {
                // Timer reached 0 (bomb exploded), stop and dispose of the old timer
                timer.Stop();
                timer.Dispose();
                timer = null;

                // Reduce player lives
                playerLives[currentPlayerIndex]--;

                // Play the life lost sound
                await PlayLifeLostSound();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Update the UI with current player lives
                    UpdateUI();
                    WordEntry.IsEnabled = false;
                    WordEntry.Text = string.Empty;

                    WordEntry.IsEnabled = (currentPlayerIndex == 0 || currentPlayerIndex == 1);
                });

                // Check if game is over
                if (playerLives[currentPlayerIndex] == 0)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Game Over", "One of the players has lost all lives!", "OK");
                        EndGame();
                    });
                }
                else
                {
                    // Switch turns and reset the game
                    SwitchPlayer();
                    StartGame(); // Start a new round with a new random timer value
                }
            
        
        


    }
}

        // Update the UI for lives and scores dynamically based on player count
        private void UpdateUI()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Update lives and scores for each player
                for (int i = 0; i < playerCount; i++)
                {
                    var playerLivesLabel = (Label)this.FindByName($"Player{i + 1}LivesLabel");
                    var playerScoreLabel = (Label)this.FindByName($"Player{i + 1}ScoreLabel");
                    var livesString = string.Concat(Enumerable.Repeat("❤️", playerLives[i]));
                    playerLivesLabel.Text = livesString;
                    playerScoreLabel.Text = $"Player {i + 1} Score: {playerScores[i]}";
                }
            });
        }

        // End the game
        private void EndGame()
        {
            tickPlayer.Stop();
            gameOver = true;
            DisplayAlert("Game Over", $"Final Score: {string.Join(", ", playerScores.Select((score, index) => $"Player {index + 1}: {score}"))}", "OK");
        }

        // Switch player after each round
        private void SwitchPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % playerCount;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Clear the WordEntry field
                WordEntry.Text = string.Empty;

                // Update lives and scores
                UpdateUI();

                // Update PlayerWordLabel text to reflect the current player
                PlayerWordLabel.Text = " ";

                // Switch the position of PlayerWordLabel based on whose turn it is
                if (currentPlayerIndex == 0)
                {
                    // Set Player 1's Word Label position (to the left side)
                    PlayerWordLabel.Margin = new Thickness(0, 10, 700, 0); // Similar to Player 1's setup
                    PlayerWordLabel.HorizontalTextAlignment = TextAlignment.Start; // Align text to the start
                }
                else
                {
                    // Set Player 2's Word Label position (to the right side)
                    PlayerWordLabel.Margin = new Thickness(700,10,0,0); // Similar to Player 2's setup
                    PlayerWordLabel.HorizontalTextAlignment = TextAlignment.End; // Align text to the end
                }

                // Ensure the label is visible
                PlayerWordLabel.IsVisible = true;
            });
        }

        // Handle the word entry and check if it's correct
        private async void WordEntry_Completed(object sender, EventArgs e)
        {
            string userWord = WordEntry.Text?.Trim().ToLower();

            if (string.IsNullOrEmpty(userWord))
            {
                await DisplayAlert("Invalid Input", "Please enter a word.", "OK");
                return;
            }

            // Check if the word has already been guessed
            if (guessedWords.Contains(userWord))
            {
                await DisplayAlert("Duplicate Word", "This word has already been guessed. Try a new word!", "OK");
                WordEntry.Text = string.Empty; // Clear the input field
                return;
            }

            // Check if the word is valid
            if (validWords.Contains(userWord))
            {
                // Increase score depending on whose turn it is
                playerScores[currentPlayerIndex]++;

                // Add the word to the guessed words list
                guessedWords.Add(userWord);

                // Play correct answer sound
                await PlayCorrectSound();

                // If the time left is below the minimum (5 seconds), reset it to the minimum time
                if (timeLeft < minTime)
                {
                    timeLeft = minTime;
                }
                else
                {
                    // Otherwise, reduce time, but ensure it doesn't go below the minimum time
                    timeLeft = Math.Max(minTime, timeLeft - timeReduction);
                }

                // Update the UI
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateUI();
                });

                // Switch turn after the guess
                SwitchPlayer();

                // Start a new round with the updated time
                StartGame();
            }
            else
            {
                await DisplayAlert("Invalid Word", "The word is not valid or didn't match the letters.", "OK");
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                WordEntry.Text = string.Empty; // Ensuring thread safety
            });
        }



        private async Task PlayCorrectSound()
        {
            try
            {
                var correctPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("correct.mp3"));
                correctPlayer.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing correct sound: {ex.Message}");
            }
        }

        private void WordEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string typedWord = e.NewTextValue?.Trim();

            if (!string.IsNullOrEmpty(typedWord))
            {
                var formattedText = new FormattedString();

                // Only check against the currentLetters combination
                foreach (var letter in typedWord)
                {
                    bool isMatch = false;

                    // Check if the letter matches any letter in the current combination
                    if (currentLetters.Contains(letter.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        // Highlight the matching letter in green
                        formattedText.Spans.Add(new Span
                        {
                            Text = letter.ToString(),
                            TextColor = Colors.Green // Correct property for color
                        });
                        isMatch = true;
                    }

                    // If it's not a match, add the letter with default color
                    if (!isMatch)
                    {
                        formattedText.Spans.Add(new Span
                        {
                            Text = letter.ToString(),
                            TextColor = Colors.Black // Correct property for color
                        });
                    }
                }

                // Set the formatted text to the label
                PlayerWordLabel.FormattedText = formattedText;
            }
            else
            {
                // If no text, clear the label
                PlayerWordLabel.FormattedText = null;
            }

        }

        private async Task PlayLifeLostSound()
        {
            try
            {
                // Play the sound when the player loses a life
                var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("lose_life.mp3"));
                player.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing life lost sound: {ex.Message}");
            }
        }



    }
}

