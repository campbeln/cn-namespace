/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Data;										//# Required to access the Picklists/MetaData class


namespace Cn.Configuration {

	///########################################################################################################################
	/// <summary>
	/// Internationalization functionality.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>May 10, 2007</LastFullCodeReview>
	public class Internationalization {
			//#### Declare the required public eNums
		#region eNums
			/// <summary>Stored internationalization picklists.<para/>These are picklists used internally by the Cn namespace.</summary>
		public enum enumInternationalizationPicklists : int {
				//#### Cn internationalization settings
			cnLanguageCodes = 100,
			cnBoolean = 101,

			cnDate_MonthNames = 200,
			cnDate_AbbreviatedMonthNames = 201,
			cnDate_WeekDayNames = 202,
			cnDate_AbbreviatedWeekDayNames = 203,
			cnDate_MonthDaySuffix = 204,
			cnDate_Meridiem = 205,
			cnDate_CalendarWeekDayNames = 206,

			cnRendererSearchForm_Boolean = 300,
			cnRendererSearchForm_Numeric = 301,
			cnRendererSearchForm_DateTime = 302,
			cnRendererSearchForm_Char = 303,
			cnRendererSearchForm_LongChar = 304,
			cnRendererSearchForm_SingleValuePicklist = 305,
			cnRendererSearchForm_MultiValuePicklist = 306,
			cnRendererSearchForm_SingleValueSearchInMultiValuePicklist = 307,
			cnRendererSearchForm_MultiValueSearchInSingleValuePicklist = 308,
			cnRendererSearchForm_IsNullStringIsNull = 309,

			cnDateMath_Frequency = 400,
			cnDateMath_DefinitionType = 401,
			cnDateMath_Calculations = 402
		}
			/// <summary>Stored internationalization values.<para/>These are constants and error messages used internally by the Cn namespace.</summary>
		public enum enumInternationalizationValues : int {
				//#### Cn configuration settings (constants)
			cnDeveloperMessages_General_MissingDefaultData = 1,

				//#### Cn internationalization settings
			cnGeneralSettings_DefaultLanguageCode = 100,

			cnLocalization_CurrencySymbol = 101,
			cnLocalization_CurrencyMask_Positive = 102,
			cnLocalization_CurrencyMask_Negetive = 103,
			cnLocalization_CurrencyMask_Zero = 104,
			cnLocalization_Date_DateTimeFormat = 150,
			cnLocalization_Date_TimeFormat = 151,
			cnLocalization_Date_DateFormat = 152,
			cnLocalization_Date_WeekOfYearCalculationEnum = 153,

			cnPicklistManagement_UniqueIDRequired = 300,
			cnPicklistManagement_UniqueNameRequired = 301,
			cnPicklistManagement_ViewPicklist = 302,
			cnPicklistManagement_PicklistWasUpdated = 303,
			cnPicklistManagement_UpdatePicklist = 304,
			cnPicklistManagement_Type = 305,
			cnPicklistManagement_Description = 306,
			cnPicklistManagement_Name = 307,
			cnPicklistManagement_PicklistNameIsCurrentlyEmpty = 308,
			cnPicklistManagement_DisplayOrder = 309,
			cnPicklistManagement_PicklistID = 310,
			cnPicklistManagement_Delete = 311,
			cnPicklistManagement_DisplayedValue = 312,
			cnPicklistManagement_StoredValue = 313,
			cnPicklistManagement_PicklistDescription = 314,
			cnPicklistManagement_PicklistName = 315,
			cnPicklistManagement_New = 316,
			cnPicklistManagement_ViewInternationalization = 317,
			cnPicklistManagement_TableColumnName = 318,
			cnPicklistManagement_TableColumnNameFormat = 319,
			cnPicklistManagement_TableColumnNamePicklist = 320,
			cnPicklistManagement_IsActive = 321,

//            cnDateMathManagement_ = 400,

