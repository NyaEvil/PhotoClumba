using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace PhotoClumba
{
    public class FTPClient
    {
        private string UserId;
        private string Password;
        private string ftpServer;
        public FtpWebResponse ftpresponse;

        public FTPClient(string userId, string password)
        {
            UserId = userId;
            Password = password;
            ftpServer = "ftp://tkton.ap35.ru";
        }

        public void UploadFile(string localFilePath, string remoteFileName)
        {
            string ftpUsername = UserId;
            string ftpPassword = Password;

            //string remoteDirectory = "/PhotoClumba/";
            // Create FTP request
            Console.WriteLine(ftpServer + "/" /*+ remoteDirectory*/ + remoteFileName);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServer + "/" /*+ remoteDirectory*/ + remoteFileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                // Read the local file
                using (FileStream fileStream = new FileStream(localFilePath, FileMode.Open))
                {
                    // Get the request stream for uploading
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        // Upload the file in chunks
                        byte[] buffer = new byte[1024];
                        int bytesRead = 0;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            requestStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                // Get the FTP response
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                ftpresponse = response;
        }

        public void DeleteFile(string fileName)
        {
            try
            {
                // Create FTP request
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServer + "/"/*+ "\\PhotoClumba"*/ + fileName);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(UserId, Password);
                // Send the request to delete the file
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.FileActionOK)
                {
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
