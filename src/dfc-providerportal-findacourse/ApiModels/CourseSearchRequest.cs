using System;
using Dfc.ProviderPortal.FindACourse.Models;

namespace Dfc.ProviderPortal.FindACourse.ApiModels
{
    public class CourseSearchRequest
    {
        public string SubjectKeyword { get; set; }
        public string DFE1619Funded { get; set; }
        public float? Distance { get; set; }
        public int[] QualificationLevels { get; set; }
        public int[] StudyModes { get; set; }
        public int[] AttendanceModes { get; set; }
        public int[] AttendancePatterns { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public CourseSearchSortBy? SortBy { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }

        public int? TopResults { get; set; }
        public int? PageNo { get; set; }
    }
}
