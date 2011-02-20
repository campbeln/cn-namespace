/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Cn.Data;										//# Required to access MetaData, Pagination, Picklists, etc.


namespace Cn.Web.Renderer {

	///########################################################################################################################
	/// <summary>
	/// Renderer.List class.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>May 16, 2007</LastFullCodeReview>
	public abstract class List : Base {
			//#### Declare the required private constants
		private const string g_cClassName = "Cn.Web.Renderer.List.";


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <seealso cref="Cn.Web.Renderer.List.Reset()"/>
		///############################################################
		/// <LastUpdated>November 6, 2009</LastUpdated>
		public List() : base(enumRendererObjectTypes.cnList, g_cClassName) {
				//#### Call our .Reset to init the class vars
			Reset();
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <seealso cref="Cn.Web.Renderer.List.Reset(Web.Settings.Current)"/>
		///############################################################
		/// <LastUpdated>November 6, 2009</LastUpdated>
		public List(Settings.Current oSettings) : base(enumRendererObjectTypes.cnList, g_cClassName) {
				//#### Call our .Reset to init the class vars
			Reset(oSettings);
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <remarks>
		/// NOTE: This protected interface is provided as a pass-thru for derived types (specificially Renderer.Form's)
		/// </remarks>
		/// <param name="eType">Enumeration representing the object type of this instance.</param>
		/// <param name="sClassPath">String representing the class path of this instance.</param>
		/// <seealso cref="Cn.Web.Renderer.Base.Reset"/>
		///############################################################
		/// <LastUpdated>March 19, 2010</LastUpdated>
		protected List(enumRendererObjectTypes eType, string sClassPath) : base(eType, sClassPath) {
		}


		//##########################################################################################
		//# Render-related Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Renders the revelent section of the provided list.
		/// </summary>
		/// <param name="iRecordsPerPage">1-based integer representing the maximum number of records so show per rendered page (does not include new records).</param>
		/// <param name="sResultsStack">String representing the related <c>Pagination</c> instance.</param>
		///############################################################
		/// <LastUpdated>May 31, 2010</LastUpdated>
		public override void Render(int iRecordsPerPage, string sResultsStack) {
				//#### Pass the call off to our base class's .DoRender (while also calling .DoCollectResults, to... well, collect the results =), signaling .DoRender to .Reset ourselves in prep. for any subsequent calls
			DoRender(DoCollectResults(iRecordsPerPage, sResultsStack, null, ""), true);
		}

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
		/// <LastUpdated>May 31, 2010</LastUpdated>
		public override void Render(int iRecordsPerPage, string[] a_sIDs, string sIDsOrderedBy) {
				//#### Pass the call off to our base class's .DoRender (while also calling .DoCollectResults, to... well, collect the results =), signaling .DoRender to .Reset ourselves in prep. for any subsequent calls
			DoRender(DoCollectResults(iRecordsPerPage, "", a_sIDs, sIDsOrderedBy), true);
		}

		///############################################################
		/// <summary>
		/// Collects the page results in preperation for rendering.
		/// </summary>
		/// <remarks>
		/// NOTE: This function resets RecordsPerPage, TableRecordIndex, TableIndex, and RenderedRecordCount, loads the sResultsStack and returns the page ResultsStack in preperation for the List or Form render.
		/// <para/>
		/// <para/>The <c>OrderBy</c> clause is set by default based on the following order of prescience:
		/// <para/>1) <c>OrderBy</c> equals the querystring's "OrderBy" field, as defined by the end-user (as long as it's safe).
		/// <para/>2) Failing 1, <c>OrderBy</c> equals this instance's Results' "OrderedBy" field, as defined by the end-user (as long as it's safe).
		/// <para/>3) Failing 1 and 2, <c>OrderBy</c> equals the <c>DefaultOrderBy</c>, as defined by the developer (as long as it's safe).
		/// <para/>4) Failing 1, 2 and 3, <c>OrderBy</c> equals a null-string.
		/// <para/><c>OrderBy</c> clause values set via the <c>OrderBy</c> property overwrite this default value.
		/// </remarks>
		/// <param name="iRecordsPerPage">1-based integer representing the maximum number of records so show per rendered page (does not include new records).</param>
		/// <param name="sResultsStack">String representing the related Data.Pagination instance.</param>
		/// <param name="a_sIDs">Array of strings representing all the expected IDs to render (including IDs that may no longer be available).</param>
		/// <param name="sIDsOrderedBy">String representing the order the passed <paramref>sIDsOrderedBy</paramref> is in.</param>
		/// <returns>Pagination object representing the revelent section of the provided List or Form.</returns>
		///############################################################
		/// <LastUpdated>March 19, 2010</LastUpdated>
		internal virtual Pagination DoCollectResults(int iRecordsPerPage, string sResultsStack, string[] a_sIDs, string sIDsOrderedBy) {
			Pagination oReturn;
			string sResultsOrderedBy = "";
			bool bGenerateResults = false;
			bool bReorderResults = false;

				//#### (Re)Set the view-specific g_oPagination and RecordsPerPage
				//####     NOTE: Only these values are (re)set here as the remaining values handeled within .Reset are important across view renders (but not across .Resets)
			g_oPagination.Reset();
			g_iRecordsPerPage = iRecordsPerPage;

				//#### If some a_sIDs were passed
			if (a_sIDs != null && a_sIDs.Length > 0) {
					//#### Load the a_sIDs under "Undefined.Undefined" and set sResultsOrderedBy to the passed sIDsOrderedBy
					//####     NOTE: We cannot .Set(the)CollectedIDs below because that is done within the base implementation of .CollectPageResults which is always called anyway.
				g_oPagination.Load("undefined", "undefined", a_sIDs);
				sResultsOrderedBy = sIDsOrderedBy;
			}
				//#### Else if the caller passed in a sResultsStack to parse
			else if (! string.IsNullOrEmpty(sResultsStack)) {
					//#### Load the passed sResultsStack into the g_oPagination (ignoring it's return value) and determine the sResultsOrderedBy
				g_oPagination.Load(sResultsStack);
				sResultsOrderedBy = g_oPagination.OrderedBy.ToString(true, false).ToLower();
			}

				//#### If the g_oPagination was populated above
			if (g_oPagination.DataIsLoaded) {
					//#### If the user has defined a .Breadcrumb.OrderBy clause that differs from the sResultsOrderedBy clause
				if (g_oSettings.Breadcrumb.OrderBy.ToLower() != sResultsOrderedBy) {
						//#### Set bGenerateResults so that .GenerateResults is called, and flip bReorderResults to true as the results set needs to be reordered
					bGenerateResults = true;
					bReorderResults = true;

						//#### .Load the .Breadcrumb's .OrderBy clause into the g_oPagination
					g_oPagination.OrderedBy.Load(g_oSettings.Breadcrumb.OrderBy);
				}
					//#### Else if there is no sResultsOrderedBy clause currently defined and the developer has set a g_sDefaultOrderBy clause
				else if (sResultsOrderedBy.Length == 0 && g_sDefaultOrderBy.Length > 0) {
						//#### Set bGenerateResults so that .GenerateResults is called, and flip bReorderResults to true as the results set needs to be reordered
					bGenerateResults = true;
					bReorderResults = true;

//! Report's .DefaultOrderBy utilized here!?!
						//#### .Load the g_sDefaultOrderBy clause into the g_oPagination
					g_oPagination.OrderedBy.Load(g_sDefaultOrderBy);
				}
					//#### Else we'll need to ensure the passed sResultsStack is representing the correct results window
				else {
						//#### If the current .Index is beneath or beyond the current results window
					if (g_oSettings.Breadcrumb.Index < g_oPagination.StartRecord || g_oSettings.Breadcrumb.Index > (g_oPagination.IDCount - 1 + g_oPagination.StartRecord)) {
							//#### Set bGenerateResults so that .GenerateResults is called, and leave bReorderResults as false because a reorder is not necessary (as this is an initial generation of the results set)
						bGenerateResults = true;
					  //bReorderResults = false;
					}
				}
			}
				//#### Else g_oPagination was not populated above
			else {
					//#### Set bGenerateResults so that .GenerateResults is called, and leave bReorderResults as false because a reorder is not necessary (as this is an initial generation of the results set)
				bGenerateResults = true;
			  //bReorderResults = false;

					//#### If the user (or developer) has defined an .OrderBy clause, .Load it now
					//####     NOTE: We reference the .Breadcrumb via our property so that it is initilized if necessary
				if (Breadcrumb.OrderBy.Length > 0) {
					g_oPagination.OrderedBy.Load(g_oSettings.Breadcrumb.OrderBy);
				}
			}

				//#### If we are supposed to bGenerate(the)Results, call .GenerateResults
				//####     NOTE: Since .GenerateResults is defined within this abstract class, we can call it directly here without the need for casting ourselves as a derived class type (as was previously the case)
			if (bGenerateResults) {
				GenerateResults(g_oPagination, bReorderResults);
			}

				//#### Collect the range of IDs from the rRenderer.Results, setting it into our oReturn value
			oReturn = g_oPagination.Range(g_oSettings.Breadcrumb.Index, iRecordsPerPage);

				//#### If the PageResults was successfully returned with .IDs, we need to .Collect(the)PageResults (which modifies and returns our oReturn value byref)
			if (oReturn != null && oReturn.IDCount > 0) {
				CollectPageResults(oReturn);
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Renders the <c>List</c>/<c>Form</c>.
		/// </summary>
		/// <param name="oPageResults">Pagination object representing this page's relevant record IDs.</param>
		/// <param name="bResetAfterRender">Boolean value representing if we are supposed to Reset the class with the current Settings after the render.</param>
		///############################################################
		/// <LastUpdated>June 22, 2010</LastUpdated>
		internal virtual void DoRender(Pagination oPageResults, bool bResetAfterRender) {
			int i;

				//#### (Re)Set the view-specific indexes/counts
				//####     NOTE: .TableIndex kinda does not need to be reset below (as it is used as the loop incrementer when .Render'ing the List/Form/Report), but is still done so below in case of a "NoResults" render
			DoResetIndexes();

				//#### If the oPage('s)Results was not successfully returned or if it's .IDCount is 0, call .DoRenderNoResults
			if (oPageResults == null || oPageResults.IDCount == 0) {
				DoRenderNoResults();
			}
				//#### Else we have some results to .Render
			else {
					//#### Traverse each .Table within the oPage('s)Results (incrementing g_iTableIndex as we go)
					//####     NOTE: .DoRenderHeaders and .DoRenderFooters are called from within the innder .Render call
				for (i = 0; i < oPageResults.TableCount; i++) {
						//#### Reset g_iTableIndex to the current table index and g_iTableRecordIndex in prep. for the loop within .Render
						//####     NOTE: We don't use g_iTableIndex as the indexer (whereby auto-incrementing it as we traverse the for) so as to allow us to reset the g_iTableIndex elsewhere in the codebase without screwing up this loop
					g_iTableIndex = i;
					g_iTableRecordIndex = -1;

						//#### Collect the current g_iTableIndex from the oPageResults, passing it into .Render
					Render(oPageResults.Table(g_iTableIndex));
				}
			}

				//#### If we are supposed to bResetAfterRender, .Reset ourselves in prep. for any subsequent calls
			if (bResetAfterRender) {
				Reset(g_oSettings);
			}
		}

		///############################################################
		/// <summary>
		/// Outputs the no results section (surrounded by the header and footer) of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>June 1, 2007</LastUpdated>
		protected override void DoRenderNoResults() {
				//##### If we're in .Printable mode
			if (Printable) {
					//#### Call the volley of subs to .Render as "NoResults", .Calculate(ing each's)PageCrawl as we go
				CalculatePageCrawl(PrintLength(enumPageSections.cnHeader));
				Header();
				CalculatePageCrawl(PrintLength(enumPageSections.cnNoResults));
				NoResults();
				CalculatePageCrawl(PrintLength(enumPageSections.cnFooter));
				Footer();
			}
				//#### Else we're not in .Printable mode, so call the volley of subs to .Render as "NoResults"
			else {
				Header();
				NoResults();
				Footer();
			}
		}

		///############################################################
		/// <summary>
		/// Outputs the header and detail header sections of the rendered page.
		/// </summary>
		/// <remarks>
		/// NOTE: We can get away with the get to inline cast of this into an IListForm because .cnReport's must override these virtual functions
		/// </remarks>
		///############################################################
		/// <LastUpdated>June 1, 2010</LastUpdated>
		protected override void DoRenderHeaders() {
				//##### If we're in .Printable mode
			if (Printable) {
					//#### Print out the the .Header (after .Calculate(ing its)PageCrawl)
					//####     NOTE: .DetailHeader is not called here as it's outputted by the first call to .CalculatePageCrawl in the for loop within .DoRenderBody (thanks to the use of bInitialDetailCall)
				CalculatePageCrawl(PrintLength(enumPageSections.cnHeader));
				Header();
			}
				//#### Else we're not in .Printable mode, so print out the .Header and .DetailHeader
			else {
				Header();
				DetailHeader();
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
			int i;

				//#### Traverse the .IDs for the oCurrentTable
			for (i = 0; i < oCurrentTable.IDs.Length; i++) {
					//#### Inc g_iRecordCount (this is not used as the loop's iterater as it keeps track of the .RenderedRecordCount across all of the results entries)
				g_iRecordCount++;

					//#### If this .ID was successfully collected within .CollectPageResults
				if (oCurrentTable.CollectedID(i)) {
						//#### If we're in .Printable mode, .Calculate(the)PageCrawl for the .Detail
					if (Printable) {
						CalculatePageCrawl(this, ref g_bInitialDetailCall, PrintLength(enumPageSections.cnDetail));
					}

						//#### Inc g_iTableRecordIndex and call our .Detail
					g_iTableRecordIndex++;
					Detail();
				}
					//#### Else this .ID was not successfully collected within .CollectPageResults
				else {
						//#### If we're in .Printable mode, .Calculate(the)PageCrawl for the .MissingRecord
					if (Printable) {
						CalculatePageCrawl(this, ref g_bInitialDetailCall, PrintLength(enumPageSections.cnMissingRecord));
					}

						//#### Call our .MissingRecord
					MissingRecord(oCurrentTable.TableName, oCurrentTable.IDs[i]);
				}
			}
		}

		///############################################################
		/// <summary>
		/// Outputs the footer and detail footer sections of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>June 1, 2010</LastUpdated>
		protected override void DoRenderFooters() {
				//##### If we're in .Printable mode
			if (Printable) {
					//#### Call the .DetailFooter and .Footer subs to complete the .Render, .Calculate(ing each's)PageCrawl as we go
					//####     NOTE: We know that the .DetailFooter will fit on the current page thanks to the .CalculatePageCrawl calls above
				g_iPageCrawl += FixLength(PrintLength(enumPageSections.cnDetailFooter));
				DetailFooter();
				CalculatePageCrawl(PrintLength(enumPageSections.cnFooter));
				Footer();
			}
				//#### Else we're not in .Printable mode
			else {
					//#### Call the .DetailFooter and .Footer subs to complete the .Render
				DetailFooter();
				Footer();
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
		/// <LastUpdated>May 19, 2004</LastUpdated>
		public virtual void DetailHeader() {}

		///############################################################
		/// <summary>
		/// Outputs a detail section of the rendered page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own page section.
		/// <para/>This function is called once per successfully collected record. This function is not called if there are no successfully collected records to render.
		/// </remarks>
		///############################################################
		/// <LastUpdated>June 21, 2004</LastUpdated>
		public virtual void Detail() {}

		///############################################################
		/// <summary>
		/// Outputs a missing record section of the rendered page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own page section.
		/// <para/>This function is called once per missing record. This function is not called if there are no missing records to render.
		/// </remarks>
		///############################################################
		/// <LastUpdated>May 19, 2004</LastUpdated>
		public virtual void MissingRecord(string sTableName, string sID) {}

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
		/// <LastUpdated>May 19, 2004</LastUpdated>
		public virtual void DetailFooter() {}

	} //# class List


} //# namespace Cn.Web.Renderer
