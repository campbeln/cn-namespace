<%@ Page Language="C#" %>
<%
/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
	NameValueCollection oServerVars = HttpContext.Current.Request.ServerVariables;
	System.Web.HttpResponse oResponse = System.Web.HttpContext.Current.Response;
	System.Web.HttpRequest oRequest = System.Web.HttpContext.Current.Request;
	Cn.Configuration.Internationalization oIntl;
	string[] a_sFiles;
	string sLanguageCode;
	string sJavaScriptDirectory;
	string sJavaScriptURL;
	bool bDebug = Cn.Data.Tools.MakeBoolean(oRequest.QueryString["Debug"], false);
	int i;

//! neek
if (true) {
	throw new Exception("This method is not used by iSAAP", new InvalidOperationException());
}

		//#### Collect the oIntl based on Cn.Web.Settings' static .Internationalization
		//####     NOTE: This allows the developer to override Cn.Web.Settings' default .Internationalization with one of their own that will apply within these .js.* script files
	oIntl = Cn.Web.Settings.Internationalization;

		//#### Set the sLanguageCode based on the querystring passed ?LanguageCode= (which actually referes to Web.Settings.Current's .EndUserMessagesLanguageCode), validating it as we go
	sLanguageCode = oIntl.ValidateLanguageCode(oRequest.QueryString["LanguageCode"]);

		//#### Collect the sJavaScriptDirectory and sJavaScriptURL based on the bDebug status
	sJavaScriptDirectory = Cn.Web.Tools.CurrentPath(false) + (bDebug ? @"\Debug" : "");
	sJavaScriptURL = Cn.Web.JavaScript.BaseURL(bDebug);

		//#### Determine the passed Mode and process accordingly
	switch (Cn.Data.Tools.MakeString(oRequest.QueryString["Mode"], "").ToLower()) {
		case "cn": {
				//#### 
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Cn.js") + "\n");
//oResponse.Write("Cn.Include('" + sJavaScriptURL + "/Cn/Cn.js.aspx?Debug=" + bDebug + "&LanguageCode=" + sLanguageCode + "');\n");
		    oResponse.Write(Cn.Net.MakeHTTPRequest.GetFile(sJavaScriptURL + "Cn/Cn.js.aspx?Debug=" + bDebug + "&LanguageCode=" + sLanguageCode).ResponseFileAsString);
			
			//#### Since no browser info is being sent in the headers in the above call, we need to reset Cn._.cs.Language below
			oResponse.Write("\nCn._.cs.Language = '" + oRequest.ServerVariables["HTTP_ACCEPT_LANGUAGE"] + "'.split(',');\n");

			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Data\Tools.js") + "\n");

			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Dates\Tools.js") + "\n");

			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Web\Tools.js") + "\n");

			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Web\Inputs\Inputs.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Web\Inputs\ComboBox.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Web\Inputs\DateTime.js") + "\n");
//oResponse.Write("Cn.Include('" + sJavaScriptURL + "/Cn/Web/Inputs/DateTime.js.aspx?Debug=" + bDebug + "&LanguageCode=" + sLanguageCode + "');\n");
            oResponse.Write(Cn.Net.MakeHTTPRequest.GetFile(sJavaScriptURL + "Cn/Web/Inputs/DateTime.js.aspx?Debug=" + bDebug + "&LanguageCode=" + sLanguageCode).ResponseFileAsString);
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Web\Inputs\HTMLEditor.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Web\Inputs\Validation.js") + "\n");
//oResponse.Write("Cn.Include('" + sJavaScriptURL + "/Cn/Web/Inputs/Validation.js.aspx?Debug=" + bDebug + "&LanguageCode=" + sLanguageCode + "');\n");
	         oResponse.Write(Cn.Net.MakeHTTPRequest.GetFile(sJavaScriptURL + "Cn/Web/Inputs/Validation.js.aspx?Debug=" + bDebug + "&LanguageCode=" + sLanguageCode).ResponseFileAsString);
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Web\Inputs\MaxLength.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Web\Inputs\Radio.js") + "\n");

			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Web\Renderer\Form.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\Cn\Web\Renderer\UserSelectedStack.js") + "\n");
			break;
		}
		case "yui": {
				//#### 
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\yui\YAHOO.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\yui\Dom.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\yui\event.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\yui\calendar.js") + "\n");

			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\yui\container.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\yui\element.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\yui\simpleeditor.js") + "\n");

			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\yui\button.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\yui\menu.js") + "\n");
			oResponse.Write(Cn.Platform.Specific.ReadFromFile(sJavaScriptDirectory + @"\yui\editor.js") + "\n");
			break;
		}
		case "css": {
				//#### 
			oResponse.Write("Cn.IncludeCSS('" + sJavaScriptURL + "/yui/calendar.css');\n");
			oResponse.Write("Cn.IncludeCSS('" + sJavaScriptURL + "/yui/editor-core.css');\n");
			break;
		}
		default: {
			oResponse.Write("alert('DEVELOPER: You must pass in a reconized Mode.');\n");
			break;
		}
	}
%>