using System;
using System.Net;
using System.Xml;
using Addressing.api.despatchbay.com;

namespace Addressing
{
	class MainClass
	{
		private static string apiendpoint;
		private static string apiuser;
		private static string apikey;

        /// <summary>
        /// Gets the authorise service. Loads a XML configuration file, creates a 
        /// Basic Auth credentioals object and applies to the Service we want to use
        /// </summary>
        /// <returns>The authorise service.</returns>
		static AddressingService GetAuthoriseService ()
		{
			// Set up some credentials
			NetworkCredential netCredential = new NetworkCredential (apiuser, apikey);
			// Create the service of type Addressing service
			AddressingService Service = new AddressingService (apiendpoint);
            Service.RequestEncoding = System.Text.Encoding.UTF8;
            Uri uri = new Uri(Service.Url);
			ICredentials credentials = netCredential.GetCredential (uri, "Basic");
			// Apply the credentials to the service
			Service.Credentials = credentials;
			return Service;
		}

        /// <summary>
        /// Loads the configuration file and sets some static variables
        /// </summary>
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


        /// <summary>
        /// Finds the address method.
        /// </summary>
        /// <returns>The address method.</returns>
        /// <param name="postcode">Postcode.</param>
        /// <param name="place">Place.</param>
        public static AddressType FindAddressMethod(string postcode, string place)
        {
            AddressType addressDetail = new AddressType();
            var Service = GetAuthoriseService();
            try
            {
                // Call the GetDomesticAddressByKey soap service
                addressDetail = Service.FindAddress(postcode, place);
            }
            catch (Exception soapEx)
            {
                Console.WriteLine("{0}", soapEx.Message);
            }
            return addressDetail;
        }

		/// <summary>
        /// Gets the domestic address by key method.
        /// </summary>
        /// <returns>The domestic address by key method.</returns>
        /// <param name="key">Key.</param>
        public static AddressType GetAddressByKeyMethod (string key)
		{
            AddressType addressDetail = new AddressType();
			var Service = GetAuthoriseService ();
			try {
				// Call the GetDomesticAddressByKey soap service
				addressDetail = Service.GetAddressByKey (key);
			} catch (Exception soapEx) {
				Console.WriteLine ("{0}", soapEx.Message);
			}
			return addressDetail;
		}

		/// <summary>
        /// Gets the domestic address keys by postcode method.
        /// </summary>
        /// <returns>The domestic address keys by postcode method.</returns>
        /// <param name="postcode">Postcode.</param>
        public static AddressKeyType[] GetAddressKeysByPostcodeMethod (string postcode)
		{
			AddressKeyType[] availableAddresses = null;
			var Service = GetAuthoriseService ();
			try {
				// Call the GetDomesticAddressKeysByPostcode soap service
                availableAddresses = Service.GetAddressKeysByPostcode (postcode);
			} catch (Exception soapEx) {
				Console.WriteLine ("{0}", soapEx.Message);

			}
			return availableAddresses;
		}

		public static void Main (string[] args)
		{

            LoadConfiguration();
            /*
             * Demonstrate getting an address by Postcode and place name/number
             * 
             **/
            Console.WriteLine("\n\n\n============================================");
            Console.WriteLine("Calling FindAddressMethod by LN12EU and 7");
            AddressType addressDetail2 = new AddressType();
            try
            {
                addressDetail2 = FindAddressMethod("LN12EU", "7");
                Console.WriteLine("Address details as follows");
                Console.WriteLine(addressDetail2.CompanyName);
                Console.WriteLine(addressDetail2.Street);
                Console.WriteLine(addressDetail2.Locality);
                Console.WriteLine(addressDetail2.TownCity);
                Console.WriteLine(addressDetail2.County);
                Console.WriteLine(addressDetail2.CountryCode);
                Console.WriteLine(addressDetail2.PostalCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

			/*
			 * Demonstrate getting a single address by its key
			 * 
			 **/
			Console.WriteLine ("\n\n\n============================================");
            Console.WriteLine ("Calling GetAddressByKeyMethod on key 1007");
            AddressType addressDetail = new AddressType();
			try {
                addressDetail = GetAddressByKeyMethod ("LN12EU1007");
				Console.WriteLine ("Address details as follows");
				Console.WriteLine (addressDetail.CompanyName);
				Console.WriteLine (addressDetail.Street);
				Console.WriteLine (addressDetail.Locality);
				Console.WriteLine (addressDetail.TownCity);
				Console.WriteLine (addressDetail.County);
				Console.WriteLine (addressDetail.CountryCode);
				Console.WriteLine (addressDetail.PostalCode);
				
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}

			/*
			 * Demonstrate getting a list of address keys by Postcode
			 * 
			 **/
			Console.WriteLine ("\n\n\n============================================");
            Console.WriteLine ("Calling GetAddressKeysByPostcodeMethod by LN12EU");
            AddressKeyType[] addressKeyArray = null;
			try {
                addressKeyArray = GetAddressKeysByPostcodeMethod ("LN12EU");
                foreach(AddressKeyType key in addressKeyArray){
                   Console.WriteLine ("Address details as follows");
                    Console.WriteLine (key.Key);
                    Console.WriteLine (key.Address);
                }
				
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}
	}
}
