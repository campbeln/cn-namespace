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
	alert("Cn.Data.Tools: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
//# </DebugCode>

	//#### If our base namespace is not setup, do so now
if (! Cn.Data) {
	Cn.Namespace("Cn.Data");
}


//########################################################################################################################
//# Data.Tools class
//# 
//#     Required Includes: Cn/Cn.js
//########################################################################################################################
//# Last Code Review: September 7, 2007
Cn.Data.Tools = Cn._.dt || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.dt = this;


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Pads the left side of the provided value with the referenced character.
	//############################################################
	//# Last Updated: September 7, 2007
	this.LPad = function(oText, sPadCharacter, iLength) {
		var sReturn = this.MakeString(oText, ''),
			i
		;

			//#### Traverse the need to pad .length of the sReturn value, prepending a sPadCharacter as we go
		for (i = sReturn.length; i < iLength; i++) {
			sReturn = sPadCharacter + sReturn;
		}

			//#### Return the above determined sReturn value to the caller
		return sReturn;
	};
//! this.RPad = function(oText, sPadCharacter, iLength) {};

	//############################################################
	//# Retrieves the character associated with the provided character code.
	//############################################################
	//# Last Updated: August 27, 2007
	this.Chr = function(iASCIICharacterCode) {
		return String.fromCharCode(iASCIICharacterCode);
	};

/*	//############################################################
	//# Retrieves the character code associated with the provided character.
	//############################################################
	//# Last Updated: August 27, 2007
	this.Asc = function(sCharacter) {
		var sValue = this.MakeString(sCharacter, '')
		var iReturn = -1;

			//#### If the passed sCharacter had a sValue, iReturn the .charAt for the 0th character
		if (sValue.length > 0) {
			iReturn = sValue.charAt(0);
		}

			//#### Return the above determined iReturn value to the caller
		return iReturn;
	};
*/
//! this.MultiValueString = function(sValues) {};
//! this.MultiValueString = function(a_sValues) {};
//! this.Normalize = function(sValue) {};
//! this.MD5 = function(sString) {};


	//##########################################################################################
	//# 'Public' Is* and Make* Data Coercion Functions
	//##########################################################################################
	//############################################################
	//# Determines if the provided value is defined.
	//#
	//#     NOTE: This is the JavaScript equivlent to C#'s "oValue != null" logic, therefore this is a JavaScript-only function.
	//############################################################
	//# Last Updated: December 3, 2009
	this.IsDefined = function(oValue) {
		return (typeof(oValue) != 'undefined');
	};

	//############################################################
	//# Determines if the provided value is a non-null string value.
	//############################################################
	//# Last Updated: August 27, 2007
	this.IsString = function(oValue) {
		var bReturn = false;

			//#### If the passed oValue .IsDefined
		if (this.IsDefined(oValue)) {
				//#### Safely re-cast the passed oValue as a string
				//####     NOTE: .MakeString cannot be used within .IsString as .MakeString uses .IsString (making it a infinite recursive loop)
			oValue = new String(oValue);

				//#### If the oValue is a null-string or contains only a null character, reset out bReturn value to false, otherwise to true
			bReturn = (oValue && oValue.length > 0 && oValue != this.Chr(0));
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

		//############################################################
		//# Converts the provided value into a string.
		//############################################################
		//# Last Updated: August 27, 2007
		this.MakeString = function(oValue, sDefaultValue) {
			var sReturn;

				//#### If the passed oValue .IsDefined
			if (this.IsDefined(oValue)) {
					//#### Safely cast the passed oValue as a string
				sReturn = new String(oValue);
			}

				//#### If the sReturn value .Is(not a)String, reset the sReturn value to the passed sDefaultValue
			if (! this.IsString(sReturn)) {
				sReturn = sDefaultValue;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

//this.IsNumber = function(oValue) {};
//	this.MakeNumber = function(oValue, sDefaultValue) {};

	//############################################################
	//# Determines if the passed oValue is a number.
	//############################################################
	//# Last Updated: May 1, 2006
	this.IsNumeric = function(oValue) {
			//#### Return the inverted value returned from isNaN
		return (! isNaN(oValue));
	};
//! this.MakeFloat = function(oValue, fDefaultValue) {};

		//############################################################
		//# Extracts any numeric data present within the provided value
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

//this.IsInteger = function(oValue) {};
//	this.MakeInteger = function(oValue, iDefaultValue) {};

	//############################################################
	//# Determines if the provided value can be converted into a boolean.
	//############################################################
	//# Last Updated: September 7, 2007
	this.IsBoolean = function(oValue) {
		var bReturn = false;

			//#### Ensure the passed oValue is a string
		oValue = this.MakeString(oValue, '');

			//#### If there is a oValue to process
		if (oValue.length > 0) {
				//#### If the oValue .IsNumeric, set our bReturn value to true (as numbers are boolean in nature)
			if (this.IsNumeric(oValue)) {
				bReturn = true;
			}
				//#### Else we need to process the oValue as a string
			else {
					//#### Determine the .toLowerCase'd oValue, resetting our bReturn value to true if it's a reconized boolean oValue
				switch (oValue.toLowerCase()) {
					case 'true':
					case 't':
					case 'yes':
					case 'y':
					case 'false':
					case 'f':
					case 'no':
					case 'n': {
						bReturn = true
						break;
					}
				}
			}
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	}

		//############################################################
		//# Converts the provided value into a boolean.
		//############################################################
		//# Last Updated: September 7, 2007
		this.MakeBoolean = function(oValue, bDefaultValue) {
			var bReturn = bDefaultValue;

				//#### Ensure the passed oValue is a string
			oValue = this.MakeString(oValue, '');

				//#### If the oValue is holding a value to test
			if (oValue.length > 0) {
					//#### If the oValue .IsNumeric, set our bReturn value based on the oValue (as numbers are boolean in nature)
				if (this.IsNumeric(oValue)) {
					bReturn = (this.MakeNumeric(oValue, 0) != 0);
				}
					//#### Else we need to process the oValue as a string
				else {
						//#### Determine the .toLowerCase'd oValue and process accordingly
					switch (oValue.toLowerCase()) {
							//#### If oValue is holding "true" or "yes", reset the bReturn value to true
						case "true":
						case "t":
						case "yes":
						case "y": {
							bReturn = true;
							break;
						}
							//#### If oValue is holding "false" or "no", reset the bReturn value to false
						case "false":
						case "f":
						case "no":
						case "n": {
							bReturn = false;
							break;
						}
					}
				}
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

/*		//############################################################
		//# Converts the provided value into a boolean integer.
		//############################################################
		//# Last Updated: August 27, 2007
		this.MakeBooleanInteger = function(oValue, bDefaultValue) {
			var iReturn;

				//#### If the passed off call to .MakeBoolean returns true, set our iReturn value to 1
			if (this.MakeBoolean(oValue, bDefaultValue)) {
				iReturn = 1;
			}
				//#### Else the .MakeBoolean call returned false, so set our iReturn value to 0
			else {
				iReturn = 0;
			}

				//#### Return the above determined iReturn value to the caller
			return iReturn;
		}
*/
	//############################################################
	//# Determines if the provided value can be converted into a date/time.
	//############################################################
	//# Last Updated: June 9, 2010
	this.IsDate = function(oValue) {
		var iEpoch,
			bReturn = (typeof(oValue) == 'object' && oValue.constructor == (new Date).constructor)
		;

			//#### If the oValue is not a Date object
		if (! bReturn) {
				//#### If Cn.Dates.Tools has been included and we are supposed to .AllowDDMMYYY dates (based on the user's Locale), reset the iEpoch to .DateParseEx's result
			if (Cn._.dst && Cn._.dst.AllowDDMMYYY()) {
				iEpoch = Cn._.dst.DateParseEx(oValue);
			}
				//#### Else collect the iEpoch via Date.parse
			else {
				iEpoch = Date.parse(oValue);
			}

				//#### Reset the return value to the initial iEpoch .parse
			bReturn = this.IsNumeric(iEpoch);

				//#### If the initial iEpoch .parse failed
			if (! bReturn) {
					//#### Retry the iEpoch collection by pre-pending a valid date, resetting the bReturn value based on it's success/failure
				iEpoch = Date.parse('2000/01/01 ' + oValue);
				bReturn = this.IsNumeric(iEpoch);
			}
		}
/*			//#### Determine the dDate defined within sValue
		dDate = new Date(sValue);

			//#### If the conversion failed
		if (! this.IsNumeric(dDate)) {
				//#### To ensure the passed sValue is not time-only, prepend Jan 1, 2000 and retest
			dDate = new Date('01-01-2000 ' + sValue);

				//#### If the secondary conversion failed, set eError to .cnIncorrectDataType
			if (! this.IsNumeric(dDate)) {
				eError = Cn._.wive.enumInputErrors.cnIncorrectDataType;
			}
		}
*/
			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

		//############################################################
		//# Converts the provided value into a date/time.
		//############################################################
		//# Last Updated: June 9, 2010
		this.MakeDate = function(oValue, dDefaultValue) {
			var dReturn = (this.IsDate(dDefaultValue) ? dDefaultValue : new Date()),
				iEpoch
			;

				//#### If the passed oValue is already a Date, reset our dReturn value accordingly
			if (typeof(oValue) == 'object' && oValue.constructor == (new Date).constructor) {
				dReturn = oValue;
			}
				//#### Else we have some coercing to do
			else {
					//#### If Cn.Dates.Tools has been included and we are supposed to .AllowDDMMYYY dates (based on the user's Locale), set the iEpoch to .DateParseEx's result
				if (Cn._.dst && Cn._.dst.AllowDDMMYYY()) {
					iEpoch = Cn._.dst.DateParseEx(oValue);
				}
					//#### Else collect the iEpoch via Date.parse
				else {
					iEpoch = Date.parse(oValue);
				}

					//#### If the iEpoch was successfully calculated from the passed oValue, reset the dReturn value to it
				if (this.IsNumeric(iEpoch)) {
					dReturn.setTime(iEpoch);
				}
					//#### Else the iEpoch was not successfully calculated from the passed oValue
				else {
						//#### Retry the iEpoch collection by pre-pending a valid date
					iEpoch = Date.parse('2000/01/01 ' + oValue);

						//#### If the iEpoch was successfully calculated from the modified oValue, reset the dReturn value to it
					if (this.IsNumeric(iEpoch)) {
						dReturn.setTime(iEpoch);
					}
				}
			}

				//#### Return the above determined dReturn value to the caller
			return dReturn;
		};

	//############################################################
	//# Determines if the provided value can be converted into a global unique identifier (GUID).
	//############################################################
	//# Last Updated: September 7, 2007
	this.IsGUID = function(oValue) {
		var bReturn = false;

			//#### Ensure the passed oValue is a string
		oValue = this.MakeString(oValue, '');

			//#### If there is a oValue to process
		if (oValue.length > 0) {
				//#### Reset the bReturn value based on if the oValue matchs the GUID RegExp pattern
			bReturn = (oValue.match(/^(\{)[0-9a-f]{8}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{12}(\})$/gi));
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	};

	//############################################################
	//# Determines if the passed oValue is within the passed range
	//#    NOTE: "return (sNumber >= sRangeMin && sNumber <= sRangeMax)" would work in 99.9% of the checks we'll do with this function, but in the case of huge/tiny numbers (such as NUMERIC(x,y)'s in Oracle), this wouldn't cut it as the numbers would be too large/small to be represented in any available numeric variables
	//############################################################
	//# Last Updated: January 13, 2010
	this.IsNumericInRange = function(oValue, sRangeMin, sRangeMax) {
		var bReturn = false;

			//#### If the passed oValue is greater then or equal to the passed sRangeMin
		if (this.LargeNumericComparison(oValue, sRangeMin) >= 0) {
				//#### If the passed oValue is less then or equal to the passed sRangeMax
			if (this.LargeNumericComparison(oValue, sRangeMax) <= 0) {
					//#### Since the passed oValue is within the passed sRangeMin/sRangeMax, flip our bReturn value to true
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
		//# Last Updated: January 13, 2010
		this.LargeNumericComparison = function(sNumber, sRange) {
				//#### Ensure the passed sNumber and sRange are strings
			sNumber = this.MakeString(sNumber, '');
			sRange = this.MakeString(sRange, '');

				//#### Define and init the required local vars
			var iNumberNumericPrecision = this.NumericPrecision(sNumber),
				iRangeNumericPrecision = this.NumericPrecision(sRange),
				iReturn,
				bNumberIsPositive = (sNumber.indexOf("-") != 0),
				bRangeIsPositive = (sRange.indexOf("-") != 0)
			;

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
					var fNumber = parseFloat(sNumber),
						fRange = parseFloat(sRange)
					;

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
						var iNumberDecimalPoint = sNumber.indexOf("."),
							iRangeDecimalPoint = sRange.indexOf(".")
						;

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
							var iCurrentNumberNumber, iCurrentRangeNumber, i;

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
	//# Last Updated: September 7, 2007
	this.NumericPrecision = function(oValue) {
		var iReturn = 0,
			bStartCounting = false,
			sCurrentChar, i
		;

			//#### Ensure the passed oValue is a string
		oValue = this.MakeString(oValue, '');

			//#### If the passed oValue .Is(not)Numeric, set the iReturn value to -1 (which indicates an error occured)
		if (! this.IsNumeric(oValue)) {
			iReturn = -1;
		}
			//#### Else the passed oValue is a number
		else {
				//#### Traverse the .length of the passed oValue
			for (i = 0; i < oValue.length; i++) {
					//#### Collect the sCurrentChar
				sCurrentChar = oValue.substr(i, 1)

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

} //# Cn.Data.Tools
