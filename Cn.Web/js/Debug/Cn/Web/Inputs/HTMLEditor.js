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
	alert("Cn.Web.Inputs.HTMLEditor: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
else if (! Cn._.dt) {
	alert("Cn.Web.Inputs.HTMLEditor: [DEVELOPER] 'Cn/Data/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.wt) {
	alert("Cn.Web.Inputs.HTMLEditor: [DEVELOPER] 'Cn/Web/Tools.js' must be included before referencing this code.");
}

	//#### If all of the required YAHOO objects do not exist, popup the related error message
else if (typeof(YAHOO) == 'undefined') {
	alert("Cn.Web.Inputs.ComboBox: [DEVELOPER] 'yui/YAHOO.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Dom) {
	alert("Cn.Web.Tools: [DEVELOPER] 'yui/Dom.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Event) {
	alert("Cn.Web.Tools: [DEVELOPER] 'yui/Event.js' must be included before referencing this code.");
}
else if (! YAHOO.widget.SimpleEditor || ! YAHOO.widget.Panel || ! YAHOO.util.Element) {
	alert("Cn.Web.Tools: [DEVELOPER] 'yui/SimpleEditor.js', 'yui/Container.js' and 'yui/Element.js' must be included before referencing this code.");
}
else if (! YAHOO.widget.Editor || ! YAHOO.widget.Menu || ! YAHOO.widget.Button) {
	alert("Cn.Web.Tools: [DEVELOPER] 'yui/Editor.js', 'yui/Menu.js' and 'yui/Button.js' must be included before referencing this code.");
}

//# </DebugCode>

	//#### If our namespace is not setup, do so now
if (! Cn.Web || ! Cn.Web.Inputs) {
	Cn.Namespace("Cn.Web.Inputs");
}


//########################################################################################################################
//# HTMLEditor class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Data/Tools.js, Cn/Web/Tools.js, [yui/Yahoo.js], yui/Event.js, yui/Dom.js
//########################################################################################################################
//# Last Code Review: 
Cn.Web.Inputs.HTMLEditor = Cn._.wih || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wih = this;

		//#### Declare the required 'private' variables
	var g_oDataTools = Cn._.dt,
		g_oWebTools = Cn._.wt,
		a_oEditors = new Array()
	;


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Defines the referenced HTMLEditor.
	//############################################################
	//# Last Updated: February 19, 2010
	this.Add = function(oTextArea, oConfiguration, iMaxLength, bSimpleEditor, bRender) {
		var oReturn = null;

			//#### Ensure the passed oTextArea is a reference
		oTextArea = g_oWebTools.GetByID(oTextArea);

			//#### If the passed oTextArea is a valid reference to an actual textarea and it does not already .Exists
		if (oTextArea && oTextArea.type && oTextArea.type.toLowerCase() == 'textarea' &&
			! this.Exists(oTextArea)
		) {
				//#### Ensure the passed bSimpleEditor and bRender are boolean values, defaulting both to true if they are non-boolean (or were missing)
			bSimpleEditor = g_oDataTools.MakeBoolean(bSimpleEditor, true);
			bRender = g_oDataTools.MakeBoolean(bRender, true);

				//#### Ensure the .container is properly set within the passed oConfiguration, creating one ourselves if we have to (so we can refer to it within the .EventHandler)
				//####     NOTE: According to the API docs for YUI.RichTextEditor, if a .container is not specified one is appended to the document.body. So with this logic we are mearly doing this ourselves as we cannot seem to get access to the .container from the .Editor object
			oConfiguration = oConfiguration || {};
			if (! oConfiguration.container || ! g_oWebTools.GetByID(oConfiguration.container)) {
				oConfiguration.container = document.createElement('div');
//				oConfiguration.container.id = g_oWebTools.GetIdentifier(oTextArea) + "_editor";
				document.body.appendChild(oConfiguration.container);
			}

				//#### Init the entry for the sTextAreaID
			a_oEditors[oTextArea] = {
//				SimpleEditor: bSimpleEditor,
				Rendered: bRender,
				TextArea: oTextArea,
				Container: oConfiguration.container,
				Canvas: null,
				MaxLength: iMaxLength,
				Editor: null
			};

				//#### If this was a bSimpleEditor request, setup the oTextArea within our a_oEditors as a .SimpleEditor
			if (bSimpleEditor) {
				a_oEditors[oTextArea].Editor = new YAHOO.widget.SimpleEditor(oTextArea, oConfiguration);
			}
				//#### Else this as a standard .Editor request, so setup the oTextArea within our a_oEditors an .Editor
			else {
				a_oEditors[oTextArea].Editor = new YAHOO.widget.Editor(oTextArea, oConfiguration);
			}

				//#### If we are supposed to bRender the .Editor, do so now
			if (bRender) {
				Render(oTextArea);
			}

				//#### Reset our oReturn value to the above created .Editor
			oReturn = a_oEditors[oTextArea].Editor;
		}

			//#### Return the above determined oReturn value to the caller
		return oReturn;
	};

	//############################################################
	//# Determines if the passed oTextArea exists as a defined HTMLEditor
	//############################################################
	//# Last Updated: February 19, 2010
	this.Exists = function(oTextArea) {
			//#### Ensure the passed oTextArea is a reference
		oTextArea = g_oWebTools.GetByID(oTextArea);

			//#### Return based on if the oTextArea .Exists within our a_oEditors
		return (a_oEditors && a_oEditors[oTextArea]);
	};

	//############################################################
	//# Renders the referenced HTMLEditor if it hasn't already been rendered.
	//############################################################
	//# Last Updated: February 19, 2010
	this.Render = function(oTextArea) {
		var oEditorBodies,
			bReturn = false
		;

			//#### Ensure the passed oTextArea is a reference
		oTextArea = g_oWebTools.GetByID(oTextArea);

			//#### If the oTextArea .Exists and it has not yet been .Rendered
		if (this.Exists(oTextArea) && a_oEditors[oTextArea].Rendered == false) {
				//#### .render the .Editor now and flip the oTextArea's .Rendered and our bReturn value
			a_oEditors[oTextArea].Editor.render();
			a_oEditors[oTextArea].Rendered = true;
			bReturn = true;

				//#### Collect the oEditorBodies (always selecting the first returned <body> tag as there should only be 1), setting the result into the .Canvas
			oEditorBodies = oEditorEntity.Container.getElementsByTagName('body');
			if (oEditorBodies && oEditorBodies.length == 1) {
				a_oEditors[oTextArea].Canvas = g_oWebTools.GetByID(oEditorBodies[0]);
			}
//# <DebugCode>
			else {
				alert("Cn.Web.Inputs.HTMLEditor: [DEVELOPER] Unable to locate YUI.RichTextEditor's IFrame.Body element.");
			}
//# </DebugCode>
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

/*	//############################################################
	//# Determines the length of the string within the referenced HTMLEditor.
	//############################################################
	//# Last Updated: February 19, 2010
	this.Length = function(oTextArea) {
		var oEditor,
			iReturn = -1
		;

			//#### Ensure the passed oTextArea is a reference
		oTextArea = g_oWebTools.GetByID(oTextArea);

			//#### If the oTextArea .Exists and it has been .Rendered
		if (this.Exists(oTextArea) && a_oEditors[oTextArea].Rendered == true) {
				//#### Collect the oEditor, .clean(it's)HTML, then collect the .length from the .getEditorHTML into our iReturn value
			oEditor = a_oEditors[oTextArea].Editor;
			oEditor.cleanHTML();
			iReturn = oEditor.getEditorHTML().length;
		}

			//#### Return the above determined iReturn value to the caller
		return iReturn;
	};
*/

	//############################################################
	//# 
	//############################################################
	//# Last Updated: February 19, 2010
	this.Data = function(oTextArea) {
		var oReturn = null;

			//#### Ensure the passed oTextArea is a reference
		oTextArea = g_oWebTools.GetByID(oTextArea);

			//#### If the passed oTextArea .Exists, reset ou oRetun value to it's entry under a_oEditors
		if (this.Exists(oTextArea)) {
			oReturn = a_oEditors[oTextArea];
		}

			//#### Return the above determined oReturn value to the caller
		return oReturn;
	}


}; //# Cn.Web.Inputs.HTMLEditor
