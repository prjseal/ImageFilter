
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
                    default:
                        return string.Empty;
                }
            }
        }

        public string[] DefaultValues
        {
            get
            {
                switch (Name.ToLower())
                {
                    case "alpha":
                        return new[] { "50" };
                    case "brightness":
                        return new[] { "0" };
                    case "contrast":
                        return new[] { "25" };
                    default:
                        return null;
                }
            }
        }
    }
}
