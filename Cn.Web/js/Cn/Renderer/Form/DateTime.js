//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined') {
	alert("Cn.Renderer.Form.DateTime: [DEVELOPER] 'Cn/Cn.js' must be included before referencing this code.");
}
else if (! Cn.Tools) {
	alert("Cn.Renderer.Form.DateTime: [DEVELOPER] 'Cn/Tools.js' must be included before referencing this code.");
}
else if (! Cn.Settings) {
	alert("Cn.Renderer.Form.DateTime: [DEVELOPER] 'Cn/Settings.js.*' must be included before referencing this code.");
}

	//#### If all of the required YAHOO objects do not exist, popup the related error message
else if (typeof(YAHOO) == 'undefined') {
	alert("Cn.Renderer.Form.DateTime: [DEVELOPER] 'yui/YAHOO.js' must be included before referencing this code.");
}
else if (! YAHOO.widget || ! YAHOO.widget.Calendar) {
	alert("Cn.Renderer.Form.DateTime: [DEVELOPER] 'yui/Calendar.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Event) {
	alert("Cn.Renderer.Form.DateTime: [DEVELOPER] 'yui/Event.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### Else if our namespace is not setup
else if (! Cn.Renderer || ! Cn.Renderer.Form) {
	Cn.namespace("Cn.Renderer.Form");
}


//########################################################################################################################
//# DateTime class
//# 
//#	    NOTE: This class self .Initilizes
//#     Required Includes: Cn/Cn.js, Cn/Tools.js, Cn/Settings.js.*, Cn/Renderer/Form/DateTimeText.js.*, [yui/Yahoo.js], yui/Calendar.js, [yui/Event.js]
//########################################################################################################################
//# Last Code Review: April 11, 2006
Cn.Renderer.Form.DateTime = new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.dt = this;

		//#### Declare the required 'private' variables
	var g_oElement = null;
	var g_sFormat = '';
	var g_iHourInc = 0;
	var g_iMinInc = 0;
	var g_iSecInc = 0;
	var g_bIs12Hour = false;
	var g_bShowTime = false;
	var g_bPadHour = false;
	var g_bPadMin = false;
	var g_bPadSec = false;

		//#### Declare the required 'public' enums
	this.enumInputTypes = {					//# NOTE: The enumInputTypes directly relate to Renderer's enumInputTypes enum. Any changes to these values must be reflected in the Renderer's enums!
		cnDefaultInput : 0,
		cnDateInput : 4,
		cnTimeInput : 5,
		cnDateTimeInput : 6
	};
	this.enumWeekDays = {					//# NOTE: The enumWeekDays directly relate to Renderer's enumWeekDays enum. Any changes to these values must be reflected in the Renderer's enums!
		cnSunday : 1,
		cnMonday : 2,
		cnTuesday : 3,
		cnWednesday : 4,
		cnThursday : 5,
		cnFriday : 6,
		cnSaturday : 7
	}
	this.enumWeekOfYearCalculations = {		//# NOTE: The enumWeekOfYearCalculations directly relate to Renderer's enumWeekOfYearCalculations enum. Any changes to these values must be reflected in the Renderer's enums!
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

		//#### Declare the required 'private' 'constants'
	var g_cBASEID = Cn._.s.FormElementPrefix + 'Cn.Renderer.Form.DateTime';
	var g_cDATEID = g_cBASEID + '.Date';
	var g_cCALENDARID = g_cBASEID + '.Calendar';
	var g_cTIMEID = g_cBASEID + '.Time';
	var g_cHOURSID = g_cBASEID + '.Hours';
	var g_cMINUTESID = g_cBASEID + '.Minutes';
	var g_cSECONDSID = g_cBASEID + '.Seconds';
	var g_cMERIDIEMID = g_cBASEID + '.Meridiem';
	var g_cMINWIDTH = 150;


	//##########################################################################################
	//# 'Public Constructors'
	//##########################################################################################
	//############################################################
	//# Initilizes the referenced ComboBox.
	//############################################################
	//# Last Updated: May 8, 2006
	this.Initilize = function() {
		var rTools = Cn._.t;
		var oCal;

			//#### .Render the DHTML, then reposition the g_cBASEID to 0,0
			//####     NOTE: The repositioning seems to be VERY important with reguard to properly positioning the .Calendar later on. I've got no idea why, but it's done for good pratices anyway.
		Render();
		rTools.Left(g_cBASEID, 0);
		rTools.Top(g_cBASEID, 0);

			//#### Setup the .Calendar with a new .Calendar_Core
		this.Calendar = new YAHOO.widget.Calendar_Core(g_cCALENDARID + '.Table', g_cCALENDARID, null, null);
		oCal = this.Calendar;

//# <DebugCode>
			//#### If our .Text settings have not yet been setup, popup the error
		if (this.Text == null) {
			alert("Cn.Renderer.Form.DateTime: [DEVELOPER] 'DateTimeText.js." + Cn._.s.ScriptFileExtension + "' must be included before referencing this code.");
		}
//# </DebugCode>

			//#### Set the .Calendar's months and week days from our .Text, then hook our own ._OnSelect its the onSelect event
		oCal.Options.LOCALE_MONTHS = this.Text.Months;
		oCal.Options.LOCALE_WEEKDAYS = this.Text.cDays;
		oCal['onSelect'] = this._OnSelect;

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
	};


	//##########################################################################################
	//# 'Public' Properties
	//##########################################################################################
	//############################################################
	//# Declare the required 'public' (sub-class stub) properties
	//############################################################
	//# Last Updated: March 29, 2006
	this.Calendar = null;
	this.Text = null;

	//############################################################
	//# Declare the required 'public' properties
	//############################################################
	//# Last Updated: April 18, 2006
	this.WeekOfYearCalculation = this.enumWeekOfYearCalculations.cnDefault;


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
	//# Last Updated: April 20, 2006
	this.Show = function(oElement, eInputType, sInitialDate, sFormat, iHourInc, iMinuteInc, iSecondInc) {
		var rTools = Cn._.t;
		var sTemp, iH, iW, iX, iY;

			//#### Set the arguments into the 'private' class variables
		g_oElement = rTools.GetByID(oElement);
		g_sFormat = String(sFormat);
		g_iHourInc = rTools.MakeNumeric(iHourInc, 0);
		g_iMinInc = rTools.MakeNumeric(iMinuteInc, 0);
		g_iSecInc = rTools.MakeNumeric(iSecondInc, 0);

			//#### Determine the value of the 'private' class varaibles based on the passed data
		g_bShowTime = (eInputType != this.enumInputTypes.cnDateInput && eInputType != this.enumInputTypes.cnDefaultInput);
		g_bIs12Hour = (sFormat.indexOf('$h') != -1);
		g_bPadHour = (sFormat.toLowerCase().indexOf('$hh') != -1);
		g_bPadMin = (sFormat.indexOf('$mm') != -1);
		g_bPadSec = (sFormat.indexOf('$ss') != -1);

			//#### If the g_oElement hasn't had its .SerialDate setup, default it now
		if (g_oElement && typeof(g_oElement.SerialDate) == 'undefined') {
			g_oElement.SerialDate = sInitialDate;
		}

			//#### ._Setup(the)TimeDisplay based on the passed data, then .Set(the)DateTime from the .SerialDate
		SetupTimeDisplay(eInputType);
		this.SetDateTime(ParseSerialDate());

		//##########
		//##########

			//#### Determine the iX/iY coords of the g_oElement, then position the ._BaseID just below it
		iX = rTools.Left(g_oElement);
		iY = (rTools.Top(g_oElement) + rTools.Height(g_oElement) - 1);
		rTools.Left(g_cBASEID, iX);
		rTools.Top(g_cBASEID, iY);

			//#### Show (or in this case, 'display') and .Repaint the g_cCALENDARID so that the iW(idth) can be properly calculated below
		rTools.Style(g_cCALENDARID, 'display', '');
		rTools.Repaint(g_cCALENDARID);

			//#### Calculate and set the iW(idth) (ensuring it is at least the g_cMINWIDTH)
		iW = rTools.Width(g_cCALENDARID);
		if (iW < g_cMINWIDTH) { iW = g_cMINWIDTH; }

			//#### Set the .Width of the g_cBASEID, ensure that the g_cCALENDARID fills its position, then .Show the g_cBASEID
			//####     NOTE: Setting the g_cCALENDARID to 100% width is required for browers with issues properly rendering the CSS ([cough] IE [cough])
		rTools.Width(g_cBASEID, iW);
		rTools.Style(g_cCALENDARID, 'width', '100%');
		rTools.Show(g_cBASEID);

			//#### Determine the iH(eight) and iW(idth) of the now showing ._BaseID
		iH = rTools.Height(g_cBASEID);
		iW = rTools.Width(g_cBASEID);

			//#### If the ._BaseID is too far right, reset it to the right margin (or the left margin if the screen is really narrow, allowing a 1px buffer either way)
		if ((iX + iW) > (rTools.WindowWidth() + rTools.WindowScrollLeft())) {
			iX = ((rTools.WindowWidth() + rTools.WindowScrollLeft()) - iW - 1);
			if (iX < 1) { iX = 1; }
			rTools.Left(g_cBASEID, iX);
		}

			//#### If the ._BaseID is too far down the page, reset it to the bottom (or the top if the screen is really short, allowing a 1px buffer either way)
		if ((iY + iH) > (rTools.WindowHeight() + rTools.WindowScrollTop())) {
			iY = ((rTools.WindowHeight() + rTools.WindowScrollTop()) - iH - 1);
			if (iY < 1) { iY = 1; }
			rTools.Top(g_cBASEID, iY);
		}

			//#### If the user agent is either IE or Netscape 4
		if (rTools.IsIE() || rTools.IsNN4()) {
				//#### Ensure that any non-exempt troublesome z-index elements are hidden underneath rfCalendarDIV
				//####     NOTE: There is no need to .GetChildElementIDs for SELECT's because we know the ._BaseID doesn't have any
			//rTools.ToggleOverlappingElements(g_cBASEID, ['SELECT'], rTools.GetChildElementIDs(g_cBASEID, ['SELECT']));
			rTools.ToggleOverlappingElements(g_cBASEID, ['SELECT'], null);
		}
	};

	//############################################################
	//# Hides the DHTML input from the user.
	//############################################################
	//# Last Updated: April 19, 2006
	this.Hide = function(bClear) {
		var rTools = Cn._.t;

			//#### If we are supposed to bClear the g_oElement, do so now
		if (bClear) {
			this.Clear();
		}

			//#### .Hide the ._BaseID (display'ing none on the .Date.Cal so it's properly hidden)
		rTools.Style(g_cCALENDARID, 'display', 'none');
		rTools.Hide(g_cBASEID);

			//#### If the user agent is either IE or Netscape 4
		if (rTools.IsIE() || rTools.IsNN4()) {
				//#### Ensure that any troublesome z-index elements that were underneath the ._BaseID are properly un-hidden (that's a word, right? =)
			rTools.ToggleOverlappingElements(g_cBASEID, ['SELECT'], null);
		}

			//#### Move the control back to 0,0 so that any scrollbars are properly reset
		rTools.Left(g_cBASEID, 0);
		rTools.Top(g_cBASEID, 0);
	};

	//############################################################
	//# Updates the value of this instance's related element to the currently defined date/time.
	//############################################################
	//# Last Updated: April 19, 2006
	this.Update = function() {
		var sSerialDate;
		var dDateTime = this.GetDateTime();
		var bReturn = false;

			//#### If we have an g_oElement to .Update
		if (g_oElement) {
				//#### Determine the sSerialDate (inc'ing .getMonth by one to transform JavaScript's 0-11 month definition into the standard 1-12)
			sSerialDate = dDateTime.getFullYear() + ' ' +
				(dDateTime.getMonth() + 1) + ' ' +
				dDateTime.getDate() + ' ' +
				dDateTime.getHours() + ' ' +
				dDateTime.getMinutes() + ' ' +
				dDateTime.getSeconds();

				//#### If this is an updated sSerialDate
			if (sSerialDate != g_oElement.SerialDate) {
					//#### .Update the g_oElement's .value and .SerialDate (formating the dDateTime as we go), then flip the bReturn value to true (as we successfully updated the value)
					//####     NOTE: This intermediary .SerialDate is used so that we do not have to constantly parse the formatted date preset within the g_oElement's .value
				g_oElement.value = this.FormatDate(dDateTime, g_sFormat);
				g_oElement.SerialDate = sSerialDate;
				bReturn = true;
			}
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Clears the value of this instance's related element.
	//############################################################
	//# Last Updated: April 11, 2006
	this.Clear = function() {
			//#### If we have an g_oElement to .Clear, .Clear its .value and .SerialDate
		if (g_oElement) {
			g_oElement.value = '';
			g_oElement.SerialDate = '';
		}
	};

	//############################################################
	//# Retrieves the current date/time displayed within this instance's control.
	//############################################################
	//# Last Updated: April 19, 2006
	this.GetDateTime = function() {
		var rTools = Cn._.t;
		var dReturn = rTools.MakeDateTime(this.Calendar.getSelectedDates()[0]);
		var iHours;

			//#### If we are ._Show(ing the)Time
		if (g_bShowTime) {
				//#### If this ._Is(a)12Hour clock and we are in the .PM, add 12 onto the .Hours (while ensuring we never end up with 24)
			if (g_bIs12Hour && rTools.InnerHTML(g_cMERIDIEMID) == this.Text.PM) {
				iHours = (rTools.MakeNumeric(rTools.InnerHTML(g_cHOURSID), 0) + 12);
				if (iHours == 24) { iHours = 12; }
			}
				//#### Else we can go with the .Hours as is, so set iHours accordingly
			else {
				iHours = rTools.MakeNumeric(rTools.InnerHTML(g_cHOURSID), 0);
				if (g_bIs12Hour && iHours == 12) { iHours = 0; }
			}

				//#### Set the time elements within the dReturn value based on what the user has specified (.(re)set(ting the)Milliseconds while we're at it)
			dReturn.setHours(iHours);
			dReturn.setMinutes(rTools.MakeNumeric(rTools.InnerHTML(g_cMINUTESID), 0));
			dReturn.setSeconds(rTools.MakeNumeric(rTools.InnerHTML(g_cSECONDSID), 0));
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
	//# Last Updated: April 19, 2006
	this.IncMeridiem = function(oElement) {
		var rTools = Cn._.t;
		var bReturn = (oElement && oElement.id);

			//#### If the passed oElement seemed valid
		if (bReturn) {
				//#### If the meridiem is currently in the afternoon, reset it to the morning
			if (rTools.InnerHTML(oElement) == this.Text.PM) {
				rTools.InnerHTML(oElement, this.Text.AM);
			}
				//#### Else the meridiem is currently in the morning (or unreconized), so reset it to the afternoon
			else {
				rTools.InnerHTML(oElement, this.Text.PM);
			}

				//#### .Update the g_oElement with the new time
			this.Update();
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};


	//##########################################################################################
	//# DateMath-related 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Formats the provided date based on the referenced format.
	//############################################################
	//# Last Updated: April 19, 2006
	this.FormatDate = function(dDateTime, sFormat) {
		var rTools = Cn._.t;
		var a_sDecoders = new Array();
		var sDollarPlaceHolder = '_DollarSign_';
		var oRegEx, sReturn, i;

			//#### Ensure the passed dDateTime is valid
			//####     NOTE: Due to the lack of a second argument in the .MakeDateTime call, the dDateTime will be defaulted to the current date/time if it is invalid
		dDateTime = rTools.MakeDateTime(dDateTime);

		//##########
		//##########

			//#### Borrow the use of i to store the month day, setting $D, $DD and $S accordingly
		i = dDateTime.getDate();
		a_sDecoders[0] = ['$D', i];
		a_sDecoders[1] = ['$DD', rTools.LPad(i, '0', 2)];
		a_sDecoders[2] = ['$S', this.Text.DaySuffixes[i - 1]];

			//#### Borrow the use of i to store the day of the week, setting $W, $WWW and $WWWW according to the above determined .DayOfWeek in the borrowed i
		i = dDateTime.getDay();
		a_sDecoders[3] = ['$W', i];
		a_sDecoders[4] = ['$WWW', this.Text.aDays[i]];
		a_sDecoders[5] = ['$WWWW', this.Text.Days[i]];

			//#### Borrow the use of i to store the month, setting $M, $MM, $MMM and $MMMM accordingly
		i = dDateTime.getMonth();
		a_sDecoders[6] = ['$M', i];
		a_sDecoders[7] = ['$MM', rTools.LPad(i, '0', 2)];
		a_sDecoders[8] = ['$MMM', this.Text.aMonths[i]];
		a_sDecoders[9] = ['$MMMM', this.Text.Months[i]];

			//#### Borrow the use of oRegEx to store the .WeekOfYear, setting $w, $ww, $yy and $yyyy accordingly
		oRegEx = this.WeekOfYear(dDateTime, this.WeekOfYearCalculation);
		a_sDecoders[10] = ['$w', oRegEx[0]];
		a_sDecoders[11] = ['$ww', rTools.LPad(oRegEx[0], '0', 2)];

			//#### Borrow the use of sReturn to store the MakeString'd .Year and then borrow i to determine its .Length, finally setting $yy and $yyyy accordingly
			//####    NOTE: We can borrow sReturn here as its value is reset below, so the value set below is ignored
		sReturn = String(oRegEx[1]);
		i = (sReturn.length - 2);
		if (i < 0) {
			i = 0;
		}
		a_sDecoders[12] = ['$yy', rTools.LPad(sReturn.substr(i), '0', 2)];
		a_sDecoders[13] = ['$yyyy', sReturn];

			//#### Borrow the use of sReturn to store the MakeString'd .getFullYear and then borrow i to determine its .Length, finally setting $YY and $YYYY accordingly
			//####    NOTE: We can borrow sReturn here as its value is reset below, so the value set below is ignored
		sReturn = String(dDateTime.getFullYear());
		i = (sReturn.length - 2);
		if (i < 0) {
			i = 0;
		}
		a_sDecoders[14] = ['$YY', rTools.LPad(sReturn.substr(i), '0', 2)];
		a_sDecoders[15] = ['$YYYY', sReturn];

			//#### Borrow the use of i to store the .DayOfYear, setting $J and $JJJ accordingly
		i = this.DayOfYear(dDateTime);
		a_sDecoders[16] = ['$J', i];
		a_sDecoders[17] = ['$JJJ', rTools.LPad(i, '0', 3)];

			//#### Set $E based on the current Timestamp (down converting the .getTime'd returned milliseconds into seconds)
			//####    NOTE: Can debug output at http://www.argmax.com/mt_blog/archive/000328.php?ts=1058415153
		a_sDecoders[18] = ['$E', parseInt(dDateTime.getTime() / 1000)];

			//#### Borrow the use of i to store the 24 hour, setting $HH and $H accordingly
		i = dDateTime.getHours();
		a_sDecoders[19] = ['$H', i];
		a_sDecoders[20] = ['$HH', rTools.LPad(i, '0', 2)];

			//#### If the .Hour within i is before noon, set $tt to "am"
		if ((i % 12) == i) {
			a_sDecoders[21] = ['$tt', this.Text.AM];
		}
			//#### Else the .Hour within i is after noon, so set $tt to "pm"
		else {
			a_sDecoders[21] = ['$tt', this.Text.PM];
		}

			//#### Determine the 12-hour time from the above collected .Hour (fixing 0 hours as necessary), then set $hh and $hh accordingly
		i = (i % 12);
		if (i == 0) {
			i = 12;
		}
		a_sDecoders[22] = ['$h', i];
		a_sDecoders[23] = ['$hh', rTools.LPad(i, '0', 2)];

			//#### Borrow the use of i to store the .Minute, setting $m and $mm accordingly
		i = dDateTime.getMinutes();
		a_sDecoders[24] = ['$m', i];
		a_sDecoders[25] = ['$mm', rTools.LPad(i, '0', 2)];

			//#### Borrow the use of i to store the .Second, setting $s and $ss accordingly
		i = dDateTime.getSeconds();
		a_sDecoders[26] = ['$s', i];
		a_sDecoders[27] = ['$ss', rTools.LPad(i, '0', 2)];

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
	//# Last Updated: April 13, 2006
	this.DayOfWeek = function(dDate) {
			//#### Ensure the passed dDate is valid
		dDate = Cn._.t.MakeDateTime(dDate);

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
		dDateTime = Cn._.t.MakeDateTime(dDateTime);

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
		dDateTime = Cn._.t.MakeDateTime(dDateTime);

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
	//# Pseudo-'Protected' Functions
	//##########################################################################################
	//############################################################
	//# Calendar on date select callback function.
	//############################################################
	//# Last Updated: April 19, 2006
	this._OnSelect = function() {
		var rThis = Cn._.dt;

			//#### .Update the g_oElement with the currently selected date, if it was not updated (i.e. - the same date was selected twice in a row) .Hide the control
			//####     NOTE: Due to scoping issues, we need to fully reference .Update and .Hide as "this." doesn't like life because only a reference to ._OnSelect is passed into the .Calendar, and that reference doesn't seem to carry its context with it (weird!?!).
		if (! rThis.Update()) {
			rThis.Hide(false);
		}
	};


	//##########################################################################################
	//# 'Private', Pseudo-'Static' Functions
	//# 
	//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
	//##########################################################################################
	//############################################################
	//# Renders the DHTML Control.
	//############################################################
	//# Last Updated: May 15, 2006
	var Render = function() {
		var rText = Cn._.dt.Text;
		var d = document;
		var sPath = Cn._.ws.BaseDirectory + 'img/';

			//#### Setup the sClose code
		var sClose = "<img src='" + sPath + "x.gif' onclick='Cn._.dt.Hide(false);' class='calNavButtons' style='float: right;' alt='" + rText.Close + "' />" +
			"<img src='" + sPath + "clear.gif' onclick='Cn._.dt.Hide(true);' class='calNavButtons' style='float: right;' alt='" + rText.Clear + "' />";

			//#### .Write out header portion of the control
			//####     NOTE: Since there is no valid way to define that the DIV uses both the CSS Cn and Cn.CalendarContainer classes, an outer SPAN is employed
		d.write("<span class='Cn'>" +
			"<div id='" + g_cBASEID + "' class='CalendarContainer' style='visibility: hidden; position: absolute;'>"
		);

			//#### .Write out the calendar input portion of the control
		d.write("<div id='" + g_cDATEID + "' style='text-align: center;'>" +
				"<img src='" + sPath + "ll.gif' onclick='Cn._.dt.Calendar.previousYear();' class='calNavButtons' style='margin: 0px;' alt='" + rText.PreviousYear + "' />" +
				"<img src='" + sPath + "l.gif' onclick='Cn._.dt.Calendar.previousMonth();' class='calNavButtons' alt='" + rText.PreviousMonth + "' />" +
				"<img src='" + sPath + "today.gif' onclick='Cn._.dt.Calendar.reset();' class='calNavButtons' alt='" + rText.Today + "' />" +
				"<img src='" + sPath + "r.gif' onclick='Cn._.dt.Calendar.nextMonth();' class='calNavButtons' alt='" + rText.NextMonth + "' />" +
				"<img src='" + sPath + "rr.gif' onclick='Cn._.dt.Calendar.nextYear();' class='calNavButtons' alt='" + rText.NextYear + "' />" +
				sClose +
				"<div id='" + g_cCALENDARID + "' style='float: left; clear: both; position: relative; padding-top: 3px;'></div>" +
			"</div>"
		);

			//#### .Write out the time input navigation portion of the control
		d.write("<div id='" + g_cTIMEID + ".X'>" +
				sClose +
			"</div>"
		);

			//#### .Write out the time input portion of the control
			//####     NOTE: "text-align: center;" does not seem to want to center align the time table, hence the use of the aniquated <center> tags below
//		d.write("<div id='" + g_cTIMEID + "' style='float: left; text-align: center; width: 100%; padding-top: 3px;'>" +
		d.write("<div id='" + g_cTIMEID + "' style='float: left; width: 100%; padding-top: 3px;'>" +
"<center>" +
			"<table border='0' cellpadding='0' cellspacing='0' style='text-align: center;'><tr><td class='calcell' id='" + g_cHOURSID + ".TD'>" +
				"<a href='javascript:void(null);' id='" + g_cHOURSID + "' onMouseDown='Cn._.dt.IncHour(this);'>0</a>"
		);

			d.write("</td><td style='width: 5px;' id='" + g_cMINUTESID + ".D'>" + rText.Delimiter +
				"</td><td class='calcell' id='" + g_cMINUTESID + ".TD'>" +
					"<a href='javascript:void(null);' id='" + g_cMINUTESID + "' onMouseDown='Cn._.dt.IncMinute(this);'>0</a>"
			);

			d.write("</td><td style='width: 5px;' id='" + g_cSECONDSID + ".D'>" + rText.Delimiter +
				"</td><td class='calcell' id='" + g_cSECONDSID + ".TD'>" +
					"<a href='javascript:void(null);' id='" + g_cSECONDSID + "' onMouseDown='Cn._.dt.IncSecond(this);'>0</a>"
			);

			d.write("</td><td style='width: 5px;' id='" + g_cMERIDIEMID + ".D'>&nbsp;" +
				"</td><td class='calcell' id='" + g_cMERIDIEMID + ".TD'>" +
					"<a href='javascript:void(null);' id='" + g_cMERIDIEMID + "' onMouseDown='Cn._.dt.IncMeridiem(this);'>-</a>" +
				"</td><td>" +
					"&nbsp;<img src='" + sPath + "now.gif' onClick='Cn._.dt.SetTime();' style='cursor: pointer;' alt='" + rText.Now + "' />"
			);

			d.write("</td></tr></table>" +
"</center>" +
				"</div>"
			);
	
			//#### .Write out footer portion of the control
			//####     NOTE: The nbsp is included so that Firefox properly renders the outer container div (else its height is wrong)
		d.write("<div style='font-size: 1px;'>&nbsp;</div>" +
			"</div>" +
			"</span>"
		);
	};

	//############################################################
	//# Configures the time input portion based on the provided input type and format.
	//############################################################
	//# Last Updated: April 19, 2006
	var SetupTimeDisplay = function(eInputType) {
		var rTools = Cn._.t;
		var rThis = Cn._.dt;
		var sDisplay, bRendered, bShow;

			//#### Determine the sDisplay for the .Date (based on if this is a .cnTimeInput eInputType), setting .Date accordingly
		sDisplay = (eInputType == rThis.enumInputTypes.cnTimeInput ? 'none' : '');
		rTools.Style(g_cDATEID, 'display', sDisplay);

			//#### Determine the sDisplay for the .Time.X (based on if this is a .cnTimeInput eInputType), setting .Date accordingly
		sDisplay = (eInputType == rThis.enumInputTypes.cnTimeInput ? '' : 'none');
		rTools.Style(g_cTIMEID + '.X', 'display', sDisplay);

			//#### Determine the sDisplay for the .Time (based on if this is a .cnDateInput or .cnDefaultInput eInputType), setting .Time accordingly
		sDisplay = (eInputType == rThis.enumInputTypes.cnDateInput || eInputType == rThis.enumInputTypes.cnDefaultInput ? 'none' : '');
		rTools.Style(g_cTIMEID, 'display', sDisplay);

			//#### If we are supposed to ._Show(the)Time
		if (g_bShowTime) {
				//#### Determine if we are to bShow the .Hours while defaulting bRendered to the value of bShow
			bShow = (g_sFormat.toLowerCase().indexOf('$h') != -1);
			bRendered = bShow;

				//#### Determine and set the sDisplay of the .Hours
			sDisplay = (bShow ? '' : 'none');
			rTools.Style(g_cHOURSID, 'display', sDisplay);
			rTools.Style(g_cHOURSID + '.TD', 'display', sDisplay);

			//##########
			//##########

				//#### Determine if we are to bShow the .Minutes
			bShow = (g_sFormat.indexOf('$m') != -1);

				//#### If we have bRendered something above and are supposed to bShow the .Minutes, set sDisplay and the .Minutes.D accordingly
			sDisplay = ((bRendered && bShow) ? '' : 'none');
			rTools.Style(g_cMINUTESID + '.D', 'display', sDisplay);

				//#### If we've not yet bRendered something above, set it based on bShow and determine the sDisplay for the .Minutes
			if (! bRendered) { bRendered = bShow; }
			sDisplay = (bShow ? '' : 'none');

				//#### Set the sDisplay of the .Minutes
			rTools.Style(g_cMINUTESID, 'display', sDisplay);
			rTools.Style(g_cMINUTESID + '.TD', 'display', sDisplay);

			//##########
			//##########

				//#### Determine if we are to bShow the .Seconds
			bShow = (g_sFormat.indexOf('$s') != -1);

				//#### If we have bRendered something above and are supposed to bShow the .Seconds, set sDisplay and the .Seconds.D accordingly
			sDisplay = ((bRendered && bShow) ? '' : 'none');
			rTools.Style(g_cSECONDSID + '.D', 'display', sDisplay);

				//#### If we've not yet bRendered something above, set it based on bShow and determine the sDisplay for the .Seconds
			if (! bRendered) { bRendered = bShow; }
			sDisplay = (bShow ? '' : 'none');

				//#### Set the sDisplay of the .Seconds
			rTools.Style(g_cSECONDSID, 'display', sDisplay);
			rTools.Style(g_cSECONDSID + '.TD', 'display', sDisplay);

			//##########
			//##########

				//#### Determine if we are to bShow the .Meridiem
			bShow = (g_sFormat.indexOf('$tt') != -1);

				//#### If we have bRendered something above and are supposed to bShow the .Meridiem, set sDisplay and the .Meridiem.D accordingly
			sDisplay = ((bRendered && bShow) ? '' : 'none');
			rTools.Style(g_cMERIDIEMID + '.D', 'display', sDisplay);

				//#### Determine the sDisplay and set the sDisplay of the .Meridiem
				//####     NOTE: Since this is the last element, we do not need to process bRendered
			sDisplay = (bShow ? '' : 'none');
			rTools.Style(g_cMERIDIEMID, 'display', sDisplay);
			rTools.Style(g_cMERIDIEMID + '.TD', 'display', sDisplay);
		}
	};

	//############################################################
	//# Parses the serial date of the current class element.
	//############################################################
	//# Last Updated: April 19, 2006
	var ParseSerialDate = function() {
		var a_sDate;
		var dReturn = new Date();

			//#### If g_oElement is valid with a defined .SerialDate
		if (g_oElement && g_oElement.SerialDate) {
				//#### .split the g_oElement's .SerialDate into the a_sDate
			a_sDate = g_oElement.SerialDate.split(' ');

				//#### If the a_sDate has the correct number of positions, set the dReturn value to a new date based on its' data (passing in 0 for the milliseconds)
				//####     NOTE: We decrement the month (index 1) as JavaScript uses 0-11 to define the months of the year (which is it's only aberrant date-related definition)
			if (a_sDate && a_sDate.length == 6) {
				dReturn = new Date(a_sDate[0], parseInt(a_sDate[1]) - 1, a_sDate[2], a_sDate[3], a_sDate[4], a_sDate[5], 0);
			}
		}

			//#### Return the above determined dReturn value to the caller
		return dReturn;
	};

	//############################################################
	//# Increments the referenced time control element.
	//############################################################
	//# Last Updated: April 19, 2006
	var DoInc = function(oElement, iMinValue, iMaxValue, iIncrement, bZeroPad) {
		var rTools = Cn._.t;
		var rThis = Cn._.dt;
		var bReturn = (oElement && oElement.id);
		var oValue;

			//#### If the passed oElement seemed valid
		if (bReturn) {
				//#### If the iIncrement is 0 then this is an unused oElement of the time, so set the oValue to 0
			if (iIncrement == 0) {
				oValue = 0;
			}
				//#### Else we have a valid iIncrement
			else {
					//#### Collect the oValue from the passed oElement plus the iIncrement
				oValue = (rTools.MakeNumeric(rTools.InnerHTML(oElement), 0) + iIncrement);

					//#### If the above determined oValue is greater then the iMaxValue, reset it to the iMinValue
				if (oValue > iMaxValue) {
					oValue = iMinValue;
				}
			}

				//#### Ensure that the oValue is properly zero-padded and String-ified, then set it back into the oElement
			bZeroPad ? oValue = rTools.LPad(oValue, '0', 2) : oValue = String(oValue);
			rTools.InnerHTML(oElement, oValue);

				//#### .Update the g_oElement with the new time
			rThis.Update();
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Sets the passed date and time into this instance's control.
	//############################################################
	//# Last Updated: April 19, 2006
	var DoSetDateTime = function(dDateTime, bSetDate, bSetTime) {
		var rTools = Cn._.t;
		var rThis = Cn._.dt;

			//#### Ensure the passed dDateTime is valid
		dDateTime = rTools.MakeDateTime(dDateTime);

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
					rTools.InnerHTML(g_cMERIDIEMID, rThis.Text.PM);
				}
					//#### Else we're still in the morning, so set the sMeridiem to .AM
				else {
					rTools.InnerHTML(g_cMERIDIEMID, rThis.Text.AM);
				}
			}

				//#### Ensure that the oHours, oMinutes and oSeconds are properly zero-padded and String-ified
			g_bPadHour ? oHours = rTools.LPad(oHours, '0', 2) : oHours = String(oHours);
			g_bPadMin ? oMinutes = rTools.LPad(oMinutes, '0', 2) : oMinutes = String(oMinutes);
			g_bPadSec ? oSeconds = rTools.LPad(oSeconds, '0', 2) : oSeconds = String(oSeconds);

				//#### Update the .InnerHTML of the controls with the above determined data
			rTools.InnerHTML(g_cHOURSID, oHours);
			rTools.InnerHTML(g_cMINUTESID, oMinutes);
			rTools.InnerHTML(g_cSECONDSID, oSeconds);

				//#### If we are not also supposed to bSet(the)Date, .Update the g_oElement with the new time now
			if (! bSetDate) {
				rThis.Update();
			}
		}

			//#### If we are supposed to bSet(the)Date
			//####     NOTE: We set the date last do to ._OnSelect's .Update call
		if (bSetDate) {
				//#### .deselectAll the current dates on the .Calendar, then .select the passed dDateTime
			rThis.Calendar.deselectAll();
			rThis.Calendar.select([dDateTime]);

				//#### Reset the .Calendar's month/year, then (re).render it (else the month is not changed)
			rThis.Calendar.setMonth(dDateTime.getMonth());
			rThis.Calendar.setYear(dDateTime.getFullYear());
//!
			rThis.Calendar.render();
		}
	};

	//############################################################
	//# Determines the simple week number (functionally equivlent to Excel's WeekNum) for the given date.
	//############################################################
	//# Last Updated: April 19, 2006
	var WeekOfYear_Simple = function(dDateTime, eStartOfWeek) {
		var rThis = Cn._.dt;
		var iDaysInFirstWeek;
		var eJan1 = rThis.DayOfWeek(new Date(dDateTime.getFullYear(), 0, 1));

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
		return Math.ceil((rThis.DayOfYear(dDateTime) - iDaysInFirstWeek) / 7) + 1;
	};

	//############################################################
	//# Determines the ISO 8601 week number (also known as the 4 day rule) for the given date.
	//############################################################
	//# Last Updated: April 19, 2006
	var WeekOfYear_ISO8601 = function(dDateTime) {
		var rThis = Cn._.dt;
		var a_iReturn = [0,0];
		var cTHURSDAY = 4;
		var iDaysInWeek1, iJan1, iDec31;

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
		a_iReturn[0] = Math.ceil((rThis.DayOfYear(dDateTime) - iDaysInWeek1) / 7);

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
			a_iReturn = rThis.WeekOfYear(new Date(a_iReturn[1] - 1, 11, 31));
		}

			//#### Return the above determined a_iReturn value to the caller
		return a_iReturn;
	};


/*	//##########################################################################################
	//# YUI-specific 'Private' Functions
	//##########################################################################################
	//############################################################
	//# Converts the passed date into a YUI reconized month definition.
	//############################################################
	//# Last Updated: March 29, 2006
	var YUIMonth = function(dDate) {
			//#### Ensure the passed dDate is valid
		dDate = Cn._.t.MakeDateTime(dDate);

			//#### Return the YUI formatted month/year to the caller
		return (dDate.getMonth() + 1) + '/' + dDate.getFullYear();
	};

	//############################################################
	//# Converts the passed date into a YUI reconized date definition.
	//############################################################
	//# Last Updated: March 29, 2006
	var ToYUIDate = function(dDate) {
			//#### Ensure the passed dDate is valid
		dDate = Cn._.t.MakeDateTime(dDate);

			//#### Return the YUI formatted date to the caller
		return (dDate.getMonth() + 1) + '/' + dDate.getDate() + '/' + dDate.getFullYear();
	};
*/

} //# Cn.Renderer.Form.DateTime
