using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VersionControlSystem
{
    class Program
    {
        private static string default_dir = @".\vcs";
        static List<VCS_Dir> dirs = null;
        static VCS_Dir curDir = null;
        
        static void Main(string[] args)
        {
            string cmd;
            string _cmd;
            string pattern = @"\[.*\]";

            Console.WriteLine("Welcome to VersionControlSystem (v1.0)\n");

            if (!Directory.Exists(default_dir))
            {
                try
                {
                    Directory.CreateDirectory(default_dir);
                }
                catch {}
            }

            GetDirs();

            VCS_Help it = new VCS_Help();
            it.ShowHelp();

            #region Распознавание команд
            do
            {
                Console.Write("->");
                cmd = Console.ReadLine().Trim().ToLower();
                _cmd = Regex.Replace(cmd, pattern, "[]");

                switch (_cmd)
                {
                    case "help":
                        it.ShowCommands();
                        break;
                    case "?":
                        it.ShowCommands();
                        break;
                    case "init []":
                        CommandInit(cmd);
                        break;
                    case "status":
                        CommandStatus();
                        break;
                    case "add []":
                        CommandAdd(cmd);
                        break;
                    case "remove []":
                        CommandRemove(cmd);
                        break;
                    case "apply []":
                        CommandApply(cmd);
                        break;
                    case "listbranch":
                        CommandListbranch();
                        break;
                    case "checkout []":
                        CommandCheckout(cmd);
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "exit":
                        return;
                    default:
                        it.ShowHelp();
                        break;
                }
            }
            while (true);
            #endregion
        }

        static void GetDirs()
        {
            dirs = new List<VCS_Dir>();
            try
            {
                foreach (string s in Directory.GetDirectories(default_dir))
                {
                    VCS_Dir dir = new VCS_Dir();
                    dir.dirName = s.Substring(default_dir.Length + 1);
                    dir.dirPath = dir.dirName.Replace("#", ":").Replace("@", "\\");
                    dir.dirFiles = new List<VCS_File>();
                    foreach (string file in Directory.GetFiles(s, "*", SearchOption.AllDirectories))
                    {
                        VCS_File _file = new VCS_File();
                        _file.attribute = "";
                        _file.curFile = file.Substring(file.LastIndexOf("\\") + 1);
                        dir.dirFiles.Add(_file);
                    }
                    dirs.Add(dir);
                }
            }
            catch { }
        }

        #region Выполнение команд
        static void CommandInit(string command)
        {
            string _dir_path = command.Substring(command.IndexOf("[") + 1, command.IndexOf("]") - (command.IndexOf("[") + 1));
            if (Directory.Exists(_dir_path))
            {
                if (dirs.Count > 0)
                {
                    foreach (VCS_Dir dir in dirs)
                    {
                        if (string.Compare(dir.dirPath, _dir_path) == 0)
                        {
                            Console.WriteLine("\nAlready done.\n");
                            return;
                        }
                    }
                }
                VCS_Dir _dir = new VCS_Dir();
                _dir.dirPath = _dir_path;
                _dir.dirName = _dir_path.ToString().Replace("\\", "@").Replace("/", "@").Replace(":", "#");
                _dir.dirFiles = new List<VCS_File>();
                dirs.Add(_dir);
                curDir = _dir;
                string to_dir = default_dir + "\\" + curDir.dirName;
                CopyDir(curDir.dirPath, to_dir);
                Console.WriteLine("\nDone.\n");
            }
            else
            {
                Console.WriteLine("\nInvalid path.\n");
            }
        }

        static void CopyDir(string fromDir, string toDir)
        {
            try
            {
                Directory.CreateDirectory(toDir);
                foreach (string s1 in Directory.GetFiles(fromDir))
                {
                    string s2 = toDir + "\\" + Path.GetFileName(s1);
                    File.Copy(s1, s2);
                    VCS_File _file = new VCS_File();
                    _file.attribute = "";
                    _file.curFile = Path.GetFileName(s1);
                    curDir.dirFiles.Add(_file);
                }
                foreach (string s in Directory.GetDirectories(fromDir))
                {
                    CopyDir(s, toDir + "\\" + Path.GetFileName(s));
                }
            }
            catch { }
        }

        static void CommandStatus()
        {
            if (curDir != null)
            {
                bool flag = false;
                Console.WriteLine("\nDirectory: " + curDir.dirPath);
                foreach (string _s in Directory.GetFiles(curDir.dirPath, "*", SearchOption.AllDirectories))
                {
                    flag = false;
                    FileInfo fi = new FileInfo(_s);
                    if (curDir.dirFiles.Count > 0)
                    {
                        foreach (VCS_File _file in curDir.dirFiles)
                        {
                            if (string.Compare(_s.Substring(_s.LastIndexOf("\\") + 1), _file.curFile) == 0)
                            {
                                FileInfo _fi = null;
                                string[] files = Directory.GetFiles(default_dir + "\\" + curDir.dirName, _file.curFile, SearchOption.AllDirectories);
                                if (files.Length > 0)
                                    _fi = new FileInfo(files[0]);
                                else
                                    _fi = fi;
                                flag = true;
                                if (string.Compare(_file.attribute, "") == 0)
                                    Console.ForegroundColor = ConsoleColor.Green;
                                else
                                    Console.ForegroundColor = ConsoleColor.Red;
                                if (fi.Length != _fi.Length)
                                    Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("file: " + _file.curFile + " <<-- " + _file.attribute);
                                if (fi.Length != _fi.Length)
                                {
                                    Console.WriteLine("\tsize: " + fi.Length + "<<--" + _fi.Length);
                                    _file.attribute = "edit";
                                }
                                else
                                    Console.WriteLine("\tsize: " + fi.Length);
                                Console.WriteLine("\tcreated: " + fi.CreationTime);
                                Console.WriteLine("\tmodified: " + fi.LastWriteTime);
                                Console.ResetColor();
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("file: " + _s.Substring(_s.LastIndexOf("\\") + 1) + " <<-- new");
                        Console.WriteLine("\tsize: " + fi.Length);
                        Console.WriteLine("\tcreated: " + fi.CreationTime);
                        Console.WriteLine("\tmodified: " + fi.LastWriteTime);
                        Console.ResetColor();
                    }
                }
                if (curDir.dirFiles.Count > 0)
                {
                    foreach (VCS_File fil in curDir.dirFiles)
                    {
                        flag = false;
                        FileInfo inf = null;
                        string[] files = Directory.GetFiles(default_dir + "\\" + curDir.dirName, fil.curFile, SearchOption.AllDirectories);
                        if (files.Length > 0)
                        {
                            inf = new FileInfo(files[0]);
                            foreach (string _s in Directory.GetFiles(curDir.dirPath, "*", SearchOption.AllDirectories))
                            {
                                if (string.Compare(_s.Substring(_s.LastIndexOf("\\") + 1), fil.curFile) == 0)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                if (Directory.GetFiles(curDir.dirPath, fil.curFile, SearchOption.AllDirectories).Length == 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    fil.attribute = "deleted";
                                    Console.WriteLine("file: " + fil.curFile + " <<-- deleted");
                                    Console.WriteLine("\tsize: " + inf.Length);
                                    Console.WriteLine("\tcreated: " + inf.CreationTime);
                                    Console.WriteLine("\tmodified: " + inf.LastWriteTime);
                                    Console.ResetColor();
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("\nDone.\n");
            }
            else
                Console.WriteLine("\nNo directory selected.\n");
        }

        static void CommandAdd(string command)
        {
            if (curDir != null)
            {
                string _file_name = command.Substring(command.IndexOf("[") + 1, command.IndexOf("]") - (command.IndexOf("[") + 1));
                if (!string.IsNullOrEmpty(_file_name))
                {
                    if (Directory.GetFiles(curDir.dirPath, _file_name, SearchOption.AllDirectories).Length > 0)
                    {
                        if (Directory.GetFiles(default_dir + "\\" + curDir.dirName, _file_name, SearchOption.AllDirectories).Length == 0)
                        {
                            VCS_File _file = new VCS_File();
                            _file.attribute = "added";
                            _file.curFile = _file_name;
                            curDir.dirFiles.Add(_file);
                            Console.WriteLine("\nDone\n");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("\nAlready added.\n");
                            return;
                        }
                    }
                }
                Console.WriteLine("\nFile not found.\n");
            }
            else
                Console.WriteLine("\nNo directory selected.\n");
        }

        static void CommandRemove(string command)
        {
            if (curDir != null)
            {
                string _file_name = command.Substring(command.IndexOf("[") + 1, command.IndexOf("]") - (command.IndexOf("[") + 1));
                if (curDir.dirFiles.Count > 0)
                {
                    foreach (VCS_File _file in curDir.dirFiles)
                    {
                        if (_file.curFile == _file_name)
                        {
                            _file.attribute = "removed";
                            Console.WriteLine("\nDone\n");
                            return;
                        }
                    }
                }
                Console.WriteLine("\nFile not found.\n");
            }
            else
                Console.WriteLine("\nNo directory selected.\n");
        }

        static void CommandApply(string command)
        {
            if (dirs.Count > 0)
            {
                string _dir_path = command.Substring(command.IndexOf("[") + 1, command.IndexOf("]") - (command.IndexOf("[") + 1));
                if (!string.IsNullOrEmpty(_dir_path))
                {
                    foreach (VCS_Dir dir in dirs)
                    {
                        if (string.Compare(dir.dirPath, _dir_path) == 0)
                        {
                            if (dir.dirFiles.Count > 0)
                            {
                                List<VCS_File> rd = new List<VCS_File>();
                                foreach (VCS_File _file in dir.dirFiles)
                                {
                                    string _atr = _file.attribute;
                                    switch (_atr)
                                    {
                                        case "added":
                                            string fileFrom = Directory.GetFiles(dir.dirPath, _file.curFile, SearchOption.AllDirectories)[0];
                                            string fileTo = default_dir + "\\" + dir.dirName + fileFrom.Substring(dir.dirPath.Length);
                                            File.Copy(fileFrom, fileTo);
                                            break;
                                        case "edit":
                                            string filefrom = Directory.GetFiles(dir.dirPath, _file.curFile, SearchOption.AllDirectories)[0];
                                            string fileto = default_dir + "\\" + dir.dirName + filefrom.Substring(dir.dirPath.Length);
                                            File.Delete(Directory.GetFiles(default_dir + "\\" + dir.dirName, _file.curFile, SearchOption.AllDirectories)[0]);
                                            File.Copy(filefrom, fileto);
                                            break;
                                        case "removed":
                                            try { File.Delete(Directory.GetFiles(default_dir + "\\" + dir.dirName, _file.curFile, SearchOption.AllDirectories)[0]); }
                                            catch { }
                                            rd.Add(_file);
                                            break;
                                        case "deleted":
                                            try { File.Delete(Directory.GetFiles(default_dir + "\\" + dir.dirName, _file.curFile, SearchOption.AllDirectories)[0]); }
                                            catch { }
                                            rd.Add(_file);
                                            break;
                                        default:
                                            break;
                                    }
                                    _file.attribute = "";
                                }
                                foreach (VCS_File f in rd)
                                    dir.dirFiles.Remove(f);
                            }
                            Console.WriteLine("\nDone.\n");
                            return;
                        }
                    }
                }
                Console.WriteLine("\nInvalid path.\n");
            }
            else
                Console.WriteLine("\nNo directory.\n");
        }

        static void CommandListbranch()
        {
            if (dirs.Count > 0)
            {
                string s = "";
                foreach (VCS_Dir dir in dirs)
                    s = string.Concat(s, dir.dirPath, " [", dirs.IndexOf(dir) + 1, "]", "\n");
                Console.WriteLine("\n" + s);
                Console.WriteLine("\nDone.\n");
            }
            else
                Console.WriteLine("\nNo directory.\n");
        }

        static void CommandCheckout(string command)
        {
            if (dirs.Count > 0)
            {
                int _num;
                string _dir_path = command.Substring(command.IndexOf("[") + 1, command.IndexOf("]") - (command.IndexOf("[") + 1));
                if (!string.IsNullOrEmpty(_dir_path))
                {
                    if (Int32.TryParse(_dir_path, out _num))
                    {
                        foreach (VCS_Dir dir in dirs)
                        {
                            if (dirs.IndexOf(dir) + 1 == _num)
                            {
                                curDir = dir;
                                Console.WriteLine("\nDone.\n");
                                return;
                            }
                        }
                    }
                    else
                    {
                        foreach (VCS_Dir dir in dirs)
                        {
                            if (string.Compare(dir.dirPath, _dir_path) == 0)
                            {
                                curDir = dir;
                                Console.WriteLine("\nDone.\n");
                                return;
                            }
                        }
                    }
                }
                Console.WriteLine("\nInvalid path.\n");
            }
            else
                Console.WriteLine("\nNo directory.\n");
        }
        #endregion
    }
}
