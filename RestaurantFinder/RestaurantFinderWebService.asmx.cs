/*
 * Poornima Dixith 
 * Bhuvaneswari Keerthivasan
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Json;

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
        /// <summary>
        /// RestaurantFinder invokes 2 APIs inturn to give result of restaurants in the area specified 
        /// </summary>
        /// <param name="st"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        [WebMethod]
        public List<string> RestaurantFinder(string street, string city, string state, string zip)
        {
            // Remove white spaces and add + to append to URL
            street = street.Replace(" ", "+");
            List<string> output = new List<string>();
            string latitude = string.Empty;
            string longitude = string.Empty;

            //unique API key to invoke the GeoCoding API 
            string key = "N7jMycb7LnY7SqyPcZCCFUpdIDny4Khx";
            //append all params to the base URL
            var serviceURL = "http://www.mapquestapi.com/geocoding/v1/address?key=" + key + "&location=" + street + @"&city=" + city + @"&state=" + state + @"&postalCode=" + zip + @"";

            try
            {
                //construct the HTTP reqeust
                //create an HTTP request instance and prepare request elements
                System.Net.HttpWebRequest serviceRequest = (HttpWebRequest)WebRequest.Create(serviceURL);
                serviceRequest.Method = "POST";
                // Send HTTP request and analyze HTTP response
                // create an instance of HTTPWebResponse  and invoke GetResponse using serviceRequest
                HttpWebResponse serviceResponse = (HttpWebResponse)serviceRequest.GetResponse();
                //check if status code is 200 and proceed
                if (serviceResponse.StatusCode == HttpStatusCode.OK)
                {
                    // gets Stream associated with the response
                    Stream receiveStream = serviceResponse.GetResponseStream();
                    System.Text.Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(receiveStream, encode, true);
                    var serviceResult = readStream.ReadToEnd();
                    //Parse the result JSON
                    JObject jObject = JObject.Parse(serviceResult);
                    //get values of latitude and longitude from the parsed JSON
                    latitude = (string)jObject["results"][0]["locations"][0]["latLng"]["lat"];
                    longitude = (string)jObject["results"][0]["locations"][0]["latLng"]["lng"];
                }
                // if the status code of the response is not 200- an error has occured
                else
                    Console.WriteLine("the response did not contain a HTTP status 200");

                JavaScriptSerializer js = new JavaScriptSerializer();
                //unique key for the API
                string user_key = "b79df45344e2e38f3d8c647aa5e244b7";
                //invoke the Zomato API with latitude and longitude receieved from the first API response
                string URL = "https://developers.zomato.com/api/v2.1/search?lat=" + latitude + "&lon=" + longitude;
                // Send HTTP request and analyze HTTP response
                // create an instance of HTTPWebResponse  and invoke GetResponse using serviceRequest
                HttpWebRequest serRequest = (HttpWebRequest)WebRequest.Create(URL);
                serRequest.Headers.Add("user-key", user_key);
                                
                // create an instance of HTTPWebResponse  and invoke GetResponse using serRequest
                HttpWebResponse serResponse = (HttpWebResponse)serRequest.GetResponse();
                //Check if the request was successful
                if (serResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get the details of all restaurants in the locality and write it in stream
                    var responseText = new StreamReader(serResponse.GetResponseStream()).ReadToEnd();
                    MemoryStream stream1 = new MemoryStream();
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(string));
                    ser.WriteObject(stream1, responseText);
                    stream1.Position = 0;
                    string result = (string)ser.ReadObject(stream1);
                    //Parse the JSON to display the results
                    JObject jObject1 = JObject.Parse(result);
                    //get the count of restaurants to loop through 
                    JArray ja = (JArray)jObject1["restaurants"];
                    for (int i = 1; i <=ja.Count; i++)
                    {
                        //loop through all Objects and add the following values to List
                        string num = i.ToString();
                        output.Add("Restaurant# " + i);
                        output.Add((string)jObject1["restaurants"][i]["restaurant"]["name"]);
                        output.Add((string)jObject1["restaurants"][i]["restaurant"]["location"]["address"]);
                        output.Add((string)jObject1["restaurants"][i]["restaurant"]["cuisines"]);
                        output.Add((string)jObject1["restaurants"][i]["restaurant"]["user_rating"]["aggregate_rating"]);
                        output.Add((string)jObject1["restaurants"][i]["restaurant"]["user_rating"]["rating_text"]);
                        output.Add("----------------------------------------------------------------------------------------");

                    }
                }
            }
            catch (Exception ex)
            {

            }

            //return the output to users
            return output;
        }
    }

}
