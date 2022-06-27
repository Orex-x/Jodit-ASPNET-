using System.Collections.Generic;
using Jodit.Models;

namespace Jodit.ViewModels;

public class ListScheduleStatementViewModel
{
    public List<ScheduleStatement> List { get; set; }
    public int GroupId { get; set; }
}