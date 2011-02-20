/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;                                       //# Required to access the Date/Decimal/Double/Int32 datatypes
using System.Web;                                   //# Required to access Request, Response, Application, etc.
using System.Collections;					        //# Required to access the Hashtable class
using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Data;										//# Required to access MetaData, Pagination, Picklists, etc.
using Cn.Data.SQL;									//# Required to access the OrderByClause class
using Cn.Configuration;								//# Required to access the Internationalization class


//namespace Cn.Web.Renderer {
//
//    ///########################################################################################################################
//    /// <summary>
//    /// <c>SearchForm</c> abstract class.
//    /// </summary>
//    /// <remarks>
//    /// This class represents an example of a heavly extended <c>Renderer.Form</c>. This class demonstrates how you could implement your own abstract <c>Renderer</c> classes to accomplish system specific tasks with a minimum of code and a maximum of reuse.
//    /// </remarks>
//    ///########################################################################################################################
//    /// <LastFullCodeReview>September 2, 2005</LastFullCodeReview>
//    public abstract class SearchForm : Form {
//        #region SearchForm
//            //#### Declare the required private variables
//        private SearchInputCollection g_oInputs;
//        private CookieMonster g_rCookieMonster;
//        private string g_sIDColumn;
//        private string g_sOrderBy;
//        private int g_iMaxResults;
//        private bool g_bFullyQualifyColumnNames;
//        private bool g_bInputOrderingVerified;
//        private bool g_bSearchAll;

//            //#### Declare the required public eNums
//            //####     NOTE: The values for enumBooleanTypes and enumComparisonTypes refer to g_cData entries within their asso. system picklists
//        #region eNums
//            /// <summary>CookieMonster search criteria value entry types</summary>
//        public enum enumCookieMonsterEntryTypes : int {
//                /// <summary>Boolean search criteria value.</summary>
//            cnBooleanEntry = 0,
//                /// <summary>Comparison type search criteria value.</summary>
//            cnComparisonTypeEntry = 1,
//                /// <summary>Search term search criteria value.</summary>
//            cnValueEntry = 2,
//                /// <summary>Include null-strings/nulls search criteria value.</summary>
//            cnIncludeNullsEntry = 3
//        }
//            /// <summary><c>RendererSearchForm</c> input types.</summary>
//        public enum enumSearchInputTypes : int {
//                /// <summary>Boolean select input.</summary>
//            cnBooleanInput = 0,
//                /// <summary>Column's related comparison select input.</summary>
//            cnComparisonTypeInput = 1,
//                /// <summary>Column's related include nulls hidden input. This input implicitly includes nulls in the search results.</summary>
//            cnIncludeNullsHiddenInput = 2,
//                /// <summary>Column's related include nulls checkbox input. This allows the end user to determine if nulls are to be included in the search results.</summary>
//            cnIncludeNullsCheckboxInput = 3
//        }
//            /// <summary><c>RendererSearchForm</c> comparison operator types.</summary>
//        public enum enumComparisonTypes : int {
//                /// <summary>Default comparison for this data type.</summary>
//            cnDefaultComparisonType = 0,
//                /// <summary>Table's data is greater then the provided value.</summary>
//            cnGeneral_GreaterThen = 100,
//                /// <summary>Table's data is less then the provided value.</summary>
//            cnGeneral_LessThen = 101,
//                /// <summary>Table's data is equal to the provided value.</summary>
//            cnGeneral_Equals = 102,
//                /// <summary>Table's data is not equal to the provided value.</summary>
//            cnGeneral_NotEqual = 103,
//                /// <summary>Table's data is null (unknown).</summary>
//            cnGeneral_IsNullStringIsNull = 104,
//                /// <summary>Table's character data begins with the provided value.</summary>
//            cnChar_Begins = 200,
//                /// <summary>Table's character data ends with the provided value.</summary>
//            cnChar_Ends = 201,
//                /// <summary>Table's character data contains the provided value.</summary>
//            cnChar_Contains = 202,
//                /// <summary>Table's character data does not contain the provided value.</summary>
//            cnChar_DoesNotContain = 203,
//                /// <summary>Table's character data is equal to the provided value.</summary>
//            cnChar_Equals = 204,
//                /// <summary>Table's character data contains the provided wildcarded value.</summary>
//            cnChar_Wildcards = 205,
//                /// <summary>Table's character data is not equal to the provided value.</summary>
//            cnChar_NotEqual = 206,
//                /// <summary>Table's managed multi-value picklist data contains all of the provided values.</summary>
//            cnMultiPicklist_ContainsAll = 300,
//                /// <summary>Table's managed multi-value picklist data contains any of the provided values.</summary>
//            cnMultiPicklist_ContainsAny = 301,
//                /// <summary>Table's managed multi-value picklist data contains none of the provided values.</summary>
//            cnMultiPicklist_ContainsNone = 302
//        }
//            /// <summary><c>RendererSearchForm</c> boolean operator types.</summary>
//        public enum enumBooleanTypes : int {
//                /// <summary>Default boolean type (equal to <c>cnOrType</c>).</summary>
//            cnDefaultBoolean = 0,
//                /// <summary>Boolean AND operator.</summary>
//            cnAndType = 100,
//                /// <summary>Boolean OR operator.</summary>
//            cnOrType = 101,
//                /// <summary>Boolean AND NOT operator.</summary>
//            cnAndNotType = 102
//        }
//            /// <summary><c>Form</c> and <c>RendererSearchForm</c> input types.</summary>
//        public enum enumInputTypes : int {
//                /// <summary>Standard <c>Form</c> input.</summary>
//            cnFormInput = 0,
//                /// <summary><c>RendererSearchForm</c> custom input.</summary>
//            cnCustomInput = 1,
//                /// <summary>Either a standard <c>Form</c> input or a custom <c>RendererSearchForm</c> input.</summary>
//            cnFormOrCustomInput = 2
//        }
//            /// <summary>Extended input value types.</summary>
//            /// <seealso cref="Cn.Web.Inputs.enumValueType"/>
//        public enum enumSearchFormDataTypes : int {
//                /// <summary>Extended metadata type is unused.</summary>
//            cnNone = Web.Inputs.enumValueTypes.cnSingleValue,

//                /// <summary>Non-searchable extended metadata type.<para/>Value is not included in automaticially generated search form SQL by <c>RendererSearchForm</c>.</summary>
//            cnNonSearchable = 1024,
//                /// <summary>Allows <c>RendererSearchForm</c> to search for a single value within a multi-value common delimited column.</summary>
//                /// <remarks>This extended type defaults to a <c>cnSingleValuePicklist</c> when the input is rendered with <c>RenderInput</c>.</remarks>
//            cnSingleValueSearchInMultiValuePicklist = Web.Inputs.enumValueTypes.cnSingleValueFromPicklist | Web.Inputs.enumValueTypes.cnMultiValuesFromPicklist,
//                /// <summary>Allows <c>RendererSearchForm</c> to search for multiple values within a single value column.</summary>
//                /// <remarks>This extended type defaults to a <c>cnMultiValuePicklist</c> when the input is rendered with <c>RenderInput</c>.</remarks>
//            cnMultiValueSearchInSingleValuePicklist = Web.Inputs.enumValueTypes.cnMultiValuesFromPicklist | 2048
//        }
//        #endregion

//            //#### Declare the required private, developer modifiable constants
//            //####     Wildcards: Defines the wildcard characters for single and multiple characters.
//        private const string g_cWildcards_SingleChararacter = "?";
//        private const string g_cWildcards_Chararacters = "*";

//            //#### Declare the required private constants
//        private const string g_cClassName = "Cn.Web.Renderer.RendererSearchForm.";


//        //##########################################################################################
//        //# Class Functions
//        //##########################################################################################
//        ///############################################################
//        /// <summary>
//        /// Initializes the class.
//        /// </summary>
//        /// <remarks>
//        /// NOTE: The passed <paramref>sInputOrdering</paramref> is ignored if we are not currently processing the Form. You are required to use <c>SetInputOrder</c> to define the input ordering on a rendered search form.
//        /// </remarks>
//        /// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
//        /// <param name="sInputOrdering">String representing the <c>PrimaryDelimiter</c> delimited string of input aliases in the order as they appear on the rendered Form.</param>
//        ///############################################################
//        /// <LastUpdated>May 21, 2007</LastUpdated>
//        public SearchForm(Web.Settings.Current oSettings, string sInputOrdering) : base(oSettings) {
//                //#### Set .NewRecordCount to 1 (as a search form, by definition consists of only 1 "new record" form)
//            NewRecordCount = 1;

//                //#### Call .Reset to init the class vars
//            Reset(oSettings, sInputOrdering);
//        }

//        ///############################################################
//        /// <summary>
//        /// Resets the class to its initilized state.
//        /// </summary>
//        ///############################################################
//        /// <LastUpdated>August 22, 2007</LastUpdated>
//        public void Reset(Web.Settings.Current oSettings, string sInputOrdering) {
//                //#### (Re)Init the global private variables
//            g_oInputs.Reset(oSettings, this, sInputOrdering);
//            g_rCookieMonster = null;
//            g_sIDColumn = "";
//            g_sOrderBy = "";
//            g_iMaxResults = 0;
//            g_bFullyQualifyColumnNames = true;
//            g_bInputOrderingVerified = false;
//            g_bSearchAll = false;
//        }


//        //##########################################################################################
//        //# Public Properties
//        //##########################################################################################
//        ///############################################################
//        /// <summary>
//        /// Gets the input's tools and management class related to this instance.
//        /// </summary>
//        /// <returns>SearchInputCollection object representing the input collection class related to this instance.</returns>
//        ///############################################################
//        /// <LastUpdated>August 21, 2007</LastUpdated>
//        new public SearchInputCollection Inputs {
//            get { return g_oInputs; }
//        }

//        ///############################################################
//        /// <summary>
//        /// Gets/sets the CookieMonster class related to this instance.
//        /// </summary>
//        /// <value>CookieMonster object that represents the instance's CookieMonster class.</value>
//        ///############################################################
//        /// <LastUpdated>September 1, 2005</LastUpdated>
//        public Cn.Web.CookieMonster CookieMonster {
//            get { return g_rCookieMonster; }
//            set { g_rCookieMonster = value; }
//        }

//        ///############################################################
//        /// <summary>
//        /// Gets/sets a value representing the ID column to retrieve that identifies the search results.
//        /// </summary>
//        /// <value>String representing the ID column to retrieve that identifies the search results.</value>
//        /// <exception cref="Cn.CnException">Thrown when the provided <paramref>value</paramref> contains potentionally malicious SQL directives (as defined within Internationalization's "DisallowedUserStrings" and "DisallowedUserWords" picklists).</exception>
//        ///############################################################
//        /// <LastUpdated>September 1, 2005</LastUpdated>
//        public string IDColumn {
//            get { return g_sIDColumn; }
//            set {
//                    //#### If the data within value is safe, set g_sOrderBy
//                if (Statements.IsUserDataSafe(value)) {
//                    g_sIDColumn = value;
//                }
//                    //#### Else malicious SQL code was detected by IsUserDataSafe, so raise the error
//                else {
//                    Internationalization.RaiseDefaultError(g_cClassName + "IDColumn", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "IDColumn", "");
//                }
//            }
//        }

//        ///############################################################
//        /// <summary>
//        /// Gets/sets a value representing the search results SQL order by clause.
//        /// </summary>
//        /// <value>String representing the search results SQL order by clause.</value>
//        /// <exception cref="Cn.CnException">Thrown when the provided <paramref>value</paramref> contains potentionally malicious SQL directives (as defined within Internationalization's "DisallowedUserStrings" and "DisallowedUserWords" picklists).</exception>
//        ///############################################################
//        /// <LastUpdated>September 1, 2005</LastUpdated>
//        public string OrderBy {
//            get { return g_sOrderBy; }
//            set {
//                    //#### If the data within value is safe, set g_sOrderBy
//                if (Statements.IsUserDataSafe(value)) {
//                    g_sOrderBy = value;
//                }
//                    //#### Else malicious SQL code was detected by IsUserDataSafe, so raise the error
//                else {
//                    Internationalization.RaiseDefaultError(g_cClassName + "OrderBy", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "OrderBy", "");
//                }
//            }
//        }

//        ///############################################################
//        /// <summary>
//        /// Gets/sets a value representing the maximum number of search results to return.
//        /// </summary>
//        /// <remarks>
//        /// The default value of 0 indicates that all results will be returned from the query.
//        /// </remarks>
//        /// <value>Integer representing the maximum number of search results to return.</value>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>value</paramref> is less then 0.</exception>
//        ///############################################################
//        /// <LastUpdated>September 1, 2004</LastUpdated>
//        public int MaxResults {
//            get { return g_iMaxResults; }
//            set {
//                    //#### If a positive number was passed, set it into g_iMaxResults
//                if (value > -1) {
//                    g_iMaxResults = value;
//                }
//                    //#### Else a negetive number was passed, so raise the error
//                else {
//                    Internationalization.RaiseDefaultError(g_cClassName + "MaxResults", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_MaxResults, "", "");
//                }
//            }
//        }

//        ///############################################################
//        /// <summary>
//        /// Gets/sets a value indicating if we are to fully qualify the column names within the generated SQL statements.
//        /// </summary>
//        /// <value>Boolean value indicating if we are to fully qualify the column names within the generated SQL statements.</value>
//        ///############################################################
//        /// <LastUpdated>February 9, 2006</LastUpdated>
//        public bool FullyQualifyColumnNames {
//            get { return g_bFullyQualifyColumnNames; }
//            set { g_bFullyQualifyColumnNames = value; }
//        }

//        ///############################################################
//        /// <summary>
//        /// Get/sets a value indicating if we are to search all of the columns.
//        /// </summary>
//        /// <remarks>
//        /// This functionality utilizes the submitted form from a <c>Renderer.RenderInput</c> quicksearch form.
//        /// </remarks>
//        /// <value>Boolean value indicating if we are to search all of the columns.</value>
//        ///############################################################
//        /// <LastUpdated>June 29, 2004</LastUpdated>
//        public bool SearchAll {
//            get { return g_bSearchAll; }
//            set { g_bSearchAll = value; }
//        }


//        //##########################################################################################
//        //# Public Read-Only Properties
//        //##########################################################################################
//        ///############################################################
//        /// <summary>
//        /// Gets a value representing the search all columns quicksearch text.
//        /// </summary>
//        /// <remarks>
//        /// This value is collected from the submitted form from a <c>Renderer.RenderInput</c> quicksearch form.
//        /// </remarks>
//        /// <value>String representing the search all columns quicksearch text.</value>
//        ///############################################################
//        /// <LastUpdated>June 30, 2004</LastUpdated>
//        public string SearchAllText {
//            get {
//                return Request.Form[Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + "rfQuickSearch"];
//            }
//        }


//        //##########################################################################################
//        //# Page Section-related Functions
//        //##########################################################################################
//        ///############################################################
//        /// <summary>
//        /// Collects/reorders the entire results set.
//        /// </summary>
//        /// <remarks>
//        /// Due to the nature of a search form, it is always represented as a single "new" form. Because of this, there are no results to collect and therefore no need to implement this function.
//        /// </remarks>
//        /// <param name="rResults">Pagination object representing entire result set's record IDs.</param>
//        /// <param name="bReorderExistingResults">Boolean value indicating if the entire results set requires re-ordering.</param>
//        ///############################################################
//        /// <LastUpdated>June 23, 2004</LastUpdated>
//        public override void GenerateResults(Cn.Data.Pagination rResults, bool bReorderExistingResults) {}

//        ///############################################################
//        /// <summary>
//        /// Collects the provided page results.
//        /// </summary>
//        /// <remarks>
//        /// Due to the nature of a search form, it is always represented as a single "new" form. Because of this, there are no results to collect and therefore no need to implement this function.
//        /// </remarks>
//        /// <param name="rPageResults">Pagination object representing this page's relevant record IDs.</param>
//        ///############################################################
//        /// <LastUpdated>June 23, 2004</LastUpdated>
//        public override void CollectPageResults(Cn.Data.Pagination rPageResults) {}

