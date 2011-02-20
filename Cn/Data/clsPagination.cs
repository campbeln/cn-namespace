/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;                                       //# Required to access the Math class
using System.Runtime.Serialization;					//# Required to access ISerializable
using Cn.Data.SQL;									//# Required to access the OrderByClause class
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Data {

	///########################################################################################################################
	/// <summary>
	/// Enables data source independant pagination logic utilizing a collection of 'ResultsStacks', each of which tracks a single 'tables' collection of 'IDs'.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>August 16, 2005</LastFullCodeReview>
	[Serializable]
	public class Pagination : ISerializable {
	#region Pagination
            //#### Declare the required private variables
		private PaginationTable[] ga_oTables;
		private PaginationTable g_oNewRecords;
		private OrderByClause g_oOrderedBy;
		private int g_iStartRecord;

            //#### Declare the required private constants
		private const string g_cClassName = "Cn.Data.Pagination.";


        //##########################################################################################
        //# Class Functions
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public Pagination() {
                //#### Call .Reset to init the class vars
			Reset("");
		}

        ///############################################################
        /// <summary>
		/// Resets the class to its initilized state.
        /// </summary>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public void Reset() {
                //#### Call .Reset to init the class vars
			Reset("");
		}

		///############################################################
		/// <summary>
		/// Initializes the class based on the provided structure.
		/// </summary>
		/// <param name="sResultsStack">String representing an instance of a Pagination class.</param>
		///############################################################
		/// <LastUpdated>December 21, 2009</LastUpdated>
		public Pagination(string sResultsStack) {
                //#### Call .Reset to init the class vars
			Reset(sResultsStack);
		}

        ///############################################################
        /// <summary>
		/// Resets the class based on the provided structure.
        /// </summary>
		/// <param name="sResultsStack">String representing an instance of a Pagination class.</param>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public void Reset(string sResultsStack) {
                //#### (Re)Init the local variables
			ga_oTables = null;
			g_oNewRecords = null;
			g_oOrderedBy = new OrderByClause();
			g_iStartRecord = 1;

                //#### If the caller passed in a sResultsStack to pre-process
			if (sResultsStack.Length > 0) {
                    //#### Attempt to .Load the sResultsStack, disreguarding the return value (as a constructor cannot return a value)
				Load(sResultsStack);
			}
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
		public Pagination(SerializationInfo info, StreamingContext ctxt) {
				//#### Call .DoReset to init the class vars
		    Reset(Data.Tools.MakeString(info.GetValue("ResultsStack", typeof(string)), ""));
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
			info.AddValue("ResultsStack", ToString());
		}


        //##########################################################################################
        //# Public Read/Write Properties
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the starting record number for the 'results window' that this instance represents.
		/// <para/>This functionality allows the developer to 'window' thru a large collection of 'tables'/'IDs' as smaller sets. It is up to the developer to utilize this property in conjunction with the <c>IDCount</c> property to determine if there are results on either side of the current 'window'.
		/// </summary>
		/// <value>1-based integer representing the starting record number within this instance.</value>
		/// <exception cref="Cn.CnException">Thrown when the passed integer is less then or equal to zero.</exception>
		/// <seealso cref="Cn.Data.Pagination.IDCount"/>
		///############################################################
		/// <LastUpdated>September 1, 2004</LastUpdated>
		public int StartRecord {
			get { return g_iStartRecord; }
			set {
                    //#### If the caller passed in a positive number, set the new g_iStartRecord
				if (value > 0) {
					g_iStartRecord = value;
				}
                    //#### Else the caller passed in a negetive number (or 0), so raise the error
				else {
					Internationalization.RaiseDefaultError(g_cClassName + "StartRecord", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_PositiveIntegerRequired, "StartRecord", "1");
				}
			}
		}


        //##########################################################################################
        //# Public Read-Only Properties
        //##########################################################################################

public PaginationTable NewRecords {
	get {
		if (g_oNewRecords == null) { g_oNewRecords = new PaginationTable(); }
		return g_oNewRecords;
	}
}

public int ErroredIDCount {
	get {
	    int iReturn = 0;
		int i;
		int j;

            //#### If there currently DataIsLoaded into our class
		if (DataIsLoaded) {
                //#### Traverse each element of the ga_oTables
			for (i = 0; i < ga_oTables.Length; i++) {
					//#### Traverse each of the .IDs within the current ga_oTables
				for (j = 0; j < ga_oTables[i].IDCount; j++) {
						//#### If the current .IDs is set as an .ErroredID, inc our iReturn value
					if (ga_oTables[i].ErroredID(i)) {
						iReturn++;
					}
				}
			}
		}

		    //#### Return the above determined iReturn value to the caller
		return iReturn;
	}
}


		///############################################################
		/// <summary>
		/// Gets the SQL Order By Clause used to order the 'IDs' within the this instance.
		/// </summary>
		/// <value>OrderByClause class reference representing the 'Order By' clause used to order the 'IDs' within this instance.</value>
		///############################################################
		/// <LastUpdated>May 11, 2004</LastUpdated>
		public OrderByClause OrderedBy {
			get { return g_oOrderedBy; }
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating if data has been loaded into this instance.
		/// </summary>
		/// <value>Boolean value signaling the presence of data within this instance.<para/>Returns true if data has been loaded within this instance, or false if no data is present.</value>
		///############################################################
		/// <LastUpdated>August 16, 2005</LastUpdated>
		public bool DataIsLoaded {
			get {
				return (ga_oTables != null && ga_oTables.Length > 0);
			}
		}

		///############################################################
		/// <summary>
		/// Gets the total count of 'tables' within this instance.
		/// </summary>
		/// <value>1-based integer representing the count of 'tables' within this instance.</value>
		///############################################################
		/// <LastUpdated>August 16, 2005</LastUpdated>
		public int TableCount {
			get {
			    int iReturn;

                    //#### If there currently DataIsLoaded into our class
				if (DataIsLoaded) {
                        //#### Set our iReturn value to the count (.Length) of tables held within the ga_oTables
					iReturn = ga_oTables.Length;
				}
                    //#### Else there Is(no)DataLoaded, so set our iReturn value to 0
				else {
					iReturn = 0;
				}

				    //#### Return the above determined iReturn value to the caller
				return iReturn;
			}
		}

		///############################################################
		/// <summary>
		/// Gets the total count of 'IDs' within this instance.
		/// </summary>
		/// <value>1-based integer representing the count of 'IDs' within this instance.</value>
		///############################################################
		/// <LastUpdated>August 16, 2005</LastUpdated>
		public int IDCount {
			get {
			    int iReturn = 0;
				int i;

                    //#### If there currently DataIsLoaded into our class
				if (DataIsLoaded) {
                        //#### Traverse each element of the ga_oTables
					for (i = 0; i < ga_oTables.Length; i++) {
                            //#### If the current results entry has data loaded
						if (ga_oTables[i].IDCount > 0) {
                                //#### Add the .Length of the .IDs onto the iReturn value
							iReturn += ga_oTables[i].IDs.Length;
						}
					}
				}

				    //#### Return the above determined iReturn value to the caller
				return iReturn;
			}
		}


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Loads the provided pagination data into this instance.
        /// </summary>
        /// <remarks>
        /// A properly formed string representation of a Pagination class looks something like this:
        /// <para/>OrderByClause:StartRecord|TableName1:IDColumn1:ID1:ID2:ID3|TableName2:IDColumn2:ID4:ID5
        /// <para/>Where:
        /// <para/>'|' represents the developer defined 'PrimaryDelimiter'.
        /// <para/>':' represents the developer defined 'SecondaryDelimiter'.
        /// <para/>'OrderByClause' represents the SQL compliant ordering statement.
        /// <para/>'StartRecord' represents the 1-based positive interger indicating the starting record number for this 'results window'.
        /// <para/>'TableName1'/'TableName2' represent 'table names' which signal to the developer the source of their associated 'IDs'. This value does not need to match the actual 'table name' within the underlying data source. This value is simply a signal to the developer indicating which underlying structure to use.
        /// <para/>'IDColumn1'/'IDColumn2' represent 'ID column names' which signal to the developer where the associated 'IDs' are held. This value does not need to match the actual 'ID column name' within the underlying data source. This value is simply a signal to the developer indicating which underlying structure to use.
        /// <para/>'ID1'...'ID5' represent the unique 'IDs' that individually identify a specific record within the associated 'table'.
        /// <para/>NOTE: Each 'TableName:IDColumn' pair must be unique within an instance. This data is used to uniquely identify each collection of 'IDs'.
        /// <para/>NOTE: Since this function is "foward facing" (meaning it deals with unscrubbed, user supplied data), errors are not raised. This function simply returns false if the passed <paramref>sResultsStack</paramref> is malformed.
        /// </remarks>
		/// <param name="sResultsStack">String representing an instance of a Pagination class.</param>
		/// <returns>Boolean value signaling if the data load was a success.<para/>Returns true if the passed <paramref>sResultsStack</paramref> was successfully loaded, or false if it was not.</returns>
        ///############################################################
		/// <LastUpdated>January 12, 2006</LastUpdated>
		public bool Load(string sResultsStack) {
			string[] a_sSecondary;
			string[] a_sPrimary;
			int iSecondaryLen;
			int iPrimaryLen;
			int i;
			int j;
			bool bReturn;

//! Add delimiter removal logic here as it's been removed from .IsUserDataSafe?

                //#### If the passed sResultsStack does not contain any potentially malicious data
			if (Statements.IsUserDataSafe(sResultsStack)) {
                    //#### Reset the bReturn value to true (as any parsing errors below will reset it to false)
				bReturn = true;

                    //#### Split apart the passed sResultsStack in prep. for the parsing below, borrowing the use of a_sSecondary to store the metadata info
                    //####     NOTE: A null-string in sResultsStack is caught in the .Length tests below
				a_sPrimary = sResultsStack.Split(Settings.PrimaryDelimiter.ToCharArray());
				a_sSecondary = a_sPrimary[0].Split(Settings.SecondaryDelimiter.ToCharArray());

                    //#### Determine the .Length of a_sPrimary
				iPrimaryLen = a_sPrimary.Length;

                    //#### If a_sPrimary has at least 1 results entry (2 elements) and the borrowed a_sSecondary has 2 elements
				if (iPrimaryLen > 1 && a_sSecondary.Length == 2) {
                        //#### Reset ga_oTables accordingly (-1 as the first element in a_sPrimary is metadata)
					ga_oTables = new PaginationTable[iPrimaryLen - 1];

                        //#### Set g_iStartRecord, fixing its value is necessary
					g_iStartRecord = Data.Tools.MakeInteger(a_sSecondary[1], 1);
					if (g_iStartRecord < 1) {
						g_iStartRecord = 1;
					}

                        //#### If the sResultsStack contains an OrderByClause, load it now
					if (a_sSecondary[0].Trim().Length > 0) {
						g_oOrderedBy.Load(a_sSecondary[0]);
					}

                        //#### Traverse the individual results entries within a_sPrimary (starting at index 1 as index 0 is metadata)
					for (i = 1; i < iPrimaryLen; i++) {
                            //#### .Split the current a_sPrimary entry into a_sSecondary and determine it's .Length
						a_sSecondary = a_sPrimary[i].Split(Settings.SecondaryDelimiter.ToCharArray());
						iSecondaryLen = a_sSecondary.Length;

                            //#### If the results entry at least has the "Table:IDColumn" metadata (IDs are not required)
						if (iSecondaryLen > 1) {
                                //#### Create the PaginationTable object for this index
							ga_oTables[i - 1] = new PaginationTable();

                                //#### GetData the .TableName and .IDColumn of this result entry
							ga_oTables[i - 1].TableName = a_sSecondary[0];
							ga_oTables[i - 1].IDColumn = a_sSecondary[1];

                                //#### If at least 1 ID was defined
							if (iSecondaryLen > 2) {
                                    //#### Init the internal aIDs array of this result entry (subtracting 2 as the first 2 elements of a_sSecondary are metadata)
                                ga_oTables[i - 1].IDs = new string[iSecondaryLen - 2];

                                    //#### Copy the IDs from a_sSecondary into .IDs (skipping the first 2 elements as they are metadata)
                                for (j = 2; j < iSecondaryLen; j++) {
                                    ga_oTables[i - 1].IDs[j - 2] = a_sSecondary[j];
                                }
							}
                                //#### Else no .IDs were defiend, so explicitly set the .IDs to null
							else {
								ga_oTables[i - 1].IDs = null;
							}
						}
                            //#### Else the current results entry is malformed, so set our bReturn value to false and exit the loop
						else {
							bReturn = false;
							break;
						}
					}
				}
                    //#### Else the passed sResultsStack was malformed, so set our bReturn value to false
				else {
					bReturn = false;
				}
			}
                //#### Else the passed sResultsStack contained some potentially malicious data, so set our bReturn value to false
			else {
				bReturn = false;
			}

                //#### If we encountered errors while Loading the passed sResultsStack, .Reset the class
			if (! bReturn) {
				Reset();
			}

			    //#### Return the above determined bReturn value to the caller
			return bReturn;
		}

        ///############################################################
        /// <summary>
        /// Loads the provided pagination data into this instance.
        /// </summary>
        /// <param name="sTableName">String representing the 'table name' within the underlying data source.</param>
        /// <param name="sIDColumn">String representing the 'ID column name' for the passed <paramref>sTableName</paramref> within the underlying data source.</param>
        /// <param name="sID">String representing the unique value that individually identifies a specific record.</param>
        /// <remarks>
        /// If the passed <paramref>sTableName</paramref>/<paramref>sIDColumn</paramref> pair is already present within this instance, its 'IDs' collection is replaced by the passed <paramref>sID</paramref> (e.g. any current 'IDs' for the <paramref>sTableName</paramref>/<paramref>sIDColumn</paramref> pair are lost).
		/// <para/>NOTE: Both <paramref>sTableName</paramref> and <paramref>sIDColumn</paramref> are compared as case insensitive.
        /// <para/>NOTE: Each <paramref>sTableName</paramref>/<paramref>sIDColumn</paramref> pair must be unique within an instance. This data is used to uniquely identify each collection of 'IDs'.
        /// </remarks>
        ///############################################################
		/// <LastUpdated>June 27, 2006</LastUpdated>
		public void Load(string sTableName, string sIDColumn, string sID) {
			string[] a_sIDs = new string[1];

                //#### Copy the passed sID into the local single element a_sIDs array and pass it off to sibling implementation
			a_sIDs[0] = sID;
			Load(sTableName, sIDColumn, a_sIDs);
		}

        ///############################################################
        /// <summary>
        /// Loads the provided pagination data into this instance.
        /// </summary>
        /// <param name="sTableName">String representing the 'table name' within the underlying data source.</param>
        /// <param name="sIDColumn">String representing the 'ID column name' for the passed <paramref>sTableName</paramref> within the underlying data source.</param>
        /// <param name="a_sIDs">String array representing the unique values that individually identify a specific records.</param>
        /// <remarks>
        /// If the passed <paramref>sTableName</paramref>/<paramref>sIDColumn</paramref> pair is already present within this instance, its 'IDs' collection is replaced by the passed <paramref>a_sIDs</paramref> (e.g. any current 'IDs' for the <paramref>sTableName</paramref>/<paramref>sIDColumn</paramref> pair are lost).
		/// <para/>NOTE: Both <paramref>sTableName</paramref> and <paramref>sIDColumn</paramref> are compared as case insensitive.
        /// <para/>NOTE: Each <paramref>sTableName</paramref>/<paramref>sIDColumn</paramref> pair must be unique within an instance. This data is used to uniquely identify each collection of 'IDs'.
		/// </remarks>
        ///############################################################
		/// <LastUpdated>June 27, 2006</LastUpdated>
		public void Load(string sTableName, string sIDColumn, string[] a_sIDs) {
			int iLogicalUBound;
			int iIndex;

                //#### Determine the iIndex of the passed sTableName/sIDColumn pair (if any)
			iIndex = FindIndex(sTableName, sIDColumn);

                //#### If an iIndex for the passed sTableName/sIDColumn pair was found above
			if (iIndex > -1) {
			    int iIDLen;

                    //#### If the located results entry has data loaded, determine its .IDs .Length
				if (ga_oTables[iIndex].IDCount > 0) {
					iIDLen = ga_oTables[iIndex].IDs.Length;
				}
                    //#### Else the located results entry does not have data loaded, so set iIDLen to 0
				else {
					iIDLen = 0;
				}

                    //#### If the passed a_sIDs is null or is otherwise empty
				if (a_sIDs == null || a_sIDs.Length == 0) {
                        //#### Set .IDs to the same and set .MissingIDCount to the value of iIDLen (as there are now that many less .IDs)
                        //####     NOTE: .MissingIDCount refers to the number of IDs "lost" at events like a reordering of the results, so the below logic is indeed correct. Positive numbers represent "added" IDs, negetive numbers represent "lost" IDs
					ga_oTables[iIndex].IDs = null;
					ga_oTables[iIndex].MissingIDCount = iIDLen;
				}
                    //#### Else the caller passed in a valid a_sIDs array
				else {
                        //#### Borrow the use of iLogicalUBound to store the UBound of the passed aIDs
					iLogicalUBound = a_sIDs.Length;

                        //#### Replace the sTableName/sIDColumn pair's current .IDs with the passed a_sIDs
                    ga_oTables[iIndex].IDs = new string[iLogicalUBound];
					a_sIDs.CopyTo(ga_oTables[iIndex].IDs, 0);

                        //#### Set .MissingIDCount based on the above regenerated results
                        //####     NOTE: .MissingIDCount refers to the number of IDs "lost" at events like a reordering of the results, so the below logic is indeed correct. Positive numbers represent "added" IDs, negetive numbers represent "lost" IDs
					ga_oTables[iIndex].MissingIDCount = (iIDLen - iLogicalUBound);
				}
			}
                //#### Else we need to add in the new sTableName/sIDColumn pair
			else {
                    //#### If there is already some data loaded
				if (DataIsLoaded) {
				    PaginationTable[] oTempResultsArray;
				    int i;

                        //#### Determine the iLogicalUBound of the to be expanded ga_oTables, then init the oTempResultsArray to fit the new entry
					iLogicalUBound = ga_oTables.Length;
					oTempResultsArray = new PaginationTable[iLogicalUBound + 1];

					    //#### Traverse the ga_oTables, shallow copying the ga_oTables into the local oTempResultsArray as we go
					for (i = 0; i < iLogicalUBound; i++) {
					    oTempResultsArray[i] = ga_oTables[i];
					}

                        //#### Finish the "Redim Preserve" by copying the oTempResultsArray back into ga_oTables (including the new entry at the tail of the array)
                    ga_oTables = oTempResultsArray;
				}
                    //#### Else this is the initial .Load
				else {
                        //#### Set the iLogicalUBound to 0 and init ga_oTables to fit its first entry
					iLogicalUBound = 0;
					ga_oTables = new PaginationTable[iLogicalUBound + 1];
				}

                    //#### Create the new PaginationTable entry at the iLogicalUBound of the ga_oTables
				ga_oTables[iLogicalUBound] = new PaginationTable();

                    //#### Fill the new ga_oTables entry with the passed sTableName/sIDColumn, and set its .MissingIDCount to 0
				ga_oTables[iLogicalUBound].TableName = sTableName;
				ga_oTables[iLogicalUBound].IDColumn = sIDColumn;
				ga_oTables[iLogicalUBound].MissingIDCount = 0;

                    //#### If the passed a_sIDs is null or is otherwise empty, set .IDs to the same
				if (a_sIDs == null || a_sIDs.Length == 0) {
					ga_oTables[iLogicalUBound].IDs = null;
				}
                    //#### Else the caller passed in a valid a_sIDs array, so copy them into the .IDs
				else {
					ga_oTables[iLogicalUBound].IDs = new string[a_sIDs.Length];
					a_sIDs.CopyTo(ga_oTables[iLogicalUBound].IDs, 0);
				}
			}
		}

		///############################################################
		/// <summary>
		/// Converts the data stored within this instance into its equivalent string representation.
		/// </summary>
		/// <returns>String representing this instance of a Pagaination class.</returns>
		///############################################################
		/// <LastUpdated>September 7, 2005</LastUpdated>
		public override string ToString() {
		    string sReturn = "";
			int i;

                //#### If there currently DataIsLoaded into our class
			if (DataIsLoaded) {
                    //#### Init the sReturn value with the header metadata
				sReturn = g_oOrderedBy.ToString(true, false) + Settings.SecondaryDelimiter + g_iStartRecord;

                    //#### Traverse the ga_oTables
				for (i = 0; i < ga_oTables.Length; i++) {
                        //#### If the current results entry has data loaded, append the Table/IDColumn metadata and the .IDs onto the return value
					if (ga_oTables[i].IDCount > 0) {
						sReturn += Settings.PrimaryDelimiter + ga_oTables[i].TableName + Settings.SecondaryDelimiter + ga_oTables[i].IDColumn + Settings.SecondaryDelimiter + string.Join(Settings.SecondaryDelimiter, ga_oTables[i].IDs);
					}
                        //#### Else the current results entry doesn't have any data loaded, so only append the Table/IDColumn metadata onto the return value
					else {
						sReturn += Settings.PrimaryDelimiter + ga_oTables[i].TableName + Settings.SecondaryDelimiter + ga_oTables[i].IDColumn;
					}
				}
			}

                //#### Return the above determined sReturn value to the caller
            return sReturn;
		}

		///############################################################
		/// <summary>
		/// Returns a structure that represents the stack element at the passed index.
		/// </summary>
		/// <param name="iIndex">0-based integer representing the desired 'table' element index.</param>
		/// <returns>PaginationTable class representing the 'table name', 'ID column name' and 'IDs' collection of the passed <paramref>iIndex</paramref>.</returns>
		///############################################################
		/// <LastUpdated>June 18, 2010</LastUpdated>
		public PaginationTable Table(int iIndex) {
		    PaginationTable oReturn = null;

                //#### If there currently DataIsLoaded into our class
			if (DataIsLoaded) {
					//#### If the passed iIndex is valid, so set our oReturn value to the passed iIndex
				if (iIndex >= 0 && iIndex < ga_oTables.Length) {
					oReturn = ga_oTables[iIndex];
				}
			}
				//#### Else if the caller is requesting the g_oNewRecords, reset our oReturn value accordingly
			else if (iIndex == -1) {
				oReturn = g_oNewRecords;
			}

			    //#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Returns a structure that represents the stack element at the passed index.
		/// </summary>
        /// <param name="sTableName">String representing the 'table name' within the underlying data source.</param>
        /// <param name="sIDColumn">String representing the 'ID column name' for the passed <paramref>sTableName</paramref> within the underlying data source.</param>
		/// <returns>PaginationTable class representing the 'table name', 'ID column name' and 'IDs' collection of the passed <paramref>iIndex</paramref>.</returns>
		///############################################################
		/// <LastUpdated>April 21, 2010</LastUpdated>
		public PaginationTable Table(string sTableName, string sIDColumn) {
				//#### Pass the call off to our sibling implementation, .Find('ing the)Index of the passed sTableName/sIDColumn as we go
			return Table(FindIndex(sTableName, sIDColumn));
		}

		///############################################################
		/// <summary>
		/// Retrieves a structure that represents the referenced range of 'IDs'.
		/// </summary>
		/// <param name="iStartRecord">1-based integer representing the starting 'ID' index.</param>
		/// <param name="iMaxReturnIDs">1-based integer representing the maximum number of 'IDs' to return.</param>
		/// <returns>Pagination class that represents the range of 'IDs' beginning with the passed <paramref>iStartRecord</paramref> up to a total of <paramref>iMaxReturnIDs</paramref>.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iStartRecord</paramref> is outside the proper range (greater then or equal to zero).</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iMaxReturnIDs</paramref> is outside the proper range (greater then zero).</exception>
		///############################################################
		/// <LastUpdated>August 23, 2005</LastUpdated>
		public Pagination Range(int iStartRecord, int iMaxReturnIDs) {
		    Pagination oReturn = null;
			int i;

                //#### If the caller passed in an invalid value for iStartRecord, raise the error
			if (iStartRecord <= 0) {
				Internationalization.RaiseDefaultError(g_cClassName + "Range", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_PositiveIntegerRequired, "iStartRecord", "1");
			}
                //#### Else if the caller passed in an invalid value for iMaxReturnIDs, raise the error
			else if (iMaxReturnIDs < 0) {
				Internationalization.RaiseDefaultError(g_cClassName + "Range", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_PositiveIntegerRequired, "iMaxReturnIDs", "0");
			}
                //#### Else if there currently DataIsLoaded into our class
			else if (DataIsLoaded) {
                    //#### If the caller is looking to retrieve the entire ga_oTables structure
				if (iStartRecord == 1 && iMaxReturnIDs == 0) {
                        //#### Reset the return value to a new instance of a Pagination, then load the .StartRecord and .OrderedBy metadata
					oReturn = new Pagination();
					oReturn.StartRecord = StartRecord;
					oReturn.OrderedBy.Load(OrderedBy.ToString(true, false));

                        //#### Traverse ga_oTables, copying each entry into the return value
					for (i = 0; i < TableCount; i++) {
						oReturn.Load(ga_oTables[i].TableName, ga_oTables[i].IDColumn, ga_oTables[i].IDs);
					}
				}
                    //#### Else we have to work for a living
				else {
                        //#### Init the local vars, initing iResultsUBound to -1 so that it is properly incremented as a 0-based number in the loop below
			        string[] a_sIDs;
			        int iResultsArrayIndex = -1;
			        int iResultsUBound = -1;
			        int iRelStartIndex = 0;     //# Set to 0 to placate the compiler
			        int iTotalIDCount = 0;
			        int iIDsLen = 0;            //# Set to 0 to placate the compiler
			        int j;

                        //#### Convert the passed iStartRecord from a 1-based number into a 0-based index for the comparisons below
					iStartRecord--;

                        //#### Traverse each element of the ga_oTables
					for (i = 0; i < ga_oTables.Length; i++) {
                            //#### If this results entry has data loaded to process
						if (ga_oTables[i].IDCount > 0) {
                                //#### Calculate this results entry's iIDsLen
							iIDsLen = ga_oTables[i].IDs.Length;

                                //#### Add the current results entry's iIDsLen onto the iTotalIDCount
							iTotalIDCount += iIDsLen;

                                //#### If we haven't yet found the iResultsArrayIndex
                                //####     NOTE: Since we need to keep the iTotalIDCount above for non-0 iMaxReturnIDs, we must do this check
                            if (iResultsArrayIndex == -1) {
                                    //#### If the current results entry holds the passed iStartRecord (only ">" is checked as iTotalIDCount is 1-based and iStartRecord is 0-based)
							    if (iTotalIDCount > iStartRecord) {
                                        //#### Set the value of iResultsArrayIndex to the current results entry index within ga_oTables
								    iResultsArrayIndex = i;

                                        //#### Set the iRelStartIndex within the current results entry (.Abs'ing the value just in case we're still within the first results entry)
                                        //####     NOTE: Since iStartRecord was decremented above, the correct 0-based iRelStartIndex is set here
								    iRelStartIndex = Math.Abs(iTotalIDCount - iIDsLen - iStartRecord);

                                        //#### If the caller passed a iMaxReturnIDs of 0 (indicating that they want all subsequent IDs)
								    if (iMaxReturnIDs == 0) {
                                            //#### Set the iResultsUBound to all the remaining results entries within ga_oTables and exit the loop (-1 to convert the 1-based .Length into a 0-based UBound)
									    iResultsUBound = (ga_oTables.Length - 1 - i);
									    break;
								    }
							    }
							}
						}

                            //#### If we have found the iResultsArrayIndex
                            //####     NOTE: This is included in a seperate if (as opposed to an else from above) so that this logic is run for every loop, else upon entry to the block above, this logic is skipped for that loop.
						if (iResultsArrayIndex != -1) {
                                //#### If we are still within the iMaxReturnIDs (skipping the "=" comparsion as it will be properly caught below)
							if (iTotalIDCount < (iStartRecord + iMaxReturnIDs)) {
                                    //#### Inc iResultsUBound as this results set is wholly contained within the requested range of IDs
								iResultsUBound++;
							}
                                //#### Else we have passed the iMaxReturnIDs (or are right at it)
							else {
                                    //#### Inc iResultsUBound so that the partial results index is counted and exit the loop
								iResultsUBound++;
								break;
							}
						}
					}

                        //#### If the iResultsArrayIndex was found above (and therefore within the range of the IDs within the ga_oTables)
					if (iResultsArrayIndex != -1) {
                            //#### Reset the return value to a new instance of a Pagination, then load the .StartRecord and .OrderedBy metadata
						oReturn = new Pagination();
						oReturn.StartRecord = StartRecord;
						oReturn.OrderedBy.Load(OrderedBy.ToString(true, false));

                            //#### Inc iResultsUBound by the value of iResultsArrayIndex for the following loop
						iResultsUBound += iResultsArrayIndex;

                            //#### Traverse the subset of ga_oTables results entries determined above
						for (i = iResultsArrayIndex; i <= iResultsUBound; i++) {
                                //#### If this results entry has data loaded
							if (ga_oTables[i].IDCount > 0) {
                                    //#### Borrow the use of iTotalIDCount to store the 0-based UBound of the .IDs
								iTotalIDCount = (ga_oTables[i].IDs.Length - 1);

                                    //#### If iMaxReturnIDs is specifing a real max and we're in the final loop
								if (iMaxReturnIDs > 0 && i == iResultsUBound) {
                                        //#### If the borrowed iTotalIDCount is less then the loop-decremented iMaxReturnIDs plus the loop-reset iRelStartIndex
									if (iTotalIDCount < (iMaxReturnIDs + iRelStartIndex)) {
                                            //#### Set iIDsLen to the iTotalIDCount less iRelStartIndex (if any)
                                            //####     NOTE: This sets iIDsLen to the number of IDs to collect from the current results entry, which is the entire .IDs array less the iRelStartIndex in the first loop, and the entire .IDs array in all subsequent loops
										iIDsLen = (iTotalIDCount - iRelStartIndex);
									}
                                        //#### Else we need to collect only the first 'X' .IDs
									else {
                                            //#### Borrow the use of iIDsLen to store the UBound of a_sIDs to the loop-decremented iMaxReturnIDs so that only the remaining .IDs are collected (converting the 1-based iMaxReturnIDs into a 0-based UBound, hence -1)
										iIDsLen = (iMaxReturnIDs - 1);
									}
								}
                                    //#### Else we need to calculate which .IDs to collect
								else {
                                        //#### Borrow the use of iIDsLen to store the UBound of a_sIDs to the UBound of the .IDs for this results entry less the iRelStartIndex (if any)
									iIDsLen = (iTotalIDCount - iRelStartIndex);
								}

                                    //#### Init a_sIDs (+1 as iIDsLen is storing the 0-based UBound)
                                a_sIDs = new string[iIDsLen + 1];

								    //#### Traverse the section of the ga_oTables's .IDs that we are interested in, copying the .IDs into the local a_sIDs
                                for (j = 0; j <= iIDsLen; j++) {
                                    a_sIDs[j] = ga_oTables[i].IDs[iRelStartIndex + j];
                                }
							}
                                //#### Else this results entry does not have data loaded, so set the local a_sIDs to null
							else {
								a_sIDs = null;
							}

                                //#### Load this results entry into the return value
							oReturn.Load(ga_oTables[i].TableName, ga_oTables[i].IDColumn, a_sIDs);

                                //#### Decrement iMaxReturnIDs by the count (hence +1, as the borrowed iIDsLen is storing the 0-based UBound) of .IDs collected in the loop
							iMaxReturnIDs -= (iIDsLen + 1);

                                //#### Ensure iRelStartIndex has been reset to 0 after the initial loop
							iRelStartIndex = 0;
						}
					}
				}
			}

			    //#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Removes the referenced 'table name'/'ID column name' pair from this instance.
		/// </summary>
		/// <remarks>
		/// NOTE: Both <paramref>sTableName</paramref> and <paramref>sIDColumn</paramref> are compared as case insensitive.
		/// </remarks>
		/// <param name="sTableName">String representing the desired table name.</param>
		/// <param name="sIDColumn">String representing the desired ID column name.</param>
		///############################################################
		/// <LastUpdated>May 31, 2004</LastUpdated>
		public bool RemoveTable(string sTableName, string sIDColumn) {
                //#### Find(the)Index and pass the call off to our sibling implementation, returning it's return value as our own
			return RemoveTable(FindIndex(sTableName, sIDColumn));
		}

		///############################################################
		/// <summary>
		/// Removes the referenced 'table name'/'ID column name' pair from this instance.
		/// </summary>
		/// <param name="iIndex">0-based integer representing the desired 'table name'/'ID column name' pair's index.</param>
		///############################################################
		/// <LastUpdated>August 16, 2005</LastUpdated>
		public bool RemoveTable(int iIndex) {
			PaginationTable[] oTempResultsArray;
			int iUBound;
			int i;
			bool bReturn;

                //#### Determine the iUBound of ga_oTables
			iUBound = (ga_oTables.Length - 1);

                //#### If the passed iIndex is outside the bounds of the ga_oTables, return false
			if (iIndex < 0 || iIndex > iUBound) {
				bReturn = false;
			}
                //#### Else the passed iIndex is within the bounds of the ga_oTables
			else {
			        //#### Init the oTempResultsArray (using the 0-based iUBound to define the 1-based .Length, where by removing one element)
			    oTempResultsArray = new PaginationTable[iUBound];

			        //#### Traverse the non-changing section of the ga_oTables, copying each element into the oTempResultsArray 
			    for (i = 0; i < iIndex; i++) {
			        oTempResultsArray[i] = ga_oTables[i];
			    }

                    //#### Traverse the changing section of the ga_oTables, starting at the passed iIndex
                    //####     NOTE: We go up to iUBound - 1 as we are moving the entries up 1 position (so we don't need to go to the "end" of the array)
				for (i = iIndex; i < iUBound; i++) {
                        //#### Move the proceeding results stack entry up over the current entry
					oTempResultsArray[i] = ga_oTables[i + 1];
				}

                    //#### Copy the above shortened oTempResultsArray back into the ga_oTables and set our bReturn value to true
                ga_oTables = oTempResultsArray;
				bReturn = true;
			}

			    //#### Return the above determined bReturn value to the caller
			return bReturn;
		}

public void SetErroredID(int iIndex, bool bNewValue) {
	int iIndexTotal = 0;
	int iCurrentIDCount;
	int i;

		//#### Traverse the ga_oTables
	for (i = 0; i < TableCount; i++) {
			//#### Get the iCurrentIDCount for this loop
		iCurrentIDCount = ga_oTables[i].IDCount;

			//#### If the iIndexTotal + the iCurrentIDCount is greater then the passed iIndex, then we have found the correct ga_oTables
		if ((iIndexTotal + iCurrentIDCount) > iIndex) {
				//#### Set the .ErroredID of the adjusted iIndex and fall from the loop
			ga_oTables[i].ErroredID(iIndex - iIndexTotal, bNewValue);
			break;
		}

			//#### Increment the iIndexTotal by the iCurrentIDCount in prep. for the next loop
		iIndexTotal += iCurrentIDCount;
	}
}

public void SetCollectedID(int iIndex, bool bNewValue) {
	int iIndexTotal = 0;
	int iCurrentIDCount;
	int i;

		//#### Traverse the ga_oTables
	for (i = 0; i < TableCount; i++) {
			//#### Get the iCurrentIDCount for this loop
		iCurrentIDCount = ga_oTables[i].IDCount;

			//#### If the iIndexTotal + the iCurrentIDCount is greater then the passed iIndex, then we have found the correct ga_oTables
		if ((iIndexTotal + iCurrentIDCount) > iIndex) {
				//#### Set the .CollectedID of the adjusted iIndex and fall from the loop
			ga_oTables[i].CollectedID(iIndex - iIndexTotal, bNewValue);
			break;
		}

			//#### Increment the iIndexTotal by the iCurrentIDCount in prep. for the next loop
		iIndexTotal += iCurrentIDCount;
	}
}


        //##########################################################################################
        //# Private Functions
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Returns the index of the referenced 'table name'/'ID column name' pair from this instance.
		/// </summary>
		/// <remarks>
		/// NOTE: Both <paramref>sTableName</paramref> and <paramref>sIDColumn</paramref> are compared as case insensitive.
		/// </remarks>
		/// <param name="sTableName">String representing the desired 'table name'.</param>
		/// <param name="sIDColumn">String representing the desired 'ID column name'.</param>
		///############################################################
		/// <LastUpdated>June 3, 2004</LastUpdated>
		private int FindIndex(string sTableName, string sIDColumn) {
			int iTableNameLen;
			int iIDColumnLen;
			int iReturn = -1;       //# Default the return value to the "index not found" value of -1
			int i;

                //#### If there DataIsLoaded in our class
			if (DataIsLoaded) {
                    //#### Lowercase the passed sTableName, sIDColumn and determine their Lens
				sTableName = sTableName.ToLower();
				sIDColumn = sIDColumn.ToLower();
				iTableNameLen = sTableName.Length;
				iIDColumnLen = sIDColumn.Length;

                    //#### Traverse ga_oTables, searching for a current entry for sTableName/sIDColumn
				for (i = 0; i < ga_oTables.Length; i++) {
                            //#### If we found a current entry for sTableName/sIDColumn (checking their Lens first as it is a far faster Comparison)
					if (iTableNameLen == ga_oTables[i].TableName.Length && iIDColumnLen == ga_oTables[i].IDColumn.Length &&
					    sTableName == ga_oTables[i].TableName.ToLower() && sIDColumn == ga_oTables[i].IDColumn.ToLower()
				    ) {
                            //#### Reset the return value to i and exit the loop
						iReturn = i;
						break;
					}
				}
			}

			    //#### Return the above determined iReturn value to the caller
			return iReturn;
		}

	#endregion


		///########################################################################################################################
		/// <summary>
		/// A child class of Pagination used to store a single 'table name'/'ID column name' entry from the parsed/loaded data.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview>August 15, 2005</LastFullCodeReview>
		public class PaginationTable {
				//#### Declare the required private variables
			private string[] ga_sIDs;
			private bool[] ga_bCollectedIDs;
			private bool[] ga_bErroredIDs;
			private string g_sTableName;
			private string g_sIDColumn;
			private int g_iMissingIDCount;

				//#### Declare the required private constants
			//private const string g_cClassName = "Cn.Data.PaginationTable";


			//##########################################################################################
			//# Class Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			///############################################################
			/// <LastUpdated>June 18, 2010</LastUpdated>
			public PaginationTable() {
					//#### Init the global class vars
				ga_bCollectedIDs = null;
				ga_bErroredIDs = null;
			}


			//##########################################################################################
			//# Public Read/Write Properties
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Gets/sets the collection of 'IDs' present within this instance.
			/// </summary>
			/// <value>String array where each index represents a unique 'ID' present within this instance.</value>
			///############################################################
			/// <LastUpdated>June 21, 2010</LastUpdated>
			public string[] IDs {
				get {
					return ga_sIDs;
				}
				set {
					int iLen;
					int i;

						//#### If the passed value is null, set ga_sIDs and ga_bCollectedIDs to null
					if (value == null || value.Length == 0) {
						ga_sIDs = null;
						ga_bCollectedIDs = null;
						ga_bErroredIDs = null;
					}
						//#### Else the caller passed in a valid array of ga_sIDs
					else {
							//#### Reset the value of ga_sIDs with the passed value and determine its iLen
						ga_sIDs = value;
						iLen = ga_sIDs.Length;

							//#### ReDim and reset the value of ga_bCollectedIDs for the new set of ga_sIDs
						ga_bCollectedIDs = new bool[iLen];
						ga_bErroredIDs = new bool[iLen];
						for (i = 0; i < iLen; i++) {
							ga_bCollectedIDs[i] = false;
							ga_bErroredIDs[i] = false;
						}
					}
				}
			}

			///############################################################
			/// <summary>
			/// Gets/sets the 'table name' of this instance.
			/// </summary>
			/// <value>String representing the 'table name'.</value>
			///############################################################
			/// <LastUpdated>May 12, 2004</LastUpdated>
			public string TableName {
				get {
					return g_sTableName;
				}
				set {
					g_sTableName = value;
				}
			}

			///############################################################
			/// <summary>
			/// Gets/sets the 'ID column name' of this instance.
			/// </summary>
			/// <value>String representing the 'ID column name'.</value>
			///############################################################
			/// <LastUpdated>May 12, 2004</LastUpdated>
			public string IDColumn {
				get {
					return g_sIDColumn;
				}
				set {
					g_sIDColumn = value;
				}
			}

			///############################################################
			/// <summary>
			/// Gets/sets the total count of missing 'IDs' within this instance.
			/// </summary>
			/// <value>1-based integer representing the count of missing 'IDs' within this instance.</value>
			///############################################################
			/// <LastUpdated>July 20, 2004</LastUpdated>
			public int MissingIDCount {
				get {
					return g_iMissingIDCount;
				}
				set {
					g_iMissingIDCount = value;
				}
			}


			//##########################################################################################
			//# Public Read-Only Properties
			//##########################################################################################
public int ErroredIDCount {
	get {
		int iReturn = 0;
		int i;

			//#### If we have some ga_sIDs
		if (ga_sIDs != null) {
				//#### Traverse the ga_bErroredIDs (which should be synced in .Length to the ga_sIDs), inc'ing our iReturn value for each .ErroredID
			for (i = 0; i < ga_bErroredIDs.Length; i++) {
				if (ga_bErroredIDs[i]) {
					iReturn++;
				}
			}
		}

			//#### Return the above determined iReturn value to the caller 
		return iReturn;
	}
}

			///############################################################
			/// <summary>
			/// Gets the total count of 'IDs' within this instance.
			/// </summary>
			/// <value>1-based integer representing the count of 'IDs' within this instance.</value>
			///############################################################
			/// <LastUpdated>June 21, 2010</LastUpdated>
			public int IDCount {
				get { return (ga_sIDs == null ? 0 : ga_sIDs.Length); }
			}


			//##########################################################################################
			//# Public Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Adds the provided ID into the IDs collection.
			/// </summary>
			/// <remarks>
			/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
			/// </remarks>
			/// <param name="sID">String representing the desired ID to add.</param>
			///############################################################
			/// <LastUpdated>June 21, 2010</LastUpdated>
			public void AddID(string sID) {
					//#### .Push the passed sID into our ga_sIDs
				ga_sIDs = Data.Tools.Push(ga_sIDs, sID);
				ga_bCollectedIDs = Data.Tools.Push(ga_bCollectedIDs, false);
				ga_bErroredIDs = Data.Tools.Push(ga_bErroredIDs, false);
			}

			///############################################################
			/// <summary>
			/// Gets/sets the 'ID was collected' value present at the referenced index (pseudo-parameterized property).
			/// </summary>
			/// <remarks>
			/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
			/// </remarks>
			/// <param name="iIndex">0-based integer representing the desired 'ID was collected' index.</param>
			/// <value>Boolean value representing if the 'ID' at the passed <paramref>iIndex</paramref> was collected.</value>
			///############################################################
			/// <LastUpdated>January 4, 2010</LastUpdated>
			public bool CollectedID(int iIndex) {
				return ga_bCollectedIDs[iIndex];
			}

			///############################################################
			/// <summary>
			/// Gets/sets the 'ID was collected' value present at the referenced index (pseudo-parameterized property).
			/// </summary>
			/// <remarks>
			/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
			/// </remarks>
			/// <param name="iIndex">0-based integer representing the desired 'ID was collected' index.</param>
			/// <param name="bNewValue">Boolean value representing the new value for the referenced index.</param>
			///############################################################
			/// <LastUpdated>January 4, 2010</LastUpdated>
			public void CollectedID(int iIndex, bool bNewValue) {
				ga_bCollectedIDs[iIndex] = bNewValue;
			}

			///############################################################
			/// <summary>
			/// Sets all of the IDs wihtin this instance as collected.
			/// </summary>
			///############################################################
			/// <LastUpdated>May 30, 2007</LastUpdated>
			public void SetCollectedIDs() {
				int i;

					//#### If we have ga_bCollectedIDs to flip
				if (ga_bCollectedIDs != null) {
						//#### Set all the values within ga_bCollectedIDs to true, skipping the actual many-to-many comparison
					for (i = 0; i < ga_bCollectedIDs.Length; i++) {
						ga_bCollectedIDs[i] = true;
					}
				}
			}

			///############################################################
			/// <summary>
			/// Sets the group of successfully collected 'IDs'.
			/// </summary>
			/// <param name="a_sIDs">String array where each index represents an 'ID' present within this instances collection of 'IDs'.</param>
			///############################################################
			/// <LastUpdated>May 2, 2007</LastUpdated>
			public void SetCollectedIDs(string[] a_sIDs) {
				int iCurrentLen;
				int iPassedLen;
				int iLen = 0;
				int i;
				int j;

						//#### Determine the iLen
				if (ga_sIDs != null) {
					iLen = ga_sIDs.Length;
				}

					//#### If a valid array of a_sIDs was passed
				if (a_sIDs != null && a_sIDs.Length > 0) {
						//#### Determine the iPassedLen
					iPassedLen = a_sIDs.Length;

						//#### If the expected (or more then the expected) number of a_sIDs were collected (assuming that the developer collected what they were supposed to)
					if (iLen <= iPassedLen) {
							//#### Set all the values within ga_bCollectedIDs to true, skipping the actual many-to-many comparison
						for (i = 0; i < iLen; i++) {
							ga_bCollectedIDs[i] = true;
						}
					}
						//#### Else some of the ga_sIDs are missing
					else {
							//#### Init all the values within ga_bCollectedIDs to false
						for (i = 0; i < iLen; i++) {
							ga_bCollectedIDs[i] = false;
						}

							//#### Traverse the .IDs and the passed a_sIDs arrays
						for (i = 0; i < iLen; i++) {
								//#### Determine the iCurrentLen for this loop
							iCurrentLen = ga_sIDs[i].Length;

								//#### Traverse the passed a_sIDs, looking for the current .ID
							for (j = 0; j < iPassedLen; j++) {
									//#### If this a_sID matches the current .ID (checking their .Lengths first as that is a far faster comparison)
								if (a_sIDs[j].Length == iCurrentLen && a_sIDs[j] == ga_sIDs[i]) {
										//#### Set this g_bCollectedID and exit the inner loop
									ga_bCollectedIDs[i] = true;
									break;
								}
							}
						}
					}
				}
					//#### Else no a_sIDs were passed in by the caller
				else {
						//#### Reset our .IDs (via the property) accordingly
						//####     NOTE: We cannot simply do this as if a page somehow has all .MissingRecord's then Renderer will erroroneously .Render .NoResults (when it should .Render the .MissingRecord's)
				  //IDs = null;

						//#### Since no a_sIDs were passed, reset the values within ga_bCollectedIDs to false
					for (i = 0; i < iLen; i++) {
						ga_bCollectedIDs[i] = false;
					}
				}
			}

			///############################################################
			/// <summary>
			/// Gets/sets the 'errored ID' value present at the referenced index (pseudo-parameterized property).
			/// </summary>
			/// <remarks>
			/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
			/// </remarks>
			/// <param name="iIndex">0-based integer representing the desired 'errored ID' index.</param>
			/// <value>Boolean value representing if the 'ID' at the passed <paramref>iIndex</paramref> is errored.</value>
			///############################################################
			/// <LastUpdated>June 18, 2010</LastUpdated>
			public bool ErroredID(int iIndex) {
				return ga_bErroredIDs[iIndex];
			}

			///############################################################
			/// <summary>
			/// Gets/sets the 'errored ID' value present at the referenced index (pseudo-parameterized property).
			/// </summary>
			/// <remarks>
			/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
			/// </remarks>
			/// <param name="iIndex">0-based integer representing the desired 'errored ID' index.</param>
			/// <param name="bNewValue">Boolean value representing the new value for the referenced index.</param>
			///############################################################
			/// <LastUpdated>June 18, 2010</LastUpdated>
			public void ErroredID(int iIndex, bool bNewValue) {
				ga_bErroredIDs[iIndex] = bNewValue;
			}

			///############################################################
			/// <summary>
			/// Returns a SQL In Clause for the 'IDs' within this instance.
			/// </summary>
			/// <returns>String that represents a single SQL In Clause containing all of the 'IDs' present within this instance.</returns>
			///############################################################
			/// <LastUpdated>September 7, 2005</LastUpdated>
			public string GenerateSQLInClause() {
				string sReturn = "";

					//#### If we have ga_sIDs to .Join, reset out sReturn value to the properly .Join'd ga_sIDs
				if (ga_sIDs != null && ga_sIDs.Length > 0) {
					sReturn = "'" + string.Join("','", ga_sIDs) + "'";
				}

					//#### Return the above determined sReturn value
				return sReturn;
			}

			///############################################################
			/// <summary>
			/// Returns a collection of SQL In Clauses for the 'IDs' within this instance.
			/// </summary>
			/// <param name="iMaxIDCountPerClause">1-based integer representing the desired maximum number of 'IDs' per returned clause.</param>
			/// <returns>String array where each index represents a single SQL In Clause with no more then the passed <paramref>iMaxIDCountPerClause</paramref>, the total of which contain all of the 'IDs' present within this instance.</returns>
			///############################################################
			/// <LastUpdated>March 2, 2007</LastUpdated>
			public string[] GenerateSQLInClause(int iMaxIDCountPerClause) {
				string[] a_sReturn;
				string sTemp;
				int iLogicalUBound;
				int iReturnCount;
				int iIDCount = IDCount;
				int i;
				int j;

					//#### If the passed iMaxIDCountPerClause is not a positive number, or it is greater then the iIDCount
				if (iMaxIDCountPerClause < 1 || (iMaxIDCountPerClause - 1) > iIDCount) {
						//#### ReDim the a_sReturn value to a single entry, then collect the result from our sibling implementation
					a_sReturn = new string[1];
					a_sReturn[0] = GenerateSQLInClause();
				}
					//#### Else we've got some work to do
				else {
						//#### Determine the iReturn(values)Count and init the a_sReturn value accordingly
						//####     NOTE: We know that iIDCount is > 0 here because if it were 0 (or less then iMaxIDCountPerClause - 1) it was caught and dealt with above.
					iReturnCount = (Data.Tools.MakeInteger(Math.Ceiling( (decimal)(iIDCount / iMaxIDCountPerClause) ), -1) + 1);
					a_sReturn = new string[iReturnCount];

						//#### Traverse our a_sReturn value
					for (i = 0; i < iReturnCount; i++) {
							//#### Determine the iLogicalUBound and reset the value of sCurrentAttribute for this loop
						iLogicalUBound = ((i * iMaxIDCountPerClause) + iMaxIDCountPerClause - 1);
						sTemp = "";

							//#### If the iLogicalUBound was calculated to be past the iIDCount, reset it to the iIDCount - 1 (transfroming it into a 0-based index)
						if (iLogicalUBound >= iIDCount) {
							iLogicalUBound = (iIDCount - 1);
						}

							//#### Traverse this loop's section of the ga_sIDs, appending them onto sCurrentAttribute as we go
						for (j = (i * iMaxIDCountPerClause); j <= iLogicalUBound; j++) {
							sTemp += "'" + ga_sIDs[j] + "',";
						}

							//#### Set the above filled sCurrentAttribute into the current a_sReturn value entry (lopping off the trailing comma as we go)
						a_sReturn[i] = sTemp.Substring(0, sTemp.Length - 1);
					}
				}

					//#### Return the above determined a_sReturn value to the caller
				return a_sReturn;
			}

		} //# class Table

	} //# class Pagination

} //# namespace Cn.Database
