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
    [QueryProperty(nameof(SetTemplateCollection), nameof(SetTemplateCollection))]
    public partial class SetListDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private SetTemplateViewModel _setTemplateViewModel;

        private readonly DatabaseContext _context;

        [ObservableProperty]
        private SetTemplateCollection setTemplateCollection;

        public SetListDetailsViewModel(SetTemplateViewModel setTemplateViewModel, DatabaseContext context)
        {
            _setTemplateViewModel = setTemplateViewModel;
            _context = context;
        }

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
