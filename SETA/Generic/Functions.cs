using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using NLog;

namespace SETA.Generic
{
    public static class Functions
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Get environment based on the brand
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public static EnvironmentsMP GetEnvironmentMp(Brands brand)
        {
            EnvironmentsMP environmentMp;
            switch (brand)
            {
                case Brands.HBVL:
                case Brands.GVA:
                case Brands.NB:
                    environmentMp = EnvironmentsMP.MP1;
                    break;

                case Brands.DS:
                case Brands.LIMNL:
                    environmentMp = EnvironmentsMP.MP2;
                    break;

                default:
                    logger.Info(
                        string.Format(
                            "Given brand ({0}) could not be found for determining the correct MPx environment. Reverting back to the default MP1 environment for further use"
                            , brand
                        )
                    );

                    environmentMp = EnvironmentsMP.MP1;
                    break;
            }

            return environmentMp;
        }

        public static string GetRestApiUrl(EnvironmentsMP environmentMp, Environments environment)
        {
            return string.Format("http://{0}.{1}mediahuis.be/restapi/api/", environmentMp.ToString(), (environment.ToString() == "prod" ? "" : environment.ToString() + ".")); //vb: http://mp2.test.mediahuis.be/restapi/api/
        }

        public static string GetUserIdFromMailAddress(string restApiUrl, string restApiUser, string restApiKey, int userListId, string mail)
        {
            string url = restApiUrl + "sync/lists/" + userListId + "/profiles/search";

            string content = "{'fields':null, 'filter':{'mail':'" + mail + "', 'op':'='}}";
            //string content = "{'fields':['id'], 'filter':{'mail':'" + mail + "', 'op':'='}}";

            string responseString = ExecuteRestApiRequest(url, restApiUser, restApiKey, content, Webmethods.POST);
            return responseString;
        }


        public static void TriggerCampaign(string restApiUrl, string restApiUser, string restApiKey, int campaignId, int userListId, int userId, int actionListId, string actionCode, string gate, Dictionary<string, string> columnParameters, Dictionary<string, string> jsonParameters)
        {
            //Debugging info log
            /*
            logger.Debug(
                string.Format(
                    "TriggerCampaign REST API method called with the following parameters:" + Environment.NewLine
                    + "\t" + "restApiUrl: {0}" + Environment.NewLine
                    + "\t" + "restApiUser: {1}" + Environment.NewLine
                    + "\t" + "restApiKey: {2}" + Environment.NewLine
                    + "\t" + "campaignId: {3}" + Environment.NewLine
                    + "\t" + "userListId: {4}" + Environment.NewLine
                    + "\t" + "userId: {5}" + Environment.NewLine
                    + "\t" + "actionListId: {6}" + Environment.NewLine
                    + "\t" + "actionCode: {7}" + Environment.NewLine
                    + "\t" + "gate: {8}" + Environment.NewLine
                    + "\t" + "columnParameters count: {9}" + Environment.NewLine
                    + "\t" + "jsonParameters count: {10}" 

                    , restApiUrl, restApiUser, restApiKey, campaignId, userListId, userId, actionListId, actionCode, gate, 
                    columnParameters != null ? columnParameters.Count : 0,
                    jsonParameters != null ? jsonParameters.Count : 0
                )
            );
            */


            string columnData = "";
            string jsonData = "";

            if (columnParameters != null)
            {
                if (columnParameters.Count > 0)
                {
                    foreach (var item in columnParameters)
                    {
                        columnData += ", \"" + item.Key + "\":\"" + item.Value + "\"";

                        //Debugging info log
                        /*
                        logger.Debug(
                            string.Format(
                                "columnParameter info - values provided:" + Environment.NewLine
                                + "\t" + "column name: {0}" + Environment.NewLine
                                + "\t" + "value: {1}"

                                , item.Key, item.Value
                            )
                        );
                        */
                    }
                }
            }

            if (jsonParameters != null)
            {
                if (jsonParameters.Count > 0)
                {
                    foreach (var item in jsonParameters)
                    {
                        //Validatie each item.value on validity of json input/syntax
                        try
                        {
                            var obj = Newtonsoft.Json.Linq.JToken.Parse(item.Value);
                        }
                        catch (Exception ex)
                        {
                            logger.Fatal(ex.Message);
                            logger.Fatal(
                                string.Format(
                                    "Invalid json object provided. Unable to parse to json object for column \"{0}\". Provided json object:" + Environment.NewLine
                                    + "\t" + "{1}"

                                    , item.Key, item.Value
                                )
                            );
                            throw;
                        }

                        //Debugging info log
                        /*
                        logger.Debug(
                            string.Format(
                                "jsonParameter info - values provided:" + Environment.NewLine
                                + "\t" + "column name: {0}" + Environment.NewLine
                                + "\t" + "value: {1}"

                                , item.Key, item.Value
                            )
                        );
                        */

                        jsonData += ", \"" + item.Key + "\": \"" + item.Value.Replace("\"", "\\\"") + "\"";
                    }
                }
            }

            string content = "" +
                 "{\"" +
                    "ActionList\":\"" + actionListId + "\"" +
                    ", \"ActionListRecord\": {" +
                        "  \"ACTIONCODE\": \"" + actionCode + "\"" +
                        /***** list of column parameters
                        ", \"COLUMN_NAME_PARAM_1\":\"" + param1 + "\"" +
                        ", \"COLUMN_NAME_PARAM_2\": \"" + param2 + "\"" +
                        ", \"...\":" + ... + "" +
                        ******/
                        columnData +


                        /***** list of column parameters containing json params
                        ", \"MAIL_DATA\": \"" + jsonData.Text.Replace("\"", "\\\"") +
                        ******/
                        jsonData +

                    //"\"" +
                    "}" +
                    ", \"Gate\":\"" + gate + "\"" +
                    ", \"User\":\"" + userId + "\"" +
                    ", \"UserListId\":\"" + userListId +
                "\"}";


            string url = restApiUrl + "async/campaigns/" + campaignId + "/trigger";

            string responseString = ExecuteRestApiRequest(url, restApiUser, restApiKey, content, Webmethods.POST);
        }

