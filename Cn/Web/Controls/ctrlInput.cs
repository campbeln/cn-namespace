/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Cn.Web.Inputs;
using Cn.Data;


namespace Cn.Web.Controls {

    ///########################################################################################################################
    /// <summary>
	/// Controls.Input class wraps the functionality of Inputs.InputData into a WebControl.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	[DefaultProperty("Text"),
		ToolboxData("<{0}:Input InputCollectionID=\"InputCollection\" type=\"text\" runat=\"server\" />")
	]
	public class Input : WebControl {
			//#### Declare the required private variables
		private InputData g_oInputData;
		private IInputCollectionControl g_oParentInputCollection;
		private string[] ga_sInitialValues;
		private string g_sAdditionalAttributes;
		private string g_sInputCollectionID;
		private string g_sInitialValue;
		private string g_sTableName;
		private string g_sColumnName;
		private bool g_bInitialValueIsExpression;
		private bool g_bInputDataProcessed;
		private bool g_bForceInitialValue;
		private enumInputTypes g_eInputType;

			//#### Declare the required enums
		#region enums
		private enum enumParentInputCollectionTypes {
			Unknown = -1,
			InputCollection = 1,
			Form = 2
		}
		#endregion

			//#### Declare the required private constants
		private const string g_cClassName = "Cn.Web.Controls.Input.";


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initilizes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		public Input() {
				//#### Init and default the global vars
			g_oInputData = new InputData("");
			g_oParentInputCollection = null;
			ga_sInitialValues = null;
			g_sAdditionalAttributes = "";
			g_sInputCollectionID = "";
			g_sInitialValue = "";
			g_bInitialValueIsExpression = false;
			g_bInputDataProcessed = false;
			g_bForceInitialValue = false;

				//#### We do not need the ViewState, so disable it
			base.EnableViewState = false;
		}


		//##########################################################################################
		//# Public Properties (WebControl-specific, not [directly] represented within InputData)
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the base control class related to this instance.
		/// </summary>
		/// <value>InputData object that manages this instance.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public InputData ControlManager {
			get { return g_oInputData; }
		}

