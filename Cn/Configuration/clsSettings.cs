/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Cn.Data;										//# Required to access the Picklists/MetaData class
using Cn.Platform;                                  //# Required to access Specific.AppSettings function


namespace Cn.Configuration {

	///########################################################################################################################
	/// <summary>
	/// Common settings utilized throughout the Cn namespace.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>November 9, 2009</LastFullCodeReview>
	public class Settings {
			//#### Declare the required private constants
			//####     PrimaryDelimiter/SecondaryDelimiter: Must be a single character only and neither can be defined as "%"! Used to delimit internally managed structures. Redefine only if you are having an unavoidable collision with either of the default delimiters.
		private const string g_cDefaultPrimaryDelimiter = "|";
		private const string g_cDefaultSecondaryDelimiter = ":";

			//#### Declare the required private constants
//		private const string g_cClassName = "Cn.Configuration.Settings.";


		//##########################################################################################
		//# Public Static Read-Only Properties
		//##########################################################################################
        ///############################################################
        /// <summary>
        /// Gets the primary common delimiter.
        /// </summary>
		/// <value>String representing the primary common delimiter.</value>
        ///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		public static string PrimaryDelimiter {
			get {
			        //#### Attempt to collect the PrimaryDelimiter from .AppSettings, else use the g_cDefaultPrimaryDelimiter
			    string sReturn = Tools.MakeString(Specific.AppSettings("PrimaryDelimiter"), g_cDefaultPrimaryDelimiter);

                    //#### Return the above determined sReturn value (while ensuring that it's only a single character)
				return sReturn.Substring(0, 1);
			}
		}

        ///############################################################
        /// <summary>
        /// Gets the secondary common delimiter.
        /// </summary>
		/// <value>String representing the secondary common delimiter.</value>
        ///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		public static string SecondaryDelimiter {
			get {
			        //#### Attempt to collect the SecondaryDelimiter from .AppSettings, else use the g_cDefaultSecondaryDelimiter
			    string sReturn = Tools.MakeString(Specific.AppSettings("SecondaryDelimiter"), g_cDefaultSecondaryDelimiter);

                    //#### Return the above determined sReturn value (while ensuring that it's only a single character)
				return sReturn.Substring(0, 1);
			}
		}


        //##########################################################################################
        //# Public Static Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Encodes any common delimiters within the provided value to their %ASCII equivalents.
        /// </summary>
		/// <param name="sValue">String representing the value to encode.</param>
        /// <returns>String value containing the encoded <paramref>sValue</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>April 21, 2010</LastUpdated>
		public static string DelimiterEncoder(string sValue) {
		    string sReturn;

                //#### .Replace any occurance of %, .PrimaryDelimiter and .SecondaryDelimiter within the passed sValue into it's %ASCIIHexCode equivalent
            sReturn = Data.Tools.MakeString(sValue, "").Replace("%", "%" + Data.Tools.Asc("%"));
            sReturn = sReturn.Replace(PrimaryDelimiter, "%" + Data.Tools.Asc(PrimaryDelimiter));
            sReturn = sReturn.Replace(SecondaryDelimiter, "%" + Data.Tools.Asc(SecondaryDelimiter));

                //#### Return the above determined sReturn value to the caller
            return sReturn;
		}

        ///############################################################
        /// <summary>
        /// Decodes any common delimiters within the provided value from their %ASCII equivalents.
        /// </summary>
		/// <param name="sValue">String representing the value to decode.</param>
        /// <returns>String value containing the decoded <paramref>sValue</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>April 21, 2010</LastUpdated>
		public static string DelimiterDecoder(string sValue) {
		    string sReturn;

                //#### Un-.Replace any occurance of the %ASCIIHexCode equivalent of %, .PrimaryDelimiter and .SecondaryDelimiter within the passed sValue to it's original value
            sReturn = Data.Tools.MakeString(sValue, "").Replace("%" + Data.Tools.Asc(SecondaryDelimiter), SecondaryDelimiter);
            sReturn = sReturn.Replace("%" + Data.Tools.Asc(PrimaryDelimiter), PrimaryDelimiter);
            sReturn = sReturn.Replace("%" + Data.Tools.Asc("%"), "%");

                //#### Return the above determined sReturn value to the caller
            return sReturn;
		}


	} //# class Settings


} //# namespace Cn.Configuration