//        ///############################################################
//        /// <summary>
//        /// Validates the current record during form processing.
//        /// </summary>
//        /// <remarks>
//        /// This implementation is a good example of a heavly modified <c>ValidateRecord</c> that returns its own SQL (as opposed to letting Renderer generate it).
//        /// </remarks>
//        /// <param name="bRecordHasChanged">Boolean value indicating if the record was changed/updated by the end user.</param>
//        /// <param name="bRecordIsValid">Boolean value indicating if the record successfully passed the simple validation (datatype, length, etc.).</param>
//        /// <returns>RecordValidater object that represents the records validity, if SQL statements are to be generated and any developer generated SQL statements.</returns>
//        /// <exception cref="Cn.CnException">Thrown when the global <c>IDColumn</c> is a null-string.</exception>
//        /// <exception cref="Cn.CnException">Thrown when no ordered input aliases have been defined.</exception>
//        /// <exception cref="Cn.CnException">Thrown when no searchable input aliases have been defined.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> has not be defined.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sInputOrdering</paramref> does not contain the correct number of elements.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sInputOrdering</paramref> does not contain a defined searchable input alias.</exception>
//        ///############################################################
//        /// <LastUpdated>August 22, 2007</LastUpdated>
//        public override RecordValidater ValidateRecord(bool bRecordHasChanged, bool bRecordIsValid) {
//            SearchInputCollection.CustomInputData oCustomFormInput;
//            Form.RecordValidater oReturn = null;
//            string[] a_sInputAliases;
//            string sCurrentWhereClause;
//            string sCurrentInputAlias;
//            string sOrderByClause = "";
//            string sWhereClause = "";
//            string sSQL;
//            int i;

//                //#### If the developer has defined a g_sIDColumn
//            if (g_sIDColumn.Length > 0) {
//                    //#### Init and default the oReturn value's properties
//                oReturn = new Form.RecordValidater((bRecordIsValid || SearchAll), false, new string[1]);

//                    //#### If this record .IsValid
//                if (oReturn.IsValid) {
//                        //#### Collect the a_sInputAliases
//                    a_sInputAliases = g_oInputs.GetInputOrdering();

//                        //#### If the caller specified that all results should be returned, set the non-limiting select statement into sSQL
////! eDbServerType?
//                    if (MaxResults == 0) {
//                        sSQL = "SELECT " + g_sIDColumn + " AS ResultID FROM " + GetSearchableTables("ValidateRecord") + " ";
//                    }
//                        //#### Else the caller has limited the number of results, so set the range limiting insert statement into sSQL
//                    else {
//                        sSQL = "SELECT TOP " + MaxResults + " " + g_sIDColumn + " AS ResultID FROM " + GetSearchableTables("ValidateRecord") + " ";
//                    }

//                        //#### If the developer has set an g_sOrderBy, set it into the local sOrderByClause
//                    if (g_sOrderBy.Length > 0) {
//                        sOrderByClause = " ORDER BY " + g_sOrderBy;
//                    }

//                        //#### If we are supposed to SearchAll
//                    if (SearchAll) {
//                            //#### Traverse the a_sInputAliases from back to front (which allows us to have a 'foward looking loop' to see if each _Boolean clause is necessary)
//                        for (i = (a_sInputAliases.Length - 1); i >= 0; i--) {
//                                //#### Collect the sCurrentInputAlias and oCustomFormInput for this loop
//                            sCurrentInputAlias = a_sInputAliases[i];
//                            oCustomFormInput = g_oInputs.GetCustom(sCurrentInputAlias);

//                                //#### If we successfully collected the oCustomFormInput above
//                            if (oCustomFormInput != null) {
//                                    //#### Get(the)CustomInputIdentifier (which will be inserted into the sWhereClause below in place of a real clause), passing in our best guess at the current SQL (which is good enough for the purpose as worse case we'll get " WHERE " in there when it shouldn't be, which has zero effect on the .GetCustomInputIdentifier call)
//                                    //####     NOTE: It's up to the developer to replace the .Identifier with a real clause via .InsertCustomInputClause
//                                sCurrentWhereClause = GetCustomInputIdentifier(sSQL + " WHERE " + sWhereClause + sOrderByClause, sCurrentInputAlias);
//                                oCustomFormInput.Identifer = sCurrentWhereClause;
//                            }
//                                //#### Else this must be a .Form .Input
//                            else {
//                                    //#### Pass the call off to DoFormatWhereClause, collecting its return into the sCurrentWhereClause
//                                sCurrentWhereClause = DoFormatWhereClause("ValidateRecord", Inputs.Get(sCurrentInputAlias), enumComparisonTypes.cnDefaultComparisonType, g_bFullyQualifyColumnNames);
//                            }

//                                //#### If a sCurrentWhereClause was returned above
//                            if (sCurrentWhereClause.Length > 0) {
//                                    //#### If this is not the first sWhereClause
//                                if (sWhereClause.Length > 0) {
//                                        //#### Prepend the sCurrentWhereClause and the .DefaultBoolean statement onto sWhereClause
//                                    sWhereClause = sCurrentWhereClause + FormatBooleanClause(enumBooleanTypes.cnDefaultBoolean) + sWhereClause;
//                                }
//                                    //#### Else this is the first clause, so set sWhereClause to the sCurrentWhereClause
//                                else {
//                                    sWhereClause = sCurrentWhereClause;
//                                }
//                            }
//                        }
//                    }
//                        //#### Else we have values for each criteria
//                    else {
//                        string sCurrentInputName;

//                            //#### If the input ordering hasn't already been verified
//                        if (! g_bInputOrderingVerified) {
//                                //#### If developer has specified that we need to .Verify(the)InputOrdering
//                            if (g_oInputs.VerifyInputOrdering) {
//                                    //#### Reset g_bInputOrderingVerified based on DoVerifyInputOrdering's return value
//                                    //####     NOTE: This function raises the error if the developer didn't define every input in the input ordering
//                                g_bInputOrderingVerified = DoVerifyInputOrdering("ValidateRecord", a_sInputAliases);
//                            }
//                                //#### Else the developer has explicitly said to not g_bVerify(the)InputOrdering, so set g_bInputOrderingVerified to true
//                            else {
//                                g_bInputOrderingVerified = true;
//                            }
//                        }

//                            //#### If the g_bInputOrdering(was properly)Verified above
//                        if (g_bInputOrderingVerified) {
//                                //#### Traverse the a_sInputAliases from back to front (which allows us to have a 'foward looking loop' to see if each _Boolean clause is necessary)
//                            for (i = (a_sInputAliases.Length - 1); i >= 0; i--) {
//                                    //#### Collect the sCurrentInputAlias, oCustomFormInput, sCurrentInputName and reset the value of sCurrentWhereClause for this loop
//                                sCurrentInputAlias = a_sInputAliases[i];
//                                oCustomFormInput = g_oInputs.GetCustom(sCurrentInputAlias);
//                                sCurrentInputName = Inputs.InputID(sCurrentInputAlias);
//                                sCurrentWhereClause = "";

//                                    //#### If we successfully collected the oCustomFormInput above
//                                if (oCustomFormInput != null) {
//                                        //#### If a value was set by the user for the sCurrentInputName on the form
//                                    if (Request.Form[sCurrentInputName] != null && Request.Form[sCurrentInputName].Length > 0) {
//                                            //#### Get(the)CustomInputIdentifier (which will be inserted into the sWhereClause below in place of a real clause), passing in our best guess at the current SQL (which is good enough for the purpose as worse case we'll get " WHERE " in there when it shouldn't be, which has zero effect on the .GetCustomInputIdentifier call)
//                                            //####     NOTE: It's up to the developer to replace the .Identifier with a real clause via .InsertCustomInputClause
//                                        sCurrentWhereClause = GetCustomInputIdentifier(sSQL + " WHERE " + sWhereClause + sOrderByClause, sCurrentInputAlias);
//                                        oCustomFormInput.Identifer = sCurrentWhereClause;
//                                    }
//                                }
//                                    //#### Else this must be a .Form .Input
//                                else {
//                                        //#### Pass the call off to DoFormatWhereClause, collecting its return into the sCurrentWhereClause (collecting the InputData and ComparisonType as we go)
//                                    sCurrentWhereClause = DoFormatWhereClause(
//                                        "ValidateRecord",
//                                        Inputs.Get(sCurrentInputAlias),
//                                        MakeComparisonType(Request.Form[Inputs.InputID(sCurrentInputAlias) + "_Comparison"]),
//                                        g_bFullyQualifyColumnNames
//                                    );
									
//                                }

//                                    //#### If a sCurrentWhereClause was returned above
//                                if (sCurrentWhereClause.Length > 0) {
//                                        //#### If this is not the first sWhereClause
//                                    if (sWhereClause.Length > 0) {
//                                            //#### Prepend the current clause and the _Boolean statement onto sWhereClause
//                                        sWhereClause = sCurrentWhereClause + FormatBooleanClause(MakeBooleanType(Request.Form[sCurrentInputName + "_Boolean"])) + sWhereClause;
//                                    }
//                                        //#### Else this is the first clause, so set sWhereClause to the sCurrentWhereClause
//                                    else {
//                                        sWhereClause = sCurrentWhereClause;
//                                    }
//                                }
//                            }
//                        }
//                    }

//                        //#### If the user defined a sWhereClause, prepend the WHERE keyword
//                    if (sWhereClause.Length > 0) {
//                        sWhereClause = " WHERE " + sWhereClause;
//                    }

//                        //#### Congeal the above constructed sSQL, sWhereClause and sOrderByClause into our return value's .SQLStatement
//                    oReturn.SQLStatements[0] = sSQL + sWhereClause + sOrderByClause;
//                }
//            }
//                //#### Else no g_sIDColumn was defined, so raise the error
//            else {
//                Internationalization.RaiseDefaultError(g_cClassName + "ValidateRecord", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_MissingIDColumn, "", "");
//            }

//                //#### Return the above determined oReturn value to the caller
//            return oReturn;
//        }


//        //##########################################################################################
//        //# Public Functions
//        //##########################################################################################
//        ///############################################################
//        /// <summary>
//        /// Sets all of the search criteria values for the current search form within the <c>CookieMonster</c>.
//        /// </summary>
//        /// <param name="sKeyPrefix">String representing the <c>CookieMonster</c> key prefix for the current search form.</param>
//        /// <exception cref="Cn.CnException">Thrown when the global <c>CookieMonster</c> object is null.</exception>
//        /// <exception cref="Cn.CnException">Thrown when no ordered input aliases have been defined.</exception>
//        ///############################################################
//        /// <LastUpdated>April 5, 2006</LastUpdated>
//        public void SetCookieMonsterValues(string sKeyPrefix) {
//            string[] a_sKeys = g_oInputs.InputAliases;
//            string[] a_sValues;
//            string sCurrentInputName;
//            string sValue = "";
//            int i;

//                //#### If the g_rCookieMonster has not been set, raise the error
//            if (g_rCookieMonster == null) {
//                Internationalization.RaiseDefaultError(g_cClassName + "SetCookieMonsterValues", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_NullCookieMonster, "SetCookieMonsterValues", "");
//            }
//                //#### Else if a_sKeys is null or not containing any data, raise the error
//            else if (a_sKeys == null || a_sKeys.Length == 0) {
//                Internationalization.RaiseDefaultError(g_cClassName + "SetCookieMonsterValues", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_NoOrderedInputAliases, "", "");
//            }
//                //#### Else we have a g_rCookieMonster to work with
//            else {
//                    //#### Traverse the a_sKeys collected above
//                for (i = 0; i < a_sKeys.Length; i++) {
//                        //#### Set the sCurrentInputName and collect its ga_sValues for this loop (always assuming a single search form, hence 1)
//                        //####     NOTE: We always assume a single form as by definition a search form is a single "new" form
//                    sCurrentInputName = Inputs.InputID(a_sKeys[i], 1);
//                    a_sValues = Request.Form.GetValues(sCurrentInputName);
//                    sValue = "";

//                        //#### If ga_sValues were found above for the sCurrentInputName
//                    if (a_sValues != null) {
//                            //#### If there is only one a_sValue, set sValue to the 0th index
//                        if (a_sValues.Length == 1) {
//                            sValue = a_sValues[0];
//                        }
//                            //#### Else we have more then a single a_sValue, so set the sValue to a properly formatted .MultiValueString
//                        else {
//                            sValue = Data.Tools.MultiValueString(a_sValues);
//                        }
//                    }

//                        //#### If the _Comparison type is .cnGeneral_IsNullStringIsNull
//                    if (MakeComparisonType(Request.Form[sCurrentInputName + "_Comparison"]) == enumComparisonTypes.cnGeneral_IsNullStringIsNull) {
//                            //#### Prepend the passed sKeyPrefix and save the field names/values into the CookieMonster (with the notable exception of the sValue as the user is searching for .cnGeneral_IsNullStringIsNull)
//                        g_rCookieMonster.Value(sKeyPrefix + a_sKeys[i] + "_Boolean", Request.Form[sCurrentInputName + "_Boolean"]);
//                        g_rCookieMonster.Value(sKeyPrefix + a_sKeys[i] + "_Comparison", Request.Form[sCurrentInputName + "_Comparison"]);
//                        g_rCookieMonster.Value(sKeyPrefix + a_sKeys[i] + "_IncludeNulls", Request.Form[sCurrentInputName + "_IncludeNulls"]);
//                        g_rCookieMonster.Value(sKeyPrefix + a_sKeys[i] + "_Value", "");
//                    }
//                        //#### Else if a sValue was set above
//                    else if (sValue.Length > 0) {
//                            //#### Prepend the passed sKeyPrefix and save the field names/values into the CookieMonster
//                        g_rCookieMonster.Value(sKeyPrefix + a_sKeys[i] + "_Boolean", Request.Form[sCurrentInputName + "_Boolean"]);
//                        g_rCookieMonster.Value(sKeyPrefix + a_sKeys[i] + "_Comparison", Request.Form[sCurrentInputName + "_Comparison"]);
//                        g_rCookieMonster.Value(sKeyPrefix + a_sKeys[i] + "_IncludeNulls", Request.Form[sCurrentInputName + "_IncludeNulls"]);
//                        g_rCookieMonster.Value(sKeyPrefix + a_sKeys[i] + "_Value", sValue);
//                    }
//                        //#### Else remove any currently set values for the sCurrentInputName
//                    else {
//                        g_rCookieMonster.RemoveKey(sKeyPrefix + a_sKeys[i] + "_Boolean");
//                        g_rCookieMonster.RemoveKey(sKeyPrefix + a_sKeys[i] + "_Comparison");
//                        g_rCookieMonster.RemoveKey(sKeyPrefix + a_sKeys[i] + "_IncludeNulls");
//                        g_rCookieMonster.RemoveKey(sKeyPrefix + a_sKeys[i] + "_Value");
//                    }
//                }
//            }
//        }

//        ///############################################################
//        /// <summary>
//        /// Removes all of the search criteria values for the current search form within the <c>CookieMonster</c>.
//        /// </summary>
//        /// <param name="sKeyPrefix">String representing the <c>CookieMonster</c> key prefix for the current search form.</param>
//        /// <exception cref="Cn.CnException">Thrown when the global <c>CookieMonster</c> object is null.</exception>
//        /// <exception cref="Cn.CnException">Thrown when no ordered input aliases have been defined.</exception>
//        ///############################################################
//        /// <LastUpdated>April 5, 2006</LastUpdated>
//        public void ResetCookieMonsterValues(string sKeyPrefix) {
//            string[] a_sKeys = g_oInputs.InputAliases;
//            string sCurrentInputName;
//            int i;

