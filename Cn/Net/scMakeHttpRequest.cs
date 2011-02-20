/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;                                       //# Required to access the Date/Decimal/Double/Int32 datatypes
using System.IO;									//# Required to access the Stream class
using System.Net;									//# Required to access the WebProxy/HttpResponse/etc. classes
using System.Text;									//# Required to access the Encoding class
using System.Collections;					        //# Required to access the Hashtable class


namespace Cn.Net {

	///########################################################################################################################
	/// <summary>
	/// Abstraction layer for HttpWebResponse and WebRequest to facilitate HTTP Transactions.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>November 5, 2006</LastFullCodeReview>
	public class MakeHTTPRequest {
	#region MakeHTTPRequest

		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Retrieves an HTTP Transaction, containing only the configured HTTP Request.
		/// </summary>
		/// <param name="sURL">String representing the required URL.</param>
		/// <returns>HTTPTransaction object representing the defined HTTP Request.</returns>
		///############################################################
		/// <LastUpdated>May 28, 2010</LastUpdated>
		public static HTTPTransaction Get(string sURL) {
			return Get(sURL, new RequestConfiguration());
		}

		///############################################################
		/// <summary>
		/// Retrieves an HTTP Transaction, containing only the configured HTTP Request.
		/// </summary>
		/// <param name="sURL">String representing the required URL.</param>
		/// <param name="oConfiguration">RequestConfiguration object representing the required HTTP Request configuration settings.</param>
		/// <returns>HTTPTransaction object representing the defined HTTP Request.</returns>
		///############################################################
		/// <LastUpdated>September 11, 2007</LastUpdated>
		public static HTTPTransaction Get(string sURL, RequestConfiguration oConfiguration) {
			HTTPTransaction oReturn = new HTTPTransaction();

				//#### Setup the oReturn value with the passed sURL, it's related .Request and the .Timeout
            oReturn.URL = sURL;
            oReturn.Request = (HttpWebRequest)WebRequest.Create(sURL);
            oReturn.Request.Timeout = oConfiguration.Timeout;

				//#### If the passed oConfiguration has a .Proxy set, set it within the oReturn value
            if (oConfiguration.Proxy != null) {
                oReturn.Request.Proxy = oConfiguration.Proxy;
            }

				//#### If we are supposed to .UseBasicAuthorization in the .Request
            if (oConfiguration.UseBasicAuthorization) {
					//#### Determine the sBasicAuthHeader, .Encoding the ":" delimited sUsername/sPassword as a byte array as we go
//! Is there a specific advantage to utilize .ASCII in place of .Default?
// Encoding.Default.GetBytes(String.Format("{0}:{1}", sUsername, sPassword));
// Encoding.ASCII.GetBytes(String.Format("{0}:{1}", sUsername, sPassword));
				string sBasicAuthHeader = "Basic " +
					Convert.ToBase64String(Encoding.ASCII.GetBytes(oConfiguration.Username + ":" + oConfiguration.Password)
				);

				//#### .Add the sBasicAuthHeader into the oReturn value's .Request
				oReturn.Request.Headers.Add("Authorization", sBasicAuthHeader);
            }

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves an HTTP Transaction, containing only the HTTP Header information retrieved as part of the HTTP Request/Response.
		/// </summary>
		/// <param name="sURL">String representing the required URL.</param>
		/// <returns>HTTPTransaction object representing the defined HTTP Request.</returns>
		///############################################################
		/// <LastUpdated>May 28, 2010</LastUpdated>
		public static HTTPTransaction GetHeaders(string sURL) {
			return GetHeaders(sURL, new RequestConfiguration());
		}

		///############################################################
		/// <summary>
		/// Retrieves an HTTP Transaction, containing only the HTTP Header information retrieved as part of the HTTP Request/Response.
		/// </summary>
		/// <param name="sURL">String representing the required URL.</param>
		/// <param name="oConfiguration">RequestConfiguration object representing the required HTTP Request configuration settings.</param>
		/// <returns>HTTPTransaction object representing the defined HTTP Request.</returns>
		///############################################################
		/// <LastUpdated>September 1, 2007</LastUpdated>
		public static HTTPTransaction GetHeaders(string sURL, RequestConfiguration oConfiguration) {
				//#### .Connect to the sURL, signaling the request to only retreve the HEADers
			HTTPTransaction oReturn = Connect(sURL, oConfiguration, "HEAD");

				//#### Ensure we .Close the request to clean up the open connections/handles/etc.
			oReturn.Close();

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves an HTTP Transaction, including the file (message body) retrieved as part of the HTTP Request/Response.
		/// </summary>
		/// <param name="sURL">String representing the required URL.</param>
		/// <returns>HTTPTransaction object representing the defined HTTP Request.</returns>
		///############################################################
		/// <LastUpdated>May 28, 2010</LastUpdated>
		public static HTTPTransaction GetFile(string sURL) {
			return GetFile(sURL, new RequestConfiguration());
		}

		///############################################################
		/// <summary>
		/// Retrieves an HTTP Transaction, including the file (message body) retrieved as part of the HTTP Request/Response.
		/// </summary>
		/// <param name="sURL">String representing the required URL.</param>
		/// <param name="oConfiguration">RequestConfiguration object representing the required HTTP Request configuration settings.</param>
		/// <returns>HTTPTransaction object representing the defined HTTP Request.</returns>
		///############################################################
		/// <LastUpdated>September 1, 2007</LastUpdated>
		public static HTTPTransaction GetFile(string sURL, RequestConfiguration oConfiguration) {
				//#### .Connect to the sURL then collect the .Response's Stream
			HTTPTransaction oReturn = Connect(sURL, oConfiguration, "GET");
			oReturn.ResponseFile = oReturn.Response.GetResponseStream();

				//#### Return the above determined oReturn value to the caller
			return oReturn;
/*
			StreamReader oStreamReader;
			string sFileContents;

				//#### Open the .GetResponseStream, reading it's sFileContents
			oStreamReader = new StreamReader(m_oWebResponse.GetResponseStream());
			sFileContents = oStreamReader.ReadToEnd();

				//#### Ensure we .Close the oConnection to clean up it's .WebRequest and .WebResponse
			Close();

				//#### Convert the sFileContents back into a Stream to return to the caller
//! this conversion is WRONG!
			return new MemoryStream( Encoding.ASCII.GetBytes(sFileContents) );
*/
		}


		//##########################################################################################
		//# Private Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Retrieves an HTTP Transaction, including the HTTP Response.
		/// </summary>
		/// <param name="sURL">String representing the required URL.</param>
		/// <param name="oConfiguration">RequestConfiguration object representing the required HTTP Request configuration settings.</param>
		/// <param name="sRequestMethod">String representing the HTTP protocol method to utilize during the HTTP Request (GET, POST, HEAD, etc.).</param>
		/// <returns>HTTPTransaction object representing the defined HTTP Request.</returns>
		///############################################################
		/// <LastUpdated>September 1, 2007</LastUpdated>
		private static HTTPTransaction Connect(string sURL, RequestConfiguration oConfiguration, string sRequestMethod) {
			HTTPTransaction oReturn;

				//#### Setup our oReturn value, including the .Request's sRequestMethod
			oReturn = Get(sURL, oConfiguration);
			oReturn.Request.Method = sRequestMethod.ToUpper();

				//#### Try to get the .Response from the .Request
			try {
				oReturn.Response = ((HttpWebResponse)oReturn.Request.GetResponse());
			}
				//#### Else a WebException of some measure or form occured, so get the .Response from it
			catch (WebException e) {
				oReturn.Response = ((HttpWebResponse)e.Response);
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}
	#endregion


		///########################################################################################################################
		/// <summary>
		/// Represents an HTTP Request's configuration.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview>September 1, 2007</LastFullCodeReview>
		public class RequestConfiguration {
			private WebProxy g_oProxy;
			private string g_sUsername;
			private string g_sPassword;
			private int g_iTimeout;
			private bool g_bUseBasicAuthorization;


			//##########################################################################################
			//# Class Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 9, 2007</LastUpdated>
			public RequestConfiguration() {
					//#### Call the Reset() to init the class vars
				Reset();
			}

			///############################################################
			/// <summary>
			/// Resets the class to its initilized state.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 9, 2007</LastUpdated>
			public void Reset() {
					//#### Default the member-level variables
				g_oProxy = null;
				g_sUsername = "";
				g_sPassword = "";
				g_iTimeout = (30 * 1000);		//# Default the g_iTimeout to 30 seconds
				g_bUseBasicAuthorization = false;
			}


			//##########################################################################################
			//# Class Properties
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Gets/sets a value representing the proxy configuration.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 8, 2007</LastUpdated>
			public WebProxy Proxy {
				get { return g_oProxy; }
				set { g_oProxy = value; }
			}

			///############################################################
			/// <summary>
			/// Gets/sets a value representing the Basic Authorization username.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 8, 2007</LastUpdated>
			public string Username {
				get { return g_sUsername; }
				set { g_sUsername = value; }
			}

			///############################################################
			/// <summary>
			/// Gets/sets a value representing the Basic Authorization password.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 8, 2007</LastUpdated>
			public string Password {
				get { return g_sPassword; }
				set { g_sPassword = value; }
			}

			///############################################################
			/// <summary>
			/// Gets/sets a value representing the request timeout (in milliseconds).
			/// </summary>
			///############################################################
			/// <LastUpdated>September 8, 2007</LastUpdated>
			public int Timeout {
				get { return g_iTimeout; }
				set { g_iTimeout = value; }
			}

			///############################################################
			/// <summary>
			/// Gets/sets Boolean value representing if Basic Authorization is to be utilized.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 8, 2007</LastUpdated>
			public bool UseBasicAuthorization {
				get { return g_bUseBasicAuthorization; }
				set { g_bUseBasicAuthorization = value; }
			}

		} //# public class RequestConfiguration


		///########################################################################################################################
		/// <summary>
		/// Represents an entire HTTP transaction.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview>September 1, 2007</LastFullCodeReview>
		public class HTTPTransaction {
			private HttpWebResponse g_oResponse;
			private HttpWebRequest g_oRequest;
			private Stream g_oResponseFile;
			private string g_sURL;

				//#### Declare the required public eNums
				/// <summary>HTTP Object types</summary>
			public enum enumHttpObjectTypes {
				cnResponse,
				cnRequest
			}

				//#### Declare the required constants
			private const int cBUFFER = 512;

			//##########################################################################################
			//# Class Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 9, 2007</LastUpdated>
			public HTTPTransaction() {
					//#### Call the Reset() to init the class vars
				Reset();
			}

			///############################################################
			/// <summary>
			/// Resets the class to its initilized state.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 9, 2007</LastUpdated>
			public void Reset() {
					//#### Default the member-level variables
				g_oResponse = null;
				g_oRequest = null;
				g_oResponseFile = null;
				g_sURL = "";
			}

			///############################################################
			/// <summary>
			/// Class destructor.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 1, 2007</LastUpdated>
			~HTTPTransaction() {
					//#### Ensure we release any open connections/handles/etc.
				Close();
			}


			//##########################################################################################
			//# Class Properties
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Gets/sets a value representing the HTTP Request.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 1, 2007</LastUpdated>
			public HttpWebRequest Request {
				get { return g_oRequest; }
				set { g_oRequest = value; }
			}

			///############################################################
			/// <summary>
			/// Gets/sets a value representing the HTTP Response.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 1, 2007</LastUpdated>
			public HttpWebResponse Response {
				get { return g_oResponse; }
				set { g_oResponse = value; }
			}

			///############################################################
			/// <summary>
			/// Gets/sets a value representing the file retrieved as part of the HTTP Response.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 1, 2007</LastUpdated>
			public Stream ResponseFile {
				get { return g_oResponseFile; }
				set { g_oResponseFile = value; }
			}

			///############################################################
			/// <summary>
			/// Gets/sets a value representing the HTTP status code for the HTTP Response.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 3, 2007</LastUpdated>
			public int HTTPStatusCode {
				get {
					int iReturn = 0;

						//#### If we have a g_oResponse, reset our iReturn value to it's .StatusCode
					if (g_oResponse != null) {
						iReturn = (int)g_oResponse.StatusCode;
					}

					return iReturn;
				}
			}

			///############################################################
			/// <summary>
			/// Gets/sets a value representing the URLfor the HTTP Request.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 1, 2007</LastUpdated>
			public string URL {
				get { return g_sURL; }
				set { g_sURL = value; }
			}


			//##########################################################################################
			//# Class Read-Only Properties
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Gets the HTTP Response file as a string.
			/// </summary>
			/// <value>String representing the file contents of the HTTP Response file.</value>
			///############################################################
			/// <LastUpdated>January 4, 2010</LastUpdated>
			public string ResponseFileAsString {
				get {
					byte[] a_byteBuffer = new byte[cBUFFER];
					StringBuilder oReturn = new StringBuilder();
					int iByteCount;

//Stream o = new MemoryStream(

						//#### Init the iByteCount with the first .Read of the g_oResponseFile
					iByteCount = g_oResponseFile.Read(a_byteBuffer, 0, cBUFFER);

						//#### While we still have some a_byteBuffer to .Append onto the oReturn value
					while (iByteCount > 0) {
							//#### .Append the current a_byteBuffer section onto the oReturn value, then .Read the next a_byteBuffer from the g_oResponseFile
						oReturn.Append(Encoding.ASCII.GetString(a_byteBuffer, 0, iByteCount));
						iByteCount = g_oResponseFile.Read(a_byteBuffer, 0, cBUFFER);
					}

						//#### Now that we have finished reading the g_oResponseFile, reset it back to the head of the Stream
//!					g_oResponseFile.Seek(0, SeekOrigin.Begin);

						//#### Return the above determined oReturn value back to the caller
					return oReturn.ToString();
				}
			}


			//##########################################################################################
			//# Public Methods
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Releases any open connections, handles, etc. held by the class.
			/// </summary>
			///############################################################
			/// <LastUpdated>September 1, 2007</LastUpdated>
			public void Close() {
					//#### Ensure we .Close the g_oResponse
				if (g_oResponse != null) {
					g_oResponse.Close();
				}
			}

			///############################################################
			/// <summary>
			/// Retrieves the HTTP headers from the desired HTTP object.
			/// </summary>
			/// <param name="eHttpObject">Enumeration representing the desired HTTP object.</param>
			/// <returns>Hashtable of strings representing the HTTP headers of the requested HTTP object.</returns>
			///############################################################
			/// <LastUpdated>September 9, 2007</LastUpdated>
			public Hashtable Headers(enumHttpObjectTypes eHttpObject) {
				Hashtable h_sReturn = null;
				string[] a_sKeys;

					//#### Determine the passed eHttpObject and process accordingly
				switch(eHttpObject) {
					case enumHttpObjectTypes.cnRequest: {
							//#### If we have a g_oRequest to process
						if (g_oRequest != null) {
								//#### Collect the a_sKeys and init the h_sReturn value to the necessary dimensions
							a_sKeys = g_oRequest.Headers.AllKeys;
							h_sReturn = new Hashtable(a_sKeys.Length);

								//#### Traverse the a_sKeys, copying each value into the h_sReturn value
							foreach (string sHeader in a_sKeys) {
								h_sReturn[sHeader] = String.Join("\n", g_oRequest.Headers.GetValues(sHeader));
							}
						}
						break;
					}

				  //case enumHttpObjectTypes.cnResponse:
					default: {
							//#### If we have a g_oResponse to process
						if (g_oResponse != null) {
								//#### Collect the a_sKeys and init the h_sReturn value to the necessary dimensions
							a_sKeys = g_oResponse.Headers.AllKeys;
							h_sReturn = new Hashtable(a_sKeys.Length);

								//#### Traverse the a_sKeys, copying each value into the h_sReturn value
							foreach (string sHeader in a_sKeys) {
								h_sReturn[sHeader] = String.Join("\n", g_oResponse.Headers.GetValues(sHeader));
							}
						}
						break;
					}
				}

					//#### Return the above determined oReturn value to the caller
				return h_sReturn;
			}

			///############################################################
			/// <summary>
			/// Saves the HTTP Response file to the specified path.
			/// </summary>
			/// <param name="sPath">String representing the path to the resulting file.</param>
			/// <param name="bOverwrite">Boolean value representing if any existing file is to be overwritten.</param>
			/// <returns>Boolean value representing if the file was successfully saved.</returns>
			///############################################################
			/// <LastUpdated>September 9, 2007</LastUpdated>
			public bool SaveResponseFile(string sPath, bool bOverwrite) {
//! make SaveFile(sPath, oStream, bOverwrite)?
				byte[] a_byteBuffer = new byte[cBUFFER];
				FileStream oFile;
				int iByteCount;
				bool bReturn = true;

					//#### If we are supposed to ensure that any existing sPath is bOverwrite'n, setup the oFile with a .Create FileMode
				if (bOverwrite) {
					oFile = new FileStream(sPath, FileMode.Create);
				}
					//#### Else we are not supposed to bOverwrite any existing sPath, so setup the oFile with a .CreateNew FileMode
				else {
					oFile = new FileStream(sPath, FileMode.CreateNew);
				}

					//#### Try to .Write the oFile
				try {
						//#### Init the iByteCount with the first .Read of the g_oResponseFile
					iByteCount = g_oResponseFile.Read(a_byteBuffer, 0, cBUFFER);

						//#### While we still have a_byteBuffer section bytes to .Write to the oFile
					while (iByteCount > 0) {
							//#### .Write the current a_byteBuffer to the oFile, then .Read the next a_byteBuffer from the g_oResponseFile
						oFile.Write(a_byteBuffer, 0, iByteCount);
						iByteCount = g_oResponseFile.Read(a_byteBuffer, 0, cBUFFER);
					}

						//#### Now that we have finished reading the g_oResponseFile, .Close the oFile and reset g_oResponseFile back to the head of the Stream
					oFile.Close();
//!					g_oResponseFile.Seek(0, SeekOrigin.Begin);
				}
					//#### Catch any oFile .Write errors from above (resetting the g_oResponseFile back to the head of the Stream while we're at it)
				catch {
					bReturn = false;
//!					g_oResponseFile.Seek(0, SeekOrigin.Begin);
				}

					//#### Return the above determined bReturn value back to the caller
				return bReturn;
			}

		} //# public class HTTPTransaction

	} //# public class MakeHttpRequest

} //# namespace Cn.Web
