namespace Omnibill.WebAPI.Controllers
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;
    using Omnibill.WebAPI.Sdk;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)] // TODO (Cameron): Intercept via middleware to add problem details.
    //[ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class DocumentsController : ControllerBase
    {
        private readonly ILogger logger;

        [Flags]
        private enum DocumentContent
        {
            None = 1 << 0,
            File = 1 << 1,
            Metatada = 1 << 2,
        }

        public DocumentsController(ILogger<DocumentsController> logger)
        {
            this.logger = logger;
        }

        // LINK (Cameron): https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0#upload-large-files-with-streaming
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> Post()
        {
            // validation
            if (string.IsNullOrEmpty(this.Request.ContentType))
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Title = "error",
                    });
            }

            // get client and tenant from token
            // get operation from invocation context

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(this.Request.ContentType), 73);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            var documentContent = default(DocumentContent);


            while (section != null)
            {
                switch (section.ContentType)
                {
                    case var contentType when contentType.Contains(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase):
                        var metadata = await JsonSerializer.DeserializeAsync<DocumentMetadata>(section.Body);
                        documentContent |= DocumentContent.Metatada;
                        break;

                    // TODO (Cameron): Revisit.
                    // LINK (Cameron): https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/mvc/models/file-uploads/samples/3.x/SampleApp/Utilities/FileHelpers.cs
                    case var contentType when contentType.Contains(MediaTypeNames.Application.Pdf, StringComparison.OrdinalIgnoreCase):
                        using (var fileStream = new FileStream("upload.pdf", FileMode.Create))
                        {
                            await section.Body.CopyToAsync(fileStream);
                        }
                        documentContent |= DocumentContent.File;
                        break;

                    default:
                        return BadRequest(); // cannot support media type section.ContentType
                }

                // drain any remaining section body that hasn't been consumed and read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            if (documentContent.HasFlag(DocumentContent.Metatada))
            {

            }

            if (documentContent.HasFlag(DocumentContent.File))
            {

            }

            return CreatedAtAction(nameof(Get), "2");
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Document))]
        public IActionResult Get(int id)
        {
            var stream = new FileStream(@"temp.pdf", FileMode.Open);
            return File(stream, "application/pdf", "invoice.pdf");
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        private class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
        {
            public void OnResourceExecuting(ResourceExecutingContext context)
            {
                var factories = context.ValueProviderFactories;
                factories.RemoveType<FormValueProviderFactory>();
                factories.RemoveType<FormFileValueProviderFactory>();
                factories.RemoveType<JQueryFormValueProviderFactory>();
            }

            public void OnResourceExecuted(ResourceExecutedContext context)
            {
            }
        }

        private record DocumentMetadata(string Type, string Name);
    }
}
