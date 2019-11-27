using System;
using System.Collections.Generic;

namespace Dfc.ProviderPortal.FindACourse.ApiModels
{
    public class CourseSearchResponse
    {
        public string ODataContext { get; set; }
        public int? ODataCount { get; set; }
        public dynamic SearchFacets { get; set; } //FACSearchFacets SearchFacets { get; set; }
        public IEnumerable<CourseSearchResponseItem> Value { get; set; }
    }

    public class CourseSearchResponseItem
    {
        public dynamic SearchScore { get; set; }
        public dynamic VenueLocation { get; set; }
        public double? GeoSearchDistance { get; set; }
        public decimal? ScoreBoost { get; set; }
        public Guid? id { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? CourseRunId { get; set; }
        public string QualificationCourseTitle { get; set; }
        public string LearnAimRef { get; set; }
        public string NotionalNVQLevelv2 { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public string VenueAttendancePattern { get; set; }
        public string VenueAttendancePatternDescription { get; set; }
        public string ProviderName { get; set; }
        public string Region { get; set; }
        public int? Status { get; set; }
        public string VenueStudyMode { get; set; }
        public string VenueStudyModeDescription { get; set; }
        public string DeliveryMode { get; set; }
        public string DeliveryModeDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public string VenueTown { get; set; }
        public int? Cost { get; set; }
        public string CostDescription { get; set; }
        public string CourseText { get; set; }
        public string UKPRN { get; set; }
        public string CourseDescription { get; set; }
    }
}
