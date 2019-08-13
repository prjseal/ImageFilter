
using System.Collections.Generic;

namespace ImageFilter.Models
{
    public class AvailableProcessorModel
    {        
        public string Name { get; set; }
        public bool IsValid { get; set; }
        public string PathToView
        {
            get
            {
                return string.Format("/app_plugins/imagefilter/controls/{0}.html", Name);
            }
        }

        public string QueryStringEntryTemplate
        {
            get {
                switch (Name.ToLower())
                {
                    case "alpha":
                        return "alpha={0}";
                    case "brightness":
                        return "brightness={0}";
                    case "contrast":
                        return "contrast={0}";
                    case "filter":
                        return "filter={0}";
                    case "flip":
                        return "flip={0}";
                    case "rotate":
                        return "rotate={0}";
                    default:
                        return string.Empty;
                }
            }
        }

        public List<object> DefaultValues
        {
            get
            {
                switch (Name.ToLower())
                {
                    case "alpha":
                        return new List<object> { 50 };
                    case "brightness":
                        return new List<object> { 0 };
                    case "contrast":
                        return new List<object> { 0 };
                    case "filter":
                        return new List<object> { "none" };
                    case "flip":
                        return new List<object> { "none" };
                    case "rotate":
                        return new List<object> { 0 };
                    default:
                        return null;
                }
            }
        }
    }
}
