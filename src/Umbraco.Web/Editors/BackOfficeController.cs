﻿using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Umbraco.Core.Configuration;
using Umbraco.Core.Manifest;
using Umbraco.Core;
using Umbraco.Web.Trees;
using Umbraco.Web.UI.JavaScript;

namespace Umbraco.Web.Editors
{
    /// <summary>
    /// A controller to render out the default back office view and JS results
    /// </summary>
    public class BackOfficeController : Controller
    {

        /// <summary>
        /// Render the default view
        /// </summary>
        /// <returns></returns>
        public ActionResult Default()
        {
            return View(GlobalSettings.Path.EnsureEndsWith('/') + "Views/Default.cshtml");
        }

        /// <summary>
        /// Returns the RequireJS file including all references found in manifests
        /// </summary>
        /// <returns></returns>
        public JavaScriptResult Application()
        {
            var plugins = new DirectoryInfo(Server.MapPath("~/App_Plugins"));
            var parser = new ManifestParser(plugins);
            var requireJs = new RequireJsInit(parser);
            var result = requireJs.GetJavascriptInitialization(RequireJsInit.GetDefaultConfig(), RequireJsInit.GetDefaultInitialization());
            return JavaScript(result);
        }

        /// <summary>
        /// Returns the JavaScript object representing the static server variables javascript object
        /// </summary>
        /// <returns></returns>
        public JavaScriptResult ServerVariables()
        {
            //now we need to build up the variables
            var d = new Dictionary<string, object>
                {
                    {"umbracoPath", GlobalSettings.Path},
                    {"contentApiBaseUrl", Url.GetUmbracoApiService<ContentController>("PostSave").TrimEnd("PostSave")},
                    {"mediaApiBaseUrl", Url.GetUmbracoApiService<MediaController>("GetRootMedia").TrimEnd("GetRootMedia")},
                    {"treeApplicationApiBaseUrl", Url.GetUmbracoApiService<ApplicationTreeApiController>("GetTreeData").TrimEnd("GetTreeData")},
                    {"contentTypeApiBaseUrl", Url.GetUmbracoApiService<ContentTypeApiController>("GetAllowedChildren").TrimEnd("GetAllowedChildren")},
                    {"mediaTypeApiBaseUrl", Url.GetUmbracoApiService<MediaTypeApiController>("GetAllowedChildren").TrimEnd("GetAllowedChildren")}
                };

            return JavaScript(ServerVariablesParser.Parse(d));
        }

    }
}