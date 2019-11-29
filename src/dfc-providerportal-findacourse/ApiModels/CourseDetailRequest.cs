using System;

namespace Dfc.ProviderPortal.FindACourse.ApiModels
{
    public class CourseDetailRequest
    {
        public Guid CourseId { get; set; }
        public Guid CourseRunId { get; set; }
    }
}