			cnEndUserMessages_Alert = 1000,
			cnEndUserMessages_NoError = 1001,
			cnEndUserMessages_IncorrectLength = 1002,
			cnEndUserMessages_ValueIsRequired = 1003,
			cnEndUserMessages_Custom = 1004,
			cnEndUserMessages_UnknownOrUnsupportedType = 1005,
			cnEndUserMessages_IncorrectDataType_Boolean = 1006,
			cnEndUserMessages_IncorrectDataType_Integer = 1007,
			cnEndUserMessages_IncorrectDataType_Float = 1008,
			cnEndUserMessages_IncorrectDataType_Currency = 1009,
			cnEndUserMessages_IncorrectDataType_DateTime = 1010,
			cnEndUserMessages_IncorrectDataType_GUID = 1011,
			cnEndUserMessages_IncorrectDataType_Other = 1012,
			cnEndUserMessages_IncorrectDataType_NotWithinPicklist = 1013,
			cnEndUserMessages_UnknownErrorCode = 1014,
			cnEndUserMessages_MissingInput = 1015,
			cnEndUserMessages_MultipleValuesSubmittedForSingleValue = 1016,
			cnEndUserMessages_ComboBox_OrSelect = 1017,
			cnEndUserMessages_DateTime_PreviousYear = 1018,
			cnEndUserMessages_DateTime_PreviousMonth = 1019,
			cnEndUserMessages_DateTime_Today = 1020,
			cnEndUserMessages_DateTime_NextMonth = 1021,
			cnEndUserMessages_DateTime_NextYear = 1022,
			cnEndUserMessages_DateTime_Close = 1023,
			cnEndUserMessages_DateTime_Now = 1024,
			cnEndUserMessages_DateTime_AM = 1025,
			cnEndUserMessages_DateTime_PM = 1026,
			cnEndUserMessages_DateTime_Delimiter = 1027,
			cnEndUserMessages_DateTime_Clear = 1028,

			cnDeveloperMessages_General_UnknownError = 1100,
			cnDeveloperMessages_General_DeveloperDefined = 1101,
			cnDeveloperMessages_General_RendererIsOpen = 1102,
			cnDeveloperMessages_General_MaliciousSQLFound = 1103,
			cnDeveloperMessages_General_MissingRequiredColumns = 1104,
			cnDeveloperMessages_General_InvalidColumnCount = 1105,
			cnDeveloperMessages_General_NoDataToLoad = 1106,
			cnDeveloperMessages_General_DataNotLoaded = 1107,
			cnDeveloperMessages_General_PositiveIntegerRequired = 1108,
			cnDeveloperMessages_General_UnknownValue = 1109,
			cnDeveloperMessages_General_IndexOutOfRange = 1110,
			cnDeveloperMessages_General_ValueRequired = 1111,

			cnDeveloperMessages_FormRenderer_UnknownInputAlias = 1200,
			cnDeveloperMessages_FormRenderer_DuplicateInputAlias = 1201,
			cnDeveloperMessages_FormRenderer_InsertNullSaveType = 1202,
			cnDeveloperMessages_FormRenderer_UnknownPicklistName = 1203,
			cnDeveloperMessages_FormRenderer_PicklistNameNotDefined = 1204,
			cnDeveloperMessages_FormRenderer_CallMultiRenderInput = 1205,
			cnDeveloperMessages_FormRenderer_CallSingleRenderInput = 1206,
    		cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Hidden = 1207,
			cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Picklist = 1208,
			cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Boolean = 1209,
			cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Integer = 1210,
			cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Float = 1211,
			cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Currency = 1212,
			cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_DateTime = 1213,
			cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Binary = 1214,
			cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_GUID = 1215,
			cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Other = 1216,
		    cnDeveloperMessages_FormRenderer_UnsupportedDataType = 1217,
    		cnDeveloperMessages_FormRenderer_SQLNotGenerated = 1218,
		    cnDeveloperMessages_FormRenderer_UpdateMissingID = 1219,
			cnDeveloperMessages_FormRenderer_PicklistIsEmpty = 1220,
			cnDeveloperMessages_FormRenderer_PicklistNotDefined = 1221,
			cnDeveloperMessages_FormRenderer_Picklist_IsAdHoc = 1222,
			cnDeveloperMessages_FormRenderer_InputAliasWithPrimaryDelimiter = 1223,