//                //#### If the g_rCookieMonster has not been set, raise the error
//            if (g_rCookieMonster == null) {
//                Internationalization.RaiseDefaultError(g_cClassName + "ResetCookieMonsterValues", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_NullCookieMonster, "ResetCookieMonsterValues", "");
//            }
//                //#### Else if a_sKeys is null or not containing any data, raise the error
//            else if (a_sKeys == null || a_sKeys.Length == 0) {
//                Internationalization.RaiseDefaultError(g_cClassName + "ResetCookieMonsterValues", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_NoOrderedInputAliases, "", "");
//            }
//                //#### Else we have a g_rCookieMonster to work with
//            else {
//                    //#### Traverse the a_sKeys collected above
//                for (i = 0; i < a_sKeys.Length; i++) {
//                        //#### Set the sCurrentInputName (always assuming a single search form, hence 1)
//                        //####     NOTE: We always assume a single form as by definition a search form is a single "new" form
//                    sCurrentInputName = Inputs.InputID(a_sKeys[i], 1);

//                        //#### Remove any currently set values for the sCurrentInputName
//                    g_rCookieMonster.RemoveKey(sKeyPrefix + a_sKeys[i] + "_Boolean");
//                    g_rCookieMonster.RemoveKey(sKeyPrefix + a_sKeys[i] + "_Comparison");
//                    g_rCookieMonster.RemoveKey(sKeyPrefix + a_sKeys[i] + "_IncludeNulls");
//                    g_rCookieMonster.RemoveKey(sKeyPrefix + a_sKeys[i] + "_Value");
//                }
//            }
//        }

//        ///############################################################
//        /// <summary>
//        /// Retrieves the referenced search criteria value for the current search form within the <c>CookieMonster</c>.
//        /// </summary>
//        /// <param name="sKeyPrefix">String representing the <c>CookieMonster</c> key prefix for the current search form.</param>
//        /// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
//        /// <param name="eEntryType">Enumeration representing the search criteria value to return.</param>
//        /// <returns>String representing the search criteria value for the passed <paramref>sInputAlias</paramref>/<paramref>eEntryType</paramref> pair.</returns>
//        /// <exception cref="Cn.CnException">Thrown when the global <c>CookieMonster</c> object is null.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eEntryType</paramref> is unreconized.</exception>
//        ///############################################################
//        /// <LastUpdated>April 5, 2006</LastUpdated>
//        public string GetCookieMonsterValue(string sKeyPrefix, string sInputAlias, enumCookieMonsterEntryTypes eEntryType) {
//            string sReturn = "";

//                //#### If the g_rCookieMonster has not been set, raise the error
//            if (g_rCookieMonster == null) {
//                Internationalization.RaiseDefaultError(g_cClassName + "GetCookieMonsterValue", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_NullCookieMonster, "GetCookieMonsterValue", "");
//            }
//                //#### Else we have a g_rCookieMonster to work with
//            else {
//                    //#### Determine the eEntryTypes and set the sReturn value accordingly (always assuming a single search form, hence 1)
//                    //####     NOTE: We always assume a single form as by definition a search form is a single "new" form
//                switch (eEntryType) {
//                    case enumCookieMonsterEntryTypes.cnBooleanEntry: {
//                        sReturn = g_rCookieMonster.Value(sKeyPrefix + sInputAlias + "_Boolean");
//                        break;
//                    }
//                    case enumCookieMonsterEntryTypes.cnComparisonTypeEntry: {
//                        sReturn = g_rCookieMonster.Value(sKeyPrefix + sInputAlias + "_Comparison");
//                        break;
//                    }
//                    case enumCookieMonsterEntryTypes.cnIncludeNullsEntry: {
//                        sReturn = g_rCookieMonster.Value(sKeyPrefix + sInputAlias + "_IncludeNulls");
//                        break;
//                    }
//                    case enumCookieMonsterEntryTypes.cnValueEntry: {
//                        sReturn = g_rCookieMonster.Value(sKeyPrefix + sInputAlias + "_Value");
//                        break;
//                    }

//                        //#### Else eEntryTypes is unreconized, so raise the error
//                    default: {
//                        Internationalization.RaiseDefaultError(g_cClassName + "GetCookieMonsterValue", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eEntryType", Cn.Data.Tools.MakeString(eEntryType, ""));
//                        break;
//                    }
//                }
//            }

//                //#### Return the above determined sReturn value to the caller
//            return sReturn;
//        }

//        ///############################################################
//        /// <summary>
//        /// Replaces the provided custom input's identifier with the passed SQL clause.
//        /// </summary>
//        /// <remarks>
//        /// NOTE: The passed <paramref>sSQL</paramref> should represent "a_sSQL[0]" from <c>SubmitResults</c> before any modifications are made to it. In other words, you should call this function for all of your defined custom form inputs before you make any further changes of the SQL statement(s) within a_sSQL.
//        /// <para/>NOTE: Best pratices for this functionality is to call this function in the same order as the <paramref>sInputAlias</paramref>'s appear on the search form.
//        /// <para/>NOTE: Since we only knew that the <paramref>sInputAlias</paramref>'s custom input identifer was unique at the time it was inserted into the where clause, we can only replace the last instance of the custom input identifer in the passed <paramref>sSQL</paramref> (as the where clause is built from back to front)
//        /// </remarks>
//        /// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
//        /// <param name="sSQL">String representing the SQL query containing the custom input identifer.</param>
//        /// <param name="sClauseToInsert">String representing the SQL clause to insert in place of the custom input identifer.</param>
//        /// <returns>String representing the modified SQL query.</returns>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is not defined as a custom input.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref>'s custom input identifier has already been replaced.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref>'s custom input identifier was not found within the passed <paramref>sSQL</paramref>.</exception>
//        ///############################################################
//        /// <LastUpdated>August 22, 2007</LastUpdated>
//        public string InsertCustomInputClause(string sInputAlias, string sSQL, string sClauseToInsert) {
//            SearchInputCollection.CustomInputData oCustomFormInput;
//            string sReturn = "";
//            int iStartingIndex;
//            int iEndingIndex;

//                //#### Collect the passed sInputAlias into a oCustomFormInput
//            oCustomFormInput = g_oInputs.GetCustom(sInputAlias);

//                //#### If the passed sInputAlias was defined as a custom form input
//            if (oCustomFormInput != null) {
//                    //#### If the oCustomFormInput .Has(not yet)BeenReplaced within the sSQL
//                if (! oCustomFormInput.HasBeenReplaced) {
//                        //#### Flip the .HasBeenReplaced flag for the sInputAlias and determine the .Identifer's iStartingIndex
//                    oCustomFormInput.HasBeenReplaced = true;
//                    iStartingIndex = sSQL.LastIndexOf(oCustomFormInput.Identifer);

//                        //#### If the .Identifer was found within the passed sSQL
//                    if (iStartingIndex > -1) {
//                            //#### Calculate the .Identifer's iEndingIndex
//                        iEndingIndex = (iStartingIndex + oCustomFormInput.Identifer.Length - 1);

//                            //#### "Replace" the last occurance of the .Identifer within sSQL with the passed sClauseToInsert, setting the result into the sReturn value
//                        sReturn = sSQL.Substring(0, iStartingIndex) + sClauseToInsert + sSQL.Substring(iEndingIndex + 1);
//                    }
//                        //#### Else the .Identifer was not found within the passed sSQL, so raise the error
//                        //####     NOTE: This error should never be called if the developer has followed the best practices stated above. It would only occure if the developer has removed the .Identifer from the passed sSQL themselves
//                    else {
//                        Internationalization.RaiseDefaultError(g_cClassName + "InsertCustomInputClause", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_InsertCustomInputClauseMissingToken, sInputAlias, "");
//                    }
//                }
//                    //#### Else the sInputAlias .Has(already)BeenReplaced, so raise the error
//                else {
//                    Internationalization.RaiseDefaultError(g_cClassName + "InsertCustomInputClause", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_InsertCustomInputClauseSecondCall, sInputAlias, "");
//                }
//            }
//                //#### Else the passed sInputAlias was not defined as a custom form input, so raise the error
//            else {
//                Internationalization.RaiseDefaultError(g_cClassName + "InsertCustomInputClause", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_UnknownCustomInputAlias, sInputAlias, "");
//            }

//                //#### Return the above determined sReturn value to the caller
//            return sReturn;
//        }

//        ///############################################################
//        /// <summary>
//        /// Translates the current value for the referenced Form input into the equivalent SQL where clause.
//        /// </summary>
//        /// <param name="sInputAlias">String representing the Form input to query.</param>
//        /// <param name="bFullyQualifyColumnNames">Boolean value indicating if the column names are to be fully qualified (i.e. - "Table.Column").</param>
//        /// <returns>String representing formatted where clause.</returns>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is null or a null-string.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> has not be defined.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is of an unknown or unsupported data type.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eComparisonType</paramref> is invalid based on the <paramref>sInputAlias</paramref>'s data type.</exception>
//        ///############################################################
//        /// <LastUpdated>April 4, 2006</LastUpdated>
//        public string FormatWhereClause(string sInputAlias, bool bFullyQualifyColumnNames) {
//            string sReturn = "";

//                //#### If the passed sInputAlias is a null-string, raise the error
//            if (sInputAlias == null || sInputAlias.Length == 0) {
//                Internationalization.RaiseDefaultError(g_cClassName + "FormatWhereClause", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "sInputAlias", "");
//            }
//                //#### Else if the sInputAlias does not .Exist within the .Form's .Inputs, raise the error
//            else if (! Inputs.Exists(sInputAlias)) {
//                Internationalization.RaiseDefaultError(g_cClassName + "FormatWhereClause", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_UnknownNonCustomInputAlias, sInputAlias, "");
//            }
//                //#### Else the passed sInputAlias is valid
//            else {
//                    //#### Pass the call off to DoFormatWhereClause, setting our sReturn value based on its (collecting the InputData and ComparisonType as we go)
//                sReturn = DoFormatWhereClause(
//                    "FormatWhereClause",
//                    Inputs.Get(sInputAlias),
//                    MakeComparisonType(Request.Form[Inputs.InputID(sInputAlias) + "_Comparison"]),
//                    bFullyQualifyColumnNames
//                );
//            }

//                //#### Return the above determined sReturn value to the caller
//            return sReturn;
//        }

//        ///############################################################
//        /// <summary>
//        /// Translates the passed boolean operator type into its related SQL boolean operator.
//        /// </summary>
//        /// <param name="eBooleanType">Enumeration representing the required boolean operator type.</param>
//        /// <returns>String representing the SQL boolean operator related to the passed <paramref>eBooleanType</paramref>.</returns>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eBooleanType</paramref> is unreconized.</exception>
//        ///############################################################
//        /// <LastUpdated>September 1, 2004</LastUpdated>
//        public string FormatBooleanClause(enumBooleanTypes eBooleanType) {
//            string sReturn = "";

//                //#### Determine the eBooleanType and set the return value accordingly
//            switch (eBooleanType) {
//                case enumBooleanTypes.cnAndType: {
//                    sReturn = " AND ";
//                    break;
//                }
//                case enumBooleanTypes.cnOrType:
//                case enumBooleanTypes.cnDefaultBoolean: {
//                    sReturn = " OR ";
//                    break;
//                }
//                case enumBooleanTypes.cnAndNotType: {
//                    sReturn = " AND NOT ";
//                    break;
//                }

//                    //#### Else the eBooleanType is unreconized, so raise the error
//                default: {
//                    Internationalization.RaiseDefaultError(g_cClassName + "FormatBooleanClause", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eBooleanType", Cn.Data.Tools.MakeString(eBooleanType, ""));
//                    break;
//                }
//            }

//                //#### Return the above determined sReturn value to the caller
//            return sReturn;
//        }


//        //##########################################################################################
//        //# Private Functions
//        //##########################################################################################
//        ///############################################################
//        /// <summary>
//        /// Retrieves the tables of all of the searchable and custom <c>Form</c> inputs.
//        /// </summary>
//        /// <param name="sFunction">String representing the calling function's name.</param>
//        /// <returns>String representing a comma delimited list of table names.</returns>
//        /// <exception cref="Cn.CnException">Thrown when no ordered input aliases have been defined.</exception>
//        /// <exception cref="Cn.CnException">Thrown when no searchable input aliases have been defined.</exception>
//        ///############################################################
//        /// <LastUpdated>August 22, 2007</LastUpdated>
//        private string GetSearchableTables(string sFunction) {
//            string[] a_sSearchableInputs = g_oInputs.SearchableInputAliases;
//            string sCurrentTable;
//            string sReturn = ",";
//            int i;

//                //#### Traverse the a_sSearchableInputs
//            for (i = 0; i < a_sSearchableInputs.Length; i++) {
//                    //#### If the current a_sSearchableInput .Exists within the .Form's .Inputs
//                if (Inputs.Exists(a_sSearchableInputs[i])) {
//                        //#### Collect the sCurrentTable (followed by a trailing ,)
//                        //####     NOTE: The .TableName is not lowercased here as some containers my consider case important (followed by a delimiting ",")
//                    sCurrentTable = Inputs.Get(a_sSearchableInputs[i]).TableName + ",";
//                }
//                    //#### Else we know that the current a_sSearchableInput exists within the g_hCustomFormInputs (as we got it successfully from .SearchableInputs above)
//                else { //# if (g_hCustomFormInputs.Contains(sInputAlias)) {
//                        //#### Collect the sCurrentTable (followed by a trailing ,)
//                        //####     NOTE: The .TableName is not lowercased here as some containers my consider case important (followed by a delimiting ",")
//                    sCurrentTable = g_oInputs.GetCustom(a_sSearchableInputs[i]).TableName + ",";
//                }

//                    //#### If the sCurrentTable (delimited by surrounding ","s) is not within the sReturn value
//                if (sReturn.ToLower().IndexOf("," + sCurrentTable.ToLower()) == -1) {
//                        //#### Append the sCurrentTable onto the sReturn value
//                    sReturn += sCurrentTable;
//                }
//            }

//                //#### Borrow the use of i to determine the .Length of the sReturn value
//            i = sReturn.Length;

//                //#### If we successfully determined a sReturn value above
//                //####     NOTE: Since .SearchableInputs raises an error if no .SearchableInputs are found, this check is really not required but is included for best pratices
//            if (i > 1) {
//                    //#### Return the above determined sReturn value to the caller (pealing off the leading/trailing ,s as we go)
//                return sReturn.Substring(1, i - 2);
//            }
//                //#### Else no searchable inputs were found above, so raise the error (and return null so the compiler shuts up)
//            else {
//                Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_NoSearchableInputs, "", "");
//                return sReturn;
//            }
//        }

//        ///############################################################
//        /// <summary>
//        /// Converts the provided value into its enumeration equivalent.
//        /// </summary>
//        /// <param name="sValue">String representing the value to convert.</param>
//        /// <returns>Enumeration representing the <paramref>sValue</paramref>'s equivalent comparison type.</returns>
//        ///############################################################
//        /// <LastUpdated>August 22, 2007</LastUpdated>
//        private enumComparisonTypes MakeComparisonType(string sValue) {
//            int iType = Cn.Data.Tools.MakeInteger(sValue, (int)enumComparisonTypes.cnDefaultComparisonType);
//            enumComparisonTypes eReturn;

//                //#### Try to cast the above determined iType into a enumComparisonTypes
//            try {
//                eReturn = (enumComparisonTypes)iType;
//            }
//                //#### If the cast fails, reset the eReturn value to .cnDefaultComparisonType
//            catch {
//                eReturn = enumComparisonTypes.cnDefaultComparisonType;
//            }

//                //#### Return the above determined eReturn value to the caller
//            return eReturn;
//        }

//        ///############################################################
//        /// <summary>
//        /// Converts the provided value into its enumeration equivalent.
//        /// </summary>
//        /// <param name="sValue">String representing the value to convert.</param>
//        /// <returns>Enumeration representing the <paramref>sValue</paramref>'s equivalent boolean type.</returns>
//        ///############################################################
//        /// <LastUpdated>August 22, 2007</LastUpdated>
//        private enumBooleanTypes MakeBooleanType(string sValue) {
//            int iType = Cn.Data.Tools.MakeInteger(sValue, (int)enumBooleanTypes.cnDefaultBoolean);
//            enumBooleanTypes eReturn;

