using System.Net.Mime;
using System.Text;
using FluentAssertions;

using AdvertisementApi.Core.Interfaces;
using AdvertisementApi.Contracts;
using AdvertisementApi.Core;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace AdvertisementApiTests
{
    public sealed class UnitTest1
    {
        public UnitTest1()
        {
        }

        private async Task<T> GetJson<T>(HttpResponseMessage response)
        {
            string jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonData)!;
        }

        [Fact]
        public void TestParser()
        {
            var app = new WebAppTest();
            IServiceScopeFactory scopeFactory = app.Services.GetService<IServiceScopeFactory>()!;

            using (var scope = scopeFactory.CreateScope())
            {
                IAdvertisementDataParser? parser = scope.ServiceProvider.GetRequiredService<IAdvertisementDataParser>();

                parser.Should().NotBeNull();

                GetTestData(out string advPlatform1, out string advPlatform2, out string advPlatform3,
                    out string advPlatform4, out string input);

                parser.TryParse(GetTestFile(), out string message, out var dataList);

                dataList.Count.Should().Be(7);

                dataList[0].platform.Should().Be(advPlatform1);
                dataList[1].platform.Should().Be(advPlatform2);
                dataList[2].platform.Should().Be(advPlatform2);
                dataList[3].platform.Should().Be(advPlatform3);
                dataList[4].platform.Should().Be(advPlatform3);
                dataList[5].platform.Should().Be(advPlatform3);
                dataList[6].platform.Should().Be(advPlatform4);
            }
        }

        [Fact]
        public async Task TestApi1()
        {
            var app = new WebAppTest();
            var client = app.CreateClient();

            GetTestData(out string advPlatform1, out string advPlatform2, out string advPlatform3,
                out string advPlatform4, out string input);

            HttpResponseMessage response;

            using (var form = new MultipartFormDataContent())
            {
                form.Add(new ByteArrayContent(GetTestFile()), "file", "file");

                response = await client.PostAsync("api/adv/add", form);

                var r1 = await GetJson<AdvertisementDataUploadResponse>(response);

                response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                r1.message.Should().Be(string.Empty);
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("api/adv/get", UriKind.Relative),
                Content = new StringContent(
                    "[\"/ru\"]",
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json),
            };

            response = await client.SendAsync(request);
            var r2 = await GetJson<AdvertisementDataResponse>(response);
            r2.advPlat.Length.Should().Be(1);
            r2.advPlat[0].Should().Be(advPlatform1);
        }

        private static void GetTestData(out string advPlatform1, out string advPlatform2,
            out string advPlatform3, out string advPlatform4, out string input)
        {
            advPlatform1 = "Yandex.Direct";
            advPlatform2 = "Rendinskiy rabochi";
            advPlatform3 = "Gazeta uralskih moskvichey";
            advPlatform4 = "Krutaya reklama";

            input =
                $"{advPlatform1}:/ru\r\n" +
                $"{advPlatform2}:/ru/svrd/revda,/ru/svrd/pervik\r\n" +
                $"{advPlatform3}:/ru/msk,/ru/permobl,/ru/chelobl\r\n" +
                $"{advPlatform4}:/ru/svrd";
        }

        private static byte[] GetTestFile()
        {
            GetTestData(out string advPlatform1, out string advPlatform2, out string advPlatform3,
                out string advPlatform4, out string input);

            return Encoding.UTF8.GetBytes(input);
        }
    }
}
