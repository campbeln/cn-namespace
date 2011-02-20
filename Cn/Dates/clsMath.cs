/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;                                       //# Required to access DateTime datatype
using System.Collections;					        //# Required to access the Hashtable class
using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Configuration;
using Cn.Data;

//# Required to access the Internationalization class


namespace Cn.Dates {

	///########################################################################################################################
	/// <summary>
	/// A collection of date calculation-related routines, including the calculation of Business Days based on the loaded holiday calculation data.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>August 10, 2005</LastFullCodeReview>
    public class Math {
	#region Math
            //#### Declare the required private variables
        private MultiArray g_oData;
        private static CollectionHelper g_oGetData;

            //#### Declare the required public eNums
        #region eNums
			/// <summary>Defined holiday definition types.</summary>
	    public enum enumDefinitionTypes : int {			//# These value *MUST* match their related "Data" field within the Internationalization picklist
				/// <summary>Defined holiday is static (i.e. - September 11, 2001).</summary>
	        cnStatic = 0,
				/// <summary>Defined holiday represents an Nth week day in the month (i.e. - 2nd Wednesday of the month, last Monday of the month, etc.).</summary>
	        cnNthWeekDayInMonth = 1,
				/// <summary>Defined data is calculated by a referenced method (i.e. - Easter Sunday).</summary>
	        cnCalculated = 2
	    }
			/// <summary>Date recurrence frequencies.</summary>
	    public enum enumFrequencies : int {				//# These value *MUST* match their related "Data" field within the Internationalization picklist
				/// <summary>Holiday recurs weekly.</summary>
	        cnWeekly = 0,
				/// <summary>Holiday recurs every second week.</summary>
	        cnFortnightly = 1,
				/// <summary>Holiday recurs monthly.</summary>
	        cnMonthly = 2,
				/// <summary>Holiday recurs every three months.</summary>
	        cnQuarterly = 3,
				/// <summary>Holiday recurs every four months.</summary>
	        cnTriAnnually = 4,
				/// <summary>Holiday recurs every six months.</summary>
	        cnSemiAnnually = 5,
				/// <summary>Holiday recurs once a year.</summary>
	        cnAnnually = 6
	    }
			/// <summary>Defined holiday external calculations.</summary>
			/// <remarks>NOTE: Matching values are only required for calculations with a "Data" &lt; 1000. All other calculations are developer defined/handled.</remarks>
	    public enum enumCalculations : int {	//# These value *MUST* match their related "Data" field within the Internationalization picklist
				/// <summary>Good Friday (Friday preceding Easter Sunday).</summary>
            cnGoodFriday = 0,
				/// <summary>Easter Sunday.</summary>
            cnEaster = 1,
				/// <summary>Easter Monday (Monday following Easter Sunday).</summary>
            cnEasterMonday = 2
	    }
	    #endregion

            //#### Declare the required private constants
	    private const string g_cClassName = "Cn.Dates.Math.";
	    private const string g_cColumnNames = "ID,Description,DefinitionType,Frequency,Country,Region,HolidayMonth,MonthDay,WeekDay,WeekInMonth,EffectiveYear,CalculatedDayFunction";
		private const string g_sBaseSQL = "SELECT " + g_cColumnNames + " FROM $TableName ORDER BY ID";
		private const string g_sDefaultTableName = "cnHolidayCalculations";


        //##########################################################################################
        //# Class Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
		/// Initializes the class.
        /// </summary>
        /// <param name="oHolidayCalculationsData">MultiArray representing the entire holiday calculations structure.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> does not contain the required column names as defined by <c>GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> with an unreconized value in 'DefinitionType'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> with an unreconized value in 'Frequency'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnStatic</c> or <c>cnNthWeekDayInMonth</c> 'DefinitionType' with an invalid value in 'HolidayMonth'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnStatic</c> 'DefinitionType' with an invalid value in 'MonthDay'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnNthWeekDayInMonth</c> 'DefinitionType' with an invalid value in 'WeekInMonth'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnNthWeekDayInMonth</c> 'DefinitionType' with an invalid value in 'WeekDay'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as both a <c>cnCalculated</c> 'DefinitionType' and a non-<c>cnAnnually</c> 'Frequency'.</exception>
        ///############################################################
		/// <LastUpdated>May 7, 2007</LastUpdated>
	    public Math(MultiArray oHolidayCalculationsData) {
				//#### Pass the call off to .DoReset
			DoReset("[Constructor]", oHolidayCalculationsData);
	    }

        ///############################################################
        /// <summary>
		/// Resets the class to its initilized state.
        /// </summary>
        /// <param name="oHolidayCalculationsData">MultiArray representing the entire holiday calculations structure.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> does not contain the required column names as defined by <c>GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> with an unreconized value in 'DefinitionType'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> with an unreconized value in 'Frequency'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnStatic</c> or <c>cnNthWeekDayInMonth</c> 'DefinitionType' with an invalid value in 'HolidayMonth'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnStatic</c> 'DefinitionType' with an invalid value in 'MonthDay'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnNthWeekDayInMonth</c> 'DefinitionType' with an invalid value in 'WeekInMonth'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnNthWeekDayInMonth</c> 'DefinitionType' with an invalid value in 'WeekDay'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as both a <c>cnCalculated</c> 'DefinitionType' and a non-<c>cnAnnually</c> 'Frequency'.</exception>
        ///############################################################
		/// <LastUpdated>August 22, 2005</LastUpdated>
		public void Reset(MultiArray oHolidayCalculationsData) {
				//#### Pass the call off to .DoReset
			DoReset("Reset", oHolidayCalculationsData);
		}

