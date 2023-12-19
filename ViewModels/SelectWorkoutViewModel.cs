using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    public partial class SelectWorkoutViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        public SelectWorkoutViewModel(DatabaseContext context)
        {
            _context = context;
        }

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
    }
}
