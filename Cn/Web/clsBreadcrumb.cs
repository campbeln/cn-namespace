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
using System.Web;                                   //# Required to access Request, Response, Application, etc.
using System.Runtime.Serialization;					//# Required to access ISerializable
using Cn.Configuration;								//# Required to access the Internationalization class

namespace Cn.Web {

    ///########################################################################################################################
    /// <summary>
	/// Manages a history of pages viewed.
    /// </summary>
    /// <remarks>
    /// This class keeps track of filename as well as querystring items Page, View, Mode, Index, OrderBy and ID in a developer-definable listing (stack) type.
    /// </remarks>
    ///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	[Serializable()]
	public class Breadcrumb : ISerializable {
			//#### Declare the required private variables
		private HttpRequest Request = HttpContext.Current.Request;      //# Since this reference will not change, it is included here rather then in .Reset
		private string g_sBreadcrumbTrail;
		private string g_sOrderBy;
		private string g_sFile;
		private string g_sFileOriginalCase;
		private string g_sPage;
		private string g_sView;
		private string g_sMode;
		private string g_sID;
		private int g_iLevel;
		private int g_iIndex;
		private enumBreadcrumbTrailTypes g_eTrailType;
		private bool g_bTrailIsDirty;

			//#### Declare the required public eNums
		#region eNums
			/// <summary>Breadcrumb trail types.</summary>
		public enum enumBreadcrumbTrailTypes : int {
				/// <summary>Views (file/page/view/mode quartet) are appended and removed as the end-user moves up and down a hieratical site structure. A previous view (and all the subsequent views) are removed only if an exact match is found.<para/>This trail functions as a LIFO (Last In First Out) queue, new views are added to the right of the trail.</summary>
			cnHieratical = 0,
				/// <summary>Each view (file/page/view/mode quartet) is represented no more then once within the trail.<para/>This trail functions as a non-determinate queue, new views are added to the right of the trail and existing file/page/view/mode quartets are overwritten in place.</summary>
			cnHistorical = 1,
				/// <summary>Each view (file/page/view/mode quartet) is indiscriminately added onto the end of the trail.<para/>New views are added to the right of the trail.</summary>
			cnBreadCrumb = 2,
				/// <summary>View stack is managed externally by the developer.<para/>The provided trail is not modified.</summary>
			cnSelfManaged = 3
		}
		#endregion

			//#### Declare the required private eNums
			//####    Breadcrumb Trail example: "TrailType|File:Page:View:Mode:Index:OrderBy:ID|...|File:Page:View:Mode:Index:OrderBy:ID" (LIFO Queue, current on right).
		private enum enumBreadcrumbTrailIndexes : int {
			cnFile = 0,
			cnPage = 1,
			cnView = 2,
			cnMode = 3,
			cnIndex = 4,
			cnOrderBy = 5,
			cnID = 6
		}

			//#### Declare the required private constants
		private const int g_cTrailLength = 7;
		private const string g_cClassName = "Cn.Web.Breadcrumb.";


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <remarks>
		/// Breadcrumb Trail example: "TrailType|File:Page:View:Mode:Index:OrderBy:ID|...|File:Page:View:Mode:Index:OrderBy:ID" (LIFO Queue, current on right).
		/// </remarks>
		/// <exception cref="Cn.CnException">Thrown when a view entry does not have the correct number of data elements within the passed <paramref>sBreadcrumbTrail</paramref>.</exception>
		///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		public Breadcrumb() {
				//#### Pass the call off to .DoReset
			DoReset("[Constructor]", FindTrail());
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <remarks>
		/// Breadcrumb Trail example: "TrailType|File:Page:View:Mode:Index:OrderBy:ID|...|File:Page:View:Mode:Index:OrderBy:ID" (LIFO Queue, current on right).
		/// </remarks>
		/// <exception cref="Cn.CnException">Thrown when a view entry does not have the correct number of data elements within the passed <paramref>sBreadcrumbTrail</paramref>.</exception>
		///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		public void Reset() {
				//#### Pass the call off to .DoReset
			DoReset("Reset", FindTrail());
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <remarks>
		/// Breadcrumb Trail example: "TrailType|File:Page:View:Mode:Index:OrderBy:ID|...|File:Page:View:Mode:Index:OrderBy:ID" (LIFO Queue, current on right).
		/// </remarks>
		/// <param name="sBreadcrumbTrail">String representing the related Breadcrumb Trail instance.</param>
		/// <exception cref="Cn.CnException">Thrown when a view entry does not have the correct number of data elements within the passed <paramref>sBreadcrumbTrail</paramref>.</exception>
		///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		public Breadcrumb(string sBreadcrumbTrail) {
				//#### Pass the call off to .DoReset
			DoReset("[Constructor]", sBreadcrumbTrail);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <remarks>
		/// Breadcrumb Trail example: "TrailType|File:Page:View:Mode:Index:OrderBy:ID|...|File:Page:View:Mode:Index:OrderBy:ID" (LIFO Queue, current on right).
		/// </remarks>
		/// <param name="sBreadcrumbTrail">String representing the related Breadcrumb Trail instance.</param>
		/// <exception cref="Cn.CnException">Thrown when a view entry does not have the correct number of data elements within the passed <paramref>sBreadcrumbTrail</paramref>.</exception>
		///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		public void Reset(string sBreadcrumbTrail) {
				//#### Pass the call off to .DoReset
			DoReset("Reset", sBreadcrumbTrail);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <remarks>
		/// Breadcrumb Trail example: "TrailType|File:Page:View:Mode:Index:OrderBy:ID|...|File:Page:View:Mode:Index:OrderBy:ID" (LIFO Queue, current on right).
		/// </remarks>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="sBreadcrumbTrail">String representing the related Breadcrumb Trail instance.</param>
		/// <exception cref="Cn.CnException">Thrown when a view entry does not have the correct number of data elements within the passed <paramref>sBreadcrumbTrail</paramref>.</exception>
		///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		private void DoReset(string sFunction, string sBreadcrumbTrail) {
			string sDOMElementPrefix;

				//#### Determine the sDOMElementPrefix
			sDOMElementPrefix = Settings.Value(Settings.enumSettingValues.cnDOMElementPrefix);