//                //#### Try to cast the above determined iType into a enumComparisonTypes
//            try {
//                eReturn = (enumBooleanTypes)iType;
//            }
//                //#### If the cast fails, reset the eReturn value to .cnDefaultBoolean
//            catch {
//                eReturn = enumBooleanTypes.cnDefaultBoolean;
//            }

//                //#### Return the above determined eReturn value to the caller
//            return eReturn;
//        }

//        ///############################################################
//        /// <summary>
//        /// Translates the provided search text and comparison type into the equivalent SQL where clause based on the referenced Form input.
//        /// </summary>
//        /// <param name="sFunction">String representing the calling function's name.</param>
//        /// <param name="oInputData">InputData representing the Form input to query.</param>
//        /// <param name="eComparisonType">Enumeration representing the required comparison type.</param>
//        /// <param name="bFullyQualifyColumnNames">Boolean value indicating if the column names are to be fully qualified (i.e. - "Table.Column").</param>
//        /// <returns>String representing formatted where clause.</returns>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is null or a null-string.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> has not be defined.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is of an unknown or unsupported data type.</exception>
//        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eComparisonType</paramref> is invalid based on the <paramref>sInputAlias</paramref>'s data type.</exception>
//        ///############################################################
//        /// <LastUpdated>August 22, 2007</LastUpdated>
//        private string DoFormatWhereClause(string sFunction, Inputs.InputData oInputData, enumComparisonTypes eComparisonType, bool bFullyQualifyColumnNames) {
//            string[] a_sValues;
//            string sColumnName;
//            string sText;
//            string sReturn = "";
//            MetaData.enumDataTypes eDataType = oInputData.DataType;
//            int iValueType = (int)oInputData.ValueType;
//            int i;
//            bool bRaiseComparisonTypeError = false;

//                //#### If we are supposed to bFullyQualify(the)ColumnNames, set the sColumnName accordingly
//            if (bFullyQualifyColumnNames) {
//                sColumnName = oInputData.TableName + "." + oInputData.ColumnName;
//            }
//                //#### Else we're not supposed to bFullyQualify(the)ColumnNames, so set the sColumnName accordingly
//            else {
//                sColumnName = oInputData.ColumnName;
//            }

//                //#### If this is a .SearchAll request, set the ga_sValues to the .SearchAllText (dimensioning ga_sValues as we go)
//            if (SearchAll) {
//                a_sValues = new string[1];
//                a_sValues[0] = SearchAllText;
//            }
//                //#### Else this is not a .SearchAll request, so set the ga_sValues to the oInputData's .Values
//            else {
//                a_sValues = oInputData.Values;
//            }

//            //##########
//            //##########

//                //#### If we are supposed to search for null-strings/nulls
//            if (eComparisonType == enumComparisonTypes.cnGeneral_IsNullStringIsNull) {
//                    //#### Get(the)IsNullStringIsNullSQL, setting it into the sReturn value
//                sReturn = GetIsNullStringIsNullSQL(sColumnName, eDataType, eComparisonType, oInputData.IsNullable, true, "");
//            }
//                //#### Else we have to work for a living
//            else {
//                    //#### If this is a .cnMultiValueSearchInSingleValuePicklistExType (many to 1) search field
//                    //####     NOTE: There are 4 kinds of picklist searches: .cnSingleValuePicklistExType (1 to 1), .cnMultiValuePicklistExType (many to many), .cnSingleValueSearchInMultiValuePicklistExType (1 to many) and cnMultiValueSearchInSingleValuePicklistExType (many to 1). This block covers only .cnMultiValueSearchInSingleValuePicklistExType (many to 1) searches.
//                    //####     NOTE: Since we can only compair a single boolean flag at a time, we remove the .cnMultiValuePicklistExType component from .cnMultiValueSearchInSingleValuePicklistExType, which leaves the bit unique only to .cnMultiValueSearchInSingleValuePicklistExType's.
//                if ((iValueType & ((int)MetaData.enumDataTypes.cnMultiValueSearchInSingleValuePicklistExType - (int)MetaData.enumDataTypes.cnMultiValuePicklistExType)) != 0) {
//                        //#### If we have ga_sValues to search for
//                    if (a_sValues != null && a_sValues.Length > 0) {
//                            //#### Determine the eComparisonType and process accordingly
//                        switch (eComparisonType) {
//                                //#### If the user wants a _ContainsNone comparison, set the borrowed sText to " NOT IN "
//                            case enumComparisonTypes.cnMultiPicklist_ContainsNone: {
//                                sText = " NOT IN ";
//                                break;
//                            }
//                                //#### Else the user wants a _ContainsAny comparison, so set the borrowed sText to " IN "
//                            default: { //# enumComparisonTypes.cnMultiPicklist_ContainsAny
//                                sText = " IN ";
//                                break;
//                            }
//                        }

//                            //#### Traverse the ga_sValues, not including the last element
//                        for (i = 0; i < (a_sValues.Length - 1); i++) {
//                                //#### Append the a_sCurrentValue onto the sReturn value
//                            sReturn += Statements.FormatForSQL(a_sValues[i], false) + "','";
//                        }

//                            //#### Congeal the sReturn value, appending the last a_sValue into its proper position
//                        sReturn = sColumnName + sText + "('" + sReturn + Statements.FormatForSQL(a_sValues[i], false) + "')";
//                    }
//                }
//                    //#### If this is a .cnSingleValueSearchInMultiValuePicklistExType (1 to many) search field
//                    //####     NOTE: There are 4 kinds of picklist searches: .cnSingleValuePicklistExType (1 to 1), .cnMultiValuePicklistExType (many to many), .cnSingleValueSearchInMultiValuePicklistExType (1 to many) and cnMultiValueSearchInSingleValuePicklistExType (many to 1). This block covers only .cnSingleValueSearchInMultiValuePicklistExType (1 to many) searches.
//                    //####     NOTE: Since .cnSingleValueSearchInMultiValuePicklistExType == .cnSingleValuePicklistExType | .cnMultiValuePicklistExType, and because we can only do one boolean flag check at a time, the logic below looks for both .cnSingleValuePicklistExType and .cnMultiValuePicklistExType.
//                else if ((iValueType & (int)Web.Inputs.enumValueTypes.cnMultiValuesFromPicklist) != 0 &&
//                    (iValueType & (int)Web.Inputs.enumValueTypes.cnSingleValueFromPicklist) != 0
//                ) {
//                        //#### If we have ga_sValues to search for
//                    if (a_sValues != null && a_sValues.Length > 0) {
//                            //#### Determine the eComparisonType and process accordingly
//                        switch (eComparisonType) {
//                                //#### If the user wants a _NotEqual comparison, set the borrowed sText to NOT LIKE
//                            case enumComparisonTypes.cnGeneral_NotEqual: {
//                                sText = " NOT LIKE ";
//                                break;
//                            }
//                                //#### Else the user wants an _Equals comparison, so set the borrowed sText to LIKE
//                            default: { //# enumComparisonTypes.cnGeneral_Equals
//                                sText = " LIKE ";
//                                break;
//                            }
//                        }

//                            //#### Borrow the use of the sReturn value to store the .PrimaryDelimiter
//                        sReturn = Configuration.Settings.PrimaryDelimiter;

//                            //#### Congeal the sReturn value, surrounding the single a_sValue with .PrimaryDelimiters (which is within the borrowed sReturn value)
//                        sReturn = sColumnName + sText + "'%" + sReturn + Statements.FormatForSQL(a_sValues[0], true) + sReturn + "%'";
//                    }
//                }
//                    //#### If this is a .cnMultiValuePicklistExType (many to many) search field
//                    //####     NOTE: There are 4 kinds of picklist searches: .cnSingleValuePicklistExType (1 to 1), .cnMultiValuePicklistExType (many to many), .cnSingleValueSearchInMultiValuePicklistExType (1 to many) and cnMultiValueSearchInSingleValuePicklistExType (many to 1). This block covers only .cnMultiValuePicklistExType (many to many) searches.
//                    //####     NOTE: Because of the .cnSingleValueSearchInMultiValuePicklistExType (1 to many) test above, we do not need to explicitly exclude .cnSingleValuePicklistExType, as any were caught above.
//                //else if ((iValueType & (int)MetaData.enumDataTypes.cnMultiValuePicklistExType) != 0 &&
//                //	(iValueType & (int)MetaData.enumDataTypes.cnSingleValuePicklistExType) == 0
//                //) {
//                else if ((iValueType & (int)Web.Inputs.enumValueTypes.cnMultiValuesFromPicklist) != 0) {
//                        //#### If we have ga_sValues to search for
//                    if (a_sValues != null && a_sValues.Length > 0) {
//                        string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter; 
//                        string sBooleanClause;

//                            //#### Determine the eComparisonType and process accordingly
//                        switch (eComparisonType) {
//                                //#### If the user wants a _ContainsAll comparison, set sBooleanClause and the borrowed sText accordingly
//                            case enumComparisonTypes.cnMultiPicklist_ContainsAll: {
//                                sBooleanClause = FormatBooleanClause(enumBooleanTypes.cnAndType);
//                                sText = " LIKE ";
//                                break;
//                            }
//                                //#### If the user wants a _ContainsNone comparison, set sBooleanClause and the borrowed sText accordingly
//                            case enumComparisonTypes.cnMultiPicklist_ContainsNone: {
//                                sBooleanClause = FormatBooleanClause(enumBooleanTypes.cnAndType);
//                                sText = " NOT LIKE ";
//                                break;
//                            }
//                                //#### Else the user wants a _ContainsAny comparison, so sBooleanClause and the borrowed sText accordingly
//                            default: { //# enumComparisonTypes.cnMultiPicklist_ContainsAny
//                                sBooleanClause = FormatBooleanClause(enumBooleanTypes.cnOrType);
//                                sText = " LIKE ";
//                                break;
//                            }
//                        }

//                            //#### Traverse the ga_sValues from back to front (skipping the first element for now)
//                        for (i = (a_sValues.Length - 1); i >= 1; i--) {
//                                //#### Prepend the sReturn value preceeded by the sBooleanClause (surrounding each a_sCurrentValue with sPrimaryDelimiters)
//                            sReturn = sBooleanClause + sColumnName + sText + "'%" + sPrimaryDelimiter + Statements.FormatForSQL(a_sValues[i], true) + sPrimaryDelimiter + "%'" + sReturn;
//                        }

//                            //#### Prepend the first a_sCurrentValue (with no proceeding sBooleanClause) while surrounding the entire clause by parens
//                        sReturn = " (" + sColumnName + sText + "'%" + sPrimaryDelimiter + Statements.FormatForSQL(a_sValues[i], true) + sPrimaryDelimiter + "%'" + sReturn + ")";
//                    }
//                }
//                    //#### Else if we have a single a_sValue to process (as no other Types have multiple values)
//                    //####     NOTE: There are 4 kinds of picklist searches: .cnSingleValuePicklistExType (1 to 1), .cnMultiValuePicklistExType (many to many), .cnSingleValueSearchInMultiValuePicklistExType (1 to many) and cnMultiValueSearchInSingleValuePicklistExType (many to 1). This block covers the .cnSingleValuePicklistExType (1 to 1) search.
//                else if (a_sValues != null && a_sValues.Length > 0 && a_sValues[0].Length > 0) {
//                        //#### Set the sText to the single a_sValue to process
//                    sText = a_sValues[0];

//                        //#### If this is a boolean-based g_iDataType
////! do all db containers store booleans as 0/1? (ie - the use of .MakeBooleanInteger below)
//                    if (eDataType == MetaData.enumDataTypes.cnBoolean) {
//                            //#### If the passed sText .IsBoolean
//                        if (Cn.Data.Tools.IsBoolean(sText)) {
//                                //#### Determine the eComparisonType and set the return value accordingly
//                            switch (eComparisonType) {
//                                case enumComparisonTypes.cnGeneral_Equals:
//                                case enumComparisonTypes.cnDefaultComparisonType: {
//                                    sReturn = sColumnName + " = " + Cn.Data.Tools.MakeBooleanInteger(sText, true).ToString();
//                                    break;
//                                }
//                                case enumComparisonTypes.cnGeneral_NotEqual: {
//                                    sReturn = sColumnName + " != " + Cn.Data.Tools.MakeBooleanInteger(sText, true).ToString();
//                                    break;
//                                }

//                                    //#### Else the eComparisonType is invalid for this g_iDataType, so flip bRaiseComparisonTypeError to true
//                                default: {
//                                    bRaiseComparisonTypeError = true;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                        //#### Else if this is a numeric-based g_iDataType
//                    else if (eDataType == MetaData.enumDataTypes.cnInteger ||
//                        eDataType == MetaData.enumDataTypes.cnFloat ||
//                        eDataType == MetaData.enumDataTypes.cnCurrency
//                    ) {
//                            //#### If this is a .cnCurrency, remove the .cnCurrencySymbol before proceeding
//                        if (eDataType == MetaData.enumDataTypes.cnCurrency) {
//                            sText = sText.Replace(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnLocalization_CurrencySymbol), "");
//                        }

//                            //#### If the sText .Is(a)Number
//                            //####     NOTE: .IsNumber is utilized so that there are no range issues with very large/small numbers
//                        if (Cn.Data.Tools.IsNumber(sText)) {
//                                //#### Determine the eComparisonType and set the return value accordingly
//                                //####     NOTE: Due to the fact that the sText passed the .IsNumber test above, we do not need to .Format(it)ForSQL
//                            switch (eComparisonType) {
//                                case enumComparisonTypes.cnGeneral_Equals:
//                                case enumComparisonTypes.cnDefaultComparisonType: {
//                                    sReturn = sColumnName + " = " + sText;
//                                    break;
//                                }
//                                case enumComparisonTypes.cnGeneral_NotEqual: {
//                                    sReturn = sColumnName + " != " + sText;
//                                    break;
//                                }
//                                case enumComparisonTypes.cnGeneral_GreaterThen: {
//                                    sReturn = sColumnName + " > " + sText;
//                                    break;
//                                }
//                                case enumComparisonTypes.cnGeneral_LessThen: {
//                                    sReturn = sColumnName + " < " + sText;
//                                    break;
//                                }

//                                    //#### Else the eComparisonType is invalid for this g_iDataType, so flip bRaiseComparisonTypeError to true
//                                default: {
//                                    bRaiseComparisonTypeError = true;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                        //#### Else if this is a date-based g_iDataType
//                    else if (eDataType == MetaData.enumDataTypes.cnDateTime) {
//                            //#### If the sText .Is(a)Date
//                        if (Cn.Data.Tools.IsDate(sText)) {
//                                //#### Determine the eComparisonType and set the return value accordingly
//                            switch (eComparisonType) {
//                                    //#### If the passed eComparisonType is valid for this g_iDataType
//                                case enumComparisonTypes.cnGeneral_GreaterThen:
//                                case enumComparisonTypes.cnGeneral_LessThen:
//                                case enumComparisonTypes.cnGeneral_NotEqual:
//                                case enumComparisonTypes.cnGeneral_Equals:
//                                case enumComparisonTypes.cnDefaultComparisonType: {
//                                        //#### Get(the)DateSearchString, passing in the default .cnDate_DateTimeFormat if the developer didn't set their own DateTime_Format
//                                        //####     NOTE: No error is raised here if the developer didn't provide a DateTime_Format because that error should/would have been raised before we got here
//                                    sReturn = GetDateSearchString(sColumnName, sText, eComparisonType, Cn.Data.Tools.MakeString(oInputData.AdditionalData["DateTime_Format"], Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnLocalization_Date_DateTimeFormat)));
//                                    break;
//                                }

