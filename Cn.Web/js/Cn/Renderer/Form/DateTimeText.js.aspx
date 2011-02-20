//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined' || ! Cn.Renderer || ! Cn.Renderer.Form || ! Cn.Renderer.Form.DateTime) {
	alert("Cn.Renderer.Form.DateTime.Text: [DEVELOPER] 'Cn/Renderer/Form/DateTime.js' must be included before referencing this code.");
}
//# </DebugCode>

<%@ Page Language="C#" %>
<%
	Cn.Settings oSettings;
	Cn.MultiArray oPicklistData;
	Hashtable h_sPicklists = new Hashtable();
	System.Web.HttpRequest oRequest = System.Web.HttpContext.Current.Request;
	string sSystemName = Cn.Tools.MakeString(oRequest.QueryString["SystemName"], "");
	int iWeekOfYearCalculation = Cn.Tools.MakeInteger(oRequest.QueryString["WeekOfYearCalculation"], 0);

		//#### If a sSystemName was provided, setup oSettings accordingly
	if (sSystemName.Length > 0) {
		oSettings = new Cn.Settings(sSystemName);
	}
		//#### Else no sSystemName was provided, so setup oSettings as the server default
	else {
		oSettings = new Cn.Settings(true);
	}

		//#### Set oSettings .LanguageCode based on the provided LanguageCode (which actually referes to the .Form's .EndUserMessagesLanguageCode)
	oSettings.LanguageCode = oRequest.QueryString["LanguageCode"];

	//##########
	//##########

		//#### Collect the .cnDate_MonthDaySuffix oPicklistData
	oPicklistData = oSettings.Values(Cn.Settings.enumInternationalizationPicklists.cnDate_MonthDaySuffix);

		//#### If the .cnDate_MonthDaySuffix oPicklistData was successfully collected above, set the MonthDaySuffix accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["MonthDaySuffix"] = Cn.Tools.ToJavaScriptArray(oPicklistData.Column("Description"));
	}

	//##########
	//##########

		//#### Collect the .cnDate_AbbreviatedWeekDayNames oPicklistData
	oPicklistData = oSettings.Values(Cn.Settings.enumInternationalizationPicklists.cnDate_AbbreviatedWeekDayNames);

		//#### If the .cnDate_AbbreviatedWeekDayNames oPicklistData was successfully collected above, set the AbbreviatedWeekDayNames accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["AbbreviatedWeekDayNames"] = Cn.Tools.ToJavaScriptArray(oPicklistData.Column("Description"));
	}

	//##########
	//##########

		//#### Collect the .cnDate_CalendarWeekDayNames oPicklistData
	oPicklistData = oSettings.Values(Cn.Settings.enumInternationalizationPicklists.cnDate_CalendarWeekDayNames);

		//#### If the .cnDate_CalendarWeekDayNames oPicklistData was successfully collected above, set the CalendarWeekDayNames accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["CalendarWeekDayNames"] = Cn.Tools.ToJavaScriptArray(oPicklistData.Column("Description"));
	}

	//##########
	//##########

		//#### Collect the .cnDate_WeekDayNames oPicklistData
	oPicklistData = oSettings.Values(Cn.Settings.enumInternationalizationPicklists.cnDate_WeekDayNames);

		//#### If the .cnDate_WeekDayNames oPicklistData was successfully collected above, set the WeekDayNames accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["WeekDayNames"] = Cn.Tools.ToJavaScriptArray(oPicklistData.Column("Description"));
	}

	//##########
	//##########

		//#### Collect the .cnDate_AbbreviatedMonthNames oPicklistData
	oPicklistData = oSettings.Values(Cn.Settings.enumInternationalizationPicklists.cnDate_AbbreviatedMonthNames);

		//#### If the .cnDate_AbbreviatedMonthNames oPicklistData was successfully collected above, set the AbbreviatedMonthNames accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["AbbreviatedMonthNames"] = Cn.Tools.ToJavaScriptArray(oPicklistData.Column("Description"));
	}

	//##########
	//##########

		//#### Collect the .cnDate_MonthNames oPicklistData
	oPicklistData = oSettings.Values(Cn.Settings.enumInternationalizationPicklists.cnDate_MonthNames);

		//#### If the .cnDate_MonthNames oPicklistData was successfully collected above, set the MonthNames accordingly
	if (oPicklistData != null && oPicklistData.RowCount > 0 && oPicklistData.Exists("Description")) {
		h_sPicklists["MonthNames"] = Cn.Tools.ToJavaScriptArray(oPicklistData.Column("Description"));
	}
%>


//########################################################################################################################
//# Supplemental DateTime class - DateTime.Text
//# 
//#     NOTE: The "WeekOfYearCalculation", "SystemName" and "LanguageCode" are included on the querystring to this script.
//#     Always Included By: Cn/Renderer/Form/DateTime.js
//########################################################################################################################
//# Last Code Review: April 18, 2006
Cn.Renderer.Form.DateTime.Text = function() {
	return {
		PreviousYear:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_PreviousYear).Replace("'", "\\'")); %>',
		PreviousMonth:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_PreviousMonth).Replace("'", "\\'")); %>',
		Today:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_Today).Replace("'", "\\'")); %>',
		NextMonth:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_NextMonth).Replace("'", "\\'")); %>',
		NextYear:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_NextYear).Replace("'", "\\'")); %>',
		Close:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_Close).Replace("'", "\\'")); %>',
		Clear:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_Clear).Replace("'", "\\'")); %>',
		Now:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_Now).Replace("'", "\\'")); %>',
		AM:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_AM).Replace("'", "\\'")); %>',
		PM:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_PM).Replace("'", "\\'")); %>',
		Delimiter:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_DateTime_Delimiter).Replace("'", "\\'")); %>',

		DaySuffixes:<% Response.Write(h_sPicklists["MonthDaySuffix"].ToString()); %>,
		aDays:<% Response.Write(h_sPicklists["AbbreviatedWeekDayNames"].ToString()); %>,
		cDays:<% Response.Write(h_sPicklists["CalendarWeekDayNames"].ToString()); %>,
		Days:<% Response.Write(h_sPicklists["WeekDayNames"].ToString()); %>,
		aMonths:<% Response.Write(h_sPicklists["AbbreviatedMonthNames"].ToString()); %>,
		Months:<% Response.Write(h_sPicklists["MonthNames"].ToString()); %>
	};

} (); //# Cn.Renderer.Form.DateTime.Text

	//#### Set the .WeekOfYearCalculation in .DateTime
Cn._.dt.WeekOfYearCalculation = <% Response.Write(iWeekOfYearCalculation); %>;

	//#### Now that the DateTime class is fully defined, .Initilize it
Cn._.dt.Initilize();
