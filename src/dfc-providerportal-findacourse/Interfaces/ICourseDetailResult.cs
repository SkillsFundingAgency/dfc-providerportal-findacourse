
using System;
using Dfc.ProviderPortal.FindACourse.Models;


namespace Dfc.ProviderPortal.FindACourse.Interfaces
{
    public interface ICourseDetailResult
    {
        Course Course { get; set; }
        Provider Provider { get; set; }
        Venue Venue { get; set; }
        Qualification Qualification { get; set; }
    }
}
