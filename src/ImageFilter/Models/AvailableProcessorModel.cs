
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
    }
}