			cnDeveloperMessages_ReportRenderer_InvalidBodyReturn = 1300,

			cnDeveloperMessages_InputTools_NoInputAliases = 1400,

			cnDeveloperMessages_RendererSearchForm_MaxResults = 1500,
			cnDeveloperMessages_RendererSearchForm_UniqueUserID = 1501,
			cnDeveloperMessages_RendererSearchForm_MissingIDColumn = 1502,
			cnDeveloperMessages_RendererSearchForm_NullCookieMonster = 1503,
			cnDeveloperMessages_RendererSearchForm_GetSQL_SingleArgument = 1504,
			cnDeveloperMessages_RendererSearchForm_GetSQL_NullIDColumnOrTables = 1505,
			cnDeveloperMessages_RendererSearchForm_GetSQL_NotASingleRecord = 1506,
			cnDeveloperMessages_RendererSearchForm_InvalidComparisonType = 1507,
			cnDeveloperMessages_RendererSearchForm_UnknownCustomInputAlias = 1508,
			cnDeveloperMessages_RendererSearchForm_InsertCustomInputClauseMissingToken = 1509,
			cnDeveloperMessages_RendererSearchForm_InsertCustomInputClauseSecondCall = 1510,
    		cnDeveloperMessages_RendererSearchForm_DuplicateInputOrderAlias = 1511,
    		cnDeveloperMessages_RendererSearchForm_InputAliasMissingFromOrdering = 1512,
    		cnDeveloperMessages_RendererSearchForm_InputOrderingIncomplete = 1513,
		    cnDeveloperMessages_RendererSearchForm_NoSearchableInputs = 1514,
		    cnDeveloperMessages_RendererSearchForm_NoOrderedInputAliases = 1515,
			cnDeveloperMessages_RendererSearchForm_UnknownNonCustomInputAlias = 1516,
			cnDeveloperMessages_RendererSearchForm_DateTimeFormatMissing = 1517,

			cnDeveloperMessages_Renderer_InvalidDecoderTruncateString = 1600,
			cnDeveloperMessages_Renderer_USSHashMissingFormName = 1601,
			cnDeveloperMessages_Renderer_RenderedJavaScript = 1602,

			cnDeveloperMessages_UserSelectedStack_NotInitialized = 1700,

			cnDeveloperMessages_DateMath_UnreconizedCalculateDayFunction = 1801,
			cnDeveloperMessages_DateMath_InvalidHolidayMonth = 1802,
			cnDeveloperMessages_DateMath_InvalidMonthDay = 1803,
			cnDeveloperMessages_DateMath_InvalidWeekInMonth = 1804,
			cnDeveloperMessages_DateMath_InvalidWeekDay = 1805,
			cnDeveloperMessages_DateMath_InvalidCalculatedFrequency = 1806,

			cnDeveloperMessages_CookieMonster_InvalidKeysValues = 1900,
			cnDeveloperMessages_CookieMonster_InvalidKeysValuesDuplicateKey = 1901,
			cnDeveloperMessages_CookieMonster_KeyValueArrayBounds = 1902,
			cnDeveloperMessages_CookieMonster_InvalidKeyName = 1903,
			cnDeveloperMessages_CookieMonster_ValueTooLong = 1904,

