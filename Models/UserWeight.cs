﻿using SQLite;
using LocalLiftLog.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class UserWeight
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int BodyWeight { get; set; }
        public string DateTime { get; set; }

        public bool ValidateBodyWeight()
        {
            if (BodyWeight < ConstantsHelper.BodyWeightMinValue || BodyWeight > ConstantsHelper.BodyWeightMaxValue) return false;

            return true;
        }
    }
}
