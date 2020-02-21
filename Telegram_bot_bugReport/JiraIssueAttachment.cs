using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_bot_bugReport
{
    class JiraIssueAttachment
    {

        private string _postUrl;
        private string _filePath;
        private string _authData;
        private string _issueKey;
        public JiraIssueAttachment(string postUrl, string filePath, string authData, string issueKey)
        {
            _postUrl = postUrl;
            _filePath = filePath;
            _authData = authData;
            _issueKey = issueKey;
            AddAttachments(_issueKey, _filePath);
        }
        private void AddAttachments(string issueKey, string filePath)
        {
            string restUrl = String.Format("{0}issue/{1}/attachments", _postUrl, issueKey);
            var filesToUpload = new List<FileInfo>();

            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine("File '{0}' doesn't exist", filePath);
            }
            var file = new FileInfo(filePath);
            filesToUpload.Add(file);

            if (filesToUpload.Count <= 0)
            {
                Console.WriteLine("No file to Upload");
            }
            PostFile(restUrl, filesToUpload);
        }
        private void PostFile(string restUrl, IEnumerable<FileInfo> filePaths)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            String boundary = String.Format("----------{0:N}", Guid.NewGuid());
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            foreach (var filePath in filePaths)
            {
                var fs = new FileStream(filePath.FullName, FileMode.Open, FileAccess.Read);
                var data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();
                writer.WriteLine("--{0}", boundary);
                writer.WriteLine("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"", filePath.Name);
                writer.WriteLine("Content-Type: application/octet-stream");
                writer.WriteLine();
                writer.Flush();
                stream.Write(data, 0, data.Length);
                writer.WriteLine();
            }
            writer.WriteLine("--" + boundary + "--");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            request = WebRequest.Create(restUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            request.Accept = "application/json";
            byte[] cred = UTF8Encoding.UTF8.GetBytes(_authData);
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(cred));
            request.Headers.Add("X-Atlassian-Token", "nocheck");
            request.ContentLength = stream.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                stream.WriteTo(requestStream);
                requestStream.Close();
            }
            try
            {
                using (response = request.GetResponse() as HttpWebResponse)
                {

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var reader = new StreamReader(response.GetResponseStream());
                        Console.WriteLine("The server returned '{0}'\n{1}", response.StatusCode, reader.ReadToEnd());
                    }
                }
                request.Abort();
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }
}
