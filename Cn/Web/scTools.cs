/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;                                       //# Required to access the Date/Decimal/Double/Int32 datatypes, Globalization/IFormatProvider enums, Convert, Math
using System.Web;                                   //# Required to access Request, Response, Application, etc.
using System.Collections;				            //# Required to access the HashTable class
using System.Collections.Specialized;				//# Required to access NameValueCollection.
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Web {

    ///########################################################################################################################
    /// <summary>
	/// General helper methods.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>July 29, 2005</LastFullCodeReview>
	public class Tools {
            //#### Declare the required private constants
	    private const string g_cClassName = "Cn.Web.Tools.";

            //#### Declare the required public eNums
        #region eNums
			/// <summary>HTTP server object types.</summary>
		public enum enumServerObject : int {
				/// <summary>Web application collection.</summary>
			cnApplication = 0,
				/// <summary>HTTP POST form collection.</summary>
			cnForm = 1,
				/// <summary>HTTP GET form collection.</summary>
			cnQueryString = 2,
				/// <summary>Web session collection.</summary>
			cnSession = 3
		}
		#endregion


        //##########################################################################################
        //# Public Static Functions
        //##########################################################################################
        ///###############################################################
        /// <summary>
		/// Retrieves the associative array of data elements stored within the passed key/value pair structure.
        /// </summary>
		/// <param name="sKeysValues">String representing a QueryString-style key/value pair structure (i.e. - "key1=value1&amp;key2=value2&amp;key3=value3").</param>
		/// <returns>Hashtable where each index represents a key/value pair stored within the passed <paramref>sKeysValues</paramref>.</returns>
        ///###############################################################
		/// <LastUpdated>January 10, 2006</LastUpdated>
		public static Hashtable KeyValueString(string sKeysValues) {
			Hashtable h_sReturn = new Hashtable();
			string[] a_sKeyValuePairs;
			string[] a_sKeyValue;
			char[] a_charEquals = { '=' };
			int i;

                //#### Split the passed sKeysValues into its a_sKeyValuePairs
			a_sKeyValuePairs = sKeysValues.Split('&');

                //#### Traverse the a_sKeyValuePairs
			for (i = 0; i < a_sKeyValuePairs.Length; i++) {
                    //#### Split the current a_sKeyValuePair apart into a key/value
				a_sKeyValue = a_sKeyValuePairs[i].Split(a_charEquals, 2);

                    //#### If there is a key and a value defined
				if (a_sKeyValue.Length == 2) {
                        //#### If the current key is already within the h_sReturn, raise the error
					if (h_sReturn.Contains(a_sKeyValue[0])) {
						Internationalization.RaiseDefaultError(g_cClassName + "ParseKeyValueString", Internationalization.enumInternationalizationValues.cnDeveloperMessages_CookieMonster_InvalidKeysValuesDuplicateKey, sKeysValues, a_sKeyValue[0]);
					}
                        //#### Else the current key is (so far) unique, so .Add the Key with it's Value into g_hFields
					else {
						h_sReturn.Add(a_sKeyValue[0], KeyValueStringDecoder(a_sKeyValue[1]));
					}
				}
                    //#### Else we have a malformed key definition, so raise the error, reset the return value and exit the loop
				else {
					Internationalization.RaiseDefaultError(g_cClassName + "ParseKeyValueString", Internationalization.enumInternationalizationValues.cnDeveloperMessages_CookieMonster_InvalidKeysValues, sKeysValues, "");
					h_sReturn.Clear();
					break;
				}
			}

                //#### Return the above determined h_sReturn value to the caller
			return h_sReturn;
		}

        ///###############################################################
        /// <summary>
		/// Formats the referenced server structure of data elements into a key/value pair structure.
        /// </summary>
        /// <param name="eServerObject">Enumeration representing the server structure of data elements to encode.</param>
        /// <param name="bIncludeBlankValues">Boolean value indicating if keys with null-string values are to be included.</param>
		/// <returns>String representing a QueryString-style key/value pair structure (i.e. - "key1=value1&amp;key2=value2&amp;key3=value3").</returns>
        ///###############################################################
		/// <LastUpdated>December 2, 2005</LastUpdated>
		public static string KeyValueString(enumServerObject eServerObject, bool bIncludeBlankValues) {
			NameObjectCollectionBase.KeysCollection oKeysCollection;
			string sValue;
			string sReturn = "";
			int i;

                //#### Determine the passed eServerObject and process accordingly
            switch (eServerObject) {
                case enumServerObject.cnApplication: {
                    HttpApplicationState oApplication = HttpContext.Current.Application;

                        //#### Retrieve the oKeysCollection
				    oKeysCollection = oApplication.Keys;

                        //#### Traverse the oKeysCollection
				    for (i = 0; i < oKeysCollection.Count; i++) {
                            //#### Reset the sValue for this loop
					    sValue = Data.Tools.MakeString(oApplication[oKeysCollection[i]], "");

                            //#### If we are supposed to bIncludeBlankValues or the sValue is holding data, append it's Key=Value pair onto the sReturn value
					    if (bIncludeBlankValues || sValue.Length > 0) {
						    sReturn += oKeysCollection[i] + "=" + KeyValueStringEncoder(sValue) + "&";
					    }
				    }
                    break;
                }
                case enumServerObject.cnForm: {
                    NameValueCollection oForm = HttpContext.Current.Request.Form;

                        //#### Retrieve the oKeysCollection
				    oKeysCollection = oForm.Keys;

                        //#### Traverse the oKeysCollection
				    for (i = 0; i < oKeysCollection.Count; i++) {
                            //#### Reset the sValue for this loop
					    sValue = Data.Tools.MakeString(oForm[oKeysCollection[i]], "");

                            //#### If we are supposed to bIncludeBlankValues or the sValue is holding data, append it's Key=Value pair onto the return value
					    if (bIncludeBlankValues || sValue.Length > 0) {
						    sReturn += oKeysCollection[i] + "=" + KeyValueStringEncoder(sValue) + "&";
					    }
				    }
                    break;
                }
                case enumServerObject.cnQueryString: {
                    NameValueCollection oQueryString = HttpContext.Current.Request.QueryString;

                        //#### Retrieve the oKeysCollection
				    oKeysCollection = oQueryString.Keys;

                        //#### Traverse the oKeysCollection
				    for (i = 0; i < oKeysCollection.Count; i++) {
                            //#### Reset the sValue for this loop
					    sValue = oQueryString[oKeysCollection[i]];

                            //#### If we are supposed to bIncludeBlankValues or the sValue is holding data, append it's Key=Value pair onto the return value
					    if (bIncludeBlankValues || sValue.Length > 0) {
						    sReturn += oKeysCollection[i] + "=" + KeyValueStringEncoder(sValue) + "&";
					    }
				    }
                    break;
                }
                case enumServerObject.cnSession: {
                    System.Web.SessionState.HttpSessionState oSession = HttpContext.Current.Session;

                        //#### Retrieve the oKeysCollection
				    oKeysCollection = oSession.Keys;

                        //#### Traverse the oKeysCollection
				    for (i = 0; i < oKeysCollection.Count; i++) {
                            //#### Reset the sValue for this loop
					    sValue = Data.Tools.MakeString(oSession[oKeysCollection[i]], "");

                            //#### If we are supposed to bIncludeBlankValues or the sValue is holding data, append it's Key=Value pair onto the return value
					    if (bIncludeBlankValues || sValue.Length > 0) {
						    sReturn += oKeysCollection[i] + "=" + KeyValueStringEncoder(sValue) + "&";
					    }
				    }
                    break;
                }
                default: {
				    Internationalization.RaiseDefaultError(g_cClassName + "BuildKeyValueString", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eServerObject", Data.Tools.MakeString(eServerObject, ""));
                    break;
                }
            }

                //#### If the sReturn value is holding key/value data, remove the trailing "&" (borrowing the use of i to store the sReturn value's .Length)
             i = sReturn.Length;
             if (i > 0) {
                    //#### Clip off the trailing &
                sReturn = sReturn.Substring(0, sReturn.Length - 1);
             }

			    //#### Return the above determined sReturn value to the caller
			return sReturn;
		}

        ///###############################################################
        /// <summary>
		/// Formats the referenced associative array of data elements into a key/value pair structure.
        /// </summary>
        /// <param name="h_sKeysValues">Hashtable of strings representing the key/value pairs.</param>
        /// <param name="bIncludeBlankValues">Boolean value indicating if keys with null-string values are to be included.</param>
		/// <returns>String representing a QueryString-style key/value pair structure (i.e. - "key1=value1&amp;key2=value2&amp;key3=value3").</returns>
        ///###############################################################
		/// <LastUpdated>December 2, 2005</LastUpdated>
		public static string KeyValueString(Hashtable h_sKeysValues, bool bIncludeBlankValues) {
//			DictionaryEntry oEntry;
			string sReturn = "";
			string sValue;
			int iLen;

                //#### If the passed h_sKeysValues contains data to process
			if (h_sKeysValues != null) {
                    //#### Traverse the passed h_sKeysValues
				foreach (DictionaryEntry oEntry in h_sKeysValues) {
                        //#### Reset the sValue for this loop
					sValue = Data.Tools.MakeString(oEntry.Value, "");

                        //#### If we are supposed to bIncludeBlankValues or the .Value is holding data, append it's Key=Value pair onto the sReturn value
					if (bIncludeBlankValues || sValue.Length > 0) {
						sReturn += Data.Tools.MakeString(oEntry.Key, "") + "=" + KeyValueStringEncoder(sValue) + "&";
					}
				}
			}

                //#### If the sReturn value is holding key/value data, remove the trailing "&"
             iLen = sReturn.Length;
             if (iLen > 0) {
                    //#### Clip off the trailing &
                sReturn = sReturn.Substring(0, iLen - 1);
             }

			    //#### Return the above determined sReturn value to the caller
			return sReturn;
		}

        ///###############################################################
        /// <summary>
		/// Formats the referenced associative array of data elements into a key/value pair structure.
        /// </summary>
        /// <remarks>
        /// NOTE: This is a .NET only function.
        /// </remarks>
        /// <param name="rViewState">Reference to the current <c>System.Web.UI.StateBag</c> representing the key/value pairs.</param>
        /// <param name="bIncludeBlankValues">Boolean value indicating if keys with null-string values are to be included.</param>
		/// <returns>String representing a QueryString-style key/value pair structure (i.e. - "key1=value1&amp;key2=value2&amp;key3=value3").</returns>
        ///###############################################################
		/// <LastUpdated>December 2, 2005</LastUpdated>
		public static string KeyValueString(System.Web.UI.StateBag rViewState, bool bIncludeBlankValues) {
			string[] a_sKeys;
			string sReturn = "";
			string sValue;
			int i;

                //#### If the passed rViewState contains data to process
			if (rViewState != null) {
                    //#### Retrieve the a_sKeys from the passed rViewState
			    a_sKeys = new string[rViewState.Count];
				rViewState.Keys.CopyTo(a_sKeys, 0);

                    //#### Traverse the collected a_sKeys
				for (i = 0; i < rViewState.Count; i++) {
                        //#### Reset the sValue for this loop
					sValue = Data.Tools.MakeString(rViewState[a_sKeys[i]], "");

                        //#### If we are supposed to bIncludeBlankValues or the sValue is holding data, append it's Key=Value pair onto the return value
					if (bIncludeBlankValues || sValue.Length > 0) {
						sReturn += a_sKeys[i] + "=" + KeyValueStringEncoder(sValue) + "&";
					}
				}
			}

                //#### If the sReturn value is holding key/value data, remove the trailing "&" (borrowing the use of i to store the sReturn value's .Length)
             i = sReturn.Length;
             if (i > 0) {
                    //#### Clip off the trailing &
                sReturn = sReturn.Substring(0, i - 1);
             }

			    //#### Return the above determined sReturn value to the caller
			return sReturn;
		}

        ///###############################################################
		/// <summary>
		/// Retrieves the current page's URL.
		/// </summary>
		/// <remarks>     NOTE: This differs from <c>Cn.Web.Settings.Value()</c>'s <c>cnUIDirectoryURL</c> case because this returns the current URL, rather then the URL with the <c>cnUIDirectory</c> appended onto it.</remarks>
		/// <returns>String representing the current page's URL.</returns>
        ///###############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public static string CurrentURL() {
				//#### Pass the call off to our sibling implementation
			return CurrentURL(true, true);
		}

        ///###############################################################
		/// <summary>
		/// Retrieves the current page's URL.
		/// </summary>
		/// <remarks>     NOTE: This differs from <c>Cn.Web.Settings.Value()</c>'s <c>cnUIDirectoryURL</c> case because this returns the current URL, rather then the URL with the <c>cnUIDirectory</c> appended onto it.</remarks>
		/// <param name="bIncludeFileName">Boolean value indicating if the filename of the running script is to be included in the URL.</param>
		/// <param name="bIncludeQueryString">Boolean value indicating if the query string is to be included in the URL.</param>
		/// <returns>String representing the current page's URL.</returns>
        ///###############################################################
		/// <LastUpdated>June 18, 2010</LastUpdated>
		public static string CurrentURL(bool bIncludeFileName, bool bIncludeQueryString) {
			NameValueCollection oServerVars = HttpContext.Current.Request.ServerVariables;
			string sReturn;

				//#### Default our sReturn value to the .ProtocolHostPortURL plus the current request's URL
			sReturn = ProtocolHostPortURL() + oServerVars["url"];

				//#### If we are not supposed to bInclude(the)FileName and one was (seemingly) included within the url, remove it from the sReturn value
			if (! bIncludeFileName && (sReturn.LastIndexOf("/") < sReturn.LastIndexOf("."))) {
				sReturn = sReturn.Substring(0, sReturn.LastIndexOf("/"));
			}

				//#### If we are supposed to bInclude(the)QueryString and a query_string was specified, append it onto the sReturn value
			if (bIncludeQueryString && oServerVars["query_string"].Length > 0) {
				sReturn += "?" + oServerVars["query_string"];
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

        ///###############################################################
		/// <summary>
		/// Retrieves the current page's protocol (aka scheme), host and optional port designation.
		/// </summary>
		/// <remarks>
		/// This function only returns a port designation if the port differ's from the protocol's default port number (HTTP's default port is 80, while HTTPS' is 443).
		///	<para/>Example: http://www.google.com/
		/// <para/>Example: http://www.google.com:8080/
		/// <para/>Example: https://www.google.com/
		/// <para/>Example: https://www.google.com:1443/
		/// </remarks>
		/// <returns>String representing the current page's protocol (aka scheme), host and optional port designation.</returns>
        ///###############################################################
		/// <LastUpdated>June 18, 2010</LastUpdated>
		public static string ProtocolHostPortURL() {
			NameValueCollection oServerVars = HttpContext.Current.Request.ServerVariables;
			string sReturn;

				//#### If this is an HTTPS request
			if (HttpContext.Current.Request.IsSecureConnection) {
//!			if (Cn.Data.Tools.MakeString(HttpContext.Current.Request.ServerVariables["HTTPS"], "").ToLower() == "on") {
					//#### Init our sReturn value with 
				sReturn = "https://" + oServerVars["server_name"];

					//#### If the server_name is running on a port other then 80, append it onto the sReturn value
					//####     NOTE: This is required as if the port is not the protocol default, it needs to be explicitly specified. See: http://computer.howstuffworks.com/web-server8.htm - "When no port is specified, the browser simply assumes that the server is using the well-known port 80"
//! neek
               if (oServerVars["server_port"] != "443") {
                    sReturn += ":" + oServerVars["server_port"];
                }
			}
				//#### Else this is not an HTTPS request
			else {
				sReturn = "http://" + oServerVars["server_name"];

					//#### If the server_name is running on a port other then 80, append it onto the sReturn value
					//####     NOTE: This is required as if the port is not the protocol default, it needs to be explicitly specified. See: http://computer.howstuffworks.com/web-server8.htm - "When no port is specified, the browser simply assumes that the server is using the well-known port 80"
//! neek
               if (oServerVars["server_port"] != "80") {
                    sReturn += ":" + oServerVars["server_port"];
                }
		}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

        ///###############################################################
		/// <summary>
		/// Retrieves the current page's path on the webserver.
		/// </summary>
		/// <param name="bIncludeFileName">Boolean value indicating if the filename of the running script is to be included in the URL.</param>
		/// <returns>String representing the current page's path on the webserver.</returns>
        ///###############################################################
		/// <LastUpdated>June 18, 2010</LastUpdated>
		public static string CurrentPath(bool bIncludeFileName) {
			NameValueCollection oServerVars = HttpContext.Current.Request.ServerVariables;
			string sReturn;

				//#### Append the url onto the sReturn value
			sReturn = oServerVars["url"];

				//#### If we are not supposed to bInclude(the)FileName and one was (seemingly) included within the url, remove it from the sReturn value
			if (! bIncludeFileName && (sReturn.LastIndexOf("/") < sReturn.LastIndexOf("."))) {
				sReturn = sReturn.Substring(0, sReturn.LastIndexOf("/"));
			}

				//#### Return the above determined sReturn value to the caller
			return HttpContext.Current.Server.MapPath(sReturn);
		}

        ///###############################################################
		/// <summary>
		/// Translates the passed full file path into it's URL equivlent.
		/// </summary>
		/// <remarks>
		/// This is basicially the reverse of Server.MapPath(...).
		/// <para/>NOTE: This function does have limitations, as if your application is in a virtual directory (as the physical directory doesn't always match the virtual directory).
		/// </remarks>
		/// <param name="sFullFilePath">String representing the full path to the file.</param>
		/// <returns>String representing the passed full file path into it's URL equivlent.</returns>
        ///###############################################################
		/// <LastUpdated>March 3, 2010</LastUpdated>
		public static string MapURL(string sFullFilePath) {
			string sDirectory = HttpContext.Current.Server.MapPath("~");
			string sReturn;

				//#### Ensure the passed sFullFilePath is a valid string
			sFullFilePath = Cn.Data.Tools.MakeString(sFullFilePath, "").ToLower();

				//#### If the sDirectory is found as-is in the passed sFullFilePath, set our sReturn value
			if (sFullFilePath.IndexOf(sDirectory) > 0) {
				sReturn = HttpContext.Current.Request.ApplicationPath + sFullFilePath.Replace(sDirectory, "").Replace("\\", "/");
			}
				//#### Else try to .ToLower the strings before the .Replace, setting the result into our sReturn value
			else {
				sReturn = HttpContext.Current.Request.ApplicationPath + sFullFilePath.ToLower().Replace(sDirectory.ToLower(), "").Replace("\\", "/");
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}



        ///###############################################################
        /// <summary>
        /// Generates a pseudo-unique user ID based on the user's requesting address, the current timestamp and fractional second.
        /// </summary>
        /// <returns>String representing a pseudo-unique user ID.</returns>
        ///###############################################################
		/// <LastUpdated>December 2, 2005</LastUpdated>
		#region public static string GenerateUserID()
/*		public static string GenerateUserID() {
			System.Collections.Specialized.NameValueCollection oServerVariables = HttpContext.Current.Request.ServerVariables;

                //#### Attempt to make a Unique UserID by concat'ing the remote IP, the Unix-style Epoch timestamp with the above determined dFractionalSecond and a hash of the usernames and user agent (browser)
			return oServerVariables["REMOTE_ADDR"] + Settings.SecondaryDelimiter + DateMath.Timestamp().ToString() + "." + DateTime.Now.Millisecond + Settings.SecondaryDelimiter + Cn.Data.Tools.MD5(oServerVariables["AUTH_USER"] + oServerVariables["REMOTE_USER"] + oServerVariables["HTTP_USER_AGENT"]);
		}*/
		#endregion


        //##########################################################################################
        //# Public Static Debug-Related Functions
        //##########################################################################################
        ///###############################################################
	    /// <summary>
	    /// Outputs the passed value surrounded by square brackets.
	    /// </summary>
	    /// <remarks>
	    /// This function is used for debugging purposes only.
	    /// </remarks>
        /// <param name="oValue">Object representing the value to output. <para/>NOTE: This value is converted into its string equivalent before being outputted.</param>
        ///###############################################################
		/// <LastUpdated>September 15, 2005</LastUpdated>
	    public static void dWrite(object oValue) {
		    HttpContext.Current.Response.Write("[[" + Data.Tools.MakeString(oValue, "") + "]]<br />");
	    }

        ///###############################################################
	    /// <summary>
	    /// Outputs the passed value surrounded by square brackets and ends the program execution.
	    /// </summary>
	    /// <remarks>
	    /// This function is used for debugging purposes only.
	    /// </remarks>
        /// <param name="oValue">Object representing the value to output. <para/>NOTE: This value is converted into its string equivalent before being outputted.</param>
        ///###############################################################
		/// <LastUpdated>May 28, 2007</LastUpdated>
	    public static void dEnd(object oValue) {
		    dWrite(oValue);
		    HttpContext.Current.Response.End();
	    }

        ///###############################################################
	    /// <summary>
	    /// Outputs the current time surrounded by square brackets and optionally ends the program execution.
	    /// </summary>
	    /// <remarks>
	    /// This function is used for debugging purposes only.
	    /// </remarks>
        /// <param name="bEndExecution">Boolean value signaling if the program execution is to end.</param>
        ///###############################################################
		/// <LastUpdated>September 15, 2005</LastUpdated>
	    public static void dGotHere(bool bEndExecution) {
		    HttpContext.Current.Response.Write("[[Got Here @ " + DateTime.Now + "]]<br />");
		    if (bEndExecution) {
		        HttpContext.Current.Response.End();
		    }
	    }


        //#######################################################################################################
        //# Private Functions
        //#######################################################################################################
        ///###############################################################
        /// <summary>
        /// Encodes the passed value for use in a key/value pair structure.
        /// </summary>
        /// <param name="sValue">String representing the value to encode.</param>
        /// <returns>String representing the encoded <param>sValue</param>.</returns>
        ///###############################################################
		/// <LastUpdated>May 28, 2007</LastUpdated>
		private static string KeyValueStringEncoder(string sValue) {
		    string sReturn;

		        //#### 
			sReturn = sValue.Replace("%", "%" + Data.Tools.Asc("%"));
			return sReturn.Replace("&", "%" + Data.Tools.Asc("&"));
		}

        ///###############################################################
        /// <summary>
        /// Decodes the passed value from a key/value pair structure's character escaping.
        /// </summary>
        /// <param name="sValue">String representing the value to encode.</param>
        /// <returns>String representing the decoded <param>sValue</param>.</returns>
        ///###############################################################
		/// <LastUpdated>May 28, 2007</LastUpdated>
		private static string KeyValueStringDecoder(string sValue) {
		    string sReturn;

		        //#### 
			sReturn = sValue.Replace("%" + Data.Tools.Asc("&"), "&");
			return sReturn.Replace("%" + Data.Tools.Asc("%"), "%");
		}

	} //# class Tools

} //# namespace Cn.Web
