using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalLiftLog.Data;
using LocalLiftLog.Models;
using LocalLiftLog.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.ViewModels
{
    public partial class WeeklyScheduleViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private Dictionary<int, string> daysOfWeek;

        [ObservableProperty]
        private ObservableCollection<WeeklySchedule> weeklySchedule;


        public WeeklyScheduleViewModel(DatabaseContext context)
        {
            _context = context;

            DaysOfWeek = new Dictionary<int, string>
                {
                    { 1, "Monday" },
                    { 2, "Tuesday" },
                    { 3, "Wednesday" },
                    { 4, "Thursday" },
                    { 5, "Friday" },
                    { 6, "Saturday" },
                    { 7, "Sunday" }
                };
        }



        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
