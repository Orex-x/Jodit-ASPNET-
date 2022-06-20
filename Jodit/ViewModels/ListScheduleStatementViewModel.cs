using System.Collections.Generic;
using Jodit.Models;

namespace Jodit.ViewModels;

public class ListScheduleStatementViewModel
{
    public List<ScheduleStatement> list { get; set; }
    public int groupId { get; set; }
}