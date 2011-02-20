//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined' || ! Cn.Renderer || ! Cn.Renderer.Form || ! Cn.Renderer.Form.Errors) {
	alert("Cn.Renderer.Form.Errors.Messages: [DEVELOPER] 'Cn/Renderer/Form/Errors.js' must be included before referencing this code.");
}
//# </DebugCode>

<%@ Page Language="C#" %>
<%
	Cn.Settings oSettings;
	System.Web.HttpRequest oRequest = System.Web.HttpContext.Current.Request;
	string sSystemName = oRequest.QueryString["SystemName"];

		//#### If a sSystemName was provided, setup oSettings accordingly
	if (sSystemName != null && sSystemName.Length > 0) {
		oSettings = new Cn.Settings(sSystemName);
	}
		//#### Else no sSystemName was provided, so setup oSettings as the server default
	else {
		oSettings = new Cn.Settings(true);
	}

		//#### Set oSettings .LanguageCode based on the provided LanguageCode (which actually referes to the .Form's .EndUserMessagesLanguageCode)
	oSettings.LanguageCode = oRequest.QueryString["LanguageCode"];
%>


//########################################################################################################################
//# Supplemental Errors class - Errors.Messages
//# 
//#     NOTE: The "SystemName" and "LanguageCode" are included on the querystring to this script.
//#     Always Included By: Cn/Renderer/Form/Errors.js
//########################################################################################################################
//# Last Code Review: March 30, 2006
Cn.Renderer.Form.Errors.Messages = function() {
	return {
		ValueIsRequired:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_ValueIsRequired).Replace("'", "\\'")); %>',
		IncorrectLength:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_IncorrectLength).Replace("'", "\\'")); %>',
		Boolean:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Boolean).Replace("'", "\\'")); %>',
		Integer:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Integer).Replace("'", "\\'")); %>',
		Float:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Float).Replace("'", "\\'")); %>',
		Currency:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Currency).Replace("'", "\\'")); %>',
		DateTime:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_DateTime).Replace("'", "\\'")); %>',
		GUID:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_GUID).Replace("'", "\\'")); %>',
		StringBased:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Other).Replace("'", "\\'")); %>',
		NotWithinPicklist:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_NotWithinPicklist).Replace("'", "\\'")); %>',
		Custom:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_Custom).Replace("'", "\\'")); %>',
		UnknownOrUnsupportedType:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_UnknownOrUnsupportedType).Replace("'", "\\'")); %>',
		MissingInput:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_MissingInput).Replace("'", "\\'")); %>',
		NoError:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_NoError).Replace("'", "\\'")); %>',
		UnknownErrorCode:'<% Response.Write(oSettings.Value(Cn.Settings.enumInternationalizationValues.cnEndUserMessages_UnknownErrorCode).Replace("'", "\\'")); %>'
	};

} (); //# Cn.Renderer.Form.Errors.Messages
