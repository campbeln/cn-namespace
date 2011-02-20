/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Data;									//# Required to access the DataSet class
using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Data;										//# Required to access MetaData, Pagination, Picklists, etc.


namespace Cn.Web.Renderer {

	///########################################################################################################################
	/// <summary>
	/// <c>TableForm</c> abstract class.
	/// </summary>
	/// <remarks>
	/// This class represents an example of an extended <c>Renderer.Form</c>. This class demonstrates how you could implement your own abstract <c>Renderer</c> classes to accomplish system specific tasks with a minimum of code and a maximum of reuse.
	/// </remarks>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	public abstract class TableForm : Form {
			//#### Declare the required private variables
		private DBMS.GetValue g_oGet;
		private string[] ga_sInputAliases;
		private int g_iColumnCount;


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <param name="sTableName">String representing the required table name.</param>
		/// <param name="sIDColumn">String representing the name of the primary key column (this column will not be included in the generated SQL statements).<para/>NOTE: Pass in a null-string if there is no ID column.</param>
		///############################################################
		/// <LastUpdated>May 30, 2007</LastUpdated>
		public TableForm(Settings.Current oSettings, string sTableName, string sIDColumn) : base(oSettings) {
			MultiArray oTable;

				//#### If the caller passed in a known sTableName/sIDColumn pair
			if (Web.Settings.MetaData.Exists(sTableName, sIDColumn)) {
					//#### Collect the oTable for the passed sTableName, reset g_iColumnCount, dimension ga_sInputAliases and determine sIDColumn's .Length
				oTable = Web.Settings.MetaData.Table(sTableName);

					//#### Pass the call off to .Initilize to do the actual work
				Initilize(oTable, sIDColumn);
			}
				//#### Else the passed sTableName/sIDColumn pair was unreconized, so raise the error
			else {
//!
			}
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <param name="oTable">MultiArray representing the table metadata to render.<para/>NOTE: Column definitions can be from more then one table.</param>
		/// <param name="sIDColumn">String representing the name of the primary key column (this column will not be included in the generated SQL statements).<para/>NOTE: Pass in a null-string if there is no ID column.</param>
		///############################################################
		/// <LastUpdated>May 30, 2007</LastUpdated>
		public TableForm(Settings.Current oSettings, MultiArray oTable, string sIDColumn) : base(oSettings) {
				//#### Pass the call off to .Initilize to do the actual work
			Initilize(oTable, sIDColumn);
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oTable">MultiArray representing the table metadata to render.<para/>NOTE: Column definitions can be from more then one table.</param>
		/// <param name="sIDColumn">String representing the name of the primary key column (this column will not be included in the generated SQL statements).<para/>NOTE: Pass in a null-string if there is no ID column.</param>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		private void Initilize(MultiArray oTable, string sIDColumn) {
			Inputs.AdditionalData oAdditionalData = new Inputs.AdditionalData();
			string sPicklistName;
			string sColumnName;
			int iIDColumnLen;
			int i;
			Inputs.enumSaveTypes eSaveType;

				//#### Init the global vars
			g_oGet = new DBMS.GetValue(this);
			ga_sInputAliases = null;
			g_iColumnCount = 0;

				//#### If the caller passed in a oTable with data
			if (oTable != null && oTable.RowCount > 0) {
					//#### Reset g_iColumnCount, dimension ga_sInputAliases and determine sIDColumn's .Length
				g_iColumnCount = oTable.RowCount;
				ga_sInputAliases = new string[g_iColumnCount];
				iIDColumnLen = sIDColumn.Length;

					//#### Traverse the oTable's .Rows (each of which represent a column within the referenced sTableName)
				for (i = 0; i < g_iColumnCount; i++) {
						//#### Collect the current sColumnName, setting it into the ga_sInputAliases as we go (as we use the sColumnName as the sInputAlias below)
					sColumnName = oTable.Value(i, "Column_Name");
					ga_sInputAliases[i] = sColumnName;

						//#### If the current sColumnName matches the passed sIDColumn (checking their .Lengths first as that is a far faster comparsion)
					if (iIDColumnLen == sColumnName.Length && sIDColumn == sColumnName) {
							//#### Set the current sColumnName/sIDColumn's eSaveType as .cnID
						eSaveType = Web.Inputs.enumSaveTypes.cnID;
					}
						//#### Else this is a non-.cnID column definition
					else {
							//#### If the current column Is_Nullable, set it's eSaveType as .cnInsertNull
						if (Data.Tools.MakeBoolean(oTable.Value(i, "Is_Nullable"), false)) {
							eSaveType = Web.Inputs.enumSaveTypes.cnInsertNull;
						}
							//#### Else the current column Is_(not)Nullable, so set it's eSaveType as .cnRequired
						else {
							eSaveType = Web.Inputs.enumSaveTypes.cnRequired;
						}
					}

						//#### Collect the current RelatedPicklist sPicklistName
					sPicklistName = Data.Tools.MakeString(oTable.Value(i, "RelatedPicklist"), "");

						//#### If a sPicklistName was collected above, set it into the oAdditionalData
					if (sPicklistName.Length > 0) {
						oAdditionalData.Picklist_Name = sPicklistName;
					}
						//#### Else no sPicklistName was found, so ensure that the related key in oAdditionalData is blank
					else {
						oAdditionalData.Picklist_Name = null;
					}

						//#### .Add the above determined input
					InputCollection.Add(sColumnName, oTable.Value(i, "Table_Name"), sColumnName, eSaveType, MetaData.enumValueTypes.cnSingleValue, oAdditionalData);
				}
			}
				//#### Else the passed a oTable that was empty, so raise the error
			else {
//!
			}
		}


		//##########################################################################################
		//# Public Read-Write Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the "set of results" utilized by this instance.
		/// </summary>
		/// <value>DataSet representing the "set of results" utilized by this instance.</value>
		///############################################################
		/// <LastUpdated>May 30, 2007</LastUpdated>
		new public DataSet Results {
			get { return g_oGet.Data; }
			set { g_oGet.Data = value; }
		}


		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
        /// <summary>
        /// Gets a value representing the URL to redirect to on a successful form submission.
        /// </summary>
		/// <param name="bErrorsOccured">Boolean value indicating if any errors occured on the form.</param>
        /// <value>String representing the URL to redirect to on a successful form submission.</value>
		///############################################################
		/// <LastUpdated>May 30, 2007</LastUpdated>
		public void RedirectURL(bool bErrorsOccured) {
				//#### If no bErrorsOccured, we need to .Redirect to the .RedirectURL 
            if (! bErrorsOccured) {
                Response.Redirect(Breadcrumb.URL());
            }
		}


		//##########################################################################################
		//# Page Section-related Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Outputs the detail header section of the rendered page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own page section.
		/// <para/>If the page has results, this function is called after <c>Header</c> and before any records are rendered via the <c>Detail</c> and/or <c>MissingRecord</c> functions. This function is not called if there are no records to render.
		/// <para/>If a printable <c>Renderer.Report</c> is being rendered, this function is called after each <c>Report.PageHeader</c>.
		/// </remarks>
		///############################################################
		/// <LastUpdated>December 3, 2009</LastUpdated>
		public override void DetailHeader() {
			Response.Write("<html>");
			Response.Write("<form id='TableForm' action='" + InputCollection.UIHook(Web.Inputs.enumUIHookTypes.cnFormAction) + "'>");
		}

		///############################################################
		/// <summary>
		/// Outputs a detail section of the rendered page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own page section.
		/// <para/>This function is called once per successfully collected record. This function is not called if there are no successfully collected records to render.
		/// </remarks>
		///############################################################
		/// <LastUpdated>August 21, 2007</LastUpdated>
		public override void Detail() {
			string sCurrentInputAlias;
			int i;
			Inputs.enumInputTypes eInputType;

				//#### If we have ga_sInputAliases to render
			if (g_iColumnCount > 0) {
					//#### Traverse the ga_sInputAliases
				for (i = 0; i < g_iColumnCount; i++) {
						//#### Collect the sCurrentInputAlias
					sCurrentInputAlias = ga_sInputAliases[i];

						//#### If the sCurrentInputAlias .Is(an)ID input, set the eInputType to .cnReadOnly
					if (InputCollection.Inputs(sCurrentInputAlias).SaveType == Web.Inputs.enumSaveTypes.cnID) {
						eInputType = Web.Inputs.enumInputTypes.cnReadOnly;
					}
						//#### Else the sCurrentInputAlias is a non-.cnID input, so set the eInputType to .cnDefaultInput
					else {
						eInputType = Web.Inputs.enumInputTypes.cnDefaultInput;
					}

						//#### .Render(the)Input for the current ga_sInputAlias
					Response.Write("<label for='" + InputCollection.InputName(sCurrentInputAlias) + "'>" + sCurrentInputAlias + "</label>: ");
					Response.Write(InputCollection.GenerateHTML(sCurrentInputAlias, eInputType, g_oGet.Value(sCurrentInputAlias), false, ""));
					Response.Write("<br/>");
				}

					//#### 
				Response.Write("<hr/>");
			}
				//#### Else we have no ga_sInputAliases to render, so raise the error
			else {
//!
			}
		}

		///############################################################
		/// <summary>
		/// Outputs the detail footer section of the rendered page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own page section.
		/// <para/>If the page has results, this function is called after the last record is rendered via the <c>Detail</c> or <c>MissingRecord</c> function. This function is not called if there are no records to render.
		/// <para/>If a printable <c>Renderer.Report</c> is being rendered, this function is called before each <c>Report.PageFooter</c>.
		/// </remarks>
		///############################################################
		/// <LastUpdated>May 30, 2007</LastUpdated>
		public override void DetailFooter() {
			Response.Write("<input type='submit' value='Save' />");
			Response.Write("</form>");
			Response.Write("</html>");
		}

	} //# public abstract class TableForm

} //# namespace Cn.Web.Renderer
