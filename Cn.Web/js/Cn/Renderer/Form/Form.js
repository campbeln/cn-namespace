//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined') {
	alert("Cn.Renderer.Form: [DEVELOPER] 'Cn/Cn.js' must be included before referencing this code.");
}
else if (! Cn.Settings) {
	alert("Cn.Renderer.Form: [DEVELOPER] 'Cn/Settings.js.*' must be included before referencing this code.");
}
//# </DebugCode>

	//#### Else the Cn namespace is present, so ensure our .namespace is setup
else {
	Cn.namespace("Cn.Renderer");
}


//########################################################################################################################
//# Form class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Settings.js.*
//########################################################################################################################
//# Last Code Review: March 1, 2006
Cn.Renderer.Form = new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.frm = this;

		//#### Declare the required 'protected' variables
	this._Info = new Array();

		//#### Declare the required 'public' enums
		//####     NOTE: The enumFormErrors directly relate to Renderer's enumFormErrors and the enumDataTypes directly related to DataSource.MetaData's enumDataTypes. Any changes to these values must be reflected in these enums!
	this.enumFormErrors = {
		cnNoError : 0,
		cnIncorrectLength : 1,
		cnIncorrectDataType : 2,
		cnValueIsRequired : 3,
		cnNotWithinPicklist : 4,
		cnUnknownOrUnsupportedType : 5,
		cnCustom : 6,
		cnMissingInput : 7
	};
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
	//# 'Public' Properties
	//##########################################################################################
	//############################################################
	//# Declare the required 'public' (sub-class stub) properties (defaulting them from the Abbreviation namespace just in case they were previously defined)
	//#     NOTE: The defaulting for .Validation is techniqually not required as the .Validation class requires the .Form class (so it should always equate to null), but it is included below for compleatness
	//############################################################
	this.SpecialListBox = (Cn._.slb ? Cn._.slb : null);
	this.Validation = (Cn._.val ? Cn._.val : null);
	this.ComboBox = (Cn._.cbo ? Cn._.cbo : null);
	this.Errors = (Cn._.err ? Cn._.err : null);


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Returns the properly formatted input name to the caller
	//############################################################
	//# Last Updated: July 18, 2004
	this.GetInputName = function(sInputAlias, iInputIndex) {
		return sInputAlias + '_' + iInputIndex;
	};


	//##########################################################################################
	//# Record-related Functions
	//##########################################################################################
	//############################################################
	//# Determines if there is a rfRecordTracker for the passed iInputIndex within the passed sFormName
	//############################################################
	//# Last Updated: March 1, 2006
	this.RecordExists = function(sFormName, iInputIndex) {
		var sValue = GetRecordTrackerValue(sFormName, iInputIndex);

			//#### Return based on if the passed iInputIndex's rfRecordTracker is holding a value
		return (sValue && sValue != '');
	};

	//############################################################
	//# Determines if the passed iInputIndex is a new record
	//############################################################
	//# Last Updated: March 1, 2006
	this.IsNewRecord = function(sFormName, iInputIndex) {
		var sValue = GetRecordTrackerValue(sFormName, iInputIndex);

			//#### Return based on if the passed iInputIndex's rfRecordTracker is holding a .value beginning with "new"
		return (sValue && sValue.substr(0, 3).toLowerCase() == 'new');
	};

	//############################################################
	//# Determines if the passed iInputIndex is a missing record
	//############################################################
	//# Last Updated: March 1, 2006
	this.IsMissingRecord = function(sFormName, iInputIndex) {
		var sValue = GetRecordTrackerValue(sFormName, iInputIndex);

			//#### Return based on if the passed iInputIndex's rfRecordTracker is holding a .value beginning with "missing"
		return (sValue && sValue.substr(0, 7).toLowerCase() == 'missing');
	};

	//############################################################
	//# Determines if the passed iRecordIndex needs to be validated
	//#    NOTE: If somehow the user submits a new record to the server w/o the JavaScript catching the errors and server side errors are generated, this code will not catch any subsequent errors as the previosuly set values will be seen as defaults (and the record will therefore not be checked). This is a non issue if you properly check the submitted data on both the client AND server sides.
	//############################################################
	//# Last Updated: March 1, 2006
	this.ProcessRecord = function(sFormName, a_sInputAliases, iRecordIndex) {
		var bReturn;

			//#### If the passed iRecordIndex .Is(a)NewRecord
		if (this.IsNewRecord(sFormName, iRecordIndex)) {
			var sInputName;
			var i;

				//#### Default bReturn to false (as we only check new records if they've been changed by the user)
			bReturn = false;

				//#### Traverse the passed a_sInputAliases
			for (i = 0; i < a_sInputAliases.length; i++) {
					//#### Reset the sInputName for this loop
				sInputName = this.GetInputName(a_sInputAliases[i], iRecordIndex);

					//#### If the user has changed the value(s) of the sInputName, flip bReturn and fall from the loop
				if (this.InputWasUpdated(sFormName, sInputName)) {
					bReturn = true;
					break;
				}
			}
		}
			//#### Else if this .Is(not a)MissingRecord, then we need to .Process(the)Record so flip bReturn to true
		else if (! this.IsMissingRecord(sFormName, iRecordIndex)) {
			bReturn = true;
		}

			//#### Return the above set bReturn to the caller
		return bReturn;
	};

	//############################################################
	//# Determines if the passed sFormName/iInputIndex's value is required for use with the data validation routines below
	//############################################################
	//# Last Updated: February 21, 2006
	this.IsRequiredForRecord = function(sFormName, iInputIndex, bIsRequired, bIsID) {
			//#### One-liner for the below logic
		//return (bIsRequired || ((!this.IsNewRecord(sFormName, iInputIndex) && bIsID));

			//#### If the input bIsRequired (as required inputs are only checked for records that pass .ProcessRecord), return true (as this input is indeed required)
		if (bIsRequired) {
			return true;
		}
			//#### Else if the input is not part of a new record and bIs(an)ID, return true (as an ID is required for existing records)
		else if (! this.IsNewRecord(sFormName, iInputIndex) && bIsID) {
			return true;
		}
			//#### Else the input is not required, so return false
		else {
			return false;
		}
	};


	//##########################################################################################
	//# HTML form interaction-related Functions
	//##########################################################################################
	//############################################################
	//# Submits the sFormName
	//############################################################
	//# Last Updated: March 1, 2006
	this.SubmitForm = function(sFormName) {
			//#### If the sFormName is a valid .form, .submit it
		if (document.forms[sFormName]) {
			document.forms[sFormName].submit();
		}
	};

	//############################################################
	//# Returns a reference to the passed sInputName to the caller
	//############################################################
	//# Last Updated: March 1, 2006
	this.Input = function(sFormName, sInputName) {
		var oForm = document.forms[sFormName];

			//#### If the oForm was successfully collected above
		if (oForm) {
				//#### If the passed sInputName is defined within the oForm, return it, else return null (this always ensures that we return 'null' for unknown sFormName's/sInputName's)
			return (oForm.elements[sInputName] ? oForm.elements[sInputName] : null);
		}
			//#### Else the rFormRef was not successfully set, so return null (this always ensures that we return 'null' for unknown sFormName's/sInputName's)
		else {
			return null;
		}
	};

	//############################################################
	//# Returns a reference to the passed sInputAlias for the passed iRecordIndex to the caller
	//############################################################
	//# Last Updated: July 11, 2006
