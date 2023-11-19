﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    [QueryProperty(nameof(WorkoutTemplate), nameof(WorkoutTemplate))]
    public partial class WorkoutDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private WorkoutTemplateViewModel _workoutTemplateViewModel;

        private readonly DatabaseContext _context;

        [ObservableProperty]
        private WorkoutTemplate workoutTemplate;

        public WorkoutDetailsViewModel(WorkoutTemplateViewModel workoutTemplateViewModel, DatabaseContext context)
        {
            _workoutTemplateViewModel = workoutTemplateViewModel;
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<SetTemplate> setList = new();

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        #nullable enable
        private async Task ExecuteAsync(Func<Task> operation)
        {
            try
            {
                #nullable disable
                await operation?.Invoke();
            }
            catch
            {

            }
            finally
            {

            }
        }

        [RelayCommand]
        private async Task CreateNewSetListAsync()
        {
            await ExecuteAsync(async () =>
            {
                SetTemplateCollection newSet = new();
                await _context.AddItemAsync<SetTemplateCollection>(newSet);
                WorkoutTemplate.SetTemplateCollectionId = newSet.Id;
                await _context.UpdateItemAsync<WorkoutTemplate>(WorkoutTemplate);
            });

            OnPropertyChanged(nameof(WorkoutTemplate));
        }

        [RelayCommand]
        public async Task LoadSetListFromSetTemplateCollectionIdAsync()
        {
            if (WorkoutTemplate is null) return;

            // Skip function if no SetTemplateCollectionId is set
            if (WorkoutTemplate.SetTemplateCollectionId == 0) return;

            SetList.Clear();

            Expression<Func<SetTemplate, bool>> predicate = entity => entity.SetTemplateCollectionId == WorkoutTemplate.SetTemplateCollectionId;

            // Check if a SetTemplateCollection with that Id exists
            await ExecuteAsync(async () =>
            {
                if (!await _context.ItemExistsByKeyAsync<SetTemplateCollection>(WorkoutTemplate.SetTemplateCollectionId))
                {
                    // Reset SetTemplateCollectionId value for current WorkoutTemplate
                    WorkoutTemplate.SetTemplateCollectionId = 0;

                    // Save the changes
                    if (!await _context.UpdateItemAsync<WorkoutTemplate>(WorkoutTemplate))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when updating Workout Template.", "OK");
                    }
                }
                OnPropertyChanged(nameof(WorkoutTemplate));
                return;
            });

            try
            {
                var filteredList = await _context.GetFilteredAsync<SetTemplate>(predicate);

                foreach (var item in filteredList)
                {
                    SetList.Add(item);
                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load workouts.", "OK");
                return;
            }
        }
    }
}