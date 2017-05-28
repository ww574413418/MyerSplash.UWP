using System;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace MyerSplash.Common
{
    public static class JumpListHelper
    {
        public static async Task SetupJumpList()
        {
            var jumpList = await JumpList.LoadCurrentAsync();
            jumpList.Items.Clear();

            var searchItem = JumpListItem.CreateWithArguments(Value.SEARCH, Value.SEARCH);
            searchItem.Logo = new Uri("ms-appx:///Assets/Icon/search.png");
            jumpList.Items.Add(searchItem);

            await jumpList.SaveAsync();
        }
    }
}
