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
	alert("Cn.Web.Inputs.MaxLength: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
else if (! Cn._.dt) {
	alert("Cn.Web.Inputs.MaxLength: [DEVELOPER] 'Cn/Data/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.wt) {
	alert("Cn.Web.Inputs.MaxLength: [DEVELOPER] 'Cn/Web/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.wi) {
	alert("Cn.Web.Inputs.MaxLength: [DEVELOPER] 'Cn/Inputs/Inputs.js' must be included before referencing this code.");
}
else if (! Cn._.wive) {
	alert("Cn.Web.Inputs.MaxLength: [DEVELOPER] 'Cn/Inputs/Validation.js' must be included before referencing this code.");
}

	//#### If all of the required YAHOO objects do not exist, popup the related error message
else if (typeof(YAHOO) == 'undefined') {
	alert("CnWeb.Inputs.MaxLength: [DEVELOPER] 'yui/YAHOO.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Event) {
	alert("Cn.Web.Inputs.MaxLength: [DEVELOPER] 'yui/Event.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### If our namespace is not setup, do so now
if (! Cn.Web || ! Cn.Web.Inputs) {
	Cn.Namespace("Cn.Web.Inputs");
}


//########################################################################################################################
//# MaxLength class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Tools.js, Cn/Renderer/Form/Form.js, Cn/Renderer/Form/Errors.js, yui/YAHOO.js, yui/Event.js
//########################################################################################################################
//# Last Code Review: 
Cn.Web.Inputs.MaxLength = Cn._.wim || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wim = this;

		//#### Declare the required 'private' variables
	var g_oElementWorkspace = Cn._.wj.ElementWorkspace,
		ga_bEnabled = new Array(),
		g_oDataTools = Cn._.dt,
		g_oWebTools = Cn._.wt,
		g_oInputs = Cn._.wi,
		g_oErrors = Cn._.wive
	;

		//#### Declare the required 'public' 'properties'
	this.EffectedTags = new Array('textarea');		//# Default the .EffectedTags array to a single entry for Textareas (as 99%+ of the time that is the only tag to be effected by this class)


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Determines if the passed sFormID has HTML textarea's with defined maxlength attributes that have MaxLength checking enabled against them.
	//############################################################
	//# Last Updated: April 13, 2007
	this.Enabled = function(sFormID) {
		var bReturn = false;

			//#### If the passed sFormID is within our ga_bEnabled forms (and has been enabled of course), reset the bReturn value to true
		if (ga_bEnabled[sFormID]) {
			bReturn = true;
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Enables MaxLength checking on all HTML textarea's with defined maxlength attributes under the passed sFormID.
	//############################################################
	//# Last Updated: September 7, 2007
	this.Enable = function(sFormID) {
			//#### If the passed sFormID isn't within our ga_bEnabled forms
		if (! ga_bEnabled[sFormID]) {
			var a_oChildren;

				//#### Collect the a_oChildren representing the .EffectedTags
				//####     NOTE: We are assuming here that the .EffectedTags are (still) an array of strings (it's up to the developer to set a proper array of strings value if they decide to change this 'property'... fingers crossed ;)
			a_oChildren = g_oWebTools.ChildrenOf(sFormID, this.EffectedTags)

				//#### If some a_oChildren were successfully collected above
			if (a_oChildren && a_oChildren.length > 0) {
					//#### Set the sFormID as being enabled
				ga_bEnabled[sFormID] = true;

					//#### Traverse the a_oChildren
				for (i = 0; i < a_oChildren.length; i++) {
						//#### If the current a_oElement has a defined maxlength attribute, attach the onChange listener to it
					if (g_oDataTools.MakeNumeric(g_oWebTools.GetAttribute(a_oChildren[i], 'maxlength'), 0) > 0) {
						YAHOO.util.Event.addListener(a_oChildren[i], "onchange", this.CheckLength);
					}
				}
			}
		}
	};

	//############################################################
	//# Callback function to check the length of the calling form element.
	//############################################################
	//# Last Updated: January 14, 2010
	this.CheckLength = function(oElement) {
			//#### If the passed oElement seems valid
		if (oElement && oElement.length && oElement.form) {
			var iMaxLength = g_oDataTools.MakeNumeric(g_oWebTools.GetAttribute(oElement, 'maxlength'), 0),
				eError = (g_oElementWorkspace[oElement.id] && g_oElementWorkspace[oElement.id].Validater ? g_oElementWorkspace[oElement.id].Validater.Error : null)
			;

				//#### If the oElement has a valid iMaxLength
				//####     NOTE: The iMaxLength > 0 is really redundant thanks to the checking we do before we attach the onChange listener, but is included here as good practice
			if (iMaxLength > 0) {
					//#### If the eError code registered against the oElement is either .cnNoError or .cnIncorrectLength
				if (eError == g_oErrors.enumInputErrors.cnNoError || eError == g_oErrors.enumInputErrors.cnIncorrectLength) {
						//#### If the oElement's .length is longer then the defined iMaxLength
						//####     NOTE: The iMaxLength > 0 is really redundant thanks to the checking we do before we attach the onChange listener, but is included here as good practice
					if (oElement.length > iMaxLength) {
							//#### .Set the g_oErrors against the oElement as having the .cnIncorrectLength
						g_oErrors.Set(oElement, g_oInputs.enumInputErrors.cnIncorrectLength, rForm.enumDataTypes.cnChar, '');
					}
						//#### Else the .length is within the range, so
					else {
							//#### Remove the g_oErrors against the oElement (.Set'ing it as having .cnNoError)
						g_oErrors.Set(oElement, g_oInputs.enumInputErrors.cnNoError, rForm.enumDataTypes.cnChar, '');
					}
				}
			}
		}
	};

}; //# Cn.Web.Inputs.MaxLength
