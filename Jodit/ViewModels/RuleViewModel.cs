using System;
using System.Collections.Generic;

namespace Jodit.ViewModels
{
    public class RuleViewModel
    {
        public int IdUser { get; set; }
        
        public int IdGroup { get; set; }
        
        public Dictionary<string, Boolean> daysOfTheWeek { get; set; }
        
    }
}