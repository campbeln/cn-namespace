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
	alert("Cn.Web.Inputs.Validation.*: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
else if (! Cn._.dt) {
	alert("Cn.Web.Inputs.Validation.*: [DEVELOPER] 'Cn/Data/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.wt) {
	alert("Cn.Web.Inputs.Validation.*: [DEVELOPER] 'Cn/Web/Tools.js' must be included before referencing this code.");
}
else if (! Cn._.wi) {
	alert("Cn.Web.Inputs.Validation.*: [DEVELOPER] 'Cn/Web/Inputs/Inputs.js' must be included before referencing this code.");
}

	//#### If all of the required YAHOO objects do not exist, popup the related error message
else if (typeof(YAHOO) == 'undefined') {
	alert("Cn.Web.Inputs.Validation.*: [DEVELOPER] 'yui/YAHOO.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Dom) {
	alert("Cn.Web.Inputs.Validation.*: [DEVELOPER] 'yui/Dom.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### If our namespace is not setup, do so now
if (! Cn.Web || ! Cn.Web.Inputs) {
	Cn.Namespace("Cn.Web.Inputs");
}


//########################################################################################################################
//# Inputs.Validation class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Tools.js, Cn/Settings.js.*, Cn/Inputs/Inputs.js
//########################################################################################################################
//# Last Code Review: September 6, 2007
Cn.Web.Inputs.Validation = Cn._.wiv || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wiv = this;

		//#### Declare the required 'private' variables
	var g_oElementWorkspace = Cn._.wj.ElementWorkspace,
		g_oIntlValue = Cn._.ci.Value,
		g_oDataTools = Cn._.dt,
		g_oWebTools = Cn._.wt,
		g_oInputs = Cn._.wi,
		g_oThis = this,
		ga_oMetaData = {}
	;

		//#### Declare the required 'public' enums
		//####     NOTE: The enumInputErrors directly relate to Cn.Web.Input's enumInputErrors. Any changes to these values must be reflected in these enums!
	this.enumInputErrors = {
		cnNoError : 0,
		cnIncorrectLength : 1,
		cnIncorrectDataType : 2,
		cnValueIsRequired : 3,
		cnNotWithinPicklist : 4,
		cnUnknownOrUnsupportedType : 5,
		cnCustom : 6,
		cnMissingInput : 7,
		cnMultipleValuesSubmittedForSingleValue : 8
	};


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Attaches the Validater classes to the referenced form's inputs which are defined within our .InputMetaData
	//############################################################
	//# Last Updated: December 17, 2009
    this.AttachValidaters = function(oFormReference, h_oInputMetaData, bAttachToForm, bIsRendererForm, fPromptUser) {
        var oCurrentInput, bReturn;

            //#### Ensure the passed oFormReference is a reference (auto-magicially grabbing the .form reference if an input reference was passed), then set our bReturn value based on what we were able to collect
        oFormReference = g_oInputs.RelatedForm(oFormReference);
		bReturn = (oFormReference && oFormReference.elements && oFormReference.elements.length > 0);

            //#### If we seem to have a valid oFormReference with .elements to process
        if (bReturn) {
				//#### Set the passed h_oInputMetaData, bIsRendererForm and fPromptUser into our .Data
			this.Data(oFormReference).hInputMetaData = h_oInputMetaData;
			this.Data(oFormReference).IsRendererForm = (bIsRendererForm == true);

				//#### If the passed fPromptUser was not a function, reset it to the default .UI.PromptUser
			if (typeof(fPromptUser) != 'function') {
				this.Data(oFormReference).PromptUser = Cn._.wiveu.PromptUser;
			}

				//#### Traverse the .elements
            for (var i = 0; i < oFormReference.elements.length; i++) {
                oCurrentInput = oFormReference.elements[i];

                    //#### Create a new Validater attached to the oCurrentInput then .Add in the our sudo-private ._ValidateDataType function
				g_oElementWorkspace[oCurrentInput.id] = g_oElementWorkspace[oCurrentInput.id] || {};
                g_oElementWorkspace[oCurrentInput.id].Validater = new Validater(oCurrentInput);
                g_oElementWorkspace[oCurrentInput.id].Validate = g_oElementWorkspace[oCurrentInput.id].Validater.Validate;
                g_oElementWorkspace[oCurrentInput.id].Validater.Add(ValidateDataType);
            }

				//#### If we are supposed to bAttachToForm
			if (bAttachToForm) {
				this.AttachToForm(oFormReference);
			}
        }

			//#### Return the abopve collected bReturn value to the caller
		return bReturn;
    };

	//############################################################
	//# Attaches .ValidateForm to the provided form's onSubmit event
	//# 
	//#     NOTE: By definition, we attach to onsubmit (as we cannot attach)
	//############################################################
	//# Last Updated: December 7, 2009
	this.AttachToForm = function(oFormReference) {
		var bReturn;

            //#### Ensure the passed oFormReference is a reference (auto-magicially grabbing the .form reference if an input reference was passed), then set our bReturn on if the oFormReference is valid
        oFormReference = g_oInputs.RelatedForm(oFormReference);
		bReturn = ((oFormReference && oFormReference.elements) ? true : false);

//# <DebugCode>
			//#### If the YAHOO.util.Event object does not exist, popup the error
		if (typeof(YAHOO) == 'undefined' || ! YAHOO.util || ! YAHOO.util.Event) {
			alert("Cn.Inputs.Validation.Errors: [DEVELOPER] 'yui/event.js' must be included before referencing this code.");
			bReturn = false;
		}
//# </DebugCode>

			//#### If we were able to successfully collect the oFormReference above
		if (bReturn) {
				//#### .add(the)Listener onto the oFormReference, passing in an object representing the oFormReference
			YAHOO.util.Event.addListener(oFormReference, 'submit', ValidateListener, oFormReference, this);
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Validates the referenced HTML form based on it's defined validation function
	//############################################################
	//# Last Updated: December 8, 2009
    this.ValidateForm = function(oFormReference) {
		var a_oAddlArguments = new Array(),
			i
		;

            //#### Ensure the passed oFormReference is a reference (auto-magicially grabbing the .form reference if an input reference was passed)
        oFormReference = g_oInputs.RelatedForm(oFormReference);

			//#### .Clear the .Errors for the passed oFormReference (so we start from a clean slate for this .ValidateForm)
		this.Errors.Clear(oFormReference);

            //#### If the oFormReference has .elements to traverse
			//####     NOTE: We really don't need to check for .elements as .RelatedForm ensures that a valid oFormReference is returned, but we do below for completeness
        if (oFormReference && oFormReference.elements) {
				//#### Traverse the passed arguments (skipping the oFormReference), copying each into the a_oAddlArguments
			for (i = 1; i < arguments.length; i++) {
				a_oAddlArguments.push(arguments[i]);
			}

				//#### Traverse the .elements
	        for (i = 0; i < oFormReference.elements.length; i++) {
	                //#### If the current .elements has a .Validate function defined, run it (passing in the a_oAddlArguments)
	            if (g_oElementWorkspace[oFormReference.elements[i].id] && g_oElementWorkspace[oFormReference.elements[i].id].Validate) {
	                g_oElementWorkspace[oFormReference.elements[i].id].Validate(a_oAddlArguments);
	            }
	        }
        }

            //#### If we have errors for our oFormReference, .PromptUser and return false
        if (this.Data(oFormReference).ErrorCount > 0) {
			this.Data(oFormReference).PromptUser(a_oAddlArguments);
            return false;
        }
            //#### Else we had no errors, so return true
        else {
            return true;
        }
    }

	//############################################################
	//# Provides access to the validation meta data for the referenced form
	//############################################################
	//# Last Updated: December 7, 2009
    this.Data = function(oFormReference) {
            //#### Ensure the passed oFormReference is a reference (auto-magicially grabbing the .form reference if an input reference was passed)
        var sFormID = g_oWebTools.GetIdentifier(g_oInputs.RelatedForm(oFormReference));

			//#### If this is a new sFormID definition, setup the entry now
		if (! ga_oMetaData[sFormID]) {
				//#### Set the sFormID key/object within the .Info with the passed vars
				//####     NOTE: hInputMetaData and hInputErrors are split as hInputMetaData referes to InputAliases, while hInputErrors relates to specific InputIDs
				//####     NOTE: hInputMetaData and hInputErrors are init'ed with "{}" so that hashtable attempts do no fail on empty/null objects
			ga_oMetaData[sFormID] = {
				FormID : sFormID,
				ErrorCount : 0,
				hInputErrors : {},
				hInputMetaData : {},
				IsRendererForm : false,
				InputAliases : function() {
					var a_sReturn = new Array();
					for (var sKey in this.hInputMetaData) {
						a_sReturn.push(sKey);
					}
					return a_sReturn;
				}
			};
		}

            //#### Always return a reference to the ga_oMetaData entry under the sFormID
        return ga_oMetaData[sFormID];
    };

	//############################################################
	//# Conditionally retrieves the referenced input's InputAlias based on if it is related to a Renderer.Form or not
	//############################################################
	//# Last Updated: Jnuary 13, 2010
    this.InputAlias = function(oInput) {
		var sID = g_oWebTools.GetIdentifier(oInput),
			oReturn = { InputAlias: sID, RecordNumber: -1 }
		;

			//#### If the .Data specifies that this .Is(a)RendererForm-related oFormReference
		if (this.Data(oInput).IsRendererForm && sID.lastIndexOf('_') > 0) {
				//#### Logicially split the sID at the last underscore into .InputAlias and .RecordIndex
			oReturn.InputAlias = sID.substr(0, sID.lastIndexOf('_'));
			oReturn.RecordIndex = g_oDataTools.MakeNumeric(sID.substr(sID.lastIndexOf('_') + 1), -1);
		}

			//#### Return the above determined oReturn value to the caller
		return oReturn;
    };


	//##########################################################################################
	//# 'Public' Validation-related Functions
	//##########################################################################################
	//############################################################
	//# Determines if the .value in the passed oInput is required
	//############################################################
	//# Last Updated: December 4, 2009
	this.IsRequired = function(oInput, bIsRequired) {
			//#### Pass the call off to .DoIs signaling it is an .cnRequired test, returning its return value as our own
			//####     NOTE: a .enumDataTypes of .cnUnknown is logicially considered a plain .IsRequired test
		return DoIs(g_oInputs.enumDataTypes.cnUnknown, oInput, bIsRequired, null, null);
	};

	//############################################################
	//# Determines if the .value in the passed oInput is a boolean value
	//############################################################
	//# Last Updated: December 4, 2009
	this.IsBoolean = function(oInput, bIsRequired) {
			//#### Pass the call off to .DoIs signaling it is an .cnBoolean test, returning its return value as our own
		return DoIs(g_oInputs.enumDataTypes.cnBoolean, oInput, bIsRequired, null, null);
	};

	//############################################################
	//# Determines if the .value in the passed oInput is a valid GUID
	//############################################################
	//# Last Updated: December 4, 2009
	this.IsGUID = function(oInput, bIsRequired) {
			//#### Pass the call off to .DoIs signaling it is an .cnGUID test, returning its return value as our own
		return DoIs(g_oInputs.enumDataTypes.cnGUID, oInput, bIsRequired, null, null);
	};

	//############################################################
	//# Determines if the .value in the passed oInput is a valid string
	//############################################################
	//# Last Updated: December 4, 2009
	this.IsString = function(oInput, bIsRequired, iMaxLength) {
			//#### Pass the call off to .DoIs signaling it is an .cnString test, returning its return value as our own
		return DoIs(g_oInputs.enumDataTypes.cnChar, oInput, bIsRequired, iMaxLength, null);
	};

	//############################################################
	//# Determines if the .value in the passed oInput is a valid date/time
	//############################################################
	//# Last Updated: December 4, 2009
	this.IsDate = function(oInput, bIsRequired, bValidateDataType) {
			//#### Pass the call off to .DoIs signaling it is an .cnDateTime test, returning its return value as our own
		return DoIs(g_oInputs.enumDataTypes.cnDateTime, oInput, bIsRequired, null, bValidateDataType);
	};

	//############################################################
	//# Determines if the .value in the passed oInput is a valid integer value
	//############################################################
	//# Last Updated: August 28, 2007
	this.IsInteger = function(oInput, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue) {
			//#### Pass the call off to .DoIsNumeric signaling it is an .cnInteger test, returning its return value as our own
		return DoIsNumeric(g_oInputs.enumDataTypes.cnInteger, oInput, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue);
	};

	//############################################################
	//# Determines if the .value in the passed oInput is a valid float value
	//############################################################
	//# Last Updated: August 28, 2007
	this.IsFloat = function(oInput, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue) {
			//#### Pass the call off to .DoIsNumeric signaling it is an .cnFloat test, returning its return value as our own
		return DoIsNumeric(g_oInputs.enumDataTypes.cnFloat, oInput, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue);
	};

	//############################################################
	//# Determines if the .value in the passed oInput is a valid currency value
	//############################################################
	//# Last Updated: August 28, 2007
	this.IsCurrency = function(oInput, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue) {
			//#### Pass the call off to .DoIsNumeric signaling it is an .cnCurrency test, returning its return value as our own
		return DoIsNumeric(g_oInputs.enumDataTypes.cnCurrency, oInput, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue);
	};


	//##########################################################################################
	//# 'Private', Pseudo-'Static' Functions
	//# 
	//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
	//##########################################################################################
	//############################################################
	//# Validates the referenced HTML form based on it's defined validation function (via .AttachToForm)
	//#
	//#     NOTE: This "overload" is required because the YUI seems to count the argument number and acts differently. So the docs call for a function with a single argument for the oEvent (even though the oArbitraryObject is passed in if provided via the second argument)
	//############################################################
	//# Last Updated: December 23, 2009
	var ValidateListener = function(oEvent) {
			//#### If .ValidateForm fails, we need to .preventDefault
			//####     NOTE: Since this is called via .addListener with Cn._.wive as the context, the full resolution is required for the function call
			//####     NOTE: We do not test bSubmitForm because by definition we attach to the "onsubmit" event and therefore always needs to be stopped in the event of a .Validate failure
		if (! Cn._.wiv.ValidateForm(arguments[1])) {
				//#### In order to prevent the sFormID from submitting, we need to .preventDefault to the oEvent listener passed first argument
				//####     NOTE: This is required because .addListener doesn't support the ability to "return" the return value from this function as a means to stop the form submittal
			YAHOO.util.Event.preventDefault(oEvent);
		}
	};

	//############################################################
	//# Determines if the referenced value is valid based on it's data type.
	//############################################################
	//# Last Updated: December 7, 2009
	var DoIs = function(eDataType, oInput, bIsRequired, iMaxLength, bValidateDate) {
		var sValue = g_oInputs.Value(oInput),
			eError = g_oThis.enumInputErrors.cnNoError
		;

			//#### If there is a sValue to process
		if (sValue.length > 0) {
				//#### Determine the eDataType and process accordingly
			switch (eDataType) {
//! case g_oInputs.enumDataTypes.cnLongChar:
				case g_oInputs.enumDataTypes.cnChar: {
						//#### If the sValue is too long, set eError to .cnIncorrectLength
					if (sValue.length > iMaxLength) {
						eError = g_oThis.enumInputErrors.cnIncorrectLength;
					}
					break;
				}
				case g_oInputs.enumDataTypes.cnBoolean: {
						//#### If the sValue .Is(not a)Boolean, set the eError
					if (! g_oDataTools.IsBoolean(sValue)) {
						eError = g_oThis.enumInputErrors.cnIncorrectDataType;
					}
					break;
				}
				case g_oInputs.enumDataTypes.cnDateTime: {
						//#### If we are to bValidate(the)Date and the sValue .Is(not a valid)DateTime, set eError to .cnIncorrectDataType
					if (bValidateDate && ! g_oDataTools.IsDate(sValue)) {
						eError = g_oThis.enumInputErrors.cnIncorrectDataType;
					}
					break;
				}
				case g_oInputs.enumDataTypes.cnGUID: {
						//#### If the sValue isn't a valid GUID, set eError to .cnIncorrectDataType
					if (! g_oDataTools.IsGUID(sValue)) {
						eError = g_oThis.enumInputErrors.cnIncorrectDataType;
					}
					break;
				}
			}
		  //case g_oInputs.enumDataTypes.cnUnknown: {
		  //		//# No action is required as this is logicially simply a presence test (which is done below if bIsRequired is true)
		  //		break;
		  //	}
		}
			//#### Else if a sValue bIsRequired, set eError to .cnValueIsRequired
		else if (bIsRequired) {
			eError = g_oThis.enumInputErrors.cnValueIsRequired;
		}

			//#### Call Errors.Set to set/clear the above determined eError
		g_oThis.Errors.Set(oInput, eError, eDataType, '');

			//#### Return the above determined eError (in case the developer is calling this)
		return eError;
	};

	//############################################################
	//# Determines if the referenced value is numeric as defined by its referenced data type.
	//############################################################
	//# Last Updated: January 13, 2010
	var DoIsNumeric = function(eDataType, oInput, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue) {
		var sValue = g_oInputs.Value(oInput),
			eError = g_oThis.enumInputErrors.cnNoError,
			nValue
		;

			//#### If this is a .cnCurrency eDataType, remove the .Localization_CurrencySymbol from the sValue
			//####     NOTE: IE errors when some currency symbols are transported via HTTP (while it doesn't error if it's loading the file from the file system!?)
		if (eDataType == g_oInputs.enumDataTypes.cnCurrency) {
			var oRegEx = new RegExp(g_oIntlValue.Localization_CurrencySymbol, "gi");
			sValue = sValue.replace(oRegEx, '');
		}

			//#### If there is a sValue to process
		if (sValue.length > 0) {
				//#### Convert sValue into a Number
			nValue = g_oDataTools.MakeNumeric(sValue, null);

				//#### If the conversion failed, set eError to .cnIncorrectDataType
			if (nValue == null) {
				eError = g_oThis.enumInputErrors.cnIncorrectDataType;
			}
				//#### Else if this is an .cnInteger eDataType and the nValue is not a whole number, set eError to .cnIncorrectDataType
			else if (eDataType == g_oInputs.enumDataTypes.cnInteger && Math.floor(nValue) != nValue) {
				eError = g_oThis.enumInputErrors.cnIncorrectDataType;
			}
				//#### Else we need to check it as a floating point number
			else {
					//#### If the sMinNumericValue or sMaxNumericValue have not been set, default to testing the value based on the iNumericPrecision
					//####     NOTE: This is done as a failsafe measure, as well as allowing partial functionality of datasources that have not yet been fully defined within DataSource.MetaData
					//####     NOTE: This is a "better then nothing" test, as testing the sValue against the iNumericPrecision will not guarentee that the value fits into the column
				if (sMinNumericValue.length == 0 || sMaxNumericValue.length == 0) {
						//#### If the nValue is too long, set eError to .cnIncorrectLength
					if (g_oDataTools.NumericPrecision(nValue) > iNumericPrecision) {
						eError = g_oThis.enumInputErrors.cnIncorrectLength;
					}
				}
					//#### Else the sMinNumericValue and sMaxNumericValue have been set
				else {
						//#### If the nValue is outside of the sMinNumericValue/sMaxNumericValue range, set eError to .cnIncorrectLength
					if (! g_oDataTools.IsNumericInRange(nValue, sMinNumericValue, sMaxNumericValue)) {
						eError = g_oThis.enumInputErrors.cnIncorrectLength;
					}
				}
			}
		}
			//#### Else if a sValue bIsRequired, set eError to .cnValueIsRequired
		else if (bIsRequired) {
			eError = g_oThis.enumInputErrors.cnValueIsRequired;
		}

			//#### Call Errors.Set to set/clear the above determined eError
		g_oThis.Errors.Set(oInput, eError, eDataType, '');

			//#### Return the above determined eError (in case the developer is calling this)
		return eError;
	}

	//############################################################
	//# Validates the passed oValidateObject (which includes the .Input reference) based on it's .DataType
	//############################################################
	//# Last Updated: December 8, 2009
    var ValidateDataType = function(oValidateObject) {
        var oInput, oInputAlias, oInputMetaData, bReturn;

            //#### Collect the local vars from the arguments (collecting the oInputMetaData based on the oInput's .form.id) and default our bReturn value
            //####     NOTE: We need to use "Cn._.wiv" rather then "this" below as YUI's .onContentReady overrideContext isn't functioning properly
        oInput = g_oInputs.Input(oValidateObject.Input);
        oInputAlias = g_oThis.InputAlias(oInput);
        oInputMetaData = g_oThis.Data(g_oWebTools.GetIdentifier(g_oInputs.RelatedForm(oInput))).hInputMetaData;
        bReturn = false;

            //#### If we were able to collect the oInputMetaData for the related form.id
        if (oInputMetaData) {
                //#### Collect the oInputMetaData based on the .InputAlias
			oInputMetaData = oInputMetaData[oInputAlias.InputAlias];

                //#### If we were able to reset oInputMetaData to the oInputAlias's entry
            if (oInputMetaData) {
                    //#### Determine the .DataType and process accordingly
                switch (oInputMetaData.DataType) {
                        //#### If this is an IsNumeric .DataType, pass the call off to .DoIsNumeric
                    case g_oInputs.enumDataTypes.cnInteger:
                    case g_oInputs.enumDataTypes.cnFloat:
                    case g_oInputs.enumDataTypes.cnCurrency: {
	                    bReturn = DoIsNumeric(oInputMetaData.DataType, oInput, oInputMetaData.IsRequired, oInputMetaData.NumericPrecision, oInputMetaData.MinimumNumericValue, oInputMetaData.MaximumNumericValue);
                        break;
                    }
                        //#### Else this is a non-numeric .DataType, so pass the call off to .DoIs
                    default: {
	                    bReturn = DoIs(oInputMetaData.DataType, oInput, oInputMetaData.IsRequired, oInputMetaData.MaximumCharacterLength, oInputMetaData.DateTime_ValidateDataType);
                    }
                }
            }

                //#### Return the above determined bReturn value to the caller
            return bReturn;
        }
    };


	//########################################################################################################################
	//# Errors class
	//# 
	//#     Required Includes: Cn/Cn.js, Cn/Tools.js, Cn/Settings.js.*, Cn/Inputs/Inputs.js, Cn/Renderer/Form/ErrorMessages.js.*, [yui/Yahoo.js], yui/Dom.js
	//########################################################################################################################
	//# Last Code Review: February 23, 2006
	this.Errors = Cn._.wive || new function() {
			//#### Set a reference to ourselves in the Abbreviation namespace
		Cn._.wive = this;

			//#### Declare the required 'private' variables
		var g_oValidation = Cn._.wiv,
			g_oIntlValue = Cn._.ci.Value,
			g_oWebSettingsValue = Cn._.ws.Value,
			g_oDataTools = Cn._.dt,
			g_oWebTools = Cn._.wt,
			g_oInputs = Cn._.wi,
			g_oErrors = Cn._.wive,
			g_oThis = this
		;


		//##########################################################################################
		//# 'Public' Functions
		//##########################################################################################
		//############################################################
		//# Sets/resets the error information for the referenced input.
		//############################################################
		//# Last Updated: February 19, 2010
		this.Set = function(oInput, eError, eDataType, sDeveloperErrorMessage) {
			var oMetaData, sErrorMessage, sInputName;

				//#### If the passed oInput is an .UnknownInput (as returned from Cn.Renderer.Form.Input) and this is not a .cnNoError, .Set_PopDeveloperError
			if (oInput && oInput.UnknownInput && eError != this.enumInputErrors.cnNoError) {
				Set_PopDeveloperError(oInput.FormID, oInput.FormID + "." + oInput.InputAlias + "." + oInput.RecordIndex);
			}
				//#### Else we can process the passed oInput
			else {
					//#### If the passed oInput is invalid and this is not a .cnNoError, .Set_PopDeveloperError
					//####     NOTE: We stupidly run this .Get twice in order to avoid a second variable as well as possible byref issues (where "oOrgInput = oInput" is only a new reference and not a deep copy)
				if (! g_oInputs.Input(oInput)) {
					if (eError != this.enumInputErrors.cnNoError) {
						Set_PopDeveloperError(oInput, oInput);
					}
				}
					//#### Else the oInput was valid
				else {
						//#### Ensure the passed oInput is a reference and collect the oMetaData
					oInput = g_oInputs.Input(oInput);
					oMetaData = g_oValidation.Data(oInput);

						//#### If the oInput is valid
					if (oInput && oInput.type) {
							//#### If this is a hidden input, lets try for the .ReadOnlyID
						if (oInput.type.toLowerCase() == 'hidden') {
								//#### Set the sInputName to the .ReadOnlyID (or to the oInput.id if it's not found)
							sInputName = g_oInputs.ReadOnlyID(oInput);
							if (! g_oWebTools.GetByID(sInputName)) { sInputName = g_oWebTools.GetIdentifier(oInput); }
						}
							//#### Else if this is a HTMLEditor
						else if (oInput.type.toLowerCase() == 'textarea' && Cn._.wih && Cn._.wih.Exists(oInput)) {
								//#### Set the sInputName to the .Canvas (or to the oInput.id if it's not found)
							sInputName = g_oWebTools.GetIdentifier(Cn._.wih.Data(oInput).Canvas);
							if (! g_oWebTools.GetByID(sInputName)) { sInputName = g_oWebTools.GetIdentifier(oInput); }
						}
					}
						//#### Else we need to set the error info into the passed oInput, so .Get(it's)Identifier
					else {
						sInputName = g_oWebTools.GetIdentifier(oInput);
					}

						//#### If an eError is being .Set
					if (eError != g_oValidation.enumInputErrors.cnNoError) {
							//#### Ensure the passed sDeveloperErrorMessage is a string
						sDeveloperErrorMessage = g_oDataTools.MakeString(sDeveloperErrorMessage, '');

							//#### If this is a .cnCustom error and a sDeveloperErrorMessage was passed in, set sErrorMessage to the passed sDeveloperErrorMessage
						if (eError == g_oValidation.enumInputErrors.cnCustom && sDeveloperErrorMessage.length > 0) {
							sErrorMessage = sDeveloperErrorMessage;
						}
							//#### Else this is not a .cnCustom error and/or no sDeveloperErrorMessage was passed, so collect the sErrorMessage from .ErrorMessage
						else {
							sErrorMessage = this.ErrorMessage(eError, eDataType);
						}

							//#### Set the error info into the sInputName and increment the .ErrorCount
						oMetaData.hInputErrors[sInputName] = { key:sInputName, input:oInput, error:eError, message:sErrorMessage };
						oMetaData.ErrorCount++;

							//#### Add the .CSSClass_FormInputError into the sInputName
						g_oWebTools.AddClass(sInputName, g_oWebSettingsValue.CSSClass_FormInputError);
					}
						//#### Else an eError has not occured (or is being un/reset)
					else {
							//#### If an .error was set against the sInputName, decrement the .ErrorCount
						if (oMetaData.hInputErrors[sInputName] && oMetaData.hInputErrors[sInputName].error != g_oValidation.enumInputErrors.cnNoError) {
							oMetaData.ErrorCount--;
						}

							//#### Destroy the entry within the oMetaData.hInputErrors, then remove the .CSSClass_FormInputError from the sInputName
						oMetaData.hInputErrors[sInputName] = null;
						g_oWebTools.RemoveClass(sInputName, g_oWebSettingsValue.CSSClass_FormInputError);
					}
				}
			}
		};

		//############################################################
		//# Streamlined interface to set a developer-defined custom error against the passed input.
		//############################################################
		//# Last Updated: December 8, 2009
		this.SetCustom = function(oInput, sDeveloperErrorMessage) {
				//#### .Set the custom error against the passed oInput
				//####     NOTE: The DataType is not required below as logicially a .cnCustom error doesn't use the DataType
			this.Set(oInput, g_oValidation.enumInputErrors.cnCustom, g_oInputs.enumDataTypes.cnUnknown, sDeveloperErrorMessage);
		};

		//############################################################
		//# Clears any errors logged against the referenced form
		//############################################################
		//# Last Updated: December 8, 2009
		this.Clear = function(oFormReference) {
			var oFormMetaData = g_oValidation.Data(oFormReference);

				//#### Reset the .ErrorCount and .hInputErrors to a blank hash (well, a blank object really)
			oFormMetaData.ErrorCount = 0;
			oFormMetaData.hInputErrors = {};
		};

//!
this.FormErrorMessages = function(oFormReference) {
	var oFormMetaData = g_oValidation.Data(oFormReference),
		a_oReturn = new Array()
	;

		//#### If we were able to collect the oFormMetaData
	if (oFormMetaData) {
			//#### Traverse the hInputErrors, one sID at a time
		for (var sID in oFormMetaData.hInputErrors) {
				//#### 
			if (oFormMetaData.hInputErrors[sID]) {
					//#### .push the .Label and .Message into our a_oReturn value
				a_oReturn.push( {
					ID: sID,
					Label: g_oWebTools.InnerHTML(g_oInputs.RelatedLabel(sID)),
					Message: oFormMetaData.hInputErrors[sID].message
				} );
			}
		}
	}

var sMessage = "";
for (var i = 0; i < a_oReturn.length; i++) {
	sMessage = sMessage + a_oReturn[i].Label + " (" + a_oReturn[i].ID + "): " + a_oReturn[i].Message + "\n";
}
alert(sMessage);

		//#### Return the above determined a_oReturn value to the caller
	return a_oReturn;
};

		//############################################################
		//# Retrieves the corrosponding ErrorMessage for the passed eError/eDataType
		//############################################################
		//# Last Updated: January 13, 2010
		this.ErrorMessage = function(eError, eDataType) {
//# <DebugCode>
				//#### If our settings have not yet been setup within Cn.Configuration.Internationalization, popup the error
			if (! Cn._.ci || ! Cn._.ci.Value.EndUserMessages_Alert) {
				alert("Cn.Inputs.Validation.Errors: [DEVELOPER] 'Validation.js." + Cn._.wj.ServerSideScriptFileExtension + "' must be included before referencing this code.");
				return null;
			}
				//#### Else we have .ErrorMessages
			else {
//# </DebugCode>
					//#### Determine the value of the passed eError, returning accordingly
				switch (eError) {
					case g_oValidation.enumInputErrors.cnValueIsRequired: {
						return g_oIntlValue.EndUserMessages_ValueIsRequired;
						break;
					}
					case g_oValidation.enumInputErrors.cnIncorrectLength: {
						return g_oIntlValue.EndUserMessages_IncorrectLength;
						break;
					}
					case g_oValidation.enumInputErrors.cnIncorrectDataType: {
							//#### Determine the value of the passed eDataType, returning accordingly
						switch (eDataType) {
							case g_oInputs.enumDataTypes.cnBoolean: {
								return g_oIntlValue.EndUserMessages_IncorrectDataType_Boolean;
								break;
							}
							case g_oInputs.enumDataTypes.cnInteger: {
								return g_oIntlValue.EndUserMessages_IncorrectDataType_Integer;
								break;
							}
							case g_oInputs.enumDataTypes.cnFloat: {
								return g_oIntlValue.EndUserMessages_IncorrectDataType_Float;
								break;
							}
							case g_oInputs.enumDataTypes.cnCurrency: {
								return g_oIntlValue.EndUserMessages_IncorrectDataType_Currency;
								break;
							}
							case g_oInputs.enumDataTypes.cnDateTime: {
								return g_oIntlValue.EndUserMessages_IncorrectDataType_DateTime;
								break;
							}
							case g_oInputs.enumDataTypes.cnGUID: {
								return g_oIntlValue.EndUserMessages_IncorrectDataType_GUID;
								break;
							}
							default: {
								return g_oIntlValue.EndUserMessages_IncorrectDataType_Other;
								break;
							}
						}
						break;
					}
					case g_oValidation.enumInputErrors.cnNotWithinPicklist: {
						return g_oIntlValue.EndUserMessages_IncorrectDataType_NotWithinPicklist;
						break;
					}
					case g_oValidation.enumInputErrors.cnCustom: {
							//#### We only get here if the developer has not setup a sDeveloperErrorMessage, so return the vanilla error message
						return g_oIntlValue.EndUserMessages_Custom;
						break;
					}
					case g_oValidation.enumInputErrors.cnUnknownOrUnsupportedType: {
						return g_oIntlValue.EndUserMessages_UnknownOrUnsupportedType;
						break;
					}
					case g_oValidation.enumInputErrors.cnMissingInput: {
						return g_oIntlValue.EndUserMessages_MissingInput;
						break;
					}
					case g_oValidation.enumInputErrors.cnNoError: {
						return g_oIntlValue.EndUserMessages_NoError;
						break;
					}
					default: {
						return g_oIntlValue.EndUserMessages_UnknownErrorCode;
						break;
					}
				}
//# <DebugCode>
			}
//# </DebugCode>


			//##########################################################################################
			//# 'Private', Pseudo-'Static' Functions
			//# 
			//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
			//##########################################################################################
			//############################################################
			//# 
			//############################################################
			//# Last Updated: December 8, 2009
			var Set_PopDeveloperError = function(oFormReference, sInputAlias) {
					//#### Increment the .ErrorCount
				g_oValidation.Data(oFormReference).ErrorCount++;

					//#### We can't sent/unset an error against a non-existent oInput so popup the error to the user/developer
				alert(g_oThis.ErrorMessage(g_oValidation.enumInputErrors.cnMissingInput, null) + "'" + sInputName + "'");
			};

		}; //# this.Errors


		//########################################################################################################################
		//# DisplayErrors class
		//# 
		//#     Required Includes: Cn/Cn.js, Cn/Tools.js, Cn/Settings.js.*
		//########################################################################################################################
		//# Last Code Review: February 23, 2006
		this.UI = Cn._.wiveu || new function() {
				//#### Set a reference to ourselves in the Abbreviation namespace
			Cn._.wiveu = this;

				//#### Declare the required 'private' variables
			var g_oValidation = Cn._.wiv,
				g_oWebSettingsValue = Cn._.ws.Value,
				g_oElementWorkspace = Cn._.wj.ElementWorkspace,
				g_oWebTools = Cn._.wt,
				g_oErrors = Cn._.wive,
				g_oInputs = Cn._.wi,
				g_oThis = this,
				g_oDIV = null
			;

				//#### Declare the required 'public' enums
			this.enumErrorUIHookTypes = {
				cnXY : 0,
				cnPopUp : 1,
				cnToolTip : 2,
				cnInLabel : 3,
				cnValidateOnBlur : 4
			};

				//#### Declare the required 'private' 'constants'
			var g_cBASEID = 'Cn.Inputs.Validation.DisplayErrors',			//# was: g_oWebSettingsValue.DOMElementPrefix + 'FormErrorsDIV'
				g_cPAGEEDGEMARGIN = 4,
				g_cCURSORMARGIN = 15
			;


			//##########################################################################################
			//# Procedural Code
			//##########################################################################################
				//#### Write out our ._DIVID to the document
			document.write("<div id='" + g_cBASEID + "' class='" + g_oWebSettingsValue.CSSClass_PopUpErrorDIV + "' style='position: absolute; visibility: hidden;'></div>");
			YAHOO.util.Event.onContentReady(g_cBASEID, function() { g_oDIV = g_oWebTools.GetByID(g_cBASEID) }, null);


			//##########################################################################################
			//# 'Public' Properties
			//##########################################################################################
			//############################################################
			//# Gets/sets the header and footer strings used when constructing the error message DIV.
			//############################################################
			//# Last Updated: January 13, 2010
			this.Header = '';
			this.Footer = '';


			//##########################################################################################
			//# 'Public' Functions
			//##########################################################################################
			//############################################################
			//# Attaches the requested error popup type to the provided list of inputs
			//############################################################
			//# Last Updated: February 19, 2010
			this.AttachListeners = function(a_oInputs, eErrorPopupType, iX, iY) {
				var oInput, i;

//# <DebugCode>
					//#### If the YAHOO.util.Event object does not exist, popup the error
				if (typeof(YAHOO) == 'undefined' || ! YAHOO.util || ! YAHOO.util.Event) {
					alert("Cn.Inputs.Validation.Errors: [DEVELOPER] 'yui/event.js' must be included before referencing this code.");
				}
					//#### Else the YAHOO.util.Event object exists
				else {
//# </DebugCode>
						//#### If we have a_oInputs to traverse
					if (a_oInputs && a_oInputs.length > 0) {
							//#### Traverse the a_oInputs
						for (i = 0; i < a_oInputs.length; i++) {
								//#### Collect the current oInput (while ensuring it's an oInput reference)
							oInput = g_oInputs.Input(a_oInputs[i]);

								//#### If the oInput is valid
							if (oInput && oInput.type) {
									//#### If the oInput is hidden, try to collect it's .ReadOnlyID instead
									//####     NOTE: If there is no .ReadOnlyID for the hidden oInput that is ok, as there is no need to hook a hidden input
								if (oInput.type.toLowerCase() == 'hidden') {
									oInput = g_oWebTools.GetByID(g_oInputs.ReadOnlyID(g_oWebTools.GetIdentifier(oInput)));
								}
									//#### Else if this is a HTMLEditor, try to collect it's .Canvas instead
								else if (oInput.type.toLowerCase() == 'textarea' && Cn._.wih && Cn._.wih.Exists(oInput) && Cn._.wih.Data(oInput).Canvas) {
									oInput = Cn._.wih.Data(oInput).Canvas;
								}
							}

								//#### If we have an oInput to process
							if (oInput && g_oElementWorkspace[oInput.id] && g_oElementWorkspace[oInput.id].Validater) { // oInput.id && oInput.id.length != '') {
									//#### Determine the eErrorPopupType, attaching to the current oInput accordingly
									//####     NOTE: A new oArgument is created inline because it is passed byref
								switch (eErrorPopupType) {
									case this.enumErrorUIHookTypes.cnPopUp: {
										YAHOO.util.Event.addListener(oInput, 'focus', Cn._.wiveu.ShowPopUp, { Input: oInput }, this);
										YAHOO.util.Event.addListener(oInput, 'blur', Cn._.wiveu.Hide, null, this);
										break;
									}
									case this.enumErrorUIHookTypes.cnXY: {
										YAHOO.util.Event.addListener(oInput, 'mouseover', Cn._.wiveu.ShowXY, { Input: oInput, x: iX, y: iY }, this);
										YAHOO.util.Event.addListener(oInput, 'mouseout', Cn._.wiveu.Hide, null, this);
										break;
									}
									case this.enumErrorUIHookTypes.cnInLabel: {
										YAHOO.util.Event.addListener(oInput, 'change', Cn._.wiveu.ShowInLabel, { Input: oInput }, this);
										YAHOO.util.Event.addListener(oInput, 'focus', Cn._.wiveu.ShowInLabel, { Input: oInput }, this);
										break;
									}
									case this.enumErrorUIHookTypes.cnValidateOnBlur : {
										YAHOO.util.Event.addListener(oInput, 'blur', g_oElementWorkspace[oInput.id].Validate, { Input: oInput }, this);
										break;
									}
									default: { //# this.enumErrorUIHookTypes.cnToolTip
										YAHOO.util.Event.addListener(oInput, 'mousemove', Cn._.wiveu.ShowToolTip, { Input: oInput }, this);
										YAHOO.util.Event.addListener(oInput, 'mouseout', Cn._.wiveu.Hide, null, this);
										break;
									}
								}
							}
						}
					}
//# <DebugCode>
				}
//# </DebugCode>
			};

			//############################################################
			//# Prompts the user via an alert that errors have occured
			//############################################################
			//# Last Updated: December 17, 2009
			this.PromptUser = function() {
				alert(Cn._.wive.Messages.Alert);
			}

/*
this.ShowIn = function(oInput, oElement) {
};
*/

			//############################################################
			//# Shows the error message within the related input label
			//############################################################
			//# Last Updated: January 14, 2010
			this.ShowInLabel = function(oInput) {
				var oLabel, sInputName, sHTML;

					//#### If this is a call from .AttachToUI, peal the .Input from the oArgument
				if (arguments.length == 2 && arguments[1].Input) {
					var oArgument = arguments[1];
					oInput = oArgument.Input;
				}

					//#### Determine the oLabel, sInputName and setup the oLabel's .ElementWorkspace entry from the passed oInput
				oLabel = g_oInputs.RelatedLabel(oInput);
				sInputName = g_oWebTools.GetIdentifier(oInput);
				g_oElementWorkspace[oLabel.id] = g_oElementWorkspace[oLabel.id] || {};

					//#### If we could locate the oLabel and the oInput has a .Validate
				if (oLabel && g_oElementWorkspace[oInput.id] && g_oElementWorkspace[oInput.id].Validate) {
						//#### If there is some .OrigionalInnerHTML, reset the oLabel's .InnerHTML and destroy the entry
					if (g_oElementWorkspace[oLabel.id].OrigionalInnerHTML) {
						g_oWebTools.InnerHTML(oLabel, g_oElementWorkspace[oLabel.id].OrigionalInnerHTML);
						g_oElementWorkspace[oLabel.id].OrigionalInnerHTML = null;
					}

						//#### If an .hInputError is defined for the passed oInput
					if (g_oValidation.Data(oInput).hInputErrors[sInputName]) {
							//#### Store the current .InnerHTML into the .ElementWorkspace, then append the .message into it
						sHTML = g_oWebTools.InnerHTML(oLabel);
						g_oElementWorkspace[oLabel.id].OrigionalInnerHTML = sHTML;
						g_oWebTools.InnerHTML(oLabel, sHTML + "<div>" + g_oValidation.Data(oInput).hInputErrors[sInputName].message + "</div>");
					}
				}
			};

			//############################################################
			//# Shows the error message at the passed iX/iY coords if the passed oInput has an error defined
			//############################################################
			//# Last Updated: January 14, 2010
			this.ShowXY = function(oInput, iX, iY) {
					//#### If this is a call from .AttachToUI, peal the iX/iY coords from the oArgument object passed into iX
				if (arguments.length == 2 && arguments[1].Input) {
					var oArgument = arguments[1];
					oInput = oArgument.Input;
					iY = oArgument.y;
					iX = oArgument.x;
				}

					//#### Pass the call off to our 'private' ._DoShow function, signaling it to show the error as a .cnXY
				DoShow(oInput, iX, iY, null, this.enumErrorUIHookTypes.cnXY);
			};

			//############################################################
			//# Shows a tooltip-style error message if the passed oInput has an error defined
			//############################################################
			//# Last Updated: December 3, 2009
			this.ShowToolTip = function(oInput, oEvent) {
					//#### If this is a call from .AttachToUI, peal the oInput from the oArgument object passed into iX
					//####     NOTE: We need to juggle the arguments through oTempEvent as oInput, oEvent and arguments[x] are all references to teh same underlying data
				if (arguments[1].Input) {
					var oTempEvent = arguments[0];
					oInput = arguments[1].Input;
					oEvent = oTempEvent;
				}

					//#### If the passed oEvent is invalid, reset it to the window's .event
				if (! oEvent)	{ oEvent = window.event; }

					//#### Pass the call off to our 'private' ._DoShow function, signaling it to show the error as a .cnToolTip
				DoShow(oInput, 0, 0, oEvent, this.enumErrorUIHookTypes.cnToolTip);
			};

			//############################################################
			//# Shows a popup-style error message if the passed oInput has an error defined
			//############################################################
			//# Last Updated: August 30, 2007
			this.ShowPopUp = function(oInput) {
					//#### If this is a call from .AttachToUI, peal the oInput from the oArgument object passed into iX
				if (arguments.length == 2 && arguments[1].Input) {
					oInput = arguments[1].Input;
				}

					//#### Pass the call off to our 'private' ._DoShow function, signaling it to show the error as a .cnPopUp
				DoShow(oInput, 0, 0, null, this.enumErrorUIHookTypes.cnPopUp);
			};

			//############################################################
			//# Hides the error message from view
			//############################################################
			//# Last Updated: September 7, 2007
			this.Hide = function() {
					//#### .Hide our ._DIV and move it to X/Y 0/0 (so that the scrollbars are properly reset)
				g_oWebTools.Hide(g_oDIV);
				g_oWebTools.Left(g_oDIV, 0);
				g_oWebTools.Top(g_oDIV, 0);

					//#### If the user agent is either IE or Netscape 4
				if (g_oWebTools.IsIE() || g_oWebTools.IsNN4()) {
						//#### Ensure that any troublesome z-index elements are properly un-hidden underneath our g_oDIV
					g_oWebTools.ToggleOverlappingElements(g_oDIV, Array('SELECT'), null);
				}
			};


			//##########################################################################################
			//# 'Private', Pseudo-'Static' Functions
			//# 
			//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
			//##########################################################################################
			//############################################################
			//# Shows the error message at the passed iX/iY coords if the passed oInput has an error defined
			//############################################################
			//# Last Updated: December 8, 2009
			var DoShow = function(oInput, iX, iY, oEvent, ePositioning) {
				var sInputName, sFormID;

					//#### Ensure the passed oInput is a reference
				oInput = g_oInputs.Input(oInput);

					//#### If the passed oInput was valid
				if (oInput) {
						//#### Determine the sInputName and the sFormID from the passed oInput
					sInputName = g_oWebTools.GetIdentifier(oInput);
					sFormID = g_oWebTools.GetIdentifier(g_oInputs.RelatedForm(oInput));

						//#### If an .hInputError is defined for the passed oInput
					if (g_oValidation.Data(sFormID).hInputErrors[sInputName]) {
							//#### Reset the HTML within our ._DIV
						g_oWebTools.InnerHTML(g_oDIV, g_oThis.Header + g_oValidation.Data(sFormID).hInputErrors[sInputName].message + g_oThis.Footer);

							//#### Determine the ePositioning type, (re)setting iX/iY accordingly
						switch (ePositioning) {
								//#### If this is a .cnToolTip ePositioning request
							case g_oThis.enumErrorUIHookTypes.cnToolTip: {
									//#### Reset the passed iX/iY coords to that of the passed oEvent
								iX = g_oWebTools.EventX(oEvent);
								iY = (g_oWebTools.EventY(oEvent) + g_cCURSORMARGIN);

									//#### Ensure that the our ._DIVID is fully visiable on screen in the X/Y coords
								if ((iX + g_oWebTools.Width(g_oDIV) + g_cPAGEEDGEMARGIN) >= (g_oWebTools.WindowWidth() + g_oWebTools.WindowScrollLeft())) {
									iX = (g_oWebTools.WindowWidth() + g_oWebTools.WindowScrollLeft() - g_oWebTools.Width(g_oDIV) - g_cPAGEEDGEMARGIN);
								}
								if ((iY + g_oWebTools.Height(g_oDIV) + g_cPAGEEDGEMARGIN) >= (g_oWebTools.WindowHeight() + g_oWebTools.WindowScrollTop())) {
									iY = (g_oWebTools.WindowHeight() + g_oWebTools.WindowScrollTop() - g_oWebTools.Height(g_oDIV) - g_cPAGEEDGEMARGIN);
								}
									//#### Ensure that the our ._DIVID is never above the top left of the screen
								if (iX < 0)		{ iX = 0; }
								if (iY < 0)		{ iY = 0; }
								break;
							}
								//#### If this is a .cnPopUp ePositioning request
							case g_oThis.enumErrorUIHookTypes.cnPopUp: {
									//#### Reset the passed iX/iY coords to that of the passed oInput
								iX = g_oWebTools.Left(oInput);
								iY = (g_oWebTools.Top(oInput) - g_oWebTools.Height(g_oDIV) - 1);

									//#### Ensure that the our ._DIVID is fully visiable on screen in the X/Y coords
								if ((iX + g_oWebTools.Width(g_oDIV) + g_cPAGEEDGEMARGIN) >= (g_oWebTools.WindowWidth() + g_oWebTools.WindowScrollLeft())) {
									iX = (g_oWebTools.WindowWidth() + g_oWebTools.WindowScrollLeft() - g_oWebTools.Width(g_oDIV) - g_cPAGEEDGEMARGIN);
								}
								if ((iY + g_oWebTools.Height(g_oDIV) + g_cPAGEEDGEMARGIN) >= (g_oWebTools.WindowHeight() + g_oWebTools.WindowScrollTop())) {
									iY = (g_oWebTools.WindowHeight() + g_oWebTools.WindowScrollTop() - g_oWebTools.Height(g_oDIV) - g_cPAGEEDGEMARGIN);
								}
									//#### Ensure that the our ._DIVID is never above the top left of the screen
								if (iX < 0)		{ iX = 0; }
								if (iY < 0)		{ iY = 0; }
								break;
							}
								//#### Else this must be a .cnXY ePositioning request
							default: { //# this.enumErrorUIHookTypes.cnXY
									//#### Since a .cnXY ePositioning request uses the developer provided iX/iY, do nothing
							  //iX = iX;
							  //iY = iY;
								break;
							}
						}

							//#### Position our g_oDIV based on the above determined iX/iY and .show it to the user
						g_oWebTools.Left(g_oDIV, iX);
						g_oWebTools.Top(g_oDIV, iY);
						g_oWebTools.Show(g_oDIV);

							//#### If the user agent is either IE or Netscape 4
						if (g_oWebTools.IsIE() || g_oWebTools.IsNN4()) {
								//#### Collect any a_oChildren SELECTs under our g_oDIV, then .push in the oInput (just in case it too is a troublesome z-index element)
							var a_oChildren = g_oWebTools.ChildrenOf(g_oDIV, ['SELECT']);
							a_oChildren.push(oInput);

								//#### Ensure that any non-a_oChildren troublesome z-index elements are hidden underneath our g_oDIV
							g_oWebTools.ToggleOverlappingElements(g_oDIV, ['SELECT'], a_oChildren);
						}
					}
				}
			};


		}; //# Cn.Inputs.Validation.Errors.UI


	}; //# Cn.Inputs.Validation.Errors


    //########################################################################################################################
    //# InputMetaDataEntry class
    //# 
    //#     Required Includes: [none]
    //########################################################################################################################
    //# Last Code Review: December 7, 2009
    this.InputMetaDataEntry = function(eDataType, bIsRequired, iMaximumCharacterLength, iNumericPrecision, sMinimumNumericValue, sMaximumNumericValue, bDateTime_ValidateDataType) {
			//#### Default our public properties to the passed values
		this.DataType = eDataType;
		this.IsRequired = bIsRequired;
		this.MaximumCharacterLength = iMaximumCharacterLength;
		this.NumericPrecision = iNumericPrecision;
		this.MinimumNumericValue = sMinimumNumericValue;
		this.MaximumNumericValue = sMaximumNumericValue;
		this.DateTime_ValidateDataType = bDateTime_ValidateDataType;
	};


    //########################################################################################################################
    //# Validater class that is attached to inputs
    //# 
    //#     Required Includes: Cn/Inputs/Inputs.js
    //########################################################################################################################
    //# Last Code Review: December 8, 2009
    function Validater(oInput) {
			//#### Declare the required 'private' variables
        var ga_fValidationFunctions = new Array(),
			g_oValidation = Cn._.wiv,
			g_oWebTools = Cn._.wt,
			g_oErrors = Cn._.wive,
			g_oInputs = Cn._.wi,
			g_oInput = g_oInputs.Input(oInput)
		;


        //############################################################
        //# Adds a function to the validation list of functions.
        //############################################################
        //# Last Updated: January 13, 2010
        this.Add = function(fFunction) {
			var bReturn = (typeof(fFunction) == 'function');

                //#### If the passed fFunction is a 'function', .push it into our ga_fValidationFunctions
            if (bReturn) {
                ga_fValidationFunctions.push(fFunction);
            }

                //#### Return the above determined bReturn value to the caller
            return bReturn;
        };

        //############################################################
        //# Clears the validation list of functions.
        //############################################################
        //# Last Updated: January 13, 2010
        this.Exists = function(fFunction) {
			var bReturn = false;

				//#### Traverse the ga_fValidationFunctions
			for (var i = 0; i < ga_fValidationFunctions.length; i++) {
					//#### If the current ga_fValidationFunctions matches the passed fFunction, flip our bReturn value and fall from the loop
				if (ga_fValidationFunctions[i] == fFunction) {
					bReturn = true;
					break;
				}
			}

                //#### Return the above determined bReturn value to the caller
            return bReturn;
        };

        //############################################################
        //# Clears the validation list of functions.
        //############################################################
        //# Last Updated: January 13, 2010
        this.Remove = function(fFunction) {
			var bReturn = false;

				//#### Traverse the ga_fValidationFunctions
			for (var i = 0; i < ga_fValidationFunctions.length; i++) {
					//#### If the current ga_fValidationFunctions matches the passed fFunction .splice it from the array, flip our bReturn value and fall from the loop
				if (ga_fValidationFunctions[i] == fFunction) {
					ga_fValidationFunctions.splice(i, 1);
					bReturn = true;
					break;
				}
			}

                //#### Return the above determined bReturn value to the caller
            return bReturn;
        };

        //############################################################
        //# Validates the related input against the defined validation list of functions.
        //# 
        //#     NOTE: Validation functions recieve an object with the following properties:
        //#         * Input; A Reference to the related HTML input.
        //#         * Values; An Array of Strings containing the values for the referenced input.
        //#         * ValidateArguments; An Array of Objects representing the arguments passed into the .Validate function (i.e. the inbuilt JavaScript 'arguments' array).
        //############################################################
        //# Last Updated: January 13, 2010
        this.Validate = function() {
			var oInputAlias = g_oValidation.InputAlias(g_oInput),
				bDoValidate = true
			;

				//#### If this is a g_oRendererForm g_oInput
				//####     NOTE: When inputs are assoicated with a g_oRendererForm, all of the InputAliases are _#'d even if they are not directly managed by the g_oRendererForm
				//####     NOTE: We do not need to test for "g_oValidation.Data(g_oInput).IsRendererForm" below as that is run within .InputAlias, and it doesn't return a positive .RecordIndex unless it is true
				//####     NOTE: We use "Cn._.wrf" inline below as it's only conditionally required.
			if (oInputAlias.RecordIndex > 0 && Cn._.wrf) {
					//#### Reset bDoValidate based on if the .RecordExists (which we check just in case the g_oInput has a colliding name) and we are supposed to .Process(the)Record
					//####     NOTE: Even though unattached a_sInputAliases can be included above, they are logicially ignored by .WasUpdated within .ProcessRecord
				bDoValidate = (Cn._.wrf.RecordExists(g_oInput, oInputAlias.RecordIndex) &&
					Cn._.wrf.ProcessRecord(g_oInput, oInputAlias.RecordIndex, g_oValidation.Data[oFormReference].InputAliases())
				);
			}

				//#### If we are still supposed to bDoValidate
			if (bDoValidate) {
					//#### Traverse the ga_fValidationFunctions, calling each function as we go passing in our g_oInput
				for (var i = 0; i < ga_fValidationFunctions.length; i++) {
					ga_fValidationFunctions[i]({
						Input: g_oInput,
						Values: g_oInputs.Values(g_oInput),
						ValidateArguments: arguments
					});
				}
			}

				//#### Return based on the value of our .Error
            return (this.Error == g_oValidation.enumInputErrors.cnNoError);
        };

		//############################################################
		//# Determines the error that has been set for this input
		//############################################################
		//# Last Updated: December 8, 2009
		this.Error = function() {
			var sID = g_oWebTools.GetIdentifier(g_oInput)
				oErrorData
			;

				//#### First attempt to collect the .hInputErrors for the sID, then for the .ReadOnlyID, then give up =)
			oErrorData = (
				g_oValidation.Data(g_oInput).hInputErrors[sID] ||
				g_oValidation.Data(g_oInput).hInputErrors[g_oInputs.ReadOnlyID(sID)] ||
				null
			);

				//#### If we successfully collected the oErrorData above, return it's .error, else return .cnNoError
			return (oErrorData ? oErrorData.error : g_oValidation.enumInputErrors.cnNoError);
		};

		//############################################################
		//# Clears the error registered against this input (if any)
		//############################################################
		//# Last Updated: December 7, 2009
		this.ClearError = function() {
				//#### Pass the call off to .Set to clear any previously set eErrors
				//####     NOTE: The DataType is ignored for .cnNoErrors, and hence there's no need to collect it from the .Data
			g_oErrors.Set(oInput, g_oValidation.enumInputErrors.cnNoError, g_oInputs.enumDataTypes.cnUnknown, '');
		};


    }; //# Cn.Web.Inputs.Validation.Validater

	
}; //# Cn.Web.Inputs.Validation
