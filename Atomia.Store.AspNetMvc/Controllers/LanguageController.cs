﻿using Atomia.Store.AspNetMvc.Infrastructure;
using Atomia.Store.AspNetMvc.Models;
using Atomia.Store.Core;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

namespace Atomia.Store.AspNetMvc.Controllers
{
    public sealed class LanguageController : Controller
    {
        private readonly ILanguageProvider languageProvider;
        private readonly ILanguagePreferenceProvider languagePreferenceProvider;

        public LanguageController()
        {
            this.languageProvider = DependencyResolver.Current.GetService<ILanguageProvider>();
            this.languagePreferenceProvider = DependencyResolver.Current.GetService<ILanguagePreferenceProvider>();
        }

        public JsonResult GetLanguages()
        {
            return JsonEnvelope.Success(new
            {
                Languages = languageProvider.GetAvailableLanguages(),
                CurrentLanguage = languagePreferenceProvider.GetPreferredLanguage()
            });
        }
    }
}