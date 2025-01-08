using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace MauiApp90
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Anton-Regular.ttf", "AntonRegular");
                });

            builder.Services.AddSingleton<IAudioManager>(AudioManager.Current);
            builder.Services.AddTransient<MainPage>();
#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
