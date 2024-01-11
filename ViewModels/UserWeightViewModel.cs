using CommunityToolkit.Mvvm.ComponentModel;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using LocalLiftLog.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace LocalLiftLog.ViewModels
{
    public partial class UserWeightViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public UserWeightViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        public ObservableCollection<UserWeight> userWeightList = new();

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

        public async Task LoadUserWeightListAsync()
        {
            await ExecuteAsync(async () =>
            {
                UserWeightList.Clear();

                var weightList = await _context.GetAllAsync<UserWeight>();

                if (weightList is not null && weightList.Any())
                {
                    weightList ??= new ObservableCollection<UserWeight>();

                    foreach (var set in weightList)
                    {
                        UserWeightList.Add(set);
                    }
                }
            });
        }
    }
}
