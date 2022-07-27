namespace WpfCraftDemoApp.ViewModels
{
    public interface IViewModelHelper
    {
        /*
         *  Checks if the search text is not null or empty.
         *  Checks if the text Doesn't contain any special characters.
         *  Returns true if above conditions met else throws the exception of type NullOrWhiteSpaceException & SpecialCharacterException.
         */
        bool IsValidsearchText(string searchText);

        
        //Shows message box
        void ShowMessageBox(string message);
    }
}
