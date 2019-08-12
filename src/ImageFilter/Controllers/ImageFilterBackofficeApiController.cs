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
using ImageProcessor.Imaging.Filters.Photo;
using Newtonsoft.Json;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Core.Services;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Constants = Umbraco.Core.Constants;
using File = System.IO.File;

namespace ImageFilter.Controllers
{
    [PluginController("ImageFilter")]
    public class ImageFilterBackofficeApiController : UmbracoAuthorizedJsonController
    {
        private IMediaPathScheme _mediaPathScheme;

        public ImageFilterBackofficeApiController(IMediaPathScheme mediaPathScheme)
        {
            _mediaPathScheme = mediaPathScheme;
        }

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
        public IHttpActionResult CreateNewMedia(ImageFilterInstruction imageFilterInstruction)
        {
            var mediaId = imageFilterInstruction.MediaId;
            var queryString = imageFilterInstruction.QueryString;
            var mediaService = Current.Services.MediaService;
            var mediaTypeService = Current.Services.MediaTypeService;

            // get mediaItem
            var mediaItem = mediaService.GetById(mediaId);
            var mediaItemAlias = mediaItem.ContentType.Alias;

            var umbracoFile = mediaItem.GetValue<string>("umbracoFile");
            if (String.IsNullOrEmpty(umbracoFile))
            {
                return BadRequest(string.Format("Couldn't retrieve the umbraco file details of the item to adjust"));
            }

            bool isNew = mediaItem.Id <= 0;
            string serverFilePath = GetServerFilePath(mediaItem, isNew);
            if (serverFilePath != null)
            {
                FileInfo fileInfo = new FileInfo(serverFilePath);
                var fileName = fileInfo.Name;

                string mediaPath = "/media/" + _mediaPathScheme.GetFilePath(Current.MediaFileSystem, Guid.NewGuid(),
                    new Guid("1df9f033-e6d4-451f-b8d2-e0cbc50a836f"), fileName);

                string newFilePath = HttpContext.Current.Server.MapPath(mediaPath);

                using (ImageFactory imageFactory = new ImageFactory(false))
                {
                    var imageToAdjust = imageFactory.Load(serverFilePath);
                    NameValueCollection settings = HttpUtility.ParseQueryString(imageFilterInstruction.QueryString);

                    string setting = settings.GetKey(0);
                    string value = settings.Get(0);

                    switch (setting)
                    {
                        case "brightness":
                            imageToAdjust.Brightness(int.Parse(value)).Save(newFilePath);
                            break;
                        case "contrast":
                            imageToAdjust.Contrast(int.Parse(value)).Save(newFilePath);
                            break;
                        case "filter":
                            switch (value)
                            {
                                case "gotham":
                                    imageToAdjust.Filter(MatrixFilters.Gotham).Save(newFilePath);
                                    break;
                                case "invert":
                                    imageToAdjust.Filter(MatrixFilters.Invert).Save(newFilePath);
                                    break;
                                case "polaroid":
                                    imageToAdjust.Filter(MatrixFilters.Polaroid).Save(newFilePath);
                                    break;
                                case "blackwhite":
                                    imageToAdjust.Filter(MatrixFilters.BlackWhite).Save(newFilePath);
                                    break;
                                case "greyscale":
                                    imageToAdjust.Filter(MatrixFilters.GreyScale).Save(newFilePath);
                                    break;
                                case "lomograph":
                                    imageToAdjust.Filter(MatrixFilters.Lomograph).Save(newFilePath);
                                    break;
                                case "sepia":
                                    imageToAdjust.Filter(MatrixFilters.Sepia).Save(newFilePath);
                                    break;
                                case "comic":
                                    imageToAdjust.Filter(MatrixFilters.Comic).Save(newFilePath);
                                    break;
                                case "hisatch":
                                    imageToAdjust.Filter(MatrixFilters.HiSatch).Save(newFilePath);
                                    break;
                                case "losatch":
                                    imageToAdjust.Filter(MatrixFilters.LoSatch).Save(newFilePath);
                                    break;
                                default:
                                    imageToAdjust.Save(newFilePath);
                                    break;
                            }
                            break;
                        case "flip":
                            imageToAdjust.Flip(flipVertically: value == "vertical", flipBoth: value == "both").Save(newFilePath);
                            break;
                        case "rotate":
                            imageToAdjust.Rotate(int.Parse(value)).Save(newFilePath);
                            break;
                    }

                    string newMediaName = mediaItem.Name + queryString.Replace("?", " ").Replace("=", " ");


                    var newMediaId = CreateMediaItem(mediaService, mediaTypeService, mediaItem.ParentId, mediaItemAlias,
                        Guid.NewGuid(), newMediaName, mediaPath);

                    return Ok(newMediaId);
                }
            }
            return BadRequest(string.Format("Couldn't find the media item to adjust"));
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

        private int CreateMediaItem(IMediaService service, IMediaTypeService mediaTypeService,
            int parentFolderId, string nodeTypeAlias, Guid key, string nodeName, 
            string mediaPath, bool checkForDuplicateName = false)
        {
            //if the item with the exact id exists we cannot install it (the package was probably already installed)
            if (service.GetById(key) != null)
                return -1;

            //cannot continue if the media type doesn't exist
            var mediaType = mediaTypeService.Get(nodeTypeAlias);
            if (mediaType == null)
            {
                Current.Logger.Warn(typeof(ImageFilterBackofficeApiController), "Could not create media, the {NodeTypeAlias} media type is missing, the Clean Starter Kit package will not function correctly", nodeTypeAlias);
                return -1;
            }

            var isDuplicate = false;

            if (checkForDuplicateName)
            {
                IEnumerable<IMedia> children;
                if (parentFolderId == -1)
                {
                    children = service.GetRootMedia();
                }
                else
                {
                    var parentFolder = service.GetById(parentFolderId);
                    if (parentFolder == null)
                    {
                        Current.Logger.Warn(typeof(ImageFilterBackofficeApiController), "No media parent found by Id {ParentFolderId} the media item {NodeName} cannot be installed", parentFolderId, nodeName);
                        return -1;
                    }

                    children = service.GetPagedChildren(parentFolderId, 0, int.MaxValue, out long totalRecords);
                }
                foreach (var m in children)
                {
                    if (m.Name == nodeName)
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }

            if (isDuplicate) return -1;

            if (parentFolderId != -1)
            {
                var parentFolder = service.GetById(parentFolderId);
                if (parentFolder == null)
                {
                    Current.Logger.Warn(typeof(ImageFilterBackofficeApiController), "No media parent found by Id {ParentFolderId} the media item {NodeName} cannot be installed", parentFolderId, nodeName);
                    return -1;
                }
            }

            var media = service.CreateMedia(nodeName, parentFolderId, nodeTypeAlias);
            if (nodeTypeAlias != "folder")
                media.SetValue("umbracoFile", JsonConvert.SerializeObject(new ImageCropperValue { Src = mediaPath }));
            if (key != Guid.Empty)
            {
                media.Key = key;
            }
            service.Save(media);
            return media.Id;
        }

    }
    public class ImageFilterInstruction
    {
        public int MediaId { get; set; }
        public string QueryString { get; set; }
    }
}


