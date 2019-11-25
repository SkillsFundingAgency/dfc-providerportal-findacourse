using System;

namespace Dfc.ProviderPortal.FindACourse.Models
{
    public class SearchCriteriaStructure
    {
        public string SubjectKeyword { get; set; }
        public string DFE1619Funded { get; set; }
        public float? Distance { get; set; }
        public string[] QualificationLevels { get; set; }
        public string[] StudyModes { get; set; }
        public string[] AttendanceModes { get; set; }
        public string[] AttendancePatterns { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public CourseSearchSortBy? SortBy { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }

        public int? TopResults { get; set; }
        public int? PageNo { get; set; }
    }
}
