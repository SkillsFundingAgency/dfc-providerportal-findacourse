using Dfc.ProviderPortal.FindACourse.Interfaces;
using Dfc.ProviderPortal.FindACourse.Models;
using Dfc.ProviderPortal.FindACourse.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.FindACourse.Controllers
{
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private ILogger _log = null;
        private ICourseService _service = null;
        private readonly SignInManager<APIUser> _signInManager;
        private readonly UserManager<APIUser> _userManager;
        private readonly IFACAuthenticationSettings _authSettings;

        public CoursesController(
            ILogger<CoursesController> logger,
            ICourseService service,
            IOptions<FACAuthenticationSettings> authSettings)
        {
            Throw.IfNull<ILogger<CoursesController>>(logger, nameof(logger));
            Throw.IfNull<ICourseService>(service, nameof(service));
            Throw.IfNull<IOptions<FACAuthenticationSettings>>(authSettings, nameof(authSettings));

            _log = logger;
            _service = service;
            _authSettings = authSettings.Value;
        }

        [Route("~/search")]
        [HttpPost]
        [ProducesResponseType(typeof(FACSearchResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Search(
            [FromBody]SearchCriteriaStructure criteria,
            [FromHeader(Name = "UserName")]string UserName,
            [FromHeader(Name = "Password")]string Password)
        {
            try
            {
                if (_authSettings.UserName != UserName || _authSettings.Password != Password)
                {
                    _log.LogWarning($"Login failed for {UserName}");
                    return new UnauthorizedResult();
                }
                else
                {
                    _log.LogInformation($"FAC search with keyword {criteria.SubjectKeyword}");
                    Task<FACSearchResult> task = _service.CourseSearch(_log, criteria);
                    task.Wait();
                    if (task.Result?.Value?.Count() > 0)
                        return new OkObjectResult(task.Result);
                    else
                        return new NoContentResult();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in Search: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("~/providersearch")]
        [HttpPost]
        [ProducesResponseType(typeof(ProviderSearchResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ProviderSearch(
            [FromBody]ProviderSearchCriteriaStructure criteria,
            [FromHeader(Name = "UserName")]string UserName,
            [FromHeader(Name = "Password")]string Password)
        {
            try
            {
                if (_authSettings.UserName != UserName || _authSettings.Password != Password)
                {
                    _log.LogWarning($"Login failed for {UserName}");
                    return new UnauthorizedResult();
                }
                else
                {
                    _log.LogInformation($"Provider search with keyword {criteria.Keyword}");
                    Task<ProviderSearchResult> task = _service.ProviderSearch(_log, criteria);
                    task.Wait();
                    if (task.Result?.Value?.Count() > 0)
                        return new OkObjectResult(task.Result);
                    else
                        return new NoContentResult();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in ProviderSearch: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("~/larssearch")]
        [HttpPost]
        [ProducesResponseType(typeof(LARSSearchResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> LARSSearch(
            [FromBody]LARSSearchCriteriaStructure criteria,
            [FromHeader(Name = "UserName")]string UserName,
            [FromHeader(Name = "Password")]string Password)
        {
            try
            {
                if (_authSettings.UserName != UserName || _authSettings.Password != Password)
                {
                    _log.LogWarning($"Login failed for {UserName}");
                    return new UnauthorizedResult();
                }
                else
                {
                    _log.LogInformation($"LARS search with keyword {criteria.Keyword}");
                    Task<LARSSearchResult> task = _service.LARSSearch(_log, criteria);
                    task.Wait();
                    if (task.Result?.Value?.Count() > 0)
                        return new OkObjectResult(task.Result);
                    else
                        return new NoContentResult();
                }
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ONSPDSearch(
            [FromBody]PostcodeSearchCriteriaStructure criteria,
            [FromHeader(Name = "UserName")]string UserName,
            [FromHeader(Name = "Password")]string Password)
        {
            try
            {
                if (_authSettings.UserName != UserName || _authSettings.Password != Password)
                {
                    _log.LogWarning($"Login failed for {UserName}");
                    return new UnauthorizedResult();
                }
                else
                {
                    _log.LogInformation($"ONSPD search with keyword {criteria.Keyword}");
                    Task<PostcodeSearchResult> task = _service.PostcodeSearch(_log, criteria);
                    task.Wait();
                    if (task.Result?.Value?.Count() > 0)
                        return new OkObjectResult(task.Result);
                    else
                        return new NoContentResult();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Error in ONSPDSearch: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}