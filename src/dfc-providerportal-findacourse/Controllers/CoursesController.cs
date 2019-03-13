
using Dfc.ProviderPortal.FindACourse.Interfaces;
using Dfc.ProviderPortal.FindACourse.Models;
using Dfc.ProviderPortal.Packages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.FindACourse.Controllers
{
    /// <summary>
    /// Controller class for Courses API
    /// </summary>
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private ILogger _log = null;
        private ICourseService _service = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public CoursesController(
            ILogger<CoursesController> logger,
            ICourseService service)
        {
            Throw.IfNull<ILogger<CoursesController>>(logger, nameof(logger));
            Throw.IfNull<ICourseService>(service, nameof(service));

            _log = logger;
            _service = service;
        }

        /// <summary>
        /// Search courses (aka Find A Course), for example:
        /// POST search
        /// </summary>
        /// <returns>Search results</returns>
        [Route("~/search")]
        [HttpPost]
        public async Task<FACSearchResult> Search([FromBody]SearchCriteriaStructure criteria)
        {
            try {
                _log.LogInformation($"FAC search started: {criteria}");
                _log.LogInformation($"FAC search with keyword {criteria.SubjectKeyword}");
                Task<FACSearchResult> task = _service.CourseSearch(_log, criteria);
                return await task;

            } catch (Exception ex) {
                //return new InternalServerErrorObjectResult(ex);
                _log.LogError(ex, "Error in CourseSearch");
                return null;
            }
        }

    }
}
