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
    [QueryProperty(nameof(WeeklySchedule), nameof(WeeklySchedule))]
    public partial class WeeklyScheduleViewModel : ObservableObject
    {
        private readonly DatabaseContext _context;

        [ObservableProperty]
        private WeeklySchedule weeklySchedule;

        private readonly string[] validDayList = { "Day1", "Day2", "Day3", "Day4", "Day5", "Day6", "Day7" };

        public WeeklyScheduleViewModel(DatabaseContext context)
        {
            _context = context;
        }

        [ObservableProperty]
        private int[] _day1TemplateIdList;

        [ObservableProperty]
        private int[] _day2TemplateIdList;

        [ObservableProperty]
        private int[] _day3TemplateIdList;

        [ObservableProperty]
        private int[] _day4TemplateIdList;

        [ObservableProperty]
        private int[] _day5TemplateIdList;

        [ObservableProperty]
        private int[] _day6TemplateIdList;

        [ObservableProperty]
        private int[] _day7TemplateIdList;

        

        

        

        [RelayCommand]
        static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public void LoadDayTemplateIdList()
        {
            Day1TemplateIdList = WeeklySchedule.GetDayTemplateIdIntArray("Day1");
            Day2TemplateIdList = WeeklySchedule.GetDayTemplateIdIntArray("Day2");
            Day3TemplateIdList = WeeklySchedule.GetDayTemplateIdIntArray("Day3");
            Day4TemplateIdList = WeeklySchedule.GetDayTemplateIdIntArray("Day4");
            Day5TemplateIdList = WeeklySchedule.GetDayTemplateIdIntArray("Day5");
            Day6TemplateIdList = WeeklySchedule.GetDayTemplateIdIntArray("Day6");
            Day7TemplateIdList = WeeklySchedule.GetDayTemplateIdIntArray("Day7");
        }

        [RelayCommand]
        private async Task Test()
        {
            string test = WeeklySchedule.ValidateTemplateIdInput("asd").ToString();
            await Shell.Current.DisplayAlert("Error", test, "OK");
        }

        [RelayCommand]
        private async Task AddTemplateIdToDay(string day)
        {
            if (validDayList.Contains(day))
            {
                int[] templateIdArray = WeeklySchedule.GetDayTemplateIdIntArray(day);

                string templateIdString;

                if (templateIdArray.Length > 0)
                {
                    templateIdString = string.Join(", ", templateIdArray);
                }
                else
                {
                    templateIdString = "Empty";
                }
                
                await Shell.Current.DisplayAlert("Success", templateIdString, "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Day", "OK");
            }
        }

    }
}
