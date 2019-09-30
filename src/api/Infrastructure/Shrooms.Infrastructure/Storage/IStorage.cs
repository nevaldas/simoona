﻿using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Shrooms.Azure
{
    public interface IStorage
    {
        Task UploadPicture(Image image, string blobKey, string mimeType, string tenantPicturesContainer);

        Task UploadPicture(Stream stream, string blobKey, string mimeType, string tenantPicturesContainer);

        Task RemovePicture(string blobKey, string tenantPicturesContainer);
    }
}