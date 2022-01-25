using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Boggle.Models
{
    public class Http
    {
        protected static string Execute(string url, string payload, string method)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;

                if (method.Equals("POST"))
                {
                    //                request.ContentType = "text/plain";
                    byte[] bytes = Encoding.ASCII.GetBytes(payload);
                    request.Method = "POST";
                    request.ContentLength = bytes.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(bytes, 0, bytes.Length);
                    dataStream.Close();
                }

                HttpWebResponse response = null;
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var encoding = ASCIIEncoding.ASCII;
                        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                        {
                            string text = reader.ReadToEnd();
                            return text;
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(url + "\r\n" + x.Message);
                return null;
            }
            Console.WriteLine("SHOULD NOT BE HERE");
            return null;
        }

        public static string RandomWord(int count)
        {
            string json =  Execute("https://random-word-api.herokuapp.com/word?number=100", null, "GET");
            var random_word = JsonConvert.DeserializeObject<List<string>>(json);
            foreach(string word in random_word)
            {
                if (word.Length == count)
                    return word.ToUpper();
            }

            return RandomWord(count); // recursive - if the previous call does not find a word with the word length

        }

        public static bool SearchWord(string word)
        {
            string html = Execute("https://api.dictionaryapi.dev/api/v2/entries/en/" + word, null, "GET");
            if (html==null)
                return false;
            return true;
        }
    }
}
