﻿
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Spatial;
using Dfc.ProviderPortal.FindACourse.Interfaces;


namespace Dfc.ProviderPortal.FindACourse.Models
{
    public class AzureSearchCourse : IAzureSearchCourse
    {
        public Guid? id { get; set; }
        public Guid? CourseId { get; set; }
        public string QualificationCourseTitle { get; set; }
        public string LearnAimRef { get; set; }
        public string NotionalNVQLevelv2 { get; set; }
        //        public string AwardOrgCode { get; set; }
        //        public string QualificationType { get; set; }
        //        public int ProviderUKPRN { get; set; }
        //        public string CourseDescription { get; set; }
        //        public string EntryRequirments { get; set; }
        //        public string WhatYoullLearn { get; set; }
        //        public string HowYoullLearn { get; set; }
        //        public string WhatYoullNeed { get; set; }
        //        public string HowYoullBeAssessed { get; set; }
        //        public string WhereNext { get; set; }
        //        public bool AdvancedLearnerLoan { get; set; }
        //        public DateTime CreatedDate { get; set; }
        //        public string CreatedBy { get; set; }
        //        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedOn { get; set; }
        //        public string UpdatedBy { get; set; }

        //        public IEnumerable<CourseRun> CourseRuns { get; set; }

        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public GeographyPoint VenueLocation { get; set; }
        public string VenueAttendancePattern { get; set; }
        public string ProviderName { get; set; }
        public string Region { get; set; }
        public int? Status { get; set; }
    }
}
