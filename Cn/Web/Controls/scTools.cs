/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections.Generic;
using System.IO;
using System.Text;									//# Required to access the StringBuilder class
using System.Reflection;
using System.Web.UI;								//# Required to access ControlCollection class
using System;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Cn.Data;
using Cn.Web.Inputs;
using Cn.Web.Renderer;


namespace Cn.Web.Controls {

	///########################################################################################################################
	/// <summary>
	/// Identifies a container that encompases the functionality of an InputCollectionControl.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>February 9, 2010</LastFullCodeReview>
	public interface IInputCollectionControl {
		///############################################################
		/// <summary>
		/// Gets/sets the data source that is provided to the items within the control.
		/// </summary>
		/// <value>Object that is provided to the items within the control.</value>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		object DataSource { get; set; }

		///############################################################
		/// <summary>
		/// Gets a collection of inputs associated with this instance.
		/// </summary>
		/// <value>List of Controls.Input objects that represent the collection of inputs associated with this instance.</value>
		///############################################################
		/// <LastUpdated>April 7, 2010</LastUpdated>
		List<Controls.Input> InputControls { get; }

		///############################################################
		/// <summary>
		/// Gets the base Input Collection class related to this instance.
		/// </summary>
		/// <value>InputCollection object that manages this instance.</value>
		///############################################################
		/// <LastUpdated>April 7, 2010</LastUpdated>
		Inputs.IInputCollection Inputs { get; }
//! would rather InputCollection, but this collides within ctrlInputCollection

		///############################################################
		/// <summary>
		/// Gets a value representing if this instance has been data bound.
		/// </summary>
		/// <value>Boolean value representing if this instance has been data bound.</value>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		bool IsDataBound { get; }

	} //# public interface IInputCollectionControl


	///########################################################################################################################
	/// <summary>
	/// WebControl Item Template container.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>February 11, 2010</LastFullCodeReview>
	public class TemplateContainer : Control, IDataItemContainer {
			//#### Declare the required private variables
		private object g_oDataItem;
		private int g_iDataItemIndex;
		private int g_iDisplayIndex;
		private enumPageSections g_ePageSection;


		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oDataItem">Object that represents the value to use when data-binding operations are performed.</param>
		/// <param name="iDataItemIndex">Integer representing the index of the data item in the data source.</param>
		/// <param name="iDisplayIndex">Integer representing the position of the data item as displayed in a control.</param>
		/// <param name="ePageSection">Enumeration representing the referenced page section.</param>
		///############################################################
		/// <LastUpdated>February 11, 2010</LastUpdated>
		public TemplateContainer(object oDataItem, int iDataItemIndex, int iDisplayIndex, enumPageSections ePageSection) {
			g_oDataItem = oDataItem;
			g_iDataItemIndex = iDataItemIndex;
			g_iDisplayIndex = iDisplayIndex;
			g_ePageSection = ePageSection;
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="ePageSection">Enumeration representing the referenced page section.</param>
		///############################################################
		/// <LastUpdated>June 2, 2010</LastUpdated>
		public TemplateContainer(enumPageSections ePageSection) {
			g_oDataItem = null;
			g_iDataItemIndex = -1;
			g_iDisplayIndex = -1;
			g_ePageSection = ePageSection;
		}

		///############################################################
		/// <summary>
		/// Gets an object that is used in simplified data-binding operations.
		/// </summary>
		/// <value>An object that represents the value to use when data-binding operations are performed.</value>
		///############################################################
		/// <LastUpdated>June 2, 2010</LastUpdated>
		public object DataItem {
			get { return g_oDataItem; }
			set { g_oDataItem = value; }
		}

		///############################################################
		/// <summary>
		/// Gets the index of the data item bound to a control.
		/// </summary>
		/// <value>An Integer representing the index of the data item in the data source.</value>
		///############################################################
		/// <LastUpdated>June 2, 2010</LastUpdated>
		public int DataItemIndex {
			get { return g_iDataItemIndex; }
			set { g_iDataItemIndex = value; }
		}

		///############################################################
		/// <summary>
		/// Gets the position of the data item as displayed in a control.
		/// </summary>
		/// <value>An Integer representing the position of the data item as displayed in a control.</value>
		///############################################################
		/// <LastUpdated>June 2, 2010</LastUpdated>
		public int DisplayIndex {
			get { return g_iDisplayIndex; }
			set { DisplayIndex = value; }
		}

		///############################################################
		/// <summary>
		/// Gets the page section this template represents.
		/// </summary>
		/// <value>Enumeration representing the page section this template represents.</value>
		///############################################################
		/// <LastUpdated>March 19, 2010</LastUpdated>
		public enumPageSections PageSection {
			get { return g_ePageSection; }
		}


	} //# internal class TemplateContainer


