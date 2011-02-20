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
	alert("Cn.Web.Inputs.Radio: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
else if (! Cn._.dt) {
	alert("Cn.Web.Inputs.Radio: [DEVELOPER] 'Cn/Data/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.wt) {
	alert("Cn.Web.Inputs.Radio: [DEVELOPER] 'Cn/Web/Tools.js' must be included before referencing this code.");
}

	//#### If all of the required YAHOO objects do not exist, popup the related error message
else if (typeof(YAHOO) == 'undefined') {
	alert("Cn.Web.Inputs.Radio: [DEVELOPER] 'yui/YAHOO.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Event) {
	alert("Cn.Web.Inputs.Radio: [DEVELOPER] 'yui/Event.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### If our base namespace is not setup, do so now
if (! Cn.Web || ! Cn.Web.Inputs) {
	Cn.Namespace("Cn.Web.Inputs");
}


//########################################################################################################################
//# Radio class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Data/Tools.js, Cn/Web/Tools.js, [yui/Yahoo.js], yui/Event.js
//########################################################################################################################
//# Last Code Review: February 18, 2010
Cn.Web.Inputs.Radio = Cn._.wir || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wir = this;

		//#### Declare the required 'private' variables
	var g_oDataTools = Cn._.dt,
		g_oThis = this
	;

		//#### Declare the required 'private' variables
	var oTimeoutID;


	//##########################################################################################
	//# 'Public' 'Properties'
	//##########################################################################################
	//############################################################
	//# Reference to the last clicked radio button element (primarily utilized internally)
	//############################################################
	//# Last Updated: February 18, 2010
	this.Last = null;

	//############################################################
	//# Defines the period of a reconized second click (in 1/1000ths of a second)
	//############################################################
	//# Last Updated: February 18, 2010
	this.Delay = 2000;


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Attaches the Radio.EventHandler to the provided list of inputs, or all radio inputs if the passed list is empty.
	//############################################################
	//# Last Updated: July 16, 2010
	this.AttachListeners = function(a_oElements, bAttachOnLoad, fCallbackHook) {
		var oArgument = {
			Elements: a_oElements,
			Hook: fCallbackHook
		};

			//#### 
		if (g_oDataTools.MakeBoolean(bAttachOnLoad, true)) {
			YAHOO.util.Event.addListener(window, 'load', DoAttachListeners, oArgument, this);
		}
			//#### 
		else {
			DoAttachListeners(null, oArgument);
		}
	};


	//############################################################
	//# 
	//#
	//#     NOTE: This "overload" is required because the YUI seems to count the argument number and acts differently. So the docs call for a function with a single argument for the oEvent (even though the oArbitraryObject is passed in if provided via the second argument)
	//############################################################
	//# Last Updated: July 15, 2010
	var DoAttachListeners = function(oEvent) {
		var a_oElements = arguments[1].Elements,
			fCallbackHook = arguments[1].Hook,
			i
		;

			//#### If the passed a_oElements are valid
		if (a_oElements && a_oElements.length > 0) {
				//#### Traverse the passed a_oElements, ensuring each is a reference
			for (i = 0; i < a_oElements.length; i++) {
				a_oElements[i] = Cn._.wt.GetByID(a_oElements[i]);
			}
		}
			//#### Else the passed a_oElements was invalid, so collect all of the document's inputs
		else {
			a_oElements = document.getElementsByTagName("input");
		}

			//#### If we have a_oElements to traverse
		if (a_oElements && a_oElements.length > 0) {
				//#### Traverse the a_oElements, attaching our .EventHandler to each that is a radio input
			for (i = 0; i < a_oElements.length; i++) {
				if (a_oElements[i] && a_oElements[i].type && a_oElements[i].type.toLowerCase() == 'radio') {
					YAHOO.util.Event.addListener(a_oElements[i], 'click', Cn._.wir.EventHandler, { Radio: a_oElements[i], Hook: fCallbackHook }, this);
				}
			}
		}
	};

	//############################################################
	//# Handles the OnClick event of the related radio input
	//# 
	//#     NOTE: This "overload" is required because the YUI seems to count the argument number and acts differently. So the docs call for a function with a single argument for the oEvent (even though the oArbitraryObject is passed in if provided via the second argument)
	//############################################################
	//# Last Updated: July 15, 2010
	this.EventHandler = function(oEvent) {
		var oRadio, fCallbackHook;

			//#### .Cancel(the)Delay of the last oTimeoutID and determine the oRadio (allowing for the YUI call's object structure)
		g_oThis.CancelDelay();
		oRadio = (arguments[1] && arguments[1].Radio ? arguments[1].Radio : oEvent);
		fCallbackHook = (arguments[1] && arguments[1].Hook ? arguments[1].Hook : null);

			//#### If the oRadio is valid, .checked and matches the .Last .checked oRadio, uncheck it and reset .Last
		if (oRadio && oRadio.checked && oRadio == g_oThis.Last) {
			oRadio.checked = false;
			g_oThis.Last = null;
		}
			//#### Else this oRadio has not been clicked twice in succession within the alloted .Delay
		else {
				//#### Reset .Last to the current oRadio and .setTimeout for the defined .Delay (if we have a valid .Delay)
			g_oThis.Last = oRadio;
			if (g_oThis.Delay > 0) {
				oTimeoutID = setTimeout("Cn._.wir.Last = null;", g_oThis.Delay);
			}
		}

		if (fCallbackHook && typeof(fCallbackHook) == 'function') {
			fCallbackHook(oRadio);
		}
	};

	//############################################################
	//# Cancels the last click delay timer
	//############################################################
	//# Last Updated: April 27, 2010
	this.CancelDelay = function() {
			//#### If we have a valid .Delay, .clear(the)Timeout and reset the oTimeout to null
		if (g_oThis.Delay > 0) {
			clearTimeout(oTimeoutID);
			oTimeoutID = null;
		}
	};


}; //# Cn.Web.Inputs.Radio
