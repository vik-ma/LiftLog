using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public partial class SetPackage : ObservableObject
    {
        public SetTemplate SetTemplate { get; set; }
        public CompletedSet CompletedSet { get; set; }

        [ObservableProperty]
        public bool isEditingSetProperties;

        [ObservableProperty]
        public bool isSetCompleted;
    }
}
