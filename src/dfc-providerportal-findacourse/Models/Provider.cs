﻿
using System;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using Dfc.ProviderPortal.FindACourse.Interfaces;


namespace Dfc.ProviderPortal.FindACourse.Models
{
    public class Provider : IProvider
    {
        public Guid id { get; set; }
        public string UnitedKingdomProviderReferenceNumber { get; set; }
        public string ProviderName { get; set; }
        public string CourseDirectoryName { get; set; }
        public string ProviderStatus { get; set; }
        public dynamic /*IProvidercontact[]*/ ProviderContact { get; set; }
        public DateTime ProviderVerificationDate { get; set; }
        public bool ProviderVerificationDateSpecified { get; set; }
        public bool ExpiryDateSpecified { get; set; }
        public object ProviderAssociations { get; set; }
        public dynamic /*IProvideralias[]*/ ProviderAliases { get; set; }
        public dynamic /*IVerificationdetail[]*/ VerificationDetails { get; set; }
        public Status Status { get; set; }

        // Apprenticeship related
        public int? ProviderId { get; set; }
        public int? UPIN { get; set; } // Needed to get LearnerSatisfaction & EmployerSatisfaction from FEChoices
        public string TradingName { get; set; }
        public bool NationalApprenticeshipProvider { get; set; }
        public string MarketingInformation { get; set; }
        public string Alias { get; set; }

        // Bulk course upload
        public dynamic /*BulkUploadStatus*/ BulkUploadStatus { get; set; }

        //public Provider(Providercontact[] providercontact, Provideralias[] provideraliases, Verificationdetail[] verificationdetails)
        //{
        //    ProviderContact = providercontact;
        //    ProviderAliases = provideraliases;
        //    VerificationDetails = verificationdetails;
        //}

        public ProviderType ProviderType { get; set; }
    }

    public enum Status
    {
        Registered = 0,
        Onboarded = 1,
        Unregistered = 2
    }

    public enum ProviderType
    {
        Undefined = 0,
        [Description("F.E.")]
        FE = 1,
        [Description("Apprenticeships")]
        Apprenticeship = 2,
        [Description("Both")]
        Both = FE | Apprenticeship
    }
}
