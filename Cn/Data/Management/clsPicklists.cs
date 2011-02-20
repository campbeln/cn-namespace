/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections;					        //# Required to access the Hashtable class
using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Data.SQL;									//# Required to access the enumDataSource
using Cn.Web.Renderer;								//# Required to access the Renderer classes
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Data.Management {

	///########################################################################################################################
	/// <summary>
	/// Picklists Manager Form abstract class (extension of the <c>Renderer.Form</c> class).
	/// </summary>
	/// <remarks>
	/// This class represents an example of an overloaded <c>Renderer.Form</c> which renders data from a custom dataset object (a <c>Data.Picklists</c> in this case).
	/// <para/>NOTE: You are required to implement your own <c>SubmitResults</c> function to commit the data to your datasource. Below is an example implementation utilizing an SQL*Server datasource:
	/// <code>
	///		public override void SubmitResults(string[] a_sSQL, bool bErrorsOccured) {
    ///		Cn.Data.Helper oDataSource;
    ///         string sSQL;
	///
    ///             //#### Establish the connection to the oDataSource
    ///         oDataSource = new Cn.Data.Helper(Cn.Data.Helper.enumConnectionType.cnSQLServer, "YOUR_SERVER_DSN_GOES_HERE");
	///
    ///             //#### Collect the form's full sSQL then commit the updates to the oDataSource
    ///         sSQL = this.SQL(a_sSQL);
    ///         oDataSource.ExecuteSQL(sSQL, false);
	///
	///			//#### Re-.GetData(the)Settings to refresh them from the above updated oDataSource, then .Close(the)Connection.
	///		oDataSource.PopulateSettings(sSystemName, true, true, true, true, Cn.Data.enumDataSource.cnSQLServer, false, true);
    ///         oDataSource.CloseConnection();
	///
    ///             //#### If no bErrorsOccured, .Redirect the user back to the form via the RedirectURL.
    ///         if (! bErrorsOccured) {
    ///             Response.Redirect(this.RedirectURL);
    ///         }
    ///     }
	/// </code>
	/// </remarks>
	///########################################################################################################################
	/// <LastFullCodeReview>August 18, 2004</LastFullCodeReview>
#region Picklist
	public abstract class Picklist : Form {
            //#### Declare the required private variables
        private Cn.Data.Picklists g_oInternationalizationPicklist;
		private MultiArray g_oPicklist;
		private MultiArray g_oParentItem;
		private string g_sColumnAssoPicklistName;
		private string g_sMetaDataPicklistName;
		private string g_sPicklistName;
		private string g_sTableName;
		private string g_sSource;
		private int g_iRecordCount;
		private enumDataSource g_eDbServer;
		private bool g_bPrintHTMLHeaderFooter;
		private bool g_bIsColumnAssoPicklist;
		private bool g_bIsMetaDataPicklist;
		private bool g_bIsSystemPicklist;

            //#### Declare the required private constants
		private const string g_cClassName = "Cn.Web.Management.Picklists.";
		private const string g_cFormName = "Renderer";


        //##########################################################################################
        //# Class Functions
        //##########################################################################################
		///############################################################
        /// <summary>
		/// Initializes the class.
        /// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <param name="eDbServer">Enumeration representing the target database server type.</param>
		///############################################################
		/// <LastUpdated>May 21, 2007</LastUpdated>
		public Picklist(Web.Settings.Current oSettings, enumDataSource eDbServer) : base(oSettings) {
                //#### Pass the call off to our private Initilize sub (passing in the default sUserPicklistsTableName)
			Initilize(eDbServer, "cnPicklists", "cnInternationalization");
		}

		///############################################################
        /// <summary>
		/// Initializes the class.
        /// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <param name="eDbServer">Enumeration representing the target database server type.</param>
        /// <param name="sUserPicklistsTableName">String representing the table name of the user picklists.</param>
        /// <param name="sInternationalizationTableName">String representing the table name of the internationalization picklists.</param>
		///############################################################
		/// <LastUpdated>May 21, 2007</LastUpdated>
		public Picklist(Web.Settings.Current oSettings, enumDataSource eDbServer, string sUserPicklistsTableName, string sInternationalizationTableName) : base(oSettings) {
                //#### Pass the call off to our private Initilize sub (passing in the passed sUserPicklistsTableName)
			Initilize(eDbServer, sUserPicklistsTableName, sInternationalizationTableName);
		}

		///############################################################
        /// <summary>
        /// Private/defacto class constructor.
        /// </summary>
		/// <param name="eDbServer">Enumeration representing the target database server type.</param>
        /// <param name="sUserPicklistsTableName">String representing the table name of the user picklists.</param>
        /// <param name="sInternationalizationTableName">String representing the table name of the internationalization picklists.</param>
		///############################################################
		/// <LastUpdated>November 16, 2005</LastUpdated>
		private void Initilize(enumDataSource eDbServer, string sUserPicklistsTableName, string sInternationalizationTableName) {
				//#### Default the value of g_bPrintHTMLHeaderFooter to true
			g_bPrintHTMLHeaderFooter = true;

				//#### Init .Internationalization's .Data into the global g_oInternationalizationPicklist
			g_oInternationalizationPicklist = new Picklists(Web.Settings.Internationalization.Data);
InputCollection.Picklists = Web.Settings.Picklists;

				//#### 
			g_sColumnAssoPicklistName = Picklists.ColumnAssociationsPicklistName;
			g_sMetaDataPicklistName = Picklists.MetaDataPicklistName;

                //#### Set the g_eDbServer to the passed data
			g_eDbServer = eDbServer;

                //#### Set the g_sPicklistName from the user-supplied PicklistToLoad (defaulting it to the metadata picklist if the user didn't supply a picklist name)
            g_sPicklistName = Request["PicklistToLoad"];
            if (g_sPicklistName == null || g_sPicklistName.Length == 0) {
				g_sPicklistName = g_sMetaDataPicklistName;
            }

                //#### Set the value of g_bIsColumnAssoPicklist and g_bIsMetaDataPicklist based on the value of the above determined g_sPicklistName, then determine the g_sSource
            g_bIsColumnAssoPicklist = (g_sPicklistName == g_sColumnAssoPicklistName);
			g_bIsMetaDataPicklist = (g_sPicklistName == g_sMetaDataPicklistName);
			g_sSource = Data.Tools.MakeString(Request["Source"], "").ToLower();

                //#### Determine the g_sSource picklist and process accordingly
            switch (g_sSource) {
                    //#### If this is an Internationalization request
                case "internationalization": {
                        //#### Set the g_bIsSystemPicklist to true (as this is a system picklist) and set g_sTableName accordingly
				    g_bIsSystemPicklist = true;
					g_sTableName = sInternationalizationTableName;

                        //#### Collect the above determined g_sPicklistName into g_oPicklist and retrieve it's Item data into g_oParentItem
				    g_oPicklist = g_oInternationalizationPicklist.Picklist(g_sPicklistName);
				    g_oParentItem = g_oInternationalizationPicklist.Items(g_sMetaDataPicklistName, g_sPicklistName);
				    break;
                }
                    //#### Else we'll assume that this is a user picklist request
                default: {  // "user"
                        //#### Set the g_bIsSystemPicklist to false (as this is not a system picklist) and set g_sTableName accordingly
				    g_bIsSystemPicklist = false;
					g_sTableName = sUserPicklistsTableName;

                        //#### Collect the above determined g_sPicklistName into g_oPicklist and retrieve it's Item data into g_oParentItem
				    g_oPicklist = InputCollection.Picklists.Picklist(g_sPicklistName);
				    g_oParentItem = InputCollection.Picklists.Items(g_sMetaDataPicklistName, g_sPicklistName);
				    break;
                }
            }

                //#### .Add the inputs
			InputCollection.Add("ID", g_sTableName, "ID", Web.Inputs.enumSaveTypes.cnID, MetaData.enumValueTypes.cnSingleValue, null);
			InputCollection.Add("PicklistID", g_sTableName, "PicklistID", Web.Inputs.enumSaveTypes.cnRequired, MetaData.enumValueTypes.cnSingleValue, null);
			InputCollection.Add("DisplayOrder", g_sTableName, "DisplayOrder", Web.Inputs.enumSaveTypes.cnRequired, MetaData.enumValueTypes.cnSingleValue, null);
			InputCollection.Add("Data", g_sTableName, "Data", Web.Inputs.enumSaveTypes.cnInsertNullString, MetaData.enumValueTypes.cnSingleValue, null);
			InputCollection.Add("Description", g_sTableName, "Description", Web.Inputs.enumSaveTypes.cnInsertNullString, MetaData.enumValueTypes.cnSingleValue, null);

                //####
            if (g_bIsSystemPicklist || g_bIsMetaDataPicklist) {
				InputCollection.Add("IsActive", g_sTableName, "IsActive", Web.Inputs.enumSaveTypes.cnIgnore, MetaData.enumValueTypes.cnSingleValue, null);
            }
            else {
				InputCollection.Add("IsActive", g_sTableName, "IsActive", Web.Inputs.enumSaveTypes.cnRequired, MetaData.enumValueTypes.cnSingleValue, null);
            }
		}


        //##########################################################################################
        //# Public Read-Write Properties
        //##########################################################################################
		///############################################################
        /// <summary>
        /// Gets/sets a value indicating if the default HTML header and footer is rendered.
        /// </summary>
        /// <value>Boolean value indicating if the default HTML header and footer is rendered.</value>
		///############################################################
		/// <LastUpdated>November 16, 2005</LastUpdated>
		public bool PrintHTMLHeaderFooter {
			get { return g_bPrintHTMLHeaderFooter; }
			set { g_bPrintHTMLHeaderFooter = value; }
		}


        //##########################################################################################
        //# Public Read-Only Properties
        //##########################################################################################
		///############################################################
        /// <summary>
        /// Gets a value representing the name of the currently loaded picklist.
        /// </summary>
        /// <value>String representing the name of the currently loaded picklist.</value>
		///############################################################
		/// <LastUpdated>August 3, 2005</LastUpdated>
		public string PicklistName {
			get { return g_sPicklistName; }
		}

		///############################################################
        /// <summary>
        /// Gets a value representing the name of the currently loaded table.
        /// </summary>
        /// <value>String representing the name of the currently loaded table.</value>
		///############################################################
		/// <LastUpdated>August 10, 2004</LastUpdated>
		public string TableName {
			get { return g_sTableName; }
		}

		///############################################################
        /// <summary>
        /// Gets a value representing the target database server type.
        /// </summary>
        /// <value>Enumeration representing the target database server type.</value>
		///############################################################
		/// <LastUpdated>August 18, 2004</LastUpdated>
		public enumDataSource DbServer {
			get { return g_eDbServer; }
		}

		///############################################################
        /// <summary>
        /// Gets a value representing the URL to redirect to on a successful form submission.
        /// </summary>
        /// <value>String representing the URL to redirect to on a successful form submission.</value>
		///############################################################
		/// <LastUpdated>August 23, 2004</LastUpdated>
		public string RedirectURL {
			get {
				return Settings.Breadcrumb.URL() + "&Source=" + g_sSource + "&PicklistToLoad=" + g_sPicklistName + "&Success=True";
			}
		}


        //##########################################################################################
        //# Public Overridable Functions
        //##########################################################################################
		///############################################################
        /// <summary>
		/// Collects/reorders the entire results set.
        /// </summary>
        /// <remarks>
        /// Since the Picklists manager does not allow the user to reorder the listing, the <paramref>bReorderExistingResults</paramref> is not utilized.
        /// </remarks>
		/// <param name="rResults">Pagination object representing entire result set's record IDs.</param>
		/// <param name="bReorderExistingResults">Boolean value indicating if the entire results set requires re-ordering.</param>
		/// <exception cref="Cn.CnException">Thrown when a metadata picklist is null.</exception>
		/// <seealso cref="Cn.Web.Renderer.Form.GenerateResults"/>
		///############################################################
		/// <LastUpdated>August 3, 2005</LastUpdated>
		public override void GenerateResults(Pagination rResults, bool bReorderExistingResults) {
			string[] a_sResults;
			int iRowCount;
			int i;

                //#### If a g_oPicklist was not successfully collected in the constructor
			if (g_oPicklist == null) {
			        //#### If this g_bIs(a)MetaDataPicklist then there was no picklist metadata to load, so raise the error
			    if (g_bIsMetaDataPicklist) {
				    Internationalization.RaiseDefaultError(g_cClassName + "GenerateResults", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_DataNotLoaded, "picklist", "");
			    }
			        //#### Else this picklist is allowed to be empty, so ensure that the passed rResults is .Reset so that NoResults is properly called
			    else {
				    rResults.Reset();
			    }
			}
                //#### Else we have a g_oPicklist to process
			else {
                    //#### Determine the iRowCount of g_oPicklist and dimension a_sResults accordingly
				iRowCount = g_oPicklist.RowCount;
				a_sResults = new string[iRowCount];

                    //#### Traverse the .Rows of the above collected g_oPicklist
				for (i = 0; i < iRowCount; i++) {
                        //#### Copy the ID for the current g_oPicklist entry into a_sResults
					a_sResults[i] = g_oPicklist.Value(i, "ID");
				}

                    //#### .Load the above determined a_sResults into rResults (using g_sPicklistName as the virtual sTableName so that we can recollect it later)
				rResults.Load(g_sPicklistName, "ID", a_sResults);
			}
		}

		///############################################################
        /// <summary>
		/// Collects the provided page results.
        /// </summary>
		/// <param name="rPageResults">Pagination object representing this page's relevant record IDs.</param>
		/// <seealso cref="Cn.Web.Renderer.Form.CollectPageResults"/>
		///############################################################
		/// <LastUpdated>August 3, 2005</LastUpdated>
		public override void CollectPageResults(Pagination rPageResults) {
                //#### Set(the successfully)CollectedIDs within this rPage('s)Results
                //####     NOTE: Since the Renderer.Results.Stack is never stored/reused by this Form (mainly because pagination is not utilized), we are guarenteed that GenerateResults was just run and that the .IDs it defined are still within the g_oPicklist. Hence this "one liner".
			rPageResults.Table(0).SetCollectedIDs(rPageResults.Table(0).IDs);
		}

		///############################################################
        /// <summary>
		/// Outputs the header section of the rendered page.
        /// </summary>
        /// <remarks>
        /// Note that the HTML form is opened within this function (and likewise closed within <c>Footer</c>). If this Form were ever included within a Report, its HTML form would be properly rendered (as DetailHeader can be called multiple times in a Report).
        /// </remarks>
		/// <seealso cref="Cn.Web.Renderer.Form.Header"/>
		///############################################################
		/// <LastUpdated>April 18, 2007</LastUpdated>
		public override void Header() {
                //#### If this g_bIs(a)SystemPicklist, force the .NewRecordCount to be 0
			if (g_bIsSystemPicklist) {
				NewRecords = 0;
			}
                //#### Else if this g_bIs(a)MetaDataPicklist, force the .NewRecordCount to be 1
                //####     NOTE: This is done because of the additional business rules reguarding picklist definitions (ie - picklist Names and child PicklistIDs must be unique). Only 1 is allowed because the rules are only checked against existing picklist definitions (and not against other new definitions). Basicially, I was too lazy to implement the other new record checking =)
			else if (g_bIsMetaDataPicklist) {
				NewRecords = 1;
			}

				//#### If we are supposed to g_bPrintHTMLHeaderFooter, do so now
			if (g_bPrintHTMLHeaderFooter) {
				Response.Write("<html>");
				Response.Write("<head>");
				Response.Write("	<title>Cn.Web.Framework.Management.Picklists</title>");

				Response.Write("<style type='text/css'>");
				Response.Write("	." + Web.Settings.Value(Web.Settings.enumSettingValues.cnCSSClass_FormInputError) + "		{ background-color: #FFCC00; }");
				Response.Write("	." + Web.Settings.Value(Web.Settings.enumSettingValues.cnCSSClass_PopUpErrorDIV) + "	{ border: 2px dashed maroon; background-color: white; padding: 3px; width: auto; white-space: nowrap; }");
				Response.Write("</style>");

				Response.Write("</head>");
				Response.Write("<body>");
			}

                //#### Write out the JavaScript error message includes, followed by the form validation
			Response.Write(InputCollection.ValidationJavaScript("", "", "", true));

                //#### Write out the top of the page's editor form
                //####     NOTE: It is best pratice to write out the page's main form within .Header (and closing it within .Footer) so that you are guarenteed that you end up with only a single HTML form for each Renderer.Form.
//    	    Response.Write("<form id='" + g_cFormName + "' id='" + g_cFormName + "' method='post' action='" + InputCollection.UIHook(Web.InputCollection.enumUIHookTypes.cnFormAction) + "' onSubmit='" + Renderer.Form.UIHook(Renderer.enumUIHookTypes.cnFormOnSubmit) + "'>");
    	    Response.Write("<form id='" + g_cFormName + "' id='" + g_cFormName + "' method='post' action='" + InputCollection.UIHook(Web.Inputs.enumUIHookTypes.cnFormAction) + "'>");

                //#### Write out the top of the outer HTML table
			Response.Write("        <table border='0' cellpadding='5' cellspacing='0'><tr><td style='vertical-align: top;'>");
		}

		///############################################################
        /// <summary>
		/// Outputs the detail header section of the rendered page.
        /// </summary>
        /// <remarks>
        /// Note that the input form specific HTML tableset is opened within this function (and likewise closed within <c>DetailFooter</c>). If this Form were ever included within a Report, these HTML tablesets would be properly rendered (as DetailHeader/DetailFooter can be called multiple times in a Report).
        /// </remarks>
		/// <seealso cref="Cn.Web.Renderer.Form.DetailHeader"/>
		///############################################################
		/// <LastUpdated>May 25, 2007</LastUpdated>
		public override void DetailHeader() {
                //#### Reset the value of g_iRecordCount
                //####     NOTE: Since Renderer.RenderedRecordCount refers to the record's previous record number when the record is validated, it is possible to get 2 gray (or white) errored records re-rendered next to each other. So in order to avoid this, g_iRecordCount is used.
			g_iRecordCount = 1;

                //#### Write out the top of the detail HTML table
			Response.Write("    <table border='0' cellpadding='5' cellspacing='0' style='border: 2px solid #666666;'><tr style='background: #666666; color: #ffffff; font-weight: bold;'><td>");

                //#### If this g_bIs(the)MetaDataPicklist, write out the column headers accordingly
			if (g_bIsMetaDataPicklist) {
				Response.Write(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_PicklistName));
				Response.Write("    </td><td>");
				Response.Write(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_PicklistDescription));
			}
				//#### Else if this is a new g_bIsColumnAssoPicklist definition, set sDisplayOrderDesc accordingly
			else if (g_bIsColumnAssoPicklist) {
				Response.Write(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_TableColumnName));
				Response.Write("    </td><td>");
				Response.Write(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_PicklistName));
			}
                //#### Else this g_bIs(not the)MetaDataPicklist, so write out the column headers accordingly
			else {
				Response.Write(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_StoredValue));
				Response.Write("    </td><td>");
				Response.Write(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_DisplayedValue));
			}

                //#### Write out the end of the header
			Response.Write("    </td><td>");
			Response.Write(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_Delete));
			Response.Write("    </td></tr>");
		}

		///############################################################
        /// <summary>
		/// Outputs the detail section of the rendered page.
        /// </summary>
        /// <remarks>
        /// Note the proper usage of the <c>cnErrorMessageToolTip</c> within this function.
        /// <para/>Note that this function is rendering data from a custom dataset object (a <c>Data.Picklists</c> in this case). As you can see, you can render data from any data source.
        /// <para/>Note that this function renders 3 different types of input records (<c>g_bIsMetaDataPicklist</c>, <c>g_bIsSystemPicklist</c> and a user picklist), essentially functioning as 3 input forms in one.
        /// </remarks>
		/// <seealso cref="Cn.Web.Renderer.Form.Detail"/>
		///############################################################
		/// <LastUpdated>November 6, 2009</LastUpdated>
		public override void Detail() {
			string sFormHook = InputCollection.UIHook(Web.Inputs.enumUIHookTypes.cnErrorMessageToolTip);

                //#### If this is an even g_iRecordCount, write out the TR as a dark row
			if ((g_iRecordCount % 2) == 0) {
				Response.Write("    <tr style='background: #cccccc;'><td style='white-space: nowrap; text-align: right;'>");
			}
                //#### Else this is an odd .RenderedRecordCount, so write out the TR as a light row
			else {
				Response.Write("    <tr><td style='white-space: nowrap; text-align: right;'>");
			}

                //#### Inc g_iRecordCount for the next call
			g_iRecordCount++;

                //#### If this .Is(a)NewRecord
			if (IsNewRecord) {
				string sDisplayOrderDesc;

                    //#### Write out the required hidden form elements
				Response.Write(InputCollection.GenerateHTML("ID", Web.Inputs.enumInputTypes.cnHidden, "", false, sFormHook));
				Response.Write(InputCollection.GenerateHTML("PicklistID", Web.Inputs.enumInputTypes.cnHidden, g_oParentItem.Value(0, "DisplayOrder"), false, sFormHook));

                    //#### If this is a new g_bIsMetaDataPicklist definition, set sDisplayOrderDesc accordingly
				if (g_bIsMetaDataPicklist) {
					sDisplayOrderDesc = Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_PicklistID);
				}
                    //#### Else this is a standard new picklist item definition, so set sDisplayOrderDesc accordingly
				else {
					sDisplayOrderDesc = Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_DisplayOrder);
				}

                    //#### .Render(the)Inputs for the new picklist item (using the above collected sFormHook as the sAttributes argument so that the PopUp JavaScript errors are shown)
				Response.Write(InputCollection.GenerateHTML("Data", Web.Inputs.enumInputTypes.cnDefaultInput, "", false, sFormHook + " size='20'"));
				Response.Write("<br />" + sDisplayOrderDesc + ": &nbsp;");
				Response.Write(InputCollection.GenerateHTML("DisplayOrder", Web.Inputs.enumInputTypes.cnDefaultInput, "", false, sFormHook + " size='2'"));
				Response.Write("    </td><td>");
				Response.Write(InputCollection.GenerateHTML("Description", Web.Inputs.enumInputTypes.cnTextarea, "", false, sFormHook + " rows='3' cols='30'"));
				Response.Write("<br/>" + Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_IsActive) + ": &nbsp;");
				Response.Write(InputCollection.GenerateHTML("IsActive", Web.Inputs.enumInputTypes.cnSelect, "", false, sFormHook));

                    //#### Write out the HTML table seperator, followed by the [ New ] moniker
				Response.Write("    </td><td style='text-align: center; vertical-align: center; font-style: italic; font-size: 10px;'>");
				Response.Write("[ " + Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_New) + " ]");
			}
                //#### Else this is an existing record
			else {
                    //#### Define and set h_sRow to the .TableRecordIndex
                    //####     NOTE: It's best pratice to use .TableRecordIndex in place of .RenderedRecordCount for indexing into your data collection. This is because if one (or more) of the current page's IDs is missing from your data collection, .RenderedRecordCount will be pointing to the wrong index within your collection. .TableRecordIndex on the other hand, is only incremented after each call to .Detail, therefore keeping proper track of the current index within your data collection.
				Hashtable h_sRow = g_oPicklist.Row(TableRecordIndex);

                    //#### Write out the required hidden form elements
				Response.Write(InputCollection.GenerateHTML("ID", Web.Inputs.enumInputTypes.cnHidden, h_sRow["ID"].ToString(), false, ""));
				Response.Write(InputCollection.GenerateHTML("PicklistID", Web.Inputs.enumInputTypes.cnHidden, h_sRow["PicklistID"].ToString(), false, ""));

                    //#### If this is a g_bIsMetaDataPicklist definition
				if (g_bIsMetaDataPicklist) {
                        //#### .Render(the)Inputs for the current picklist item as .ReadOnlyInput(s)
					Response.Write(InputCollection.GenerateHTML("Data", Web.Inputs.enumInputTypes.cnReadOnly, h_sRow["Data"].ToString(), false, ""));
					Response.Write("<br />" + Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_PicklistID) + ": &nbsp;");
					Response.Write(InputCollection.GenerateHTML("DisplayOrder", Web.Inputs.enumInputTypes.cnReadOnly, h_sRow["DisplayOrder"].ToString(), false, ""));
					Response.Write("    </td><td>");
					Response.Write(InputCollection.GenerateHTML("Description", Web.Inputs.enumInputTypes.cnReadOnly, h_sRow["Description"].ToString(), false, ""));

                        //#### Write out the HTML table seperator
					Response.Write("    </td><td style='text-align: center; vertical-align: center; font-style: italic; font-size: 10px;'>");

                        //#### If this g_bIs(a)SystemPicklist definition (or if this is the g_sMetaDataPicklistName/g_sColumnAssoPicklistName), only write out a non-breaking space (to ensure that the tableset is properly rendered by all browsers)
					if (g_bIsSystemPicklist || h_sRow["Data"].ToString() == g_sMetaDataPicklistName || h_sRow["Data"].ToString() == g_sColumnAssoPicklistName) {
						Response.Write("&nbsp;");
					}
                        //#### Else this is a User Picklists
					else {
						string sCheckboxName = InputCollection.InputName("DeleteMe");

                            //#### Write out the Delete checkbox (since this item type is deletable)
						Response.Write("<input type='checkbox' name='" + sCheckboxName + "' id='" + sCheckboxName + "' value='True' />");
					}
				}
                    //#### Else if this is a g_bIsSystemPicklist definition
				else if (g_bIsSystemPicklist) {
                        //#### .Render(the)Inputs for the current picklist item (using the above collected sFormHook as the sAttributes argument so that the PopUp JavaScript errors are shown)
					Response.Write(InputCollection.GenerateHTML("Data", Web.Inputs.enumInputTypes.cnReadOnly, h_sRow["Data"].ToString(), false, ""));
					Response.Write("<br />" + Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_DisplayOrder) + ": &nbsp;");
					Response.Write(InputCollection.GenerateHTML("DisplayOrder", Web.Inputs.enumInputTypes.cnDefaultInput, h_sRow["DisplayOrder"].ToString(), false, sFormHook + " size='2'"));
					Response.Write("    </td><td>");
					Response.Write(InputCollection.GenerateHTML("Description", Web.Inputs.enumInputTypes.cnTextarea, h_sRow["Description"].ToString(), false, sFormHook + " rows='3' cols='30'"));

                        //#### Write out the HTML table seperator, then only write out a non-breaking space (to ensure that the tableset is properly rendered by all browsers)
					Response.Write("    </td><td style='text-align: center; vertical-align: center; font-style: italic; font-size: 10px;'>");
					Response.Write("&nbsp;");
				}
                    //#### Else this is a Picklists item (or g_bIsColumnAssoPicklist) definition
				else {
					string sCheckboxName = InputCollection.InputName("DeleteMe");

                        //#### .Render(the)Inputs for the current picklist item (using the above collected sFormHook as the sAttributes argument so that the PopUp JavaScript errors are shown)
					Response.Write(InputCollection.GenerateHTML("Data", Web.Inputs.enumInputTypes.cnDefaultInput, h_sRow["Data"].ToString(), false, sFormHook + " size='20'"));
					Response.Write("<br />" + Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_DisplayOrder) + ": &nbsp;");
					Response.Write(InputCollection.GenerateHTML("DisplayOrder", Web.Inputs.enumInputTypes.cnDefaultInput, h_sRow["DisplayOrder"].ToString(), false, sFormHook + " size='2'"));
					Response.Write("    </td><td>");
					Response.Write(InputCollection.GenerateHTML("Description", Web.Inputs.enumInputTypes.cnTextarea, h_sRow["Description"].ToString(), false, sFormHook + " rows='3' cols='30'"));
					Response.Write("<br/>" + Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_IsActive) + ": &nbsp;");
					Response.Write(InputCollection.GenerateHTML("IsActive", Web.Inputs.enumInputTypes.cnSelect, h_sRow["IsActive"].ToString(), false, sFormHook));

                        //#### Write out the HTML table seperator, then write out the Delete checkbox (since this item type is deletable)
					Response.Write("    </td><td style='text-align: center; vertical-align: center; font-style: italic; font-size: 10px;'>");
					Response.Write("<input type='checkbox' name='" + sCheckboxName + "' id='" + sCheckboxName + "' value='True' />");
				}
			}

                //#### Close off the above opened TR
			Response.Write("    </td></tr>");
		}

		///############################################################
        /// <summary>
		/// Outputs the detail footer section of the rendered page.
        /// </summary>
        /// <remarks>
        /// Note that the input form specific HTML tableset is closed within this function (and likewise opened within <c>DetailHeader</c>). If this Form were ever included within a Report, these HTML tablesets would be properly rendered (as DetailHeader/DetailFooter can be called multiple times in a Report).
        /// </remarks>
		/// <seealso cref="Cn.Web.Renderer.Form.DetailFooter"/>
		///############################################################
		/// <LastUpdated>August 17, 2004</LastUpdated>
    	public override void DetailFooter() {
                //#### Write out the bottom of the detail HTML table
			Response.Write("    </table>");
		}

		///############################################################
        /// <summary>
		/// Outputs the no results section of the rendered page.
        /// </summary>
		/// <seealso cref="Cn.Web.Renderer.Form.NoResults"/>
		///############################################################
		/// <LastUpdated>November 23, 2005</LastUpdated>
		public override void NoResults() {
		        //#### Write out the error message to the screen
			Response.Write(Web.Settings.Internationalization.ValueDecoder(Internationalization.enumInternationalizationValues.cnPicklistManagement_PicklistNameIsCurrentlyEmpty, g_sPicklistName, "", Settings.EndUserMessagesLanguageCode, false) + "<p />");
		}

		///############################################################
        /// <summary>
		/// Outputs the footer section of the rendered page.
        /// </summary>
        /// <remarks>
        /// Note that the HTML form is closed within this function (and likewise opened within <c>Header</c>). If this Form were ever included within a Report, its HTML form would be properly rendered (as DetailHeader can be called multiple times in a Report).
        /// </remarks>
		/// <seealso cref="Cn.Web.Renderer.Form.Footer"/>
		///############################################################
		/// <LastUpdated>November 15, 2005</LastUpdated>
		public override void Footer() {
			MultiArray oPicklist;
			string[] a_sDataColumn;
			string sParentDescription;
			string sParentData;

                //#### If we are .Process(ing the)Form, set the sParentDescription and sParentData from the submitted .Form
                //####     NOTE: This is required because the g_oParentItem is not updated until after it's collected within Intilize()
//!
			if (IsPostBack) {
				sParentDescription = Request.Form["ParentDescription"];
				sParentData = Request.Form["ParentData"];
			}
                //#### Else we're not .Process(ing the)Form, so set the sParentDescription and sParentData from the global vars
			else {
				sParentDescription = g_oParentItem.Value(0, "Description");
				sParentData = g_sPicklistName;
			}

                //#### Break the outer table into its second column, then output the top of the second column's DIV
			Response.Write("        </td><td style='vertical-align: top;'>");
			Response.Write("<div style='position: fixed;'>");

                //#### Output the top of the inner table
			Response.Write("    <table border='0' cellpadding='5' cellspacing='0' style='border: 2px solid #666666;'><tr><td style='background: #666666; color: #ffffff; font-weight: bold;'>");
			Response.Write(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_Name));
			Response.Write("    </td><td>");

                //#### If this g_bIs(a)SystemPicklist, g_bIs(a)MetaDataPicklist or g_bIs(a)ColumnAssoPicklist, write out the g_sPicklistName as a read only field
                //####     NOTE: .RenderInput/.Inputs.Render* are not used for the hidden inputs below because they are simple, vanilla hidden inputs, so why not write them out ourselves!?
			if (g_bIsSystemPicklist || g_bIsMetaDataPicklist || g_bIsColumnAssoPicklist) {
				Response.Write("<input type='hidden' name='ParentData' id='ParentData' value='" + g_sPicklistName + "' />" + g_sPicklistName);
			}
                //#### Else this is a non-system, non-metadata picklist, so write out the sParentData as an editable field
			else {
				Response.Write("<input type='text' name='ParentData' id='ParentData' size='35' value='" + sParentData + "' />");
			}

                //#### Write out the picklist Description form element and close the inner table
                //####     NOTE: .RenderInput/.Inputs.Render* are not used for the textarea below because it is a simple, vanilla textarea, so why not write them out ourselves!?
			Response.Write("    </td></tr><tr style='background: #cccccc;'><td style='background: #666666; color: #ffffff; font-weight: bold;'>");
			Response.Write(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_Description));
			Response.Write("    </td><td>");
			Response.Write("<textarea name='ParentDescription' id='ParentDescription' rows='3' cols='30'>" + sParentDescription + "</textarea>");
			Response.Write("    </td></tr></table>");

                //#### Write out the current g_sSource/g_sPicklistName and the Save button
			Response.Write("<input type='hidden' name='Source' value='" + g_sSource + "' />");
			Response.Write("<input type='hidden' name='PicklistToLoad' id='PicklistToLoad' value='" + g_sPicklistName + "' />");
			Response.Write("<br /><input type='button' name='Load' value='" + Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_UpdatePicklist) + "' onClick='" + InputCollection.UIHook(Web.Inputs.enumUIHookTypes.cnFormDoSubmit) + "' />");

                //#### If the update succeeded, write out the _PicklistWasUpdated text
			if (Data.Tools.MakeBoolean(Request.QueryString["Success"], false)) {
				Response.Write("&nbsp;&nbsp;<b style='color: red;'>" + Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_PicklistWasUpdated) + "</b>");
			}

                //#### Write out the bottom of the page's editor form
			Response.Write("</form><p />");

				//#### If the developer has defined their own .Form.InputCollection.Picklists
			if (InputCollection.Picklists != null) {
					//#### Collect the _PicklistMetaData from the developer's .Picklists
				oPicklist = InputCollection.Picklists.Picklist(g_sMetaDataPicklistName);

					//#### 
				a_sDataColumn = oPicklist.Column("Data");
				oPicklist = Picklists.Picklist(a_sDataColumn, a_sDataColumn);

					//#### Write out the picklist navigational form and elements
				Response.Write("<form id='LoadUser' id='LoadUser' action='" + Settings.Breadcrumb.URL() + "' method='post'>");
//!				Renderer.Form.InputCollection().RenderSelectInput("PicklistToLoad", "", g_sPicklistName, oPicklist, false);
				Response.Write(InputCollection.HTMLBuilder.Select("PicklistToLoad", "", g_sPicklistName, false, oPicklist, g_oSettings));
				Response.Write("<input type='submit' name='Load' value='" + Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_ViewPicklist) + "' />");
				Response.Write("<input type='hidden' name='Source' value='User'>");
				Response.Write("</form>");
			}

				//#### Collect the _PicklistMetaData from the g_oInternationalizationPicklist
			oPicklist = g_oInternationalizationPicklist.Picklist(g_sMetaDataPicklistName);

				//#### 
			a_sDataColumn = oPicklist.Column("Data");
			oPicklist = Picklists.Picklist(a_sDataColumn, a_sDataColumn);

				//#### Write out the picklist navigational form and elements
			Response.Write("<form id='LoadInternationalization' id='LoadInternationalization' action='" + Settings.Breadcrumb.URL() + "' method='post'>");
