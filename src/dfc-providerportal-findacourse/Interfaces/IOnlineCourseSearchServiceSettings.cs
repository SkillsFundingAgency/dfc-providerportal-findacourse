﻿namespace Dfc.ProviderPortal.FindACourse.Interfaces
{
    public interface IOnlineCourseSearchServiceSettings
    {
        string SearchService { get; }
        string ApiUrl { get; }
        string ApiVersion { get; }
        string QueryKey { get; }
        string Index { get; }
        int DefaultTop { get; }
        int MaxTop { get; }
    }
}