        ///############################################################
        /// <summary>
        /// Resets the class to its initilized state, while loading the provided holiday calculations data into this instance.
        /// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
        /// <param name="oHolidayCalculationsData">MultiArray representing the entire holiday calculations structure.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> does not contain the required column names as defined by <c>GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> with an unreconized value in 'DefinitionType'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> with an unreconized value in 'Frequency'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnStatic</c> or <c>cnNthWeekDayInMonth</c> 'DefinitionType' with an invalid value in 'HolidayMonth'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnStatic</c> 'DefinitionType' with an invalid value in 'MonthDay'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnNthWeekDayInMonth</c> 'DefinitionType' with an invalid value in 'WeekInMonth'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as a <c>cnNthWeekDayInMonth</c> 'DefinitionType' with an invalid value in 'WeekDay'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oHolidayCalculationsData</paramref> contains one or more <c>Rows</c> defined as both a <c>cnCalculated</c> 'DefinitionType' and a non-<c>cnAnnually</c> 'Frequency'.</exception>
        ///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
	    private void DoReset(string sFunction, MultiArray oHolidayCalculationsData) {
		    int iTemp;
		    int iMonth;
		    int iYear;
		    int i;
		    bool bErrorOccured = false;
		    bool bIsCalculated;

                //#### If the passed oHolidayCalculationsData contains no .Rows (or is null), raise the error
		    if (oHolidayCalculationsData == null || oHolidayCalculationsData.RowCount == 0) {
			    Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_NoDataToLoad, "oHolidayCalculationsData", "");
		    }
                //#### Else we've got some data to process
		    else {
                    //#### If the passed oHolidayCalculationsData contains the required columns
			    if (oHolidayCalculationsData.Exists(GetData.RequiredColumns)) {
                        //#### Traverse the .Rows within the passed oHolidayCalculationsData
				    for (i = 0; i < oHolidayCalculationsData.RowCount; i++) {
				            //#### Borrow the use of iTemp to store the MakeInteger'd DefinitionType
				        iTemp = Cn.Data.Tools.MakeInteger(oHolidayCalculationsData.Value(i, "DefinitionType"), -1);

                            //#### Determine the current value of the DefinitionType and process accordingly
                        switch (iTemp) {
                                //#### If the DefinitionType is a cnStatic type
                            case (int)enumDefinitionTypes.cnStatic: {
                                    //#### Set iYear to the EffectiveYear and filp bIsCalculated to false
						        iYear = Cn.Data.Tools.MakeInteger(oHolidayCalculationsData.Value(i, "EffectiveYear"), -1);
						        bIsCalculated = false;

                                    //#### Set iMonth to the HolidayMonth then ensure it has been properly set, raising the error if it hasn't
						        iMonth = Cn.Data.Tools.MakeInteger(oHolidayCalculationsData.Value(i, "HolidayMonth"), -1);
						        if (iMonth < 1 || iMonth > 12) {
							        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DateMath_InvalidHolidayMonth, "Static", oHolidayCalculationsData.Value(i, "HolidayMonth"));
						        }

                                    //#### Set the borrowed iTemp to the MonthDay then ensure it has been properly set, raising the error if it hasn't
						        iTemp = Cn.Data.Tools.MakeInteger(oHolidayCalculationsData.Value(i, "MonthDay"), -1);
						        if (iTemp < 1 || iTemp > Dates.Tools.DaysInMonth(iMonth, iYear)) {
							        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DateMath_InvalidMonthDay, "Static", oHolidayCalculationsData.Value(i, "MonthDay"));
						        }
                                break;
                            }

                                //#### If g_cDefinitionType is a cnNthWeekDayInMonth type
                            case (int)enumDefinitionTypes.cnNthWeekDayInMonth: {
                                    //#### Filp bIsCalculated to false
						        bIsCalculated = false;

                                    //#### Set iMonth to g_cHolidayMonth then ensure it has been properly set, raising the error if it hasn't
						        iMonth = Cn.Data.Tools.MakeInteger(oHolidayCalculationsData.Value(i, "HolidayMonth"), -1);
						        if (iMonth < 1 || iMonth > 12) {
							        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DateMath_InvalidHolidayMonth, "NthWeekdayInMonth", oHolidayCalculationsData.Value(i, "HolidayMonth"));
						        }

                                    //#### Ensure that WeekInMonth has been properly set, raising the error if it hasn't
						        if (Cn.Data.Tools.MakeInteger(oHolidayCalculationsData.Value(i, "WeekInMonth"), 0) == 0) {
							        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DateMath_InvalidWeekInMonth, "NthWeekdayInMonth", oHolidayCalculationsData.Value(i, "WeekInMonth"));
						        }

                                    //#### Borrow the use of iTemp to store the WeekDay
                                iTemp = Cn.Data.Tools.MakeInteger(oHolidayCalculationsData.Value(i, "WeekDay"), -1);

                                    //#### Determine the value of the WeekDay in the borrowed i and process accordingly
                                switch (iTemp) {
                                        //#### If g_cWeekDay is a reconized enumWeekDays, do nothing
                                    case (int)enumWeekDays.cnSunday:
                                    case (int)enumWeekDays.cnMonday:
                                    case (int)enumWeekDays.cnTuesday:
                                    case (int)enumWeekDays.cnWednesday:
                                    case (int)enumWeekDays.cnThursday:
                                    case (int)enumWeekDays.cnFriday:
                                    case (int)enumWeekDays.cnSaturday: {
                                        break;
                                    }
                                        //#### Else g_cWeekDay is unreconized, so raise the error
                                    default: {
							            Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DateMath_InvalidWeekDay, "NthWeekdayInMonth", oHolidayCalculationsData.Value(i, "WeekDay"));
                                        break;
                                    }
                                }
                                break;
                            }

