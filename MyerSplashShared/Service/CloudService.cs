using JP.API;
using JP.Utils.Network;
using MyerSplashShared.API;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyerSplashShared.Service
{
    public class CloudService : IService
    {
        private List<KeyValuePair<string, string>> GetDefaultParam()
        {
            var param = new List<KeyValuePair<string, string>>();
            param.Add(new KeyValuePair<string, string>("client_id", Request.AppKey));
            return param;
        }

        internal async Task<CommonRespMsg> GetImagesAsync(int page, int pageCount, CancellationToken token, string url)
        {
            var param = GetDefaultParam();
            param.Add(new KeyValuePair<string, string>("page", page.ToString()));
            param.Add(new KeyValuePair<string, string>("per_page", pageCount.ToString()));

            var result = await HttpRequestSender.SendGetRequestAsync(Request.AppendParamsToUrl(url, param), token);
            return result;
        }

        internal async Task<CommonRespMsg> GetRandomImagesAsync(int count, CancellationToken token)
        {
            var param = GetDefaultParam();
            param.Add(new KeyValuePair<string, string>("count", count.ToString()));

            var result = await HttpRequestSender.SendGetRequestAsync(Request.AppendParamsToUrl(Request.GetRandomImages, param), token);
            return result;
        }

        internal async Task<CommonRespMsg> SearchImagesAsync(int page, int pageCount, CancellationToken token, string query)
        {
            var param = GetDefaultParam();
            param.Add(new KeyValuePair<string, string>("page", page.ToString()));
            param.Add(new KeyValuePair<string, string>("per_page", pageCount.ToString()));
            param.Add(new KeyValuePair<string, string>("query", query));

            var result = await HttpRequestSender.SendGetRequestAsync(Request.AppendParamsToUrl(Request.SearchImages, param), token);
            return result;
        }

        internal async Task<CommonRespMsg> SearchImagesAsync(int page, int pageCount, string query, CancellationToken token)
        {
            var param = GetDefaultParam();
            param.Add(new KeyValuePair<string, string>("page", page.ToString()));
            param.Add(new KeyValuePair<string, string>("per_page", pageCount.ToString()));
            param.Add(new KeyValuePair<string, string>("query", query));

            var result = await HttpRequestSender.SendGetRequestAsync(Request.AppendParamsToUrl(Request.SearchImages, param), token);
            return result;
        }

        internal async Task<CommonRespMsg> GetImageDetailAsync(string id, CancellationToken token)
        {
            var param = GetDefaultParam();
            var url = Request.AppendParamsToUrl(Request.GetImageDetail + id, param);

            var result = await HttpRequestSender.SendGetRequestAsync(url, token);
            return result;
        }
    }
}