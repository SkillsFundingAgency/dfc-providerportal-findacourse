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
        [ProducesResponseType(typeof(FACSearchResult), StatusCodes.Status200OK)]
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
                    DFE1619Funded = request.DFE1619Funded,
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
                return new OkObjectResult(result);
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
        [ProducesResponseType(typeof(ProviderSearchResult), StatusCodes.Status200OK)]
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
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in ProviderSearch: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("~/providerget")]
        [HttpPost]
        [ProducesResponseType(typeof(Provider), StatusCodes.Status200OK)]
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
                    return new OkObjectResult(result);
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
        [ProducesResponseType(typeof(LARSSearchResult), StatusCodes.Status200OK)]
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
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in LARSSearch: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("~/onspdsearch")]
        [HttpPost]
        [ProducesResponseType(typeof(PostcodeSearchResult), StatusCodes.Status200OK)]
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
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in ONSPDSearch: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}