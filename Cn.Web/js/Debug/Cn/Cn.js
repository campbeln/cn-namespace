/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

/*
//# functions like an RPC, while defining the requested in the global namespace (if necessary) in a sync. mannor
Cn.$ = Cn.$ || new function(sNamespace, sFunction, sScriptURL) {
	//oMyFunction = Cn.$('My.Namespace', 'MyFunction', '/path/to/my.js');
	//oMyTextBox = Cn.$('Cn.Web.Tools', 'GetByID')('MyTextBox');
	//oWebTools = Cn.$('Cn.Web.Tools');
	//oWebTools = Cn.$('wt');

	var fReturn = function() { return null; };

	if (Cn.IsKnown(sNamespace)) {
		if (Cn.Exists(sNamespace)) {
			fReturn = window[sNamespace];
		}
		else {
			Cn.Define(sNamespace);
		}
	}

	if (sFunction && sFunction.length > 0 && fReturn[sFunction]) {
		fReturn = fReturn[sFunction];
	}

	return fReturn;
}

alert("CN JS Version July-6-2010 9:35am (per request by Elena O.)");
*/

//########################################################################################################################
//# Cn NameSpace
//# 
//#     Required Includes: [none]
//########################################################################################################################
//# Last Code Review: September 7, 2007
if (typeof (Cn) == "undefined" || ! Cn) {
    var Cn = {};
}

	//############################################################
	//# Define the Abbreviation namespace
	//############################################################
	//# Last Updated: August 23, 2007
	Cn._ = Cn._ || {},

	//############################################################
	//# Define the Configuration namespace
	//############################################################
	//# Last Updated: January 13, 2010
	Cn.Configuration = Cn.Configuration || {};

	//############################################################
	//# Define the Web namespace
	//############################################################
	//# Last Updated: January 13, 2010
	Cn.Web = Cn.Web || {};

	//############################################################
	//# Returns the referenced namespace (creating it if it did not already exist)
	//#     Examples:
	//#         Cn.Namespace("Renderer.Form");
	//#         Cn.Namespace("Cn.Renderer.Form");
	//#     Either of the above calls returns the .Form namespace under Cn.Renderer.
	//#     In both cases, the Renderer followed by the Form namespaces are created if they did not previously exist.
	//# 
	//#     NOTE: This function is a full re-implementation of "YAHOO.namespace" from Yahoo!'s "YAHOO.js" (http://developer.yahoo.net/yui/).
	//############################################################
	//# Last Updated: September 7, 2007
	Cn.Namespace = function(sNamespace) {
		var oReturn = null,
			a_sNamespaces, i
		;

			//#### If the passed sNamespace is holding a value
		if (sNamespace && sNamespace.length > 0) {
				//#### Default the oReturn value to a reference to Cn
			oReturn = Cn;

				//#### .split the sNamespace into its elements and determine the starting index of the loop
				//####     NOTE: 'Cn' is implied, so it is ignored if it is present within the sNamespace, hence the 'i =' logic below
			a_sNamespaces = sNamespace.split('.');
			i = ((a_sNamespaces[0] == 'Cn') ? 1 : 0);

				//#### Traverse the a_sNamespaces, starting at the above determined i
			for (i = i; i < a_sNamespaces.length; i++) {
					//#### Reset sNamespace for this loop, then set/create it within Cn/the oReturn value
					//####     NOTE: The validity of the current sNamespace is not checked as we assume that "you must be at least this smart to ride this ride"
				sNamespace = a_sNamespaces[i];
				oReturn[sNamespace] = oReturn[sNamespace] || {};
				oReturn = oReturn[sNamespace];
			}
		}

			//#### Return the above determined oReturn value to the caller
		return oReturn;
	};

	//############################################################
	//# Dynamicially includes the referenced script into the DOM of the current page.
	//# 
	//#     NOTE: The client browser must support dynamic appending elements into the DOM (Netscape 6+, Opera 7+, etc.).
	//#     NOTE: This function is a full re-implementation of "dhtmlLoadScript" and "staticLoadScript" by Moshe Moskowitz (http://www.codehouse.com/javascript/articles/external/).
	//############################################################
	//# Last Updated: February 24, 2010
	Cn.Include = function(sScriptURL) {
			//#### 
		if (! ScriptAlreadyIncluded(sScriptURL, 'script')) {
				//#### Try to dynamicially add a DOM object
			try {
					//#### Create a new oScript element and setup it's properties
				var oScript = document.createElement("script");
				oScript.type = "text/javascript";
				oScript.src = sScriptURL;

					//#### Append the oScript element into the HEAD of the document
				document.getElementsByTagName("head")[0].appendChild(oScript);
			}
				//#### If the dynamic DOM addition errored, try the static method
			catch (e) {
				document.write('<' + 'script src="' + sScriptURL + '" type="text/javascript"><' + '/script>');
			}
		}
	};

