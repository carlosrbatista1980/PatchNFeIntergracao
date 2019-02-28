using System;
using Microsoft.Win32;
using System.IO;
using System.Security;
using System.Security.Principal;

namespace PatchNFeIntergracao
{
    class Program
    {
        static string log = AppDomain.CurrentDomain.BaseDirectory + @"\log.txt";
        static RegistryKey k = Registry.ClassesRoot;
        static string kk = "Record";
        static RegistryKey HLM = Registry.LocalMachine;
        static string soft = "Software";
        static RegistryKey sceInstallDir = Registry.LocalMachine;
        //static RegistrySecurity rs = new RegistrySecurity();
        static string currentUser = Environment.UserDomainName + "\\" + Environment.UserName;
        static string auxantes = "";
        static string installDir = (string)Registry.GetValue(sceInstallDir.ToString() + @"\SOFTWARE\SCE\SCEExec\AutoMagazine", "installDir", null);
        static string var;        
        static string[] progress = {"#", "##", "###" , "####", "#####", "######", "#######", "########", "#########", "##########"};
        static int count = 0;
        static int contador = 0;

        static WindowsIdentity userID = WindowsIdentity.GetCurrent();
        static WindowsPrincipal user = new WindowsPrincipal(userID);

        
        

        public static void SearchSubKeys(RegistryKey regKey, string skey)
        {
            //RegistryKey regKey = Registry.ClassesRoot;
            //string[] valuenames = regKey.GetSubKeyNames();
            if (regKey == null)
                return;

            foreach (string s in regKey.GetSubKeyNames())
            {
                try
                {
                    /*
                    if (s == "SAM" || s == "SECURITY" || s == "Restricted")
                    {
                        continue;
                    }
                     */

                    using (RegistryKey key = regKey.OpenSubKey(s))
                    {
                        try
                        {
                            //string installDir = (string)Registry.GetValue(sceInstallDir.ToString() + @"\SOFTWARE\SCE\SCEExec\AutoMagazine", "installDir", null);
                            //string var = (string)Registry.GetValue(key.Name, "Codebase", null);

                            //rs = key.GetAccessControl();
                            //rs.AddAccessRule(new RegistryAccessRule(currentUser, RegistryRights.WriteKey | RegistryRights.Delete | RegistryRights.FullControl, AccessControlType.Allow));

                            if (key == null)
                            {
                                return;
                            }
                            else
                            {
                                var = (string)Registry.GetValue(key.Name, "Codebase", null);
                            }

                            //barra de Progresso
                            count++;
                            if (count > 9)
                            {
                                count = 0;
                                
                            }

                            if ("Record" == skey || "Software" == skey)
                            {
                                if (var != null)
                                {
                                    //volta:
                                    var = var.ToLower();
                                    auxantes = var;
                                    ShowPercent();

                                    if (var.Contains("nfeintegracao.dll"))
                                    {

                                        try
                                        {
                                            //rs = key.GetAccessControl();
                                            //rs.AddAccessRule(new RegistryAccessRule(currentUser, RegistryRights.WriteKey | RegistryRights.Delete | RegistryRights.FullControl, AccessControlType.Allow));

                                            auxantes = var.Replace(var, "file:///" + installDir.Replace("\\", "/") + "NFe/NFeIntegracao.DLL");

                                            //goto volta;
                                            regKey.OpenSubKey(s, true).SetValue("CodeBase", auxantes);

                                            using (StreamWriter sw1 = File.AppendText(log))
                                            {
                                                sw1.WriteLine(DateTime.Now.ToString() + " CHAVE: [" + key.ToString() + "]");
                                                sw1.WriteLine("### Mudou de: --> (" + var + ")");
                                                sw1.WriteLine("### Para: --> (" + auxantes + ")");
                                                sw1.WriteLine("");
                                                contador++;
                                                //sw1.WriteLine(key.ToString() + " (Mudou de: --> ({0}) Para: --> ({1})", var, auxantes);
                                            }
                                        }
                                        catch (ArgumentNullException ex)
                                        {
                                            //goto volta;
                                            using (StreamWriter sw2 = File.AppendText(log))
                                            {
                                                sw2.WriteLine("### ArgumentNullException ###: --> " + ex.Message);
                                            }
                                        }
                                        catch (SecurityException e)
                                        {
                                            using (StreamWriter sw2 = File.AppendText(log))
                                            {
                                                sw2.WriteLine("### ERRO ###: --> " + e.Message + " OBS: Tente executar como administrador");
                                            }
                                        }
                                    }

                                    //var = var.Replace("", "");

                                }
                            }
                        }

                        catch (Exception x)
                        {
                            //using (StreamWriter sw3 = File.AppendText(log))
                            //{
                            //    sw3.WriteLine("### SecurityException não foi possível ler  ###: --> " + key.Name + "\\" + s + " --> " + x.Message);
                            //}
                        }
                        finally
                        {
                            SearchSubKeys(key, skey);
                        }
                    }
                }

                catch (Exception e)
                {
                    //using (StreamWriter sw4 = File.AppendText(log))
                    //{
                    //    sw4.WriteLine("### ERRO ###: --> " + regKey.Name + "\\" + s + " --> " + e.Message);
                    //}

                    continue;
                }
            }
            
        
        }


        static void ShowPercent()
        {
            //Console.Clear();
            // para conseguir fazer a barra de progresso eu estava limpando todo o console
            // e reinserindo as informações, o resultado era um efeito de pisca-pisca
            // com essa nova abordagem eu consigo mudar apenas a posição que eu quero dentro da matriz do console

            Console.SetCursorPosition(29, 0); // Coloco o cursor na coluna 29, linha 0
            Console.Write("          "); // insiro 10 caracteres em branco
            Console.SetCursorPosition(29, 0); // volto com o cursor para a posição 29,0
            Console.WriteLine(progress[count]); // reescrevo a barra de progresso
            
            Console.SetCursorPosition(26, 2); // coloco o cursor na coluna 26, linha 2
            Console.WriteLine("[" + contador + "]"); // escrevo a variavel que conta os registros modificados
        }


        static void Main(string[] args)
        {

            File.Delete(log);

            Console.WriteLine("   Processando registros... ");
            Console.WriteLine("");
            Console.WriteLine("   Registros modificados ");

            if (user.IsInRole(WindowsBuiltInRole.Administrator))
            {
                SearchSubKeys(k, kk);
                SearchSubKeys(HLM, soft);
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Execute o programa como administrador");
                Console.WriteLine("");
            }

            

            Console.WriteLine("\n\nProcessou : [" + contador + "] Registros");
            Console.WriteLine("\nAperte enter para finalizar");
            Console.ReadLine();
            
        }
    }
}
