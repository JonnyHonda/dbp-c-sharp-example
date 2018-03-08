using System;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Net;
using System.Xml;
using Shipping.api.despatchbay.com;

namespace wsdlConsole
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
		static ShippingService GetAuthoriseService ()
		{
			// Set up some credentials
			NetworkCredential netCredential = new NetworkCredential (apiuser, apikey);
			// Create the service of type Shipping service
			ShippingService Service = new ShippingService (apiendpoint);
			Service.RequestEncoding = System.Text.Encoding.UTF8;
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

			try {
				XmlNode node;
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


        static ServiceType[] GetAvailableServicesMethod (ShipmentRequestType shipment)
		{
			ServiceType[] availableServices = null;
			var Service = GetAuthoriseService ();
			try {
				// Call the GetDomesticServices soap service
				availableServices = Service.GetAvailableServices (shipment);

			} catch (Exception soapEx) {
				Console.WriteLine ("{0}", soapEx.Message);

			}
			return availableServices;
		}



		public static void Main (string[] args)
		{
			LoadConfiguration ();

			int count = 0;
			ServiceType[] availableServices = null;
			/**
			 * Demonstrate getting a list of all services available for a given postcode
			 * 
			 **/

			Console.WriteLine ("\n\n\n============================================");
			try {
                // First we need to build a shipment request object
                ShipmentRequestType Shipment = new ShipmentRequestType();
                //shipment.ServiceID = null;
                //shipment.ClientReference = null;
                //shipment.FollowShipment = null;
                ParcelType Parcel = new ParcelType();
                Parcel.Contents = "Logo";
                Parcel.Height = 10;
                Parcel.Length = 10;
                Parcel.Width = 10;
                Parcel.Weight = 100;
                Parcel.Value = 100;

                ParcelType[] Parcels = new ParcelType[1];
                Parcels[0] = Parcel;

                // Sender Address
                AddressType Address = new AddressType();
                Address.Street = "4077 Korea Street";
                Address.TownCity = "Lincoln";
                Address.PostalCode = "LN6 3QR";
                Address.CountryCode = "GB";

                // Receipient Address
                RecipientAddressType RecipientAddress = new RecipientAddressType();
                RecipientAddress.RecipientName = "Cprl Klingor";
                RecipientAddress.RecipientEmail = "klingor@gmail.com";
                RecipientAddress.RecipientTelephone = "01522 76767676";
                RecipientAddress.RecipientAddress = Address;

                // Sender Address
               // Address = null;
                Address.Street = "Shropshire Street";
                Address.TownCity = "Lincoln";
                Address.PostalCode = "LN1 2UE";
                Address.CountryCode = "GB";

                SenderAddressType SenderAddress = new SenderAddressType();
                SenderAddress.SenderName = "Hawkeye Pearce";
                SenderAddress.SenderAddress = Address;
                SenderAddress.SenderEmail = "john.burrin@thesalegroup.co.uk";
                SenderAddress.SenderTelephone = "01522 000000";

                Shipment.Parcels = Parcels;
                Shipment.RecipientAddress = RecipientAddress;
                Shipment.SenderAddress = SenderAddress;


                availableServices = GetAvailableServicesMethod (Shipment);

				// iterate though the list of returned services
				count = 0;
				foreach (ServiceType element in availableServices) {
					count += 1;
					System.Console.WriteLine ("Service id:{0} - {1} £{2}", element.ServiceID, element.Name, element.Cost);
				}
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}



		}
	}
}