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
using System.Collections.Generic;
using System.Data;
using System.Web.UI;								//# Required to access INamingContainer
using System.Web.UI.WebControls;					//# Required to access WebControl
using System.ComponentModel;
using Cn.Collections;
using Cn.Web.Inputs;


namespace Cn.Web.Controls {

	///########################################################################################################################
	/// <summary>
	/// Common static functionality used by all InputCollection-based WebControls.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	internal class InputCollectionControlCommon {

		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Binds a data source to the invoked server control and all its child controls.
		/// </summary>
		/// <param name="l_oInputs">List of Controls.Inputs representing the collection of inputs.</param>
		/// <param name="oDataSource">Object representing the data source to bind to the control.</param>
		/// <param name="iRowIndex">Integer representing the row index to use within the data source.</param>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static void DoDataBind(IList<Controls.Input> l_oInputs, object oDataSource, int iRowIndex) {
				//#### If we have l_oInputs to .DataBind and a oDataSource to bind from
			if (l_oInputs.Count > 0 && oDataSource != null) {
					//#### If the specified oDataSource is a DataTable
				if (oDataSource is DataTable) {
					DataTable oDataTable = oDataSource as DataTable;

						//#### .Map the oDataSource into the l_oInputs
					Mirror.Map(oDataTable, l_oInputs, iRowIndex, false);
				}
					//#### Else if the specified oDataSource is a MultiArray
				else if (oDataSource is MultiArray) {
					MultiArray oMultiArray = oDataSource as MultiArray;

						//#### .Map the oDataSource into the l_oInputs
					Mirror.Map(oMultiArray, l_oInputs, iRowIndex);
				}
					//#### Else if the specified oDataSource is a DataSet
				else if (oDataSource is DataSet) {
					DataSet oDataSet = oDataSource as DataSet;

						//#### Traverse the oDataSource's .Tables
					for (int i = 0; i < oDataSet.Tables.Count; i++) {
							//#### .Map the current oDataSet.Tables into the l_oInputs
						Mirror.Map(oDataSet.Tables[i], l_oInputs, iRowIndex, true);
					}
				}
			}
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
		/// <returns>Boolean value indicating if it is necessary to raise the <c>GenerateHTML</c> event.</returns>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		public static bool DoRenderInput(string sInputAlias, enumInputTypes eInputType, string sInitialValue, string[] a_sInitialValues, bool bForceInitialValue, string sAttributes, HtmlTextWriter writer, IInputCollection oInputCollection) {
			bool bReturn = false;

