//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined') {
	alert("Cn.Renderer.Form.Errors: [DEVELOPER] 'Cn/Cn.js' must be included before referencing this code.");
}
else if (! Cn.Tools) {
	alert("Cn.Renderer.Form.Errors: [DEVELOPER] 'Cn/Tools.js' must be included before referencing this code.");
}
else if (! Cn.Settings) {
	alert("Cn.Renderer.Form.Errors: [DEVELOPER] 'Cn/Settings.js.*' must be included before referencing this code.");
}
else if (! Cn.Renderer || ! Cn.Renderer.Form) {
	alert("Cn.Renderer.Form.Errors: [DEVELOPER] 'Cn/Renderer/Form/Form.js' must be included before referencing this code.");
}

	//#### If all of the required YAHOO objects do not exist, popup the related error message
else if (typeof(YAHOO) == 'undefined') {
	alert("Cn.Renderer.Form.DateTime: [DEVELOPER] 'yui/YAHOO.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Event) {
	alert("Cn.Renderer.Form.DateTime: [DEVELOPER] 'yui/Event.js' must be included before referencing this code.");
}
//# </DebugCode>


//########################################################################################################################
//# Errors class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Tools.js, Cn/Settings.js.*, Cn/Renderer/Form/Form.js, Cn/Renderer/Form/ErrorMessages.js.*, [yui/Yahoo.js], yui/Event.js
//########################################################################################################################
//# Last Code Review: February 23, 2006
Cn.Renderer.Form.Errors = new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.err = this;

		//#### Declare the required 'private' variables
	var g_oDIV = null;

		//#### Declare the required 'private' enums
	var enumShowErrorPosition = {
		cnXY : 0,
		cnPopUp : 1,
		cnToolTip : 2
	};

		//#### Declare the required 'private' 'constants'
	var g_cLABELIDSUFFIX = '_Label';
	var g_cBASEID = 'Cn.Renderer.Form.Errors';							//# was: Cn._.s.FormElementPrefix + 'FormErrorsDIV'
	var g_cPAGEEDGEMARGIN = 4;
	var g_cCURSORMARGIN = 15;


	//##########################################################################################
	//# Procedural Code
	//##########################################################################################
		//#### Write out our ._DIVID to the document and set it into WZ_DragDrop as a managed DIV
	document.write("<div id='" + g_cBASEID + "' class='" + Cn._.s.ErrorDIVCSSClass + "' style='visibility: hidden; position: absolute;'></div>");
	g_oDIV = Cn._.t.GetByID(g_cBASEID);


	//##########################################################################################
	//# 'Public' Properties
	//##########################################################################################
	//############################################################
	//# Gets the Error.Messages object, which contains the error message strings.
	//############################################################
	//# Last Updated: February 21, 2006
	this.Messages = null;

	//############################################################
	//# Gets/sets the header and footer strings used when constructing the error message DIV.
	//############################################################
	//# Last Updated: April 19, 2006
	this.DIVHeader = '';
	this.DIVFooter = '';


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Sets/resets the error information for the referenced input.
	//############################################################
	//# Last Updated: May 4, 2006
	this.Set = function(sFormName, sInputName, eError, eDataType, sDeveloperErrorMessage) {
		var rForm = Cn._.frm;
		var oInput = rForm.Input(sFormName, sInputName);
		var sErrorMessage, bApplyCSSToLabel;

			//#### If the oInput was successfully collected above
		if (oInput) {
				//#### Determine if we need to bApply(the)CSSTo(the)Label based on if the oInput's .type is hidden
			bApplyCSSToLabel = (oInput.type && oInput.type.toLowerCase() == 'hidden');

				//#### If an eError occured
			if (eError != rForm.enumFormErrors.cnNoError) {
					//#### Ensure the passed sDeveloperErrorMessage is a String
				sDeveloperErrorMessage = new String(sDeveloperErrorMessage);

					//#### If this is a .cnCustom error and a sDeveloperErrorMessage was passed in, set sErrorMessage to the passed sDeveloperErrorMessage
				if (eError == rForm.enumFormErrors.cnCustom && sDeveloperErrorMessage.length > 0) {
					sErrorMessage = sDeveloperErrorMessage;
				}
					//#### Else this is not a .cnCustom error and/or no sDeveloperErrorMessage was passed, so collect the sErrorMessage form .GetMessage
				else {
					sErrorMessage = this.GetMessage(eError, eDataType);
				}

					//#### If we need to bApply(the)CSSTo(the)Label, set the error info into the sInputName's ._LabelIDPrefix as well
				if (bApplyCSSToLabel) {
					var sLabelID = Cn._.s.FormElementPrefix + sInputName + g_cLABELIDSUFFIX;
					rForm._Forms(sFormName).hInputErrors[sLabelID] = { key:sLabelID, error:eError, message:sErrorMessage };
					rForm._Forms(sFormName).ErrorCount++;
				}
					//#### Else set the error info into the sInputName
				else {
					rForm._Forms(sFormName).hInputErrors[sInputName] = { key:sInputName, error:eError, message:sErrorMessage };
					rForm._Forms(sFormName).ErrorCount++;
				}

					//#### Prepend the .InputErrorCSSClass from the sInputName (or its asso. ._LabelIDPrefix)
				ManageClassName(sInputName, bApplyCSSToLabel, true);
			}
				//#### Else an eError has not occured
			else {
					//#### If the error info was applied to the sInputName's ._LabelIDPrefix, destroy it accordingly
				if (bApplyCSSToLabel) {
					rForm._Forms(sFormName).hInputErrors[Cn._.s.FormElementPrefix + sInputName + g_cLABELIDSUFFIX] = null;
				}
					//#### Else the error info was applied to the sInputName, so destroy it accordingly
				else {
					rForm._Forms(sFormName).hInputErrors[sInputName] = null;
				}

					//#### Remove the .InputErrorCSSClass from the sInputName (or its asso. ._LabelIDPrefix)
				ManageClassName(sInputName, bApplyCSSToLabel, false);
			}
		}
			//#### Else if an error occured, we can't sent/unset an error against a non-existent oInput so popup the error to the user/developer
		else if (eError != rForm.enumFormErrors.cnNoError) {
			alert(this.GetMessage(rForm.enumFormErrors.cnMissingInput, null) + "'" + sInputName + "'");
		}
	};

	//############################################################
	//# Streamlined interface to set a developer-defined custom error against the referenced input.
	//############################################################
	//# Last Updated: February 23, 2006
	this.SetCustom = function(sFormName, sInputName, sErrorMessage) {
		this.Set(sFormName, sInputName, Cn._.frm.enumFormErrors.cnCustom, -1, sErrorMessage);
	};

	//############################################################
	//# 
	//############################################################
	//# Last Updated: April 19, 2006
	this.Count = function(sFormName) {
		var rForm = Cn._.frm;
		var iReturn = -1;

			//#### If the sFormName is reconized, reset the iReturn value to its .ErrorCount
		if (rForm._Exists(sFormName)) {
			iReturn = rForm._Forms(sFormName).ErrorCount;
		}

			//#### Return the above determined iReturn value to the caller
		return iReturn;
	};

	//############################################################
	//# 
	//############################################################
	//# Last Updated: April 19, 2006
	this.ResetCount = function(sFormName) {
		var rForm = Cn._.frm;
		var bReturn = false;

			//#### If the sFormName is reconized, reset its .ErrorCount and flip the bReturn value to true
		if (rForm._Exists(sFormName)) {
			rForm._Forms(sFormName).ErrorCount = 0;
			bReturn = true;
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Clears all of the errors for the referenced record index.
	//############################################################
	//# Last Updated: April 19, 2006
	this.ClearRecord = function(sFormName, a_sInputAliases, iRecordIndex) {
		var rForm = Cn._.frm;
		var i;

			//#### If some a_sInputAliases were passed
		if (a_sInputAliases && a_sInputAliases.length > 0) {
				//#### Traverse the passed a_sInputAliases
			for (i = 0; i < a_sInputAliases.length; i++) {
					//#### Call .Set to clear any previously set eErrors
				this.Set(sFormName, rForm.GetInputName(a_sInputAliases[i], iRecordIndex), rForm.enumFormErrors.cnNoError, -1, '');
			}
		}
	};

	//############################################################
	//# Retrieves the corrosponding ErrorMessage for the passed eError/eDataType
	//############################################################
	//# Last Updated: April 19, 2006
	this.GetMessage = function(eError, eDataType) {
		var rForm = Cn._.frm;

//# <DebugCode>
			//#### If our error .Messages have not yet been setup, popup the error and return null
		if (this.Messages == null) {
			alert("Cn.Renderer.Form.Errors: [DEVELOPER] 'ErrorMessages.js." + Cn._.s.ScriptFileExtension + "' must be included before referencing this code.");
			return null;
		}
//# </DebugCode>
			//#### Else we have .ErrorMessages
		else {
				//#### Determine the value of the passed eError, returning accordingly
			switch (eError) {
				case rForm.enumFormErrors.cnValueIsRequired: {
					return this.Messages.ValueIsRequired;
					break;
				}
				case rForm.enumFormErrors.cnIncorrectLength: {
					return this.Messages.IncorrectLength;
					break;
				}
				case rForm.enumFormErrors.cnIncorrectDataType: {
						//#### Determine the value of the passed eDataType, returning accordingly
					switch (eDataType) {
						case rForm.enumDataTypes.cnBoolean: {
							return this.Messages.Boolean;
							break;
						}
						case rForm.enumDataTypes.cnInteger: {
							return this.Messages.Integer;
							break;
						}
						case rForm.enumDataTypes.cnFloat: {
							return this.Messages.Float;
							break;
						}
						case rForm.enumDataTypes.cnCurrency: {
							return this.Messages.Currency;
							break;
						}
						case rForm.enumDataTypes.cnDateTime: {
							return this.Messages.DateTime;
							break;
						}
						case rForm.enumDataTypes.cnGUID: {
							return this.Messages.GUID;
							break;
						}
						default: {
							return this.Messages.StringBased;
							break;
						}
					}
					break;
				}
				case rForm.enumFormErrors.cnNotWithinPicklist: {
					return this.Messages.NotWithinPicklist;
					break;
				}
				case rForm.enumFormErrors.cnCustom: {
						//#### We only get here if the developer has not setup a sDeveloperErrorMessage, so return the vanilla error message
					return this.Messages.Custom;
					break;
				}
				case rForm.enumFormErrors.cnUnknownOrUnsupportedType: {
					return this.Messages.UnknownOrUnsupportedType;
					break;
				}
				case rForm.enumFormErrors.cnMissingInput: {
					return this.Messages.MissingInput;
					break;
				}
				case rForm.enumFormErrors.cnNoError: {
					return this.Messages.NoError;
					break;
				}
				default: {
					return this.Messages.UnknownErrorCode;
					break;
				}
			}
		}
	};

	//############################################################
	//# Determines if an error has been set for the passed sFormName/sInputName
	//############################################################
	//# Last Updated: April 19, 2006
//IsSet
	this.HasError = function(sFormName, sInputName) {
		var rForm = Cn._.frm;

			//#### Return based on if an .hInputError is defined for the sInputName (or its ._LabelIDPrefix) for the passed sFormName
		return (rForm._Forms(sFormName).hInputErrors[sInputName] ||
			rForm._Forms(sFormName).hInputErrors[Cn._.s.FormElementPrefix + sInputName + g_cLABELIDSUFFIX]
		);
	};

	//############################################################
	//# Shows the error message at the passed iX/iY coords if the passed oInput has an error defined
	//############################################################
	//# Last Updated: May 9, 2006
	this.Show = function(sFormName, oInput, iX, iY) {
			//#### Pass the call off to our 'private' ._DoShow function, signaling it to show the error as a .cnXY
		DoShow(sFormName, oInput, iX, iY, null, enumShowErrorPosition.cnXY);
	};

	//############################################################
	//# Shows a tooltip-style error message if the passed oInput has an error defined
	//############################################################
	//# Last Updated: May 9, 2006
	this.ShowToolTip = function(sFormName, oInput, oEvent) {
			//#### If the passed oEvent is invalid, reset it to the window's .event
		if (! oEvent)	{ oEvent = window.event; }

			//#### Pass the call off to our 'private' ._DoShow function, signaling it to show the error as a .cnToolTip
		DoShow(sFormName, oInput, 0, 0, oEvent, enumShowErrorPosition.cnToolTip);
	};

	//############################################################
	//# Shows a popup-style error message if the passed oInput has an error defined
	//############################################################
	//# Last Updated: May 9, 2006
	this.ShowPopUp = function(sFormName, oInput) {
			//#### Pass the call off to our 'private' ._DoShow function, signaling it to show the error as a .cnPopUp
		DoShow(sFormName, oInput, 0, 0, null, enumShowErrorPosition.cnPopUp);
	};

	//############################################################
	//# Hides the error message from view
	//############################################################
	//# Last Updated: April 19, 2006
	this.Hide = function() {
		var rTools = Cn._.t;

			//#### .Hide our ._DIV and move it to X/Y 0/0 (so that the scrollbars are properly reset)
		rTools.Hide(g_oDIV);
		rTools.Left(g_oDIV, 0);
		rTools.Top(g_oDIV, 0);

			//#### If the user agent is either IE or Netscape 4
		if (rTools.IsIE() || rTools.IsNN4()) {
				//#### Ensure that any troublesome z-index elements are properly un-hidden underneath our ._DIVID
			rTools.ToggleOverlappingElements(g_cBASEID, Array('SELECT'), null);
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
	//# Last Updated: May 9, 2006
	var DoShow = function(sFormName, oInput, iX, iY, oEvent, ePositioning) {
		var rTools = Cn._.t;
		var rForm = Cn._.frm;
		var rThis = Cn._.err;
		var iX, iY;

			//#### Determine the sInputName of the passed oInput (optionally collecting it's .name if it's .id is not defined)
		var sInputName = oInput.id;
		if (! sInputName || sInputName.length == 0)		{ sInputName = oInput.name; }

			//#### If an .hInputError is defined for the passed oInput
		if (rForm._Forms(sFormName).hInputErrors[sInputName]) {
				//#### Reset the HTML within our ._DIV
			rTools.InnerHTML(g_oDIV, rThis.DIVHeader + rForm._Forms(sFormName).hInputErrors[sInputName].message + rThis.DIVFooter);

				//#### Determine the ePositioning type, (re)setting iX/iY accordingly
			switch (ePositioning) {
					//#### If this is a .cnToolTip ePositioning request
				case enumShowErrorPosition.cnToolTip: {
						//#### Reset the passed iX/iY coords to that of the passed oEvent
					iX = rTools.EventX(oEvent);
					iY = (rTools.EventY(oEvent) + g_cCURSORMARGIN);

						//#### Ensure that the our ._DIVID is fully visiable on screen in the X/Y coords
					if ((iX + rTools.Width(g_oDIV) + g_cPAGEEDGEMARGIN) >= (rTools.WindowWidth() + rTools.WindowScrollLeft())) {
						iX = (rTools.WindowWidth() + rTools.WindowScrollLeft() - rTools.Width(g_oDIV) - g_cPAGEEDGEMARGIN);
					}
					if ((iY + rTools.Height(g_oDIV) + g_cPAGEEDGEMARGIN) >= (rTools.WindowHeight() + rTools.WindowScrollTop())) {
						iY = (rTools.WindowHeight() + rTools.WindowScrollTop() - rTools.Height(g_oDIV) - g_cPAGEEDGEMARGIN);
					}
						//#### Ensure that the our ._DIVID is never above the top left of the screen
					if (iX < 0)		{ iX = 0; }
					if (iY < 0)		{ iY = 0; }
					break;
				}
					//#### If this is a .cnPopUp ePositioning request
				case enumShowErrorPosition.cnPopUp: {
						//#### Reset the passed iX/iY coords to that of the passed oInput
					iX = rTools.Left(oInput);
					iY = (rTools.Top(oInput) - rTools.Height(g_oDIV) - 1);

						//#### Ensure that the our ._DIVID is fully visiable on screen in the X/Y coords
					if ((iX + rTools.Width(g_oDIV) + g_cPAGEEDGEMARGIN) >= (rTools.WindowWidth() + rTools.WindowScrollLeft())) {
						iX = (rTools.WindowWidth() + rTools.WindowScrollLeft() - rTools.Width(g_oDIV) - g_cPAGEEDGEMARGIN);
					}
					if ((iY + rTools.Height(g_oDIV) + g_cPAGEEDGEMARGIN) >= (rTools.WindowHeight() + rTools.WindowScrollTop())) {
						iY = (rTools.WindowHeight() + rTools.WindowScrollTop() - rTools.Height(g_oDIV) - g_cPAGEEDGEMARGIN);
					}
						//#### Ensure that the our ._DIVID is never above the top left of the screen
					if (iX < 0)		{ iX = 0; }
					if (iY < 0)		{ iY = 0; }
					break;
				}
					//#### Else this must be a .cnXY ePositioning request
				default: { //# enumShowErrorPosition.cnXY
						//#### Since a .cnXY ePositioning request uses the developer provided iX/iY, do nothing
				  //iX = iX;
				  //iY = iY;
					break;
				}
			}

				//#### Position our ._DIVID based on the above determined iX/iY and .show it to the user
			rTools.Left(g_oDIV, iX);
			rTools.Top(g_oDIV, iY);
			rTools.Show(g_oDIV);

				//#### If the user agent is either IE or Netscape 4
			if (rTools.IsIE() || rTools.IsNN4()) {
					//#### Define and set a_sExemptIDs to any troublesome z-index elements under our ._DIVID
				var a_sExemptIDs = rTools.GetChildElementIDs(g_cBASEID, ['SELECT']);

					//#### If our ._DIV has some child elements, .push the sInputName onto the array (just in case it's a troublesome z-index element)
				if (a_sExemptIDs && a_sExemptIDs.length > 0) {
					a_sExemptIDs.push(sInputName);
				}
					//#### Else our ._DIVID had no child elements, so reset a_sExemptIDs to an array holding only the sInputName (just in case it's a troublesome z-index element)
				else {
					a_sExemptIDs = new Array(sInputName);
				}

					//#### Ensure that any non-exempt troublesome z-index elements are hidden underneath our ._DIVID
				rTools.ToggleOverlappingElements(g_cBASEID, ['SELECT'], a_sExemptIDs);
			}
		}
	};

	//############################################################
	//# Manages the .className of the referenced element, adding or removing the Input Error CSS Class as necessary.
	//############################################################
	//# Last Updated: April 21, 2006
	var ManageClassName = function(sID, bApplyCSSToLabel, bApply) {
		var rTools = Cn._.t;
		var oTarget;
		var sInputErrorCSSClass = String(Cn._.s.InputErrorCSSClass);
		var iLength = (sInputErrorCSSClass.length + 1); //# +1 to allow for the trailing space

			//#### If we are supposed to bApply(the)CSSTo(the)Label, set the oTarget to the passed sID's ._LabelIDPrefix
		if (bApplyCSSToLabel) {
			oTarget = rTools.GetByID(Cn._.s.FormElementPrefix + sID + g_cLABELIDSUFFIX);
		}
			//#### Else set the oTarget to the passed sID
		else {
			oTarget = rTools.GetByID(sID);
		}

			//#### If the above determined oTarget is a valid reference
		if (oTarget) {
				//#### If we are supposed to bApply the .InputErrorCSSClass to the .className
			if (bApply) {
					//#### If the .InputErrorCSSClass class is not already at the front of the .className, prepend it
				if (oTarget.className.substr(0, iLength) != sInputErrorCSSClass + ' ') {
					oTarget.className = sInputErrorCSSClass + ' ' + oTarget.className;
				}
			}
				//#### Else we're supposed to remove the .InputErrorCSSClass
			else {
					//#### If the .InputErrorCSSClass class is at the front of the .className, remove it
				if (oTarget.className.substr(0, iLength) == sInputErrorCSSClass + ' ') {
					oTarget.className = oTarget.className.substr(iLength, oTarget.className.length);
				}
			}
		}
	}

} //# Cn.Renderer.Form.Errors
