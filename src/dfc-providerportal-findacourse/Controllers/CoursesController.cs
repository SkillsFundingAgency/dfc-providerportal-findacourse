
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Dfc.ProviderPortal.FindACourse.Interfaces;
using Dfc.ProviderPortal.FindACourse.Models;
using Dfc.ProviderPortal.FindACourse.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.AspNetCore.Http;

namespace Dfc.ProviderPortal.FindACourse.Controllers
{
    /// <summary>
    /// Controller class for Courses API
    /// </summary>
    //[Authorize]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private ILogger _log = null;
        private ICourseService _service = null;
        private readonly SignInManager<APIUser> _signInManager;
        private readonly UserManager<APIUser> _userManager;
        private readonly IFACAuthenticationSettings _authSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public CoursesController(
            ILogger<CoursesController> logger,
            //SignInManager<APIUser> signInManager,
            //UserManager<APIUser> userManager,
            ICourseService service,
            IOptions<FACAuthenticationSettings> authSettings)
        {
            Throw.IfNull<ILogger<CoursesController>>(logger, nameof(logger));
            Throw.IfNull<ICourseService>(service, nameof(service));
            Throw.IfNull<IOptions<FACAuthenticationSettings>>(authSettings, nameof(authSettings));
            //Throw.IfNull<SignInManager<APIUser>>(signInManager, nameof(signInManager));
            //Throw.IfNull<UserManager<APIUser>>(_userManager, nameof(_userManager));

            _log = logger;
            _service = service;
            _authSettings = authSettings.Value;
            //_signInManager = signInManager;
            //_userManager = userManager;
        }

        /// <summary>
        /// Search courses (aka Find A Course), for example:
        /// POST search
        /// </summary>
        /// <returns>Search results</returns>
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
            try {
                //var result = await _signInManager.PasswordSignInAsync(UserName, Password, false, false);

                //if (result.Succeeded)
                //{
                //    _log.LogInformation("User logged in.");

                //    var principal = (ClaimsPrincipal)Thread.CurrentPrincipal;
                //    ClaimsIdentity identity = (ClaimsIdentity)User.Identity;

                //    var user = await _userManager.FindByEmailAsync(UserName);
                //    if (user != null)
                //    {
                //        var claims = await _userManager.GetClaimsAsync(user);
                //        foreach (var claim in claims)
                //            identity.AddClaim(new Claim(claim.Type, claim.Value));
                //    }

                //    //return LocalRedirect(returnUrl);
                //    //}

                //    //if (result.RequiresTwoFactor)
                //    //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });

                //} else if (result.IsLockedOut) {
                //    _log.LogWarning($"User account for {UserName} locked out");
                //    return null; // new FACSearchResult(); { Value = new List<FACSearchResultItem>() { new FACSearchResultItem() {  }  }

                //} else {


                if (_authSettings.UserName != UserName || _authSettings.Password != Password) {
                    _log.LogWarning($"Login failed for {UserName}");
                    return new UnauthorizedResult();

                } else {
                    _log.LogInformation($"FAC search with keyword {criteria.SubjectKeyword}");
                    Task<FACSearchResult> task = _service.CourseSearch(_log, criteria);
                    task.Wait();
                    if (task.Result?.Value?.Count() > 0)
                        return new OkObjectResult(task.Result);
                    else
                        return new NoContentResult();
                }

            } catch (Exception ex) {
                //return new InternalServerErrorObjectResult(ex);
                _log.LogError(ex, $"Error in Search: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Search providers, for example:
        /// POST providersearch
        /// </summary>
        /// <returns>Provider search results</returns>
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
            try {

                if (_authSettings.UserName != UserName || _authSettings.Password != Password) {
                    _log.LogWarning($"Login failed for {UserName}");
                    return new UnauthorizedResult();

                } else {
                    _log.LogInformation($"Provider search with keyword {criteria.Keyword}");
                    Task<ProviderSearchResult> task = _service.ProviderSearch(_log, criteria);
                    task.Wait();
                    if (task.Result?.Value?.Count() > 0)
                        return new OkObjectResult(task.Result);
                    else
                        return new NoContentResult();
                }

            } catch (Exception ex) {
                //return new InternalServerErrorObjectResult(ex);
                _log.LogError(ex, $"Error in Search: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
