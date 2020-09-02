using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ids4Server.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ids4Server.Controllers
{
    public class ConsentController : Controller
    {
        private readonly Services.ConsentService _consentService;

        public ConsentController(Services.ConsentService consentService)
        {
            _consentService = consentService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string ReturnUrl)
        {
            var model = await _consentService.BuildConsentViewModel(ReturnUrl);
            if (model == null)
            {

            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel model)
        {
            var result = await _consentService.ProcessConsent(model);
            if (result.isRedirect)
            {
                return Redirect(result.RedirectUrl);
            }
            if (!string.IsNullOrEmpty(result.ValidationError))
            {
                ModelState.AddModelError("", result.ValidationError);
            }
            return View(result.consentViewModel);
        }
    }
}
