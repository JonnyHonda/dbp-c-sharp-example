using System;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Net;
using System.Xml;
using Addressing.api.despatchbaypro.com;

namespace Addressing
{
	class MainClass
	{
		private static string apiendpoint;
		private static string apiuser;
		private static string apikey;

		/**
		 * Connects to the api endpoint, authorises and returns a ShippingService Object
		 *  
		**/
		static AddressingService GetAuthoriseService ()
		{
			// Set up some credentials
			NetworkCredential netCredential = new NetworkCredential (apiuser, apikey);
			// Create the service of type Shipping service
			AddressingService Service = new AddressingService (apiendpoint);
			Uri uri = new Uri (Service.Url);
			ICredentials credentials = netCredential.GetCredential (uri, "Basic");
			// Apply the credentials to the service
			Service.Credentials = credentials;
			return Service;
		}

		/**
		 * Loads configuration values from the configuration.xml
		 * 
		 * 
		**/
		static void LoadConfiguration ()
		{
			XmlDocument doc = new XmlDocument ();
			doc.Load ("configuration.xml");
			XmlNode node;
			try {
				node = doc.DocumentElement.SelectSingleNode ("/configuration/apiendpoint");
				apiendpoint = node.InnerText;
				node = doc.DocumentElement.SelectSingleNode ("/configuration/apiuser");
				apiuser = node.InnerText;
				node = doc.DocumentElement.SelectSingleNode ("/configuration/apikey");
				apikey = node.InnerText;
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}

		public static AddressDetailType GetDomesticAddressByLookupMethod (string postcode, string place)
		{
			AddressDetailType addressDetail = null;
			var Service = GetAuthoriseService ();
			try {
				// Call the GetDomesticAddressByKey soap service
				addressDetail = Service.GetDomesticAddressByLookup (postcode, place);
			} catch (Exception soapEx) {
				Console.WriteLine ("{0}", soapEx.Message);
			}
			return addressDetail;
		}

		public static AddressDetailType GetDomesticAddressByKeyMethod (string key)
		{
			AddressDetailType addressDetail = null;
			var Service = GetAuthoriseService ();
			try {
				// Call the GetDomesticAddressByKey soap service
				addressDetail = Service.GetDomesticAddressByKey (key);
			} catch (Exception soapEx) {
				Console.WriteLine ("{0}", soapEx.Message);
			}
			return addressDetail;
		}

		public static AddressKeyType[] GetDomesticAddressKeysByPostcodeMethod (string postcode)
		{
			AddressKeyType[] availableAddresses = null;
			var Service = GetAuthoriseService ();
			try {
				// Call the GetDomesticAddressKeysByPostcode soap service
				availableAddresses = Service.GetDomesticAddressKeysByPostcode (postcode);
			} catch (Exception soapEx) {
				Console.WriteLine ("{0}", soapEx.Message);

			}
			return availableAddresses;

		}

		public static void Main (string[] args)
		{
			/**
			 * Demonstrate Getting a list of address keays from a given postcode
			 * 
			 **/
			Console.WriteLine ("\n\n\n============================================");
			Console.WriteLine ("Calling GetDomesticAddressKeysByPost on LN1 2EU");

			LoadConfiguration ();
			AddressKeyType[] availableAddresses = null;

			try {
				availableAddresses = GetDomesticAddressKeysByPostcodeMethod ("LN12EU");
				int count = 0;
				Console.WriteLine ("The following keys found");
				foreach (AddressKeyType element in availableAddresses) {
					count += 1;
					Console.WriteLine ("Key #{0}: Address {1}", count, element.Key, element.Address);
				}
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}

			/**
			 * Demonstrate getting a single address by its key
			 * 
			 **/
			Console.WriteLine ("\n\n\n============================================");
			Console.WriteLine ("Calling GetDomesticAddressKeysByPost on key 1007");
			AddressDetailType addressDetail = null;
			try {
				addressDetail = GetDomesticAddressByKeyMethod ("LN12EU1007");
				Console.WriteLine ("Address details as follows");
				Console.WriteLine (addressDetail.CompanyName);
				Console.WriteLine (addressDetail.Street);
				Console.WriteLine (addressDetail.Locality);
				Console.WriteLine (addressDetail.Town);
				Console.WriteLine (addressDetail.County);
				Console.WriteLine (addressDetail.Country);
				Console.WriteLine (addressDetail.Postcode);
				Console.WriteLine (addressDetail.Key);
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}

			/**
			 * Demonstrate getting an address by Postcode and place name/number
			 * 
			 **/
			Console.WriteLine ("\n\n\n============================================");
			Console.WriteLine ("Calling GetDomesticAddressByLookup by LN12EU and 7");
			AddressDetailType addressDetail2 = null;
			try {
				addressDetail2 = GetDomesticAddressByLookupMethod ("LN12EU", "7");
				Console.WriteLine ("Address details as follows");
				Console.WriteLine (addressDetail2.CompanyName);
				Console.WriteLine (addressDetail2.Street);
				Console.WriteLine (addressDetail2.Locality);
				Console.WriteLine (addressDetail2.Town);
				Console.WriteLine (addressDetail2.County);
				Console.WriteLine (addressDetail2.Country);
				Console.WriteLine (addressDetail2.Postcode);
				Console.WriteLine (addressDetail2.Key);
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}
	}
}
