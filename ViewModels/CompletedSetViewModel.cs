using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
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

        [ObservableProperty]
        private ObservableCollection<CompletedSetCollection> completedSetCollectionList = new();

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

        public async Task LoadCompletedSetCollectionsAsync()
        {
            await ExecuteAsync(async () =>
            {
                CompletedSetCollectionList.Clear();

                var completedSetsCollections = await _context.GetAllAsync<CompletedSetCollection>();

                if (completedSetsCollections is not null && completedSetsCollections.Any())
                {
                    completedSetsCollections ??= new ObservableCollection<CompletedSetCollection>();

                    foreach (var setCollection in completedSetsCollections)
                    {
                        CompletedSetCollectionList.Add(setCollection);
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
        private async Task CreateCompletedSetCollectionAsync()
        {
            await ExecuteAsync(async () =>
            {
                CompletedSetCollection setCollection = new();
                await _context.AddItemAsync<CompletedSetCollection>(setCollection);
                CompletedSetCollectionList.Add(setCollection);
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
        private async Task UpdateCompletedSetCollectionAsync(int id)
        {
            CompletedSetCollection completedSetCollection = CompletedSetCollectionList.FirstOrDefault(p => p.Id == id);

            if (completedSetCollection is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<CompletedSetCollection>(completedSetCollection))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Completed Set Collection.", "OK");
                }

                await LoadCompletedSetCollectionsAsync();
            });
        }

        [RelayCommand]
        private async Task DeleteCompletedSetCollectionAsync(int id)
        {
            CompletedSetCollection completedSetCollection = CompletedSetCollectionList.FirstOrDefault(p => p.Id == id);

            if (completedSetCollection is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<CompletedSetCollection>(completedSetCollection))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Completed Set Collection.", "OK");
                }
            });

            await LoadCompletedSetCollectionsAsync();

            await DeleteCompletedSetsByCompletedSetCollectionId(id);
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
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Completed Set.", "OK");
                }
            });

            await LoadCompletedSetsAsync();
        }

        private async Task DeleteCompletedSetsByCompletedSetCollectionId(int id)
        {
            Expression<Func<CompletedSet, bool>> predicate = entity => entity.CompletedSetCollectionId == id;

            IEnumerable<CompletedSet> filteredList = null;
            try
            {
                filteredList = await _context.GetFilteredAsync<CompletedSet>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Set Templates.", "OK");
            }

            foreach (var item in filteredList)
            {
                await ExecuteAsync(async () =>
                {
                    if (!await _context.DeleteItemAsync<CompletedSet>(item))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when deleting Set Template.", "OK");
                    }
                });
            }

            await LoadCompletedSetsAsync();
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
