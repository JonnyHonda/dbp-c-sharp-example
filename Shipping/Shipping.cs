using Shipping.com.despatchbay.api;
using System;
using System.Net;
using System.Xml;


namespace wsdlConsole
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
		static ShippingService GetAuthoriseService ()
		{
			// Set up some credentials
			NetworkCredential netCredential = new NetworkCredential (apiuser, apikey);

			// Create the service of type Shipping service
			ShippingService Service = new ShippingService ();
			Service.RequestEncoding = System.Text.Encoding.UTF8;
			Uri uri = new Uri (Service.Url);
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


        /// <summary>
        /// Gets the available services method.
        /// See the API v14 documention for more information - https://github.com/despatchbay/api.v14/wiki/Shipping-Service
        /// </summary>
        /// <returns>an array of ServiceType[] </returns>
        /// <param name="shipment">Shipment.</param>
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

        /// <summary>
        /// Add a Shipment Method
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        static String AddShipmentMethod(ShipmentRequestType shipment)
        {
            String ShipmentID = null;
            var Service = GetAuthoriseService();
            try
            {
                // Call the GetDomesticServices soap service
                Console.WriteLine(shipment.ServiceID);
                ShipmentID = Service.AddShipment(shipment);
                
            }
            catch (Exception soapEx)
            {
                Console.WriteLine("{0}", soapEx.Message);

            }
            return ShipmentID;
        }

        /// <summary>
        /// Book Shipment Method
        /// </summary>
        /// <param name="shipments"></param>
        /// <returns></returns>
        static ShipmentReturnType[] BookShipmentsMethod(String[] shipments)
        {
            ShipmentReturnType[] BookingResult = null;
            var Service = GetAuthoriseService();
            try
            {
                // Call the GetDomesticServices soap service
                BookingResult = Service.BookShipments(shipments);

            }
            catch (Exception soapEx)
            {
                Console.WriteLine("{0}", soapEx.Message);

            }
            return BookingResult;
        }


        /// <summary>
        /// Example to Get available services, Add a shipment to the account and the book the collection
        /// </summary>
        /// <param name="args"></param>
        public static void Main (string[] args)
		{
			LoadConfiguration ();

			int count = 0;
			ServiceType[] availableServices = null;

			try {
                // First we need to build a shipment request object
                ShipmentRequestType Shipment = new ShipmentRequestType();
                //shipment.ServiceID = null;
                //shipment.ClientReference = null;
                //shipment.FollowShipment = null;

                ParcelType Parcel = new ParcelType();
                Parcel.Height = 10; // This is cm
                Parcel.Length = 10; // This is cm
                Parcel.Width = 10;  // This is cm
                Parcel.Weight = 10; // This is kg
                Parcel.Value = 100; // This is GBP
                Parcel.Contents = "New Dress for Klingor";


                // Obviously we could add more parcels here
                ParcelType[] Parcels = new ParcelType[1];
                Parcels[0] = Parcel;

                AddressType Address = new AddressType();

                // Receipient Details
                RecipientAddressType RecipientAddress = new RecipientAddressType();
                RecipientAddress.RecipientName = "Cprl Klingor";
                RecipientAddress.RecipientEmail = "klingor@gmail.com";
                RecipientAddress.RecipientTelephone = "01522 76767676";

                Address.CompanyName = "The SaleGroup";
                Address.Street = "Unit 6 The Regatta";
                Address.Locality = "Henley Way";
                Address.TownCity = "Lincoln";
                Address.County = "Lincolnshire";
                Address.PostalCode = "LN6 3QR";
                Address.CountryCode = "GB";

                RecipientAddress.RecipientAddress = Address;

    			SenderAddressType SenderAddress = new SenderAddressType();
                SenderAddress.SenderName = "Hawkeye Pearce";
                SenderAddress.SenderEmail = "john.burrin@thesalegroup.co.uk";
                SenderAddress.SenderTelephone = "01522 000000";

                Address.Street = "West Parade ";
                Address.TownCity = "Lincoln";
                Address.PostalCode = "LN1 1YP";
                Address.County = "Lincolnshire";
                Address.CountryCode = "GB";

                SenderAddress.SenderAddress = Address;

                // Put the Shipment together
                Shipment.Parcels = Parcels;
                Shipment.RecipientAddress = RecipientAddress;
                Shipment.SenderAddress = SenderAddress;
               
                // Call the service
                availableServices = GetAvailableServicesMethod (Shipment);

				// iterate though the list of returned services
				count = 0;
				foreach (ServiceType element in availableServices) {
					count += 1;
                    if (count == 1)
                    {
                        // Manually apply the First service
                        Shipment.ServiceID = element.ServiceID;

                        // Not sure why, but errors with ServiceID = 0
                        // if ServiceIDSpecified is Not set to true;
                        Shipment.ServiceIDSpecified = true;
                    }
					System.Console.WriteLine ("Service id:{0} - {1} £{2}", element.ServiceID, element.Name, element.Cost);
				}
                

                Shipment.ClientReference = "Dummy Client Ref";
                // We need a collection Date
                CollectionDateType CollectionDate = new CollectionDateType();

                // Set the Collection date for tomorrow 
                CollectionDate.CollectionDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
 
                Shipment.CollectionDate = CollectionDate;

                // Add Shipment
                String ShipmentID = AddShipmentMethod(Shipment);
                Console.WriteLine(ShipmentID);

                // Book collection
                // This is deliberatly written to handle one booking request
                String [] Bookings = new String[1];
                Bookings[0] = ShipmentID;

                ShipmentReturnType[] BookingResults = null;
                BookingResults = BookShipmentsMethod(Bookings);
                // iterate though the list of returned services
                count = 0;
                foreach (ShipmentReturnType element in BookingResults)
                {
                    count += 1;
                    System.Console.WriteLine("Service id:{0} - ShipmentID {1} - Label Url{2}", element.ServiceID,element.ShipmentID, element.LabelsURL);
                }
            } catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}
	}
}