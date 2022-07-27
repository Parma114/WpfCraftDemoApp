using System.Windows;
using WpfCraftDemoApp.Exceptions;
using WpfCraftDemoApp.Resources;

namespace WpfCraftDemoApp.ViewModels
{
    public class ViewModelHelper : IViewModelHelper
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ViewModelHelper));

        
        public bool IsValidsearchText(string searchText)
        {
            string pecialChar = Resource.InputNotValid;
            string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_,";

            if (string.IsNullOrWhiteSpace(searchText))
            {
                throw new NullOrWhiteSpaceException();
            }

            foreach (var item in specialChar)
            {
                if (searchText.IndexOf(item) != -1)
                {
                    //log.Error("String contains special character.");
                    throw new SpecialCharacterException();
                }
            }

            return true;
        }

        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }
    }
}