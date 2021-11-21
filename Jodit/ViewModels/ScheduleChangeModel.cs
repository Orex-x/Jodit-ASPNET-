using System.Collections;
using Jodit.Models;

namespace Jodit.ViewModels
{
    public class ScheduleChangeModel
    {
        public ScheduleChange ScheduleChange { get; set; }
        
        public ArrayList DateTimes { get; set; }

        public int IdScheduleStatement { get; set; }
    }
}