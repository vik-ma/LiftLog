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
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
