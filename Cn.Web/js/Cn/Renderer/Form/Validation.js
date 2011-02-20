//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined') {
	alert("Cn.Renderer.Form.Validation: [DEVELOPER] 'Cn/Cn.js' must be included before referencing this code.");
}
else if (! Cn.Tools) {
	alert("Cn.Renderer.Form.Validation: [DEVELOPER] 'Cn/Tools.js' must be included before referencing this code.");
}
else if (! Cn.Settings) {
	alert("Cn.Renderer.Form.Validation: [DEVELOPER] 'Cn/Settings.js.*' must be included before referencing this code.");
}
else if (! Cn.Renderer || ! Cn.Renderer.Form) {
	alert("Cn.Renderer.Form.Validation: [DEVELOPER] 'Cn/Renderer/Form/Form.js' must be included before referencing this code.");
}
else if (! Cn.Renderer.Form.Errors) {
	alert("Cn.Renderer.Form.Validation: [DEVELOPER] 'Cn/Renderer/Form/Errors.js' must be included before referencing this code.");
}
//# </DebugCode>


//########################################################################################################################
//# Validation class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Tools.js, Cn/Settings.js.*, Cn/Renderer/Form/Form.js, Cn/Renderer/Form/Errors.js
//########################################################################################################################
//# Last Code Review: February 24, 2006
Cn.Renderer.Form.Validation = new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.val = this;


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Validates the referenced HTML form based on it's defined validation function
	//############################################################
	//# Last Updated: May 1, 2006
	this.Validate = function(sFormName, bSubmitForm) {
			//#### Pass the call off to the defined .fValidate function for the sFormName
		return Cn._.frm._Forms(sFormName).fValidate(bSubmitForm);
	}

	//############################################################
	//# Defines the validation function for the referenced form name.
	//############################################################
	//# Last Updated: May 1, 2006
	this.Define = function(sFormName, fValidationFunction) {
			//#### (Re)Define the sFormName's related .fValidate function
		Cn._.frm._Forms(sFormName).fValidate = fValidationFunction;
	}

	//############################################################
	//# Determines if the .value in the passed sFormName/sInputName input is required
	//############################################################
	//# Last Updated: April 19, 2006