var ScriptAlreadyIncluded = function(sScriptURL, sTagName) {
	var a_oElements,
		i,
		bReturn = false
	;

	a_oElements = document.getElementsByTagName(sTagName);

	if (a_oElements) {
		for (i = 0; i < a_oElements.length; i++) {
			if (new String(a_oElements[i].src).indexOf(sScriptURL) > 0 || new String(a_oElements[i].href).indexOf(sScriptURL) > 0) {
				bReturn = true;
				break;
			}
		}
	}

	return bReturn;
};

Cn.IncludeCSS = function(sScriptURL) {
		//#### 
	if (! ScriptAlreadyIncluded(sScriptURL, 'link')) {
			//#### Try to dynamicially add a DOM object
		try {
				//#### Create a new oCSS element and setup it's properties
			var oCSS = document.createElement("link");
			oCSS.type = "text/css";
			oCSS.rel = "stylesheet";
			oCSS.media = "screen";
			oCSS.href = sScriptURL;

				//#### Append the oCSS element into the HEAD of the document
			document.getElementsByTagName("head")[0].appendChild(oCSS);
		}
			//#### If the dynamic DOM addition errored, try the static method
		catch (e) {
			document.write('<' + 'link href="' + sScriptURL + '" type="text/css" rel="stylesheet" media="screen"><' + '/script>');
		}
	}
};

	//############################################################
	//# Dynamicially includes the referenced namespace into the DOM of the current page.
	//############################################################
	//# Last Updated: June 18, 2010
	Cn.Define = function(sNamespace) {
		var sLanguageCode = Cn._.ws.EndUserMessagesLanguageCode,
			sFileExtension = Cn._.wj.ServerSideScriptFileExtension,
			sPath = Cn._.wj.BaseDirectory,
			bDefined
		;
			//#### Ensure the passed sNamespace is a string
		sNamespace = new String(sNamespace);

			//#### Detemine if the passed sNamespace has been previously bDefined
//!		bDefined = (typeof(window[sNamespace]) == 'undefined' || ! window[sNamespace]) ? false : true;
		bDefined = eval('typeof(' + sNamespace + ') != "undefined"');

			//#### If the passed sNamespace has not been bDefined
		if (! bDefined) {
				//#### Determine the passed sNamespace and process accordingly
			switch (sNamespace.toLowerCase()) {
					//#### Cn.
				case 'cn.data.tools':
				case 'cn._.dt': {
					Cn.Include(sPath + 'Cn/Data/Tools.js');
					break;
				}
				case 'cn.dates.tools':
				case 'cn._.dst': {
					Cn.Define('cn.data.tools');
					Cn.Include(sPath + 'Cn/Dates/Tools.js');
					break;
				}

					//#### Cn.Web.
				case 'cn.web.tools':
				case 'cn._.wt': {
					Cn.Define('Cn.Data.Tools');
					Cn.Define('YAHOO.util.Dom');
					Cn.Define('YAHOO.util.Event');
					Cn.Include(sPath + 'Cn/Web/Tools.js');
					break;
				}
				case 'cn.web.inputs':
				case 'cn._.wi': {
					Cn.Define('Cn.Data.Tools');
					Cn.Define('Cn.Web.Tools');
					Cn.Include(sPath + 'Cn/Web/Inputs/Inputs.js');
					break;
				}
				case 'cn.web.inputs.combobox':
				case 'cn._.wic': {
					Cn.Define('Cn.Web.Tools');
					Cn.Define('Cn.Web.Inputs');
					Cn.Include(sPath + 'Cn/Web/Inputs/ComboBox.js');
					break;
				}
				case 'cn.web.inputs.datetime':
				case 'cn._.wid': {
					Cn.IncludeCSS('yui/calendar.css');
					Cn.Define('Cn.Dates.Tools');
					Cn.Define('Cn.Data.Tools');
					Cn.Define('Cn.Web.Tools');
					Cn.Define('YAHOO.util.Event');
					Cn.Define('YAHOO.widget.Calendar');
					Cn.Include(sPath + 'Cn/Web/Inputs/DateTime.js');
					Cn.Include(sPath + 'Cn/Web/Inputs/DateTime.js.' + sFileExtension + '?LanguageCode=' + sLanguageCode);
					break;
				}
				case 'cn.web.inputs.htmleditor':
				case 'cn._.wih': {
					Cn.IncludeCSS('yui/editor-core.css');
					Cn.Define('Cn.Data.Tools');
					Cn.Define('Cn.Web.Tools');
					Cn.Define('YAHOO.util.Dom');
					Cn.Define('YAHOO.util.Event');
					Cn.Define('YAHOO.widget.SimpleEditor');
					Cn.Define('YAHOO.widget.Editor');
					Cn.Include(sPath + 'Cn/Web/Inputs/HTMLEditor.js');
					break;
				}
				case 'cn.web.inputs.maxlength':
				case 'cn._.wim': {
					Cn.Define('Cn.Data.Tools');
					Cn.Define('Cn.Web.Tools');
					Cn.Define('Cn.Web.Inputs');
					Cn.Define('Cn.Web.Inputs.Validation.Errors');
					Cn.Define('YAHOO.util.Event');
					Cn.Include(sPath + 'Cn/Web/Inputs/MaxLength.js');
					break;
				}
				case 'cn.web.inputs.radio':
				case 'cn._.wir': {
					Cn.Define('Cn.Data.Tools');
					Cn.Define('Cn.Web.Tools');
					Cn.Define('YAHOO.util.Event');
					Cn.Include(sPath + 'Cn/Web/Inputs/Radio.js');
					break;
				}
				case 'cn.web.inputs.validation':
				case 'cn._.wiv':
				case 'cn.web.inputs.validation.errors':
				case 'cn._.wive':
				case 'cn.web.inputs.validation.errors.ui':
				case 'cn._.wiveu': {
					Cn.Define('Cn.Data.Tools');
					Cn.Define('Cn.Web.Tools');
					Cn.Define('Cn.Web.Inputs');
					Cn.Define('YAHOO.util.Dom');
					Cn.Include(sPath + 'Cn/Web/Inputs/Validation.js');
					Cn.Include(sPath + 'Cn/Web/Inputs/Validation.js.' + sFileExtension + '?LanguageCode=' + sLanguageCode);
					break;
				}

					//#### Cn.Renderer.
				case 'cn.web.renderer.complexsorter': {
					Cn.Include(sPath + 'Cn/Web/Renderer/ComplexSorter.js');
					break;
				}
				case 'cn.web.renderer.form': {
					Cn.Define('Cn.Web.Tools');
					Cn.Define('Cn.Web.Inputs');
					Cn.Include(sPath + 'Cn/Web/Renderer/Form.js');
					break;
				}
				case 'cn.web.renderer.userselectedstack': {
					Cn.Include(sPath + 'Cn/Web/Renderer/UserSelectedStack.js');
					break;
				}

					//#### YAHOO.
				case 'yahoo': {
					Cn.Include(sPath + 'yui/YAHOO.js');
					break;
				}
				case 'yahoo.util.dom': {
					Cn.Define('YAHOO');
					Cn.Include(sPath + 'yui/Dom.js');
					break;
				}
				case 'yahoo.util.event': {
					Cn.Define('YAHOO');
					Cn.Include(sPath + 'yui/event.js');
					break;
				}
				case 'yahoo.widget.calendar': {
					Cn.Define('YAHOO');
					Cn.Include(sPath + 'yui/calendar.js');
					break;
				}

				case 'yahoo.widget.simpleeditor': {
					Cn.Define('YAHOO');
					Cn.Define('YAHOO.widget.Panel');
					Cn.Define('YAHOO.util.Element');
					Cn.Include(sPath + 'yui/simpleeditor.js');
					break;
				}
				case 'yahoo.widget.panel': {
					Cn.Define('YAHOO');
					Cn.Include(sPath + 'yui/panel.js');
					break;
				}
				case 'yahoo.util.element': {
					Cn.Define('YAHOO');
					Cn.Include(sPath + 'yui/element.js');
					break;
				}

				case 'yahoo.widget.editor': {
					Cn.Define('YAHOO');
					Cn.Define('YAHOO.widget.Menu');
					Cn.Define('YAHOO.widget.Button');
					Cn.Include(sPath + 'yui/editor.js');
					break;
				}
				case 'yahoo.widget.button': {
					Cn.Define('YAHOO');
					Cn.Include(sPath + 'yui/button.js');
					break;
				}
				case 'yahoo.widget.menu': {
					Cn.Define('YAHOO');
					Cn.Include(sPath + 'yui/menu.js');
					break;
				}
			}
		}
	};


