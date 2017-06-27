using FluentFTP;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace GoCD.Powershell.Commands
{
    public sealed class UploadDirectoryToFtp : Command
    {
        public readonly string Host;
        public readonly string Username;
        public readonly string Password;
        public readonly string Directory;

        public UploadDirectoryToFtp(string host, string username, string password, string directory)
        {
            Host = host;
            Username = username;
            Password = password;
            Directory = directory;
        }
    }


    public sealed class UploadDirectoryToFtpHandler : CommandHandler<UploadDirectoryToFtp>
    {
        private readonly Action<string> writeVerbose;

        public UploadDirectoryToFtpHandler(Action<string> writeVerbose)
        {
            this.writeVerbose = writeVerbose;
        }

        public void Execute(UploadDirectoryToFtp command)
        {
            using (var client = CreateClient(command))
            {
                writeVerbose($"Connecting to host: {command.Host}");
                client.Connect();

                UploadDirectory(client, command.Directory, String.Empty);

                writeVerbose("Disconnecting from host");
                client.Disconnect();
            }
        }

        private void UploadDirectory(FtpClient client, string directoryPath, string uploadPath)
        {
            writeVerbose($"Current Directory: {directoryPath}");

            var filesInDirectory = Directory.GetFiles(directoryPath, "*.*");
            writeVerbose($"File count in current directory: {filesInDirectory.Count()}");

            filesInDirectory
                .Select(localFile => UploadFile(client, localFile, $"{uploadPath}/{Path.GetFileName(localFile)}"));


            string[] subDirectories = Directory.GetDirectories(directoryPath);
            writeVerbose($"Subdirectory count in current directory: {subDirectories.Count()}");

            subDirectories
                .Select(subDirectory => UploadFilesInDirectory(client, subDirectory, $"{uploadPath}/{Path.GetFileName(subDirectory)}"));


            foreach (string subDirectory in subDirectories)
            {
                var directoryUploadPath = $"{uploadPath}/{Path.GetFileName(subDirectory)}";

                writeVerbose($"Creating Directory: {directoryUploadPath}");
                client.CreateDirectory(directoryUploadPath);

                UploadDirectory(client, subDirectory, directoryUploadPath);
            }
        }

        private bool UploadFilesInDirectory(FtpClient client, string localPath, string remotePath)
        {
            writeVerbose($"Creating Directory: {remotePath}");
            client.CreateDirectory(remotePath);

            UploadDirectory(client, localPath, remotePath);

            return true;
        }

        private bool UploadFile(FtpClient client, string localPath, string remotePath)
        {
            writeVerbose($"Uploading file [{localPath}] to [{remotePath}]");
            return client.UploadFile(localPath, remotePath, FtpExists.Overwrite, true, FtpVerify.None);
        }

        private FtpClient CreateClient(UploadDirectoryToFtp command)
        {
            return new FtpClient(command.Host)
            {
                Credentials = new NetworkCredential(command.Username, command.Password)
            };
        }
    }




}
