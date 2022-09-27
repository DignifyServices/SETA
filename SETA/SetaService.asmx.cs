using SETA.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using NLog;
using System.Configuration;

namespace SETA
{
    /// <summary>
    /// Summary description for SetaService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SetaService : System.Web.Services.WebService
    {
        EnvironmentsMP environmentMp;
        Environments environment;
        Brands brand;

        string restApiUrl;
        string restApiUser =  ConfigurationManager.AppSettings.Get("restApiUser");
        string restApiKey;


        private static Logger logger = LogManager.GetCurrentClassLogger();  //or NLog.LogManager.GetLogger("blah blah") can also be used



        public void TestGetUserIdFromMailAddress(int userlistId, string mail)
        {
            SetDefaults("HBVL", "test");
            int userid = GetUserIdFromMailAddress(userlistId, mail);
        }



        /// <summary>
        /// Specific trigger campaign for retargeting mails
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="userListId"></param>
        /// <param name="mail"></param>
        /// <param name="actionListId"></param>
        /// <param name="actionCode"></param>
        /// <param name="gate"></param>
        /// <param name="brand"></param>
        /// <param name="environment"></param>
        /// <param name="var1"></param>
        [WebMethod]
        public void Retargeting(int campaignId, int userListId, string mail, int actionListId, string actionCode, string gate, string brand, string environment, string var1)
        {
            SetDefaults(brand, environment);

            var columnParameters = new Dictionary<string, string>() {
                { "var1", var1 }
            };

            int userId = GetUserIdFromMailAddress(userListId, mail);

            Functions.TriggerCampaign(restApiUrl, restApiUser, restApiKey, campaignId, userListId, userId, actionListId, actionCode, gate, columnParameters, null);
        }

        /// <summary>
        /// Specific trigger campaign for aboshop voucher mails
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="userListId"></param>
        /// <param name="mail"></param>
        /// <param name="actionListId"></param>
        /// <param name="actionCode"></param>
        /// <param name="gate"></param>
        /// <param name="brand"></param>
        /// <param name="environment"></param>
        /// <param name="var1"></param>
        [WebMethod]
        public void AboshopVoucher(int campaignId, int userListId, string mail, int actionListId, string actionCode, string gate, string brand, string environment, string aboshopData)
        {
            SetDefaults(brand, environment);

            var jsonParameters = new Dictionary<string, string>() {
                { "ABOSHOP_DATA", aboshopData },
            };

            int userId = GetUserIdFromMailAddress(userListId, mail);

            Functions.TriggerCampaign(restApiUrl, restApiUser, restApiKey, campaignId, userListId, userId, actionListId, actionCode, gate, null, jsonParameters);
        }


        public void AboshopConfirmation(int campaignId, int userListId, string mail, int actionListId, string actionCode, string gate, string brand, string environment, string aboshopData)
        {
            SetDefaults(brand, environment);

            var jsonParameters = new Dictionary<string, string>() {
                { "ABOSHOP_DATA", aboshopData }
            };

            int userId = GetUserIdFromMailAddress(userListId, mail);

            Functions.TriggerCampaign(restApiUrl, restApiUser, restApiKey, campaignId, userListId, userId, actionListId, actionCode, gate, null, jsonParameters);
        }


        /*
        [WebMethod]
        public void CommunicationCenter(int campaignId, int userListId, int userId, int actionListId, string actionCode, string gate, string brand, string environment, string jsonData)
        {

            var jsonParameters = new Dictionary<string, string>() {
                { "MAIL_DATA", jsonData }
            };

            TriggerCampaign(campaignId, userListId, userId, actionListId, actionCode, gate, this.brand, this.environment, null, jsonParameters);
        }
        */

        /*
        [WebMethod]
        public void Aanmeldmuur(int campaignId, int userListId, int userId, int actionListId, string actionCode, string gate, string brand, string environment, string flowUrl, string alternateUrl, string allowedArticles, string unsubscribeUrl, string offerName, string variant, string sourceRegistration, string articleTitle)
        {                
            var columnParameters = new Dictionary<string, string>() {
                { "FLOW_URL", flowUrl },
                { "ALTERNATE_URL", alternateUrl },
                { "ALLOWED_ARTICLES", allowedArticles },
                { "UNSUBSCRIBE_URL", unsubscribeUrl },
                { "OFFER_NAME", offerName },
                { "VARIANT", variant },
              //  { "sourceRegistration", sourceRegistration },
              //  { "articleTitle", articleTitle },
            };

            TriggerCampaign(campaignId, userListId, userId, actionListId, actionCode, gate, brand, environment, columnParameters, null);
        }
        */

        /*
        [WebMethod]
        public void test(int campaignId, int userListId, int userId, int actionListId, string actionCode, string gate, string brand, string environment, string var1, string var2, string var3, string jsonData, string jsonData2)
        {
            var columnParameters = new Dictionary<string, string>() {
                { "var1", var1 },
                { "var2", var2 },
                { "var3", var3 },
            };

            var jsonParameters = new Dictionary<string, string>() {
                { "MAIL_DATA", jsonData },
                { "INPUT_DATA2", jsonData2 }
            };

            TriggerCampaign(campaignId, userListId, userId, actionListId, actionCode, gate, brand, environment, columnParameters, jsonParameters);
        }
        */


        private void SetDefaults(string brand, string environment)
        {
            #region set "environment" variable
            if (environment.ToLower() == "uat")
            {
                this.environment = Environments.preview;
            }
            else
            {
                if (!Enum.TryParse(environment, true, out Environments tempEnvironment))
                {
                    logger.Debug(
                        string.Format(
                            "Unable to parse environment based on given params" + Environment.NewLine
                            + "\t" + "brand: {0}" + Environment.NewLine
                            + "\t" + "environment: {1}"
                            , brand, environment
                        )
                    );
                }

                this.environment = tempEnvironment;
            }
            #endregion


            #region set "brand" variable
            if (!Enum.TryParse(brand, true, out Brands tempBrand))
            {
                logger.Debug(
                    string.Format(
                        "Unable to parse \"brand\" based on given params" + Environment.NewLine
                        + "\t" + "brand: {0}"
                        , brand, environment
                    )
                );
            }

            this.brand = tempBrand;
            #endregion

            environmentMp = Functions.GetEnvironmentMp(this.brand);

            #region set "restApiKey" variable
            var restApiKeyConfigName = string.Format("restApiKey_{0}_{1}", environmentMp, this.environment.ToString());
            restApiKey = ConfigurationManager.AppSettings.Get(restApiKeyConfigName);

            if (string.IsNullOrEmpty(restApiKey))
            {
                var errorMessage = "Unable to retrieve restApiKey from web.config value. Looking for web.config parameter with key: " + restApiKeyConfigName + ". Either the parameter name is invalid or the parameter is not present within the web.config file";
                logger.Fatal(errorMessage);

                throw new ConfigurationErrorsException(errorMessage);
            }
            #endregion

            
            restApiUrl = Functions.GetRestApiUrl(environmentMp, this.environment);
        }

        private int GetUserIdFromMailAddress(int userlistId, string mail)
        {
            var response = Functions.GetUserIdFromMailAddress(restApiUrl, restApiUser, restApiKey, userlistId, mail);

            try
            {
                var profileSearch = Newtonsoft.Json.JsonConvert.DeserializeObject<BusinessObjects.ProfileSearch>(response);

                //error: Mail address was not found
                if (profileSearch.Result.Count == 0)
                {
                    logger.Fatal(
                        string.Format(
                            "Mail address was not found" + Environment.NewLine
                            + "\t" + "userlistId: {0}" + Environment.NewLine
                            + "\t" + "mail: {1}"
                            , userlistId, mail
                        )
                    );
                }


                //info: mail address was found multiple times
                if (profileSearch.Result.Count > 1)
                {
                    string multipleResults = "";
                    foreach (var item in profileSearch.Result)
                    {
                        multipleResults +=
                            string.Format(
                                Environment.NewLine 
                                + "\t\t" + "Id: {0}"
                                + "\t\t" + "name: {1}"

                                , item.Id, item.Name
                            );
                    }

                    logger.Debug(
                        string.Format(
                            "Multiple user records have been found according to parameters" + Environment.NewLine
                            + "\t" + "userlistId: {0}" + Environment.NewLine
                            + "\t" + "mail: {1}" + Environment.NewLine
                            + "\t" + "Resultset"
                            + "{2}"
                            , userlistId, mail, multipleResults
                        )
                    );
                }

                return profileSearch.Result.First().Id.Value;
            }
            catch (Exception ex)
            {
                logger.Debug("Error while converting REST API profileSearch response object");
                logger.Fatal(ex.Message);
                logger.Fatal(
                    string.Format(
                        "Debug parameters ------- " + Environment.NewLine
                        + "\t" + "userlistId: {0}" + Environment.NewLine
                        + "\t" + "mail: {1}"
                        , userlistId, mail)
                );
                logger.Fatal(ex.StackTrace);

                throw;
            }
        }
    }
}
