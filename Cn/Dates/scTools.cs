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
using Cn.Data;		                                //# Required to access the Picklists class
using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Dates {

        //#### Declare the required public eNums
    #region eNums
		/// <summary>Days of the week.</summary>
    public enum enumWeekDays : int {
			/// <summary>Sunday.</summary>
	    cnSunday = 1,
			/// <summary>Monday.</summary>
	    cnMonday = 2,
			/// <summary>Tuesday.</summary>
	    cnTuesday = 3,
			/// <summary>Wednesday.</summary>
	    cnWednesday = 4,
			/// <summary>Thursday.</summary>
	    cnThursday = 5,
			/// <summary>Friday.</summary>
	    cnFriday = 6,
			/// <summary>Saturday.</summary>
	    cnSaturday = 7
    }
		/// <summary>Defined week of year (week number) calculations.</summary>
    public enum enumWeekOfYearCalculations : int {
			/// <summary>Default week of year (week number) calculation, which logicially equates to <c>cnISO8601</c>.</summary>
		cnDefault = 0,
			/// <summary>ISO 8601 week numbers (a.k.a. - the 4 day rule).</summary>
			/// <remarks>
			/// ISO 8601 defines the Week as always starting with Monday being Day 1 and finishing with Sunday being Day 7. Therefore, the days of a single ISO Week can be in two different Calendar Years; and, because a Calendar Year has one or two more than 52×7 days, an ISO Year has either 52 or 53 Weeks.
			/// <para/>Thus the ISO 8601 Week Numbers of a year are 01 to 52 or 53, which does not include zero. Part of Week 01 may be in the previous Calendar Year; part of Week 52 may be in the following Calendar Year; if a year has a Week 53, part of that week must be in the following Calendar Year. On average, six times out of seven, adjacent Dec 31st and Jan 1st are in the same Week. (quoted from: http://www.merlyn.demon.co.uk/weekinfo.htm#WkNo)
			/// </remarks>
		cnISO8601 = 1,
			/// <summary>Absolute week numbers (a.k.a - the Jan 1 rule).</summary>
			/// <remarks>
			/// An absolute week number is the 7 day period that a date falls within, based solely on the first day of the year, regardless of the day of the week.
			/// <para/>Week 1 is always Jan-1 to Jan-7, week 2 is always Jan-8 to Jan-14, and so on.  If the year begins on a Thursday, then each "week" is from Thursday to the following Wednesday. The absolute week number will always be between 1 and 53.   Week 53 will have either one or two days, depending on whether the year is a leap year.   If the year is a not a leap year, week 53 will consist of one day: Dec-31.  If the year is a leap year, week 53 will consist of two days: Dec-30 and Dec-31. (quoted from: http://www.cpearson.com/excel/weeknum.htm)
			/// </remarks>
		cnAbsolute = 2,

			/// <summary>Simple week numbers, weeks beginning on Sunday.</summary>
			/// <remarks>
			/// January 1st is always within week 1, and there are either 53 or 54 weeks in a year. This functions just like Excel's WeekNum function.
			/// <para/>Week 1 is generally less than 7 days; and, about once per 28 years, there is a Week 54 consisting of Sunday Dec 31st, 2000 being one such year. (quoted from: http://www.merlyn.demon.co.uk/weekinfo.htm#WkNo)
			/// </remarks>
		cnSimple_Sunday = 101,
			/// <summary>Simple week numbers, weeks beginning on Monday.</summary>
			/// <remarks>
			/// See the simple week number <see href="Cn.DateMath.enumWeekOfYearCalculations.cnSimple_Sunday">remarks</see>.
			/// </remarks>
		cnSimple_Monday = 102,
			/// <summary>Simple week numbers, weeks beginning on Tuesday.</summary>
			/// <remarks>
			/// See the simple week number <see href="Cn.DateMath.enumWeekOfYearCalculations.cnSimple_Sunday">remarks</see>.
			/// </remarks>
		cnSimple_Tuesday = 103,
			/// <summary>Simple week numbers, weeks beginning on Wednesday.</summary>
			/// <remarks>
			/// See the simple week number <see href="Cn.DateMath.enumWeekOfYearCalculations.cnSimple_Sunday">remarks</see>.
			/// </remarks>
		cnSimple_Wednesday = 104,
			/// <summary>Simple week numbers, weeks beginning on Thurday.</summary>
			/// <remarks>
			/// See the simple week number <see href="Cn.DateMath.enumWeekOfYearCalculations.cnSimple_Sunday">remarks</see>.
			/// </remarks>
		cnSimple_Thursday = 105,
			/// <summary>Simple week numbers, weeks beginning on Friday.</summary>
			/// <remarks>
			/// See the simple week number <see href="Cn.DateMath.enumWeekOfYearCalculations.cnSimple_Sunday">remarks</see>.
			/// </remarks>
		cnSimple_Friday = 106,
			/// <summary>Simple week numbers, weeks beginning on Saturday.</summary>
			/// <remarks>
			/// See the simple week number <see href="Cn.DateMath.enumWeekOfYearCalculations.cnSimple_Sunday">remarks</see>.
			/// </remarks>
		cnSimple_Saturday = 107
    }
    #endregion


	///########################################################################################################################
	/// <summary>
	/// A collection of date calculation-related routines, including the calculation of Business Days based on the loaded holiday calculation data.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>August 10, 2005</LastFullCodeReview>
    public class Tools {
            //#### Declare the required private constants
	    private const string g_cClassName = "Cn.Dates.Tools.";


        //#######################################################################################################
        //# Public Static Functions
        //#######################################################################################################
        ///############################################################
        /// <summary>
        /// Formats the provided date based on the referenced format.
        /// </summary>
        /// <param name="sDate">String representing the date to format.</param>
        /// <param name="sFormat">String representing the format to apply to the provided date.</param>
        /// <param name="oInternationalization">Instance of a Cn.Configuration.Internationalization class that has been loaded with the revelent internationalization data.</param>
        /// <returns>String value containing the formatted <paramref>sDate</paramref> as defined within the passed <paramref>sFormat</paramref> and <paramref>sLanguageCode</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
	    public static string FormatDateTime(string sDate, string sFormat, Internationalization oInternationalization) {
                //#### Force the passed sDate into a Date datatype (defaulting to .MinValue if the conversion fails) and pass the call off to .DoFormatDateTime
		    return DoFormatDateTime("FormatDateTime", Data.Tools.MakeDate(sDate, DateTime.MinValue), sFormat, oInternationalization, enumWeekOfYearCalculations.cnDefault);
	    }

        ///############################################################
        /// <summary>
        /// Formats the provided date based on the referenced format.
        /// </summary>
        /// <param name="dDate">DateTime representing the date to format.</param>
        /// <param name="sFormat">String representing the format to apply to the provided date.</param>
        /// <param name="oInternationalization">Instance of a Cn.Configuration.Internationalization class that has been loaded with the revelent internationalization data.</param>
        /// <returns>String value containing the formatted <paramref>dDate</paramref> as defined within the passed <paramref>sFormat</paramref> and <paramref>sLanguageCode</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
	    public static string FormatDateTime(DateTime dDate, string sFormat, Internationalization oInternationalization) {
                //#### Pass the passed arguments off to .DoFormatDateTime
		    return DoFormatDateTime("FormatDateTime", dDate, sFormat, oInternationalization, enumWeekOfYearCalculations.cnDefault);
	    }

        ///############################################################
        /// <summary>
        /// Formats the provided date based on the referenced format.
        /// </summary>
        /// <param name="sDate">String representing the date to format.</param>
        /// <param name="sFormat">String representing the format to apply to the provided date.</param>
        /// <param name="oInternationalization">Instance of a Cn.Configuration.Internationalization class that has been loaded with the revelent internationalization data.</param>
		/// <param name="eWeekOfYearCalculation">Enumeration representing which week of year (week number) calculation to utilize.</param>
        /// <returns>String value containing the formatted <paramref>sDate</paramref> as defined within the passed <paramref>sFormat</paramref> and <paramref>sLanguageCode</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
	    public static string FormatDateTime(string sDate, string sFormat, Internationalization oInternationalization, enumWeekOfYearCalculations eWeekOfYearCalculation) {
                //#### Force the passed sDate into a Date datatype (defaulting to .MinValue if the conversion fails) and pass the call off to .DoFormatDateTime
		    return DoFormatDateTime("FormatDateTime", Data.Tools.MakeDate(sDate, DateTime.MinValue), sFormat, oInternationalization, eWeekOfYearCalculation);
	    }

        ///############################################################
        /// <summary>
        /// Formats the provided date based on the referenced format.
        /// </summary>
        /// <param name="dDate">DateTime representing the date to format.</param>
        /// <param name="sFormat">String representing the format to apply to the provided date.</param>
        /// <param name="oInternationalization">Instance of a Cn.Configuration.Internationalization class that has been loaded with the revelent internationalization data.</param>
		/// <param name="eWeekOfYearCalculation">Enumeration representing which week of year (week number) calculation to utilize.</param>
        /// <returns>String value containing the formatted <paramref>dDate</paramref> as defined within the passed <paramref>sFormat</paramref> and <paramref>sLanguageCode</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
	    public static string FormatDateTime(DateTime dDate, string sFormat, Internationalization oInternationalization, enumWeekOfYearCalculations eWeekOfYearCalculation) {
                //#### Pass the passed arguments off to .DoFormatDateTime
		    return DoFormatDateTime("FormatDateTime", dDate, sFormat, oInternationalization, eWeekOfYearCalculation);
	    }

        ///############################################################
		/// <summary>
		/// Determines the Nth week day of the provided month/year.
		/// </summary>
        /// <remarks>
        /// Example #1: The second Wednesday in March - NthWeekdayInMonth(2005, 3, 2, enumWeekDays.cnWednesday)
        /// Example #2: The last Monday in June - NthWeekdayInMonth(2005, 6, -1, enumWeekDays.cnMonday)
        /// </remarks>
		/// <param name="iYear">Integer representing the required year.</param>
		/// <param name="iMonth">Integer representing the required month.</param>
		/// <param name="iNthWeek">Non-zero integer representing the required week.</param>
		/// <param name="eWeekday">Enumeration representing the required week day.</param>
        /// <returns>DateTime representing the the date of the <paramref>eWeekday</paramref> within the <paramref>iNthWeek</paramref> for the provided <paramref>iMonth</paramref>/<paramref>iYear</paramref>.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iNthWeek</paramref> is 0.</exception>
        ///############################################################
		/// <LastUpdated>August 10, 2005</LastUpdated>
	    public static DateTime NthWeekdayInMonth(int iYear, int iMonth, int iNthWeek, enumWeekDays eWeekday) {
		    DateTime dReturn;
		    int iInitialDay;

                //#### If the passed iNthWeek is 0, raise the error
            if (iNthWeek == 0) {
				Internationalization.RaiseDefaultError(g_cClassName + "NthWeekdayInMonth", Internationalization.enumInternationalizationValues.cnDeveloperMessages_DateMath_InvalidWeekInMonth, "NthWeekdayInMonth", Data.Tools.MakeString(iNthWeek, ""));
				dReturn = DateTime.MinValue;
            }
                //#### Else if the user-supplied iNthWeek is greater then 0 (meaning we count from the front of the month)
		    else if (iNthWeek > 0) {
                    //#### Init our dReturn value to the first of the iMonth and determine the iInitialDay
			    dReturn = new DateTime(iYear, iMonth, 1);
			    iInitialDay = DayOfWeek(dReturn);

                    //#### Reset iInitialDay to the required offset
			    iInitialDay = (Data.Tools.MakeInteger(eWeekday, 0) - iInitialDay);
			    if (iInitialDay < 0) {
				    iInitialDay += 7;
			    }

                    //#### Decrement iNthWeek in prep. of the calculation below
			    iNthWeek--;

                    //#### Set our dReturn value to dReturn + iNthWeeks and the offset
			    dReturn = dReturn.AddDays((iNthWeek * 7) + iInitialDay);
		    }
                //#### Else the user-supplied iNthWeek is less then or equal to 0 (meaning we count from the back of the month)
		    else {
                    //#### Init our dReturn value to the end of the iMonth and determine the iInitialDay
			    dReturn = new DateTime(iYear, iMonth, DaysInMonth(iMonth, iYear));
			    iInitialDay = DayOfWeek(dReturn);

                    //#### Reset iInitialDay to the required offset
			    iInitialDay = (iInitialDay - Data.Tools.MakeInteger(eWeekday, 0) + 7);
			    if (iInitialDay > 0) {
				    iInitialDay = (iInitialDay % 7);
			    }

                    //#### Decrement iNthWeek in prep. of the calculation below
			    iNthWeek = (System.Math.Abs(iNthWeek) - 1);

                    //#### Set our dReturn value to dReturn - iNthWeeks and the offset
			    dReturn = dReturn.AddDays((iNthWeek * -7) - iInitialDay);
		    }

		        //#### Return the above determined dreturn value to the caller
		    return dReturn;
	    }

        ///############################################################
        /// <summary>
        /// Determines the week day of the passed date.
        /// </summary>
        /// <remarks>
        /// NOTE: Required by C# as there is no "DatePart(.WeekDay)".
        /// </remarks>
        /// <param name="dDate">DateTime representing the date in question.</param>
        /// <returns>Enumeration representing the week day for the passed <paramref>dDate</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>December 2, 2005</LastUpdated>
        public static int DayOfWeek(DateTime dDate) {
            enumWeekDays eReturn;

                //#### Determine the .DayOfWeek of the passed dDate, setting our iReturn value accordingly
            switch (dDate.DayOfWeek) {
                case System.DayOfWeek.Sunday: {
                    eReturn = enumWeekDays.cnSunday;
                    break;
                }
                case System.DayOfWeek.Monday: {
                    eReturn = enumWeekDays.cnMonday;
                    break;
                }
                case System.DayOfWeek.Tuesday: {
                    eReturn = enumWeekDays.cnTuesday;
                    break;
                }
                case System.DayOfWeek.Wednesday: {
                    eReturn = enumWeekDays.cnWednesday;
                    break;
                }
                case System.DayOfWeek.Thursday: {
                    eReturn = enumWeekDays.cnThursday;
                    break;
                }
                case System.DayOfWeek.Friday: {
                    eReturn = enumWeekDays.cnFriday;
                    break;
                }
                default: { //case System.DayOfWeek.Saturday: {
                    eReturn = enumWeekDays.cnSaturday;
                    break;
                }
            }

                //#### Return the above determined eReturn value to the caller
            return (int)eReturn;
        }

        ///############################################################
        /// <summary>
        /// Retrieves the current Unix-style EPOCH timestamp.
        /// </summary>
        /// <remarks>
        /// NOTE: Can debug output at http://www.argmax.com/mt_blog/archive/000328.php?ts=1058415153
        /// </remarks>
        /// <returns>Double representing the current Unix-style EPOCH timestamp.</returns>
        ///############################################################
		/// <LastUpdated>December 2, 2005</LastUpdated>
        public static double Timestamp() {
                //#### Determine the TimeSpan between GMT's .UtcNow and the GMT EPOCH time (1977-1-1 00:00:00 GMT)
            TimeSpan oTS = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0));

                //#### Return the .TotalSeconds difference between the .UtcNow and the EPOCH
            return oTS.TotalSeconds;
        }

        ///############################################################
        /// <summary>
        /// Retrieves the current timezone offset (in seconds).
        /// </summary>
        /// <returns>Integer representing the the number of seconds between the current local time and the current GMT (UTC) time.</returns>
        ///############################################################
		/// <LastUpdated>December 2, 2005</LastUpdated>
        public static int TimeZoneOffset() {
                //#### Determine the oGMTOffset TimeSpan between the local .Now and .UtcNow, then return the .TotalSeconds of that difference
            TimeSpan oGMTOffset = (DateTime.Now - DateTime.UtcNow);
            return (int)oGMTOffset.TotalSeconds;
        }

        ///############################################################
        /// <summary>
        /// Determines the number of days in the provided month for the passed year.
        /// </summary>
        /// <remarks>
        /// NOTE: This function returns 0 if the passed <paramref>iMonth</paramref> is unreconized. <paramref>iMonth</paramref> should be within the following range: 1-12.
        /// </remarks>
		/// <param name="iYear">Integer representing the required year.</param>
		/// <param name="iMonth">Integer representing the required month.</param>
        /// <returns>Integer representing the number of days within the passed <paramref>iMonth</paramref>/<paramref>iYear</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>December 2, 2005</LastUpdated>
	    public static int DaysInMonth(int iMonth, int iYear) {
	        int iReturn;

                //#### Determine the passed iMonth and process accordingly
            switch (iMonth) {
                    //#### If iMonth is Jan, Mar, May, Jul, Aug, Oct or Dec
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12: {
                    iReturn = 31;
                    break;
                }
                    //#### If iMonth is Apr, Jun, Sep, or Nov
                case 4:
                case 6:
                case 9:
                case 11: {
                    iReturn = 30;
                    break;
                }
                    //#### If iMonth is Feb
                case 2: {
                        //#### If iYear is a leap year, return 29
			        if (IsLeapYear(iYear)) {
				        iReturn = 29;
			        }
                        //#### Else iYear is a standard year, so return 28
			        else {
				        iReturn = 28;
			        }
                    break;
                }
                    //#### Else the passed iMonth is invalid
                default: {
                    iReturn = 0;
                    break;
                }
            }

                //#### Return the above determined iReturn value to the caller
            return iReturn;
	    }

        ///############################################################
        /// <summary>
        /// Determines if the passed year is a leap year.
        /// </summary>
        /// <remarks>
        ///	The three rules which the Gregorian calendar uses to determine leap year are as follows:
        ///		1) Years divisible by four are leap years, unless...
        ///		2) Years also divisible by 100 are not leap years, except...
        ///		3) Years divisible by 400 are leap years.
        /// </remarks>
		/// <param name="iYear">Integer representing the required year.</param>
        /// <returns>Boolean value indicating if the passed <paramref>iYear</paramref> is a leap year.</returns>
        ///############################################################
		/// <LastUpdated>December 2, 2005</LastUpdated>
	    public static bool IsLeapYear(int iYear) {
	        bool bReturn;

                //#### If iYear is evenly divisible by 4, it might be a leap year
		    if ((iYear % 4) == 0) {
                    //#### If iYear is evenly divisible by 100, it might be a leap year
			    if ((iYear % 100) == 0) {
                        //#### If iYear is also evenly divisible by 400, it is a leap year (according to rule #3)
				    if ((iYear % 400) == 0) {
					    bReturn = true;
				    }
                        //#### Else iYear is not a leap year (according to rule #2)
				    else {
					    bReturn = false;
				    }
			    }
                    //#### Else iYear must be a leap year (according to rule #1 and #2)
			    else {
				    bReturn = true;
			    }
		    }
                //#### Else iYear is not a leap year as it is not evenly divisible by 4 (according to rule #1)
		    else {
			    bReturn = false;
		    }

		        //#### Return the above determined bReturn value to the caller
		    return bReturn;
	    }

        ///############################################################
		/// <summary>
		/// Determines the referenced week number for the given date.
		/// </summary>
		/// <param name="dDateTime">DateTime representing the date in question.</param>
		/// <param name="eCalculation">Enumeration representing which week of year (week number) calculation to utilize.</param>
		/// <returns>Array of 1-based integers where the first index represents the week number for the passed <paramref>dDateTime</paramref> and the second index represents the week number's associated year.</returns>
        ///############################################################
		/// <LastUpdated>March 2, 2007</LastUpdated>
        public static int[] WeekOfYear(DateTime dDateTime, enumWeekOfYearCalculations eCalculation) {
			int[] a_iReturn = new int[2];

				//#### Default the a_iReturn value's year (index 1) to the passed dDateTime's .Year
			a_iReturn[1] = dDateTime.Year;

				//#### Determine the eCalculation and process accordingly
			switch (eCalculation) {
					//#### If this is an .cnISO8601 (or .cnDefault) week number request, pass the call off to .WeekOfYear_ISO8601
				case enumWeekOfYearCalculations.cnDefault:
				case enumWeekOfYearCalculations.cnISO8601: {
					a_iReturn = WeekOfYear_ISO8601(dDateTime);
					break;
				}

					//#### If this is an .cnAbsolute week number request
				case enumWeekOfYearCalculations.cnAbsolute: {
						//#### Calculate the .cnAbsolute week number based on the rounded up Julian .DayOfYear (as .cnAbsolute week numbers are based on days since Jan 1st, irrespective of its week day)
					a_iReturn[0] = (int)System.Math.Ceiling( (decimal)(dDateTime.DayOfYear / 7) );
					break;
				}

				//##########
				//##########

					//#### If this is an .cnSimple_* week number request, pass the call off to .WeekOfYear_Simple
				case enumWeekOfYearCalculations.cnSimple_Sunday: {
						//#### Determine a_iReturn value's week (index 0)
					a_iReturn[0] = WeekOfYear_Simple(dDateTime, enumWeekDays.cnSunday);
					break;
				}
				case enumWeekOfYearCalculations.cnSimple_Monday: {
						//#### Determine a_iReturn value's week (index 0)
					a_iReturn[0] = WeekOfYear_Simple(dDateTime, enumWeekDays.cnMonday);
					break;
				}
				case enumWeekOfYearCalculations.cnSimple_Tuesday: {
						//#### Determine a_iReturn value's week (index 0)
					a_iReturn[0] = WeekOfYear_Simple(dDateTime, enumWeekDays.cnTuesday);
					break;
				}
				case enumWeekOfYearCalculations.cnSimple_Wednesday: {
						//#### Determine a_iReturn value's week (index 0)
					a_iReturn[0] = WeekOfYear_Simple(dDateTime, enumWeekDays.cnWednesday);
					break;
				}
				case enumWeekOfYearCalculations.cnSimple_Thursday: {
						//#### Determine a_iReturn value's week (index 0)
					a_iReturn[0] = WeekOfYear_Simple(dDateTime, enumWeekDays.cnThursday);
					break;
				}
				case enumWeekOfYearCalculations.cnSimple_Friday: {
						//#### Determine a_iReturn value's week (index 0)
					a_iReturn[0] = WeekOfYear_Simple(dDateTime, enumWeekDays.cnFriday);
					break;
				}
				case enumWeekOfYearCalculations.cnSimple_Saturday: {
						//#### Determine a_iReturn value's week (index 0)
					a_iReturn[0] = WeekOfYear_Simple(dDateTime, enumWeekDays.cnSaturday);
					break;
				}

			}

				//#### Return the above determined a_iReturn value to the caller
			return a_iReturn;
        }


        //#######################################################################################################
        //# Private Static Functions
        //#######################################################################################################
        ///############################################################
        /// <summary>
        /// Formats the provided date based on the referenced format.
        /// </summary>
        /// <remarks>
        /// The provided date format is specified using the constants listed below. The provided examples, which are presented directly after each constant in parenthesis, are based on the following date: "2005-January-5 14:45:05 GMT".
        /// <para/> * Year:
        /// <para/>     $YY (ex: 05)
        /// <para/>     $YYYY (ex: 2005)
        /// <para/> * Month:
        /// <para/>     $M (ex: 1)
        /// <para/>     $MM (ex: 01)
        /// <para/>     $MMM (ex: Jan)
        /// <para/>     $MMMM (ex: January)
        /// <para/> * Day:
        /// <para/>     $D (ex: 5)
        /// <para/>     $DD (ex: 5)
        /// <para/> * Day Suffix (i.e.: 1st, 2nd, 3rd):
        /// <para/>     $S (ex: th)
        /// <para/> * Day of Week (Sunday = 1 ... Saturday = 7):
        /// <para/>     $W (ex: 4)
        /// <para/>     $WWW (ex: Wed)
        /// <para/>     $WWWW (ex: Wednesday)
        /// <para/> * Day of Year:
        /// <para/>     $J (ex: 5)
        /// <para/>     $JJJ (ex: 005)
        /// <para/> * Week of Year (including its year, as required by ISO 8601 week numbers):
        /// <para/>     $w (ex: 1)
        /// <para/>     $ww (ex: 01)
        /// <para/>     $yy (ex: 05)
        /// <para/>     $yyyy (ex: 2005)
		/// <para/>         NOTE: These values are based on the week numbering scheme referenced by <paramref>eWeekOfYearCalculation</paramref>. There are a host of other week numbering schemes (see: http://www.merlyn.demon.co.uk/weekinfo.htm#WkNo, http://www.merlyn.demon.co.uk/weekcalc.htm#NIP and http://www.merlyn.demon.co.uk/js-dates.htm), many of which seem to be anything but "standard". If you require a different week numbering scheme, please contact Nick Campbeln via email: "renderer_weeknumbering@nick.campbeln.com".
        /// <para/> * UNIX Epoch Timestamp:
        /// <para/>     $E (ex: 1104936305)
        /// <para/> * 24 Hour:
        /// <para/>     $H (ex: 14)
        /// <para/>     $HH (ex: 14)
        /// <para/> * 12 Hour:
        /// <para/>     $h (ex: 2)
        /// <para/>     $hh (ex: 2)
        /// <para/> * Mederian (i.e.: am/pm):
        /// <para/>     $tt (ex: pm)
        /// <para/> * Minute:
        /// <para/>     $m (ex: 45)
        /// <para/>     $mm (ex: 45)
        /// <para/> * Second:
        /// <para/>     $s (ex: 5)
        /// <para/>     $ss (ex: 05)
        /// <para/> * Escaped dollar sign:
        /// <para/>     $$ (ex: $)
        /// </remarks>
		/// <param name="sFunction">String representing the calling function's name.</param>
        /// <param name="dDate">DateTime representing the date to format.</param>
        /// <param name="sFormat">String representing the format to apply to the provided date.</param>
        /// <param name="oInternationalization">Instance of a Cn.Configuration.Internationalization class that has been loaded with the revelent internationalization data.</param>
		/// <param name="eWeekOfYearCalculation">Enumeration representing which week of year (week number) calculation to utilize.</param>
        /// <returns>String value containing the formatted <paramref>dDate</paramref> as defined within the passed <paramref>sFormat</paramref> and <paramref>sLanguageCode</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>May 10, 2007</LastUpdated>
	    private static string DoFormatDateTime(string sFunction, DateTime dDate, string sFormat, Internationalization oInternationalization, enumWeekOfYearCalculations eWeekOfYearCalculation) {
			MultiArray oPicklistData;
			string[,] a_sDecoders = new string[28, 2];
			int[] a_iWeekOfYear;
			string sDollarPlaceHolder = "_DollarSign_";
			string sLanguageCode = oInternationalization.LanguageCode;
			string sReturn;
			int i;

                //#### If the passed dDate is a reconized date and we have a sFormat defined
//		    if (dDate != null && sFormat.Length > 0) {
		    if (sFormat.Length > 0 && dDate != DateTime.MinValue) {
                    //#### Borrow the use of i to store the month .Day, setting $D, $DD and $S accordingly
			    i = dDate.Day;
			    a_sDecoders[0, 0] = "$D";
			    a_sDecoders[0, 1] = Data.Tools.MakeString(i, "");
			    a_sDecoders[1, 0] = "$DD";
			    a_sDecoders[1, 1] = Data.Tools.LPad(i, "0", 2);
			    a_sDecoders[2, 0] = "$S";
				oPicklistData = oInternationalization.Values(Internationalization.enumInternationalizationPicklists.cnDate_MonthDaySuffix, sLanguageCode);
			    a_sDecoders[2, 1] = Picklists.Decoder(oPicklistData, Data.Tools.MakeString(i, "[DecodeFailed]"), true);

                    //#### Borrow the use of i to store the DayOfWeek, setting $W, $WWW and $WWWW according to the above determined .DayOfWeek in the borrowed i
                i = DayOfWeek(dDate);
			    a_sDecoders[3, 0] = "$W";
			    a_sDecoders[3, 1] = Data.Tools.MakeString(i, "");
			    a_sDecoders[4, 0] = "$WWW";
				oPicklistData = oInternationalization.Values(Internationalization.enumInternationalizationPicklists.cnDate_AbbreviatedWeekDayNames, sLanguageCode);
			    a_sDecoders[4, 1] = Picklists.Decoder(oPicklistData, Data.Tools.MakeString(i, "[DecodeFailed]"), true);
			    a_sDecoders[5, 0] = "$WWWW";
				oPicklistData = oInternationalization.Values(Internationalization.enumInternationalizationPicklists.cnDate_WeekDayNames, sLanguageCode);
			    a_sDecoders[5, 1] = Picklists.Decoder(oPicklistData, Data.Tools.MakeString(i, "[DecodeFailed]"), true);

                    //#### Borrow the use of i to store the .Month, setting $M, $MM, $MMM and $MMMM accordingly
			    i = dDate.Month;
			    a_sDecoders[6, 0] = "$M";
			    a_sDecoders[6, 1] = Data.Tools.MakeString(i, "");
			    a_sDecoders[7, 0] = "$MM";
			    a_sDecoders[7, 1] = Data.Tools.LPad(i, "0", 2);
			    a_sDecoders[8, 0] = "$MMM";
				oPicklistData = oInternationalization.Values(Internationalization.enumInternationalizationPicklists.cnDate_AbbreviatedMonthNames, sLanguageCode);
			    a_sDecoders[8, 1] = Picklists.Decoder(oPicklistData, Data.Tools.MakeString(i, "[DecodeFailed]"), true);
			    a_sDecoders[9, 0] = "$MMMM";
				oPicklistData = oInternationalization.Values(Internationalization.enumInternationalizationPicklists.cnDate_MonthNames, sLanguageCode);
			    a_sDecoders[9, 1] = Picklists.Decoder(oPicklistData, Data.Tools.MakeString(i, "[DecodeFailed]"), true);

					//#### Retrieve the a_iWeekOfYear, setting $w, $ww, $yy, and $yyyy accordingly
				a_iWeekOfYear = WeekOfYear(dDate, eWeekOfYearCalculation);
				a_sDecoders[10, 0] = "$w";
				a_sDecoders[10, 1] = Data.Tools.MakeString(a_iWeekOfYear[0], "");
				a_sDecoders[11, 0] = "$ww";
				a_sDecoders[11, 1] = Data.Tools.LPad(a_iWeekOfYear[0], "0", 2);

                    //#### Borrow the use of sReturn to store the MakeString'd year from a_iWeekOfYear and then borrow i to determine its .Length, finally setting $yy and $yyyy accordingly
                    //####     NOTE: We can borrow sReturn here as its value is reset below, so the value set below is ignored
			    sReturn = Data.Tools.MakeString(a_iWeekOfYear[1], "");
			    i = (sReturn.Length - 2);
			    if (i < 0) {
					i = 0;
			    }
			    a_sDecoders[12, 0] = "$yy";
			    a_sDecoders[12, 1] = Data.Tools.LPad(sReturn.Substring(i), "0", 2);
			    a_sDecoders[13, 0] = "$yyyy";
			    a_sDecoders[13, 1] = sReturn;

                    //#### Borrow the use of sReturn to store the MakeString'd .Year and then borrow i to determine its .Length, finally setting $YY and $YYYY accordingly
                    //####     NOTE: We can borrow sReturn here as its value is reset below, so the value set below is ignored
			    sReturn = Data.Tools.MakeString(dDate.Year, "");
			    i = (sReturn.Length - 2);
			    if (i < 0) {
					i = 0;
			    }
			    a_sDecoders[14, 0] = "$YY";
			    a_sDecoders[14, 1] = Data.Tools.LPad(sReturn.Substring(i), "0", 2);
			    a_sDecoders[15, 0] = "$YYYY";
			    a_sDecoders[15, 1] = sReturn;

                    //#### Borrow the use of i to store the .DayOfYear, setting $J and $JJJ accordingly
			    i = dDate.DayOfYear;
			    a_sDecoders[16, 0] = "$J";
			    a_sDecoders[16, 1] = Data.Tools.MakeString(i, "");
			    a_sDecoders[17, 0] = "$JJJ";
			    a_sDecoders[17, 1] = Data.Tools.LPad(i, "0", 3);

                    //#### Set $E based on the current Timestamp
                    //####     NOTE: Can debug output at http://www.argmax.com/mt_blog/archive/000328.php?ts=1058415153
			    a_sDecoders[18, 0] = "$E";
			    a_sDecoders[18, 1] = Data.Tools.MakeString(Timestamp(), "");

                    //#### Borrow the use of i to store the 24 .Hour, setting $HH and $H accordingly
			    i = dDate.Hour;
			    a_sDecoders[19, 0] = "$H";
			    a_sDecoders[19, 1] = Data.Tools.MakeString(i, "");
			    a_sDecoders[20, 0] = "$HH";
			    a_sDecoders[20, 1] = Data.Tools.LPad(i, "0", 2);

                    //#### If the .Hour within i is before noon, set $tt to "am"
			    a_sDecoders[21, 0] = "$tt";
				oPicklistData = oInternationalization.Values(Internationalization.enumInternationalizationPicklists.cnDate_Meridiem, sLanguageCode);
			    if ((i % 12) == i) {
					a_sDecoders[21, 1] = Picklists.Decoder(oPicklistData, "am", true);
			    }
                    //#### Else the .Hour within i is after noon, so set $tt to "pm"
			    else {
					a_sDecoders[21, 1] = Picklists.Decoder(oPicklistData, "pm", true);
			    }

                    //#### Determine the 12-hour time from the above collected .Hour (fixing 0 hours as necessary), then set $hh and $hh accordingly
			    i = (i % 12);
			    if (i == 0) {
				    i = 12;
			    }
			    a_sDecoders[22, 0] = "$h";
			    a_sDecoders[22, 1] = Data.Tools.MakeString(i, "");
			    a_sDecoders[23, 0] = "$hh";
			    a_sDecoders[23, 1] = Data.Tools.LPad(i, "0", 2);

                    //#### Borrow the use of i to store the .Minute, setting $m and $mm accordingly
			    i = dDate.Minute;
			    a_sDecoders[24, 0] = "$m";
			    a_sDecoders[24, 1] = Data.Tools.MakeString(i, "");
			    a_sDecoders[25, 0] = "$mm";
			    a_sDecoders[25, 1] = Data.Tools.LPad(i, "0", 2);

                    //#### Borrow the use of i to store the .Second, setting $s and $ss accordingly
			    i = dDate.Second;
			    a_sDecoders[26, 0] = "$s";
			    a_sDecoders[26, 1] = Data.Tools.MakeString(i, "");
			    a_sDecoders[27, 0] = "$ss";
			    a_sDecoders[27, 1] = Data.Tools.LPad(i, "0", 2);

				//##########
				//##########

                    //#### Ensure the sDollarPlaceHolder is not within the passed sFormat (modifying it as necessary to make it unique)
			    while (sFormat.IndexOf(sDollarPlaceHolder) > -1) {
				    sDollarPlaceHolder += "_";
			    }

                    //#### Replace any instances of $$ with the above determined sDollarPlaceHolder within sFormat, setting the result into the sReturn value
			    sReturn = sFormat.Replace("$$", sDollarPlaceHolder);

                    //#### Traverse the above defined a_sDecoders, replacing each key with it's above determined value within the sReturn value
                    //####     NOTE: a_sDecoders is traversed in reverse to because definitions are set from shortest to longest (i.e. - $M, $MM, $MMM, and $MMMM)
			    for (i = 27; i >= 0; i--) {
				    sReturn = sReturn.Replace(a_sDecoders[i, 0], a_sDecoders[i, 1]);
			    }

                    //#### Un-replace any insances of sDollarPlaceHolder within the return value with a plain ole $
			    sReturn = sReturn.Replace(sDollarPlaceHolder, "$");
		    }
                //#### Else the passed dDate is nothing or is otherwise unreconized, or the sFormat is a null-string, so set our sReturn value to a null-string
		    else {
			    sReturn = "";
		    }

		        //#### Return the above determined sReturn value to the caller
		    return sReturn;
	    }

		///############################################################
		/// <summary>
		/// Determines the simple week number (functionally equivlent to Excel's WeekNum) for the given date.
		/// </summary>
		/// <param name="dDateTime">DateTime representing the date in question.</param>
		/// <param name="eStartOfWeek">Enumeration representing the starting day of the week.</param>
		/// <returns>1-based integer representing the simple week number for the passed <paramref>dDateTime</paramref> based on the provided <paramref>eStartOfWeek</paramref>.</returns>
		///############################################################
		/// <LastUpdated>March 2, 2007</LastUpdated>
		private static int WeekOfYear_Simple(DateTime dDateTime, enumWeekDays eStartOfWeek) {
			int iDaysInFirstWeek;
			int iJan1 = DayOfWeek(new DateTime(dDateTime.Year, 1, 1));

				//#### If iJan1 was on the eStartOf(the)Week, set iDaysInFirstWeek to 7
				//####     NOTE: This logic is correct thanks to the +1 in the return line below. This accounts for the first week (even if, as in this case, the first week is a full week)
			if (iJan1 == (int)eStartOfWeek) {
				iDaysInFirstWeek = 7;
			}
				//#### Else iJan1 differs from the eStartOf(the)Week
			else {
					//#### Calculate the iDaysIn(the)FirstWeek based on the eStart(day)Of(the)Week less Jan 1st's .DayOfWeek (+7/%7 so that the result is positive and properly looped back around)
				iDaysInFirstWeek = (((int)eStartOfWeek - iJan1 + 7) % 7);
			}

				//#### Determine and return the .WeekOfYear_Simple based on the passed dDateTime's .DayOfYear, less the iDaysIn(the)FirstWeek +1 (to allow for the first week)
			return (int)System.Math.Ceiling( (decimal)((dDateTime.DayOfYear - iDaysInFirstWeek) / 7) ) + 1;
		}

		///############################################################
		/// <summary>
		/// Determines the ISO 8601 week number (also known as the 4 day rule) for the given date.
		/// </summary>
		/// <remarks>
		/// Based on code written by Simen Sandelien and provided (without license) at "http://konsulent.sandelien.no/VB_help/Week/".
		/// <para/>The .NET GetWeekOfYear() method will NOT produce correct ISO 8601 week numbers regardless of culture or CalendarWeekRule settings. Here is an explanation of the problem, as well as a custom method that does the right calculation.
		/// <para/>I [Simen Sandelien] recently encountered a situation where I needed my C#.NET application to display the correct week number associated with a date. Knowing that the different countries of the world have different rules associated with their calendars I assumed that this would be the perfect situation to bring in the new culture features of .NET. Here in Norway, and apparently in many other parts of Europe we use the so called FirstFourDayWeek rule to determine which week (at the beginning of the year) is supposed to be week number 1. In the USA they use the so called FirstDay rule to determine the same thing.  
		/// </remarks>
		/// <param name="dDateTime">DateTime representing the date in question.</param>
		/// <returns>Array of 1-based integers where the first index represents the ISO week number (1-53) for the passed date and the second index represents the ISO week number's associated year.</returns>
		///############################################################
		/// <LastUpdated>March 2, 2007</LastUpdated>
		private static int[] WeekOfYear_ISO8601(DateTime dDateTime) {
			// Updated April 12, 2006. Cleaned and refactored by Nick Campbeln

			// Updated 2004.09.27. Cleaned the code and fixed a bug. Compared the algorithm with
			// code published here . Tested code successfully against the other algorithm 
			// for all dates in all years between 1900 and 2100.
			// Thanks to Marcus Dahlberg for pointing out the deficient logic.

			// Calculates the ISO 8601 Week Number
			// In this scenario the first day of the week is monday, 
			// and the week rule states that:
			// [...] the first calendar week of a year is the one 
			// that includes the first Thursday of that year and 
			// [...] the last calendar week of a calendar year is 
			// the week immediately preceding the first 
			// calendar week of the next year.
			// The first week of the year may thus start in the 
			// preceding year
			#region Original Code
		/*
			const int JAN = 1;
			const int DEC = 12;
			const int LASTDAYOFDEC = 31;
			const int FIRSTDAYOFJAN = 1;
			const int THURSDAY = 4;
			bool ThursdayFlag = false;

			// Get the day number since the beginning of the year
			int DayOfYear = date.DayOfYear;

			// Get the numeric weekday of the first day of the 
			// year (using sunday as FirstDay)
			int StartWeekDayOfYear = 
				(int)(new DateTime(date.Year, JAN, FIRSTDAYOFJAN)).DayOfWeek;
			int EndWeekDayOfYear = 
				(int)(new DateTime(date.Year, DEC, LASTDAYOFDEC)).DayOfWeek;

			// Compensate for the fact that we are using monday
			// as the first day of the week
			if( StartWeekDayOfYear == 0)
				StartWeekDayOfYear = 7;
			if( EndWeekDayOfYear == 0)
				EndWeekDayOfYear = 7;

			// Calculate the number of days in the first and last week
			int DaysInFirstWeek = 8 - (StartWeekDayOfYear  );
			int DaysInLastWeek = 8 - (EndWeekDayOfYear );

			// If the year either starts or ends on a thursday it will have a 53rd week
			if (StartWeekDayOfYear == THURSDAY || EndWeekDayOfYear == THURSDAY)
				ThursdayFlag = true;

			// We begin by calculating the number of FULL weeks between the start of the year and
			// our date. The number is rounded up, so the smallest possible value is 0.
			int FullWeeks = (int) Math.Ceiling((DayOfYear - (DaysInFirstWeek))/7.0);
			
			int WeekNumber = FullWeeks;
				
			// If the first week of the year has at least four days, then the actual week number for our date
			// can be incremented by one.
			if (DaysInFirstWeek >= THURSDAY)
				WeekNumber = WeekNumber +1;

			// If week number is larger than week 52 (and the year doesn't either start or end on a thursday)
			// then the correct week number is 1.
			if (WeekNumber > 52 && !ThursdayFlag)
				WeekNumber = 1;

			// If week number is still 0, it means that we are trying to evaluate the week number for a
			// week that belongs in the previous year (since that week has 3 days or less in our date's year).
			// We therefore make a recursive call using the last day of the previous year.
			if (WeekNumber == 0)
				WeekNumber = WeekNumber_Entire4DayWeekRule(
					new DateTime(date.Year-1, DEC, LASTDAYOFDEC));
			return WeekNumber;
		*/
			#endregion

			int[] a_iReturn = new int[2];
			int iDaysInWeek1;
			int iJan1;
			int iDec31;
			const int cTHURSDAY = 4;

				//#### Default the a_iReturn value's year (index 1) to the passed dDateTime's .Year
			a_iReturn[1] = dDateTime.Year;

				//#### Determine the iJan1 and iDec31 days of the week for the year (index 1)
				//####     NOTE: Sunday = 0 ... Saturday = 6
			iJan1 = (int)(new DateTime(a_iReturn[1], 1, 1).DayOfWeek);
			iDec31 = (int)(new DateTime(a_iReturn[1], 12, 31).DayOfWeek);

				//#### Convert iJan1 to 1-based and starting on Monday (as per ISO 8601)
				//####     NOTE: Due to the nature of the calculations below, we do not need to do this conversion on iDec31
				//####     NOTE: Monday = 1 ... Sunday = 7
			if (iJan1 == 0) {
				iJan1 = 7;
			}

				//#### Determine the number of iDaysInWeek1
				//####     NOTE: Since Monday is the start of the week (as per ISO 8601) and iJan1 was converted to conform to "Monday = 1 ... Sunday = 7" above, -8 is the correct means to calculate the iDaysInWeek1 (ex: 8 - 7 [Sun] = 1 day in that week)
			iDaysInWeek1 = (8 - iJan1);

				//#### Calculate the number of weeks (index 0) (NOT including the first week) between the start of the year (index 1) and the passed dDateTime
				//####     NOTE: Partial weeks are also counted thanks to the rounding up of .Ceiling
			a_iReturn[0] = (int)System.Math.Ceiling( (decimal)((dDateTime.DayOfYear - iDaysInWeek1) / 7) );

				//#### If the iDaysInWeek1 includes cTHURSDAY, include the first week in the a_iReturn value's week (index 0) (so inc iReturn by 1)
				//####     NOTE: Since "Monday = 1 ... Sunday = 7", the simple "greater than" calculation below works as required
			if (iDaysInWeek1 >= cTHURSDAY) {
				a_iReturn[0]++;
			}

				//#### If the week (index 0) has been calculated to 53 on a non-53 week year
				//####     NOTE: .Years with 53 weeks must either start or end on a cTHURSDAY, otherwise they would not have more then 364 days to push them into the 53rd week (as 364 / 7 == 52, so any year with 364 or fewer days has 52 weeks)
			if (a_iReturn[0] > 52 && iJan1 != cTHURSDAY && iDec31 != cTHURSDAY) {
					//#### Increment the year (index 1) and reset the week (index 0) to 1 (as the date is part of next year's week count)
				a_iReturn[1]++;
				a_iReturn[0] = 1;
			}

				//#### If the week (index 0) is still 0, then we are looking at the last week of the previous year
			if (a_iReturn[0] == 0) {
					//#### Recurse to calculate the week containing Dec31 for the previous year (index 1)
				a_iReturn = WeekOfYear_ISO8601(new DateTime(a_iReturn[1] - 1, 12, 31));
			}

				//#### Return the above determined a_iReturn value to the caller
			return a_iReturn;
		}

		#region Other WeekOfYear_ISO8601 Implementation
