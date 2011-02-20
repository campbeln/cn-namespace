//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).


//########################################################################################################################
//# Cn NameSpace
//# 
//#     Required Includes: [none]
//########################################################################################################################
//# Last Code Review: February 21, 2006
var Cn = window.Cn || {};

	//############################################################
	//# Define the Abbreviation namespace
	//############################################################
	//# Last Updated: May 15, 2006
	Cn._ = {},

	//############################################################
	//# Returns the referenced namespace (creating it if it did not already exist)
	//#     Examples:
	//#         Cn.namespace("Renderer.Form");
	//#         Cn.namespace("Cn.Renderer.Form");
	//#     Either of the above calls returns the .Form namespace under Cn.Renderer.
	//#     In both cases, the Renderer followed by the Form namespaces are created if they did not previously exist.
	//# 
	//#     NOTE: This function is a full re-implementation of "YAHOO.namespace" from Yahoo!'s "YAHOO.js" (http://developer.yahoo.net/yui/).
	//############################################################
	//# Last Updated: May 15, 2006
	Cn.namespace = function(sNameSpace) {
		var oReturn = null;

			//#### If the passed sNameSpace is holding a value
		if (sNameSpace && sNameSpace.length > 0) {
				//#### Default the oReturn value to a reference to Cn
			oReturn = Cn;

				//#### .split the sNameSpace into its elements and determine the starting index of the loop
				//####     NOTE: "Cn" is implied, so it is ignored if it is present within the sNameSpace, hence the "i =" logic below
			var a_sNameSpaces = sNameSpace.split(".");
			var sNameSpace;
			var i = (a_sNameSpaces[0] == "Cn") ? 1 : 0;

				//#### Traverse the a_sNameSpaces, starting at the above determined i
			for (i = i; i < a_sNameSpaces.length; i++) {
					//#### Reset sNameSpace for this loop, then set/create it within Cn/the oReturn value
					//####     NOTE: The validity of the current sNameSpace is not checked as we assume that "you must be at least this smart to ride this ride"
				sNameSpace = a_sNameSpaces[i];
				oReturn[sNameSpace] = oReturn[sNameSpace] || {};
				oReturn = oReturn[sNameSpace];
			}
		}

			//#### Return the above determined oReturn value to the caller
		return oReturn;
	};