			cnDeveloperMessages_DbMetaData_InvalidTable = 2001,
			cnDeveloperMessages_DbMetaData_InvalidIsNullable = 2002,
			cnDeveloperMessages_DbMetaData_BlankTableOrColumnName = 2003,
			cnDeveloperMessages_DbMetaData_InvalidDataType = 2004,
			cnDeveloperMessages_DbMetaData_InvalidTableName = 2005,
			cnDeveloperMessages_DbMetaData_InvalidTableColumnName = 2006,
			cnDeveloperMessages_DbMetaData_InvalidMinMamimumNumericValue = 2007,
			cnDeveloperMessages_DbMetaData_MissingMinMamimumNumericValue = 2008,
			cnDeveloperMessages_DbMetaData_InvalidParentChildRelationship = 2009,

			cnDeveloperMessages_Picklists_InvalidPicklistID = 2100,
			cnDeveloperMessages_Picklists_DataDescriptionDimensions = 2101,

			cnDeveloperMessages_MultiArray_DuplicateColumnName = 2200,
			cnDeveloperMessages_MultiArray_ColumnNameNotFound = 2201,
			cnDeveloperMessages_MultiArray_NoRows = 2202,
			cnDeveloperMessages_MultiArray_LastColumn = 2203,
			cnDeveloperMessages_MultiArray_IDColumnRequired = 2204,

			cnDeveloperMessages_DbTools_WhereClauseRequired = 2300,
			cnDeveloperMessages_DbTools_InsertUpdateColumnsRequired = 2301
		}
		#endregion

			//#### Declare the required private variables
		private static MultiArray g_oDefaultData;
		private Picklists g_oPicklists;
        private static CollectionHelper g_oGetData;
		private string g_sLanguageCode;

			//#### Declare the required private, developer modifiable constants
			//####     InitialLoadLanguageCode: This is the language code used if there is no data loaded into the g_cVariableName_Settings Picklists
			//####     MissingDefaultData: The developer error message that is displayed if the global (application) variable containing the Settings picklist cannot be loaded.
		private const string g_cInitialLoadLanguageCode = "en";
		private const string g_cMissingDefaultData = "Error enum '$sExtraInfo1' occured with the following infomation: '$sExtraInfo2'. Additionally, an error occured accessing the default Internationalization data used by all instances (have you set Internationalization.DefaultData?).";

			//#### Declare the required private constants
	  //private const string g_cClassName = "Cn.Configuration.Internationalization.";
		private const string g_sDefaultTableName = "cnInternationalization";


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
        ///############################################################
        /// <summary>
		/// Initializes the class.
        /// </summary>
        /// <param name="oInternationalizationData">MultiArray representing the entire, self-referencing Internationalization picklists structure.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oInternationalizationData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oInternationalizationData</paramref> does not contain the required column names as defined by <c>Picklists.GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oInternationalizationData</paramref> contains one or more <c>Rows</c> with a non-numeric value in 'PicklistID'.</exception>
        ///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public Internationalization(MultiArray oInternationalizationData) {
				//#### Pass the call off to .DoReset
			DoReset(oInternationalizationData);
		}

