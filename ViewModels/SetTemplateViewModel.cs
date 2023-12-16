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
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
