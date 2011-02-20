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
using System.Web;                                   //# Required to access Request, Response, Application, etc.
using System.Collections;                           //# Required to access the Hashtable class
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Web {

    ///########################################################################################################################
    /// <summary>
    /// Enables the persistent storage of key/value pairs in the end user's cookie collection.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>August 3, 2005</LastFullCodeReview>
	public class CookieMonster {
            //#### Declare the required private variables
		private Hashtable gh_sFields = new Hashtable();
		private string g_sName;
		private string g_sUserID;
		private int g_iMaxLength;
		private int g_iTimeout;
		private bool g_bIsNewCookie;

            //#### Declare the required private constants
		private const string g_cClassName = "Cn.Web.CookieMonster.";
		private const int g_cDefaultMaxLength = 10000;      //# Defines the default max length (in characters) for the entire stored data


        //##########################################################################################
        //# Class Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
		/// Initializes the class.
        /// </summary>
        /// <param name="sName">String representing the base name of the cookie collection.</param>
        ///############################################################
		/// <LastUpdated>July 12, 2005</LastUpdated>
		public CookieMonster(string sName) {
                //#### Call the Reset() to init the class vars
			Reset(sName, -1);
		}

        ///############################################################
        /// <summary>
		/// Initializes the class.
        /// </summary>
        /// <param name="sName">String representing the base name of the cookie collection.</param>
		/// <param name="iMaxLength">Integer representing the maximum character length of the data stored across the entire cookie collection.</param>
        ///############################################################
		/// <LastUpdated>July 12, 2005</LastUpdated>
		public CookieMonster(string sName, int iMaxLength) {
               //#### Call the Reset() to init the class vars
 			Reset(sName, iMaxLength);
		}

        ///############################################################
        /// <summary>
		/// Resets the class to its initilized state.
        /// </summary>
        /// <param name="sName">String representing the base name of the cookie collection.</param>
        ///############################################################
		/// <LastUpdated>August 10, 2005</LastUpdated>
		public void Reset(string sName) {
		        //#### Pass the call off to our sibling implementation, passing in -1 so that g_cDefaultMaxLength is used for the iMaxLength
		    Reset(sName, -1);
		}

        ///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
        /// <param name="sName">String representing the base name of the cookie collection.</param>
		/// <param name="iMaxLength">Integer representing the maximum character length of the stored data.</param>
        ///############################################################
		/// <LastUpdated>August 12, 2005</LastUpdated>
		public void Reset(string sName, int iMaxLength) {
                //#### (re)Init the private vars with their initial values
			g_sName = sName;
			g_sUserID = "";
			g_iMaxLength = iMaxLength;

                //#### If the passed iMaxLength was <= 0, then default to the g_cDefaultMaxLength
			if (g_iMaxLength <= 0) {
				g_iMaxLength = g_cDefaultMaxLength;
			}

                //#### Init the Timeout vars via the class property so that they are both properly set and Clear the gh_sFields
			Timeout = 90;
			Clear();

                //#### Parse the cookies (setting g_bIsNewCookie and gh_sFields while we go)
			Parse();
		}


        //##########################################################################################
        //# Public Read/Write Properties
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Gets/sets a value representing the end user's ID.
        /// </summary>
        ///############################################################
		/// <LastUpdated>August 13, 2004</LastUpdated>
		public string UserID {
			get { return g_sUserID; }
			set { g_sUserID = value; }
		}

        ///############################################################
        /// <summary>
        /// Gets/sets a value representing the base name of this instance's cookie collection.
        /// </summary>
        ///############################################################
		/// <LastUpdated>May 20, 2004</LastUpdated>
		public string Name {
			get { return g_sName; }
			set { g_sName = value; }
		}

        ///############################################################
        /// <summary>
        /// Gets/sets a value representing the desired timeout for this instance's cookie collection.
        /// </summary>
        ///############################################################
		/// <LastUpdated>October 5, 2004</LastUpdated>
		public int Timeout {
			get { return g_iTimeout; }
			set {
				DateTime dTimeout;

                    //#### Set the g_iTimeout to the passed Value
				g_iTimeout = value;

                    //#### If the caller decided to have a session cookie, set dTimeout to null (or in this case .MinValue)
				if (g_iTimeout == 0) {
					dTimeout = DateTime.MinValue;
				}
                    //#### Else the developer decided to have an expiring cookie, so calculate dTimeout
				else {
					dTimeout = DateTime.Now.AddMinutes(g_iTimeout);
				}

                    //#### Call UpdateCookieTimeouts to reset our collection of cookies to the new dTimeout
				UpdateCookieTimeouts(g_sName, dTimeout);
			}
		}


        //##########################################################################################
        //# Public Read-Only Properties
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Gets a value representing the keys present within this instance.
        /// </summary>
        ///############################################################
		/// <LastUpdated>August 7, 2004</LastUpdated>
		public string[] Keys {
			get {
				string[] a_sReturn = new string[gh_sFields.Keys.Count];

                    //#### Copy the .Keys into the a_sReturn value, then return it to the caller
				gh_sFields.Keys.CopyTo(a_sReturn, 0);
				return a_sReturn;
			}
		}

        ///############################################################
        /// <summary>
        /// Get a value representing the 1-based count of key/value pairs within this instance.
        /// </summary>
        ///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public int KeyCount {
			get { return gh_sFields.Count; }
		}

        ///############################################################
        /// <summary>
        /// Gets a value representing the maximum character length of the stored data.
        /// </summary>
        ///############################################################
		/// <LastUpdated>July 12, 2005</LastUpdated>
		public int MaxLength {
			get {  return g_iMaxLength; }
		}

        ///############################################################
        /// <summary>
        /// Gets a value indicating whether this instance represents a new cookie collection.
        /// </summary>
        ///############################################################
		/// <LastUpdated>May 20, 2004</LastUpdated>
		public bool IsNewCookie {
			get { return g_bIsNewCookie; }
		}


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
        ///############################################################
		/// <summary>
		/// Gets/sets the string value present at the referenced row/column (pseudo-parameterized property).
		/// </summary>
		/// <remarks>
		/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
		/// </remarks>
        /// <param name="sKey">String representing the desired key.</param>
		/// <returns>String representing the value of the passed <paramref>sKey</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>December 21, 2005</LastUpdated>
		public string Value(string sKey) {
			return Data.Tools.MakeString(gh_sFields[sKey], "");
		}

        ///############################################################
		/// <summary>
		/// Gets/sets the string value present at the referenced row/column (pseudo-parameterized property).
		/// </summary>
		/// <remarks>
		/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
		/// </remarks>
        /// <param name="sKey">String representing the desired key.</param>
		/// <param name="sNewValue">String representing the new value for the referenced key.</param>
        ///############################################################
		/// <LastUpdated>December 21, 2005</LastUpdated>
		public void Value(string sKey, string sNewValue) {
                //#### If the passed sKey is invalid, raise the error
			if (sKey == null || sKey.Length == 0 || sKey.IndexOf("=") > -1 || sKey.IndexOf("&") > -1 || sKey.IndexOf("%") > -1) {
				Internationalization.RaiseDefaultError(g_cClassName + "Value", Internationalization.enumInternationalizationValues.cnDeveloperMessages_CookieMonster_InvalidKeyName, sNewValue, "");
			}
                //#### Else the passed sKey is valid, so (re)set it's value within gh_sFields
			else {
				gh_sFields[sKey] = sNewValue;
			}
		}

        ///############################################################
        /// <summary>
		/// Converts the key/value pair data within this instance into its equivalent string representation.
        /// </summary>
        /// <returns>String representing the key/value pair data stored within the cookie collection.</returns>
        ///############################################################
		/// <LastUpdated>September 9, 2005</LastUpdated>
		public override string ToString() {
			HttpCookieCollection oCookies = HttpContext.Current.Request.Cookies;
			string sReturn = "";
			int i = 1;

                //#### Do while we still have CookieMonster cookies to process
			while (oCookies[g_sName + i] != null && Data.Tools.IsString(oCookies[g_sName + i].Value)) {
                    //#### Append this sCookie's .Value onto the sReturn value and inc i for the next loop
				sReturn += oCookies[g_sName + i].Value;
				i++;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

        ///############################################################
        /// <summary>
        /// Places the key/value pair data (spaned across this instance's cookie collection) onto the user's system.
        /// </summary>
        /// <remarks>
        /// Each cookie represents a "crumb" of the key/value pair data that is no longer then the HTTP specification maximum of 4kb per cookie.
        /// </remarks>
		/// <exception cref="Cn.CnException">Thrown when the key/value pair data stored within this instance's cookie collection is longer then the specified <c>MaxLength</c>.</exception>
        ///############################################################
		/// <LastUpdated>August 23, 2005</LastUpdated>
		public void Place() {
			HttpCookieCollection oCookies = HttpContext.Current.Response.Cookies;
			HttpCookie oNewCookie;
			string sCMData;
			DateTime dExpires;
			int iCookieIndex = 1;
			int iCrumbSize;
			int iLen;
			int i;

                //#### Drop(all of the)CurrentCookies from the remote system
			DropCurrentCookies(g_sName);

                //#### Init sCMData with the UserID (using g_sUserID's property so one is generated if need be) and it's trailing PrimaryDelimiter, then determine it's iLen
                //####     NOTE: %^#ing dumbass Microsoft %^#ty ass programming fix... it seems they URLDecode incomming cookies, while not URLEncoding them on the way out... %^#ing idiots
//          sCMData = Current.Server.UrlEncode(Me.UserID & PrimaryDelimiter & BuildKeyValueString(gh_sFields, False))
			sCMData = CookieEncode(UserID + Configuration.Settings.PrimaryDelimiter + Tools.KeyValueString(gh_sFields, false));
			iLen = sCMData.Length;

                //#### If the determined iLen of sCMData is less then the .MaxLength
			if (iLen < g_iMaxLength) {
                    //#### Set the iCrumbSize to the length of the g_sName plus 2 (to allow for up to 2 digits in cookie numbers) plus 1 (to allow for the equal sign) plus 5 (to allow for an additional buffer) minus 4096 (4kb of data, which is the absolute max length of a single cookie)
				iCrumbSize = (4096 - (g_sName.Length + 2 + 1 + 5));

                    //#### If the caller decided to have a session cookie, set dExpires to null (or in this case .MinValue)
				if (g_iTimeout == 0) {
					dExpires = DateTime.MinValue;
				}
                    //#### Else the caller decided to have an expiring cookie, so set dExpires to the developer-defined time
				else {
					dExpires = DateTime.Now.AddMinutes(g_iTimeout);
				}

                    //#### Move along the compiled sCMData, placing a single "cookie crumb" as we go
				for (i = 0; i < iLen; i += iCrumbSize) {
						//#### If we still have a full iCrumbSize, set the current Cookie's .Value accordingly
					if ((i + iCrumbSize - 1) < iLen) {
						oNewCookie = new HttpCookie(g_sName + iCookieIndex, sCMData.Substring(i, iCrumbSize));
					}
						//#### Else grab the remainder of the sCMData as less then (or exactly) a full iCrumbSize is left
					else {
						oNewCookie = new HttpCookie(g_sName + iCookieIndex, sCMData.Substring(i));
					}

                        //#### .Add the oNewCookie and set the current Cookie's .Expires to the above determined dExpires
					oNewCookie.Expires = dExpires;
                    oCookies.Add(oNewCookie);

                        //#### Inc the iCookieIndex for the next Cookie
					iCookieIndex += 1;
				}

                    //#### Place a trailing blank Cookie
				oNewCookie = new HttpCookie(g_sName + iCookieIndex, "");
                oCookies.Add(oNewCookie);
				oCookies[g_sName + iCookieIndex].Expires = dExpires;
			}
                //#### Else the determined sCMData is too long to fit into the CookieMonster, so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + "Place", Internationalization.enumInternationalizationValues.cnDeveloperMessages_CookieMonster_ValueTooLong, Data.Tools.MakeString(iLen, ""), Data.Tools.MakeString(g_iMaxLength, ""));
			}
		}

        ///############################################################
        /// <summary>
        /// Explicitly destroys this instance's cookie collection.
        /// </summary>
        ///############################################################
		/// <LastUpdated>October 5, 2004</LastUpdated>
		public void Abandon() {
                //#### Call Clear to remove all of the key/value pairs from gh_sFields, then explicitly Drop(the)CurrentCookies
			Clear();
			DropCurrentCookies(g_sName);
		}

        ///############################################################
        /// <summary>
		/// Removes the referenced key/value pair from this instance.
        /// </summary>
        /// <param name="sKey">String representing the desired key.</param>
		/// <returns>Boolean value signaling if the removal was a success.<para/>Returns true if the passed <paramref>sKey</paramref> was removed, or false if it was not.</returns>
        ///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
		public bool RemoveKey(string sKey) {
		    bool bReturn;

                //#### If the passed sKey is present within gh_sFields, .RemoveKey the sKey and set the bReturn value to true
			if (gh_sFields.Contains(sKey)) {
				gh_sFields.Remove(sKey);
				bReturn = true;
			}
                //#### Else the passed sKey is not within gh_sFields, so set the bReturn value to false
			else {
				bReturn = false;
			}
			
			    //#### Return the above determined bReturn value to the caller
			return bReturn;
		}

        ///############################################################
        /// <summary>
		/// Removes all key/value pairs from this instance.
        /// </summary>
        ///############################################################
		/// <LastUpdated>May 20, 2004</LastUpdated>
		public void Clear() {
			gh_sFields.Clear();
		}


        //##########################################################################################
        //# Private Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Parses any previously set cookie "crumb" data into this instance.
        /// </summary>
        ///############################################################
		/// <LastUpdated>January 10, 2006</LastUpdated>
		private void Parse() {
			string[] a_sCookie;
			string sCMData;

                //#### Default g_bIsNewCookie to true and retrieve the sCMData from our own CookieValue property
			g_bIsNewCookie = true;
			sCMData = ToString();

                //#### If there was data placed into sCMData
			if (sCMData.Length > 0) {
                    //#### Split off the g_sUserID and the remainder of the sCMData
				a_sCookie = HttpContext.Current.Server.UrlDecode(sCMData).Split(Configuration.Settings.PrimaryDelimiter.ToCharArray(), 2);

                    //#### If the sCMData seems to have been properly formed
				if (a_sCookie.Length == 2) {
                        //#### Set g_bIsNewCookie to false, as we're parsing previous cookie data
					g_bIsNewCookie = false;

                        //#### Set the value of g_sUserID and split the rest of sCMData apart into gh_sFields
					g_sUserID = a_sCookie[0];
					gh_sFields = Tools.KeyValueString(a_sCookie[1]);
				}
			}
		}

        ///############################################################
        /// <summary>
        /// Updates the this instance's cookie collection with the provided timeout.
        /// </summary>
        /// <param name="sName">String representing the base name of the cookie collection.</param>
        /// <param name="dTimeout">DateTime representing the desired timeout for the cookie collection.</param>
        ///############################################################
		/// <LastUpdated>October 6, 2004</LastUpdated>
		private void UpdateCookieTimeouts(string sName, DateTime dTimeout) {
			HttpCookieCollection oCookies = HttpContext.Current.Request.Cookies;
			int iCookieIndex = 1;

                //#### While we still have CookieMonster cookies to traverse
			while (oCookies[sName + iCookieIndex] != null) {
                    //#### Update the cookies .Expires with the passed dTimeout
				oCookies[sName + iCookieIndex].Expires = dTimeout;

                    //#### Increment the iCookieIndex for the next loop
				iCookieIndex++;
			}
		}

        ///############################################################
        /// <summary>
        /// Explicitly destroys this instance's cookie collection, both logicially and physicially.
        /// </summary>
        /// <param name="sName">String representing the base name of the cookie collection.</param>
        ///############################################################
		/// <LastUpdated>April 6, 2005</LastUpdated>
		private void DropCurrentCookies(string sName) {
			HttpCookieCollection oCookies = HttpContext.Current.Request.Cookies;
			int iCookieIndex = 1;

                //#### While we still have CookieMonster cookies to traverse
			while (oCookies[sName + iCookieIndex] != null) {
                    //#### Reset the value of the cookie (logicially removing it) then attempt to physicially remove it by setting a .Expires date to the past
                    //####     NOTE: .RemoveKey is not used here as it does not "remove" the cookie from the browser, but only from the local collection.
				oCookies[sName + iCookieIndex].Value = "";
				oCookies[sName + iCookieIndex].Expires = DateTime.Now.AddYears(-10);
//				oCookies.RemoveKey(sName + iCookieIndex);

                    //#### Increment the iCookieIndex for the next loop
				iCookieIndex++;
			}
		}

        ///############################################################
        /// <summary>
        /// Encodes the provided data as a cookie safe string.
        /// </summary>
        /// <param name="sData">String representing the data to encode.</param>
        /// <returns>String representing the encoded <paramref>sData</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>April 7, 2005</LastUpdated>
		private string CookieEncode(string sData) {
		    string sReturn;

                //#### Replace the restricted cookie characters ("semi-colon, comma and white space" per "") from the passed sData, returning the resulting string to the caller
                //####     NOTE: Only the cookie-ish characters are replaced in the passed sData as opposed to simply using Server.UrlEncode in order to produce the shortest possible resulting string to store within the cookie
			sReturn = sData.Replace("%", "%25");
			sReturn = sReturn.Replace("+", "%2b");
			sReturn = sReturn.Replace(" ", "+");
			sReturn = sReturn.Replace("\t", "%09");     //# tab
			sReturn = sReturn.Replace("\n", "%0a");     //# line feed
			sReturn = sReturn.Replace("\r", "%0d");     //# carrage return
			sReturn = sReturn.Replace(",", "%2c");
			return sReturn.Replace(";", "%3b");
		}

	} //# class CookieMonster

} //# namespace Cn.Web

