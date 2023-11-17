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
    public partial class SetTemplateViewModel : ObservableObject 
    {
        private readonly DatabaseContext _context;

        public SetTemplateViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private ObservableCollection<SetTemplate> setTemplateList = new();

        [ObservableProperty]
        private ObservableCollection<SetTemplateCollection> setTemplateCollectionList = new();

        public async Task LoadSetTemplatesAsync()
        {
            await ExecuteAsync(async () =>
            {
                SetTemplateList.Clear();

                var setTemplates = await _context.GetAllAsync<SetTemplate>();

                if (setTemplates is not null && setTemplates.Any())
                {
                    setTemplates ??= new ObservableCollection<SetTemplate>();

                    foreach (var set in setTemplates)
                    {
                        SetTemplateList.Add(set);
                    }
                }
            });
        }

        public async Task LoadSetTemplateCollectionsAsync()
        {
            await ExecuteAsync(async () =>
            {
                SetTemplateCollectionList.Clear();

                var setTemplatesCollections = await _context.GetAllAsync<SetTemplateCollection>();

                if (setTemplatesCollections is not null && setTemplatesCollections.Any())
                {
                    setTemplatesCollections ??= new ObservableCollection<SetTemplateCollection>();

                    foreach (var setCollection in setTemplatesCollections)
                    {
                        SetTemplateCollectionList.Add(setCollection);
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
        private async Task CreateSetTemplateAsync()
        {
            await ExecuteAsync(async () =>
            {
                SetTemplate set = new();
                await _context.AddItemAsync<SetTemplate>(set);
                SetTemplateList.Add(set);
            });
        }

        [RelayCommand]
        private async Task CreateSetTemplateCollectionAsync()
        {
            await ExecuteAsync(async () =>
            {
                SetTemplateCollection setCollection = new();
                await _context.AddItemAsync<SetTemplateCollection>(setCollection);
                SetTemplateCollectionList.Add(setCollection);
            });
        }

        [RelayCommand]
        private async Task UpdateSetTemplateAsync(int id)
        {
            SetTemplate setTemplate = SetTemplateList.FirstOrDefault(p => p.Id == id);

            if (setTemplate is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<SetTemplate>(setTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Set Template.", "OK");
                }

                await LoadSetTemplatesAsync();
            });
        }

        [RelayCommand]
        private async Task UpdateSetTemplateCollectionAsync(int id)
        {
            SetTemplateCollection setTemplateCollection = SetTemplateCollectionList.FirstOrDefault(p => p.Id == id);

            if (setTemplateCollection is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.UpdateItemAsync<SetTemplateCollection>(setTemplateCollection))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when updating Set Template Collection.", "OK");
                }

                await LoadSetTemplateCollectionsAsync();
            });
        }

        [RelayCommand]
        private async Task DeleteSetTemplateCollectionAsync(int id)
        {
            SetTemplateCollection setTemplateCollection = SetTemplateCollectionList.FirstOrDefault(p => p.Id == id);

            if (setTemplateCollection is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<SetTemplateCollection>(setTemplateCollection))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Set Template Collection.", "OK");
                }
            });

            await LoadSetTemplateCollectionsAsync();
        }

        [RelayCommand]
        private async Task DeleteSetTemplateAsync(int id)
        {
            SetTemplate setTemplate = SetTemplateList.FirstOrDefault(p => p.Id == id);

            if (setTemplate is null)
                return;

            await ExecuteAsync(async () =>
            {
                if (!await _context.DeleteItemAsync<SetTemplate>(setTemplate))
                {
                    await Shell.Current.DisplayAlert("Error", "Error occured when deleting Set Template.", "OK");
                }
            });

            await LoadSetTemplatesAsync();

            await DeleteSetTemplateCollectionsBySetTemplateId(id);
        }

        private async Task DeleteSetTemplateCollectionsBySetTemplateId(int id)
        {
            Expression<Func<SetTemplateCollection, bool>> predicate = entity => entity.SetTemplateId == id;

            IEnumerable<SetTemplateCollection> filteredWtcList = null;
            try
            {
                filteredWtcList = await _context.GetFilteredAsync<SetTemplateCollection>(predicate);
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occured when trying to load Set Template Collections.", "OK");
            }

            foreach (var item in filteredWtcList)
            {
                await ExecuteAsync(async () =>
                {
                    if (!await _context.DeleteItemAsync<SetTemplateCollection>(item))
                    {
                        await Shell.Current.DisplayAlert("Error", "Error occured when deleting Set Template Collection.", "OK");
                    }
                });
            }

            await LoadSetTemplateCollectionsAsync();
        }

        [RelayCommand]
        private async Task GoToSetListDetails(int id)
        {
            SetTemplateCollection setTemplateCollection = SetTemplateCollectionList.FirstOrDefault(p => p.Id == id);

            if (setTemplateCollection is null)
            {
                await Shell.Current.DisplayAlert("Error", "Set List does not exist", "OK");
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                ["SetTemplateCollection"] = setTemplateCollection
            };

            await Shell.Current.GoToAsync($"{nameof(SetListDetailsPage)}?Id={id}", navigationParameter);
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
