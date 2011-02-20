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
using System.Web.UI;
using System.Web.UI.WebControls;
using Cn.Data;
using Cn.Web.Renderer;


namespace Cn.Web.Controls {


	///########################################################################################################################
	/// <summary>
	/// Extends a <c>Renderer.List</c> or <c>Renderer.Form</c> instance for use within their related WebControls.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	internal interface Control_ListOrForm {
		/// ############################################################
		/// <summary>
		/// Gets/sets a IEnumerator object representing the data bound data source.
		/// </summary>
		/// ############################################################
		IEnumerator DataSourceEnumerator { get; set; }

		/// ############################################################
		/// <summary>
		/// Gets/sets a boolean value representing if we are supposed to create the child controls of our parent webcontrol.
		/// </summary>
		/// ############################################################
		bool CreateChildControls { get; set; }
	}


	///########################################################################################################################
	/// <summary>
	/// Controls.List class wraps the functionality of Renderer.List into a WebControl.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	[ToolboxData("<{0}:List runat=\"server\"></{0}List>")]
	public class List : WebControl, INamingContainer {
	#region List
			//#### Declare the required private variables
		protected HtmlTextWriter g_oRenderWriter;
		protected IEnumerator g_oDataSourceEnumerator;
		protected Pagination g_oPageResults;
		protected object g_oDataSource;
		protected Renderer.List g_oListOrForm;
		protected ITemplate g_oTemplate_Header;
		protected ITemplate g_oTemplate_NoResults;
		protected ITemplate g_oTemplate_DetailHeader;
		protected ITemplate g_oTemplate_Detail;
		protected ITemplate g_oTemplate_MissingRecord;
		protected ITemplate g_oTemplate_DetailFooter;
		protected ITemplate g_oTemplate_Footer;
		protected string g_sResultsStack;
		protected int g_iRecordsPerPage;
		protected int g_iTemplateIndex;
		protected bool g_bIsDataBound;


		//##########################################################################################
		//# Class Constructor
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 9, 2010</LastUpdated>
		public List() {
			    //#### Pass the call off to .DoReset, signaling that it should bSetListOrForm
			DoReset(true);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initialized state.
		/// </summary>
		/// <param name="bSetListOrForm"></param>
		///############################################################
		/// <LastUpdated>March 19, 2010</LastUpdated>
		protected void DoReset(bool bSetListOrForm) {
				//#### If we are to bSetListOrForm, do so now
			if (bSetListOrForm) {
				g_oListOrForm = new Control_List(new Settings.Current(), this);
			}

			    //#### Init our own global private variables
			g_oRenderWriter = null;
			g_oDataSourceEnumerator = null;
			g_oPageResults = null;
			g_oDataSource = null;
			g_sResultsStack = "";
			g_iRecordsPerPage = 0;
		  //g_iTemplateIndex = 0;		//# This is reset as required before calls to .DoCreateChildControls and .DoRenderChildControls
			g_bIsDataBound = false;
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
		/// Delegate signature defintion for the <c>Form.GenerateResults</c> events.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public delegate void GenerateResultsEventHandler(object sender, GenerateResultsEventArgs e);

			///############################################################
			/// <summary>
			/// Collects/reorders the entire results set.
			/// </summary>
			/// <remarks>
			/// A delegate definition is required for this event.
			/// </remarks>
			///############################################################
			/// <LastUpdated>February 10, 2010</LastUpdated>
			public event GenerateResultsEventHandler GenerateResults;
			internal void RaiseGenerateResults(GenerateResultsEventArgs e) {
					//#### If the developer has defined delegate(s), raise the event
				if (GenerateResults != null) {
					GenerateResults(this, e);
				}
					//#### Else the required delegate(s) were not defined, so raise the error
				else {
//! should use a CnException
throw new NotImplementedException("You must define a delegate for GenerateResults.");
				}
			}

		///############################################################
		/// <summary>
		/// Delegate signature defintion for the <c>Form.CollectPageResults</c> events.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public delegate void CollectPageResultsEventHandler(object sender, CollectPageResultsEventArgs e);

			///############################################################
			/// <summary>
			/// Collects the provided page results.
			/// </summary>
			///############################################################
			/// <LastUpdated>February 10, 2010</LastUpdated>
			public event CollectPageResultsEventHandler CollectPageResults;
			internal bool RaiseCollectPageResults(CollectPageResultsEventArgs e) {
				bool bReturn = (CollectPageResults != null);

					//#### If the developer has defined delegate(s), raise the event
				if (bReturn) {
					CollectPageResults(this, e);
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}

		///############################################################
		/// <summary>
		/// Delegate signature defintion for the <c>Form.PrintLength</c> events.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public delegate int PrintLengthEventHandler(object sender, PrintLengthEventArgs e);

			///############################################################
			/// <summary>
			/// Returns the length the passed page section crawls down a printed page.
			/// </summary>
			///############################################################
			/// <LastUpdated>February 10, 2010</LastUpdated>
			public event PrintLengthEventHandler PrintLength;
			internal bool RaisePrintLength(PrintLengthEventArgs e, out int iPrintLength) {
				bool bReturn = (PrintLength != null);

					//#### Init the passed iPrintLength
					//####     NOTE: The default value of iPrintLength set below doesn't matter, as if there is no defined .PrintLength delegate, this value is ignored anyway.
				iPrintLength = 0;

					//#### If the developer has defined delegate(s), raise the event and collect it's iReturn value
				if (PrintLength != null) {
					iPrintLength = PrintLength(this, e);
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the data source that is provided to the items within the control.
		/// </summary>
		/// <value>Object (optonally containing a list of values) that is provided to the items within the control.</value>
		///############################################################
		/// <LastUpdated>February 11, 2010</LastUpdated>
		public object DataSource {
			get { return g_oDataSource; }
			set { g_oDataSource = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the Template representing the header section of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		[TemplateContainer(typeof(TemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate Header {
			get { return g_oTemplate_Header; }
			set { g_oTemplate_Header = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the Template representing the no results section of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		[TemplateContainer(typeof(TemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate NoResults {
			get { return g_oTemplate_NoResults; }
			set { g_oTemplate_NoResults = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the Template representing the detail header section of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		[TemplateContainer(typeof(TemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate DetailHeader {
			get { return g_oTemplate_DetailHeader; }
			set { g_oTemplate_DetailHeader = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the Template representing the detail section of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		[TemplateContainer(typeof(TemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate Detail {
			get { return g_oTemplate_Detail; }
			set { g_oTemplate_Detail = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the Template representing the missing record section of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		[TemplateContainer(typeof(TemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate MissingRecord {
			get { return g_oTemplate_MissingRecord; }
			set { g_oTemplate_MissingRecord = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the Template representing the detail footer section of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		[TemplateContainer(typeof(TemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate DetailFooter {
			get { return g_oTemplate_DetailFooter; }
			set { g_oTemplate_DetailFooter = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the Template representing the footer section of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		[TemplateContainer(typeof(TemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate Footer {
			get { return g_oTemplate_Footer; }
			set { g_oTemplate_Footer = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the results set related to this instance.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>Results</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>String representation of a Pagination object that represents this instance's results set.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public string Results {
			get { return g_sResultsStack; }
			set { g_sResultsStack = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the count of records per rendered page (not including new records).
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>RecordsPerPage</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// <para/>NOTE: This property does not make much sense for <c>Report</c>s as <c>Report</c>s would generally represent the entire set of results (i.e. a <c>RecordsPerPage</c> value of "0"). But if you choose to only represent X records on a given report, you can utilize this property. Do note however that <c>RecordsPerPage</c> refers to records rendered per page load, *NOT* per phisically printed page!
		/// </remarks>
		/// <value>1-based integer representing the count of records per rendered page (not including new records).</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public int RecordsPerPage {
			get { return g_iRecordsPerPage; }
			set { g_iRecordsPerPage = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the default SQL Order By Clause for the current instance.
		/// </summary>
		/// <remarks>
		/// The <c>OrderBy</c> clause is set by default based on the following order of prescience:
		/// <para/>1) <c>OrderBy</c> equals the querystring's "OrderBy" field, as defined by the end-user (as long as it's safe).
		/// <para/>2) Failing 1, <c>OrderBy</c> equals this instance's Results' "OrderedBy" field, as defined by the end-user (as long as it's safe).
		/// <para/>3) Failing 1 and 2, <c>OrderBy</c> equals the <c>DefaultOrderBy</c>, as defined by the developer (as long as it's safe).
		/// <para/>4) Failing 1, 2 and 3, <c>OrderBy</c> equals a null-string.
		/// <para/><c>OrderBy</c> clause values set via the <c>OrderBy</c> property overwrite this default value.
		/// <para/>
		/// <para/>NOTE: This value is meaningless for <c>Report</c>s as the value is always sourced from the underlying <c>List</c> or <c>Form</c>.
		/// </remarks>
		/// <value>String representing the default SQL order by clause for the current instance.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public string DefaultOrderBy {
			get { return g_oListOrForm.DefaultOrderBy; }
			set { g_oListOrForm.DefaultOrderBy = value; }
		}

		///############################################################
		/// <summary>
		/// Get/sets a value indicating the total height of a printed page.
		/// </summary>
		/// <remarks>
		/// Due to the web's poor ability to format pages for printing, this method of counting the current number of units (inches, cm, mm, etc.) the current text would be on a printed page was devised. For example - if one record occupys approximately 1 inch of vertical page space, the top of the forth record would start at approximately 3 inches down the page. Though not perfect, this method allows the developer to better accommodate the printed page by controlling where page breaks occur.
		/// <para/>NOTE: The units you choose to return from this function (inches, cm, mm, etc.) are irrelevant, so long as they agree with the units returned by the <c>PageLength</c> functions in the related <c>List</c>, <c>Form</c> and/or <c>Report</c>.
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>PageLength</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>1-based integer representing the total height of a printed page in a unit of measure.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public int PageLength {
			get { return g_oListOrForm.PageLength; }
			set { g_oListOrForm.PageLength = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if we are in printable mode.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>Printable</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>Boolean value indicating if we are in printable mode.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public bool Printable {
			get { return g_oListOrForm.Printable; }
			set { g_oListOrForm.Printable = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if we are in debug mode.
		/// </summary>
		/// <remarks>
		/// When enabled, this functionality utilizes carrage return/line feed character(s) during rendering, as well as the non-compresssed JavaScript oFiles in order to ease debugging.
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>Debug</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>Boolean value indicating if we are in debug mode.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public bool Debug {
			get { return g_oListOrForm.Debug; }
			set { g_oListOrForm.Debug = value; }
		}


		//##########################################################################################
		//# Public Read-Only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the base control class related to this instance.
		/// </summary>
		/// <value>Renderer.List object that manages this instance.</value>
		///############################################################
		/// <LastUpdated>February 9, 2010</LastUpdated>
		public Renderer.List ControlManager {
			get { return g_oListOrForm as Renderer.List; }
		}

		///############################################################
		/// <summary>
		/// Gets a System.Web.UI.ControlCollection object that represents the child controls for a specified server control in the UI hierarchy.
		/// </summary>
		/// <value>System.Web.UI.ControlCollection object that represents the child controls for a specified server control in the UI hierarchy.</value>
		///############################################################
		/// <LastUpdated>June 1, 2010</LastUpdated>
		public override ControlCollection Controls {
			get {
					//#### .EnsureChildControls are created, then return the .base.Controls to the caller
//!				this.EnsureChildControls();
				return base.Controls;
			}
		}

		///############################################################
		/// <summary>
		/// Gets a value representing if this instance has been data bound.
		/// </summary>
		/// <value>Boolean value representing if this instance has been data bound.</value>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public bool IsDataBound {
			get { return g_bIsDataBound; }
		}


		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Binds a data source to the invoked server control and all its child controls.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 22, 2010</LastUpdated>
		public override void DataBind() {
				//#### Flip the value of g_bIsDataBound
			g_bIsDataBound = true;

				//#### If we have a valid IEnumerable g_oDataSource
			if (g_oDataSource != null && g_oDataSource is IEnumerable) {
					//#### Set the g_oDataSourceEnumerator and ensure it's at the beginning
				g_oDataSourceEnumerator = ((IEnumerable)g_oDataSource).GetEnumerator();
				g_oDataSourceEnumerator.Reset();
			}
				//#### Else the g_oDataSource was either null or not IEnumerable, so raise the error
			else {
//! raise error?
			}

				//#### (Re).Create(the)ChildControls (as they need to be (re)created with g_bIsDataBound set to true)
			CreateChildControls();

				//#### Now pass the call off to the base implementation
//!			base.DataBind();
		}


		//##########################################################################################
		//# Protected/Internal Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
		///############################################################
		/// <LastUpdated>May 31, 2010</LastUpdated>
		protected override void Render(HtmlTextWriter writer) {
				//#### Set the passed writer into our g_oRenderWriter
			g_oRenderWriter = writer;

				//#### .EnsureChildControls are setup and reset the g_iTemplateIndex in prep. for the .DoRender below
			EnsureChildControls();
			g_iTemplateIndex = 0;

				//#### .DoRender the g_oListOrForm, signaling it to .Reset it (as is per the usual call structure)
			g_oListOrForm.DoRender(g_oPageResults, true);

				//#### Now pass the call off to the base implementation
				//####     NOTE: We do not call our base.Render below because .RenderPageSection call's each .Controls' .RenderControl. Since .Render only call's .RenderChildren (in this implementation), which in turn only iterates over the .Controls calling each's .RenderControl we don't need to call our base.Render.
		  //base.Render(writer);
		}

		///############################################################
		/// <summary>
		/// Collects the value of the <c>g_oPageResults</c> variable.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 22, 2010</LastUpdated>
		protected void DoCollectPageResults() {
				//#### If we've not yet collected the g_oPageResults
			if (g_oPageResults == null) {
				string[] a_sIDs = null;
				int iIDCount = 0;

					//#### If the g_sResultsStack was not set
				if (string.IsNullOrEmpty(g_sResultsStack)) {
						//#### If we have a g_oDataSourceEnumerator to traverse/count, we'll use it to populate the a_sIDs
					if (g_oDataSourceEnumerator != null) {
							//#### Count the number of records within the g_oDataSource, then .Reset it back to the beginning
						while (g_oDataSourceEnumerator.MoveNext()) {
							iIDCount++;
						}
						g_oDataSourceEnumerator.Reset();

							//#### Dimension the a_sIDs to the above determined iIDCount, then treverse it in reverse filling each index with it's 1-based count
							//####     NOTE: We traverse the a_sIDs just so we don't need an iterator specificially defined for the loop (as iIDCount will work for the purpose, provided we go in reverse)
						a_sIDs = new string[iIDCount];
						for (/* iIDCount = iIDCount */; iIDCount > 0; iIDCount--) {
							a_sIDs[iIDCount - 1] = iIDCount.ToString();
						}
					}
				}

					//#### Call .DoCollectResults to retrieve the g_oPageResults
				g_oPageResults = g_oListOrForm.DoCollectResults(g_iRecordsPerPage, g_sResultsStack, a_sIDs, g_oListOrForm.DefaultOrderBy);
			}
		}

		///############################################################
		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preperation for posting back or rendering.
		/// </summary>
		///############################################################
		/// <LastUpdated>June 2, 2010</LastUpdated>
		protected override void CreateChildControls() {
			Control_ListOrForm oControl_ListOrForm = ((Control_ListOrForm)g_oListOrForm);

				//#### Setup the oControl_ListOrForm as in .CreateChildControls mode and pass in our .DataSourceEnumerator
			oControl_ListOrForm.CreateChildControls = true;
			oControl_ListOrForm.DataSourceEnumerator = g_oDataSourceEnumerator;

				//#### Collect the g_oPageResults, .Clear any .Controls we currently have and reset g_iTemplateIndex in prep. for the .DoRender below
//DoCreateDefaultChildControls();
			DoCollectPageResults();
Controls.Clear();
			g_iTemplateIndex = 0;

				//#### .DoRender the g_oListOrForm (signaling it NOT to .Reset as this is a intermidient render) and flip it back out of .CreateChildControls mode
				//####     NOTE: The first action taken by .DoRender is to .DoResetIndexes, so there is no need to call .DoResetIndexes explicitly here.
			g_oListOrForm.DoRender(g_oPageResults, false);
			oControl_ListOrForm.CreateChildControls = false;

				//#### Flip .ChildControlsCreated so the .Controls aren't regenerated
			ChildControlsCreated = true;
		}

/*
protected interface ControlTemplate {
	TemplateContainer InstantiatedContainer;
	ITemplate BaseTemplate;
}

protected Dictionary<enumPageSections, ControlTemplate> g_oTemplates = new Dictionary<enumPageSections, ControlTemplate>();

protected virtual void DoCreateDefaultChildControls() {
	ITemplate oTemplate;

		//#### 
	oTemplate = GetTemplate(enumPageSections.cnHeader);
	if (oTemplate != null) {
		g_oTemplates[enumPageSections.cnHeader].InstantiatedContainer = new TemplateContainer(enumPageSections.cnHeader);
		oTemplate.InstantiateIn(g_oTemplates[enumPageSections.cnHeader].InstantiatedContainer);
		g_oTemplates[enumPageSections.cnHeader].BaseTemplate = oTemplate;
	}
	oTemplate = GetTemplate(enumPageSections.cnFooter);
	if (oTemplate != null) {
		g_oTemplates[enumPageSections.cnFooter].InstantiatedContainer = new TemplateContainer(enumPageSections.cnFooter);
		oTemplate.InstantiateIn(g_oTemplates[enumPageSections.cnFooter].InstantiatedContainer);
		g_oTemplates[enumPageSections.cnFooter].BaseTemplate = oTemplate;
	}

		//#### 
	oTemplate = GetTemplate(enumPageSections.cnDetailHeader);
	if (oTemplate != null) {
		g_oTemplates[enumPageSections.cnDetailHeader].InstantiatedContainer = new TemplateContainer(enumPageSections.cnDetailHeader);
		oTemplate.InstantiateIn(g_oTemplates[enumPageSections.cnDetailHeader].InstantiatedContainer);
		g_oTemplates[enumPageSections.cnDetailHeader].BaseTemplate = oTemplate;
	}
	oTemplate = GetTemplate(enumPageSections.cnDetailFooter);
	if (oTemplate != null) {
		g_oTemplates[enumPageSections.cnDetailFooter].InstantiatedContainer = new TemplateContainer(enumPageSections.cnDetailFooter);
		oTemplate.InstantiateIn(g_oTemplates[enumPageSections.cnDetailFooter].InstantiatedContainer);
		g_oTemplates[enumPageSections.cnDetailFooter].BaseTemplate = oTemplate;
	}

		//#### 
	oTemplate = GetTemplate(enumPageSections.cnNoResults);
	if (oTemplate != null) {
		g_oTemplates[enumPageSections.cnNoResults].InstantiatedContainer = new TemplateContainer(enumPageSections.cnNoResults);
		oTemplate.InstantiateIn(g_oTemplates[enumPageSections.cnNoResults].InstantiatedContainer);
		g_oTemplates[enumPageSections.cnNoResults].BaseTemplate = oTemplate;
	}

		//#### 
	oTemplate = GetTemplate(enumPageSections.cnDetail);
	if (oTemplate != null) {
		g_oTemplates[enumPageSections.cnDetail].InstantiatedContainer = new TemplateContainer(enumPageSections.cnDetail);
		oTemplate.InstantiateIn(g_oTemplates[enumPageSections.cnDetail].InstantiatedContainer);
		g_oTemplates[enumPageSections.cnDetail].BaseTemplate = oTemplate;
	}
	oTemplate = GetTemplate(enumPageSections.cnMissingRecord);
	if (oTemplate != null) {
		g_oTemplates[enumPageSections.cnMissingRecord].InstantiatedContainer = new TemplateContainer(enumPageSections.cnMissingRecord);
		oTemplate.InstantiateIn(g_oTemplates[enumPageSections.cnMissingRecord].InstantiatedContainer);
		g_oTemplates[enumPageSections.cnMissingRecord].BaseTemplate = oTemplate;
	}
}
*/
		///############################################################
		/// <summary>
		/// Creates the control(s) for the specified page section of the WebControl.
		/// </summary>
		/// <param name="ePageSection">Enumeration representing the referenced page section.</param>
		/// <param name="iExistingRecordIndex">Integer representing the index of the data item in the data source.</param>
		/// <param name="oDataItem">Object representing the value to use when data-binding operations are preformed.</param>
		///############################################################
		/// <LastUpdated>June 2, 2010</LastUpdated>
		protected void DoCreateChildControl(enumPageSections ePageSection, int iExistingRecordIndex, object oDataItem) {
			TemplateContainer oTemplateContainer;
			ITemplate oTemplate = GetTemplate(ePageSection);

				//#### If the passed oTemplate has been defined by the developer
			if (oTemplate != null) {
					//#### Set the oTemplateContainer based on the passed oDataItem, assoicate it with the oTemplate then .Add the oTemplateContainer into our Controls
				oTemplateContainer = new TemplateContainer(oDataItem, iExistingRecordIndex, g_iTemplateIndex, ePageSection);
				oTemplate.InstantiateIn(oTemplateContainer);
				Controls.AddAt(g_iTemplateIndex, oTemplateContainer);

					//#### If we are g_bIsDataBound, .DataBind the .Control now
				if (g_bIsDataBound) {
					Controls[g_iTemplateIndex].DataBind();
				}

					//#### Inc g_iTemplateIndex in prep. for the next .DoCreateChildControl
				g_iTemplateIndex++;
			}
/*
			TemplateContainer oTemplateContainer;

				//#### If the ePageSection's ITemplate has been defined by the developer
				//####     NOTE: Only developer defined ePageSection's are added into g_oTemplates, so there is no need to call .GetTemplate and test it as non-null
				//####     NOTE: Since we are "reusing" .InstantiatedContainer's that were .Controls.Add'd at an earlier stage in the process then normal, we may run into issues 
			if (g_oTemplates.ContainsKey(ePageSection)) {
					//#### Determine the ePageSection and process accordingly
				switch (ePageSection) {
						//#### If this is a ePageSection that can be called multiple times per Render
					case enumPageSections.cnDetail:
					case enumPageSections.cnDetail_NewForm:
					case enumPageSections.cnMissingRecord:
					case enumPageSections.cnDetailHeader:
					case enumPageSections.cnDetailFooter: {
							//#### If this is a subsiquent call for this ePageSection
						if (g_oTemplates[ePageSection].InstantiatedContainer == null) {
								//#### Set the oTemplateContainer based on the passed oDataItem and assoicate it with the .BaseTemplate
							oTemplateContainer = new TemplateContainer(oDataItem, iExistingRecordIndex, g_iTemplateIndex, ePageSection);
							g_oTemplates[ePageSection].BaseTemplate.InstantiateIn(oTemplateContainer);
						}
							//#### Else this is the first call for this ePageSection, so we can use the pre-generated .InstantiatedContainer
						else {
								//#### Collect the .InstantiatedContainer from the g_oTemplates, then null out the .InstantiatedContainer (so it is not used again)
							oTemplateContainer = g_oTemplates[ePageSection].InstantiatedContainer;
							g_oTemplates[ePageSection].InstantiatedContainer = null;

								//#### Reset the .InstantiatedContainer's properties to the passed data
							oTemplateContainer.DataItem = oDataItem;
							oTemplateContainer.DataItemIndex = iExistingRecordIndex;
							oTemplateContainer.DisplayIndex = g_iTemplateIndex;
						}

							//#### .Add in the above determined/updated oTemplateContainer into our .Controls collection
						Controls.AddAt(g_iTemplateIndex, oTemplateContainer);
						break;
					}
						//#### If this is a ePageSection that is only called once per Render
					case enumPageSections.cnHeader:
					case enumPageSections.cnFooter:
					case enumPageSections.cnNoResults: {
							//#### Collect the .InstantiatedContainer from the g_oTemplates, reset it's .DisplayIndex and .Add it into our .Controls collection
							//####     NOTE: We only update the .DisplayIndex below as the other properties are left as the defaults they were initially set to for these ePageSection's
						oTemplateContainer = g_oTemplates[ePageSection].InstantiatedContainer;
						oTemplateContainer.DisplayIndex = g_iTemplateIndex;
						Controls.AddAt(g_iTemplateIndex, oTemplateContainer);
						break;
					}
				}

					//#### If we are g_bIsDataBound, .DataBind the .Control now
				if (g_bIsDataBound) {
					Controls[g_iTemplateIndex].DataBind();
				}

					//#### Inc g_iTemplateIndex in prep. for the next .DoCreateChildControl
				g_iTemplateIndex++;
			}
*/
		}

		///############################################################
		/// <summary>
		/// Renders the control(s) for the specified page section of the WebControl.
		/// </summary>
		/// <param name="ePageSection">Enumeration representing the referenced page section.</param>
		///############################################################
		/// <LastUpdated>June 28, 2010</LastUpdated>
		internal void DoRenderChildControl(enumPageSections ePageSection) {
			ITemplate oTemplate = GetTemplate(ePageSection);

				//#### If the oTemplate has been defined by the developer
				//####     NOTE: This is checked to ensure that we only .RenderControl for those oTemplate's that have been defined (as all calls are made, reguardless of oTemplate existence from our g_oListOrForm)
			if (oTemplate != null) {
					//#### If we are g_bIsDataBound, .DataBind the .Control now
				if (g_bIsDataBound) {
					Controls[g_iTemplateIndex].DataBind();
				}

					//#### .Render(the next)Control via the .Render set g_oRenderWriter, then inc g_iTemplateIndex
					//####     NOTE: Since each template was .Add'd in in order within .DoCreateChildControl and is called here again in order, there is no need to ensure each oTemplate is of the expected type.
				Controls[g_iTemplateIndex].RenderControl(g_oRenderWriter);
				g_iTemplateIndex++;
			}
		}


		//##########################################################################################
		//# Private Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Translates the provided page section into it's related template.
		/// </summary>
		/// <param name="ePageSection">Enumeration representing the referenced page section.</param>
		/// <returns>ITemplate object representing the related template.</returns>
		///############################################################
		/// <LastUpdated>May 31, 2010</LastUpdated>
		protected ITemplate GetTemplate(enumPageSections ePageSection) {
			ITemplate oReturn = null;

				//#### Determine the ePageSection, setting our oReturn value accordingly
			switch (ePageSection) {
				case enumPageSections.cnDetail:
				case enumPageSections.cnDetail_NewForm: {
					oReturn = g_oTemplate_Detail;
					break;
				}
				case enumPageSections.cnMissingRecord: {
					oReturn = g_oTemplate_MissingRecord;
					break;
				}
				case enumPageSections.cnDetailHeader: {
					oReturn = g_oTemplate_DetailHeader;
					break;
				}
				case enumPageSections.cnDetailFooter: {
					oReturn = g_oTemplate_DetailFooter;
					break;
				}
				case enumPageSections.cnHeader: {
					oReturn = g_oTemplate_Header;
					break;
				}
				case enumPageSections.cnFooter: {
					oReturn = g_oTemplate_Footer;
					break;
				}
				case enumPageSections.cnNoResults: {
					oReturn = g_oTemplate_NoResults;
					break;
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}
	#endregion


		///########################################################################################################################
		/// <summary>
		/// Renderer.List class implementation for the Control.List WebControl.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview></LastFullCodeReview>
		private class Control_List : Renderer.List, Control_ListOrForm {
				//#### Declare the required private variables
			private Controls.List g_oWebControl;
			private IEnumerator g_oDataSourceEnumerator;
			private bool g_bCreateChildControls;


			//##########################################################################################
			//# Class Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
			/// <param name="oWebControl">List object representing the parent WebControl.</param>
			/// <seealso cref="Cn.Web.Renderer.Form.Reset(Web.Settings.Current)"/>
			///############################################################
			/// <LastUpdated>May 28, 2010</LastUpdated>
			public Control_List(Settings.Current oSettings, Controls.List oWebControl) : base(oSettings) {
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
					//#### If we are in g_bCreateChildControls mode
				if (g_bCreateChildControls) {
						//#### If we have a g_oDataSourceEnumerator, pass in the .TableRecordIndex as this is a valid, non-new .cnDetail section
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


		} //# private class Control_List


	} //# public class List


} //# namespace Cn.Web.Controls
