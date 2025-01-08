using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;
//using MauiApp90.Resources;



namespace MauiApp90
{
    public partial class MainPage : ContentPage
    {
        
        int count = 0;
        private bool isMusicPlaying = false;
        private readonly IAudioManager audioManager;
        private IAudioPlayer backgroundPlayer;
        bool isBackgroundMusicOn = true; // Initial state: Music OFF

        public MainPage(IAudioManager audioManager)
        {
            InitializeComponent();
            InitializeApp();
           
            this.audioManager = audioManager;

    }

        //all loading page logic
        private async void InitializeApp()
        {
            //circle
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            //percent
            double totalDuration = 3000; // 3 seconds for loading
            double interval = 100; // Update every 100ms (~10 updates per second)
            double steps = totalDuration / interval;

            //simulate loading process
            for (int i = 0; i <= steps; i++)
            {
                double percentage = (i / steps) * 100; // Calculate percentage
               PercentageLabel.Text = $"{percentage:0}%"; // Update the label text with the percentage
                await Task.Delay((int)interval); // Delay between updates
            }

            // Fade out the black overlay
            await LoadingImage.FadeTo(0, 500, Easing.Linear);
            LoadingImage.IsVisible = false; // Set the black overlay to invisible after fade out

            // Hide the loading indicator after loading completes
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;

            //hiding the loading percentage after its completed
            PercentageLabel.IsVisible = false;

            // Show main content and fade it in
            MainContent.IsVisible = true;
            await MainContent.FadeTo(1, 500, Easing.Linear);

            SettingsButton.IsVisible = true;

            //avengers theme song after loading is done
            PlayBackgroundMusic();
        }

        //method for loading music
        private async void PlayBackgroundMusic()
        {
            if (!isMusicPlaying)
            {
                try
                {
                    var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("TheThemeT.mp3"));
                    player.Play();
                    isMusicPlaying = true; // Set the flag to true to indicate the music is playing
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error playing music: {ex.Message}");
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            backgroundPlayer?.Stop();
        }



        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeApp();  // Call the initialization logic
        }


        //StartGame function
        private async void OnStartGameClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StartPage());

        }
       

        
        //hovering over Start button
        private async void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("hover.mp3"));
            player.Play();


            var button = sender as Button;
            button.BorderColor = Colors.White;
            button.BorderWidth = 3;
            await button.FadeTo(1, 300);
        }

        //hovering of start button
        private async void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
        {
            var button = sender as Button;
            button.BorderColor = Colors.Transparent;
            button.BorderWidth = 2;  // Revert to thinner transparent border
            await button.FadeTo(0.7, 300); // Optional fade-out effect
        }

        //settings button
        private async void SettingsButton_Clicked(object sender, EventArgs e)
        {
            if (!SettingsTab.IsVisible)
            {
                SettingsTab.IsVisible = true;
                await SettingsTab.TranslateTo(0, 0, 250, Easing.SinInOut); // Slide in
            }
            else
            {
                await SettingsTab.TranslateTo(-300, 0, 250, Easing.SinInOut); // Slide out
                SettingsTab.IsVisible = false;
            }
        }
    



        //setting button hover effect
        private async void PointerGestureRecognizer_PointerEntered_2(object sender, PointerEventArgs e)
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("hover.mp3"));
            player.Play();


            SettingsButton.BorderColor = Colors.White;
            SettingsButton.BorderWidth = 3;  // Make the border more visible
            await SettingsButton.FadeTo(1, 300); // Fade-in effect (optional)
        }
        private async void PointerGestureRecognizer_PointerExited_2(object sender, PointerEventArgs e)
        {
            SettingsButton.BorderColor = Colors.Transparent;
            SettingsButton.BorderWidth = 2;  // Revert to thinner transparent border
            await SettingsButton.FadeTo(0.7, 300); // Optional fade-out effect
        }



        //backgroud music button effect on the menu slide
        private async void BackgroundMusicButton_Clicked(object sender, EventArgs e)
        {
            isBackgroundMusicOn = !isBackgroundMusicOn;

            var button = sender as Button;

            if (isBackgroundMusicOn)
            {
                // Update button text and styling
                button.Text = "Turn Background Music OFF";
                button.BackgroundColor = Color.FromArgb("#770000"); // Dark red
                await PlayBackgroundMusicAsync(); // Play the music
            }
            else
            {
                // Update button text and styling
                button.Text = "Turn Background Music ON";
                button.BackgroundColor = Color.FromArgb("#550000"); // Even darker red
                StopBackgroundMusic(); // Stop the music
            }
        }

        //function to stop / start and change visuals
        private async Task PlayBackgroundMusicAsync()
        {
            // Code to play music
            Console.WriteLine("Background music playing...");
            await Task.CompletedTask;
        }

        // Example method to stop background music
        private void StopBackgroundMusic()
        {

            
        }


        //intruction button
        private async void InstructionButton_Clicked(object sender, EventArgs e)
        {
            // Navigate to the InstructionsPage
            await Navigation.PushAsync(new InstructionsPage());
        }
    }
    }
   

