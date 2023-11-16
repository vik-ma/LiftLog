﻿using CommunityToolkit.Maui;
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

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            builder.Services.AddTransient<ProgramListPage>();
            builder.Services.AddTransient<ProgramListViewModel>();

            builder.Services.AddTransient<ProgramDetailsPage>();
            builder.Services.AddTransient<ProgramDetailsViewModel>();

            builder.Services.AddTransient<ScheduleListPage>();
            builder.Services.AddTransient<ScheduleListViewModel>();

            builder.Services.AddTransient<WeeklySchedulePage>();
            builder.Services.AddTransient<WeeklyScheduleViewModel>();

            builder.Services.AddTransient<CustomSchedulePage>();
            builder.Services.AddTransient<CustomScheduleViewModel>();

            builder.Services.AddTransient<WorkoutTemplateListPage>();
            builder.Services.AddTransient<WorkoutTemplateViewModel>();

            builder.Services.AddTransient<SetTemplateListPage>();
            builder.Services.AddTransient<SetTemplateViewModel>();

            return builder.Build();
        }
    }
}