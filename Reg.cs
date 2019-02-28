using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace PatchNFeIntergracao
{
    class Reg
    {
        public static void SearchSubKeys(RegistryKey root, String searchKey)
        {

            foreach (string keyname in root.GetSubKeyNames())
            {
                try
                {
                    using (RegistryKey key = root.OpenSubKey(keyname))
                    {
                        if (keyname == searchKey)
                            Console.WriteLine("Registry key found : {0} contains {1} values",
                                key.Name, key.ValueCount);

                        SearchSubKeys(key, searchKey);
                    }
                }
                catch (System.Security.SecurityException)
                {
                }
            }
        }

        
    }
}

