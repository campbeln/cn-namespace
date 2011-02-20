//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined') {
	alert("Cn.Renderer.Form.ComboBox: [DEVELOPER] 'Cn/Cn.js' must be included before referencing this code.");
}
else if (! Cn.Tools) {
	alert("Cn.Renderer.Form.ComboBox: [DEVELOPER] 'Cn/Tools.js' must be included before referencing this code.");
}
else if (! Cn.Settings) {
	alert("Cn.Renderer.Form.ComboBox: [DEVELOPER] 'Cn/Settings.js.*' must be included before referencing this code.");
}
//# </DebugCode>

	//#### Else if our namespace is not setup
else if (! Cn.Renderer || ! Cn.Renderer.Form) {
	Cn.namespace("Cn.Renderer.Form");
}


//########################################################################################################################
//# ComboBox class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Tools.js, Cn/Settings.js.*
//########################################################################################################################
//# Last Code Review: March 16, 2006
Cn.Renderer.Form.ComboBox = new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.cbo = this;

		//#### Declare the required 'private' variables
	var ga_oInputs = new Array();

		//#### Declare the required 'private' 'constants'
	var g_cARROWWIDTH = 16;


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Defines the referenced ComboBox.
	//############################################################
	//# Last Updated: April 13, 2007
	this.Define = function(sInputID) {
		var rTools = Cn._.t;
		var sDIVName = Cn._.s.FormElementPrefix + sInputID + "_DIV";
		var oSelect = rTools.GetByID(Cn._.s.FormElementPrefix + sInputID + "_Select");
		var oInput = rTools.GetByID(sInputID);
		var oDIV = rTools.GetByID(sDIVName);
		var iX, iY;

			//#### If the passed sInputID isn't within the our ga_oInputs, add it in now
		if (ga_oInputs[sInputID] == null) {
			ga_oInputs[sInputID] = { key:sInputID, RenderTries:0 };
		}

			//#### If the oDIV has been rendered by the browser (as we know that the .Width of the oDIV will be greater then 0)
		if (rTools.Width(oDIV) > 0) {
				//#### Default the oSelect's .selectedIndex and copy a reference to its .options into the oInput
			oSelect.selectedIndex = -1;
			//oInput.options = oSelect.options;

				//#### If the requesting browser is Netscape Navigator v4.x, simply .Show the oDIV
			if (rTools.IsNN4()) {
				rTools.Show(oDIV);
			}
				//#### Else the requesting browser is not Netscape Navigator v4.x
			else {
					//#### Set the oSelect's .Width based on the oInput's .Width and the ._cArrowWidth
				rTools.Width(oSelect, (rTools.Width(oInput) + g_cARROWWIDTH));

					//#### .Clip the oDIV so that only the oSelect's dropdown arrow is visiable (-2 so that the dropdown arrow covers the right 2px border of the oInput)
				rTools.Clip(oDIV, 0, rTools.Width(oDIV), rTools.Height(oDIV), (rTools.Width(oDIV) - g_cARROWWIDTH - 2));

					//#### Determine the iX/iY corrds of the oSelect's dropdown arrow in relation to the oInput
					//####     NOTE: Since we know that the select will always be the last thing in the oDIV, we align the bottoms (hence the iY calculation)
				iX = ((rTools.Left(oInput) + rTools.Width(oInput)) - (rTools.Width(oDIV) - g_cARROWWIDTH));
				iY = (rTools.Top(oInput) - rTools.Height(oDIV) + rTools.Height(oInput));

					//#### Move the oDIV into position then .Show it
				rTools.Left(oDIV, iX);
				rTools.Top(oDIV, iY);
				rTools.Show(oDIV);
//rTools.Style(oSelect, 'color', 'red');
//rTools.Style(oDIV, 'z-index', '0');
//rTools.Style(oSelect, 'z-index', '0');
			}
		}
			//#### Else the oDIV has not yet been rendered by the browser
		else {
				//#### If we haven't yet reached 3 full seconds (60 * 1/20 = 3) of trying to setup the ComboBox
			if (ga_oInputs[sInputID].RenderTries < 60) {
					//#### Inc the .RenderTries and set(the)Timeout to recall ourselves in 1/20th of a second
				ga_oInputs[sInputID].RenderTries++;
				setTimeout("Cn._.cbo.Define('" + sInputID + "');", 50);
			}
		}
	};

	//############################################################
	//# Updates the related ComboBox's value based on the user selected list value.
	//############################################################
	//# Last Updated: March 17, 2006
	this.OnChange = function(oSelect, sInputID) {
			//#### Update the .value of the sInputID, then reset the oSelect's .selectedIndex
		Cn._.t.GetByID(sInputID).value = oSelect.options[oSelect.selectedIndex].text;
		oSelect.selectedIndex = -1;
	};

} //# Cn.Renderer.Form.ComboBox
