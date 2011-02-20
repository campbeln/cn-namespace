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
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;						            //# Required by MD5
using System.Security.Cryptography;		            //# Required by MD5
using System.Text.RegularExpressions;				//# Required by IsGUID
using Cn.Collections;
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Data {

		//#### Declare the required enums
	#region eNums
		/// <summary>Reconized database servers/data sources.</summary>
	public enum enumDataSource : int {
			/// <summary>Not a RDBMS.</summary>
		cnNone = -1,
		//cnMySQL = 0,
			/// <summary>Oracle.</summary>
		cnOracle = 1,
			/// <summary>Microsoft's SQL*Server.</summary>
		cnSQLServer = 2,
		//cnSybase = 3,
		//cnPostgreSQL = 4,
		//cnmSQL = 5,
		//cnFirebird = 6,
		//cnInterBase = 7,
		//cnSAPDb = 8,
		//cnMaxDB = 9,
		//cnDB2 = 10,
		//cnInformix = 11,
		//cnAccess = 12,

		/// <summary>Microsoft's Sharepoint underlying data structure.</summary>
		cnSharePointPseudobase = 100
	}
	#endregion


    ///########################################################################################################################
    /// <summary>
	/// General helper methods.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>July 29, 2005</LastFullCodeReview>
	public class Tools {
            //#### Declare the required private constants
	  //private const string g_cClassName = "Cn.Data.Tools.";


        //##########################################################################################
        //# Public Static Functions
        //##########################################################################################
        ///###############################################################
		/// <summary>
		/// Appends the provided value onto the end of the provided array.
		/// </summary>
		/// <param name="a_oArray">Array of Type T that represents the values to copy.</param>
		/// <param name="oValue">Type T representing the new value to add onto the end of the Array of Type T.</param>
		/// <returns>Array of Type T's representing a deep copy of the passed <paramref name="a_oArray"/> with the passed <paramref name="oValue"/> appended onto the end.</returns>
        ///###############################################################
		/// <LastUpdated>June 21, 2010</LastUpdated>
		public static T[] Push<T>(T[] a_oArray, T oValue) {
			T[] a_oReturn;
			int iOrigionalLength;
			int i;

				//#### If the passed array is valid
			if (a_oArray != null && a_oArray.Length > 0) {
					//#### Set the iOrigionalLength of the passed a_oArray, setup our a_oReturn value with an extra space the fill it with the passed oValue
				iOrigionalLength = a_oArray.Length;
				a_oReturn = new T[iOrigionalLength + 1];
				a_oReturn[iOrigionalLength] = oValue;

					//#### Traverse the passed a_oArray, copying each value into our a_oReturn value
				for (i = 0; i < iOrigionalLength; i++) {
					a_oReturn[i] = a_oArray[i];
				}
			}
				//#### Else the passed a_oArray was logicially or physicially empty, so just fill our a_oReturn value with the passed oValue
			else {
				a_oReturn = new T[1];
				a_oReturn[0] = oValue;
			}

				//#### Return the above determined a_oReturn value to the caller
			return a_oReturn;
		}

        ///###############################################################
		/// <summary>
		/// Removes the last index of the provided array.
		/// </summary>
		/// <param name="a_sArray">Array of Type T that represents the values to copy.</param>
		/// <returns>Array of Type T representing a deep copy of the passed <paramref name="a_oArray"/> with last index removed.</returns>
        ///###############################################################
		/// <LastUpdated>June 21, 2010</LastUpdated>
		public static T[] Pop<T>(T[] a_oArray) {
			T[] a_oReturn;
			int iOrigionalLength;
			int i;

				//#### If the passed array is valid
			if (a_oArray != null && a_oArray.Length > 1) {
					//#### Set the iOrigionalLength of the passed a_oArray, setup our a_oReturn value with an extra space the fill it with the passed sValue
				iOrigionalLength = a_oArray.Length;
				a_oReturn = new T[iOrigionalLength - 1];

					//#### Traverse the passed a_oArray, copying each value into our a_oReturn value
				for (i = 0; i < (iOrigionalLength - 1); i++) {
					a_oReturn[i] = a_oArray[i];
				}
			}
				//#### Else the passed a_oArray was logicially or physicially empty or would have become so (as there was max 1 index), so just set our a_oReturn value to a 0 .Length array
			else {
				a_oReturn = new T[0];
			}

				//#### Return the above determined a_oReturn value to the caller
			return a_oReturn;
		}

        ///###############################################################
        /// <summary>
        /// Pads the left side of the provided value with the referenced character.
        /// </summary>
        /// <param name="oValue">Object representing the value to pad. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
        /// <param name="sPadCharacter">String representing the single character to use to pad the provided value.</param>
        /// <param name="iLength">1-based integer representing the required length of the return value.</param>
		/// <returns>String representing the padded value.</returns>
        ///###############################################################
		/// <LastUpdated>February 19, 2010</LastUpdated>
	    public static string LPad(object oValue, string sPadCharacter, int iLength) {
		    string sReturn = MakeString(oValue, "").Trim();
		    int iPadCharacterLength;
		    int i;

				//#### Ensure the passed sPadCharacter is a valid string and iLength is valid
			sPadCharacter = MakeString(sPadCharacter, " ");
			iPadCharacterLength = sPadCharacter.Length;
			iLength = (iLength < sReturn.Length ? sReturn.Length : iLength);

			    //#### Traverse the sReturn value, prepending one sPadCharacter at a time until we reach the required iLength
		    for (i = sReturn.Length; i < iLength; i = i + iPadCharacterLength) {
			    sReturn = sPadCharacter + sReturn;
		    }

				//#### If the passed sPadCharacter was longer then a single character and the sReturn value is longer then the iLength, .Substring it
				//####     NOTE: .Length(10) - iLength(6) == 4 - 1(as .Length(10) is 1 based) - 1(as iLength(6) is 1 based) == 2
			if (iPadCharacterLength > 1 && sReturn.Length > iLength) {
				sReturn = sReturn.Substring(sReturn.Length - iLength);
			}

			    //#### Return the above determined sReturn value to the caller
		    return sReturn;
	    }

        ///###############################################################
        /// <summary>
        /// Pads the right side of the provided value with the referenced character.
        /// </summary>
        /// <param name="oValue">Object representing the value to pad. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
        /// <param name="sPadCharacter">String representing the single character to use to pad the provided value.</param>
        /// <param name="iLength">1-based integer representing the required length of the return value.</param>
		/// <returns>String representing the padded value.</returns>
        ///###############################################################
		/// <LastUpdated>February 19, 2010</LastUpdated>
	    public static string RPad(object oValue, string sPadCharacter, int iLength) {
		    string sReturn = MakeString(oValue, "").Trim();
		    int iPadCharacterLength;
		    int i;

				//#### Ensure the passed sPadCharacter is a valid string and iLength is valid
			sPadCharacter = MakeString(sPadCharacter, " ");
			iPadCharacterLength = sPadCharacter.Length;
			iLength = (iLength < sReturn.Length ? sReturn.Length : iLength);

			    //#### Traverse the sReturn value, appending one sPadCharacter at a time until we reach the required iLength
		    for (i = sReturn.Length; i < iLength; i = i + iPadCharacterLength) {
			    sReturn += sPadCharacter;
		    }

				//#### If the passed sPadCharacter was longer then a single character and the sReturn value is longer then the iLength, .Substring it
			if (iPadCharacterLength > 1 && sReturn.Length > iLength) {
				sReturn = sReturn.Substring(0, iLength);
			}

			    //#### Return the above determined sReturn value to the caller
		    return sReturn;
	    }

        ///###############################################################
        /// <summary>
        /// Retrieves the character associated with the provided character code.
        /// </summary>
        /// <param name="iASCIICharacterCode">Integer representing the ASCII character code to decode.</param>
        /// <returns>String representing the character associated with the passed <paramref>iASCIICharacterCode</paramref>.</returns>
        ///###############################################################
		/// <LastUpdated>September 15, 2005</LastUpdated>
	    public static string Chr(int iASCIICharacterCode) {
	            //#### Cast the passed iASCIICharacterCode into a single value byte array
		    byte[] a_byteValue = { (byte)iASCIICharacterCode };

			    //#### Return the character of the passed iASCIICharacterCode
		    return Encoding.ASCII.GetString(a_byteValue);
	    }

        ///###############################################################
        /// <summary>
        /// Retrieves the character code associated with the provided character.
        /// </summary>
        /// <param name="sCharacter">String representing the character to encode.</param>
        /// <returns>Integer representing the ASCII character code associated with the first character of the passed <paramref>sCharacter</paramref>. NOTE: -1 is returned if the passed <paramref>sCharacter</paramref> is null or a null-string.</returns>
        ///###############################################################
		/// <LastUpdated>January 25, 2010</LastUpdated>
	    public static int Asc(string sCharacter) {
		    int iReturn = -1;

			    //#### If the passed sCharacter is not null (or a null-string)
		    if (! string.IsNullOrEmpty(sCharacter)) {
				    //#### Set the iReturn value to the character code of the passed sCharacter
			    iReturn = Encoding.ASCII.GetBytes(sCharacter.Substring(0, 1))[0];
		    }

			    //#### Return the above determined iReturn value to the caller
		    return iReturn;
	    }

        ///###############################################################
		/// <summary>
		/// Retrieves the array of data elements stored within the passed multi-value structure.
		/// </summary>
		/// <param name="sValues">String representing a multi-value structure as created by <c>MultiValueString</c> (i.e. - "|value1|value2|value3|value4|").</param>
		/// <returns>String array where each index represents a data element stored within the passed <paramref>sValues</paramref>.</returns>
        ///###############################################################
		/// <LastUpdated>January 10, 2006</LastUpdated>
		public static string[] MultiValueString(string sValues) {
			string[] a_sReturn = null;
			string sPrimaryDelimiter = Settings.PrimaryDelimiter;
			int iPrimaryDelimiterLen = sPrimaryDelimiter.Length;
			int iStart = 0;
			int iLength;

				//#### .Make(the passed sValues a)String, then determine its .Length
			sValues = MakeString(sValues, "");
			iLength = sValues.Length;

				//#### If the caller passed in a sValues to process
			if (iLength > 0) {
					//#### If the sValues .StartsWith a .PrimaryDelimiter, decrement iStart and iLength accordingly
				if (sValues.StartsWith(sPrimaryDelimiter)) {
					iStart = iPrimaryDelimiterLen;
					iLength = (iLength - iPrimaryDelimiterLen);
				}

					//#### If the sValues .EndsWith a .PrimaryDelimiter, decrement iLength accordingly
				if (sValues.EndsWith(sPrimaryDelimiter)) {
					iLength = (iLength - iPrimaryDelimiterLen);
				}

					//#### If the above calculated iLength is greater then 0 (indicating a string of more then just 2 .PrimaryDelimiters or less)
				if (iLength > 0) {
						//#### .Split the passed sValues (while pealing out its actual value), setting the result into the a_sReturn value
					a_sReturn = sValues.Substring(iStart, iLength).Split(sPrimaryDelimiter.ToCharArray());
				}
			}

				//#### Return the above determined a_sReturn value to the caller
			return a_sReturn;
		}

        ///###############################################################
		/// <summary>
		/// Formats the array of data elements into a multi-value structure.
		/// </summary>
		/// <param name="a_sValues">String array where each index represents a data element.</param>
		/// <returns>String representing a multi-value structure. (i.e. - "|value1|value2|value3|value4|")</returns>
        ///###############################################################
		/// <LastUpdated>December 2, 2005</LastUpdated>
		public static string MultiValueString(string[] a_sValues) {
			string sReturn = "";

				//#### If the caller passed in ga_sValues to process (while checking that a single entry array holds a value)
			if (a_sValues != null && a_sValues.Length > 0 && (a_sValues[0].Length > 0 || a_sValues.Length > 1)) {
					//#### .Join the passed ga_sValues with .PrimaryDelimiters (while pre/appending them as well), setting the result into the sReturn value
					//####     NOTE: .PrimaryDelimiter is prepended/appended onto the return value so that all of the ga_sValues are fully delimited (mainly so we can do .RenderSearchForm searches against them)
				sReturn = Settings.PrimaryDelimiter + String.Join(Settings.PrimaryDelimiter, a_sValues) + Settings.PrimaryDelimiter;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

        ///###############################################################
        /// <summary>
        /// Replaces all whitespace within the provided value with single spaces.
        /// </summary>
        /// <remarks>
        /// The term "normalize" in this instance conforms to XML's "collapse" style of text normalization. All occurrences of "tabs", "line feeds" and "carriage returns" are replaced with a "space". Subsequent to this, contiguous sequences of "spaces" are collapsed to a single "space", and initial and/or final "spaces" are deleted.
        /// <para/>For additional information, please see "XML Schema Part 1, Section 3.1.4: White Space Normalization during Validation" available at "http://www.w3.org/TR/2001/REC-xmlschema-1-20010502/#section-White-Space-Normalization-during-Validation".
        /// </remarks>
        /// <param name="sString">String representing the value to normalize.</param>
        /// <returns>String representing the normalized value.</returns>
        ///###############################################################
		/// <LastUpdated>July 28, 2005</LastUpdated>
	    public static string Normalize(string sString) {
		    string sReturn;
		    int iLen;

                //#### Transform all the whitepsace characters within the passed sString into spaces, placing the result into the sReturn value (while triming any leading/trailing spaces)
		    sReturn = sString.Replace("\r", " ");   //# carrage return
		    sReturn = sReturn.Replace("\n", " ");   //# line feed
		    sReturn = sReturn.Replace("\t", " ");   //# tab
		    sReturn = sReturn.Trim();

                //#### Traverse the return value while there are still multiaple conjoined spaces present within the sReturn value
		    do {
                    //#### Determine the current iLen of the sReturn value
			    iLen = sReturn.Length;

                    //#### Replace all double-spaces within the sReturn value with a single space
			    sReturn = sReturn.Replace("  ", " ");
		    } while (iLen > sReturn.Length);

			    //#### Return the above determined sReturn value to the caller
		    return sReturn;
	    }

        ///###############################################################
        /// <summary>
        /// Retrieves the MD5 digest of the provided value.
        /// </summary>
        /// <param name="sString">String representing the value to encode.</param>
        /// <returns>String representing the MD5 digest of the passed <paramref>sString</paramref>.</returns>
        ///###############################################################
		/// <LastUpdated>July 28, 2005</LastUpdated>
	    public static string MD5(string sString) {
		    UnicodeEncoding oUnicodeEnc = new UnicodeEncoding();
		    MD5CryptoServiceProvider oMD5 = new MD5CryptoServiceProvider();
		    byte[] a_byteString;
		    string sReturn;
		    int i;

                //#### Translate the passed sString into a Unicode byte array
		    a_byteString = oUnicodeEnc.GetBytes(sString);

                //#### Compute the MD5 digest, then reset the sReturn value to a null-string
		    a_byteString = oMD5.ComputeHash(a_byteString);
		    sReturn = "";

                //#### Traverse the above collected MD5 digest within a_byteString, converting and appending each byte into it's hex equivalent onto the sReturn value
		    for (i = 0; i < a_byteString.Length; i++) {
			    sReturn += a_byteString[i].ToString("x2");
		    }

                //#### UCase and return the above determined sReturn value
		    return sReturn.ToUpper();
	    }


        //############################################################################################################
        //# Public Is* and Make* Data Coercion Functions
        //############################################################################################################
        ///###############################################################
        /// <summary>
        /// Determines if the provided value is a non-null string value.
        /// </summary>
        /// <remarks>
        /// This method checks for three distinct types of "null":
        /// <para/>   1) If the passed value is a null pointer, i.e. - equal to "null" (or "Nothing" in VB.Net).
        /// <para/>   2) If the passed value is a null-string, i.e. - equal to "".
        /// <para/>   3) If the passed value is a null character, i.e. - equal to ASCII character code 0.
        /// <para/>If any one of these criteria are satisfied, the value is determined to be "null".
        /// </remarks>
        /// <param name="oValue">Object representing the value to test.</param>
		/// <returns>Boolean value signaling if the passed <paramref>oValue</paramref> is a null value.<para/>Returns true if the <paramref>oValue</paramref> is a null value, or false if it is not.</returns>
        ///###############################################################
		/// <LastUpdated>January 25, 2010</LastUpdated>
	    public static bool IsString(object oValue) {
			string sValue;
            bool bReturn = false;

				//#### If the passed oValue is not null
			if (oValue != null) {
					//#### .ToString the passed oValue
					//####     NOTE: .MakeString cannot be used within .IsString as .MakeString uses .IsString (making it a infinite recursive loop)
//!				sValue = oValue as string;
				sValue = oValue.ToString();

					//#### If the sValue is a null-string or contains only a null character, reset out bReturn value to false, otherwise to true
//!					//####     NOTE: We need to recheck for != null because "oValue as string" returns null if the cast fails
				bReturn = (! string.IsNullOrEmpty(sValue) && sValue != Chr(0) && sValue != DBNull.Value.ToString());
		    }

			    //#### Return the above determined bReturn value to the caller
		    return bReturn;
	    }

			///###############################################################
			/// <summary>
			/// Converts the provided value into a string.
			/// </summary>
			/// <param name="oValue">Object representing the value to convert.</param>
			/// <param name="sDefaultValue">String representing the value to return if the conversion results in a null value (as defined by <c>IsNull</c>).</param>
			/// <returns>String representation of the passed <paramref>oValue</paramref>. If the conversion results in a null value (as defined by <c>IsNull</c>), the passed <paramref>sDefaultValue</paramref> is returned instead.</returns>
			/// <seealso cref="Cn.Data.Tools.IsString"/>
			///###############################################################
			/// <LastUpdated>August 27, 2007</LastUpdated>
			public static string MakeString(object oValue, string sDefaultValue) {
				string sReturn;

					//#### Attempt to utilize the Convert.ToString function (catching any errors while returning sDefaultValue)
					//####     NOTE: Convert.ToString returns null if the oValue is null (rather then throwing an error like oValue.ToString)
				try {
					sReturn = Convert.ToString(oValue);
				} catch {
					sReturn = sDefaultValue;
				}

					//#### If the sReturn value .Is(not a)String, reset the sReturn value to the passed sDefaultValue
				if (! IsString(sReturn)) {
//!					sReturn = (IsString(sDefaultValue) ? sDefaultValue : "");
					sReturn = sDefaultValue;
				}

					//#### Return the above determined sReturn value to the caller
				return sReturn;
			}

        ///###############################################################
        /// <summary>
        /// Determines if the provided value is strictly numeric in nature.
        /// </summary>
        /// <remarks>
        /// This is essentually a stricter (as only some characters are considered numeric) while slopier (as order of those characters are not considerd) form of "IsNumber". This method ensures that the provided value only contains numeric characters (0-9, ",", "-" or "." only).
        /// <para/>Due to its implementation, this method can handle numbers beyond the minimum/maximum values imposed by the language and/or system architecture.
		/// <para/>The following examples of passed values and their related return values will help detail the use of this method:
        /// <para/>   "-9876.54" returns True
		/// <para/>   "-$9876.548" returns False
		/// <para/>   "£-9876.548" returns False
		/// <para/>   "-9,876.54" returns True
		/// <para/>   "(9876.54)" returns False
/// <para/>   "98-76.54" returns True
		/// <para/>   "Tell you what - 9 monkeys, 8 fish, 7 dogs and 6 cats. $54 in pet food." returns False
/// <para/>   "-.-9.8.-.7.6.5.4." returns True
		/// <para/>   "- monkeys fish dogs cats." returns False
        /// <para/>Numeric (Adj.); of or relating to or denoting numbers.
		/// </remarks>
        /// <param name="oValue">Object representing the value to test. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
		/// <returns>Boolean value signaling if the passed <paramref>oValue</paramref> is numeric.<para/>Returns true if the <paramref>oValue</paramref> can be successfully converted, or false if it cannot.</returns>
		/// <seealso cref="IsNumber"/>
        ///###############################################################
		/// <LastUpdated>February 15, 2006</LastUpdated>
	    public static bool IsNumeric(object oValue) {
		    byte[] a_byteCharCodes;
			int i;
		    byte byteAsciiCode;
	        bool bReturn = (oValue != null);

                //#### Define the character constants
			byte cComma = (byte)Asc(",");
			byte cMinus = (byte)Asc("-");
			byte cDot = (byte)Asc(".");
			byte c0 = (byte)Asc("0");
			byte c9 = (byte)Asc("9");

                //#### If the passed oValue is not null
		    if (bReturn) {
			        //#### .ToString the passed oValue while determining its a_byteCharCodes
		        a_byteCharCodes = Encoding.ASCII.GetBytes(oValue.ToString().ToCharArray());

					//#### Redefault the bReturn value based on the .Length of the a_byteCharCodes
				bReturn = (a_byteCharCodes.Length > 0);

					//#### If the passed oValue is not a null-string
				if (bReturn) {
						//#### Traverse the a_byteCharCodes from front to back
					for (i = 0; i < a_byteCharCodes.Length; i++) {
							//#### Determine the byteAsciiCode of the current character within a_byteCharCodes
						byteAsciiCode = a_byteCharCodes[i];

							//#### If the byteAsciiCode of the current character is not a 0-9, a minus, a comma or a dot, reset our bReturn value to false and fall out of the loop
						if (byteAsciiCode != cDot &&
							byteAsciiCode != cComma &&
							byteAsciiCode != cMinus &&
							(byteAsciiCode < c0 || byteAsciiCode > c9)
						) {
							bReturn = false;
							break;
						}
					}
				}
		    }

			    //#### Return the above determined bReturn value to the caller
		    return bReturn;
	    }

			///###############################################################
			/// <summary>
			/// Extracts any numeric data present within the provided value.
			/// </summary>
			/// <remarks>
			/// By most measures, this method is very "dumb". This method will extract all numeric data from the passed value indiscriminate of its position or the surrounding characters. The only intelligence this method utilizes is to determine if it will include a leading dash ("-") or a single decimal point (".") in the return value.
			/// <para/>The following examples of passed values and their related return values will help detail the use of this method:
			/// <para/>   "-9876.54" returns -9876.54
			/// <para/>   "-$9876.548" returns -9876.54
			/// <para/>   "£-9876.548" returns -9876.54
			/// <para/>   "-9,876.54" returns -9876.54
/// <para/>   "(9876.54)" returns 9876.54
			/// <para/>   "98-76.54" returns 9876.54
			/// <para/>   "Tell you what - 9 monkeys, 8 fish, 7 dogs and 6 cats. $54 in pet food." returns -9876.54
			/// <para/>   "-.-9.8.-.7.6.5.4." returns -.987654
			/// <para/>   "- monkeys fish dogs cats." returns the default value
			/// <para/>As you can see, a leading dash ("-") is perpended onto any following digits. The first decimal point (".") is also included positionally within any preceding/subsequent digits. Due to this behavior, this function is not intended to be used on random blocks of text. It is best used to force a "mostly" numeric value into a string, such as user supplied currency values (which is exactly how Renderer.Form uses it).
			/// </remarks>
			/// <param name="oValue">Object representing the value to query. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
			/// <param name="sDefaultValue">String representing the value to return if the conversion fails.</param>
			/// <returns>String representation of the numeric data within the passed <paramref>oValue</paramref>. If the conversion fails, the passed <paramref>sDefaultValue</paramref> is returned instead.</returns>
			///###############################################################
			/// <LastUpdated>August 27, 2007</LastUpdated>
			public static string MakeNumeric(object oValue, string sDefaultValue) {
				StringBuilder oReturn = new StringBuilder();
				byte[] a_byteCharCodes;
				string sValue;
				string sReturn = sDefaultValue;
				int i;
				bool bFoundDash = false;
				bool bFoundDot = false;

					//#### Define the required local constants
				byte cMinus = (byte)Asc("-");
				byte cDot = (byte)Asc(".");
				byte c0 = (byte)Asc("0");
				byte c9 = (byte)Asc("9");

					//#### If the passed oValue is not null
				if (oValue != null) {
						//#### .ToString the passed oValue, then determine its a_byteCharCodes
					sValue = oValue.ToString();
					a_byteCharCodes = Encoding.ASCII.GetBytes(sValue.ToCharArray());

						//#### If the a_byteCharCodes were successfully collected above
					if (a_byteCharCodes != null && a_byteCharCodes.Length > 0) {
							//#### Traverse the string-ified oValue's a_byteCharCodes from front to back, 1 character at a time
						for (i = 0; i < a_byteCharCodes.Length; i++) {
								//#### If the current a_byteCharCode is a number, .Append it onto oReturn value
							if (a_byteCharCodes[i] >= c0 && a_byteCharCodes[i] <= c9) {
								oReturn.Append(sValue.Substring(i, 1));
							}
								//#### Else if we've found a cMinus before finding any numbers, set the oReturn value to a dash and flip bFoundDash to true
							else if (a_byteCharCodes[i] == cMinus && ! bFoundDash && oReturn.Length == 0) {
								oReturn.Append("-");
								bFoundDash = true;
							}
								//#### Else if this is the first cDot, .Append it onto oReturn value and flip bFoundDot to true
							else if (! bFoundDot && a_byteCharCodes[i] == cDot) {
								oReturn.Append(".");
								bFoundDot = true;
							}
						}

							//#### Set the above constructed oReturn value into the real sReturn value
						sReturn = oReturn.ToString();

							//#### Determine the sReturn value and process accordingly
						switch (sReturn) {
								//#### The the above constructed sReturn value is holding non-numeric data, reset our sReturn value to the passed sDefaultValue
							case ".":
							case "-":
							case "-.": {
								sReturn = sDefaultValue;
								break;
							}
						}
					}
				}

					//#### Return the above determined sReturn value to the caller
				return sReturn;
			}

        ///###############################################################
        /// <summary>
        /// Determines if the provided value is numeric in nature.
        /// </summary>
		/// <param name="oValue">Object representing the value to test. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
		/// <returns>Boolean value signaling the ability to successfully convert the passed <paramref>oValue</paramref> into a double.<para/>Returns true if the <paramref>oValue</paramref> can be successfully converted, or false if it cannot.</returns>
		/// <seealso cref="IsNumeric"/>
        ///###############################################################
		/// <LastUpdated>August 27, 2007</LastUpdated>
	    public static bool IsNumber(object oValue) {
            double dJunk;

				//#### .Try(to)Parse the sValue as a Double, resetting our bReturn value to the result
			return Double.TryParse(Convert.ToString(oValue), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out dJunk);
	    }
//! public static double MakeNumeric(object oValue, double dDefaultValue)

				///###############################################################
				/// <summary>
				/// Converts the provided value into a double.
				/// </summary>
				/// <param name="oValue">Object representing the value to convert. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
				/// <param name="dDefaultValue">Double representing the value to return if the conversion fails.</param>
				/// <returns>Double representation of the passed <paramref>oValue</paramref>. If the conversion fails, the passed <paramref>dDefaultValue</paramref> is returned instead.</returns>
				/// <seealso cref="IsNumber"/>
				///###############################################################
				/// <LastUpdated>August 27, 2007</LastUpdated>
				public static double MakeDouble(object oValue, double dDefaultValue) {
					double dReturn;

						//#### .Try(to)Parse the sValue, resetting the dReturn to the dDefaultValue if it fails
					if (! Double.TryParse(Convert.ToString(oValue), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out dReturn)) {
						dReturn = dDefaultValue;
					}

						//#### Return the above determined dReturn value to the caller
					return dReturn;
				}

				///###############################################################
				/// <summary>
				/// Converts the provided value into a decimal.
				/// </summary>
				/// <param name="oValue">Object representing the value to convert. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
				/// <param name="dDefaultValue">Decimal representing the value to return if the conversion fails.</param>
				/// <returns>Decimal representation of the passed <paramref>oValue</paramref>. If the conversion fails, the passed <paramref>dDefaultValue</paramref> is returned instead.</returns>
				/// <seealso cref="IsNumber"/>
				///###############################################################
				/// <LastUpdated>August 27, 2007</LastUpdated>
				public static decimal MakeDecimal(object oValue, decimal dDefaultValue) {
					decimal dReturn;

						//#### .Try(to)Parse the sValue, resetting the dReturn to the dDefaultValue if it fails
					if (! Decimal.TryParse(Convert.ToString(oValue), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out dReturn)) {
						dReturn = dDefaultValue;
					}

						//#### Return the above determined dReturn value to the caller
					return dReturn;
				}

        ///###############################################################
        /// <summary>
        /// Determines if the provided value can be converted into an integer without a loss of precision.
        /// </summary>
        /// <param name="oValue">Object representing the value to test. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
		/// <returns>Boolean value signaling the ability to successfully convert the passed <paramref>oValue</paramref> into a integer.<para/>Returns true if the <paramref>oValue</paramref> can be successfully converted, or false if it cannot.</returns>
        ///###############################################################
		/// <LastUpdated>August 27, 2007</LastUpdated>
	    public static bool IsInteger(object oValue) {
			string sValue = Convert.ToString(oValue);
            bool bReturn = false;

				//#### If the passed oValue was not null (and string-able)
			if (sValue != null) {
					//#### .Trim the passed oValue
				sValue = sValue.Trim();

					//#### If the sValue is holding a value to test that .IsNumeric
				if (sValue.Length > 0 && IsNumber(sValue)) {
						//#### Determine if sValue is an integer (a whole number), resetting the bReturn value accordingly
						//####     NOTE: We pass in 0.1 as the sDefaultValue for .MakeNumber to ensure that the comparsion fails should the sValue not be made a number (as Math.Floor will always return a whole number)
					bReturn = (MakeNumeric(sValue, "0.1") == Math.Floor(MakeDouble(sValue, 0)).ToString());
				}
			}

			    //#### Return the above determined bReturn value to the caller
		    return bReturn;
	    }

			///###############################################################
			/// <summary>
			/// Converts the provided value into an integer.
			/// </summary>
			/// <param name="oValue">Object representing the value to convert. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
			/// <param name="iDefaultValue">Integer representing the value to return if the conversion fails.</param>
			/// <returns>Integer representation of the passed <paramref>oValue</paramref>. If the conversion fails, the passed <paramref>iDefaultValue</paramref> is returned instead.</returns>
			/// <seealso cref="IsNumber"/>
			///###############################################################
			/// <LastUpdated>August 17, 2007</LastUpdated>
			public static int MakeInteger(object oValue, int iDefaultValue) {
	//            string sValue;
	//            int iReturn = iDefaultValue;

	//                //#### If the passed oValue is not null
	//            if (oValue != null) {
	//                    //#### Determine the .ToString'd version of the passed oValue
	//                sValue = oValue.ToString();
	////!				sValue = oValue as string;

	////!					//#### If the sValue is not a null-string and .IsNumeric
	////				if (sValue.Length > 0 && IsNumeric(sValue)) {
	////						//#### .Parse sValue as a integer (catching any errors while returning iDefaultValue)
	////					try {
	////						iReturn = Int32.Parse(sValue);
	////					} catch {
	////						iReturn = iDefaultValue;
	////					}
	////				}
	//                    //#### .Try(to)Parse the sValue, resetting the iReturn to the iDefaultValue if it fails
	//                if (! int.TryParse(sValue, out iReturn)) {
	//                    iReturn = iDefaultValue;
	//                }
	//            }
				int iReturn;

					//#### .Try(to)Parse the sValue, resetting the iReturn to the iDefaultValue if it fails
				if (! Int32.TryParse(Convert.ToString(oValue), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out iReturn)) {
					iReturn = iDefaultValue;
				}

					//#### Return the above determined iReturn value to the caller
				return iReturn;
			}

        ///###############################################################
        /// <summary>
        /// Determines if the provided value can be converted into a boolean.
        /// </summary>
        /// <remarks>
		/// NOTE: Any changes made below must also be made in the JavaScript IsBoolean function.
		/// </remarks>
        /// <param name="oValue">Object representing the value to test. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
		/// <returns>Boolean value signaling the ability to successfully convert the passed <paramref>oValue</paramref> into a boolean.<para/>Returns true if the <paramref>oValue</paramref> can be successfully converted, or false if it cannot.</returns>
        ///###############################################################
		/// <LastUpdated>August 27, 2007</LastUpdated>
	    public static bool IsBoolean(object oValue) {
			string sValue = Convert.ToString(oValue);
            bool bReturn = false;

				//#### If the passed oValue was not null (and string-able)
			if (sValue != null) {
					//#### .Trim the passed oValue
				sValue = sValue.Trim();

					//#### If the sValue is holding a value to test
				if (sValue.Length > 0) {
						//#### If the sValue .IsNumeric, set our bReturn value to true (as numbers are boolean in nature)
					if (IsNumber(sValue)) {
						bReturn = true;
					}
						//#### Else we need to process the sValue as a string
					else {
							//#### Determine the .ToLower'd sValue and process accordingly
						switch (sValue.ToLower()) {
								//#### If the sValue is a reconized string value, reset the bReturn value to true
							case "t":
							case "true":
							case "yes":
							case "y":
							case "f":
							case "false":
							case "no":
							case "n": {
								bReturn = true;
								break;
							}
						}
					}
				}
			}

			    //#### Return the above determined bReturn value to the caller
		    return bReturn;
	    }

			///###############################################################
			/// <summary>
			/// Converts the provided value into a boolean.
			/// </summary>
			/// <remarks>
			/// Assumes the rules of boolean algebra, where 0 equals "false" and non-0 equals "true".
			/// </remarks>
			/// <param name="oValue">Object representing the value to convert.</param>
			/// <param name="bDefaultValue">Boolean value representing the value to return if the conversion fails.</param>
			/// <returns>Boolean value representation of the passed <paramref>oValue</paramref>. If the conversion fails, the passed <paramref>bDefaultValue</paramref> is returned instead.</returns>
			/// <seealso cref="Cn.Data.Tools.MakeBooleanInteger"/>
			/// <seealso cref="IsNumber"/>
			///###############################################################
			/// <LastUpdated>August 27, 2007</LastUpdated>
			public static bool MakeBoolean(object oValue, bool bDefaultValue) {
				string sValue = Convert.ToString(oValue);
				bool bReturn = bDefaultValue;

					//#### If the passed oValue was not null (and string-able)
				if (sValue != null) {
						//#### .Trim the passed oValue
					sValue = sValue.Trim();

						//#### If the sValue is holding a value to test
					if (sValue.Length > 0) {
							//#### If the sValue .IsNumeric, set our bReturn value based on the sValue (as numbers are boolean in nature)
						if (IsNumber(sValue)) {
							bReturn = (MakeDouble(sValue, 0) != 0);
						}
							//#### Else we need to process the sValue as a string
						else {
								//#### Determine the .ToLower'd sValue and process accordingly
							switch (sValue.ToLower()) {
									//#### If sValue is holding "true" or "yes", reset the bReturn value to true
								case "t":
								case "true":
								case "yes":
								case "y": {
									bReturn = true;
									break;
								}
									//#### If sValue is holding "false" or "no", reset the bReturn value to false
								case "f":
								case "false":
								case "no":
								case "n": {
									bReturn = false;
									break;
								}
							}
						}
					}
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}

			///###############################################################
			/// <summary>
			/// Converts the provided value into a boolean integer.
			/// </summary>
			/// <remarks>
			/// Assumes the rules of boolean algebra, where 0 equals "false" and non-0 equals "true".
			/// </remarks>
			/// <param name="oValue">Object representing the value to convert.</param>
			/// <param name="bDefaultValue">Boolean value representing the value to return if the conversion fails.</param>
			/// <returns>Integer representation of the passed <paramref>oValue</paramref>. If the conversion fails, the passed <paramref>bDefaultValue</paramref>'s integer representation is returned instead.<para/>Returns 1 if the result was true, and 0 if the result was false.</returns>
			/// <seealso cref="Cn.Data.Tools.MakeBoolean"/>
			///###############################################################
			/// <LastUpdated>July 28, 2005</LastUpdated>
			public static int MakeBooleanInteger(object oValue, bool bDefaultValue) {
				int iReturn;

					//#### If the passed off call to .MakeBoolean returns true, set our iReturn value to 1
				if (MakeBoolean(oValue, bDefaultValue)) {
					iReturn = 1;
				}
					//#### Else the .MakeBoolean call returned false, so set our iReturn value to 0
				else {
					iReturn = 0;
				}

					//#### Return the above determined iReturn value to the caller
				return iReturn;
			}

        ///###############################################################
        /// <summary>
        /// Determines if the provided value can be converted into a date/time.
        /// </summary>
        /// <param name="oValue">Object representing the value to test. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
		/// <returns>Boolean value signaling the ability to successfully convert the passed <paramref>oValue</paramref> into a date/time.<para/>Returns true if the <paramref>oValue</paramref> can be successfully converted, or false if it cannot.</returns>
        ///###############################################################
		/// <LastUpdated>August 27, 2007</LastUpdated>
	    public static bool IsDate(object oValue) {
			string sValue = Convert.ToString(oValue);
	        DateTime dParsedDate = DateTime.MinValue;

				//#### If the passed oValue was not null (and string-able)
			if (IsString(sValue)) {
					//#### Attempt to .Parse the sValue as a date (catching any errors)
				try {
					//dParsedDate = Convert.ToDateTime(sValue);
					dParsedDate = DateTime.Parse(sValue);
				} catch {
					dParsedDate = DateTime.MinValue;
				}
			}

				//#### If the above determined dParsedDate is not the .MinValue, return true, else return false to the caller
			return (dParsedDate != DateTime.MinValue);
	    }

			///###############################################################
			/// <summary>
			/// Converts the provided value into a date/time.
			/// </summary>
			/// <param name="oValue">Object representing the value to convert.</param>
			/// <param name="dDefaultValue">Date/time representing the value to return if the conversion fails.</param>
			/// <returns>Date/time representation of the passed <paramref>oValue</paramref>. If the conversion fails, the passed <paramref>dDefaultValue</paramref> is returned instead.</returns>
			///###############################################################
			/// <LastUpdated>August 27, 2007</LastUpdated>
			public static DateTime MakeDate(object oValue, DateTime dDefaultValue) {
				DateTime dReturn;

					//#### .Try(to)Parse the oValue, resetting the dReturn to the dDefaultValue if it fails
				if (! DateTime.TryParse(Convert.ToString(oValue), out dReturn)) {
					dReturn = dDefaultValue;
				}

					//#### Return the above determined dReturn value to the caller
				return dReturn;
			}

        ///###############################################################
        /// <summary>
        /// Determines if the provided value can be converted into a global unique identifier (GUID).
        /// </summary>
        /// <param name="oValue">Object representing the value to test. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
		/// <returns>Boolean value signaling the ability to successfully convert the passed <paramref>oValue</paramref> into a global unique identifier (GUID).<para/>Returns true if the <paramref>oValue</paramref> can be successfully converted, or false if it cannot.</returns>
        ///###############################################################
		/// <LastUpdated>September 16, 2005</LastUpdated>
        public static bool IsGUID(object oValue) {
			string sValue;
            bool bReturn = false;

				//#### If the passed oValue is not null
			if (oValue != null) {
					//#### Determine the .ToString'd version of the passed oValue
				sValue = oValue.ToString();

					//#### If the sValue is holding a value to test
				if (sValue.Length > 0) {
						//#### Reset our bReturn value based on the regular expression pattern match of the sValue
					bReturn = Regex.IsMatch(sValue, @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$");
				}
            }

			    //#### Return the above determined bReturn value to the caller
		    return bReturn;
        }

		///###############################################################
		/// <summary>
		/// Converts the provided value into an enumeration.
		/// </summary>
		/// <param name="oValue">Object representing the value to convert.</param>
		/// <param name="eDefaultValue">Enumeration representing the value to return if the conversion fails.</param>
		/// <returns>Enumeration representation of the passed <paramref>oValue</paramref>. If the conversion fails, the passed <paramref>dDefaultValue</paramref> is returned instead.</returns>
		///###############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
    	public static T MakeEnum<T>(object oValue, T eDefaultValue) {
    		string sValue;
    		T eReturn = eDefaultValue;

    		//#### .MakeString, .Trim and .ToLower the passed oValue into sDecodeValue
    		sValue = MakeString(oValue, "").Trim().ToLower();

    		//#### If the passed oValue .IsDefined (without having to do any of the gyrations below)
    		if (Enum.IsDefined(typeof(T), oValue)) {
    			//#### Reset our eReturn value 
    			eReturn = (T)Enum.Parse(typeof(T), sValue);
    		}
    			//#### Else an indepth check is required
    			//####     NOTE: This check is really only done if the passed oValue is invalid or has been ToLower'd, so while it's not very efficient it's only called when necessary
    		else {
    			string[] a_sEnumNames = Enum.GetNames(typeof(T));
    			int i = 0;

    			//#### If  we have a sDefaultValue to check and some a_sEnumNames were collected above
    			//####     NOTE: Since Enum.IsDefined does not do case-insensitive checks, we have to do it ourselves
    			if (sValue.Length > 0 && a_sEnumNames != null && a_sEnumNames.Length > 0) {
    				//#### Traverse the a_sEnumValues
    				for (i = 0; i < a_sEnumNames.Length; i++) {
    					//#### If the current .ToLower'd a_sEnumName equals sDecodeValue
    					if (a_sEnumNames[i].ToLower() == sValue) {
    						//#### Reset our eReturn value to the .Parse'd Enum, set i so we know the value was found and break from the for loop
    						eReturn = (T)Enum.Parse(typeof(T), a_sEnumNames[i]);
    						i = -1;
    						break;
    					}
    				}
    			}

    			//#### If the oValue has not yet been found
    			if (i != -1) {
    				//#### Borrow the use of i to store the forced integer value of the passed oValue (defaulting the value to the passed eDefaultValue if it's not an int-able value)
    				i = MakeInteger(oValue, Convert.ToInt32(eDefaultValue));

    				//#### If the integer in the above borrowed i .IsDefined, reset our eReturn value to it's casted enum equivlent
    				//####     NOTE: Enum.Parse only deals with strings, and since we defaulted the .MakeInteger'd value to eDefaultValue, we have to go through the gyrations to re-string a int'd string
    				if (Enum.IsDefined(typeof(T), i)) {
    					eReturn = (T)Enum.Parse(typeof(T), i.ToString());
    				}
    			}
    		}

    		//#### Return the above determined eReturn value to the caller
    		return eReturn;

/*
	if (! typeof(T).IsEnum) {
		throw new NotSupportedException("T must be an Enum");
	}

	return (T)Enum.Parse(typeof(T), source, true);



	static object MakeEnum(Type tEnumType, string oValue, object enumDefaultValue) {

			//#### If the pass oValue is defined within the tEnumType
		if (Enum.IsDefined(tEnumType, oValue)) {
			oReturn = Enum.Parse(tEnumType, oValue, true);
		}


		foreach ( System.Reflection.FieldInfo fi in t.GetFields() ) {
			if ( fi.Name == oValue ) {
				return fi.GetValue( null );
			}
		}
          
              // We use null because
                                         // enumeration values
                                         // are static

      throw new Exception( string.Format("Can't convert {0} to {1}", oValue,
                                          t.ToString()) );


		public static T MakeEnum<T>(int iDecodeValue, T eDefaultValue) {
			T eReturn = eDefaultValue;

				//#### If the passed iDecodeValue .IsDefined
			if (Enum.IsDefined(typeof(T), iDecodeValue)) {
					//#### Reset our eReturn value 
				eReturn = (T)Enum.Parse(typeof(T), iDecodeValue.ToString());
			}

				//#### Return the above determined eReturn value to the caller
			return eReturn;
		}
*/
    	}

        ///###############################################################
		/// <summary>
		/// Determines if the passed numeric value is within the passed range.
		/// </summary>
        /// <remarks>
        /// Due to its implementation, this method can handle numeric values beyond the minimum/maximum values imposed by the language and/or system architecture.
        /// <para/>"return (sNumber &gt;= sRangeMin &amp;&amp; sNumber &lt;= sRangeMax)" would work in 99.9% of the checks we'll do with this function, but in the case of huge/tiny numbers (such as NUMERIC(x,y)'s in Oracle), this wouldn't cut it as the numbers would be too large/small to be represented in any available numeric datatypes.
        /// <para/>NOTE: Assumes that the passed objects are all numeric in nature.
        /// <para/>NOTE: This should match the logic present within rfDataValidation.js.*'s rfIsNumberInRange.
        /// <para/>Numeric (Adj.); of or relating to or denoting numbers.
        /// </remarks>
        /// <param name="oValue">Object representing the numeric value to compare. <para/>NOTE: This value is converted into its string equivalent before the comparison.</param>
        /// <param name="oRangeMin">Object representing the minimum numeric value to use as a comparison. <para/>NOTE: This value is converted into its string equivalent before the comparison.</param>
        /// <param name="oRangeMax">Object representing the maximum numeric value to use as a comparison. <para/>NOTE: This value is converted into its string equivalent before the comparison.</param>
		/// <returns>Boolean value signaling the presence of the <paramref>oValue</paramref> within the passed range.<para/>Returns true if the <paramref>oValue</paramref> is within the passed range, or false if it is not.</returns>
        ///###############################################################
		/// <LastUpdated>January 8, 2010</LastUpdated>
        public static bool IsNumericInRange(object oValue, object oRangeMin, object oRangeMax) {
            bool bReturn = false;

				//#### If the passed values are all .IsNumeric
			if (IsNumeric(oValue) && IsNumeric(oRangeMin) && IsNumeric(oRangeMax)) {
					//#### If the passed oValue is greater then or equal to the passed oRangeMin
				if (LargeNumericComparison(oValue, oRangeMin) >= 0) {
						//#### If the passed oValue is less then or equal to the passed oRangeMax
					if (LargeNumericComparison(oValue, oRangeMax) <= 0) {
							//#### Since the passed oValue is within the passed oRangeMin/oRangeMax, flip our bReturn value to true
						bReturn = true;
					}
				}
			}

		        //#### Return the above determined bReturn value to the caller
	        return bReturn;
        }

			///###############################################################
			/// <summary>
			/// Determines the position of the provided numeric value in relation to the provided range.
			/// </summary>
			/// <remarks>
			/// Due to its implementation, this method can handle numeric values beyond the minimum/maximum values imposed by the language and/or system architecture.
			/// <para/>NOTE: Assumes that the passed objects are all numeric in nature.
			/// <para/>NOTE: This should match the logic present within rfDataValidation.js.*'s rfLargeNumberComparison.
			/// <para/>Numeric (Adj.); of or relating to or denoting numbers.
			/// </remarks>
			/// <param name="oValue">Object representing the numeric value to compare. <para/>NOTE: This value is converted into its string equivalent before the comparison.</param>
			/// <param name="oRange">Object representing the numeric value to use as a comparison. <para/>NOTE: This value is converted into its string equivalent before the comparison.</param>
			/// <returns>Integer signaling the position of the passed <paramref>oValue</paramref> in relation to the passed <paramref>oRange</paramref>.<para/>Returns -1 if <paramref>oValue</paramref> is less then <paramref>oRange</paramref>, 1 if <paramref>oValue</paramref> is greater then <paramref>oRange</paramref>, and 0 if the passed values are equal, or if one of the passed values was non-numeric.</returns>
			///###############################################################
			/// <LastUpdated>January 8, 2010</LastUpdated>
			public static int LargeNumericComparison(object oValue, object oRange) {
				string sValue;
				string sRange;
				int iValueNumericPrecision = NumericPrecision(oValue);
				int iRangeNumericPrecision = NumericPrecision(oRange);
				int iReturn;
				bool bValueIsPositive;
				bool bRangeIsPositive;

					//#### If the passed sValue or sRange were non-numeric, set our iReturn value to 0 (which is numericially equal)
					//####     NOTE: An error is not raised here because this function only validates the range, not the data types of the passed objects
				if (iValueNumericPrecision == -1 || iRangeNumericPrecision == -1) {
					iReturn = 0;
				}
					//#### Else we have numbers to process
				else {
						//#### Determine if the .ToString'd versions of oValue and oRange, then if the bValueIsPositive and the bRangeIsPositive
						//####     NOTE: We know that both oValue and oRange are not null as we were able to successfully collect thier .NumericPrecisions above
					sValue = oValue.ToString();
					sRange = oRange.ToString();
					bValueIsPositive = (sValue.IndexOf("-") != 0);
					bRangeIsPositive = (sRange.IndexOf("-") != 0);

						//#### If the signs of the passed sValue and sRange do not match
					if (bValueIsPositive != bRangeIsPositive) {
							//#### If the bValueIsPositive, then the sRange is negetive, so set our iReturn value to 1 (as sValue is greater then the sRange)
						if (bValueIsPositive) {
							iReturn = 1;
						}
							//#### Else the sValue is negetive and the bRangeIsPositive, so set our iReturn value to -1 (as sValue is less then the sRange)
						else {
							iReturn = -1;
						}
					}
						//#### Else the signs of the passed sValue and sRange match
					else {
							//#### If the above-determined NumericPrecision's are specifying numbers of less then 1 billion
						if (iRangeNumericPrecision < 10 && iValueNumericPrecision < 10) {
								//#### Define and init the additionally required vars
								//####     NOTE: We know that both sValue and sRange are numeric as non-numeric value are caught by NumericPrecision above
							double dNumber = MakeDouble(sValue, 0);
							double dRange = MakeDouble(sRange, 0);

								//#### If the sValue and sRange are equal, set our iReturn value to 0
							if (dNumber == dRange) {
								iReturn = 0;
							}
								//#### Else if the sValue is greater then the sRange, set our iReturn value to 1
							else if (dNumber > dRange) {
								iReturn = 1;
							}
								//#### Else the dNumber is less then the sRange, so set our iReturn value to -1
							else {
								iReturn = -1;
							}
						}
							//#### Else we're dealing with number ranges over 1 billion, so let's get creative...
						else {
								//#### If the iNumber('s)NumericPrecision is less then the iRange('s)NumericPrecision
							if (iValueNumericPrecision < iRangeNumericPrecision) {
									//#### If the bValueIsPositive (and thanks to the check above the bRangeIs(also)Positive), return -1 (as the sNumber is a smaller positive number then the sRange, making it less)
								if (bValueIsPositive) {
									iReturn = -1;
								}
									//#### Else the bValueIs(not)Positive (and thanks to the check above the bRangeIs(also not)Positive), so return 1 (as the sNumber is a smaller negetive number then the sRange, making it greater)
								else {
									iReturn = 1;
								}
							}
								//#### Else if the iNumber('s)NumericPrecision is more then the iRange('s)NumericPrecision
							else if (iValueNumericPrecision > iRangeNumericPrecision) {
									//#### If the bValueIsPositive (and thanks to the check above the bRangeIs(also)Positive), return 1 (as the sNumber is a bigger positive number then the sRange, making it greater)
								if (bValueIsPositive) {
									iReturn = 1;
								}
									//#### Else the bValueIs(not)Positive (and thanks to the check above the bRangeIs(also not)Positive), so return -1 (as the sNumber is a bigger negetive number then the sRange, making it less)
								else {
									iReturn = -1;
								}
							}
								//#### Else the iNumber('s)NumericPrecision is equal to the iRange('s)NumericPrecision, so additional checking is required
							else {
									//#### Define and set the additionally required decimal point position variables
								int iNumberDecimalPoint = sValue.IndexOf(".");
								int iRangeDecimalPoint = sRange.IndexOf(".");

									//#### If either/both of the decimal points were not found above, reset iNumberDecimalPoint/iRangeDecimalPoint to their respective .Lengths (which logicially places the iRangeDecimalPoint at the end of the sRange, which is where it is located)
									//####     NOTE: Since this function only checks that the passed sValue is within the passed range, the values "whole" -vs- "floating point" number distinction is ignored as for our purposes, it is unimportant.
								if (iNumberDecimalPoint == -1) {
									iNumberDecimalPoint = sValue.Length;
								}
								if (iRangeDecimalPoint == -1) {
									iRangeDecimalPoint = sRange.Length;
								}

									//#### If the sValue's decimal point is to the left of sRange's (making sValue less then sRange), set our iReturn value to -1
								if (iNumberDecimalPoint < iRangeDecimalPoint) {
									iReturn = -1;
								}
									//#### Else if the sValue's decimal point is to the right of sRange's (making sValue greater then sRange), set our iReturn value to 1
								else if (iNumberDecimalPoint > iRangeDecimalPoint) {
									iReturn = 1;
								}
									//#### Else the sValue's decimal point is in the same position as the sRange's decimal point
								else {
										//#### Define and init the additionally required vars
									int iCurrentNumberNumber;
									int iCurrentRangeNumber;
									int i;

										//#### Default our iReturn value to 0 (as only > and < are checked in the loop below, so if the loop finishes without changing the iReturn value then the sValue and sRange are equal)
									iReturn = 0;

										//#### Setup the value for i based on if the bValueIsPositive (setting it to 0 if it is, or 1 if it isn't)
										//####     NOTE: This is done to skip over the leading "-" sign in negetive numbers (yea it's ugly, but it works!)
										//####     NOTE: Since at this point we know that signs of sValue and sRange match, we only need to check bValueIsPositive's value
									i = (bValueIsPositive) ? (0) : (1);

										//#### Traverse the sValue/sRange strings from front to back (based on the above determined starting position)
										//####     NOTE: Since everything is is the same position and the same precision, we know that sValue's .lenght is equal to sRange's
									for (/* i = i */; i < sValue.Length; i++) {
											//#### As long as we're not looking at the decimal point
										if (i != iNumberDecimalPoint) {
												//#### Determine the iCurrentNumberNumber and iCurrentRangeNumber for this loop
											iCurrentNumberNumber = MakeInteger(sValue.Substring(i, 1), -1);
											iCurrentRangeNumber = MakeInteger(sRange.Substring(i, 1), -1);

												//#### If the iCurrentNumberNumber is less then the iCurrentRangeNumber
											if (iCurrentNumberNumber < iCurrentRangeNumber) {
													//#### sValue is less then sRange, so set our iReturn value to -1 and fall from the loop
												iReturn = -1;
												break;
											}
												//#### Else if the iCurrentNumberNumber is greater then the iCurrentRangeNumber
											if (iCurrentNumberNumber > iCurrentRangeNumber) {
													//#### sValue is greater then sRange, so set our iReturn value to 1 and fall from the loop
												iReturn = 1;
												break;
											}
										}
									}
								}
							}
						}
					}
				}

					//#### Return the above determined iReturn value to the caller
				return iReturn;
			}

		///###############################################################
		/// <summary>
		/// Determines the count of significant digits within the provided number. 
		/// </summary>
		/// <remarks>
		/// The count of significant digits is returned, meaning leading zeros are ignored while trailing zeros are counted.
		/// <para/>Due to its implementation, this method can handle numbers beyond the minimum/maximum values imposed by the language and/or system architecture.
		/// <para/>NOTE: Assumes that the passed objects are all numeric in nature.
		/// <para/>NOTE: This should match the logic present within rfDataValidation.js.*'s rfNumericPrecision.
		/// </remarks>
		/// <param name="oValue">Object representing the value to query. <para/>NOTE: This value is converted into its string equivalent before the test.</param>
		/// <returns>1-based integer representing the count of significant digits within the passed <paramref>oValue</paramref>.</returns>
		///###############################################################
		/// <LastUpdated>September 16, 2005</LastUpdated>
		public static int NumericPrecision(object oValue) {
			byte[] a_byteCharCodes;
			int iReturn = 0;
			int i;
			bool bStartCounting = false;

				//#### Determine the ASCII codes for "0" and "9"
			byte c0 = (byte)Asc("0");
			byte c9 = (byte)Asc("9");

				//#### If the passed oValue is not null and .IsNumeric
			if (oValue != null && IsNumber(oValue)) {
					//#### Determine the a_byteCharCodes of the passed .ToString'd oValue
					//####     NOTE: We know that oValue is not null if it .IsNumeric, hence the use of .ToString is ok
				a_byteCharCodes = Encoding.ASCII.GetBytes(oValue.ToString().ToCharArray());

					//#### Traverse a_byteCharCodes from front to back 1 character code at a time
				for (i = 0; i < a_byteCharCodes.Length; i++) {
						//#### If the a_byteCharCode is a number
					if (a_byteCharCodes[i] >= c0 && a_byteCharCodes[i] <= c9) {
							//#### If we are supposed to bStartCounting, or if the current a_byteCharCode is not a 0
							//####     NOTE: This is done so we ignore leading 0's (while trailing 0's are still counted)
						if (bStartCounting || a_byteCharCodes[i] != c0) {
								//#### Inc iReturn and ensure bStartCounting is true
							iReturn++;
							bStartCounting = true;
						}
					}
				}
			}
				//#### Else the passed sValue is null or not a number, so reset the iReturn value to -1 (which indicates an error occured)
			else {
				iReturn = -1;
			}

				//#### Return the above determined iReturn value to the caller
			return iReturn;
		}

    	///############################################################
    	/// <summary>
    	/// Converts the provided set of results into a MultiArray.
    	/// </summary>
    	/// <param name="oResults">"Set of results" containing the data to convert.</param>
    	/// <returns>MultiArray containing the passed <paramref>oResults</paramref>.</returns>
    	///############################################################
    	/// <LastUpdated>December 5, 2005</LastUpdated>
    	public static MultiArray ToMultiArray(DataTable oResults) {
    		MultiArray oReturn = null;
    		Hashtable h_sRow = new Hashtable();
    		string[] a_sColumnNames;
    		int iColumnCount;
    		int i;
    		int j;

    		//#### If the passed oResults has .Columns to traverse
    		if (oResults != null && oResults.Columns.Count > 0) {
    			//#### Determine the iColumnCount, then dimension the a_sColumnNames accordingly
    			iColumnCount = oResults.Columns.Count;
    			a_sColumnNames = new string[iColumnCount];

    			//#### Collect the a_sColumnNames from the passed oResults
    			for (i = 0; i < iColumnCount; i++) {
    				a_sColumnNames[i] = oResults.Columns[i].ColumnName;
    			}

    			//#### Load the above collected a_sColumnNames into the oReturn value
    			oReturn = new MultiArray(a_sColumnNames);

    			//#### Traverse the .Rows within the passed oResults (if any)
    			for (i = 0; i < oResults.Rows.Count; i++) {
    				//#### Traverse the a_sColumnNames, loading each value into the h_sRow as we go
    				for (j = 0; j < iColumnCount; j++) {
    					h_sRow[a_sColumnNames[j]] = MakeString(oResults.Rows[i][a_sColumnNames[j]], "");
    				}

    				//#### Insert the above filled h_sRow into the oReturn value
    				oReturn.InsertRow(h_sRow);
    			}
    		}

    		//#### Return the above determined oReturn value to the caller
    		return oReturn;
    	}

    	///############################################################
    	/// <summary>
    	/// Extracts a column from the provided set of results.
    	/// </summary>
    	/// <param name="oResults">"Set of results" containing the data to extract.</param>
    	/// <param name="sColumnName">String representing the column name to locate.</param>
    	/// <returns>String array where each element represents a row's <paramref>sColumnName</paramref> value.</returns>
    	///############################################################
    	/// <LastUpdated>December 5, 2005</LastUpdated>
    	public static string[] ToColumnArray(DataTable oResults, string sColumnName) {
    		string[] a_sReturn = null;

    		//#### If the passed oResults has .Rows to traverse
    		if (oResults != null && oResults.Rows.Count > 0) {
    			//#### If the passed oResults has the passed sColumnName
    			if (oResults.Columns.Contains(sColumnName)) {
    				//#### Pass the call off to our sibling implementation, setting our a_sReturn value to its
    				a_sReturn = ToColumnArray(oResults, oResults.Columns[sColumnName].Ordinal);
    			}
    		}

    		//#### Return the above determined oReturn value to the caller
    		return a_sReturn;
    	}

    	///############################################################
    	/// <summary>
    	/// Extracts a column from the provided set of results.
    	/// </summary>
    	/// <param name="oResults">"Set of results" containing the data to extract.</param>
    	/// <param name="iColumnIndex">0-based integer representing the desired column index.</param>
    	/// <returns>String array where each element represents a row's <paramref>iColumnIndex</paramref> value.</returns>
    	///############################################################
    	/// <LastUpdated>December 5, 2005</LastUpdated>
    	public static string[] ToColumnArray(DataTable oResults, int iColumnIndex) {
    		string[] a_sReturn = null;
    		int i;

    		//#### If the passed oResults has .Rows to traverse
    		if (oResults != null && oResults.Rows.Count > 0) {
    			//#### If the passed iColumnIndex is within the valid range
    			if (iColumnIndex >= 0 && iColumnIndex < oResults.Columns.Count) {
    				//#### Dimension the a_sReturn value
    				a_sReturn = new string[oResults.Rows.Count];

    				//#### Traverse the oResults, copying each .Row's iColumnIndex into the a_sReturn value
    				for (i = 0; i < oResults.Rows.Count; i++) {
    					a_sReturn[i] = oResults.Rows[i][iColumnIndex].ToString();
    				}
    			}
    		}

    		//#### Return the above determined oReturn value to the caller
    		return a_sReturn;
    	}
	} //# class Tools

} //# namespace Cn.Data
