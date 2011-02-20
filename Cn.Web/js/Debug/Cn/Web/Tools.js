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
	alert("Cn.Web.Tools: [DEVELOPER] 'Cn/Cn.js' and 'Cn/Cn.js.*' must be included before referencing this code.");
}
else if (! Cn._.dt) {
	alert("Cn.Web.Tools: [DEVELOPER] 'Cn/Data/Tools.js' must be included before referencing this code.");
}

	//#### If all of the required YAHOO objects do not exist, popup the related error message
else if (typeof(YAHOO) == 'undefined') {
	alert("Cn.Web.Tools: [DEVELOPER] 'yui/YAHOO.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Dom) {
	alert("Cn.Web.Tools: [DEVELOPER] 'yui/Dom.js' must be included before referencing this code.");
}
else if (! YAHOO.util || ! YAHOO.util.Event) {
	alert("Cn.Web.Tools: [DEVELOPER] 'yui/Event.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### If our base namespace is not setup, do so now
if (! Cn.Web) {
	Cn.Namespace("Cn.Web");
}


//########################################################################################################################
//# Web.Tools class
//# 
//#     Required Includes: Cn/Cn.js, [yui/Yahoo.js], yui/Dom.js, yui/Event.js (for .EventX/.EventY)
//########################################################################################################################
//# Last Code Review: September 7, 2007
Cn.Web.Tools = Cn._.wt || new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.wt = this;

		//#### Declare the required 'private' variables
	var g_oYUIEvent = YAHOO.util.Event,
		g_oYUIDom = YAHOO.util.Dom,
		g_oElementWorkspace = Cn._.wj.ElementWorkspace,
		g_oDataTools = Cn._.dt,
		g_oThis = this
	;


	//##########################################################################################
	//# 'Public' Web-related Functions
	//##########################################################################################
	//############################################################
	//# Determines if the currently running client is Netscape Navigator 4
	//############################################################
	//# Last Updated: March 23, 2006
	this.IsNN4 = function() {
		return (YAHOO.env.ua.gecko == 4);
	};

	//############################################################
	//# Determines if the currently running client is IE
	//############################################################
	//# Last Updated: March 23, 2006
	this.IsIE = function() {
		return (YAHOO.env.ua.ie > 0);
	};

	//############################################################
	//# Returns an object reference to the provided element ID.
	//# 
	//#     NOTE: The YUI equivlent within YAHOO.utils.Dom is not as universal as X Lib's implementation
	//############################################################
	//# Last Updated: February 22, 2010
	this.GetByID = function(sID) {
		var oReturn = g_oYUIDom.get(sID);

/*		if (! oReturn) {
			oReturn = document.getElementsByName(sID);
			if (oReturn) {
				oReturn = oReturn[0];
			}
		}
*/
		return oReturn;
	};

	//############################################################
	//# Determines the passed oElements ID or name
	//############################################################
	//# Last Updated: September 7, 2007
	this.GetIdentifier = function(oElement) {
		var sReturn = '';

			//#### Ensure the passed oElement is an element reference
		oElement = this.GetByID(oElement);

			//#### If a valid oElement was passed in
		if (oElement) {
				//#### If the oElement has a valid .id, reset our sReturn value to it
			if (oElement.id && oElement.id.length > 0) {
				sReturn = oElement.id;
			}
				//#### Else if the oElement has a valid .name, reset our sReturn value to it
			else if (oElement.name && oElement.name.length > 0) {
				sReturn = oElement.name;
			}
		}

			//#### Return the above determined sReturn value to the caller
		return sReturn;
	};

	//############################################################
	//# Retrieves an array of elements based on the provided tag names that are children under the parent element
	//############################################################
	//# Last Updated: January 14, 2010
	this.ChildrenOf = function(oElement, a_sTagNames) {
		var a_oReturn = new Array(),
			a_sDOMElements, oCurrentParent, i, j
		;

			//#### Ensure the passed oElement is an element reference
		oElement = this.GetByID(oElement);

			//#### If the passed oElement and a_sTagNames are valid
		if (oElement && a_sTagNames && a_sTagNames.length > 0) {
				//#### Traverse the a_sTagNames
			for (i = 0; i < a_sTagNames.length; i++) {
					//#### Collect the array of ELEMENTS from the DOM
				a_sDOMElements = document.getElementsByTagName(g_oDataTools.MakeString(a_sTagNames[i], '').toUpperCase());

					//#### Traverse the collected a_sDOMElements
				for (j = 0; j < a_sDOMElements.length; j++) {
						//#### Set the oCurrentParent for the loop-and-a-half below
					oCurrentParent = a_sDOMElements[j].offsetParent;

						//#### While we have a valid oCurrentParent reference to process
					while (oCurrentParent) {
							//#### If the oCurrentParent equals the oElement, .push the current a_sDOMElement into the a_oReturn value and reset oCurrentParent so we fall from the loop (as we have found oElement)
						if (oCurrentParent == oElement) {
							a_oReturn.push(a_sDOMElements[j]);
							oCurrentParent = null;
						}
							//#### Else reset oCurrentParent to it's own parent
						else {
							oCurrentParent = oCurrentParent.offsetParent;
						}
					}
				}
			}
		}

			//#### Return the above determined a_oReturn value to the caller
		return a_oReturn;
	};

	//############################################################
	//# Gets/sets the HTML code present within the referenced oElement.
	//############################################################
	//# Last Updated: December 17, 2009
	this.InnerHTML = function(oElement, sHTML) {
		var sReturn = "";

			//#### Ensure the passed oElement is an element reference
		oElement = this.GetByID(oElement);

			//#### If the oElement was successfully collected
		if (oElement) {
			if (g_oDataTools.IsDefined(sHTML)) {
				oElement.innerHTML = sHTML;
			}

				//#### Reset our sReturn value to the .innerHTML
			sReturn = oElement.innerHTML;
		}

			//#### Return the above determined sReturn value to the caller
		return sReturn;
	};

	//############################################################
	//# Determines the referenced sID's sAttribute.
	//# 
	//#      NOTE: This is based on logic present within xGetElementsByAttribute [Copyright 2002-2007 Michael Foster (Cross-Browser.com)] Part of X, a Cross-Browser Javascript Library, Distributed under the terms of the GNU LGPL
	//############################################################
	//# Last Updated: December 7, 2009
	this.GetAttribute = function(oElement, sAttribute) {
		var sReturn = '';

			//#### Ensure the passed oElement is a reference
		oElement = this.GetByID(oElement);

			//#### If the oElement is valid
		if (oElement) {
				//#### Attempt to set the sReturn value via the standard .getAttribute interface or via the asso. array interface
			sReturn = (oElement.getAttribute(sAttribute) || oElement[sAttribute]);

				//#### If the sReturn value is not a string
			if (typeof(sReturn) != 'string') {
					//#### If this was a 'style' sAttribute request and the sReturn value is an object
				if (sAttribute.toLowerCase() == 'style' && typeof(sReturn).toLowerCase() == 'object') {
						//#### If the .cssText property exists, reset our sReturn value to it, else to a null-string
						//####     NOTE: This whole function is required because stupid IE doesn't return a string for the .style
					sReturn = (sReturn.cssText || '');
				}
					//#### Else something other then a string was found above, so return a null-string to the caller
				else {
					sReturn = '';
				}
			}
		}

			//#### Return the above determiend sReturn value to the caller
		return sReturn;
	};


	//##########################################################################################
	//# 'Public' CSS Style-related Functions
	//##########################################################################################
	//############################################################
	//# Determines if the referenced element is visible.
	//############################################################
	//# Last Updated: December 3, 2009
	this.IsVisible = function(oElement) {
			//#### Return based on the 'visibility' and the 'display' of the oElement
		return (this.Style(oElement, 'visibility').toLowerCase() != 'hidden');
//!		return (this.Style(oElement, 'visibility').toLowerCase() != 'hidden' && this.Style(oElement, 'display').toLowerCase() != 'none');
	};

	//############################################################
	//# Shows the referenced element.
	//############################################################
	//# Last Updated: September 7, 2007
	this.Show = function(oElement) {
		this.Style(oElement, 'visibility', 'visible');
	};

	//############################################################
	//# Hides the referenced element.
	//############################################################
	//# Last Updated: September 7, 2007
	this.Hide = function(oElement) {
		this.Style(oElement, 'visibility', 'hidden');
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
	//# Gets/sets the referenced element's style, optionally setting it to a new value.
	//############################################################
	//# Last Updated: April 27, 2006
	this.Style = function(oElement, sStyle, sNewValue) {
			//#### Ensure the passed oElement is a reference
		oElement = this.GetByID(oElement);

			//#### If the oElement is valid
		if (oElement) {
				//#### If the caller passed in a sNewValue, .set(the)Style
			if (g_oDataTools.IsDefined(sNewValue)) {
				g_oYUIDom.setStyle(oElement, sStyle, sNewValue);
				return null;
			}
				//#### Else the caller must be requesting the current value of the sStyle, so .get(the)Style
			else {
				return g_oYUIDom.getStyle(oElement, sStyle);
			}
		}
	};

	//############################################################
	//# Clips the referenced element to the provided dimensions.
	//############################################################
	//# Last Updated: December 3, 2009
	this.Clip = function(oElement, iTop, iRight, iBottom, iLeft) {
        g_oYUIDom.setStyle(oElement, 'clip', 'rect(' + iTop + 'px ' + iRight + 'px ' + iBottom + 'px ' + iLeft + 'px)');
	};

	//############################################################
	//# Adds the provided CSS class name to the referenced ID
	//############################################################
	//# Last Updated: September 7, 2007
	this.AddClass = function(oElement, sClassName) {
			//#### Ensure the passed oElement is a reference
		oElement = this.GetByID(oElement);

			//#### If the oElement is valid
		if (oElement) {
				//#### If the sClassName is not already defined against the oElement, add it now
			if (! g_oYUIDom.hasClass(oElement, sClassName)) {
				g_oYUIDom.addClass(oElement, sClassName);
			}
		}
	};

	//############################################################
	//# Removes the provided CSS class name from the referenced ID
	//############################################################
	//# Last Updated: September 7, 2007
	this.RemoveClass = function(oElement, sClassName) {
			//#### Ensure the passed oElement is a reference
		oElement = this.GetByID(oElement);

			//#### If the oElement is valid, remove the sClassName
		if (oElement) {
			g_oYUIDom.removeClass(oElement, sClassName);
		}
	};
/*
this.ClassName = function(oObject) {
	var sReturn = undefined,
		a_sClassName
	;

		//#### 
	if (oObject && oObject.constructor && oObject.constructor.toString) {
			//#### 
		a_sClassName = oObject.constructor.toString().match(/function\s*(\w+)/);
//		sReturn = oObject.constructor.toString();

			//#### 
		if (a_sClassName && a_sClassName.length == 2) {
			sReturn = a_sClassName[1];
		}
	}

		//#### 
	return sReturn;
};
*/


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
		if (g_oDataTools.IsNumeric(iNewX)) {
			return g_oYUIDom.setXY(oElement, [iNewX, null]);
		}
			//#### Else a iNewX was not passed in, so pass the call off to the YUI's .getXY to get the absolute X coordinate (hence sub-0 to get X)
		else {
			return g_oYUIDom.getXY(oElement)[0];
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
		if (g_oDataTools.IsNumeric(iNewY)) {
			return g_oYUIDom.setXY(oElement, [null, iNewY]);
		}
			//#### Else a iNewY was not passed in, so pass the call off to YUI's .getXY to get the absolute Y coordinate (hence sub-1 to get Y)
		else {
			return g_oYUIDom.getXY(oElement)[1];
		}
	};

	//############################################################
	//# Gets/sets the width of the referenced element.
	//############################################################
	//# Last Updated: December 3, 2009
	this.Width = function(oElement, iNewWidth) {
			//#### Ensure the passed oElement is a reference
		oElement = this.GetByID(oElement);

	        //#### If a iNewWidth was passed, set it now
	    if (g_oDataTools.IsNumeric(iNewWidth)) {
	        g_oYUIDom.setStyle(oElement, 'width', iNewWidth+'px');
	    }

            //#### Collect the oElement's oRegion, then return the calculated .Width
        var oRegion = g_oYUIDom.getRegion(oElement);
        return (oRegion.right - oRegion.left);
	};

	//############################################################
	//# Gets/sets the height of the referenced element.
	//############################################################
	//# Last Updated: December 3, 2009
	this.Height = function(oElement, iNewHeight) {
			//#### Ensure the passed oElement is a reference
		oElement = this.GetByID(oElement);

	        //#### If a iNewHeight was passed, set it now
	    if (g_oDataTools.IsNumeric(iNewHeight)) {
	        g_oYUIDom.setStyle(oElement, 'height', iNewHeight+'px');
	    }

            //#### Collect the oElement's oRegion, then return the calculated .Height
        var oRegion = g_oYUIDom.getRegion(oElement);
        return (oRegion.bottom - oRegion.top);
	};

	//############################################################
	//# Retrieves the height of the client's window.
	//############################################################
	//# Last Updated: April 11, 2006
	this.WindowHeight = function() {
		return g_oYUIDom.getClientHeight();
	};

	//############################################################
	//# Retrieves the width of the client's window.
	//############################################################
	//# Last Updated: April 11, 2006
	this.WindowWidth = function() {
		return g_oYUIDom.getClientWidth();
	};

	//############################################################
	//# Retrieves the number of pixels scrolled left in the client's window.
	//############################################################
	//# Last Updated: March 17, 2006
	this.WindowScrollLeft = function() {
		return g_oYUIDom.getDocumentScrollLeft();
	};

	//############################################################
	//# Retrieves the number of pixels scrolled from the top in the client's window.
	//############################################################
	//# Last Updated: March 17, 2006
	this.WindowScrollTop = function() {
		return g_oYUIDom.getDocumentScrollTop();
	};

	//############################################################
	//# Retrieves the X coordinate for the referenced event.
	//############################################################
	//# Last Updated: April 21, 2006
	this.EventX = function(oEvent) {
			//#### If the passed oEvent is not setup, reset it to the window.event
		if (! oEvent) { oEvent = window.event; }

			//#### Return the .getPageX
		return g_oYUIEvent.getPageX(oEvent);
	};

	//############################################################
	//# Retrieves the Y coordinate for the referenced event.
	//############################################################
	//# Last Updated: April 21, 2006
	this.EventY = function(oEvent) {
			//#### If the passed oEvent is not setup, reset it to the window.event
		if (! oEvent) { oEvent = window.event; }

			//#### Return the .getPageY
		return g_oYUIEvent.getPageY(oEvent);
	};

	//############################################################
	//# Determines if the referenced elements overlap in the 2D plane.
	//# 
	//#    NOTE: This function really only needs to run correctly under those browsers that have Z-Index issues with certian elements, so true cross-platform compatibility is not really required for this function
	//############################################################
	//# Last Updated: September 7, 2007
	this.Overlap = function(oElement1, oElement2) {
		var bReturn = false,
			iX1, iX2, iA1, iA2, iY1, iY2, iB1, iB2
		;

			//#### Ensure the passed oElements are references
		oElement1 = this.GetByID(oElement1);
		oElement2 = this.GetByID(oElement2);

			//#### If the passed oElements are valid
		if (oElement1 && oElement1) {
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
		}

			//#### Return the above determined bReturn to the caller
		return bReturn;
	};

	//############################################################
	//# Toggles the visibility of all of the referenced element types (excluding the exempted IDs) on the page that overlap the referenced element.
	//#    NOTE: This function really only needs to run correctly under those browsers that have Z-Index issues with certian elements, so true cross-platform compatibility is not really required for this function
	//############################################################
	//# Last Updated: September 7, 2007
	this.ToggleOverlappingElements = function(oElement, a_sTagNames, a_oExemptElements) {
		var a_sDOMElements, oCurrentElement, sElementID, bIsExempt, i, j, k;

			//#### Ensure the passed oElement is an element reference
		oElement = this.GetByID(oElement);

			//#### If the passed oElement and a_sTagNames are valid
		if (oElement && a_sTagNames && a_sTagNames.length > 0) {
				//#### If we have a valid a_oExemptElements array
			if (a_oExemptElements && a_oExemptElements.length > 0) {
					//#### Traverse the a_oExemptElements, ensuring each is a element reference
				for (i = 0; i < a_oExemptElements.length; i++) {
					a_oExemptElements[i] = this.GetByID(a_oExemptElements[i]);
				}

					//#### .push the oElement into it (just in case it's a troublesome z-index element)
				a_oExemptElements.push(oElement);
			}
				//#### Else the passed a_oExemptElements is not a valid array, so reset it to a single element array containing the oElement (just in case it's a troublesome z-index element)
			else {
				a_oExemptElements = new Array(oElement);
			}

				//#### Collect the sElementID
			sElementID = this.GetIdentifier(oElement);

				//#### Traverse the passed a_sTagNames
			for (i = 0; i < a_sTagNames.length; i++) {
					//#### Collect the array of ELEMENTS from the DOM
				a_sDOMElements = document.getElementsByTagName(a_sTagNames[i]);

					//#### Traverse the collected a_sDOMElements
				for (j = 0; j < a_sDOMElements.length; j++) {
						//#### Reset the oCurrentElement and bIsExempt for this loop
					oCurrentElement = a_sDOMElements[j];
					bIsExempt = false;

						//#### Traverse a_oExemptElements
					for (k = 0; k < a_oExemptElements.length; k++) {
							//#### If the oCurrentElement equals the current a_oExemptElement
						if (oCurrentElement == a_oExemptElements[k]) {
								//#### Flip bIsExempt and fall out of the loop
							bIsExempt = true;
							break;
						}
					}

						//#### As long as the oCurrentElement bIs(not)Exempt
					if (! bIsExempt) {
							//#### If the oElement overlaps the oCurrentElement and the oElement .Is(currently)Visible
						if (this.Overlap(oElement, oCurrentElement) && this.IsVisible(oElement)) {
								//#### Call .UpdateOverlappingInfo for the oCurrentElement, signaling it to append the sElementID
							UpdateOverlappingInfo(oCurrentElement, sElementID, true);
						}
							//#### Else they do not .Overlap (or the oElement is not visible)
						else {
								//#### Call .UpdateOverlappingInfo for the oCurrentElement, signaling it to remove the sElementID
							UpdateOverlappingInfo(oCurrentElement, sElementID, false);
						}
					}
				}
			}
		}
	};


	//##########################################################################################
	//# 'Private', Pseudo-'Static' Functions
	//# 
	//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
	//##########################################################################################
	//############################################################
	//# Maintains the .ElementWorkspace.Z object for the referenced element.
	//#    NOTE: This function really only needs to run correctly under those browsers that have Z-Index issues with certian elements, so true cross-platform compatibility is not really required for this function
	//############################################################
	//# Last Updated: March 2, 2010
	var UpdateOverlappingInfo = function(oElement, sOverlappingID, bAppend) {
		var a_sIDs, oElementZ, i;

			//#### Ensure the passed oElement has a .ElementWorkspace object setup
		g_oElementWorkspace[oElement.id] = g_oElementWorkspace[oElement.id] || {};
		oElementZ = g_oElementWorkspace[oElement.id];

			//#### If the oElement doesn't have a .Z object setup and we're supposed to bAppend the passed sOverlappingID
		if (! oElementZ.Z && bAppend) {
				//#### Setup oElement's .Z object
			oElementZ.Z = {
				IDs : new Array(),
				InitiallyVisible : g_oThis.IsVisible(oElement)
			};
		}

			//#### If the oElement has its .Z object setup
		if (oElementZ.Z) {
				//#### Setup the reference to the oElement's .Z.IDs
			a_sIDs = oElementZ.Z.IDs;

				//#### If we're supposed to bAppend the passed sOverlappingID
			if (bAppend) {
					//#### Traverse the oElement's a_sIDs, looking for a sOverlappingID entry (if any)
				for (i = 0; i < a_sIDs.length; i++) {
						//#### If a sOverlappingID entry was found, flip bAppend to false and fall out of the loop
					if (a_sIDs[i] == sOverlappingID) {
						bAppend = false;
						break;
					}
				}

					//#### If an existing sOverlappingID entry was not found above
				if (bAppend) {
						//#### .push the sOverlappingID into the current oElement
					oElementZ.Z.IDs.push(sOverlappingID);

						//#### If the oElement .Is(currently)Visible, .Hide it
					if (g_oThis.IsVisible(oElement)) {
						g_oThis.Hide(oElement);
					}
				}
			}
				//#### Else we're supposed to remove the passed sOverlappingID
			else {
					//#### Traverse oElement's .Z.IDs, looking for the sOverlappingID
				for (i = 0; i < a_sIDs.length; i++) {
						//#### If a sOverlappingID entry was found, .splice it from oElement and fall out of the loop
					if (a_sIDs[i] == sOverlappingID) {
						oElementZ.Z.IDs.splice(i, 1);
						break;
					}
				}

					//#### If oElement's .Z.IDs is empty
				if (a_sIDs.length == 0) {
						//#### If the oElement was .InitiallyVisible, .Show it now
					if (oElementZ.Z.InitiallyVisible) { 
						g_oThis.Show(oElement);
					}

						//#### Destroy oElement's .Z object
					oElementZ.Z = null;
				}
			}
		}
	};

} //# Cn.Web.Tools
