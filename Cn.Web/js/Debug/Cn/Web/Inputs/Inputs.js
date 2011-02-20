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
	alert("Cn.Web.Inputs & .Form: [DEVELOPER] 'Cn/Cn.js' & 'Cn/Cn.js.*' must be included before referencing this code.");
}
else if (! Cn._.dt) {
	alert("Cn.Web.Inputs & .Form: [DEVELOPER] 'Cn/Data/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.wt) {
	alert("Cn.Web.Inputs & .Form: [DEVELOPER] 'Cn/Web/Tools.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### If our namespace is not setup, do so now
if (! Cn.Web) {
	Cn.Namespace("Cn.Web");
}


//########################################################################################################################
//# Inputs class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Tools.js
//########################################################################################################################
//# Last Code Review: 
Cn.Web.Inputs = Cn._.wi || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wi = this;

		//#### Declare the required 'private' variables
	var g_oWebSettingsValue = Cn._.ws.Value,
		g_oDataTools = Cn._.dt,
		g_oWebTools = Cn._.wt,
		g_oThis = this
	;

		//#### Declare the required 'private' 'constants'
	var g_cREADONLYIDSUFFIX = '_ReadOnly';

		//#### Declare the required 'public' enums
		//####     NOTE: The enumDataTypes directly related to DataSource.MetaData's enumDataTypes. Any changes to these values must be reflected in these enums!
//! move into validation?
	this.enumDataTypes = {
		cnUnknown : 0,
		cnBoolean : 1,
		cnInteger : 2,
		cnFloat : 4,
		cnCurrency : 8,
		cnChar : 16,
		cnLongChar : 32,
		cnDateTime : 64,
		cnBinary : 128,
		cnGUID : 256,
		cnUnsupported : 32768
	};


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Returns a reference to the requested HMTL input
	//# 
	//#     NOTE: HTML inputs that exist outside of a HTML form may not be returned as they should not have a valid .form reference
	//############################################################
	//# Last Updated: January 13, 2010
	this.Input = function() {
		var oReturn = null;

			//#### If we have arguments to process
		if (arguments) {
				//#### Determine the number of passed arguments and process accordingly
			switch (arguments.length) {
				case 1: {
					oReturn = GetInputByID(arguments[0]);
					break;
				}		
				case 2: {
					oReturn = GetInputByFormElements(arguments[0], arguments[1]);
					break;
				}		
			}
		}

			//#### Return the above determined oReturn value to the caller
		return oReturn;
	};
		//############################################################
		//# Returns a reference to the passed sID to the caller
		//############################################################
		//# Last Updated: December 8, 2009
		var GetInputByID = function(oInput) {
			var oReturn = null,
				oFormReference
			;

				//#### Ensure the passed oInput is a reference (as an ID is a valid argument) and retrieve it's related oFormReference
			oInput = g_oWebTools.GetByID(oInput);
			oFormReference = g_oThis.RelatedForm(oInput);

				//#### If we were able to get an oInput and oFormReference
			if (oInput && oFormReference) {
					//#### If the .id or .name exist within the oFormReference, we have a valid oInput to oReturn
				if ((oInput.id && oFormReference.elements[oInput.id]) || (oInput.name && oFormReference.elements[oInput.name])) {
					oReturn = oInput;
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		};
		//############################################################
		//# Returns a reference to the passed oFormReference/sInputName to the caller
		//############################################################
		//# Last Updated: December 8, 2009
		var GetInputByFormElements = function(oFormReference, sInputName) {
			var oReturn = null;

				//#### Ensure the passed oFormReference is a reference (auto-magicially grabbing the .form reference if an input reference was passed)
			oFormReference = g_oThis.RelatedForm(oFormReference);

				//#### If the oFormReference was successfully collected above
			if (oFormReference) {
					//#### If the passed sInputName is defined within the oFormReference, set our oReturn value to it, else to null (this always ensures that we return 'null' for unknown oFormReference/sInputName combinations)
				oReturn = (oFormReference.elements[sInputName] ? oFormReference.elements[sInputName] : null);
			}
		  //	//#### Else we were unable to collect the oFormReference, so try to collect the oReturn value with the passed sInputName as an ID
		  //	//####     NOTE: We do not do this as the caller specified a Form/Input pair, and if we cannot find that paring we return null
		  //else {
		  //	oReturn = GetInputByID(sInputName);
		  //}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		};

	//############################################################
	//# Returns a reference to the requested HMTL form
	//############################################################
	//# Last Updated: January 13, 2010
    this.RelatedForm = function(oFormReference) {
		var oReturn = null;

            //#### Ensure the passed oFormReference is a reference (auto-magicially grabbing the .form reference if an input reference was passed)
        oFormReference = g_oWebTools.GetByID(oFormReference);
        if (oFormReference && oFormReference.form) { oFormReference = oFormReference.form; }

			//#### If we were able to get a oFormReference
		if (oFormReference) {
				//#### If the .id or .name exist within the document.forms, we have a valid oFormReference to oReturn
			if ((oFormReference.id && document.forms[oFormReference.id]) || (oFormReference.name && document.forms[oFormReference.name])) {
				oReturn = oFormReference;
			}
		}

			//#### Return the above determined oReturn value to the caller
        return oReturn;
    };

	//############################################################
	//# Returns the label associated with the passed oInput
	//############################################################
	//# Last Updated: January 13, 2010
	this.RelatedLabel = function(oInput) {
		var oReturn = null,
			a_oLabels, sInputID, i
		;

			//#### Ensure the passed oInput is a reference
		oInput = this.Input(oInput);

			//#### If a valid oInput was passed in
		if (oInput) {
				//#### Collect the a_oLabels and the sInputID
			a_oLabels = document.getElementsByTagName("label");
			sInputID = g_oWebTools.GetIdentifier(oInput);

				//#### Traverse the a_oLabels
			for (i = 0; i < a_oLabels.length; i++) {
					//#### If this a_oLabel is .htmlFor the sInputID, set it into our oReturn value and fall from the loop
				if (a_oLabels[i].htmlFor == sInputID) {
					oReturn = a_oLabels[i];
					break;
				}
			}
		}

			//#### Return the above determined oReturn value to the caller
		return oReturn;

/*
        var a_oLabels = document.getElementsByTagName('label'),
			oReturn = null,
			sID
		;

            //#### Ensure the passed oInput is a reference and collect it's sID
        oInput = g_oWebTools.GetByID(oInput);
        sID = g_oWebTools.GetIdentifier(oInput);

            //#### If we were able to collect some a_oLabels and we have a valid oInput
        if (a_oLabels && a_oLabels.length > 0 && oInput) {
                //#### Traverse the a_oLabels
            for (var i = 0; i < a_oLabels.length; i++) {
                    //#### If the current a_oLabels' .For (note the cAsINg!) matches our oInput
                if (g_oWebTools.GetByID(a_oLabels[i].For) == oInput) {
                    oReturn = a_oLabels[i];
                    break;
                }
            }
        }

            //#### If we still have not collected a oReturn value, attempt to collect the pseudo-label for .cnReadOnly fields
        if (! oReturn) {
            g_oWebTools.GetByID(g_oWebSettingsValue.DOMElementPrefix + sID + g_cREADONLYIDSUFFIX);
        }

            //#### Return the above determined oReturn value to the caller
        return oReturn;
*/
	};

	//############################################################
	//# Returns the assoicated Label name for the passed sInputName
	//############################################################
	//# Last Updated: January 13, 2010
	this.ReadOnlyID = function(oInput) {
		return g_oWebSettingsValue.DOMElementPrefix + g_oWebTools.GetIdentifier(oInput) + g_cREADONLYIDSUFFIX;
	}

	//############################################################
	//# Returns the value(s) of the passed oInput as a string
	//############################################################
	//# Last Updated: August 30, 2007
	this.Value = function(oInput) {
		var a_sValues = this.Values(oInput),
			sReturn = ""
		;

			//#### If we were able to find some a_sValues for the passed oInput
		if (a_sValues) {
				//#### Determine the .length of the a_sValues and process accordingly
			switch (a_sValues.length) { 
					//#### If only a single a_sValue was found above, reset our sReturn value to it
				case 1: {
					sReturn = g_oDataTools.MakeString(a_sValues[0], "");
					break;
				}
					//#### Else more then one a_sValue was found, so force it into a string
//! Allow for some sort of bDelimit function?
				default: {
					sReturn = g_oDataTools.MakeString(a_sValues.join(""), "");
					break;
				}
			}
		}

			//#### Return the above determined sReturn value to the caller
		return sReturn;
	};

	//############################################################
	//# Returns the value(s) of the passed oInput as an array
	//############################################################
	//# Last Updated: February 22, 2010
	this.Values = function(oInput) {
		var a_oInput = new Array(),
			a_sReturn = new Array(''),
			iReturnIndex = 0,
			i
		;

			//#### Ensure the passed oInput is a reference
//		oInput = this.Input(oInput);

			//#### If a valid oInput was passed in
		if (oInput) {
				//#### If the passed oInput represents a single input, place it into the 0th element in a_oInput
			if (oInput.type) {
				a_oInput[0] = oInput;
			}
				//#### Else the passed oInput (probably) represents multiple inputs, so it into a_oInput
			else {
				a_oInput = oInput;
			}

				//#### If the above determined a_oInput is an array
			if (g_oDataTools.IsNumeric(a_oInput.length)) {
					//#### Traverse the above determined a_oInput
				for (i = 0; i < a_oInput.length; i++) {
						//#### If the current a_oInput is valid
					if (a_oInput[i] && a_oInput[i].type) {
							//#### Determine the .toLowerCase'd .type of the current a_oInput and process accordingly
						switch (a_oInput[i].type.toLowerCase()) { 
							case 'text':
							case 'textarea':
							case 'password':
							case 'hidden':
							case 'button':
							case 'submit':
							case 'reset': {
									//#### Add the current a_oInput's .value into the a_sReturn array (post inc'ing iReturnIndex)
								a_sReturn[iReturnIndex++] = a_oInput[i].value;
								break;
							}

							case 'radio':
							case 'checkbox': {
									//#### If the current a_oInput is .checked, add it's .value into the a_sReturn array (post inc'ing iReturnIndex)
								if (a_oInput[i].checked) {
									a_sReturn[iReturnIndex++] = a_oInput[i].value;
								}
								break;
							}

							case 'image': {
									//#### Add the current a_oInput's identifier into the a_sReturn array (post inc'ing iReturnIndex)
								a_sReturn[iReturnIndex++] = g_oWebTools.GetIdentifier(a_oInput[i]);
								break;
							}

							case 'select-one': {
									//#### Add the current a_oInput's selected .options' .value into the a_sReturn array (post inc'ing iReturnIndex)
								a_sReturn[iReturnIndex++] = a_oInput[i].options[a_oInput[i].selectedIndex].value;
								break;
							}

							case 'select-multiple': {
									//#### Traverse the current a_oInput's .options
								for (var j = 0; j < a_oInput[i].options.length; j++) {
										//#### If the current .option is has been .selected, add it's .value into the a_sReturn array (post inc'ing iReturnIndex)
									if (a_oInput[i].options[j].selected) {
										a_sReturn[iReturnIndex++] = a_oInput[i].options[j].value;
									}
								}
								break;
							}

							case 'file':
							default: {
									//#### Else the current a_oInput is of an unreconized or unprocessable type, so set null into the a_sReturn array for the current a_oInput (post inc'ing iReturnIndex)
								a_sReturn[iReturnIndex++] = null;
								break;
							}
						}
					}
				}
			}
		}

			//#### If no .Values were found above, reset out a_sReturn value to an empty Array
		if (iReturnIndex == 0) {
			a_sReturn = new Array();
		}

			//#### Return the above determined a_sReturn value to the caller
		return a_sReturn;
	};

	//############################################################
	//# Determines if the passed oInput has been changed by the user
	//#    NOTE: This code was inspired by the example given at: http://codestore.net/store.nsf/unid/DOMM-4UTKE6?OpenDocument
	//############################################################
	//# Last Updated: February 22, 2010
	this.WasUpdated = function(oInput) {
		var a_oInput = new Array(),
			bReturn = false,
			iReturnIndex = 0,
			oInput, i
		;

			//#### Ensure the passed oInput is a reference
//		oInput = this.Input(oInput);

			//#### If a valid oInput was passed
		if (oInput) {
				//#### If the passed oInput represents a single input, place it into the 0th element in a_oInput
			if (oInput.type) {
				a_oInput[0] = oInput;
			}
				//#### Else the passed oInput (probably) represents multiple inputs, so it into a_oInput
			else {
				a_oInput = oInput;
			}

				//#### If the above determined a_oInput is an array
			if (! isNaN(a_oInput.length)) {
					//#### Traverse the above determined a_oInput
				for (i = 0; i < a_oInput.length; i++) {
						//#### If the current a_oInput is valid
					if (a_oInput[i] && a_oInput[i].type) {
							//#### Determine the .toLowerCase'd .type of the current a_oInput and process accordingly
						switch (a_oInput[i].type.toLowerCase()) { 
							case 'text':
							case 'textarea':
							case 'password': {
									//#### If the a_oInput's .value differs from its .defaultValue, reset the bReturn value to true
								if (a_oInput[i].value != a_oInput[i].defaultValue) {
									bReturn = true;
								}
								break;
							}

							case 'radio':
							case 'checkbox': {
									//#### If the a_oInput's .checked value differs from its .defaultChecked value, reset the bReturn value to true
								if (a_oInput[i].checked != a_oInput[i].defaultChecked) {
									bReturn = true;
								}
								break;
							}

							case 'select-one': 
							case 'select-multiple': {
								var j;
								var bDefaultValueSpecified = false;

									//#### Traverse the a_oInput's .options to determine if the developer specified any as .defaultSelected
								for (j = 0; j < a_oInput[i].options.length; j++) {
										//#### If the current .option is set as .defaultSelected, flip bDefaultValueSpecified and fall from the loop
									if (a_oInput[i].options[j].defaultSelected) {
										bDefaultValueSpecified = true;
										break;
									}
								}

									//#### (Re)Traverse the a_oInput's .options
								for (j = 0; j < a_oInput[i].options.length; j++) {
										//#### If the developer set some .defaultSelected .options
									if (bDefaultValueSpecified) {
											//#### If the a_oInput's .selected value differs from its .defaultSelected value, reset the bReturn value to true and fall from the loop
										if (a_oInput[i].options[j].selected != a_oInput[i].options[j].defaultSelected) {
											bReturn = true;
											break;
										}
									}
										//#### Else there are not any .defaultSelected .options set, so if the user has selected something other then the first .option, reset the bReturn value to true
									else if (a_oInput[i].options[j].selected && j != 0) {
										bReturn = true;
									}
								}
								break;
							}
						}
					}

						//#### If the bReturn value was flipped above, fall from the loop
					if (bReturn) {
						break;
					}
				}
			}
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

}; //# Cn.Web.Inputs



//########################################################################################################################
//# Inputs.Form class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Tools.js
//########################################################################################################################
//# Last Code Review: September 6, 2007
Cn.Web.Inputs.Form = Cn._.wif || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wif = this;

		//#### Declare the required 'private' variables
	var g_oInputs = Cn._.wi;


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Safely submits the referenced HTML form
	//############################################################
	//# Last Updated: December 8, 2009
	this.Submit = function(oFormReference) {
    	    //#### Ensure the passed oFormReference is a reference (auto-magicially grabbing the .form reference if an input reference was passed)
        oFormReference = g_oInputs.RelatedForm(oFormReference);

			//#### If the oFormReference is a valid .form, .submit it
		if (oFormReference && oFormReference.submit) {
			oFormReference.submit();
		}
	};

	//############################################################
	//# Determines if any of the passed sFormName's inputs have been changed by the user
	//############################################################
	//# Last Updated: December 8, 2009
	this.WasUpdated = function(oFormReference) {
		var i, bReturn = false;

    	    //#### Ensure the passed oFormReference is a reference (auto-magicially grabbing the .form reference if an input reference was passed)
        oFormReference = g_oInputs.RelatedForm(oFormReference);

			//#### If the oFormReference was successfully collected
		if (oFormReference && oFormReference.elements) {
				//#### Traverse the oFormReference's .elements
			for (i = 0; i < oFormReference.elements.length; i++) {
					//#### If the current .elements .WasUpdated, flip our bReturn value and fall from the loop
				if (g_oInputs.WasUpdated(oFormReference.elements[i])) {
					bReturn = true;
					break;
				}
			}
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Retrieves the inputs that have been changed under the referenced form
	//############################################################
	//# Last Updated: December 8, 2009
	this.UpdatedInputs = function(oFormReference) {
		var a_oReturn = new Array();

    	    //#### Ensure the passed oFormReference is a reference (auto-magicially grabbing the .form reference if an input reference was passed)
        oFormReference = g_oInputs.RelatedForm(oFormReference);

			//#### If we were able to collect the oFormReference and it has .elements to traverse
		if (oFormReference && oFormReference.elements) {
				//#### Traverse the .elements
			for (var i = 0; i < oFormReference.elements.length; i++) {
					//#### If the current .element .WasUpdated, .push it onto the sReturn value
				if (g_oInputs.WasUpdated(oFormReference.elements[i])) {
					a_oReturn.push(oFormReference.elements[i]);
				}
			}
		}

			//#### Return the above determined a_oReturn value to the caller
		return a_oReturn;
	};

}; //# Cn.Web.Inputs.Form
