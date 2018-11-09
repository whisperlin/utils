using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Web;
using System.Web.Script.Serialization;

namespace TranslateWord
{
    class JsonData
    {
        public string from { get; set; }
        public string to { get; set; }
        public string tgt_text { get; set; }
    }

    //{"from":"en","to":"zh","tgt_text":"你还好吗。 \n"}
    public class XiaoNiu
    {

        private const String host = "http://api.niutrans.vip";
        private const String path = "/NiuTransServer/translation";  //"/NiuTransServer/translation";
        private const String method = "GET";
        public static String apikey = "2bcdfc7d5b2f8217b7bdfa3a8e89799b";

      
        public static string TranslateWord(string  text)
        {
            string res = "";
            try
            {
               
                String querys = "from=en&to=zh&apikey=" + apikey + "& src_text=" + text;


                String url = host + path + "?" + querys;
                HttpWebRequest httpRequest = null;
                HttpWebResponse httpResponse = null;


                httpRequest = (HttpWebRequest)WebRequest.Create(url);

                System.Console.WriteLine(url);
                httpRequest.Method = method;


                try
                {
                    httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.ToString());
                    httpResponse = (HttpWebResponse)ex.Response;
                }
                Console.WriteLine(httpResponse.StatusCode);
                Console.WriteLine(httpResponse.Method);
                Console.WriteLine(httpResponse.Headers);
                Stream st = httpResponse.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                res = reader.ReadToEnd();

                //JObject jo = (JObject)JsonConvert.DeserializeObject(jsonText);
                Console.WriteLine(res);
                Console.WriteLine("\n");
                //JsonConverter.DeserializeObject<JsonData>(res);
                //JSONArray getJsonArray = JSONArray.fromObject(res);


                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                //执行反序列化
                JsonData obj = jsonSerializer.Deserialize<JsonData>(res);

                return obj.tgt_text;
            }
            catch (Exception e)
            {
                return res;
            }
            
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
