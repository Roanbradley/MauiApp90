using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace MauiApp90
{
    public partial class StartPage : ContentPage
    {
        private readonly List<(string ImageSource, string Difficulty)> _badGuyData;
        private int _currentIndex;

        public StartPage()
        {
            InitializeComponent();

            // Initialize bad guy data
            _badGuyData = new List<(string, string)>
            {
                ("venommmm.jpg", "Easy"),
                ("magnetooo.jpg", "Medium"),
                ("thanosss.jpg", "Hard")
            };

            // Set the default image and difficulty text
            _currentIndex = 0;
            UpdateDisplay();
        }

        private void LeftArrowButton_Clicked(object sender, EventArgs e)
        {
            // Navigate to the previous item
            _currentIndex = (_currentIndex - 1 + _badGuyData.Count) % _badGuyData.Count;
            UpdateDisplay();
        }

        private void RightArrowButton_Clicked(object sender, EventArgs e)
        {
            // Navigate to the next item
            _currentIndex = (_currentIndex + 1) % _badGuyData.Count;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            // Update the image and difficulty text
            BadGuyImage.Source = _badGuyData[_currentIndex].ImageSource;
            DifficultyLabel.Text = _badGuyData[_currentIndex].Difficulty;
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new PlayerSelectionPage());
        }
    }
}
