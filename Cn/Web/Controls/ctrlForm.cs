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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cn.Data;
using Cn.Web.Inputs;
using Cn.Web.Renderer;


namespace Cn.Web.Controls {

	///########################################################################################################################
	/// <summary>
	/// Controls.Form class wraps the functionality of Renderer.Form into a WebControl.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	[ToolboxData("<{0}:Form runat=\"server\"></{0}Form>")]
	public class Form : Controls.List, IInputCollectionControl {
	#region Form
			//#### Declare the required private variables
		List<Controls.Input> gl_oInputs;
		private Control_Form g_oForm;


		//##########################################################################################
		//# Class Constructor
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>May 31, 2010</LastUpdated>
		public Form() {
			    //#### Init our own global private variables
			    //####     NOTE: We declare g_oForm in addition to g_oListOrForm because the IControlListForm interface does not include the Form-only interfaes (duh! =), so g_oForm is used for the Form-specific interfaces.
			gl_oInputs = new List<Input>();
			g_oForm = new Control_Form(new Settings.Current(), this);
			g_oListOrForm = ((Renderer.List)g_oForm);

			    //#### Pass the call off to .DoReset
			DoReset(false);
		}


		//##########################################################################################
		//# Private New Functions to hide the unwanted functions/properties of our base class
		//##########################################################################################
// ReSharper disable UnusedMember.Local
		private new string AccessKey() { return base.AccessKey; }
		private new System.Drawing.Color BackColor() { return base.BackColor; }
		private new System.Drawing.Color BorderColor() { return base.BorderColor; }
		private new BorderStyle BorderStyle() { return base.BorderStyle; }
		private new Unit BorderWidth() { return base.BorderWidth; }
		private new Style ControlStyle() { return base.ControlStyle; }
		private new bool ControlStyleCreated() { return base.ControlStyleCreated; }
		private new void CopyBaseAttributes(WebControl controlSrc) { base.CopyBaseAttributes(controlSrc); }
		private new string CssClass() { return base.CssClass; }
		private new bool EnableTheming() { return base.EnableTheming; }
//private new bool EnableViewState() { return base.EnableViewState; }
		private new void Focus() { base.Focus(); }
		private new FontInfo Font() { return base.Font; }
		private new System.Drawing.Color ForeColor() { return base.ForeColor; }
		private new Unit Height() { return base.Height; }
		private new void MergeStyle(Style s) { base.MergeStyle(s); }
//private new void RenderBeginTag(HtmlTextWriter writer) { base.RenderBeginTag(writer); }
//private new void RenderEndTag(HtmlTextWriter writer) { base.RenderEndTag(writer); }
		private new string SkinID() { return base.SkinID; }
		private new CssStyleCollection Style() { return base.Style; }
		private new short TabIndex() { return base.TabIndex; }
		private new string ToolTip() { return base.ToolTip; }
		private new Unit Width() { return base.Width; }
// ReSharper restore UnusedMember.Local


		//##########################################################################################
		//# Class Events
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Enables the generation of custom HTML controls.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public event GenerateHTMLEventHandler GenerateHTML;

		///############################################################
		/// <summary>
		/// Delegate signature defintion for the <c>Form.ValidateRecord</c> events.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public delegate Renderer.Form.RecordValidater ValidateRecordEventHandler(object sender, ValidateRecordEventArgs e);

