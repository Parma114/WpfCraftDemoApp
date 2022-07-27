using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WpfCraftDemoApp.Exceptions;
using WpfCraftDemoApp.Models;

namespace WpfCraftDemoApp.Services
{
    public class FlickrService : IService
    {
        log4net.ILog log = log4net.LogManager.GetLogger(typeof(FlickrService));

        private readonly string _url;
        private HttpClient _httpClient;

        public FlickrService(string url)
        {
            _url = url;
            _httpClient = new HttpClient();
        }

        public FlickrService(string url, HttpClient httpClient)
        {
            _url = url;
            _httpClient = httpClient;
        }


        // Takes searchText as input and returns ImageUrl Object.
        public async Task<ImageUrlsModel> GetSearchResult(string searchText)
        {
            ImageUrlsModel imageUrl = new ImageUrlsModel();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_url + "/?tags=" + searchText + "&format=json&nojsoncallback=1");

                imageUrl = JsonDeserialize(await response.Content.ReadAsStringAsync());
                imageUrl.status = response.StatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw new CustomHttpResponseException(ex.Message, ex.InnerException);
            }

            return imageUrl;
        }

        // Deserializes the Json string into ImageUrl Object.
        internal ImageUrlsModel JsonDeserialize(string jsonString)
        {
            log.Info("Starting the Json Deserialization.");
            ImageUrlsModel imageUrl = new ImageUrlsModel();
            try
            {
                dynamic json = JsonConvert.DeserializeObject(jsonString);

                JToken token = json["items"];
                int length = json["items"].Count;
                imageUrl.Urls = new string[length];

                for (int i = 0; i < length; i++)
                {
                    var item = json["items"][i];
                    imageUrl.Urls[i] = item["media"]["m"];
                }
            }
            catch (Exception ex)
            {
                log.Error("Deserialization failed. See the Exception.");
                throw ex;
            }

            log.Info("Deserialization Done. Returning...");

            return imageUrl;
        }
    }
}
