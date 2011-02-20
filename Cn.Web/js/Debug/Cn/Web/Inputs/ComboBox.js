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
	alert("Cn.Web.Inputs.ComboBox: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
else if (! Cn._.wt) {
	alert("Cn.Web.Inputs.ComboBox: [DEVELOPER] 'Cn/Web/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.wi) {
	alert("Cn.Web.Inputs.ComboBox: [DEVELOPER] 'Cn/Web/Inputs/Inputs.js' must be included before referencing this code.");
}

	//#### If all of the required YAHOO objects do not exist, popup the related error message
else if (typeof(YAHOO) == 'undefined') {
	alert("Cn.Web.Inputs.ComboBox: [DEVELOPER] 'yui/YAHOO.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Event) {
	alert("Cn.Web.Inputs.ComboBox: [DEVELOPER] 'yui/Event.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### If our base namespace is not setup, do so now
if (! Cn.Web || ! Cn.Web.Inputs) {
	Cn.Namespace("Cn.Web.Inputs");
}


//########################################################################################################################
//# ComboBox class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Tools.js, Cn/Settings.js.*, [yui/Yahoo.js], yui/Event.js
//########################################################################################################################
//# Last Code Review: December 23, 2009
Cn.Web.Inputs.ComboBox = Cn._.wic || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wic = this;

		//#### Declare the required 'private' variables
		//####     NOTE: These are only used within .AttachListeners which is called via YUI's .onContentReady so it doesn't have access to these variables
  //var g_oWebSettingsValue = Cn._.ws.Value;
	var g_oElementWorkspace = Cn._.wj.ElementWorkspace,
		g_oWebTools = Cn._.wt
	;

		//#### Declare the required 'private' 'constants'
	var g_cARROWWIDTH = 16;

	
	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Defines the referenced ComboBox.
	//############################################################
	//# Last Updated: January 11, 2010
	this.Add = function(oInput) {
		oInput = g_oWebTools.GetByID(oInput);
		var sDIVName = g_oSettings.DOMElementPrefix + g_oWebTools.GetIdentifier(oInput) + "_DIV";

			//#### If the passed oInput hasn't be setup as a ComboBox, set the .onContentReady event to add it as a comboBox
		if (! this.Exists(oInput)) {
			YAHOO.util.Event.onContentReady(sDIVName, AttachListeners, { Input: oInput, ArrowWidth: g_cARROWWIDTH }, Cn._.wic);
		}
	};

	//############################################################
	//# Determines if the passed oInput exists as a defined ComboBox
	//############################################################
	//# Last Updated: March 2, 2010
	this.Exists = function(oInput) {
		oInput = g_oWebTools.GetByID(oInput);
		return (oInput && g_oElementWorkspace[oInput.id] && g_oElementWorkspace[oInput.id].ComboBox);
	};


	//##########################################################################################
	//# 'Protected', Pseudo-'Static' Functions
	//# 
	//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
	//##########################################################################################
	//############################################################
	//# Attaches the listeners to the referenced input.
	//#
	//#     NOTE: YUI seems to count the argument number and acts differently. So the docs call for a function with a single argument for the oEvent (even though the oArbitraryObject is passed in if provided via the second argument)
	//############################################################
	//# Last Updated: January 13, 2010
	var AttachListeners = function(oEvent) {
		var oWebSettingsValue = Cn._.ws.Value,	//# NOTE: Since this is called via .addListener, the full resolution is required for the function call
			oWebTools = Cn._.wt,				//# NOTE: Since this is called via .addListener, the full resolution is required for the function call
			oInput = oWebTools.GetByID(arguments[1].Input),
			sInputID = oWebTools.GetIdentifier(oInput),
			oSelect = oWebTools.GetByID(oWebSettingsValue.DOMElementPrefix + sInputID + "_Select"),
			sDIVName = oWebSettingsValue.DOMElementPrefix + sInputID + "_DIV",
			oDIV = oWebTools.GetByID(sDIVName),
			iX, iY, iArrowWidth = arguments[1].ArrowWidth
		;

			//#### If we were able to successfully collect the oInput, oSelect and oDIV
			//####     NOTE: We don't bother checking arguments[1] as it's only via .addListener that we are called, so it's all good!
		if (oInput && oSelect && oDIV) {
				//#### Default the oSelect's .selectedIndex
			oSelect.selectedIndex = -1;

				//#### If the requesting browser is Netscape Navigator v4.x, simply .Show the oDIV
			if (oWebTools.IsNN4()) {
				oWebTools.Show(oDIV);
			}
				//#### Else the requesting browser is not Netscape Navigator v4.x
			else {
					//#### Set the oSelect's .Width based on the oInput's .Width and the ._cArrowWidth
				oWebTools.Width(oSelect, (oWebTools.Width(oInput) + iArrowWidth));

					//#### .Clip the oDIV so that only the oSelect's dropdown arrow is visiable (-2 so that the dropdown arrow covers the right 2px border of the oInput)
				oWebTools.Clip(oDIV, 0, oWebTools.Width(oDIV), oWebTools.Height(oDIV), (oWebTools.Width(oDIV) - iArrowWidth - 2));

					//#### Determine the iX/iY corrds of the oSelect's dropdown arrow in relation to the oInput
					//####     NOTE: Since we know that the select will always be the last thing in the oDIV, we align the bottoms (hence the iY calculation)
				iX = ((oWebTools.Left(oInput) + oWebTools.Width(oInput)) - (oWebTools.Width(oDIV) - iArrowWidth));
				iY = (oWebTools.Top(oInput) - oWebTools.Height(oDIV) + oWebTools.Height(oInput));

					//#### Attach a new .EventHandler instance onto the oInput, then set it to listen for 'OnChange' events
				g_oElementWorkspace[oInput.id] = g_oElementWorkspace[oInput.id] || {};
				g_oElementWorkspace[oInput.id].ComboBox = new EventHandler(oInput, oSelect);
				YAHOO.util.Event.addListener(oInput, 'change', g_oElementWorkspace[oInput.id].ComboBox.OnChange, null, oInput);

					//#### Move the oDIV into position then .Show it
				oWebTools.Left(oDIV, iX);
				oWebTools.Top(oDIV, iY);
				oWebTools.Show(oDIV);
			}
		}
	};


    //########################################################################################################################
    //# Validater class that is attached to inputs
    //# 
    //#     Required Includes: Cn/Inputs/Inputs.js
    //########################################################################################################################
    //# Last Code Review: December 23, 2009
    function EventHandler(oInput, oSelect) {
			//#### Declare the required 'private' variables
		var g_oInputs = Cn._.wi,
			g_oInput = g_oInputs.Input(oInput),
			g_oSelect = g_oInputs.Input(oSelect)
		;


		//############################################################
		//# Updates the related ComboBox's value based on the user selected list value.
		//############################################################
		//# Last Updated: December 23, 2009
		this.OnChange = function() {
				//#### Update the .value of the g_oInput, then reset the g_oSelect's .selectedIndex
			g_oInput.value = g_oSelect.options[g_oSelect.selectedIndex].text;
			g_oSelect.selectedIndex = -1;
		};
	}; //# Cn.Web.Inputs.ComboBox.EventHandler


}; //# Cn.Web.Inputs.ComboBox
