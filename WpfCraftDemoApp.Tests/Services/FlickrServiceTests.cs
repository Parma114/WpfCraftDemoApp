using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfCraftDemoApp.Exceptions;
using WpfCraftDemoApp.Models;
using WpfCraftDemoApp.Services;

namespace WpfCraftDemoApp.Tests.Services
{
    internal class JsonObject
    {
        public JsonObject(int num)
        {
            items = new Item[num];
            for (int i = 0; i < num; i++)
            {
                items[i] = new Item(i);
            }
        }
        public Item[] items { get; set; }
    }

    internal class Item
    {
        public Item(int i)
        {
            media = new Media(i);
        }
        public Media media { get; set; }
    }

    internal class Media
    {
        public Media(int i)
        {
            m = "URL:" + i.ToString();
        }
        public string m { get; set; }
    }

    [TestFixture]
    public class FlickrServiceTests
    {
        Mock<HttpMessageHandler> _mockHttpHandler;
        string _apiEndPoint = "http://localhost:80";

        public void Setup(object jsonObject, HttpStatusCode statusCode)
        {
            var _jsonObject = JsonConvert.SerializeObject(jsonObject);

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = statusCode;
            httpResponse.Content = new StringContent(_jsonObject, Encoding.UTF8, "application/json");

            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _mockHttpHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);
        }

        [Test]
        public async Task GetSearchResultReturnsSuccess_WhenAPIResponseIsProper()
        {
            JsonObject jsonObject = new JsonObject(3);
            Setup(jsonObject, HttpStatusCode.OK);

            IService service = new FlickrService(_apiEndPoint, new HttpClient(_mockHttpHandler.Object));
            ImageUrlsModel result = await service.GetSearchResult("");

            Assert.IsNotNull(result);
            Assert.That(result.status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Urls[0], Is.EqualTo("URL:0"));
            Assert.That(result.Urls[1], Is.EqualTo("URL:1"));
            Assert.That(result.Urls[2], Is.EqualTo("URL:2"));
        }

        [Test]
        public void GetSearchResultThrowsException_WhenResponseIsInvalid()
        {
            Item item = new Item(1);
            Setup(item, HttpStatusCode.OK);

            IService service = new FlickrService(_apiEndPoint, new HttpClient(_mockHttpHandler.Object));

            Assert.ThrowsAsync<CustomHttpResponseException>(async () => await service.GetSearchResult(""));
        }

        [Test]
        public void GetSearchResultThrowsWebException()
        {
            var webresponse = new Mock<HttpWebResponse>();
            webresponse.Setup(c => c.StatusCode).Returns(HttpStatusCode.Unauthorized);
            WebException webException = new WebException("UnAutherised", null, WebExceptionStatus.ProtocolError, webresponse.Object);
            Exception ex = new Exception("Custom Unit Test Exception", webException);

            var _mockHttpHandler = new Mock<HttpMessageHandler>();
            _mockHttpHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .Returns(() => { throw ex; });

            IService service = new FlickrService(_apiEndPoint, new HttpClient(_mockHttpHandler.Object));

            Assert.ThrowsAsync<CustomHttpResponseException>(async () => await service.GetSearchResult(""));
        }

        [Test]
        public async Task GetSearchResultReturnsEmpty_WhenContentIsNull()
        {
            JsonObject jsonObject = new JsonObject(0);
            Setup(jsonObject, HttpStatusCode.NoContent);

            IService service = new FlickrService(_apiEndPoint, new HttpClient(_mockHttpHandler.Object));
            ImageUrlsModel result = await service.GetSearchResult("");

            Assert.That(result.status, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.IsEmpty(result.Urls);
        }
    }
}