//                                    //#### Else the eComparisonType is invalid for this g_iDataType, so flip bRaiseComparisonTypeError to true
//                                default: {
//                                    bRaiseComparisonTypeError = true;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                        //#### Else if this is a string-based g_iDataType
//                    else if (eDataType == MetaData.enumDataTypes.cnChar ||
//                        eDataType == MetaData.enumDataTypes.cnLongChar
//                    ) {
//                            //#### Determine the eComparisonType and set the return value accordingly
//                        switch (eComparisonType) {
//                            case enumComparisonTypes.cnChar_Contains:
//                            case enumComparisonTypes.cnDefaultComparisonType: {
//                                sReturn = sColumnName + " LIKE '%" + Statements.FormatForSQL(sText, true) + "%'";
//                                break;
//                            }
//                            case enumComparisonTypes.cnChar_DoesNotContain: {
//                                sReturn = sColumnName + " NOT LIKE '%" + Statements.FormatForSQL(sText, true) + "%'";
//                                break;
//                            }
//                            case enumComparisonTypes.cnChar_Begins: {
//                                sReturn = sColumnName + " LIKE '" + Statements.FormatForSQL(sText, true) + "%'";
//                                break;
//                            }
//                            case enumComparisonTypes.cnChar_Ends: {
//                                sReturn = sColumnName + " LIKE '%" + Statements.FormatForSQL(sText, true) + "'";
//                                break;
//                            }
//                            case enumComparisonTypes.cnChar_Equals:
//                            case enumComparisonTypes.cnGeneral_Equals: {
//                                    //#### If this is a short string-based g_iDataType, this is an allowed eComparisonType so set the sReturn value accordingly
//                                if (eDataType == MetaData.enumDataTypes.cnChar) {
//                                    sReturn = sColumnName + " = '" + Statements.FormatForSQL(sText, false) + "'";
//                                }
//                                    //#### Else this is a long string-based g_iDataType, so flip bRaiseComparisonTypeError to true
//                                else {
//                                    bRaiseComparisonTypeError = true;
//                                }
//                                break;
//                            }
//                            case enumComparisonTypes.cnChar_NotEqual:
//                            case enumComparisonTypes.cnGeneral_NotEqual: {
//                                    //#### If this is a short string-based g_iDataType, this is an allowed eComparisonType so set the sReturn value accordingly
//                                if (eDataType == MetaData.enumDataTypes.cnChar) {
//                                    sReturn = sColumnName + " != '" + Statements.FormatForSQL(sText, false) + "'";
//                                }
//                                    //#### Else this is a long string-based g_iDataType, so flip bRaiseComparisonTypeError to true
//                                else {
//                                    bRaiseComparisonTypeError = true;
//                                }
//                                break;
//                            }

//                            case enumComparisonTypes.cnChar_Wildcards: {
//                                    //#### Transform the g_cWildcards_SingleChararacter into its SQL wildcard equivalent (if any)
////! eDbServerType?
//                                if (sReturn.Length > 0) {
//                                    sText = sText.Replace(g_cWildcards_SingleChararacter, "_");
//                                }

//                                    //#### Transform the g_cWildcards_Chararacters into its SQL wildcard equivalent (if any)
//                                if (sReturn.Length > 0) {
//                                    sText = sText.Replace(g_cWildcards_Chararacters, "%");
//                                }

//                                    //#### Format the where clause with the modified sText, setting the result into the sReturn value
//                                sReturn = sColumnName + " LIKE '" + Statements.FormatForSQL(sText, false) + "'";
//                                break;
//                            }

//                                //#### Else the eComparisonType is invalid for this g_iDataType, so flip bRaiseComparisonTypeError to true
//                            default: {
//                                bRaiseComparisonTypeError = true;
//                                break;
//                            }
//                        }
//                    }
//                        //#### Else if this is a currently unsupported g_iDataType, raise the error
//                    else if (eDataType == MetaData.enumDataTypes.cnBinary ||
//                        eDataType == MetaData.enumDataTypes.cnGUID
//                    ) {
//                        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnsupportedDataType, oInputData.InputAlias, "");
//                    }
//                        //#### Else the g_iDataType is .UnknownType or .UnreconizedType, so raise the error
//                    else {
//                        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "DataType", Cn.Data.Tools.MakeString(eDataType, ""));
//                    }
//                }

//                    //#### If we are supposed to include null-strings/nulls
//                if (Cn.Data.Tools.MakeBoolean(Request.Form[Inputs.InputID(oInputData.InputAlias) + "_IncludeNulls"], false)) {
//                        //#### If we already have a sReturn value, Get(the)IsNullStringIsNullSQL (properly positioning it as we go)
//                        //####     NOTE: We only include blanks if search criteria was specified by the user. If the user wants only blanks, then they must select it from the eComparisonType
//                    if (sReturn.Length > 0) {
//                        sReturn = "(" + sReturn + GetIsNullStringIsNullSQL(sColumnName, eDataType, eComparisonType, oInputData.IsNullable, false, "OR") + ")";
//                    }
// /*							//#### Else we do not yet have a sReturn value, so just Get(the)IsNullStringIsNullSQL
//                    else {
//                        sReturn = GetIsNullStringIsNullSQL(sColumnName, g_iDataType, eComparisonType, oInputData.IsNullable, false, "");
//                    }
//*/					}
//            }

//                //#### If we are supposed to bRaise(the)ComparisonTypeError, do so now
//            if (bRaiseComparisonTypeError) {
//                Internationalization.RaiseDefaultError(g_cClassName + sFunction, Cn.Configuration.Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_InvalidComparisonType, oInputData.InputAlias, "");
//            }

//                //#### Return the above determined sReturn value to the caller
//            return sReturn;
//        }

//        ///############################################################
//        /// <summary>
//        /// Returns the proper is null-string/is null SQL logic based on the provided information.
//        /// </summary>
//        /// <param name="sColumnName">String representing the column name.</param>
//        /// <param name="eDataType">Enumerated value representing the column's datatype.</param>
//        /// <param name="eComparisonType">Enumeration indicating the type of comparison is to take place.<para/>NOTE: Only the <c>cnGeneral_*</c> enums are reconized by this function. Any non-<c>cnGeneral_*</c> enums are logicially equated as <c>cnGeneral_Equals</c>.</param>
//        /// <param name="bIsNullable">Boolean value representing if the <paramref>sColumnName</paramref> is allowed to contain a null value according to the <c>DataSource</c>.</param>
//        /// <param name="bRequestFromDropDown">Boolean value indicating if the user searched for null-strings/nulls via the comparison type dropdown box.</param>
//        /// <param name="sStatementPrefix">String representing the SQL to include on the front of a non-null-string return value.</param>
//        /// <returns>String representing the proper is null-string/is null SQL logic based on the provided information.</returns>
//        ///############################################################
//        /// <LastUpdated>August 22, 2007</LastUpdated>
//        private string GetIsNullStringIsNullSQL(string sColumnName, MetaData.enumDataTypes eDataType, enumComparisonTypes eComparisonType, bool bIsNullable, bool bRequestFromDropDown, string sStatementPrefix) {
//            string sReturn = "";
//            bool bProcess = bRequestFromDropDown;

//                //#### If we are currently not supposed to bProcess this request (meaning this is a request from the _IncludeNulls checkbox or hidden input since bProcess is defaulted to the value of the passed bRequestFromDropDown)
//            if (! bProcess) {
//                    //#### Determine the eComparisonType, and if it is an excluding-style eComparisonType, flip bProcess to true
//                switch (eComparisonType) {
//                    case enumComparisonTypes.cnChar_DoesNotContain:
//                    case enumComparisonTypes.cnChar_NotEqual:
//                    case enumComparisonTypes.cnGeneral_NotEqual:
//                    case enumComparisonTypes.cnMultiPicklist_ContainsNone: {
//                        bProcess = true;
//                        break;
//                    }
//                }
//            }

//                //#### If we are to bProcess this call
//            if (bProcess) {
//                    //#### If the bRequest(came)From(the)DropDown and the sColumnName is a string-based g_iDataType
//                    //####     NOTE: We only process bRequest(s)From(the)DropDown below because those requests do not include search values (so we need to look for null-strings in addition to NULLs).
//                if (bRequestFromDropDown && (
//                    eDataType == MetaData.enumDataTypes.cnChar ||
//                    eDataType == MetaData.enumDataTypes.cnLongChar
//                )) {
//                        //#### If the sColumnName bIs(also)Nullable, set the sReturn value to the IS NULL and is null-string logic
//                    if (bIsNullable) {
//                        sReturn = " " + sStatementPrefix + " (" + sColumnName + " IS NULL OR " + sColumnName + " = '')";
//                    }
//                        //#### Else this oInputData bIs(not)Nullable, so set the sReturn value to the is null-string logic
//                    else {
//                        sReturn = " " + sStatementPrefix + " " + sColumnName + " = ''";
//                    }
//                }
//                    //#### Else if the sColumnName bIsNullable, set the sReturn value to the IS NULL logic
//                    //####     NOTE: Non-bRequestFromDropDown calls are routed thru this logic because they include search values (so there is no need to search for null-strings in addition to NULLs).
//                else if (bIsNullable) {
//                    sReturn = " " + sStatementPrefix + " " + sColumnName + " IS NULL";
//                }
//            }

//                //#### Return the above determined sReturn value to the caller
//            return sReturn;
//        }

//        ///############################################################
//        /// <summary>
//        /// Returns the custom input identifer string that is inserted into the automaticially generated SQL statement for the provided custom Form inputs.
//        /// </summary>
//        /// <remarks>
//        /// NOTE: It is problematic ensuring that the value set into the sReturn value is unique within the entire where clause (as we'd have to build the whole thing first to be 100% sure the user didn't happen to include the exact string returned below as part of their search), but if the developer follows the best pratices outlined for the replacement of custom inputs, this issue if fully avoided as we are guarenteed that at in the revelent section of the where clause the returned string is unique.
//        /// <para/>NOTE: The <paramref>sInputAlias</paramref> is assumed to be a custom input (it is not verified by this function).
//        /// </remarks>
//        /// <param name="sSQL">String representing the current SQL statement.</param>
//        /// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
//        /// <returns>String representing the custom input identifer string for the passed custom <paramref>sInputAlias</paramref>.</returns>
//        ///############################################################
//        /// <LastUpdated>September 2, 2005</LastUpdated>
//        private string GetCustomInputIdentifier(string sSQL, string sInputAlias) {
//            string sVanillaText = "$" + Web.Settings.Value(Web.Settings.enumSettingValues.cnDOMElementPrefix) + "CustomInput_" + sInputAlias;
//            string sReturn;
//            int i = 0;

//                //#### Default the sReturn value to the sVanillaText for the $CustomInput_ variable followed by its MD5
//            sReturn = sVanillaText + "_" + Cn.Data.Tools.MD5(sVanillaText);

//                //#### Do while the identifier in the return value is present within the passed sSQL
//            while (sSQL.IndexOf(sReturn) > -1) {
//                    //#### Append the current value of i onto the sVanillaText in the MD5 calculation, then inc i just in case we need to try again
//                    //####     NOTE: Thsi is really an industrial strength approach to this problem, but since the user can litterly enter anything they choose to search for, this over-the-top approach is called for
//                sReturn = sVanillaText + "_" + Cn.Data.Tools.MD5(sVanillaText + i);
//                i++;
//            }

//                //#### Return the above determined sReturn value to the caller
//            return sReturn;
//        }

//        ///############################################################
//        /// <summary>
//        /// Returns a SQL query block that compares only the present date/time elements.
//        /// </summary>
//        /// <param name="sColumnName">String representing the column name.</param>
//        /// <param name="sDate">String representing the date/time </param>
//        /// <param name="eComparisonType">Enumeration indicating the type of comparison is to take place.<para/>NOTE: Only the <c>cnGeneral_*</c> enums are reconized by this function. Any non-<c>cnGeneral_*</c> enums are logicially equated as <c>cnGeneral_Equals</c>.</param>
//        /// <param name="sDateTimeFormat">String representing the input's date/time format.</param>
//        /// <returns>String representing the SQL query block that compares only the present date/time elements.</returns>
//        ///############################################################
//        /// <LastUpdated>May 26, 2006</LastUpdated>
//        private string GetDateSearchString(string sColumnName, string sDate, enumComparisonTypes eComparisonType, string sDateTimeFormat) {
//            string sReturn = "";
//            string sFirstOperator;
//            string sOperator;
////! eDbServerType?
////! $E ?

//                //#### Determine the eComparisonType, setting the sFirstOperator and sOperator accordingly
//                //####     NOTE: The smallest portion of the date is the only element that can be _GreaterThen or _LessThen. All other elements MUST also be "or equal to" or the logic will not function correctly (ie - "YYYY >= 2005 && MM >= 1 && DD > 1" is correct, where as "YYYY > 2005 && MM > 1 && DD > 1" is not).
//            switch (eComparisonType) {
//                case enumComparisonTypes.cnGeneral_NotEqual: {
//                    sFirstOperator = "!=";
//                    sOperator = "!=";
//                    break;
//                }
//                case enumComparisonTypes.cnGeneral_GreaterThen: {
//                    sFirstOperator = ">";
//                    sOperator = ">=";
//                    break;
//                }
//                case enumComparisonTypes.cnGeneral_LessThen: {
//                    sFirstOperator = "<";
//                    sOperator = "<=";
//                    break;
//                }
//                default: { //#case enumComparisonTypes.cnGeneral_Equals, enumComparisonTypes.cnDefaultComparisonType
//                    sFirstOperator = "=";
//                    sOperator = "=";
//                    break;
//                }
//            }

//                //#### Remove any instances of \$ from the passed sDateTimeFormat (as escaped $ are not token identifiers, which is all that we are interested in)
//            sDateTimeFormat = sDateTimeFormat.Replace("\\$", "");

//                //#### Format the passed sDate
//            sDate = Statements.FormatForSQL(sDate, false);

//                //#### If the passed sDateTimeFormat contains a definition for a second
//            if (sDateTimeFormat.IndexOf("$s") > -1) {
//                    //#### If we have already found the first date part, use the sOperator in this comparison
//              //if (sReturn.Length > 0) {
//              //	sReturn += "DATEPART(ss, " + sColumnName + ") " + sOperator + " DATEPART(ss, '" + sDate + "') AND ";
//              //}
//                    //#### Else we have not yet bFound(the)First date part, so use the sFirstOperator in this comparison
//              //else {
//                    sReturn += "DATEPART(ss, " + sColumnName + ") " + sFirstOperator + " DATEPART(ss, '" + sDate + "') AND ";
//              //}
//            }

//                //#### If the passed sDateTimeFormat contains a definition for a minute
//            if (sDateTimeFormat.IndexOf("$m") > -1) {
//                    //#### If we have already found the first date part, use the sOperator in this comparison
//                if (sReturn.Length > 0) {
//                    sReturn += "DATEPART(mi, " + sColumnName + ") " + sOperator + " DATEPART(mi, '" + sDate + "') AND ";
//                }
//                    //#### Else we have not yet bFound(the)First date part, so use the sFirstOperator in this comparison and flip bFoundFirst
//                else {
//                    sReturn += "DATEPART(mi, " + sColumnName + ") " + sFirstOperator + " DATEPART(mi, '" + sDate + "') AND ";
//                }
//            }

//                //#### If the passed sDateTimeFormat contains a definition for a 12 or 24-based hour
//            if (sDateTimeFormat.IndexOf("$H") > -1 ||
//                sDateTimeFormat.IndexOf("$h") > -1
//            ) {
//                    //#### If we have already found the first date part, use the sOperator in this comparison
//                if (sReturn.Length > 0) {
//                    sReturn += "DATEPART(hh, " + sColumnName + ") " + sOperator + " DATEPART(hh, '" + sDate + "') AND ";
//                }
//                    //#### Else we have not yet bFound(the)First date part, so use the sFirstOperator in this comparison and flip bFoundFirst
//                else {
//                    sReturn += "DATEPART(hh, " + sColumnName + ") " + sFirstOperator + " DATEPART(hh, '" + sDate + "') AND ";
//                }
//            }

