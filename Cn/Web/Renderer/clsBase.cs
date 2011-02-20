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
using System.Text;									//# Required to access the StringBuilder class


namespace Cn.Web.Renderer {

		//#### Declare the required public eNums
	#region eNums
		/// <summary>Renderer List, Form and Report page sections.</summary>
	public enum enumPageSections : int {		//# Used in Report/ListRenderer.PrintLengths
			/// <summary>Report page header section.</summary>
		cnPageHeader = 0,
			/// <summary>List/Form/Report header section.</summary>
		cnHeader = 1,
			/// <summary>List/Form detail header section.</summary>
		cnDetailHeader = 2,
			/// <summary>List/Form record detail section.</summary>
		cnDetail = 3,
			/// <summary>Form new record detail section.</summary>
		cnDetail_NewForm = 4,
			/// <summary>List/Form missing record section.</summary>
		cnMissingRecord = 5,
			/// <summary>List/Form detail footer section.</summary>
		cnDetailFooter = 6,
			/// <summary>List/Form/Report no results returned section.</summary>
		cnNoResults = 7,
			/// <summary>List/Form/Report footer section.</summary>
		cnFooter = 8,
			/// <summary>Report page footer section.</summary>
		cnPageFooter = 9,
	}
		/// <summary>General Renderer input types.</summary>
	public enum enumGeneralInputTypes : int {	//# Used in Renderer.RenderInput
			/// <summary>User selected stack checkbox input (and associated JavaScript code).</summary>
		cnUserSelectedStackCheckbox = 0,
			/// <summary>DHTML complex column sorter.<para/>Allows the end user to define their own column sorting.</summary>
		cnComplexSorter = 1,
			/// <summary>RendererSearchForm quick search criteria form.</summary>
		cnQuickSearch = 2
	}
	#endregion


	///########################################################################################################################
	/// <summary>
	/// A web presentation layer for the output of data from any data source (DBMS, XML, flat-file, etc.).
	/// </summary>
	/// <remarks>
	/// NOTE: This class is only used internally by Renderer.
	/// </remarks>
	///########################################################################################################################
	/// <LastFullCodeReview>May 22, 2007</LastFullCodeReview>
	public abstract class Base {
			//#### Declare the required private/protected variables
		private List g_oListOrForm;
		private string g_sClassPath;
		private enumRendererObjectTypes g_eType;
		private bool g_bRenderedUserSelectedStackJSForThisInvocation;
		private bool g_bPrintable;

			//#### Declare the required protected variables
		protected Pagination g_oPagination;
		protected Report g_oParentReport;
		protected bool g_bInitialDetailCall;
			/// <summary>Gets/sets the current enviroment's HttpResponse object.</summary>
		protected HttpResponse Response;
			/// <summary>Gets/sets the current enviroment's HttpRequest object.</summary>
		protected HttpRequest Request;
			/// <summary>Gets/sets the current enviroment's Web.Settings.Current object.</summary>
		protected Settings.Current g_oSettings;
		protected string g_sDefaultOrderBy;
			/// <summary>Gets/sets the current record's 0-based index within the current table (only includes existing records).</summary>
		protected int g_iTableRecordIndex;
			/// <summary>Gets/sets the results stack's 0-based current table index.</summary>
		protected int g_iTableIndex;
			/// <summary>Gets/sets the 1-based count of records per rendered page (not including new records).</summary>
		protected int g_iRecordsPerPage;
			/// <summary>Gets/sets the current record's 1-based index (includes existing and missing records).</summary>
		protected int g_iRecordCount;
			/// <summary>Gets/sets a value indicating the current page number.</summary>
		protected int g_iPageCount;
			/// <summary>Get/sets a value indicating the total height of a printed page.</summary>
		protected int g_iPageLength;
			/// <summary>Gets/sets a value indicating the unit of measure crawl down the current page.</summary>
		protected int g_iPageCrawl;
			/// <summary>Gets/sets a value indicating if we are permitted to utilize Response.Write.</summary>
		protected bool g_bAllowResponseWrite;

			//#### Declare the required public eNums
		#region eNums
			/// <summary>Enumeration representing the reconized <c>Renderer</c> object types.</summary>
		public enum enumRendererObjectTypes : int {
				/// <summary>Unknown object type.</summary>
			cnUnknown = 0,
				/// <summary>Renderer Report.</summary>
			cnReport = 0,
				/// <summary>Renderer List.</summary>
			cnList = 1,
				/// <summary>Renderer Form.</summary>
			cnForm = 2
		}
		#endregion

