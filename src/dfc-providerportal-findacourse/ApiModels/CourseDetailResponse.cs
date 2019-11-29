using System;
using Dfc.ProviderPortal.FindACourse.Models;

namespace Dfc.ProviderPortal.FindACourse.ApiModels
{
    public class CourseDetailResponse
    {
        public Guid id { get; set; }
        public string QualificationCourseTitle { get; set; }
        public string LearnAimRef { get; set; }
        public string NotionalNVQLevelv2 { get; set; }
        public string AwardOrgCode { get; set; }
        public string QualificationType { get; set; }
        public string CourseDescription { get; set; }
        public string EntryRequirements { get; set; }
        public string WhatYoullLearn { get; set; }
        public string HowYoullLearn { get; set; }
        public string WhatYoullNeed { get; set; }
        public string HowYoullBeAssessed { get; set; }
        public string WhereNext { get; set; }
        public dynamic Provider { get; set; }
        public CourseRun CourseRun { get; set; }
        public object FEChoices { get; set; }
    }
}
