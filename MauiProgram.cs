﻿global using CommunityToolkit.Maui;
global using CommunityToolkit.Mvvm.ComponentModel;
global using System.Collections.ObjectModel;
global using CommunityToolkit.Mvvm.Input;
global using CommunityToolkit.Maui.Views;
global using SQLite;
global using System.Globalization;
global using System.Linq.Expressions;
global using LocalLiftLog.Data;
global using LocalLiftLog.Pages;
global using LocalLiftLog.Models;
global using LocalLiftLog.ViewModels;
global using LocalLiftLog.Helpers;
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
                    fonts.AddFont("Inter-Regular.ttf", "Inter");
                    fonts.AddFont("Inter-Medium.ttf", "InterMedium");
                    fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DatabaseContext>();
            builder.Services.AddSingleton<ExerciseDataManager>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            builder.Services.AddTransient<WorkoutRoutineListPage>();
            builder.Services.AddTransient<WorkoutRoutineListViewModel>();

            builder.Services.AddTransient<WorkoutRoutineDetailsPage>();
            builder.Services.AddTransient<WorkoutRoutineDetailsViewModel>();

            builder.Services.AddTransient<WorkoutTemplateListPage>();
            builder.Services.AddTransient<WorkoutTemplateViewModel>();

            builder.Services.AddTransient<WorkoutDetailsPage>();
            builder.Services.AddTransient<WorkoutDetailsViewModel>();

            builder.Services.AddTransient<SetListPage>();
            builder.Services.AddTransient<SetListViewModel>();

            builder.Services.AddTransient<TimePeriodListPage>();
            builder.Services.AddTransient<TimePeriodViewModel>();

            builder.Services.AddTransient<CustomExerciseListPage>();
            builder.Services.AddTransient<CustomExerciseViewModel>();

            builder.Services.AddTransient<ExerciseListPage>();
            builder.Services.AddTransient<ExerciseListViewModel>();

            builder.Services.AddTransient<CreateSetTemplatePage>();
            builder.Services.AddTransient<CreateSetTemplateViewModel>();

            builder.Services.AddTransient<UserPreferencesPage>();
            builder.Services.AddSingleton<UserPreferencesViewModel>();

            builder.Services.AddTransient<UserWeightPage>();
            builder.Services.AddTransient<UserWeightViewModel>();

            builder.Services.AddTransient<NewWorkoutPage>();
            builder.Services.AddTransient<NewWorkoutViewModel>();

            builder.Services.AddTransient<WorkoutPage>();
            builder.Services.AddSingleton<WorkoutViewModel>();
            builder.Services.AddTransient<WorkoutOperatingSetPage>();

            builder.Services.AddTransient<WorkoutListPage>();
            builder.Services.AddTransient<WorkoutListViewModel>();

            builder.Services.AddTransient<SchedulePage>();
            builder.Services.AddTransient<ScheduleViewModel>();

            builder.Services.AddTransient<ExerciseDetailsPage>();
            builder.Services.AddTransient<ExerciseDetailsViewModel>();


            return builder.Build();
        }
    }
}