				//#### Collect g_sFileOriginalCase, g_sFile, g_sPage, g_sView, g_sMode, g_iIndex, g_sID and g_sOrderBy from the QueryString (ensuring g_iIndex is numeric) and the g_sFile name of the currently executing script
				//####     NOTE: QueryString example - "?Page=X&Mode=Y&View=Z&Index=1&OrderBy=ColumnName&ID=123"
                //####     NOTE: This same Request collection logic exists within .InitilizeTrail
			g_sFileOriginalCase = Data.Tools.MakeString(Request.ServerVariables["Script_Name"], "").Trim();
			g_sFile = g_sFileOriginalCase.ToLower();
			g_sPage = Data.Tools.MakeString(Request.QueryString[sDOMElementPrefix + "Page"], "").Trim().ToLower();
			g_sView = Data.Tools.MakeString(Request.QueryString[sDOMElementPrefix + "View"], "").Trim().ToLower();
			g_sMode = Data.Tools.MakeString(Request.QueryString[sDOMElementPrefix + "Mode"], "").Trim().ToLower();
			g_iIndex = Data.Tools.MakeInteger(Request.QueryString[sDOMElementPrefix + "Index"], 1);
			g_sID = Data.Tools.MakeString(Request.QueryString[sDOMElementPrefix + "ID"], "");
			g_sOrderBy = Data.Tools.MakeString(Request.QueryString[sDOMElementPrefix + "OrderBy"], "").Trim();

				//#### Fix g_iIndex if the user supplied value was invalid, resetting it to 1
			if (g_iIndex < 1) {
				g_iIndex = 1;
			}

				//#### If g_sID or g_sOrderBy contain potentionally malicious data, reset their value to a null-string (as they are used directly within SQL statements)
			if (! Data.SQL.Statements.IsUserDataSafe(g_sID)) {
				g_sID = "";
			}
			if (! Data.SQL.Statements.IsUserDataSafe(g_sOrderBy)) {
				g_sOrderBy = "";
			}

				//#### If the user did not set a (safe) g_sOrderBy
				//####     NOTE: The returned OrderBy is based on the following order of prescience: g_sOrderBy = the QueryString's "OrderBy" from the end-user (as long as it's safe), or else = Results.OrderedBy.ToString originally from the end-user, or else = the developer's .DefaultOrderBy (as set within .DefaultOrderBy), or else it remains a null-string
/*			if (g_sOrderBy.Length == 0) {
					//#### Set g_sOrderBy to the .OrderedBy clause within the .Results
					//####     NOTE: If there is no .OrderedBy set within the .Results, all is well as it returns a null-string and will be reset if/when the developer sets a .DefaultOrderBy
				g_sOrderBy = Results.OrderedBy.ToString(true);
			}
*/
				//#### Ensure g_bTrailIsDirty is true then process the passed sBreadcrumbTrail, ignoring the result (while setting g_sBreadcrumbTrail, g_iLevel and resetting g_bTrailIsDirty by-proxy)
				//####     NOTE: ProcessTrail is called here to catch improperly formed sBreadcrumbTrail's (not to mention to init with the initial sBreadcrumbTrail!). Besides, g_sBreadcrumbTrail is only regenerated if the developer changes g_sPage, g_sView, g_sMode, g_iIndex, g_sOrderBy, or g_sID.
			g_bTrailIsDirty = true;
			ProcessTrail(sFunction, sBreadcrumbTrail);
		}


		//##########################################################################################
		//# ISerializable Required Functions
		//##########################################################################################
        ///############################################################
        /// <summary>
		/// Initializes the class based on the provided SerializationInfo.
        /// </summary>
		/// <param name="info">Standard SerializationInfo object.</param>
		/// <param name="ctxt">Standard StreamingContext object.</param>
        ///############################################################
		/// <LastUpdated>December 21, 2009</LastUpdated>
		public Breadcrumb(SerializationInfo info, StreamingContext ctxt) {
				//#### Call .DoReset to init the class vars
		    Reset(Data.Tools.MakeString(info.GetValue("BreadcrumbTrail", typeof(string)), ""));
		}

