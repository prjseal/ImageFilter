using System.IO;
using System.Web;

namespace ImageFilter.Models
{
    public class MemoryStreamPostedFile : HttpPostedFileBase
    {

        public MemoryStreamPostedFile(MemoryStream ms, string fileName)
        {
            this.ContentLength = (int)ms.Length;
            this.FileName = fileName;
            this.InputStream = ms;
        }
        public override int ContentLength
        {
            get;
        }

        public override string FileName { get; }

        public override Stream InputStream { get; }


    }
}