	///########################################################################################################################
	/// <summary>
	/// Delegate signature defintion for the <c>GenerateHTML</c> events.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>February 10, 2010</LastFullCodeReview>
	public delegate string GenerateHTMLEventHandler(object sender, GenerateHTMLEventArgs oEventArgs);


	///########################################################################################################################
	/// <summary>
	/// Event arguments required by the <c>GenerateResults</c> event.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>February 10, 2010</LastFullCodeReview>
	public class GenerateHTMLEventArgs : EventArgs {
			//#### Declare the required private variables
		private string[] ga_sInitialValues;
		private string g_sInitialValue;
		private string g_sAttributes;
		private string g_sInputAlias;
		private enumInputTypes g_eInputType;


		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="eInputType">Enumeration representing the input type to render.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="a_sInitialValues">Array of strings where each element represents the initial values of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public GenerateHTMLEventArgs(string sInputAlias, enumInputTypes eInputType, string sInitialValue, string[] a_sInitialValues, string sAttributes) {
			ga_sInitialValues = a_sInitialValues;
			g_sInitialValue = sInitialValue;
			g_sAttributes = sAttributes;
			g_sInputAlias = sInputAlias;
			g_eInputType = eInputType;
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the initial values of the input.
		/// </summary>
		/// <value>Array of strings where each element represents the initial values of the input.</value>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public string[] InitialValues {
			get { return ga_sInitialValues; }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the initial value of the input.
		/// </summary>
		/// <value>String representing the initial value of the input.</value>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public string InitialValue {
			get { return g_sInitialValue; }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the additional HTML attributes to apply to the input.
		/// </summary>
		/// <value>String representing the additional HTML attributes to apply to the input.</value>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public string Attributes {
			get { return g_sAttributes; }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the HTML input's unique base name.
		/// </summary>
		/// <value>String representing the HTML input's unique base name.</value>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public string InputAlias {
			get { return g_sInputAlias; }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the input type to render.
		/// </summary>
		/// <value>Enumeration representing the input type to render.</value>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public enumInputTypes InputType {
			get { return g_eInputType; }
		}
	} //# public class GenerateHTMLEventArgs


	///########################################################################################################################
	/// <summary>
	/// Event arguments required by the <c>GenerateResults</c> event.
	/// </summary>
	///########################################################################################################################
	/// <LastUpdated>December 8, 2009</LastUpdated>
	public class GenerateResultsEventArgs : EventArgs {
			//#### Declare the required private variables
		private Pagination g_oResults;
		private bool g_bReorderExistingResults;


		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oResults">Pagination object representing entire result set's record IDs.</param>
		/// <param name="bReorderExistingResults">Boolean value indicating if the entire results set requires re-ordering.</param>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public GenerateResultsEventArgs(Pagination oResults, bool bReorderExistingResults) {
			g_oResults = oResults;
			g_bReorderExistingResults = bReorderExistingResults;
		}

		///############################################################
		/// <summary>
		/// Gets a value representing entire result set's record IDs.
		/// </summary>
		/// <value>Pagination object representing entire result set's record IDs.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public Pagination Results {
			get { return g_oResults; }
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating if the entire results set requires re-ordering.
		/// </summary>
		/// <value>Boolean value indicating if the entire results set requires re-ordering.</value>
		///############################################################
		/// <LastUpdated>December 1, 2009</LastUpdated>
		public bool ReorderExistingResults {
			get { return g_bReorderExistingResults; }
		}
	} //# public class GenerateResultsEventArgs


	///########################################################################################################################
	/// <summary>
	/// Event arguments required by the <c>GenerateResults</c> event.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>December 1, 2009</LastFullCodeReview>
	public class CollectPageResultsEventArgs : EventArgs {
			//#### Declare the required private variables
		private Pagination g_oPageResults;


		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oPageResults">Pagination object representing this page's relevant record IDs.</param>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public CollectPageResultsEventArgs(Pagination oPageResults) {
			g_oPageResults = oPageResults;
		}

