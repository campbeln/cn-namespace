/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

<%@ Page Language="C#" %>
<%
	System.Web.HttpRequest oRequest = System.Web.HttpContext.Current.Request;
	Cn.Configuration.Internationalization oIntl;
	Cn.Collections.MultiArray oPicklistData;
	Hashtable h_sPicklists = new Hashtable();
	string sLanguageCode;
	int iWeekOfYearCalculation;

		//#### Collect the oIntl based on Cn.Web.Settings' static .Internationalization
		//####     NOTE: This allows the developer to override Cn.Web.Settings' default .Internationalization wiht one of thier own that will apply within these .js.* script files
	oIntl = Cn.Web.Settings.Internationalization;

		//#### Set the sLanguageCode based on the querystring passed ?LanguageCode= (which actually referes to Web.Settings.Current's .EndUserMessagesLanguageCode), validating it as we go
	sLanguageCode = oIntl.ValidateLanguageCode(oRequest.QueryString["LanguageCode"]);

		//#### Determine the iWeekOfYearCalculation based on oIntl's .cnLocalization_Date_WeekOfYearCalculationEnum for the above set .LanguageCode, defaulting to .enumWeekOfYearCalculations.cnDefault is one has not been defined
	iWeekOfYearCalculation = Cn.Data.Tools.MakeInteger(
		oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnLocalization_Date_WeekOfYearCalculationEnum),
		(int)Cn.Dates.enumWeekOfYearCalculations.cnDefault
	);

	//#####
	//#####

		//#### Collect the .cnDate_MonthDaySuffix oPicklistData
	oPicklistData = oIntl.Values(Cn.Configuration.Internationalization.enumInternationalizationPicklists.cnDate_MonthDaySuffix, sLanguageCode);

		//#### If the .cnDate_MonthDaySuffix oPicklistData was successfully collected above, set the MonthDaySuffix accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["MonthDaySuffix"] = Cn.Web.JavaScript.ToArray(oPicklistData.Column("Description"));
	}

	//#####
	//#####

		//#### Collect the .cnDate_AbbreviatedWeekDayNames oPicklistData
	oPicklistData = oIntl.Values(Cn.Configuration.Internationalization.enumInternationalizationPicklists.cnDate_AbbreviatedWeekDayNames, sLanguageCode);

		//#### If the .cnDate_AbbreviatedWeekDayNames oPicklistData was successfully collected above, set the AbbreviatedWeekDayNames accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["AbbreviatedWeekDayNames"] = Cn.Web.JavaScript.ToArray(oPicklistData.Column("Description"));
	}

	//#####
	//#####

		//#### Collect the .cnDate_CalendarWeekDayNames oPicklistData
	oPicklistData = oIntl.Values(Cn.Configuration.Internationalization.enumInternationalizationPicklists.cnDate_CalendarWeekDayNames, sLanguageCode);

		//#### If the .cnDate_CalendarWeekDayNames oPicklistData was successfully collected above, set the CalendarWeekDayNames accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["CalendarWeekDayNames"] = Cn.Web.JavaScript.ToArray(oPicklistData.Column("Description"));
	}

	//#####
	//#####

		//#### Collect the .cnDate_WeekDayNames oPicklistData
	oPicklistData = oIntl.Values(Cn.Configuration.Internationalization.enumInternationalizationPicklists.cnDate_WeekDayNames, sLanguageCode);

		//#### If the .cnDate_WeekDayNames oPicklistData was successfully collected above, set the WeekDayNames accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["WeekDayNames"] = Cn.Web.JavaScript.ToArray(oPicklistData.Column("Description"));
	}

	//#####
	//#####

		//#### Collect the .cnDate_AbbreviatedMonthNames oPicklistData
	oPicklistData = oIntl.Values(Cn.Configuration.Internationalization.enumInternationalizationPicklists.cnDate_AbbreviatedMonthNames, sLanguageCode);

		//#### If the .cnDate_AbbreviatedMonthNames oPicklistData was successfully collected above, set the AbbreviatedMonthNames accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["AbbreviatedMonthNames"] = Cn.Web.JavaScript.ToArray(oPicklistData.Column("Description"));
	}

	//#####
	//#####

		//#### Collect the .cnDate_MonthNames oPicklistData
	oPicklistData = oIntl.Values(Cn.Configuration.Internationalization.enumInternationalizationPicklists.cnDate_MonthNames, sLanguageCode);

		//#### If the .cnDate_MonthNames oPicklistData was successfully collected above, set the MonthNames accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["MonthNames"] = Cn.Web.JavaScript.ToArray(oPicklistData.Column("Description"));
	}
%>

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined' || ! Cn._.ws || ! Cn._.ci || ! Cn._.wid) {
	alert("Cn.Inputs.DateTime.Text: [DEVELOPER] 'Cn/Inputs/DateTime.js' must be included before referencing this code.");
}
//# </DebugCode>


//########################################################################################################################
//# Required settings-related values for DateTime.js
//# 
//#     NOTE: The "WeekOfYearCalculation" and "LanguageCode" are included on the querystring to this script.
//#     Always Included Along With: Cn/Inputs/DateTime.js
//########################################################################################################################
//# Last Code Review: January 13, 2010

	//#### Set the .Value's under Cn.Configuration.Internationalization
Cn._.ci.Value.EndUserMessages_DateTime_PreviousYear = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_PreviousYear, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_DateTime_PreviousMonth = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_PreviousMonth, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_DateTime_Today = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_Today, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_DateTime_NextMonth = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_NextMonth, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_DateTime_NextYear = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_NextYear, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_DateTime_Close = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_Close, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_DateTime_Clear = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_Clear, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_DateTime_Now = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_Now, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_DateTime_AM = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_AM, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_DateTime_PM = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_PM, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_DateTime_Delimiter = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_DateTime_Delimiter, sLanguageCode), "'")); %>';
Cn._.ci.Value.Localization_Date_WeekOfYearCalculationEnum = <% Response.Write(iWeekOfYearCalculation); %>;

	//#### Set the .Values' under Cn.Configuration.Internationalization
Cn._.ci.Values.Date_MonthNames = <% Response.Write(Cn.Data.Tools.MakeString(h_sPicklists["MonthNames"], "")); %>;
Cn._.ci.Values.Date_MonthDaySuffix = <% Response.Write(Cn.Data.Tools.MakeString(h_sPicklists["MonthDaySuffix"], "")); %>;
Cn._.ci.Values.Date_AbbreviatedWeekDayNames = <% Response.Write(Cn.Data.Tools.MakeString(h_sPicklists["AbbreviatedWeekDayNames"], "")); %>;
Cn._.ci.Values.Date_CalendarWeekDayNames = <% Response.Write(Cn.Data.Tools.MakeString(h_sPicklists["CalendarWeekDayNames"], "")); %>;
Cn._.ci.Values.Date_WeekDayNames = <% Response.Write(Cn.Data.Tools.MakeString(h_sPicklists["WeekDayNames"], "")); %>;
Cn._.ci.Values.Date_AbbreviatedMonthNames = <% Response.Write(Cn.Data.Tools.MakeString(h_sPicklists["AbbreviatedMonthNames"], "")); %>;
