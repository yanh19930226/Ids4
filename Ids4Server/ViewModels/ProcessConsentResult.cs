using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ids4Server.ViewModels
{
    public class ProcessConsentResult
    {
        public string RedirectUrl { get; set; }
        public bool isRedirect => RedirectUrl != null;
        public string ValidationError { get; set; }
        public ConsentViewModel consentViewModel { get; set; }
    }
}
