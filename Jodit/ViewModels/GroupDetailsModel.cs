using System.Collections.Generic;
using Jodit.Models;

namespace Jodit.ViewModels
{
    public class GroupDetailsModel
    {
        public UserGroup UserGroup { get; set; }

        public List<ScheduleChange> ScheduleChanges { get; set; }
    }
}