/*
        ///############################################################
		/// <summary>
		/// Determines the ISO 8601 week number for a given year.
		/// </summary>
		/// <remarks>
		/// This code was written by Siva Ram Mateti and provided (without license) at "http://www.thecodeproject.com/csharp/GregToISO.asp?msg=396665".
		/// </remarks>
		/// <param name="dt">DateTime representing the date in question.</param>
		/// <returns>1-based integer representing the ISO week number (1-53) for the passed date.</returns>
        ///############################################################
		/// <LastUpdated>January 16, 2005</LastUpdated>
		private static int ISOWeekNumber(DateTime dt) {
			// Set Year
			int yyyy=dt.Year;

			// Set Month
			int mm=dt.Month;
	        
			// Set Day
			int dd=dt.Day;

			// Declare other required variables
			int DayOfYearNumber;
			int Jan1WeekDay;
			int WeekNumber=0, WeekDay;
	    
			int i,j,k,l,m,n;
			int[] Mnth = new int[12] {0,31,59,90,120,151,181,212,243,273,304,334};

			int YearNumber;
	        
			// Set DayofYear Number for yyyy mm dd
			DayOfYearNumber = dd + Mnth[mm-1];

			// Increase of Dayof Year Number by 1, if year is leapyear and month is february
			if ((IsLeapYear(yyyy) == true) && (mm == 2))
				DayOfYearNumber += 1;

			// Find the Jan1WeekDay for year 
			i = (yyyy - 1) % 100;
			j = (yyyy - 1) - i;
			k = i + i/4;
			Jan1WeekDay = 1 + (((((j / 100) % 4) * 5) + k) % 7);

			// Calcuate the WeekDay for the given date
			l= DayOfYearNumber + (Jan1WeekDay - 1);
			WeekDay = 1 + ((l - 1) % 7);

			// Find if the date falls in YearNumber set WeekNumber to 52 or 53
			if ((DayOfYearNumber <= (8 - Jan1WeekDay)) && (Jan1WeekDay > 4))
			{
				YearNumber = yyyy - 1;
				if ((Jan1WeekDay == 5) || ((Jan1WeekDay == 6) && (Jan1WeekDay > 4)))
					WeekNumber = 53;
				else
					WeekNumber = 52;
			}
			else
				YearNumber = yyyy;
	        

			// Set WeekNumber to 1 to 53 if date falls in YearNumber
			if (YearNumber == yyyy)
			{
				if (IsLeapYear(yyyy)==true)
					m = 366;
				else
					m = 365;
				if ((m - DayOfYearNumber) < (4-WeekDay))
				{
					YearNumber = yyyy + 1;
					WeekNumber = 1;
				}
			}
	        
			if (YearNumber==yyyy) {
				n=DayOfYearNumber + (7 - WeekDay) + (Jan1WeekDay -1);
				WeekNumber = n / 7;
				if (Jan1WeekDay > 4)
					WeekNumber -= 1;
			}

			return (WeekNumber);
		}
*/
		#endregion

    } //# class Tools

} //# namespace Cn.Date
