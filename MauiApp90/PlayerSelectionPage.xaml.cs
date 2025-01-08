using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;
using System;

namespace MauiApp90
{
    public partial class PlayerSelectionPage : ContentPage
    {
        public PlayerSelectionPage()
        {
            InitializeComponent();
        }

        // Event handler for Picker selection change
        private void OnPlayerCountChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            var selectedPlayerCount = picker.SelectedIndex;

            if (selectedPlayerCount != -1)
            {
                // Display the selected number of players for feedback
                DisplayAlert("Player Count", $"You selected {picker.Items[selectedPlayerCount]}", "OK");
            }
        }

        // Event handler for the "Next" button
        private async void OnNextClicked(object sender, EventArgs e)
        {
            if (PlayerCountPicker.SelectedIndex != -1)
            {
                // Extract the number from the selected item (e.g., "2 Players" -> 2)
                string selectedItem = PlayerCountPicker.SelectedItem.ToString();
                int playerCount = int.Parse(selectedItem.Split(' ')[0]);  // Split and get the first part

                // Display the selected player count (for debugging)
                await DisplayAlert("Player Count", $"Game will be played with {playerCount} players.", "OK");
                var audioManager = DependencyService.Get<IAudioManager>();
                // Pass the player count to the GamePage via its constructor
                await Navigation.PushAsync(new GamePage(playerCount, audioManager));  // Pass playerCount to GamePage
            }
            else
            {
                await DisplayAlert("Error", "Please select the number of players first.", "OK");
            }
        }
    }
}