		///############################################################
		/// <summary>
		/// Gets a value representing this page's relevant record IDs.
		/// </summary>
		/// <value>Pagination object representing this page's relevant record IDs.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public Pagination PageResults {
			get { return g_oPageResults; }
		}
	} //# public class CollectPageResultsEventArgs


	///########################################################################################################################
	/// <summary>
	/// Event arguments required by the <c>PrintLength</c> event.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>December 1, 2009</LastFullCodeReview>
	public class PrintLengthEventArgs : EventArgs {
			//#### Declare the required private variables
		private enumPageSections g_ePageSection;


		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="ePageSection">Enumeration representing the referenced page section.</param>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public PrintLengthEventArgs(enumPageSections ePageSection) {
			g_ePageSection = ePageSection;
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the referenced page section.
		/// </summary>
		/// <value>Enumeration representing the referenced page section.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public enumPageSections PageSection {
			get { return g_ePageSection; }
		}
	} //# public class PrintLengthEventArgs


	///########################################################################################################################
	/// <summary>
	/// Provides functionality to render ASP.NET server control output streams to a string.
	/// </summary>
	/// <remarks>
	/// NOTE: There is overlap between this class and GetAttributes. Some refactoring is rquired.
	/// </remarks>
	///########################################################################################################################
	/// <LastFullCodeReview>February 11, 2010</LastFullCodeReview>
	public class HTMLTextToString {
			//#### Declare the required private variables
		private StringBuilder g_oStringBuilder;
		private StringWriter g_oStringWriter;
		private HtmlTextWriter g_oHTMLTextWriter;


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 11, 2010</LastUpdated>
		public HTMLTextToString() {
				//#### Pass the call off to .Reset to setup the global vars
			Reset();
		}

		///############################################################
		/// <summary>
		/// Resets the class to it's initialized state.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 11, 2010</LastUpdated>
		public void Reset() {
				//#### Setup the global vars
			g_oStringBuilder = new StringBuilder();
			g_oStringWriter = new StringWriter(g_oStringBuilder);
			g_oHTMLTextWriter = new HtmlTextWriter(g_oStringWriter);
		}


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets an object rewired to render ASP.NET server control output streams to a string.
		/// </summary>
		/// <value>HtmlTextWriter object representing an object rewired to render ASP.NET server control output streams to a string.</value>
		///############################################################
		/// <LastUpdated>February 11, 2010</LastUpdated>
		public HtmlTextWriter HTMLTextWriter {
			get { return g_oHTMLTextWriter; }
		}


		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Returns a string representing the ASP.NET server control output stream.
		/// </summary>
		/// <returns>String representing the ASP.NET server control output stream.</returns>
		///############################################################
		/// <LastUpdated>February 11, 2010</LastUpdated>
		public override string ToString() {
			return g_oStringBuilder.ToString();
		}


	} //# public class HTMLTextToString


    ///########################################################################################################################
    /// <summary>
	/// General helper methods.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	public class Tools {

		private const string gc_sDEFAULTVALUEPROPERTIES = "Value,Text";


		//##########################################################################################
		//# Public Static Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Locates Controls of the requested type under the passed root control.
		/// </summary>
		/// <remarks>
		/// This implementation does not utilize recursion to accomplish the heirarchically based search.
		/// </remarks>
		/// <typeparam name="T">Class-based Type representing the type to locate.</typeparam>
		/// <param name="oRootControl">Control representing the starting point of the search.</param>
		/// <param name="bFindFirstOnly">Boolean value representing if the search should stop on the first located Type.</param>
		/// <returns>Array of T type where each index represents a located Control of type T.</returns>
		///############################################################
		/// <LastUpdated>February 11, 2010</LastUpdated>
		public static T[] FindControl<T>(Control oRootControl, bool bFindFirstOnly) where T : class {
			LinkedList<Control> oSearchList = new LinkedList<Control>(); 
			LinkedList<Control> oReturnList = new LinkedList<Control>(); 
			Control oCurrentControl = oRootControl;
			T[] a_oReturn = new T[0];
			int i;

				//#### While the oCurrentControl is non-null
			while (oCurrentControl != null) {
					//#### If this is the T type we are looking for, push it into the oReturnList (optionally falling from the loop if we are to bFindFirstOnly)
				if (oCurrentControl is T) {
					oReturnList.AddLast(oCurrentControl);
					if (bFindFirstOnly) { break; }
				}

					//#### If the oCurrentControl has child(ren) .Controls of it's own to search
				if (oCurrentControl.HasControls()) {
						//#### Traverse the oCurrentControl's child(ren) .Controls
					for (i = 0; i < oCurrentControl.Controls.Count; i++) {
							//#### If this is the T type we are looking for, push it into the oReturnList (optionally falling from the loop if we are to bFindFirstOnly)
						if (oCurrentControl.Controls[i] is T) {
							oReturnList.AddLast(oCurrentControl.Controls[i]);
							if (bFindFirstOnly) { break; }
						}

							//#### If the oCurrentControl has grandchild(ren) .Controls, push the current child .Controls it onto our oSearchList to search in a later loop
						if (oCurrentControl.Controls[i] != null && oCurrentControl.Controls[i].HasControls()) {
							oSearchList.AddLast(oCurrentControl.Controls[i]);
						}
					}
				}

					//#### Pop the .First.Value from the oSearchList into the (new) oCurrentControl in prep. for the next loop, then .Remove it from the oSearchList
				oCurrentControl = (oSearchList.First != null ? oSearchList.First.Value : null);
				oSearchList.Remove(oCurrentControl);
			}

				//#### If we were able to locate some Controls of T type above
			if (oReturnList.Count > 0) {
					//#### Redimension our a_oReturn value to the above collected oReturnList.Count
				a_oReturn = new T[oReturnList.Count];

					//#### Traverse the oReturnList, copying each into our a_oReturn value
					//####     NOTE: We should probably just return the oReturnList to the caller, but the rest of the namespace deals in vanilla arrays, so we do so here as well
				for (i = 0; i < oReturnList.Count; i++) {
					a_oReturn[i] = oReturnList.First.Value as T;
					oReturnList.RemoveFirst();
				}
			}

				//#### Return the above determined a_oReturn value to the caller
			return a_oReturn;
		}

		public static Control[] FindControl(Control oRootControl, string sID, bool bFindFirstOnly) {
			LinkedList<Control> oSearchList = new LinkedList<Control>(); 
			LinkedList<Control> oReturnList = new LinkedList<Control>(); 
			Control oCurrentControl = oRootControl;
			Control[] a_oReturn = new Control[0];
			int i;

				//#### While the oCurrentControl is non-null
			while (oCurrentControl != null) {
					//#### If this is the T type we are looking for, push it into the oReturnList (optionally falling from the loop if we are to bFindFirstOnly)
				if (oCurrentControl.ID == sID) {
					oReturnList.AddLast(oCurrentControl);
					if (bFindFirstOnly) { break; }
				}

					//#### If the oCurrentControl has child(ren) .Controls of it's own to search
				if (oCurrentControl.HasControls()) {
						//#### Traverse the oCurrentControl's child(ren) .Controls
					for (i = 0; i < oCurrentControl.Controls.Count; i++) {
							//#### If this is the T type we are looking for, push it into the oReturnList (optionally falling from the loop if we are to bFindFirstOnly)
						if (oCurrentControl.Controls[i].ID == sID) {
							oReturnList.AddLast(oCurrentControl.Controls[i]);
							if (bFindFirstOnly) { break; }
						}

							//#### If the oCurrentControl has grandchild(ren) .Controls, push the current child .Controls it onto our oSearchList to search in a later loop
						if (oCurrentControl.Controls[i] != null && oCurrentControl.Controls[i].HasControls()) {
							oSearchList.AddLast(oCurrentControl.Controls[i]);
						}
					}
				}

					//#### Pop the .First.Value from the oSearchList into the (new) oCurrentControl in prep. for the next loop, then .Remove it from the oSearchList
				oCurrentControl = (oSearchList.First != null ? oSearchList.First.Value : null);
				oSearchList.Remove(oCurrentControl);
			}

				//#### If we were able to locate some Controls of T type above
			if (oReturnList.Count > 0) {
					//#### Redimension our a_oReturn value to the above collected oReturnList.Count
				a_oReturn = new Control[oReturnList.Count];

					//#### Traverse the oReturnList, copying each into our a_oReturn value
					//####     NOTE: We should probably just return the oReturnList to the caller, but the rest of the namespace deals in vanilla arrays, so we do so here as well
				for (i = 0; i < oReturnList.Count; i++) {
					a_oReturn[i] = oReturnList.First.Value as Control;
					oReturnList.RemoveFirst();
				}
			}

				//#### Return the above determined a_oReturn value to the caller
			return a_oReturn;
		}



		public static IInputCollectionControl FindRelatedInputCollectionControl(Control oInputControl) {
			IInputCollectionControl oReturn = null;

				//#### If our (grand).Parent cannot be set into our oReturn value
				//####     NOTE: When .SetParentInputCollection sees a .TemplateContainer, it automaticially recurses to test the grand-.Parent, so there is no need to send in .Parent.Parent below (though that would be more correct)
			if (! SetParentInputCollection(oInputControl.Parent, ref oReturn)) {
				Controls.InputCollection[] a_oInputCollections;

					//#### Attempt to locate the first Controls.InputCollection on the oInputControl's .Page
				a_oInputCollections = Web.Controls.Tools.FindControl<Controls.InputCollection>(oInputControl.Page, true);

					//#### If an a_oInputCollections was located above, set our oReturn value to it
				if (a_oInputCollections != null && a_oInputCollections.Length > 0) {
					oReturn = a_oInputCollections[0];
				}
					//#### Else there were no Controls.InputCollection's in the oInputControl's .Page
				else {
					Controls.Form[] a_oForms;

						//#### Attempt to locate the first Controls.Form on the oInputControl's .Page
						//####     NOTE: Technicially, this should not be a required search as the Controls.Form should be the .Parent, and hence caught above
					a_oForms = Web.Controls.Tools.FindControl<Controls.Form>(oInputControl.Page, true);

						//#### If a a_oForms was located above, set g_oParentInputCollection to it
					if (a_oForms != null && a_oForms.Length > 0) {
						oReturn = a_oForms[0];
					}
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

			///############################################################
			/// <summary>
			/// Safely sets <c>g_oParentInputCollection</c> to a valid WebControl representing an InputCollection.
			/// </summary>
			/// <param name="oControl">IInputCollectionControl object representing the object to set into <c>oInputCollectionControl</c>.</param>
			/// <returns>Boolean value indicating if the passed <paramref name="oParentInputCollection"/> was a valid WebControl representing an InputCollection.</returns>
			///############################################################
			/// <LastUpdated>February 26, 2010</LastUpdated>
			private static bool SetParentInputCollection(Control oControl, ref IInputCollectionControl oInputCollectionControl) {
				Type oType;
				bool bReturn = false;

					//#### If the passed oControl is non-null, get it's oType
				if (oControl != null) {
					oType = oControl.GetType();

						//#### If the passed oControl is of a reconized oType, reset the oInputCollectionControl and flip or bReturn value to true
					if (oType == typeof(Controls.InputCollection) ||
						oType == typeof(Controls.Form)
					) {
						oInputCollectionControl = oControl as IInputCollectionControl;
						bReturn = true;
					}
						//#### Else if the passed oControl's .Parent is a .TemplateContainer, recurse to test it's grand-.Parent (as it's probably an .InputCollection/.Form)
					else if (oType == typeof(Controls.TemplateContainer)) {
						bReturn = SetParentInputCollection(oControl.Parent, ref oInputCollectionControl);
					}
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}

		public static Inputs.IInputCollection FindRelatedInputCollection(Control oInputControl) {
			Inputs.IInputCollection oReturn = null;
			Type oType;

				//#### If the passed oInputControl is valid
			if (oInputControl != null) {
				oType = oInputControl.GetType();

					//#### If the passed oInputControl is a Controls.InputCollection
				if (oType == typeof(Controls.InputCollection)) {
					oReturn = ((Controls.InputCollection)oInputControl).ControlManager;
				}
					//#### Else if the passed oInputControl is a Controls.Form
				else if (oType == typeof(Controls.Form)) {
					oReturn = ((Controls.Form)oInputControl).ControlManager.InputCollection;
				}
					//#### Else the passed oInputControl is unreconized, so attempt to .FindRelatedInputCollectionControl and recurse to determine it's .FindRelatedInputCollection
					//####     NOTE: Since .FindRelatedInputCollectionControl only returns a valid IInputCollectionControl or null, we cannot have an inifite recursive loop
				else {
					oReturn = FindRelatedInputCollection((Control)FindRelatedInputCollectionControl(oInputControl));
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}


		///############################################################
		/// <summary>
		/// Retrieves all of the input *HTML attributes from the referenced Control.
		/// </summary>
		/// <remarks>
		/// This function retrieves all of the non-identifying, non-input related *HTML attributes from the referenced Control. The following attributes are NOT included in the returned string:
		///     ID, Name, Type
		/// </remarks>
		/// <param name="oControl">WebControl object representing the *HTML Control you wish to retrieve the attributes from.</param>
		/// <returns>String representing the *HTML attributes from the referenced Control.</returns>
		///############################################################
		/// <LastUpdated>October 2, 2007</LastUpdated>
		public static string GetInputAttributes(WebControl oControl) {
			string[] a_sAttributesToExclude = {"type", "id", "name"};

				//#### Pass the call off to .GetAttributes along with the standard input-related a_sAttributesToExclude, returning it's result as our own
			return GetAttributes(oControl, a_sAttributesToExclude);
		}

		///############################################################
		/// <summary>
		/// Retrieves all of the *HTML attributes from the referenced Control.
		/// </summary>
		/// <param name="oControl">WebControl object representing the *HTML Control you wish to retrieve the attributes from.</param>
/// <param name="a_sAttributesToExclude"></param>
		/// <returns>String representing the *HTML attributes from the referenced Control.</returns>
		///############################################################
		/// <LastUpdated>October 2, 2007</LastUpdated>
		public static string GetAttributes(WebControl oControl, string[] a_sAttributesToExclude) {
			StringBuilder oStringBuilder = new StringBuilder();
			StringWriter oStringWriter;
			HtmlTextWriter oHtmlTextReader;
			Regex regexExcludedAttributes;
			Regex regexBeginTag = new Regex("^<[a-z]* (.*)(/)?>$", RegexOptions.IgnoreCase);
			string sReturn = "";
			int i;

				//#### Establish the dummy oHtmlTextReader with an underlying oStringWriter that uses our own oStringBuilder as its storage
			oStringWriter = new System.IO.StringWriter(oStringBuilder);
			oHtmlTextReader = new HtmlTextWriter(oStringWriter);

				//#### If the passed oControl is not null
			if (oControl != null) {
					//#### Collect the .Render('d)BeginTag into the dummy oHtmlTextReader then .Normalize it's underlying oStringBuilder, storing the results into our sReturn value
				oControl.RenderBeginTag(oHtmlTextReader);
				sReturn = Data.Tools.Normalize(oStringBuilder.ToString());

					//#### Peal out the regexBeginTag from our sReturn value, surrounding the result with spaces
				sReturn = " " + regexBeginTag.Split(sReturn)[1] + " ";

					//#### If the caller passed in a_sAttributesToExclude
				if (a_sAttributesToExclude != null) {
						//#### Traverse the a_sAttributesToExclude, removing each from our sReturn value as we go
					for (i = 0; i < a_sAttributesToExclude.Length; i++) {
						regexExcludedAttributes = new Regex("( " + a_sAttributesToExclude[i].Trim() + "=(['\"])?[^$2|^\\s]*($2)? )", RegexOptions.IgnoreCase);
						sReturn = regexExcludedAttributes.Replace(sReturn, " ", 1);
					}
				}
			}

				//#### Return the above determined sReturn value to the caller (.Trim'ing it as we go)
			return sReturn.Trim();
		}


		public static string GetAttributes(AttributeCollection oAttributes) {
			StringBuilder oReturn = new StringBuilder();
			string[] a_sKeys;
			string sCurrentAttribute;
			int iAttributeCount;
			int i;

				//#### Determine the iAttributeCount, then populate the a_sKeys
			iAttributeCount = oAttributes.Count;
			a_sKeys = new string[iAttributeCount];
			oAttributes.Keys.CopyTo(a_sKeys, 0);

				//#### Traverse the Attributes a_sKeys
			for (i = 0; i < iAttributeCount; i++) {
				sCurrentAttribute = oAttributes[a_sKeys[i]];

					//#### If the sCurrentAttribute contains a single-qoute ('), render the value surrounded by double-qoutes (")
				if (sCurrentAttribute.IndexOf("'") > 0) {
					oReturn.Append(a_sKeys[i] + "=\"" + sCurrentAttribute + "\" ");
				}
					//#### Else the sCurrentAttribute does not contain a single-qoute ('), so render the value surrounded by single-qoutes (')
				else {
					oReturn.Append(a_sKeys[i] + "='" + sCurrentAttribute + "' ");
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn.ToString();
		}



		public static bool GetValue(Control oControl, out string sValue) {
			string[] a_sValues;
			bool bReturn;

				//#### Pass the call off to our sister implementation, collecting it's bReturn value then .Join the a_sValues into our own sValue
			bReturn = GetValue(oControl, out a_sValues);
			sValue = string.Join("", a_sValues);

				//#### Return the above collected bReturn value to our caller
			return bReturn;
		}

		public static bool GetValue(Control oControl, out string[] a_sValues) {
			Type oType = oControl.GetType();
			bool bReturn = false;

				//#### Prep the a_sValues for a single value
			a_sValues = new string[1];
			a_sValues[0] = "";

				//#### Determine the oType.Name, process the passed oControl accordingly while resetting our bReturn value to true if we successfully set the sValue
			switch (oType.Name.ToLower()) {
				case "textbox": {
					a_sValues[0] = ((TextBox)oControl).Text;
					bReturn = true;
					break;
				}
				case "dropdownlist": {
					DropDownList oDropDown = (DropDownList)oControl;
					a_sValues[0] = oDropDown.Items[oDropDown.SelectedIndex].Value;
					bReturn = true;
					break;
				}
				case "listbox": {
					ListBox oList = (ListBox)oControl;

						//#### Pass the call off to GetValueFromListItemCollection to set the a_sValues byref and our bReturn value
					bReturn = GetValueFromListItemCollection(oList.Items, ref a_sValues);
					break;
				}
				case "checkbox": {
					a_sValues[0] = Data.Tools.MakeBoolean(((CheckBox)oControl).Checked, false).ToString();
					bReturn = true;
					break;
				}
				case "checkboxlist": {
					CheckBoxList oList = (CheckBoxList)oControl;

						//#### Pass the call off to GetValueFromListItemCollection to set the a_sValues byref and our bReturn value
					bReturn = GetValueFromListItemCollection(oList.Items, ref a_sValues);
					break;
				}
				case "radiobutton": {
					a_sValues[0] = Data.Tools.MakeBoolean(((RadioButton)oControl).Checked, false).ToString();
					bReturn = true;
					break;
				}
				case "radiobuttonlist": {
					RadioButtonList oList = (RadioButtonList)oControl;

						//#### Traverse the oList.Items
//! can't use foreach here?
					for (int i = 0; i < oList.Items.Count; i++) {
							//#### If the current .Items is .Selected, set our a_sValues, flip our bReturn value and fall out of the loop
						if (oList.Items[i].Selected) {
							a_sValues[0] = oList.Items[i].Value;
							bReturn = true;
							break;
						}
					}
					break;
				}

					//#### Else this wasn't an inbuilt oControl, so use Reflection to find a ca_sDEFAULTVALUEPROPERTIES
				default: {
					string[] a_sDefaultValueProperties = gc_sDEFAULTVALUEPROPERTIES.Split(',');

						//#### Traverse the a_sDefaultValueProperties
					for (int i = 0; i < a_sDefaultValueProperties.Length; i++) {
							//#### Attempt to collect the current oProperty
						PropertyInfo oProperty = oType.GetProperty(a_sDefaultValueProperties[i]);

							//#### If we successfully collected a oProperty above and we .CanWrite to it, .Set(the)Value and fall from the loop
						if (oProperty != null && oProperty.CanRead) {
							a_sValues[0] = Data.Tools.MakeString(oProperty.GetValue(oControl, null), "");
							bReturn = true;
							break;
						}
					}
					break;
				}
			}

				//#### Return the above determined bReturn value
			return bReturn;
		}

			private static bool GetValueFromListItemCollection(ListItemCollection oItems, ref string[] a_sValues) {
				string[] a_sTemp;
				int iCount = oItems.Count;
				int iCurrentIndex = 0;
				int i;
				bool bReturn = false;

					//#### Dimension a_sTemp to the max possible values
				a_sTemp = new string[iCount];

					//#### Traverse the oItems
				foreach (ListItem oItem in oItems) {
						//#### If the current oItem is .Selected, set it's .Value into a_sTemp and inc iCurrentIndex
					if (oItem.Selected) {
						a_sTemp[iCurrentIndex] = oItem.Value;
						iCurrentIndex++;
					}
				}

					//#### If oItem's were .Selected above
				if (iCurrentIndex > 0) {
						//#### Redimension the passed byref a_sValues to the above collected values and flip our bReturn value
					a_sValues = new string[iCurrentIndex];
					bReturn = true;

						//#### Copy the items from a_sTemp into a_sValues
					for (i = 0; i < iCurrentIndex; i++) {
						a_sValues[i] = a_sTemp[i];
					}
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			} 

		public static bool SetValue(Control oControl, string sValue) {
			string[] a_sValues = { sValue };

				//#### Pass the call off to our sister implementation, returning it's return value as our own
			return SetValue(oControl, a_sValues);
		}

		public static bool SetValue(Control oControl, string[] a_sValues) {
			Type oType = oControl.GetType();
			string sValue;
			int iValuesCount;
			int i;
			bool bReturn = false;

				//#### If the passed a_sValues are invalid, reset them to a single element with a null-string
			if (a_sValues == null || a_sValues.Length == 0) {
				a_sValues = new string[1];
				a_sValues[0] = "";
			}

				//#### Set the sValue and iValuesCount
			sValue = string.Join("", a_sValues);
			iValuesCount = a_sValues.Length;

				//#### Determine the oType.Name, process the passed oControl accordingly while resetting our bReturn value to true if we successfully set the sValue
			switch (oType.Name.ToLower()) {
				case "textbox": {
					((TextBox)oControl).Text = sValue;
					bReturn = true;
					break;
				}
				case "dropdownlist": {
					DropDownList oDropDown = (DropDownList)oControl;
					ListItem oItem = oDropDown.Items.FindByValue(sValue);

						//#### If an oItem was located for the sValue above, set the .SelectedIndex and flip bReturn to true
					if (oItem != null) {
						oDropDown.SelectedIndex = oDropDown.Items.IndexOf(oItem);
						bReturn = true;
					}
					break;
				}
				case "listbox": {
					ListBox oList = (ListBox)oControl;

						//#### Traverse the passed a_sValues
					for (i = 0; i < iValuesCount; i++) {
							//#### Traverse the oList.Items
						foreach (ListItem oItem in oList.Items) {
								//#### If the current oItem.Value matches our current a_sValues
							if (oItem.Value == a_sValues[i]) {
									//#### Flip oItem's .Selected and our bReturn value to true then fall from the inner loop
								oItem.Selected = true;
								bReturn = true;
								break;
							}
						}
					}
					break;
				}
				case "checkbox": {
					((CheckBox)oControl).Checked = Data.Tools.MakeBoolean(sValue, false);
					bReturn = true;
					break;
				}
				case "checkboxlist": {
					CheckBoxList oList = (CheckBoxList)oControl;

						//#### Traverse the passed a_sValues
					for (i = 0; i < iValuesCount; i++) {
							//#### Traverse the oList.Items
						foreach (ListItem oItem in oList.Items) {
								//#### If the current oItem.Value matches our current a_sValues
							if (oItem.Value == a_sValues[i]) {
									//#### Flip oItem's .Selected and our bReturn value to true then fall from the inner loop
								oItem.Selected = true;
								bReturn = true;
								break;
							}
						}
					}
					break;
				}
				case "radiobutton": {
					((RadioButton)oControl).Checked = Data.Tools.MakeBoolean(sValue, false);
					bReturn = true;
					break;
				}
				case "radiobuttonlist": {
					RadioButtonList oList = (RadioButtonList)oControl;

						//#### Traverse the oList.Items
//! is this not able to foreach?
					for (i = 0; i < oList.Items.Count; i++) {
							//#### If the current .Items.Value matches our sValue
						if (oList.Items[i].Value == sValue) {
								//#### Flip .Item's .Selected and our bReturn value to true then fall from the inner loop
							oList.Items[i].Selected = true;
							bReturn = true;
							break;
						}
					}
					break;
				}

					//#### Else oControl isn't an inbuilt Control, so use Reflection to find a gc_sDEFAULTVALUEPROPERTIES
				default: {
					string[] a_sDefaultValueProperties = gc_sDEFAULTVALUEPROPERTIES.Split(',');

						//#### Traverse the a_sDefaultValueProperties
					for (i = 0; i < a_sDefaultValueProperties.Length; i++) {
							//#### Attempt to collect the current oProperty
						PropertyInfo oProperty = oType.GetProperty(a_sDefaultValueProperties[i]);

							//#### If we successfully collected a oProperty above and we .CanWrite to it, .Set(the)Value and fall from the loop
						if (oProperty != null && oProperty.CanWrite) {
							oProperty.SetValue(oControl, sValue, null);
							bReturn = true;
							break;
						}
					}
					break;
				}
			}

				//#### Return the above determined bReturn value
			return bReturn;
		}



	} //# public class Tools


} //# namespace Cn.Web.Controls
