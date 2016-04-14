using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTiler
{
    internal class FileSearchExpand
    {
        /// <summary>
        /// Expand a single file search pattern into a list of fully qualified filenames by expanding wildcards.
        /// </summary>
        /// <param name="pattern">
        ///  Includes optional path with no wildcards, and file pattern with optional wildcards.
        ///  </param>
        /// <returns>
        /// List of fully qualified filenames
        /// </returns>
        public static IEnumerable<string> ExpandFileSearchPattern(string pattern)
        {
            string directory = Path.GetDirectoryName(pattern);
            string file = Path.GetFileName(pattern);

            if (string.IsNullOrEmpty(directory))
            {
                directory = Directory.GetCurrentDirectory();
            }

            directory = Path.GetFullPath(directory);
            return Directory.GetFiles(directory, file);
        }

        /// <summary>
        /// Expand a series of file search patterns, an produce a unified list of fully qualified filenames
        /// </summary>
        /// <param name="patterns">
        /// List of file search patterns
        /// </param>
        /// <returns>
        /// List of fully qualified filenames
        /// </returns>
        public static IEnumerable<string> ExpandFileSearchPatterns(IEnumerable<string> patterns)
        {
            IEnumerable<string> tmp = null;

            foreach (var pattern in patterns)
            {
                if (tmp == null)
                {
                    tmp = ExpandFileSearchPattern(pattern);
                }
                else
                {
                    tmp = tmp.Concat(ExpandFileSearchPattern(pattern));
                }
            }

            return tmp;
        }
    }
}