//                //#### If the passed sDateTimeFormat contains a definition for a month day
//            if (sDateTimeFormat.IndexOf("$D") > -1) {
//                    //#### If we have already found the first date part, use the sOperator in this comparison
//                if (sReturn.Length > 0) {
//                    sReturn += "DATEPART(dd, " + sColumnName + ") " + sOperator + " DATEPART(dd, '" + sDate + "') AND ";
//                }
//                    //#### Else we have not yet bFound(the)First date part, so use the sFirstOperator in this comparison and flip bFoundFirst
//                else {
//                    sReturn += "DATEPART(dd, " + sColumnName + ") " + sFirstOperator + " DATEPART(dd, '" + sDate + "') AND ";
//                }
//            }

//                //#### If the passed sDateTimeFormat contains a definition for a month
//            if (sDateTimeFormat.IndexOf("$M") > -1) {
//                    //#### If we have already found the first date part, use the sOperator in this comparison
//                if (sReturn.Length > 0) {
//                    sReturn += "DATEPART(mm, " + sColumnName + ") " + sOperator + " DATEPART(mm, '" + sDate + "') AND ";
//                }
//                    //#### Else we have not yet bFound(the)First date part, so use the sFirstOperator in this comparison and flip bFoundFirst
//                else {
//                    sReturn += "DATEPART(mm, " + sColumnName + ") " + sFirstOperator + " DATEPART(mm, '" + sDate + "') AND ";
//                }
//            }

//                //#### If the passed sDateTimeFormat contains a definition for the year
//            if (sDateTimeFormat.IndexOf("$YY") > -1) {
//                    //#### If we have already found the first date part, use the sOperator in this comparison
//                if (sReturn.Length > 0) {
//                    sReturn += "DATEPART(yy, " + sColumnName + ") " + sOperator + " DATEPART(yy, '" + sDate + "') AND ";
//                }
//                    //#### Else we have not yet bFound(the)First date part, so use the sFirstOperator in this comparison and flip bFoundFirst
//                else {
//                    sReturn += "DATEPART(yy, " + sColumnName + ") " + sFirstOperator + " DATEPART(yy, '" + sDate + "') AND ";
//                }
//            }

//                //#### If we determined a sReturn value above
//            if (sReturn.Length > 0) {
//                    //#### Pre/apending parens to our sReturn value and remove the trailing boolean operater as we go (minus 5 to peal off the trailing " AND ")
//                sReturn = "(" + sReturn.Substring(0, sReturn.Length - 5) + ")";
//            }

//                //#### Return the above determined sReturn value to the caller
//            return sReturn;
//        }
//        #endregion


//        ///########################################################################################################################
//        /// <summary>
//        /// Input Collection implementation for <c>Renderer.Form</c>.
//        /// </summary>
//        ///########################################################################################################################
//        /// <LastFullCodeReview></LastFullCodeReview>
//        #region SearchInputCollection
//        public class SearchInputCollection : Inputs.InputCollection {
//                //#### Declare the required private variables
//            private Hashtable g_hCustomFormInputs;
//            private string g_sInputOrdering;
//            private string g_sInputAliases;
//            private bool g_bVerifyInputOrdering;
//            private bool g_bOrderingFromForm;

//                //#### Declaire the required public classes
//            public class CustomInputData {
//                public string Identifer;
//                public string TableName;
//                public MetaData.enumDataTypes DataType;
//                public bool HasBeenReplaced;
//            }

//                //#### Declare the required private constants
//            private const string g_cClassName = "Cn.Web.Renderer.RendererSearchForm.SearchInputCollection.";


//            //##########################################################################################
//            //# Class Functions
//            //##########################################################################################
//            public SearchInputCollection(Settings.Current oSettings, Form oForm) : base(oSettings, oForm) {}



//            ///############################################################
//            /// <summary>
//            /// Resets the class to its initilized state.
//            /// </summary>
//            /// <remarks>
//            /// NOTE: The passed <paramref>sInputOrdering</paramref> is ignored if we are not currently processing the Form. You are required to use <c>SetInputOrder</c> to define the input ordering on a rendered search form.
//            /// </remarks>
//            /// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
//            /// <param name="oForm">Form object reference to the related <c>Form</c> object.</param>
//            /// <param name="sInputOrdering">String representing the <c>PrimaryDelimiter</c> delimited string of input aliases in the order as they appear on the rendered Form.</param>
//            ///############################################################
//            /// <LastUpdated>August 22, 2007</LastUpdated>
//            public void Reset(Settings.Current oSettings, Renderer.Form oForm, string sInputOrdering) {
//                string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter;

//                    //#### (Re)set the base variables
//                base.Reset(oSettings, oForm);

//                    //#### (Re)set the global variables
//                g_hCustomFormInputs = new Hashtable();
//                g_sInputAliases = "";
//                g_bVerifyInputOrdering = true;

//                    //#### If the caller passed in an sInputOrdering and we are supposed to .Process(the)Form
//                if (sInputOrdering != null && sInputOrdering.Length > 0 && IsPostBack) {
//                    int iLen = sPrimaryDelimiter.Length;

//                        //#### Set the g_sInputOrdering (optionally pre/appending a sPrimaryDelimiter if it's not already there)
//                    g_sInputOrdering = sInputOrdering;
//                    if (g_sInputOrdering.Substring(0, iLen) != sPrimaryDelimiter) {
//                        g_sInputOrdering = sPrimaryDelimiter + g_sInputOrdering;
//                    }
//                    if (g_sInputOrdering.Substring((g_sInputOrdering.Length - iLen), iLen) != sPrimaryDelimiter) {
//                        g_sInputOrdering += sPrimaryDelimiter;
//                    }

//                        //#### Set g_bOrderingFromForm to false (as the order was defined from the constructor/.Reset call)
//                    g_bOrderingFromForm = false;
//                }
//                    //#### Else the caller didn't pass in an sInputOrdering (or we're not .Process(ing the)Form)
//                else {
//                        //#### Default g_sInputOrdering to a leading sPrimaryDelimiter and set g_bOrderingFromForm to true (as the developer must define the ordering from the form)
//                    g_sInputOrdering = sPrimaryDelimiter;
//                    g_bOrderingFromForm = true;
//                }
//            }


//            //##########################################################################################
//            //# Public Properties
//            //##########################################################################################
//            ///############################################################
//            /// <summary>
//            /// Gets/sets a value indicating if we are to verify the input ordering.
//            /// </summary>
//            /// <value>Boolean value indicating if we are to verify the input ordering.</value>
//            ///############################################################
//            /// <LastUpdated>May 17, 2005</LastUpdated>
//            public bool VerifyInputOrdering {
//                get { return g_bVerifyInputOrdering; }
//                set { g_bVerifyInputOrdering = value; }
//            }


//            //##########################################################################################
//            //# Public Read-Only Properties
//            //##########################################################################################
//            ///############################################################
//            /// <summary>
//            /// Gets a value representing all of the defined <c>RendererSearchForm</c> input aliases.
//            /// </summary>
//            /// <value>Array of strings where each element represents a defined <c>RendererSearchForm</c> input alias.</value>
//            ///############################################################
//            /// <LastUpdated>February 9, 2006</LastUpdated>
//            new public string[] InputAliases {
//                get {
//                    string[] a_sReturn = null;
//                    string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter;
//                    int iLen = g_sInputAliases.Length;

//                        //#### If there is a value in g_sInputAliases, we can return something other then null
//                    if (iLen > 0) {
//                            //#### Return the .Split g_sInputOrdering variable to the caller (removing the trailing sPrimaryDelimiter while we're at it)
//                        a_sReturn = g_sInputAliases.Substring(0, (iLen - sPrimaryDelimiter.Length)).Split(sPrimaryDelimiter.ToCharArray());
//                    }

//                        //#### Return the above determined a_sReturn value to the caller
//                    return a_sReturn;
//                }
//            }

//            ///############################################################
//            /// <summary>
//            /// Gets a value representing the defined standard <c>RendererSearchForm</c> input aliases.
//            /// </summary>
//            /// <remarks>
//            /// NOTE: This is a simple alias to <see cref='Cn.Web.Renderer.InputTools.InputAliases'>Renderer.Form.Inputs.InputAliases</see> provided for ease of use only.
//            /// </remarks>
//            /// <value>Array of strings where each element represents a standard <c>RendererSearchForm</c> input alias.</value>
//            ///############################################################
//            /// <LastUpdated>August 22, 2007</LastUpdated>
//            private string[] StandardInputAliases {
//                get { return base.InputAliases(); }
//            }

//            ///############################################################
//            /// <summary>
//            /// Gets a value representing the defined custom <c>RendererSearchForm</c> input aliases.
//            /// </summary>
//            /// <value>Array of strings where each element represents a defined custom <c>RendererSearchForm</c> input alias.</value>
//            ///############################################################
//            /// <LastUpdated>February 9, 2006</LastUpdated>
//            private string[] CustomInputAliases {
//                get {
//                    string[] a_sReturn = null;
//                    int iCount = g_hCustomFormInputs.Count;

//                        //#### If we have some defined g_hCustomFormInputs
//                    if (iCount > 0) {
//                            //#### Retieve the .Keys from the g_hCustomFormInputs into the a_sReturn value
//                        a_sReturn = new string[iCount];
//                        g_hCustomFormInputs.Keys.CopyTo(a_sReturn, 0);
//                    }

//                        //#### Return the above determined a_sReturn value to the caller
//                    return a_sReturn;
//                }
//            }

//            ///############################################################
//            /// <summary>
//            /// Gets a value representing the defined searchable <c>RendererSearchForm</c> input aliases.
//            /// </summary>
//            /// <returns>Array of strings where each element represents a searchable <c>RendererSearchForm</c> input alias.</returns>
//            ///############################################################
//            /// <LastUpdated>August 22, 2007</LastUpdated>
//            public string[] SearchableInputAliases {
//                get {
//                    CustomInputData oCustomFormInput;
//                    Inputs.InputData oFormInput;
//                    string[] a_sInputAliases;
//                    string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter;
//                    string sReturn = "";
//                    int i;

//                        //#### Get the .CustomInputAliases
//                    a_sInputAliases = CustomInputAliases;

//                        //#### If we have custom a_sInputAliases to traverse
//                    if (a_sInputAliases != null && a_sInputAliases.Length > 0) {
//                            //#### Traverse the custom a_sInputAliases
//                        for (i = 0; i < a_sInputAliases.Length; i++) {
//                                //#### Collect the oCustomFormInput for this loop
//                            oCustomFormInput = ((CustomInputData)g_hCustomFormInputs[a_sInputAliases[i]]);

//                                //#### If the oCustomFormInput is not a .cnNonSearchable
//                            if (((int)oCustomFormInput.DataType & (int)enumSearchFormDataTypes.cnNonSearchable) == 0) {
//                                    //#### Append the current a_sInputAlias onto the sReturn value (followed by a trailing sPrimaryDelimiter)
//                                sReturn += a_sInputAliases[i] + sPrimaryDelimiter;
//                            }
//                        }
//                    }

//                        //#### Get the base's .InputAliases
//                    a_sInputAliases = base.InputAliases();

//                        //#### If we have custom a_sInputAliases to traverse
//                    if (a_sInputAliases != null && a_sInputAliases.Length > 0) {
//                            //#### Traverse the custom a_sInputAliases
//                        for (i = 0; i < a_sInputAliases.Length; i++) {
//                                //#### Collect the oFormInput for this loop
//                            oFormInput = base.Get(a_sInputAliases[i]);

//                                //#### If the oFormInput is not a .cnNonSearchable
//                            if (oFormInput.AdditionalMetaData.ExtendedDataType == (int)enumSearchFormDataTypes.cnNonSearchable) {
//                                    //#### Append the current a_sInputAlias onto the sReturn value (followed by a trailing sPrimaryDelimiter)
//                                sReturn += a_sInputAliases[i] + sPrimaryDelimiter;
//                            }
//                        }
//                    }

//                        //#### If some searchable inputs were found above, return the .Split sReturn value to the caller (borrowing the use of i to store the sReturn value's .Length and pealing off the trailing sPrimaryDelimiter as we go)
//                    i = sReturn.Length;
//                    if (i > 0) {
//                        return sReturn.Substring(0, i - sPrimaryDelimiter.Length).Split(sPrimaryDelimiter.ToCharArray());
//                    }
//                        //#### Else no searchable inputs were found above, so return null
//                    else {
//                        return null;
//                    }
//                }
//            }

//            ///############################################################
//            /// <summary>
//            /// Gets a value representing the defined input ordering (delimited by the <c>PrimaryDelimiter</c>s).
//            /// </summary>
//            /// <value>String representing the defined input ordering (delimited by the <c>PrimaryDelimiter</c>s).</value>
//            ///############################################################
//            /// <LastUpdated>August 31, 2005</LastUpdated>
//            public string InputOrdering {
//                get {
//                    string sReturn = "";
//                    int iLenDelimiter = Configuration.Settings.PrimaryDelimiter.Length;
//                    int iLen = g_sInputOrdering.Length;

//                        //#### If there is a (real) value in g_sInputOrdering, we can return something other then a null-string
//                    if (iLen > (iLenDelimiter * 2)) {
//                            //#### Return the private g_sInputOrdering variable to the caller (removing the leading/trailing .PrimaryDelimiter while we're at it)
//                        sReturn = g_sInputOrdering.Substring(iLenDelimiter, (iLen - (iLenDelimiter * 2)));
//                    }

//                        //#### Return the above determined sReturn value to the caller
//                    return sReturn;
//                }
//            }


//            //##########################################################################################
//            //# Public Functions
//            //##########################################################################################
//            ///############################################################
//            /// <summary>
//            /// Determines if this instance contains the referenced input alias.
//            /// </summary>
//            /// <remarks>
//            /// NOTE: This implementation checks both standard <c>Form</c> inputs and custom <c>RendererSearchForm</c> inputs.
//            /// </remarks>
//            /// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
//            /// <returns></returns>
//            /// <returns>Boolean value signaling the existance of the passed <paramref>sInputAlias</paramref>.<para/>Returns true if the passed <paramref>sInputAlias</paramref> is defined within this instance, or false if it is not present.</returns>
//            ///############################################################
//            /// <LastUpdated>December 21, 2005</LastUpdated>
//            new public bool Exists(string sInputAlias) {
//                return Exists(sInputAlias, enumInputTypes.cnFormOrCustomInput);
//            }

//            ///############################################################
//            /// <summary>
//            /// Determines if this instance contains the referenced input alias.
//            /// </summary>
//            /// <remarks>
//            /// NOTE: This implementation checks both standard <c>Form</c> inputs and custom <c>RendererSearchForm</c> inputs.
//            /// </remarks>
//            /// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
//            /// <param name="eInputType">Enumeration representing the input collection to check.</param>
//            /// <returns>Boolean value signaling the existance of the passed <paramref>sInputAlias</paramref>.<para/>Returns true if the passed <paramref>sInputAlias</paramref> is defined within this instance, or false if it is not present.</returns>
//            ///############################################################
//            /// <LastUpdated>August 22, 2007</LastUpdated>
//            public bool Exists(string sInputAlias, enumInputTypes eInputType) {
//                bool bReturn;

//                    //#### Determine the eInputType and process accordingly
//                switch (eInputType) {
//                        //#### If this is a cnCustomInput request, set the bReturn value accordingly
//                    case enumInputTypes.cnCustomInput: {
//                        bReturn = g_hCustomFormInputs.Contains(sInputAlias);
//                        break;
//                    }
//                        //#### If this is a cnFormInput request, set the bReturn value accordingly
//                    case enumInputTypes.cnFormInput: {
//                        bReturn = base.Exists(sInputAlias);
//                        break;
//                    }
//                        //#### If this is a cnFormOrCustomInput request, set the bReturn value accordingly
//                    case enumInputTypes.cnFormOrCustomInput: {
//                        bReturn = (g_hCustomFormInputs.Contains(sInputAlias) || base.Exists(sInputAlias));
//                        break;
//                    }

