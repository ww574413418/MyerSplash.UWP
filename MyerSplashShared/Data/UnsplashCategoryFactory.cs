using JP.Utils.Debug;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace MyerSplash.Data
{
    public static class UnsplashCategoryFactory
    {
        public async static Task<ObservableCollection<UnsplashCategory>> GetCategoriesAsync()
        {
            ObservableCollection<UnsplashCategory> categories = new ObservableCollection<UnsplashCategory>();
            categories.Insert(0, new UnsplashCategory()
            {
                Title = "Featured",
            });
            categories.Insert(0, new UnsplashCategory()
            {
                Title = "New",
            });
            categories.Insert(0, new UnsplashCategory()
            {
                Title = "Random",
            });
            try
            {
                var assets = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
                var json = await assets.GetFolderAsync("Json");
                var file = await json.GetFileAsync("built_in_categories.json");
                var text = await FileIO.ReadTextAsync(file);
                var list = JsonConvert.DeserializeObject<ObservableCollection<UnsplashCategory>>(text);
                foreach (var category in list)
                {
                    categories.Add(category);
                }
            }
            catch (Exception e)
            {
                await Logger.LogAsync(e);
            }

            return categories;
        }
    }
}
