/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/


using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Web.Inputs {


    ///########################################################################################################################
    /// <summary>
	/// 
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>July 29, 2005</LastFullCodeReview>
	public class Tools {
			//#### Declare the required private constants
		//private const string g_cClassName = "Cn.Web.Inputs.Tools";


		//##########################################################################################
		//# Public Static Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Formats the provided date.
		/// </summary>
		/// <remarks>
		/// If the passed <paramref>sInputSpecificFormat</paramref> is a null-string, its value is modified by reference to indicate the default date format as defined within <c>Renderer</c>'s loaded <c>Settings</c> (per the passed <paramref>eInputType</paramref>) that the <paramref>sDateToFormat</paramref> was formatted with.
		/// </remarks>
		/// <param name="sDateToFormat">String representing the date to format.</param>
		/// <param name="sInputSpecificFormat">Reference to a string representing the required date format.</param>
		/// <param name="eInputType">Enumeration representing the type of input to render.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the formatted <paramref>sDateToFormat</paramref>.</returns>
		///############################################################
		/// <LastUpdated>November 13, 2009</LastUpdated>
		public static string FormatDateTime(string sDateToFormat, ref string sInputSpecificFormat, enumInputTypes eInputType, Settings.Current oSettings) {
 			string sEndUserMessagesLanguageCode = oSettings.EndUserMessagesLanguageCode;
			string sReturn;
 			Dates.enumWeekOfYearCalculations eWeekOfYearCalculation;

				//#### If the developer didn't pass in a sInputSpecificFormat
			if (sInputSpecificFormat.Length == 0) {
					//#### Determine the passed eInputType, resetting the sInputSpecificFormat to the related default value from .Settings
				switch (eInputType) {
					case enumInputTypes.cnDate: {
						sInputSpecificFormat = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnLocalization_Date_DateFormat, sEndUserMessagesLanguageCode);
						break;
					}
					case enumInputTypes.cnDateTime: {
						sInputSpecificFormat = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnLocalization_Date_DateTimeFormat, sEndUserMessagesLanguageCode);
						break;
					}
					case enumInputTypes.cnTime: {
						sInputSpecificFormat = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnLocalization_Date_TimeFormat, sEndUserMessagesLanguageCode);
						break;
					}
				}
			}

				//#### Collect the eWeekOfYearCalculation
			eWeekOfYearCalculation = Data.Tools.MakeEnum(Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnLocalization_Date_WeekOfYearCalculationEnum, sEndUserMessagesLanguageCode), Dates.enumWeekOfYearCalculations.cnDefault);

				//#### If a sInputSpecificFormat was passed in (or successfully determined above)
			if (sInputSpecificFormat.Length > 0) {
					//#### Set the return value to the formatted sDateToFormat on the above determined sInputSpecificFormat
					//####     NOTE: The sInputSpecificFormat is defaulted to the localized default date/time format by the callers, it's not done here because we don't have access to the related oSettings object
				sReturn = Dates.Tools.FormatDateTime(sDateToFormat, sInputSpecificFormat, Settings.Internationalization, eWeekOfYearCalculation);
			}
				//#### Else a sInputSpecificFormat was not successfully determined above, so simply return the passed sDateToFormat as is
			else {
				sReturn = sDateToFormat;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Formats the provided value for proper rendering within an HTML form.
		/// </summary>
		/// <remarks>
		/// This function replaces apostrophes and quotes (aka - single and double quotes) with their HTML equivalents ("&#039;" and "&quot;" in this case) so as to ensure proper delimiting within a HTML form input.
		/// </remarks>
		/// <param name="sValue">String representing the value to format.</param>
		/// <returns>String representing the formatted value.</returns>
		///############################################################
		/// <LastUpdated>December 13, 2005</LastUpdated>
		public static string FormatForForm(string sValue) {
			sValue = sValue.Replace("\"", "&quot;");
			return sValue.Replace("'", "&#039;");
		}

		///############################################################
		/// <summary>
		/// Escapes the character in the provided value.
		/// </summary>
		/// <remarks>
		/// This function escapes the referenced <paramref>sCharacter</paramref> by prepending a backslash ("\") before each instance of it within the sCharacter <paramref>sValue</paramref>.
		/// </remarks>
		/// <param name="sValue">String representing the value to process.</param>
		/// <param name="sCharacter">String representing the single character to escape.</param>
		/// <returns>String representing the escaped <paramref>sValue</paramref>.</returns>
		///############################################################
		/// <LastUpdated>December 13, 2005</LastUpdated>
		public static string EscapeCharacters(string sValue, string sCharacter) {
			return sValue.Replace(sCharacter, "\\" + sCharacter);
		}


	} //# public class Tools


} //# namespace Cn.Web.Inputs