//!			Renderer.Form.InputCollection().RenderSelectInput("PicklistToLoad", "", g_sPicklistName, oPicklist, false);
			Response.Write(InputCollection.HTMLBuilder.Select("PicklistToLoad", "", g_sPicklistName, false, oPicklist, g_oSettings));
			Response.Write("<input type='submit' name='Load' value='" + Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnPicklistManagement_ViewInternationalization) + "' />");
			Response.Write("<input type='hidden' name='Source' value='Internationalization'>");
			Response.Write("</form>");

			    //#### Output the bottom of the second column's DIV and end the 
			Response.Write("</div>");
			Response.Write("        </td></tr></table>");

				//#### If we are supposed to g_bPrintHTMLHeaderFooter, do so now
			if (g_bPrintHTMLHeaderFooter) {
				Response.Write("</body>");
				Response.Write("</html>");
			}
		}

		///############################################################
        /// <summary>
		/// Validates the current record during form processing.
        /// </summary>
        /// <remarks>
        /// Note that this function builds its own SQL statements as a means to enforce the editable business rules of the 3 picklist types (<c>g_bIsMetaDataPicklist</c>, <c>g_bIsSystemPicklist</c> and a user picklist).
        /// </remarks>
		/// <param name="bRecordHasChanged">Boolean value indicating if the record was changed/updated by the end user.</param>
		/// <param name="bRecordIsValid">Boolean value indicating if the record successfully passed the simple validation (datatype, length, etc.).</param>
		/// <returns>RecordValidater object that represents the records validity, if SQL statements are to be generated and any developer generated SQL statements.</returns>
		/// <seealso cref="Cn.Web.Renderer.Form.ValidateRecord"/>
		///############################################################
		/// <LastUpdated>March 26, 2010</LastUpdated>
		public override RecordValidater ValidateRecord(bool bRecordDataIsValid, bool bRecordIsLogiciallyValid, bool bRecordHasChanged) {
		    RecordValidater oReturn = new RecordValidater();

                //#### If this .Is(a)NewRecord
			if (IsNewRecord) {
                    //#### If the new bRecordHasChanged
				if (bRecordHasChanged) {
						//#### If this is a g_bIsColumnAssoPicklist, we need to .Validate(the)ColumnAsso
					if (g_bIsColumnAssoPicklist) {
						bRecordDataIsValid = ValidateColumnAsso();
					}

                        //#### If this g_bIs(a)MetaDataPicklist definition
					if (g_bIsMetaDataPicklist) {
						string sDisplayOrder = InputCollection.Inputs("DisplayOrder").Value;
						string sCurrentData;
						string sData = InputCollection.Inputs("Data").Value.Trim();
						int iLen = sData.Length;
						int i;

                            //#### Traverse the .Rows of g_oPicklist (which will be the g_bIsMetaDataPicklist)
						for (i = 0; i < g_oPicklist.RowCount; i++) {
                                //#### Reset sCurrentData for this loop
							sCurrentData = g_oPicklist.Value(i, "Data").Trim();

                                //#### If the current DisplayOrder matches the submitted sDisplayOrder
							if (g_oPicklist.Value(i, "DisplayOrder") == sDisplayOrder) {
                                    //#### Set the error into the .Input
								InputCollection.Inputs("DisplayOrder").ErrorType = MetaData.enumValueErrorTypes.cnCustom;
								InputCollection.Inputs("DisplayOrder").ErrorMessage = Web.Settings.Internationalization.ValueDecoder(Internationalization.enumInternationalizationValues.cnPicklistManagement_UniqueIDRequired, sDisplayOrder, "", Settings.EndUserMessagesLanguageCode, false);

                                    //#### Flip bRecordDataIsValid
								bRecordDataIsValid = false;
							}

                                //#### If the sCurrentData matches the submitted sData (checking its .Length first as that is a far faster Comparison)
							if (sCurrentData.Length == iLen && sCurrentData == sData) {
                                    //#### Set the error into the .Input
								InputCollection.Inputs("Data").ErrorType = MetaData.enumValueErrorTypes.cnCustom;
								InputCollection.Inputs("Data").ErrorMessage = Web.Settings.Internationalization.ValueDecoder(Internationalization.enumInternationalizationValues.cnPicklistManagement_UniqueNameRequired, g_oPicklist.Value(i, "Data").Trim(), "", Settings.EndUserMessagesLanguageCode, false);

                                    //#### Flip bRecordDataIsValid
								bRecordDataIsValid = false;
							}
						}

                            //#### .Trim the Data from the user
						InputCollection.Inputs("Data").Value = InputCollection.Inputs("Data").Value.Trim();
					}
                        //#### Else this is a single picklist item definition
					else {
                            //#### Reset the value of PicklistID based on the data within g_oParentItem, then .Trim the Data
                            //####     NOTE: This is done to help ensure security on this form as the g_oParentItem should be correctly holding this data
						InputCollection.Inputs("PicklistID").Value = g_oParentItem.Value(0, "DisplayOrder");
						InputCollection.Inputs("Data").Value = InputCollection.Inputs("Data").Value.Trim();
					}

                        //#### Setup our return value/object based on the value of bRecordDataIsValid
                    oReturn.IsValid = bRecordDataIsValid;
                    oReturn.GenerateSQLStatements = bRecordDataIsValid;
				}
                    //#### Else the new record is unchanged, so setup our return value/object to a valid record with no SQL (i.e. - don't insert it)
				else {
                    oReturn.IsValid = true;
                    oReturn.GenerateSQLStatements = false;
				}
			}
                //#### Else this is an existing record
			else {
				ColumnDescription[] a_oColumns;

                    //#### If we're supposed to delete this record
				if (Data.Tools.MakeBoolean(Request.Form[InputCollection.InputName("DeleteMe")], false)) {
						//#### Setup the single a_oColumns entry in prep for its use below
					a_oColumns = new ColumnDescription[1];
					a_oColumns[0] = new ColumnDescription("", "", true, enumValueOperators.cnWhereEqualTo);

//! what about deleting a g_bIsSystemPicklist? raise error?
                        //#### If this g_bIs(a)MetaDataPicklist definition
					if (g_bIsMetaDataPicklist) {
						string[] a_sSQL = new string[2];

                            //#### Setup the local a_sSQL with the DELETE clauses
						a_oColumns[0].ColumnName = "ID";
						a_oColumns[0].Value = InputCollection.Inputs("ID").Value;
						a_sSQL[0] = Statements.Delete(g_sTableName, a_oColumns);
						a_oColumns[0].ColumnName = "PicklistID";
						a_oColumns[0].Value = InputCollection.Inputs("DisplayOrder").Value;
						a_sSQL[1] = Statements.Delete(g_sTableName, a_oColumns);

							//#### Set the above created DELETE clauses into the oReturn value
						oReturn = new RecordValidater(true, false, a_sSQL);
					}
                        //#### Else this is a single picklist item definition
					else {
						string[] a_sSQL = new string[1];

                            //#### Setup the local a_sSQL with the DELETE clause and set it into the return value/object
						a_oColumns[0].ColumnName = "ID";
						a_oColumns[0].Value = InputCollection.Inputs("ID").Value;
						a_sSQL[0] = Statements.Delete(g_sTableName, a_oColumns);
                        oReturn.IsValid = true;
                        oReturn.GenerateSQLStatements = false;
                        oReturn.SQLStatements = a_sSQL;
					}
				}
                    //#### Else we may have to update the record
				else {
                        //#### If the data was submitted from the g_bIsMetaDataPicklist (which is not allowed to update picklist item info)
					if (g_bIsMetaDataPicklist) {
                            //#### Setup our return value/object so that no record is updated
                            //####     NOTE: This is done to help ensure security on this form as the g_bIsMetaDataPicklist is not allowed to update picklist info, only it's own picklist metadata info
                        oReturn.IsValid = true;
                        oReturn.GenerateSQLStatements = false;
					}
                        //#### Else this record was submitted from a child picklist
					else {
							//#### If this is a g_bIsColumnAssoPicklist, we need to .Validate(the)ColumnAsso
						if (g_bIsColumnAssoPicklist) {
							bRecordDataIsValid = ValidateColumnAsso();
						}

                            //#### If the current bRecordDataIsValid and the bRecordHasChanged
						if (bRecordDataIsValid && bRecordHasChanged) {
							string[] a_sSQL = new string[1];

								//#### Setup the a_oColumns in prep for its use below
							a_oColumns = new ColumnDescription[5];

                                //#### Setup the a_sSQL statement with the bare minimum columns to update
                                //####     NOTE: This is done to help ensure that the picklist metadata (PicklistID) isn't erroniously changed.
                                //####     NOTE: Since the g_bIsMetaDataPicklist is non-editable, setting the DisplayOrder (Child PicklistID) from the submitted data shouldn't cause a security issue
							a_oColumns[0] = new ColumnDescription("ID", InputCollection.Inputs("ID").Value, true, enumValueOperators.cnWhereEqualTo);
							a_oColumns[1] = new ColumnDescription("Data", InputCollection.Inputs("Data").Value.Trim(), true, enumValueOperators.cnInsertIfPresent);
							a_oColumns[2] = new ColumnDescription("Description", InputCollection.Inputs("Description").Value, true, enumValueOperators.cnInsertIfPresent);
							a_oColumns[3] = new ColumnDescription("DisplayOrder", InputCollection.Inputs("DisplayOrder").Value, true, enumValueOperators.cnInsertIfPresent);
							a_oColumns[4] = new ColumnDescription("IsActive", InputCollection.Inputs("IsActive").Value, true, enumValueOperators.cnInsertIfPresent);
							a_sSQL[0] = Statements.Update(g_sTableName, a_oColumns);

                                //#### Set the above generated a_sSQL into the return value/object
                            oReturn.IsValid = true;
                            oReturn.GenerateSQLStatements = false;
                            oReturn.SQLStatements = a_sSQL;
						}
                            //#### Else the record is either invalid or hasn't changed (either way, SQL will not be generated)
						else {
                                //#### Setup our return value/object based on the passed values so the proper steps are taken with the record
                            oReturn.IsValid = bRecordDataIsValid;
                            oReturn.GenerateSQLStatements = bRecordHasChanged;
						}
					}
				}
			}

			    //#### Return the above determined oReturn value to the caller
		    return oReturn;
		}


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
		///############################################################
        /// <summary>
        /// Retrieves the SQL for the entire picklist management form (including the picklist's own name and description).
        /// </summary>
		/// <param name="a_sSQL">String array where each index represents a developer provided or system generated SQL statement (i.e. - the argument you recieve within <c>SubmitResults</c>).</param>
        /// <returns>String representing the SQL for the entire picklist management form.</returns>
		///############################################################
		/// <LastUpdated>November 6, 2009</LastUpdated>
		public string GetSQL(string[] a_sSQL) {
			string sReturn;

                //#### If we are currently .Process(ing the)Form
			if (IsPostBack) {
                    //#### If this g_bIs(a)SystemPicklist, g_bIs(the)MetaDataPicklist or g_bIs(the)ColumnAssoPicklist only allow the editing of the Description
				if (g_bIsSystemPicklist || g_bIsMetaDataPicklist || g_bIsColumnAssoPicklist) {
					ColumnDescription[] oColumns = new ColumnDescription[2];

                        //#### Setup the oColumns then Build(the)UpdateStatement, returning it's result to the caller
					oColumns[0] = new ColumnDescription("ID", g_oParentItem.Value(0, "ID"), true, enumValueOperators.cnWhereEqualTo);
					oColumns[1] = new ColumnDescription("Description", Request.Form["ParentDescription"], true, enumValueOperators.cnInsertNullString);
					sReturn = Statements.Update(g_sTableName, oColumns) + "; ";
				}
                    //#### Else this is a User Defined picklist, so allow the editing of the Data and Description
				else {
					ColumnDescription[] oColumns = new ColumnDescription[3];

                        //#### Reset the value of g_sPicklistName to the submitted ParentData (mainly so RedirectURL works correctly)
					g_sPicklistName = Data.Tools.MakeString(Request.Form["ParentData"], "").Trim();

                        //#### Setup the oColumns then Build(the)UpdateStatement, returning its result to the caller
					oColumns[0] = new ColumnDescription("ID", g_oParentItem.Value(0, "ID"), true, enumValueOperators.cnWhereEqualTo);
					oColumns[1] = new ColumnDescription("Data", g_sPicklistName, true, enumValueOperators.cnInsertIfPresent);
					oColumns[2] = new ColumnDescription("Description", Request.Form["ParentDescription"], true, enumValueOperators.cnInsertNullString);
					sReturn = Statements.Update(g_sTableName, oColumns) + "; ";
				}
			}
                //#### Else we're not currently .Process(ing the)Form, so return a null-string
			else {
				sReturn = "";
			}

                //#### If some a_sSQL statements were passed in, append them onto the sReturn value
            if (a_sSQL != null) {
                sReturn += string.Join("; ", a_sSQL) + "; ";
            }

			    //#### Return the above determined sReturn value to the caller
			return sReturn;
		}


