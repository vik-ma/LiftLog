﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Models
{
    public class WorkoutTemplateList
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int WorkoutTemplateId { get; set; }
        public int ScheduleId { get; set; }
    }

}