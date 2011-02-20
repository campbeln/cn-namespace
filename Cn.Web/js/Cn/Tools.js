//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined') {
	alert("Cn.Tools: [DEVELOPER] 'Cn/Cn.js' must be included before referencing this code.");
}

	//#### If all of the required YAHOO objects do not exist, popup the related error message
else if (typeof(YAHOO) == 'undefined') {
	alert("Cn.Tools: [DEVELOPER] 'yui/YAHOO.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Dom) {
	alert("Cn.Tools: [DEVELOPER] 'yui/Dom.js' must be included before referencing this code.");
}
//else if (! YAHOO.util || ! YAHOO.util.Event) {
//	alert("Cn.Tools: [DEVELOPER] 'yui/Event.js' must be included before referencing this code.");
//}
//# </DebugCode>


//########################################################################################################################
//# Tools class
//# 
//#     Required Includes: Cn/Cn.js, [yui/Yahoo.js], yui/Dom.js, yui/Event.js (for .EventX/.EventY)
//#     Missing YUI functionality: Clip, WindowsScrollTop, WindowsScrollLeft, Height, Width, InnerHTML, IsNumeric, IsIE, IsNN4, ~Left, ~Top, ~GetByID (X Lib's imp.s are better)
//########################################################################################################################
//# Last Code Review: February 23, 2006
Cn.Tools = new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.t = this;


	//##########################################################################################
	//# 'Public' Properties
	//##########################################################################################
	//############################################################
	//# Define the public temp variable stub
	//############################################################
	//# Last Updated: March 23, 2006
	this.TempVariable = null;


	//##########################################################################################
	//# 'Protected' Properties
	//##########################################################################################
	//############################################################
	//# Declare the required 'protected' (sub-class stub) properties
	//############################################################
	//# Last Updated: March 17, 2006
	this._x = null;


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Determines if the currently running client is Netscape Navigator 4
	//############################################################
	//# Last Updated: March 23, 2006
	this.IsNN4 = function() {
		return this._x.NN4;
	};

	//############################################################
	//# Determines if the currently running client is IE
	//############################################################
	//# Last Updated: March 23, 2006
	this.IsIE = function() {
		return this._x.IE4Up;
	};

	//############################################################
	//# Returns an object reference to the provided element ID.
	//# 
	//#     NOTE: The YUI equivlent within YAHOO.utils.Dom is not as universal as X Lib's implementation
	//############################################################
	//# Last Updated: March 17, 2006
	this.GetByID = function(sID) {
		return this._x.GetElementById(sID);
	};

	//############################################################
	//# Determines if the passed oValue is a number.
	//############################################################
	//# Last Updated: May 1, 2006
	this.IsNumeric = function(oValue) {
			//#### Return the inverted value returned from isNaN
		return (! isNaN(oValue));
	};

	//############################################################
	//# 
	//# 
	//#     NOTE: This function utilizes JavaScript's parseFloat function, so all it's caveats apply (specificially, "17.68 monkeys" equates to 17.68).
	//############################################################
	//# Last Updated: April 7, 2006
	this.MakeNumeric = function(oValue, fDefaultValue) {
		var fReturn = parseFloat(oValue);

			//#### If the passed oValue was not successfully parsed above, reset the fReturn value to the passed fDefaultValue
		if (! this.IsNumeric(fReturn)) {
			fReturn = fDefaultValue;
		}

			//#### Return the above determined fReturn value to the caller
		return fReturn;
	};

	//############################################################
	//# 
	//############################################################
	//# Last Updated: April 13, 2006
	this.IsDateTime = function(oDateTime) {
		var iEpoch = Date.parse(oDateTime);
		var bReturn = (oDateTime && oDateTime.setTime);

			//#### If the oDateTime is not a (seemingly) Date object
		if (! bReturn) {
				//#### Reset the return value to the initial iEpoch .parse
			bReturn = this.IsNumeric(iEpoch);

				//#### If the initial iEpoch .parse failed
			if (! bReturn) {
					//#### Retry the iEpoch collection by pre-pending a valid date, resetting the bReturn value based on it's success/failure
				iEpoch = Date.parse('2000-01-01 ' + oDateTime);
				bReturn = this.IsNumeric(iEpoch);
			}
		}
/*			//#### Determine the dDate defined within sValue
		dDate = new Date(sValue);

			//#### If the conversion failed
		if (! Cn._.t.IsNumeric(dDate)) {
				//#### To ensure the passed sValue is not time-only, prepend Jan 1, 2000 and retest
			dDate = new Date('01-01-2000 ' + sValue);

				//#### If the secondary conversion failed, set eError to .cnIncorrectDataType
			if (! Cn._.t.IsNumeric(dDate)) {
				eError = Cn._.frm.enumFormErrors.cnIncorrectDataType;
			}
		}
*/
			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# 
	//############################################################
	//# Last Updated: April 13, 2006
	this.MakeDateTime = function(oDateTime, dDefaultValue) {
		var dReturn = this.IsDateTime(dDefaultValue) ? dDefaultValue : new Date();
		var iEpoch = Date.parse(oDateTime);

			//#### If the iEpoch was successfully calculated from the passed oDateTime, reset the dReturn value to it
		if (this.IsNumeric(iEpoch)) {
			dReturn.setTime(iEpoch);
		}
			//#### Else the iEpoch was not successfully calculated from the passed oDateTime
		else {
				//#### Retry the iEpoch collection by pre-pending a valid date
			iEpoch = Date.parse('2000-01-01 ' + oDateTime);

				//#### If the iEpoch was successfully calculated from the modified oDateTime, reset the dReturn value to it
			if (this.IsNumeric(iEpoch)) {
				dReturn.setTime(iEpoch);
			}
		}

			//#### Return the above determined dReturn value to the caller
		return dReturn;
	};

	//############################################################
	//# Shows the referenced element.
	//############################################################
	//# Last Updated: April 21, 2006
	this.Show = function(oElement) {
		this.Style(this.GetByID(oElement), 'visibility', 'visible');
	};

	//############################################################
	//# Hides the referenced element.
	//############################################################
	//# Last Updated: April 21, 2006
	this.Hide = function(oElement) {
		this.Style(this.GetByID(oElement), 'visibility', 'hidden');
	};

	//############################################################
	//# Forces the browser to repaint the referenced element.
	//############################################################
	//# Last Updated: April 20, 2006
	this.Repaint = function(oElement) {
		this.Hide(oElement);
		this.Show(oElement);
	};

	//############################################################
	//# Determines if the referenced element is visible.
	//############################################################
	//# Last Updated: April 11, 2006
	this.IsVisible = function(oElement) {
			//#### Return based on the 'visibility' and the 'display' of the oElement
		return (this._x.Visibility(oElement) != 'hidden');
//!		return (this.Style(oElement, 'visibility').toLowerCase() != 'hidden' && this.Style(oElement, 'display').toLowerCase() != 'none');
	};

	//############################################################
	//# Determines the referenced element's style, optionally setting it to a new value.
	//############################################################
	//# Last Updated: April 19, 2006
	this.Style = function(oElement, sStyle, sNewValue) {
			//#### Ensure the passed oElement is a reference
		oElement = this.GetByID(oElement);

			//#### If the oElement is valid
		if (oElement) {
				//#### If the caller passed in a sNewValue, .set(the)Style
			if (typeof(sNewValue) != 'undefined') {
				YAHOO.util.Dom.setStyle(oElement, sStyle, sNewValue);
				return null;
			}
				//#### Else the caller must be requesting the current value of the sStyle, so .get(the)Style
			else {
				return YAHOO.util.Dom.getStyle(oElement, sStyle);
			}
		}
	};

	//############################################################
	//# 
	//############################################################
	//# Last Updated: March 24, 2006
	this.InnerHTML = function(oElement, sHTML) {
		return this._x.InnerHtml(oElement, sHTML);
	};

	//############################################################
	//# Pads the left side of the provided value with the referenced character.
	//############################################################
	//# Last Updated: April 14, 2006
	this.LPad = function(oText, sPadCharacter, iLength) {
		var sReturn = String(oText);
		var i;

			//#### Traverse the need to pad .length of the sReturn value, prepending a sPadCharacter as we go
		for (i = sReturn.length; i < iLength; i++) {
			sReturn = sPadCharacter + sReturn;
		}

			//#### Return the above determined sReturn value to the caller
		return sReturn;
	};

	//############################################################
	//# Retrieves a string array where each index represents the ID of a child element of the referenced element.
	//############################################################
	//# Last Updated: February 23, 2006
	this.GetChildElementIDs = function(sID, a_sTagNames) {
		var a_sDOMElements, oParent, i, j;
		var a_sReturn = new Array();
		var oID = this.GetByID(sID);

			//#### Traverse the passed a_sTagNames
		for (i = 0; i < a_sTagNames.length; i++) {
				//#### Collect the array of ELEMENTS from the DOM
			a_sDOMElements = document.getElementsByTagName(a_sTagNames[i].toUpperCase());

				//#### Traverse the collected a_sDOMElements
			for (j = 0; j < a_sDOMElements.length; j++) {
					//#### Set the oParent for the loop-and-a-half below
				oParent = a_sDOMElements[j].offsetParent;

					//#### While we have a valid oParent reference to process
				while (oParent) {
						//#### If the oParent equals the oID
					if (oParent == oID) {
							//#### If a valid .id is present, .push it onto the a_sReturn value
						if (a_sDOMElements[j].id && a_sDOMElements[j].id != '') {
							a_sReturn.push(a_sDOMElements[j].id)
						}
							//#### Else .push the .name onto the a_sReturn value
						else {
							a_sReturn.push(a_sDOMElements[j].name)
						}

							//#### Reset oPatent to null
						oParent = null;
					}
						//#### Else reset oParent to it's own parent
					else {
						oParent = oParent.offsetParent;
					}
				}
			}
		}

			//#### Return the above determined a_sReturn value to the caller
		return a_sReturn;
	};

	//############################################################
	//# Toggles the visibility of all of the referenced element types (excluding the exempted IDs) on the page that overlap the referenced element.
	//#    NOTE: This function really only needs to run correctly under those browsers that have Z-Index issues with certian elements, so true cross-platform compatibility is not really required for this function
	//############################################################
	//# Last Updated: April 26, 2006
	this.ToggleOverlappingElements = function(sID, a_sTagNames, a_sExemptIDs) {
		var oID = this.GetByID(sID);
		var a_sDOMElements, oElement, sNameID, bIsExempt, i, j, k;

			//#### If the passed a_sExemptIDs is not a valid array, reset it to a single element array containing the passed sID (just in case it's a troublesome z-index element)
		if (! a_sExemptIDs || ! a_sExemptIDs.length) {
			a_sExemptIDs = [sID];
		}
			//#### Else we have a valid a_sExemptIDs array, so .push the passed sID into it (just in case it's a troublesome z-index element)
		else {
			a_sExemptIDs.push(sID);
		}

			//#### Traverse the passed a_sTagNames
		for (i = 0; i < a_sTagNames.length; i++) {
				//#### Collect the array of ELEMENTS from the DOM
			a_sDOMElements = document.getElementsByTagName(a_sTagNames[i].toUpperCase());

				//#### Traverse the collected a_sDOMElements
			for (j = 0; j < a_sDOMElements.length; j++) {
					//#### Reset oElement and bIsExempt for this loop
				oElement = a_sDOMElements[j];
				bIsExempt = false;

					//#### If the current oElement has a valid .id, set sNameID to it
				if (oElement.id && oElement.id != '') {
					sNameID = oElement.id;
				}
					//#### Else default sNameID to the current oElement's .name
				else {
					sNameID = oElement.name;
				}

					//#### If some a_sExemptIDs were passed
				if (a_sExemptIDs != null && a_sExemptIDs.length > 0) {
						//#### Traverse a_sExemptIDs
					for (k = 0; k < a_sExemptIDs.length; k++) {
							//#### If the current oElement's sNameID equals the current a_sExemptIDs
						if (sNameID == a_sExemptIDs[k]) {
								//#### Flip bIsExempt and fall out of the loop
							bIsExempt = true;
							break;
						}
					}
				}

					//#### As long as the current oElement bIs(not)Exempt
				if (! bIsExempt) {
						//#### If the oID overlaps the current oElement and the oID .Is(currently)Visible
					if (this.Overlap(oID, oElement) && this.IsVisible(oID)) {
							//#### Call .UpdateOverlappingInfo, signaling it to append the sID for oElement
						UpdateOverlappingInfo(oElement, sID, true);
					}
						//#### Else they do not .Overlap (or the oID is not visible), but if the current oElement .Is(not currently)Visible
					else if (! this.IsVisible(oElement)) {
							//#### Call .UpdateOverlappingInfo, signaling it to remove the sID for oElement
						UpdateOverlappingInfo(oElement, sID, false);
					}
				}
			}
		}
	};


	//##########################################################################################
	//# Data Validation-related Functions
	//##########################################################################################
	//############################################################
	//# Determines if the passed sNumber is within the passed range
	//#    NOTE: "return (sNumber >= sRangeMin && sNumber <= sRangeMax)" would work in 99.9% of the checks we'll do with this function, but in the case of huge/tiny numbers (such as NUMERIC(x,y)'s in Oracle), this wouldn't cut it as the numbers would be too large/small to be represented in any available numeric variables
	//############################################################
	//# Last Updated: February 21, 2006
	this.IsNumberInRange = function(sNumber, sRangeMin, sRangeMax) {
		var bReturn = false;

			//#### If the passed sNumber is greater then or equal to the passed sRangeMin
		if (this.LargeNumberComparison(sNumber, sRangeMin) >= 0) {
				//#### If the passed sNumber is less then or equal to the passed sRangeMax
			if (this.LargeNumberComparison(sNumber, sRangeMax) <= 0) {
					//#### Since the passed sNumber is within the passed sRangeMin/sRangeMax, flip our bReturn value to true
				bReturn = true;
			}
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Determines if the passed sNumber is greater then, less then or equal to the passed sRange
	//#     Return values:
	//#          -1 if sNumber is less then sRange
	//#          1 if sNumber is greater then sRange
	//#          0 if the passed values are equal, or if one of the passed values was non-numeric
	//############################################################
	//# Last Updated: February 21, 2006
	this.LargeNumberComparison = function(sNumber, sRange) {
			//#### Ensure the passed sNumber and sRange are strings
		sNumber = new String(sNumber);
		sRange = new String(sRange);

			//#### Define and init the required local vars
		var iNumberNumericPrecision = this.NumericPrecision(sNumber);
		var iRangeNumericPrecision = this.NumericPrecision(sRange);
		var iReturn;
		var bNumberIsPositive = (sNumber.indexOf("-") != 0);
		var bRangeIsPositive = (sRange.indexOf("-") != 0);

			//#### If the passed sNumber or sRange were non-numeric, set our iReturn value to 0
		if (iNumberNumericPrecision == -1 || iRangeNumericPrecision == -1) {
			iReturn = 0;
		}
			//#### Else if the signs of the passed sNumber and sRange do not match
		else if (bNumberIsPositive != bRangeIsPositive) {
				//#### If the bNumberIsPositive, then the sRange is negetive, so set our iReturn value to 1 (as sNumber is greater then the sRange)
			if (bNumberIsPositive) {
				iReturn = 1;
			}
				//#### Else the sNumber is negetive and the bRangeIsPositive, so set our iReturn value to -1 (as sNumber is less then the sRange)
			else {
				iReturn = -1;
			}
		}
			//#### Else the signs of the passed sNumber and sRange match
		else {
				//#### If the above-determined .NumericPrecision's are specifying numbers of less then 1 billion
			if (iRangeNumericPrecision < 10 && iNumberNumericPrecision < 10) {
					//#### Define and init the additionally required vars
					//####     NOTE: We know that both sNumber and sRange are numeric as non-numeric value are caught by .NumericPrecision above
				var fNumber = parseFloat(sNumber);
				var fRange = parseFloat(sRange);

					//#### If the sNumber and sRange are equal, set our iReturn value to 0
				if (fNumber == fRange) {
					iReturn = 0;
				}
					//#### Else if the sNumber is greater then the sRange, set our iReturn value to 1
				else if (fNumber > fRange) {
					iReturn = 1;
				}
					//#### Else the fNumber is less then the sRange, so set our iReturn value to -1
				else {
					iReturn = -1;
				}
			}
				//#### Else we're dealing with number ranges over 1 billion, so let's get creative...
			else {
					//#### If the iNumber('s)NumericPrecision is less then the iRange('s)NumericPrecision
				if (iNumberNumericPrecision < iRangeNumericPrecision) {
						//#### If the bNumberIsPositive (and thanks to the check above the bRangeIs(also)Positive), return -1 (as the sNumber is a smaller positive number then the sRange, making it less)
					if (bNumberIsPositive) {
						iReturn = -1;
					}
						//#### Else the bNumberIs(not)Positive (and thanks to the check above the bRangeIs(also not)Positive), so return 1 (as the sNumber is a smaller negetive number then the sRange, making it greater)
					else {
						iReturn = 1;
					}
				}
					//#### Else if the iNumber('s)NumericPrecision is more then the iRange('s)NumericPrecision
				else if (iNumberNumericPrecision > iRangeNumericPrecision) {
						//#### If the bNumberIsPositive (and thanks to the check above the bRangeIs(also)Positive), return 1 (as the sNumber is a bigger positive number then the sRange, making it greater)
					if (bNumberIsPositive) {
						iReturn = 1;
					}
						//#### Else the bNumberIs(not)Positive (and thanks to the check above the bRangeIs(also not)Positive), so return -1 (as the sNumber is a bigger negetive number then the sRange, making it less)
					else {
						iReturn = -1;
					}
				}
					//#### Else the iNumber('s)NumericPrecision is equal to the iRange('s)NumericPrecision, so additional checking is required
				else {
						//#### Define and set the additionally required decimal point position variables
					var iNumberDecimalPoint = sNumber.indexOf(".");
					var iRangeDecimalPoint = sRange.indexOf(".");

						//#### If either/both of the decimal points were not found above, reset iNumberDecimalPoint/iRangeDecimalPoint to their respective .lengths (which logicially places the iRangeDecimalPoint at the end of the sCurrentRange, which is where it is located)
						//####    NOTE: Since this function only checks that the passed sNumber is within the passed range, the values "whole" -vs- "floating point" number distinction is ignored as for our purposes, it is unimportant.
					if (iNumberDecimalPoint == -1) {
						iNumberDecimalPoint = sNumber.length;
					}
					if (iRangeDecimalPoint == -1) {
						iRangeDecimalPoint = sRange.length;
					}

						//#### If the sNumber's decimal point is to the left of sRange's (making sNumber less then sRange), set our iReturn value to -1
					if (iNumberDecimalPoint < iRangeDecimalPoint) {
						iReturn = -1;
					}
						//#### Else if the sNumber's decimal point is to the right of sRange's (making sNumber greater then sRange), set our iReturn value to 1
					else if (iNumberDecimalPoint > iRangeDecimalPoint) {
						iReturn = 1;
					}
						//#### Else the sNumber's decimal point is in the same position as the sRange's decimal point
					else {
							//#### Define and init the additionally required vars
						var iCurrentNumberNumber;
						var iCurrentRangeNumber;
						var i;

							//#### Default our iReturn value to 0 (as only > and < are checked in the loop below, so if the loop finishes without changing the iReturn value then the sNumber and sRange are equal)
						iReturn = 0;

							//#### Setup the value for i based on if the bNumberIsPositive (setting it to 0 if it is, or 1 if it isn't)
							//####    NOTE: This is done to skip over the leading "-" sign in negetive numbers (yea it's ugly, but it works!)
							//####    NOTE: Since at this point we know that signs of sNumber and sRange match, we only need to check bNumberIsPositive's value
						i = (bNumberIsPositive) ? (0) : (1);

							//#### Traverse the sNumber/sRange strings from front to back (based on the above determined starting position)
							//####     NOTE: Since everything is is the same position and the same precision, we know that sNumber's .lenght is equal to sRange's
						for (i = i; i < sNumber.length; i++) {
								//#### As long as we're not looking at the decimal point
							if (i != iNumberDecimalPoint) {
									//#### Determine the iCurrentNumberNumber and iCurrentRangeNumber for this loop
								iCurrentNumberNumber = parseInt(SubStr(sNumber, i, 1));
								iCurrentRangeNumber = parseInt(SubStr(sRange, i, 1));

									//#### If the iCurrentNumberNumber is less then the iCurrentRangeNumber
								if (iCurrentNumberNumber < iCurrentRangeNumber) {
										//#### sNumber is less then sRange, so set our iReturn value to -1 and fall from the loop
									iReturn = -1;
									exit;
								}
									//#### Else if the iCurrentNumberNumber is greater then the iCurrentRangeNumber
								if (iCurrentNumberNumber > iCurrentRangeNumber) {
										//#### sNumber is greater then sRange, so set our iReturn value to 1 and fall from the loop
									iReturn = 1;
									exit;
								}
							}
						}
					}
				}
			}
		}

			//#### Return the above determined iReturn value to the caller
		return iReturn;
	};

	//############################################################
	//# Determines the numeric precision of the passed sValue (i.e. - counts how many numeric places there are within the number, not including leading 0's)
	//############################################################
	//# Last Updated: April 19, 2006
	this.NumericPrecision = function(sValue) {
		var iReturn = 0;
		var bStartCounting = false;
		var sCurrentChar, i;

			//#### Ensure the passed sValue is a string
		sValue = new String(sValue);

			//#### If the passed sValue .IsNotANumber, set the iReturn value to -1 (which indicates an error occured)
		if (! this.IsNumeric(sValue)) {
			iReturn = -1;
		}
			//#### Else the passed sValue is a number
		else {
				//#### Traverse the .length of the passed sValue
			for (i = 0; i < sValue.length; i++) {
					//#### Collect the sCurrentChar
				sCurrentChar = sValue.substr(i, 1)

					//#### If the sCurrentChar is a number
				if (! this.IsNumeric(sCurrentChar)) {
						//#### If we are supposed to bStartCounting, or if the sCurrentChar is not a 0
						//####    NOTE: This is done so we ignore leading 0's (trailing 0's are still counted)
					if (bStartCounting || sCurrentChar != '0') {
							//#### Inc iReturn and ensure bStartCounting is true
						iReturn++;
						bStartCounting = true;
					}
				}
			}
		}

			//#### Return the above incremented iReturn to the caller
		return iReturn;
	};


	//##########################################################################################
	//# Coordinate-related Functions
	//##########################################################################################
	//############################################################
	//# Gets/sets the absolute X coordinate for the referenced element.
	//############################################################
	//# Last Updated: April 21, 2006
	this.Left = function(oElement, iNewX) {
			//#### Ensure the passed oElement is a reference
		oElement = this.GetByID(oElement);

			//#### If the caller passed in a iNewX, pass the call off to the YUI's .setXY
		if (this.IsNumeric(iNewX)) {
			return YAHOO.util.Dom.setXY(oElement, [iNewX, null]);
		}
			//#### Else a iNewX was not passed in, so pass the call off to the YUI's .getXY to get the absolute X coordinate
		else {
			return YAHOO.util.Dom.getXY(oElement)[0];
		}
	};

	//############################################################
	//# Gets/sets the absolute Y coordinate for the referenced element.
	//############################################################
	//# Last Updated: April 21, 2006
	this.Top = function(oElement, iNewY) {
			//#### Ensure the passed oElement is a reference
		oElement = this.GetByID(oElement);

			//#### If the caller passed in a iNewY, pass the call off to the YUI's .setXY
		if (this.IsNumeric(iNewY)) {
			return YAHOO.util.Dom.setXY(oElement, [null, iNewY]);
		}
			//#### Else a iNewY was not passed in, so pass the call off to YUI's .getXY to get the absolute Y coordinate
		else {
			return YAHOO.util.Dom.getXY(oElement)[1];
		}
	};

	//############################################################
	//# Retrieves the X coordinate for the referenced event.
	//############################################################
	//# Last Updated: April 21, 2006
	this.EventX = function(oEvent) {
			//#### If the passed oEvent is not setup, reset it to the window.event
		if (! oEvent) { oEvent = window.event; }

			//#### Return the .pageX value from ._x's .Event object
		return YAHOO.util.Event.getPageX(oEvent);
	};

	//############################################################
	//# Retrieves the Y coordinate for the referenced event.
	//############################################################
	//# Last Updated: April 21, 2006
	this.EventY = function(oEvent) {
			//#### If the passed oEvent is not setup, reset it to the window.event
		if (! oEvent) { oEvent = window.event; }

			//#### Return the .pageY value from ._x's .Event object
		return YAHOO.util.Event.getPageY(oEvent);
	};

	//############################################################
	//# Retrieves the width of the referenced element.
	//############################################################
	//# Last Updated: March 17, 2006
	this.Width = function(oElement, iNewWidth) {
		return this._x.Width(oElement, iNewWidth);
	};

	//############################################################
	//# Retrieves the height of the referenced element.
	//############################################################
	//# Last Updated: March 17, 2006
	this.Height = function(oElement, iNewHeight) {
		return this._x.Height(oElement, iNewHeight);
	};

	//############################################################
	//# Retrieves the height of the client's window.
	//############################################################
	//# Last Updated: April 11, 2006
	this.WindowHeight = function() {
		return YAHOO.util.Dom.getClientHeight();
	};

	//############################################################
	//# Retrieves the width of the client's window.
	//############################################################
	//# Last Updated: April 11, 2006
	this.WindowWidth = function() {
		return YAHOO.util.Dom.getClientWidth();
	};

	//############################################################
	//# Retrieves the number of pixels scrolled left in the client's window.
	//############################################################
	//# Last Updated: March 17, 2006
	this.WindowScrollLeft = function() {
		return this._x.ScrollLeft(null, true);
	};

	//############################################################
	//# Retrieves the number of pixels scrolled from the top in the client's window.
	//############################################################
	//# Last Updated: March 17, 2006
	this.WindowScrollTop = function() {
		return this._x.ScrollTop(null, true);
	};

	//############################################################
	//# Clips the referenced element to the provided dimensions.
	//############################################################
	//# Last Updated: March 17, 2006
	this.Clip = function(oElement, iTop, iRight, iBottom, iLeft) {
		return this._x.Clip(oElement, iTop, iRight, iBottom, iLeft);
	};

	//############################################################
	//# Determines if the referenced elements overlap in the 2D plane.
	//#    NOTE: This function really only needs to run correctly under those browsers that have Z-Index issues with certian elements, so true cross-platform compatibility is not really required for this function
	//############################################################
	//# Last Updated: April 19, 2006
	this.Overlap = function(oElement1, oElement2) {
		var bReturn = false;
		var iX1, iX2, iA1, iA2, iY1, iY2, iB1, iB2;

			//#### Set the Y (vertical) coords
		iB1 = this.Top(oElement1);
		iB2 = iB1 + this.Height(oElement1);
		iY1 = this.Top(oElement2);
		iY2 = iY1 + this.Height(oElement2);

			//#### If the elements seem to be in the way verticially
		if ((iY1 <= iB1 && iY2 > iB1) || (iY1 >= iB1 && iY1 < iB2)) {
				//#### Set the X (horozontal) coords
			iA1 = this.Left(oElement1);
			iA2 = iA1 + this.Width(oElement1);
			iX1 = this.Left(oElement2);
			iX2 = iX1 + this.Width(oElement2);

				//#### If the passed elements also overlap horozontally, flip bReturn to true
			if ((iX1 <= iA1 && iX2 > iA1) || (iX1 >= iA1 && iX1 < iA2)) {
				bReturn = true;
			}
		}

			//#### Return the above determined bReturn to the caller
		return bReturn;
	};


	//##########################################################################################
	//# 'Private', Pseudo-'Static' Functions
	//# 
	//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
	//##########################################################################################
	//############################################################
	//# Maintains the .Cn object for the referenced element.
	//#    NOTE: This function really only needs to run correctly under those browsers that have Z-Index issues with certian elements, so true cross-platform compatibility is not really required for this function
	//############################################################
	//# Last Updated: April 26, 2006
	var UpdateOverlappingInfo = function(oElement, sOverlappingID, bAppend) {
		var rThis = Cn._.t;
		var i;

			//#### If the oElement doesn't have a .CnZ object setup and we're supposed to bAppend the passed sOverlappingID
		if (! oElement.CnZ && bAppend) {
				//#### Setup oElement's .CnZ object
			oElement.CnZ = {
				IDs : new Array(),
				InitiallyVisible : rThis.IsVisible(oElement)
			};
		}

			//#### If the oElement has its .CnZ object setup
		if (oElement.CnZ) {
				//#### If we're supposed to bAppend the passed sOverlappingID
			if (bAppend) {
					//#### Traverse the oElement's .CnZ.IDs, looking for a sOverlappingID entry (if any)
				for (i = 0; i < oElement.CnZ.IDs.length; i++) {
						//#### If a sOverlappingID entry was found, flip bAppend to false and fall out of the loop
					if (oElement.CnZ.IDs[i] == sOverlappingID) {
						bAppend = false;
						break;
					}
				}

					//#### If an existing sOverlappingID entry was not found above
				if (bAppend) {
						//#### .push the sOverlappingID into the current oElement
					oElement.CnZ.IDs.push(sOverlappingID);

						//#### If the oElement .Is(currently)Visible, .Hide it
					if (rThis.IsVisible(oElement)) {
						rThis.Hide(oElement);
					}
				}
			}
				//#### Else we're supposed to remove the passed sOverlappingID
			else {
					//#### Traverse oElement's .CnZ.IDs, looking for a sOverlappingID entry (if any)
				for (i = 0; i < oElement.CnZ.IDs.length; i++) {
						//#### If a sOverlappingID entry was found, .splice it from oElement and fall out of the loop
					if (oElement.CnZ.IDs[i] == sOverlappingID) {
						oElement.CnZ.IDs.splice(i, 1);
						break;
					}
				}

					//#### If oElement's .CnZ.IDs is empty
				if (oElement.CnZ.IDs.length == 0) {
						//#### If the oElement was .InitiallyVisible, .Show it now
					if (oElement.CnZ.InitiallyVisible) { 
						rThis.Show(oElement);
					}

						//#### Destroy oElement's .CnZ object
					oElement.CnZ = null;
				}
			}
		}
	};

} //# Cn.Tools


//########################################################################################################################
//# x class (X Lib v4.11, April 13, 2007)
//# 
//#     © 2001-2007 Michael Foster (Cross-Browser.com), Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL
//#     "Cn.Tools._x" namespace wrapper code and related X modifications by Nick Campbell
//########################################################################################################################
//# Last Code Review: n/a
Cn.Tools._x = new function() {

	this.About = function() {
		alert('"Cn.Tools._x" uses modified code from "X", a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL\n\n"X" code is Copyright 2001-2007 Michael Foster (Cross-Browser.com)');
	};


	// xSniffer, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	//# var xOp7Up,xOp6Dn,xIE4Up,xIE4,xIE5,xNN4,xUA=navigator.userAgent.toLowerCase();
	this.Op7Up = false;
	this.Op6Dn = false;
	this.IE4Up = false;
	this.IE4 = false;
	this.IE5 = false;
	this.NN4 = false;
	this.UA=navigator.userAgent.toLowerCase()

	if(window.opera){
	  var i=this.UA.indexOf('opera');
	  if(i!=-1){
		var v=parseInt(this.UA.charAt(i+6));
		xOp7Up=v>=7;
		xOp6Dn=v<7;
	  }
	}
	else if(navigator.vendor!='KDE' && document.all && this.UA.indexOf('msie')!=-1){
	  this.IE4Up=parseFloat(navigator.appVersion)>=4;
	  this.IE4=this.UA.indexOf('msie 4')!=-1;
	  this.IE5=this.UA.indexOf('msie 5')!=-1;
	}
	else if(document.layers){this.NN4=true;}
	this.Mac=this.UA.indexOf('mac')!=-1;

	// this.InnerHtml, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.InnerHtml = function(e,h)
	{
	  if(!(e=this.GetElementById(e)) || !this.Str(e.innerHTML)) return null;
	  var s = e.innerHTML;
	  if (this.Str(h)) {e.innerHTML = h;}
	  return s;
	};

	// this.Height, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

//getHeight: function(el) {
//    return YAHOO.util.Dom.getStyle(el, 'height');
//}

	this.Height = function(e,h)
	{
	  if(!(e=this.GetElementById(e))) return 0;
	  if (this.Num(h)) {
		if (h<0) h = 0;
		else h=Math.round(h);
	  }
	  else h=-1;
	  var css=this.Def(e.style);
	  if (e == document || e.tagName.toLowerCase() == 'html' || e.tagName.toLowerCase() == 'body') {
		h = xClientHeight();
	  }
	  else if(css && this.Def(e.offsetHeight) && this.Str(e.style.height)) {
		if(h>=0) {
		  var pt=0,pb=0,bt=0,bb=0;
		  if (document.compatMode=='CSS1Compat') {
			var gcs = this.GetComputedStyle;
			pt=gcs(e,'padding-top',1);
			if (pt !== null) {
			  pb=gcs(e,'padding-bottom',1);
			  bt=gcs(e,'border-top-width',1);
			  bb=gcs(e,'border-bottom-width',1);
			}
			// Should we try this as a last resort?
			// At this point getComputedStyle and currentStyle do not exist.
			else if(this.Def(e.offsetHeight,e.style.height)){
			  e.style.height=h+'px';
			  pt=e.offsetHeight-h;
			}
		  }
		  h-=(pt+pb+bt+bb);
		  if(isNaN(h)||h<0) return;
		  else e.style.height=h+'px';
		}
		h=e.offsetHeight;
	  }
	  else if(css && this.Def(e.style.pixelHeight)) {
		if(h>=0) e.style.pixelHeight=h;
		h=e.style.pixelHeight;
	  }
	  return h;
	};

	// this.Width, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.Width = function(e,w)
	{
	  if(!(e=this.GetElementById(e))) return 0;
	  if (this.Num(w)) {
		if (w<0) w = 0;
		else w=Math.round(w);
	  }
	  else w=-1;
	  var css=this.Def(e.style);
	  if (e == document || e.tagName.toLowerCase() == 'html' || e.tagName.toLowerCase() == 'body') {
		w = xClientWidth();
	  }
	  else if(css && this.Def(e.offsetWidth) && this.Str(e.style.width)) {
		if(w>=0) {
		  var pl=0,pr=0,bl=0,br=0;
		  if (document.compatMode=='CSS1Compat') {
			var gcs = this.GetComputedStyle;
			pl=gcs(e,'padding-left',1);
			if (pl !== null) {
			  pr=gcs(e,'padding-right',1);
			  bl=gcs(e,'border-left-width',1);
			  br=gcs(e,'border-right-width',1);
			}
			// Should we try this as a last resort?
			// At this point getComputedStyle and currentStyle do not exist.
			else if(this.Def(e.offsetWidth,e.style.width)){
			  e.style.width=w+'px';
			  pl=e.offsetWidth-w;
			}
		  }
		  w-=(pl+pr+bl+br);
		  if(isNaN(w)||w<0) return;
		  else e.style.width=w+'px';
		}
		w=e.offsetWidth;
	  }
	  else if(css && this.Def(e.style.pixelWidth)) {
		if(w>=0) e.style.pixelWidth=w;
		w=e.style.pixelWidth;
	  }
	  return w;
	};

	// this.ScrollLeft, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.ScrollLeft = function(e, bWin)
	{
	  var offset=0;
	  if (!this.Def(e) || bWin || e == document || e.tagName.toLowerCase() == 'html' || e.tagName.toLowerCase() == 'body') {
		var w = window;
		if (bWin && e) w = e;
		if(w.document.documentElement && w.document.documentElement.scrollLeft) offset=w.document.documentElement.scrollLeft;
		else if(w.document.body && this.Def(w.document.body.scrollLeft)) offset=w.document.body.scrollLeft;
	  }
	  else {
		e = this.GetElementById(e);
		if (e && this.Num(e.scrollLeft)) offset = e.scrollLeft;
	  }
	  return offset;
	};

	// this.ScrollTop, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.ScrollTop = function(e, bWin)
	{
	  var offset=0;
	  if (!this.Def(e) || bWin || e == document || e.tagName.toLowerCase() == 'html' || e.tagName.toLowerCase() == 'body') {
		var w = window;
		if (bWin && e) w = e;
		if(w.document.documentElement && w.document.documentElement.scrollTop) offset=w.document.documentElement.scrollTop;
		else if(w.document.body && this.Def(w.document.body.scrollTop)) offset=w.document.body.scrollTop;
	  }
	  else {
		e = this.GetElementById(e);
		if (e && this.Num(e.scrollTop)) offset = e.scrollTop;
	  }
	  return offset;
	};

	// this.Clip, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.Clip = function(e,t,r,b,l)
	{
	  if(!(e=this.GetElementById(e))) return;
	  if(e.style) {
		if (this.Num(l)) e.style.clip='rect('+t+'px '+r+'px '+b+'px '+l+'px)';
		else e.style.clip='rect(0 '+parseInt(e.style.width)+'px '+parseInt(e.style.height)+'px 0)';
	  }
	};


	// this.PageX, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.PageX = function(e)
	{
	  var x = 0;
	  e = this.GetElementById(e);
	  while (e) {
		if (this.Def(e.offsetLeft)) x += e.offsetLeft;
		e = this.Def(e.offsetParent) ? e.offsetParent : null;
	  }
	  return x;
	};

	// this.PageY, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.PageY = function(e)
	{
	  var y = 0;
	  e = this.GetElementById(e);
	  while (e) {
		if (this.Def(e.offsetTop)) y += e.offsetTop;
		e = this.Def(e.offsetParent) ? e.offsetParent : null;
	  }
	  return y;
	};

	// Was this.GetElementById, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.GetElementById = function(e)
	{
	  if(typeof(e)=='string') {
		if(document.getElementById) e=document.getElementById(e);
		else if(document.all) e=document.all[e];
		else e=null;
	  }
	  return e;
	};

	// xVisibility, Copyright 2003-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL
//! improvement pending
	this.Visibility = function(e, bShow)
	{
	  if(!(e=this.GetElementById(e))) return null;
	  if(e.style && this.Def(e.style.visibility)) {
		if (this.Def(bShow)) e.style.visibility = bShow ? 'visible' : 'hidden';
		return e.style.visibility;
	  }
	  return null;
	};

	// this.Def, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.Def = function()
	{
	  for(var i=0; i<arguments.length; ++i){if(typeof(arguments[i])=='undefined') return false;}
	  return true;
	};

	// this.Num, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.Num = function()
	{
	  for(var i=0; i<arguments.length; ++i){if(isNaN(arguments[i]) || typeof(arguments[i])!='number') return false;}
	  return true;
	};

	// this.Str, Copyright 2001-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.Str = function(s)
	{
	  for(var i=0; i<arguments.length; ++i){if(typeof(arguments[i])!='string') return false;}
	  return true;
	};

	// this.GetComputedStyle, Copyright 2002-2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.GetComputedStyle = function(e, p, i)
	{
	  if(!(e=this.GetElementById(e))) return null;
	  var s, v = 'undefined', dv = document.defaultView;
	  if(dv && dv.getComputedStyle){
		s = dv.getComputedStyle(e,'');
		if (s) v = s.getPropertyValue(p);
	  }
	  else if(e.currentStyle) {
		v = e.currentStyle[this.Camelize(p)];
	  }
	  else return null;
	  return i ? (parseInt(v) || 0) : v;
	}

	// xCamelize, Copyright 2007 Michael Foster (Cross-Browser.com)
	// Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL

	this.Camelize = function(cssPropStr)
	{
	  var i, c, a = cssPropStr.split('-');
	  var s = a[0];
	  for (i=1; i<a.length; ++i) {
		c = a[i].charAt(0);
		s += a[i].replace(c, c.toUpperCase());
	  }
	  return s;
	}

} //# Cn.Tools._x
