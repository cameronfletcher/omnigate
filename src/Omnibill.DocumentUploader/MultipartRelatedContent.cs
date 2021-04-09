namespace Omnibill.DocumentUploader
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class MultipartRelatedContent : MultipartContent
    {
        private const string subType = "related";

        public MultipartRelatedContent()
            : base(subType)
        {
        }

        public MultipartRelatedContent(string boundary)
            : base(subType, boundary)
        {
        }

        public override void Add(HttpContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if (content.Headers.ContentDisposition == null)
            {
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue(subType);
            }

            base.Add(content);
        }

        public void Add(HttpContent content, string name)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be empty.", nameof(name));
            }

            AddInternal(content, name, null);
        }

        public void Add(HttpContent content, string name, string fileName)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Value cannot be empty.", nameof(fileName));
            }

            AddInternal(content, name, fileName);
        }

        private void AddInternal(HttpContent content, string name, string fileName)
        {
            if (content.Headers.ContentDisposition == null)
            {
                ContentDispositionHeaderValue header = new ContentDispositionHeaderValue(subType);
                header.Name = name;
                header.FileName = fileName;
                header.FileNameStar = fileName;

                content.Headers.ContentDisposition = header;
            }
            base.Add(content);
        }
    }
}