//                        //#### Else the passed eInputType was unreconized, so raise the error
//                    default: {
//                        Internationalization.RaiseDefaultError(g_cClassName + "Exists", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eInputType", Cn.Data.Tools.MakeString(eInputType, ""));
//                        bReturn = false;
//                        break;
//                    }
//                }
				
//                    //#### Return the above determined bReturn value to the caller
//                return bReturn;
//            }


//public CustomInputData GetCustom(string sInputAlias) {
//    CustomInputData oReturn = null;

//        //#### If the passed sInputAlias exists within the g_hCustomFormInputs, reset the oReturn value
//    if (g_hCustomFormInputs.Contains(sInputAlias)) {
//        oReturn = (CustomInputData)g_hCustomFormInputs[sInputAlias];
//    }

//        //#### Return the above determined oReturn value to the caller
//    return oReturn;
//}



//            public void Add(string sTableName, string sColumnName, string sInputAlias, enumSaveTypes eSaveType, Inputs.enumValueTypes eValueType, enumSearchFormDataTypes eExtendedDataType, Hashtable h_sAdditionalData) {
//                Inputs.InputData.structAdditionalMetaData oAdditionalMetaData = new Inputs.InputData.structAdditionalMetaData();
//                bool bIsRequired = false;

//                    //#### If the passed eSaveType indicates an bIsRequired column, reset bIsRequired to true
//                if (eSaveType == enumSaveTypes.cnRequired || eSaveType == enumSaveTypes.cnID) {
//                    bIsRequired = true;
//                }

//                    //#### GetData the oAdditionalMetaData with the passed data
//                oAdditionalMetaData.SaveType = (int)eSaveType;
//                oAdditionalMetaData.IsID = (eSaveType == enumSaveTypes.cnID);
//                oAdditionalMetaData.ExtendedDataType = (int)eExtendedDataType;

//                    //#### Pass the call off to our base sibling implementation to do the actual work
//                base.Add(sTableName, sColumnName, sInputAlias, eValueType, bIsRequired, h_sAdditionalData, oAdditionalMetaData);
//            }

//            ///############################################################
//            /// <summary>
//            /// Defines a <c>Form</c> input.
//            /// </summary>
//            /// <remarks>
//            /// NOTE: <c>RendererSearchForm</c> requires that any input defined as a <c>cnDateTime</c> carry and explicit definition for its <c>DateTime_Format</c> within <c>h_sAdditionalData</c>. This is done so that only the present date/time parts are searched on. It is required because we do not know at input definition if the <c>cnDateTime</c> input will be a date, date/time or time input.
//            /// <para/>NOTE: This implementation exists to facilitate the <c>DefineCustomInput</c> functionality. You must use this implementation to define inputs for a <c>RendererSearchForm</c>.
//            /// </remarks>
//            /// <param name="sTableName">String representing the column's source table name.</param>
//            /// <param name="sColumnName">String representing the column name.</param>
//            /// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
//            /// <param name="eSaveType">Enumeration representing the HTML input's form processing requirements.</param>
//            /// <param name="eExtendedDataType">Enumerated multi-value representing the input's extended datatype(s), which define the HTML input's form rendering requirements.</param>
//            /// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.FormRenderer.DefineInput'>FormRenderer.DefineInput</see>'s remarks).</param>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is null or a null-string.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is already defined.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sTableName</paramref>/<paramref>sColumnName</paramref> pair does not exist.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is defined as a <c>cnDateTime</c> and the related <c>h_sAdditionalData</c>'s <c>DateTime_Format</c> key is null or a null-string.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> on a <paramref>sColumnName</paramref> defind as not nullable by the data source.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnSingleValuePicklistExType</c> and/or <c>cnMultiValuePicklistExType</c> on a non <c>Picklist_IsAdHoc</c> and the global <c>Picklists</c> is null or contains no <c>Rows</c>.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnSingleValuePicklistExType</c> and/or <c>cnMultiValuePicklistExType</c> on a non <c>Picklist_IsAdHoc</c> and the <c>Picklist_Name</c> is null or a null-string.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnSingleValuePicklistExType</c> and/or <c>cnMultiValuePicklistExType</c> on a non <c>Picklist_IsAdHoc</c> and the <c>Picklist_Name</c> is not a reconized picklist name within the global <c>Picklists</c>.</exception>
//            /// <seealso cref="Cn.Web.Renderer.Form.DefineInput"/>
//            ///############################################################
//            /// <LastUpdated>May 10, 2007</LastUpdated>
//            public void DefineInput(string sTableName, string sColumnName, string sInputAlias, enumSaveTypes eSaveType, enumSearchFormDataTypes eExtendedDataType, Hashtable h_sAdditionalData) {
//                int iDataType;

//                    //#### If the passed sInputAlias is a null-string, raise the error
//                if (sInputAlias == null || sInputAlias.Length == 0) {
//                    Internationalization.RaiseDefaultError(g_cClassName + "DefineInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "sInputAlias", "");
//                }
//                    //#### Else if the sInputAlias has already been defined either as a g_hCustomFormInput or as a .Form.Input, raise the error
//                    //####     NOTE: The .Form.Input sInputAlias's are actually checked within .Form's .DefineInput, but in order to ensure that the error is raised from this call we also check it here
//                else if (g_hCustomFormInputs.Contains(sInputAlias) || base.Exists(sInputAlias)) {
//                    Internationalization.RaiseDefaultError(g_cClassName + "DefineInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_DuplicateInputAlias, sInputAlias, sTableName + "." + sColumnName);
//                }
//                    //#### Else if the passed sTableName/sColumnName pair does not .Exist, raise the error
//                else if (! Web.Settings.MetaData.Exists(sTableName, sColumnName)) {
//                    Internationalization.RaiseDefaultError(g_cClassName + "DefineInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbMetaData_InvalidTableColumnName, sTableName + "." + sColumnName, "");
//                }
//                    //#### Else the passed data seems valid
//                else {
//                        //#### Collect the g_iDataType for the passed sTableName/sColumnName pair
//                    iDataType = Cn.Data.Tools.MakeInteger(Web.Settings.MetaData.Value(sTableName, sColumnName, MetaData.enumMetaDataTypes.cnData_Type), (int)MetaData.enumDataTypes.cnUnknown);

//                        //#### If this is a .cnDateTime and a related DateTime_Format has not been defined, raise the error
//                    if ((iDataType & (int)MetaData.enumDataTypes.cnDateTime) != 0 &&
//                        (h_sAdditionalData == null || Cn.Data.Tools.MakeString(h_sAdditionalData["DateTime_Format"], "").Length == 0)
//                    ) {
//                        Internationalization.RaiseDefaultError(g_cClassName + "DefineInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_DateTimeFormatMissing, sInputAlias, "");
//                    }
//                        //#### Else all of the required data is present
//                    else {
//                            //#### Append the sInputAlias (followed by a trailing .PrimaryDelimiter) onto g_sInputAliases
//                        g_sInputAliases += sInputAlias + Configuration.Settings.PrimaryDelimiter;

//                            //#### Hand the call off to the .Form to actually .Define(the)Input (and continue the sInputAlias checking)
//                        DefineInput(sTableName, sColumnName, sInputAlias, eSaveType, eExtendedDataType, h_sAdditionalData);
//                    }
//                }
//            }

//            ///############################################################
//            /// <summary>
//            /// Defines a custom <c>RendererSearchForm</c> input, allowing for the definition of more complex searches.
//            /// </summary>
//            /// <remarks>
//            /// This functionality allows you to create searches that are more complicated then a simple multi-table selects. This allows you to define searches that, for example search across sub-tables (i.e. - "SELECT This, That FROM Table1, Table2 WHERE This = 'x' AND That = 'y';"). Since you as the developer is responsible for the SQL clauses related to these custom inputs, you are allowed to do anything that an SQL select statement would allow.
//            /// <para/>NOTE: Unless you are passing in a bare <c>*ExType</c> for <paramref>eDataType</paramref>, it is highly recommended that you utilize the code snipit below to determine the <paramref>eDataType</paramref> (else you will end up with hard-coded column datatype definitions):
//            /// <code>
//            /// int g_iDataType = Cn.Data.Tools.MakeInteger(Renderer.Settings.Value(sTableName, sColumnName, Data.MetaData.enumMetaDataTypes.cnData_Type), (int)Data.MetaData.enumDataTypes.cnUnknown);
//            /// </code>
//            /// </remarks>
//            /// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
//            /// <param name="sTableName">String representing the custom input's source table name.</param>
//            /// <param name="eDataType">Enumerated multi-value representing the column's datatype and the HTML input's form rendering requirements.</param>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is null or a null-string.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sTableName</paramref> is null or a null-string.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is already defined.</exception>
//            ///############################################################
//            /// <LastUpdated>December 14, 2005</LastUpdated>
//            public void DefineCustomInput(string sInputAlias, string sTableName, MetaData.enumDataTypes eDataType) {
//                CustomInputData oInputData = new CustomInputData();

//                    //#### If the passed sInputAlias is null or a null-string, raise the error
//                if (sInputAlias == null || sInputAlias.Length == 0) {
//                    Internationalization.RaiseDefaultError(g_cClassName + "DefineCustomInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, sInputAlias, "");
//                }
//                    //#### Else if the passed sTableName is null or a null-string, raise the error
//                else if (sTableName == null || sTableName.Length == 0) {
//                    Internationalization.RaiseDefaultError(g_cClassName + "DefineCustomInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, sTableName, "");
//                }
//                    //#### Else if the sInputAlias has already been defined either as a g_hCustomFormInput or as a .Form.Input, raise the error
//                    //####     NOTE: The .Form.Input sInputAlias's are actually checked within .Form's .DefineInput, but in order to ensure that the error is raised from this call we also check it here
//                else if (g_hCustomFormInputs.Contains(sInputAlias) || base.Exists(sInputAlias)) {
//                    Internationalization.RaiseDefaultError(g_cClassName + "DefineCustomInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_DuplicateInputAlias, sInputAlias, sTableName + ".[" + sInputAlias + "]");
//                }
//                    //#### Else the passed sInputAlias is unique
//                else {
//                        //#### Append the sInputAlias (followed by a trailing .PrimaryDelimiter) onto g_sInputAliases
//                    g_sInputAliases += sInputAlias + Configuration.Settings.PrimaryDelimiter;

//                        //#### .Add the passed sInputAlias and the eDataType into the g_hCustomFormInputs
//                    oInputData.Identifer = "";
//                    oInputData.TableName = sTableName;
//                    oInputData.DataType = eDataType;
//                    oInputData.HasBeenReplaced = false;
//                    g_hCustomFormInputs.Add(sInputAlias, oInputData);
//                }
//            }

//            ///############################################################
//            /// <summary>
//            /// Renders the requested search Form input.
//            /// </summary>
//            /// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
//            /// <param name="eSearchInputType">Enumeration representing the search form input type to render.</param>
//            /// <param name="sInitialValue">String representing the initial value of the input.</param>
//            /// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSearchInputType</paramref> is unreconized.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSearchInputType</paramref> is <c>cnComparisonTypeInput</c> and the <paramref>sInputAlias</paramref> is of an unsupported data type.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSearchInputType</paramref> is <c>cnComparisonTypeInput</c> and the <paramref>sInputAlias</paramref> is of an unknown or unreconized data type.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is of an unsupported data type.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is of an unknown or unreconized data type.</exception>
//            /// <returns>String representing the requested XHTML/DHTML control.</returns>
//            ///############################################################
//            /// <LastUpdated>August 6, 2007</LastUpdated>
//            public string GetInput(string sInputAlias, enumSearchInputTypes eSearchInputType, string sInitialValue, string sAttributes) {
//                MultiArray oPicklist;
//                string sReturn = "";

//                    //#### Determine the passed eSearchInputType and process accordingly
//                switch (eSearchInputType) {
//                        //#### If we're supposed to render a .cnBooleanInput combo
//                    case enumSearchInputTypes.cnBooleanInput: {
//                            //#### Determine the oPicklist for the passed sInputAlias
//                        oPicklist = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnRendererSearchForm_Boolean, Settings.EndUserMessagesLanguageCode);

//                            //#### Determine the sInputAlias then pass the call off to .Inputs .WriteSelectInput to render the _Boolean input
//                        sReturn = Web.Inputs.Select(InputID(sInputAlias) + "_Boolean", sAttributes, sInitialValue, false, oPicklist, oSettings);
//                        break;
//                    }

//                        //#### If we're supposed to render a .cnIncludeNullsHiddenInput hidden input
//                    case enumSearchInputTypes.cnIncludeNullsHiddenInput: {
//                        sReturn = Web.Inputs.Hidden(InputID(sInputAlias) + "_IncludeNulls", sAttributes, "true", oSettings);
//                        break;
//                    }

//                        //#### If we're supposed to render a .cnIncludeNullsCheckboxInput checkbox input
//                    case enumSearchInputTypes.cnIncludeNullsCheckboxInput: {
//                        sReturn = Web.Inputs.Checkbox(InputID(sInputAlias) + "_IncludeNulls", sAttributes, ref sInitialValue, oSettings);
//                        break;
//                    }

//                        //#### If we're supposed to render a .cnComparisonTypeInput combo
//                    case enumSearchInputTypes.cnComparisonTypeInput: {
//                            //#### Determine the oPicklist for the passed sInputAlias
//                        oPicklist = Picklist("Get", sInputAlias);

//                            //#### Determine the sInputAlias then pass the call off to .Inputs .WriteSelectInput to render the _Comparison input
//                        sReturn = Web.Inputs.Select(InputID(sInputAlias) + "_Comparison", sAttributes, sInitialValue, false, oPicklist, oSettings);
//                        break;
//                    }

//                        //#### Else the eSearchInputType is unreconized, so raise the error
//                    default: {
//                        Internationalization.RaiseDefaultError(g_cClassName + "RenderInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eSearchInputType", Cn.Data.Tools.MakeString(eSearchInputType, ""));
//                        break;
//                    }
//                }

//                    //#### Return the above determined sReturn value to the caller
//                return sReturn;
//            }

//            ///############################################################
//            /// <summary>
//            /// Sets the ordering of the provided Form input.
//            /// </summary>
//            /// <remarks>
//            /// This function allows us to properly construct the where clause, as we need to know the ordering of the inputs so the end-user entered boolean logic is respected.
//            /// NOTE: We do not ensure that the passed <paramref>sInputAlias</paramref> is searchable because of <c>Renderer.Form</c>'s <c>ModifyInput</c> function (which can change the input definition at any time before the Form is processed). If a <paramref>sInputAlias</paramref> is set here that is later found to be a <c>cnNonSearchable</c> during the record validation, the error is raised at that time.
//            /// </remarks>
//            /// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref>'s order has already been set.</exception>
//            ///############################################################
//            /// <LastUpdated>February 9, 2006</LastUpdated>
//            public void SetInputOrder(string sInputAlias) {
//                string sPrimaryDelimiter = Configuration.Settings.PrimaryDelimiter;

//                    //#### If the g_sInputOrdering was set from the constructor/.Reset call
//                if (! g_bOrderingFromForm) {
//                        //#### Reset g_sInputOrdering to a leading sPrimaryDelimiter and flip g_bOrderingFromForm (as the developer is now defining the input ordering from the form)
//                    g_sInputOrdering = sPrimaryDelimiter;
//                    g_bOrderingFromForm = true;
//                }

//                    //#### If the sInputAlias has not already been defined in the input ordering
//                if (g_sInputOrdering.IndexOf(sPrimaryDelimiter + sInputAlias + sPrimaryDelimiter) == -1) {
//                        //#### Append the passed sInputAlias (followed by a trailing sPrimaryDelimiter) onto g_sInputOrdering
//                    g_sInputOrdering += sInputAlias + sPrimaryDelimiter;
//                }
//                    //#### Else the developer has already defined the sInputAlias within the input ordering, so raise the error
//                else {
//                    Internationalization.RaiseDefaultError(g_cClassName + "SetInputOrder", Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_DuplicateInputOrderAlias, sInputAlias, "");
//                }
//            }


