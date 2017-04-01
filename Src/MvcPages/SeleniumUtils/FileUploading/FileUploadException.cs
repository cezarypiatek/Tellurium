using System;

namespace Tellurium.MvcPages.SeleniumUtils.FileUploading
{
    public class FileUploadException:ApplicationException
    {
        public FileUploadException(string message) : base(message)
        {
        }
    }
}