using CommunityToolkit.Maui;
using LocalLiftLog.Data;
using LocalLiftLog.Pages;
using LocalLiftLog.ViewModels;
using Microsoft.Extensions.Logging;

namespace LocalLiftLog
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DatabaseContext>();
            builder.Services.AddSingleton<RoutineListViewModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<RoutineDetailsPage>();
            builder.Services.AddTransient<RoutineDetailsViewModel>();

            return builder.Build();
        }
    }
}