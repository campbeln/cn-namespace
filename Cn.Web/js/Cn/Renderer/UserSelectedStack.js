//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined') {
	alert("Cn.Renderer.UserSelectedStack: [DEVELOPER] 'Cn/Cn.js' must be included before referencing this code.");
}
else if (! Cn.Settings) {
	alert("Cn.Renderer.UserSelectedStack: [DEVELOPER] 'Cn/Settings.js.*' must be included before referencing this code.");
}
//# </DebugCode>

	//#### Else the Cn namespace is present, so ensure our .namespace is setup
else {
	Cn.namespace("Cn.Renderer");
}


//########################################################################################################################
//# UserSelectedStack class
//# 
//#     Required Includes: Cn/Cn.js, Cn/Settings.js.*
//########################################################################################################################
//# Last Code Review: February 24, 2006
Cn.Renderer.UserSelectedStack = new function() {
		//#### Set a reference to ourselves in the Abbreviation namespace
	Cn._.uss = this;

		//#### Declare the required 'private' variables
	var ga_oInputs = new Array();
	var g_sPrimaryDelimiter = Cn._.s.PrimaryDelimiter;
	var g_sSecondaryDelimiter = Cn._.s.SecondaryDelimiter;
	var g_sBaseID = Cn._.s.FormElementPrefix + 'cnUSSCheckbox';
	var g_iCheckboxCount = 0;


	//##########################################################################################
	//# 'Public' Functions
	//##########################################################################################
	//############################################################
	//# Properly resets the User Selected Stack to a 'blank' stack
	//############################################################
	//# Last Updated: June 1, 2006
	this.Clear = function(sFormName) {
		var oInput = GetInput(sFormName);

			//#### If we were able to collect a valid oInput reference
		if (oInput) {
				//#### Reset the UserSelectedStack's value to a logicially null-stack
			oInput.value = g_sSecondaryDelimiter + '1' + g_sPrimaryDelimiter;

				//#### Call .UpdateCheckboxes to ensure any rendered checkboxes are properly reset based on the above revised UserSelectedStack
			UpdateCheckboxes(sFormName);
		}
	};

	//############################################################
	//# Manages the UserSelectedStack, properly adding/removing sIDs and table definitions as required
	//############################################################
	//# Last Updated: May 8, 2006
	this.Update = function(oCheckBox, sTableName, sIDColumn, sIDs, bInsertIDs) {
		var sFormName = oCheckBox.form.name;
		var oSection = GetSection(sFormName, sTableName, sIDColumn);
		var oInput = GetInput(sFormName);

			//#### If the oSection was .bFound
		if (oSection.bFound) {
			var a_sStack = oInput.value.split(g_sPrimaryDelimiter);
			var a_sIDs = sIDs.split(g_sSecondaryDelimiter);
			var sTableIDColumn = sTableName + g_sSecondaryDelimiter + sIDColumn;
			var i;

				//#### Pre/append .SecondaryDelimiters around oSection's .sIDs in prep. for the checks below
			oSection.sIDs = g_sSecondaryDelimiter + oSection.sIDs + g_sSecondaryDelimiter;

				//#### If we are supposed to bInsert(the passed)IDs into the UserSelectedStack
			if (bInsertIDs) {
				var sDelimitedID;

					//#### Traverse the passed a_sIDs
				for (i = 0; i < a_sIDs.length; i++) {
						//#### Set the sDelimitedID for this loop
					sDelimitedID = g_sSecondaryDelimiter + a_sIDs[i] + g_sSecondaryDelimiter;

						//#### If the sDelimitedID is not already within the oSection's .sIDs, append it followed by a .SecondaryDelimiter
					if (oSection.sIDs.indexOf(sDelimitedID) == -1) {
						oSection.sIDs = oSection.sIDs + a_sIDs[i] + g_sSecondaryDelimiter;
					}
				}

					//#### Reset the value of the a_sStack's related oSection.iIndex with the above-determined .sIDs (less the trailing .SecondaryDelimiter)
				a_sStack[oSection.iIndex] = sTableIDColumn + oSection.sIDs.substr(0, oSection.sIDs.length - g_sSecondaryDelimiter.length);

					//#### Reset the .value of the UserSelectedStack form element with the re-congealed a_sStack
				oInput.value = a_sStack.join(g_sPrimaryDelimiter);
			}
				//#### Else we need to remove the passed sIDs from the current stack element
			else {
				var oRegEx;

					//#### Traverse the passed a_sIDs
				for (i = 0; i < a_sIDs.length; i++) {
						//#### Remove any instances of the current a_sIDs[i] from oSection's .sIDs
					oRegEx = new RegExp(g_sSecondaryDelimiter + a_sIDs[i] + g_sSecondaryDelimiter, 'g');
					oSection.sIDs = oSection.sIDs.replace(oRegEx, g_sSecondaryDelimiter);
				}

					//#### If all of the .sIDs were removed above (which would leave only a single .SecondaryDelimiter behind)
				if (oSection.sIDs == g_sSecondaryDelimiter) {
						//#### Reset the value of the borrowed sTableName in prep. for the loop below
					sTableName = a_sStack[0];

						//#### Traverse the non-metadata sections of the a_sStack
					for (i = 1; i < a_sStack.length; i++) {
							//#### As long as this is not the blank oSection.iIndex, append the current a_sStack element onto the borrowed sTableName, preceeded by a .PrimaryDelimiter
						if (i != oSection.iIndex) {
							sTableName = sTableName + g_sPrimaryDelimiter + a_sStack[i];
						}
					}

						//#### If only the metadata is left in the borrowed sTableName
					if (sTableName == a_sStack[0]) {
							//#### Reset the .value of the UserSelectedStack form element with the properly delimited metadata
						oInput.value = sTableName + g_sPrimaryDelimiter;
					}
						//#### Else we still have ID data within the borrowed sTableName
					else {
							//#### Reset the .value of the UserSelectedStack form element with the re-congealed a_sStack in the borrowed sTableName
						oInput.value = sTableName;
					}
				}
					//#### Else there are still some .sIDs within the oSection
				else {
						//#### Reset the value of the a_sStack's related oSection.iIndex with the above-determined .sIDs (less the trailing .SecondaryDelimiter)
					a_sStack[oSection.iIndex] = sTableIDColumn + oSection.sIDs.substr(0, oSection.sIDs.length - g_sSecondaryDelimiter.length);

						//#### Reset the .value of the UserSelectedStack form element with the re-congealed a_sStack
					oInput.value = a_sStack.join(g_sPrimaryDelimiter);
				}
			}
		}
			//#### Else if we are supposed to bInsert(the passed)IDs into the UserSelectedStack
		else if (bInsertIDs) {
			var sCurrentUSS = oInput.value;

				//#### If the sCurrentUSS is a null-string, reset it's value to a valid metadata section
				//####     NOTE: Since we are going to have at least one Table/IDColumn/IDs entry, we need a valid metadata section
			if (! sCurrentUSS || sCurrentUSS == '') {
				sCurrentUSS = g_sSecondaryDelimiter + '1' + g_sPrimaryDelimiter;
			}

				//#### If there is not a trailing .PrimaryDelimiter on the sCurrentUSS's value (meaning that we are adding a subsequent sTableName/sIDColumn section)
			if (sCurrentUSS.substr(sCurrentUSS.length - g_sPrimaryDelimiter.length) != g_sPrimaryDelimiter) {
					//#### Append a .PrimaryDelimiter onto the sCurrentUSS's value
				sCurrentUSS = sCurrentUSS + g_sPrimaryDelimiter;
			}

				//#### Append the passed data onto the sCurrentUSS's value
			oInput.value = sCurrentUSS + sTableName + g_sSecondaryDelimiter + sIDColumn + g_sSecondaryDelimiter + sIDs;
		}

			//#### Call .UpdateCheckboxes to ensure any rendered checkboxes are properly reset based on the above revised UserSelectedStack
		UpdateCheckboxes(sFormName);
	};

	//############################################################
	//# Writes out a UserSelectedStack checkbox to the screen
	//#    NOTE: Since the UserSelectedStack functionality relies completly on JavaScript, its checkboxes are also printed out using javascript. So, if the user doesn't have JS enabled, they will never even see the checkboxes. Additionally, this JS determines if the checkbox is to be checked or not, else this checking code would be required within Renderer
	//############################################################
	//# Last Updated: May 8, 2006
	this.RenderCheckbox = function(sFormName, sTableName, sIDColumn, sID, sAttributes) {
		var sValue = sTableName + g_sSecondaryDelimiter + sIDColumn + g_sSecondaryDelimiter + sID;

			//#### If the passed data is already within the UserSpecifiedStack, append CHECKED='true' onto the passed sAttributes
		if (HasID(sFormName, sTableName, sIDColumn, sID)) {
			sAttributes = sAttributes + " CHECKED='true'";
		}

			//#### Inc the .CheckboxCount and write out the checkbox
		g_iCheckboxCount++;
		document.write("<input type='checkbox' name='" + g_sBaseID + "' value='" + sValue + "' onClick=\"Cn._.uss.Update(this, '" + sTableName + "', '" + sIDColumn + "', '" + sID + "', this.checked);\" " + sAttributes + " />");
	};

	//############################################################
	//# Gets the count of IDs stored within the single-table UserSelectedStack
	//############################################################
	//# Last Updated: June 1, 2006
	this.IDCount = function(sFormName) {
		var oInput = GetInput(sFormName);
		var iReturn = 0;
		var a_sStack, a_sSection, i;

			//#### If we were able to collect a valid oInput reference
		if (oInput && oInput.value) {
				//#### Collect the a_sStack
			a_sStack = oInput.value.split(g_sPrimaryDelimiter);

				//#### If we have non-metadata a_sSections to traverse
			if (a_sStack.length > 1) {
					//#### Traverse the non-metadata section of the a_sStack
				for (i = 1; i < a_sStack.length; i++) {
						//#### Collect the current a_sSection
					a_sSection = a_sStack[i].split(g_sSecondaryDelimiter);

						//#### If there are IDs in this section, add them onto the iReturn value (removing the TableName:IDColumn from the .length, hence -2)
					if (a_sSection.length > 2) {
						iReturn += (a_sSection.length - 2);
					}
				}
			}
		}

			//#### Return the above determined iReturn value to the caller
		return iReturn;
	};


	//##########################################################################################
	//# 'Private', Pseudo-'Static' Functions
	//# 
	//#     NOTE: These functions kinda act as if they are static, as they do not have access to "this" (at least in reference to the class).
	//##########################################################################################
	//############################################################
	//# Defines the form name as User Selected Stack-enabled
	//############################################################
	//# Last Updated: May 5, 2006
	var GetInput = function(sFormName) {
		var rThis = Cn._.uss;

			//#### If the passed sFormName is new
		if (! ga_oInputs[sFormName]) {
				//#### Insert a new sFormName entry within the ga_oInputs with a reference to the related UserSelectedStack
			ga_oInputs[sFormName] = (document.forms[sFormName] ? document.forms[sFormName].elements[Cn._.s.FormElementPrefix + 'cnUserSelectedStack'] : null);

				//#### If the UserSelectedStack form element is not currently holding a value, then .Clear it
			if (ga_oInputs[sFormName] != null && ga_oInputs[sFormName].value.length == 0) {
				rThis.Clear(sFormName);
			}
		}

			//#### Return the sFormName as stroed within the ga_oInputs to the caller
		return ga_oInputs[sFormName];
	};

	//############################################################
	//# Locates and returns the index and the delimited IDs section for the passed sFormName/sTableName/sIDColumn
	//############################################################
	//# Last Updated: June 1, 2006
	var GetSection = function(sFormName, sTableName, sIDColumn) {
		var oInput = GetInput(sFormName);
		var a_sStack;
		var sTableIDColumn = sTableName + g_sSecondaryDelimiter + sIDColumn + g_sSecondaryDelimiter;
		var sSectionIDs = '';
		var bFoundSection = false;
		var i;

			//#### If the oInput has a .value to .split
		if (oInput && oInput.value) {
				//#### .split the oInput's .value into the a_sStack
			a_sStack = oInput.value.split(g_sPrimaryDelimiter);

				//#### Traverse the ID data section of the a_sStack (skipping index 0 as it's metadata)
			for (i = 1; i < a_sStack.length; i++) {
					//#### If the sTableIDColumn is found in the correct position in the current a_sStack element
				if (a_sStack[i].indexOf(sTableIDColumn) == 0) {
						//#### Reset sSectionIDs with the IDs section for the current a_sStack element, flip bFoundSection and fall out of the loop
					sSectionIDs = a_sStack[i].substr(sTableIDColumn.length);
					bFoundSection = true;
					break;
				}
			}
		}

			//#### Return the above-determined i, sSectionIDs and bFoundSection to the caller
		return { iIndex:i, sIDs:sSectionIDs, bFound:bFoundSection };
	};

	//############################################################
	//# Determines if the passed sID exists within the referenced UserSelectedStack section
	//############################################################
	//# Last Updated: September 10, 2004
	var HasID = function(sFormName, sTableName, sIDColumn, sID) {
		var oSection = GetSection(sFormName, sTableName, sIDColumn);
		var bReturn = false;

			//#### If the referenced oSection was .bFound
		if (oSection.bFound) {
				//#### Modify the passed sID and oSection.sIDs by pre/appending .SecondaryDelimiters so that all of the IDs are surrounded by .SecondaryDelimiters for the search below
			sID = g_sSecondaryDelimiter + sID + g_sSecondaryDelimiter;
			oSection.sIDs = g_sSecondaryDelimiter + oSection.sIDs + g_sSecondaryDelimiter;

				//#### Reset the value of bReturn based on the presence of the passed sID within oSection's .sIDs
			bReturn = (oSection.sIDs.indexOf(sID) > -1);
		}

			//#### Return the above determined bReturn to the caller
		return bReturn;
	};

	//############################################################
	//# Updates any rendered checkboxes for the passed sFormName based on the current UserSelectedStack
	//############################################################
	//# Last Updated: May 5, 2006
	var UpdateCheckboxes = function(sFormName) {
		var i = 0;
		var a_sData, oCheckboxes, oCheckbox;

			//#### Collect the oCheckboxes
		oCheckboxes = document.forms[sFormName].elements[g_sBaseID];

			//#### If the oCheckboxes were successfully collected above
		if (oCheckboxes) {
				//#### Collect the oCheckbox based on if oCheckboxes represents a single or multiple oCheckboxes
			oCheckbox = (oCheckboxes.type ? oCheckboxes : oCheckboxes[i]);

				//#### While we still have a oCheckbox to process
			while (oCheckbox && oCheckbox.value) {
					//#### Reset the value of a_sData for this loop
				a_sData = oCheckbox.value.split(g_sSecondaryDelimiter);

					//#### If the above collected a_sData has the proper number of elements
				if (a_sData.length == 3) {
						//#### Reset the oCheckboxes' .checked value based on the ID's presence within the current UserSelectedStack
					oCheckbox.checked = HasID(sFormName, a_sData[0], a_sData[1], a_sData[2]);
				}

					//#### Inc i for the next loop and reset oCheckbox for the next loop
				i++;
				oCheckbox = (oCheckboxes[i] && oCheckboxes[i].value ? oCheckboxes[i] : null);
			}
		}
	};

} //# Cn.Renderer.UserSelectedStack
