using System;
using Tracking.api.despatchbaypro.com;
using System.Net;
using System.Xml;

namespace Tracking
{
	class MainClass
	{
		private static string apiendpoint;
		private static string apiuser;
		private static string apikey;
		//	private static string ShipmentID;

		/**
		 * Connects to the api endpoint, authorises and returns a ShippingService Object
		 *  
		**/
		static TrackingService GetAuthoriseService ()
		{
			// Set up some credentials
			NetworkCredential netCredential = new NetworkCredential (apiuser, apikey);
			// Create the service of type Shipping service
			TrackingService Service = new TrackingService (apiendpoint);
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

		public static TrackingType[] GetTrackingMethod (string trackingcode)
		{
			TrackingType[] trackingDetail = null;
			var Service = GetAuthoriseService ();
			try {
				// Call the GetDomesticAddressByKey soap service
				trackingDetail = Service.GetTracking (trackingcode);
			} catch (Exception soapEx) {
				Console.WriteLine ("{0}", soapEx.Message);
			}
			return trackingDetail;
		}

		public static void Main (string[] args)
		{
			LoadConfiguration ();
			/**
			 * Demonstrate Getting tracking information
			 * 
			 **/
			Console.WriteLine ("\n\n\n============================================");
			Console.WriteLine ("Calling GetTracking");
			TrackingType[] trackingDetail = null;
			trackingDetail = GetTrackingMethod ("FG028036929GB");
			int count = 0;
			Console.WriteLine ("The following tracking events found");
			foreach (TrackingType element in trackingDetail) {
				count += 1;
				Console.WriteLine ("Key #{0}, Code:{1}, Date:{2}, Description:{3}, Location:{4}, Signitory:{5}, Time:{6}", 
					count, element.Code, element.Date, element.Description, element.Location, element.Signatory, element.Time);
			}
		}
	}
}