        ///############################################################
        /// <summary>
		/// Stores the state of the class into the provided SerializationInfo.
        /// </summary>
		/// <param name="info">Standard SerializationInfo object.</param>
		/// <param name="ctxt">Standard StreamingContext object.</param>
        ///############################################################
		/// <LastUpdated>December 21, 2009</LastUpdated>
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
			info.AddValue("BreadcrumbTrail", ToString());
		}


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the current SQL order by clause.
		/// </summary>
		/// <remarks>
		/// The <c>OrderBy</c> clause is set by default based on the following order of prescience:
		/// <para/>1) <c>OrderBy</c> equals the querystring's "OrderBy" field, as defined by the end-user (as long as it's safe).
		/// <para/>2) Failing 1, <c>OrderBy</c> equals this instance's ResultsStack's "OrderedBy" field, as defined by the end-user (as long as it's safe).
		/// <para/>3) Failing 1 and 2, <c>OrderBy</c> equals the <c>DefaultOrderBy</c>, as defined by the developer (as long as it's safe).
		/// <para/>4) Failing 1, 2 and 3, <c>OrderBy</c> equals a null-string.
		/// <para/><c>OrderBy</c> clause values set via the <c>OrderBy</c> property overwrite this default value.
		/// <para/>NOTE: The value of <c>OrderBy</c> is consumed when the <c>Render</c> function is called for a List, Form or Report. Because of this, this value should be modified previous to all <c>Render</c> calls if you are hoping to change the SQL order by clause.
		/// </remarks>
		/// <exception cref="Cn.CnException">Thrown when the provided <paramref>value</paramref> contains potentionally malicious SQL directives (as defined within Internationalization's "DisallowedUserStrings" and "DisallowedUserWords" picklists).</exception>
		///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
		public string OrderBy {
			get { return g_sOrderBy; }
			set {
					//#### If the data within value is safe, set g_sOrderBy
				if (Data.SQL.Statements.IsUserDataSafe(value)) {
					g_sOrderBy = Data.Tools.MakeString(value, "");
				}
					//#### Else malicious SQL code was detected by .IsUserDataSafe, so raise the error
				else {
					Internationalization.RaiseDefaultError(g_cClassName + "OrderBy", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "OrderBy", "");
				}

					//#### Flip g_bTrailIsDirty to true
				g_bTrailIsDirty = true;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets the current file.
		/// </summary>
		/// <remarks>
		/// All values set via this property are implicitly lowercased.
		/// </remarks>
		/// <value>String representing the current file.</value>
		///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
		public string File {
			get { return g_sFileOriginalCase; }
			set {
					//#### Reset g_sFileOriginalCase and g_sFile to the passed lowercased value and flip g_bTrailIsDirty to true
				g_sFileOriginalCase = Data.Tools.MakeString(value, "");
				g_sFile = g_sFileOriginalCase.ToLower();
				g_bTrailIsDirty = true;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets the current page.
		/// </summary>
		/// <remarks>
		/// All values set via this property are implicitly lowercased.
		/// </remarks>
		/// <value>String representing the current page.</value>
		///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
		public string Page {
			get { return g_sPage; }
			set {
					//#### Reset g_sPage to the passed lowercased value and flip g_bTrailIsDirty to true
				g_sPage = Data.Tools.MakeString(value, "").ToLower();
				g_bTrailIsDirty = true;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets the current view.
		/// </summary>
		/// <remarks>
		/// All values set via this property are implicitly lowercased.
		/// </remarks>
		/// <value>String representing the current view.</value>
		///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
		public string View {
			get { return g_sView; }
			set {
					//#### Reset g_sView to the passed lowercased value and flip g_bTrailIsDirty to true
				g_sView = Data.Tools.MakeString(value, "").ToLower();
				g_bTrailIsDirty = true;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets the current mode.
		/// </summary>
		/// <remarks>
		/// All values set via this property are implicitly lowercased.
		/// </remarks>
		/// <value>String representing the current mode.</value>
		///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
		public string Mode {
			get { return g_sMode; }
			set {
					//#### Reset g_sMode to the passed lowercased value and flip g_bTrailIsDirty to true
				g_sMode = Data.Tools.MakeString(value, "").ToLower();
				g_bTrailIsDirty = true;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets the current ID.
		/// </summary>
		/// <value>String representing the current ID.</value>
		/// <exception cref="Cn.CnException">Thrown when the provided <paramref>value</paramref> contains potentionally malicious SQL directives (as defined within Internationalization's "DisallowedUserStrings" and "DisallowedUserWords" picklists).</exception>
		///############################################################
		/// <LastUpdated>June 8, 2006</LastUpdated>
		public string ID {
			get {  return g_sID; }
			set {
					//#### If the data within value is safe, set g_sID
				if (Data.SQL.Statements.IsUserDataSafe(value)) {
					g_sID = Data.Tools.MakeString(value, "");
				}
					//#### Else malicious SQL code was detected by IsUserDataSafe, so raise the error
				else {
					Internationalization.RaiseDefaultError(g_cClassName + "ID", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "ID", "");
				}

					//#### Flip g_bTrailIsDirty to true
				g_bTrailIsDirty = true;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets the current record index.
		/// </summary>
		/// <remarks>
		/// NOTE: The value of <c>Index</c> is consumed when the <c>Render</c> function is called for a List, Form or Report. Because of this, this value should be modified previous to all <c>Render</c> calls if you are hoping to change the current record index.
		/// </remarks>
		/// <value>1-based integer representing the current record index.</value>
		/// <exception cref="Cn.CnException">Thrown when the passed value is less then 0.</exception>
		///############################################################
		/// <LastUpdated>September 1, 2004</LastUpdated>
		public int Index {
			get { return g_iIndex; }
			set {
					//#### If the caller passed in a positive number, set the new g_iIndex and flip g_bTrailIsDirty to true
				if (value > 0) {
					g_iIndex = value;
					g_bTrailIsDirty = true;
				}
					//#### Else the caller passed in a negetive number, so raise the error
				else {
					Internationalization.RaiseDefaultError(g_cClassName + "Index", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_PositiveIntegerRequired, "Index", "1");
				}
			}
		}


		//##########################################################################################
		//# Public Read-Only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the realtive trail level.
		/// </summary>
		/// <remarks>
		/// The <c>Level</c> allows you to determine how the user is moving around site based on this instance's Breadcrumb Trail. Each <c>TrailType</c> behaves differently in relation to the <c>Level</c>, as detailed below.
		/// <para/><para/>For a <c>cnHieratical</c> Breadcrumb Trail, the following rules apply:
		/// <para/>If the <c>Level</c> is 0, then the end-user is moving horizontally within the same file/page/mode/view as they were viewing on the last page load (i.e. - they are paging thru a list of results).
		/// <para/>If the <c>Level</c> is 1, then the end-user is moving verticially downward (i.e. - they are "drilling down" into the results, such as viewing a "detail" page).
		/// <para/>If the <c>Level</c> is a negetive number, then the end-user is moving verticially upward (i.e. - they are utilizing a bread crumb trail to navigate back up thru the site structure).
		/// <para/><para/>For a <c>cnHistorical</c> Breadcrumb Trail, the following rules apply:
		/// <para/>If the <c>Level</c> is 1, then the file/page/view/mode is a new entry within the Breadcrumb Trail, so a new Breadcrumb Trail entry has been appended onto the trail.
		/// <para/>If the <c>Level</c> is -1, then the file/page/view/mode already existed within the Breadcrumb Trail, so the current entry has been updated.
		/// <para/><para/>For a <c>cnBreadCrumb</c> Breadcrumb Trail, the following rules apply:
		/// <para/>The <c>Level</c> is always 1, as we are always "moving down a level".
		/// <para/><para/>For a <c>cnSelfManaged</c> Breadcrumb Trail, the following rules apply:
		/// <para/>In the default implementation, the <c>Level</c> is always 1, as the Breadcrumb Trail is self managed and we therefore have no way of telling what is going on, end-user movement wise.
		/// </remarks>
		/// <value>Integer representing the realtive view level.</value>
		///############################################################
		/// <LastUpdated>December 21, 2009</LastUpdated>
		public virtual int Level {
			get { return g_iLevel; }
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating the view stack instance's type.
		/// </summary>
		/// <value>Enumeration indicating the view stack instance's type.</value>
		///############################################################
		/// <LastUpdated>December 21, 2009</LastUpdated>
		public enumBreadcrumbTrailTypes TrailType {
			get { return g_eTrailType; }
		}


		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the Breadcrumb Trail for this instance.
		/// </summary>
		/// <value>String representing the Breadcrumb Trail based on the current context.</value>
		///############################################################
		/// <LastUpdated>December 21, 2009</LastUpdated>
		public override string ToString() {
				//#### Return the results of .ProcessTrail (which will recalculate g_sBreadcrumbTrail if the g_bTrailIsDirty)
			return ProcessTrail("ToString", g_sBreadcrumbTrail);
		}

		///############################################################
		/// <summary>
        /// Builds an empty view stack conforming to the passed view stack type.
		/// </summary>
		/// <param name="eBreadcrumbTrailType">Enumeration representing the type of the provided view stack.</param>
		/// <param name="rRequest">Reference representing the HttpRequest object servicing the current request.</param>
		/// <value>String representing the current page's view stack conforming to the passed view stack type.</value>
		///############################################################
		/// <LastUpdated>November 10, 2009</LastUpdated>
		public static string InitilizeTrail(enumBreadcrumbTrailTypes eBreadcrumbTrailType, HttpRequest rRequest) {
			string sSecondaryDelimiter;
			string sDOMElementPrefix;

				//#### Determine the sDOMElementPrefix and sSecondaryDelimiter
			sDOMElementPrefix = Settings.Value(Settings.enumSettingValues.cnDOMElementPrefix);
			sSecondaryDelimiter = Configuration.Settings.SecondaryDelimiter;

                //#### Calculate and return the Breadcrumb Trail that represents the passed eBreadcrumbTrailType and the current page		
                //####     NOTE: This same Request collection logic exists within .DoReset
		    return TrailTypeToString(eBreadcrumbTrailType) + Configuration.Settings.PrimaryDelimiter +
		        Data.Tools.MakeString(rRequest.ServerVariables["Script_Name"], "").Trim() + sSecondaryDelimiter +
		        Data.Tools.MakeString(rRequest.QueryString[sDOMElementPrefix + "Page"], "").Trim().ToLower() + sSecondaryDelimiter +
		        Data.Tools.MakeString(rRequest.QueryString[sDOMElementPrefix + "View"], "").Trim().ToLower() + sSecondaryDelimiter +
		        Data.Tools.MakeString(rRequest.QueryString[sDOMElementPrefix + "Mode"], "").Trim().ToLower() + sSecondaryDelimiter +
		        Data.Tools.MakeInteger(rRequest.QueryString[sDOMElementPrefix + "Index"], 1) + sSecondaryDelimiter +
		        Data.Tools.MakeString(rRequest.QueryString[sDOMElementPrefix + "ID"], "") + sSecondaryDelimiter +
		        Data.Tools.MakeString(rRequest.QueryString[sDOMElementPrefix + "OrderBy"], "").Trim()
		    ;
		}

		///############################################################
		/// <summary>
		/// Builds a url containing the provided information.
		/// </summary>
		/// <param name="sFile">String representing the file value to use.</param>
		/// <param name="sPage">String representing the page value to use.</param>
		/// <param name="sView">String representing the view value to use.</param>
		/// <param name="sMode">String representing the mode value to use.</param>
		/// <param name="iIndex">1-based integer representing the starting record index to use.</param>
		/// <param name="sOrderBy">String representing the SQL order by clause to use.</param>
		/// <param name="sID">String representing the ID value to use.</param>
		/// <returns>String representing the Renderer querystring containing the provided information.</returns>
		/// <seealso cref="Cn.Web.Breadcrumb.GetURL"/>
		///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
		public string URL(string sFile, string sPage, string sView, string sMode, int iIndex, string sOrderBy, string sID) {
			string sDOMElementPrefix = Settings.Value(Settings.enumSettingValues.cnDOMElementPrefix);
			string sReturn = "";

				//#### Optionally append sPage, sView, sMode, iIndex, sOrderBy and sID if values for them were passed onto our sReturn value
			if (sPage.Length > 0) {
				sReturn += "&" + sDOMElementPrefix + "Page=" + sPage;
			}
			if (sView.Length > 0) {
				sReturn += "&" + sDOMElementPrefix + "View=" + sView;
			}
			if (sMode.Length > 0) {
				sReturn += "&" + sDOMElementPrefix + "Mode=" + sMode;
			}
			if (iIndex > 1) {
				sReturn += "&" + sDOMElementPrefix + "Index=" + iIndex;
			}
			if (sOrderBy.Length > 0) {
				sReturn += "&" + sDOMElementPrefix + "OrderBy=" + sOrderBy;
			}
			if (sID.Length > 0) {
				sReturn += "&" + sDOMElementPrefix + "ID=" + sID;
			}

				//#### If some optionally appended values were added to the sReturn value above, peal off the leading "&" and replace it with a querystring "?"
			if (sReturn.Length > 0) {
				sReturn = "?" + sReturn.Substring(1);
			}

				//#### Prepend the sFile onto the above determined sReturn value
			sReturn = sFile + sReturn;


				//#### Replace all spaces with web-encoded equivalent +'s (as they are prettier then %20's) and return the sReturn value to the caller
			return sReturn.Replace(" ", "+");
		}

		///############################################################
		/// <summary>
		/// Retrieves a Renderer querystring containing the information from the requested view stack index.
		/// </summary>
		/// <remarks>
		/// The passed <paramref>iIndex</paramref> can either be a positive or negetive integer. A positive (or 0) <paramref>iIndex</paramref> value will retrieve the index starting from the left of the provided <paramref>sBreadcrumbTrail</paramref>. A negetive <paramref>iIndex</paramref> value will retrieve the index starting from the right of the provided <paramref>sBreadcrumbTrail</paramref>.
		/// <para/>NOTE: If the provided <paramref>iIndex</paramref> is not within the bounds of the passed <paramref>sBreadcrumbTrail</paramref>, "?" is returned (which is the beginning of a querystring).
		/// </remarks>
		/// <param name="iIndex">0-based, negetive allowed index of the required view within the view stack.</param>
		/// <returns>String representing the Renderer querystring containing the information for the requested view stack index.</returns>
		/// <seealso cref="URL(string,string,string,string,int,string,string)"/>
		///############################################################
		/// <LastUpdated>September 1, 2004</LastUpdated>
		public string URL(int iIndex) {
				//#### Pass the call off our sibling implementation, while Process(ing the)Trail
				//####     NOTE: An exception tag is not included above because the g_sBreadcrumbTrail is tested at initilization/reset by .ProcessTrail (and therefore has no errors).
			return URL(iIndex, ProcessTrail("URL", g_sBreadcrumbTrail));
		}

		///############################################################
		/// <summary>
		/// Retrieves a Renderer querystring containing the information from the requested view stack index.
		/// </summary>
		/// <remarks>
		/// The passed <paramref>iIndex</paramref> can either be a positive or negetive integer. A positive (or 0) <paramref>iIndex</paramref> value will retrieve the index starting from the left of the provided <paramref>sBreadcrumbTrail</paramref>. A negetive <paramref>iIndex</paramref> value will retrieve the index starting from the right of the provided <paramref>sBreadcrumbTrail</paramref>.
		/// <para/>NOTE: If the provided <paramref>iIndex</paramref> is not within the bounds of the passed <paramref>sBreadcrumbTrail</paramref>, "?" is returned (which is the beginning of a querystring).
		/// </remarks>
		/// <param name="iIndex">0-based, negetive allowed index of the required view within the view stack.</param>
		/// <param name="sBreadcrumbTrail">String representing the related Breabcrumb Trail instance.</param>
		/// <returns>String representing the Renderer querystring containing the information for the requested view stack index.</returns>
		/// <exception cref="Cn.CnException">Thrown when a view entry does not have the correct number of data elements within the passed <paramref>sBreadcrumbTrail</paramref>.</exception>
		/// <seealso cref="URL(string,string,string,string,int,string,string)"/>
		///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		public string URL(int iIndex, string sBreadcrumbTrail) {
			string[] a_sBreadcrumbTrail;
			string sReturn = "?";
			int iUBound;

				//#### If there is a sBreadcrumbTrail to process
			if (!string.IsNullOrEmpty(sBreadcrumbTrail)) {
					//#### Pull sBreadcrumbTrail apart into its individual stack elements, then determine its iUBound (.Length - 1 as it's 1-based)
				a_sBreadcrumbTrail = sBreadcrumbTrail.Split(Cn.Configuration.Settings.PrimaryDelimiter.ToCharArray());
				iUBound = (a_sBreadcrumbTrail.Length - 1);

					//#### If we're supposed to start at the end of the passed sBreadcrumbTrail
				if (iIndex < 0) {
						//#### Set iIndex to the iUBound plus the passed negetive iIndex (which has the effect of counting from the back of a_sBreadcrumbTrail)
						//####     NOTE: "+ 1" as the 0th element is metadata
					iIndex = (iUBound + iIndex + 1);
				}
				    //#### Else the iIndex is positive (or 0), so inc. (as the 0th element is metadata)
				else {
				    iIndex++;
				}

					//#### If the passed (or above modified) iIndex is within the bounds of the passed sBreadcrumbTrail
					//####     NOTE: "iIndex > 0" as the 0th element is metadata
				if (iIndex > 0 && iIndex <= iUBound) {
						//#### Pull the view apart for the iIndex, setting the results back into the borrowed a_sBreadcrumbTrail
					a_sBreadcrumbTrail = a_sBreadcrumbTrail[iIndex].Split(Configuration.Settings.SecondaryDelimiter.ToCharArray());

						//#### If the view at the iIndex seems to be properly formed
					if (a_sBreadcrumbTrail.Length == g_cTrailLength) {
							//#### Build the return value based on the values for the view
						sReturn = URL(a_sBreadcrumbTrail[(int)enumBreadcrumbTrailIndexes.cnFile],
							a_sBreadcrumbTrail[(int)enumBreadcrumbTrailIndexes.cnPage],
							a_sBreadcrumbTrail[(int)enumBreadcrumbTrailIndexes.cnView],
							a_sBreadcrumbTrail[(int)enumBreadcrumbTrailIndexes.cnMode],
							Data.Tools.MakeInteger(a_sBreadcrumbTrail[(int)enumBreadcrumbTrailIndexes.cnIndex], 1),
							a_sBreadcrumbTrail[(int)enumBreadcrumbTrailIndexes.cnOrderBy],
							a_sBreadcrumbTrail[(int)enumBreadcrumbTrailIndexes.cnID]
						);
					}
						//#### Else the view is malformed, so error out
					else {
						Internationalization.RaiseDefaultError(g_cClassName + "URL", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "BreadcrumbTrail", sBreadcrumbTrail);
					}
				}
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves a Renderer querystring containing the information for the current file/page/view/mode.
		/// </summary>
		/// <returns>String representing the Renderer querystring containing the information for the current page/view/mode.</returns>
		/// <seealso cref="URL(string,string,string,string,int,string,string)"/>
		///############################################################
		/// <LastUpdated>September 1, 2004</LastUpdated>
		public string URL() {
				//#### Pass the call off our sibling implementation, while Process(ing the)Trail
				//####     NOTE: An exception tag is not included above because the g_sBreadcrumbTrail is tested at initilization/reset by .ProcessTrail (and therefore has no errors).
			return URL(g_sFileOriginalCase, g_sPage, g_sView, g_sMode, ProcessTrail("URL", g_sBreadcrumbTrail));
		}

		///############################################################
		/// <summary>
		/// Retrieves a Renderer querystring containing the information for the provided file/page/view/mode.
		/// </summary>
		/// <remarks>
		/// NOTE: If the provided page/view/mode is not found within the passed <paramref>sBreadcrumbTrail</paramref>, the default page/view/mode querystring is returned in its place.
		/// </remarks>
		/// <param name="sFile">String representing the file value to locate.</param>
		/// <param name="sPage">String representing the page value to locate.</param>
		/// <param name="sView">String representing the view value to locate.</param>
		/// <param name="sMode">String representing the mode value to locate.</param>
		/// <returns>String representing the Renderer querystring containing the information for the provided page/view/mode.</returns>
		/// <seealso cref="URL(string,string,string,string,int,string,string)"/>
		///############################################################
		/// <LastUpdated>September 1, 2004</LastUpdated>
		public string URL(string sFile, string sPage, string sView, string sMode) {
				//#### Pass the call off our sibling implementation, while Process(ing the)Trail
				//####     NOTE: An exception tag is not included above because the g_sBreadcrumbTrail is tested at initilization/reset by .ProcessTrail (and therefore has no errors).
			return URL(sFile, sPage, sView, sMode, ProcessTrail("URL", g_sBreadcrumbTrail));
		}

		///############################################################
		/// <summary>
		/// Retrieves a Renderer querystring containing the information for the provided page/view/mode.
		/// </summary>
		/// <remarks>
		/// NOTE: If the provided page/view/mode is not found within the passed <paramref>sBreadcrumbTrail</paramref>, the default page/view/mode querystring is returned in its place.
		/// </remarks>
		/// <param name="sFile">String representing the file value to locate.</param>
		/// <param name="sPage">String representing the page value to locate.</param>
		/// <param name="sView">String representing the view value to locate.</param>
		/// <param name="sMode">String representing the mode value to locate.</param>
		/// <param name="sBreadcrumbTrail">String representing the related Breadcrumb Trail instance.</param>
		/// <returns>String representing the Renderer querystring containing the information for the provided page/view/mode.</returns>
		/// <exception cref="Cn.CnException">Thrown when a view entry does not have the correct number of data elements within the passed <paramref>sBreadcrumbTrail</paramref>.</exception>
		/// <seealso cref="URL(string,string,string,string,int,string,string)"/>
		///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		public string URL(string sFile, string sPage, string sView, string sMode, string sBreadcrumbTrail) {
			string[] a_sCurrentView;
			string[] a_sBreadcrumbTrail;
			string sOrigFile;
			string sReturn = "";
			int iFileLen;
			int iPageLen;
			int iViewLen;
			int iModeLen;
			int i;

				//#### Keep track of the original sFile casing (as it matters under some enviroments)
			sOrigFile = sFile;

				//#### Lowercase the passed sPage, sView, and sMode in preperation for the comparisons below then determine their asso. .Lengths
			sFile = Data.Tools.MakeString(sFile, "").ToLower();
			sPage = Data.Tools.MakeString(sPage, "").ToLower();
			sView = Data.Tools.MakeString(sView, "").ToLower();
			sMode = Data.Tools.MakeString(sMode, "").ToLower();
			iFileLen = sFile.Length;
			iPageLen = sPage.Length;
			iViewLen = sView.Length;
			iModeLen = sMode.Length;

				//#### If there is a sBreadcrumbTrail to process
			if (sBreadcrumbTrail != null && sBreadcrumbTrail.Length > 0) {
					//#### Pull sBreadcrumbTrail apart into it's individual stack elements
				a_sBreadcrumbTrail = sBreadcrumbTrail.Split(Configuration.Settings.PrimaryDelimiter.ToCharArray());

					//#### Traverse the a_sBreadcrumbTrail from back to front (the most recient views are added on the right, no matter what mode it's in)
					//####     NOTE: "i > 0" as the 0th element is metadata
				for (i = (a_sBreadcrumbTrail.Length - 1); i > 0; i--) {
						//#### Pull the a_sCurrentView apart
					a_sCurrentView = a_sBreadcrumbTrail[i].Split(Configuration.Settings.SecondaryDelimiter.ToCharArray());

						//#### If the a_sCurrentView seems to be properly formed
					if (a_sCurrentView.Length == g_cTrailLength) {
							//#### If the sFile, sPage, sView and sMode match the current a_sCurrentView (first checking their Len()s, as that is a faster Comparison)
						if (iFileLen == a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnFile].Length &&
							iPageLen == a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnPage].Length &&
							iViewLen == a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnView].Length &&
							iModeLen == a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnMode].Length &&
							sFile == a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnFile].ToLower() &&
							sPage == a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnPage].ToLower() &&
							sView == a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnView].ToLower() &&
							sMode == a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnMode].ToLower()
						) {
								//#### Borrow the use of iPageLen to hold the MakeInteger'd .cnIndex from the a_sCurrentView
							iPageLen = Data.Tools.MakeInteger(a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnIndex], 0);

								//#### Build the sReturn value based on the values for the current view
							sReturn = URL(sOrigFile, sPage, sView, sMode, iPageLen, a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnOrderBy], a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnID]);
							break;
						}
					}
						//#### Else the current a_sCurrentView is malformed, so error out
					else {
						Internationalization.RaiseDefaultError(g_cClassName + "URL", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "BreadcrumbTrail", sBreadcrumbTrail);
					}
				}
			}

				//#### If the passed sFile/sPage/sView/sMode was not found in the passed sBreadcrumbTrail above
			if (sReturn.Length == 0) {
					//#### Set the return value to the passed sOrigFile, sPage, sView and sMode
				sReturn = URL(sOrigFile, sPage, sView, sMode, 1, "", "");
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves a column's sortable querystring based on the current <c>OrderBy</c>, inverting the initial sort order as required.
		/// </summary>
		/// <remarks>
		/// This function compares the <paramref>sInitialColumnOrderBy</paramref> clause to the current, user defined <c>OrderBy</c> clause. If the user currently has the view sorted by the <paramref>sInitialColumnOrderBy</paramref> clause, the inverse clause is returned (i.e. - "LastName,FirstName" would become "LastName DESC,FirstName DESC"). Else if the user has the view sorted by other criteria, the provided <paramref>sInitialColumnOrderBy</paramref> is returned unchanged.
		/// </remarks>
		/// <param name="sInitialColumnOrderBy">String representing the initial column SQL order by clause.</param>
		/// <returns>String representing the revelent column's sortable querystring.</returns>
		/// <seealso cref="GenerateSQLOrderBy"/>
		///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
		public string URL(string sInitialColumnOrderBy) {
				//#### Determine the sInitialColumn('s)OrderBy
			sInitialColumnOrderBy = GenerateSQLOrderBy(sInitialColumnOrderBy);

				//#### Pass the global vars along with the local sInitialColumnOrderBy into URL (passing in 1 for the g_iIndex so that we always start at the beginning of the sorted list), returning it's result
			return URL(g_sFile, g_sPage, g_sView, g_sMode, 1, sInitialColumnOrderBy, g_sID);
		}

		///############################################################
		/// <summary>
		/// Retrieves a column's SQL order by clause based on the current <c>OrderBy</c>, inverting the initial sort order as required.
		/// </summary>
		/// <remarks>
		/// This function compares the <paramref>sInitialColumnOrderBy</paramref> clause to the current, user defined <c>OrderBy</c> clause. If the user currently has the view sorted by the <paramref>sInitialColumnOrderBy</paramref> clause, the inverse clause is returned (i.e. - "LastName,FirstName" would become "LastName DESC,FirstName DESC"). Else if the user has the view sorted by other criteria, the provided <paramref>sInitialColumnOrderBy</paramref> is returned unchanged.
		/// </remarks>
		/// <param name="sInitialColumnOrderBy">String representing the initial column SQL order by clause.</param>
		/// <returns>String representing the revelent column's SQL order by clause.</returns>
		/// <seealso cref="URL(string)"/>
		///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public string GenerateSQLOrderBy(string sInitialColumnOrderBy) {
			string sReturn;

				//#### If we are supposed to invert the clause (because the passed sInitialColumnOrderBy matches the g_sOrderBy (checking its .Length first as it is a faster comparison))
			if (! string.IsNullOrEmpty(sInitialColumnOrderBy) &&
				sInitialColumnOrderBy.Length == g_sOrderBy.Length &&
				sInitialColumnOrderBy.ToLower() == g_sOrderBy.ToLower()
			) {
				Data.SQL.OrderByClause oOrderBy = new Data.SQL.OrderByClause(sInitialColumnOrderBy);

					//#### Invert the loaded sInitialColumnOrderBy and set it into our sReturn value
				oOrderBy.InvertSortOrder();
				sReturn = oOrderBy.ToString(true, false);
			}
				//#### Else the passed sInitialColumnOrderBy does not match the current g_sOrderBy (or is a null-string), so set our sReturn value to it
			else {
				sReturn = sInitialColumnOrderBy;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}


		//#######################################################################################################
		//# Private Functions
		//#######################################################################################################
		///############################################################
		/// <summary>
		/// Processes the provided view stack based on the current context.
		/// </summary>
		/// <remarks>
		/// Breadcrumb Trail example: "TrailType|Page:View:Mode:Index:OrderBy:ID|...|Page:View:Mode:Index:OrderBy:ID" (LIFO Queue, current on right).
		/// <para/>NOTE: This function modifies the Level as necessary.
		/// </remarks>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="sBreadcrumbTrail">String representing the related Breadcrumb Trail instance.</param>
		/// <returns>String representing the updated view stack based on the current context.</returns>
		/// <exception cref="Cn.CnException">Thrown when a view entry does not have the correct number of data elements within the passed <paramref>sBreadcrumbTrail</paramref>.</exception>
		///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
		private string ProcessTrail(string sFunction, string sBreadcrumbTrail) {
			string sReturn;

				//#### If the value in g_sBreadcrumbTrail is no longer current
			if (g_bTrailIsDirty) {
				string[] a_sCurrentView;
				string[] a_sBreadcrumbTrail;
				string sCurrentFPVM;
                string sBreadcrumbTrailType;
                string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter;
                string sSecondaryDelimiter = Configuration.Settings.SecondaryDelimiter;
				int i;

					//#### Set the value of sCurrentFPVM to this g_sFile:g_sPage:g_sView:g_sMode:g_iIndex:g_sOrderBy:g_sID
				sCurrentFPVM = g_sFileOriginalCase + sSecondaryDelimiter + g_sPage + sSecondaryDelimiter + g_sView + sSecondaryDelimiter + g_sMode + sSecondaryDelimiter + g_iIndex + sSecondaryDelimiter + g_sOrderBy + sSecondaryDelimiter + g_sID;

					//#### Pull sBreadcrumbTrail apart into its individual stack elements and collect the sBreadcrumbTrailType
				a_sBreadcrumbTrail = sBreadcrumbTrail.Split(sPrimaryDelimiter.ToCharArray());
                sBreadcrumbTrailType = Data.Tools.MakeString(a_sBreadcrumbTrail[0], "").ToLower();

					//#### If the passed sBreadcrumbTrail is empty or otherwise invalid (requiring at least 2 entries as the first is metadata holding the TrailType)
				if (sBreadcrumbTrail.Length == 0 || a_sBreadcrumbTrail.Length < 2) {
						//#### Default the value of g_eTrailType to .cnHieratical
					g_eTrailType = enumBreadcrumbTrailTypes.cnHieratical;

						//#### Set g_sBreadcrumbTrail to the sCurrentFPVM (plus the prefixed TrailType) and set g_iLevel to 1 (as we have "moved down a level" to a new view)
					g_sBreadcrumbTrail = TrailTypeToString(g_eTrailType) + sPrimaryDelimiter + sCurrentFPVM;
					g_iLevel = 1;
				}
					//#### Else the sBreadcrumbTrail is holding a value to parse
				else {
						//#### Determine the value of sBreadcrumbTrailType and process accordingly
					switch (sBreadcrumbTrailType) {
							//#### If this is a .cnHistorical style sBreadcrumbTrail
							//####     NOTE: Each PVM is represended no more then once within the g_sBreadcrumbTrail. The newest entries are added on the right.
						case "historical": {
							int iFileLen = g_sFile.Length;
							int iPageLen = g_sPage.Length;
							int iViewLen = g_sView.Length;
							int iModeLen = g_sMode.Length;

								//#### Set the value of g_eTrailType
							g_eTrailType = enumBreadcrumbTrailTypes.cnHistorical;

								//#### Traverse the a_sBreadcrumbTrail searching for the current g_sFile/g_sPage/g_sView/g_sMode
								//####     NOTE: "i = 0" is utilized as the 0th index is metadata
							for (i = 1; i < a_sBreadcrumbTrail.Length; i++) {
									//#### Set the a_sCurrentView for this loop (lowercasing the current a_sBreadcrumbTrail in prep for the comparisons below)
								a_sCurrentView = a_sBreadcrumbTrail[i].ToLower().Split(sSecondaryDelimiter.ToCharArray());

									//#### If the a_sCurrentView seems to be properly formatted
								if (a_sCurrentView.Length == g_cTrailLength) {
										//#### If the a_sCurrentView matches the current g_sFile/g_sPage/g_sView/g_sMode (checking their .Lengths first as it is a far faster comparison)
									if (a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnFile].Length == iFileLen &&
										a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnPage].Length == iPageLen &&
										a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnView].Length == iViewLen &&
										a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnMode].Length == iModeLen &&
										a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnFile] == g_sFile &&
										a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnPage] == g_sPage &&
										a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnView] == g_sView &&
										a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnMode] == g_sMode
									) {
											//#### Reset the a_sCurrentView within a_sBreadcrumbTrail to the revised sCurrentFPVM, re-.Join the a_sBreadcrumbTrail into g_sBreadcrumbTrail and set g_iLevel to -1 (which represents the current g_sPage/g_sView/g_sMode already existed within the sBreadcrumbTrail)
										a_sBreadcrumbTrail[i] = sCurrentFPVM;
										g_sBreadcrumbTrail = string.Join(sPrimaryDelimiter, a_sBreadcrumbTrail);
										g_iLevel = -1;

											//#### Set i to the special value of -2 (so we skip the additional processing below) and exit the loop
										i = -2;
										break;
									}
								}
									//#### Else the current sBreadcrumbTrail is malformed, so error out
								else {
									Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "BreadcrumbTrail", sBreadcrumbTrail);
								}
							}

								//#### If the current g_sFile/g_sPage/g_sView/g_sMode was not found above
							if (i != -2) {
									//#### Append the sCurrentFPVM onto the passed sBreadcrumbTrail (setting the result into g_sBreadcrumbTrail) and set g_iLevel to 1 (as we are always "moving down a level")
								g_sBreadcrumbTrail = sBreadcrumbTrail + sPrimaryDelimiter + sCurrentFPVM;
								g_iLevel = 1;
							}
							break;
						}

							//#### If this is a .cnSelfManaged style sBreadcrumbTrail
							//####     NOTE: Self Managed view stacks are just as they sound - managed by the developer. The g_sBreadcrumbTrail is set to whatever the developer passed in inside of sBreadcrumbTrail
						case "selfmanaged": {
								//#### Set the value of g_eTrailType
							g_eTrailType = enumBreadcrumbTrailTypes.cnSelfManaged;

							    //#### Pass the call off to the optionally overriden .SelfManagedTrail, collecting it's return value back into the g_sBreadcrumbTrail
							g_sBreadcrumbTrail = SelfManagedTrail(sBreadcrumbTrail);
							break;
						}

							//#### If this is a .cnBreadCrumb style sBreadcrumbTrail
							//####     NOTE: Each PVM is indiscriminately added to the end of the g_sBreadcrumbTrail. The newest entries are added on the right.
						case "breadcrumb": {
								//#### Set the value of g_eTrailType
							g_eTrailType = enumBreadcrumbTrailTypes.cnBreadCrumb;

								//#### Append the sCurrentFPVM onto the passed sBreadcrumbTrail (setting the result into g_sBreadcrumbTrail) and set g_iLevel to 1 (as we are always "moving down a level")
							g_sBreadcrumbTrail = sBreadcrumbTrail + sPrimaryDelimiter + sCurrentFPVM;
							g_iLevel = 1;
							break;
						}
						
							//#### Else this must be a .cnHieratical style sBreadcrumbTrail
							//####     NOTE: PVMs are appended and removed as the user moves up and down the structure. PVMs are only removed if an exact match for the current g_sPage/g_sView/g_sMode is found within sBreadcrumbTrail. This functions as a LIFO queue.
						default: { //case "hieratical": {
							int iUBound;
							int iLen;

								//#### Set the values of g_eTrailType, iUBound and the iLen
							g_eTrailType = enumBreadcrumbTrailTypes.cnHieratical;
							iUBound = (a_sBreadcrumbTrail.Length - 1);
							iLen = sCurrentFPVM.Length;

								//#### Borrow the use of the sReturn value to store the .ToLower'ed version of the sCurrentFPVM
								//####     NOTE: The sReturn value is always reset below before returning the value to the caller, so there is no need to reset this to a null-string after the loop
							sReturn = sCurrentFPVM.ToLower();

								//#### Traverse the a_sBreadcrumbTrail looking for the newest (hence from back to front) sBreadcrumbTrail element that matches the .ToLower'ed sCurrentFPVM (as in the borrowed sReturn value)
								//####     NOTE: "i > 0" is utilized as the 0th index is metadata
							for (i = iUBound; i > 0; i--) {
									//#### If this BreadcrumbTrail element exactly matches the sCurrentFPVM (first checking it's .Length, as that is a far faster comparison)
								if (a_sBreadcrumbTrail[i].Length == iLen && a_sBreadcrumbTrail[i].ToLower() == sReturn) {
										//#### Set g_iLevel to the number of levels up (negetive) we are traversing
									g_iLevel = (i - iUBound);

										//#### Reset the value of iUBound and set the g_sBreadcrumbTrail to a null-string in prep for the loop below
									iUBound = i;
									g_sBreadcrumbTrail = "";

										//#### Borrow the use of iLen to traverse the still valid section of the a_sBreadcrumbTrail
										//####     NOTE: We go up to (but do not include) the iUBound index so that we can avoid having to peal off a trailing .PrimaryDelimiter (the iUBound index is appended below)
									for (iLen = 0; iLen < iUBound; iLen++) {
											//#### Append the current a_sBreadcrumbTrail element back onto the g_sBreadcrumbTrail (followed by a trailing .PrimaryDelimiter)
										g_sBreadcrumbTrail += a_sBreadcrumbTrail[iLen] + sPrimaryDelimiter;
									}

										//#### Append the final valid index onto the g_sBreadcrumbTrail, reset i to the special value of -2 and exit the loop
									g_sBreadcrumbTrail += a_sBreadcrumbTrail[iUBound];
									i = -2;
									break;
								}
							}

								//#### If the sCurrentFPVM was not found above
							if (i != -2) {
									//#### Set the a_sCurrentView for this loop (lowercasing the current a_sBreadcrumbTrail in prep for the comparisons below)
								a_sCurrentView = a_sBreadcrumbTrail[iUBound].ToLower().Split(sSecondaryDelimiter.ToCharArray());

									//#### If the a_sCurrentView seems to be properly formatted
								if (a_sCurrentView.Length == g_cTrailLength) {
										//#### If the a_sCurrentView matches the current g_sFile/g_sPage/g_sView/g_sMode
										//####     NOTE: Since this comparison is only made once (i.e. not within a loop), the .Lengths are not comparied
									if (a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnFile] == g_sFile &&
										a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnPage] == g_sPage &&
										a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnView] == g_sView &&
										a_sCurrentView[(int)enumBreadcrumbTrailIndexes.cnMode] == g_sMode
									) {
											//#### Reset the a_sCurrentView with the sCurrentFPVM, setting g_sBreadcrumbTrail to the resulting string and set g_iLevel to 0 (as we are staying on the current view level)
										a_sBreadcrumbTrail[iUBound] = sCurrentFPVM;
										g_sBreadcrumbTrail = string.Join(sPrimaryDelimiter, a_sBreadcrumbTrail);
										g_iLevel = 0;
									}
										//#### Else this is a new BreadcrumbTrail element
									else {
											//#### Append the sCurrentFPVM onto the sBreadcrumbTrail (setting the resulting string into g_sBreadcrumbTrail) and set g_iLevel to 1 (as we have moved down to a new view level)
										g_sBreadcrumbTrail = sBreadcrumbTrail + sPrimaryDelimiter + sCurrentFPVM;
										g_iLevel = 1;
									}
								}
									//#### Else the current sBreadcrumbTrail is malformed, so error out
								else {
									Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "BreadcrumbTrail", sBreadcrumbTrail);
								}
							}
							break;
						}						
					}
				}

					//#### Set our sReturn value to the above (re)created g_sBreadcrumbTrail and flip g_bTrailIsDirty to false
				sReturn = g_sBreadcrumbTrail;
				g_bTrailIsDirty = false;
			}
				//#### Else the value in g_sBreadcrumbTrail is current, so set our sReturn value to it
			else {
				sReturn = g_sBreadcrumbTrail;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Converts the provided view stack type enumeration into it's assoicated string equivlent.
		/// </summary>
		/// <param name="eBreadcrumbTrailType">Enumeration representing the type of view stack.</param>
		///############################################################
		/// <LastUpdated>November 12, 2009</LastUpdated>
        private static string TrailTypeToString(enumBreadcrumbTrailTypes eBreadcrumbTrailType) {
			return eBreadcrumbTrailType.ToString().Substring(2);
        }

		///############################################################
		/// <summary>
		/// Attempts to find the view stack using it's default name from the .Request and .Session objects
		/// </summary>
		///############################################################
		/// <LastUpdated>March 4, 2010</LastUpdated>
		private string FindTrail() {
			string sDOMElementPrefix;
			string sReturn;

				//#### Determine the sDOMElementPrefix
			sDOMElementPrefix = Settings.Value(Settings.enumSettingValues.cnDOMElementPrefix);

                //#### First try and collect the BreadcrumbTrail from the Request (.Form, .QueryString, etc)
		    sReturn = Data.Tools.MakeString(Request[sDOMElementPrefix + "BreadcrumbTrail"], "");

		        //#### If a BreadcrumbTrail was not found above, try to collect it from the Session
		    if (sReturn.Length == 0 && HttpContext.Current.Session != null) {
		        sReturn = Data.Tools.MakeString(HttpContext.Current.Session[sDOMElementPrefix + "BreadcrumbTrail"], "");
		    }

		        //#### If a BreadcrumbTrail was still not found above, default to the DefaultBreadcrumbTrail
		    if (sReturn.Length == 0) {
		        sReturn = InitilizeTrail(Settings.DefaultBreadcrumbTrailType, Request);
		    }

		        //#### Return the above determined sReturn value (if any)
		    return sReturn;
		}

		///############################################################
		/// <summary>
		/// Processes the view stack when it's type is set to "SelfManaged".
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to process the view stack when it's type is set to "SelfManaged".
		/// </remarks>
		/// <param name="sBreadcrumbTrail">String representing the related Breadcrumb Trail instance that is currently being processed.</param>
		///############################################################
		/// <LastUpdated>November 9, 2009</LastUpdated>
        protected virtual string SelfManagedTrail(string sBreadcrumbTrail) {
				//#### Assume a g_iLevel of 1 and simply return the passed sBreadcrumbTrail (which is set into g_sBreadcrumbTrail)
				//####     *OLD* NOTE: Since there is no means for the developer to specify the Level within the ResultsStack string itself, there is no way to allow the developer to set their own Level
			g_iLevel = 1;
			return sBreadcrumbTrail;
        }

	} //# public class Breadcrumb

} //# namespace Cn.Web
