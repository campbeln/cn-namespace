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
using System.Collections;					        //# Required to access the Hashtable class
using Cn.Data;										//# Required to access MetaData, Pagination, Picklists, etc.
using Cn.Data.SQL;									//# Required to access the OrderByClause class
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Web.Renderer {

	///########################################################################################################################
	/// <summary>
	/// Renderer.Form class.
	/// </summary>
	/// <remarks>
	/// Fun fact: This was the last VB.Net module to be converted to C# in the version 3.x to 4.0 code conversion.
	/// </remarks>
	///########################################################################################################################
	/// <LastFullCodeReview>September 6, 2005</LastFullCodeReview>
	public abstract class Form : List {
	#region Form
			//#### Declare the required private variables
		private FormInputCollection g_oInputCollection = null;
		private string g_sName;
		private int g_iErroredRecordCounter;		//# <summary>1-based integer representing the current errored record's count.</summary>
		private int g_iErroredRecordCount;
		private int g_iNewRecordSubmittedCounter;
		private int g_iNewRecordCounter;
		private int g_iNewRecordCount;				//# <summary>1-based integer representing the new record count.</summary>
		private bool g_bOnErrorRerenderAllRecords;
		private bool g_bNewRecordsRenderedFirst;	//# <summary>Boolean value indicating if new records are to be printed before the existing records.</summary>
		private bool g_bTrackRecordChanges;
		private bool g_bIsNewRecord;				//# <summary>Boolean value indicating if a new record is currently being printed.</summary>
		private bool g_bIsPostBack;
		private bool g_bIsErroredRecord;

			//#### Declare the required private eNums
		#region eNums
			/// <summary><c>Form</c> record tracker modes.</summary>
		private enum enumRecordTrackerModes : int {
				/// <summary>Unknown/unreconized record mode.</summary>
			cnUnknown = -1,
				/// <summary>New record mode.</summary>
			cnNew = 0,
				/// <summary>Missing record mode</summary>
			cnMissing = 1,
				/// <summary>Existing record mode.</summary>
			cnExisting = 2
		}
		#endregion

			//#### Declare the required private structures
		#region Structs
		private struct structColumnCollection {
			public ColumnDescription[] Columns;
			public string[] InputAliases;
			public string TableName;
		}
		private struct structRecordTracker {
			public string OrigionalRecordMD5;
			public int TableIndex;
			public int TableRecordIndex;
			public enumRecordTrackerModes RecordType;
		}
		#endregion

			//#### Declare the required private constants
		private const string g_cClassName = "Cn.Web.Renderer.Form.";


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <seealso cref="Cn.Web.Renderer.Form.Reset()"/>
		///############################################################
		/// <LastUpdated>March 4, 2010</LastUpdated>
		public Form() : base(enumRendererObjectTypes.cnForm, g_cClassName) {
				//#### Setup the g_oInputCollection now
				//####     NOTE: This is setup here due to the interplay of base.Reset, our own DoReset and the .Settings logic (so we need to ensure we have a valid reference from the beginning)
			g_oInputCollection = new FormInputCollection(this);

				//#### Call our .Reset to init the class vars
			Reset();
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <seealso cref="Cn.Web.Renderer.Form.Reset(Web.Settings.Current)"/>
		///############################################################
		/// <LastUpdated>March 4, 2010</LastUpdated>
		public Form(Settings.Current oSettings) : base(enumRendererObjectTypes.cnForm, g_cClassName) {
				//#### Setup the g_oInputCollection now
				//####     NOTE: This is setup here due to the interplay of base.Reset, our own DoReset and the .Settings logic (so we need to ensure we have a valid reference from the beginning)
			g_oInputCollection = new FormInputCollection(this);

				//#### Call our .Reset to init the class vars
			Reset(oSettings);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 4, 2010</LastUpdated>
		public override void Reset() {
				//#### Call the base .Reset implementation to reset the base variables (implicitly using the .Current .Settings)
			base.Reset();

			    //#### Call DoReset to do the actual work
			DoReset();
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		///############################################################
		/// <LastUpdated>March 4, 2010</LastUpdated>
		public override void Reset(Settings.Current oSettings) {
				//#### Call the base .Reset implementation to reset the base variables
			base.Reset(oSettings);

			    //#### Call DoReset to do the actual work
			DoReset();
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <remarks>
		/// NOTE: This *MUST* be called *AFTER* base.Reset as this function requires that oSettings has been set/defined.
		/// </remarks>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
        private void DoReset() {
			    //#### (Re)Init our own global private variables
			    //####     NOTE: g_iNewRecordSubmittedCounter is included below as .DoResetIndexes is called at the top of .DoRender after .DoCollectPageResults is run (and would therefore be erroriounously reset is it was in .DoResetIndexes)
			g_sName = "";
			g_iNewRecordSubmittedCounter = 0;
			g_iErroredRecordCount = 0;
			g_iNewRecordCount = 0;
			g_bOnErrorRerenderAllRecords = false;
			g_bNewRecordsRenderedFirst = false;
		    g_bTrackRecordChanges = true;
			g_bIsNewRecord = false;
			g_bIsErroredRecord = false;

				//#### .DoReset(our)Indexes (which is actually called by base.Reset), then re-reset g_iErroredRecordCounter to its' initial, invalid index (hence -1, which signals .ErroredRecordCount to return the non-"er" value)
				//####     NOTE: We .DoResetIndexes in it's own sub as to ensure they are always reset to the same initial values
		  //DoResetIndexes();
			g_iErroredRecordCounter = -1;

				//#### .Reset the g_oInputCollection
			g_oInputCollection.Reset(this);

			    //#### Set g_bIsPostBack via it's .IsPostBack property so that all the child objects are properly set as well
			    //#### IsPostBack is true if the IsPostBack Querystring flag is set, a RecordTracker exists for the first record or the Referer is the same as the current page
//!			IsPostBack = (Data.Tools.MakeBoolean(Request.QueryString[Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + "IsPostBack"], false) ||
//				Data.Tools.MakeInteger(ParseRecordTracker(1)[0], (int)enumRecordTrackerModes.cnUnknown) != (int)enumRecordTrackerModes.cnUnknown ||
//				Data.Tools.MakeString(Request.UrlReferrer, "").ToLower() == Data.Tools.MakeString(Request.Url, "").ToLower()
//			);

		    IsPostBack = (Data.Tools.MakeBoolean(Request.QueryString[Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + "IsPostBack"], false) ||
			    ParseRecordTracker(1).RecordType != enumRecordTrackerModes.cnUnknown
		    );
	    }

		///############################################################
		/// <summary>
		/// Resets the class's indexes to their pre-loop state.
		/// </summary>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		protected override void DoResetIndexes() {
				//#### Pass the call off to our .base implementation
			base.DoResetIndexes();

				//#### (Re)Set the view-specific g_iErroredRecordCounter and g_iNewRecordCounter
				//####     NOTE: All of the values are set so as allow to pre-incrementing (hence the -1/0's)
				//####     NOTE: When g_iErroredRecordCounter and g_iNewRecordCounter are not -1, it signals .ErroredRecordCount/.NewRecordCount to use their values. These values are set to -1 in .DoReset and 0 below to allow this logic to function properly
				//####     NOTE: We do not reset g_iNewRecordCount below, as it's only used as a counter during render, so we must set it then and not here (else we always have 0 new records to render!)
			g_iErroredRecordCounter = 0;
			g_iNewRecordCounter = 0;
		}


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the settings for this instance.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>Settings</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>Cn.Web.Settings.Current instance representing the current enviroment.</value>
		///############################################################
		/// <LastUpdated>March 4, 2010</LastUpdated>
		public override Settings.Current Settings {
			get { return base.Settings; }
			set {
				base.Settings = value;

					//#### Ensure our underlying g_oInputCollection has the new .Settings
				g_oInputCollection.Settings = value;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the unique name of the form.
		/// </summary>
		/// <remarks>
		/// This is provided to allow multiple forms to be present on a single page. The <c>Name</c> is prefixed onto the record trackers for each form and therefore must be unique across a page's forms so as to ensure that multiple values are not present for each form's record trackers.
		/// </remarks>
		/// <value>String representing the unique name of the form.</value>
		///############################################################
		/// <LastUpdated>April 21, 2010</LastUpdated>
		public string Name {
			get { return g_sName; }
			set {
				g_sName = value;

					//#### 
				IsPostBack = (Data.Tools.MakeBoolean(Request.QueryString[Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + "IsPostBack"], false) ||
					ParseRecordTracker(1).RecordType != enumRecordTrackerModes.cnUnknown
				);
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
			get { return g_iNewRecordCount; }
			set { g_iNewRecordCount = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if all records are to be re-rendered when errors occur on post-back.
		/// </summary>
		/// <value>Boolean value indicating if all records are to be re-rendered when errors occur on post-back.</value>
		///############################################################
		/// <LastUpdated>March 30, 2010</LastUpdated>
		public bool OnErrorRerenderAllRecords {
			get { return g_bOnErrorRerenderAllRecords; }
			set { g_bOnErrorRerenderAllRecords = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if new records are to be rendered before existing records.
		/// </summary>
		/// <value>Boolean value indicating if new records are to be rendered before existing records.</value>
		///############################################################
		/// <LastUpdated>February 8, 2010</LastUpdated>
		public bool NewRecordsRenderedFirst {
			get { return g_bNewRecordsRenderedFirst; }
			set { g_bNewRecordsRenderedFirst = value; }
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
		/// <LastUpdated>May 21, 2007</LastUpdated>
		public bool TrackRecordChanges {
			get {
					//#### Set the return value based if we are supposed to g_bTrackRecordChanges and if we're not currently in .IsReadOnly mode
				return (g_bTrackRecordChanges && ! g_oSettings.IsReadOnly);
			}
			set { g_bTrackRecordChanges = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if we are to process the current form.
		/// </summary>
		/// <value>Boolean value indicating if we are to process the current form.</value>
		///############################################################
		/// <LastUpdated>August 9, 2007</LastUpdated>
		public bool IsPostBack {
			get { return g_bIsPostBack; }
			set {
					//#### Reset g_bIsPostBack to the passed value then reset the g_oInputCollection.IsPostBack based on our own g_bIsPostBack
				g_bIsPostBack = value;
				g_oInputCollection.IsPostBack = g_bIsPostBack;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if the <c>Form</c> is to be rendered as read only.
		/// </summary>
		/// <value>Boolean value indicating if the <c>Form</c> is to be rendered as read only.</value>
		///############################################################
		/// <LastUpdated>May 21, 2007</LastUpdated>
		public bool IsReadOnly {
			get { return g_oSettings.IsReadOnly; }
			set { g_oSettings.IsReadOnly = value; }
		}


		//##########################################################################################
		//# Public Read-Only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the input's tools and management class related to this instance.
		/// </summary>
		/// <returns>InputCollection object representing the input collection class related to this instance.</returns>
		///############################################################
		/// <LastUpdated>February 10, 2010</LastUpdated>
		public FormInputCollection InputCollection {
			get { return g_oInputCollection; }
		}

		///############################################################
		/// <summary>
		/// Gets the current input record's number (based on rendered records).
		/// </summary>
		/// <value>1-based integer for the current input records number.</value>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		public override int RecordCount {
			get {
				int iReturn;

					//#### If we are not to g_bOnErrorRerenderAllRecords and g_iErroredRecordCounter has been inc'd (meaning this is a re-.Render), iReturn it to the caller
					//####     NOTE: g_iErroredRecordCounter in only inc'd at render and is reset within .DoResetIndexes, while g_iErroredRecordCount is inc'd in .DoValidation and is maintained until a full .Reset. This allows for .DoValidation calls to properly retrieve g_iRecordCount while re-.Render calls get g_iErroredRecordCounter, which represents the proper .RecordCount at re-render
				if (! g_bOnErrorRerenderAllRecords && g_iErroredRecordCounter > 0) {
					iReturn = g_iErroredRecordCounter;
				}
					//#### Else there have been no Form errors (or we're in g_bOnErrorRerenderAllRecords mode), so set the iReturn value to our .base's .RecordCount
				else {
					iReturn = base.RecordCount;
				}

					//#### Return the above determined iReturn value to the caller
				return iReturn;
			}
		}

		///############################################################
		/// <summary>
		/// Gets the count of records submitted to this instance.
		/// </summary>
		/// <value>1-based integer representing the count of records submitted to this instance.</value>
		///############################################################
		/// <LastUpdated>April 21, 2010</LastUpdated>
		public int SubmittedRecordCount {
			get {
				structRecordTracker oRecordTracker;
				int iReturn = 1;

					//#### Do while there is a valid RecordTracker for the "current" record index within the iReturn value
				do {
						//#### Determine the current oRecordTracker data and post-inc the iReturn value in prep. for the next loop
					oRecordTracker = ParseRecordTracker(iReturn);
					iReturn++;
				} while (oRecordTracker.RecordType != enumRecordTrackerModes.cnUnknown);

					//#### Return the above determined iReturn value to the caller (minus 1 to account for the last invalid post-inc above)
				return (iReturn - 1);
			}
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the current count of errored records.
		/// </summary>
		/// <value>1-based integer for the current count of errored records.</value>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		public int ErroredRecordCount {
				//####     NOTE: g_iErroredRecordCounter in only inc'd at render and is reset within .DoResetIndexes, while g_iErroredRecordCount is inc'd in .DoValidation and is maintained until a full .Reset. This allows for .DoValidation calls to properly retrieve g_iRecordCount while re-.Render calls get g_iErroredRecordCounter, which represents the proper .RecordCount at re-render
			get { return (g_iErroredRecordCounter == -1 ? g_iErroredRecordCount : g_iErroredRecordCounter); }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the current count of new record forms.
		/// </summary>
		/// <value>Integer representing the current count of new record forms.</value>
		///############################################################
		/// <LastUpdated>June 24, 2010</LastUpdated>
		public int NewRecordCount {
			get { return g_iNewRecordCounter; }
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating if errors have occured during the validation of the user submitted data.
		/// </summary>
		/// <value>Boolean value indicating if errors have occured during the validation of the user submitted data.</value>
		///############################################################
		/// <LastUpdated>June 21, 2010</LastUpdated>
		public bool ErrorsOccured {
			get { return (g_iErroredRecordCount > 0); }
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating if the current record is a new record.
		/// </summary>
		/// <value>Boolean value indicating if the current record is a new record.</value>
		///############################################################
		/// <LastUpdated>April 23, 2010</LastUpdated>
		public bool IsNewRecord {
//! old logic		//#### Set the return value based on the integer value in the first element of the cnRecordTracker_
//				return (Cn.Data.Tools.MakeInteger(ParseRecordTracker(g_rGlobalVars.RenderedRecordCount)[0], (int)enumRecordTrackerModes.cnUnknown) == (int)enumRecordTrackerModes.cnNew);
			get { return g_bIsNewRecord; }
			internal set { g_bIsNewRecord = value; }
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating if the current record contains errors.
		/// </summary>
		/// <value>Boolean value indicating if the current record contains errors.</value>
		///############################################################
		/// <LastUpdated>April 23, 2010</LastUpdated>
		public bool IsErroredRecord {
			get { return g_bIsErroredRecord; }
		}


		//##########################################################################################
		//# Render-related Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Renders the revelent section of the provided <c>Form</c>.
		/// </summary>
		/// <remarks>
		/// NOTE: Since we need to call our own DoCollectResults, DoRender and Reset function, we must override this implementation from List even though the code matches.
		/// </remarks>
		/// <param name="iRecordsPerPage">1-based integer representing the maximum number of records so show per rendered page (does not include new records).</param>
		/// <param name="sResultsStack">String representing the related Data.Pagination instance.</param>
/// <exception cref="Cn.CnException">Thrown when there are no defined <c>Inputs</c> for this instance.</exception>
		/// <exception cref="Cn.CnException">If auto-generated SQL statements were requested by <see cref='Cn.Web.Renderer.Form.ValidateRecord'>ValidateRecord</see>'s return value: Thrown when one of the defined <c>Inputs</c> has an input alias containing the <see cref='Cn.Configuration.Settings.PrimaryDelimiter'>PrimaryDelimiter</see>.</exception>
		///############################################################
		/// <LastUpdated>May 31, 2010</LastUpdated>
		public override void Render(int iRecordsPerPage, string sResultsStack) {
				//#### Pass the call off to our own .DoRender (while also calling .DoCollectResults, to... well, collect the results =), signaling .DoRender to .Reset ourselves in prep. for any subsequent calls
			DoRender(DoCollectResults(iRecordsPerPage, sResultsStack, null, ""), true);
		}

		///############################################################
		/// <summary>
		/// Renders the revelent section of the provided list.
		/// </summary>
		/// <remarks>
		/// NOTE: Since we need to call our own DoCollectResults, DoRender and Reset function, we must override this implementation from List even though the code matches.
		/// </remarks>
		/// <param name="iRecordsPerPage">1-based integer representing the maximum number of records so show per rendered page (does not include new records).</param>
		/// <param name="a_sIDs">Array of strings representing all the expected IDs to render (including IDs that may no longer be available).</param>
		/// <param name="sIDsOrderedBy">String representing the order the passed <paramref>sIDsOrderedBy</paramref> is in.</param>
/// <exception cref="Cn.CnException">Thrown when there are no defined <c>Inputs</c> for this instance.</exception>
		/// <exception cref="Cn.CnException">If auto-generated SQL statements were requested by <see cref='Cn.Web.Renderer.Form.ValidateRecord'>ValidateRecord</see>'s return value: Thrown when one of the defined <c>Inputs</c> has an input alias containing the <see cref='Cn.Configuration.Settings.PrimaryDelimiter'>PrimaryDelimiter</see>.</exception>
		///############################################################
		/// <LastUpdated>May 31, 2010</LastUpdated>
		public override void Render(int iRecordsPerPage, string[] a_sIDs, string sIDsOrderedBy) {
				//#### Pass the call off to our own .DoRender (while also calling .DoCollectResults, to... well, collect the results =), signaling .DoRender to .Reset ourselves in prep. for any subsequent calls
			DoRender(DoCollectResults(iRecordsPerPage, "", a_sIDs, sIDsOrderedBy), true);
		}

	    ///############################################################
	    /// <summary>
	    /// Renders the provided list.
	    /// </summary>
	    /// <param name="oCurrentTable">PaginationTable object representing the relevant record IDs for the current table index.</param>
	    ///############################################################
	    /// <LastUpdated>June 18, 2010</LastUpdated>
	    public override void Render(Pagination.PaginationTable oCurrentTable) {
				//#### If this g_bIs(a)PostBack, .ErrorsOccured and we are only re-rendering the errored records
			if (g_bIsPostBack && ErrorsOccured && ! g_bOnErrorRerenderAllRecords) {
					//#### Reset the g_iTableIndex based on the value within the borrowed .IDColumn of the oCurrentTable
				g_iTableIndex = Data.Tools.MakeInteger(oCurrentTable.IDColumn, -1);
			}

				//#### Pass the call off to our .base implementation
			base.Render(oCurrentTable);
	    }

		///############################################################
		/// <summary>
		/// Collects the page results in preperation for rendering.
		/// </summary>
		/// <param name="iRecordsPerPage">1-based integer representing the maximum number of records so show per rendered page (does not include new records).</param>
		/// <param name="sResultsStack">String representing the related Data.Pagination instance.</param>
		/// <param name="a_sIDs">Array of strings representing all the expected IDs to render (including IDs that may no longer be available).</param>
		/// <param name="sIDsOrderedBy">String representing the order the passed <paramref>sIDsOrderedBy</paramref> is in.</param>
		/// <returns>Pagination object representing the revelent section of the provided List or Form.</returns>
		/// <exception cref="Cn.CnException">If auto-generated SQL statements were requested by <see cref='Cn.Web.Renderer.Form.ValidateRecord'>ValidateRecord</see>'s return value: Thrown when one of the defined <c>Inputs</c> has an input alias containing the <see cref='Cn.Configuration.Settings.PrimaryDelimiter'>PrimaryDelimiter</see>.</exception>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		internal override Pagination DoCollectResults(int iRecordsPerPage, string sResultsStack, string[] a_sIDs, string sIDsOrderedBy) {
			Pagination oReturn;

				//#### If this g_bIs(a)PostBack
			if (g_bIsPostBack) {
					//#### Call DoValidation to process the .Form
					//####     NOTE: .DoValidation builds the oReturn based on the submitted RecordTracker_s (which only represent the submitted [so not necessarly the full] oPageResults), flipping any records as .ErroredID as necessary. Partial/narrowing representation of the oPageResults is fine as the g_iTableIndex and g_iTableRecordIndex is stored within the RecordTracker_
				oReturn = DoValidation();
			}
				//#### Else this g_bIs(not a)PostBack
			else {
					//#### Call our base class's .DoCollectResults to "recollect" the passed results
					//####     NOTE: Since we always deal with all of the results, we need to do the .base functionality first
				oReturn = base.DoCollectResults(iRecordsPerPage, sResultsStack, a_sIDs, sIDsOrderedBy);
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Outputs the no results section (surrounded by the header and footer) of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 18, 2010</LastUpdated>
		protected override void DoRenderNoResults() {
				//##### If we're in .Printable mode
			if (Printable) {
					//#### Print out the .Header (after .Calculate(ing its)PageCrawl)
				CalculatePageCrawl(PrintLength(enumPageSections.cnHeader));
				Header();

					//#### Print out the .NoResults (after Calculate(ing its)PageCrawl), while calling .RenderNewRecords to pre-post render any new records
				RenderNewRecords(true, false);
				CalculatePageCrawl(PrintLength(enumPageSections.cnNoResults));
				NoResults();
				RenderNewRecords(false, false);

					//#### Print out the .Footer (after Calculate(ing its)PageCrawl)
				CalculatePageCrawl(PrintLength(enumPageSections.cnFooter));
				Footer();
			}
				//#### Else we're not in .Printable mode
			else {
					//#### Call the volley of subs to .Render the "NoResults" rForm, while calling .RenderNewRecords to pre-post render any new records
				Header();
				RenderNewRecords(true, true);
				NoResults();
				RenderNewRecords(false, true);
				Footer();
			}
		}

		///############################################################
		/// <summary>
		/// Outputs the detail and missing record sections (as required) of the rendered page.
		/// </summary>
		/// <param name="oCurrentTable">PaginationTable object representing the relevant record IDs for the current table index.</param>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		protected override void DoRenderBody(Pagination.PaginationTable oCurrentTable) {
			structRecordTracker oRecordTracker;
			int i;

				//#### .RenderNewRecords, signaling this is a render before request
			RenderNewRecords(true, false);

				//#### Traverse the .IDs for the oCurrentTable
			for (i = 0; i < oCurrentTable.IDs.Length; i++) {
					//#### Determine if this g_bIs(an)ErroredRecord and inc g_iRecordCount
					//####     NOTE: g_iRecordCount is not used as the loop's iterater as it keeps track of the .RenderedRecordCount across all of the results entries
					//####     NOTE: If there are errors and this is a non-g_bOnErrorRerenderAllRecords, only errored records at their valid g_iRecordCount are re-rendered below
				g_bIsErroredRecord = oCurrentTable.ErroredID(i);
				g_iRecordCount++;

					//#### If we are supposed to (re)render the current IDs
					//####     NOTE: We render all records on a non-postback, or all records on a postback if g_bOnErrorRerenderAllRecords is set, or only g_bIsErroredRecord's on a postback (the inner paren's are not actually required, but makes the logic a bit more clear)
				if (! g_bIsPostBack || (g_bOnErrorRerenderAllRecords || g_bIsErroredRecord)) {
						//#### If this .ID was successfully collected within .CollectPageResults
					if (oCurrentTable.CollectedID(i)) {
							//#### If we're in .Printable mode, .Calculate(the)PageCrawl for the .Detail
						if (Printable) {
							CalculatePageCrawl(this, ref g_bInitialDetailCall, PrintLength(enumPageSections.cnDetail));
						}

							//#### If this g_bIs(a)PostBack
						if (g_bIsPostBack) {
								//#### If this g_bIs(an)ErroredRecord, inc g_iErroredRecordCounter
								//####     NOTE: g_iErroredRecordCounter in only inc'd at render and is reset within .DoResetIndexes, while g_iErroredRecordCount is inc'd in .DoValidation and is maintained until a full .Reset. This allows for .DoValidation calls to properly retrieve g_iRecordCount while re-.Render calls get g_iErroredRecordCounter, which represents the proper .RecordCount at re-render
							if (g_bIsErroredRecord) {
								g_iErroredRecordCounter++;
							}

								//#### .Parse(the)RecordTracker, setting g_iTableRecordIndex to it's stored value
								//####     NOTE: The initial MD5 is passed in here rather then sCurrentRecordsMD5 so that subsequent reposts of this record are still properly identified as having been changed from the original (which only the initial MD5 represents).
							oRecordTracker = ParseRecordTracker(g_iRecordCount);
							g_iTableRecordIndex = oRecordTracker.TableRecordIndex;

								//#### Call our .Detail and .Render(ing the)HiddenRecordTracker with the .OrigionalRecordMD5
							Detail();
							RenderHiddenRecordTracker("Render", enumRecordTrackerModes.cnExisting, oRecordTracker.OrigionalRecordMD5);
						}
							//#### Else this g_bIs(not a)PostBack
						else {
								//#### Inc g_iTableRecordIndex, call our .Detail and .Render(the initial)HiddenRecordTracker
							g_iTableRecordIndex++;
							Detail();
							RenderHiddenRecordTracker("Render", enumRecordTrackerModes.cnExisting, this);
						}
					}
						//#### Else this .ID was not successfully collected within .CollectPageResults
					else {
							//#### If we're in .Printable mode, .Calculate(the)PageCrawl for the .MissingRecord
						if (Printable) {
							CalculatePageCrawl(this, ref g_bInitialDetailCall, PrintLength(enumPageSections.cnMissingRecord));
						}

							//#### .Render(the)HiddenRecordTracker and call our .Detail
						RenderHiddenRecordTracker("Render", enumRecordTrackerModes.cnMissing, this);
						MissingRecord(oCurrentTable.TableName, oCurrentTable.IDs[i]);
					}
				}
			}

				//#### Reset g_bIsNewRecord and g_bIsErroredRecord to their default values
			g_bIsNewRecord = false;
			g_bIsErroredRecord = false;

				//#### .RenderNewRecords, signaling this is a render after request
			RenderNewRecords(false, false);
		}

		///############################################################
		/// <summary>
		/// Renders the required new forms for the provided <c>Form</c>.
		/// </summary>
		/// <param name="bRenderNewRecordsFirst">Boolean value representing if we are to render new records before or after existing records.</param>
		/// <param name="bRenderHeaderFooter">Boolean value representing if we are to render the headers and footers.</param>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		private void RenderNewRecords(bool bRenderNewRecordsFirst, bool bRenderHeaderFooter) {
			int iCurrentTableIndex = g_iTableIndex;
			bool bCurrentIsReadOnly = g_oSettings.IsReadOnly;
			bool bErroredRecords = (g_oPagination == null ? false : g_oPagination.NewRecords.ErroredIDCount > 0);

				//#### If we have new records to render and .NewRecords(are to be)RenderedFirst and this is a bRenderNewRecordsFirst call *OR* if .NewRecords(are *NOT* to be)RenderedFirst and this is *NOT* a bRenderRecordsFirst call
				//####     NOTE: The logic below is a bit odd, but it works as true==true and false==false, while the mis-matched pairs will be ignored
			if (g_iNewRecordCount > 0 && NewRecordsRenderedFirst == bRenderNewRecordsFirst) {
					//#### If we are supposed to (re)render any of the g_bIsNewRecord's
					//####     NOTE: We render all records on a non-postback, or all records on a postback if g_bOnErrorRerenderAllRecords is set, or only g_bIsErroredRecord's on a postback (looking at bErroredRecords for this initial test)
				if (! g_bIsPostBack || (g_bOnErrorRerenderAllRecords || bErroredRecords)) {
						//#### If we are to bRenderHeaderFooter (ensuring we're also not in .Printable mode), render the .DetailHeader
					if (bRenderHeaderFooter && ! Printable) {
						DetailHeader();
					}

						//#### Reset the values for the g_iTableIndex and g_iTableRecordIndex (as since these are g_bIsNewRecord's these values are not operable, hence setting them to invalid/default values)
					g_iTableIndex = -1;
					g_iTableRecordIndex = -1;

						//#### Flip g_bIsNewRecord and force g_oSettings into non-read only mode
						//####     NOTE: We can set these outside the loop below because the only external calls made are to .CalculatePageCrawl, .Detail and .RenderHiddenRecordTracker all of which should know the current status of the record
					g_bIsNewRecord = true;
					g_oSettings.IsReadOnly = false;

						//#### Traverse the g_iNewRecordCount of new records
					for (g_iNewRecordCounter = 1; g_iNewRecordCounter <= g_iNewRecordCount; g_iNewRecordCounter++) {
							//#### Determine if this g_bIs(an)ErroredRecord and inc g_iRecordCount
							//####     NOTE: If there are errors and this is a non-g_bOnErrorRerenderAllRecords, only errored records at their valid g_iRecordCount are re-rendered below
						g_bIsErroredRecord = (g_oPagination == null || g_oPagination.NewRecords.IDCount == 0 ? false : g_oPagination.NewRecords.ErroredID(g_iNewRecordCounter - 1));
						g_iRecordCount++;

							//#### If we are supposed to (re)render the current g_iNewRecordCounter
							//####     NOTE: We render all records on a non-postback, or all records on a postback if g_bOnErrorRerenderAllRecords is set, or only g_bIsErroredRecord's on a postback
						if (! g_bIsPostBack || (g_bOnErrorRerenderAllRecords || g_bIsErroredRecord)) {
								//#### If we're in .Printable mode, .Calculate(the)PageCrawl for the .Detail
								//####     NOTE: We use the version of .CalculatePageCrawl that outputs the .DetailHeader when necessary
							if (Printable) {
								CalculatePageCrawl(this, ref g_bInitialDetailCall, PrintLength(enumPageSections.cnDetail_NewForm));
							}

								//#### Call .Detail to print out the new record
								//####     NOTE: There is no need to check the g_iTableRecordIndex value as a g_bIsNewRecord, the g_iTableRecordIndex is not applicable (as it'll either be -1 or the last valid index)
							Detail();

								//#### If this g_bIs(a)PostBack, .Render(the)HiddenRecordTracker with the .OrigionalRecordMD5
								//####     NOTE: The initial MD5 is passed in here rather then sCurrentRecordsMD5 so that subsequent reposts of this record are still properly identified as having been changed from the original (which only the initial MD5 represents).
							if (g_bIsPostBack) {
								RenderHiddenRecordTracker("Render", enumRecordTrackerModes.cnNew, ParseRecordTracker(g_iRecordCount).OrigionalRecordMD5);
							}
								//#### Else this g_bIs(not a)PostBack, so .Render(the initial)HiddenRecordTracker
							else {
								RenderHiddenRecordTracker("Render", enumRecordTrackerModes.cnNew, this);
							}

								//#### Un-reflip g_bIsErroredRecord
							g_bIsErroredRecord = false;
						}
					}

						//#### Un-reset g_iNewRecordCounter (which was inc'd 1 past it's last valid value), g_iTableIndex, then un-reflip g_bIsNewRecord and reset g_oSettings's .IsReadOnly value to its previous value
						//####     NOTE: There is no need to un-reset g_iTableRecordIndex as we are eather at the beginning of the .DoRenderBody loop (and it was -1 already) or at the end of the loop (where it is about to be reset to -1)
					g_iNewRecordCounter--;
					g_iTableIndex = iCurrentTableIndex;
					g_bIsNewRecord = false;
					g_oSettings.IsReadOnly = bCurrentIsReadOnly;

						//#### If we are to bRenderHeaderFooter (ensuring we're also not in .Printable mode), render the .DetailFooter
					if (bRenderHeaderFooter && ! Printable) {
						DetailFooter();
					}
				}
			}
		}


		//##########################################################################################
		//# Page Section-related Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Validates the current record during form processing.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own record validation.
		/// <para/>This function is called after the <c>Class Constructor</c> once per submitted record when processing the form. This function is not called if there are no submitted records to validate.
		/// <para/>NOTE: This function is only utilized when processing a <c>Renderer.Form</c>. It is not utilized if we are rendering a <c>Renderer.Form</c>.
		/// </remarks>
		/// <param name="bRecordDataIsValid">Boolean value indicating if the all of the record's data successfully passed 'simple' validation (datatype, length, etc.).</param>
		/// <param name="bRecordIsLogiciallyValid">Boolean value indicating if the record is logicially valid.</param>
		/// <param name="bRecordHasChanged">Boolean value indicating if the record was changed/updated by the end user.</param>
		/// <returns>RecordValidater object that represents the records validity, if SQL statements are to be generated and any developer generated SQL statements.</returns>
		///############################################################
		/// <LastUpdated>March 26, 2010</LastUpdated>
		public virtual RecordValidater ValidateRecord(bool bRecordDataIsValid, bool bRecordIsLogiciallyValid, bool bRecordHasChanged) {
				//#### Set the passed bRecordIsLogiciallyValid into our return value's .IsValid, set it's .GenerateSQLStatements based on if the bRecordHasChanged and pass in a null for the additional .SQLStatements
			return new RecordValidater(bRecordIsLogiciallyValid, bRecordHasChanged, null);
		}

		///############################################################
		/// <summary>
		/// Submits the successfully collected and validated data into the data source.
		/// </summary>
		/// <remarks>
		/// Required function overload to process a page's submitted records.
		/// <para/>This function is called after the last <c>ValidateRecord</c>, or after the <c>Class Constructor</c> if there are no records to submit.
		/// <para/>NOTE: This function is only utilized when processing a <c>Renderer.Form</c>. It is not utilized if we are rendering a <c>Renderer.Form</c>.
		/// </remarks>
		/// <param name="a_sSQL">String array where each index represents a developer provided or system generated SQL statement.</param>
		///############################################################
		/// <LastUpdated>April 19, 2010</LastUpdated>
		public abstract void SubmitResults(string[] a_sSQL);


		//##########################################################################################
		//# Private Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Validates the user submitted data.
		/// </summary>
		/// <exception cref="Cn.CnException">Thrown when there are no defined <c>Inputs</c> for this instance.</exception>
		/// <exception cref="Cn.CnException">If auto-generated SQL statements were requested by <see cref='Cn.Web.Renderer.Form.ValidateRecord'>ValidateRecord</see>'s return value: Thrown when one of the defined <c>Inputs</c> has an input alias containing the <see cref='Cn.Configuration.Settings.PrimaryDelimiter'>PrimaryDelimiter</see>.</exception>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		private Pagination DoValidation() {
			structColumnCollection[] a_oColumnCollection = null;
			Pagination oReturn_PageResults = new Pagination();
			RecordValidater oRecordValidater;
			Hashtable h_sSQL = new Hashtable();
			structRecordTracker oRecordTracker;
			string[] a_sSQLStatements = null;
			string[] a_sKeys = null;
			string sCurrentRecordsMD5 = "";
			string sSQLKeys = "";
			int iColumnCollectionCount = -1;
			int iSQLIndex;
			int iKeyCount;
			int i;
			bool bRecordIsLogiciallyValid;
			bool bRecordHasChanged;
			bool bSimpleErrorsOccured;
			bool bTrackRecordChanges = this.TrackRecordChanges;

				//#### If the g_iErroredRecordCount hasn't been inc'd then this is our first rodeo
//! is this a necessary check?
			if (g_iErroredRecordCount == 0) {
					//#### Collect the initial a_sRecordTracker (hence "1")
				oRecordTracker = ParseRecordTracker(1);

					//#### .DoResetIndexes, then re-reset the g_iTableIndex based on the oRecordTracker's value
					//####     NOTE: These are called/set here as we do not call Renderer.Base.Render
				DoResetIndexes();
				g_iTableIndex = oRecordTracker.TableIndex;

					//#### Determine the iKeyCount
				iKeyCount = InputCollection.Count;

					//#### If there are .Keys to collect, get them now
				if (iKeyCount > 0) {
					a_sKeys = InputCollection.InputAliases;
				}

					//#### Do while we still have records to process
				while (oRecordTracker.RecordType != enumRecordTrackerModes.cnUnknown) {
						//#### If the current g_iTableIndex is valid and does not yet exist within our oReturn_PageResults value, .Load it in now 
					if (g_iTableIndex > -1 && oReturn_PageResults.TableCount <= g_iTableIndex) {
						oReturn_PageResults.Load("undefined", g_iTableIndex.ToString(), new string[0]);
					}

						//#### If this is a .cnMissing record
					if (oRecordTracker.RecordType == enumRecordTrackerModes.cnMissing) {
							//#### Add g_iRecordCount into our oReturn_PageResults value (ID'd by the g_iRecordCount, flipping .CollectedID as we go since this is an .cnExisting record)
							//####     NOTE: The value of .IDs is inconsiquential, while the overall .IDCount and their positions realitive to their g_iRecordCount is important
							//####     NOTE: We don't need to flip .CollectedID below as it defaults to false
						oReturn_PageResults.Table(g_iTableIndex).AddID(g_iRecordCount.ToString());
					  //oReturn_PageResults.SetCollectedID(g_iRecordCount, false);
					}
						//#### Else this is not a .cnMissing record
					else {
							//#### Inc g_iRecordCount, then reset bSimpleErrorsOccured and the RecordTracker_ values for this loop
						g_iRecordCount++;
						bSimpleErrorsOccured = false;
						g_bIsNewRecord = (oRecordTracker.RecordType == enumRecordTrackerModes.cnNew);
						g_iTableIndex = oRecordTracker.TableIndex;
						g_iTableRecordIndex = oRecordTracker.TableRecordIndex;

							//#### If this is a g_bIsNewRecord
						if (g_bIsNewRecord) {
								//#### Inc g_iNewRecordCounter and add g_iRecordCount into g_oPagination's .NewRecords (ID'd by the g_iRecordCount)
								//####     NOTE: The value of .IDs is inconsiquential, while the overall .IDCount and their positions realitive to their g_iRecordCount is important
								//####     NOTE: We use the g_oPagination object below because .RenderNewRecords doesn't have access to the oPageResults. Besides, the location of the .NewRecords is not important because they are not rendered by Pagination.ToString() (so they are run-time data only).
							g_iNewRecordCounter++;
							g_oPagination.NewRecords.AddID(g_iRecordCount.ToString());
						}
							//#### Else this isn't a g_bIsNewRecord
							//####     NOTE: Since this is used to refer to the index within any developer defined and utilized results set, we only want to inc it on non-new existing records
						else {
								//#### Add g_iRecordCount into our oReturn_PageResults value (ID'd by the g_iRecordCount [-1 to convert it to an index], flipping .CollectedID as we go since this is an .cnExisting record)
								//####     NOTE: The value of .IDs is inconsiquential, while the overall .IDCount and their positions realitive to their g_iRecordCount is important
								//####     NOTE: We don't inc g_iTableRecordIndex here as it's collected from the oRecordTracker (which stores the original value)
							oReturn_PageResults.Table(g_iTableIndex).AddID(g_iRecordCount.ToString());
							oReturn_PageResults.SetCollectedID(g_iRecordCount - 1, true);
						}

							//#### If we're supposed to bTrack(the)RecordChanges
						if (bTrackRecordChanges) {
								//#### Calculate the sCurrentRecordsMD5 (and bSimpleErrorsOccured while we're at it)
								//####     NOTE: Since CalculateCurrentMD5 traverses the a_sKeys already, .ValidateInputData is called within it
							sCurrentRecordsMD5 = CalculateCurrentMD5AndValidate(a_sKeys, ref bSimpleErrorsOccured, InputCollection);

								//#### Send the current record into .ValidateRecord (calculating bRecordHasChanged based on the comparison of the MD5 within a_sRecordTracker and the sCurrentRecordsMD5), collecting its returned oRecordValidater
							bRecordHasChanged = (oRecordTracker.OrigionalRecordMD5 != sCurrentRecordsMD5);
							bRecordIsLogiciallyValid = ((! bSimpleErrorsOccured) || (! bRecordHasChanged && g_bIsNewRecord));
							oRecordValidater = ValidateRecord(! bSimpleErrorsOccured, bRecordIsLogiciallyValid, bRecordHasChanged);
						}
							//#### Else we're not supposed to bTrackRecordChanges
						else {
								//#### Traverse the a_sKeys for the current record
							for (i = 0; i < iKeyCount; i++) {
									//#### If the InputData for the current a_sKey .Is(not)Valid, flip bSimpleErrorsOccured to true
								if (! g_oInputCollection.Inputs(a_sKeys[i]).IsValid) {
									bSimpleErrorsOccured = true;
								}
							}

								//#### Send the current record into .ValidateRecord (setting bRecordHasChanged to true as we're erring on the side of caution since we are not bTrackRecordChanges), collecting its returned oRecordValidater
								//####     NOTE: We don't need to run the full bRecordIsLogiciallyValid below because bRecordHasChanged is always true
							bRecordHasChanged = true;
							bRecordIsLogiciallyValid = (! bSimpleErrorsOccured);		//# ((! bSimpleErrorsOccured) || (! bRecordHasChanged && g_bIsNewRecord));
							oRecordValidater = ValidateRecord(! bSimpleErrorsOccured, bRecordIsLogiciallyValid, bRecordHasChanged);
						}

							//#### If the bRecordHasChanged and this g_bIsNewRecord, inc the g_iNewRecordSubmittedCounter and set the .NewRecords as a .CollectedID
							//####     NOTE: We really don't need to set the .CollectedID below, but it is done to be consistent with the operation of non-.NewRecords
						if (bRecordHasChanged && g_bIsNewRecord) {
							g_iNewRecordSubmittedCounter++;
							g_oPagination.NewRecords.CollectedID(g_iNewRecordCounter - 1, true);
						}

							//#### If the developer returned a valid oRecordValidater
						if (oRecordValidater != null) {
								//#### If the developer returned .IsValid
							if (oRecordValidater.IsValid) {
									//#### If the developer returned some (additional) .SQLStatements, store them first
								if (oRecordValidater.SQLStatements != null) {
										//#### Collect the (additional) .SQLStatements into the local a_sSQLStatements
									a_sSQLStatements = oRecordValidater.SQLStatements;

										//#### Reset the iSQLIndex for the loop below
									iSQLIndex = 0;

										//#### Traverse the (additional) a_sSQLStatements
									for (i = 0; i < a_sSQLStatements.Length; i++) {
											//#### If a statement has been defined in the current index, insert it into its own iSQLIndex within h_sSQL, append the "index" onto sSQLKeys and inc iSQLIndex
										if (! string.IsNullOrEmpty(a_sSQLStatements[i])) {
											h_sSQL[g_iRecordCount + "_" + iSQLIndex] = a_sSQLStatements[i];
											sSQLKeys += "|" + g_iRecordCount + "_" + iSQLIndex;
											iSQLIndex++;
										}
									}
								}

									//#### If the developer wants us to .Generate(the)SQLStatements
								if (oRecordValidater.GenerateSQLStatements) {
										//#### If the a_oColumnCollection hasn't been setup yet, collect it and the iColumnCollectionCount
									if (a_oColumnCollection == null) {
										a_oColumnCollection = GetColumnCollection("Render");
										iColumnCollectionCount = a_oColumnCollection.Length;
									}
										//#### Else the a_oColumnCollection has been setup, so we need to update it with the new .Values
									else {
											//#### Traverse the a_oColumnCollection
										for (i = 0; i < iColumnCollectionCount; i++) {
												//#### Traverse the current a_oColumnCollection's .InputAliases (borrowing the use of iSQLIndex as the iterator)
											for (iSQLIndex = 0; iSQLIndex < a_oColumnCollection[i].InputAliases.Length; iSQLIndex++) {
													//#### Update the current .Columns' .Value from the .Form.Input's .Value
												a_oColumnCollection[i].Columns[iSQLIndex].Value = InputCollection.Inputs(a_oColumnCollection[i].InputAliases[iSQLIndex]).Value;
											}
										}
									}

										//#### (Re)Dimension the a_sSQLStatements in prep for the loops below
									a_sSQLStatements = new string[iColumnCollectionCount];

										//#### If this is a g_bIsNewRecord
									if (g_bIsNewRecord) {
											//#### Traverse the a_oColumnCollection, generating 1 a_sSQLStatement at a time for each .TableName
										for (i = 0; i < iColumnCollectionCount; i++) {
											a_sSQLStatements[i] = Statements.Insert(a_oColumnCollection[i].TableName, a_oColumnCollection[i].Columns);
										}
									}
										//#### Else this must be an existing record, so build an .Update statement based on the current record and place it in the local h_sSQL
									else {
											//#### Traverse the a_oColumnCollection, generating 1 a_sSQLStatement at a time for each .TableName
										for (i = 0; i < iColumnCollectionCount; i++) {
											a_sSQLStatements[i] = Statements.Update(a_oColumnCollection[i].TableName, a_oColumnCollection[i].Columns);
										}
									}

										//#### If some a_sSQLStatements were returned above
									if (a_sSQLStatements.Length > 0) {
											//#### Reset the iSQLIndex for the loop below
										iSQLIndex = 0;

											//#### Traverse the a_sSQLStatements
										for (i = 0; i < a_sSQLStatements.Length; i++) {
												//#### If a statement has been defined in the current index, insert it into its own iSQLIndex within h_sSQL then append the "index" onto sSQLKeys (inc'ing iSQLIndex as we go)
											if (! string.IsNullOrEmpty(a_sSQLStatements[i])) {
												h_sSQL[g_iRecordCount + "_" + iSQLIndex] = a_sSQLStatements[i];
												sSQLKeys += "|" + g_iRecordCount + "_" + iSQLIndex;
												iSQLIndex++;
											}
										}
									}
								}
							}
								//#### Else the developer returned false, indicating that the record is invalid
							else {
									//#### Inc. the g_iErroredRecordCount
									//####     NOTE: g_iErroredRecordCounter in only inc'd at render and is reset within .DoResetIndexes, while g_iErroredRecordCount is inc'd in .DoValidation and is maintained until a full .Reset. This allows for .DoValidation calls to properly retrieve g_iRecordCount while re-.Render calls get g_iErroredRecordCounter, which represents the proper .RecordCount at re-render
								g_iErroredRecordCount++;

									//#### If this g_bIs(a)NewRecord, flip it into an .ErroredID
									//####     NOTE: We use the g_oPagination object below because .RenderNewRecords doesn't have access to the oPageResults. Besides, the location of the .NewRecords is not important because they are not rendered by Pagination.ToString() (so they are run-time data only).
								if (g_bIsNewRecord) {
									g_oPagination.NewRecords.ErroredID(g_iNewRecordCounter - 1, true);
								}
									//#### Else this g_bIs(not a)NewRecord
								else {
										//#### Use the base .SetErroredID to flip the related .IDs to be an .ErroredID
										//####     NOTE: Since we deal in absolute g_iRecordCount's at .DoValidate, while the oReturn_PageResults deals with relative .IDCount's (per g_iTableIndex), we need to use the special .SetErroredID interface to translate the absolute g_iRecordCount into the proper relative .ErroredID
										//####     NOTE: We need to translate the g_iRecordCount into an index (hence the -1) as well as ignore any rendered g_bIsNewRecord's (hence the -g_iNewRecordCounter). Also we are guarenteed that g_iRecordCount > g_iNewRecordCounter as this g_bIs(not a)NewRecord.
									oReturn_PageResults.SetErroredID(g_iRecordCount - g_iNewRecordCounter - 1, true);
								}
							}
						}
					}

						//#### Collect the next a_sRecordTracker (hence +1) in prep. for the next loop
					oRecordTracker = ParseRecordTracker(g_iRecordCount + 1);
				}

					//#### Ensure that the g_bIsNewRecord flag has been reset to false
				g_bIsNewRecord = false;

					//#### Redetermine the iKeyCount
				iKeyCount = h_sSQL.Count;

					//#### If there are no a_sKeys to copy, set a_sKeys to an empty array
				if (iKeyCount == 0) {
					a_sKeys = new string[0];
				}
					//#### Else we have some a_sKeys to copy
				else {
						//#### Collect the a_sKeys in order from the .Split sSQLKeys (pealing off the leading delimiter as we go)
						//####     NOTE: h_sSQL.Count will always equal a_sKeys.Length because of how sSQLKeys is filled above
					a_sKeys = sSQLKeys.Substring(1).Split("|".ToCharArray());

						//#### Traverse a_sKeys, resetting each .Key to it's related .Item value from within h_sSQL
					for (i = 0; i < iKeyCount; i++) {
						a_sKeys[i] = Data.Tools.MakeString(h_sSQL[a_sKeys[i]], "");
					}
				}

					//#### Pass in the borrowed a_sKeys (which is holding the above collected/generated SQL statements) into .SubmitResults
				SubmitResults(a_sKeys);
			}

				//#### Return the above determined oReturn_PageResults value to the caller
			return oReturn_PageResults;
		}

		///############################################################
		/// <summary>
		/// Splits the defined <c>Inputs</c> into table-specific groupings.
		/// </summary>
		/// <remarks>
		/// NOTE: If you request that SQL statements be auto-generated in the return value for <see cref='Cn.Web.Renderer.Form.ValidateRecord'>ValidateRecord</see>, you are not permitted to define an input alias that contains the <see cref='Cn.Configuration.Settings.PrimaryDelimiter'>PrimaryDelimiter</see>.
		/// </remarks>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <returns>Array of structColumnCollections where each element represents a table-specific grouping of <c>Inputs</c>.</returns>
		/// <exception cref="Cn.CnException">Thrown when one of the defined <c>Inputs</c> has an input alias containing the <see cref='Cn.Configuration.Settings.PrimaryDelimiter'>PrimaryDelimiter</see>.</exception>
		/// <exception cref="Cn.CnException">Thrown when there are no defined <c>Inputs</c> for this instance.</exception>
		///############################################################
		/// <LastUpdated>February 10, 2006</LastUpdated>
		private structColumnCollection[] GetColumnCollection(string sFunction) {
			structColumnCollection[] a_oReturn = null;
			Hashtable h_sTableInfo = new Hashtable();
			string[] a_sInputAliases = InputCollection.InputAliases;
			string[] a_sTableNames;
			bool[] a_bGenerateSQL;
			string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter;
			string sCurrentInputAlias;
			string sCurrentTableName;
			int iCount;
			int i;
			int j;

				//#### If we have a_sInputAliases to traverse
			if (a_sInputAliases != null && a_sInputAliases.Length > 0) {
					//#### Traverse the a_sInputAliases
				for (i = 0; i < a_sInputAliases.Length; i++) {
						//#### Collect the sCurrentInputAlias and sCurrentTableName for this loop
					sCurrentInputAlias = Data.Tools.MakeString(a_sInputAliases[i], "");
					sCurrentTableName = InputCollection.Inputs(sCurrentInputAlias).TableName.ToLower();

						//#### If the sCurrentInputAlias contains the sPrimaryDelimiter, raise the error
					if (sCurrentInputAlias.IndexOf(sPrimaryDelimiter) != -1) {
						Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_InputAliasWithPrimaryDelimiter, sCurrentInputAlias, sPrimaryDelimiter);
					}
						//#### Else we can safely store the sCurrentInputAlias
					else {
							//#### If this is an existing sCurrentTableName, append the sCurrentInputAlias onto its value (delimited by a sPrimaryDelimiter)
						if (h_sTableInfo.Contains(sCurrentTableName)) {
							h_sTableInfo[sCurrentTableName] = Data.Tools.MakeString(h_sTableInfo[sCurrentTableName], "") + sPrimaryDelimiter + sCurrentInputAlias;
						}
							//#### Else this is a new sCurrentTableName, so .Add it into the h_sTableInfo with the sCurrentInputAlias (delimited by a sPrimaryDelimiter)
						else {
							h_sTableInfo.Add(sCurrentTableName, sPrimaryDelimiter + sCurrentInputAlias);
						}
					}
				}

					//#### Determine h_sTableInfo's .Count, then dimension the a_oReturn value and a_bGenerateSQL
					//####     NOTE: We do not check iCount before dimensioning the a_oReturn value bacause we know there will be at least a single input due to the test above (with a valid .TableName no less)
				iCount = h_sTableInfo.Count;
				a_oReturn = new structColumnCollection[iCount];
				a_bGenerateSQL = new bool[iCount];

					//#### Determine the a_sTableNames from h_sTableInfo's .Keys
				a_sTableNames = new string[iCount];
				h_sTableInfo.Keys.CopyTo(a_sTableNames, 0);

					//#### Traverse h_sTableInfo
				for (i = 0; i < iCount; i++) {
						//#### .Split out the current a_sTableNames value into the a_sInputAliases (pealing off the leading sPrimaryDelimiter as we go) and default the a_bGenerateSQL flag to false
					a_sInputAliases = h_sTableInfo[ a_sTableNames[i] ].ToString().Substring(sPrimaryDelimiter.Length).Split(sPrimaryDelimiter.ToCharArray());
					a_bGenerateSQL[i] = false;

						//#### Setup the a_oReturn value's current index, then set that index's .TableName, .InputAliases and dimension its .Columns
					a_oReturn[i] = new structColumnCollection();
					a_oReturn[i].TableName = a_sTableNames[i];
					a_oReturn[i].InputAliases = a_sInputAliases;
					a_oReturn[i].Columns = new ColumnDescription[a_sInputAliases.Length];

						//#### Traverse the a_sInputAliases
					for (j = 0; j < a_sInputAliases.Length; j++) {
							//#### GetData the current .Column based on the .Form's .Input
						a_oReturn[i].Columns[j] = new ColumnDescription(InputCollection.Inputs(a_sInputAliases[j]));

							//#### If the a_bGenerateSQL flag has not been set to true for this table, reset it based on the .Columns' .IsInsertOrUpdateColumn
						if (! a_bGenerateSQL[i]) {
							a_bGenerateSQL[i] = a_oReturn[i].Columns[j].IsInsertOrUpdateColumn;
						}
					}
				}

					//#### Traverse a_bGenerateSQL
				for (i = 0; i < iCount; i++) {
						//#### If the current a_oReturn value index is not set to a_bGenerateSQL (bacause none of its .Columns were set as .IsInsertOrUpdateColumn)
					if (! a_bGenerateSQL[i]) {
							//#### Traverse the a_oReturn value, moving each index up one place
						for (j = i; j < (iCount - 1); j++) {
							a_oReturn[j] = a_oReturn[j + 1];
						}

							//#### Decrement the iCount, as we logicially removed one index above
						iCount--;
					}
				}

					//#### If all of the a_oReturn value's indexes were removed above, reset the a_oReturn value to a 0-length array
					//####     NOTE: We return a 0-length array here as opposed tot he more traditional null due to the logic of the calling function (which is a null means the arry has not yet been setup). So, in order to avoid unnecessary subsequent calls, we return a 0-length array.
				if (iCount == 0) {
					a_oReturn = new structColumnCollection[0];
				}
					//#### Else if some of the a_oReturn value's indexes were logicially removed above, so let's physicially remove them
				if (iCount != a_oReturn.Length) {
					structColumnCollection[] a_oTemp;
					
						//#### Copy the a_oReturn value into the a_oTemp array, then (re)dimension the a_oReturn value to its proper sizing
					a_oTemp = a_oReturn;
					a_oReturn = new structColumnCollection[iCount];

						//#### Traverse the a_oReturn value, refilling it with the references stored wtihin the a_oTemp array
					for (i = 0; i < iCount; i++) {
						a_oReturn[i] = a_oTemp[i];
					}
				}
			}
				//#### Else no a_sInputAliases are defined, so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_InputTools_NoInputAliases, "", "");
			}

				//#### Return the above determined a_oReturn value to the caller
			return a_oReturn;
		}

		///############################################################
		/// <summary>
		/// Calculates the MD5 for the current record.
		/// </summary>
		/// <param name="oInputCollection">Form.InputCollection object reference to the related <c>Form</c>'s <c>Inputs</c> object.</param>
		/// <returns>String representing the MD5 hash for the current record.</returns>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		private static string CalculateCurrentMD5(FormInputCollection oInputCollection) {
			string[] a_sInputAliases;
			string sReturn = "";
			int i;

				//#### If the developer has loaded some .Inputs
			if (oInputCollection.Count > 0) {
					//#### Collect the a_sInputAliases
				a_sInputAliases = oInputCollection.InputAliases;

					//#### Traverse the a_sInputAliases for the .RenderedRecordCount
				for (i = 0; i < a_sInputAliases.Length; i++) {
						//#### Append the current a_sInputAlias .Value onto the sReturn value followed by a common delimiter
						//####     NOTE: The common delimiter is in place to mark the boundries of the values. It doesn't matter if the values also contain a "," as the string is never split by them. They simply ensure that each value is represented within the compiled string (i.e. - ",hi,," != ",,hi,").
					sReturn += oInputCollection.Inputs(a_sInputAliases[i]).Value + ",";
				}

					//#### .Calculate(the)CurrentMD5 for the above determined .Value's for the .RenderedRecordCount
					//####     NOTE: The eventual trailing "," is left on from the loop above as its consistent presence does not adversely effect the resulting .MD5
				sReturn = Data.Tools.MD5(sReturn);
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Calculates the MD5 for the current record while also determining if simple errors have occured.
		/// </summary>
		/// <param name="a_sInputAliases">Array of strings representing the input aliases to utilize.</param>
		/// <param name="oInputCollection">Form.InputCollection object reference to the related <c>Form</c>'s <c>Inputs</c> object.</param>
		/// <param name="bSimpleErrorsOccured">Reference to a boolean value indicating if errors occured during validation (updated by reference).</param>
		/// <returns>String representing the MD5 hash for the current record.</returns>
		///############################################################
		/// <LastUpdated>March 24, 2010</LastUpdated>
		private static string CalculateCurrentMD5AndValidate(string[] a_sInputAliases, ref bool bSimpleErrorsOccured, FormInputCollection oInputCollection) {
			Inputs.InputData oInputData;
			string sReturn = "";
			int i;

				//#### If we have a_sInputAliases
			if (a_sInputAliases != null) {
					//#### Traverse the a_sInputAliases for the .RenderedRecordCount
				for (i = 0; i < a_sInputAliases.Length; i++) {
						//#### Retrieve the oInputData for the current a_sInputAlias
					oInputData = oInputCollection.Inputs(a_sInputAliases[i]);

						//#### Append the current .Value onto the return value followed by a common delimiting ","
						//####     NOTE: The common delimiting "," is in place to mark the boundries of the values. It doesn't matter if the values also contain a "," as the string is never split by the delimiters. They simply ensure that each value is represented within the compiled string (i.e. - ",hi,," != ",,hi,").
					sReturn += oInputData.Value + ",";

						//#### If the oInputData .Is(not)Valid, flip the by ref bSimpleErrorsOccured to true
						//####     NOTE: This is called here as we are already traversing the a_sInputAliases, so we might as well .Validate the oInputData now
					if (! oInputData.IsValid) {
						bSimpleErrorsOccured = true;
//!Cn.Tools.dWrite("Errored: " + a_sInputAliases[i] + " with " + oInputData.Errors + " (Value = " + oInputData.Value + ")");
					}
				}
			}

				//#### Calculate and return the .MD5 of the above determined sReturn value to the caller
				//####     NOTE: The eventual trailing "," is left on from the loop above as its consistent presence does not adversely effect the resulting .MD5
			return Data.Tools.MD5(sReturn);
		}


		//##########################################################################################
		//# Private Record Tracker-related Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Renders the hidden record tracker form element for the current.
		/// </summary>
		/// <remarks>
		/// NOTE: This MUST be called after the Detail has been called (else the .Values won't be set)!
		/// </remarks>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="eRecordType">Enumeration representing the type of the current record.</param>
		/// <param name="oForm">Form object reference to the related <c>Form</c> object.</param>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		private void RenderHiddenRecordTracker(string sFunction, enumRecordTrackerModes eRecordType, Form oForm) {
				//#### If we are g_bAllow('d to use)ResponseWrite
			if (g_bAllowResponseWrite) {
					//#### If we are supposed to .Track(the)RecordChanges and this is not a .cnMissing
				if (oForm.TrackRecordChanges && eRecordType != enumRecordTrackerModes.cnMissing) {
						//#### Pass the call off to our sibling implementation to do the actual work, calculating the .Current(rendered)Record's MD5 as we go
					RenderHiddenRecordTracker(sFunction, eRecordType, CalculateCurrentMD5(oForm.InputCollection));
				}
					//#### Else we do not need to calculate the MD5
				else {
						//#### Pass the call off to our sibling implementation to do the actual work
					RenderHiddenRecordTracker(sFunction, eRecordType, "");
				}
			}
		}

		///############################################################
		/// <summary>
		/// Renders the hidden record tracker form element for the current.
		/// </summary>
		/// <remarks>
		/// NOTE: This MUST be called after the Detail has been called (else the .Values won't be set)!
		/// </remarks>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="eRecordType">Enumeration representing the type of the current record.</param>
		/// <param name="sRecordsMD5">String representing the MD5 hash for the current record.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eRecordType</paramref> is unreconized or unknown.</exception>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		private void RenderHiddenRecordTracker(string sFunction, enumRecordTrackerModes eRecordType, string sRecordsMD5) {
			string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter;
			string sRecordType = "";
			string sInputName = Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + g_sName + "_RecordTracker_" + RecordCount;

				//#### If we are g_bAllow('d to use)ResponseWrite
			if (g_bAllowResponseWrite) {
					//#### Determine the eRecordType and set sRecordType accordingly
				switch (eRecordType) {
					case enumRecordTrackerModes.cnExisting: {
						sRecordType = "Existing";
						break;
					}
					case enumRecordTrackerModes.cnNew: {
						sRecordType = "New";
						break;
					}
					case enumRecordTrackerModes.cnMissing: {
						sRecordType = "Missing";
						break;
					}

						//#### Else the eRecordType is not reconized/is .cnUnknown (both of which are invalid for rendering a RecordTracker_), so raise the error
						//####     NOTE: This error should never be called, as this is only consumed internally by Renderer
					default: {
						Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eRecordType", Data.Tools.MakeString(eRecordType, ""));
						break;
					}
				}

					//#### .Write out the RecordTracker_ for the .RenderedRecordCount
					//####     NOTE: The RecordTracker_ is formatted as: "RecordType|TableIndex|TableRecordIndex|OriginalRecordMD5"
				Response.Write(
					InputCollection.HTMLBuilder.Hidden(sInputName, "",
						sRecordType + sPrimaryDelimiter +
						g_iTableIndex + sPrimaryDelimiter +
						g_iTableRecordIndex + sPrimaryDelimiter +
						sRecordsMD5
					)
				);
			}
		}

		///############################################################
		/// <summary>
		/// Parses the record tracker form element for the provided record number.
		/// </summary>
		/// <remarks>
		/// RecordTracker Example: "RecordType|TableIndex|TableRecordIndex|OriginalRecordMD5"
		/// <para/>NOTE: This function is only utilized by Renderer.Form's and was only moved here to keep the RecordTracker functionality in a single place.
		/// </remarks>
		/// <param name="iRecordNumber">Integer representing the record number of the tracker to parse.</param>
/// <returns>Array of three strings where the first element represents the record's type as an integer, the second element represents the table index and the third element represents the original record's MD5 hash.</returns>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		private structRecordTracker ParseRecordTracker(int iRecordNumber) {
			structRecordTracker oReturn = new structRecordTracker();
			string[] a_sRecordTracker;

				//#### Default our oReturn value
			oReturn.RecordType = enumRecordTrackerModes.cnUnknown;
			oReturn.TableIndex = -1;
			oReturn.TableRecordIndex = -1;
			oReturn.OrigionalRecordMD5 = "";

				//#### Split the iRecordNumber cnRecordTracker_ into a_sRecordTracker
			a_sRecordTracker = Data.Tools.MakeString(Request.Form[Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + g_sName + "_RecordTracker_" + iRecordNumber], "").Split(Configuration.Settings.PrimaryDelimiter.ToCharArray());

				//#### If the submitted a_sRecordTracker was properly formed
				//####     NOTE: Malformed cnRecordTracker_'s are handeled by the default values of our oReturn value
			if (a_sRecordTracker.Length == 4 && Data.Tools.IsInteger(a_sRecordTracker[1])) {
					//#### Determine the submitted a_sRecordTracker's mode, setting our oReturn value's .RecordType accordingly
				switch (a_sRecordTracker[0].ToLower()) {
					case "existing": {
						oReturn.RecordType = enumRecordTrackerModes.cnExisting;
						break;
					}
					case "new": {
						oReturn.RecordType = enumRecordTrackerModes.cnNew;
						break;
					}
					case "missing": {
						oReturn.RecordType = enumRecordTrackerModes.cnMissing;
						break;
					}
				}

					//#### If the .RecordType was reconized above, finish populating our oReturn value with the data from the submitted a_sRecordTracker
				if (oReturn.RecordType != enumRecordTrackerModes.cnUnknown) {
					oReturn.TableIndex = Data.Tools.MakeInteger(a_sRecordTracker[1], -1);
					oReturn.TableRecordIndex = Data.Tools.MakeInteger(a_sRecordTracker[2], -1);
					oReturn.OrigionalRecordMD5 = a_sRecordTracker[3];
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}
	#endregion


		///########################################################################################################################
		/// <summary>
		/// Return value structure for <c>Form.ValidateRecord</c> to define a records validity and SQL statements.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview>May 16, 2007</LastFullCodeReview>
		public class RecordValidater {
				//#### Declare the required private variables
			private string[] ga_sSQLStatements;
			private bool g_bGenerateSQLStatements;
			private bool g_bIsValid;


			//##########################################################################################
			//# Class Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			/// <remarks>
			/// This default constructor initilizes the class defining the record as valid, SQL will be automaticially generated and no custom SQL statements have been defined. In other words, <c>IsValid</c> == true, <c>GenerateSQL</c> == true and <c>SQLStatements</c> == null.
			/// </remarks>
			///############################################################
			/// <LastUpdated>January 11, 2010</LastUpdated>
			public RecordValidater() {
					//#### Call .Reset to init the class vars
				Reset();
			}

			///############################################################
			/// <summary>
			/// Resets the class to its initilized state.
			/// </summary>
			///############################################################
			/// <LastUpdated>January 11, 2010</LastUpdated>
			public void Reset() {
					//#### Call .Reset to init the class vars
				Reset(true, true, null);
			}
			

			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			/// <remarks>
			/// Any SQL statements included within the <paramref>a_sSQLStatements</paramref> are returned to the developer in the order provided within the <c>a_sSQL</c> argument of <c>SubmitResults</c> in record order. These are followed by any SQL statements that were auto-generated based on the value of <paramref>bGenerateSQL</paramref>. For example, if the developer returned 2 SQL statements within the <paramref>a_sSQLStatements</paramref> for each record and requested that statements be auto-generated via <paramref>bGenerateSQL</paramref>, the <c>a_sSQL</c> argument of <c>SubmitResults</c> would be in the following order (assuming 2 records were submitted):
			/// <param/>     Record1_SQLArrayStatement1, Record1_SQLArrayStatement2, Record1_GenerateSQLStatement1, Record2_SQLArrayStatement1, Record2_SQLArrayStatement2, Record2_GenerateSQLStatement1
			/// <param/>NOTE: SQL statements (either provided via <paramref>a_sSQLStatements</paramref> or auto-generated) are only placed within the <c>a_sSQL</c> argument of <c>SubmitResults</c> if the record <paramref>bIsValid</paramref>. SQL statements for records with a <paramref>bIsValid</paramref> value of false are not included within the <c>a_sSQL</c> argument of <c>SubmitResults</c>.
			/// </remarks>
			/// <param name="bIsValid">Boolean value indicating if the record is valid.</param>
			/// <param name="bGenerateSQLStatements">Boolean value indicating if the system is to automatically generate SQL queries for this record.</param>
			/// <param name="a_sSQLStatements">String array where each element represents one developer defined SQL statement.</param>
			///############################################################
			/// <LastUpdated>January 11, 2010</LastUpdated>
			public RecordValidater(bool bIsValid, bool bGenerateSQLStatements, string[] a_sSQLStatements) {
					//#### Call .Reset to init the class vars
				Reset(bIsValid, bGenerateSQLStatements, a_sSQLStatements);
			}

			///############################################################
			/// <summary>
			/// Resets the class to its initilized state.
			/// </summary>
			/// <remarks>
			/// Any SQL statements included within the <paramref>a_sSQLStatements</paramref> are returned to the developer in the order provided within the <c>a_sSQL</c> argument of <c>SubmitResults</c> in record order. These are followed by any SQL statements that were auto-generated based on the value of <paramref>bGenerateSQL</paramref>. For example, if the developer returned 2 SQL statements within the <paramref>a_sSQLStatements</paramref> for each record and requested that statements be auto-generated via <paramref>bGenerateSQL</paramref>, the <c>a_sSQL</c> argument of <c>SubmitResults</c> would be in the following order (assuming 2 records were submitted):
			/// <param/>     Record1_SQLArrayStatement1, Record1_SQLArrayStatement2, Record1_GenerateSQLStatement1, Record2_SQLArrayStatement1, Record2_SQLArrayStatement2, Record2_GenerateSQLStatement1
			/// <param/>NOTE: SQL statements (either provided via <paramref>a_sSQLStatements</paramref> or auto-generated) are only placed within the <c>a_sSQL</c> argument of <c>SubmitResults</c> if the record <paramref>bIsValid</paramref>. SQL statements for records with a <paramref>bIsValid</paramref> value of false are not included within the <c>a_sSQL</c> argument of <c>SubmitResults</c>.
			/// </remarks>
			/// <param name="bIsValid">Boolean value indicating if the record is valid.</param>
			/// <param name="bGenerateSQLStatements">Boolean value indicating if the system is to automatically generate SQL queries for this record.</param>
			/// <param name="a_sSQLStatements">String array where each element represents one developer defined SQL statement.</param>
			///############################################################
			/// <LastUpdated>January 11, 2010</LastUpdated>
			public void Reset(bool bIsValid, bool bGenerateSQLStatements, string[] a_sSQLStatements) {
					//#### Set the class variables to the passed data
				ga_sSQLStatements = a_sSQLStatements;
				g_bGenerateSQLStatements = bGenerateSQLStatements;
				g_bIsValid = bIsValid;
			}


			//##########################################################################################
			//# Public Read-Write Properties
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Gets/sets a value where each element represents one developer defined SQL statement.
			/// </summary>
			/// <remarks>
			/// Any SQL statements included within the <c>SQLStatements</c> are returned to the developer in the order provided within the <c>a_sSQL</c> argument of <c>SubmitResults</c> in record order. These are followed by any SQL statements that were auto-generated based on the value of <c>GenerateSQL</c>. For example, if the developer returned 2 SQL statements within the <c>SQLStatements</c> for each record and requested that statements be auto-generated via <c>GenerateSQL</c>, the <c>a_sSQL</c> argument of <c>SubmitResults</c> would be in the following order (assuming 2 records were submitted):
			/// <param/>     Record1_SQLArrayStatement1, Record1_SQLArrayStatement2, Record1_GenerateSQLStatement1, Record2_SQLArrayStatement1, Record2_SQLArrayStatement2, Record2_GenerateSQLStatement1
			/// <param/>NOTE: SQL statements (either provided via <c>SQLStatements</c> or auto-generated) are only placed within the <c>a_sSQL</c> argument of <c>SubmitResults</c> if the record <c>IsValid</c>. SQL statements for records with a <c>IsValid</c> value of false are not included within the <c>a_sSQL</c> argument of <c>SubmitResults</c>.
			/// </remarks>
			/// <value>String array where each element represents a developer defined SQL statement associated with the current record.</value>
			///############################################################
			/// <LastUpdated>January 16, 2006</LastUpdated>
			public string[] SQLStatements {
				get { return ga_sSQLStatements; }
				set { ga_sSQLStatements = value; }
			}

			///############################################################
			/// <summary>Gets/sets a value indicating if the system is to automatically generate SQL Insert/Update statements for this record.</summary>
			/// <remarks>
			/// Any SQL statements included within the <c>SQLStatements</c> are returned to the developer in the order provided within the <c>a_sSQL</c> argument of <c>SubmitResults</c> in record order. These are followed by any SQL statements that were auto-generated based on the value of <c>GenerateSQL</c>. For example, if the developer returned 2 SQL statements within the <c>SQLStatements</c> for each record and requested that statements be auto-generated via <c>GenerateSQL</c>, the <c>a_sSQL</c> argument of <c>SubmitResults</c> would be in the following order (assuming 2 records were submitted):
			/// <param/>     Record1_SQLArrayStatement1, Record1_SQLArrayStatement2, Record1_GenerateSQLStatement1, Record2_SQLArrayStatement1, Record2_SQLArrayStatement2, Record2_GenerateSQLStatement1
			/// <param/>NOTE: SQL statements (either provided via <c>SQLStatements</c> or auto-generated) are only placed within the <c>a_sSQL</c> argument of <c>SubmitResults</c> if the record <c>IsValid</c>. SQL statements for records with a <c>IsValid</c> value of false are not included within the <c>a_sSQL</c> argument of <c>SubmitResults</c>.
			/// </remarks>
			/// <value>Boolean value indicating if the system is to automatically generate SQL Insert/Update statements for this record.</value>
			///############################################################
			/// <LastUpdated>January 16, 2006</LastUpdated>
			public bool GenerateSQLStatements {
				get { return g_bGenerateSQLStatements; }
				set { g_bGenerateSQLStatements = value; }
			}

			///############################################################
			/// <summary>Gets/sets a value indicating if the record is valid.</summary>
			/// <remarks>
			/// Any SQL statements included within the <c>SQLStatements</c> are returned to the developer in the order provided within the <c>a_sSQL</c> argument of <c>SubmitResults</c> in record order. These are followed by any SQL statements that were auto-generated based on the value of <c>GenerateSQL</c>. For example, if the developer returned 2 SQL statements within the <c>SQLStatements</c> for each record and requested that statements be auto-generated via <c>GenerateSQL</c>, the <c>a_sSQL</c> argument of <c>SubmitResults</c> would be in the following order (assuming 2 records were submitted):
			/// <param/>     Record1_SQLArrayStatement1, Record1_SQLArrayStatement2, Record1_GenerateSQLStatement1, Record2_SQLArrayStatement1, Record2_SQLArrayStatement2, Record2_GenerateSQLStatement1
			/// <param/>NOTE: SQL statements (either provided via <c>SQLStatements</c> or auto-generated) are only placed within the <c>a_sSQL</c> argument of <c>SubmitResults</c> if the record <c>IsValid</c>. SQL statements for records with a <c>IsValid</c> value of false are not included within the <c>a_sSQL</c> argument of <c>SubmitResults</c>.
			/// </remarks>
			/// <value>Boolean value indicating if the record is valid.</value>
			///############################################################
			/// <LastUpdated>January 16, 2006</LastUpdated>
			public bool IsValid {
				get { return g_bIsValid; }
				set { g_bIsValid = value; }
			}

		} //# class RecordValidater


		///########################################################################################################################
		/// <summary>
		/// 
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview></LastFullCodeReview>
		public class FormInputCollection : Inputs.InputCollectionBase {
				//#### Declare the required private/protected variables
			private Form g_oParentForm;

				//#### Declare the required private constants
	  	  //private const string g_cClassName = "Cn.Web.Renderer.Form.InputCollection.";
			private const string g_cISRENDERERFORM = "true";


			//##########################################################################################
			//# Class Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			/// <param name="oForm">Form object reference to the related <c>Form</c> object.</param>
			///############################################################
			/// <LastUpdated>February 10, 2010</LastUpdated>
			public FormInputCollection(Form oForm) {
					//#### Pass the call off to .Reset to init the class vars
				Reset(oForm);
			}

			///############################################################
			/// <summary>
			/// Resets the class to its initilized state.
			/// </summary>
			/// <param name="oForm">Form object reference to the related <c>Form</c> object.</param>
			///############################################################
			/// <LastUpdated>December 11, 2009</LastUpdated>
			public void Reset(Form oForm) {
					//#### Pass the call off to .DoReset
				DoReset(oForm.Settings);

					//#### (Re)set the global variables
				g_oParentForm = oForm;
			}


			//##########################################################################################
			//# Public Properties
			//##########################################################################################
/*			///############################################################
			/// <summary>
			/// Gets/sets a value representing the input's extended datatype(s), which define the HTML input's form rendering requirements.
			/// </summary>
			/// <returns>Enumerated multi-value integer representing the input's extended datatype(s).</returns>
			///############################################################
			/// <LastUpdated>November 12, 2009</LastUpdated>
			public int ExtendedDataType {
				get { return g_iExtendedDataType; }
				set { g_iExtendedDataType = value; }
			}
*/

			//##########################################################################################
			//# Public Read-Only Properties
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Gets the related <c>Form</c> object (if any).
			/// </summary>
			/// <value>Form object reference to the related <c>Form</c> object.</value>
			///############################################################
			/// <LastUpdated>January 4, 2010</LastUpdated>
			public Form Parent {
				get { return g_oParentForm; }
			}


			//##########################################################################################
			//# Public Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Retrieves the input data for the referenced input alias.
			/// </summary>
			/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
			/// <returns>Object representing the InputData for the passed <paramref>sInputAlias</paramref>.</returns>
			///############################################################
			/// <LastUpdated>March 24, 2010</LastUpdated>
			public override Inputs.InputData Inputs(string sInputAlias) {
					//#### Pass the call off to our sister implementation
				return Inputs(sInputAlias, g_oParentForm.RecordCount);
			}

			///############################################################
			/// <summary>
			/// Retrieves the input data for the referenced input alias.
			/// </summary>
			/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
			/// <param name="iRecordNumber">Integer representing the record number.</param>
			/// <returns>Object representing the InputData for the passed <paramref>sInputAlias</paramref>.</returns>
			///############################################################
			/// <LastUpdated>February 10, 2010</LastUpdated>
			public Inputs.InputData Inputs(string sInputAlias, int iRecordNumber) {
				Inputs.InputData oReturn = null;

					//#### If the passed sInputAlias .Exists, reset the oReturn value to the InputData present within gh_oInputs
				if (Exists(sInputAlias)) {
					oReturn = (Inputs.InputData)(gh_oInputs[sInputAlias]);

						//#### If the .Value/.Values have not yet been set for the passed iRecordNumber
						//####     NOTE: This is done so we don't re-load values that have been loaded previously
					if (oReturn.SourceRecordNumber != iRecordNumber) {
							//#### If this is a call during a g_bIsPostBack
							//####     NOTE: Since the developer expects the .Values to remain static within the object we only load them on the initial call, hence all the checks below
						if (g_bIsPostBack) {
								//#### Collect the ga_sValues from Request's .Form for the passed iRecordNumber and set .ValueIsFromForm based on its difference from the .DefaultValue
							oReturn.Values = g_oParentForm.Settings.Request.Form.GetValues(InputName(sInputAlias, iRecordNumber));
							oReturn.ValueIsFromForm = (oReturn.Value != oReturn.DefaultValue);
							oReturn.SetSourceRecordNumber(iRecordNumber);
						}
							//#### Else this is a non-g_bIsPostBack call
						else {
								//#### (Re)Load .Value with the .DefaultValue and reset .ValueIsFromForm to false (as since this is a non-g_bIsPostBack call, the .Value will never be from the form)
							oReturn.Value = oReturn.DefaultValue;
							oReturn.ValueIsFromForm = false;
							oReturn.SetSourceRecordNumber(iRecordNumber);
						}
					}
				}

					//#### Return the above determined oReturn value to the caller
				return oReturn;
			}

			///############################################################
			/// <summary>
			/// Validates the inputs defined within this instance.
			/// </summary>
			/// <returns>A Boolean value indicating if all the inputs defined within this instance are valid.</returns>
			///############################################################
			/// <LastUpdated>April 12, 2010</LastUpdated>
			public override bool Validate() {
/*				string[] a_sKeys = InputAliases;
				int i;
				bool bReturn = true;

					//#### If this is not a g_oParentForm related instance and we have a_sKeys to traverse
				if (a_sKeys != null && a_sKeys.Length > 0) {
						//#### Collect the initial a_sRecordTracker (hence iCurrentRecord = 1), then determine the iRecordTrackerMode
					int iCurrentRecord = 1;
					string[] a_sRecordTracker = g_oParentForm.ParseRecordTracker(iCurrentRecord);
					int iRecordTrackerMode = Cn.Data.Tools.MakeInteger(a_sRecordTracker[0], (int)enumRecordTrackerModes.cnUnknown);

						//#### Do while we still have records to process
					while (iRecordTrackerMode != (int)enumRecordTrackerModes.cnUnknown) {
							//#### As long as this is not a .cnMissing
						if (iRecordTrackerMode != (int)enumRecordTrackerModes.cnMissing) {
								//#### Traverse the a_sKeys
							for (i = 0; i < a_sKeys.Length; i++) {
									//#### If the InputData for the current a_sKey .Is(not)Valid, flip our bReturn value to false
								if (! Inputs(a_sKeys[i], iCurrentRecord).IsValid) {
									bReturn = false;
//!Cn.Tools.dWrite("Errored: " + a_sKeys[i] + " with " + GetInputMetaData(a_sKeys[i]).Errors + " (Value = " + GetInputMetaData(a_sKeys[i]).Value + ")");
								}
							}
						}

						//#### Inc iCurrentRecord, collect the next a_sRecordTracker and reset g_iTableIndex for the next loop
						iCurrentRecord++;
						a_sRecordTracker = g_oParentForm.ParseRecordTracker(iCurrentRecord);

							//#### Reset the iRecordTrackerMode for the next loop
						iRecordTrackerMode = Cn.Data.Tools.MakeInteger(a_sRecordTracker[0], (int)enumRecordTrackerModes.cnUnknown);
					}
				}

					//#### Traverse our .ChildCollections (if any), resetting our bReturn value to its
				bReturn = DoValidateChildCollections(bReturn);

					//#### Return the above determined bReturn value to the caller
				return bReturn;
*/
					//#### Call .DoValidation to process the .Form (ignoring it's returned reconstructed oPageResults as we don't require them)
				g_oParentForm.DoValidation();

					//#### Return the result of .DoValidateChildCollections to the caller (while passing in if .ErrorsOccured during .DoValidation)
				return DoValidateChildCollections(! g_oParentForm.ErrorsOccured);
			}

			///############################################################
			/// <summary>
			/// Retrieves each record's user submitted value for the referenced input alias group of HTML inputs.
			/// </summary>
			/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
			/// <returns>Array of strings where each element represents a record's submitted value for the <paramref>sInputAlias</paramref> group of HTML inputs.</returns>
			///############################################################
			/// <LastUpdated>November 26, 2009</LastUpdated>
			public override string[] Column(string sInputAlias) {
				HttpRequest oRequest = HttpContext.Current.Request;
				string[] a_sReturn = null;
				int iCount;
				int i;

					//#### If the passed sInputAlias .Exists
				if (Exists(sInputAlias)) {
						//#### Reset the value of iCount to it's .SubmittedRecordCount and re-dimension our a_sReturn value accordingly
					iCount = g_oParentForm.SubmittedRecordCount;
					a_sReturn = new string[iCount];

						//#### Traverse the a_sReturn, populating each element with it's asso. submitted form value
						//####     NOTE: sInputAlias's are 1-based, hence the +1 below
					for (i = 0; i < iCount; i++) {
						a_sReturn[i] = oRequest.Form[InputName(sInputAlias, i + 1)];
					}
				}

					//#### Return the above determined sReturn value to the caller
				return a_sReturn;
			}

			///############################################################
			/// <summary>
			/// Retrieves the HTML input name for the current record number.
			/// </summary>
			/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
			/// <returns>String representing the HTML input name for the current record number.</returns>
			///############################################################
			/// <LastUpdated>June 22, 2010</LastUpdated>
			public override string InputName(string sInputAlias) {
					//#### Reset our sReturn value to the passed sInputAlias appended with the .CurrentInputRecord number
					//####     NOTE: .CurrentInputRecord is used below because its based on the rendered count
					//####     NOTE: No presence checking is done here to allow for the developer to render their own inputs outside of Define/Get
				return sInputAlias + "_" + g_oParentForm.RecordCount;
			}

			///############################################################
			/// <summary>
			/// Retrieves the HTML input name for the provided record number.
			/// </summary>
			/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
			/// <param name="iRecordNumber">Integer representing the record number.</param>
			/// <returns>String representing the HTML input name for the provided record number.</returns>
			///############################################################
			/// <LastUpdated>January 11, 2010</LastUpdated>
			public string InputName(string sInputAlias, int iRecordNumber) {
					//#### Return the passed sInputAlias appended with the passed iRecordNumber
					//####     NOTE: No presence checking is done here to allow for the developer to render their own inputs outside of Define/Get
				return sInputAlias + "_" + iRecordNumber;
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
				return "Cn._.wt.GetByID('" + InputName(sInputAlias, 1) + "').form.id";
			}

			///############################################################
			/// <summary>
			/// Get the Set error javascript code block
			/// </summary>
			/// <param name="oInputData">Object representing the InputData instance to deeply copy into this instance.</param>
			/// <returns>String representing the JavaScript code block to Set an error.</returns>
			///############################################################
			/// <LastUpdated>December 11, 2009</LastUpdated>
			protected override string GetSetErrorJavaScript(Inputs.InputData oInputData) {
					//#### Collect the .Input from the .Renderer.Form
				return "Cn._.wive.Set(Cn._.wrf.Input(" + GetFormIDJavaScript(oInputData.InputAlias) + ", '" + Web.Inputs.Tools.EscapeCharacters(oInputData.InputAlias, "'") + "', " + g_oParentForm.RecordCount + "), " + (int)oInputData.ErrorType + ", " + (int)oInputData.DataType + ", '" + Web.Inputs.Tools.EscapeCharacters(oInputData.ErrorMessage, "'") + "'); ";
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


		} //# class FormInputCollection


	} //# class Form

} //# namespace Cn.Web.Renderer
