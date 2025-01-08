    using Plugin.Maui.Audio;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    namespace MauiApp90
    {
        public partial class InstructionsPage : ContentPage
        {
            // Instructions array
            private List<string> instructions = new List<string>
            {
                "    Players take turns guessing words from the displayed letters.",
                "   Type a word and submit it. If it's valid, you score a point..",
                "   The timer counts down each round—if it hits zero, you lose a life.",
                "   The game ends when a player runs out of lives. The last player standing wins!"
            };

            // Audio manager and player variables
            private readonly IAudioManager audioManager;
            private IAudioPlayer nextButtonPlayer;

            // Index to track the current step
            private int currentStep = 0;

            public InstructionsPage()
            {
                InitializeComponent();
                this.audioManager = DependencyService.Get<IAudioManager>(); // Ensure you're properly initializing audioManager
            }

            private async void NextButton_Clicked(object sender, EventArgs e)
            {
                try
                {
                    // Play the hover effect sound when the Next button is clicked
                    var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("hover.mp3"));
                    player.Play();

                    // Logic to navigate or show the next instruction
                    Debug.WriteLine("Next button clicked!");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error playing sound: {ex.Message}");
                }

                currentStep++;

                // If there are more instructions, update the label, otherwise show the last instruction or hide button
                if (currentStep < instructions.Count)
                {
                    InstructionLabel.Text = instructions[currentStep];
                }
                else
                {
                    InstructionLabel.Text = "You're ready to start!";
                    (sender as Button).IsVisible = false;
                }
            }

            // Method for handling the "HOME" button click (if you want to navigate back)
            private async void OnBackclicked(object sender, EventArgs e)
            {
                await Navigation.PopAsync();
            }
        }
    }
