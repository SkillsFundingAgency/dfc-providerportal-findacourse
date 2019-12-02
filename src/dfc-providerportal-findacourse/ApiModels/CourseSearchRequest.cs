﻿using System;
using Dfc.ProviderPortal.FindACourse.Models;

namespace Dfc.ProviderPortal.FindACourse.ApiModels
{
    public class CourseSearchRequest : IPagedRequest
    {
        public string SubjectKeyword { get; set; }
        public float? Distance { get; set; }
        public string ProviderName { get; set; }
        public int[] QualificationLevels { get; set; }
        public int[] StudyModes { get; set; }
        public int[] AttendanceModes { get; set; }
        public int[] AttendancePatterns { get; set; }
        public int[] DeliveryModes { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public CourseSearchSortBy? SortBy { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public int? Limit { get; set; }
        public int? Start { get; set; }
    }
}
