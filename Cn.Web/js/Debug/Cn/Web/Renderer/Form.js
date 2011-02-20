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
	alert("Cn.Web.Renderer.Form: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
else if (! Cn._.wt) {
	alert("Cn.Web.Renderer.Form: [DEVELOPER] 'Cn/Web/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.wi) {
	alert("Cn.Web.Renderer.Form: [DEVELOPER] 'Cn/Web/Inputs/Inputs.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### If our namespace is not setup, do so now
if (! Cn.Web || ! Cn.Web.Renderer) {
	Cn.Namespace("Cn.Web.Renderer");
}


//########################################################################################################################
//# Form class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Settings.js.*, Cn/Inputs/Inputs.js
//########################################################################################################################
//# Last Code Review: December 9, 2009
Cn.Web.Renderer.Form = Cn._.wrf || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wrf = this;

		//#### Declare the required 'private' variables
	var g_oWebSettingsValue = Cn._.ws.Value,
		g_oElementWorkspace = Cn._.wj.ElementWorkspace,
		g_oWebTools = Cn._.wt,
		g_oInputs = Cn._.wi
	;

		//#### Declare the required 'public' enums
		//####     NOTE: The enumRecordTrackerModes directly related to Renderer's enumRecordTrackerModes. Any changes to these values must be reflected in these enums!
	this.enumRecordTrackerModes = {
		cnUnknown : -1,
		cnNew : 0,
		cnMissing : 1,
		cnExisting : 2
	};


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Returns the properly formatted input name to the caller
	//############################################################
	//# Last Updated: January 13, 2010
	this.InputName = function(sInputAlias, iRecordIndex) {
		return sInputAlias + '_' + iRecordIndex;
	};

	//############################################################
	//# Returns a reference to the passed oFormReference/sInputName+iRecordIndex to the caller
	//############################################################
	//# Last Updated: January 13, 2010
	this.Input = function(oFormReference, sInputAlias, iRecordIndex) {
	        //#### Pass the call off to g_oInputs.Get, returning it's oInput, else an object specifying which input is missing
		return (
		    g_oInputs.Input(oFormReference, this.InputName(sInputAlias, iRecordIndex)) ||
		    {
				UnknownInput : true,
		        FormID : g_oWebTools.GetIdentifier(oFormReference),
		        InputAlias : sInputAlias,
		        RecordIndex : iRecordIndex
		    }
		);
	};

	//############################################################
	//# Determines record mode of the passed oFormReference/iRecordIndex
	//############################################################
	//# Last Updated: December 8, 2009
	this.RecordMode = function(oFormReference, iRecordIndex) {
		var oRecordTracker = this.Input(oFormReference, g_oWebSettingsValue.DOMElementPrefix + 'cnRecordTracker', iRecordIndex),
			sValue = g_oInputs.Value(oRecordTracker).toLowerCase(),
			eReturn = this.enumRecordTrackerModes.cnUnknown
		;

			//#### If we have a sValue to process
		if (sValue.length > 0) {
				//#### Else if this is a .cnExisting record, reset our eReturn value accordingly
			if (sValue.substr(0, 8) == 'existing') {
				eReturn = this.enumRecordTrackerModes.cnExisting;
			}
				//#### If this is a .cnNew record, reset our eReturn value accordingly
			else if (sValue.substr(0, 3) == 'new') {
				eReturn = this.enumRecordTrackerModes.cnNew;
			}
				//#### Else if this is a .cnMissing record, reset our eReturn value accordingly
			else if (sValue.substr(0, 7) == 'missing') {
				eReturn = this.enumRecordTrackerModes.cnMissing;
			}
		}

			//#### Return the above determined eReturn value to the caller
		return eReturn;
	}

	//############################################################
	//# Determines if the record for the passed oFormReference/iRecordIndex exists
	//############################################################
	//# Last Updated: December 8, 2009
	this.RecordExists = function(oFormReference, iRecordIndex) {
			//#### Return based on if the passed oFormReference/iRecordIndex is not .cnUnknown
		return (this.RecordMode(oFormReference, iRecordIndex) != this.enumRecordTrackerModes.cnUnknown);
	};

	//############################################################
	//# Determines if the passed iRecordIndex needs to be validated
	//# 
	//#    NOTE: If somehow the user submits a new record to the server w/o the JavaScript catching the errors and server side errors are generated, this code will not catch any subsequent errors as the previosuly set values will be seen as defaults (and the record will therefore not be checked). This is a non issue if you properly check the submitted data on both the client AND server sides.
	//############################################################
	//# Last Updated: December 8, 2009
	this.ProcessRecord = function(oFormReference, iRecordIndex, a_sInputAliases) {
		var bReturn = false
			i
		;

			//#### Determine the .RecordMode and process accordingly
		switch (this.RecordMode(oFormReference, iRecordIndex)) {
				//#### If this is a .cnNew record
			case this.enumRecordTrackerModes.cnNew: {
					//#### If this is a single new record, we want to force the check (as there is nothing else on the screen) so set our bReturn value to true
				if (iRecordIndex == 1 && ! this.RecordExists(oFormReference, 2)) {
					bReturn = true;
				}
					//#### Else this is not a single .cnNew record
				else {
						//#### If some a_sInputAliases were passed
					if (a_sInputAliases && a_sInputAliases.length > 0) {
							//#### Traverse the passed a_sInputAliases
						for (i = 0; i < a_sInputAliases.length; i++) {
								//#### If the user has changed the value(s) of the current a_sInputAliases, flip bReturn and fall from the loop
							if (g_oInputs.WasUpdated(this.Input(oFormReference, a_sInputAliases[i], iRecordIndex))) {
								bReturn = true;
								break;
							}
						}
					}
				}
				break;
			}
				//#### If this is a .cnExisting record, then we need to .Process(the)Record so flip bReturn to true
			case this.enumRecordTrackerModes.cnExisting: {
				bReturn = true;
				break;
			}
		}

			//#### Return the above set bReturn to the caller
		return bReturn;
	};

	//############################################################
	//# Clears all of the errors for the referenced record index.
	//############################################################
	//# Last Updated: December 8, 2009
	this.ClearRecordErrors = function(oFormReference, iRecordIndex, a_sInputAliases) {
		var oInput;

			//#### If some a_sInputAliases were passed
		if (a_sInputAliases && a_sInputAliases.length > 0) {
				//#### Traverse the passed a_sInputAliases, .Clear'ing the .Errors as we go
			for (var i = 0; i < a_sInputAliases.length; i++) {
					//#### Collect the current oInput, then if it has a .Validater, .Clear(it's)Error
				oInput = this.Input(oFormReference, a_sInputAliases[i], iRecordIndex);
				if (oInput && g_oElementWorkspace[oInput.id] && g_oElementWorkspace[oInput.id].Validater) {
					g_oElementWorkspace[oInput.id].Validater.ClearError();
				}
			}
		}
	};

}; //# Cn.Renderer.Form
