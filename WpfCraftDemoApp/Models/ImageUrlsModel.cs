using System.Net;

namespace WpfCraftDemoApp.Models
{
    public class ImageUrlsModel
    {
        public HttpStatusCode status { get; set; }

        public string[] Urls { get; set; }
    }
}
