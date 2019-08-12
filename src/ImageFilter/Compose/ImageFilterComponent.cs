using ImageFilter.Controllers;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;

namespace ImageFilter.Compose
{
    public class ImageFilterComponent : Umbraco.Core.Composing.IComponent
    {
        // initialize: runs once when Umbraco starts
        public void Initialize()
        {
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;
        }

        // terminate: runs once when Umbraco stops
        public void Terminate()
        {
        }

        private void ServerVariablesParser_Parsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("HttpContext is null");
            }

            //Create a .NET MVC URL Helper
            var urlHelper =
                new UrlHelper(
                    new RequestContext(
                        new HttpContextWrapper(
                            HttpContext.Current),
                        new RouteData()));

            if (!e.ContainsKey("PJSealImageFilter"))
                e.Add("PJSealImageFilter", new Dictionary<string, object>
                {
                    {
                        "ImageFilterApiUrl",
                        urlHelper.GetUmbracoApiServiceBaseUrl<ImageFilterBackofficeApiController>(
                            controller => controller.GetImageProccessorOptions())
                    }
                });
        }
    }
}
