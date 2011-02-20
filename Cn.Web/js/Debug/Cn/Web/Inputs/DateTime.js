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
	alert("Cn.Web.Inputs.DateTime: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
else if (! Cn._.dst) {
	alert("Cn.Web.Inputs.DateTime: [DEVELOPER] 'Cn/Dates/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.dt) {
	alert("Cn.Web.Inputs.DateTime: [DEVELOPER] 'Cn/Data/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.wt) {
	alert("Cn.Web.Inputs.DateTime: [DEVELOPER] 'Cn/Web/Tools.js' must be included before referencing this code.");
}

	//#### If all of the required YAHOO objects do not exist, popup the related error message
else if (typeof(YAHOO) == 'undefined') {
	alert("Cn.Web.Inputs.DateTime: [DEVELOPER] 'yui/YAHOO.js' must be included before referencing this code.");
}
else if (! YAHOO.widget || ! YAHOO.widget.Calendar) {
	alert("Cn.Web.Inputs.DateTime: [DEVELOPER] 'yui/Calendar.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Event) {
	alert("Cn.Web.Inputs.DateTime: [DEVELOPER] 'yui/Event.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### If our namespace is not setup
if (! Cn.Web || ! Cn.Web.Inputs) {
	Cn.Namespace("Cn.Web.Inputs");
}


//########################################################################################################################
//# DateTime class
//# 
//#	    NOTE: This class self ._Initilize
//#     Required Includes: Cn/Cn.js, Cn/Tools.js, Cn/Settings.js.*, Cn/Renderer/Form/DateTimeText.js.*, [yui/Yahoo.js], yui/Calendar.js, [yui/Event.js]
//########################################################################################################################
//# Last Code Review: April 11, 2006
Cn.Web.Inputs.DateTime = Cn._.wid || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wid = this;

		//#### Declare the required 'private' variables
	var g_oWebSettingsValue = Cn._.ws.Value,
		g_oElementWorkspace = Cn._.wj.ElementWorkspace,
		g_oDatesTools = Cn._.dst,
		g_oDataTools = Cn._.dt,
		g_oWebSettings = Cn._.ws;
		g_oWebTools = Cn._.wt,
		g_oIntlValues = Cn._.ci.Values,
		g_oIntlValue = Cn._.ci.Value,
		g_oThis = this
	;

		//#### Declare the required 'private' variables
	var g_oElement = null,
		g_sFormat = '',
		g_iHourInc = 0,
		g_iMinInc = 0,
		g_iSecInc = 0,
		g_bIs12Hour = false,
		g_bShowTime = false,
		g_bPadHour = false,
		g_bPadMin = false,
		g_bPadSec = false
	;

		//#### Declare the required 'public' enums
	this.enumInputTypes = {					//# NOTE: The enumInputTypes directly relate to Cn.Web.Input's enumInputTypes enum. Any changes to these values must be reflected in the Cn.Web.Input's enums!
		cnDefaultInput : 0,
		cnDate : 4,
		cnTime : 5
	  //cnDateTime : 6
	};

		//#### Declare the required 'private' 'constants'
	var g_cBASEID = g_oWebSettingsValue.DOMElementPrefix + 'Cn.Web.Inputs.DateTime',
		g_cDATEID = g_cBASEID + '.Date',
		g_cCALENDARID = g_cBASEID + '.Calendar',
		g_cTIMEID = g_cBASEID + '.Time',
		g_cHOURSID = g_cBASEID + '.Hours',
		g_cMINUTESID = g_cBASEID + '.Minutes',
		g_cSECONDSID = g_cBASEID + '.Seconds',
		g_cMERIDIEMID = g_cBASEID + '.Meridiem',
		g_cMINWIDTH = 150
	;

		//#### Procedural code
	g_oIntlValue.Localization_Date_WeekOfYearCalculationEnum = g_oDatesTools.enumWeekOfYearCalculations.cnDefault;


	//##########################################################################################
	//# 'Public' Properties
	//##########################################################################################
	//############################################################
	//# Declare the required 'public' (sub-class stub) properties
	//############################################################
	//# Last Updated: March 29, 2006
	this.Calendar = null;

	//############################################################
	//# Get/set a value representing if the calendar is to be closed on each date selection
	//############################################################
	//# Last Updated: April 6, 2010
	this.HideOnDateSelection = false;


	//##########################################################################################
	//# 'Public Read-Only' Properties
	//##########################################################################################
	//############################################################
	//# Gets the base id name for the rendered HTML elements.
	//############################################################
	//# Last Updated: June 8, 2006
	this.BaseID = function() {
		return g_cBASEID;
	};

	//############################################################
	//# Gets the hour increment value.
	//############################################################
	//# Last Updated: March 30, 2006
	this.HourInc = function() {
		return g_iHourInc;
	};

	//############################################################
	//# Gets the minute increment value.
	//############################################################
	//# Last Updated: March 30, 2006
	this.MinuteInc = function() {
		return g_iMinInc;
	};

	//############################################################
	//# Gets the second increment value.
	//############################################################
	//# Last Updated: March 30, 2006
	this.SecondInc = function() {
		return g_iSecInc;
	};


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Displays the DHTML input to the user as requested.
	//############################################################
	//# Last Updated: April 16, 2010
	this.Show = function(sInputID, sFormat, eInputType, iHourInc, iMinuteInc, iSecondInc) {
		var oWorkspace, dDate, sTemp, iH, iW, iX, iY;

			//#### If this is our first call, we need to .Initilize ourselves
		if (this.Calendar == null) {
			Initilize();
		}

			//#### Set the arguments into the 'private' class variables
		g_oElement = g_oWebTools.GetByID(sInputID);
		g_sFormat = String(sFormat);
		g_iHourInc = g_oDataTools.MakeNumeric(iHourInc, 1);
		g_iMinInc = g_oDataTools.MakeNumeric(iMinuteInc, 1);
		g_iSecInc = g_oDataTools.MakeNumeric(iSecondInc, 1);

			//#### Determine the value of the 'private' class varaibles based on the passed data
		eInputType = g_oDataTools.MakeNumeric(eInputType, this.enumInputTypes.cnDefaultInput);
		g_bShowTime = (eInputType != this.enumInputTypes.cnDate && eInputType != this.enumInputTypes.cnDefaultInput);
		g_bIs12Hour = (sFormat.indexOf('$h') != -1);
		g_bPadHour = (sFormat.toLowerCase().indexOf('$hh') != -1);
		g_bPadMin = (sFormat.indexOf('$mm') != -1);
		g_bPadSec = (sFormat.indexOf('$ss') != -1);

			//#### Ensure the g_oElement has a .ElementWorkspace object setup
		g_oElementWorkspace[g_oElement.id] = g_oElementWorkspace[g_oElement.id] || {};
		g_oElementWorkspace[g_oElement.id].Cal = g_oElementWorkspace[g_oElement.id].Cal || {};
		oWorkspace = g_oElementWorkspace[g_oElement.id].Cal;

			//#### If the g_oElement hasn't had its .SerialDate setup, default it now and .addListener for OnChange so it get's reset as it should
		if (g_oElement && ! g_oDataTools.IsDefined(oWorkspace.SerialDate)) {
			oWorkspace.SerialDate = '';
			YAHOO.util.Event.addListener(g_oElement, 'blur', EventHandler, g_oElement.id);
		}

            //#### Collect the current dDate from the g_oElement's .value (or if it's not a reconized date, use the current date/time)
        dDate = ParseSerialDate();

			//#### ._Setup(the)TimeDisplay based on the passed data, then .Set(the)DateTime from the .SerialDate and do an .Update
		SetupTimeDisplay(eInputType);
		this.SetDateTime(dDate);
        this.Update();

		//##########
		//##########

			//#### Determine the iX/iY coords of the g_oElement, then position the ._BaseID just below it
		iX = g_oWebTools.Left(g_oElement);
		iY = (g_oWebTools.Top(g_oElement) + g_oWebTools.Height(g_oElement) - 1);
		g_oWebTools.Left(g_cBASEID, iX);
		g_oWebTools.Top(g_cBASEID, iY);

			//#### Show (or in this case, 'display') and .Repaint the g_cCALENDARID so that the iW(idth) can be properly calculated below
		g_oWebTools.Style(g_cCALENDARID, 'display', '');
		g_oWebTools.Repaint(g_cCALENDARID);

			//#### Calculate and set the iW(idth) (ensuring it is at least the g_cMINWIDTH)
		iW = g_oWebTools.Width(g_cCALENDARID);
		if (iW < g_cMINWIDTH) { iW = g_cMINWIDTH; }

			//#### Set the .Width of the g_cBASEID, ensure that the g_cCALENDARID fills its position, then .Show the g_cBASEID
			//####     NOTE: Setting the g_cCALENDARID to 100% width is required for browers with issues properly rendering the CSS ([cough] IE [cough])
		g_oWebTools.Width(g_cBASEID, iW);
		g_oWebTools.Style(g_cCALENDARID, 'width', '100%');
		g_oWebTools.Show(g_cBASEID);

			//#### Determine the iH(eight) and iW(idth) of the now showing ._BaseID
		iH = g_oWebTools.Height(g_cBASEID);
		iW = g_oWebTools.Width(g_cBASEID);

			//#### If the ._BaseID is too far right, reset it to the right margin (or the left margin if the screen is really narrow, allowing a 1px buffer either way)
		if ((iX + iW) > (g_oWebTools.WindowWidth() + g_oWebTools.WindowScrollLeft())) {
			iX = ((g_oWebTools.WindowWidth() + g_oWebTools.WindowScrollLeft()) - iW - 1);
			if (iX < 1) { iX = 1; }
			g_oWebTools.Left(g_cBASEID, iX);
		}

			//#### If the ._BaseID is too far down the page, reset it to the bottom (or the top if the screen is really short, allowing a 1px buffer either way)
		if ((iY + iH) > (g_oWebTools.WindowHeight() + g_oWebTools.WindowScrollTop())) {
			iY = ((g_oWebTools.WindowHeight() + g_oWebTools.WindowScrollTop()) - iH - 1);
			if (iY < 1) { iY = 1; }
			g_oWebTools.Top(g_cBASEID, iY);
		}

			//#### If the user agent is either IE or Netscape 4
		if (g_oWebTools.IsIE() || g_oWebTools.IsNN4()) {
				//#### Ensure that any non-exempt troublesome z-index elements are hidden underneath rfCalendarDIV
				//####     NOTE: There is no need to get the .ChildrenOf for SELECT's because we know the ._BaseID doesn't have any
		  //g_oWebTools.ToggleOverlappingElements(g_cBASEID, ['SELECT'], g_oWebTools.ChildrenOf(g_cBASEID, ['SELECT']));
			g_oWebTools.ToggleOverlappingElements(g_cBASEID, ['SELECT'], null);
		}
	};

	//############################################################
	//# Hides the DHTML input from the user.
	//############################################################
	//# Last Updated: December 23, 2009
	this.Hide = function(bClear) {
			//#### If we are supposed to bClear the g_oElement, do so now
		if (bClear) {
			this.Clear();
		}

			//#### .Hide the ._BaseID (display'ing none on the .Date.Cal so it's properly hidden)
		g_oWebTools.Style(g_cCALENDARID, 'display', 'none');
		g_oWebTools.Hide(g_cBASEID);

			//#### If the user agent is either IE or Netscape 4
		if (g_oWebTools.IsIE() || g_oWebTools.IsNN4()) {
				//#### Ensure that any troublesome z-index elements that were underneath the ._BaseID are properly un-hidden (that's a word, right? =)
			g_oWebTools.ToggleOverlappingElements(g_cBASEID, ['SELECT'], null);
		}

			//#### Move the control back to 0,0 so that any scrollbars are properly reset
		g_oWebTools.Left(g_cBASEID, 0);
		g_oWebTools.Top(g_cBASEID, 0);
	};

	//############################################################
	//# Updates the value of this instance's related element to the currently defined date/time.
	//############################################################
	//# Last Updated: March 3, 2010
	this.Update = function() {
		var oWorkspace = g_oElementWorkspace[g_oElement.id],
			sSerialDate,
			dDateTime = this.GetDateTime(),
			bReturn = false
		;

			//#### If we have an g_oElement to .Update
		if (g_oElement && oWorkspace && oWorkspace.Cal) {
				//#### Determine the sSerialDate (inc'ing .getMonth by one to transform JavaScript's 0-11 month definition into the standard 1-12)
			sSerialDate = dDateTime.getFullYear() + ' ' +
				(dDateTime.getMonth() + 1) + ' ' +
				dDateTime.getDate() + ' ' +
				dDateTime.getHours() + ' ' +
				dDateTime.getMinutes() + ' ' +
				dDateTime.getSeconds();

				//#### Reset the oWorkspace to the .Cal object
			oWorkspace = oWorkspace.Cal;

				//#### If this is a new sSerialDate
			if (oWorkspace.SerialDate != sSerialDate) {
					//#### .Update the g_oElement's .value as well as the oWorkspace's .Value and .SerialDate (formating the dDateTime as we go), then flip the bReturn value to true (as we successfully updated the value)
					//####     NOTE: This intermediary .SerialDate is used so that we do not have to constantly parse the formatted date preset within the g_oElement's .value
				g_oElement.value = g_oDatesTools.FormatDateTime(dDateTime, g_sFormat);
				oWorkspace.Value = g_oElement.value;
				oWorkspace.SerialDate = sSerialDate;
				bReturn = true;

					//#### .focus on the g_oElement
				g_oElement.focus();
			}
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Clears the value of this instance's related element.
	//############################################################
	//# Last Updated: March 3, 2010
	this.Clear = function() {
		var oWorkspace = g_oElementWorkspace[g_oElement.id];

			//#### If we have an g_oElement to .Clear, .Clear its .value(s) and .SerialDate
		if (g_oElement && oWorkspace && oWorkspace.Cal) {
			g_oElement.value = '';
			oWorkspace.Cal.Value = '';
			oWorkspace.Cal.SerialDate = '';
		}
	};

	//############################################################
	//# Retrieves the current date/time displayed within this instance's control.
	//############################################################
	//# Last Updated: January 13, 2010
	this.GetDateTime = function() {
		var dReturn = g_oDataTools.MakeDate(this.Calendar.getSelectedDates()[0]),
			iHours
		;

			//#### If we are ._Show(ing the)Time
		if (g_bShowTime) {
				//#### If this ._Is(a)12Hour clock and we are in the .PM, add 12 onto the .Hours (while ensuring we never end up with 24)
			if (g_bIs12Hour && g_oWebTools.InnerHTML(g_cMERIDIEMID) == g_oIntlValue.EndUserMessages_DateTime_PM) {
				iHours = (g_oDataTools.MakeNumeric(g_oWebTools.InnerHTML(g_cHOURSID), 0) + 12);
				if (iHours == 24) { iHours = 12; }
			}
				//#### Else we can go with the .Hours as is, so set iHours accordingly
			else {
				iHours = g_oDataTools.MakeNumeric(g_oWebTools.InnerHTML(g_cHOURSID), 0);
				if (g_bIs12Hour && iHours == 12) { iHours = 0; }
			}

				//#### Set the time elements within the dReturn value based on what the user has specified (.(re)set(ting the)Milliseconds while we're at it)
			dReturn.setHours(iHours);
			dReturn.setMinutes(g_oDataTools.MakeNumeric(g_oWebTools.InnerHTML(g_cMINUTESID), 0));
			dReturn.setSeconds(g_oDataTools.MakeNumeric(g_oWebTools.InnerHTML(g_cSECONDSID), 0));
			dReturn.setMilliseconds(0);
		}
			//#### Else we are not ._Show(ing the)Time, so reset the time portion of the dReturn value to midnight
		else {
			dReturn.setHours(0);
			dReturn.setMinutes(0);
			dReturn.setSeconds(0);
			dReturn.setMilliseconds(0);
		}

			//#### Return the above determined dReturn value to the caller
		return dReturn;
	};

	//############################################################
	//# Sets the passed date and time into this instance's control.
	//############################################################
	//# Last Updated: April 11, 2006
	this.SetDateTime = function(dDateTime) {
			//#### Pass the call off to .DoSetDateTime, signaling it to update the date and time
		DoSetDateTime(dDateTime, true, true);
	};

	//############################################################
	//# Sets the passed date into this instance's control.
	//############################################################
	//# Last Updated: April 11, 2006
	this.SetDate = function(dDate) {
			//#### Pass the call off to .DoSetDateTime, signaling it to only update the date
		DoSetDateTime(dDate, true, false);
	};

	//############################################################
	//# Sets the passed time into this instance's control.
	//############################################################
	//# Last Updated: April 11, 2006
	this.SetTime = function(dTime) {
			//#### Pass the call off to .DoSetDateTime, signaling it to only update the time
		DoSetDateTime(dTime, false, true);
	};

	//############################################################
	//# Increments the hour.
	//############################################################
	//# Last Updated: March 30, 2006
	this.IncHour = function(oElement) {
			//#### If this._Is(a)12Hour clock, pass the call off to ._DoInc accordingly while returning its return value as our own
		if (g_bIs12Hour) {
			return DoInc(oElement, 1, 12, g_iHourInc, g_bPadHour);
		}
			//#### If it must be a 24 hour clock, so pass the call off to ._DoInc accordingly while returning its return value as our own
		else {
			return DoInc(oElement, 0, 23, g_iHourInc, g_bPadHour);
		}
	};

	//############################################################
	//# Increments the minute.
	//############################################################
	//# Last Updated: March 30, 2006
	this.IncMinute = function(oElement) {
			//#### Pass the call off to ._DoInc, returning its return value as our own
		return DoInc(oElement, 0, 59, g_iMinInc, g_bPadMin);
	};

	//############################################################
	//# Increments the second.
	//############################################################
	//# Last Updated: March 30, 2006
	this.IncSecond = function(oElement) {
			//#### Pass the call off to ._DoInc, returning its return value as our own
		return DoInc(oElement, 0, 59, g_iSecInc, g_bPadSec);
	};

	//############################################################
	//# Increments the meridiem.
	//############################################################
	//# Last Updated: January 13, 2010
	this.IncMeridiem = function(oElement) {
		var bReturn = (oElement && oElement.id);

			//#### If the passed oElement seemed valid
		if (bReturn) {
				//#### If the meridiem is currently in the afternoon, reset it to the morning
			if (g_oWebTools.InnerHTML(oElement) == g_oIntlValue.EndUserMessages_DateTime_PM) {
				g_oWebTools.InnerHTML(oElement, g_oIntlValue.EndUserMessages_DateTime_AM);
			}
				//#### Else the meridiem is currently in the morning (or unreconized), so reset it to the afternoon
			else {
				g_oWebTools.InnerHTML(oElement, g_oIntlValue.EndUserMessages_DateTime_PM);
			}

				//#### .Update the g_oElement with the new time
			this.Update();
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};


	//##########################################################################################
	//# 'Private', Pseudo-'Static' Functions
	//# 
	//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
	//##########################################################################################
	//############################################################
	//# Initilizes the class.
	//############################################################
	//# Last Updated: January 13, 2010
	var Initilize = function() {
		var oCal;

//# <DebugCode>
			//#### If our settings have not yet been setup within Cn.Configuration.Internationalization, popup the error
		if (! Cn._.ci || ! Cn._.ci.Values || ! Cn._.ci.Values.Date_MonthNames) {
//!			alert("Cn.Web.Inputs.DateTime.DateTime: [DEVELOPER] 'DateTime.js." + Cn._.wj.ServerSideScriptFileExtension + "' must be included before referencing this code.");
		}
		else {
//# </DebugCode>

			//#### .Render the DHTML, then reposition the g_cBASEID to 0,0
			//####     NOTE: The repositioning seems to be VERY important with reguard to properly positioning the .Calendar later on. I've got no idea why, but it's done for good pratices anyway.
		Render();
		g_oWebTools.Left(g_cBASEID, 0);
		g_oWebTools.Top(g_cBASEID, 0);

			//#### Setup the .Calendar with a new .Calendar_Core
		g_oThis.Calendar = new YAHOO.widget.Calendar(g_cCALENDARID + '.Calendar', g_cCALENDARID, {
//			iframe: false,
//			LOCALE_MONTHS: 'long',
			MONTHS_LONG: g_oIntlValues.Date_MonthNames,
//			LOCALE_WEEKDAYS: 'short',
			WEEKDAYS_SHORT: g_oIntlValues.Date_CalendarWeekDayNames,
			strings: { previousMonth: '', nextMonth: '', close: '' }
		} );
		oCal = g_oThis.Calendar;

			//#### Set the .Calendar's months and week days from our .Text, then hook our own DoOnSelect its the onSelect event
//		oCal.Options.LOCALE_MONTHS = g_oIntlValues.Date_MonthNames;
//		oCal.Options.LOCALE_WEEKDAYS = g_oIntlValues.Date_CalendarWeekDayNames;
		oCal.selectEvent.subscribe(DoOnSelect, oCal, true);

			//#### 
//		oCal.Style.CSS_CALENDAR = '';					//# Container table
//		oCal.Style.CSS_HEADER = '';						//# 
//		oCal.Style.CSS_HEADER_TEXT = '';				//# Calendar header
//		oCal.Style.CSS_FOOTER = '';						//# Calendar footer
//		oCal.Style.CSS_CELL = '';						//# Calendar day cell
//		oCal.Style.CSS_CELL_OOM = '';					//# Calendar OOM (out of month) cell
//		oCal.Style.CSS_CELL_SELECTED = '';				//# Calendar selected cell
//		oCal.Style.CSS_CELL_RESTRICTED = '';			//# Calendar restricted cell
//		oCal.Style.CSS_CELL_TODAY = '';					//# Calendar cell for today's date
//		oCal.Style.CSS_ROW_HEADER = '';					//# The cell preceding a row (used for week number by default)
//		oCal.Style.CSS_ROW_FOOTER = '';					//# The cell following a row (not implemented by default)
//		oCal.Style.CSS_WEEKDAY_CELL = '';				//# The cells used for labeling weekdays
//		oCal.Style.CSS_WEEKDAY_ROW = '';				//# The row containing the weekday label cells
//		oCal.Style.CSS_BORDER = '';						//# The border style used for the default UED rendering
//		oCal.Style.CSS_CONTAINER = '';					//# Special container class used to properly adjust the sizing and float
//		oCal.Style.CSS_NAV_LEFT = '';					//# Left navigation arrow
//		oCal.Style.CSS_NAV_RIGHT = '';					//# Right navigation arrow
//		oCal.Style.CSS_CELL_TOP = '';					//# Outlying cell along the top row
//		oCal.Style.CSS_CELL_LEFT = '';					//# Outlying cell along the left row
//		oCal.Style.CSS_CELL_RIGHT = '';					//# Outlying cell along the right row
//		oCal.Style.CSS_CELL_BOTTOM = '';				//# Outlying cell along the bottom row
//		oCal.Style.CSS_CELL_HOVER = '';					//# Cell hover style
//		oCal.Style.CSS_CELL_HIGHLIGHT1 = '';			//# Highlight color 1 for styling cells
//		oCal.Style.CSS_CELL_HIGHLIGHT2 = '';			//# Highlight color 2 for styling cells
//		oCal.Style.CSS_CELL_HIGHLIGHT3 = '';			//# Highlight color 3 for styling cells
//		oCal.Style.CSS_CELL_HIGHLIGHT4 = '';			//# Highlight color 4 for styling cells

//# <DebugCode>
		}
//# </DebugCode>
	};
	YAHOO.util.Event.addListener(window, 'load', Initilize, null, null);

	//############################################################
	//# Calendar on date select callback function.
	//############################################################
	//# Last Updated: April 6, 2010
	var DoOnSelect = function() {
			//#### .Update the g_oElement with the currently selected date, if it was not updated (i.e. - the same date was selected twice in a row) .Hide the control
			//####     NOTE: Due to scoping issues, we need to fully reference .Update and .Hide as "this." doesn't like life because only a reference to .DoOnSelect is passed into the .Calendar, and that reference doesn't seem to carry its context with it (weird!?!).
		if (! g_oThis.Update() || g_oThis.HideOnDateSelection) {
			g_oThis.Hide(false);
		}
	};

	//############################################################
	//# OnBlur event handler.
	//############################################################
	//# Last Updated: March 15, 2010
	var EventHandler = function(oEvent) {
		var sID = arguments[1],
			oWorkspace = Cn._.wj.ElementWorkspace[sID].Cal,
			sValue = Cn._.wt.GetByID(sID).value
		;

			//#### If the .Value is not equal to the sID's current .value, reset the oWorkspace's .Value and .SerialDate
		if (oWorkspace.Value != sValue) {
			oWorkspace.Value = sValue;
			oWorkspace.SerialDate = '';
		}

	};

	//############################################################
	//# Renders the DHTML Control.
	//############################################################
	//# Last Updated: June 18, 2010
	var Render = function() {
		var a_sImageNames = 'll,l,today,r,rr,x,clear,show'.split(/,/),
			oSpan = document.createElement('span'),
			oImage,
			sPath = g_oWebSettings.Value.UIDirectory + 'img/',
			sInnerHTML,
			sClose,
			i
		;

			//#### Preload the images
		for (i = 0; i < a_sImageNames; i++) {
			oImage = new Image();
			oImage.src = sPath + a_sImageNames[i] + '.gif';
		}

			//#### Setup the sClose code
		sClose = "<img src='" + sPath + "x.gif' onclick='Cn._.wid.Hide(false);' class='calNavButtons' style='float: right;' alt='" + g_oIntlValue.EndUserMessages_DateTime_Close + "' />" +
			"<img src='" + sPath + "clear.gif' onclick='Cn._.wid.Hide(true);' class='calNavButtons' style='float: right;' alt='" + g_oIntlValue.EndUserMessages_DateTime_Clear + "' />";

			//#### .Write out header portion of the control
			//####     NOTE: Since there is no valid way to define that the DIV uses both the CSS Cn and Cn.CalendarContainer classes, an outer SPAN is employed
		sInnerHTML = "<div id='" + g_cBASEID + "' class='CalendarContainer' style='visibility: hidden; position: absolute;'>";

			//#### .Write out the calendar input portion of the control
		sInnerHTML += "<div id='" + g_cDATEID + "' style='text-align: center;'>" +
				"<img src='" + sPath + "ll.gif' onclick='Cn._.wid.Calendar.previousYear();' class='calNavButtons' style='margin: 0px;' alt='" + g_oIntlValue.EndUserMessages_DateTime_PreviousYear + "' />" +
				"<img src='" + sPath + "l.gif' onclick='Cn._.wid.Calendar.previousMonth();' class='calNavButtons' alt='" + g_oIntlValue.EndUserMessages_DateTime_PreviousMonth + "' />" +
				"<img src='" + sPath + "today.gif' onclick='Cn._.wid.Calendar.select(Cn._.wid.Calendar.today); Cn._.wid.Hide(false);' class='calNavButtons' alt='" + g_oIntlValue.EndUserMessages_DateTime_Today + "' />" +
				"<img src='" + sPath + "r.gif' onclick='Cn._.wid.Calendar.nextMonth();' class='calNavButtons' alt='" + g_oIntlValue.EndUserMessages_DateTime_NextMonth + "' />" +
				"<img src='" + sPath + "rr.gif' onclick='Cn._.wid.Calendar.nextYear();' class='calNavButtons' alt='" + g_oIntlValue.EndUserMessages_DateTime_NextYear + "' />" +
				sClose +
				"<div id='" + g_cCALENDARID + "' style='float: left; clear: both; position: relative; padding-top: 3px;'></div>" +
			"</div>"
		;

			//#### .Write out the time input navigation portion of the control
		sInnerHTML +="<div id='" + g_cTIMEID + ".X'>" +
				sClose +
			"</div>"
		;

			//#### .Write out the time input portion of the control
			//####     NOTE: "text-align: center;" does not seem to want to center align the time table, hence the use of the aniquated <center> tags below
//		sInnerHTML += "<div id='" + g_cTIMEID + "' style='float: left; text-align: center; width: 100%; padding-top: 3px;'>" +
		sInnerHTML += "<div id='" + g_cTIMEID + "' style='float: left; width: 100%; padding-top: 3px;'>" +
"<center>" +
			"<table border='0' cellpadding='0' cellspacing='0' style='text-align: center;'><tr><td class='calcell' id='" + g_cHOURSID + ".TD'>" +
				"<a href='javascript:void(null);' id='" + g_cHOURSID + "' onMouseDown='Cn._.wid.IncHour(this);'>0</a>"
		;

			sInnerHTML += "</td><td style='width: 5px;' id='" + g_cMINUTESID + ".D'>" + g_oIntlValue.EndUserMessages_DateTime_Delimiter +
				"</td><td class='calcell' id='" + g_cMINUTESID + ".TD'>" +
					"<a href='javascript:void(null);' id='" + g_cMINUTESID + "' onMouseDown='Cn._.wid.IncMinute(this);'>0</a>"
			;

			sInnerHTML += "</td><td style='width: 5px;' id='" + g_cSECONDSID + ".D'>" + g_oIntlValue.EndUserMessages_DateTime_Delimiter +
				"</td><td class='calcell' id='" + g_cSECONDSID + ".TD'>" +
					"<a href='javascript:void(null);' id='" + g_cSECONDSID + "' onMouseDown='Cn._.wid.IncSecond(this);'>0</a>"
			;

			sInnerHTML += "</td><td style='width: 5px;' id='" + g_cMERIDIEMID + ".D'>&nbsp;" +
				"</td><td class='calcell' id='" + g_cMERIDIEMID + ".TD'>" +
					"<a href='javascript:void(null);' id='" + g_cMERIDIEMID + "' onMouseDown='Cn._.wid.IncMeridiem(this);'>-</a>" +
				"</td><td>" +
					"&nbsp;<img src='" + sPath + "now.gif' onClick='Cn._.wid.SetTime();' style='cursor: pointer;' alt='" + g_oIntlValue.EndUserMessages_DateTime_Now + "' />"
			;

			sInnerHTML += "</td></tr></table>" +
"</center>" +
				"</div>"
			;
	
			//#### .Write out footer portion of the control
			//####     NOTE: The nbsp is included so that Firefox properly renders the outer container div (else its height is wrong)
		sInnerHTML += "<div style='font-size: 1px;'>&nbsp;</div>" +
			"</div>"
		;

			//#### Set the oSpan's class, .innerHTML then .append(the)Child oSpan onto the .body
			//####     NOTE: We use the oSpan/.appendChild approach below so that the oSpan exists within the proper section of the document (else we couldn't be called on the first request as a document.write would be outside the .body tag)
		g_oWebTools.AddClass(oSpan, 'Cn');
		oSpan.innerHTML = sInnerHTML;
		document.body.appendChild(oSpan);
	};

	//############################################################
	//# Configures the time input portion based on the provided input type and format.
	//############################################################
	//# Last Updated: December 23, 2009
	var SetupTimeDisplay = function(eInputType) {
		var sDisplay, bRendered, bShow;

			//#### Determine the sDisplay for the .Date (based on if this is a .cnTime eInputType), setting .Date accordingly
		sDisplay = (eInputType == g_oThis.enumInputTypes.cnTime ? 'none' : '');
		g_oWebTools.Style(g_cDATEID, 'display', sDisplay);

			//#### Determine the sDisplay for the .Time.X (based on if this is a .cnTime eInputType), setting .Date accordingly
		sDisplay = (eInputType == g_oThis.enumInputTypes.cnTime ? '' : 'none');
		g_oWebTools.Style(g_cTIMEID + '.X', 'display', sDisplay);

			//#### Determine the sDisplay for the .Time (based on if this is a .cnDate or .cnDefaultInput eInputType), setting .Time accordingly
		sDisplay = (eInputType == g_oThis.enumInputTypes.cnDate || eInputType == g_oThis.enumInputTypes.cnDefaultInput ? 'none' : '');
		g_oWebTools.Style(g_cTIMEID, 'display', sDisplay);

			//#### If we are supposed to ._Show(the)Time
		if (g_bShowTime) {
				//#### Determine if we are to bShow the .Hours while defaulting bRendered to the value of bShow
			bShow = (g_sFormat.toLowerCase().indexOf('$h') != -1);
			bRendered = bShow;

				//#### Determine and set the sDisplay of the .Hours
			sDisplay = (bShow ? '' : 'none');
			g_oWebTools.Style(g_cHOURSID, 'display', sDisplay);
			g_oWebTools.Style(g_cHOURSID + '.TD', 'display', sDisplay);

			//##########
			//##########

				//#### Determine if we are to bShow the .Minutes
			bShow = (g_sFormat.indexOf('$m') != -1);

				//#### If we have bRendered something above and are supposed to bShow the .Minutes, set sDisplay and the .Minutes.D accordingly
			sDisplay = ((bRendered && bShow) ? '' : 'none');
			g_oWebTools.Style(g_cMINUTESID + '.D', 'display', sDisplay);

				//#### If we've not yet bRendered something above, set it based on bShow and determine the sDisplay for the .Minutes
			if (! bRendered) { bRendered = bShow; }
			sDisplay = (bShow ? '' : 'none');

				//#### Set the sDisplay of the .Minutes
			g_oWebTools.Style(g_cMINUTESID, 'display', sDisplay);
			g_oWebTools.Style(g_cMINUTESID + '.TD', 'display', sDisplay);

			//##########
			//##########

				//#### Determine if we are to bShow the .Seconds
			bShow = (g_sFormat.indexOf('$s') != -1);

				//#### If we have bRendered something above and are supposed to bShow the .Seconds, set sDisplay and the .Seconds.D accordingly
			sDisplay = ((bRendered && bShow) ? '' : 'none');
			g_oWebTools.Style(g_cSECONDSID + '.D', 'display', sDisplay);

				//#### If we've not yet bRendered something above, set it based on bShow and determine the sDisplay for the .Seconds
			if (! bRendered) { bRendered = bShow; }
			sDisplay = (bShow ? '' : 'none');

				//#### Set the sDisplay of the .Seconds
			g_oWebTools.Style(g_cSECONDSID, 'display', sDisplay);
			g_oWebTools.Style(g_cSECONDSID + '.TD', 'display', sDisplay);

			//##########
			//##########

				//#### Determine if we are to bShow the .Meridiem
			bShow = (g_sFormat.indexOf('$tt') != -1);

				//#### If we have bRendered something above and are supposed to bShow the .Meridiem, set sDisplay and the .Meridiem.D accordingly
			sDisplay = ((bRendered && bShow) ? '' : 'none');
			g_oWebTools.Style(g_cMERIDIEMID + '.D', 'display', sDisplay);

				//#### Determine the sDisplay and set the sDisplay of the .Meridiem
				//####     NOTE: Since this is the last element, we do not need to process bRendered
			sDisplay = (bShow ? '' : 'none');
			g_oWebTools.Style(g_cMERIDIEMID, 'display', sDisplay);
			g_oWebTools.Style(g_cMERIDIEMID + '.TD', 'display', sDisplay);
		}
	};

	//############################################################
	//# Parses the serial date of the current class element.
	//############################################################
	//# Last Updated: June 9, 2010
	var ParseSerialDate = function() {
		var oWorkspace = g_oElementWorkspace[g_oElement.id],
			a_sDate,
			dReturn = new Date()
		;

			//#### If g_oElement is valid
		if (g_oElement) {
				//#### If g_oElement has a defined .SerialDate
			if (oWorkspace && oWorkspace.Cal && oWorkspace.Cal.SerialDate != '') {
					//#### .split the g_oElement's .SerialDate into the a_sDate
				a_sDate = oWorkspace.Cal.SerialDate.split(' ');

					//#### If the a_sDate has the correct number of positions, set the dReturn value to a new date based on its' data (passing in 0 for the milliseconds)
					//####     NOTE: We decrement the month (index 1) as JavaScript uses 0-11 to define the months of the year (which is it's only aberrant date-related definition)
				if (a_sDate && a_sDate.length == 6) {
					dReturn = new Date(a_sDate[0], parseInt(a_sDate[1]) - 1, a_sDate[2], a_sDate[3], a_sDate[4], a_sDate[5], 0);
				}
			}
				//#### Else if the g_oElement has a .value, attempt to .Make(it a)Date
				//####     NOTE: This is the proper order of operations, as we should always check the .SerialDate first (which is reset to a null-string if the user manually edits the date)
			else if (g_oElement.value) {
				dReturn = g_oDataTools.MakeDate(g_oElement.value, dReturn);
			}
		}

			//#### Return the above determined dReturn value to the caller
		return dReturn;
	};

	//############################################################
	//# Increments the referenced time control element.
	//############################################################
	//# Last Updated: January 13, 2010
	var DoInc = function(oElement, iMinValue, iMaxValue, iIncrement, bZeroPad) {
		var bReturn = (oElement && oElement.id),
			oValue
		;

			//#### If the passed oElement seemed valid
		if (bReturn) {
				//#### If the iIncrement is 0 then this is an unused oElement of the time, so set the oValue to 0
			if (iIncrement == 0) {
				oValue = 0;
			}
				//#### Else we have a valid iIncrement
			else {
					//#### Collect the oValue from the passed oElement plus the iIncrement
				oValue = (g_oDataTools.MakeNumeric(g_oWebTools.InnerHTML(oElement), 0) + iIncrement);

					//#### If the above determined oValue is greater then the iMaxValue, reset it to the iMinValue
				if (oValue > iMaxValue) {
					oValue = iMinValue;
				}
			}

				//#### Ensure that the oValue is properly zero-padded and String-ified, then set it back into the oElement
			bZeroPad ? oValue = g_oDataTools.LPad(oValue, '0', 2) : oValue = String(oValue);
			g_oWebTools.InnerHTML(oElement, oValue);

				//#### .Update the g_oElement with the new time
			g_oThis.Update();
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Sets the passed date and time into this instance's control.
	//############################################################
	//# Last Updated: June 9, 2010
	var DoSetDateTime = function(dDateTime, bSetDate, bSetTime) {
			//#### Ensure the passed dDateTime is valid
		dDateTime = g_oDataTools.MakeDate(dDateTime, new Date());

			//#### If we are supposed to bSet(the)Time
		if (bSetTime) {
				//#### Determine the oHours, oMinutes and oSeconds, rounding down to the next asso. increment value
			var oHours = parseInt(dDateTime.getHours() / parseInt(g_iHourInc)) * g_iHourInc;
			var oMinutes = parseInt(dDateTime.getMinutes() / parseInt(g_iMinInc)) * g_iMinInc;
			var oSeconds = parseInt(dDateTime.getSeconds() / parseInt(g_iSecInc)) * g_iSecInc;

				//#### If this g_bIs(a)12Hour clock
			if (g_bIs12Hour) {
					//#### If oHours is noon or beyond
				if (oHours >= 12) {
						//#### Fix oHours as necessary and set the .Meridiem to .PM
					if (oHours > 12) { oHours = (oHours - 12); }
					g_oWebTools.InnerHTML(g_cMERIDIEMID, g_oIntlValue.EndUserMessages_DateTime_PM);
				}
					//#### Else we're still in the morning, so set the sMeridiem to .AM
				else {
					g_oWebTools.InnerHTML(g_cMERIDIEMID, g_oIntlValue.EndUserMessages_DateTime_AM);
				}
			}

				//#### Ensure that the oHours, oMinutes and oSeconds are properly zero-padded and String-ified
			g_bPadHour ? oHours = g_oDataTools.LPad(oHours, '0', 2) : oHours = String(oHours);
			g_bPadMin ? oMinutes = g_oDataTools.LPad(oMinutes, '0', 2) : oMinutes = String(oMinutes);
			g_bPadSec ? oSeconds = g_oDataTools.LPad(oSeconds, '0', 2) : oSeconds = String(oSeconds);

				//#### Update the .InnerHTML of the controls with the above determined data
			g_oWebTools.InnerHTML(g_cHOURSID, oHours);
			g_oWebTools.InnerHTML(g_cMINUTESID, oMinutes);
			g_oWebTools.InnerHTML(g_cSECONDSID, oSeconds);

				//#### If we are not also supposed to bSet(the)Date, .Update the g_oElement with the new time now
			if (! bSetDate) {
				g_oThis.Update();
			}
		}

			//#### If we are supposed to bSet(the)Date
			//####     NOTE: We set the date last do to .DoOnSelect's .Update call
		if (bSetDate) {
				//#### .deselectAll the current dates on the .Calendar
			g_oThis.Calendar.deselectAll();
			
				//#### .select the passed dDateTime, which is validated and returned if valid or reset to Today if not
			dDateTime = (g_oThis.Calendar.select(dDateTime)[0] || new Date());

				//#### Reset the .Calendar's month/year, then (re).render it (else the month is not changed)
			g_oThis.Calendar.setMonth(dDateTime.getMonth());
			g_oThis.Calendar.setYear(dDateTime.getFullYear());
//!
			g_oThis.Calendar.render();
		}
	};


}; //# Cn.Web.Inputs.DateTime
