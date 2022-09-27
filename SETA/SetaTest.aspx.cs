using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NLog;
using SETA.Generic;

namespace SETA
{

    public partial class SetaTest : System.Web.UI.Page
    {
		SetaService service = new SetaService();
		string jsonData;

		protected void Page_Load(object sender, EventArgs e)
        {
			//HBVL test (Retargeting)
			//service.Retargeting(110214, 10303, "jimmy.guarraci@mediahuis.be", 12659, "RTEMAIL1EXA-HBVL", "RTEMAIL1EXA-HBVL", "HBVL", "test", "DIGI");

			//hbvl prev (Retargeting)
			//service.Retargeting(108238, 10303, "jimmy.guarraci@mediahuis.be", 12718, "RTEMAIL1EXA-HBVL", "RTEMAIL1EXA-HBVL", "HBVL", "preview", "DIGI");

			//DS prev (Retargeting)
			//service.Retargeting(153042, 2855, "jimmy.guarraci@mediahuis.be", 12604, "RTEMAIL1PR-DS", "RTEMAIL1PR-DS", "DS", "preview", "DIGI");

			//get mail address
			//service.TestGetUserIdFromMailAddress(10303, "jimmy.guarraci@mediahuis.be");


			//hbvl test (Aboshop vouchers)
			//AboshopVoucherHbvlTst();
		}

		private void AboshopVoucherHbvlTst()
        {
			jsonData = @"
	[{
		""ID"": 1,
		""PARAM"": ""ABOSHOP_DATA"",
		""CONTENT"": {
			""PERSON_FIRSTNAME"": ""Tim"",
			""PERSON_LASTNAME"": ""Van Hal"",
			""PERSON_MAIL"": ""jimmy.guarraci@mediahuis.be"",
			""PERSON_ACCOUNTNUMBER"": null,
			""ADDRESS_STREET"": ""Katwilgweg"",
			""ADDRESS_NUMBER"": ""2"",
			""ADDRESS_BOX"": """",
			""ADDRESS_ZIPCODE"": ""2050"",
			""ADDRESS_CITY"": ""Antwerpen"",
			""ADDRESS_COUNTRY"": ""BE"",
			""PRODUCT_READING_TYPE"": ""SLIM"",
			""PRODUCT_DESCRIPTION"": ""Digitale krant in de week + papieren krant op zaterdag"",
			""PRODUCT_PLUS_ARTICLES"": false,
			""PRODUCT_DIGITAL_PUBLICATIONS"": false,
			""PRODUCT_PAPER_WEEK"": false,
			""PRODUCT_PAPER_WEEKEND"": false,
			""PRODUCT_PAPER_COMPONENT"": false,
			""PRODUCT_UNIT_PRICE"": 67.5,
			""PRODUCT_DIGITAL_RIGHTS"": false,
			""PRODUCT_DISCOUNT"": ""-34% korting"",
			""PRODUCT_PRICE_DESCRIPTION"": ""<strong>€ 22,50</strong>/maand\n"",
			""ORDER_TYPE"": ""voucher"",
			""ORDER_CATEGORY"": ""voucherPurchase"",
			""ORDER_DURATION_NUMBER"": 3,
			""ORDER_DURATION_DATEPART"": ""m"",
			""ORDER_PAYMENT_TYPE"": ""bancontact"",
			""ORDER_PAYMENT_TYPE_EXTRA_INFO"": """",
			""ORDER_RECURRING_PAYMENT"": false,
			""ORDER_DELIVERY_TYPE"": ""Mail"",
			""ORDER_START_DATE"": null,
			""ORDER_DELIVERY_STORE"": null,
			""ORDER_DELIVERY_STORE_ADDRESS"": null,
			""ORDER_DELIVERY_STORE_COUNTRY"": null,
			""ORDER_FIXED_TERM"": null,
			""AGREEMENT_ID"": null,
			""VOUCHER_CODE"": ""2BA5-DD62-9F54""
		}
	}]";
			service.AboshopVoucher(110484, 10303, "jimmy.guarraci@mediahuis.be", 12605, "VOUCHER_PURCHASE", "ABOSHOP_VOUCHER_HBVL", "HBVL", "test", jsonData);

		}
	}
}