//! name?
	this.IsRequired = function(sFormName, sInputName, bIsRequired) {
		var rForm = Cn._.frm;
		var sValue = rForm.InputValue(sFormName, sInputName, true);
		var eError = rForm.enumFormErrors.cnNoError;

			//#### If a sValue is not present but bIsRequired, set eError to _ValueIsRequired
		if (sValue.length == 0 && bIsRequired) {
			eError = rForm.enumFormErrors.cnValueIsRequired;
		}

			//#### Call Errors.Set to set/clear the above determined eError
		Cn._.err.Set(sFormName, sInputName, eError, rForm.enumDataTypes.cnBoolean, '');

			//#### Return the above determined eError (in case the developer is calling this)
		return eError;
	};

	//############################################################
	//# Determines if the .value in the passed sFormName/sInputName input is a boolean value
	//############################################################
	//# Last Updated: May 1, 2006
	this.IsBoolean = function(sFormName, sInputName, bIsRequired) {
		var rForm = Cn._.frm;
		var sValue = rForm.InputValue(sFormName, sInputName, true);
		var eError = rForm.enumFormErrors.cnNoError;

			//#### If there is a sValue to process
		if (sValue.length > 0) {
				//#### If the sValue isN(ot)aN(umber) (as numbers are boolean in nature)
			if (isNaN(sValue)) {
					//#### Determine the lowercased sValue, setting eError if an unreconized sValue is present
				switch (sValue.toLowerCase()) {
					case 'true':
					case 'false':
					case 'yes':
					case 'no': {
						break;
					}

					default: {
						eError = rForm.enumFormErrors.cnIncorrectDataType;
						break;
					}
				}
			}
		}
			//#### Else if a the sValue bIsRequired, set eError to .cnValueIsRequired
		else if (bIsRequired) {
			eError = rForm.enumFormErrors.cnValueIsRequired;
		}

			//#### Call Errors.Set to set/clear the above determined eError
		Cn._.err.Set(sFormName, sInputName, eError, rForm.enumDataTypes.cnBoolean, '');

			//#### Return the above determined eError (in case the developer is calling this)
		return eError;
	};

	//############################################################
	//# Determines if the .value in the passed sFormName/sInputName input is a valid integer value
	//############################################################
	//# Last Updated: April 19, 2006
	this.IsInteger = function(sFormName, sInputName, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue) {
			//#### Pass the call off to .IsNumeric signaling it is an .cnInteger test, returning its return value as our own
		return IsNumeric(sFormName, sInputName, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue, Cn._.frm.enumDataTypes.cnInteger);
	};

	//############################################################
	//# Determines if the .value in the passed sFormName/sInputName input is a valid float value
	//############################################################
	//# Last Updated: April 19, 2006
	this.IsFloat = function(sFormName, sInputName, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue) {
			//#### Pass the call off to .IsNumeric signaling it is an .cnFloat test, returning its return value as our own
		return IsNumeric(sFormName, sInputName, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue, Cn._.frm.enumDataTypes.cnFloat);
	};

	//############################################################
	//# Determines if the .value in the passed sFormName/sInputName input is a valid currency value
	//############################################################
	//# Last Updated: April 19, 2006
	this.IsCurrency = function(sFormName, sInputName, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue) {
			//#### Pass the call off to .IsNumeric signaling it is an .cnCurrency test, returning its return value as our own
		return IsNumeric(sFormName, sInputName, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue, Cn._.frm.enumDataTypes.cnCurrency);
	};

	//############################################################
	//# Determines if the .value in the passed sFormName/sInputName input is a valid date/time
	//############################################################
	//# Last Updated: April 19, 2006
	this.IsDateTime = function(sFormName, sInputName, bIsRequired, bValidateDataType) {
		var rForm = Cn._.frm;
		var dDate, sValue, eError;

			//#### Retrieve the sValue for the passed sFormName/sInputName and default eError to _NoError
		sValue = rForm.InputValue(sFormName, sInputName, true);
		eError = rForm.enumFormErrors.cnNoError;

			//#### If there is a sValue to process
		if (sValue.length > 0) {
				//#### If we are to bValidate(the)DataType and the sValue .Is(not a valid)DateTime, set eError to .cnIncorrectDataType
			if (bValidateDataType && ! Cn._.t.IsDateTime(sValue)) {
				eError = rForm.enumFormErrors.cnIncorrectDataType;
			}
		}
			//#### Else if a sValue bIsRequired, set eError to .cnValueIsRequired
		else if (bIsRequired) {
			eError = rForm.enumFormErrors.cnValueIsRequired;
		}

			//#### Call Errors.Set to set/clear the above determined eError
		Cn._.err.Set(sFormName, sInputName, eError, rForm.enumDataTypes.cnDateTime, '');

			//#### Return the above determined eError (in case the developer is calling this)
		return eError;
	};

	//############################################################
	//# Determines if the .value in the passed sFormName/sInputName input is a valid GUID
	//############################################################
	//# Last Updated: April 19, 2006
	this.IsGUID = function(sFormName, sInputName, bIsRequired) {
		var rForm = Cn._.frm;
		var sValue = rForm.InputValue(sFormName, sInputName, true);
		var eError = rForm.enumFormErrors.cnNoError;

			//#### If there is a sValue to process
		if (sValue.length > 0) {
				//#### If the sValue doesn't match the GUID RegExp pattern, set eError to .cnIncorrectDataType
			if (! sValue.match(/^(\{)[0-9a-f]{8}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{12}(\})$/gi)) {
				eError = rForm.enumFormErrors.cnIncorrectDataType;
			}
		}
			//#### Else if a sValue bIsRequired, set eError to .cnValueIsRequired
		else if (bIsRequired) {
			eError = rForm.enumFormErrors.cnValueIsRequired;
		}

			//#### Call Errors.Set to set/clear the above determined eError
		Cn._.err.Set(sFormName, sInputName, eError, rForm.enumDataTypes.cnGUID, '');

			//#### Return the above determined eError (in case the developer is calling this)
		return eError;
	};

	//############################################################
	//# Determines if the .value in the passed sFormName/sInputName input is a valid string
	//############################################################
	//# Last Updated: April 19, 2006
	this.IsString = function(sFormName, sInputName, bIsRequired, iMaxLength) {
		var rForm = Cn._.frm;
		var sValue = rForm.InputValue(sFormName, sInputName, true);
		var eError = rForm.enumFormErrors.cnNoError;

			//#### If there is a sValue to process
		if (sValue.length > 0) {
				//#### If the sValue is too long, set eError to .cnIncorrectLength
			if (sValue.length > iMaxLength) {
				eError = rForm.enumFormErrors.cnIncorrectLength;
			}
		}
			//#### Else if a sValue bIsRequired, set eError to .cnValueIsRequired
		else if (bIsRequired) {
			eError = rForm.enumFormErrors.cnValueIsRequired;
		}

			//#### Call Errors.Set to set/clear the above determined eError
		Cn._.err.Set(sFormName, sInputName, eError, rForm.enumDataTypes.cnChar, '');

			//#### Return the above determined eError (in case the developer is calling this)
		return eError;
	};


	//##########################################################################################
	//# 'Private', Pseudo-'Static' Functions
	//# 
	//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
	//##########################################################################################
	//############################################################
	//# Determines if the referenced value is numeric as defined by its referenced data type.
	//############################################################
	//# Last Updated: May 4, 2006
	var IsNumeric = function(sFormName, sInputName, bIsRequired, iNumericPrecision, sMinNumericValue, sMaxNumericValue, eDataType) {
		var rTools = Cn._.t;
		var rForm = Cn._.frm;
		var sValue, eError, nValue;

			//#### Retrieve the sValue for the passed sFormName/sInputName and default eError to _NoError
		sValue = rForm.InputValue(sFormName, sInputName, true);
		eError = rForm.enumFormErrors.cnNoError;

			//#### If this is a .cnCurrency eDataType, remove the .CurrencySymbol from the sValue
			//####     NOTE: IE errors when some currency symbols are transported via HTTP (while it doesn't error if it's loading the file from the file system!?)
		if (eDataType == rForm.enumDataTypes.cnCurrency) {
			var oRegEx = new RegExp(Cn._.s.CurrencySymbol, "gi");
			sValue = sValue.replace(oRegEx, '');
		}

			//#### If there is a sValue to process
		if (sValue.length > 0) {
				//#### Convert sValue into a Number
			nValue = rTools.MakeNumeric(sValue, null);

				//#### If the conversion failed, set eError to .cnIncorrectDataType
			if (nValue == null) {
				eError = rForm.enumFormErrors.cnIncorrectDataType;
			}
				//#### Else if this is an .cnInteger eDataType and the nValue is not a whole number, set eError to .cnIncorrectDataType
			else if (eDataType == rForm.enumDataTypes.cnInteger && Math.floor(nValue) != nValue) {
				eError = rForm.enumFormErrors.cnIncorrectDataType;
			}
				//#### 
			else {
					//#### If the sMinNumericValue or sMaxNumericValue have not been set, default to testing the value based on the iNumericPrecision
					//####     NOTE: This is done as a failsafe measure, as well as allowing partial functionality of datasources that have not yet been fully defined within DataSource.MetaData
					//####     NOTE: This is a "better then nothing" test, as testing the sValue against the iNumericPrecision will not guarentee that the value fits into the column
				if (sMinNumericValue.length == 0 || sMaxNumericValue.length == 0) {
						//#### If the nValue is too long, set eError to .cnIncorrectLength
					if (rTools.NumericPrecision(nValue) > iNumericPrecision) {
						eError = rForm.enumFormErrors.cnIncorrectLength;
					}
				}
					//#### Else the sMinNumericValue and sMaxNumericValue have been set
				else {
						//#### If the nValue is outside of the sMinNumericValue/sMaxNumericValue range, set eError to .cnIncorrectLength
					if (! rTools.IsNumberInRange(nValue, sMinNumericValue, sMaxNumericValue)) {
						eError = rForm.enumFormErrors.cnIncorrectLength;
					}
				}
			}
		}
			//#### Else if a sValue bIsRequired, set eError to .cnValueIsRequired
		else if (bIsRequired) {
			eError = rForm.enumFormErrors.cnValueIsRequired;
		}

			//#### Call Errors.Set to set/clear the above determined eError
		Cn._.err.Set(sFormName, sInputName, eError, eDataType, '');

			//#### Return the above determined eError (in case the developer is calling this)
		return eError;
	}

}; //# Cn.Renderer.Form.Validation
