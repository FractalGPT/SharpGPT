using System.ComponentModel;

namespace FractalGPT.SharpGPTLib.Clients.Tavily.Models;

public enum TimeRange
{
    [Description(null)]
    All = 1,

    [Description("day")]
    Day = 2,

    [Description("week")]
    Week = 3,

    [Description("month")]
    Month = 4,

    [Description("year")]
    Year = 5,
}
