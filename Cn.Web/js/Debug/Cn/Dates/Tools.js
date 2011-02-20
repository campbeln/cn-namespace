/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined') {
	alert("Cn.Dates.Tools: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
else if (! Cn._.dt) {
	alert("Cn.Dates.Tools: [DEVELOPER] 'Cn/Data/Tools.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### If our namespace is not setup
if (! Cn.Dates) {
	Cn.Namespace("Cn.Dates");
}

	//#### Auto-include the Cn.Data.Tools class
//!Cn.Define("Cn._.dt");


//########################################################################################################################
//# DateTime class
//# 
//#	    NOTE: This class self ._Initilize
//#     Required Includes: Cn/Cn.js, Cn/Data/Tools.js
//########################################################################################################################
//# Last Code Review: April 11, 2006
Cn.Dates.Tools = Cn._.dst || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.dst = this;

		//#### Declare the required 'private' variables
	var	g_oConfigurationSettings = Cn._.cs,
		g_oIntlValues = Cn._.ci.Values,
		g_oIntlValue = Cn._.ci.Value,
		g_oDataTools = Cn._.dt,
		g_oThis = this
	;

		//#### Declare the required 'public' enums
	this.enumWeekDays = {					//# NOTE: The enumWeekDays directly relate to Cn.Dates's enumWeekDays enum. Any changes to these values must be reflected in the Cn.Dates's enums!
		cnSunday : 1,
		cnMonday : 2,
		cnTuesday : 3,
		cnWednesday : 4,
		cnThursday : 5,
		cnFriday : 6,
		cnSaturday : 7
	}
	this.enumWeekOfYearCalculations = {		//# NOTE: The enumWeekOfYearCalculations directly relate to Cn.Dates's enumWeekOfYearCalculations enum. Any changes to these values must be reflected in the Cn.Dates's enums!
		cnDefault : 0,
		cnISO8601 : 1,
		cnAbsolute : 2,
		cnSimple_Sunday : 100,
		cnSimple_Monday : 101,
		cnSimple_Tuesday : 102,
		cnSimple_Wednesday : 103,
		cnSimple_Thursday : 104,
		cnSimple_Friday : 105,
		cnSimple_Saturday : 106
	};


	//##########################################################################################
	//# DateMath-related 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Determines if the primary English language setting is American English
	//############################################################
	//# Last Updated: June 9, 2010
	this.AllowDDMMYYY = function() {
		var a_sLanguage = g_oConfigurationSettings.Language,  //['dk','en','en-us']
			sCurrentLanguage,
			bReturn = true,
			i
		;

			//#### Traverse the a_sLanguages (as reported by the browser)
		for (i = 0; i < a_sLanguage.length; i++) {
				//#### Collect the .MakeString'd .toLowerCase'd sCurrentLanguage for this loop
			sCurrentLanguage = g_oDataTools.MakeString(a_sLanguage[i], '').toLowerCase();

				//#### If this is the first English definition
			if (sCurrentLanguage.indexOf('en') == 0) {
					//#### If this is a definition for American-influenced English, re-flip our bReturn value to false
					//####     NOTE: MM/DD/YYYY format usage: http://en.wikipedia.org/wiki/Calendar_date; Language codes: http://msdn.microsoft.com/en-us/library/ms533052%28VS.85%29.aspx
                if (sCurrentLanguage.indexOf('en-us') == 0 ||		// en-us = English (United States) + Palau, Micronesia
                    sCurrentLanguage.indexOf('en-ca') == 0 ||		// en-ca = English (Canada)
                    sCurrentLanguage.indexOf('en-ph') == 0 ||		// en-ph = English (Philippians)
                    sCurrentLanguage.indexOf('en-bz') == 0			// en-bz = English (Belize)
                ) {
					bReturn = false;
				}

					//#### Fall from the loop (as we've found the first English definition)
				break;
			}
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Extends the standard Date.parse functionality by allowing for DD/MM/YYYY formatted dates in non-US locales
	//############################################################
	//# Last Updated: June 9, 2010
	this.DateParseEx = function(sDateString) {
		var a_sMatches = null,
			dReturn = null
		;

			//#### If we are supposed to .AllowDDMMYYY dates
		if (this.AllowDDMMYYY()) {
				//#### Setup a oRegEx to locate "## ## ####" (allowing for any sort of delimiter except a '\n') then collect the a_sMatches from the passed sDateString
			var oRegEx = new RegExp("(([0-9]{2}|[0-9]{1})[^0-9]+?([0-9]{2}|[0-9]{1})[^0-9]+?([0-9]{4}))", "i");
			a_sMatches = oRegEx.exec(sDateString);
		}

			//#### If we were able to find a_sMatches for a non-American English "DD MM YYYY" formatted date
		if (a_sMatches != null) {
			var oRegEx = new RegExp(a_sMatches[1], "i");

				//#### .parse the sDateString, but replacing the "DD?MM?YYYY" with "YYYY/MM/DD" beforehand
				//####     NOTE: a_sMatches[0]=[Default]; a_sMatches[1]=DD?MM?YYYY; a_sMatches[2]=DD; a_sMatches[3]=MM; a_sMatches[4]=YYYY
			dReturn = Date.parse(g_oDataTools.MakeString(sDateString, '').replace(oRegEx, a_sMatches[4] + "/" + a_sMatches[3] + "/" + a_sMatches[2]));
		}
			//#### Else .parse the passed sDateString
		else {
			dReturn = Date.parse(sDateString);
		}

			//#### Return the above determined dReturn value to the caller
		return dReturn;
	};

	//############################################################
	//# Formats the provided date based on the referenced format.
	//############################################################
	//# Last Updated: January 13, 2010
	this.FormatDateTime = function(dDateTime, sFormat) {
		var a_sDecoders = new Array(),
			sDollarPlaceHolder = '_DollarSign_',
			oRegEx, sReturn, i
		;

			//#### Ensure the passed dDateTime is valid
			//####     NOTE: Due to the lack of a second argument in the .MakeDate call, the dDateTime will be defaulted to the current date/time if it is invalid
		dDateTime = g_oDataTools.MakeDate(dDateTime);

		//##########
		//##########

			//#### Borrow the use of i to store the month day, setting $D, $DD and $S accordingly
		i = dDateTime.getDate();
		a_sDecoders[0] = ['$D', i];
		a_sDecoders[1] = ['$DD', g_oDataTools.LPad(i, '0', 2)];
		a_sDecoders[2] = ['$S', g_oIntlValues.Date_MonthDaySuffix[i - 1]];

			//#### Borrow the use of i to store the day of the week, setting $W, $WWW and $WWWW according to the above determined .DayOfWeek in the borrowed i
		i = dDateTime.getDay();
		a_sDecoders[3] = ['$W', i];
		a_sDecoders[4] = ['$WWW', g_oIntlValues.Date_AbbreviatedWeekDayNames[i]];
		a_sDecoders[5] = ['$WWWW', g_oIntlValues.Date_WeekDayNames[i]];

			//#### Borrow the use of i to store the month, setting $M, $MM, $MMM and $MMMM accordingly
			//####     NOTE: .getMonth returns a 0-based month, hence the need to +1 on the number outputs below
		i = dDateTime.getMonth();
		a_sDecoders[6] = ['$M', i + 1];
		a_sDecoders[7] = ['$MM', g_oDataTools.LPad(i + 1, '0', 2)];
		a_sDecoders[8] = ['$MMM', g_oIntlValues.Date_AbbreviatedMonthNames[i]];
		a_sDecoders[9] = ['$MMMM', g_oIntlValues.Date_MonthNames[i]];

			//#### Borrow the use of oRegEx to store the .WeekOfYear, setting $w, $ww, $yy and $yyyy accordingly
		oRegEx = this.WeekOfYear(dDateTime, g_oIntlValue.Localization_Date_WeekOfYearCalculationEnum);
		a_sDecoders[10] = ['$w', oRegEx[0]];
		a_sDecoders[11] = ['$ww', g_oDataTools.LPad(oRegEx[0], '0', 2)];

			//#### Borrow the use of sReturn to store the MakeString'd .Year and then borrow i to determine its .Length, finally setting $yy and $yyyy accordingly
			//####    NOTE: We can borrow sReturn here as its value is reset below, so the value set below is ignored
		sReturn = String(oRegEx[1]);
		i = (sReturn.length - 2);
		if (i < 0) {
			i = 0;
		}
		a_sDecoders[12] = ['$yy', g_oDataTools.LPad(sReturn.substr(i), '0', 2)];
		a_sDecoders[13] = ['$yyyy', sReturn];

			//#### Borrow the use of sReturn to store the MakeString'd .getFullYear and then borrow i to determine its .Length, finally setting $YY and $YYYY accordingly
			//####    NOTE: We can borrow sReturn here as its value is reset below, so the value set below is ignored
		sReturn = String(dDateTime.getFullYear());
		i = (sReturn.length - 2);
		if (i < 0) {
			i = 0;
		}
		a_sDecoders[14] = ['$YY', g_oDataTools.LPad(sReturn.substr(i), '0', 2)];
		a_sDecoders[15] = ['$YYYY', sReturn];

			//#### Borrow the use of i to store the .DayOfYear, setting $J and $JJJ accordingly
		i = this.DayOfYear(dDateTime);
		a_sDecoders[16] = ['$J', i];
		a_sDecoders[17] = ['$JJJ', g_oDataTools.LPad(i, '0', 3)];

			//#### Set $E based on the current Timestamp (down converting the .getTime'd returned milliseconds into seconds)
			//####    NOTE: Can debug output at http://www.argmax.com/mt_blog/archive/000328.php?ts=1058415153
		a_sDecoders[18] = ['$E', parseInt(dDateTime.getTime() / 1000)];

			//#### Borrow the use of i to store the 24 hour, setting $HH and $H accordingly
		i = dDateTime.getHours();
		a_sDecoders[19] = ['$H', i];
		a_sDecoders[20] = ['$HH', g_oDataTools.LPad(i, '0', 2)];

			//#### If the .Hour within i is before noon, set $tt to "am"
		if ((i % 12) == i) {
			a_sDecoders[21] = ['$tt', g_oIntlValue.EndUserMessages_DateTime_AM];
		}
			//#### Else the .Hour within i is after noon, so set $tt to "pm"
		else {
			a_sDecoders[21] = ['$tt', g_oIntlValue.EndUserMessages_DateTime_PM];
		}

			//#### Determine the 12-hour time from the above collected .Hour (fixing 0 hours as necessary), then set $hh and $hh accordingly
		i = (i % 12);
		if (i == 0) {
			i = 12;
		}
		a_sDecoders[22] = ['$h', i];
		a_sDecoders[23] = ['$hh', g_oDataTools.LPad(i, '0', 2)];

			//#### Borrow the use of i to store the .Minute, setting $m and $mm accordingly
		i = dDateTime.getMinutes();
		a_sDecoders[24] = ['$m', i];
		a_sDecoders[25] = ['$mm', g_oDataTools.LPad(i, '0', 2)];

			//#### Borrow the use of i to store the .Second, setting $s and $ss accordingly
		i = dDateTime.getSeconds();
		a_sDecoders[26] = ['$s', i];
		a_sDecoders[27] = ['$ss', g_oDataTools.LPad(i, '0', 2)];

		//##########
		//##########

			//#### Ensure the sDollarPlaceHolder is not within the passed sFormat (modifying it as necessary to make it unique)
		while (sFormat.indexOf(sDollarPlaceHolder) > -1) {
			sDollarPlaceHolder = sDollarPlaceHolder + '_';
		}

			//#### Replace any instances of $$ with the above determined sDollarPlaceHolder within sFormat, setting the result into the sReturn value
		oRegEx = new RegExp('\\$\\$', 'g');
		sReturn = sFormat.replace(oRegEx, sDollarPlaceHolder);

			//#### Traverse the above defined a_sDecoders, replacing each key with it's above determined value within the sReturn value
			//####    NOTE: a_sDecoders is traversed in reverse to because definitions are set from shortest to longest (i.e. - $M, $MM, $MMM, and $MMMM)
//! a_sDecoders.length?
		for (i = 27; i >= 0; i--) {
			oRegEx = new RegExp('\\' + a_sDecoders[i][0], 'g');
			sReturn = sReturn.replace(oRegEx, a_sDecoders[i][1]);
		}

			//#### Un-replace any insances of sDollarPlaceHolder within the sReturn value with a plain ole $, returning the result to the caller
		oRegEx = new RegExp(sDollarPlaceHolder, 'g');
		return sReturn.replace(oRegEx, '\\$');
	};

	//############################################################
	//# Determines the week day of the passed date..
	//############################################################
	//# Last Updated: December 23, 2009
	this.DayOfWeek = function(dDate) {
			//#### Ensure the passed dDate is valid
		dDate = g_oDataTools.MakeDate(dDate);

			//#### Determine the enumWeekDay of the passed dDate, returning the result to the caller
			//####     NOTE: Since JavaScript's .getDay returns "Sun = 0 ... Sat = 6", all we must do to convert it into a proper enumWeekDays is to +1
		return dDate.getDay() + 1;
	};

	//############################################################
	//# Gets the day of the year for the provided date.
	//############################################################
	//# Last Updated: April 12, 2006
	this.DayOfYear = function(dDateTime) {
		var iReturn;

			//#### Ensure the passed dDateTime is valid
		dDateTime = g_oDataTools.MakeDate(dDateTime);

			//#### Retrieve the number of milliseconds different between the passed dDateTime and Jan 1st of the year it represents
		iReturn = (dDateTime.getTime() - new Date(dDateTime.getFullYear(), 0, 1, 0, 0, 0, 0).getTime());

			//#### Convert the above retrieved value from milliseconds into seconds (hence / 1000), then into whole (Math.ceil) days (60 seconds in a minute, 60 minutes in an hour and 24 hours in a day, hence 60 / 60 / 24) + 1 (as it is a 1-based calculation), returning the result to the caller
		return (Math.ceil(iReturn / 1000 / 60 / 60 / 24) + 1);
	};

	//############################################################
	//# Determines the referenced week number for the given date.
	//############################################################
	//# Last Updated: April 13, 2006
	this.WeekOfYear = function(dDateTime, eCalculation) {
		var a_iReturn = [0,0];

			//#### Ensure the passed dDateTime is valid
		dDateTime = g_oDataTools.MakeDate(dDateTime);

			//#### Default the a_iReturn value's year (index 1) to the passed dDateTime's .getFullYear
		a_iReturn[1] = dDateTime.getFullYear();

			//#### Determine the eCalculation and process accordingly
		switch (eCalculation) {
				//#### If this is an .cnISO8601 (or .cnDefault) week number request, pass the call off to .WeekOfYear_ISO8601
			case this.enumWeekOfYearCalculations.cnDefault:
			case this.enumWeekOfYearCalculations.cnISO8601: {
				a_iReturn = WeekOfYear_ISO8601(dDateTime);
				break;
			}

				//#### If this is an .cnAbsolute week number request
			case this.enumWeekOfYearCalculations.cnAbsolute: {
					//#### Calculate the .cnAbsolute week number based on the rounded up Julian .DayOfYear (as .cnAbsolute week numbers are based on days since Jan 1st, irrespective of its week day)
				a_iReturn[0] = Math.ceil(this.DayOfYear(dDateTime) / 7);
				break;
			}

			//##########
			//##########

				//#### If this is an .cnSimple_* week number request, pass the call off to .WeekOfYear_Simple
			case this.enumWeekOfYearCalculations.cnSimple_Sunday: {
					//#### Determine a_iReturn value's week (index 0)
				a_iReturn[0] = WeekOfYear_Simple(dDateTime, this.enumWeekDays.cnSunday);
				break;
			}
			case this.enumWeekOfYearCalculations.cnSimple_Monday: {
					//#### Determine a_iReturn value's week (index 0)
				a_iReturn[0] = WeekOfYear_Simple(dDateTime, this.enumWeekDays.cnMonday);
				break;
			}
			case this.enumWeekOfYearCalculations.cnSimple_Tuesday: {
					//#### Determine a_iReturn value's week (index 0)
				a_iReturn[0] = WeekOfYear_Simple(dDateTime, this.enumWeekDays.cnTuesday);
				break;
			}
			case this.enumWeekOfYearCalculations.cnSimple_Wednesday: {
					//#### Determine a_iReturn value's week (index 0)
				a_iReturn[0] = WeekOfYear_Simple(dDateTime, this.enumWeekDays.cnWednesday);
				break;
			}
			case this.enumWeekOfYearCalculations.cnSimple_Thursday: {
					//#### Determine a_iReturn value's week (index 0)
				a_iReturn[0] = WeekOfYear_Simple(dDateTime, this.enumWeekDays.cnThursday);
				break;
			}
			case this.enumWeekOfYearCalculations.cnSimple_Friday: {
					//#### Determine a_iReturn value's week (index 0)
				a_iReturn[0] = WeekOfYear_Simple(dDateTime, this.enumWeekDays.cnFriday);
				break;
			}
			case this.enumWeekOfYearCalculations.cnSimple_Saturday: {
					//#### Determine a_iReturn value's week (index 0)
				a_iReturn[0] = WeekOfYear_Simple(dDateTime, this.enumWeekDays.cnSaturday);
				break;
			}

		}

			//#### Return the above determined a_iReturn value to the caller
		return a_iReturn;
	};


	//##########################################################################################
	//# 'Private', Pseudo-'Static' Functions
	//# 
	//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
	//##########################################################################################
	//############################################################
	//# Determines the simple week number (functionally equivlent to Excel's WeekNum) for the given date.
	//############################################################
	//# Last Updated: January 13, 2010
	var WeekOfYear_Simple = function(dDateTime, eStartOfWeek) {
		var iDaysInFirstWeek,
			eJan1 = g_oThis.DayOfWeek(new Date(dDateTime.getFullYear(), 0, 1))
		;

			//#### If eJan1 was on the eStartOf(the)Week, set iDaysInFirstWeek to 7
			//####     NOTE: This logic is correct thanks to the +1 in the return line below. This accounts for the first week (even if, as in this case, the first week is a full week)
		if (eJan1 == eStartOfWeek) {
			iDaysInFirstWeek = 7;
		}
			//#### Else eJan1 differs from the eStartOf(the)Week
		else {
				//#### Calculate the iDaysIn(the)FirstWeek based on the eStart(day)Of(the)Week less Jan 1st's .DayOfWeek (+7/%7 so that the result is positive and properly looped back around)
			iDaysInFirstWeek = ((eStartOfWeek - eJan1 + 7) % 7);
		}

			//#### Determine and return the .WeekOfYear_Simple based on the passed dDateTime's .DayOfYear, less the iDaysIn(the)FirstWeek +1 (to allow for the first week)
		return Math.ceil((g_oThis.DayOfYear(dDateTime) - iDaysInFirstWeek) / 7) + 1;
	};

	//############################################################
	//# Determines the ISO 8601 week number (also known as the 4 day rule) for the given date.
	//############################################################
	//# Last Updated: December 23, 2009
	var WeekOfYear_ISO8601 = function(dDateTime) {
		var a_iReturn = [0,0],
			cTHURSDAY = 4,
			iDaysInWeek1, iJan1, iDec31
		;

			//#### Default the a_iReturn value's year (index 1) to the passed dDateTime's year
		a_iReturn[1] = dDateTime.getFullYear();

			//#### Determine the iJan1 and iDec31 days of the week for the year (index 1)
			//####     NOTE: Sunday = 0 ... Saturday = 6
			//####     NOTE: January = 0 ... December = 11
		iJan1 = new Date(a_iReturn[1], 0, 1).getDay();
		iDec31 = new Date(a_iReturn[1], 11, 31).getDay();

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
			//####     NOTE: Partial weeks are also counted thanks to the rounding up of .ceil
		a_iReturn[0] = Math.ceil((g_oThis.DayOfYear(dDateTime) - iDaysInWeek1) / 7);

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
			a_iReturn = g_oThis.WeekOfYear(new Date(a_iReturn[1] - 1, 11, 31));
		}

			//#### Return the above determined a_iReturn value to the caller
		return a_iReturn;
	};


}; //# Cn.Web.Inputs.DateTime