			//#### Declare the required private constants
	  //private const string g_cClassName = "Cn.Web.Renderer.Base";		//# NOTE: g_sClassPath usurps the need for this constant


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="eType">Enumeration representing the object type of this instance.</param>
		/// <param name="sClassPath">String representing the class path of this instance.</param>
		/// <seealso cref="Cn.Web.Renderer.Base.Reset"/>
		///############################################################
		/// <LastUpdated>March 18, 2010</LastUpdated>
		public Base(enumRendererObjectTypes eType, string sClassPath) {
				//#### Set the global private variables that remain unchanged (or who's values are important) across .Resets
			g_eType = eType;
			g_sClassPath = sClassPath;
			
				//#### Set the value of g_oListOrForm based on the g_eType 
			if (g_eType == enumRendererObjectTypes.cnList || g_eType == enumRendererObjectTypes.cnForm) {
				g_oListOrForm = this as Renderer.List;
			}
			else {
				g_oListOrForm = null;
			}

				//#### Call .Reset to init our own global variables
				//####     NOTE: This call does not need to be done here as the inheriting class calls it's own .Reset within its own constructor which in turn calls the base.Reset
		  //Reset(oSettings);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		///############################################################
		/// <LastUpdated>November 6, 2009</LastUpdated>
		public virtual void Reset() {
				//#### Call .Reset to init our own global variables (using the .Current .Settings)
		    Reset(new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		///############################################################
		/// <LastUpdated>June 1, 2010</LastUpdated>
		public virtual void Reset(Settings.Current oSettings) {
				//#### (Re)Init the global protected variables
			g_iRecordsPerPage = -1;
			g_iPageCount = 1;
			g_iPageLength = 0;
			g_iPageCrawl = 0;

				//#### .DoReset(our)Indexes
				//####     NOTE: We do this in it's own sub as to ensure they are always reset to the same initial values
			DoResetIndexes();

				//#### Set the oSettings via the property (so that Request/Response is set correctly)
			Settings = oSettings;

				//#### (Re)Init the global private variables
			g_oPagination = new Pagination();
			Parent = null;						//# Set .Parent via the property to ensure the rules are properly applied to the value
			DefaultOrderBy = "";						//# Set .DefaultOrderBy via the property to ensure the rules are properly applied to the value
			g_bRenderedUserSelectedStackJSForThisInvocation = false;
			g_bAllowResponseWrite = true;
		    g_bInitialDetailCall = true;
			g_bPrintable = Data.Tools.MakeBoolean(
				Request.QueryString[Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + "Printable"],
				false
			);
		}

		///############################################################
		/// <summary>
		/// Resets the class's indexes to their pre-loop state.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 22, 2010</LastUpdated>
		protected virtual void DoResetIndexes() {
				//#### (Re)Set the view-specific TableRecordIndex, TableIndex and RenderedRecordCount
				//####     NOTE: Only these values are (re)set here as the remaining values handeled within .Reset are important across view renders (but not across .Resets)
				//####     NOTE: All of the values are set so as allow to pre-incrementing (hence the -1/0's)
			g_iTableRecordIndex = -1;
			g_iTableIndex = -1;
			g_iRecordCount = 0;
		}


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the parent <c>Report</c> class related to this instance (if any).
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: <c>Report</c>s are not permitted to have a <c>Parent</c> defined. Only <c>List</c>s and <c>Form</c>s are permitted to have a non-null <c>Parent</c>.
		/// </remarks>
		/// <value>Report object that represents the instance's related <c>Report</c> class (if any).</value>
		///############################################################
		/// <LastUpdated>January 10, 2010</LastUpdated>
		public Report Parent {
			get {
				return g_oParentReport;
			}
			set {
					//#### If we are a .cnReport we are not allowed to have a g_oParentReport, so ensure our g_oParentReport is null
					//####     NOTE: This logic allows us to avoid infinate loops in the properties below (as if we were our own .Parent, the properties would loop infinitly)
				if (g_eType == enumRendererObjectTypes.cnReport) {
					g_oParentReport = null;
				}
					//#### Else we are not a .cnReport, so set our g_oParentReport to the passed value
				else {
					g_oParentReport = value;
				}
			}
		}

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
		public virtual Settings.Current Settings {
			get {
					//#### If the g_oParentReport has not been set, return our own oSettings
				if (g_oParentReport == null) {
					return g_oSettings;
				}
					//#### Else we have a g_oParentReport, so return it's .Settings
				else {
					return g_oParentReport.Settings;
				}
			}
			set {
					//#### If the g_oParentReport has not been set, return our own oSettings
				if (g_oParentReport == null) {
					g_oSettings = value;
					Response = g_oSettings.Response;
					Request = g_oSettings.Request;
				}
					//#### Else we have a g_oParentReport, so return it's .Settings
				else {
					g_oParentReport.Settings = value;
					Response = g_oParentReport.Settings.Response;
					Request = g_oParentReport.Settings.Request;
				}
			}
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
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public string DefaultOrderBy {
			get {
					//#### Always return our own g_sDefaultOrderBy
					//####     NOTE: Since Reports do not utilize this property, we always return our own g_sDefaultOrderBy (as opposed to forwarding the call onto g_oParentReport.DefaultOrderBy).
				return g_sDefaultOrderBy;
			}
			set {
					//#### If we are a .cnReport we do not utilize the g_sDefaultOrderBy, so ensure our g_sDefaultOrderBy is a null-string
				if (g_eType == enumRendererObjectTypes.cnReport) {
					g_sDefaultOrderBy = "";
				}
					//#### Else we are not a .cnReport
				else {
						//#### If the data within the passed value is safe, set the g_sDefaultOrderBy
						//####     NOTE: Since Reports do not utilize this property, we always set our own g_sDefaultOrderBy (as opposed to forwarding the call onto g_oParentReport.DefaultOrderBy).
					if (Statements.IsUserDataSafe(value)) {
						g_sDefaultOrderBy = Data.Tools.MakeString(value, "");
					}
						//#### Else malicious SQL code was detected by .IsUserDataSafe, so set g_sDefaultOrderBy to a null-string
					else {
						g_sDefaultOrderBy = "";
//!						Internationalization.RaiseDefaultError(g_cClassName + "DefaultOrderBy", Cn.Configuration.Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "DefaultOrderBy", "");
// <exception cref="Cn.CnException">Thrown when the provided <paramref>value</paramref> contains potentionally malicious SQL directives (as defined within Cn.Data.SQL.Statements "DisallowedUserStrings" and "DisallowedUserWords" arrays).</exception>
					}
				}
			}
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
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public int PageLength {
			get {
					//#### If the g_oParentReport has not been set, return our own g_iPageLength
				if (g_oParentReport == null) {
					return g_iPageLength;
				}
					//#### Else we have a g_oParentReport, so return it's .PageLength
				else {
					return g_oParentReport.PageLength;
				}
			}
			set {
					//#### If the g_oParentReport has not been set, set our own g_iPageLength
				if (g_oParentReport == null) {
					g_iPageLength = value;
				}
					//#### Else we have a g_oParentReport, so set it's .PageLength
				else {
					g_oParentReport.PageLength = value;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if we are permitted to utilize Response.Write.
		/// </summary>
		/// <remarks>
		/// This is primarly in place to allow for "pseudo-renders" like that required during the CreateChildControl period of WebControls (as no rendering is actually done by the initial call). This can also be used by developers who choose to handle the rendering of their own page breaks and record trackers, should such a need arise.
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>AllowResponseWrite</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>Boolean value indicating if we are permitted to utilize Response.Write.</value>
		///############################################################
		/// <LastUpdated>June 1, 2010</LastUpdated>
		public bool AllowResponseWrite {
			get {
					//#### If the g_oParentReport has not been set, return our own g_bAllowResponseWrite
				if (g_oParentReport == null) {
					return g_bAllowResponseWrite;
				}
					//#### Else we have a g_oParentReport, so return it's .AllowResponseWrite
				else {
					return g_oParentReport.AllowResponseWrite;
				}
			}
			set {
					//#### If the g_oParentReport has not been set, set our own g_bAllowResponseWrite
				if (g_oParentReport == null) {
					g_bAllowResponseWrite = value;
				}
					//#### Else we have a g_oParentReport, so set it's .AllowResponseWrite
				else {
					g_oParentReport.AllowResponseWrite = value;
				}
			}
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
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public bool Printable {
			get {
					//#### If the g_oParentReport has not been set, return our own g_bPrintable
				if (g_oParentReport == null) {
					return g_bPrintable;
				}
					//#### Else we have a g_oParentReport, so return it's .Printable
				else {
					return g_oParentReport.Printable;
				}
			}
			set {
					//#### If the g_oParentReport has not been set, set our own g_bPrintable
				if (g_oParentReport == null) {
					g_bPrintable = value;
				}
					//#### Else we have a g_oParentReport, so set it's .Printable
				else {
					g_oParentReport.Printable = value;
				}
			}
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
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public bool Debug {
			get {
					//#### If the g_oParentReport has not been set, return our own oSettings.Debug
				if (g_oParentReport == null) {
					return g_oSettings.Debug;
				}
					//#### Else we have a g_oParentReport, so return it's .Debug
				else {
					return g_oParentReport.Debug;
				}
			}
			set {
					//#### If the g_oParentReport has not been set, set our own oSettings.Debug
				if (g_oParentReport == null) {
					g_oSettings.Debug = value;
				}
					//#### Else we have a g_oParentReport, so set it's .Debug
				else {
					g_oParentReport.Debug = value;
				}
			}
		}


		//##########################################################################################
		//# Public Read-Only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the <c>Breadcrumb</c> trail related to this instance.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>Breadcrumb</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>Breadcrumb object that represents this instance's Breadcrumb trail.</value>
		///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		public Breadcrumb Breadcrumb {
			get {
					//#### If the g_oParentReport has not been set
				if (g_oParentReport == null) {
						//#### If the .Breadcrumb has not yet been init'd, create a new dummy instance now
					if (g_oSettings.Breadcrumb == null) {
						g_oSettings.Breadcrumb = new Breadcrumb();
					}

						//#### Return the oSettings.Breadcrumb to the caller
					return g_oSettings.Breadcrumb;
				}
					//#### Else we have a g_oParentReport, so return it's .Breadcrumb
				else {
					return g_oParentReport.Breadcrumb;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets the results set related to this instance.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>Results</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>Pagination object that represents this instance's results set.</value>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public Pagination Results {
			get {
					//#### If the g_oParentReport has not been set, return our own g_oPagination
				if (g_oParentReport == null) {
					return g_oPagination;
				}
					//#### Else we have a g_oParentReport, so return it's .Results
				else {
					return g_oParentReport.Results;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets the user selected stack related to this instance.
		/// </summary>
		/// <value>String representing the related Data.Pagination instance for this instance's user selected stack.</value>
		///############################################################
		/// <LastUpdated>May 16, 2007</LastUpdated>
		public string UserSelectedStack {
			get {
//! utilize the Report's or...?
				return Data.Tools.MakeString(Request.Form[Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + "UserSelectedStack"], "").Trim();
			}
		}

		///############################################################
		/// <summary>
		/// Gets the carrage return/line feed character(s) utilized by this instance.
		/// </summary>
		/// <value>String representing the carrage return/line feed character(s) utilized by this instance.</value>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public string CRLF {
			get {
					//#### Collect the local bDebug from our own property (which will grab it's value from the g_oParentReport if there is one)
				bool bDebug = Debug;

					//#### If the bDebug flag was enabled, return CRLF as a carrage return/line feed
				if (bDebug) {
					return "\n";
				}
					//#### Else the bDebug flag was disabled, so return CRLF as a null-string
				else {
					return "";
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets the count of records per rendered page (not including new records).
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>RecordsPerPage</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// <para/>NOTE: This property does not make much sense for <c>Report</c>s as <c>Report</c>s would generally represent the entire set of results (i.e. a <c>RecordsPerPage</c> value of "0"). But if you choose to only represent X records on a given report, you can utilize this property. Do note however that <c>RecordsPerPage</c> refers to records rendered per page load, *NOT* per phisically printed page!
		/// </remarks>
		/// <value>1-based integer representing the count of records per rendered page (not including new records).</value>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public int RecordsPerPage {
			get {
					//#### If the g_oParentReport has not been set, return our own g_iRecordsPerPage
				if (g_oParentReport == null) {
					return g_iRecordsPerPage;
				}
					//#### Else we have a g_oParentReport, so return it's .RecordsPerPage
				else {
					return g_oParentReport.RecordsPerPage;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets the current record's number (includes both existing and missing records).
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>RenderedRecordCount</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>1-based integer representing the current record's number (includes both existing and missing records).</value>
		///############################################################
		/// <LastUpdated>June 1, 2010</LastUpdated>
		public virtual int RecordCount {
			get {
					//#### If the g_oParentReport has not been set, return our own g_iRecordCount
				if (g_oParentReport == null) {
					return g_iRecordCount;
				}
					//#### Else we have a g_oParentReport, so return it's .RenderedRecordCount
//! probably not correct!? when is g_oParentReport's g_iRecordCount inc'd?
				else {
					return g_oParentReport.RecordCount;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets the current record's index (only includes existing records).
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>TableRecordIndex</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>0-based integer representing the current record's index (only includes existing records).</value>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		public int TableRecordIndex {
			get {
					//#### If the g_oParentReport has not been set, return our own g_iTableRecordIndex
				if (g_oParentReport == null) {
					return g_iTableRecordIndex;
				}
					//#### Else we have a g_oParentReport, so return it's .TableRecordIndex
				else {
					return g_oParentReport.TableRecordIndex;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets the results stack's current table index.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>TableIndex</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>0-based integer representing the current table's index.</value>
		///############################################################
		/// <LastUpdated>June 1, 2010</LastUpdated>
		public int TableIndex {
			get {
					//#### If the g_oParentReport has not been set, return our own g_iTableIndex
				if (g_oParentReport == null) {
					return g_iTableIndex;
				}
					//#### Else we have a g_oParentReport, so return it's .TableIndex
				else {
					return g_oParentReport.TableIndex;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating the current page number.
		/// </summary>
		/// <remarks>
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>PageCount</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>1-based integer representing the current page number.</value>
		///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public int PageCount {
			get {
					//#### If the g_oParentReport has not been set, return our own g_iPageCount
				if (g_oParentReport == null) {
					return g_iPageCount;
				}
					//#### Else we have a g_oParentReport, so return it's .PageCount
				else {
					return g_oParentReport.PageCount;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating the unit of measure crawl down the current page.
		/// </summary>
		/// <remarks>
		/// Due to the web's poor ability to format pages for printing, this method of counting the current number of units (inches, cm, mm, etc.) the current text would be on a printed page was devised. For example - if one record occupies approximately 1 inch of vertical page space, the top of the forth record would start at approximately 3 inches down the page. Though not perfect, this method allows the developer to better accommodate the printed page by controlling where page breaks occur.
		/// <para/>NOTE: The units returned from this function (inches, cm, mm, etc.) are irrelevant, as they are based on the units returned by the <c>PageLength</c> functions in the related <c>RendererList</c>, <c>RendererForm</c> and/or <c>RendererReport</c>.
		/// <para/>NOTE: If a <c>Parent</c> Report has been defined, this call is forwarded onto it's own <c>PageCrawl</c> property (as it is the <c>Report</c> that is responsible for the rendering of the <c>List</c> or <c>Form</c>).
		/// </remarks>
		/// <value>1-based integer representing the unit of measure crawl down the current page.</value>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public int PageCrawl {
			get {
					//#### If the g_oParentReport has not been set, return our own g_iPageCrawl
				if (g_oParentReport == null) {
					return g_iPageCrawl;
				}
					//#### Else we have a g_oParentReport, so return it's .PageCrawl
				else {
					return g_oParentReport.PageCrawl;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the object type of this instance.
		/// </summary>
		/// <value>Enumeration representing the object type of this instance.</value>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public enumRendererObjectTypes Type {
			get { return g_eType; }
		}


		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Renders the requested general input.
		/// </summary>
		/// <remarks>
		/// NOTE: <c>cnComplexSorter</c> is not yet implemented.
		/// <para/>For a <c>cnQuickSearch</c> type, <paramref>sAdditionalData</paramref> equates to the quicksearch form's initial search criteria. <paramref>oAdditionalData</paramref> equates to a string representing the quicksearch form text input's additional attributes.
		/// <para/>For a <c>cnUserSelectedStackCheckbox</c> type, <paramref>sAdditionalData</paramref> equates to the current record's ID. <paramref>oAdditionalData</paramref> equates to a Hashtable of string containing values for the "FormName" (required), "TableName", "IDColumn", "UserSelectedStack" and the checkbox's additional "Attributes".
		/// </remarks>
		/// <param name="eGeneralInputType">Enumeration representing the general input type to render.</param>
		/// <param name="sAdditionalData">String representing the additional data required by the general input.</param>
		/// <param name="oAdditionalData">Object representing the additional data required by the general input.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eGeneralInputType</paramref> is unreconized.</exception>
		/// <exception cref="Cn.CnException">Thrown when a <paramref>eGeneralInputType</paramref> of cnUserSelectedStackCheckbox is requested and the passed <paramref>oAdditionalData</paramref>'s (casted as a Hashtable of strings) key "FormName" contains a null-string.</exception>
		/// <returns>String representing the requested XHTML/DHTML control.</returns>
		///############################################################
		/// <LastUpdated>January 8, 2010</LastUpdated>
		public string GenerateHTMLInput(enumGeneralInputTypes eGeneralInputType, string sAdditionalData, object oAdditionalData) {
//! May remove function by placing each piece of functionality under Web.Inputs?
			StringBuilder oReturn = new StringBuilder();
			string sDOMElementPrefix = Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix);

				//#### Determine the value of the passed eGeneralInputType and process accordingly
			switch (eGeneralInputType) {
					//#### If this is a .cnComplexSorter call
				case enumGeneralInputTypes.cnComplexSorter: {
//!
Internationalization.RaiseDefaultError(g_sClassPath + "RenderInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_DeveloperDefined, "The ComplexSorter functionality is yet to be implemented.", "");
					break;
				}

					//#### If this is a .cnQuickSearch call
//! Requires that this is not a part of a Renderer.Form thanks to the "RecordTracker_1"
				case enumGeneralInputTypes.cnQuickSearch: {
						//#### .append out the required hidden cnRecordTracker_ (acting as though this is the first New record within .TableIndex 0 with no MD5) as well as the rfQuickSearch text input (Formating(the sAdditionalData)For(the)Form as we go) onto our oReturn value
//! broken thanks to Form.Name functionality
//					oReturn.Append("<input type='hidden' name='" + sDOMElementPrefix + "FORMNAMEHERE_RecordTracker_1' value='" + Inputs.Tools.FormatForForm("New" + Configuration.Settings.PrimaryDelimiter + "0" + Configuration.Settings.PrimaryDelimiter) + "' />" +
//						"<input type='text' name='" + sDOMElementPrefix + "rfQuickSearch' value='" + Inputs.Tools.FormatForForm(sAdditionalData) + "' " + Data.Tools.MakeString(oAdditionalData, "") + " />"
//					);
					break;
				}

					//#### If this is a .cnUserSelectedStackCheckbox call
				case enumGeneralInputTypes.cnUserSelectedStackCheckbox: {
					Hashtable h_sHash = (Hashtable)oAdditionalData;
					string sFormName;
					string sCRLF = CRLF;

						//#### If the required h_sHash["FormName"] element is missing (or the h_sHash is null), raise the error
					if (h_sHash == null || h_sHash["FormName"] == null) {
						Internationalization.RaiseDefaultError(g_sClassPath + "RenderInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_Renderer_USSHashMissingFormName, "", "");
					}
						//#### Else we need to ensure a non null-string FormName was passed
					else {
							//#### Determine the sFormName
						sFormName = Data.Tools.MakeString(h_sHash["FormName"], "");

							//#### If sFormName is a null-string, raise the error
						if (sFormName.Length == 0) {
							Internationalization.RaiseDefaultError(g_sClassPath + "RenderInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_Renderer_USSHashMissingFormName, "", "");
						}
							//#### Else we were passed everything we need to render the .UserSelectedStackCheckbox
						else {
								//#### .Append the hidden form field along with the reference to the external JavaScript file and Initialize the JavaScript vars (if this is the first call)
							oReturn.Append(JavaScript.GenerateFileReferences(JavaScript.enumJavaScriptFiles.cnCnRendererUserSelectedStack, g_oSettings) + sCRLF);

								//#### If we've not yet g_bRenderedUserSelectedStackJSForThisInvocation
								//####     NOTE: This additional check is necessary because this function is called at each checkbox write (and not in an init. call like the rest of the JS functions).
							if (! g_bRenderedUserSelectedStackJSForThisInvocation) {
									//#### Flip g_bRenderedUserSelectedStackJSForThisInvocation
								g_bRenderedUserSelectedStackJSForThisInvocation = true;

									//#### .Append the required hidden form element and initilization JS onto the sReturn value (Formating(the sUserSelectedStack)For(the)Form as we go)
								oReturn.Append("<input type='hidden' name='" + Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + "UserSelectedStack' value='" + Inputs.Tools.FormatForForm(Data.Tools.MakeString(h_sHash["UserSelectedStack"], "")) + "' />" + sCRLF);
							}

								//#### .Append the JavaScript that in turn writes out the checkbox
								//####     NOTE: Since the UserSelectedResultsStack functionality relies completly on JavaScript it is also used to print out the checkboxes. So if the user doesn't have JS enabled, they will never even see the checkboxes.
							oReturn.Append(JavaScript.BlockStart + sCRLF +
								"Cn._.wru.RenderCheckBox('" + Inputs.Tools.EscapeCharacters(sFormName, "'") + "', '" +
									Inputs.Tools.EscapeCharacters(Data.Tools.MakeString(h_sHash["TableName"], ""), "'") + "', '" +
									Inputs.Tools.EscapeCharacters(Data.Tools.MakeString(h_sHash["IDColumn"], ""), "'") + "', '" +
									Inputs.Tools.EscapeCharacters(sAdditionalData, "'") + "', '" +
									Inputs.Tools.EscapeCharacters(Data.Tools.MakeString(h_sHash["Attributes"], ""), "'") +
								"');" + sCRLF +
								JavaScript.BlockEnd + sCRLF
							);
						}
					}
					break;
				}

					//#### Else the eGeneralInputType was unreconized, so raise the error
				default: {
					Internationalization.RaiseDefaultError(g_sClassPath + "RenderInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eGeneralInputType", Data.Tools.MakeString(eGeneralInputType, ""));
					break;
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn.ToString();
		}

		///############################################################
		/// <summary>
		/// Renders the hidden Renderer form elements.
		/// </summary>
		/// <remarks>
		/// This function outputs the hidden "BreadcrumbTrail" and "ResultsStack" Renderer form elements. This allows you to submit these values along with any on-page HTML forms.
		/// </remarks>
		///############################################################
		/// <LastUpdated>May 16, 2007</LastUpdated>
		#region public void WriteHiddenElements()
/*		public void WriteHiddenElements() {
				//#### Pass the call off to our sibling implementation, determining the g_sBreadcrumbTrail and g_oPagination.ToString as we go
			WriteHiddenElements(Settings.Breadcrumb.ToString(), g_oPagination.ToString());
		}
*/
		#endregion

		///############################################################
		/// <summary>
		/// Renders the hidden Renderer form elements.
		/// </summary>
		/// <remarks>
		/// This function outputs the hidden "BreadcrumbTrail" and "ResultsStack" Renderer form elements. This allows you to submit these values along with any on-page HTML forms.
		/// </remarks>
		/// <param name="sBreadcrumbTrail">String representing the related <c>Breadcrumb</c> instance.</param>
		/// <param name="sResultsStack">String representing the related <c>Pagination</c> instance.</param>
		///############################################################
		/// <LastUpdated>May 16, 2007</LastUpdated>
		#region public void WriteHiddenElements(string sBreadcrumbTrail, string sResultsStack)
/*		public void WriteHiddenElements(string sBreadcrumbTrail, string sResultsStack) {
			string sFormElementPrefix = Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix);

				//#### Write out the Renderer managed hidden form fields (Formating(the sBreadcrumbTrail/sResultsStack)For(the)Form as we go)
			Response.Write("<input type='hidden' name='" + sFormElementPrefix + "BreadcrumbTrail' value='" + Web.Inputs.FormatForForm(sBreadcrumbTrail) + "' />\n" + 
				"<input type='hidden' name='" + sFormElementPrefix + "ResultsStack' value='" + Web.Inputs.FormatForForm(sResultsStack) + "' />\n"
			);
		}
*/
		#endregion


		//##########################################################################################
		//# Page Section-related Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Collects/reorders the entire results set.
		/// </summary>
		/// <remarks>
		/// Required function overload to collect the entire results set of relevant record IDs.
		/// <para/>If the page has no results or results that need to be re-ordered, this function is called by the <c>Class Constructor</c>. This function is not called if there are results that are properly ordered.
		/// </remarks>
		/// <param name="oResults">Pagination object representing entire result set's record IDs.</param>
		/// <param name="bReorderExistingResults">Boolean value indicating if the entire results set requires re-ordering.</param>
		///############################################################
		/// <LastUpdated>June 2, 2004</LastUpdated>
		public abstract void GenerateResults(Pagination oResults, bool bReorderExistingResults);

		///############################################################
		/// <summary>
		/// Collects the provided page results.
		/// </summary>
		/// <remarks>
		/// Required function overload to collect a page's relevant records as referenced within the passed <paramref>rPageResults</paramref>.
		/// <para/>If the page has results, this function is called after <c>GenerateResults</c> or the <c>Class Constructor</c> (depending on the state of the provided <paramref>sResultsStack</paramref>). This function is not called if there are no records to render.
		/// <para/>NOTE: The <paramref>rPageResults</paramref> parameter is equivalent to this code snipit: <code>rPageResults = Renderer.Results.getRange(Renderer.Index, Renderer.List.RecordsPerPage)</code>
		/// </remarks>
		/// <param name="oPageResults">Pagination object representing this page's relevant record IDs.</param>
		///############################################################
		/// <LastUpdated>May 30, 2007</LastUpdated>
		public virtual void CollectPageResults(Pagination oPageResults) {
			int i;

				//#### Traverse the oPageResults' .Table, defaulting all .IDs as being successfully collected
			for (i = 0; i < oPageResults.TableCount; i++) {
				oPageResults.Table(i).SetCollectedIDs();
			}
		}

		///############################################################
		/// <summary>
		/// Returns the length the passed page section crawls down a printed page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own print length definition.
		/// <para/>This function is called once per page section when a <c>Renderer.Report</c> is being rendered in print mode. This means that you can calculate the page crawl on a per record basis if need be.
		/// <para/>Due to the web's poor ability to format pages for printing, this method of counting the current number of units (inches, cm, mm, etc.) the current text would be on a printed page was devised. For example - if one record occupies approximately 1 inch of vertical page space, the top of the forth record would start at approximately 3 inches down the page. Though not perfect, this method allows the developer to better accommodate the printed page by controlling where page breaks occur.
		/// <para/>NOTE: The units you choose to return from this function (inches, cm, mm, etc.) are irrelevant, so long as they agree with the units returned by the <c>PageLength</c> functions in the related <c>RendererList</c>, <c>RendererForm</c> and/or <c>RendererReport</c>.
		/// </remarks>
		/// <returns>Integer representing the page crawl in units for the referenced <paramref>ePageSection</paramref>.</returns>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		public virtual int PrintLength(enumPageSections ePageSection) {
			return 0;

			#region Example C# switch statement to implement
//                //#### NOTE: Only set the enumPageSections that apply to the current object .Type (i.e. there is no need to define a .cnPageHeader for a List or a Form)! These cases will be caught by the default block at the end of the switch statement.
//            switch (ePageSection) {
//                case enumPageSections.cnPageHeader: {
//                    return x;
//                    break;
//                }
//                case enumPageSections.cnHeader: {
//                    return y;
//                    break;
//                }
//                case enumPageSections.cnDetailHeader: {
//                    return z;
//                    break;
//                }
//                case enumPageSections.cnDetail: {
//                    return a;
//                    break;
//                }
//                case enumPageSections.cnDetail_NewForm: {
//                    return b;
//                    break;
//                }
//                case enumPageSections.cnMissingRecord: {
//                    return c;
//                    break;
//                }
//                case enumPageSections.cnDetailFooter: {
//                    return d;
//                    break;
//                }
//                case enumPageSections.cnNoResults: {
//                    return e;
//                    break;
//                }
//                case enumPageSections.cnFooter: {
//                    return f;
//                    break;
//                }
//                case enumPageSections.cnPageFooter: {
//                    return g;
//                    break;
//                }
//                default: {
//                        //#### Else it's an undefined enumPageSections so return a length of zero
//                    return 0;
//                    break;
//                }
//            }
			#endregion
		}

		///############################################################
		/// <summary>
		/// Outputs the header section of the rendered page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own page section.
		/// <para/>If the page has results, this function is called after <c>CollectPageResults</c>. If the page has no results, this function is called after <c>GenerateResults</c> or the <c>Class Constructor</c> (depending on the state of the provided <paramref>sResultsStack</paramref>). This function is called for every page render.
		/// </remarks>
		///############################################################
		/// <LastUpdated>May 19, 2004</LastUpdated>
		public virtual void Header() {}

		///############################################################
		/// <summary>
		/// Outputs the no results section of the rendered page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own page section.
		/// <para/>If the page has no results, this function is called after <c>Header</c>. This function is not called if there are records to render.
		/// </remarks>
		///############################################################
		/// <LastUpdated>May 19, 2004</LastUpdated>
		public virtual void NoResults() {}

		///############################################################
		/// <summary>
		/// Outputs the footer section of the rendered page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own page section.
		/// <para/>If the page has results, this function is called after <c>DetailFooter</c>. If the page has no results, this function is called after <c>NoResults</c>. This function is called for every page render.
		/// </remarks>
		///############################################################
		/// <LastUpdated>May 19, 2004</LastUpdated>
		public virtual void Footer() {}


		//##########################################################################################
		//# Render-related Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Renders the revelent section of the provided list.
		/// </summary>
		/// <remarks>
		/// Required function overload to render the revelent section of the provided list.
		/// </remarks>
		/// <param name="iRecordsPerPage">1-based integer representing the maximum number of records so show per rendered page (does not include new records).</param>
		/// <param name="sResultsStack">String representing the related Data.Pagination instance.</param>
		///############################################################
		/// <LastUpdated>May 21, 2007</LastUpdated>
		public abstract void Render(int iRecordsPerPage, string sResultsStack);

		///############################################################
		/// <summary>
		/// Renders the revelent section of the provided list.
		/// </summary>
		/// <remarks>
		/// Required function overload to render the revelent section of the provided list.
		/// </remarks>
		/// <param name="iRecordsPerPage">1-based integer representing the maximum number of records so show per rendered page (does not include new records).</param>
		/// <param name="a_sIDs">Array of strings representing all the expected IDs to render (including IDs that may no longer be available).</param>
		/// <param name="sIDsOrderedBy">String representing the order the passed <paramref>sIDsOrderedBy</paramref> is in.</param>
		///############################################################
		/// <LastUpdated>November 19, 2009</LastUpdated>
		public abstract void Render(int iRecordsPerPage, string[] a_sIDs, string sIDsOrderedBy);

	    ///############################################################
	    /// <summary>
	    /// Renders the provided list.
	    /// </summary>
	    /// <param name="oCurrentTable">PaginationTable object representing the relevant record IDs for the current table index.</param>
	    ///############################################################
	    /// <LastUpdated>April 21, 2010</LastUpdated>
	    public virtual void Render(Pagination.PaginationTable oCurrentTable) {
			    //#### If the oCurrentTable has data loaded
		    if (oCurrentTable.IDCount > 0) {
				    //#### Call .DoRenderHeaders, .DoRenderBody (which loops thru the .Detail elements) and .DoRenderFooters to .Render the oCurrentTable
			    DoRenderHeaders();
			    DoRenderBody(oCurrentTable);
			    DoRenderFooters();
		    }
			    //#### Else the oCurrentTable has no loaded data, so call .DoRenderNoResults
		    else {
			    DoRenderNoResults();
		    }
	    }

		///############################################################
		/// <summary>
		/// Outputs the no results section (surrounded by the header and footer) of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>May 21, 2007</LastUpdated>
		protected abstract void DoRenderNoResults();

		///############################################################
		/// <summary>
		/// Outputs the header and detail header sections of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 18, 2010</LastUpdated>
		protected abstract void DoRenderHeaders();

		///############################################################
		/// <summary>
		/// Outputs the detail and missing record sections (as required) of the rendered page.
		/// </summary>
		/// <param name="oCurrentTable">PaginationTable object representing the relevant record IDs for the current table index.</param>
		///############################################################
		/// <LastUpdated>March 18, 2010</LastUpdated>
		protected abstract void DoRenderBody(Pagination.PaginationTable oCurrentTable);

		///############################################################
		/// <summary>
		/// Outputs the footer and detail footer sections of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 18, 2010</LastUpdated>
		protected abstract void DoRenderFooters();


		//##########################################################################################
		//# Printing-related Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Inserts a page break.
		/// </summary>
		///############################################################
		/// <LastUpdated>June 1, 2010</LastUpdated>
		protected void DoRenderPageBreak() {
				//#### If we are g_bAllow('d to use)ResponseWrite
			if (g_bAllowResponseWrite) {
					//#### If there is no g_oParentReport
				if (g_oParentReport == null) {
						//#### Print the .cnHTMLPageBreak, reset the value of g_iPageCrawl (as we're starting on a new page) and increment the g_iPageCount
						//####     NOTE: The g_iPageCrawl is set to 0 (and not the implicate .FixLength'd .PageHeader's length) because the g_iPageCrawl will be preincremented by subsequent calls to other functions (so it's not our job to inc it here, just to reset it).
					Response.Write(Web.Settings.Value(Web.Settings.enumSettingValues.cnHTMLPageBreak));
					g_iPageCrawl = 0;
					g_iPageCount++;
				}
					//#### Else there is a g_oParentReport
				else {
						//#### Inc g_iPageCrawl by the .FixLength'd .PageFooter's length, then call the g_oParentReport's .PageFooter
						//####     NOTE: Internal calls are handeled by the .CalculatePageCrawl functions, where the .PrintLength is considered. When this is called by a developer, they should know what they're doing and have already accounted for this implicit .PageFooter
					g_iPageCrawl += FixLength(g_oParentReport.PrintLength(enumPageSections.cnPageFooter));
					g_oParentReport.PageFooter();

						//#### Print the .cnHTMLPageBreak and increment the g_iPageCount
					Response.Write(Web.Settings.Value(Web.Settings.enumSettingValues.cnHTMLPageBreak));
					g_iPageCount++;

						//#### Reset g_iPageCrawl to the .FixLength'd .PageHeader's length, then call the g_oParentReport's .PageHeader
					g_iPageCrawl = FixLength(g_oParentReport.PrintLength(enumPageSections.cnPageHeader));
					g_oParentReport.PageHeader();
				}
			}
		}

		///############################################################
		/// <summary>
		/// Calculates the unit of measure crawl down the current page.
		/// </summary>
		/// <remarks>
		/// NOTE: If the passed <paramref>iSectionLength</paramref> does not fit on the current page, an implicit page break is inserted.
		/// </remarks>
		/// <param name="iSectionLength">1-based integer representing the length of the current section.</param>
		///############################################################
		/// <LastUpdated>August 31, 2005</LastUpdated>
		protected void CalculatePageCrawl(int iSectionLength) {
				//#### If the .Section(does not)FitsOn(the)PageCount, .Insert(the)PageBreak (which we know fits from previous calls)
			if (! SectionFitsOnCurrentPage(iSectionLength)) {
				DoRenderPageBreak();
			}

				//#### Add the iSectionLength onto the (.InsertPageBreak revised) value of g_iPageCrawl
			g_iPageCrawl += iSectionLength;
		}

		///############################################################
		/// <summary>
		/// Calculates the unit of measure crawl down the current page for a <c>Detail</c> or <c>MissingRecord</c> section.
		/// </summary>
		/// <remarks>
		/// NOTE: If v1.x of C# supported Templates (or, in C# vanicular: Generics) this could be implemented with a single definition. But since dotNet 1.x does not support Templates (Generics) we do it in 2 definitions below which differ only in their signatures ("List oListForm" -vs- "Form oListForm").
		/// </remarks>
		/// <param name="oListForm"><c>List</c> or <c>Form</c> object reference to the list/form class to render.</param>
		/// <param name="bInitialCall">Reference to a boolean value indicating if this is the initial or a subsequent function call (value is updated by reference).</param>
		/// <param name="iDetailSectionLength">1-based integer representing the length of the current list/form's <c>Detail</c> section.</param>
		///############################################################
		/// <LastUpdated>March 18, 2010</LastUpdated>
		protected void CalculatePageCrawl(Base oListForm, ref bool bInitialCall, int iDetailSectionLength) {
			Renderer.List oListOrForm = oListForm as Renderer.List;

				//#### Set the values of iDetailHeaderLen and iDetailFooterLen (waiting to fix them later)
			int iDetailHeaderLen = oListOrForm.PrintLength(enumPageSections.cnDetailHeader);
			int iDetailFooterLen = oListOrForm.PrintLength(enumPageSections.cnDetailFooter);

				//#### If this is the bInitialCall
			if (bInitialCall) {
					//#### Reset the ByRef bInitialCall to false
				bInitialCall = false;
				
					//#### If a full detail .Section(does not)FitsOn(the)PageCount, .Insert(the)PageBreak
					//####     NOTE: By definition, a full .DetailHeader/.Detail/.DetailFooter volly must fit on a single page, hence the use of .FixLength below
					//####     NOTE: Since this is the bInitialCall, we do not need to render a preceeding .DetailFooter, hence the lone .InsertPageBreak below
					//####     NOTE: .InsertPageBreak resets g_iPageCrawl as required
					//####     NOTE: No .DetailFooter is outputted below as this is the bInitialCall
				if (! SectionFitsOnCurrentPage(iDetailHeaderLen + iDetailSectionLength + iDetailFooterLen)) {
					DoRenderPageBreak();
				}

					//#### Inc g_iPageCrawl by the .FixLength'd .DetailHeader's length, then call the .DetailHeader
					//####     NOTE: We individually increment g_iPageCrawl by iDetailHeaderLen, then later by iDetailSectionLength below to allow the g_iPageCrawl to be properly set for each rendering function call
				g_iPageCrawl += FixLength(iDetailHeaderLen);
				oListOrForm.DetailHeader();

					//#### Inc g_iPageCrawl by the .FixLength'd iDetailSectionLength
				g_iPageCrawl += FixLength(iDetailSectionLength);
			}
				//#### Else this is a subsequent call to print out a single .Detail or .MissingRecord
			else {
					//#### If a bottom detail .SectionFitsOn(the)PageCount
				if (SectionFitsOnCurrentPage(iDetailSectionLength + iDetailFooterLen)) {
						//#### Inc g_iPageCrawl by the .FixLength'd iDetailSectionLength
					g_iPageCrawl += FixLength(iDetailSectionLength);
				}
					//#### Else we need to break the page, closing off the previous .Details (with a .DetailFooter) and opening up the next (with a .DetailHeader)
				else {
						//#### Inc g_iPageCrawl by the .FixLength'd .DetailFooter's length, call the .DetailFooter (which we know will fit based on earlier calls) then .Insert(a)PageBreak (which prints the .PageFooter/.PageHeader)
						//####     NOTE: We increment g_iPageCrawl by iDetailFooterLen even though it will be reset by .DoRenderPageBreak to allow the g_iPageCrawl to be properly set for each rendering function call
					g_iPageCrawl += FixLength(iDetailFooterLen);
					oListOrForm.DetailFooter();
					DoRenderPageBreak();

						//#### Inc g_iPageCrawl by the .FixLength'd .DetailHeader's length, then call the .DetailHeader
						//####     NOTE: We individually increment g_iPageCrawl by iDetailHeaderLen, then later by iDetailSectionLength below to allow the g_iPageCrawl to be properly set for each rendering function call
					g_iPageCrawl += FixLength(iDetailHeaderLen);
					oListOrForm.DetailHeader();

						//#### Inc g_iPageCrawl by the .FixLength'd iDetailSectionLength
					g_iPageCrawl += FixLength(iDetailSectionLength);
				}
			}
		}

		///############################################################
		/// <summary>
		/// Ensures that the provided section length is a positive number less then or equal to the <c>PageLength</c>.
		/// </summary>
		/// <param name="iSectionLength">1-based integer representing the length of the current section.</param>
		/// <returns>1-based integer representing the single page remainder length of the current section.</returns>
		///############################################################
		/// <LastUpdated>August 31, 2004</LastUpdated>
		protected int FixLength(int iSectionLength) {
			int iReturn;

				//#### If there is stuff to print within the referenced section (and g_iPageLength is properly set to calculate the page breaks)
			if (iSectionLength > 0 && g_iPageLength > 0) {
					//#### Ensure that the iSectionLength is never longer then the g_iPageLength, setting the result into the iReturn value
				iReturn = (iSectionLength % g_iPageLength);

					//#### If the iSectionLength was exactly (or evenly divisable by) g_iPageLength, reset the iReturn value to the g_iPageLength
				if (iReturn == 0) {
					iReturn = g_iPageLength;
				}
			}
				//#### Else the passed iSectionLength is 0 or the g_iPageLength in not properly set, so set the iReturn value to 0
			else {
				iReturn = 0;
			}

				//#### Return the above determined iReturn value to the caller
			return iReturn;
		}

		///############################################################
		/// <summary>
		/// Determines if the provided section length fits on the current page.
		/// </summary>
		/// <param name="iSectionLength">1-based integer representing the length of the current section.</param>
		/// <returns>Boolean value indicating if the passed <paramref>iSectionLength</paramref> fits on the current page.</returns>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		protected bool SectionFitsOnCurrentPage(int iSectionLength) {
			bool bReturn;

				//#### Ensure the passed iSectionLength is properly .FixLength'd
			iSectionLength = FixLength(iSectionLength);

				//#### If more then just the .PageHeader has been Printed
				//####     NOTE: For non-Reports, the .cnPageHeader should be 0 so the below logic will work
			if (g_iPageCrawl > FixLength(g_oParentReport.PrintLength(enumPageSections.cnPageHeader))) {
					//#### If the iSectionLength (including a trailing .PageFooter) will run off the end of the current page, set the bReturn value to false
				if ((g_iPageCrawl + iSectionLength + FixLength(g_oParentReport.PrintLength(enumPageSections.cnPageFooter))) > g_iPageLength) {
					bReturn = false;
				}
					//#### Else the passed iSectionLength will fit on the current page, so set the bReturn value to true
				else {
					bReturn = true;
				}
			}
				//#### Else only the oPageHeader has been .Print'd, so iSectionLength will (by definition) fit on the current page, so so set the bReturn value to true
			else {
				bReturn = true;
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

	} //# class Base

} //# namespace Cn.Web.Renderer
