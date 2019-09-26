
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.FindACourse.Interfaces;


namespace Dfc.ProviderPortal.FindACourse.Models
{
    public class CourseDetailResult : ICourseDetailResult
    {
        public Course Course { get; set; }
        public Provider Provider { get; set; }
        public Venue Venue { get; set; }
        public Qualification Qualification { get; set; }
    }
}