			///############################################################
			/// <summary>
			/// Validates the current record during form processing.
			/// </summary>
			///############################################################
			/// <LastUpdated>February 10, 2010</LastUpdated>
			public event ValidateRecordEventHandler ValidateRecord;
			internal bool RaiseValidateRecord(ValidateRecordEventArgs e, out Renderer.Form.RecordValidater oRecordValidator) {
				bool bReturn = (ValidateRecord != null);

					//#### Init the passed oRecordValidator
					//####     NOTE: We set the oRecordValidator to null so as to ensure the default logic is used within Renderer.Form should it need to be. Else we would the the potentional to have two definitions of a "default" oRecordValidator, here and wihtin the base implementation of .ValidateRecord.
				oRecordValidator = null;

					//#### If the developer has defined delegate(s), raise the event (collecting the returned oRecordValidator)
					//####     NOTE: The oRecordValidator is passed as an out parameter, so the assignment below is "passed" back to the caller
				if (bReturn) {
					oRecordValidator = ValidateRecord(this, e);
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}


		///############################################################
		/// <summary>
		/// Delegate signature defintion for the <c>Form.SubmitResults</c> events.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public delegate void SubmitResultsEventHandler(object sender, SubmitResultsEventArgs e);

			///############################################################
			/// <summary>
			/// Submits the successfully collected and validated data into the data source.
			/// </summary>
			/// <remarks>
			/// A delegate definition is required for this event.
			/// </remarks>
			///############################################################
			/// <LastUpdated>February 10, 2010</LastUpdated>
			public event SubmitResultsEventHandler SubmitResults;
			internal void RaiseSubmitResults(SubmitResultsEventArgs e) {
					//#### If the developer has defined delegate(s), raise the event
				if (SubmitResults != null) {
					SubmitResults(this, e);
				}
					//#### Else the required delegate(s) were not defined, so raise the error
				else {
//! should use a CnException
throw new NotImplementedException();
				}
			}


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets a value representing the unique ID of this instance.
		/// </summary>
		/// <remarks>
		/// This value is copied into the underlying <c>Renderer.Form</c>'s <c>Name</c> property which is used to ensure that any rendered record tracker names are themselves unique.
		/// </remarks>
		/// <value>String representing the unique ID of this instance.</value>
		///############################################################
		/// <LastUpdated>June 1, 2010</LastUpdated>
		public override string ID {
			get { return base.ID; }
			set {
				base.ID = value;
				g_oForm.Name = value;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the number of new record forms to render.
		/// </summary>
		/// <value>Integer representing the number of new record forms to render.</value>
		///############################################################
		/// <LastUpdated>June 24, 2010</LastUpdated>
		public int NewRecords {
			get { return g_oForm.NewRecords; }
			set { g_oForm.NewRecords = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if new records are to be rendered before existing records.
		/// </summary>
		/// <value>Boolean value indicating if new records are to be rendered before existing records.</value>
		///############################################################
		/// <LastUpdated>February 9, 2010</LastUpdated>
		public bool NewRecordsRenderedFirst {
			get { return g_oForm.NewRecordsRenderedFirst; }
			set { g_oForm.NewRecordsRenderedFirst = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if we are to track record changes.
		/// </summary>
		/// <remarks>
		/// This function signals the <c>Form</c> to calculate MD5 hashs of the original and submitted records. If the hashs differ, then the end user made changes to the record and it therefore needs to be updated. If the hases match, then the end user didn't make any changes to the record, so there is no need to update the record.
		/// <para/>This calculation's results are made available to the developer via the <c>bRecordHasChanged</c> argument of the <c>RendererForm</c>'s <c>ValidateRecord</c> function. If this functionality is not enabled (i.e. - the value of this property is "false"), then the <c>bRecordHasChanged</c> argument of the <c>RendererForm</c>'s <c>ValidateRecord</c> function is always "true".
		/// </remarks>
		/// <value>Boolean value indicating if we are to track record changes.</value>
		///############################################################
		/// <LastUpdated>February 9, 2010</LastUpdated>
		public bool TrackRecordChanges {
			get { return g_oForm.TrackRecordChanges; }
			set { g_oForm.TrackRecordChanges = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if we are to process the current form.
		/// </summary>
		/// <value>Boolean value indicating if we are to process the current form.</value>
		///############################################################
		/// <LastUpdated>February 9, 2010</LastUpdated>
		public bool IsPostBack {
			get { return g_oForm.IsPostBack; }
			set { g_oForm.IsPostBack = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if the <c>Form</c> is to be rendered as read only.
		/// </summary>
		/// <value>Boolean value indicating if the <c>Form</c> is to be rendered as read only.</value>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public bool IsReadOnly {
			get { return g_oForm.Settings.IsReadOnly; }
			set { g_oForm.Settings.IsReadOnly = value; }
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
		/// <LastUpdated>February 11, 2010</LastUpdated>
		[Bindable(true),
			Category("Behavior"),
			DefaultValue("true")
		]
		public bool DisplayErrorMessagesViaJavaScript {
			get { return g_oForm.InputCollection.DisplayErrorMessagesViaJavaScript; }
			set { g_oForm.InputCollection.DisplayErrorMessagesViaJavaScript = value; }
		}


		//##########################################################################################
		//# Public Read-Only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the base control class related to this instance.
		/// </summary>
		/// <value>Renderer.Form object that manages this instance.</value>
		///############################################################
		/// <LastUpdated>February 9, 2010</LastUpdated>
		public new Renderer.Form ControlManager {
			get { return g_oForm; }
		}

		///############################################################
		/// <summary>
		/// Gets a collection of inputs associated with this instance.
		/// </summary>
		/// <value>List of Controls.Input objects that represent the collection of inputs associated with this instance.</value>
		///############################################################
		/// <LastUpdated>April 7, 2010</LastUpdated>
		public List<Controls.Input> InputControls {
			get { return gl_oInputs; }
		}

		///############################################################
		/// <summary>
		/// Gets the base Input Collection class related to this instance.
		/// </summary>
		/// <value>InputCollection object that manages this instance.</value>
		///############################################################
		/// <LastUpdated>April 7, 2010</LastUpdated>
		public Inputs.IInputCollection Inputs {
			get { return g_oForm.InputCollection; }
		}


		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Binds a data source to the invoked server control and all its child controls.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public override void DataBind() {
				//#### Flip the value of g_bIsDataBound (so .DoDataBind called during .Render), then call our .base implementation
			g_bIsDataBound = true;
			base.DataBind();
		}


		//##########################################################################################
		//# Protected/Internal Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preperation for posting back or rendering.
		/// </summary>
		///############################################################
		/// <LastUpdated>May 31, 2010</LastUpdated>
		protected override void CreateChildControls() {
				//#### If this g_bIs(a)PostBack, we need to ensure that all the templates are pre-generated to ensure the .Inputs exist as the developer is expecting
			if (g_oForm.IsPostBack) {
					//#### Reset the g_iTemplateIndex in prep. for rendering below
				g_iTemplateIndex = 0;

					//#### 
//! inefficient as hell, need better solution(?)
//! Need same logic in List? Dev has less control over when Inputs are defined, so may be best to do it in the base List
				DoCreateChildControl(enumPageSections.cnHeader, -1, null);
				DoCreateChildControl(enumPageSections.cnDetailHeader, -1, null);
				DoCreateChildControl(enumPageSections.cnDetail, -1, g_oDataSourceEnumerator);
				if (g_oForm.NewRecordCount > 0) {
					g_oForm.IsNewRecord = true;
					DoCreateChildControl(enumPageSections.cnDetail_NewForm, -1, g_oDataSourceEnumerator);
					g_oForm.IsNewRecord = false;
				}
				DoCreateChildControl(enumPageSections.cnMissingRecord, -1, g_oDataSourceEnumerator);
				DoCreateChildControl(enumPageSections.cnDetailFooter, -1, null);
				DoCreateChildControl(enumPageSections.cnNoResults, -1, null);
				DoCreateChildControl(enumPageSections.cnFooter, -1, null);
			}

				//#### Pass the call off to our base class's .DoCollectResults
			base.CreateChildControls();
		}

/*
protected override void DoCreateDefaultChildControls() {
	ITemplate oTemplate;

		//#### Call our .base inplementation (as the only difference between a Renderer.List and a Renderer.Form are .cnDetail_NewForm's, which are handeled below)
	base.DoCreateDefaultChildControls();

		//#### If we have .NewRecordCount's to render
	if (g_oForm.NewRecordCount > 0) {
			//#### Flip .IsNewRecord to true
		g_oForm.IsNewRecord = true;

			//#### Collect the oTemplate for a .cnDetail_NewForm, .Instantiate('ing)In the .InstantiatedContainer (if there is a valid oTemplate)
		oTemplate = GetTemplate(enumPageSections.cnDetail_NewForm);
		if (oTemplate != null) {
			g_oTemplates[enumPageSections.cnDetail_NewForm].InstantiatedContainer = new TemplateContainer(enumPageSections.cnDetail_NewForm);
			oTemplate.InstantiateIn(g_oTemplates[enumPageSections.cnDetail_NewForm].InstantiatedContainer);
			g_oTemplates[enumPageSections.cnDetail_NewForm].BaseTemplate = oTemplate;
		}

			//#### Un-flip .IsNewRecord to false
		g_oForm.IsNewRecord = false;
	}
}
*/

		///############################################################
		/// <summary>
		/// Adds an input into the collection.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: <c>Input</c>s defined as <c>cnBoolean</c>s always return values based on <see cref="Cn.Data.Tools.MakeBooleanInteger">MakeBooleanInteger</see>, where zero equates to false and non-zero equates to true.
		/// <para/>NOTE: If you request that SQL statements be auto-generated in the return value for <see cref='Cn.Web.Renderer.Form.ValidateRecord'>ValidateRecord</see>, you are not permitted to define an input alias that contains the <see cref='Cn.Configuration.Settings.PrimaryDelimiter'>PrimaryDelimiter</see>.
		/// </remarks>
		/// <param name="oInputControl">Controls.Input representing the Input control to add into this instance.</param>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		internal void Add(Controls.Input oInputControl) {
				//#### .Add the passed oInputControl into the underlying g_oInputCollection and then into our gl_oInputs
			g_oForm.InputCollection.Add(oInputControl.ControlManager);
			gl_oInputs.Add(oInputControl);
		}

		///############################################################
		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <remarks>
		/// NOTE: This function will ignore any non-existant <c>sInputAlias</c>'s.
		/// </remarks>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="eInputType">Enumeration representing the input type to render.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="a_sInitialValues">Array of strings where each element represents an initial value of the input.</param>
		/// <param name="bForceInitialValue">Boolean value representing if the value of the input is always to be set to <paramref name="sInitialValue"/>/<paramref name="a_sInitialValues"/>.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="writer">HtmlTextWriter object as automatically provided by the host ASPX page.</param>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		internal void RenderInput(string sInputAlias, enumInputTypes eInputType, string sInitialValue, string[] a_sInitialValues, bool bForceInitialValue, string sAttributes, HtmlTextWriter writer) {
				//#### If this is a .cnCustom control (as .DoRenderInput was unable to do the render itself)
				//####     NOTE: An event is required for the control as there is no other way not to call .Parent's .Render function, so this hook/event allows the developer to change the .Render if they so choose.
			if (! InputCollectionControlCommon.DoRenderInput(sInputAlias, eInputType, sInitialValue, a_sInitialValues, bForceInitialValue, sAttributes, writer, g_oForm.InputCollection)) {
					//#### If the developer has defined delegate(s), raise the .GenerateHTML event
				if (GenerateHTML != null) {
					writer.Write(GenerateHTML(this, new GenerateHTMLEventArgs(sInputAlias, eInputType, sInitialValue, a_sInitialValues, sAttributes)));
				}
					//#### Else the required delegate(s) were not defined, so raise the error
				else {
//! should use a CnException
throw new NotImplementedException();
				}
			}
		}
	#endregion


		///########################################################################################################################
		/// <summary>
		/// Renderer.Form class implementation for the Control.Form WebControl.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview></LastFullCodeReview>
		private class Control_Form : Renderer.Form, Control_ListOrForm {
				//#### Declare the required private variables
			private Controls.Form g_oWebControl;
			private IEnumerator g_oDataSourceEnumerator;
			private bool g_bCreateChildControls;


			//##########################################################################################
			//# Class Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			/// <param name="oSettings">Web.Settings.Current instance representing the current enviroment.</param>
			/// <param name="oWebControl">Controls.Form object representing the parent WebControl.</param>
			/// <seealso cref="Cn.Web.Renderer.Form.Reset(Web.Settings.Current)"/>
			///############################################################
			/// <LastUpdated>May 28, 2010</LastUpdated>
			public Control_Form(Settings.Current oSettings, Controls.Form oWebControl) : base(oSettings) {
					//#### Collect the g_oWebControl, then init g_oDataSourceEnumerator and g_bCreateChildControls
				g_oWebControl = oWebControl;
				g_oDataSourceEnumerator = null;
				g_bCreateChildControls = false;
			}


			//##########################################################################################
			//# Public Properties
			//##########################################################################################
			/// ############################################################
			/// <summary>
			/// Gets/sets a IEnumerator object representing the data bound data source.
			/// </summary>
			/// ############################################################
			/// <LastUpdated>May 28, 2010</LastUpdated>
			public IEnumerator DataSourceEnumerator {
				get { return g_oDataSourceEnumerator; }
				set { g_oDataSourceEnumerator = value; }
			}

			/// ############################################################
			/// <summary>
			/// Gets/sets a boolean value representing if we are supposed to create the child controls of our parent webcontrol.
			/// </summary>
			/// ############################################################
			/// <LastUpdated>June 1, 2010</LastUpdated>
			public bool CreateChildControls {
				get { return g_bCreateChildControls; }
				set {
						//#### Set g_bCreateChildControls to the passed value, then set g_bAllowResponseWrite to the inverse (as we need to dis-g_bAllowResponseWrite if we are in g_bCreateChildControls mode)
					g_bCreateChildControls = value;
					g_bAllowResponseWrite = (! g_bCreateChildControls);
				}
			}


			//##########################################################################################
			//# Render-related Functions
			//##########################################################################################
			/// ############################################################
			/// <summary>
			/// Collects/reorders the entire results set.
			/// </summary>
			/// <remarks>
			/// Required function overload to collect the entire results set of relevant record IDs.
			/// <para/>If the page has no results or results that need to be re-ordered, this function is called by the <c>Class Constructor</c>. This function is not called if there are results that are properly ordered.
			/// </remarks>
			/// <param name="oResults">Pagination object representing entire result set's record IDs.</param>
			/// <param name="bReorderExistingResults">Boolean value indicating if the entire results set requires re-ordering.</param>
			/// ############################################################
			/// <LastUpdated>February 5, 2010</LastUpdated>
			public override void GenerateResults(Pagination oResults, bool bReorderExistingResults) {
					//#### .Raise(the)GenerateResults event within our parent g_oWebControl
				g_oWebControl.RaiseGenerateResults(new GenerateResultsEventArgs(oResults, bReorderExistingResults));
			}

			/// ############################################################
			/// <summary>
			/// Collects the provided page results.
			/// </summary>
			/// <remarks>
			/// Required function overload to collect a page's relevant records as referenced within the passed <paramref>rPageResults</paramref>.
			/// <para/>If the page has results, this function is called after <c>GenerateResults</c> or the <c>Class Constructor</c> (depending on the state of the provided <paramref>sResultsStack</paramref>). This function is not called if there are no records to render.
			/// <para/>NOTE: The <paramref>rPageResults</paramref> parameter is equivalent to this code snipit: <code>rPageResults = Renderer.Results.getRange(Renderer.Index, Renderer.List.RecordsPerPage)</code>
			/// </remarks>
			/// <param name="oPageResults">Pagination object representing this page's relevant record IDs.</param>
			/// ############################################################
			/// <LastUpdated>February 5, 2010</LastUpdated>
			public override void CollectPageResults(Pagination oPageResults) {
					//#### If there were no defined CollectPageResults delegates, call our .base implementation
				if (! g_oWebControl.RaiseCollectPageResults(new CollectPageResultsEventArgs(oPageResults))) {
					base.CollectPageResults(oPageResults);
				}
			}


			//##########################################################################################
			//# Page Section-related Functions
			//##########################################################################################
			/// ############################################################
			/// <summary>
			/// Returns the length the passed page section crawls down a printed page.
			/// </summary>
			/// <remarks>
			/// <para/>This function is called once per page section when a <c>Renderer.Report</c> is being rendered in print mode. This means that you can calculate the page crawl on a per record basis if need be.
			/// <para/>Due to the web's poor ability to format pages for printing, this method of counting the current number of units (inches, cm, mm, etc.) the current text would be on a printed page was devised. For example - if one record occupies approximately 1 inch of vertical page space, the top of the forth record would start at approximately 3 inches down the page. Though not perfect, this method allows the developer to better accommodate the printed page by controlling where page breaks occur.
			/// <para/>NOTE: The units you choose to return from this function (inches, cm, mm, etc.) are irrelevant, so long as they agree with the units returned by the <c>PageLength</c> functions in the related <c>RendererList</c>, <c>RendererForm</c> and/or <c>RendererReport</c>.
			/// </remarks>
			/// <param name="ePageSection">Enumeration representing the referenced page section.</param>
			/// <returns>
			/// Integer representing the page crawl in units for the referenced <paramref>ePageSection</paramref>.
			/// </returns>
			/// ############################################################
			/// <LastUpdated>February 10, 2010</LastUpdated>
			public override int PrintLength(enumPageSections ePageSection) {
				int iReturn;

					//#### If there were no defined PrintLength delegates, call our .base implementation
					//####     NOTE: The our parameter and .base implementation gyrations are done below to ensure that the default logic is run if no delegate is defined
				if (! g_oWebControl.RaisePrintLength(new PrintLengthEventArgs(ePageSection), out iReturn)) {
					iReturn = base.PrintLength(ePageSection);
				}

					//#### Return the above determined iReturn value to the caller
				return iReturn;
			}

			/// ############################################################
			/// <summary>
			/// Outputs the header section of the rendered page.
			/// </summary>
			/// <remarks>
			/// If the page has results, this function is called after <c>CollectPageResults</c>. If the page has no results, this function is called after <c>GenerateResults</c> or the <c>Class Constructor</c> (depending on the state of the provided <paramref>sResultsStack</paramref>). This function is called for every page render.
			/// </remarks>
			/// ############################################################
			/// <LastUpdated>May 28, 2010</LastUpdated>
			public override void Header() {
					//#### If we are in g_bCreateChildControls mode
				if (g_bCreateChildControls) {
					g_oWebControl.DoCreateChildControl(enumPageSections.cnHeader, -1, null);
				}
					//#### Else we need to .DoRenderChildControl
				else {
					g_oWebControl.DoRenderChildControl(enumPageSections.cnHeader);
				}
			}

			/// ############################################################
			/// <summary>
			/// Outputs the no results section of the rendered page.
			/// </summary>
			/// <remarks>
			/// If the page has no results, this function is called after <c>Header</c>. This function is not called if there are records to render.
			/// </remarks>
			/// ############################################################
			/// <LastUpdated>May 28, 2010</LastUpdated>
			public override void NoResults() {
					//#### If we are in g_bCreateChildControls mode
				if (g_bCreateChildControls) {
					g_oWebControl.DoCreateChildControl(enumPageSections.cnNoResults, -1, null);
				}
					//#### Else we need to .DoRenderChildControl
				else {
					g_oWebControl.DoRenderChildControl(enumPageSections.cnNoResults);
				}
			}

			/// ############################################################
			/// <summary>
			/// Outputs the detail header section of the rendered page.
			/// </summary>
			/// <remarks>
			/// If the page has results, this function is called after <c>Header</c> and before any records are rendered via the <c>Detail</c> and/or <c>MissingRecord</c> functions. This function is not called if there are no records to render.
			/// <para/>If a printable <c>Renderer.Report</c> is being rendered, this function is called after each <c>Report.PageHeader</c>.
			/// </remarks>
			/// ############################################################
			/// <LastUpdated>May 28, 2010</LastUpdated>
			public override void DetailHeader() {
					//#### If we are in g_bCreateChildControls mode
				if (g_bCreateChildControls) {
					g_oWebControl.DoCreateChildControl(enumPageSections.cnDetailHeader, -1, null);
				}
					//#### Else we need to .DoRenderChildControl
				else {
					g_oWebControl.DoRenderChildControl(enumPageSections.cnDetailHeader);
				}
			}

			/// ############################################################
			/// <summary>
			/// Outputs a detail section of the rendered page.
			/// </summary>
			/// <remarks>
			/// This function is called once per successfully collected record. This function is not called if there are no successfully collected records to render.
			/// </remarks>
			/// ############################################################
			/// <LastUpdated>May 28, 2010</LastUpdated>
			public override void Detail() {
					//#### If our g_oWebControl .IsDataBound and this .Is(not a)NewRecord, .DoDataBind for the current TableRecordIndex
//! is this still necessary?
				if (g_oWebControl.IsDataBound && ! IsNewRecord) {
					InputCollectionControlCommon.DoDataBind(g_oWebControl.InputControls, g_oWebControl.DataSource, TableRecordIndex);
				}

					//#### If we are in g_bCreateChildControls mode
				if (g_bCreateChildControls) {
						//#### If we have a g_oDataSourceEnumerator, pass in the .TableRecordIndex as this is a valid, non-New .cnDetail section
					if (g_oDataSourceEnumerator != null) {
							//#### If we can .MoveNext, .DoCreateChildControl for the .Current record
						if (g_oDataSourceEnumerator.MoveNext()) {
							g_oWebControl.DoCreateChildControl(enumPageSections.cnDetail, TableRecordIndex, g_oDataSourceEnumerator.Current);
						}
							//#### Else the .MoveNext failed, so raise the error
						else {
//! raise an error?
Web.Tools.dWrite(".MoveNext failed on the g_oDataSource!?");
							g_oWebControl.DoCreateChildControl(enumPageSections.cnDetail, -1, null);
						}
					}
						//#### Else there is no g_oDataSourceEnumerator, so pass in an invalid index and null
					else {
						g_oWebControl.DoCreateChildControl(enumPageSections.cnDetail, -1, null);
					}
				}
					//#### Else we need to .DoRenderChildControl
				else {
					g_oWebControl.DoRenderChildControl(enumPageSections.cnDetail);
				}
			}

			/// ############################################################
			/// <summary>
			/// Outputs a missing record section of the rendered page.
			/// </summary>
			/// <remarks>
			/// This function is called once per missing record. This function is not called if there are no missing records to render.
			/// </remarks>
			/// <param name="sTableName"></param>
			/// <param name="sID"></param>
			/// ############################################################
			/// <LastUpdated>May 28, 2010</LastUpdated>
			public override void MissingRecord(string sTableName, string sID) {
					//#### If we are in g_bCreateChildControls mode
				if (g_bCreateChildControls) {
					g_oWebControl.DoCreateChildControl(enumPageSections.cnMissingRecord, -1, null);
				}
					//#### Else we need to .DoRenderChildControl
				else {
					g_oWebControl.DoRenderChildControl(enumPageSections.cnMissingRecord);
				}
			}

			/// ############################################################
			/// <summary>
			/// Outputs the detail footer section of the rendered page.
			/// </summary>
			/// <remarks>
			/// If the page has results, this function is called after the last record is rendered via the <c>Detail</c> or <c>MissingRecord</c> function. This function is not called if there are no records to render.
			/// <para/>If a printable <c>Renderer.Report</c> is being rendered, this function is called before each <c>Report.PageFooter</c>.
			/// </remarks>
			/// ############################################################
			/// <LastUpdated>May 28, 2010</LastUpdated>
			public override void DetailFooter() {
					//#### If we are in g_bCreateChildControls mode
				if (g_bCreateChildControls) {
					g_oWebControl.DoCreateChildControl(enumPageSections.cnDetailFooter, -1, null);
				}
					//#### Else we need to .DoRenderChildControl
				else {
					g_oWebControl.DoRenderChildControl(enumPageSections.cnDetailFooter);
				}
			}

			/// ############################################################
			/// <summary>
			/// Outputs the footer section of the rendered page.
			/// </summary>
			/// <remarks>
			/// If the page has results, this function is called after <c>DetailFooter</c>. If the page has no results, this function is called after <c>NoResults</c>. This function is called for every page render.
			/// </remarks>
			/// ############################################################
			/// <LastUpdated>May 28, 2010</LastUpdated>
			public override void Footer() {
					//#### If we are in g_bCreateChildControls mode
				if (g_bCreateChildControls) {
					g_oWebControl.DoCreateChildControl(enumPageSections.cnFooter, -1, null);
				}
					//#### Else we need to .DoRenderChildControl
				else {
					g_oWebControl.DoRenderChildControl(enumPageSections.cnFooter);
				}
			}

			/// ############################################################
			/// <summary>
			/// Validates the current record during form processing.
			/// </summary>
			/// <remarks>
			/// This function is called after the <c>Class Constructor</c> once per submitted record when processing the form. This function is not called if there are no submitted records to validate.
			/// <para/>NOTE: This function is only utilized when processing a <c>Renderer.Form</c>. It is not utilized if we are rendering a <c>Renderer.Form</c>.
			/// </remarks>
			/// <param name="bRecordDataIsValid">Boolean value indicating if the all of the record's data successfully passed 'simple' validation (datatype, length, etc.).</param>
			/// <param name="bRecordIsLogiciallyValid">Boolean value indicating if the record is logicially valid.</param>
			/// <param name="bRecordHasChanged">Boolean value indicating if the record was changed/updated by the end user.</param>
			/// <returns>
			/// RecordValidater object that represents the records validity, if SQL statements are to be generated and any developer generated SQL statements.
			/// </returns>
			/// ############################################################
			/// <LastUpdated>March 26, 2010</LastUpdated>
			public override RecordValidater ValidateRecord(bool bRecordDataIsValid, bool bRecordIsLogiciallyValid, bool bRecordHasChanged) {
				RecordValidater oReturn;

					//#### If there were no defined ValidateRecord delegates, call our .base implementation
					//####     NOTE: The our parameter and .base implementation gyrations are done below to ensure that the default logic is run if no delegate is defined
				if (! g_oWebControl.RaiseValidateRecord(new ValidateRecordEventArgs(bRecordDataIsValid, bRecordIsLogiciallyValid, bRecordHasChanged), out oReturn)) {
					oReturn = base.ValidateRecord(bRecordDataIsValid, bRecordIsLogiciallyValid, bRecordHasChanged);
				}
				
					//#### Return the above determined oReturn value to the caller
				return oReturn;
			}

			/// ############################################################
			/// <summary>
			/// Submits the successfully collected and validated data into the data source.
			/// </summary>
			/// <remarks>
			/// Required function overload to process a page's submitted records.
			/// <para/>This function is called after the last <c>ValidateRecord</c>, or after the <c>Class Constructor</c> if there are no records to submit.
			/// <para/>NOTE: This function is only utilized when processing a <c>Renderer.Form</c>. It is not utilized if we are rendering a <c>Renderer.Form</c>.
			/// </remarks>
			/// <param name="a_sSQL">String array where each index represents a developer provided or system generated SQL statement.</param>
			/// ############################################################
			/// <LastUpdated>April 19, 2010</LastUpdated>
			public override void SubmitResults(string[] a_sSQL) {
					//#### .Raise(the)PrintLength event within our parent g_oWebControl
				g_oWebControl.RaiseSubmitResults(new SubmitResultsEventArgs(a_sSQL));
			}


		} //# private class Control_Form


		///########################################################################################################################
		/// <summary>
		/// Event arguments required by the <c>Form.ValidateRecord</c> event.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview>March 26, 2010</LastFullCodeReview>
		public class ValidateRecordEventArgs : EventArgs {
				//#### Declare the required private variables
			private bool g_bRecordDataIsValid;
			private bool g_bRecordIsLogiciallyValid;
			private bool g_bRecordHasChanged;


			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			/// <param name="bRecordDataIsValid">Boolean value indicating if the all of the record's data successfully passed 'simple' validation (datatype, length, etc.).</param>
			/// <param name="bRecordIsLogiciallyValid">Boolean value indicating if the record is logicially valid.</param>
			/// <param name="bRecordHasChanged">Boolean value indicating if the record was changed/updated by the end user.</param>
			///############################################################
			/// <LastUpdated>March 26, 2010</LastUpdated>
			public ValidateRecordEventArgs(bool bRecordDataIsValid, bool bRecordIsLogiciallyValid, bool bRecordHasChanged) {
				g_bRecordDataIsValid = bRecordDataIsValid;
				g_bRecordIsLogiciallyValid = bRecordIsLogiciallyValid;
				g_bRecordHasChanged = bRecordHasChanged;
			}

			///############################################################
			/// <summary>
			/// Gets a value indicating whether [record has changed].
			/// </summary>
			/// <value>Boolean value indicating if the record was changed/updated by the end user.</value>
			///############################################################
			/// <LastUpdated>December 1, 2009</LastUpdated>
			public bool RecordHasChanged {
				get { return g_bRecordHasChanged; }
			}

			///############################################################
			/// <summary>
			/// Gets a value indicating if the record is logicially valid.
			/// </summary>
			/// <value>Boolean value indicating if the record is logicially valid.</value>
			///############################################################
			/// <LastUpdated>March 26, 2010</LastUpdated>
			public bool RecordIsLogiciallyValid {
				get { return g_bRecordIsLogiciallyValid; }
			}

			///############################################################
			/// <summary>
			/// Gets a value indicating if the record's data successfully passed the 'simple' validation (datatype, length, etc.).
			/// </summary>
			/// <value>Boolean value indicating if the record successfully passed the 'simple' validation (datatype, length, etc.).</value>
			///############################################################
			/// <LastUpdated>March 26, 2010</LastUpdated>
			public bool RecordDataIsValid {
				get { return g_bRecordDataIsValid; }
			}
		} //# public class ValidateRecordEventArgs


		///########################################################################################################################
		/// <summary>
		/// Event arguments required by the <c>Form.SubmitResults</c> event.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview>December 1, 2009</LastFullCodeReview>
		public class SubmitResultsEventArgs : EventArgs {
				//#### Declare the required private variables
			private string[] ga_sSQL;


			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			/// <param name="a_sSQL">String array where each index represents a developer provided or system generated SQL statement.</param>
			/// <param name="bErrorsOccured">Boolean value indicating if any errors occured on the form.</param>
			///############################################################
			/// <LastUpdated>April 19, 2010</LastUpdated>
			public SubmitResultsEventArgs(string[] a_sSQL) {
				ga_sSQL = a_sSQL;
			}

			///############################################################
			/// <summary>
			/// Gets a value representing developer provided or system generated SQL statement(s).
			/// </summary>
			/// <value>String array where each index represents a developer provided or system generated SQL statement.</value>
			///############################################################
			/// <LastUpdated>December 1, 2009</LastUpdated>
			public string[] SQL {
				get { return ga_sSQL; }
			}
		} //# public class SubmitResultsEventArgs


	} //# public class Form


} //# namespace Cn.Web.Controls
