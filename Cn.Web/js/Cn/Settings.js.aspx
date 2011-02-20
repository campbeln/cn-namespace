//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined') {
	alert("Cn.Settings: [DEVELOPER] 'Cn/Cn.js' must be included before referencing this code.");
}
//# </DebugCode>

<%@ Page Language="C#" %>
<%
	System.Web.HttpRequest oRequest = System.Web.HttpContext.Current.Request;
	Cn.Web.Framework.Renderer.RendererSettings oRendererSettings = new Cn.Web.Framework.Renderer.RendererSettings(oRequest.QueryString["SystemName"]);

		//#### Set oRendererSettings .Internationalization.LanguageCode based on the provided LanguageCode (which actually referes to the .Form's .EndUserMessagesLanguageCode)
	oRendererSettings.Internationalization.LanguageCode = oRequest.QueryString["LanguageCode"];
%>


//########################################################################################################################
//# Settings class
//# 
//#     Required Includes: Cn/Cn.js
//########################################################################################################################
//# Last Code Review: April 13, 2007
Cn.Settings = new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.s = this;

		//#### Declare the required 'public' 'properties'
		//####     NOTE: .JavaScriptDirectory does not include "Debug/" even if we are in debug mode (as debug mode is only set at runtime and .Settings are defined before runtime)
	this.PrimaryDelimiter = '<% Response.Write(Cn.Settings.PrimaryDelimiter.Replace("'", "\\'")); %>';
	this.SecondaryDelimiter = '<% Response.Write(Cn.Settings.SecondaryDelimiter.Replace("'", "\\'")); %>';

	this.CurrencySymbol = '<% Response.Write(oRendererSettings.Value(Cn.Settings.enumInternationalizationValues.cnCurrencySymbol).Replace("'", "\\'")); %>';

	this.ScriptFileExtension = '<% Response.Write(oRendererSettings.Value(Cn.Web.Framework.Renderer.enumSettingValues.cnScriptFileExtension).Replace("'", "\\'")); %>';
	this.InputErrorCSSClass = '<% Response.Write(oRendererSettings.Value(Cn.Web.Framework.Renderer.enumSettingValues.cnCSSClass_FormInputError).Replace("'", "\\'")); %>';
	this.FormElementPrefix = '<% Response.Write(oRendererSettings.Value(Cn.Web.Framework.Renderer.enumSettingValues.cnFormElementPrefix).Replace("'", "\\'")); %>';
	this.ErrorDIVCSSClass = '<% Response.Write(oRendererSettings.Value(Cn.Web.Framework.Renderer.enumSettingValues.cnCSSClass_PopUpErrorDIV).Replace("'", "\\'")); %>';

	this.BaseDirectory = '<% Response.Write(oRendererSettings.Value(Cn.Web.Framework.Renderer.enumSettingValues.cnBaseDirectory).Replace("'", "\\'")); %>\_Cn\';
	this.JavaScriptDirectory = '<% Response.Write(oRendererSettings.Value(Cn.Web.Framework.Renderer.enumSettingValues.cnJavaScriptDirectory).Replace("'", "\\'")); %>';


	//##########################################################################################
	//# 'Procedual Code'
	//##########################################################################################
		//#### If the .CurrencySymbol is a dollar sign, be sure it's properly escaped for any RegEx calls
	if (this.CurrencySymbol == '\$') {
		this.CurrencySymbol = '\\\$';
	}

} //# Cn.Settings
