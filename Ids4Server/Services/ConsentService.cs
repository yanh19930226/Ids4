using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Ids4Server.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ids4Server.Services
{
    public class ConsentService
    {
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;
        public ConsentService(IClientStore clientStore, IResourceStore resourceStore, IIdentityServerInteractionService identityServerInteractionService)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }

        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request,  InputConsentViewModel model)
        {
            var rememberConsent = model?.RememberConsent ?? true;
            var selectedScops = model?.ScopesConsented ?? Enumerable.Empty<string>();
           
            var vm = new ConsentViewModel()
            {
                ClientName = request.Client.ClientName ?? request.Client.ClientId,
                ClientLogoUrl = request.Client.LogoUri,
                ClientUrl = request.Client.ClientUri,
                RememberConsent = rememberConsent,
                IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(i => CreateScopeViewModel(i, selectedScops.Contains(i.Name) || model == null)),
            };
            var apiScopes = new List<ScopeViewModel>();
            foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
            {
                var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    var scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                    apiScopes.Add(scopeVm);
                }
            }
            vm.ResourceScopes = apiScopes;

            return vm;
        }
        private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource, bool check)
        {
            return new ScopeViewModel()
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Descripton = identityResource.Description,
                Required = identityResource.Required,
                Checked = check || identityResource.Required,
                Emphasize = identityResource.Emphasize
            };
        }

        private ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            var displayName = apiScope.DisplayName ?? apiScope.Name;
            if (!String.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            {
                displayName += ":" + parsedScopeValue.ParsedParameter;
            }
            return new ScopeViewModel
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = displayName,
                Descripton = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }
        /// <summary>
        /// BuildConsentViewModel
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <param name="model">二次选中的时候发生作用</param>
        /// <returns></returns>
        public async Task<ConsentViewModel> BuildConsentViewModel(string ReturnUrl, InputConsentViewModel model = null)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(ReturnUrl);
            if (request == null)
                return null;
            var vm = CreateConsentViewModel(request,model);
            vm.ReturnUrl = ReturnUrl;
            return vm;
        }

        public async Task<ProcessConsentResult> ProcessConsent(InputConsentViewModel model)
        {
            ConsentResponse response = null;
            var result = new ProcessConsentResult();
            if (model.Button == "no")
            {
                response = new ConsentResponse { Error = AuthorizationError.AccessDenied };
            }
            else if (model.Button == "yes")
            {
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    response = new ConsentResponse()
                    {
                        ScopesValuesConsented = model.ScopesConsented.ToArray(),
                        RememberConsent = model.RememberConsent
                    };
                }
                result.ValidationError = "请至少选中一个权限";
            }
            if (response != null)
            {
                var request = await _identityServerInteractionService.GetAuthorizationContextAsync(model.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(request, response);
                result.RedirectUrl = model.ReturnUrl;
            }
            else
            {
                var consentViewModel = await BuildConsentViewModel(model.ReturnUrl, model);
                result.consentViewModel = consentViewModel;
            }
            return result;
        }
    }
}
