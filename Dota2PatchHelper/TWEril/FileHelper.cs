using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWEril
{
    internal static class FileHelper
    {
        /// <summary>
        /// 获取指定目录所有文件
        /// </summary>
        public static FileInfo[] GetFiles(string path)
        {
            try
            {
                var di = new DirectoryInfo(path);
                return di.GetFiles();
            }
            catch
            {
                //...
            }
            return null;
        }

        /// <summary>
        /// 获取指定目录所有文件（指定扩展名）
        /// </summary>
        public static FileInfo[] GetFiles(string path, string exname)
        {
            var files = GetFiles(path);
            if (files != null)
            {
                return files.Where(x => x.Extension == exname).ToArray();
            }
            return null;
        }

        /// <summary>
        /// 获取指定文件SHA256
        /// </summary>
        public static string GetFileSHA256(string filepath)
        {
            if (File.Exists(filepath))
            {
                System.IO.FileStream _fs = null;
                System.Security.Cryptography.SHA256Managed Sha256 = null;
                byte[] by = null;
                string S = null;
                try
                {
                    _fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open);
                    Sha256 = new System.Security.Cryptography.SHA256Managed();
                    by = Sha256.ComputeHash(_fs);
                    S = BitConverter.ToString(by).Replace("-", "");
                }
                catch
                {
                    return null;
                }
                Sha256.Clear();
                Sha256.Dispose();
                _fs.Close();
                _fs.Dispose();
                return S;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定文件夹所有文件
        /// </summary>
        public static List<FileInfo> GetFolderAllFiles(string folderpath)
        {
            //初始化根目录
            DirectoryInfo dirinfo = null;
            try
            {
                dirinfo = new DirectoryInfo(folderpath);
            }
            catch
            {
                return null;
            }

            //初始化临时文件夹目录表
            var dirlist = new List<DirectoryInfo>();
            DirectoryInfo[] tempdirs = null;
            try
            {
                tempdirs = dirinfo.GetDirectories();
            }
            catch
            {
                return null;
            }

            //查找所有文件夹
            int res = 0;
        RE:
            res++;
            if (res < 129) 
            {
                foreach (var item in tempdirs)
                {
                    dirlist.Add(item);
                    DirectoryInfo[] dirs = null;
                    try
                    {
                        dirs = item.GetDirectories();
                    }
                    catch
                    {
                        continue;
                    }
                    if (dirs.Length > 0)
                    {
                        tempdirs = dirs;
                        goto RE;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            //整理文件
            var files = new List<FileInfo>();
            foreach (var item in dirlist)
            {
                FileInfo[] tempfiles = null;
                try
                {
                    tempfiles = item.GetFiles();
                }
                catch
                {
                    continue;
                }
                foreach (var item2 in tempfiles)
                {
                    files.Add(item2);
                }
            }
            return files;
        }

    }
}
