using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCraftDemoApp.Models;

namespace WpfCraftDemoApp.Services
{
    public interface IService
    {
        /*
         * Input : searchText (String user wants to search)
         * Output : Returns ImageUrl Object.
         */
        Task<ImageUrlsModel> GetSearchResult(string searchText);
    }
}
