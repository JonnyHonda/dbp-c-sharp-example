using System;
using System.Net;
using System.Xml;
using Tracking.api.despatchbay.com;

namespace Tracking
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
		static TrackingService GetAuthoriseService ()
		{
			// Set up some credentials
			NetworkCredential netCredential = new NetworkCredential (apiuser, apikey);
			// Create the service of type Tracking service
			TrackingService Service = new TrackingService (apiendpoint);
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
        /// Gets the tracking method.
        /// </summary>
        /// <returns>TrackingReturnType</returns>
        /// <param name="trackingcode">Trackingcode.</param>
        public static TrackingReturnType GetTrackingMethod (string trackingcode)
		{
            TrackingReturnType trackingDetail = new TrackingReturnType();
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
			/*
			 * Demonstrate Getting tracking information
			 * To run correctly you'll need a valid tracking number in despatch bay
			 * And it has to a parcel in your account
			 **/
			// Test if input arguments were supplied:
			if (args.Length == 0)
			{
				System.Console.WriteLine("Please enter a tracking number.");
				System.Console.WriteLine("Usage: Tracking.exe PBBQ3451945001");
				return;
			}
			string trackingNumber = args[0];
			Console.WriteLine ("\n\n\n============================================");
			Console.WriteLine ("Calling GetTracking");
            TrackingReturnType trackingDetail = new TrackingReturnType();
			trackingDetail = GetTrackingMethod (trackingNumber);
            Console.WriteLine("Tracking Detail for Courier {0}",trackingDetail.CourierName);
            Console.WriteLine("Service {0}", trackingDetail.ServiceName);
            Console.WriteLine("Tracking Type {0}", trackingDetail.TrackingType);
				int count = 0;
				Console.WriteLine ("The following tracking events found");
			try{
				foreach (TrackingEventType element in trackingDetail.TrackingHistory) {
					count += 1;
					Console.WriteLine ("Key #{0}, Code:{1}, Date:{2}, Description:{3}, Location:{4}, Signitory:{5}, Time:{6}", 
						count, element.Code, element.Date, element.Description, element.Location, element.Signatory, element.Time);
				}
			}catch(Exception ex){
				Console.WriteLine ("No tracking available for {0}", args[0]);
			}
		}
	}
}
