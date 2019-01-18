using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core.Components;
using ImageFilter.Controllers;
using Umbraco.Web;
using Umbraco.Web.UI.JavaScript;

namespace ImageFilter
{
    public class ImageFilterComponent : IComposer
    {
        public void Compose(Composition composition)
        {
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;
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
