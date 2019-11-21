using System;
using System.Linq;
using System.Threading.Tasks;
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CourseSearch([FromBody]SearchCriteriaStructure criteria)
        {
            try
            {
                _log.LogInformation($"FAC search with keyword {criteria.SubjectKeyword}");
                var result = await _service.CourseSearch(_log, criteria);
                return new OkObjectResult(result);
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CourseGet([FromBody]CourseDetailStructure criteria)
        {
            try
            {
                _log.LogInformation($"FAC CourseDetail called for CourseId {criteria.CourseId}");
                var result = await _service.CourseDetail(criteria.CourseId, criteria.RunId);
                if (result != null)
                {
                    return new OkObjectResult(result);
                }
                else
                {
                    return new NoContentResult();
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
        public async Task<ActionResult> ProviderSearch([FromBody]ProviderSearchCriteriaStructure criteria)
        {
            try
            {
                _log.LogInformation($"Provider search with keyword {criteria.Keyword}");
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ProviderGet([FromBody]string PRN)
        {
            try
            {
                _log.LogInformation($"FAC ProviderGet called for PRN {PRN}");
                var result = await _service.ProviderDetail(PRN);
                if (result != null)
                {
                    return new OkObjectResult(result);
                }
                else
                {
                    return new NoContentResult();
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
        public async Task<ActionResult> LARSSearch([FromBody]LARSSearchCriteriaStructure criteria)
        {
            try
            {
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
        public async Task<ActionResult> ONSPDSearch([FromBody]PostcodeSearchCriteriaStructure criteria)
        {
            try
            {
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