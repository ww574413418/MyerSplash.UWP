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
            if (jumpList.Items.Count > 0)
            {
                return;
            }

            jumpList.Items.Clear();

            //var randomItem = JumpListItem.CreateWithArguments(Constant.RANDOM_KEY, "Random");
            //randomItem.Logo = new Uri("ms-appx:///Assets/Icon/dice.png");
            var searchItem = JumpListItem.CreateWithArguments(Constant.SEARCH_KEY, "Search");
            searchItem.Logo = new Uri("ms-appx:///Assets/Icon/search.png");

            //jumpList.Items.Add(randomItem);
            jumpList.Items.Add(searchItem);

            await jumpList.SaveAsync();
        }
    }
}
