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
	bool bDebug = Cn.Data.Tools.MakeBoolean(oRequest.QueryString["Debug"], false);

		//#### Collect the oIntl based on Cn.Web.Settings' static .Internationalization
		//####     NOTE: This allows the developer to override Cn.Web.Settings' default .Internationalization with one of their own that will apply within these .js.* script files
	oIntl = Cn.Web.Settings.Internationalization;

		//#### Set the sLanguageCode based on the querystring passed ?LanguageCode= (which actually referes to Web.Settings.Current's .EndUserMessagesLanguageCode), validating it as we go
	sLanguageCode = oIntl.ValidateLanguageCode(oRequest.QueryString["LanguageCode"]);
%>

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined' || ! Cn._.cs || ! Cn._.wj || ! Cn._.ws) {
	alert("Cn.Settings: [DEVELOPER] 'Cn/Cn.js' must be included before referencing this code.");
}
//# </DebugCode>


//########################################################################################################################
//# Required settings-related values for Cn.js
//# 
//#     Always Included Along With: Cn.js
//########################################################################################################################
//# Last Code Review: January 13, 2010

	//#### Set the properties under Cn.Configuration.Settings
Cn._.cs.PrimaryDelimiter = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(Cn.Configuration.Settings.PrimaryDelimiter, "'")); %>';
Cn._.cs.SecondaryDelimiter = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(Cn.Configuration.Settings.SecondaryDelimiter, "'")); %>';
Cn._.cs.Language = '<% Response.Write(Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"]); %>'.split(',');

	//#### Set the properties under Cn.Web.JavaScript
	//####     NOTE: .BaseDirectory points to the base JavaScript directory (taking into account if we are in Debug mode or not, hence the difference from Cn.Web.Settings.Value.UIDirectory)
Cn._.wj.ServerSideScriptFileExtension = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(Cn.Web.JavaScript.ServerSideScriptFileExtension, "'")); %>';
Cn._.wj.BaseDirectory = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(Cn.Web.JavaScript.BaseDirectory(bDebug), "'")); %>/';

	//#### Set the .Value's under Cn.Web.Settings
Cn._.ws.Value.DOMElementPrefix = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(Cn.Web.Settings.Value(Cn.Web.Settings.enumSettingValues.cnDOMElementPrefix), "'")); %>';
Cn._.ws.Value.UIDirectory = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(Cn.Web.Settings.Value(Cn.Web.Settings.enumSettingValues.cnUIDirectory), "'")); %>';
Cn._.ws.EndUserMessagesLanguageCode = '<% Response.Write(Cn.Web.Inputs.Tools.EscapeCharacters(sLanguageCode, "'")); %>';
