
using Plugin.Maui.Audio;

namespace MauiApp90
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            MainPage = new NavigationPage(serviceProvider.GetRequiredService<MainPage>());
   
           //
           //MainPage = new NavigationPage(new StartPage());  // Wrap StartPage in a NavigationPage
            

            DependencyService.Register<IAudioManager, AudioManager>();
        }
    }
}

