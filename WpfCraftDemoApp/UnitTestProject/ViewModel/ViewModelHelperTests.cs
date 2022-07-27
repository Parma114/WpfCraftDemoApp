using NUnit.Framework;
using WpfCraftDemoApp.Exceptions;
using WpfCraftDemoApp.ViewModel;

namespace WpfCraftDemoApp.Tests.ViewModel
{
    internal class ViewModelHelperTests
    {
        [Test]
        public void IsValidSearchTextReturnsTrue()
        {
            string searchText1 = "1 Dog";
            string searchText2 = "The statue of liberty";

            Assert.IsTrue(ViewModelHelper.IsValidsearchText(searchText1));
            Assert.IsTrue(ViewModelHelper.IsValidsearchText(searchText2));
        }

        [Test]
        public void IsValidSearchTextThrowsNullOrWhiteSpaceException()
        {
            string searchText1 = null;
            string searchText2 = string.Empty;
            string searchText3 = "";

            Assert.Throws<NullOrWhiteSpaceException>(() => ViewModelHelper.IsValidsearchText(searchText1));
            Assert.Throws<NullOrWhiteSpaceException>(() => ViewModelHelper.IsValidsearchText(searchText2));
            Assert.Throws<NullOrWhiteSpaceException>(() => ViewModelHelper.IsValidsearchText(searchText3));
        }

        [Test]
        public void IsValidSearchTextThrowsSpecialCharacterException()
        {
            string searchText1 = "Tom & Jerry";
            string searchText2 = "(Cat)";
            string searchText3 = "Cat, Dog?";

            Assert.Throws<SpecialCharacterException>(() => ViewModelHelper.IsValidsearchText(searchText1));
            Assert.Throws<SpecialCharacterException>(() => ViewModelHelper.IsValidsearchText(searchText2));
            Assert.Throws<SpecialCharacterException>(() => ViewModelHelper.IsValidsearchText(searchText3));
        }
    }
}
