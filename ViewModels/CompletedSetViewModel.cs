using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using LocalLiftLog.Helpers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq.Expressions;

namespace LocalLiftLog.ViewModels
{
    public partial class CompletedSetViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;
        public CompletedSetViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<CompletedSet> completedSetList = new();

        public async Task LoadCompletedSetsAsync()
        {
            await ExecuteAsync(async () =>
            {
                CompletedSetList.Clear();

                var completedSets = await _context.GetAllAsync<CompletedSet>();

                if (completedSets is not null && completedSets.Any())
                {
                    completedSets ??= new ObservableCollection<CompletedSet>();

                    foreach (var set in completedSets)
                    {
                        CompletedSetList.Add(set);
                    }
                }
            });
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
        private async Task CreateCompletedSetAsync()
        {
            await ExecuteAsync(async () =>
            {
                CompletedSet set = new();
                await _context.AddItemAsync<CompletedSet>(set);
                CompletedSetList.Add(set);
            });
        }

        [RelayCommand]
        private async Task UpdateCompletedSetAsync(int id)
        {
            CompletedSet completedSet = CompletedSetList.FirstOrDefault(p => p.Id == id);

            if (completedSet is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<CompletedSet>(completedSet))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Completed Set.", "OK");
                }

                await LoadCompletedSetsAsync();
            });
        }

        [RelayCommand]
        private async Task DeleteCompletedSetAsync(int id)
        {
            CompletedSet completedSet = CompletedSetList.FirstOrDefault(p => p.Id == id);

            if (completedSet is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<CompletedSet>(completedSet))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Completed Set.", "OK");
                }
            });

            await LoadCompletedSetsAsync();
        }

        [RelayCommand]
        private async Task MarkCompletedSetAsComplete(int id)
        {
            CompletedSet completedSet = CompletedSetList.FirstOrDefault(p => p.Id == id);

            if (completedSet is null)
                return;

            string currentDateTimeString = DateTimeHelper.GetCurrentFormattedDateTime();

            completedSet.IsCompleted = true;
            completedSet.TimeCompleted = currentDateTimeString;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<CompletedSet>(completedSet))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Completed Set.", "OK");
                }

                await LoadCompletedSetsAsync();
            });
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
