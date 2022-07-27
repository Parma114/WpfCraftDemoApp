using System.Net;

namespace WpfCraftDemoApp.Models
{
    public class ImageUrlsModel
    {
        public HttpStatusCode status;

        public string[] Urls { get; set; }
    }
}
