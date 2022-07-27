using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows.Input;
using System.Windows.Threading;
using WpfCraftDemoApp.Exceptions;
using WpfCraftDemoApp.Models;
using WpfCraftDemoApp.Resources;
using WpfCraftDemoApp.Services;

namespace WpfCraftDemoApp.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private log4net.ILog log = log4net.LogManager.GetLogger(typeof(ViewModel));
        private IService _apiService;
        private Dispatcher _dispatcher;
        private IViewModelHelper _viewModelHelper;
        private ImageUrlsModel _imageUrls;


        // Property to bind with search text in the UI.
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                NotifyPropertyChanged("SearchText");
            }
        }


        // Property to bind with the search status we are showing in the UI.
        private string _searchStatus;
        public string SearchStatus
        {
            get { return _searchStatus; }
            set
            {
                _searchStatus = value;
                NotifyPropertyChanged("SearchStatus");
            }
        }


        // Property to bind with attached property ScrollBehaviour.
        private bool _autoScrollToTop;
        public bool AutoScrollToTop
        {
            get { return _autoScrollToTop; }
            set
            {
                _autoScrollToTop = value;
                NotifyPropertyChanged("AutoScrollToTop");
            }
        }


        // Observable collection that is bind to the VirtulizingWrapPanel to show the images.
        private ObservableCollection<string> _imageUrl;
        public ObservableCollection<string> ImageUrlCollection
        {
            get
            {
                return _imageUrl;
            }
            set
            {
                _imageUrl = value;
                NotifyPropertyChanged("ImageUri");
            }
        }


        // Command binded to search button & enter event of search textbox.
        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new RelayCommand(StartSearch, CanStartSearch);
                return _searchCommand;
            }
        }


        // Constructor
        public ViewModel(IService service)
        {
            _apiService = service;
            ImageUrlCollection = new ObservableCollection<string>();
            _viewModelHelper = new ViewModelHelper();
            AutoScrollToTop = false;
        }

        public ViewModel(IService service, Dispatcher dispatcher, IViewModelHelper viewModelHelper)
        {
            _apiService = service;
            _dispatcher = dispatcher;
            _viewModelHelper = viewModelHelper;
            ImageUrlCollection = new ObservableCollection<string>();
            AutoScrollToTop = false;
        }

        // Notifies the UI or ViewModel that the property has changed.
        private void NotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        public event PropertyChangedEventHandler PropertyChanged;



        // Function passed as callback to RelayCommand.
        private bool CanStartSearch(object parameter)
        {
            try
            {
                if (_viewModelHelper.IsValidsearchText(SearchText))
                {
                    SearchStatus = string.Empty;
                    return true;
                }
            }
            catch (NullOrWhiteSpaceException)
            {
                SearchStatus = Resource.EnterTextForSearch;
            }
            catch (SpecialCharacterException)
            {
                SearchStatus = Resource.InputNotValid;
            }
            catch { }

            return false;
        }


        // Starts the API call and sets the results to observable collection. Passed as callback to RelayCommand.
        private async void StartSearch(object parameter)
        {
            ImageUrlCollection.Clear();
            SearchStatus = string.Format(Resource.SearchingImages, SearchText);

            try
            {
                log.Info("Starting the search for " + SearchText);
                _imageUrls = await _apiService.GetSearchResult(SearchText);
            }
            catch(CustomHttpResponseException cex)
            {
                _viewModelHelper.ShowMessageBox(Resource.CouldNotGetResponse);
                return;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                _viewModelHelper.ShowMessageBox(Resource.SomethingWentWrong);
                return;
            }

            _dispatcher.Invoke(
                () =>
                {
                    try
                    {
                        log.Info("StartSearch Dispatcher Running.");
                        AutoScrollToTop = true;

                        // If we got OK status then load the urls to observable collection object.
                        if (_imageUrls.status == HttpStatusCode.OK)
                        {
                            log.Info("Got Ok status code for " + SearchText);
                            SearchStatus = string.Format(Resource.SearchImagesResults, SearchText);
                            SearchText = string.Empty;

                            foreach (var item in _imageUrls.Urls)
                            {
                                ImageUrlCollection.Add(item);
                            }
                        }
                        else
                        {
                            log.Error("Got status code : " + _imageUrls.status.ToString());
                            SearchStatus = Resource.SomethingWentWrong;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("Caught Exception in StartSearch Dispatcher");
                        log.Error(ex.Message);
                    }
                });
        }
    }
}
