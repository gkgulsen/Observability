using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Tracing
{
    public class ServiceHelper
    {
        static HttpClient httpClient = new HttpClient();
        public async Task Work1()
        {
            using var activity = ActivitySourceProvider.Source.StartActivity();

            await MakeRequestToGoogle();

            Console.WriteLine("Work1 çalıştı");
        }

        public async Task<int> MakeRequestToGoogle()
        {
            var eventTags = new ActivityTagsCollection();

            using var activity = ActivitySourceProvider.Source.StartActivity(kind: System.Diagnostics.ActivityKind.Internal, name: "MakeRequestToGoogle");

            activity?.AddEvent(new ActivityEvent("google istek gönderildi"));

            activity?.AddTag("request.method", "GET");

            var result = await httpClient.GetAsync("https://google.com");

            var responseContent = await result.Content.ReadAsStringAsync();

            eventTags.Add("google body length:", responseContent.Length);
            activity?.AddEvent(new ActivityEvent("google istek tamamlandı", tags: eventTags));


            return responseContent.Length;
        }
    }
}
