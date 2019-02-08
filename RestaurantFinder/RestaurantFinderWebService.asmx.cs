using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestaurantFinder
{
    /// <summary>
    /// Summary description for RestaurantFinderWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RestaurantFinderWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string RestaurantFinder(string street, string city, string state, string zip)
        {
            string str = street;
            string str1 = str.Replace(" ", "+");

            string key = "N7jMycb7LnY7SqyPcZCCFUpdIDny4Khx";
            var serviceURL = "http://www.mapquestapi.com/geocoding/v1/address?key=" + key + "&location=" + str1 + @"&city=" + city + @"&state=" + state + @"&postalCode=" + zip + @"";

            System.Net.HttpWebRequest serviceRequest = (HttpWebRequest)WebRequest.Create(serviceURL);
            serviceRequest.Method = "POST";
            HttpWebResponse serviceResponse = (HttpWebResponse)serviceRequest.GetResponse();
            string latitude="", longitude="";
            if (serviceResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = serviceResponse.GetResponseStream();
                System.Text.Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader readStream = new StreamReader(receiveStream, encode, true);
                var serviceResult = readStream.ReadToEnd();
                JObject jObject = JObject.Parse(serviceResult);
                 latitude = (string)jObject["results"][0]["locations"][0]["latLng"]["lat"];
                 longitude = (string)jObject["results"][0]["locations"][0]["latLng"]["lng"];                
            }
            else
            {
                Console.WriteLine("OOps");
                //return "";
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            string user_key = "b79df45344e2e38f3d8c647aa5e244b7";
            string URL = "https://developers.zomato.com/api/v2.1/search?lat=" + latitude + "&lon=" + longitude;
            //user-key: b79df45344e2e38f3d8c647aa5e244b7
            HttpWebRequest serRequest = (HttpWebRequest)WebRequest.Create(URL);
            serRequest.Headers.Add("user-key", user_key);

            using (WebResponse response = serRequest.GetResponse())
            {
                var responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //responseText.co
                return responseText;
            }
        }
        /*
        [WebMethod]
        public void GetAddress(string street, string city, string state, string zip)

        {
            string str = street;
            string str1 = str.Replace(" ", "+");

            string key = "N7jMycb7LnY7SqyPcZCCFUpdIDny4Khx";
            var serviceURL = "http://www.mapquestapi.com/geocoding/v1/address?key=" + key + "&location=" + str1 + @"&city=" + city + @"&state=" + state + @"&postalCode=" + zip + @"";

            System.Net.HttpWebRequest serviceRequest = (HttpWebRequest)WebRequest.Create(serviceURL);
            serviceRequest.Method = "POST";
            HttpWebResponse serviceResponse = (HttpWebResponse)serviceRequest.GetResponse();

            if (serviceResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = serviceResponse.GetResponseStream();
                System.Text.Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader readStream = new StreamReader(receiveStream, encode, true);
                var serviceResult = readStream.ReadToEnd();
                JObject jObject = JObject.Parse(serviceResult);
                string latitude = (string)jObject["results"][0]["locations"][0]["latLng"]["lat"];
                string longitude = (string)jObject["results"][0]["locations"][0]["latLng"]["lng"];
                RestaurantFinder(latitude, longitude);
                // return jObject.ToString();
            }
            else
            {
                Console.WriteLine("OOps");
                //return "";
            }

        }
    */}

}