        ///############################################################
        /// <summary>
		/// Resets the class to its initilized state.
        /// </summary>
        /// <param name="oInternationalizationData">MultiArray representing the entire, self-referencing Internationalization picklists structure.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oInternationalizationData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oInternationalizationData</paramref> does not contain the required column names as defined by <c>Picklists.GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oInternationalizationData</paramref> contains one or more <c>Rows</c> with a non-numeric value in 'PicklistID'.</exception>
        ///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public void Reset(MultiArray oInternationalizationData) {
				//#### Pass the call off to .DoReset
			DoReset(oInternationalizationData);
		}

        ///############################################################
		/// <summary>
		/// Resets the class to its initilized state, loading the provided Internationalization picklist data into this instance.
		/// </summary>
        /// <param name="oInternationalizationData">MultiArray representing the entire, self-referencing Internationalization picklists structure.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oInternationalizationData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oInternationalizationData</paramref> does not contain the required column names as defined by <c>Picklists.GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oInternationalizationData</paramref> contains one or more <c>Rows</c> with a non-numeric value in 'PicklistID'.</exception>
		/// <seealso cref="Cn.Data.Picklists"/>
        ///############################################################
		/// <LastUpdated>January 19, 2010</LastUpdated>
		private void DoReset(MultiArray oInternationalizationData) {
				//#### (Re)Init the g_oPicklists, flipping its .StrictConversions to true (so that we can tell when a .Value is not found)
			g_oPicklists = new Picklists(oInternationalizationData);
			g_oPicklists.StrictDecodes = true;

				//#### Default the g_oDefaultData to the initial oInternationalizationData
				//####     NOTE: Since most of the time in single language enviroments this is what would be done anyway, we commit this by default
			g_oDefaultData = oInternationalizationData.Data;

			    //#### Reload our .LanguageCode form the newly loaded oInternationalizationData (utilizing our property to ensure the logic is run correctly)
			LanguageCode = "";
		}


		//##########################################################################################
		//# Public Read-Write Properties
		//##########################################################################################
        ///############################################################
        /// <summary>
        /// Gets/sets the ISO639 2-letter language code used by this instance.
        /// </summary>
        /// <remarks>
        /// If an unreconized language code is passed, the default language code (as defined within this instance's Internationalization data, or failing that within the local constant) is used.
        /// </remarks>
		/// <value>String representing the ISO639 2-letter language code used by this instance.</value>
		/// <seealso cref="Cn.Configuration.Internationalization.ValidateLanguageCode"/>
        ///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
		public string LanguageCode {
			get { return g_sLanguageCode; }
			set { g_sLanguageCode = ValidateLanguageCode(value); }
		}


		//##########################################################################################
		//# Public Static Read-Write Properties
		//##########################################################################################
        ///############################################################
        /// <summary>
        /// Gets/sets the default Internationalization data used by all instances.
        /// </summary>
		/// <value>MultiArray representing the entire, self-referencing Internationalization picklists structure used by all instances.</value>
        ///############################################################
		/// <LastUpdated>May 10, 2007</LastUpdated>
		public static MultiArray DefaultData {
			get { return g_oDefaultData; }
			set { g_oDefaultData = value; }
		}


		//##########################################################################################
		//# Public Read-Only Properties
		//##########################################################################################
        ///############################################################
        /// <summary>
		/// Assists in the collection of the underlying structure which defines this instance.
        /// </summary>
		/// <value>CollectionHelper instance configured to collect the underlying structure which defines this instance.</value>
        ///############################################################
		/// <LastUpdated>January 14, 2010</LastUpdated>
		public static CollectionHelper GetData {
			get {
					//#### If g_oGetData hasn't be setup, do so now
				if (g_oGetData == null) {
					g_oGetData = new CollectionHelper(Picklists.GetData.RequiredColumns, Picklists.GetData.BaseSQLStatement, g_sDefaultTableName);
				}

				return g_oGetData;
			}
		}

        ///############################################################
        /// <summary>
        /// Gets a deep copy of the underlying Internationalization data used by this instance.
        /// </summary>
		/// <value>MultiArray representing a deep copy of the underlying Internationalization data used by this instance.</value>
        ///############################################################
		/// <LastUpdated>May 10, 2007</LastUpdated>
		public MultiArray Data {
			get { return g_oPicklists.Data; }
		}


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Retrieves the requested Internationalization picklist.
        /// </summary>
        /// <param name="ePicklist">Enumeration representing the required Internationalization picklist.</param>
		/// <returns>MultiArray containing all the entries for the passed <paramref>ePicklist</paramref> defined within this instance, or null if it was not defined.</returns>
        ///############################################################
		/// <LastUpdated>May 10, 2007</LastUpdated>
        public MultiArray Values(enumInternationalizationPicklists ePicklist) {
				//#### Pass the call off to our sibling implementation (while passing in the g_sLanguageCode for the required sLanguageCode)
			return Values(ePicklist, g_sLanguageCode);
        }

        ///############################################################
        /// <summary>
        /// Retrieves the requested Internationalization picklist using the referenced language code.
        /// </summary>
        /// <param name="ePicklist">Enumeration representing the required Internationalization picklist.</param>
		/// <param name="sLanguageCode">String representing the ISO639 2-letter language code to use when collecting the value.</param>
		/// <returns>MultiArray containing all the entries for the passed <paramref>ePicklist</paramref> defined within this instance, or null if it was not defined.</returns>
        ///############################################################
		/// <LastUpdated>May 29, 2007</LastUpdated>
        public MultiArray Values(enumInternationalizationPicklists ePicklist, string sLanguageCode) {
            MultiArray oReturn;
			string sPicklistName;

				//#### Determine the value of the passed ePicklist, setting our return value accordingly
			switch (ePicklist) {
					//#### Cn internationalization picklists: non-sLanguageCode prefixed PicklistNames
                case enumInternationalizationPicklists.cnLanguageCodes: {
                    oReturn = g_oPicklists.Picklist("LanguageCodes");
                    break;
                }

					//#### Cn internationalization picklists: sLanguageCode prefixed PicklistNames
				default: {
						//#### Determine the .ToString-ified version of the passed ePicklist
					sPicklistName = ePicklist.ToString();

						//#### If the sPicklistName value is longer then 2 characters, peal off the leading "cn"
						//####     NOTE: This should always be the case, but we check just in case
					if (sPicklistName.Length > 2) {
						sPicklistName = sPicklistName.Substring(2);
					}

						//#### Toss the sPicklistName into .Picklist (prefixed with the passed sLanguageCode), resetting the oReturn value accordingly
					oReturn = g_oPicklists.Picklist(sLanguageCode + sPicklistName);
					break;
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
        }

        ///############################################################
        /// <summary>
        /// Retrieves the requested Internationalization value.
        /// </summary>
        /// <param name="eValue">Enumeration representing the required Internationalization value.</param>
        /// <returns>String representing the requested Internationalization value.</returns>
        ///############################################################
		/// <LastUpdated>May 10, 2007</LastUpdated>
        public string Value(enumInternationalizationValues eValue) {
				//#### Pass the call off to our sibling implementation (while passing in the g_sLanguageCode for the required sLanguageCode)
			return Value(eValue, g_sLanguageCode);
        }

        ///############################################################
        /// <summary>
        /// Retrieves the requested Internationalization value using the referenced language code.
        /// </summary>
        /// <param name="eValue">Enumeration representing the required Internationalization value.</param>
		/// <param name="sLanguageCode">String representing the ISO639 2-letter language code to use when collecting the value.</param>
        /// <returns>String representing the requested Internationalization value.</returns>
        ///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
        public string Value(enumInternationalizationValues eValue, string sLanguageCode) {
			string[] a_sPicklistInfo;
			string sReturn;

				//#### Borrow the use of the sReturn value to determine the .ToString-ified version of the passed eValue
			sReturn = eValue.ToString();

				//#### If the borrowed sReturn value is longer then 2 characters, peal off the leading "cn"
			if (sReturn.Length > 2) {
				sReturn = sReturn.Substring(2);
			}

				//#### Determine the PicklistName and DecodeValue from the borrowed sReturn value
			a_sPicklistInfo = sReturn.Split("_".ToCharArray(), 2);

				//#### Determine the value of the passed eValue, setting our return value accordingly
			switch (eValue) {
					//#### Cn internationalization values: constants
				case enumInternationalizationValues.cnDeveloperMessages_General_MissingDefaultData: {
					sReturn = g_cMissingDefaultData;
					break;
				}

					//#### Cn internationalization values: non-sLanguageCode prefixed PicklistNames
				case enumInternationalizationValues.cnGeneralSettings_DefaultLanguageCode: {
						//#### Toss the PicklistName and DecodeValue into the .Decoder, resetting the sReturn value accordingly
					sReturn = g_oPicklists.Decoder(a_sPicklistInfo[0], a_sPicklistInfo[1]);
					break;
				}

					//#### Cn internationalization values: sLanguageCode prefixed PicklistNames
				default: {
						//#### Toss the PicklistName (prefixed with the sLanguageCode) and DecodeValue into the .Decoder, resetting the sReturn value accordingly
					sReturn = g_oPicklists.Decoder(sLanguageCode + a_sPicklistInfo[0], a_sPicklistInfo[1]);
					break;
				}
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
        }

		///############################################################
		/// <summary>
		/// Validates the provided ISO639 2-letter language code as being defined within this instance.
		/// </summary>
        /// <remarks>
        /// If an unreconized language code is passed, the default language code (as defined within this instance's Internationalization data, or failing that within the local constant) is used.
        /// </remarks>
		/// <param name="sLanguageCode">String representing the ISO639 2-letter language code.</param>
		/// <returns>String representing the validated ISO639 2-letter language code.</returns>
		///############################################################
		/// <LastUpdated>May 29, 2007</LastUpdated>
		public string ValidateLanguageCode(string sLanguageCode) {
			string sReturn = sLanguageCode;

				//#### If the passed sLanguageCode was null or a null-string (as stored within our sReturn value)
			if (string.IsNullOrEmpty(sReturn)) {
					//#### Reset our sReturn value based on the value of .cnGeneralSettings_DefaultLanguageCode (defaulting it to g_cInitialLoadLanguageCode is it is not set)
				sReturn = Cn.Data.Tools.MakeString(Value(enumInternationalizationValues.cnGeneralSettings_DefaultLanguageCode), g_cInitialLoadLanguageCode);
			}

				//#### If the thus far's above determined sReturn value is unreconized, reset it to the g_cInitialLoadLanguageCode
				//####     NOTE: We do not check the g_cInitialLoadLanguageCode as we'll assume that the developer has set it to a valid value (and if not nothing will be decoded)
			if (! g_oPicklists.Exists("LanguageCodes", sReturn)) {
				sReturn = g_cInitialLoadLanguageCode;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}


        //##########################################################################################
        //# Public RaiseError-related Functions
        //##########################################################################################
        ///############################################################
		/// <summary>
		/// Raises a CnException-based error utilizing the default internationalization data.
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
        /// <param name="eErrorMessage">Enumeration representing the required error message value.</param>
		/// <param name="sExtraInfo1">String representing the value to replace any occurances of '$sExtraInfo1' with.</param>
		/// <param name="sExtraInfo2">String representing the value to replace any occurances of '$sExtraInfo2' with.</param>
		/// <exception cref="Cn.CnException">Thrown when called.</exception>
        ///############################################################
		/// <LastUpdated>June 5, 2007</LastUpdated>
        public static void RaiseDefaultError(string sFunction, enumInternationalizationValues eErrorMessage, string sExtraInfo1, string sExtraInfo2) {
			Internationalization oIntl;

				//#### If the .DefaultData has been defined
			if (DefaultData != null && DefaultData.RowCount > 0) {
					//#### Create an instance based on the .DefaultData and .Raise(the)Error
				oIntl = new Internationalization(DefaultData);
				oIntl.RaiseError(sFunction, eErrorMessage, sExtraInfo1, sExtraInfo2);
			}
				//#### Else the .DefaultData isn't properly set, so throw our own CnException based on the g_cMissingDefaultData (aka -.cnDeveloperMessages_General_MissingDefaultData) error
			else {
				throw new CnException(sFunction,
					sFunction + ": " +
						DoValueDecoder(g_cMissingDefaultData, eErrorMessage.ToString(), "[" + sExtraInfo1 + "], [" + sExtraInfo2 + "]", true, (int)enumInternationalizationValues.cnDeveloperMessages_General_MissingDefaultData),
					(int)eErrorMessage
				);
			}
        }

        ///############################################################
		/// <summary>
		/// Raises a CnException-based error.
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
        /// <param name="eErrorMessage">Enumeration representing the required error message value.</param>
		/// <param name="sExtraInfo1">String representing the value to replace any occurances of '$sExtraInfo1' with.</param>
		/// <param name="sExtraInfo2">String representing the value to replace any occurances of '$sExtraInfo2' with.</param>
		/// <exception cref="Cn.CnException">Thrown when called.</exception>
        ///############################################################
		/// <LastUpdated>June 5, 2007</LastUpdated>
		public void RaiseError(string sFunction, enumInternationalizationValues eErrorMessage, string sExtraInfo1, string sExtraInfo2) {
				//#### Hand the passed data off to a thrown CnException (ValueDecoder-ing the passed eErrorMessage as we go)
			throw new CnException(sFunction,
				sFunction + ": " + ValueDecoder(eErrorMessage, sExtraInfo1, sExtraInfo2, g_sLanguageCode, true),
				(int)eErrorMessage
			);
		}

        ///############################################################
        /// <summary>
        /// Decodes the referenced Internationalization value utilizing the provided extra information.
        /// </summary>
        /// <param name="eValue">Enumeration representing the required Internationalization value.</param>
		/// <param name="sExtraInfo1">String representing the value to replace any occurances of '$sExtraInfo1' with.</param>
		/// <param name="sExtraInfo2">String representing the value to replace any occurances of '$sExtraInfo2' with.</param>
		/// <param name="sLanguageCode">String representing the ISO639 2-letter language code.</param>
		/// <param name="bShowValueNumber">Boolean value indicating if the value number is to be shown at the end of the message.</param>
        /// <returns>String value containing the decoded <paramref>eValue</paramref> as defined within this instance.</returns>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
        public string ValueDecoder(enumInternationalizationValues eValue, string sExtraInfo1, string sExtraInfo2, string sLanguageCode, bool bShowValueNumber) {
				//#### Pass the call off to .DoValueDecoder (getting the .Value for the passed eValue as we go)
			return DoValueDecoder(Value(eValue, sLanguageCode), sExtraInfo1, sExtraInfo2, bShowValueNumber, (int)eValue);
        }


        //#######################################################################################################
        //# Private Functions
        //#######################################################################################################
        ///############################################################
        /// <summary>
        /// Decodes the referenced Internationalization value utilizing the provided extra information.
        /// </summary>
        /// <param name="sValue">String representing the required Internationalization value.</param>
		/// <param name="sExtraInfo1">String representing the value to replace any occurances of '$sExtraInfo1' with.</param>
		/// <param name="sExtraInfo2">String representing the value to replace any occurances of '$sExtraInfo2' with.</param>
		/// <param name="bShowValueNumber">Boolean value indicating if the value number is to be shown at the end of the message.</param>
		/// <param name="iValueNumber">Integer value representing the value number of the required Internationalization value.</param>
        /// <returns>String value containing the decoded <paramref>sValue</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>May 16, 2007</LastUpdated>
        private static string DoValueDecoder(string sValue, string sExtraInfo1, string sExtraInfo2, bool bShowValueNumber, int iValueNumber) {
			string sReturn = Cn.Data.Tools.MakeString(sValue, "");

                //#### Replace the PHP-ish variables in the above-determined sReturn value with the passed (or above modified) sExtraInfo1/sExtraInfo2
            sReturn = sReturn.Replace("$sExtraInfo1", sExtraInfo1);
            sReturn = sReturn.Replace("$sExtraInfo2", sExtraInfo2);
            
				//#### If the caller has decided to bShow(the)ValueNumber, append it onto the sReturn value
			if (bShowValueNumber) {
				sReturn += " [#" + iValueNumber + "]";
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
        }

	} //# class Internationalization

} //# namespace Cn.Configuration
