//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined') {
	alert("Cn.Renderer.Form.SpecialListBox: [DEVELOPER] 'Cn/Cn.js' must be included before referencing this code.");
}
//# </DebugCode>

	//#### Else if our namespace is not setup
else if (! Cn.Renderer || ! Cn.Renderer.Form) {
	Cn.namespace("Cn.Renderer.Form");
}


//########################################################################################################################
//# SpecialListBox class
//# 
//#     Required Includes: Cn/Cn.js
//########################################################################################################################
//# Last Code Review: February 24, 2006
Cn.Renderer.Form.SpecialListBox = new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.slb = this;


	//##########################################################################################
	//# SpecialListBox-related Subs/Functions
	//##########################################################################################
	//############################################################
	//# Adds the :hover CSS pseudo-class functionality to browsers that do not support it.
	//#     NOTE: This function is required by IE, as it has no :hover pseudo-class on labels
	//#     NOTE: The overall SpecialListBox functionality is based on the example (http://c82.net/article.php?ID=25) provided by Nicholas Rougeux (c82@c82.net).
	//#         License: Feel free to use the example on this page in your own works. If you choose to do so, drop me a line. I'd like to hear how you used it.
	//############################################################
	//# Last Updated: May 4, 2006
	this.Hover = function(oObject, bIsMouseOver) {
			//#### If the passed oObject has a .className to interrorgate
		if (oObject) {
			var bClassPresent = (oObject.className && oObject.className.length >= 5 && oObject.className.substr(oObject.className.length - 5) == 'hover');

				//#### If this bIs(a)MouseOver call and the bClass(is not)Present, set the .className to include the hover sub-class
			if (bIsMouseOver) {
				if (! bClassPresent) {
					oObject.className += ' hover';
				}
			}
				//#### Else this a onMouseOut call, so if the bClassPresent, remove it from the .className
			else if (bClassPresent) {
				oObject.className = oObject.className.substr(1, oObject.className.length - 6);
			}
		}
	};

} //# Cn.Renderer.Form.SpecialListBox
