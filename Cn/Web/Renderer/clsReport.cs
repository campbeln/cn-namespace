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
	/// Renderer.Report class.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>May 22, 2007</LastFullCodeReview>
	public abstract class Report : Base {
			//#### Declare the required private constants
		private const string g_cClassName = "Cn.Web.Renderer.Report.";


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <seealso cref="Cn.Web.Renderer.Report.Reset"/>
		///############################################################
		/// <LastUpdated>November 13, 2009</LastUpdated>
		public Report() : base(enumRendererObjectTypes.cnReport, g_cClassName) {
				//#### Call .Reset to init the class vars
			Reset();
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <seealso cref="Cn.Web.Renderer.Report.Reset"/>
		///############################################################
		/// <LastUpdated>November 13, 2009</LastUpdated>
		public Report(Web.Settings.Current oSettings) : base(enumRendererObjectTypes.cnReport, g_cClassName) {
				//#### Call .Reset to init the class vars
			Reset(oSettings);
		}


		//##########################################################################################
		//# Render-related Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Renders the revelent section of the provided <c>Report</c>.
		/// </summary>
		/// <param name="iRecordsPerPage">1-based integer representing the maximum number of records so show per rendered page (does not include new records).</param>
		/// <param name="sResultsStack">String representing the related Data.Pagination instance.</param>
		/// <exception cref="Cn.CnException">Thrown when the <c>RendererObject</c> returned from the <paramref>rReport</paramref>'s <c>Body</c> is unreconized.</exception>
		///############################################################
		/// <LastUpdated>March 17, 2010</LastUpdated>
		public override void Render(int iRecordsPerPage, string sResultsStack) {
				//#### Pass the call off to our base class's .DoRender (while also calling .DoCollectResults, to... well, collect the results =)
//! will these calls to .base will function properly?
//			base.DoRender(base.DoCollectResults(iRecordsPerPage, sResultsStack, null, ""));
Web.Tools.dEnd("this needs to be revamped to work with the updated Renderer.Base");

				//#### .Reset ourselves in prep. for any subsequent calls
			Reset(g_oSettings);
		}

//! public override void Render(int iRecordsPerPage, string[] a_sIDs, string sIDsOrderedBy)

		///############################################################
		/// <summary>
		/// Outputs the no results section (surrounded by the header and footer) of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		protected override void DoRenderNoResults() {
				//##### If we're in .Printable mode
			if (Printable) {
					//#### Calculate the g_iPageCrawl while rendering the .PageHeader and .Header
					//####     NOTE: By definition, the .PageHeader and .Header will fit on a single page (or is otherwise handeled by the developer), hence the inline calculation and the use of .FixLength
				g_iPageCrawl = FixLength(PrintLength(enumPageSections.cnPageHeader));
				PageHeader();
				g_iPageCrawl = FixLength(PrintLength(enumPageSections.cnHeader));
				Header();

					//#### Print out the .NoResults (after .Calculate(ing its)PageCrawl)
				CalculatePageCrawl(PrintLength(enumPageSections.cnNoResults));
				NoResults();

					//#### Calculate the g_iPageCrawl while rendering the .PageHeader and .Header
					//####     NOTE: By definition, the .PageFooter and .Footer will fit on a single page (or is otherwise handeled by the developer), hence the inline calculation and the use of .FixLength
				CalculatePageCrawl(PrintLength(enumPageSections.cnFooter));
				Footer();
				g_iPageCrawl = FixLength(PrintLength(enumPageSections.cnFooter));
				PageFooter();
			}
				//#### Else we're not in .Printable mode
			else {
					//#### Call the volley of subs to .Render the "NoResults" Report
				PageHeader();
				Header();
				NoResults();
				Footer();
				PageFooter();
			}
		}

		///############################################################
		/// <summary>
		/// Outputs the header and detail header sections of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		protected override void DoRenderHeaders() {
				//#### If we're in .Printable mode, Calculate the g_iPageCrawl inline for the .PageHeader and .Header
			if (Printable) {
					//#### Calculate the g_iPageCrawl while rendering the .PageHeader and .Header
					//####     NOTE: By definition, the .PageHeader and .Header will fit on a single page (or is otherwise handeled by the developer), hence the inline calculation and the use of .FixLength
				g_iPageCrawl = FixLength(PrintLength(enumPageSections.cnPageHeader));
				PageHeader();
				g_iPageCrawl = FixLength(PrintLength(enumPageSections.cnHeader));
				Header();
			}
				//#### Else we're not in .Printable mode, so just render the .PageHeader and .Header
			else {
				PageHeader();
				Header();
			}
		}

		///############################################################
		/// <summary>
		/// Outputs the detail and missing record sections (as required) of the rendered page.
		/// </summary>
		/// <param name="oCurrentTable">PaginationTable object representing the relevant record IDs for the current table index.</param>
		///############################################################
		/// <LastUpdated>November 13, 2009</LastUpdated>
		protected override void DoRenderBody(Pagination.PaginationTable oCurrentTable) {
				//#### .Render the returned object from the .Body call (which will be either a .cnList or .cnForm)
			Body(oCurrentTable.TableName, oCurrentTable.IDColumn).Render(oCurrentTable);
		}

		///############################################################
		/// <summary>
		/// Outputs the footer and detail footer sections of the rendered page.
		/// </summary>
		///############################################################
		/// <LastUpdated>May 22, 2007</LastUpdated>
		protected override void DoRenderFooters() {
				//#### If we're in .Printable mode, .Calculate(the)PageCrawl for the .Footer
			if (Printable) {
					//#### Calculate the g_iPageCrawl while rendering the .PageHeader and .Header
					//####     NOTE: By definition, the .PageFooter and .Footer will fit on a single page (or is otherwise handeled by the developer), hence the inline calculation and the use of .FixLength
				CalculatePageCrawl(PrintLength(enumPageSections.cnFooter));
				Footer();
				g_iPageCrawl = FixLength(PrintLength(enumPageSections.cnFooter));
				PageFooter();
			}
				//#### Else we're not in .Printable mode, so just render the .Footer and .PageFooter
			else {
				Footer();
				PageFooter();
			}
		}


		//##########################################################################################
		//# Page Section-related Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Outputs the page header section of the rendered page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own page section.
		/// <para/>This function is called at the top of every rendered page (once for a non-printable page, and at the top of each printable page). This function is called for every page render.
		/// </remarks>
		///############################################################
		/// <LastUpdated>May 19, 2004</LastUpdated>
		public virtual void PageHeader() {}

		///############################################################
		/// <summary>
		/// Returns a reference to the next section of the report.
		/// </summary>
		/// <remarks>
		/// This function is called once per collected <c>PaginationTable</c>. This function is not called if there are no collected <c>PaginationTable</c>s to render.
		/// <para/>NOTE: This page section should not output any code to the end user, as there is no associated <c>PrintLength</c> calculation for the <c>Body</c>.
		/// </remarks>
		/// <returns><c>List</c> or <c>Form</c> object that represents the next section of the <c>Report</c>.</returns>
		///############################################################
		/// <LastUpdated>March 18, 2010</LastUpdated>
		public abstract List Body(string sTableName, string sIDColumn);

		///############################################################
		/// <summary>
		/// Outputs the page footer section of the rendered page.
		/// </summary>
		/// <remarks>
		/// Optionally overload this function to define your own page section.
		/// <para/>This function is called at the bottom of every rendered page after the current <c>List.DetailFooter</c> or <c>Form.DetailFooter</c> (once for a non-printable page, or at the bottom of each printable page). This function is called for every page render.
		/// </remarks>
		///############################################################
		/// <LastUpdated>May 19, 2004</LastUpdated>
		public virtual void PageFooter() {}

	} //# class Report

} //# namespace Cn.Web.Renderer