//! GetInput
	this.RInput = function(sFormName, sInputAlias, iRecordIndex) {
		return this.Input(sFormName, this.GetInputName(sInputAlias, iRecordIndex));
	}

	//############################################################
	//# Returns the value(s) of the passed sFormName/sInputName
	//############################################################
	//# Last Updated: May 4, 2006
	this.InputValue = function(sFormName, sInputName, bForceString) {
		var a_oInput = [null];
		var a_sReturn = [''];
		var iReturnIndex = 0;
		var i;

			//#### Retrieve a reference to the passed sFormName/sInputName
		var oInput = this.Input(sFormName, sInputName);

			//#### If the oInput was successfully collected above
		if (oInput != null) {
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
									//#### Add the current a_oInput's .id (or failing that, it's .name) into the a_sReturn array (post inc'ing iReturnIndex)
								a_sReturn[iReturnIndex++] = (a_oInput[i].id || a_oInput[i].name);
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

			//#### Determine the .length of the above filled a_sReturn
		switch (a_sReturn.length) { 
				//#### If only a single value was found above, so return it (as a String) to the caller
			case 1: {
				return new String(a_sReturn[0]);
				break;
			}
				//#### Else more then one value was found
			default: {
					//#### If we're supposed to bForceString, return the null-string .join'ed a_sReturn
				if (bForceString) {
					return a_sReturn.join('');
				}
					//#### Else return the unadulterated a_sReturn to the caller
				else {
					return a_sReturn;
				}
				break;
			}
		}
	};

	//############################################################
	//# Returns the value(s) of the passed sFormName/sInputAlias for the passed iRecordIndex to the caller
	//############################################################
	//# Last Updated: July 11, 2006
//! GetInputValue
	this.RInputValue = function(sFormName, sInputAlias, iRecordIndex, bForceString) {
		return this.InputValue(sFormName, this.GetInputName(sInputAlias, iRecordIndex), bForceString);
	}

	//############################################################
	//# Determines if any of the passed sFormName's inputs have been changed by the user
	//############################################################
	//# Last Updated: June 7, 2006
	this.FormWasUpdated = function(sFormName) {
		var oForm = document.forms[sFormName];
		var i;
		var bReturn = false;

			//#### If the oForm was successfully collected
		if (oForm && oForm.elements) {
				//#### Traverse the oForm's .elements
			for (i = 0; i < oForm.elements.length; i++) {
					//#### Reset out bReturn value based on the result from .InputWasUpdated
				bReturn = this.InputWasUpdated(sFormName, oForm.elements[i].name);

					//#### If our bReturn value was flipped to true by .InputWasUpdated, fall from the loop
				if (bReturn) {
					break;
				}
			}
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Determines if the passed oInput has been changed by the user
	//#    NOTE: This code was inspired by the example given at: http://codestore.net/store.nsf/unid/DOMM-4UTKE6?OpenDocument
	//############################################################
	//# Last Updated: March 1, 2006
	this.InputWasUpdated = function(sFormName, sInputName) {
		var a_oInput = new Array(null);
		var bReturn = false;
		var iReturnIndex = 0;
		var oInput, i;

			//#### Collect the oInput based on the passed arguments
		oInput = this.Input(sFormName, sInputName)

			//#### If the oInput was successfully collected above
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

	//############################################################
	//# Retrieves the input names of the inputs that have been changed
	//############################################################
	//# Last Updated: March 1, 2006
	this.GetUpdatedInputs = function(sFormName, a_sInputAliases) {
		var a_sReturn = new Array('');
		var iCurrentRecord = 1;
		var iReturnIndex = 0;
		var sInputName, i;

			//#### While we still have existing records
		while (this.RecordExists(sFormName, iCurrentRecord)) {
				//#### Traverse the current record's a_sInputAliases
			for (i = 0; i < a_sInputAliases.length; i++) {
					//#### Collect the current sInputName
				sInputName = this.GetInputName(a_sInputAliases[i], iCurrentRecord);

					//#### If the current .InputWasUpdated, append it onto the sReturn value (and post inc. iReturnIndex while we're at it)
				if (this.InputWasUpdated(sFormName, sInputName)) {
					a_sReturn[iReturnIndex++] = sInputName;
				}
			}

				//#### Inc the iCurrentRecord in prep for the next loop
			iCurrentRecord++;
		}

			//#### If no updates were found above, reset the a_sReturn value to null
		if (a_sReturn.length == 1 && a_sReturn[0] == '') {
			a_sReturn = null;
		}

			//#### Return the above determined a_sReturn value to the caller
		return a_sReturn;
	};


	//##########################################################################################
	//# 'Protected' Functions
	//##########################################################################################
	//############################################################
	//# 
	//############################################################
	//# Last Updated: May 1, 2006
	this._Forms = function(sFormName) {
			//#### If this is a new sFormName definition
		if (! this._Info[sFormName]) {
				//#### Set the sFormName key/object within the .Info with the passed vars
			this._Info[sFormName] = {
				key : sFormName,
				ErrorCount : 0,
				hInputErrors : new Array(),
				fValidate : this._DummyValidate
			};
		}

			//#### Return the sFormName entry stored within ._Info to the caller
		return this._Info[sFormName];
	};

	//############################################################
	//# 
	//############################################################
	//# Last Updated: May 1, 2006
	this._Exists = function(sFormName) {
			//#### Determine if the sFormName ._Exists
		return (typeof(this._Info[sFormName]) != 'undefined');
	};

	//############################################################
	//# Default data validation function reference gfor newly created ._Info objects
	//############################################################
	//# Last Updated: May 1, 2006
	this._DummyValidate = function(bSubmitForm) {
	//	if (bSubmitForm)	{ Cn._.frm.SubmitForm(sFormName); }
		return false;
	};


	//##########################################################################################
	//# 'Private', Pseudo-'Static' Functions
	//# 
	//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
	//##########################################################################################
	//############################################################
	//# Determines the value of the referenced RecordTracker
	//############################################################
	//# Last Updated: May 1, 2006
	var GetRecordTrackerValue = function(sFormName, iInputIndex) {
		var rThis = Cn._.frm;
		var sInputName = rThis.GetInputName(Cn._.s.FormElementPrefix + 'rfRecordTracker', iInputIndex);

			//#### Return the rfRecordTracker's .InputValue to the caller
		return rThis.InputValue(sFormName, sInputName, true);
	};

} //# Cn.Renderer.Form
