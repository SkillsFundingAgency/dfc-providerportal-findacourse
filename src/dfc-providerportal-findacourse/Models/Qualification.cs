﻿
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.FindACourse.Interfaces;


namespace Dfc.ProviderPortal.FindACourse.Models
{
    public class Qualification : IQualification
    {
        public string NotionalNVQLevelv2 { get; }
        public string AwardOrgCode { get; }
        public string TotalQualificationTime { get; }
        public string UnitType { get; }
        public string AwardOrgName { get; }
        public string LearnAimRef { get; }
        public string LearnAimRefTitle { get; }
        public string LearnDirectClassSystemCode1 { get; }
        public string LearnDirectClassSystemCode2 { get; }
        public string GuidedLearningHours { get; }
    }
}
