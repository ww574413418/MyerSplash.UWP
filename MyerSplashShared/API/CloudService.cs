using JP.API;
using JP.Utils.Network;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyerSplashShared.API
{
    public static class CloudService
    {
        private static List<KeyValuePair<string, string>> GetDefaultParam()
        {
            var param = new List<KeyValuePair<string, string>>();
            param.Add(new KeyValuePair<string, string>("client_id", UrlHelper.AppKey));
            return param;
        }

        public static async Task<CommonRespMsg> GetImages(int page, int pageCount, CancellationToken token, string url)
        {
            var param = GetDefaultParam();
            param.Add(new KeyValuePair<string, string>("page", page.ToString()));
            param.Add(new KeyValuePair<string, string>("per_page", pageCount.ToString()));

            var result = await HttpRequestSender.SendGetRequestAsync(UrlHelper.MakeFullUrlForGetReq(url, param), token);
            return result;
        }

        public static async Task<CommonRespMsg> GetRandomImages(int count, CancellationToken token)
        {
            var param = GetDefaultParam();
            param.Add(new KeyValuePair<string, string>("count", count.ToString()));

            var result = await HttpRequestSender.SendGetRequestAsync(UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetRandomImages, param), token);
            return result;
        }

        public static async Task<CommonRespMsg> SearchImages(int page, int pageCount, CancellationToken token, string query)
        {
            var param = GetDefaultParam();
            param.Add(new KeyValuePair<string, string>("page", page.ToString()));
            param.Add(new KeyValuePair<string, string>("per_page", pageCount.ToString()));
            param.Add(new KeyValuePair<string, string>("query", query));

            var result = await HttpRequestSender.SendGetRequestAsync(UrlHelper.MakeFullUrlForGetReq(UrlHelper.SearchImages, param), token);
            return result;
        }

        public static async Task<CommonRespMsg> GetCategories(CancellationToken token)
        {
            var param = GetDefaultParam();

            var result = await HttpRequestSender.SendGetRequestAsync(UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetCategories, param), token);
            return result;
        }

        public static async Task<CommonRespMsg> SearchImages(int page, int pageCount, string query, CancellationToken token)
        {
            var param = GetDefaultParam();
            param.Add(new KeyValuePair<string, string>("page", page.ToString()));
            param.Add(new KeyValuePair<string, string>("per_page", pageCount.ToString()));
            param.Add(new KeyValuePair<string, string>("query", query));

            var result = await HttpRequestSender.SendGetRequestAsync(UrlHelper.MakeFullUrlForGetReq(UrlHelper.SearchImages, param), token);
            return result;
        }

        public static async Task<CommonRespMsg> GetImageDetail(string id, CancellationToken token)
        {
            var param = GetDefaultParam();
            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetImageDetail + id, param);

            var result = await HttpRequestSender.SendGetRequestAsync(url, token);
            return result;
        }
    }
}
