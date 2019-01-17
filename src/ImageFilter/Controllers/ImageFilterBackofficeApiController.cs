using ImageFilter.Models;
using ImageProcessor.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<AvailableProcessorModel> GetImageProccessorOptions()
        {

            var availableProcessors = ImageProcessorConfiguration.Instance.AvailableWebGraphicsProcessors;
            var validProcessors = new List<AvailableProcessorModel>();
            foreach(var processor in availableProcessors )
            {
                var currentProcessor = new AvailableProcessorModel { Name = processor.Key.Name };
                if (System.IO.File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(currentProcessor.PathToView)))
                {
                    currentProcessor.IsValid = true;
                    validProcessors.Add(currentProcessor);
                }
            }

            return validProcessors.OrderBy(p=>p.Name).ToList();
        }
    }
}
