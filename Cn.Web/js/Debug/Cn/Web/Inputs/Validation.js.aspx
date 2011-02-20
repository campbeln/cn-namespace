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
	string sLanguageCode;

		//#### Collect the oIntl based on Cn.Web.Settings' static .Internationalization
		//####     NOTE: This allows the developer to override Cn.Web.Settings' default .Internationalization wiht one of thier own that will apply within these .js.* script files
	oIntl = Cn.Web.Settings.Internationalization;

		//#### Set the sLanguageCode based on the querystring passed ?LanguageCode= (which actually referes to Web.Settings.Current's .EndUserMessagesLanguageCode), validating it as we go
	sLanguageCode = oIntl.ValidateLanguageCode(oRequest.QueryString["LanguageCode"]);
%>

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined' || ! Cn._.ws || ! Cn._.ci) {
	alert("Cn.Settings: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
//# </DebugCode>


//########################################################################################################################
//# Required settings-related values for Validation.js
//# 
//#     NOTE: The "SystemName" and "LanguageCode" are included on the querystring to this script.
//#     Always Included Along With: Cn/Inputs/Validation.js
//########################################################################################################################
//# Last Code Review: January 13, 2010

	//#### Set the .Value's under Cn.Web.Settings
Cn._.ws.Value.CSSClass_FormInputError = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(Cn.Web.Settings.Value(Cn.Web.Settings.enumSettingValues.cnCSSClass_FormInputError), "'")); %>';
Cn._.ws.Value.CSSClass_PopUpErrorDIV = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(Cn.Web.Settings.Value(Cn.Web.Settings.enumSettingValues.cnCSSClass_PopUpErrorDIV), "'")); %>';

	//#### Set the .Value's under Cn.Configuration.Internationalization
Cn._.ci.Value.EndUserMessages_ValueIsRequired = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_ValueIsRequired, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_IncorrectLength = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectLength, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_Custom = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_Custom, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_UnknownOrUnsupportedType = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_UnknownOrUnsupportedType, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_MissingInput = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_MissingInput, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_NoError = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_NoError, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_UnknownErrorCode = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_UnknownErrorCode, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_Alert = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_Alert, sLanguageCode), "'"));  %>';
Cn._.ci.Value.EndUserMessages_IncorrectDataType_Boolean = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Boolean, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_IncorrectDataType_Integer = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Integer, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_IncorrectDataType_Float = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Float, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_IncorrectDataType_Currency = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Currency, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_IncorrectDataType_DateTime = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_DateTime, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_IncorrectDataType_GUID = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_GUID, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_IncorrectDataType_Other = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Other, sLanguageCode), "'")); %>';
Cn._.ci.Value.EndUserMessages_IncorrectDataType_NotWithinPicklist = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_NotWithinPicklist, sLanguageCode), "'")); %>';
Cn._.ci.Value.Localization_CurrencySymbol = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(oIntl.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnLocalization_CurrencySymbol, sLanguageCode), "'")); %>';

	//#### If the .Localization_CurrencySymbol is a dollar sign, be sure it's properly escaped for any RegEx calls
if (Cn._.ci.Value.Localization_CurrencySymbol == '\$') {
	Cn._.ci.Value.Localization_CurrencySymbol = '\\\$';
}
