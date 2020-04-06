﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.ProviderPortal.FindACourse.ApiModels;
using Dfc.ProviderPortal.FindACourse.ApiModels.Faoc;
using Dfc.ProviderPortal.FindACourse.Interfaces.Faoc;
using Dfc.ProviderPortal.FindACourse.Models.Search.Faoc;
using Dfc.ProviderPortal.Packages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Dfc.ProviderPortal.FindACourse.Controllers
{
    [ApiController]
    public class OnlineCoursesController : ControllerBase, IActionFilter
    {
        private static readonly Dictionary<string, string> _courseSearchFacetMapping = new Dictionary<string, string>()
        {
            { "NotionalNVQLevelv2", "QualificationLevel" },
            { "VenueStudyMode", "StudyMode" },
            { "VenueAttendancePattern", "AttendancePattern" }
        };

        private readonly ILogger _log;
        private readonly IOnlineCourseService _service;

        public OnlineCoursesController(
            ILogger<CoursesController> logger,
            IOnlineCourseService service)
        {
            Throw.IfNull(logger, nameof(logger));
            Throw.IfNull(service, nameof(service));

            _log = logger;
            _service = service;
        }

        [Route("~/onlinecoursesearch")]
        [HttpPost]
        [ProducesResponseType(typeof(OnlineCourseSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> OnlineCourseSearch([FromBody]OnlineCourseSearchRequest request)
        {
            var criteria = new OnlineCourseSearchCriteria()
            {
                ProviderName = request.ProviderName,
                QualificationLevels = request.QualificationLevels,
                SortBy = request.SortBy,
                Start = request.Start,
                StartDateFrom = request.StartDateFrom,
                StartDateTo = request.StartDateTo,
                SubjectKeyword = request.SubjectKeyword,
            };

            var result = await _service.OnlineCourseSearch(_log, criteria);

            var response = new OnlineCourseSearchResponse()
            {
                Limit = result.Limit,
                Start = result.Start,
                Total = result.Total,
                Facets = result.Facets.ToDictionary(
                    f => _courseSearchFacetMapping.GetValueOrDefault(f.Key, f.Key),
                    f => f.Value.Select(v => new FacetCountResult()
                    {
                        Value = v.Value,
                        Count = v.Count.GetValueOrDefault()
                    })),
                Results = result.Items.Select(i => new OnlineCourseSearchResponseItem()
                {
                    QualificationCourseTitle = i.Course.QualificationCourseTitle,
                    LearnAimRef = i.Course.LearnAimRef,
                    NotionalNVQLevelv2 = i.Course.NotionalNVQLevelv2,
                    AwardOrgCode = i.Course.AwardOrgCode,
                    QualificationType = i.Course.QualificationType,
                    CourseDescription = i.Course.CourseDescription,
                    EntryRequirements = i.Course.EntryRequirements,
                    WhatYoullLearn = i.Course.WhatYoullLearn,
                    HowYoullLearn = i.Course.HowYoullLearn,
                    WhatYoullNeed = i.Course.WhatYoullNeed,
                    HowYoullBeAssessed = i.Course.HowYoullBeAssessed,
                    WhereNext = i.Course.WhereNext,
                    AdultEducationBudget = i.Course.AdultEducationBudget,
                    AdvancedLearnerLoan = i.Course.AdvancedLearnerLoan,
                    CourseName = i.Course.CourseName,
                    CourseId = i.Course.CourseId,
                    CourseRunId = i.Course.CourseRunId,
                    StartDate = !i.Course.FlexibleStartDate ? i.Course.StartDate : null,
                    FlexibleStartDate = i.Course.FlexibleStartDate,
                    CourseWebsite = i.Course.CourseWebsite,
                    ProviderUKPRN = i.Course.ProviderUKPRN,
                    ProviderName = i.Course.ProviderName,
                    ProviderAddressLine1 = i.Course.ProviderAddressLine1,
                    ProviderAddressLine2 = i.Course.ProviderAddressLine2,
                    ProviderTown = i.Course.ProviderTown,
                    ProviderPostcode = i.Course.ProviderPostcode,
                    ProviderCounty = i.Course.ProviderCounty,
                    ProviderEmail = i.Course.ProviderEmail,
                    ProviderTelephone = i.Course.ProviderTelephone,
                    ProviderFax = i.Course.ProviderFax,
                    ProviderWebsite = i.Course.ProviderWebsite,
                    ProviderLearnerSatisfaction = i.Course.ProviderLearnerSatisfaction,
                    ProviderEmployerSatisfaction = i.Course.ProviderEmployerSatisfaction,
                }).ToList()
            };

            return new OkObjectResult(response);
        }


        [NonAction]
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ProblemDetailsException pde)
            {
                _log.LogInformation(
                    $"Request error on {context.ActionDescriptor.DisplayName}\nTitle: {pde.ProblemDetails.Title}\nDetail: {pde.ProblemDetails.Detail}");

                context.Result = new ObjectResult(pde.ProblemDetails)
                {
                    ContentTypes = new Microsoft.AspNetCore.Mvc.Formatters.MediaTypeCollection()
                    {
                        new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/problem+json")
                    },
                    StatusCode = pde.ProblemDetails.Status ?? 400
                };

                context.ExceptionHandled = true;
            }
        }

        [NonAction]
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}