using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.ProviderPortal.FindACourse.Settings;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;

namespace Dfc.ProviderPortal.FindACourse
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

            return EnsureScoringProfilesOnCoursesIndex();
        }

        private async Task EnsureScoringProfilesOnCoursesIndex()
        {
            var index = await _service.Indexes.GetAsync(_coursesIndexName);

            if (index.ScoringProfiles.Count == 0)
            {
                index.ScoringProfiles.Add(new ScoringProfile()
                {
                    Name = "region-boost",
                    Functions = new List<ScoringFunction>()
                    {
                        new MagnitudeScoringFunction(
                            "ScoreBoost",
                            boost: 100,
                            new MagnitudeScoringParameters()
                            {
                                BoostingRangeStart = 1,
                                BoostingRangeEnd = 100,
                                ShouldBoostBeyondRangeByConstant = true
                            },
                            interpolation: ScoringFunctionInterpolation.Linear)
                    }
                });

                await _service.Indexes.CreateOrUpdateAsync(index);
            }
        }
    }
}
