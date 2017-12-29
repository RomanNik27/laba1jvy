using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionControlSystem
{
    class VCS_File
    {
        private string _curFile;

        public string curFile
        {
            get { return _curFile; }
            set { _curFile = value; }
        }

        private string _attribute;

        public string attribute
        {
            get { return _attribute; }
            set { _attribute = value; }
        }
    }
}
