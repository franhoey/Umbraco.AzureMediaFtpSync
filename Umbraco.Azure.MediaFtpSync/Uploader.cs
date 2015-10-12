using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Umbraco.Azure.MediaFtpSync
{
    class Uploader
    {
        private const string FILE_PATH_PROPERTY_KEY = "umbracoFile";
        private const string CONTENT_TYPE_PROPERTY_KEY = "contentType";
        private const string FOLDER_CONTENT_TYPE = "Folder";

        private readonly ConfigReader _config;
        private readonly string _applicationServerPath;

        public Uploader(ConfigReader config)
        {
            _config = config;
            _applicationServerPath = HttpContext.Current.Server.MapPath(string.Empty);
        }

        public void UploadToAzure(IMedia mediaItem)
        {
            if(!IsValidMediaItem(mediaItem))
                return;
            
            var files = GetMediaFilePaths(mediaItem);
            files.ForEach(UploadToAzure);
        }

        private void UploadToAzure(FileInfo fileInfo)
        {
            var url = string.Concat(_config.FtpWebsiteRoot,
                ConvertSystemPathToWebPath(fileInfo.FullName.Substring(_applicationServerPath.Length)));
            EnsureFtpDirectory(url);

            var request = (FtpWebRequest)WebRequest.Create(url);

            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_config.FtpUserName, _config.FtpPassword, _config.FtpDomain);

            var file = GetFile(fileInfo);
            request.ContentLength = file.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(file, 0, file.Length);
                requestStream.Close();
            }

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                response.Close();
            }
        }

        private void EnsureFtpDirectory(string ftpFilePath)
        {
            if (FtpDirectoryExists(ftpFilePath))
                return;

            var lastSlashPosition = ftpFilePath.LastIndexOf("/");
            var ftpDirectory = ftpFilePath.Substring(0, lastSlashPosition);

            var request = (FtpWebRequest)WebRequest.Create(ftpDirectory);

            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(_config.FtpUserName, _config.FtpPassword, _config.FtpDomain);
            
            using (var response = (FtpWebResponse)request.GetResponse())
            {
                response.Close();
            }
        }
        private bool FtpDirectoryExists(string ftpFilePath)
        {
            var lastSlashPosition = ftpFilePath.LastIndexOf("/");
            var ftpDirectory = ftpFilePath.Substring(0, lastSlashPosition);

            var request = (FtpWebRequest)WebRequest.Create(ftpDirectory);

            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(_config.FtpUserName, _config.FtpPassword, _config.FtpDomain);

            try
            {
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    response.Close();
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        private IEnumerable<FileInfo> GetMediaFilePaths(IMedia mediaItem)
        {
            var directory = GetMediaFolderPath(mediaItem);
            return directory.GetFiles();
        }
        
        private byte[] GetFile(FileInfo fileInfo)
        {
            return System.IO.File.ReadAllBytes(fileInfo.FullName);
        }

        private DirectoryInfo GetMediaFolderPath(IMedia mediaItem)
        {
            var property = mediaItem.Properties[FILE_PATH_PROPERTY_KEY];
            if (property == null)
                throw new InvalidOperationException("Cannot file umbraco file property");

            var filePath = string.Concat(_applicationServerPath, ConvertWebPathToSystemPath(((string)property.Value)));
            var fi = new FileInfo(filePath);
            if(!fi.Exists)
                throw new InvalidOperationException("Cannot find file to upload to Azure");

            return fi.Directory;
        }

        private bool IsValidMediaItem(IMedia mediaItem)
        {
            return mediaItem.ContentType.Alias != FOLDER_CONTENT_TYPE;
        }

        private string ConvertWebPathToSystemPath(string path)
        {
            return path.Replace("/", "\\");
        }

        private string ConvertSystemPathToWebPath(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}