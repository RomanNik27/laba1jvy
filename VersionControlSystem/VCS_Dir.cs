using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VersionControlSystem
{
    class VCS_Dir
    {
        private string _dirPath;

        public string dirPath
        {
            get { return _dirPath; }
            set { _dirPath = value; }
        }

        private string _dirName;

        public string dirName
        {
            get { return _dirName; }
            set { _dirName = value; }
        }

        private List<VCS_File> _dirFiles;

        public List<VCS_File> dirFiles
        {
            get { return _dirFiles; }
            set { _dirFiles = value; }
        }
    }
}
