namespace WpfCraftDemoApp.Tests.ViewModelTest
{
    using NUnit.Framework;
    using WpfCraftDemoApp.ViewModels;
    using Moq;
    using WpfCraftDemoApp.Services;
    using WpfCraftDemoApp.Models;
    using System;
    using System.Threading;
    using System.Windows;
    using System.Net;
    using WpfCraftDemoApp.Exceptions;

    public class ViewModelTests
    {
        string _apiEndPoint = "http://localhost:80";
        Mock<IService> mockService = new Mock<IService>();

        [SetUp]
        public void Setup()
        {
            mockServiceSetup(4, HttpStatusCode.OK);
        }

        public void mockServiceSetup(int numberOfImages, HttpStatusCode statusCode)
        {
            mockService.Setup(m => m.GetSearchResult(It.IsAny<string>())).ReturnsAsync(() =>
            {
                ImageUrlsModel imageUrls = new ImageUrlsModel();
                imageUrls.Urls = new string[numberOfImages];
                imageUrls.status = statusCode;
                for (int i = 0; i < numberOfImages; i++)
                {
                    imageUrls.Urls[i] = "Url : " + (i + 1).ToString();
                }
                return imageUrls;
            });
        }

        void RunTestWithDispatcher(Action testAction)
        {
            var thread = new Thread(() =>
            {
                testAction();
            });

            thread.IsBackground = true;
            thread.TrySetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        [Test]
        public void CanExecuteReturnsTrueOnValidSearchtext()
        {
            ViewModel viewModel = new ViewModel(mockService.Object);
            
            viewModel.SearchText = "Tom and Jerry";
            Assert.IsTrue(viewModel.SearchCommand.CanExecute(null));

            viewModel.SearchText = "4 dogs";
            Assert.IsTrue(viewModel.SearchCommand.CanExecute(null));

            viewModel.SearchText = "Louis IV";
            Assert.IsTrue(viewModel.SearchCommand.CanExecute(null));
        }

        [Test]
        public void CanExecuteReturnsFalseOnNullOrWhiltespaceSearchText()
        {
            ViewModel viewModel = new ViewModel(mockService.Object);

            viewModel.SearchText = "";
            Assert.IsFalse(viewModel.SearchCommand.CanExecute(null));

            viewModel.SearchText = null;
            Assert.IsFalse(viewModel.SearchCommand.CanExecute(null));

            viewModel.SearchText = string.Empty;
            Assert.IsFalse(viewModel.SearchCommand.CanExecute(null));
        }

        [Test]
        public void CanExecuteReturnsFalseOnInvalidSearchText()
        {
            ViewModel viewModel = new ViewModel(mockService.Object);

            viewModel.SearchText = "Tom & Jerry";
            Assert.IsFalse(viewModel.SearchCommand.CanExecute(null));

            viewModel.SearchText = "4-dogs";
            Assert.IsFalse(viewModel.SearchCommand.CanExecute(null));

            viewModel.SearchText = "Louis (IV)";
            Assert.IsFalse(viewModel.SearchCommand.CanExecute(null));
        }

        [Test]
        public void ExecuteSetsTheImageUrlsToCollection()
        {
            RunTestWithDispatcher(() =>
            {
                Window mainwindow = new MainWindow();
                var viewModel = new ViewModel(mockService.Object, mainwindow.Dispatcher, new ViewModelHelper());
                mainwindow.DataContext = viewModel;

                viewModel.SearchText = "Nature";
                viewModel.SearchCommand.Execute(null);

                Assert.IsNotEmpty(viewModel.ImageUrlCollection);
                Assert.AreEqual(4, viewModel.ImageUrlCollection.Count);
            });
        }

        [Test]
        public void ExecuteSetsEmptyInImageUrlCollectionWhenStatusIsNotOk()
        {
            RunTestWithDispatcher(() =>
            {
                mockServiceSetup(0, HttpStatusCode.NoContent);
                Window mainwindow = new MainWindow();
                var viewModel = new ViewModel(mockService.Object, mainwindow.Dispatcher, new ViewModelHelper());
                mainwindow.DataContext = viewModel;

                viewModel.SearchText = "Nature";
                viewModel.SearchCommand.Execute(null);

                Assert.IsEmpty(viewModel.ImageUrlCollection);
            });
        }

        [Test]
        public void ExecuteSetsCollectionEmptyWhenServiceThrowsException()
        {
            Mock<IViewModelHelper> mockVMHelper = new Mock<IViewModelHelper>();
            mockVMHelper.Setup(m => m.ShowMessageBox(It.IsAny<string>()));
            mockService.Setup(m => m.GetSearchResult(It.IsAny<string>())).ThrowsAsync(new CustomHttpResponseException());
            RunTestWithDispatcher(() =>
            {
                Window mainwindow = new MainWindow();
                var viewModel = new ViewModel(mockService.Object, mainwindow.Dispatcher, mockVMHelper.Object);
                mainwindow.DataContext = viewModel;

                viewModel.SearchText = "Nature";
                viewModel.SearchCommand.Execute(null);

                Assert.IsEmpty(viewModel.ImageUrlCollection);
            });
        }



    }
}