private bool ValidateColumnAsso() {
	string[] a_sTableColumnName = InputCollection.Inputs("Data").Value.Split('.');
	bool bReturn = true;

		//#### If the provided a_sTableColumnName was not in the proper format (Table.Column), set the error and flip our bReturn value to false
	if (a_sTableColumnName.Length != 2) {
		InputCollection.Inputs("Data").ErrorType = MetaData.enumValueErrorTypes.cnCustom;
		InputCollection.Inputs("Data").ErrorMessage = Web.Settings.Internationalization.ValueDecoder(Internationalization.enumInternationalizationValues.cnPicklistManagement_TableColumnNameFormat, InputCollection.Inputs("Data").Value, "", Settings.EndUserMessagesLanguageCode, false);
		bReturn = false;
	}
		//#### Else if the provided Picklist name does not .Exist, set the error and flip our bReturn value to false
	else if (! InputCollection.Picklists.Exists(InputCollection.Inputs("Description").Value)) {
		InputCollection.Inputs("Description").ErrorType = MetaData.enumValueErrorTypes.cnCustom;
		InputCollection.Inputs("Description").ErrorMessage = Web.Settings.Internationalization.ValueDecoder(Internationalization.enumInternationalizationValues.cnPicklistManagement_TableColumnNamePicklist, InputCollection.Inputs("Description").Value, "", Settings.EndUserMessagesLanguageCode, false);
		bReturn = false;
	}
	
		//#### Return the above determined bReturn value to the caller
	return bReturn;
}
	} //# class Picklists
#endregion

} //# namespace Cn.Web.Management
