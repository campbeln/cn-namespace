/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/


namespace Cn.Data.SQL {

    ///########################################################################################################################
    /// <summary>
	/// Represents a SQL Order By clause as an object.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>August 12, 2005</LastFullCodeReview>
	public class OrderByClause {
            //#### Declare the required private variables
		private string[] ga_sColumns;
		private bool[] ga_bSortAscending;
		private bool[] ga_bEnabled;
		private int g_iColumnCount;

            //#### Declare the required private constants
	  //private const string g_cClassName = "Cn.Data.OrderByClause.";


        //##########################################################################################
        //# Class Functions
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public OrderByClause() {
                //#### Pass the call off to .Reset
			Reset("");
		}

        ///############################################################
        /// <summary>
		/// Resets the class to its initilized state.
        /// </summary>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public void Reset() {
                //#### Pass the call off to our sibling implementation
			Reset("");
		}

		///############################################################
		/// <summary>
		/// Initializes the class based on the provided Order By clause.
		/// </summary>
		/// <param name="sOrderByClause">String representing a SQL Order By clause.</param>
		///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public OrderByClause(string sOrderByClause) {
                //#### Pass the call off to .Reset
			Reset(sOrderByClause);
		}

        ///############################################################
        /// <summary>
		/// Resets the class to its initilized state.
        /// </summary>
 		/// <param name="sOrderByClause">String representing a SQL Order By clause.</param>
       ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public void Reset(string sOrderByClause) {
                //#### (Re)Init the local variables
			ga_sColumns = null;
			ga_bSortAscending = null;
			ga_bEnabled = null;
			g_iColumnCount = 0;

                //#### If the caller passed in a sOrderByClause to pre-process
			if (sOrderByClause.Length > 0) {
                    //#### Attempt to .Load the sOrderByClause, disreguarding the return value as a constructor cannot return a value
				Load(sOrderByClause);
			}
		}


        //##########################################################################################
        //# Public Read-Only Properties
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the column count within this instance.
		/// </summary>
		/// <value>1-based integer representing the count of columns within this instance.</value>
		///############################################################
		/// <LastUpdated>September 13, 2005</LastUpdated>
		public int ColumnCount {
			get { return g_iColumnCount; }
		}


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Loads the provided Order By clause into this instance.
        /// </summary>
		/// <param name="sOrderByClause">String representing a SQL Order By clause.</param>
		/// <returns>Boolean value signaling if the data load was a success.<para/>Returns true if the passed <paramref>sOrderByClause</paramref> was successfully loaded, or false if it was not.</returns>
        ///############################################################
		/// <LastUpdated>September 13, 2005</LastUpdated>
		public bool Load(string sOrderByClause) {
			string[] a_sStatement;
			string[] a_sClause;
			string sTemp;
			int iStatementLen;
			int iClauseLen;
			int i;
			bool bReturn = true;

                //#### .Normalize the passed sOrderByClause
			sOrderByClause = Data.Tools.Normalize(sOrderByClause);

				//#### If the sOrderByClause is holding a value to parse
			if (sOrderByClause.Length > 0) {
					//#### If the passed sOrderByClause does not contain any potentially malicious data
				if (Statements.IsUserDataSafe(sOrderByClause)) {
						//#### Pull the passed sOrderByClause apart into its individual a_sClauses
					a_sClause = sOrderByClause.Split(',');

						//#### Determine the iClauseLen of a_sClause, .Set(ting our own)ColumnCount accordingly (and in turn ReDim'ing the global arrays)
					iClauseLen = a_sClause.Length;
					SetCount(iClauseLen);

						//#### Traverse each a_sStatement within a_sClause
					for (i = 0; i < iClauseLen; i++) {
							//#### .Trim and .Split the current a_sClause into a_sStatement, determine its .Length and set the current ga_bEnabled
						a_sStatement = a_sClause[i].Trim().Split(' ');
						iStatementLen = a_sStatement.Length;
						ga_bEnabled[i] = true;

							//#### If only a single column was defined
						if (iStatementLen == 1) {
								//#### Set the column name from the a_sStatement and the default sort direction
							ga_sColumns[i] = a_sStatement[0];
							ga_bSortAscending[i] = true;
						}
							//#### Else if a column name and sort direction was defined
						else if (iStatementLen == 2) {
								//#### Set the column name from the a_sStatement and borrow the use of sCurrentAttribute to store the lowercased sort direction
							ga_sColumns[i] = a_sStatement[0];
							sTemp = a_sStatement[1].ToLower();

								//#### If the sort direction is ascending, set ga_bSortAscending accordingly
								//####     NOTE: A switch is not used here due to the use of the break; statement below to get out of the for loop
							if (sTemp == "asc" || sTemp == "ascending") {
								ga_bSortAscending[i] = true;
							}
								//#### If the sort direction is descending, set ga_bSortAscending accordingly
							else if (sTemp == "desc" || sTemp == "descending") {
								ga_bSortAscending[i] = false;
							}
								//#### Else the second element of the a_sStatement was unreconized, making the entire a_sStatement malformed
							else {
									//#### Set the bReturn value to false and exit the loop
								bReturn = false;
								break;
							}
						}
							//#### Else the a_sStatement had too many elements to be properly formed
						else {
								//#### Set the return value to false and exit the loop
							bReturn = false;
							break;
						}
					}
				}
					//#### Else the passed sOrderByClause did contain some potentially malicious data, so return false
				else {
					bReturn = false;
				}
			}
				//#### Else the sOrderByClause is not holding a value, so flip the bReturn value to false so we are .Reset below
			else {
				bReturn = false;
			}

                //#### If we encountered errors while Loading (or there was nothing to parse in) the passed sOrderByClause, so .Reset the class
			if (! bReturn) {
				Reset();
			}

			    //#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Gets/sets the column name string value present at the referenced index.
		/// </summary>
		/// <param name="iIndex">0-based integer representing the desired column name index.</param>
		/// <returns>String representing the column name of the passed <paramref>iIndex</paramref>.</returns>
		///############################################################
		/// <LastUpdated>November 4, 2005</LastUpdated>
		public string ColumnName(int iIndex) {
			return ga_sColumns[iIndex];
		}

		///############################################################
		/// <summary>
		/// Gets/sets the column name string value present at the referenced index.
		/// </summary>
		/// <param name="iIndex">0-based integer representing the desired column name index.</param>
		/// <param name="sNewValue">String representing the new value for the referenced index.</param>
		///############################################################
		/// <LastUpdated>November 4, 2005</LastUpdated>
		public void RenameColumn(int iIndex, string sNewValue) {
			ga_sColumns[iIndex] = sNewValue;
		}

		///############################################################
		/// <summary>
		/// Gets/sets the sort ascending boolean value present at the referenced index (pseudo-parameterized property).
		/// </summary>
		/// <remarks>
		/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
		/// </remarks>
		/// <param name="iIndex">0-based integer representing the desired sort ascending index.</param>
		/// <value>Boolean value signaling the if the column at the passed <paramref>iIndex</paramref> is sorted in ascending or descending order.<para/>Returns true if the column is sorted in ascending order, or false if it is sorted in descending order.</value>
		///############################################################
		/// <LastUpdated>November 4, 2005</LastUpdated>
		public bool SortAscending(int iIndex) {
			return ga_bSortAscending[iIndex];
		}

		///############################################################
		/// <summary>
		/// Gets/sets the sort ascending boolean value present at the referenced index (pseudo-parameterized property).
		/// </summary>
		/// <remarks>
		/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
		/// </remarks>
		/// <param name="iIndex">0-based integer representing the desired sort ascending index.</param>
		/// <param name="bNewValue">Boolean value representing the new value for the referenced index.</param>
		///############################################################
		/// <LastUpdated>November 4, 2005</LastUpdated>
		public void SortAscending(int iIndex, bool bNewValue) {
			ga_bSortAscending[iIndex] = bNewValue;
		}

		///############################################################
		/// <summary>
		/// Gets/sets the column enabled boolean value present at the referenced index.
		/// </summary>
		/// <param name="iIndex">0-based integer representing the desired enabled index.</param>
		/// <value>Boolean value signaling the if the column at the passed <paramref>iIndex</paramref> is enabled.<para/>Returns true if the column is enabled, or false if it is not.</value>
		///############################################################
		/// <LastUpdated>November 4, 2005</LastUpdated>
		public bool Enabled(int iIndex) {
			return ga_bEnabled[iIndex];
		}

		///############################################################
		/// <summary>
		/// Gets/sets the column enabled boolean value present at the referenced index.
		/// </summary>
		/// <param name="iIndex">0-based integer representing the desired enabled index.</param>
		/// <param name="bNewValue">Boolean value representing the new value for the referenced index.</param>
		///############################################################
		/// <LastUpdated>November 4, 2005</LastUpdated>
		public void Enable(int iIndex, bool bNewValue) {
			ga_bEnabled[iIndex] = bNewValue;
		}

        ///############################################################
        /// <summary>
        /// Enables the referenced columns that are present within this instance.
        /// </summary>
		/// <param name="sEnabledColumns">String representing the comma delimited column names to enable.</param>
        ///############################################################
		/// <LastUpdated>August 12, 2005</LastUpdated>
		public void EnableOnly(string sEnabledColumns) {
			string[] a_sEnabledColumns;
			string sCurrentColumn;
			int iCurrentLen;
			int i;
			int j;

                //#### Traverse the ga_bEnabled array, disabling each as we go
			for (i = 0; i < g_iColumnCount; i++) {
				ga_bEnabled[i] = false;
			}

                //#### If the caller passed in a list of sEnabledColumns to process
			if (sEnabledColumns.Length > 0) {
                    //#### Lowercase, .Normalize and .Split the passed sEnabledColumns into a_sEnabledColumns
				a_sEnabledColumns = Data.Tools.Normalize(sEnabledColumns.ToLower()).Split(',');

                    //#### Traverse a_sEnabledColumns
				for (i = 0; i < a_sEnabledColumns.Length; i++) {
                        //#### .Trim and lowercase the sCurrentColumn value and determine its .Length in prep. of the comparisons below
					sCurrentColumn = a_sEnabledColumns[i].Trim().ToLower();
					iCurrentLen = a_sEnabledColumns[i].Length;

                        //#### Traverse the .Length of the global arrays
					for (j = 0; j < g_iColumnCount; j++) {
                            //#### If we've not already enabled this ga_sColumn
						if (! ga_bEnabled[j]) {
                                //#### If the current ga_sColumn matches the sCurrentColumn (checking their .Lengths first as it is a far faster comparison)
							if (ga_sColumns[j].Length == iCurrentLen && ga_sColumns[j].ToLower() == sCurrentColumn) {
                                    //#### Enable the ga_sColumn and exit the inner for loop
								ga_bEnabled[j] = true;
								break;
							}
						}
					}
				}
			}
		}

        ///############################################################
        /// <summary>
        /// Re-enables all of the columns present within this instance.
        /// </summary>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public void EnableAll() {
			int i;

                //#### Traverse the ga_bEnabled array, resetting each ga_bEnabled entry to true
			for (i = 0; i < g_iColumnCount; i++) {
				ga_bEnabled[i] = true;
			}
		}

        ///############################################################
        /// <summary>
        /// Inverts the current sort ordering of this instance.
        /// </summary>
        ///############################################################
		/// <LastUpdated>May 25, 2004</LastUpdated>
		public void InvertSortOrder() {
			int i;

                //#### Traverse the ga_bSortAscending array, inverting each ga_bSortAscending entry
			for (i = 0; i < g_iColumnCount; i++) {
				ga_bSortAscending[i] = (! ga_bSortAscending[i]);
			}
		}

        ///############################################################
        /// <summary>
        /// Retrieves a properly formatted SQL Order By clause based on the current instance's <c>Enabled</c> columns.
        /// </summary>
        /// <returns>String containing a SQL Order By clause based on the current instance, or a null-string if no clause has been defined.</returns>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public override string ToString() {
        		//#### Pass the call off to our sibling implementation
        	return ToString(false, true);
        }

        ///############################################################
        /// <summary>
        /// Retrieves a properly formatted SQL Order By clause based on the current instance.
        /// </summary>
        /// <param name="bReturnAllColumns">Boolean value signaling if all of columns present within this instance are to be returned.<para/>True returns all columns present within this instance, false returns only those columns currently set as <c>Enabled</c>.</param>
        /// <param name="bIncludeOrderBy">Boolean value signaling if the "ORDER BY" keywords are to be included within the return value.</param>
        /// <returns>String containing a SQL Order By clause based on the current instance, or a null-string if no clause has been defined.</returns>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public string ToString(bool bReturnAllColumns, bool bIncludeOrderBy) {
		    string sReturn = "";
			int i;

                //#### Traverse the .Length of the global arrays
			for (i = 0; i < g_iColumnCount; i++) {
                    //#### If we're supposed to bReturnAllColumns or if this entry is ga_bEnabled
				if (bReturnAllColumns || ga_bEnabled[i]) {
                        //#### If this entry is to be sorted in ascending order
					if (ga_bSortAscending[i]) {
                            //#### Add the g_aColumns onto the return value
						sReturn += ga_sColumns[i] + ",";
					}
                        //#### Else this entry is to be sorted in decending order
					else {
                            //#### Add the g_aColumns followed by the DESCending keyword onto the return value
						sReturn += ga_sColumns[i] + " DESC,";
					}
				}
			}

                //#### If an OrderByClause was constructed above
			i = sReturn.Length;
			if (i > 0) {
					//#### Peal off the trailing comma from our sReturn value
				sReturn = sReturn.Substring(0, i - 1);

					//#### If we are to bIncludeOrderBy, prepend it now
				if (bIncludeOrderBy) {
					sReturn = "ORDER BY " + sReturn;
				}
			}

			    //#### Return the above determined sReturn value to the caller
			return sReturn;
		}


		//##########################################################################################
		//# Private Functions
		//##########################################################################################
        ///############################################################
		/// <summary>
		/// Redimensions the global arrays to the referenced dimension count.
		/// </summary>
		/// <param name="iColumnCount">1-based integer representing the desired array element count.</param>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		private void SetCount(int iColumnCount) {
				//#### Reset the global g_iColumnCount to the passed iColumnCount
			g_iColumnCount = iColumnCount;

				//#### Reset the global arrays to the new g_iColumnCount
			ga_sColumns = new string[g_iColumnCount];
			ga_bEnabled = new bool[g_iColumnCount];
			ga_bSortAscending = new bool[g_iColumnCount];
		}

	} //# class OrderByClause

} //# namespace Cn.Data.SQL