//########################################################################################################################
//# Internationalization class
//# 
//#     Required Includes: Cn/Cn.js
//########################################################################################################################
//# Last Code Review: 
Cn.Configuration.Internationalization = Cn._.ci || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.ci = this;


	//##########################################################################################
	//# 'Public Properties'
	//##########################################################################################
	this.Value = {};							//# This stub is populated as necessary by other scripts
	this.Values = {};							//# This stub is populated as necessary by other scripts


}; //# Cn.Configuration.Internationalization


//########################################################################################################################
//# Settings class
//# 
//#     Required Includes: Cn/Cn.js
//########################################################################################################################
//# Last Code Review: January 13, 2010
Cn.Configuration.Settings = Cn._.cs || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.cs = this;


	//##########################################################################################
	//# 'Public Properties'
	//##########################################################################################
  //this.PrimaryDelimiter = "?";				//# This stub is populated as necessary by other scripts
  //this.SecondaryDelimiter = "?";				//# This stub is populated as necessary by other scripts
  //this.Language = "?";						//# This stub is populated as necessary by other scripts


}; //# Cn.Configuration.Settings


//########################################################################################################################
//# JavaScript class
//# 
//#     Required Includes: Cn/Cn.js
//########################################################################################################################
//# Last Code Review: 
Cn.Web.JavaScript = Cn._.wj || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wj = this;


	//##########################################################################################
	//# 'Public Properties'
	//##########################################################################################
  //this.ServerSideScriptFileExtension = "?";	//# This stub is populated as necessary by other scripts

	//############################################################
	//# Provides a location to store temporary variables without poluting the JavaScript domain.
	//# 
	//#     NOTE: Any object/entry can be created under this object, though care should be taken to avoid collisions with any other functionality using the Workspace concurrently.
	//#     It is advised to use a variable name based on the name and class path of the function utilizing the Workspace, ie:
	//#         Cn._.wj.Workspace.wiv_ValidationJavaScript = {};
	//#     Where "wiv_" represents the related namespace "Cn._.wiv" (aka "Cn.Web.Inputs.Validation") and "ValidationJavaScript" represents the name of the function utilizing the .Workspace
	//############################################################
	//# Last Updated: January 13, 2010
	this.Workspace = {};

	//############################################################
	//# Provides a location to store Input metadata information without poluting the JavaScript domain.
	//# 
	//#     NOTE: This should be used as follows:
	//#         Cn._.wj.ElementWorkspace[Cn._.wt.getByID(oInput).id] = {};
	//#     Where the key to the ElementWorkspace array is the oInput object reference.
	//############################################################
	//# Last Updated: January 14, 2010
	this.ElementWorkspace = new Array();


}; //# Cn.Web.JavaScript


//########################################################################################################################
//# Settings class
//# 
//#     Required Includes: Cn/Cn.js
//########################################################################################################################
//# Last Code Review: 
Cn.Web.Settings = Cn._.ws || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.ws = this;


	//##########################################################################################
	//# 'Public Properties'
	//##########################################################################################
  //this.EndUserMessagesLanguageCode = "?";		//# This stub is populated as necessary by other scripts
	this.Value = {};							//# This stub is populated as necessary by other scripts


}; //# Cn.Web.JavaScript