                                //#### If the DefinitionType is a cnCalculated type
                            case (int)enumDefinitionTypes.cnCalculated: {
                                    //#### Filp bIsCalculated to true
						        bIsCalculated = true;
                                break;
                            }

                                //#### Else the g_cDefinitionType is unreconized, so raise the error and filp bIsCalculated to false
                            default: {
						        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "DefinitionType", oHolidayCalculationsData.Value(i, "DefinitionType"));
						        bIsCalculated = false;
                                break;
                            }
                        }

				            //#### Borrow the use of iTemp to store the MakeInteger'd Frequency
				        iTemp = Cn.Data.Tools.MakeInteger(oHolidayCalculationsData.Value(i, "Frequency"), -1);

                            //#### Determine the current value of the Frequency in the borrowed sCurrentAttribute and process accordingly
                        switch (iTemp) {
                                //#### If the Frequency is a reconized non-cnAnnually type
                            case (int)enumFrequencies.cnWeekly:
                            case (int)enumFrequencies.cnFortnightly:
                            case (int)enumFrequencies.cnMonthly:
                            case (int)enumFrequencies.cnQuarterly:
                            case (int)enumFrequencies.cnTriAnnually:
                            case (int)enumFrequencies.cnSemiAnnually: {
                                    //#### If this is also a bIsCalculated date, raise the error
						        if (bIsCalculated) {
							        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DateMath_InvalidCalculatedFrequency, oHolidayCalculationsData.Value(i, "Frequency"), "");
						        }
						        break;
                            }
                                //#### If the Frequency is a reconized cnAnnually type, do nothing
                            case (int)enumFrequencies.cnAnnually: {
						        break;
                            }
                                //#### Else the g_cFrequency is unreconized, so raise the error
                            default: {
						        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "Frequency", oHolidayCalculationsData.Value(i, "Frequency"));
						        break;
                            }
                        }
				    }
			    }
                    //#### Else the passed a_sHolidayCalculationsData did not contain the required columns, so raise the error and set bErrorOccured to true
			    else {
				    Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MissingRequiredColumns, "oHolidayCalculationsData", "GetData.SQL");
				    bErrorOccured = true;
			    }

                    //#### If no bError(s)Occured above
		        if (! bErrorOccured) {
                        //#### Do a deep copy of the data within the passed oHolidayCalculationsData into the global g_oData
					g_oData = oHolidayCalculationsData.Data;
		        }
		    }
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
					//#### If the g_oGetData hasn't been setup yet, do so now
				if (g_oGetData == null) {
					g_oGetData = new CollectionHelper(g_cColumnNames.Split(','), g_sBaseSQL, g_sDefaultTableName);
				}

				return g_oGetData;
			}
		}

        ///############################################################
        /// <summary>
        /// Gets a deep copy of the underlying structure which defines this instance.
        /// </summary>
		/// <value>Deep copy of the MultiArray which defines the metadata of this instance.</value>
        ///############################################################
		/// <LastUpdated>December 18, 2009</LastUpdated>
		public MultiArray Data {
			get {
			    MultiArray oReturn = null;

					//#### If the global g_oData is not null
				if (g_oData != null) {
						//#### Make a deep copy of the g_oData it into our oReturn value
					oReturn = g_oData.Data;
				}

                    //#### Return the above determined oReturn value to the caller
				return oReturn;
			}
		}


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Calculates the date that is the provided business days from the passed date based on the passed country/region pair.
        /// </summary>
        /// <param name="dDate">DateTime representing the date the calculation begins.</param>
        /// <param name="iBusinessDays">Integer representing the number of business days (positive or negetive) from the provided date.</param>
        /// <param name="sCountry">String representing the desired developer-defined country code.</param>
        /// <param name="sRegion">String representing the desired developer-defined region code.</param>
        /// <param name="bIncludeWeekends">Boolean value signaling if weekends are to be counted as business days within the calculation.</param>
        /// <returns>DateTime representing the the date that is the provided <paramref>iBusinessDays</paramref> from the passed <paramref>dDate</paramref> based on the provided <paramref>sCountry</paramref>/<paramref>sRegion</paramref> pair.</returns>
        ///############################################################
		/// <LastUpdated>September 13, 2005</LastUpdated>
	    public DateTime AddBusinessDays(DateTime dDate, int iBusinessDays, string sCountry, string sRegion, bool bIncludeWeekends) {
		    HolidayYear oHolidayYear;
		    int i;

                //#### Collect oHolidayYear based on the passed dDate
		    oHolidayYear = GetYearsHolidays(dDate.Year);

                //#### If the caller is trying to retrieve the next business day (including today if it is one)
            if (iBusinessDays == 0) {
                    //#### As long as dDate (optionally) Is(a)Weekend or is within oHolidayYear
			    while (IsWeekend(dDate, bIncludeWeekends) || oHolidayYear.HasDate(dDate, sCountry, sRegion)) {
                        //#### Increment dDate by one day
				    dDate = dDate.AddDays(1);

                        //#### If dDate has crossed outside of oHolidayYear's .Year
				    if (dDate.Year != oHolidayYear.Year) {
                            //#### Regenerate oHolidayYear based on dDate's new .Year
					    oHolidayYear = GetYearsHolidays(dDate.Year);
				    }
			    }
            }
                //#### Else if the caller is looking for a business day in the future
		    else if (iBusinessDays > 0) {
                    //#### Traverse the passed iBusinessDays
			    for (i = 1; i <= iBusinessDays; i++) {
                        //#### As long as dDate (optionally) Is(a)Weekend or is within oHolidayYear
				    do {
                            //#### Increment dDate by one day
					    dDate = dDate.AddDays(1);

                            //#### If dDate has crossed outside of oHolidayYear's .Year
					    if (dDate.Year != oHolidayYear.Year) {
                                //#### Regenerate oHolidayYear based on dDate's new .Year
						    oHolidayYear = GetYearsHolidays(dDate.Year);
					    }
				    } while (IsWeekend(dDate, bIncludeWeekends) || oHolidayYear.HasDate(dDate, sCountry, sRegion));
			    }
		    }
                //#### Else the caller is looking for a business day in the past
		    else {
                    //#### Traverse the passed iBusinessDays
			    for (i = -1; i >= iBusinessDays; i--) {
                        //#### As long as dDate (optionally) Is(a)Weekend or is within oHolidayYear
				    do {
                                //#### Decrement dDate by one day
					    dDate = dDate.AddDays(-1);

                            //#### If dDate has crossed outside of oHolidayYear's .Year
					    if (dDate.Year != oHolidayYear.Year) {
                                //#### Regenerate oHolidayYear based on dDate's new Year
						    oHolidayYear = GetYearsHolidays(dDate.Year);
					    }
				    } while (IsWeekend(dDate, bIncludeWeekends) || oHolidayYear.HasDate(dDate, sCountry, sRegion));
			    }
		    }

                //#### Return the above calculated dDate to the caller
		    return dDate;
	    }

        ///############################################################
        /// <summary>
        /// Calculates the number of business days between the provided dates based on the passed country/region pair.
        /// </summary>
        /// <remarks>
        /// NOTE: The passed <paramref>dDate1</paramref> does not need to be before the passed <paramref>dDate2</paramref> (or vise versa). The ordering of <paramref>dDate1</paramref> and <paramref>dDate2</paramref> is not important.
        /// </remarks>
        /// <param name="dDate1">DateTime representing the date the calculation begins.</param>
        /// <param name="dDate2">DateTime representing the date the calculation ends.</param>
        /// <param name="sCountry">String representing the desired developer-defined country code.</param>
        /// <param name="sRegion">String representing the desired developer-defined region code.</param>
        /// <param name="bIncludeWeekends">Boolean value signaling if weekends are to be counted as business days within the calculation.</param>
        /// <returns>Integer representing the number of business days between the provided <paramref>dDate1</paramref> and <paramref>dDate2</paramref> based on the provided <paramref>sCountry</paramref>/<paramref>sRegion</paramref> pair.</returns>
        ///############################################################
		/// <LastUpdated>September 13, 2005</LastUpdated>
	    public int BusinessDaysDateDiff(DateTime dDate1, DateTime dDate2, string sCountry, string sRegion, bool bIncludeWeekends) {
		    HolidayYear oHolidayYear;
		    int iReturn = -1;               //# Default the return value to -1 (as it's preincremented below)
		    bool bInvertResult = true;

                //#### If the passed dDate1 is less then dDate2
		    if (dDate1 < dDate2) {
                    //#### Declare and set dTempDate to dDate1 and flip bInvertResult to false
			    DateTime dTempDate = dDate1;
			    bInvertResult = false;

                    //#### Finish flipping the dates into the proper config
			    dDate1 = dDate2;
			    dDate2 = dTempDate;
		    }

                //#### Collect oHolidayYear based on the passed (or the above flipped) dDate2
		    oHolidayYear = GetYearsHolidays(dDate2.Year);

                //#### Do while dDate1 is still greater then or equal to dDate2
		    do {
                    //#### Preincrement the iReturn value by 1
			    iReturn++;

                    //#### Do... while dDate2 (optionally) Is(a)Weekend or is within oHolidayYear
			    do {
                        //#### Increment dDate2 by one day
				    dDate2 = dDate2.AddDays(1);

                        //#### If dDate2 has crossed outside of oHolidayYear's .Year
				    if (dDate2.Year != oHolidayYear.Year) {
                            //#### Regenerate oHolidayYear based on dDate2's new .Year
					    oHolidayYear = GetYearsHolidays(dDate2.Year);
				    }
			    } while (dDate1 >= dDate2 && (IsWeekend(dDate2, bIncludeWeekends) || oHolidayYear.HasDate(dDate2, sCountry, sRegion)));
		    } while (dDate1 >= dDate2);

                //#### If we are supposed to bInvert(the)Result, do so now
		    if (bInvertResult) {
			    iReturn = (iReturn * -1);
		    }

		        //#### Return the above determined iReturn value to the caller
		    return iReturn;
	    }

        ///############################################################
        /// <summary>
        /// Determines if the passed date is a holiday based on the passed country/region pair.
        /// </summary>
        /// <param name="dDate">DateTime representing the date in question.</param>
        /// <param name="sCountry">String representing the desired developer-defined country code.</param>
        /// <param name="sRegion">String representing the desired developer-defined region code.</param>
        /// <returns>Boolean value indicating if the passed <paramref>dDate</paramref> is a holiday within the provided <paramref>sCountry</paramref>/<paramref>sRegion</paramref> pair.</returns>
        ///############################################################
		/// <LastUpdated>September 13, 2005</LastUpdated>
	    public bool IsHoliday(DateTime dDate, string sCountry, string sRegion) {
                //#### Determine if the passed dDate/sCountry/sRegion combo is within its .Year's Holidays, returning the result of .HasDate as our own
		    return GetYearsHolidays(dDate.Year).HasDate(dDate, sCountry, sRegion);
	    }

        ///############################################################
        /// <summary>
        /// Calculates the referenced floating holiday within the provided year.
        /// </summary>
        /// <remarks>
        /// This function should be overridden by your own implementation which handles your additionally defined floating holidays.
        /// </remarks>
        /// <param name="iCalculationEnum">Enumeration representing the required floating holiday calculation.</param>
		/// <param name="iYear">Integer representing the required year.</param>
        /// <returns>DateTime representing the date of the floating holiday referenced by <paramref>iCalculationEnum</paramref> for the passed <paramref>iYear</paramref>.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iCalculationEnum</paramref> is unreconized by this implementation.</exception>
        ///############################################################
		/// <LastUpdated>August 24, 2005</LastUpdated>
	    public virtual DateTime CalculateFloatingHoliday(int iCalculationEnum, int iYear) {
	        DateTime dReturn;

                //#### Determine the passed iCalculationEnum and process accordingly
            switch (iCalculationEnum) {
                case (int)enumCalculations.cnGoodFriday: {
			        dReturn = EasterSunday(iYear).AddDays(-2);
			        break;
                }
                case (int)enumCalculations.cnEaster: {
			        dReturn = EasterSunday(iYear);
			        break;
                }
                case (int)enumCalculations.cnEasterMonday: {
			        dReturn = EasterSunday(iYear).AddDays(1);
			        break;
                }
                default: {
			        Internationalization.RaiseDefaultError(g_cClassName + "CalculateFloatingHoliday", Internationalization.enumInternationalizationValues.cnDeveloperMessages_DateMath_UnreconizedCalculateDayFunction, Cn.Data.Tools.MakeString(iCalculationEnum, ""), "");
			        dReturn = DateTime.MinValue;
			        break;
                }
            }

                //#### Return the above determined dReturn value to the caller
            return dReturn;
	    }


        //#######################################################################################################
        //# Private Functions
        //#######################################################################################################
        ///############################################################
        /// <summary>
        /// Retrieves a HolidayYear class populated with the passed year's defined holidays.
        /// </summary>
		/// <param name="iYear">Integer representing the required year.</param>
        /// <returns>HolidayYear class populated with the passed <paramref>iYear</paramref>'s defined holidays.</returns>
        ///############################################################
		/// <LastUpdated>October 7, 2005</LastUpdated>
	    private HolidayYear GetYearsHolidays(int iYear) {
	        HolidayYear oReturn = new HolidayYear(iYear);
		    DateTime dInsertDate;
		    string sCountry;
		    string sRegion;
		    int iIncrementBy;
		    int iIterations;
		    int iWeekInMonth;
		    int iMonthDay;
		    int iMonth;
		    int iTemp;
		    int i;
		    int j;
		    enumWeekDays eDay;
		    bool bByMonths;

                //#### Traverse the .Rows of the global g_oData
		    for (i = 0; i < g_oData.RowCount; i++) {
                    //#### Borrow the use of j to store the EffectiveYear from within the g_oData
			    j = Cn.Data.Tools.MakeInteger(g_oData.Value(i, "EffectiveYear"), -1);

                    //#### If the EffectiveYear is all (= -1) or for the passed iYear
			    if (j == -1 || j == iYear) {
                        //#### Set the local variables with the values from within the g_oData
				    sCountry = g_oData.Value(i, "Country");
				    sRegion = g_oData.Value(i, "Region");
				    iWeekInMonth = Cn.Data.Tools.MakeInteger(g_oData.Value(i, "WeekInMonth"), 1);
				    iMonthDay = Cn.Data.Tools.MakeInteger(g_oData.Value(i, "MonthDay"), 1);
				    iMonth = Cn.Data.Tools.MakeInteger(g_oData.Value(i, "HolidayMonth"), 1);
				    eDay = (enumWeekDays)Cn.Data.Tools.MakeInteger(g_oData.Value(i, "WeekDay"), (int)enumWeekDays.cnSunday);

				        //#### Borrow the use of iTemp to store the MakeInteger'd Frequency
				    iTemp = Cn.Data.Tools.MakeInteger(g_oData.Value(i, "Frequency"), -1);

                        //#### Determine the Frequency and process accordingly
                    switch (iTemp) {
                            //#### If this is a cnWeekly, set iIterations to ensure that all weeks for a given year are included
                        case (int)enumFrequencies.cnWeekly: {
					        iIncrementBy = 7;
					        iIterations = (52 + 4);
					        bByMonths = false;
                            break;
                        }
                            //#### If this is a cnFortnightly, setting iIterations to ensure that all weeks for a given year are included
                        case (int)enumFrequencies.cnFortnightly: {
					        iIncrementBy = 14;
					        iIterations = (26 + 2);
					        bByMonths = false;
                            break;
                        }
                            //#### If this is a cnMonthly
                        case (int)enumFrequencies.cnMonthly: {
					        iIncrementBy = 1;
					        iIterations = 12;
					        bByMonths = true;
                            break;
                        }
                            //#### If this is a cnQuarterly
                        case (int)enumFrequencies.cnQuarterly: {
					        iIncrementBy = 3;
					        iIterations = 4;
					        bByMonths = true;
                            break;
                        }
                            //#### If this is a cnTriAnnually
                        case (int)enumFrequencies.cnTriAnnually: {
					        iIncrementBy = 4;
					        iIterations = 3;
					        bByMonths = true;
                            break;
                        }
                            //#### If this is a cnSemiAnnually
                        case (int)enumFrequencies.cnSemiAnnually: {
					        iIncrementBy = 6;
					        iIterations = 2;
					        bByMonths = true;
                            break;
                        }
                            //#### If this is an cnAnnually
                        case (int)enumFrequencies.cnAnnually: {
					        iIncrementBy = 0;
					        iIterations = 1;
					        bByMonths = true;
                            break;
                        }
                            //#### Else set the local vars to the unknown status
                        default: {
					        iIncrementBy = 0;
					        iIterations = -1;
					        bByMonths = false;
                            break;
                        }
                    }

				        //#### Borrow the use of iTemp to store the MakeInteger'd DefinitionType
				    iTemp = Cn.Data.Tools.MakeInteger(g_oData.Value(i, "DefinitionType"), -1);

                        //#### Determine the DefinitionType and process accordingly
                    switch (iTemp) {
                            //#### If this is a cnStatic business holiday
                        case (int)enumDefinitionTypes.cnStatic: {
                                //#### Set dInsertDate to the serialized date data in the passed a_sHolidayCalculations
					        dInsertDate = new DateTime(iYear, iMonth, iMonthDay);

                                //#### If the dInsertDate is successfully set within the oReturn value (meaning dInsertDate was inside the passed iYear) and we have more iIterations to insert
					        if (oReturn.AddLocality(dInsertDate, sCountry, sRegion) && iIterations > 1) {
                                    //#### If we are supposed to jump bByMonths
						        if (bByMonths) {
                                        //#### Traverse the remainder of the iIterations (as the first was completed above)
							        for (j = 2; j <= iIterations; j++) {
                                            //#### Add iIncrementBy months onto the dInsertDate
								        dInsertDate = dInsertDate.AddMonths(iIncrementBy);

                                            //#### If the dInsertDate is not set within the return value, exit the inner for loop (as dInsertDate was outside of the passed iYear)
								        if (! oReturn.AddLocality(dInsertDate, sCountry, sRegion)) {
									        break;
								        }
							        }
						        }
                                    //#### Else we're supposed to jump by days
						        else {
                                        //#### Traverse the remainder of the iIterations (as the first was completed above)
							        for (j = 2; j <= iIterations; j++) {
                                            //#### Add iIncrementBy days onto the dInsertDate
								        dInsertDate = dInsertDate.AddDays(iIncrementBy);

                                            //#### If the dInsertDate is not set within the return value, exit the inner for loop (as dInsertDate was outside of the passed iYear)
								        if (! oReturn.AddLocality(dInsertDate, sCountry, sRegion)) {
									        break;
								        }
							        }
						        }
					        }
                            break;
                        }
                            //#### If this is a cnNthWeekDayInMonth business holiday
                        case (int)enumDefinitionTypes.cnNthWeekDayInMonth: {
                                //#### Set dInsertDate to the calculated NthWeekdayInMonth
					        dInsertDate = Dates.Tools.NthWeekdayInMonth(iYear, iMonth, iWeekInMonth, eDay);

                                //#### If the dInsertDate is successfully set within the return value (meaning dInsertDate was inside the passed iYear) and we have more iIterations to insert
					        if (oReturn.AddLocality(dInsertDate, sCountry, sRegion) && iIterations > 1) {
                                    //#### If we are supposed to jump bByMonths (which covers monthly, quarterly, semi-annually and annually)
						        if (bByMonths) {
                                        //#### Traverse the remainder of the iIterations (as the first was completed above)
							        for (j = 2; j <= iIterations; j++) {
                                            //#### Add iIncrementBy onto iMonth
								        iMonth += iIncrementBy;

                                            //#### If we have passed December, exit the inner for loop
								        if (iMonth > 12) {
									        break;
								        }
                                            //#### Else we still may be within iYear
								        else {
                                                //#### Recalculate the dInsertDate's NthWeekdayInMonth
									        dInsertDate = Cn.Dates.Tools.NthWeekdayInMonth(iYear, iMonth, iWeekInMonth, eDay);

                                                //#### If the dInsertDate is not set within the return value, exit the inner for loop (as dInsertDate was outside of the passed iYear)
									        if (! oReturn.AddLocality(dInsertDate, sCountry, sRegion)) {
										        break;
									        }
								        }
							        }
						        }
                                    //#### Else we're supposed to jump by days (which covers weekly and fortnightly)
						        else {
                                        //#### Traverse the remainder of the iIterations (as the first was completed above)
							        for (j = 2; j <= iIterations; j++) {
                                            //#### Add iIncrementBy days onto the dInsertDate
								        dInsertDate = dInsertDate.AddDays(iIncrementBy);

                                            //#### If the dInsertDate is not set within the return value, exit the inner for loop (as dInsertDate was outside of the passed iYear)
								        if (! oReturn.AddLocality(dInsertDate, sCountry, sRegion)) {
									        break;
								        }
							        }
						        }
					        }
                            break;
                        }
                            //#### If this is a cnCalculated business holiday
                        case (int)enumDefinitionTypes.cnCalculated: {
                                //#### Calculate dInsertDate by calling CalculateFloatingBusinessDay
					        dInsertDate = CalculateFloatingHoliday(Cn.Data.Tools.MakeInteger(g_oData.Value(i, "CalculatedDayFunction"), -1), iYear);

                                //#### Insert the dInsertDate into the return value, ignoring the return value (as CalculateFloatingBusinessDay only calculates for the passed iYear)
					        oReturn.AddLocality(dInsertDate, sCountry, sRegion);
                            break;
                        }
                    }
			    }
		    }

               //#### Return the above determined oReturn value to the caller
           return oReturn;
	    }

        ///############################################################
        /// <summary>
        /// Retrieves the date for Easter Sunday
        /// </summary>
        /// <remarks>
        /// Got this from http://www.erlandsendata.no/english/index.php?d=enfunctionsdateeaster.
        /// </remarks>
		/// <param name="iYear">Integer representing the required year.</param>
        /// <returns>DateTime representing Easter Sunday for the passed <paramref>iYear</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>October 7, 2004</LastUpdated>
	    private DateTime EasterSunday(int iYear) {
		    int i;
		    int iMoreThen48;

                //#### Run the calculation (yea, don't ask me what its doing...)
		    i = (((255 - 11 * (iYear % 19)) - 21) % 30) + 21;
		    iMoreThen48 = Cn.Data.Tools.MakeInteger((i > 48), 0);
		    return new DateTime(iYear, 3, 1).AddDays(i + iMoreThen48 + 6 - ((iYear + iYear / 4 + i + iMoreThen48 + 1) % 7));
	    }

        ///############################################################
        /// <summary>
        /// Determines if the passed date is a weekend day, taking into account the passed bIncludeWeekends (which basicially tells us to always return false).
        /// </summary>
        /// <param name="dDate">DateTime representing the date in question.</param>
        /// <param name="bIncludeWeekends">Boolean value signaling if weekends are to be counted as business days within the calculation.</param>
        /// <returns>Boolean value representing if the passed <paramref>dDate</paramref> is considered as a weekend day.</returns>
        ///############################################################
		/// <LastUpdated>June 22, 2004</LastUpdated>
	    private bool IsWeekend(DateTime dDate, bool bIncludeWeekends) {
		    int iDayOfWeek = Cn.Dates.Tools.DayOfWeek(dDate);

                //#### If we're not to bIncludeWeekends and iDayOfWeek is .cnSaturday or .cnSunday, return true, else return false
		    return (! bIncludeWeekends && (iDayOfWeek == (int)enumWeekDays.cnSaturday || iDayOfWeek == (int)enumWeekDays.cnSunday));
	    }
	#endregion


		///########################################################################################################################
		/// <summary>
		/// A child class of DateMath used to store a single year's calculated holidays.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview>August 10, 2005</LastFullCodeReview>
	    private class HolidayYear {
                //#### Declare the required private variables
		    private Hashtable h_dDates;
		    private int g_iYear;


            //##########################################################################################
            //# Class Functions
            //##########################################################################################
			///############################################################
            /// <summary>
            /// Initializes the class.
            /// </summary>
			/// <param name="iYear">Integer representing the required year.</param>
			///############################################################
			/// <LastUpdated>January 11, 2010</LastUpdated>
		    public HolidayYear(int iYear) {
					//#### Call .Reset to init the class vars
			    Reset(iYear);
		    }

			///############################################################
			/// <summary>
			/// Resets the class to its initilized state.
			/// </summary>
			///############################################################
			/// <LastUpdated>January 11, 2010</LastUpdated>
		    public void Reset(int iYear) {
		    		//#### Init the g_iYear to the passed iYear
		    	g_iYear = iYear;
		    	h_dDates = new Hashtable();
		    }


            //##########################################################################################
            //# Public Read-Only Properties
            //##########################################################################################
			///############################################################
            /// <summary>
            /// Gets an integer representing the year loaded into this instance.
            /// </summary>
            /// <value>Integer representing the year loaded into this instance.</value>
			///############################################################
			/// <LastUpdated>August 10, 2005</LastUpdated>
	        public int Year {
		        get {
			        return g_iYear;
		        }
	        }


            //##########################################################################################
            //# Public Functions
            //##########################################################################################
			///############################################################
            /// <summary>
            /// Retrieves the collection of localities for the provided date.
            /// </summary>
	        /// <param name="dDate">DateTime representing the date in question.</param>
			/// <returns>String array containing the localities for the passed <paramref>dDate</paramref> (i.e. - "CountryCode:RegionCode", where ":" is the defined SecondaryDelimiter).</returns>
			///############################################################
			/// <LastUpdated>August 11, 2005</LastUpdated>
		    public string[] Localities(DateTime dDate) {
				return Cn.Data.Tools.MakeString(h_dDates[dDate], "").Split(Settings.PrimaryDelimiter.ToCharArray());
		    }

			///############################################################
            /// <summary>
            /// Determines if the provided date is considered a holiday in any country/region pair.
            /// </summary>
	        /// <param name="dDate">DateTime representing the date in question.</param>
            /// <returns>Boolean value representing if the passed <paramref>dDate</paramref> is considered a holiday within this instance.</returns>
			///############################################################
			/// <LastUpdated>August 10, 2005</LastUpdated>
		    public bool HasDate(DateTime dDate) {
				return h_dDates.Contains(dDate);
		    }

			///############################################################
            /// <summary>
            /// Determines if the provided date is considered a holiday in the passed country/region pair.
            /// </summary>
	        /// <param name="dDate">DateTime representing the date in question.</param>
			/// <param name="sCountry">String representing the desired developer-defined country code.</param>
			/// <param name="sRegion">String representing the desired developer-defined region code.</param>
            /// <returns></returns>
			///############################################################
			/// <LastUpdated>August 17, 2005</LastUpdated>
		    public bool HasDate(DateTime dDate, string sCountry, string sRegion) {
		        bool bReturn = false;

                    //#### If the passed dDate is within h_dDates
				if (h_dDates.Contains(dDate)) {
					string[] a_sLocalities = Localities(dDate);
					int iSecondaryDelimiterLen = Settings.SecondaryDelimiter.Length;
					int iCurrentLen;
					int iCountryLen;
					int iRegionLen;
					int i;

                        //#### .Trim the passed sCountry/sRegion pair (as the a_sLocalities are all lowercase), and determine their .Lengths
					sCountry = sCountry.Trim();
					sRegion = sRegion.Trim();
					iCountryLen = sCountry.Length;
					iRegionLen = sRegion.Length;

                        //#### Traverse the a_sLocalities
					for (i = 0; i < a_sLocalities.Length; i++) {
					        //#### Determine the iCurrentLen for this loop
					    iCurrentLen = a_sLocalities[i].Length;

                            //#### If the iCurrentLen is identifying dDate as a global date (i.e. - it's only a .SecondaryDelimiter)
						if (iCurrentLen == iSecondaryDelimiterLen) {
                                //#### Set our bReturn value to true and exit the loop
							bReturn = true;
							break;
						}
						    //#### Else if the current locality is identifying dDate as a country-wide date (i.e. - it's only the passed sCountry and a .SecondaryDelimiter) (checking their .Lenghts first as that is a far faster comparison)
						else if (iCurrentLen == (iCountryLen + iSecondaryDelimiterLen) && a_sLocalities[i] == sCountry + Settings.SecondaryDelimiter) {
                                //#### Set our bReturn value to true and exit the loop
							bReturn = true;
							break;
						}
                            //#### Else if the current a_sDateRegion matches the passed sCountry/sRegion pair (checking their .Lenghts first as that is a far faster comparison)
						else if (iCurrentLen == (iCountryLen + iSecondaryDelimiterLen + iRegionLen) && a_sLocalities[i] == sCountry + Settings.SecondaryDelimiter + sRegion) {
                                //#### Set our bReturn value to true and exit the loop
							bReturn = true;
							break;
						}
					}
				}

				    //#### Return the above determined bReturn value to the caller
				return bReturn;
		    }

			///############################################################
            /// <summary>
            /// Sets the provided country/region pair as a locality for the passed date.
            /// </summary>
	        /// <param name="dDate">DateTime representing the date in question.</param>
			/// <param name="sCountry">String representing the desired developer-defined country code.</param>
			/// <param name="sRegion">String representing the desired developer-defined region code.</param>
            /// <returns>Boolean value indicating if the <paramref>sCountry</paramref>/<paramref>sRegion</paramref> locality pair was successfully set for the passed <paramref>dDate</paramref>.</returns>
			///############################################################
			/// <LastUpdated>October 7, 2005</LastUpdated>
		    public bool AddLocality(DateTime dDate, string sCountry, string sRegion) {
		        bool bReturn;

                    //#### .Trim the passed sCountry and sRegion
			    sCountry = sCountry.Trim();
			    sRegion = sRegion.Trim();

                    //#### If the passed dDate is already within h_dDates
			    if (h_dDates.Contains(dDate)) {
                        //#### Append the passed sCountry/sRegion onto the end of the dDate's list and set our bReturn value to true
//####     NOTE: Duplicate sCountry/sRegion pairs (although stupid and unnecessary) are not checked for here as their presence within the set list is a minor issue, only effecting the HasDate() check above with more checks should duplicated Country/Region pairs appear before the Country/Region being searched for. Besides, the developer shouldn't define duplicate entries!
				    h_dDates[dDate] = Cn.Data.Tools.MakeString(h_dDates[dDate], "") + Settings.PrimaryDelimiter + sCountry + Settings.SecondaryDelimiter + sRegion;
				    bReturn = true;
			    }
                    //#### Else if the .Year within dDate matches our own g_iYear
			    else if (dDate.Year == g_iYear) {
                        //#### dDate is a new key for h_dDates, so add it in now and set the return value to true
				    h_dDates.Add(dDate, sCountry + Settings.SecondaryDelimiter + sRegion);
				    bReturn = true;
			    }
                    //#### Else the passed dDate was outside of our Year, so return false
			    else {
				    bReturn = false;
			    }

				    //#### Return the above determined bReturn value to the caller
				return bReturn;
		    }

	    } //# class HolidayYear

    } //# class DateMath

} //# namespace Cn
