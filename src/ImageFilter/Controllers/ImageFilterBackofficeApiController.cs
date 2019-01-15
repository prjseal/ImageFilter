using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace ImageFilter.Controllers
{
    [PluginController("ImageFilter")]
    public class ImageFilterBackofficeApiController : UmbracoAuthorizedJsonController
    {
        public string CreateImageWithFilterOption()
        {
            //just setting up dummy api method at the moment
            string url = "/umbraco#/media/media/edit/1061";
            return url;
        }
    }
}