        public static void GetSetaConfig()
        {
            //TODO: zorg ervoor dat er een functie bestaat om de config tabel voor seta uit te lezen ivm unit testing
            //enkele variabelen: omgeving van waar de config uitgelezen moet worden
            //omgeving (tst, prev, ...) ivm where clausule van config lijn
        }

        private static string ExecuteRestApiRequest(string url, string restApiUser, string restApiKey, string content, Webmethods webmethod)
        {
            string restApiCallId = "";

            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = webmethod.ToString();
                request.ContentType = "application/json";

                string timestamp = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                string input = string.Format("{0}-{1}-{2}", timestamp, request.Method, request.RequestUri.PathAndQuery);

                string signature = string.Empty;
                var hmacsha256Algorithm = new HMACSHA256(Encoding.UTF8.GetBytes(restApiKey));
                using (hmacsha256Algorithm)
                {
                    hmacsha256Algorithm.Initialize();
                    signature = hmacsha256Algorithm.ComputeHash(Encoding.UTF8.GetBytes(input)).ToHex().ToUpperInvariant();
                }

                request.Headers.Add(HttpRequestHeader.Authorization, string.Format("hmac {0}:{1}:{2}", restApiUser, signature, timestamp));


                if (!string.IsNullOrEmpty(content))
                {
                    using (Stream stream = request.GetRequestStream())
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream))
                        {
                            sw.Write(content);
                        }
                    }
                }

                restApiCallId = Guid.NewGuid().ToString();
                logger.Debug(
                    string.Format(
                        "Start of executing REST API call (call id: {0})" + Environment.NewLine
                        + "\t" + "url: {1}" + Environment.NewLine
                        + "\t" + "restApiUser: {2}" + Environment.NewLine
                        + "\t" + "restApiKey: {3}" + Environment.NewLine
                        + "\t" + "content: {4}" + Environment.NewLine
                        + "\t" + "webmethod: {5}"

                        , restApiCallId, url, restApiUser, restApiKey, content, webmethod)
                );

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                
                string responseString = string.Empty;

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }


                logger.Debug(
                    string.Format(
                        "REST API response (call id: {0})" + Environment.NewLine
                        + "\t" + "status code: {1}" + Environment.NewLine
                        + "\t" + "status description: {2}" + Environment.NewLine
                        + "\t" + "response: {3}"

                        , restApiCallId, response.StatusCode, response.StatusDescription, responseString
                    )
                );

                return responseString;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error while executing REST API call");
                logger.Fatal(ex.Message);
                logger.Fatal(ex.StackTrace);

                throw;
            }
            finally {
                if (!string.IsNullOrEmpty(restApiCallId)) {
                    logger.Debug("End of executing REST API call (call id: " + restApiCallId + ")");
                }
            }
        }




        private static string ToHex(this byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            var chars = new char[checked(data.Length * 2)];
            for (int index = 0; index < data.Length; ++index)
            {
                byte num = data[index];
                chars[2 * index] = ((byte)((uint)num >> 4)).NibbleToHex();
                chars[(2 * index) + 1] = ((byte)(num & 15U)).NibbleToHex();
            }

            return new string(chars);
        }

        private static char NibbleToHex(this byte nibble)
        {
            return (int)nibble < 10 ? (char)(nibble + 48) : (char)(nibble - 10 + 65);
        }
    }
}