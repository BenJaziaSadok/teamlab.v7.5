/* 
 * 
 * (c) Copyright Ascensio System Limited 2010-2014
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * http://www.gnu.org/licenses/agpl.html 
 * 
 */

#region Import

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using ASC.Collections;
using ASC.Common.Threading.Workers;
using ASC.Data.Storage;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.FileUploader;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ASC.Web.CRM.Resources;

#endregion

namespace ASC.Web.CRM.Classes
{
    public class ResizeWorkerItem
    {
        public int ContactID { get; set; }

        public bool UploadOnly { get; set; }

        public String TmpDirName { get; set; }

        public Size[] RequireFotoSize { get; set; }

        public byte[] ImageData { get; set; }

        public IDataStore DataStore { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ResizeWorkerItem)) return false;

            var item = (ResizeWorkerItem)obj;

            return item.ContactID.Equals(ContactID) && RequireFotoSize.Equals(item.RequireFotoSize) && ImageData.Length == item.ImageData.Length;

        }

        public override int GetHashCode()
        {
            return ContactID ^ RequireFotoSize.GetHashCode() ^ ImageData.Length;
        }
    }

    public class ContactPhotoHandler : IFileUploadHandler
    {
        public FileUploadResult ProcessUpload(HttpContext context)
        {
            var fileUploadResult = new FileUploadResult();

            if (!ProgressFileUploader.HasFilesToUpload(context)) return fileUploadResult;

            var file = new ProgressFileUploader.FileToUpload(context);

            if (String.IsNullOrEmpty(file.FileName) || file.ContentLength == 0)
                throw new InvalidOperationException("Invalid file.");

            if (0 < SetupInfo.MaxImageUploadSize && SetupInfo.MaxImageUploadSize < file.ContentLength)
            {
                fileUploadResult.Success = false;
                fileUploadResult.Message = FileSizeComment.GetFileImageSizeNote(CRMCommonResource.ErrorMessage_UploadFileSize,false).HtmlEncode();
                return fileUploadResult;
            }

            if (FileUtility.GetFileTypeByFileName(file.FileName) != FileType.Image) {
                    fileUploadResult.Success = false;
                    fileUploadResult.Message = CRMJSResource.ErrorMessage_NotImageSupportFormat.HtmlEncode();
                    return fileUploadResult;
            }


            var contactId = Convert.ToInt32(context.Request["contactID"]);
            var uploadOnly = Convert.ToBoolean(context.Request["uploadOnly"]);
            var tmpDirName = Convert.ToString(context.Request["tmpDirName"]);
            var photoUri = "";

            if (contactId != 0)
            {
                photoUri = ContactPhotoManager.UploadPhoto(file.InputStream, contactId, uploadOnly);
            }
            else
            {
                if (String.IsNullOrEmpty(tmpDirName))
                {
                    tmpDirName = Guid.NewGuid().ToString();
                }
                photoUri = ContactPhotoManager.UploadPhoto(file.InputStream, tmpDirName);
            }
            fileUploadResult.Success = true;
            fileUploadResult.Data = photoUri;

            return fileUploadResult;
        }
    }

    public static class ContactPhotoManager
    {
        #region Members

        private static readonly String PhotosBaseDirName = "photos";
        private static readonly String PhotosDefaultTmpDirName = "temp";

        private static readonly SynchronizedDictionary<int, IDictionary<Size, string>> _photoCache = new SynchronizedDictionary<int, IDictionary<Size, string>>();

        private static readonly WorkerQueue<ResizeWorkerItem> ResizeQueue = new WorkerQueue<ResizeWorkerItem>(2, TimeSpan.FromSeconds(30), 1, true);

        private static readonly Size _oldBigSize = new Size(145, 145);

        private static readonly Size _bigSize = new Size(200, 200);
        private static readonly Size _mediumSize = new Size(82, 82);
        private static readonly Size _smallSize = new Size(40, 40);

        private static readonly Object _synchronizedObj = new Object();

        #endregion

        #region Cache and DataStore Methods

        private static String FromCache(int contactID, Size photoSize)
        {
            if (_photoCache.ContainsKey(contactID))
            {
                if (_photoCache[contactID].ContainsKey(photoSize))
                    return _photoCache[contactID][photoSize];
                else if (photoSize == _bigSize && _photoCache[contactID].ContainsKey(_oldBigSize)){
                    return _photoCache[contactID][_oldBigSize];
                }
            }

            return String.Empty;
        }

        private static void RemoveFromCache(int contactID)
        {
            lock (_synchronizedObj)
            {
                _photoCache.Remove(contactID);
            }
        }

        private static void ToCache(int contactID, String photoUri, Size photoSize)
        {
            lock (_synchronizedObj)
            {

                if (_photoCache.ContainsKey(contactID))
                    if (_photoCache[contactID].ContainsKey(photoSize))
                        _photoCache[contactID][photoSize] = photoUri;
                    else
                        _photoCache[contactID].Add(photoSize, photoUri);
                else
                    _photoCache.Add(contactID, new Dictionary<Size, string> { { photoSize, photoUri } });
            }
        }

        private static String FromDataStore(int contactID, Size photoSize)
        {
            return FromDataStore(contactID, photoSize, false, null);
        }

        private static String FromDataStore(int contactID, Size photoSize, Boolean isTmpDir, String tmpDirName)
        {
            var directoryPath = !isTmpDir
                                    ? BuildFileDirectory(contactID)
                                    : (String.IsNullOrEmpty(tmpDirName) ? BuildFileTmpDirectory(contactID) : BuildFileTmpDirectory(tmpDirName));

            var filesURI = Global.GetStore().ListFiles(directoryPath, BuildFileName(contactID, photoSize) + "*", false);

            if (filesURI.Length == 0 && photoSize == _bigSize)
            {
                filesURI = Global.GetStore().ListFiles(directoryPath, BuildFileName(contactID, _oldBigSize) + "*", false);
            }

            if (filesURI.Length == 0)
            {
                return String.Empty;
            }

            return filesURI[0].ToString();
        }

        private static String FromDataStore(Size photoSize, String tmpDirName)
        {
            var directoryPath = BuildFileTmpDirectory(tmpDirName);

            if (!Global.GetStore().IsDirectory(directoryPath))
                return String.Empty;

            var filesURI = Global.GetStore().ListFiles(directoryPath, BuildFileName(0, photoSize) + "*", false);

            if (filesURI.Length == 0) return String.Empty;

            return filesURI[0].ToString();
        }

        #endregion

        #region Private Methods

        private static String GetPhotoUri(int contactID, bool isCompany, Size photoSize)
        {
            var photoUri = FromCache(contactID, photoSize);

            if (!String.IsNullOrEmpty(photoUri)) return photoUri;

            photoUri = FromDataStore(contactID, photoSize);

            if (String.IsNullOrEmpty(photoUri))
                photoUri = GetDefaultPhoto(isCompany, photoSize);

            ToCache(contactID, photoUri, photoSize);

            return photoUri;
        }

        private static String BuildFileDirectory(int contactID)
        {
            var s = contactID.ToString("000000");

            return String.Concat(PhotosBaseDirName, "/", s.Substring(0, 2), "/",
                                 s.Substring(2, 2), "/",
                                 s.Substring(4), "/");
        }

        private static String BuildFileTmpDirectory(int contactID)
        {
            return String.Concat(BuildFileDirectory(contactID), PhotosDefaultTmpDirName, "/");
        }

        private static String BuildFileTmpDirectory(string tmpDirName)
        {
            if (String.IsNullOrEmpty(tmpDirName))
                throw new ArgumentException();
            if (tmpDirName.StartsWith(PathProvider.BaseAbsolutePath))
            {
                tmpDirName = tmpDirName.Substring(PathProvider.BaseAbsolutePath.Length - 1);
            }
            if (tmpDirName.Contains(PhotosBaseDirName))
            {
                tmpDirName = String.Concat(tmpDirName.Substring(tmpDirName.IndexOf(PhotosBaseDirName)).TrimEnd('/'), "/");
            }
            else
            {
                tmpDirName = String.Concat(PhotosBaseDirName, "/", tmpDirName.TrimEnd('/'), "/");
            }
            return tmpDirName;
        }

        private static String BuildFileName(int contactID, Size photoSize)
        {
            return String.Format("contact_{0}_{1}_{2}", contactID, photoSize.Width, photoSize.Height);
        }

        private static String BuildFilePath(int contactID, Size photoSize, String imageExtension)
        {
            if (photoSize.IsEmpty || contactID == 0)
                throw new ArgumentException();

            return String.Concat(BuildFileDirectory(contactID), BuildFileName(contactID, photoSize), imageExtension);
        }

        private static String BuildFileTmpPath(int contactID, Size photoSize, String imageExtension, String tmpDirName)
        {
            var s = contactID.ToString("000000");

            if (photoSize.IsEmpty || (contactID == 0 && String.IsNullOrEmpty(tmpDirName)))
                throw new ArgumentException();

            return String.Concat(
                String.IsNullOrEmpty(tmpDirName)
                    ? BuildFileTmpDirectory(contactID)
                    : BuildFileTmpDirectory(tmpDirName),
                BuildFileName(contactID, photoSize), imageExtension);
        }

        private static byte[] ToByteArray(Stream inputStream)
        {
            var data = new byte[inputStream.Length];

            var br = new BinaryReader(inputStream);
            br.Read(data, 0, (int)inputStream.Length);
            br.Close();

            return data;
        }

        private static string GetImgFormatName(ImageFormat format)
        {

            if (format.Equals(ImageFormat.Bmp)) return "bmp";
            if (format.Equals(ImageFormat.Emf)) return "emf";
            if (format.Equals(ImageFormat.Exif)) return "exif";
            if (format.Equals(ImageFormat.Gif)) return "gif";
            if (format.Equals(ImageFormat.Icon)) return "icon";
            if (format.Equals(ImageFormat.Jpeg)) return "jpeg";
            if (format.Equals(ImageFormat.MemoryBmp)) return "MemoryBMP";
            if (format.Equals(ImageFormat.Png)) return "png";
            if (format.Equals(ImageFormat.Tiff)) return "tiff";
            if (format.Equals(ImageFormat.Wmf)) return "wmf";

            return "jpg";
        }

        private static ImageCodecInfo GetCodecInfo(ImageFormat format)
        {
            var mimeType = string.Format("image/{0}", GetImgFormatName(format));
            if (mimeType == "image/jpg") mimeType = "image/jpeg";
            var encoders = ImageCodecInfo.GetImageEncoders();
            foreach (var e in
                encoders.Where(e => e.MimeType.Equals(mimeType, StringComparison.InvariantCultureIgnoreCase)))
            {
                return e;
            }
            return 0 < encoders.Length ? encoders[0] : null;
        }

        private static byte[] SaveToBytes(Image img)
        {
            return SaveToBytes(img, img.RawFormat);
        }

        private static byte[] SaveToBytes(Image img, ImageFormat source)
        {
            var data = new byte[0];
            using (var memoryStream = new MemoryStream())
            {
                var encParams = new EncoderParameters(1);
                encParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)100);
                img.Save(memoryStream, GetCodecInfo(source), encParams);
                data = memoryStream.ToArray();
            }
            return data;
        }

        private static void ExecResizeImage(ResizeWorkerItem resizeWorkerItem)
        {
            foreach (var fotoSize in resizeWorkerItem.RequireFotoSize)
            {
                var data = resizeWorkerItem.ImageData;
                using (var stream = new MemoryStream(data))
                using (var img = new Bitmap(stream))
                {
                    var imgFormat = img.RawFormat;
                    if (fotoSize != img.Size)
                    {
                        using (var img2 = DoThumbnail(img, fotoSize))
                        {
                            data = SaveToBytes(img2, imgFormat);
                        }
                    }
                    else
                    {
                        data = SaveToBytes(img);
                    }

                    var fileExtension = String.Concat("." + GetImgFormatName(ImageFormat.Jpeg));

                    var photoPath = !resizeWorkerItem.UploadOnly
                                        ? BuildFilePath(resizeWorkerItem.ContactID, fotoSize, fileExtension)
                                        : BuildFileTmpPath(resizeWorkerItem.ContactID, fotoSize, fileExtension, resizeWorkerItem.TmpDirName);

                    using (var fileStream = new MemoryStream(data))
                    {
                        var photoUri = resizeWorkerItem.DataStore.Save(photoPath, fileStream).ToString();
                        photoUri = String.Format("{0}?cd={1}", photoUri, DateTime.UtcNow.Ticks);

                        if (!resizeWorkerItem.UploadOnly)
                        {
                            ToCache(resizeWorkerItem.ContactID, photoUri, fotoSize);
                        }
                    }
                }
            }
        }

        private static String GetDefaultPhoto(bool isCompany, Size photoSize)
        {
            int contactID;

            if (isCompany)
                contactID = -1;
            else
                contactID = -2;

            var defaultPhotoUri = FromCache(contactID, photoSize);

            if (!String.IsNullOrEmpty(defaultPhotoUri)) return defaultPhotoUri;

            if (isCompany)
                defaultPhotoUri = WebImageSupplier.GetAbsoluteWebPath(String.Format("empty_company_logo_{0}_{1}.png", photoSize.Height, photoSize.Width), ProductEntryPoint.ID);
            else
                defaultPhotoUri = WebImageSupplier.GetAbsoluteWebPath(String.Format("empty_people_logo_{0}_{1}.png", photoSize.Height, photoSize.Width), ProductEntryPoint.ID);

            ToCache(contactID, defaultPhotoUri, photoSize);

            return defaultPhotoUri;
        }

        #endregion


        #region Delete Methods

        public static void DeletePhoto(int contactID, bool isTmpDir)
        {
            DeletePhoto(contactID, isTmpDir, null, true);
        }

        public static void DeletePhoto(int contactID, bool isTmpDir, string tmpDirName, bool recursive)
        {
            if (contactID == 0)
                throw new ArgumentException();

            lock (_synchronizedObj)
            {
                ResizeQueue.GetItems().Where(item => item.ContactID == contactID)
                           .All(item =>
                                    {
                                        ResizeQueue.Remove(item);
                                        return true;
                                    });

                var photoDirectory = !isTmpDir
                                         ? BuildFileDirectory(contactID)
                                         : (String.IsNullOrEmpty(tmpDirName) ? BuildFileTmpDirectory(contactID) : BuildFileTmpDirectory(tmpDirName));
                var store = Global.GetStore();

                if (store.IsDirectory(photoDirectory))
                {
                    store.DeleteFiles(photoDirectory, "*", recursive);
                    if (recursive)
                    {
                        store.DeleteDirectory(photoDirectory);
                    }
                }

                if (!isTmpDir)
                {
                    RemoveFromCache(contactID);
                }
            }
        }

        public static void DeletePhoto(int contactID)
        {
            DeletePhoto(contactID, false);
        }

        #endregion

        public static void TryUploadPhotoFromTmp(int contactID, bool isNewContact, string tmpDirName)
        {
            var directoryTmpPath = String.IsNullOrEmpty(tmpDirName) ? BuildFileTmpDirectory(contactID) : BuildFileTmpDirectory(tmpDirName);
            var directoryPath = BuildFileDirectory(contactID);
            var dataStore = Global.GetStore();

            if (!dataStore.IsDirectory(directoryTmpPath))
                return;

            try
            {
                if (dataStore.IsDirectory(directoryPath))
                {
                    DeletePhoto(contactID, false, null, false);
                }
                foreach (var photoSize in new[] { _bigSize, _mediumSize, _smallSize })
                {
                    var photoTmpPath = FromDataStore(isNewContact ? 0 : contactID, photoSize, true, tmpDirName);
                    photoTmpPath = photoTmpPath.Substring(photoTmpPath.IndexOf(directoryTmpPath));
                    var imageExtension = photoTmpPath.Substring(photoTmpPath.LastIndexOf("."));

                    var photoPath = String.Concat(directoryPath, BuildFileName(contactID, photoSize), imageExtension).TrimStart('/');

                    using (var photoTmpStream = dataStore.GetReadStream(photoTmpPath))
                    {
                        var data = ToByteArray(photoTmpStream);
                        using (var fileStream = new MemoryStream(data))
                        {
                            var photoUri = dataStore.Save(photoPath, fileStream).ToString();
                            photoUri = String.Format("{0}?cd={1}", photoUri, DateTime.UtcNow.Ticks);
                            ToCache(contactID, photoUri, photoSize);
                        }
                    }
                }
                DeletePhoto(contactID, true, tmpDirName, true);
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("ASC.CRM").ErrorFormat("TryUploadPhotoFromTmp for contactID={0} failed witth error: {1}", contactID, ex);
                return;
            }
        }

        #region Get Photo Methods

        public static String GetSmallSizePhoto(int contactID, bool isCompany)
        {
            if (contactID <= 0)
                return GetDefaultPhoto(isCompany, _smallSize);

            return GetPhotoUri(contactID, isCompany, _smallSize);
        }

        public static String GetMediumSizePhoto(int contactID, bool isCompany)
        {
            if (contactID <= 0)
                return GetDefaultPhoto(isCompany, _mediumSize);

            return GetPhotoUri(contactID, isCompany, _mediumSize);
        }

        public static String GetBigSizePhoto(int contactID, bool isCompany)
        {
            if (contactID <= 0)
                return GetDefaultPhoto(isCompany, _bigSize);

            return GetPhotoUri(contactID, isCompany, _bigSize);
        }

        #endregion

        private static Image DoThumbnail(Image image, Size size)
        {
            var width = size.Width;
            var height = size.Height;
            var realWidth = image.Width;
            var realHeight = image.Height;

            var maxSide = realWidth > realHeight ? realWidth : realHeight;

            var alignWidth = (maxSide == realWidth);

            double scaleFactor = (alignWidth) ? (realWidth/(1.0*width)) : (realHeight/(1.0*height));

            if (scaleFactor < 1) scaleFactor = 1;


            int finalWidth, finalHeigth;

            finalWidth = (int)(realWidth/scaleFactor);
            finalHeigth = (int)(realHeight/scaleFactor);

            var thumbnail = new Bitmap(finalWidth, finalHeigth);

            using (var graphic = Graphics.FromImage(thumbnail))
            {
                graphic.Clear(Color.White);
                graphic.SmoothingMode = SmoothingMode.AntiAlias;
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.DrawImage(image, 0, 0, finalWidth, finalHeigth);
            }

            return thumbnail;

        }

        private static String ResizeToBigSize(byte[] imageData, string tmpDirName)
        {
            return ResizeToBigSize(imageData, 0, true, tmpDirName);
        }

        private static String ResizeToBigSize(byte[] imageData, int contactID, bool uploadOnly, string tmpDirName)
        {
            var resizeWorkerItem = new ResizeWorkerItem
                {
                    ContactID = contactID,
                    UploadOnly = uploadOnly,
                    RequireFotoSize = new[] { _bigSize },
                    ImageData = imageData,
                    DataStore = Global.GetStore(),
                    TmpDirName = tmpDirName
                };

            ExecResizeImage(resizeWorkerItem);

            if (!uploadOnly)
            {
                return FromCache(contactID, _bigSize);
            }
            else if (String.IsNullOrEmpty(tmpDirName))
            {
                return FromDataStore(contactID, _bigSize, true, null);
            }
            else
            {
                return FromDataStore(_bigSize, tmpDirName);
            }
        }

        private static void ExecGenerateThumbnail(byte[] imageData, int contactID, bool uploadOnly)
        {
            ExecGenerateThumbnail(imageData, contactID, uploadOnly, null);
        }

        private static void ExecGenerateThumbnail(byte[] imageData, string tmpDirName)
        {
            ExecGenerateThumbnail(imageData, 0, true, tmpDirName);
        }


        private static void ExecGenerateThumbnail(byte[] imageData, int contactID, bool uploadOnly, string tmpDirName)
        {
            var resizeWorkerItem = new ResizeWorkerItem
                {
                    ContactID = contactID,
                    UploadOnly = uploadOnly,
                    RequireFotoSize = new[] { _mediumSize, _smallSize },
                    ImageData = imageData,
                    DataStore = Global.GetStore(),
                    TmpDirName = tmpDirName
                };

            if (!ResizeQueue.GetItems().Contains(resizeWorkerItem))
                ResizeQueue.Add(resizeWorkerItem);

            if (!ResizeQueue.IsStarted) ResizeQueue.Start(ExecResizeImage);
        }

        private static byte[] ToByteArray(Stream inputStream, int streamLength)
        {
            using (var br = new BinaryReader(inputStream))
            {
                return br.ReadBytes(streamLength);
            }
        }

        #region UploadPhoto Methods

        public static String UploadPhoto(String imageUrl, int contactID, bool uploadOnly)
        {
            var request = (HttpWebRequest)WebRequest.Create(imageUrl);
            using (var response = request.GetResponse())
            {
                using (var inputStream = response.GetResponseStream())
                {
                    var imageData = ToByteArray(inputStream, (int)response.ContentLength);
                    return UploadPhoto(imageData, contactID, uploadOnly);
                }
            }
        }

        public static String UploadPhoto(Stream inputStream, int contactID, bool uploadOnly)
        {
            var imageData = ToByteArray(inputStream);
            return UploadPhoto(imageData, contactID, uploadOnly);
        }

        public static String UploadPhoto(byte[] imageData, int contactID, bool uploadOnly)
        {
            if (contactID == 0)
                throw new ArgumentException();

            DeletePhoto(contactID, uploadOnly, null, false);

            ExecGenerateThumbnail(imageData, contactID, uploadOnly);

            return ResizeToBigSize(imageData, contactID, uploadOnly, null);
        }

        public static String UploadPhoto(String imageUrl, int contactID)
        {
            return UploadPhoto(imageUrl, contactID, false);
        }

        public static String UploadPhoto(Stream inputStream, int contactID)
        {
            return UploadPhoto(inputStream, contactID, false);
        }

        public static String UploadPhoto(byte[] imageData, int contactID)
        {
            return UploadPhoto(imageData, contactID, false);
        }

        public static String UploadPhoto(String imageUrl, String tmpDirName)
        {
            var request = (HttpWebRequest)WebRequest.Create(imageUrl);
            using (var response = request.GetResponse())
            {
                using (var inputStream = response.GetResponseStream())
                {
                    var imageData = ToByteArray(inputStream, (int)response.ContentLength);
                    return UploadPhoto(imageData, tmpDirName);
                }
            }
        }

        public static String UploadPhoto(Stream inputStream, String tmpDirName)
        {
            var imageData = ToByteArray(inputStream);
            return UploadPhoto(imageData, tmpDirName);
        }

        public static String UploadPhoto(byte[] imageData, String tmpDirName)
        {
            ExecGenerateThumbnail(imageData, tmpDirName);
            return ResizeToBigSize(imageData, tmpDirName);
        }

        #endregion
    }

    #region Exception Classes

    public class UnknownImageFormatException : Exception
    {
        public UnknownImageFormatException()
            : base("unknown image file type")
        {
        }

        public UnknownImageFormatException(Exception inner)
            : base("unknown image file type", inner)
        {
        }
    }

    public class ImageWeightLimitException : Exception
    {
        public ImageWeightLimitException()
            : base("image with is too large")
        {
        }
    }

    public class ImageSizeLimitException : Exception
    {
        public ImageSizeLimitException()
            : base("image size is too large")
        {
        }
    }

    #endregion
}