using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CLARiNET
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
           this IEnumerable<TSource> source, int batchSize)
        {
            var items = new TSource[batchSize];
            var count = 0;

            foreach (var item in source)
            {
                items[count++] = item;
                if (count == batchSize)
                {
                    yield return items;

                    items = new TSource[batchSize];
                    count = 0;
                }
            }

            if (count > 0)
                yield return items.Take(count);
        }


        public static void BasicAuth(this HttpRequestMessage http, string username, string password)
        {
            var authenticationString = $"{username}:{password}";
            var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
            http.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
        }
    }
}
