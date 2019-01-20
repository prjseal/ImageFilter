using ImageFilter.Models;
using ImageProcessor;
using ImageProcessor.Web.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace ImageFilter.Controllers
{
    [PluginController("ImageFilter")]
    public class ImageFilterBackofficeApiController : UmbracoAuthorizedJsonController
    {
        public List<AvailableProcessorModel> GetImageProccessorOptions()
        {

            var availableProcessors = ImageProcessorConfiguration.Instance.AvailableWebGraphicsProcessors;
            var validProcessors = new List<AvailableProcessorModel>();
            foreach (var processor in availableProcessors)
            {
                var currentProcessor = new AvailableProcessorModel { Name = processor.Key.Name };
                if (System.IO.File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(currentProcessor.PathToView)))
                {
                    currentProcessor.IsValid = true;
                    validProcessors.Add(currentProcessor);
                }
            }

            return validProcessors.OrderBy(p => p.Name).ToList();
        }

        [HttpPost]
        public IHttpActionResult CreateMediaItem(ImageFilterInstruction imageFilterInstruction)
        {
            var mediaId = imageFilterInstruction.MediaId;
            var queryString = imageFilterInstruction.QueryString;
            var mediaService = Services.MediaService;

            // get mediaItem
            var mediaItem = mediaService.GetById(mediaId);
            var mediaItemAlias = mediaItem.ContentType.Alias;
            if (mediaItem == null)
            {
                return BadRequest(string.Format("Couldn't find the media item to rotate"));
            }
            var umbracoFile = mediaItem.GetValue<string>("umbracoFile");
            if (String.IsNullOrEmpty(umbracoFile))
            {
                return BadRequest(string.Format("Couldn't retrieve the umbraco file details of the item to rotate"));
            }

            bool isNew = mediaItem.Id <= 0;
            string serverFilePath = GetServerFilePath(mediaItem, isNew);
            string newFileName = Guid.NewGuid().ToString() + ".jpg";
            if (serverFilePath != null)
            {
                using (ImageFactory imageFactory = new ImageFactory(false))
                {
                    var imageToRotate = imageFactory.Load(serverFilePath);
                    var ms = new MemoryStream();

                    NameValueCollection settings = HttpUtility.ParseQueryString(imageFilterInstruction.QueryString);

                    string setting = settings.GetKey(0);
                    string value = settings.Get(0);

                    switch (setting)
                    {
                        case "brightness":
                            imageToRotate.Brightness(int.Parse(value)).Save(ms);
                            break;
                        case "contrast":
                            imageToRotate.Contrast(int.Parse(value)).Save(ms);
                            break;
                        case "filter":
                            //TODO
                            imageToRotate.Save(ms);
                            break;
                        case "flip":
                            imageToRotate.Flip(flipVertically: value == "vertical", flipBoth: value == "both");
                            break;
                        case "rotate":
                            imageToRotate.Rotate(int.Parse(value));
                            break;
                    }

                    ms.Position = 0;
                    var memoryStreamPostedFile = new MemoryStreamPostedFile(ms, newFileName);
                    string newMediaName = mediaItem.Name + queryString.Replace("?", " ").Replace("=", " ");
                    var newMediaItem = mediaService.CreateMedia(newMediaName, mediaItem.ParentId, mediaItemAlias);
                    newMediaItem.SetValue("umbracoFile", memoryStreamPostedFile);
                    mediaService.Save(newMediaItem);
                    ms.Dispose();
                    return Ok(newMediaItem.Id);
                }
            }
            return BadRequest(string.Format("Couldn't find the media item to rotate"));
        }

        /// <summary>
        /// Gets the path of the file on the server
        /// </summary>
        /// <param name="mediaItem">The item to get the path from</param>
        /// <param name="isNew">Is this a new file or an existing one?</param>
        /// <returns>The path of the file on the server</returns>
        private string GetServerFilePath(IMedia mediaItem, bool isNew)
        {
            string filePath = (string)mediaItem.Properties["umbracoFile"].Values.FirstOrDefault().EditedValue;
            if (filePath != null)
            {
                if (!filePath.StartsWith("/media/"))
                {
                    filePath = GetFilePathFromJson(filePath);
                }
                return HttpContext.Current.Server.MapPath(filePath);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the path of the existing file
        /// </summary>
        /// <param name="filePath">The json version of the file path</param>
        /// <returns>A string for the path of the file</returns>
        private static string GetFilePathFromJson(string filePath)
        {
            var jsonFileDetails = JObject.Parse(filePath);
            string src = jsonFileDetails["src"].ToString();
            filePath = src;
            return filePath;
        }
               
    }
    public class ImageFilterInstruction
    {
        public int MediaId { get; set; }
        public string QueryString { get; set; }
        public bool CreateNewMediaItem { get; set; }
    }
}

    
