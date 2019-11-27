using System;
using System.Linq;
using System.Threading.Tasks;
using Dfc.ProviderPortal.FindACourse.ApiModels;
using Dfc.ProviderPortal.FindACourse.Interfaces;
using Dfc.ProviderPortal.FindACourse.Models;
using Dfc.ProviderPortal.Packages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dfc.ProviderPortal.FindACourse.Controllers
{
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ILogger _log;
        private readonly ICourseService _service;

        public CoursesController(
            ILogger<CoursesController> logger,
            ICourseService service)
        {
            Throw.IfNull(logger, nameof(logger));
            Throw.IfNull(service, nameof(service));

            _log = logger;
            _service = service;
        }

        [Route("~/coursesearch")]
        [HttpPost]
        [ProducesResponseType(typeof(CourseSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CourseSearch([FromBody]CourseSearchRequest request)
        {
            try
            {
                var criteria = new SearchCriteriaStructure()
                {
                    AttendanceModes = request.AttendanceModes,
                    AttendancePatterns = request.AttendancePatterns,
                    Distance = request.Distance,
                    PageNo = request.PageNo,
                    Postcode = request.Postcode,
                    QualificationLevels = request.QualificationLevels,
                    SortBy = request.SortBy,
                    StartDateFrom = request.StartDateFrom,
                    StartDateTo = request.StartDateTo,
                    StudyModes = request.StudyModes,
                    SubjectKeyword = request.SubjectKeyword,
                    TopResults = request.TopResults,
                    Town = request.Town
                };

                _log.LogInformation($"FAC search with keyword {criteria.SubjectKeyword}");
                var result = await _service.CourseSearch(_log, criteria);

                var response = new CourseSearchResponse()
                {
                    ResultCount = result.ResultCount,
                    Facets = result.Facets.ToDictionary(
                        f => f.Key,
                        f => f.Value.Select(v => new FacetCountResult()
                        {
                            Value = v.Value,
                            Count = v.Count.Value
                        })),
                    Value = result.Items.Select(i => new CourseSearchResponseItem()
                    {
                        Cost = i.Course.Cost,
                        CostDescription = i.Course.CostDescription,
                        CourseDescription = i.Course.CourseDescription,
                        CourseId = i.Course.CourseId,
                        CourseRunId = i.Course.CourseRunId,
                        CourseText = i.Course.CourseText,
                        DeliveryMode = i.Course.DeliveryMode,
                        DeliveryModeDescription = i.Course.DeliveryModeDescription,
                        GeoSearchDistance = i.Distance,
                        id = i.Course.id,
                        LearnAimRef = i.Course.LearnAimRef,
                        NotionalNVQLevelv2 = i.Course.NotionalNVQLevelv2,
                        ProviderName = i.Course.ProviderName,
                        QualificationCourseTitle = i.Course.QualificationCourseTitle,
                        Region = i.Course.Region,
                        ScoreBoost = i.Course.ScoreBoost,
                        SearchScore = i.Score,
                        StartDate = i.Course.StartDate,
                        Status = i.Course.Status,
                        UKPRN = i.Course.UKPRN,
                        UpdatedOn = i.Course.UpdatedOn,
                        VenueAddress = i.Course.VenueAddress,
                        VenueAttendancePattern = i.Course.VenueAttendancePattern,
                        VenueAttendancePatternDescription = i.Course.VenueAttendancePatternDescription,
                        VenueLocation = i.Course.VenueLocation,
                        VenueName = i.Course.VenueName,
                        VenueStudyMode = i.Course.VenueStudyMode,
                        VenueStudyModeDescription = i.Course.VenueStudyModeDescription,
                        VenueTown = i.Course.VenueTown
                    })
                };

                return new OkObjectResult(response);
            }
            catch (ProblemDetailsException ex)
            {
                return new ObjectResult(ex.ProblemDetails)
                {
                    ContentTypes = new Microsoft.AspNetCore.Mvc.Formatters.MediaTypeCollection()
                    {
                        new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/problem+json")
                    },
                    StatusCode = ex.ProblemDetails.Status ?? 400
                };
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in Search: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("~/courseget")]
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CourseGet([FromBody]CourseGetRequest request)
        {
            try
            {
                _log.LogInformation($"FAC CourseDetail called for CourseId {request.CourseId}");
                var result = await _service.CourseDetail(request.CourseId, request.RunId);
                if (result != null)
                {
                    return new OkObjectResult(result);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception ex) {
                _log.LogError(ex, $"Error in CourseGet: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("~/providersearch")]
        [HttpPost]
        [ProducesResponseType(typeof(ProviderSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ProviderSearch([FromBody]ProviderSearchRequest request)
        {
            try
            {
                var criteria = new ProviderSearchCriteriaStructure()
                {
                    Keyword = request.Keyword,
                    Region = request.Region,
                    TopResults = request.TopResults,
                    Town = request.Town
                };

                _log.LogInformation($"Provider search with keyword {request.Keyword}");
                var result = await _service.ProviderSearch(_log, criteria);

                var response = new ProviderSearchResponse()
                {
                    ODataContext = result.ODataContext,
                    ODataCount = result.ODataCount,
                    SearchFacets = result.SearchFacets,
                    Value = result.Value.Select(i => new ProviderSearchResponseItem()
                    {
                        CourseDirectoryName = i.CourseDirectoryName,
                        id = i.id,
                        Name = i.Name,
                        Postcode = i.Postcode,
                        ProviderAlias = i.ProviderAlias,
                        ProviderStatus = i.ProviderStatus,
                        Region = i.Region,
                        Status = i.Status,
                        Town = i.Town,
                        TradingName = i.TradingName,
                        UKPRN = i.UKPRN
                    })
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in ProviderSearch: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("~/providerget")]
        [HttpPost]
        [ProducesResponseType(typeof(ProviderGetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ProviderGet([FromBody]ProviderGetRequest request)
        {
            try
            {
                _log.LogInformation($"FAC ProviderGet called for PRN {request.UKPRN}");
                var result = await _service.ProviderDetail(request.UKPRN);

                if (result != null)
                {
                    var response = new ProviderGetResponse()
                    {
                        Alias = result.Alias,
                        BulkUploadStatus = result.BulkUploadStatus,
                        CourseDirectoryName = result.CourseDirectoryName,
                        ExpiryDateSpecified = result.ExpiryDateSpecified,
                        id = result.id,
                        MarketingInformation = result.MarketingInformation,
                        NationalApprenticeshipProvider = result.NationalApprenticeshipProvider,
                        ProviderAliases = result.ProviderAliases,
                        ProviderAssociations = result.ProviderAssociations,
                        ProviderContact = result.ProviderContact,
                        ProviderId = result.ProviderId,
                        ProviderName = result.ProviderName,
                        ProviderStatus = result.ProviderStatus,
                        ProviderType = result.ProviderType,
                        ProviderVerificationDate = result.ProviderVerificationDate,
                        ProviderVerificationDateSpecified = result.ProviderVerificationDateSpecified,
                        Status = result.Status,
                        TradingName = result.TradingName,
                        UnitedKingdomProviderReferenceNumber = result.UnitedKingdomProviderReferenceNumber,
                        UPIN = result.UPIN,
                        VerificationDetails = result.VerificationDetails
                    };

                    return new OkObjectResult(response);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in ProviderGet: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("~/larssearch")]
        [HttpPost]
        [ProducesResponseType(typeof(LARSSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> LARSSearch([FromBody]LARSSearchRequest request)
        {
            try
            {
                var criteria = new LARSSearchCriteriaStructure()
                {
                    AwardOrgAimRef = request.AwardOrgAimRef,
                    AwardOrgCode = request.AwardOrgCode,
                    Keyword = request.Keyword,
                    NotionalNVQLevelv2 = request.NotionalNVQLevelv2,
                    SectorSubjectAreaTier1 = request.SectorSubjectAreaTier1,
                    SectorSubjectAreaTier2 = request.SectorSubjectAreaTier2,
                    TopResults = request.TopResults
                };

                _log.LogInformation($"LARS search with keyword {criteria.Keyword}");
                var result = await _service.LARSSearch(_log, criteria);

                var response = new LARSSearchResponse()
                {
                    ODataContext = result.ODataContext,
                    ODataCount = result.ODataCount,
                    SearchFacets = result.SearchFacets,
                    Value = result.Value
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in LARSSearch: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("~/onspdsearch")]
        [HttpPost]
        [ProducesResponseType(typeof(PostcodeSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ONSPDSearch([FromBody]PostcodeSearchRequest request)
        {
            try
            {
                var criteria = new PostcodeSearchCriteriaStructure()
                {
                    Keyword = request.Keyword,
                    TopResults = request.TopResults
                };

                _log.LogInformation($"ONSPD search with keyword {criteria.Keyword}");
                var result = await _service.PostcodeSearch(_log, criteria);

                var response = new PostcodeSearchResponse()
                {
                    ODataContext = result.ODataContext,
                    ODataCount = result.ODataCount,
                    SearchFacets = result.SearchFacets,
                    Value = result.Value
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in ONSPDSearch: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}