//            //##########################################################################################
//            //# Private Functions
//            //##########################################################################################
//            ///############################################################
//            /// <summary>
//            /// Retrieves the <see cref="InputOrdering">InputOrdering</see> as an array.
//            /// </summary>
//            /// <value>Array of strings where each element represents a <c>RendererSearchForm</c> input alias, in its defined ordering.</value>
//            ///############################################################
//            /// <LastUpdated>February 9, 2006</LastUpdated>
//            internal string[] GetInputOrdering() {
//                string[] a_sReturn = null;
//                string sInputOrdering = InputOrdering;

//                    //#### If the sInputOrdering has been defined
//                if (sInputOrdering.Length > 0) {
//                        //#### .Split it out into the a_sReturn value
//                    a_sReturn = InputOrdering.Split(Configuration.Settings.PrimaryDelimiter.ToCharArray());
//                }

//                    //#### Return the above determined a_sReturn value to the caller
//                return a_sReturn;
//            }

//            ///############################################################
//            /// <summary>
//            /// Verifies that the developer defined input ordering includes all of the searchable Form inputs.
//            /// </summary>
//            /// <param name="sFunction">String representing the calling function's name.</param>
//            /// <param name="a_sSearchableInputs">String array where each element represents a searchable input alias.</param>
//            /// <returns>Boolean value indicating if the developer defined all of the searchable input aliases in the input ordering.</returns>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sInputOrdering</paramref> does not contain the correct number of elements.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sInputOrdering</paramref> does not contain a defined searchable input alias.</exception>
//            /// <exception cref="Cn.CnException">Thrown when no ordered input aliases have been defined.</exception>
//            /// <exception cref="Cn.CnException">Thrown when no searchable input aliases have been defined.</exception>
//            ///############################################################
//            /// <LastUpdated>February 9, 2006</LastUpdated>
//            private bool DoVerifyInputOrdering(string sFunction, string[] a_sSearchableInputs) {
//                string[] a_sInputOrdering = GetInputOrdering();
//                string sCurrentValue;
//                int iLen;
//                int i;
//                int j;
//                bool bFoundAlias = false;

//                    //#### If the a_sInputOrdering was successfully collectd above
//                if (a_sInputOrdering != null && a_sInputOrdering.Length > 0) {
//                        //#### Determine a_sInputOrdering .Length
//                    iLen = a_sInputOrdering.Length;

//                        //#### If the .Length's of the a_sSearchableInputs and the a_sInputOrdering match
//                    if (a_sSearchableInputs.Length == iLen) {
//                            //#### Traverse the a_sSearchableInputs
//                        for (i = 0; i < iLen; i++) {
//                                //#### Reset the value of bFoundAlias and sCurrentValue for this loop
//                            bFoundAlias = false;
//                            sCurrentValue = a_sSearchableInputs[i];

//                                //#### Traverse the a_sInputOrdering
//                            for (j = 0; j < iLen; j++) {
//                                    //#### If the current a_sInputOrdering alias matches a_sSearchableInput's sCurrentValue (checking their .Length's first as that is a far faster comparison)
//                                if (a_sInputOrdering[j].Length == sCurrentValue.Length &&
//                                    a_sInputOrdering[j] == sCurrentValue
//                                ) {
//                                        //#### Flip bFoundAlias to true and fall from the inner loop
//                                    bFoundAlias = true;
//                                    break;
//                                }
//                            }

//                                //#### If the current a_sSearchableInput alias was not found above, raise the error (and fall out of the outer loop for good measure)
//                            if (! bFoundAlias) {
//                                Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_InputAliasMissingFromOrdering, a_sSearchableInputs[i], "");
//                                break;
//                            }
//                        }
//                    }
//                        //#### Else the developer didn't specify all of the a_sSearchableInputs within the a_sInputOrdering, so raise the error
//                    else {
//                        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_InputOrderingIncomplete, "", "");
//                    }
//                }
//                    //#### Else the .InputOrdering has not been defined, so raise the error
//                else {
//                    Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_RendererSearchForm_NoOrderedInputAliases, "", "");
//                }

//                    //#### Return the last set value of bFoundAlias to the caller (as if an error occured above it'll be set to false, and if the checks above succeed then it'll be true)
//                return bFoundAlias;
//            }

//            ///############################################################
//            /// <summary>
//            /// Retrieves the relevant search form comparison picklist for the provided Form input.
//            /// </summary>
//            /// <param name="sFunction">String representing the calling function's name.</param>
//            /// <param name="sInputAlias">String representing the Form input to query.</param>
//            /// <returns>MultiArray object representing the relevant search form comparison picklist for the provided <paramref>sInputAlias</paramref>.</returns>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is of an unsupported data type.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> is of an unknown or unreconized data type.</exception>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> has not be defined.</exception>
//            ///############################################################
//            /// <LastUpdated>April 5, 2006</LastUpdated>
//            private MultiArray Picklist(string sFunction, string sInputAlias) {
//                MultiArray oReturn = null;
//                string sPicklistData = "";
//                int iDataType = GetInputDataType(sFunction, sInputAlias);
//                bool bIsStringBased = false;

//                    //#### If this is a .cnMultiValueSearchInSingleValuePicklistExType (many to 1) search field
//                    //####     NOTE: There are 4 kinds of picklist searches: .cnSingleValuePicklistExType (1 to 1), .cnMultiValuePicklistExType (many to many), .cnSingleValueSearchInMultiValuePicklistExType (1 to many) and cnMultiValueSearchInSingleValuePicklistExType (many to 1). This block covers only .cnMultiValueSearchInSingleValuePicklistExType (many to 1) searches.
//                    //####     NOTE: Since we can only compair a single boolean flag at a time, we remove the .cnMultiValuePicklistExType component from .cnMultiValueSearchInSingleValuePicklistExType, which leaves the bit unique only to .cnMultiValueSearchInSingleValuePicklistExType's.
//                if ((iDataType & ((int)MetaData.enumDataTypes.cnMultiValueSearchInSingleValuePicklistExType - (int)MetaData.enumDataTypes.cnMultiValuePicklistExType)) != 0) {
//                    sPicklistData = "MultiValueSearchInSingleValuePicklist";
//                    oReturn = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnRendererSearchForm_MultiValueSearchInSingleValuePicklist, Settings.EndUserMessagesLanguageCode);
//                }
//                    //#### If this is a .cnSingleValueSearchInMultiValuePicklistExType (1 to many) search field
//                    //####     NOTE: There are 4 kinds of picklist searches: .cnSingleValuePicklistExType (1 to 1), .cnMultiValuePicklistExType (many to many), .cnSingleValueSearchInMultiValuePicklistExType (1 to many) and cnMultiValueSearchInSingleValuePicklistExType (many to 1). This block covers only .cnSingleValueSearchInMultiValuePicklistExType (1 to many) searches.
//                    //####     NOTE: Since .cnSingleValueSearchInMultiValuePicklistExType == .cnSingleValuePicklistExType | .cnMultiValuePicklistExType, and because we can only do one boolean flag check at a time, the logic below looks for both .cnSingleValuePicklistExType and .cnMultiValuePicklistExType.
//                else if ((iDataType & (int)MetaData.enumDataTypes.cnMultiValuePicklistExType) != 0 &&
//                    (iDataType & (int)MetaData.enumDataTypes.cnSingleValuePicklistExType) != 0
//                ) {
//                    sPicklistData = "SingleValueSearchInMultiValuePicklist";
//                    oReturn = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnRendererSearchForm_SingleValueSearchInMultiValuePicklist, Settings.EndUserMessagesLanguageCode);
//                }
//                    //#### Else if this is a .cnMultiValuePicklistExType (many to many) extended g_iDataType
//                    //####     NOTE: There are 4 kinds of picklist searches: .cnSingleValuePicklistExType (1 to 1), .cnMultiValuePicklistExType (many to many), .cnSingleValueSearchInMultiValuePicklistExType (1 to many) and cnMultiValueSearchInSingleValuePicklistExType (many to 1). This block covers only .cnMultiValuePicklistExType (many to many) searches.
//                    //####     NOTE: Because of the .cnSingleValueSearchInMultiValuePicklistExType (1 to many) test above, we do not need to explicitly exclude .cnSingleValuePicklistExType, as any were caught above.
//                //else if ((g_iDataType & (int)MetaData.enumDataTypes.cnMultiValuePicklistExType) != 0 &&
//                //	(g_iDataType & (int)MetaData.enumDataTypes.cnSingleValuePicklistExType) == 0
//                //) {
//                else if ((iDataType & (int)MetaData.enumDataTypes.cnMultiValuePicklistExType) != 0) {
//                    sPicklistData = "MultiValuePicklist";
//                    oReturn = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnRendererSearchForm_MultiValuePicklist, Settings.EndUserMessagesLanguageCode);
//                }
//                    //#### Else if this is a boolean or a single value picklist extended g_iDataType
//                else if ((iDataType & (int)MetaData.enumDataTypes.cnBoolean) != 0 ||
//                    (iDataType & (int)MetaData.enumDataTypes.cnSingleValuePicklistExType) != 0
//                ) {
//                    sPicklistData = "Boolean";
//                    oReturn = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnRendererSearchForm_SingleValuePicklist, Settings.EndUserMessagesLanguageCode);
//                }
//                    //#### Else if this is a numeric-based g_iDataType
//                else if ((iDataType & (int)MetaData.enumDataTypes.cnInteger) != 0 ||
//                    (iDataType & (int)MetaData.enumDataTypes.cnFloat) != 0 ||
//                    (iDataType & (int)MetaData.enumDataTypes.cnCurrency) != 0
//                ) {
//                    sPicklistData = "Numeric";
//                    oReturn = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnRendererSearchForm_Numeric, Settings.EndUserMessagesLanguageCode);
//                }
//                    //#### Else if this is a date-based sType
//                else if ((iDataType & (int)MetaData.enumDataTypes.cnDateTime) != 0) {
//                    sPicklistData = "DateTime";
//                    oReturn = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnRendererSearchForm_DateTime, Settings.EndUserMessagesLanguageCode);
//                }
//                    //#### Else if this is a short string-based sType
//                else if ((iDataType & (int)MetaData.enumDataTypes.cnChar) != 0) {
//                    sPicklistData = "Char";
//                    bIsStringBased = true;
//                    oReturn = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnRendererSearchForm_Char, Settings.EndUserMessagesLanguageCode);
//                }
//                    //#### Else if this is a long string-based sType
//                else if ((iDataType & (int)MetaData.enumDataTypes.cnLongChar) != 0) {
//                    sPicklistData = "LongChar";
//                    bIsStringBased = true;
//                    oReturn = Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnRendererSearchForm_LongChar, Settings.EndUserMessagesLanguageCode);
//                }
//                    //#### Else if this is a currently unsupported g_iDataType, raise the error
//                else if ((iDataType & (int)MetaData.enumDataTypes.cnBinary) != 0 ||
//                    (iDataType & (int)MetaData.enumDataTypes.cnGUID) != 0
//                ) {
//                    Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnsupportedDataType, sInputAlias, "");
//                }
//                    //#### Else the g_iDataType is .UnknownType or .UnreconizedType, so raise the error
//                else {
//                    Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "DataType", Cn.Data.Tools.MakeString(iDataType, ""));
//                }

//                //##########
//                //##########
				
//                    //#### If the sPicklistData was determined above and the sInputAlias .Exists and .IsNullable (or bIsStringBased), we need to add the .cnGeneral_IsNullStringIsNull option into the oReturn value
//                if (sPicklistData.Length > 0 && base.Exists(sInputAlias) &&
//                    (base.Get(sInputAlias).IsNullable || bIsStringBased)
//                ) {
//                    string[] a_sDescriptionValues;
//                    string[] a_sDataValues;
//                    string[] a_sColumn;
//                    int i;

//                        //#### Borrow the use of i to determine the oReturn value's .RowCount, then dimension a_sDescriptionValues and a_sDataValues accordingly (allowing for the additional row, hence +1)
//                    i = oReturn.RowCount;
//                    a_sDescriptionValues = new string[i + 1];
//                    a_sDataValues = new string[i + 1];

//                        //#### Copy oReturn value's "Description" into a_sColumn, then copy the value into a_sDescriptionValues
//                    a_sColumn = oReturn.Column("Description");
//                    for (i = 0; i < a_sColumn.Length; i++) {
//                        a_sDescriptionValues[i] = a_sColumn[i];
//                    }

//                        //#### Set the .cnGeneral_IsNullStringIsNull description into the last element of the a_sDescriptionValues
//                    a_sDescriptionValues[i] = Cn.Data.Picklists.Decoder(
//                        Web.Settings.Internationalization.Values(Internationalization.enumInternationalizationPicklists.cnRendererSearchForm_IsNullStringIsNull, Settings.EndUserMessagesLanguageCode),
//                        sPicklistData,
//                        false
//                    );

//                        //#### Copy oReturn value's "Data" into a_sColumn, then copy the value into a_sDescriptionValues
//                    a_sColumn = oReturn.Column("Data");
//                    for (i = 0; i < a_sColumn.Length; i++) {
//                        a_sDataValues[i] = a_sColumn[i];
//                    }

//                        //#### Set the int-ified (then .ToString-ified) .cnGeneral_IsNullStringIsNull data into the last element of the a_sDataValues
//                    a_sDataValues[i] = ((int)(enumComparisonTypes.cnGeneral_IsNullStringIsNull)).ToString();

//                        //#### Reset the oReturn value to the above constructed a_sDataValues/a_sDescriptionValues
//                    oReturn = Cn.Data.Picklists.Picklist(a_sDataValues, a_sDescriptionValues);
//                }

//                    //#### Return the above determined oReturn value to the caller
//                return oReturn;
//            }

//            ///############################################################
//            /// <summary>
//            /// Retrieves the data type of the provided Form input.
//            /// </summary>
//            /// <param name="sFunction">String representing the calling function's name.</param>
//            /// <param name="sInputAlias">String representing the Form input to query.</param>
//            /// <returns>Integer representing the data type of the passed <paramref>sInputAlias</paramref>.</returns>
//            /// <exception cref="Cn.CnException">Thrown when the passed <paramref>sInputAlias</paramref> has not be defined.</exception>
//            ///############################################################
//            /// <LastUpdated>August 22, 2007</LastUpdated>
//            private int GetInputDataType(string sFunction, string sInputAlias) {
//                int iReturn = (int)MetaData.enumDataTypes.cnUnknown;

//                    //#### If the passed sInputAlias .Exists within the .Form's .Inputs, set the iReturn value to its .MakeInteger'd .DataType (passing in the defaulted iReturn value as the iDefaultValue)
//                if (base.Exists(sInputAlias)) {
//                    iReturn = Cn.Data.Tools.MakeInteger(base.Get(sInputAlias).DataType, iReturn);
//                }
//                    //#### Else if the passed sInputAlias exists within the g_hCustomFormInputs, set the iReturn value to its .MakeInteger'd .DataType (passing in the defaulted iReturn value as the iDefaultValue)
//                    //####     NOTE: Since CustomInputData's .DataType is an eNum (and not an int as above), we need to cast it as an int before the .MakeInteger call (stupid, I know but that we are guarenteed to at least end up with .cnUnknown)
//                else if (Exists(sInputAlias, enumInputTypes.cnCustomInput)) {
//                    iReturn = Cn.Data.Tools.MakeInteger( (int)((SearchInputCollection.CustomInputData)g_hCustomFormInputs[sInputAlias]).DataType, iReturn);
//                }
//                    //#### Else the passed sInputAlias has not been defined, so raise the error
//                else {
//                    Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnknownInputAlias, sInputAlias, "");
//                }

//                    //#### Return the above determined iReturn value to the caller
//                return iReturn;
//            }

//        } //# public class SearchInputCollection
//        #endregion

//    } //# class SearchForm

//} //# namespace Cn.Web.Renderer
