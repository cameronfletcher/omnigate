namespace Omnibill.DocumentUploader
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    internal class Program
    {
        // LINK (Cameron): https://tools.ietf.org/html/rfc2387#section-4
        public static async Task Main(string[] args)
        {
            // json
            var data = new Data("invoice", "bob");
            using var jsonContent = JsonContent.Create(data);

            // pdf
            using var stream = new FileStream("raw.pdf", FileMode.Open);
            using var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("related") // not sure about this
            {
                Name = "anything",
                FileName = "123.pdf"
            };


            using var multipartContent = new MultipartRelatedContent
            {
                fileContent,
                jsonContent
            };

            var requestUri = "http://localhost:5000/documents";

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                "token");

            var result = await client.PostAsync(requestUri, multipartContent);

            Console.WriteLine($"Response : {result.StatusCode}");
        }

        internal record Data(string Type, string Name);
    }
}
