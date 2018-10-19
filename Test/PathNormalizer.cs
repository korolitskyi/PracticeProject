using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Test
{
    [Flags]
    public enum PathAttributes
    {
        File = 1,
        Directory = 2
    }

    public class NormalizedPath
    {
        private readonly string _basePath;

        private readonly PathAttributes _attributes;

        private bool IsFile()
        {
            return (_attributes & PathAttributes.File) == PathAttributes.File;
        }

        public NormalizedPath(string path, PathAttributes attributes = PathAttributes.File)
        {

            var normalizedPath = path.Replace("\\", "/");

            while (normalizedPath.Contains("//"))
            {
                normalizedPath = normalizedPath.Replace("//", "/");
            }

            //---------SpecChar restriction---------
            //normalizedPath = Regex.Replace(normalizedPath, @"[^\w!\-.*'()\/ ?]", "?", RegexOptions.None);

            normalizedPath = normalizedPath.Trim(' ', '/');

            _basePath = normalizedPath;
            _attributes = attributes;
        }

        public string GetAmazonPath()
        {
            if (!string.IsNullOrEmpty(_basePath) && !IsFile())
                return _basePath + "/";
            return _basePath;
        }

        private string GetNormalPath()
        {
            return Normalize(_basePath, _attributes);
        }

        public string GetRelativeName()
        {
            return _basePath.Split('/').LastOrDefault(e => !string.IsNullOrEmpty(e)) ?? "";
        }

        public string GetRelativePath()
        {
            var splitArr = _basePath.Split('/');
            var res = string.Join("/", splitArr.Take(splitArr.Length - 1));
            return Normalize(res, _attributes);
        }

        public NormalizedPath Combine(string path, PathAttributes attributes)
        {
            if (IsFile())
                throw new ArgumentException("You cannot add path to path with extension");

            //combinedPath may contain "//" but it will be replaced to "/" in the constructor.
            var combinedPath = GetNormalPath() + path;

            return new NormalizedPath(combinedPath, attributes);
        }

        public static NormalizedPath operator +(NormalizedPath source, NormalizedPath target)
        {
            if (source.IsFile())
                throw new ArgumentException("You cannot add path to path with extension");

            //combinedPath may contain "//" but it will be replaced to "/" in the constructor.
            var combinedPath = source.GetNormalPath() + target.GetNormalPath();

            return new NormalizedPath(combinedPath, target._attributes);
        }

        public static implicit operator string(NormalizedPath obj)
        {
            return obj.GetNormalPath();
        }

        private static string Normalize(string path, PathAttributes attributes)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = "/" + path;

                if ((attributes & PathAttributes.File) != PathAttributes.File)
                    path += "/";
            }

            return path;
        }
    }
}
