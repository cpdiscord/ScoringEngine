﻿using System.IO.Compression;
using System.Threading.Tasks;
using System.IO;
/* Network authoritative commands: (todo but also temporary)

    FAIL(id) => Fail and disable a check
    ENABLE(id) => Enable a check. If it was previously failed, the state will still be disabled
    DISABLE(id) => Stop a check from ticking. This does not fail the tick, so if failing was intended, use FAIL(id)
    REPORT(id) => Force the state of the check onto the top of the queue

*/


namespace Engine.Installer.Core
{
    /// <summary>
    /// All networking interfaces and components that require networking
    /// </summary>
    public static class Networking
    {
        private const string GitUsername = "dynamix-team";
        private const string GitRepoName = "ScoringEngine";
        private const string GitRepoURLFormat = "https://github.com/{0}/{1}/archive/master.zip"; //Username->0, Repo->1
        private const string ZipName = "engine.zip";
        


        internal static string ProjectURL
        {
            get
            {
                return string.Format(GitRepoURLFormat, GitUsername, GitRepoName);
            }
        }

        /// <summary>
        /// Download the engine to the path specified
        /// </summary>
        /// <param name="path">The path to download the engine to</param>
        /// <returns></returns>
        internal static async Task<bool> DownloadEngine(string path)
        {
            try
            {
                while(Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    await Task.Delay(100);
                }
                Directory.CreateDirectory(path);
            }
            catch
            {
                //todo: kill windows explorer when we start the installer
                return false;
            }
            try
            {
                bool result = await DownloadResource(ProjectURL, path, ZipName);
                if (!result)
                    return false;
                ZipFile.ExtractToDirectory(Path.Combine(path, ZipName), path);
            }
            catch
            {
                //report an error to the installer, this is CRITICAL
                return false;
            }
            return true;
        }

        /// <summary>
        /// Download an internet resource
        /// </summary>
        /// <param name="URL">The url to try to access</param>
        /// <param name="outdir">The directory to write the file to</param>
        /// <param name="outname">The name of the file to write</param>
        /// <returns>The result of the operation</returns>
        public static async Task<bool> DownloadResource(string URL, string outdir, string outname)
        {
            try
            {
                byte[] FileData;
                using (var client = new System.Net.Http.HttpClient())
                {
                    FileData = await client.GetByteArrayAsync(URL);
                }
                File.WriteAllBytes(Path.Combine(outdir, outname), FileData);
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
