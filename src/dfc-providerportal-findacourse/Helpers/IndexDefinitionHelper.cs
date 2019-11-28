using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dfc.ProviderPortal.FindACourse.Settings;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;

namespace Dfc.ProviderPortal.FindACourse.Helpers
{
    public class IndexDefinitionHelper
    {
        private readonly SearchServiceClient _service;
        private readonly string _coursesIndexName;

        public IndexDefinitionHelper(IOptions<SearchServiceSettings> settings)
        {
            _service = new SearchServiceClient(settings.Value.SearchService, new SearchCredentials(settings.Value.AdminKey));
            _coursesIndexName = settings.Value.Index;
        }

        public Task UpdateIndexDefinitions()
        {
            // TODO Move all index definitions here

            return EnsureCoursesIndex();
        }

        private async Task EnsureCoursesIndex()
        {
            var index = new Index()
            {
                Name = _coursesIndexName,
                Fields = new List<Field>()
                {
                    new Field("id", DataType.String) { IsKey = true, IsFacetable = false, IsFilterable = false, IsSearchable = false, IsSortable = false },
                    new Field("CourseId", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("CourseRunId", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("QualificationCourseTitle", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("LearnAimRef", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("NotionalNVQLevelv2", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("Status", DataType.Int32) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("VenueName", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("VenueAddress", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("VenueLocation", DataType.GeographyPoint) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = true },
                    new Field("VenueAttendancePattern", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("ProviderName", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("Region", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("Weighting", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = true },
                    new Field("ScoreBoost", DataType.Double) { IsFacetable = true, IsFilterable = true, IsSearchable = false, IsSortable = true },
                    new Field("UpdatedOn", DataType.DateTimeOffset) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("VenueStudyMode", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("DeliveryMode", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("StartDate", DataType.DateTimeOffset) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = true },
                    new Field("VenueTown", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("Cost", DataType.Int32) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = true },
                    new Field("CostDescription", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("CourseText", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("VenueAttendancePatternDescription", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("VenueStudyModeDescription", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("DeliveryModeDescription", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("UKPRN", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("CourseDescription", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("CourseName", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("FlexibleStartDate", DataType.Boolean) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false }
                },
                ScoringProfiles = new List<ScoringProfile>()
                {
                    new ScoringProfile()
                    {
                        Name = "region-boost",
                        Functions = new List<ScoringFunction>()
                        {
                            new MagnitudeScoringFunction(
                                "ScoreBoost",
                                boost: 100,
                                parameters: new MagnitudeScoringParameters()
                                {
                                    BoostingRangeStart = 1,
                                    BoostingRangeEnd = 100,
                                    ShouldBoostBeyondRangeByConstant = true
                                },
                                interpolation: ScoringFunctionInterpolation.Linear)
                        }
                    }
                }
            };

            await _service.Indexes.CreateOrUpdateAsync(index);
        }
    }
}
