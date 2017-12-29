using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionControlSystem
{
    class VCS_Help
    {
        public void ShowHelp()
        {
            Console.WriteLine("\nRun 'help' or '?' to display the help index.\n");
        }

        public void ShowCommands()
        {
            Console.WriteLine("\n");
            ShowCommand1();
            ShowCommand2();
            ShowCommand3();
            ShowCommand4();
            ShowCommand5();
            ShowCommand6();
            ShowCommand7();
            ShowCommand8();
            ShowCommand9();
            Console.WriteLine("\n");
        }

        static void ShowCommand1()
        {
            Console.WriteLine("'init [dir_path]' - инициализировать СКВ для папки");
        }

        static void ShowCommand2()
        {
            Console.WriteLine("'status' - показать статус отслеживаемых файлов");
        }

        static void ShowCommand3()
        {
            Console.WriteLine("'add [file_path]' - добавить файл под версионный контроль");
        }

        static void ShowCommand4()
        {
            Console.WriteLine("'remove [file_path]' - убрать файл из под версионного контроля");
        }

        static void ShowCommand5()
        {
            Console.WriteLine("'apply [dir_path]' - сохранить изменения в папке");
        }

        static void ShowCommand6()
        {
            Console.WriteLine("'listbranch' - показать все отслеживаемые папки");
        }

        static void ShowCommand7()
        {
            Console.WriteLine("'checkout [dir_path]' ИЛИ '[dir_number]' - перейти к указанной директории");
        }

        static void ShowCommand8()
        {
            Console.WriteLine("'clear' - очистить консоль");
        }

        static void ShowCommand9()
        {
            Console.WriteLine("'exit' - выход из программы");
        }
    }
}
