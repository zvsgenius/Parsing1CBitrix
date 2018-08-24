using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;

namespace Parsing1CBitrix
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Files";

            if (!CreateFolders(path))
                return;

            List<string> links = new List<string>();

            ParsingLinks(ref links);

            if (links.Count == 0)
                return;

            #region Creating the Result File
            try
            {
                using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\result.txt"))
                {
                    foreach (string link in links)
                    {
                        string pathFile = CreateFile(link, path);

                        if (pathFile == null)
                            return;

                        string email = ParsingEmail(pathFile);


                        sw.WriteLine(email);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The Result File could not be write:");
                Console.WriteLine(e.Message);
                return;
            }

            #endregion Creating the Result File

            Console.WriteLine("ok");
            Console.ReadLine();
        }

        private static void ParsingLinks(ref List<string> links)
        {
            try
            {

                using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "links.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        links.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process opened files is failed: {0}", e.ToString());
            }
        }

        private static string ParsingEmail(string pathFile)
        {
            string text = null;

            try
            {
                using (StreamReader sr = new StreamReader(pathFile))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        text += line;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nОшибка чтения файла: " + pathFile + "\n" + e.Message);
                return null;
            }

            string email;

            int indexString1 = text.IndexOf("string1", StringComparison.Ordinal);
            if (indexString1 == -1)
                return null;

            int indexString1Start = text.IndexOf("\"", indexString1, StringComparison.Ordinal) + 1;
            if (indexString1Start == -1)
                return null;

            int indexString1End = text.IndexOf("\"", indexString1Start, StringComparison.Ordinal);
            if (indexString1End == -1)
                return null;

            email = text.Substring(indexString1Start, indexString1End - indexString1Start) + "@";

            int indexString3 = text.IndexOf("string3", StringComparison.Ordinal);
            if (indexString3 == -1)
                return null;

            int indexString3Start = text.IndexOf("\"", indexString3, StringComparison.Ordinal) + 1;
            if (indexString3Start == -1)
                return null;

            int indexString3End = text.IndexOf("\"", indexString3Start, StringComparison.Ordinal);
            if (indexString3End == -1)
                return null;

            email += text.Substring(indexString3Start, indexString3End - indexString3Start);

            return email;
        }

        private static bool CreateFolders(string path)
        {
                try
                {
                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("\nСоздание папки вызвало ошибку:\n" + e.Message);
                    return false;
                }
            return true;
        }

        private static string CreateFile(string link, string path)
        {
            WebRequest req = WebRequest.Create(link);

            HttpWebRequest request = (HttpWebRequest)req;
            request.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.117 Safari/537.36";

            WebResponse resp;

            try
            {
                resp = request.GetResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nerror downloading the page\n" + e.Message);
                return null;
            }

            Stream istrm = resp.GetResponseStream();

            int index0 = link.IndexOf("https://www.1c-bitrix.ru/partners/", StringComparison.Ordinal);

            if (index0 == -1)
                return null;

            string newPath = path + "\\" + link.Substring(35);

            try
            {
                using (FileStream fileStream = File.Create(newPath))
                {
                    istrm.CopyTo(fileStream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("nСтраница:" + link + "Не загружена. Ошибка:\n" + e.Message);
                return null;
            }

            resp.Close();
            return newPath;
        }
    }

}
