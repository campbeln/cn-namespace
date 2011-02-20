/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/


using System.Web;                                   //# Required to access Request, Response, Application, etc.
using System.Text;									//# Required to access the StringBuilder class
using System.Collections;					        //# Required to access the Hashtable class
using System.Collections.Generic;
using Cn.Data;										//# Required to access the MetaData, Picklist class
using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Web.Inputs {

		//#### Declare the required public Enums
	#region eNums
		/// <summary>Renderer Form input types.</summary>
	public enum enumInputTypes : int {
			/// <summary>Default input for this data type.</summary>
		cnDefaultInput = 0,
			/// <summary>XHTML-based text input.</summary>
		cnText = 1,
			/// <summary>XHTML-based textarea input.</summary>
		cnTextarea = 2,
			/// <summary>XHTML-based password input.</summary>
		cnPassword = 3,
			/// <summary>DHTML-based calendar date picker input.</summary>
		cnDate = 4,
			/// <summary>DHTML-based time picker input.</summary>
		cnTime = 5,
			/// <summary>DHTML-based calendar date/time picker input.</summary>
		cnDateTime = 6,
			/// <summary>XHTML-based option group input.</summary>
		cnOption = 7,
			/// <summary>XHTML-based select input.</summary>
		cnSelect = 8,
			/// <summary>DHTML-based combobox input.</summary>
		cnComboBox = 9,
			/// <summary>XHTML-based file input.</summary>
		cnFile = 10,
			/// <summary>XHTML-based hidden input.</summary>
		cnHidden = 11,
			/// <summary>XHTML-based multi-select input.</summary>
		cnMultiSelect = 12,
			/// <summary>XHTML-based checkbox input.</summary>
		cnCheckbox = 13,
			/// <summary>XHTML-based checkboxes group input.</summary>
		cnCheckboxes = 14,
			/// <summary>XHTML-based hidden input with value outputted to the screen.</summary>
		cnReadOnly = 15,
			/// <summary>DHTML-based checked list box checkboxes group input.</summary>
		cnCheckedListBox = 16,
			/// <summary>DHTML-based option list box option group input.</summary>
		cnOptionListBox = 17,
			/// <summary>DHTML-based WYSIWYG HTML editor input.</summary>
		cnHTMLEditor = 18,
            /// <summary>Specifies that this input will be rendered by the developer and not by the input collection.</summary>
        cnCustom = 19
	}
		/// <summary>Form programming "hook" types.</summary>
	public enum enumUIHookTypes : int {
			/// <summary>HTML form "action" attribute value for Renderer Form processing.</summary>
		cnFormAction = 0,
			/// <summary>HTML button/submit "onclick" attribute value for Renderer Form client-side pre-processing.</summary>
		cnFormDoSubmit = 1,
			/// <summary>HTML form "onsubmit" attribute value for Renderer Form client-side pre-processing.</summary>
		cnFormOnSubmit = 2,
			/// <summary>HTML input "onfocus"/"onblur" atributes for the Renderer client-side error reporting.</summary>
		cnErrorMessagePopUp = 3,
			/// <summary>HTML input "onmouseover"/"onmouseout" atributes for the Renderer client-side error reporting.</summary>
		cnErrorMessageToolTip = 4
	}
		/// <summary>Column validation/insertion descriptions.</summary>
	public enum enumSaveTypes : int {
			/// <summary>Column is an ID (and is therefore implicitly required).</summary>
		cnID = 1,
			/// <summary>Column value is required.</summary>
		cnRequired = 2,

			/// <summary>Column is included within auto generated SQL statements if the user supplied value is non-blank. If the user supplied value is blank, the column is ignored.</summary>
		cnInsertIfPresent = 10,
			/// <summary>Column is included within auto generated SQL statements. If the user supplied value is blank, a null-string value is inserted into the column.</summary>
		cnInsertNullString = 11,
			/// <summary>Column is included within auto generated SQL statements. If the user supplied value is blank, null is inserted into the column.</summary>
		cnInsertNull = 12,

			/// <summary>Column will not be included within auto generated SQL statements.</summary>
		cnIgnore = 20
	}
	#endregion


	///########################################################################################################################
	/// <summary>
	/// Identifies a container that encompases the functionality of an InputCollection.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>February 9, 2010</LastFullCodeReview>
	public interface IInputCollection {
		void Add(string sInputAlias, string sTableName, string sColumnName, enumSaveTypes eSaveType, MetaData.enumValueTypes eValueType, AdditionalData oAdditionalData);
		void Add(string sInputAlias, enumSaveTypes eSaveType, MetaData.enumDataTypes eDataType, MetaData.enumValueTypes eValueType, string sDefaultValue, bool bIsNullable, int iMaximumCharacterLength, string sMinimumNumericValue, string sMaximumNumericValue, int iNumericPrecision, int iNumericScale, AdditionalData oAdditionalData);
		void Add(InputData oInputData);
		IInputBuilder HTMLBuilder { get; set; }
		string GenerateHTML(string sInputAlias, enumInputTypes eInputType, string sInitialValue, bool bForceInitialValue, string sAttributes);
		string GenerateHTML(string sInputAlias, enumInputTypes eInputType, string[] a_sInitialValues, bool bForceInitialValues, string sAttributes);

		Settings.Current Settings { get; }
		InputData Inputs(string sInputAlias);
		bool Exists(string sInputAlias);
		Picklists Picklists { get; set; }
		List<IInputCollection> ChildCollections { get; }
		bool Validate();
		bool DisplayErrorMessagesViaJavaScript { get; set; }
		bool IsPostBack { get; set; }
		string GenerateCSS();
		string ValidationJavaScript(string sCustomPreValidationScript, string sCustomRecordValidationScript, string sCustomPostValidationScript, bool bIncludeScriptBlock);
		int Count { get; }
		string[] InputAliases { get; }
		string InputName(string sInputAlias);

	} //# public interface IInputCollection


	///########################################################################################################################
	/// <summary>
	/// Represents the collection of defined <c>Form</c> Inputs.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>November 26, 2009</LastFullCodeReview>
	public abstract class InputCollectionBase : IInputCollection {
			//#### Declare the required private/protected variables
		private List<IInputCollection> gl_oChildCollections;
		private Picklists g_oPicklists;
		private Settings.Current g_oSettings;
		protected Hashtable gh_oInputs;
		private bool g_bDisplayErrorMessagesViaJavaScript;
		protected bool g_bIsPostBack;

			//#### Declare the required static private/protected variables
		private static IInputBuilder g_oHTMLBuilder;

			//#### Declare the required private constants
		private const string g_cClassName = "Cn.Web.Inputs.InputCollectionBase.";


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		///############################################################
		/// <LastUpdated>March 11, 2010</LastUpdated>
		protected void DoReset(Settings.Current oSettings) {
				//#### (Re)set the global variables
			gl_oChildCollections = new List<IInputCollection>();
			g_oPicklists = Web.Settings.Picklists;
			g_oSettings = oSettings;
			gh_oInputs = new Hashtable();
			g_bDisplayErrorMessagesViaJavaScript = true;
			g_bIsPostBack = false;
		}


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the settings for this instance.
		/// </summary>
		/// <value>Cn.Web.Settings.Current instance representing the current enviroment.</value>
		///############################################################
		/// <LastUpdated>March 3, 2010</LastUpdated>
		public Settings.Current Settings {
			get { return g_oSettings; }
			set { g_oSettings = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the developer defined Picklists class related to this instance.
		/// </summary>
		/// <value>Picklists object that represents the instance's developer defined Picklists class.</value>
		///############################################################
		/// <LastUpdated>June 7, 2005</LastUpdated>
		public Picklists Picklists {
			get { return g_oPicklists; }
			set { g_oPicklists = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the developer defined Picklists class related to this instance.
		/// </summary>
		/// <value>Picklists object that represents the instance's developer defined Picklists class.</value>
		///############################################################
		/// <LastUpdated>January 8, 2010</LastUpdated>
		public IInputBuilder HTMLBuilder {
			get {
					//#### If a g_oBuilder has not been defined, lazy load a new DefaultBuilder
				if (g_oHTMLBuilder == null) {
					g_oHTMLBuilder = new HTMLBuilder();
				}
				return g_oHTMLBuilder;
			}
			set { g_oHTMLBuilder = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating we are to display error messages via JavaScript.
		/// </summary>
		/// <remarks>
		/// In order to utilize this functionality, the developer must initilize the JavaScript error messages via <c>UIHook</c>.
		/// </remarks>
		/// <value>Boolean value indicating if we are to display error messages via JavaScript.</value>
		///############################################################
		/// <LastUpdated>January 8, 2010</LastUpdated>
		public bool DisplayErrorMessagesViaJavaScript {
			get { return g_bDisplayErrorMessagesViaJavaScript; }
			set { g_bDisplayErrorMessagesViaJavaScript = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if this is a form post back.
		/// </summary>
		/// <value>Boolean value indicating if this is a form post back.</value>
		///############################################################
		/// <LastUpdated>August 9, 2007</LastUpdated>
		public bool IsPostBack {
			get { return g_bIsPostBack; }
			set { g_bIsPostBack = value; }
		}


		//##########################################################################################
		//# Public Read-Only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the developer defined child InputCollection class related to this instance.
		/// </summary>
		/// <value>IInputCollection object that represents the instance's developer defined child InputCollection class.</value>
		///############################################################
		/// <LastUpdated>March 11, 2010</LastUpdated>
		public List<IInputCollection> ChildCollections {
			get { return gl_oChildCollections; }
		}

		///############################################################
		/// <summary>
		/// Retrieves the input data for the referenced input alias.
		/// </summary>
		/// <remarks>
		/// NOTE: Is is a simple alias for <c>Inputs(string sInputAlias)</c>.
		/// </remarks>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <returns>Object representing the InputData for the passed <paramref>sInputAlias</paramref>.</returns>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public InputData this[string sInputAlias] {
			get {
//! is this working correctly?
				return Inputs(sInputAlias);
			}
		}

		///############################################################
		/// <summary>
		/// Gets the count of inputs defined within this instance.
		/// </summary>
		/// <value>1-based integer representing the count of inputs defined within this instance.</value>
		///############################################################
		/// <LastUpdated>June 23, 2004</LastUpdated>
		public int Count {
			get { return gh_oInputs.Count; }
		}

		///############################################################
		/// <summary>
		/// Gets a deep copy of the inputs defined within this instance.
		/// </summary>
		/// <value>Deep copy of the InputData object array which defines the inputs of this instance.</value>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public InputData[] Data {
			get {
				InputData[] a_oReturn = null;
				string[] a_sInputAliases = InputAliases;
				int iLen;
				int i;

					//#### If there were a_sInputAliases defined for this instance
				if (a_sInputAliases != null && a_sInputAliases.Length > 0) {
						//#### Determine the .Length of the a_sInputAliases and dimension a_oReturn accordingly
					iLen = a_sInputAliases.Length;
					a_oReturn = new InputData[iLen];

						//#### Traverse the a_sInputAliases
					for (i = 0; i < iLen; i++) {
							//#### Deep copy the current gh_oInputs into a new InputData object within the a_oReturn value
						a_oReturn[i] = new InputData((InputData)gh_oInputs[a_sInputAliases[i]]);
					}
				}

					//#### Return the above determined a_oReturn value to the caller
				return a_oReturn;
			}
		}

		///############################################################
		/// <summary>
		/// Gets the collection of table names defined within this instance.
		/// </summary>
		/// <returns>String array where each element represents a table name defined within this instance.</returns>
		/// <exception cref="Cn.CnException">Thrown when there are no defined <c>Inputs</c> for this instance.</exception>
		///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public string[] TableNames {
			get {
				string[] a_sReturn = null;
				string[] a_sInputAliases = InputAliases;
				string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter;
				string sCurrentTable;
				string sFoundTables;
				int i;

					//#### If there were a_sInputAliases defined for this instance
				if (a_sInputAliases != null && a_sInputAliases.Length > 0) {
						//#### Init the sFoundTables to a leading sPrimaryDelimiter
					sFoundTables = sPrimaryDelimiter;

						//#### Traverse the gh_oInputs
					for (i = 0; i < a_sInputAliases.Length; i++) {
							//#### Collect the sCurrentTable (followed by a sPrimaryDelimiter)
							//####     NOTE: The .TableName is not lowercased here as some containers my consider case important
						sCurrentTable = ((InputData)gh_oInputs[a_sInputAliases[i]]).TableName + sPrimaryDelimiter;

							//#### If the sCurrentTable (delimited by surrounding sPrimaryDelimiters) is not within the sFoundTables
	//! maybe use Hashtable.keys to accomplish same? use List?
						if (sFoundTables.IndexOf(sPrimaryDelimiter + sCurrentTable) == -1) {
								//#### Append the sCurrentTable onto the sFoundTables
							sFoundTables += sCurrentTable;
						}
					}

						//#### If we actually sFound(some)Tables above, .Split them apart into the a_sReturn value (removing the leading and trailing sPrimaryDelimiters as we go)
					if (sFoundTables.Length > 1) {
						a_sReturn = sFoundTables.Substring(1, sFoundTables.Length - 2).Split(sPrimaryDelimiter.ToCharArray());
					}
				}

					//#### Return the above determined a_sReturn value to the caller
				return a_sReturn;
			}
		}

		///############################################################
		/// <summary>
		/// Gets the collection of input aliases defined within this instance.
		/// </summary>
		/// <returns>String array where each element represents an input alias defined within this instance.</returns>
		///############################################################
		/// <LastUpdated>March 17, 2010</LastUpdated>
		public string[] InputAliases {
			get {
				string[] a_sReturn;
				int iLen = gh_oInputs.Count;

					//#### If there are gh_oInputs to return
				if (iLen > 0) {
						//#### Resize the a_sReturn value to fit the .Keys, then copy them in the a_sReturn value
					a_sReturn = new string[iLen];
					gh_oInputs.Keys.CopyTo(a_sReturn, 0);
				}
					//#### 
				else {
					a_sReturn = new string[0];
				}

					//#### Return the above determined a_sReturn value to the caller
				return a_sReturn;
			}
		}


		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Adds an input into the collection that is assoicated with a Table/Column pair defined within the <c>MetaData</c>.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: <c>Input</c>s defined as <c>cnBoolean</c>s always return values based on <see cref="Cn.Data.Tools.MakeBooleanInteger">MakeBooleanInteger</see>, where zero equates to false and non-zero equates to true.
		/// <para/>NOTE: If you request that SQL statements be auto-generated in the return value for <see cref='Cn.Web.Renderer.Form.ValidateRecord'>ValidateRecord</see>, you are not permitted to define an input alias that contains the <see cref='Cn.Configuration.Settings.PrimaryDelimiter'>PrimaryDelimiter</see>.
		/// </remarks>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="sTableName">String representing the column's source table name.</param>
		/// <param name="sColumnName">String representing the column name.</param>
		/// <param name="eSaveType">Enumeration representing the HTML input's form processing requirements.</param>
/// <param name="eValueType"></param>
		/// <param name="oAdditionalData">AdditionalData representing the additionally definable properties of the input.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is null or a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is already defined.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> while also defind as not IsNullable.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sTableName</paramref>/<paramref>sColumnName</paramref> pair does not exist within the <c>MetaData</c>.</exception>
		/// <seealso cref="Cn.Web.Renderer.SearchForm.Add"/>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public void Add(string sInputAlias, string sTableName, string sColumnName, enumSaveTypes eSaveType, MetaData.enumValueTypes eValueType, AdditionalData oAdditionalData) {
			InputData oInputData;

				//#### If the passed sInputAlias is valid and unique
				//####    NOTE: .DoAdd_ValidateNewInputAlias raises errors if the sInputAlias checks fail
			if (DoAdd_ValidateNewInputAlias(sInputAlias)) {
					//#### Collect the oInputData based on the passed data
				oInputData = new InputData(sInputAlias, sTableName, sColumnName, eSaveType, eValueType, oAdditionalData);

					//#### If the oInputData returned above is valid (meaning no errors occured)
				if (oInputData != null) {
						//#### Set ourselves as it's .Parent, then set it into the gh_oInputs under the its .InputAlias
					oInputData.Parent = this;
					gh_oInputs[oInputData.InputAlias] = oInputData;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Adds an input into the collection.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: <c>Input</c>s defined as <c>cnBoolean</c>s always return values based on <see cref="Cn.Data.Tools.MakeBooleanInteger">MakeBooleanInteger</see>, where zero equates to false and non-zero equates to true.
		/// <para/>NOTE: If you request that SQL statements be auto-generated in the return value for <see cref='Cn.Web.Renderer.Form.ValidateRecord'>ValidateRecord</see>, you are not permitted to define an input alias that contains the <see cref='Cn.Configuration.Settings.PrimaryDelimiter'>PrimaryDelimiter</see>.
		/// </remarks>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="eSaveType">Enumeration representing the HTML input's form processing requirements.</param>
		/// <param name="eDataType">Enumerated value representing the column's datatype.</param>
		/// <param name="eValueType">Enumerated value representing the column's stored value type.</param>
		/// <param name="sDefaultValue">String representing the column's default value.</param>
		/// <param name="bIsNullable">Boolean value indicating if the column is permitted to hold a null value.</param>
		/// <param name="iMaximumCharacterLength">Integer representing the column's maximum character length.</param>
		/// <param name="sMinimumNumericValue">String representing the column's minimum numeric value.</param>
		/// <param name="sMaximumNumericValue">String representing the column's maximum numeric value.</param>
		/// <param name="iNumericPrecision">Integer representing the column's numeric precision.</param>
		/// <param name="iNumericScale">Integer representing the column's numeric scale.</param>
		/// <param name="oAdditionalData">AdditionalData representing the additionally definable properties of the input.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is null or a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is already defined.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> while also defind as not <paramref name="bIsNullable"/>.</exception>
		/// <seealso cref="Cn.Web.Renderer.SearchForm.Add"/>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public void Add(string sInputAlias, enumSaveTypes eSaveType, MetaData.enumDataTypes eDataType, MetaData.enumValueTypes eValueType, string sDefaultValue, bool bIsNullable, int iMaximumCharacterLength, string sMinimumNumericValue, string sMaximumNumericValue, int iNumericPrecision, int iNumericScale, AdditionalData oAdditionalData) {
			InputData oInputData;

				//#### If the passed sInputAlias is valid and unique
				//####    NOTE: .DoAdd_ValidateNewInputAlias raises errors if the sInputAlias checks fail
			if (DoAdd_ValidateNewInputAlias(sInputAlias)) {
					//#### Collect the oInputData based on the passed data
				oInputData = new InputData(sInputAlias, eSaveType, eDataType, eValueType, sDefaultValue, bIsNullable, iMaximumCharacterLength, sMinimumNumericValue, sMaximumNumericValue, iNumericPrecision, iNumericScale, oAdditionalData);

					//#### If the oInputData returned above is valid (meaning no errors occured)
				if (oInputData != null) {
						//#### Set ourselves as it's .Parent, then set it into the gh_oInputs under the its .InputAlias
					oInputData.Parent = this;
					gh_oInputs[oInputData.InputAlias] = oInputData;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Adds an input into the collection.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: <c>Input</c>s defined as <c>cnBoolean</c>s always return values based on <see cref="Cn.Data.Tools.MakeBooleanInteger">MakeBooleanInteger</see>, where zero equates to false and non-zero equates to true.
		/// <para/>NOTE: If you request that SQL statements be auto-generated in the return value for <see cref='Cn.Web.Renderer.Form.ValidateRecord'>ValidateRecord</see>, you are not permitted to define an input alias that contains the <see cref='Cn.Configuration.Settings.PrimaryDelimiter'>PrimaryDelimiter</see>.
		/// </remarks>
		/// <param name="oInputData">Object representing the InputData instance to deeply copy into this instance.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is null or a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is already defined.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> while also defind as not IsNullable.</exception>
		/// <seealso cref="Cn.Web.Renderer.SearchForm.Add"/>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public void Add(InputData oInputData) {
				//#### If the passed .InputAlias is valid and unique
				//####    NOTE: .DoAdd_ValidateNewInputAlias raises errors if the .InputAlias checks fail
			if (DoAdd_ValidateNewInputAlias(oInputData.InputAlias)) {
					//#### Recollect a deep copy the oInputData based on the passed data
//! do we need to do a deep copy?
				oInputData = new InputData(oInputData);

					//#### If the oInputData returned above is valid (meaning no errors occured)
				if (oInputData != null) {
						//#### Set ourselves as it's .Parent, then set it into the gh_oInputs under the its .InputAlias
					oInputData.Parent = this;
					gh_oInputs[oInputData.InputAlias] = oInputData;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Removes an input from the collection.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <returns>Boolean value representing if the input removal was a success.</returns>
		///############################################################
		/// <LastUpdated>August 8, 2007</LastUpdated>
		public bool Remove(string sInputAlias) {
			bool bReturn = Exists(sInputAlias);

				//#### If the passed sInputAlias .Exists, .Remove it from our gh_oInputs
			if (bReturn) {
				gh_oInputs.Remove(sInputAlias);
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Returns the requested input as represented by XHTML/DHTML code.
		/// </summary>
		/// <remarks>
		/// This implementation does not render <c>cnMultiValueFromPicklist</c> or <c>cnMultiValueSearchInSingleValuePicklist</c> inputs. Call the multi value implementation to render <c>cnMultiValueFromPicklist</c> or <c>cnMultiValueSearchInSingleValuePicklist</c> inputs.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>SaveType</c> set as <c>cnSingleValuePicklistExType</c> are:
		/// <para/>   <c>cnSelect</c>*, <c>cnComboBox</c>, <c>cnOption</c>, <c>cnOptionListBox</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>SaveType</c> set as <c>cnMultiValuePicklistExType</c> are:
		/// <para/>   <c>cnCheckboxes</c>*, <c>cnCheckedListBox</c>, <c>cnMultiSelect</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnBoolean</c> are:
		/// <para/>   <c>cnCheckbox</c>*, <c>cnOption</c>, <c>cnOptionListBox</c>, <c>cnSelect</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnInteger</c> are:
		/// <para/>   <c>cnText</c>*, <c>cnCheckbox</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnFloat</c> or <c>cnCurrency</c> are:
		/// <para/>   <c>cnText</c>*, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnDateTime</c> are:
		/// <para/>   <c>cnDate</c>*, <c>cnTime</c>, <c>cnDateTime</c>, <c>cnText</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnBinary</c> are:
		/// <para/>   <c>cnFile</c>*, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnGUID</c> are:
		/// <para/>   <c>cnText</c>*, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnUnknown</c> or <c>cnUnsupported</c> are:
		/// <para/>   None, an error is raised.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnChar</c> or <c>cnLongChar</c> are:
		/// <para/>   <c>cnText</c>*, <c>cnTextarea</c>, <c>cnHTMLEditor</c>, <c>cnPassword</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>* - <paramref>eInputType</paramref> utilised when <c>cnDefaultInput</c> is passed in.
		/// <para/>NOTE: Inputs with a <c>SaveType</c> set as <c>cnNonSearchableExType</c> or <c>cnVerbatumExType</c> are rendered based on their <c>DataType</c>.
		/// </remarks>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="eInputType">Enumeration representing the input type to render.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="bForceInitialValue">Boolean value representing if the value of the input is always to be set to <paramref name="sInitialValue"/>.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eInputType</paramref> is unreconized or unsupported for the current input's <c>DataType</c>/<c>SaveType</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eInputType</paramref> contains <c>cnSingleValuePicklistExType</c> and the global <c>Picklists</c> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eInputType</paramref> contains <c>cnSingleValuePicklistExType</c> and is defined as <c>Picklist_IsAdHoc</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eInputType</paramref> contains <c>cnMultiValuePicklistExType</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is of an unknown or unsupported data type.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> has not be defined.</exception>
		/// <exception cref="Cn.CnException">Thrown when the global <c>FormName</c> is a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the global <c>Picklists</c> does not contain the refrenced PicklistName.</exception>
		/// <returns>String representing the requested XHTML/DHTML input.</returns>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		public string GenerateHTML(string sInputAlias, enumInputTypes eInputType, string sInitialValue, bool bForceInitialValue, string sAttributes) {
			InputData oInputData = Inputs(sInputAlias);
			string sEndUserMessagesLanguageCode = Web.Settings.EndUserMessagesLanguageCode(g_oSettings);
			string sInputName = InputName(sInputAlias);
			string sReturn = "";
			MetaData.enumDataTypes eDataType;
			MetaData.enumValueTypes eValueType;

				//#### If the passed sInputAlias .Exists (as .Get returns null if the sInputAlias is unreconized)
			if (oInputData != null) {
					//#### Set the value of eDataType and eValueType
				eDataType = oInputData.DataType;
				eValueType = oInputData.ValueType;

					//#### If we're not supposed to bForceInitialValue and the .ValueIsFrom(the)Form or the passed sInitialValue is blank
				if (! bForceInitialValue && (oInputData.ValueIsFromForm || sInitialValue.Length == 0)) {
						//#### Reset sInitialValue to the .Value
					sInitialValue = oInputData.Value;
				}
					//#### Else we need to set the oInputData's .Value to the passed sInitialValue
					//####     NOTE: The sInitialValue is set into .Value here so the oInputData reflects these values
				else {
					oInputData.Value = sInitialValue;
				}

					//#### If this g_bIs(a)PostBack, oInputData .Is(not)Valid and the caller didn't provide a "class=" within sAttributes
				if (g_bIsPostBack && ! oInputData.IsValid && sAttributes.ToLower().IndexOf("class=") == -1) {
						//#### Append the .cnCSSClass_FormInputError class onto the tail of the passed sAttributes
					sAttributes += " class='" + Web.Settings.Value(Web.Settings.enumSettingValues.cnCSSClass_FormInputError) + "'";
				}

					//#### If this is a .cnSingleValueFromPicklist eValueType (or implicitly a .cnSingleValueSearchInMultiValuePicklistExType)
					//####     NOTE: "single-from-single" and "single-from-multi" picklists are processed here ("multi-from-multi" picklist are caught below)
				if (eValueType == MetaData.enumValueTypes.cnSingleValueFromPicklist) {
						//#### If this is not a Picklist_IsAdHoc (meaning it should be within the provided g_oPicklists)
					if (! oInputData.AdditionalData.Picklist_IsAdHoc) {
							//#### If the developer has setup the g_oPicklists
						if (g_oPicklists != null) {
								//#### Collect the oPicklist
							MultiArray oPicklist = g_oPicklists.Picklist(oInputData.AdditionalData.Picklist_Name);

								//#### If we successfully collected the oPicklist above
							if (oPicklist != null) {
									//#### Determine the passed eInputType and process accordingly
								switch (eInputType) {
									case enumInputTypes.cnDefaultInput:
									case enumInputTypes.cnSelect: {
										sReturn = HTMLBuilder.Select(sInputName, sAttributes, sInitialValue, oInputData.AdditionalData.Picklist_AddLeadingBlankOption, oPicklist, g_oSettings);
										break;
									}
									case enumInputTypes.cnComboBox: {
										sReturn = HTMLBuilder.ComboBox(sInputName, sAttributes, sInitialValue, oInputData.MaximumCharacterLength, oInputData.AdditionalData.ComboBox_SelectAttributes, oPicklist, g_oSettings);
										break;
									}
									case enumInputTypes.cnOption: {
										sReturn = HTMLBuilder.Option(sInputName, oInputData.AdditionalData.MultiValue_InputAttributes, sInitialValue, sAttributes, oPicklist, oInputData.AdditionalData.Data, g_oSettings);
										break;
									}
									case enumInputTypes.cnOptionListBox: {
										sReturn = HTMLBuilder.OptionListBox(sInputName, oInputData.AdditionalData.MultiValue_InputAttributes, sInitialValue, sAttributes, oPicklist, oInputData.AdditionalData.Data, g_oSettings);
										break;
									}
									case enumInputTypes.cnHidden: {
										sReturn = HTMLBuilder.Hidden(sInputName, sAttributes, sInitialValue, g_oSettings);
										break;
									}
									case enumInputTypes.cnReadOnly: {
										sReturn = HTMLBuilder.Hidden(sInputName, oInputData.AdditionalData.ReadOnly_InputAttributes, sInitialValue, Picklists.Decoder(oPicklist, sInitialValue, g_oPicklists.StrictDecodes), sAttributes, g_oSettings);
										break;
									}

									default: {
										Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Picklist, "", "");
										break;
									}
								}
							}
								//#### Else the sPicklistName does not .Exist, so raise the error
							else {
								Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnknownPicklistName, oInputData.AdditionalData.Picklist_Name, sInputAlias);
							}
						}
							//#### Else the developer has not setup the g_oPicklists, so raise the error
						else {
							Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_PicklistIsEmpty, "", "");
						}
					}
						//#### Else this was defined as an external picklist, so raise the error
					else {
						Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_Picklist_IsAdHoc, sInputAlias, "GenerateHTML");
					}
				}
					//#### Else if this is a .cnMultiValuePicklistExType g_iDataType, raise the error
				else if (eValueType == MetaData.enumValueTypes.cnMultiValuesFromPicklist) {
					Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_CallMultiRenderInput, "", "");
				}
					//#### Else if this is a .cnBoolean eDataType
				else if (eDataType == MetaData.enumDataTypes.cnBoolean) {
						//#### If the sInitialValue .IsBoolean, convert it into its string-ified .MakeBooleanInteger (else leave it alone for the control to deal with)
						//####     NOTE: We can .MakeBooleanInteger below with a default of "false" because we already know that the sInitialValue .IsBoolean (and therefore the default will not be used)
						//####     NOTE: We run this code even though it is run within .Checkbox because the value is pumped into other input builder functions
					if (Cn.Data.Tools.IsBoolean(sInitialValue)) {
						sInitialValue = Cn.Data.Tools.MakeBooleanInteger(sInitialValue, false).ToString();
					}

						//#### Determine the eInputType and process accordingly
					switch (eInputType) {
						case enumInputTypes.cnDefaultInput:
						case enumInputTypes.cnCheckbox: {
							sReturn = HTMLBuilder.CheckBox(sInputName, sAttributes, ref sInitialValue, g_oSettings);
							break;
						}
						case enumInputTypes.cnOption: {
								//#### Collect the .cnBoolean oPicklist from the .Settings, then write out the .cnOption
							MultiArray oPicklist = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnBoolean, sEndUserMessagesLanguageCode);
							sReturn = HTMLBuilder.Option(sInputName, oInputData.AdditionalData.MultiValue_InputAttributes, sInitialValue, sAttributes, oPicklist, oInputData.AdditionalData.Data, g_oSettings);
							break;
						}
						case enumInputTypes.cnOptionListBox: {
								//#### Collect the .cnBoolean oPicklist from the .Settings, then .Write(out the)OptionInput
							MultiArray oPicklist = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnBoolean, sEndUserMessagesLanguageCode);
							sReturn = HTMLBuilder.OptionListBox(sInputName, oInputData.AdditionalData.MultiValue_InputAttributes, sInitialValue, sAttributes, oPicklist, oInputData.AdditionalData.Data, g_oSettings);
							break;
						}
						case enumInputTypes.cnSelect: {
								//#### Collect the .cnBoolean oPicklist from the .Settings, then .Write(out the)SelectInput
							MultiArray oPicklist = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnBoolean, sEndUserMessagesLanguageCode);
							sReturn = HTMLBuilder.Select(sInputName, sAttributes, sInitialValue, false, oPicklist, g_oSettings);
							break;
						}
						case enumInputTypes.cnHidden: {
							sReturn = HTMLBuilder.Hidden(sInputName, sAttributes, sInitialValue, g_oSettings);
							break;
						}
						case enumInputTypes.cnReadOnly: {
								//#### Collect the .cnBoolean oPicklist from the .Settings, then .Write(out the)HiddenInput (.Decode'ing the sInitialValue from the oPicklist as we go)
							MultiArray oPicklist = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnBoolean, sEndUserMessagesLanguageCode);
							sReturn = HTMLBuilder.Hidden(sInputName, oInputData.AdditionalData.ReadOnly_InputAttributes, sInitialValue, Cn.Data.Picklists.Decoder(oPicklist, sInitialValue, false), sAttributes, g_oSettings);
							break;
						}

						default: {
							Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Boolean, "", "");
							break;
						}
					}
				}
					//#### Else if this is a .cnInteger eDataType
				else if (eDataType == MetaData.enumDataTypes.cnInteger) {
						//#### Determine the eInputType and process accordingly
					switch (eInputType) {
						case enumInputTypes.cnDefaultInput:
						case enumInputTypes.cnText: {
							sReturn = HTMLBuilder.TextBox(sInputName, sAttributes, sInitialValue, oInputData.NumericPrecision, g_oSettings);
							break;
						}
						case enumInputTypes.cnCheckbox: {
							sReturn = HTMLBuilder.CheckBox(sInputName, sAttributes, ref sInitialValue, g_oSettings);
							break;
						}
						case enumInputTypes.cnHidden: {
							sReturn = HTMLBuilder.Hidden(sInputName, sAttributes, sInitialValue, g_oSettings);
							break;
						}
						case enumInputTypes.cnReadOnly: {
							sReturn = HTMLBuilder.Hidden(sInputName, oInputData.AdditionalData.ReadOnly_InputAttributes, sInitialValue, sInitialValue, sAttributes, g_oSettings);
							break;
						}

						default: {
							Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Integer, "", "");
							break;
						}
					}
				}
					//#### Else if this is a .cnFloat eDataType
				else if (eDataType == MetaData.enumDataTypes.cnFloat) {
						//#### Determine the eInputType and process accordingly
					switch (eInputType) {
						case enumInputTypes.cnDefaultInput:
						case enumInputTypes.cnText: {
								//#### Add 2 onto .NumericPrecision to allow for a "." and a "-"
							sReturn = HTMLBuilder.TextBox(sInputName, sAttributes, sInitialValue, (oInputData.NumericPrecision + 2), g_oSettings);
							break;
						}
						case enumInputTypes.cnHidden: {
							sReturn = HTMLBuilder.Hidden(sInputName, sAttributes, sInitialValue, g_oSettings);
							break;
						}
						case enumInputTypes.cnReadOnly: {
							sReturn = HTMLBuilder.Hidden(sInputName, oInputData.AdditionalData.ReadOnly_InputAttributes, sInitialValue, sInitialValue, sAttributes, g_oSettings);
							break;
						}

						default: {
							Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Float, "", "");
							break;
						}
					}
				}
					//#### Else if this is a .cnCurrency eDataType
				else if (eDataType == MetaData.enumDataTypes.cnCurrency) {
						//#### If there is an sInitialValue to format
					if (sInitialValue.Length > 0) {
							//#### Format the sInitialValue as a currency
							//####     NOTE: 'Standard' Masks: "$ #,##0.00" "$ -#,##0.00" "$ 0.00"
							//####     NOTE: Masks as defined by MSDN @ http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpguide/html/cpconcustomnumericformatstrings.asp
						sInitialValue = Cn.Data.Tools.MakeDecimal(sInitialValue, 0).ToString(
							Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnLocalization_CurrencyMask_Positive, sEndUserMessagesLanguageCode) + ";" +
							Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnLocalization_CurrencyMask_Negetive, sEndUserMessagesLanguageCode) + ";" +
							Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnLocalization_CurrencyMask_Zero, sEndUserMessagesLanguageCode)
						);
					}

						//#### Determine the eInputType and process accordingly
					switch (eInputType) {
						case enumInputTypes.cnDefaultInput:
						case enumInputTypes.cnText: {
								//#### Add 3 onto .NumericPrecision to allow for a ".", "-" and a .cnCurrencySymbol
							sReturn = HTMLBuilder.TextBox(sInputName, sAttributes, sInitialValue, (oInputData.NumericPrecision + 3), g_oSettings);
							break;
						}
						case enumInputTypes.cnHidden: {
							sReturn = HTMLBuilder.Hidden(sInputName, sAttributes, sInitialValue, g_oSettings);
							break;
						}
						case enumInputTypes.cnReadOnly: {
							sReturn = HTMLBuilder.Hidden(sInputName, oInputData.AdditionalData.ReadOnly_InputAttributes, sInitialValue, sInitialValue, sAttributes, g_oSettings);
							break;
						}

						default: {
							Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Currency, "", "");
							break;
						}
					}
				}
					//#### Else if this is a .cnDateTime eDataType
				else if (eDataType == MetaData.enumDataTypes.cnDateTime) {
						//#### Define and collect the sInputSpecificFormat
					string sInputSpecificFormat = oInputData.AdditionalData.DateTime_Format;

//! how are .cnDate* inputs values submitted? Formatted or unformatted?
						//#### Determine the eInputType and process accordingly
					switch (eInputType) {
						case enumInputTypes.cnDate:		//# This is the .cnDefaultInput as defined within DateTime.Show (JS)
						case enumInputTypes.cnDefaultInput: {
							sReturn = HTMLBuilder.Date(sInputName, sAttributes, ref sInitialValue, oInputData.AdditionalData.Data, g_oSettings);
							break;
						}
						case enumInputTypes.cnTime: {
							sReturn = HTMLBuilder.Time(sInputName, sAttributes, ref sInitialValue, oInputData.AdditionalData.Data, g_oSettings);
							break;
						}
						case enumInputTypes.cnDateTime: {
							sReturn = HTMLBuilder.DateTime(sInputName, sAttributes, ref sInitialValue, oInputData.AdditionalData.Data, g_oSettings);
							break;
						}
						case enumInputTypes.cnText: {
//!							sReturn = Builder.Textbox(sInputName, sAttributes, sInitialValue, 0, oSettings);
							sReturn = HTMLBuilder.TextBox(sInputName, sAttributes, Tools.FormatDateTime(sInitialValue, ref sInputSpecificFormat, eInputType, g_oSettings), 0, g_oSettings);
							break;
						}
						case enumInputTypes.cnHidden: {
							sReturn = HTMLBuilder.Hidden(sInputName, sAttributes, sInitialValue, g_oSettings);
//!							sReturn = Builder.Hidden(sInputName, sAttributes, Tools.FormatDateTime(sInitialValue, ref sInputSpecificFormat, eInputType, g_oSettings), g_oSettings);
							break;
						}
						case enumInputTypes.cnReadOnly: {
								//#### Write out the hidden/read-only input, Format(ing the passed sInitialValue as a)DateTimeForInput using the appropriate sInputSpecificFormat
							sReturn = HTMLBuilder.Hidden(sInputName, oInputData.AdditionalData.ReadOnly_InputAttributes, sInitialValue, Web.Inputs.Tools.FormatDateTime(sInitialValue, ref sInputSpecificFormat, eInputType, g_oSettings), sAttributes, g_oSettings);
							break;
						}

						default: {
							Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_DateTime, "", "");
							break;
						}
					}
				}
					//#### Else if this is a .cnBinary eDataType
				else if (eDataType == MetaData.enumDataTypes.cnBinary) {
						//#### Determine the eInputType and process accordingly
					switch (eInputType) {
						case enumInputTypes.cnDefaultInput:
						case enumInputTypes.cnFile: {
							sReturn = HTMLBuilder.File(sInputName, sAttributes, g_oSettings);
							break;
						}
						case enumInputTypes.cnHidden: {
							sReturn = HTMLBuilder.Hidden(sInputName, sAttributes, sInitialValue, g_oSettings);
							break;
						}
						case enumInputTypes.cnReadOnly: {
							sReturn = HTMLBuilder.Hidden(sInputName, oInputData.AdditionalData.ReadOnly_InputAttributes, sInitialValue, sInitialValue, sAttributes, g_oSettings);
							break;
						}

						default: {
							Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Binary, "", "");
							break;
						}
					}
				}
					//#### Else if this is a .cnGUID eDataType
				else if (eDataType == MetaData.enumDataTypes.cnGUID) {
						//#### Determine the eInputType and process accordingly
					switch (eInputType) {
						case enumInputTypes.cnDefaultInput:
						case enumInputTypes.cnText: {
								//####     NOTE: 36 is the length of a GUID
							sReturn = HTMLBuilder.TextBox(sInputName, sAttributes, sInitialValue, 36, g_oSettings);
							break;
						}
						case enumInputTypes.cnHidden: {
							sReturn = HTMLBuilder.Hidden(sInputName, sAttributes, sInitialValue, g_oSettings);
							break;
						}
						case enumInputTypes.cnReadOnly: {
							sReturn = HTMLBuilder.Hidden(sInputName, oInputData.AdditionalData.ReadOnly_InputAttributes, sInitialValue, sInitialValue, sAttributes, g_oSettings);
							break;
						}

						default: {
							Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_GUID, "", "");
							break;
						}
					}
				}
					//#### Else if this is a .cnUnknown or .cnUnsupported g_iDataType, raise the error
				else if (eDataType == MetaData.enumDataTypes.cnUnknown ||
					eDataType == MetaData.enumDataTypes.cnUnsupported
				) {
					Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnsupportedDataType, sInputAlias, "");
				}
					//#### Else this is a string-based eDataType
				else {
						//#### Determine the eInputType and process accordingly
					switch (eInputType) {
						case enumInputTypes.cnDefaultInput:
						case enumInputTypes.cnText: {
							sReturn = HTMLBuilder.TextBox(sInputName, sAttributes, sInitialValue, oInputData.MaximumCharacterLength, g_oSettings);
							break;
						}
						case enumInputTypes.cnTextarea: {
							sReturn = HTMLBuilder.TextBox(sInputName, sAttributes, sInitialValue, oInputData.MaximumCharacterLength, g_oSettings);
							break;
						}
//! neek
						case enumInputTypes.cnHTMLEditor: {
							sReturn = HTMLBuilder.HTMLEditor(sInputName, sAttributes, sInitialValue, oInputData.MaximumCharacterLength, g_oSettings);
							break;
						}
						case enumInputTypes.cnPassword: {
							sReturn = HTMLBuilder.Password(sInputName, sAttributes, sInitialValue, oInputData.MaximumCharacterLength, g_oSettings);
							break;
						}
						case enumInputTypes.cnHidden: {
							sReturn = HTMLBuilder.Hidden(sInputName, sAttributes, sInitialValue, g_oSettings);
							break;
						}
						case enumInputTypes.cnReadOnly: {
							sReturn = HTMLBuilder.Hidden(sInputName, oInputData.AdditionalData.ReadOnly_InputAttributes, sInitialValue, sInitialValue, sAttributes, g_oSettings);
							break;
						}

						default: {
							Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Other, "", "");
							break;
						}
					}
				}

					//#### If we are being called during a post back
				if (g_bIsPostBack) {
						//#### If we are supposed to g_bDisplayErrorMessagesViaScript and the oInputData .Is(not)Valid
					if (g_bDisplayErrorMessagesViaJavaScript && ! oInputData.IsValid) {
						string sCRLF = g_oSettings.CRLF;

							//#### Append the start of the Error.Set JS code onto the sReturn value
						sReturn = sReturn + JavaScript.GenerateFileReferences(JavaScript.enumJavaScriptFiles.cnCnInputsValidation, g_oSettings) + sCRLF +
							JavaScript.BlockStart + sCRLF;

							//#### Append the .GetSetErrorJavaScript onto our sReturn value
							//####     NOTE: A StringBuilder is not used in this function because it's only on errors where the sReturn value is appended to
						sReturn = sReturn + GetSetErrorJavaScript(oInputData) + sCRLF;

							//#### Append the remainder of the Error.Set JS code onto the sReturn value
						sReturn = sReturn + sCRLF + JavaScript.BlockEnd + sCRLF;
					}
				}

					//#### Set the above modified sInitialValue back into the .Value
				oInputData.Value = sInitialValue;
			}
				//#### Else the passed sInputAlias is not within Inputs, so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnknownInputAlias, sInputAlias, "");
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Returns the requested input as represented by XHTML/DHTML code.
		/// </summary>
		/// <remarks>
		/// This implementation only renders <c>cnMultiValuePicklistExType</c> or <c>cnMultiValueSearchInSingleValuePicklist</c> inputs. All other input rendering is done with the single value implementation.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>SaveType</c> set as <c>cnSingleValuePicklistExType</c> are:
		/// <para/>   <c>cnSelect</c>*, <c>cnComboBox</c>, <c>cnOption</c>, <c>cnOptionListBox</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>SaveType</c> set as <c>cnMultiValuePicklistExType</c> are:
		/// <para/>   <c>cnCheckboxes</c>*, <c>cnCheckedListBox</c>, <c>cnMultiSelect</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnBoolean</c> are:
		/// <para/>   <c>cnCheckbox</c>*, <c>cnOption</c>, <c>cnOptionListBox</c>, <c>cnSelect</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnInteger</c> are:
		/// <para/>   <c>cnText</c>*, <c>cnCheckbox</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnFloat</c> or <c>cnCurrency</c> are:
		/// <para/>   <c>cnText</c>*, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnDateTime</c> are:
		/// <para/>   <c>cnDate</c>*, <c>cnTime</c>, <c>cnDateTime</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnBinary</c> are:
		/// <para/>   <c>cnFile</c>*, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnGUID</c> are:
		/// <para/>   <c>cnText</c>*, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnUnknown</c> or <c>cnUnsupported</c> are:
		/// <para/>   None, an error is raised.
		/// <para/>Available <paramref>eInputType</paramref>s for inputs with a <c>DataType</c> set as <c>cnChar</c> or <c>cnLongChar</c> are:
		/// <para/>   <c>cnText</c>*, <c>cnTextarea</c>, <c>cnHTMLEditor</c>, <c>cnPassword</c>, <c>cnHidden</c>, <c>cnReadOnly</c> and <c>cnDefaultInput</c>.
		/// <para/>* - <paramref>eInputType</paramref> utilised when <c>cnDefaultInput</c> is passed in.
		/// <para/>NOTE: Inputs with a <c>SaveType</c> set as <c>cnNonSearchableExType</c> or <c>cnVerbatumExType</c> are rendered based on their <c>DataType</c>.
		/// </remarks>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="eInputType">Enumeration representing the input type to render.</param>
		/// <param name="a_sInitialValues">Array of strings where each element represents an initial value of the input.</param>
		/// <param name="bForceInitialValues">Boolean value representing if the value of the input is always to be set to <paramref name="a_sInitialValues"/>.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eInputType</paramref> is unreconized or unsupported for the current input's <c>DataType</c>/<c>SaveType</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the global <c>Picklists</c> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the <paramref>sInputAlias</paramref> is defined as <c>Picklist_IsAdHoc</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eInputType</paramref> does not contain <c>cnMultiValuePicklistExType</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> has not be defined.</exception>
		/// <exception cref="Cn.CnException">Thrown when the global <c>FormName</c> is a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the global <c>Picklists</c> does not contain the refrenced PicklistName.</exception>
		/// <returns>String representing the requested XHTML/DHTML input.</returns>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		public string GenerateHTML(string sInputAlias, enumInputTypes eInputType, string[] a_sInitialValues, bool bForceInitialValues, string sAttributes) {
			MultiArray oPicklist;
			InputData oInputData = Inputs(sInputAlias);
			string sInputName = InputName(sInputAlias);
			string sReturn = "";
			MetaData.enumValueTypes eValueType;

				//#### If the passed sInputAlias .Exists (as .Get returns null if the sInputAlias is unreconized)
			if (oInputData != null) {
					//#### Set the eValueType
				eValueType = oInputData.ValueType;

					//#### If we're not supposed to bForceInitialValues and the .ValueIsFrom(the)Form or the passed a_sInitialValues is empty
				if (! bForceInitialValues && (oInputData.ValueIsFromForm || a_sInitialValues == null || a_sInitialValues.Length == 0)) {
						//#### Reset the passed a_sInitialValues to the .Values within the oInputData
					a_sInitialValues = oInputData.Values;
				}
					//#### Else we need to set the oInputData's .Values to the passed a_sInitialValues
					//####     NOTE: The a_sInitialValues is set into .Values here so the oInputData reflects these values
				else {
					oInputData.Values = a_sInitialValues;
				}

					//#### If this g_bIs(a)PostBack, oInputData .Is(not)Valid and the caller didn't provide a "class=" within sAttributes
				if (g_bIsPostBack && ! oInputData.IsValid && sAttributes.ToLower().IndexOf("class=") == -1) {
						//#### Append the .cnCSSClass_FormInputError class onto the tail of the passed sAttributes
					sAttributes += " class='" + Web.Settings.Value(Web.Settings.enumSettingValues.cnCSSClass_FormInputError) + "'";
				}

					//#### If this is a .cnMultiValuesFromPicklist eValueType (or implicitly a .cnMultiValueSearchInSingleValuePicklistExType) and *not* a .cnSingleValuePicklistExType
//!				if (eValueType == MetaData.enumValueTypes.cnMultiValuesFromPicklist &&
//					eValueType == MetaData.enumValueTypes.cnSingleValueFromPicklist
//				) {
				if (eValueType == MetaData.enumValueTypes.cnMultiValuesFromPicklist) {
						//#### If this is not a .Picklist_IsAdHoc (meaning it should be within the provided g_oPicklists)
					if (! oInputData.AdditionalData.Picklist_IsAdHoc) {
							//#### If the developer has setup the g_oPicklists
						if (g_oPicklists != null) {
								//#### Collect the oPicklist from the developer-provided Picklist_Name within .AdditionalData
							oPicklist = g_oPicklists.Picklist(oInputData.AdditionalData.Picklist_Name);

								//#### If the sPicklist exists (as unreconized sPicklistNames are returned as null from .Picklist)
							if (oPicklist != null) {
									//#### Determine the eInputType and process accordingly
								switch (eInputType) {
									case enumInputTypes.cnDefaultInput:
									case enumInputTypes.cnCheckboxes: {
										sReturn = HTMLBuilder.CheckBoxes(sInputName, oInputData.AdditionalData.MultiValue_InputAttributes, a_sInitialValues, sAttributes, oPicklist, oInputData.AdditionalData.Data, g_oSettings);
										break;
									}
									case enumInputTypes.cnCheckedListBox: {
										sReturn = HTMLBuilder.CheckedListBox(sInputName, oInputData.AdditionalData.MultiValue_InputAttributes, a_sInitialValues, sAttributes, oPicklist, oInputData.AdditionalData.Data, g_oSettings);
										break;
									}
									case enumInputTypes.cnMultiSelect: {
										sReturn = HTMLBuilder.MultiSelect(sInputName, sAttributes, a_sInitialValues, oInputData.AdditionalData.Picklist_AddLeadingBlankOption, oPicklist, oInputData.AdditionalData.Data, g_oSettings);
										break;
									}
									case enumInputTypes.cnHidden: {
										sReturn = HTMLBuilder.Hidden(sInputName, sAttributes, Cn.Data.Tools.MultiValueString(a_sInitialValues), g_oSettings);
										break;
									}
									case enumInputTypes.cnReadOnly: {
											//#### Define and setup the sDisplayedValue (.Join'ing the .Decoder'd a_sInitialValues via the defined "MultiValue_Delimiter"), then write out the .Hidden input
										string sDisplayedValue = string.Join(oInputData.AdditionalData.MultiValue_Delimiter, Picklists.Decoder(oPicklist, a_sInitialValues, g_oPicklists.StrictDecodes));
										sReturn = HTMLBuilder.Hidden(sInputName, oInputData.AdditionalData.ReadOnly_InputAttributes, Cn.Data.Tools.MultiValueString(a_sInitialValues), sDisplayedValue, sAttributes, g_oSettings);
										break;
									}

									default: {
										if (eValueType == MetaData.enumValueTypes.cnMultiValuesFromPicklist) {
//! new error message _MultiValues
											Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnreconizedFormInputType_Picklist, "", "");
										}
										else {
											Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_CallSingleRenderInput, "", "");
										}
										break;
									}
								}
							}
								//#### Else the sPicklistName does not .Exist, so raise the error
							else {
								Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnknownPicklistName, oInputData.AdditionalData.Picklist_Name, sInputAlias);
							}
						}
							//#### Else the developer has not setup the g_oPicklists, so raise the error
						else {
							Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_PicklistIsEmpty, "", "");
						}
					}
						//#### Else this was defined as an external picklist (meaning the asso. picklist is not within the provided .Picklists), so raise the error
					else {
						Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_Picklist_IsAdHoc, sInputName, "GenerateHTML");
					}
				}
					//#### Else this is a type that should be routed thru our sibling implementation, so raise the error
				else {
					Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_CallSingleRenderInput, "", "");
				}

					//#### If we are being called during a post back
				if (g_bIsPostBack) {
						//#### If we are supposed to g_bDisplayErrorMessagesViaScript and the oInputData .Is(not)Valid
					if (g_bDisplayErrorMessagesViaJavaScript && ! oInputData.IsValid) {
						string sCRLF = g_oSettings.CRLF;

							//#### Append the start of the Error.Set JS code onto the sReturn value
							//####     NOTE: A StringBuilder is not used in this function because it's only on errors where the sReturn value is appended to
						sReturn = sReturn + JavaScript.GenerateFileReferences(JavaScript.enumJavaScriptFiles.cnCnInputsValidation, g_oSettings) + sCRLF +
							JavaScript.BlockStart + sCRLF;

							//#### Append the .GetSetErrorJavaScript onto our sReturn value
							//####     NOTE: A StringBuilder is not used in this function because it's only on errors where the sReturn value is appended to
						sReturn = sReturn + GetSetErrorJavaScript(oInputData) + sCRLF;

							//#### Append the remainder of the Error.Set JS code onto the sReturn value
						sReturn = sReturn + sCRLF + JavaScript.BlockEnd + sCRLF;
					}
				}

					//#### Set the a_sInitialValues back into the .Value
				oInputData.Values = a_sInitialValues;
			}
				//#### Else the passed sInputAlias is not within .FormInputs, so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + "GenerateHTML", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnknownInputAlias, sInputAlias, "");
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves the input data for the referenced input alias.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <returns>Object representing the InputData for the passed <paramref>sInputAlias</paramref>.</returns>
		///############################################################
		/// <LastUpdated>July 15, 2010</LastUpdated>
		public virtual InputData Inputs(string sInputAlias) {
			InputData oReturn = null;

				//#### If the passed sInputAlias .Exists, reset the oReturn value to the InputData present within gh_oInputs
			if (Exists(sInputAlias)) {
				oReturn = (InputData)(gh_oInputs[sInputAlias]);

					//#### If the .Value/.Values have not yet been set
					//####     NOTE: The g_iSourceRecordNumber defaults to -1 if the InputCollection is not assoicated with a g_oParentForm
				if (oReturn.SourceRecordNumber == -1) {
						//#### If this is a call during a g_bIsPostBack
						//####     NOTE: Since the developer expects the .Values to remain static within the object we only load them on the initial call, hence all the checks below
					if (g_bIsPostBack) {
							//#### (Re)Collect the .Values from the Request's .Form and set .ValueIsFromForm based on its difference from the .DefaultValue
						oReturn.Values = g_oSettings.Request.Form.GetValues(sInputAlias);

int i;
string[] a_sKeys = g_oSettings.Request.Form.AllKeys;
string sInputAliasToLower = sInputAlias.ToLower();

	//#### Traverse the a_sKeys, looking for the sInputAlias
for (i = 0; i < a_sKeys.Length; i++) {
		//#### If the current a_sKeys matches the sInputAliasToLower, flip .ValueWasSubmitted to true and fall from the loop
		//####     NOTE: Radio buttons and checkboxes that are blank (no values selected) are not present within the a_sKeys, so they are not set as .ValueWasSubmitted. This is the nature of HTTP POSTs and cannot be helped.
	if (a_sKeys[i].ToLower() == sInputAliasToLower) {
		oReturn.ValueWasSubmitted = true;
		break;
	}
}

//!						oReturn.ValueIsFromForm = (oReturn.ValueWasSubmitted && oReturn.Value != oReturn.DefaultValue);
						oReturn.ValueIsFromForm = (oReturn.Value != oReturn.DefaultValue);
oReturn.SetSourceRecordNumber(1);
					}
						//#### Else this is a non-g_bIsPostBack call
					else {
							//#### (Re)Load .Value with the .DefaultValue and reset .ValueIsFromForm to false (as since this is a non-g_bIsPostBack call, the .Value will never be from the form)
						oReturn.Value = oReturn.DefaultValue;
						oReturn.ValueIsFromForm = false;
oReturn.SetSourceRecordNumber(1);
					}
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves the input data collection(s) for the referenced table name/column name.
		/// </summary>
		/// <remarks>
		/// Since it is allowed to define multiple <c>Inputs</c> that refer to the same <paramref>sTableName</paramref>/<paramref>sColumnName</paramref> pair, this function returns an array of <c>InputData</c> objects that refer to the pair.
		/// <para/>NOTE: This is far more process intensive then the sibling implementation <c>Input(sInputAlias)</c>. Use this function only when you do not have access to the input's <c>sInputAlias</c>!
		/// </remarks>
		/// <param name="sTableName">String representing the column's source table name.</param>
		/// <param name="sColumnName">String representing the column name.</param>
		/// <returns>Array of <c>InputData</c> objects containing the <c>Inputs</c> defined within this instance that reference the passed <paramref>sTableName</paramref>/<paramref>sColumnName</paramref> pair.</returns>
		///############################################################
		/// <LastUpdated>August 31, 2007</LastUpdated>
		public InputData[] Inputs(string sTableName, string sColumnName) {
			InputData[] a_oReturn = null;
			InputData oCurrentInput;
			string[] a_sInputAliases = InputAliases;
			int iReturnCount = 0;
			int iInputCount = gh_oInputs.Count;
			int i;

				//#### If there were a_sInputAliases defined for this instance
			if (a_sInputAliases != null && a_sInputAliases.Length > 0) {
					//#### .ToLower the passed sTableName and sColumnName in prep for the comparisons below
				sTableName = sTableName.ToLower();
				sColumnName = sColumnName.ToLower();

					//#### Dimension the a_oReturn value to the maximum number of InputData objects we could return
				a_oReturn = new InputData[iInputCount];

					//#### Traverse the .Inputs/a_sInputAliases
				for (i = 0; i < iInputCount; i++) {
						//#### Collect the oCurrentInput for this loop from our sister implementation
					oCurrentInput = Inputs(a_sInputAliases[i]);

						//#### If the oCurrentInput's .TableName and .ColumnName match the passed sTableName and sColumnName (checking their .Lengths first as that is a far faster comparison)
					if (oCurrentInput.TableName.Length == sTableName.Length && oCurrentInput.ColumnName.Length == sColumnName.Length &&
						oCurrentInput.TableName.ToLower() == sTableName && oCurrentInput.ColumnName.ToLower() == sColumnName
					) {
							//#### Set the oCurrentInput into the next available index within the a_oReturn value, then inc iReturnCount
						a_oReturn[iReturnCount] = oCurrentInput;
						iReturnCount++;
					}
				}

					//#### If not all of the .Inputs were located above (else all were found, so do nothing as the a_oReturn's .length is already correct)
				if (iReturnCount != iInputCount) {
						//#### Determine the number of .Inputs located above and process accordingly
					switch (iReturnCount) {
							//#### If no .Inputs were located above, reset the return value to null
						case 0: {
							a_oReturn = null;
							break;
						}
							//#### If a single .Input was located above
						case 1: {
								//#### Redimension the a_oReturn value, utilizing oCurrentInput to store the single above located .Input
							oCurrentInput = a_oReturn[0];
							a_oReturn = new InputData[1];
							a_oReturn[0] = oCurrentInput;
							break;
						}
							//#### Else more then one, but not all of the .Inputs were located above
						default: {
							InputData[] a_oReturnTemp = new InputData[iReturnCount];

								//#### Traverse the valid indexes within the a_oReturn value, copying the entries into a_oReturnTemp
							for (i = 0; i < iReturnCount; i++) {
								a_oReturnTemp[i] = a_oReturn[i];
							}

								//#### Un-recopy the a_oReturn value
							a_oReturn = a_oReturnTemp;
							break;
						}
					}
				}
			}

				//#### Return the above determined a_oReturn value to the caller
			return a_oReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves the HTML input name for the current record number.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <returns>String representing the HTML input name for the current input.</returns>
		///############################################################
		/// <LastUpdated>February 25, 2010</LastUpdated>
		public virtual string InputName(string sInputAlias) {
				//#### Since thsi is the base implementation (which does not account for Renderer.Form functions), simply return the passed sInputAlias
			return sInputAlias;
		}

		///############################################################
		/// <summary>
		/// Returns the requested <c>Renderer.Form</c> HTML form hook.
		/// </summary>
		/// <remarks>
		/// The returned code snipits allow <c>Renderer.Form</c> to attach to various HTML form events, as well as initilize client side validation, DIV management and CSS formatting.
		/// <para/>NOTE: <paramref>eUIHookType</paramref> values for <c>cnOnSubmit</c> or <c>cnDoSubmit</c> contain double quotes (") so any HTML attribute you are including them within must be delimited by single quotes (').
		/// </remarks>
		/// <param name="eUIHookType">Enumeration representing the form hook type to return.</param>
		/// <returns>String representing the requested <paramref>eUIHookType</paramref>.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eUIHookType</paramref> is unreconized.</exception>
		///############################################################
		/// <LastUpdated>December 3, 2009</LastUpdated>
		public string UIHook(enumUIHookTypes eUIHookType) {
			string sReturn = "";

				//#### Determine the passed eUIHookType and sReturn the .cnErrorMessagePopUp JavaScript code snipit
			switch (eUIHookType) {
				case enumUIHookTypes.cnErrorMessagePopUp: {
					sReturn = " onFocus='Cn._.wiveu.ShowPopUp(this);' onBlur='Cn._.wiveu.Hide();'";
					break;
				}
				case enumUIHookTypes.cnErrorMessageToolTip: {
					sReturn = " onMouseMove='Cn._.wiveu.ShowToolTip(this, event);' onMouseOut='Cn._.wiveu.Hide();'";
					break;
				}
				case enumUIHookTypes.cnFormOnSubmit: {
					sReturn = " return Cn._.wiv.Validate(this, false);";
					break;
				}
				case enumUIHookTypes.cnFormDoSubmit: {
//! not checking we have .InputAliases
					sReturn = " Cn._.wiv.Validate(" + GetFormIDJavaScript(InputAliases[0]) + ", true);";
					break;
				}
				case enumUIHookTypes.cnFormAction: {
					HttpRequest oRequest = g_oSettings.Request;

						//#### If we're already in g_bIsPostBack mode, return the Script_Name/Query_String as-is
					if (g_bIsPostBack) {
						sReturn = oRequest.ServerVariables["Script_Name"] + "?" + oRequest.ServerVariables["Query_String"];
					}
						//#### Else we need to append the IsPostBack=True key/value onto the Script_Name/Query_String
					else {
							//#### If there is a Query_String to append, we'll need a leading & on the IsPostBack key/value
						if (oRequest.ServerVariables["Query_String"].Length > 0) {
							sReturn = oRequest.ServerVariables["Script_Name"] + "?" + oRequest.ServerVariables["Query_String"] + "&" + Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + "IsPostBack=True";
						}
							//#### Else there is no Query_String to append, so simply append the IsPostBack key/value
						else {
							sReturn = oRequest.ServerVariables["Script_Name"] + "?" + Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + "IsPostBack=True";
						}
					}
					break;
				}

					//#### Else the passed eUIHookType is unreconized, so raise the error
				default: {
					Internationalization.RaiseDefaultError(g_cClassName + "UIHook", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eUIHookType", Cn.Data.Tools.MakeString(eUIHookType, ""));
					break;
				}
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

public string GenerateCSS() {
	string sBaseDirectory = Web.Settings.Value(Web.Settings.enumSettingValues.cnUIDirectory);
	string sCRLF = g_oSettings.CRLF;
	/*	string sReturn = "";
	int iRenderedJSFiles = oSettings.RenderedJavaScriptFiles;

		//#### As long as the iRenderedJavaScriptFiles is not set to .cnDisableJavaScriptRendering
	if (iRenderedJavaScriptFiles != (int)Web.JavaScript.enumJavaScriptFiles.cnDisableJavaScriptRendering) {
			//#### If the .cnYUICalendar eJavaScriptFile has been rendered
		if ((iRenderedJavaScriptFiles & (int)Web.JavaScript.enumJavaScriptFiles.cnYUICalendar) != 0) {
			sReturn += "@import url(" + sBaseDirectory + "css/Calendar.css);" + sCRLF;
		}

			//#### If the .cnCnTools eJavaScriptFile has been rendered
		if ((iRenderedJavaScriptFiles & (int)Web.JavaScript.enumJavaScriptFiles.cnCnTools) != 0) {
			sReturn += "@import url(" + sBaseDirectory + "css/SpecialListBox.css);" + sCRLF;
		}

			//#### If the .cnCnInputsMaxLength eJavaScriptFile has been rendered
		if ((iRenderedJavaScriptFiles & (int)Web.JavaScript.enumJavaScriptFiles.cnCnInputsMaxLength) != 0) {
			sReturn += "TEXTAREA { behavior: url(" + sBaseDirectory + "js/maxlength.htc); }" + sCRLF;
		}
	}
	*/

		//#### Return the .cnFormInputsCSS snipit
		//####     NOTE: Since it is not possible to create a good cross platform Textarea maxlength routine (thanks to the inability to detect the user pasting in data), the IE-only .htc is used below
	return "@import url(" + sBaseDirectory + "css/Calendar.css);" + sCRLF +
		"@import url(" + sBaseDirectory + "css/SpecialListBox.css);" + sCRLF +
		"TEXTAREA { behavior: url(" + sBaseDirectory + "js/maxlength.htc); }";
}

		///############################################################
		/// <summary>
		/// Validates the inputs defined within this instance.
		/// </summary>
		/// <remarks>
		/// NOTE: If ChildCollections are defined, it is validated AFTER the inputs defined within this instance. In other words, the order of precidence is "Parent-Child-GrandChild...".
		/// </remarks>
		/// <returns>A Boolean value indicating if all the inputs defined within this instance are valid.</returns>
		///############################################################
		/// <LastUpdated>March 17, 2010</LastUpdated>
		public virtual bool Validate() {
			string[] a_sKeys = InputAliases;
			int i;
			bool bReturn = true;

				//#### If we have a_sKeys to traverse
			if (a_sKeys != null && a_sKeys.Length > 0) {
					//#### Traverse the a_sKeys
				for (i = 0; i < a_sKeys.Length; i++) {
						//#### If the InputData for the current a_sKey .Is(not)Valid, flip our bReturn value to false
					if (! Inputs(a_sKeys[i]).IsValid) {
						bReturn = false;
//!Cn.Tools.dWrite("Errored: " + a_sKeys[i] + " with " + GetInputMetaData(a_sKeys[i]).Errors + " (Value = " + GetInputMetaData(a_sKeys[i]).Value + ")");
					}
				}
			}

				//#### Traverse our .ChildCollections (if any), resetting our bReturn value to its
			bReturn = DoValidateChildCollections(bReturn);

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves a structure that represents the current values for the inputs defined within this instance as a <c>MultiArray</c>-style row.
		/// </summary>
		/// <remarks>
		/// <param/>NOTE: The return value holds the <c>Value</c> for each <c>ColumnName</c>. If a <c>ColumnName</c> is not unique across two or more <c>InputAlias</c>es, then a unique numbered <c>ColumnName</c> is generated (i.e. - "ID" would become "ID1", "ID2", etc.). If a <c>ColumnName</c> is not unique across more then 1,000 <c>InputAlias</c>es, all subsequent <c>Values</c> are ignored (i.e. - "ID999" is the largest unique numbered <c>ColumnName</c> that is generated).
		/// </remarks>
		/// <returns>Hashtable of String Arrays where each key entry represents the data for an InputAlias' input values defined within this instance.</returns>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public Hashtable Row() {
			Hashtable h_sReturn = null;
			string[] a_sInputAliases = InputAliases;
			int i;

				//#### If there were a_sInputAliases defined for this instance
			if (a_sInputAliases != null && a_sInputAliases.Length > 0) {
					//#### Reset our h_sReturn value
				h_sReturn = new Hashtable();

					//#### Traverse the a_sInputAliases, .Add'ing each's .Values into our h_sReturn value
//! Store .Value?
				for (i = 0; i < a_sInputAliases.Length; i++) {
					h_sReturn.Add(a_sInputAliases[i], ((InputData)gh_oInputs[a_sInputAliases[i]]).Values);
				}
			}

				//#### Return the above determined h_sReturn value to the caller
			return h_sReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves each record's user submitted value for the referenced input alias group of HTML inputs.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <returns>Array of strings where each element represents a record's submitted value for the <paramref>sInputAlias</paramref> group of HTML inputs.</returns>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		public virtual string[] Column(string sInputAlias) {
			HttpRequest oRequest = HttpContext.Current.Request;
			string[] a_sReturn = null;

				//#### If the passed sInputAlias .Exists
			if (Exists(sInputAlias)) {
					//#### Re-dimension our a_sReturn value for a single entry and get the value from the oRequest
				a_sReturn = new string[1];
				a_sReturn[0] = oRequest.Form[sInputAlias];
			}

				//#### Return the above determined sReturn value to the caller
			return a_sReturn;
		}

		///############################################################
		/// <summary>
		/// Collects the column names defined within this instance.
		/// </summary>
		/// <param name="bFullyQualifyColumnNames">Boolean value indicating if the column names are to be fully qualified (i.e. - "Table.Column").</param>
		/// <returns>String array where each element represents the name of a column.</returns>
		///############################################################
		/// <LastUpdated>August 31, 2007</LastUpdated>
		public string[] ColumnNames(bool bFullyQualifyColumnNames) {
			string[] a_sInputAliases = InputAliases;
			string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter;
			string sCurrentColumnName;
			string sReturn;
			int i;

				//#### Init the sReturn value to a leading sPrimaryDelimiter
			sReturn = sPrimaryDelimiter;

				//#### If there were a_sInputAliases defined for this instance
			if (a_sInputAliases != null && a_sInputAliases.Length > 0) {
					//#### If the caller wants to have bFullyQualify('d)ColumnNames
				if (bFullyQualifyColumnNames) {
					InputData oInputData;

						//#### Traverse the a_sInputAliases
					for (i = 0; i < a_sInputAliases.Length; i++) {
							//#### Collect the oInputData, then reset the sCurrentColumnName (followed by a sPrimaryDelimiter)
							//####     NOTE: The .TableName/.ColumnName are not lowercased here as some containers consider case important
						oInputData = ((InputData)gh_oInputs[a_sInputAliases[i]]);
						sCurrentColumnName = oInputData.TableName + "." + oInputData.ColumnName + sPrimaryDelimiter;

							//#### If the sCurrentColumnName (which is surrounded by sPrimaryDelimiters) is not already within the sReturn value, append it now
//! maybe use Hashtable.keys to accomplish same? use List?
						if (sReturn.IndexOf(sPrimaryDelimiter + sCurrentColumnName) == -1) {
							sReturn += sCurrentColumnName;
						}
					}
				}
					//#### Else the caller only wants the ColumnNames
				else {
						//#### Traverse the a_sInputAliases
					for (i = 0; i < a_sInputAliases.Length; i++) {
							//#### Reset the sCurrentColumnName (followed by a sPrimaryDelimiter)
						sCurrentColumnName = ((InputData)gh_oInputs[a_sInputAliases[i]]).ColumnName + sPrimaryDelimiter;

							//#### If the sCurrentColumnName (which is surrounded by ','s) is not already within the sReturn value, append it now
//! maybe use Hashtable.keys to accomplish same? use List?
						if (sReturn.IndexOf(sPrimaryDelimiter + sCurrentColumnName) == -1) {
							sReturn += sCurrentColumnName;
						}
					}
				}
			}

				//#### If .ColumnNames were found (i.e. - sReturn != sPrimaryDelimiter)
			if (sReturn.Length > 1) {
					//#### Split the above built sReturn value (removing it's leading/trailing ','s first), returning the result to the caller
				return sReturn.Substring(1, sReturn.Length - 2).Split(sPrimaryDelimiter.ToCharArray());
			}
				//#### Else no .ColumnNames were found, so return null
			else {
				return null;
			}
		}

		///############################################################
		/// <summary>
		/// Collects the column names for the provided table defined within this instance.
		/// </summary>
		/// <param name="sTableName">String representing the name of the table whos column names we are to collect.</param>
		/// <returns>String array where each element represents the name of a column for the passed <paramref>sTableName</paramref>.</returns>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public string[] ColumnNames(string sTableName) {
			InputData oInputData;
			string[] a_sInputAliases = InputAliases;
			string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter;
			string sCurrentColumnName;
			string sCurrentTableName;
			string sReturn;
			int iLen;
			int i;

				//#### Init the sReturn value to a leading sPrimaryDelimiter
			sReturn = sPrimaryDelimiter;

				//#### If there were a_sInputAliases defined for this instance
			if (a_sInputAliases != null && a_sInputAliases.Length > 0) {
					//#### Determine the passed sTableName's .Length
				iLen = sTableName.Length;

					//#### Traverse the a_sInputAliases
				for (i = 0; i < a_sInputAliases.Length; i++) {
						//#### Reset oInputData and the sCurrentTableName for this loop
					oInputData = (InputData)(gh_oInputs[a_sInputAliases[i]]);
					sCurrentTableName = oInputData.TableName;

						//#### If the sCurrentTableName matches the passed sTableName (checking their .Length's first as that is a far faster comparison)
					if (sCurrentTableName.Length == iLen && sCurrentTableName == sTableName) {
							//#### Set the sCurrentColumnName
						sCurrentColumnName = oInputData.ColumnName + sPrimaryDelimiter;

							//#### If the sCurrentColumnName (which is surrounded by sPrimaryDelimiters) is not already within sReturn, append it now
//! maybe use Hashtable.keys to accomplish same? use List?
						if (sReturn.IndexOf(sPrimaryDelimiter + sCurrentColumnName) == -1) {
							sReturn += sCurrentColumnName;
						}
					}
				}
			}

				//#### If .ColumnNames were found above for the passed sTableName (i.e. - sReturn != sPrimaryDelimiter)
			if (sReturn.Length > 1) {
					//#### Split the above built sReturn value (removing it's leading/trailing sPrimaryDelimiters first), returning the result to the caller
				return sReturn.Substring(1, sReturn.Length - 2).Split(sPrimaryDelimiter.ToCharArray());
			}
				//#### Else no column names were found for the passed sTableName, so return null
			else {
				return null;
			}
		}

		///############################################################
		/// <summary>
		/// Determines if this instance contains the referenced input alias.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <returns>Boolean value signaling the existance of the passed <paramref>sInputAlias</paramref>.<para/>Returns true if the passed <paramref>sInputAlias</paramref> is defined within this instance, or false if it is not present.</returns>
		///############################################################
		/// <LastUpdated>May 23, 2005</LastUpdated>
		public bool Exists(string sInputAlias) {
			return gh_oInputs.Contains(sInputAlias);
		}

		///############################################################
		/// <summary>
		/// Returns the standard record validation JavaScript function code for client side data validation.
		/// </summary>
		/// <remarks>
		/// This function physicially writes out and attaches the anonymous JavaScript data validation function for this instance's <c>FormName</c> into the validation functionality.
		/// </remarks>
		/// <param name="sCustomPreValidationScript">String representing a snipit of JavaScript code to run before per-record validation is begun.</param>
		/// <param name="sCustomRecordValidationScript">String representing a snipit of JavaScript code to run during each record's validation.</param>
		/// <param name="sCustomPostValidationScript">String representing a snipit of JavaScript code to run after per-record validation is finished.</param>
		/// <param name="bIncludeScriptBlock">Boolean value indicating if the validation JavaScript is to be rendered within its own script block.</param>
		/// <exception cref="Cn.CnException">Thrown when the global <c>FormName</c> is a null-string.</exception>
		/// <returns>String representing the standard record validation JavaScript function code snipit for client side data validation.</returns>
		///############################################################
		/// <LastUpdated>December 3, 2009</LastUpdated>
		public string ValidationJavaScript(string sCustomPreValidationScript, string sCustomRecordValidationScript, string sCustomPostValidationScript, bool bIncludeScriptBlock) {
			StringBuilder oReturn = new StringBuilder();
			InputData oInputData;
			string[] a_sInputAliases = InputAliases;
			string sCRLF = g_oSettings.CRLF;
			int i;

				//#### If there were a_sInputAliases defined for this instance
			if (a_sInputAliases != null && a_sInputAliases.Length > 0) {
					//#### Output the external JavaScript file references for the .cnCnRendererFormValidation
				oReturn.Append(JavaScript.GenerateFileReferences(JavaScript.enumJavaScriptFiles.cnCnRendererForm, g_oSettings));

					//#### If we are supposed to bRender(the)ScriptBlock, open the block now
				if (bIncludeScriptBlock) {
					oReturn.Append(JavaScript.BlockStart + sCRLF);
				}
/*
					//#### Collect the sFormID and sFirstInputID
				sFormID = GetFormIDJavaScript(a_sInputAliases[0], out sFirstInputID);

					//#### Define this form's validation function, utilizing .JavaScript's .Workspace to store the function
				oReturn.Append(
					"Cn._.wj.Workspace.wiv_ValidationJavaScript = function(bSubmitForm) {" + sCRLF +
						"var rErrors = Cn._.wive;" + sCRLF +
						"var rForm = Cn._.wrf;" + sCRLF +
						"var a_sInputAliases = " + Cn.Web.JavaScript.ToArray(a_sInputAliases) + ";" + sCRLF +
						"var sFormID = " + sFormID + ";" + sCRLF +
						"var bProcessFirstRecordOnly = " + (g_oParentForm == null ? "true" : "false") + ";" + sCRLF +
						"var iCurrentRecord = 1;" + sCRLF
				);
					//#### .Reset any current rErrors
				oReturn.Append(
						"rErrors.Reset(sFormID);" + sCRLF
				);

					//#### If the caller passed in some sCustomPreValidationJS, print it out now
				if (sCustomPreValidationScript.Length > 0) {
					oReturn.Append(sCustomPreValidationScript + sCRLF);
				}

					//#### Traverse the a_sInputAliases, validating each of the inputs on the current form, setting any encountered errors implicitly
				for (i = 0; i < a_sInputAliases.Length; i++) {
						//#### Collect the oInputData for this loop
						//####     NOTE: Since we have no need for the .Values, we can do this directly rather then via .Get
					oInputData = (InputData)(gh_oInputs[a_sInputAliases[i]]);

						//#### If this oInputData .IsAttachedToDataSource
					if (oInputData.IsAttachedToDataSource) {
							//#### Determine the sValidationCall for the current a_sInputAliases
						sValidationCall = oInputData.GetValidationScript(sFormID);

							//#### If a sValidationCall was returned for the current a_sInputAliases, .Append it onto oAttachedInputsScript
						if (sValidationCall.Length > 0) {
							oAttachedInputsScript.Append(sValidationCall + ";" + sCRLF);
						}
					}
						//#### Else this oInputData .Is(not)AttachedToDataSource
					else {
							//#### Determine the sValidationCall for the current a_sInputAliases
						sValidationCall = oInputData.GetValidationScript(sFormID);

							//#### If a sValidationCall was returned for the current a_sInputAliases, .Append it onto our oReturn value
						if (sValidationCall.Length > 0) {
							oAttachedInputsScript.Append(sValidationCall + ";" + sCRLF);
						}
					}
				}

					//#### If there were oAttachedInputsScript's .Append'd above
				if (oAttachedInputsScript.Length > 0) {
						//#### While we still have records to traverse
					oReturn.Append(
							"while (rForm.RecordExists(sFormID, iCurrentRecord) || bProcessFirstRecordOnly) {" + sCRLF
					);
						//#### If we are supposed to .Process(the)Record (or this bIs(a)SingleNewRecord)
					oReturn.Append(
								"if (rForm.ProcessRecord(sFormID, iCurrentRecord, a_sInputAliases) || bProcessFirstRecordOnly) {" + sCRLF
					);

						//#### If the caller passed in some sCustomRecordValidationJS, print it out now
					if (sCustomRecordValidationScript.Length > 0) {
						oReturn.Append(sCustomRecordValidationScript + sCRLF);
					}

						//#### .Append the oAttachedInputsScript onto our oReturn value
					oReturn.Append(oAttachedInputsScript.ToString() + sCRLF);

						//#### Finish writing out the bottom of if
					oReturn.Append("}" + sCRLF);

						//#### Else we aren't ment to check this record, so make sure any currently set errors are cleared
					oReturn.Append(
								"else {" + sCRLF +
									"rForm.ClearRecordErrors(sFormID, iCurrentRecord, a_sInputAliases);" + sCRLF +
								"}" + sCRLF
					);
						//#### Inc the iCurrentRecord for the next loop, ensure bProcessFirstRecordOnly is false and write out the bottom of the loop
					oReturn.Append(
								"iCurrentRecord++;" + sCRLF +
								"bProcessFirstRecordOnly = false;" + sCRLF +
							"}" + sCRLF
					);
				}

					//#### If the caller passed in some sCustomPostValidationJS, print it out now
				if (sCustomPostValidationScript.Length > 0) {
					oReturn.Append(sCustomPostValidationScript + sCRLF);
				}

					//#### If some errors were located above, alert the user and return false
				oReturn.Append(
						"if (rErrors.Count(sFormID) > 0) {" + sCRLF +
							"alert('" + Web.Inputs.Tools.EscapeCharacters(Web.Settings.Internationalization.Value(Cn.Configuration.Internationalization.enumInternationalizationValues.cnEndUserMessages_Alert), "'") + "');" + sCRLF +
							"return false;" + sCRLF +
						"}" + sCRLF
				);
					//#### Else the data was successfully validated
				oReturn.Append(
						"else {" + sCRLF
				);
					//#### If we're supposed to bSubmit(the)Form, do so now while returning true
				oReturn.Append(
							"if (bSubmitForm) { Cn._.wif.Submit(sFormID); }" + sCRLF +
							"return true;" + sCRLF +
						"}" + sCRLF +
					"};" + sCRLF
				);

					//#### .Define the validation function for the g_sFormID (as stored in .JavaScript's .Workspace, then reset it to null)
					//####     NOTE: Since the sFirstInputID must (should) exist within the HTML Form, we can wait for it to show-up within the DOM rather then the Form directly (as we may not have the Form's ID)
	//! need to make .AttachToForm and .enumErrorUIHookTypes setable by argument
				oReturn.Append(
					"YAHOO.util.Event.onContentReady('" + sFirstInputID + "', function() {" + sCRLF +
						"Cn._.wiv.Define(" + sFormID + ", Cn._.wj.Workspace.wiv_ValidationJavaScript);" + sCRLF +
						"Cn._.wive.AttachToForm(" + sFormID + ", false, Cn._.wive.enumErrorUIHookTypes.cnToolTip, 0, 0);" + sCRLF +
						"Cn._.wj.Workspace.wiv_ValidationJavaScript = null;" + sCRLF +
					"}, null, false);"
				);
*/

					//#### Borrow the use of Cn.Web.JavaScript.Workspace to store our form validater function, then setup our oInputMetaData
				oReturn.Append("Cn._.wj.Workspace.wiv_ValidationJavaScript = function() {" + sCRLF +
					"var a_oInputMetaData = {};" + sCRLF
				);

					//#### Traverse the a_sInputAliases
				for (i = 0; i < a_sInputAliases.Length; i++) {
						//#### Collect the oInputData for this loop
						//####     NOTE: Since we have no need for the .Values, we can do this directly rather then via .Get
					oInputData = (InputData)(gh_oInputs[a_sInputAliases[i]]);

						//#### .Append the entry for the current a_sInputAliases onto our oReturn value, which populates the oInputMetaData
					oReturn.Append("a_oInputMetaData['" + a_sInputAliases[i] + "'] = new Cn._.wiv.InputMetaDataEntry(" +
							"Cn._.wi.enumDataTypes." + oInputData.DataType + "," +
							(! oInputData.IsNullable).ToString().ToLower() + "," +
							oInputData.MaximumCharacterLength + "," +
							oInputData.NumericPrecision + "," +
							"'" + oInputData.MinimumNumericValue + "'," +
							"'" + oInputData.MaximumNumericValue + "'," +
							oInputData.AdditionalData.DateTime_ValidateDataType.ToString().ToLower() +
						");" + sCRLF
					);
				}

					//#### Call Cn.Inputs.Validation.InputMetaData with the name of the first a_sInputAliases (who's .form.id will be collected) and the above populated oInputMetaData
//! make argument to specify if to bAttachToForm
				oReturn.Append("Cn._.wiv.AttachValidaters('" + a_sInputAliases[0] + "', a_oInputMetaData, true, " + GetIsRendererFormJavaScript() + ");" + sCRLF
					+ "};" + sCRLF
				);

					//#### When the first a_sInputAliases is .onContentReady, we are assured that it's form is also ready (as by definition HTML inputs must exist within an HTML form), so call the above borrowed .Workspace as a function to populate the InputMetaData then reset the .Workspace to null
//! make argument to specify if to .AttachListeners
				oReturn.Append(
					"YAHOO.util.Event.onContentReady('" + a_sInputAliases[0] + "', function() {" + sCRLF +
						"Cn._.wj.Workspace.wiv_ValidationJavaScript();" + sCRLF +
						"Cn._.wiveu.AttachListeners(Cn._.wi.RelatedForm('" + a_sInputAliases[0] + "').elements, Cn._.wiveu.enumErrorUIHookTypes.cnValidateOnBlur);" + sCRLF +
						"Cn._.wj.Workspace.wiv_ValidationJavaScript = null;" + sCRLF +
					"}, null, Cn._.wiv);"
				);

					//#### If we are supposed to bRender(the)ScriptBlock, close the block now
				if (bIncludeScriptBlock) {
					oReturn.Append(JavaScript.BlockEnd + sCRLF);
				}
			}

				//#### Return the oReturn value to the caller
			return oReturn.ToString();
		}


		//##########################################################################################
		//# Protected Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Validates that the passed <paramref name="sInputAlias"/> is valid and unique for the <c>Add</c> functions.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <returns>Boolean value representing if the passed <paramref name="sInputAlias"/> is valid and unique.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is null or a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is already defined.</exception>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		protected bool DoAdd_ValidateNewInputAlias(string sInputAlias) {
			bool bReturn = true;

				//#### If the passed sInputAlias is null or a null-string, flip our bReturn value and raise the error
			if (string.IsNullOrEmpty(sInputAlias)) {
				bReturn = false;
				Internationalization.RaiseDefaultError(g_cClassName + "Add", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "sInputAlias", "");
			}
				//#### Else if the passed sInputAlias already .Exists, flip our bReturn value and raise the error
			else if (Exists(sInputAlias)) {
				bReturn = false;
				Internationalization.RaiseDefaultError(g_cClassName + "Add", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_DuplicateInputAlias, sInputAlias, sInputAlias);
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Traverses the ChildCollections, validating each in turn.
		/// </summary>
		/// <param name="bCurrentReturn">Boolean value representing the current boolean return value of the caller.</param>
		/// <returns>Boolean value representing if the ChildCollections passed or failed validation.</returns>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		protected bool DoValidateChildCollections(bool bCurrentReturn) {
			int i;
			bool bReturn = bCurrentReturn;

/*
			IInputCollection oDecendantCollection = g_oChildCollection;

				//#### Traverse our oDecendantCollection's, ensuring we don't have a reference to ourselves (so we avoid an infinite recusive loop)
			do {
					//#### If the current oDecendantCollection is a reference to ourselves, cue the banjos and raise the error
				if (oDecendantCollection == this) {
					//# cue banjos =)
					Internationalization.RaiseDefaultError(g_cClassName + "Validate", Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbMetaData_InvalidParentChildRelationship, "ChildCollections", "");
				}

					//#### Determine our next oDecendantCollection (if any)
				oDecendantCollection = oDecendantCollection.ChildCollection;
			} while(oDecendantCollection != null);
*/

				//#### Traverse our gl_oChildCollections (if any)
			for (i = 0; i < gl_oChildCollections.Count; i++) {
					//#### .Validate each valid gl_oChildCollections, flipping our bReturn value if it's .Validate is unsuccessful
				if (gl_oChildCollections[i] != null && ! gl_oChildCollections[i].Validate()) {
					bReturn = false;
				}
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Gets the HTML Form ID JavaScript code block.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <returns>String representing the JavaScript code block to collect the HTML Form ID.</returns>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		protected abstract string GetFormIDJavaScript(string sInputAlias);

		///############################################################
		/// <summary>
		/// Gets the Set error javascript code block
		/// </summary>
		/// <param name="oInputData">Object representing the InputData instance to deeply copy into this instance.</param>
		/// <returns>String representing the JavaScript code block to Set an error.</returns>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		protected abstract string GetSetErrorJavaScript(InputData oInputData);

		///############################################################
		/// <summary>
		/// Gets the IsRendererForm javascript code block
		/// </summary>
		/// <remarks>
		/// NORE: This is a very stupid little function, but it helps .GetValidationScript because this value is the only change between the base and Renderer.Form versions
		/// </remarks>
		/// <returns>String representing a JavaScript boolean value indicating if this is a Renderer.Form instance.</returns>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		protected abstract string GetIsRendererFormJavaScript();


	} //# public abstract class InputCollectionBase


	///########################################################################################################################
	/// <summary>
	/// Represents the collection of defined <c>Form</c> Inputs.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>November 26, 2009</LastFullCodeReview>
	public class InputCollection : InputCollectionBase {
			//#### Declare the required private constants
	  //private const string g_cClassName = "Cn.Web.Inputs.InputCollection.";
		private const string g_cISRENDERERFORM = "false";


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public InputCollection() {
				//#### Pass the call off to .Reset to init the class vars
			Reset();
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		public InputCollection(Settings.Current oSettings) {
				//#### Pass the call off to .Reset to init the class vars
			Reset(oSettings);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		public void Reset() {
				//#### Pass the call off to .Reset to init the class vars
			Reset(new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		public void Reset(Settings.Current oSettings) {
				//#### Pass the call off to our base implementation
			DoReset(oSettings);
		}


		//##########################################################################################
		//# Protected Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the HTML Form ID JavaScript code block.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <returns>String representing the JavaScript code block to collect the HTML Form ID.</returns>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		protected override string GetFormIDJavaScript(string sInputAlias) {
				//#### Pre/append the JS code to collect the .form.id from the sInputID, returning it to the caller
			return "Cn._.t.GetByID('" + sInputAlias + "').form.id";
		}

		///############################################################
		/// <summary>
		/// Gets the Set error javascript code block
		/// </summary>
		/// <param name="oInputData">Object representing the InputData instance to deeply copy into this instance.</param>
		/// <returns>String representing the JavaScript code block to Set an error.</returns>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		protected override string GetSetErrorJavaScript(InputData oInputData) {
			return "Cn._.wive.Set(Cn._.wi.Get('" + Web.Inputs.Tools.EscapeCharacters(oInputData.InputAlias, "'") + "'), " + (int)oInputData.ErrorType + ", " + (int)oInputData.DataType + ", '" + Web.Inputs.Tools.EscapeCharacters(oInputData.ErrorMessage, "'") + "'); ";
		}

		///############################################################
		/// <summary>
		/// Gets the IsRendererForm javascript code block
		/// </summary>
		/// <remarks>
		/// NORE: This is a very stupid little function, but it helps .GetValidationScript because this value is the only change between the base and Renderer.Form versions
		/// </remarks>
		/// <returns>String representing a JavaScript boolean value indicating if this is a Renderer.Form instance.</returns>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		protected override string GetIsRendererFormJavaScript() {
			return g_cISRENDERERFORM;
		}


	} //# public class InputCollection


} //# namespace Cn.Web