		  //	//#### If the passed sInputAlias .Exists
		  //if (oInputCollection.Exists(sInputAlias)) {
					//#### If this is a non-.cnCustom control
				if (eInputType != enumInputTypes.cnCustom) {
						//#### Flip our bReturn value to true
					bReturn = true;

						//#### If the sInputAlias .IsMultiValue, call the a_sInitialValues version of .GenerateHTML
					if (oInputCollection.Inputs(sInputAlias).IsMultiValue) {
						writer.Write(oInputCollection.GenerateHTML(sInputAlias, eInputType, a_sInitialValues, bForceInitialValue, sAttributes));
					}
						//#### Else the sInputAlias .Is(not a)MultiValue, so call the sInitialValue version of .GenerateHTML
					else {
						writer.Write(oInputCollection.GenerateHTML(sInputAlias, eInputType, sInitialValue, bForceInitialValue, sAttributes));
					}
				}
		  //}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}


	} //# internal class InputCollectionControlCommon


	///########################################################################################################################
	/// <summary>
	/// Controls.InputCollection class wraps the functionality of Inputs.InputCollection into a WebControl.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>September 6, 2005</LastFullCodeReview>
	[ToolboxData("<{0}:InputCollection runat=\"server\" id=\"InputCollection\" IsReadOnly=\"false\" DisplayErrorMessagesViaJavaScript=\"true\" />")]
	public class InputCollection : WebControl, IInputCollectionControl, INamingContainer {
			//#### Declare the required private variables
		List<Controls.Input> gl_oInputs;
		Inputs.InputCollection g_oInputCollection;
		protected ITemplate g_oTemplate_Form;
		private object g_oDataSource;
		private bool g_bIsDataBound;


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public InputCollection() {
				//#### Default the g_oInputs to a new instance and init the gl_oInputs
			g_oInputCollection = new Inputs.InputCollection(new Settings.Current());
			gl_oInputs = new List<Input>();
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
		/// Enables the generation of custom HTML controls.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public event GenerateHTMLEventHandler GenerateHTML;


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the data source that is provided to the items within the control.
		/// </summary>
		/// <value>Object that is provided to the items within the control.</value>
		///############################################################
		/// <LastUpdated>February 11, 2010</LastUpdated>
		public object DataSource {
			get { return g_oDataSource; }
			set { g_oDataSource = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the Template representing the form section of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		[TemplateContainer(typeof(TemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate Form {
			get { return g_oTemplate_Form; }
			set { g_oTemplate_Form = value; }
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
		/// <LastUpdated>November 27, 2009</LastUpdated>
		[Bindable(true),
			Category("Behavior"),
			DefaultValue("true")
		]
		public bool DisplayErrorMessagesViaJavaScript {
			get { return g_oInputCollection.DisplayErrorMessagesViaJavaScript; }
			set { g_oInputCollection.DisplayErrorMessagesViaJavaScript = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if the <c>Form</c> is to be rendered as read only.
		/// </summary>
		/// <remarks>
		/// NOTE: This property references <c>Inputs.Settings.IsReadOnly</c> and is available here for documentation of the related WebControl attribute only.
		/// </remarks>
		/// <value>Boolean value indicating if the <c>Form</c> is to be rendered as read only.</value>
		///############################################################
		/// <LastUpdated>November 23, 2009</LastUpdated>
		[Bindable(true),
			Category("Behavior"),
			DefaultValue("false")
		]
		public bool IsReadOnly {
			get { return g_oInputCollection.Settings.IsReadOnly; }
			set { g_oInputCollection.Settings.IsReadOnly = value; }
		}


		//##########################################################################################
		//# Public Read-Only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the base control class related to this instance.
		/// </summary>
		/// <value>InputCollection object that manages this instance.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		[Category("Management")]
		public Inputs.InputCollection ControlManager {
			get { return g_oInputCollection; }
//!			set { g_oInputCollection = value; }
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
			get { return g_oInputCollection; }
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
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public override void DataBind() {
				//#### Flip the value of g_bIsDataBound, then pass the call into .DoDataBind to do the actual work, then onto our .base implementation
			g_bIsDataBound = true;
			InputCollectionControlCommon.DoDataBind(gl_oInputs, g_oDataSource, 0);
			base.DataBind();
		}


		//##########################################################################################
		//# Protected/Internal Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Raises the System.Web.UI.Control.Init event.
		/// </summary>
		/// <param name="e">An System.EventArgs object that contains the event data.</param>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		protected override void OnInit(EventArgs e) {
				//#### Pass the call off to the base implementation
			base.OnInit(e);

				//#### Copy the .Page's .IsPostBack into the g_oInputs.IsPostBack
			g_oInputCollection.IsPostBack = Page.IsPostBack;
		}

		///############################################################
		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">The System.Web.UI.HtmlTextWriter object that recieves the control content.</param>
		///############################################################
		/// <LastUpdated>March 19, 2010</LastUpdated>
		protected override void Render(HtmlTextWriter writer) {
			TemplateContainer oTemplateContainer;

				//#### .Write out the .GenerateCSS
			writer.Write("<style>" + g_oInputCollection.GenerateCSS() + "</style>");

				//#### If we are to .DisplayErrorMessagesViaJavaScript, .write out the .ValidationJavaScript to manage the g_oInputs
			if (g_oInputCollection.DisplayErrorMessagesViaJavaScript) {
				writer.Write(g_oInputCollection.ValidationJavaScript("", "", "", true));
			}

				//#### If the g_oTemplate_Form has been defined by the developer
			if (g_oTemplate_Form != null) {
					//#### Set the oTemplateContainer based on the g_oDataSource, assoicate it with the g_oTemplate_Form then .Add the oTemplateContainer into our Controls
//! NEEK this is not really a .cnDetail!
				oTemplateContainer = new TemplateContainer(g_oDataSource, -1, 1, Cn.Web.Renderer.enumPageSections.cnDetail);
				g_oTemplate_Form.InstantiateIn(oTemplateContainer);
				Controls.Add(oTemplateContainer);

					//#### Now pass the call off to the base implementation (which will render the g_oTemplate_Form)
				base.Render(writer);
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
		/// <param name="oInputControl">Controls.Input representing the Input control to add into this instance.</param>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		internal void Add(Controls.Input oInputControl) {
				//#### .Add the passed oInputControl into the underlying g_oInputCollection and then into our gl_oInputs
			g_oInputCollection.Add(oInputControl.ControlManager);
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
		/// <LastUpdated>February 15, 2010</LastUpdated>
		internal void RenderInput(string sInputAlias, enumInputTypes eInputType, string sInitialValue, string[] a_sInitialValues, bool bForceInitialValue, string sAttributes, HtmlTextWriter writer) {
				//#### If this is a .cnCustom control (as .DoRenderInput was unable to do the render itself)
				//####     NOTE: An event is required for the control as there is no other way not to call .Parent's .Render function, so this hook/event allows the developer to change the .Render if they so choose.
			if (! InputCollectionControlCommon.DoRenderInput(sInputAlias, eInputType, sInitialValue, a_sInitialValues, bForceInitialValue, sAttributes, writer, g_oInputCollection)) {
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


	} //# public class InputCollection


} //# namespace Cn.Web.Controls
