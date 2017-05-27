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

            var searchItem = JumpListItem.CreateWithArguments(Key.ACTION_KEY, Value.SEARCH);
            jumpList.Items.Add(searchItem);

            await jumpList.SaveAsync();
        }
    }
}
