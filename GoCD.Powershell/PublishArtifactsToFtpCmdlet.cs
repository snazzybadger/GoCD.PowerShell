using System.Collections.Generic;
using System.Management.Automation;
using System;
using GoCD.Powershell.Commands;

namespace GoCd.Powershell
{
    [Cmdlet(VerbsData.Publish, "ArtifactsToFtp")]
    public class PublishArtifactsToFtpCmdlet : Cmdlet
    {
        [Parameter(Position = 0)]
        public string Host{ get; set; }

        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true)]
        public string UserName { get; set; }

        [Parameter(Position = 2, ValueFromPipelineByPropertyName = true)]
        public string Password { get; set; }

        [Parameter(Position = 3)]
        public string Source { get; set; }



        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            var command = new UploadDirectoryToFtp(Host, UserName, Password, Source);
            var handler = new UploadDirectoryToFtpHandler(WriteVerbose);

            handler.Execute(command);
        }
    }









    public sealed class PathInfo
    {
        public readonly string LocalPath;
        public readonly string RemotePath;

        public PathInfo(string localPath, string remotePath)
        {
            LocalPath = localPath;
            RemotePath = remotePath;
        }
    }



    public static class Extentions
    {
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (action == null) throw new ArgumentNullException("action");
            foreach (T item in sequence)
                action(item);
        }
    }

}