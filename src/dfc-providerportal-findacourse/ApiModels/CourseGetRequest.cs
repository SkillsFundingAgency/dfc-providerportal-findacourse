using System;

namespace Dfc.ProviderPortal.FindACourse.ApiModels
{
    public class CourseGetRequest
    {
        public Guid CourseId { get; set; }
        public Guid RunId { get; set; }
    }
}