//! to remove?
public AdditionalData AdditionalData {
	get { return g_oInputData.AdditionalData; }
}


		///############################################################
		/// <summary>
		/// Gets/sets the parent input collection control related to this instance.
		/// </summary>
		/// <remarks>
		/// If an <c>InputCollection</c> has not been explicitly set, the following order of operations is undertaken to automaticially determine the <c>Control</c> representing an <c>InputCollection</c>:
		/// <para/>1) <c>InputCollectionID</c> is used to find the developer specified control.
		/// <para/>2) The <c>Parent</c> control is used.
		/// <para/>3) The <c>Page</c>'s <c>Controls</c> are searched recursively, and the first control found of type <c>Controls.InputCollection</c> is used.
		/// <para/>4) The <c>Page</c>'s <c>Controls</c> are searched recursively, and the first control found of type <c>Controls.Form</c> is used.
		/// <para/>
		/// <para/>NOTE: If a <c>Control</c> is located, but is not of a type that represents an <c>InputCollection</c>, it is ignored and the next operation is undertaken to find a valid <c>InputCollection</c>-based control.
		/// <para/>NOTE: It is far more efficient for the developer to place <c>Cn:Input</c> server tags directly under a <c>Controls.InputCollection</c> or <c>Controls.Form</c>, or to set an <c>InputCollectionID</c> then it is to rely on the inbuild searching mechnism. Besides, only the first found <c>InputCollection</c>-based control is returned which may or may not represent the correct <c>InputCollection</c>.
		/// <para/>NOTE: This does not exactly serve the same purpose as <c>InputData.Parent</c>, as this referes to the control which itself holds a reference to the underlying <c>IInputCollection</c> object, as represented by <c>InputData.Parent</c> (so the control acts as a "man in the middle").
		/// </remarks>
		/// <value>InputData object that manages this instance.</value>
		///############################################################
		/// <LastUpdated>June 24, 2010</LastUpdated>
		public IInputCollectionControl InputCollection {
			get {
				Control[] a_oControls;
				int i;

					//#### If we haven't yet found our g_oParentInputCollection 
				if (g_oParentInputCollection == null) {
						//#### If we have a g_sInputCollectionID to look for
					if (! string.IsNullOrEmpty(g_sInputCollectionID)) {
							//#### Find all of the controls under our .Page's heirarchy that have an ID matching the g_sInputCollectionID
						a_oControls = Tools.FindControl(this.Page, g_sInputCollectionID, false);

							//#### Traverse the found a_oControls (if any)
						for (i = 0; i < a_oControls.Length; i++) {
								//#### If the current a_oControls is an IInputCollectionControl, (safely) cast it into our g_oParentInputCollection and fall from the loop
							if (a_oControls[i] is IInputCollectionControl) {
								g_oParentInputCollection = a_oControls[0] as IInputCollectionControl;
								break;
							}
						}
					}

						//#### If the g_sInputCollectionID could not be set into our g_oParentInputCollection (or there wasn't one to look for)
					if (g_oParentInputCollection == null) {
							//#### Pass the call into .FindRelatedInputCollectionControl, collecting it's return into our g_oParentInputCollection
						g_oParentInputCollection = Tools.FindRelatedInputCollectionControl(this);
					}
				}

					//#### Return the above determined g_oParentInputCollection to the caller
				return g_oParentInputCollection;
			}
			set {
				g_oParentInputCollection = value;

					//#### Since the g_oParentInputCollection has been reset, we need to re-.ProcessInputData
//! should now work with changes made to scMirror/etc?
//				ProcessInputData("InputCollection", true);
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the ID of the parent InputCollection control.
		/// </summary>
		/// <value>String representing the ID of the parent InputCollection control.</value>
		///############################################################
		/// <LastUpdated>March 10, 2010</LastUpdated>
		[Category("Misc"),
			Description("String representing the ID of the parent InputCollection control."),
			DefaultValue("")
		]
		public string InputCollectionID {
			get { return g_sInputCollectionID; }
			set {
				g_sInputCollectionID = value;

					//#### Reset the value of our g_oParentInputCollection so we force it to be recollected on next request
				g_oParentInputCollection = null;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the initial values of the input.
		/// </summary>
		/// <value>Array of Strings representing the initial values of the input.</value>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		[Category("Data"),
			Description("."),
			DefaultValue(""),
			Bindable(true)
		]
		public string[] InitialValues {
			get { return ga_sInitialValues; }
			set { ga_sInitialValues = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the initial value of the input.
		/// </summary>
		/// <value>String representing the initial value of the input.</value>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		[Category("Data"),
			Description("."),
			DefaultValue(""),
			Bindable(true)
		]
		public string InitialValue {
			get { return g_sInitialValue; }
			set { g_sInitialValue = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the additional attributes that will be added to the primary HTML element for this input.
		/// </summary>
		/// <value>String value representing the additional attributes that will be added to the primary HTML element for this input.</value>
		///############################################################
		/// <LastUpdated>July 14, 2010</LastUpdated>
		public string AdditionalAttributes {
			get { return g_sAdditionalAttributes; }
			set { g_sAdditionalAttributes = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing if the value of the input is always to be set to the initial value.
		/// </summary>
		/// <value>Boolean value representing if the value of the input is always to be set to the initial value.</value>
		///############################################################
		/// <LastUpdated>July 14, 2010</LastUpdated>
		public bool ForceInitialValue {
			get { return g_bForceInitialValue; }
			set { g_bForceInitialValue = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if the value of <c>InitialValue</c>/<c>InitialValues</c> is an expression.
		/// </summary>
		/// <remarks>
		/// If this is set to "true", the array of strings within <c>InitialValues</c>, or the string value specified within <c>InitialValue</c> will be interprated as a period-delimited class path into our <c>Parent</c>'s <c>DataSource</c>.
		/// </remarks>
		/// <value>Boolean value indicating if the value of <c>InitialValue</c>/<c>InitialValues</c> is an expression.</value>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public bool InitialValueIsExpression {
//! delete this logic?
			get { return g_bInitialValueIsExpression; }
			set { g_bInitialValueIsExpression = value; }
		}


		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input type to render.
		/// </summary>
		/// <returns>Enumeration representing the input type to render.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Appearance"),
			Description("."),
			DefaultValue("")
		]
		public enumInputTypes InputType {
			get { return g_eInputType; }
			set { g_eInputType = value; }
		}


		//##########################################################################################
		//# Public Properties (represented within InputData)
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the collection of values for the input represented by this instance.
		/// </summary>
		/// <remarks>
		/// NOTE: <c>Input</c>s defined as <c>cnBoolean</c>s always return values based on <see cref="Cn.Data.Tools.MakeBooleanInteger">MakeBooleanInteger</see>, where zero equates to false and non-zero equates to true.
		/// </remarks>
		/// <value>String array where each element represents a single value of the collection of values for the input represented by this instance.</value>
		///############################################################
		/// <LastUpdated>April 7, 2010</LastUpdated>
		[Category("Appearance"),
			Description("Current values of the input."),
			DefaultValue(null),
			Bindable(true)
		]
		public string[] Values {
			get { return g_oParentInputCollection.Inputs.Inputs(g_oInputData.InputAlias).Values; }
			set { g_oInputData.Values = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the value(s) of the input represented by this instance.
		/// </summary>
		/// <remarks>
		/// NOTE: If the input has multiple values, they will be returned as a <c>Cn.Tools.MultiValueString</c> for a <c>Get</c> call, and will be reset to the single provided string value for a <c>Set</c> call.
		/// </remarks>
		/// <value>String representing the value(s) of the input represented by this instance (serialized as a MultiValueString is multiple values exist).</value>
		///############################################################
		/// <LastUpdated>April 7, 2010</LastUpdated>
		[Category("Appearance"),
			Description("String representing the value(s) of the input represented by this instance (serialized as a MultiValueString is multiple values exist)."),
			DefaultValue(""),
			Bindable(true)
		]
		public string Value {
			get { return g_oParentInputCollection.Inputs.Inputs(g_oInputData.InputAlias).Value; }
			set { g_oInputData.Value = value; }
		}

//! ErrorMessage?

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's default value.
		/// </summary>
		/// <returns>String representing the input's default value.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Data Definition"),
			Description("String representing the input's default value."),
			DefaultValue(""),
			Bindable(true)
		]
		public string DefaultValue {
			get { return g_oInputData.DefaultValue; }
			set { g_oInputData.DefaultValue = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's minimum numeric value.
		/// </summary>
		/// <returns>String representing the input's minimum numeric value.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Data Definition"),
			Description("String representing the input's minimum numeric value."),
			DefaultValue(""),
			Bindable(true)
		]
		public string MinimumNumericValue {
			get { return g_oInputData.MinimumNumericValue; }
			set { g_oInputData.MinimumNumericValue = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's maximum numeric value.
		/// </summary>
		/// <returns>String representing the input's maximum numeric value.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Data Definition"),
			Description("String representing the column's maximum numeric value."),
			DefaultValue(""),
			Bindable(true)
		]
		public string MaximumNumericValue {
			get { return g_oInputData.MaximumNumericValue; }
			set { g_oInputData.MaximumNumericValue = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's maximum character length.
		/// </summary>
		/// <returns>Integer representing the input's maximum character length.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Data Definition"),
			Description("Integer representing the column's maximum character length."),
			DefaultValue(-1),
			Bindable(true)
		]
		public int MaximumCharacterLength {
			get { return g_oInputData.MaximumCharacterLength; }
			set { g_oInputData.MaximumCharacterLength = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's numeric precision.
		/// </summary>
		/// <returns>Integer representing the input's numeric precision.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Data Definition"),
			Description("Integer representing the column's numeric precision."),
			DefaultValue(-1),
			Bindable(true)
		]
		public int NumericPrecision {
			get { return g_oInputData.NumericPrecision; }
			set { g_oInputData.NumericPrecision = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's numeric scale.
		/// </summary>
		/// <returns>Integer representing the input's numeric scale.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Data Definition"),
			Description("Integer representing the column's numeric scale."),
			DefaultValue(-1),
			Bindable(true)
		]
		public int NumericScale {
			get { return g_oInputData.NumericScale; }
			set { g_oInputData.NumericScale = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's form processing requirements.
		/// </summary>
		/// <returns>Enumeration representing the input's form processing requirements.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> on a <paramref>sColumnName</paramref> also defind as not nullable.</exception>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Data Definition"),
			Description("Enumeration representing the input's form processing requirements."),
			DefaultValue(enumSaveTypes.cnInsertIfPresent)
		]
		public enumSaveTypes SaveType {
			get { return g_oInputData.SaveType; }
			set { g_oInputData.SaveType = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's data type.
		/// </summary>
		/// <returns>Enumeration representing the input's data type.</returns>
		///############################################################
		/// <LastUpdated>February 9, 2010</LastUpdated>
		[Category("Data Definition"),
			Description("Enumeration representing the input's data type."),
			DefaultValue(MetaData.enumDataTypes.cnUnknown)
		]
		public MetaData.enumDataTypes DataType {
			get { return g_oInputData.DataType; }
			set { g_oInputData.DataType = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's stored value type.
		/// </summary>
		/// <returns>Enumeration representing the input's stored value type.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Data Definition"),
			Description("Enumeration representing the input's stored value type."),
			DefaultValue(MetaData.enumValueTypes.cnSingleValue)
		]
		public MetaData.enumValueTypes ValueType {
			get { return g_oInputData.ValueType; }
			set { g_oInputData.ValueType = value; }
		}

//! ErrorType?
//! IsValueFromForm?

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if the column permits null values.
		/// </summary>
		/// <returns>Boolean value indicating if the column permits null values.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> on a <paramref>sColumnName</paramref> also defind as not nullable.</exception>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Data Definition"),
			Description("Boolean value indicating if the column permits null values."),
			DefaultValue(true)
		]
		public bool IsNullable {
			get { return g_oInputData.IsNullable; }
			set { g_oInputData.IsNullable = value; }
		}


		//##########################################################################################
		//# Public Properties (represented within InputData as Read-Only Properties)
		//##########################################################################################
//! AdditionalData?

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the HTML input's base name.
		/// </summary>
		/// <remarks>
		/// NOTE: The related property of InputData.InputAlias is read-only.
		/// </remarks>
		/// <returns>String representing the HTML input's unique base name.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("Misc"),
			Description("The base HTML DOM identifier string for the input (known internally as 'InputAlias')."),
			DefaultValue("")
		]
		public override string ID {
			get { return base.ID; }
			set { base.ID = value; }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the input's source table name.
		/// </summary>
		/// <remarks>
		/// NOTE: The related property of InputData.InputAlias is read-only.
		/// </remarks>
		/// <returns>String representing the input's source table name.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("DataSource Awareness"),
			Description("String representing the input's source table name."),
			DefaultValue("")
		]
		public string TableName {
			get { return g_sTableName; }
			set { g_sTableName = value; }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the input's source column name.
		/// </summary>
		/// <remarks>
		/// NOTE: The related property of InputData.InputAlias is read-only.
		/// </remarks>
		/// <returns>String representing the input's source column name.</returns>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Category("DataSource Awareness"),
			Description("String representing the input's source column name."),
			DefaultValue("")
		]
		public string ColumnName {
			get { return g_sColumnName; }
			set { g_sColumnName = value; }
		}

//! SourceRecordIndex?
//! IsAttachedToDataSource?
//! IsValid?

		//##########################################################################################
		//# Protected Override Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Raises the System.Web.UI.Control.Init event.
		/// </summary>
		/// <param name="e">An System.EventArgs object that contains the event data.</param>
		///############################################################
		/// <LastUpdated>June 24, 2010</LastUpdated>
		protected override void OnLoad(EventArgs e) {
				//#### Pass the call off to the base implementation, then call .ProcessInputData
				//####     NOTE: OnInit is too early, as the attributes values are not yet set. OnPreRender is too late, as the <c>Form</c> has already been iterated past the first <c>RenderedRecordCount</c>.
			base.OnLoad(e);
			ProcessInputData("OnLoad", false);
		}

//! neek
//		protected override void TrackViewState() {
//			ProcessInputData("TrackViewState", false);
//			base.TrackViewState();
//		}
//		protected override void EnsureChildControls() {
//			ProcessInputData("EnsureChildControls", false);
//			base.EnsureChildControls();
//		}
//		protected override void CreateChildControls() {
//			ProcessInputData("CreateChildControls", false);
//			base.CreateChildControls();
//		}

		///############################################################
		/// <summary>
		/// Raises the System.Web.UI.Control.DataBinding event.
		/// </summary>
		/// <param name="e">An System.EventArgs object that contains the event data.</param>
		///############################################################
		/// <LastUpdated>July 9, 2010</LastUpdated>
		protected override void OnDataBinding(EventArgs e) {
				//#### Pass the call off to the base implementation, then call .ProcessInputData
			base.OnDataBinding(e);
			ProcessInputData("OnDataBinding", true);
		}

		///############################################################
		/// <summary>
		/// Writes out the XHTML/DHTML code necessary to render this control.
		/// </summary>
		/// <param name="output">HtmlTextWriter object as automatically provided by the host ASPX page.</param>
		///############################################################
        /// <LastUpdated>April 14, 2010</LastUpdated>
        protected override void Render(HtmlTextWriter output) {
				//#### If we are supposed to be .Visible
			if (Visible) {
//				string sAttributes = Web.Controls.Tools.GetInputAttributes(this);
				IInputCollectionControl oInputCollectionControl = InputCollection;
				HTMLTextToString oStringWriter = new HTMLTextToString();
				string sAttributes;

					//#### .Render our .Attributes to the oStringWriter, then recollect them into our own sAttributes while appending any g_sAdditionalAttributes
				Attributes.Render(oStringWriter.HTMLTextWriter);
				sAttributes = oStringWriter.ToString() + " " + g_sAdditionalAttributes;

					//#### If the g_bInitialValueIsExpression and we are .IsDataBound
				if (g_bInitialValueIsExpression && oInputCollectionControl.IsDataBound) {
						//#### If the ga_sInitialValues is null or empty, .Split the value within g_sInitialValue
					if (ga_sInitialValues == null || ga_sInitialValues.Length == 0) {
						Data.Tools.MakeString(g_sInitialValue, "").Split('.');
					}

						//#### If we have a valid oInputCollectionControl and can successfully .GetPropertyValueAs the value from the g_oDataSource into our g_sInitialValue
					if (oInputCollectionControl != null &&
						Data.Mirror.GetPropertyValueAs(oInputCollectionControl.DataSource, ga_sInitialValues, out g_sInitialValue)
					) {
						DoRenderInput(sAttributes, output);
					}
						//#### Else we could not locate the Expression within the .DataSource, so raise the error
					else {
//! raise error
					}
				}
					//#### Else the g_bInitialValueIs(not an)Expression, so just .DoRender(the)Input
				else {
					DoRenderInput(sAttributes, output);
				}
			}
		}


		//##########################################################################################
		//# Private Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Determines the type of the Parent this instance resides under.
		/// </summary>
		/// <value>Enumeration representing the type of the Parent this instance resides under (if any).</value>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		private enumParentInputCollectionTypes ParentInputCollectionType() {
			IInputCollectionControl oParent = InputCollection;
			Type oType;
			enumParentInputCollectionTypes eReturn = enumParentInputCollectionTypes.Unknown;

				//#### If the oParent was successfully collected above, determine it's oType
			if (oParent != null) {
				oType = oParent.GetType();

					//#### If this is an .InputCollection, reset the eReturn value accordingly
				if (oType == typeof(Controls.InputCollection)) {
					eReturn = enumParentInputCollectionTypes.InputCollection;
				}
					//#### Else if this is an .Form, reset the eReturn value accordingly
				else if (oType == typeof(Controls.Form)) {
					eReturn = enumParentInputCollectionTypes.Form;
				}
			}

				//#### Return the above determined eReturn value to the caller
			return eReturn;
		}

		///############################################################
		/// <summary>
		/// Pre-processes the global <c>g_oInputData</c> variable, loading in the g_sTableName/g_sColumnName if they have been defined by the developer.
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="bForceProcessing">Boolean value representing if we are to force processing.</param>
		/// <remarks>
		/// This double-handeling is necessary as the <c>g_oInputData</c> is origionally setup in our Constructor as a non-.IsAttachedToDatasource input. So if the developer has defined an .IsAttachedToDatasource input, a re-definition of the <c>g_oInputData</c> is required before we can fulfill the request.
		/// </remarks>
		///############################################################
		/// <LastUpdated>June 24, 2010</LastUpdated>
		private void ProcessInputData(string sFunction, bool bForceProcessing) {
			InputData oTempInputData;

				//#### If the g_bInputData(hasn't been)Processed or we are to bForce(the)Processing
			if (! g_bInputDataProcessed || bForceProcessing) {
					//#### Flip g_bInputDataProcessed so we aren't re-run
				g_bInputDataProcessed = true;

					//#### If the developer set a g_sTableName or g_sColumnName, we need to reset g_oInputData to an .IsAttachedToDatasource input
				if (! string.IsNullOrEmpty(g_sTableName) || ! string.IsNullOrEmpty(g_sColumnName)) {
						//#### Setup the oTempInputData that .IsAttachedToDatasource
					oTempInputData = new InputData(
						base.ID,
						g_sTableName,
						g_sColumnName,
						g_oInputData.SaveType,
						g_oInputData.ValueType,
						g_oInputData.AdditionalData
					);
				}
					//#### Else setup the oTempInputData with the base.ID
				else {
					oTempInputData = new InputData(
						base.ID,
						g_oInputData.SaveType,
						g_oInputData.DataType,
						g_oInputData.ValueType,
						g_oInputData.DefaultValue,
						g_oInputData.IsNullable,
						g_oInputData.MaximumCharacterLength,
						g_oInputData.MinimumNumericValue,
						g_oInputData.MaximumNumericValue,
						g_oInputData.NumericPrecision,
						g_oInputData.NumericScale,
						g_oInputData.AdditionalData
					);
				}

					//#### .Map the properties from the developer set g_oInputData into the oTempInputData, then reset the g_oInputData with the updated InputData
					//####     NOTE: It would be more efficient to set the properties ourselves below (rather then using .Mirror's reflection functions), but it would need to be manually synced with any changes within InputData, so in the interest of maintiance .Mirror is used below (yea, I know... laziness ;)
//! maybe have a copy function within InputData to accomplish this?
				Data.Mirror.MapAs(g_oInputData, oTempInputData);
oTempInputData.Parent = g_oInputData.Parent;
				g_oInputData = oTempInputData;

					//#### If the g_oInputData.Parent has not yet been setup
				if (g_oInputData.Parent == null) {
						//#### Determine the .ParentInputCollectionType and process accordingly
						//####     NOTE: We need to set the .Parent within our g_oInputData because on .Add our g_oInputData is deep copied into the .InputCollection (and therefore when it's .Parent is set, our g_oInputData's .Parent remains blank)
					switch (ParentInputCollectionType()) {
						case enumParentInputCollectionTypes.InputCollection: {
							g_oInputData.Parent = ((Controls.InputCollection)InputCollection).ControlManager;
							g_oInputData.Parent.Add(g_oInputData);
							break;
						}
						case enumParentInputCollectionTypes.Form: {
							Renderer.Form oForm = ((Controls.Form)InputCollection).ControlManager;
							g_oInputData.Parent = ((Controls.Form)InputCollection).ControlManager.InputCollection;

								//#### If this is the first .RenderedRecordCount, .Add our g_oInputData
								//####     NOTE: 
//!							if ((oForm.IsPostBack && oForm.RenderedRecordCount == 0) ||
//								(! oForm.IsPostBack && oForm.RenderedRecordCount == 1)
//							) {
							if (! g_oInputData.Parent.Exists(g_oInputData.InputAlias)) {
								g_oInputData.Parent.Add(g_oInputData);
							}
							break;
						}
					}
				}

					//#### If the g_oInputData.Parent was not successfully collected above, raise the error
				if (g_oInputData.Parent == null) {
//! need new CnException
throw new Exception(g_cClassName + sFunction + ": Cannot locate the InputCollection");
				}
			}
		}

		///############################################################
		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="output">HtmlTextWriter object as automatically provided by the host ASPX page.</param>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		private void DoRenderInput(string sAttributes, HtmlTextWriter output) {
				//#### Determine the .ParentInputCollectionType and render the input accordingly
			switch (ParentInputCollectionType()) {
				case enumParentInputCollectionTypes.Form: {
					((Form)InputCollection).RenderInput(ID, g_eInputType, g_sInitialValue, ga_sInitialValues, g_bForceInitialValue, sAttributes, output);
					break;
				}
				case enumParentInputCollectionTypes.InputCollection: {
					((InputCollection)InputCollection).RenderInput(ID, g_eInputType, g_sInitialValue, ga_sInitialValues, g_bForceInitialValue, sAttributes, output);
					break;
				}
			}
		}

/*
		protected virtual IEnumerable ResolveDataSource(object oDataSource) {
			IEnumerable oReturn = null;
			IListSource oListSource = oDataSource as IListSource;
			ITypedList oTypedList;
			IList oList;
			PropertyDescriptorCollection oProperties;
			PropertyDescriptor oProperty = null;

				//#### If we were passed a valid oDataSource to query
			if (oDataSource != null) {
					//#### If the oListSource was successfully set above
				if (oListSource != null) {
					oList = oListSource.GetList();

						//#### If the oListSource .Contains(a)ListCollection
					if (oListSource.ContainsListCollection) {
						oTypedList = oList as ITypedList;

							//#### 
						if (oTypedList != null) {
								//#### 
							oProperties = oTypedList.GetItemProperties(new PropertyDescriptor[0]);

								//#### 
							if ((oProperties == null) || (oProperties.Count == 0)) {
								throw new Exception("The selected data source did not contain any data members to bind to.");
							}
								//#### 
							else {
								if (Rules.IsNullOrEmpty(this.dataMember)) {
									// bind to first object in collection if no member specified (eg. first DataTable in Dataset)
									oProperty = oProperties[0];
								}
								else {
									oProperty = oProperties.Find(this.dataMember, true);
								}

								if (oProperty != null) {
									object listObject = oList[0];
									object memberObject = oProperty.GetValue(listObject);
									if (memberObject != null && memberObject is IEnumerable) {
										return (IEnumerable)memberObject;
									}
								}

								throw new Exception("A list corresponding to the selected DataMember was not found.");
							}
						}				
					}
						//#### Else the oListSource is not a list of collections (i.e. a DataTable), so set our oReturn value to the oList
					else {
						oReturn = ((IEnumerable)oList);
					}
				}

					//#### 
				if (oDataSource is IEnumerable) {
					oReturn = ((IEnumerable)oDataSource);
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}
*/


/*
public class Native {
	private Input g_oParentInput;
	private TextBox g_oTextBox;
	private DropDownList g_oSelect;
	private bool g_bUsingNativeControls;

	public enum enumNativeControls : int {
		RadioButtonList = 1,
		CheckBoxList = 2,
		FileUpload = 3,
		TextBox = 4,
		Select = 5
	}


	public Native(Input oParentInput) {
			//#### 
		g_oParentInput = oParentInput;
		g_oTextBox = null;
		g_oSelect = null;
		g_bUsingNativeControls = false;
	}


internal void SyncControlProperties() {
}


	public TextBox TextBox {
		get {
				//#### If the g_oTextBox has not yet been setup
			if (g_oTextBox == null) {
				enumInputTypes eInputType;

					//#### Collect the eInputType from our g_oParentInput's .Type
				eInputType = Data.Tools.MakeEnum("cn" + g_oParentInput.InputType, enumInputTypes.cnDefaultInput);

					//#### Determine eInputType and process accordingly
				switch (eInputType) {
					case enumInputTypes.cnComboBox:
					case enumInputTypes.cnDate:
					case enumInputTypes.cnDateTime:
//!					case enumInputTypes.cnDefaultInput:
case enumInputTypes.cnHTMLEditor:
case enumInputTypes.cnPassword:
					case enumInputTypes.cnText:
					case enumInputTypes.cnTextarea:
					case enumInputTypes.cnTime: {
							//#### Ensure that the g_bUsingNativeControls flag is flipped to true
						g_bUsingNativeControls = true;

							//#### Init the g_oTextBox, the pre-populate it's values with the data from the g_oParentInput
						g_oTextBox = new TextBox();
//! is .Value correct here?
						g_oTextBox.Text = g_oParentInput.Value;
						g_oTextBox.ID = g_oParentInput.ID;
//!						g_oTextBox.Enabled = g_oParentInput.Settings.IsReadOnly;
						break;
					}
				}
			}

				//#### Return the above determined g_oTextBox to the caller
			return g_oTextBox;
		}
	}

	public DropDownList Select {
		get {
				//#### If the g_oSelect has not yet been setup
			if (g_oSelect == null) {
				enumInputTypes eInputType;

					//#### Collect the eInputType from our g_oParentInput's .Type
				eInputType = Data.Tools.MakeEnum("cn" + g_oParentInput.InputType, enumInputTypes.cnDefaultInput);

					//#### Determine the eInputType and process accordingly
				switch (eInputType) {
					case enumInputTypes.cnComboBox:
//!					case enumInputTypes.cnDefaultInput:
					case enumInputTypes.cnMultiSelect:
					case enumInputTypes.cnSelect: {
							//#### Ensure that the g_bUsingNativeControls flag is flipped to true
						g_bUsingNativeControls = true;

							//#### Init the g_oTextBox, the pre-populate it's values with the data from the g_oParentInput
						g_oSelect = new DropDownList();
//!
						break;
					}
				}
			}

				//#### Return the above determined g_oSelect to the caller
			return g_oSelect;
		}
	}


	public bool Exists(enumNativeControls eNativeControl) {
		bool bReturn = false;

			//#### Determine the value of the passed eNativeControl, setting the sReturn value accordingly
		switch (eNativeControl) {
			case enumNativeControls.RadioButtonList: {
//				bReturn = g_bAccessedRadioButtonList;
				break;
			}
			case enumNativeControls.CheckBoxList: {
//				bReturn = g_bAccessedCheckBoxList;
				break;
			}
			case enumNativeControls.FileUpload: {
//				bReturn = g_bAccessedFileUpload;
				break;
			}
			case enumNativeControls.TextBox: {
				bReturn = (g_oTextBox != null);
				break;
			}
			case enumNativeControls.Select: {
				bReturn = (g_oSelect != null);
				break;
			}
		}

			//#### Return the above determined bReturn value to the caller
		return bReturn;
	}
}
*/

	} //# public class Input


} //# namespace Cn.Web.Controls
