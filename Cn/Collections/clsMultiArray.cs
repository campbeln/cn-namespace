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
using System.Collections;					        //# Required to access the Hashtable class
using System.Runtime.Serialization;					//# Required to access ISerializable
using Cn.Data.SQL;		                            //# Required to access the SQL statement functionality used within .SQL
using Cn.Configuration;								//# Required to access the Internationalization class


/*
 * c# indexer (default property)
public string this[] {
    get {
        return this.Value;
    }
    set {
        this.Value = value;
    }
}
*/

namespace Cn.Collections {

	///########################################################################################################################
	/// <summary>
	/// Represents a collection of rows and columns organised based on their names and positions within the structure.
	/// </summary>
	/// <remarks>
	/// Fun fact: This was the first C# module built for Renderer (no VB.Net counterpart ever existed for this class).
	/// </remarks>
	///########################################################################################################################
	/// <LastFullCodeReview>June 4, 2007</LastFullCodeReview>
	[Serializable]
	public class MultiArray : ISerializable {
	#region MultiArray
			//#### Define the required private variables
		private DataContainer g_oData;

	        //#### Declare the required public eNums
	    #region eNums
			/// <summary>SQL Statements types.</summary>
        public enum enumStatementTypes : int {
				/// <summary>SQL insert statement.</summary>
            cnInsert = 0,
				/// <summary>SQL update statement.</summary>
            cnUpdate = 1
        }
        #endregion

			//#### Declare the required private constants
		private const string g_cClassName = "Cn.Collections.MultiArray.";


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class based on the provided column names.
		/// </summary>
		/// <param name="a_sInitialColumnNames">String array representing the desired initial column names.</param>
		/// <exception cref="Cn.CnException">Thrown when a null-string column name is within the passed <paramref>a_sInitialColumnNames</paramref>.</exception>
		/// <exception cref="Cn.CnException">Thrown when a column name is represented more then once within the passed <paramref>a_sInitialColumnNames</paramref>.</exception>
		/// <seealso cref="Cn.Collections.MultiArray.Reset(string[])"/>
		///############################################################
		/// <LastUpdated>August 22, 2005</LastUpdated>
		public MultiArray(string[] a_sInitialColumnNames) {
				//#### Call .DoReset to init the class vars
			DoReset("[Constructor]", a_sInitialColumnNames);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state based on the provided column names.
		/// </summary>
		/// <param name="a_sInitialColumnNames">String array representing the desired initial column names.</param>
		/// <exception cref="Cn.CnException">Thrown when a null-string column name is within the passed <paramref>a_sInitialColumnNames</paramref>.</exception>
		/// <exception cref="Cn.CnException">Thrown when a column name is represented more then once within the passed <paramref>a_sInitialColumnNames</paramref>.</exception>
		///############################################################
		/// <LastUpdated>August 17, 2005</LastUpdated>
		public void Reset(string[] a_sInitialColumnNames) {
				//#### Call .DoReset to re-init the class vars (disreguarting .DoReset's return value)
			DoReset("Reset", a_sInitialColumnNames);
		}

		///############################################################
		/// <summary>
		/// Initializes the class based on the provided structure.
		/// </summary>
		/// <param name="sMultiArrayString">String representing an instance of a MultiArray class.</param>
		/// <exception cref="Cn.CnException">Thrown when a null-string column name is within the passed <paramref>sMultiArrayString</paramref>.</exception>
		/// <exception cref="Cn.CnException">Thrown when a column name is represented more then once within the passed <paramref>sMultiArrayString</paramref>.</exception>
		/// <exception cref="Cn.CnException">Thrown when a row does not have the correct number of columns within the passed <paramref>sMultiArrayString</paramref>.</exception>
		/// <seealso cref="Cn.Collections.MultiArray.Reset(string)"/>
		///############################################################
		/// <LastUpdated>August 22, 2005</LastUpdated>
		public MultiArray(string sMultiArrayString) {
				//#### Call .DoReset to init the class vars
		    DoReset("[Constructor]", sMultiArrayString);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state based on the provided structure.
		/// </summary>
		/// <param name="sMultiArrayString">String representing an instance of a MultiArray class.</param>
		/// <exception cref="Cn.CnException">Thrown when a null-string column name is within the passed <paramref>sMultiArrayString</paramref>.</exception>
		/// <exception cref="Cn.CnException">Thrown when a column name is represented more then once within the passed <paramref>sMultiArrayString</paramref>.</exception>
		/// <exception cref="Cn.CnException">Thrown when a row does not have the correct number of columns within the passed <paramref>sMultiArrayString</paramref>.</exception>
		///############################################################
		/// <LastUpdated>August 17, 2005</LastUpdated>
		public void Reset(string sMultiArrayString) {
				//#### Call .DoReset to re-init the class vars (disreguarting .DoReset's return value)
			DoReset("Reset", sMultiArrayString);
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <seealso cref="Cn.Collections.MultiArray.Reset()"/>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public MultiArray() {
				//#### Call .DoReset to init the class vars
		    DoReset();
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void Reset() {
				//#### Call .DoReset to init the class vars
		    DoReset();
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		private void DoReset() {
				//#### (Re)Init the global variables
			g_oData = new DataContainer();
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="a_sColumnNames">String array representing the desired column names.</param>
		/// <returns>Boolean value signaling the success/failure of the call.</returns>
		/// <exception cref="Cn.CnException">Thrown when a null-string column name is within the passed <paramref>a_sInitialColumnNames</paramref>.</exception>
		/// <exception cref="Cn.CnException">Thrown when a column name is represented more then once within the passed <paramref>a_sInitialColumnNames</paramref>.</exception>
		/// <seealso cref="Cn.Collections.MultiArray.Reset(string[])"/>
		///############################################################
		/// <LastUpdated>November 2, 2009</LastUpdated>
		private void DoReset(string sFunction, string[] a_sColumnNames) {
			int iColumnNamesLen;
			int i;

				//#### Call our base sibling implementation to (re)init the global variables
			DoReset();

				//#### If there are entries within the passed a_sColumnNames to process
			if (a_sColumnNames != null && a_sColumnNames.Length > 0) {
					//#### Determine the iColumnNamesLen and (re)dimension g_oData's .ColumnNames accordingly
				iColumnNamesLen = a_sColumnNames.Length;
				g_oData.ColumnNames = new string[iColumnNamesLen];

					//#### Pre-populate the .ColumnNames with null-string (so the .ColumnIndex call works below)
				for (i = 0; i < iColumnNamesLen; i++) {
					g_oData.ColumnNames[i] = "";
				}

					//#### Traverse the passed a_sColumnNames
				for (i = 0; i < iColumnNamesLen; i++) {
						//#### If the current a_sColumnName is a null-string, raise the error
					if (string.IsNullOrEmpty(a_sColumnNames[i])) {
			            Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "a_sColumnNames[" + i + "]", "");
					  //break;
					}
						//#### Else if the current a_sColumnName already exists, raise the error
					else if (ColumnIndex(a_sColumnNames[i]) != -1) {
			            Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_MultiArray_DuplicateColumnName, a_sColumnNames[i], "a_sColumnNames[" + i + "]");
					  //break;
					}
						//#### Else the current a_sColumnName is unique (so far)
					else {
							//#### Add the current a_sColumnName into g_oData's .ColumnNames
							//####     NOTE: We do not .ToLower() the current a_sColumnNames here as we want to preserve the casing (as some systems/objects care about the casing), so we need to inline .ToLower() the .ColumnNames as we use them internally (which is just within .ColumnIndex)
						g_oData.ColumnNames[i] = a_sColumnNames[i];
					}
				}
			}
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="sMultiArrayString">String representing an instance of a MultiArray class.</param>
		/// <exception cref="Cn.CnException">Thrown when a null-string column name is within the passed <paramref>sMultiArrayString</paramref>.</exception>
		/// <exception cref="Cn.CnException">Thrown when a column name is represented more then once within the passed <paramref>sMultiArrayString</paramref>.</exception>
		/// <exception cref="Cn.CnException">Thrown when a row does not have the correct number of columns within the passed <paramref>sMultiArrayString</paramref>.</exception>
		/// <seealso cref="Cn.Collections.MultiArray.Reset(string)"/>
		///############################################################
		/// <LastUpdated>January 20, 2010</LastUpdated>
		private void DoReset(string sFunction, string sMultiArrayString) {
		    string[] a_sSecondary;
		    string[] a_sPrimary;
		    char[] a_charSecondaryDelimiter = Settings.SecondaryDelimiter.ToCharArray();
		    int iColumnCount;
		    int iPrimaryLen;
		    int i;
		    int j;

				//#### Ensure the passed sMultiArrayString is a string
			sMultiArrayString = Cn.Data.Tools.MakeString(sMultiArrayString, "");

				//#### If we have a sMultiArrayString to parse
			if (sMultiArrayString.Length > 0) {
					//#### .Split the passed sMultiArrayString apart by its .PrimaryDelimiters, then .Split the metadata section by its .SecondaryDelimiters and finially determine a_sPrimary's .Length
				a_sPrimary = sMultiArrayString.Split(Settings.PrimaryDelimiter.ToCharArray());
				a_sSecondary = a_sPrimary[0].Split(a_charSecondaryDelimiter);
				iPrimaryLen = a_sPrimary.Length;

					//#### Call our sibling implementation to load the column names defined within a_sSecondary and set iColumnCount
				DoReset(sFunction, a_sSecondary);
				iColumnCount = ColumnCount;

					//#### Traverse the defined .Rows within a_sPrimary (starting at index 1 as index 0 was metadata that was processed above)
					//####     NOTE: This functions correctly if no .Rows were defined within the sMultiArrayString as in that case iPrimaryLen would be 1, and this loop would therefor be skipped
				for (i = 1; i < iPrimaryLen; i++) {
						//#### .Split the current .Row by its .SecondaryDelimiters
					a_sSecondary = a_sPrimary[i].Split(a_charSecondaryDelimiter);

						//#### If the correct number of columns have been defined in the current .Row
					if (a_sSecondary.Length == iColumnCount) {
							//#### Traverse the a_sSecondary array, .DelimiterDecoder-ing each value
							//####     NOTE: An explicit deep copy of a_sSecondary is not required as it was not passed in by the caller and it is reset to a new string array instance at each loop, so the current instance will only be referenced within gh_aRows
						for (j = 0; j < iColumnCount; j++) {
							a_sSecondary[j] = Settings.DelimiterDecoder(a_sSecondary[j]);
						}

							//#### Call .ManageRows to insert the current .Row (as represented by a_sSecondary)
							//####     NOTE: Since the 0th index within the passed sMultiArrayString is metadata, we decrement i below to reference the next available .Row index
						ManageRows(sFunction, true, (i - 1), a_sSecondary);
					}
						//#### Else the passed a_sRow does not have the proper number of columns, so raise the error
					else {
						Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_InvalidColumnCount, ColumnCount.ToString(), "");
					}
				}
			}
				//#### Else there is no sMultiArrayString to parse, so we need to (re)init the global variables ourselves
			else {
					//#### Call our base sibling implementation to (re)init the global variables
				DoReset();
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
		public MultiArray(SerializationInfo info, StreamingContext ctxt) {
				//#### Call .DoReset to init the class vars
		    DoReset("[Constructor]", Cn.Data.Tools.MakeString(info.GetValue("MultiArrayString", typeof(string)), ""));
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
			info.AddValue("MultiArrayString", ToString());
		}


		//##########################################################################################
		//# Public Read-only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets a deep copy of the data stored defined within this instance.
		/// </summary>
		/// <value>Deep copy of the data stored defined within this instance.</value>
		///############################################################
		/// <LastUpdated>December 23, 2009</LastUpdated>
		public MultiArray Data {
			get {
				MultiArray oReturn = new MultiArray(g_oData.ColumnNames);
				int i;

					//#### Traverse our .Rows, inserting each in turn into our oReturn value
				for (i = 0; i < RowCount; i++) {
					oReturn.InsertRow(RowAsArray(i));
				}

					//#### Return the oReturn value to the caller
				return oReturn;
			}
		}

		///############################################################
		/// <summary>
		/// Gets the row count within this instance.
		/// </summary>
		/// <value>1-based integer representing the count of rows within this instance.</value>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public int RowCount {
			get {
				int iReturn = 0;

					//#### g_oData's .Rows is not null, reset our iReturn value to it's .Length
				if (g_oData.Rows != null) {
					iReturn = g_oData.Rows.Length;
				}

					//#### Return the above determined iReturn value to the caller
				return iReturn;
			}
		}

		///############################################################
		/// <summary>
		/// Gets the column count within this instance.
		/// </summary>
		/// <value>1-based integer representing the count of columns within this instance.</value>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public int ColumnCount {
			get {
				int iReturn = 0;

					//#### g_oData's .ColumnNames is not null, reset our iReturn value to it's .Length
				if (g_oData.ColumnNames != null) {
					iReturn = g_oData.ColumnNames.Length;
				}

					//#### Return the above determined iReturn value to the caller
				return iReturn;
			}
		}

		///############################################################
		/// <summary>
		/// Gets the column names present within this instance.
		/// </summary>
		/// <value>String array where each index represents a column name present within this instance.</value>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public string[] ColumnNames {
			get { return Platform.Specific.DeepCopy(g_oData.ColumnNames); }
		}


		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the string value present at the referenced row/column (pseudo-parameterized property).
		/// </summary>
		/// <remarks>
		/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
		/// <para/>NOTE: If the passed <paramref>iRowIndex</paramref> is invalid or the passed <paramref>sColumnName</paramref> is unreconized, a null-string is returned.
		/// </remarks>
		/// <param name="iRowIndex">0-based integer representing the desired row index.</param>
		/// <param name="sColumnName">String representing the desired column name.</param>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public string Value(int iRowIndex, string sColumnName) {
				//#### Pass the call off to .DoValue
			return DoValue(iRowIndex, ColumnIndex(sColumnName), false, "");
		}

		///############################################################
		/// <summary>
		/// Gets/sets the string value present at the referenced row/column (pseudo-parameterized property).
		/// </summary>
		/// <remarks>
		/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
		/// </remarks>
		/// <param name="iRowIndex">0-based integer representing the desired row index.</param>
		/// <param name="sColumnName">String representing the desired column name.</param>
		/// <param name="sNewValue">String representing the new value for the referenced index.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iRowIndex</paramref> is outside the proper range (0 to (<c>RowCount</c> - 1)).</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sColumnName</paramref> does not exist within this instance.</exception>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		public void Value(int iRowIndex, string sColumnName, string sNewValue) {
			int iColumnIndex = ColumnIndex(sColumnName);

				//#### If the passed sColumnName was unreconized, raise the error
			if (iColumnIndex == -1) {
				Internationalization.RaiseDefaultError(g_cClassName + "Value", Internationalization.enumInternationalizationValues.cnDeveloperMessages_MultiArray_ColumnNameNotFound, sColumnName, "");
			}
				//#### Else the passed sColumnName was valid
			else {
					//#### Pass the call off to .DoValue
				DoValue(iRowIndex, ColumnIndex(sColumnName), true, sNewValue);
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets the string value present at the referenced row/column (pseudo-parameterized property).
		/// </summary>
		/// <remarks>
		/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
		/// <para/>NOTE: If the passed <paramref>iRowIndex</paramref> or <paramref>iColumnIndex</paramref> is invalid, a null-string is returned.
		/// </remarks>
		/// <param name="iRowIndex">0-based integer representing the desired row index.</param>
		/// <param name="iColumnIndex">0-based integer representing the desired column index.</param>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public string Value(int iRowIndex, int iColumnIndex) {
				//#### Pass the call off to .DoValue
			return DoValue(iRowIndex, iColumnIndex, false, "");
		}

		///############################################################
		/// <summary>
		/// Gets/sets the string value present at the referenced row/column (pseudo-parameterized property).
		/// </summary>
		/// <remarks>
		/// This is a pseudo-parameterized property implimentation. The overloaded pair of methods serve the same function as a parameterized property would.
		/// </remarks>
		/// <param name="iRowIndex">0-based integer representing the desired row index.</param>
		/// <param name="iColumnIndex">0-based integer representing the desired column index.</param>
		/// <param name="sNewValue">String representing the new value for the referenced index.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iRowIndex</paramref> is outside the proper range (0 to (<c>RowCount</c> - 1)).</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iColumnIndex</paramref> is outside the proper range (0 to (<c>ColumnCount</c> - 1)).</exception>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void Value(int iRowIndex, int iColumnIndex, string sNewValue) {
				//#### Pass the call off to .DoValue
			DoValue(iRowIndex, iColumnIndex, true, sNewValue);
		}

		///############################################################
		/// <summary>
		/// Determines if this instance contains a specific column name.
		/// </summary>
		/// <param name="sColumnName">String representing the column name to locate.</param>
		/// <returns>Boolean value signaling the existance of the passed <paramref>sColumnName</paramref>.<para/>Returns true if the <paramref>sColumnName</paramref> is present within this instance, or false if it is not present.</returns>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public bool Exists(string sColumnName) {
				//#### Return based on the return value of .ColumnIndex
			return (ColumnIndex(sColumnName) != -1);
		}

		///############################################################
		/// <summary>
		/// Determines if this instance contains all of the provided column names.
		/// </summary>
		/// <param name="a_sColumnNames">String array representing the column names to locate.</param>
		/// <returns>Boolean value signaling the existance of all of the column names within the passed <paramref>a_sColumnNames</paramref>.<para/>Returns true if all of the <paramref>a_sColumnNames</paramref> are present within this instance, or false if one or more are not present.</returns>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public bool Exists(string[] a_sColumnNames) {
			bool bReturn = false;
			int i;

				//#### If the caller passed in a valid a_sColumnNames
			if (a_sColumnNames != null && a_sColumnNames.Length > 0) {
					//#### Traverse the passed a_sColumnNames
				for (i = 0; i < a_sColumnNames.Length; i++) {
						//#### Reset our bReturn value based on the return value of .ColumnIndex
					bReturn = (ColumnIndex(a_sColumnNames[i]) != -1);

						//#### If our bReturn value has been set to false, fall out of the loop
					if (! bReturn) {
						break;
					}
				}
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves the column index of the referenced column.
		/// </summary>
		/// <param name="sColumnName">String representing the desired column name.</param>
		/// <returns>0-based integer representing the column index of the passed <paramref>sColumnName</paramref>.</returns>
		///############################################################
		/// <LastUpdated>January 25, 2010</LastUpdated>
		public int ColumnIndex(string sColumnName) {
			string[] a_sColumnNames = g_oData.ColumnNames;
			int iReturn = -1;
			int iColumnNameLen;
			int i;

				//#### If we have a_sColumnNames to traverse
			if (a_sColumnNames != null && a_sColumnNames.Length > 0) {
					//#### .ToLower the passed sColumnName (making sure it's a string as we go)
				sColumnName = Cn.Data.Tools.MakeString(sColumnName, "").ToLower();
				iColumnNameLen = sColumnName.Length;

					//#### Traverse the a_sColumnNames
				for (i = 0; i < a_sColumnNames.Length; i++) {
						//#### If the sCurrentColumnName matches the passed sColumnName (checking their .Length's first as that is a far faster comparsion)
						//####     NOTE: When the a_sColumnNames are inserted they are not lower cased, so so we have to .ToLower the current a_sColumnName below
					if (a_sColumnNames[i].Length == iColumnNameLen && a_sColumnNames[i].ToLower() == sColumnName) {
							//#### Reset our iReturn value to the current index and exit the loop
						iReturn = i;
						break;
					}
				}
			}

				//#### Return the above determined iReturn value to the caller
			return iReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves the column name of the referenced column.
		/// </summary>
		/// <param name="iColumnIndex">0-based integer representing the desired column index.</param>
		/// <returns>String representing the column name of the passed <paramref>iColumnIndex</paramref>.</returns>
		///############################################################
		/// <LastUpdated>January 21, 2010</LastUpdated>
		public string ColumnName(int iColumnIndex) {
			string sReturn = "";

				//#### If the passed iColumnIndex is valid (signaling .ValidateColumnIndex to not raise any errors)
			if (g_oData.ValidateColumnIndex(g_cClassName + "ColumnName", iColumnIndex, false)) {
					//#### Reset our sReturn value to the .ColumnName of the passed iColumnIndex
				sReturn = g_oData.ColumnNames[iColumnIndex];
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves a structure that represents the referenced row.
		/// </summary>
		/// <param name="iRowIndex">0-based integer representing the desired row index.</param>
		/// <returns>Hashtable of strings where each entry represents the string data for each column of the passed <paramref>iRowIndex</paramref>, or null if it was not defined.</returns>
		///############################################################
		/// <LastUpdated>February 19, 2010</LastUpdated>
		public Hashtable Row(int iRowIndex) {
			Hashtable oReturn = new Hashtable();
			string[] a_sColumnNames = g_oData.ColumnNames;
			string[] a_sCurrentRow;
			int i;

				//#### If the passed iRowIndex is valid (signaling .ValiateRowIndex not to raise any errors)
			if (g_oData.ValiateRowIndex(g_cClassName + "Row", iRowIndex, false)) {
					//#### Collect the a_sCurrentRow
				a_sCurrentRow = g_oData.Rows[iRowIndex].Columns;

					//#### Traverse the .ColumnNames, copying each .ColumnName and .Column value into our oReturn value as we go
				for (i = 0; i < a_sColumnNames.Length; i++) {
					oReturn[a_sColumnNames[i]] = a_sCurrentRow[i];
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves a structure that represents the referenced row.
		/// </summary>
		/// <param name="iRowIndex">0-based integer representing the desired row index.</param>
		/// <returns>Array of strings where each entry represents the string data for each column of the passed <paramref>iRowIndex</paramref>, or null if it was not defined.</returns>
		///############################################################
		/// <LastUpdated>February 19, 2010</LastUpdated>
		public string[] RowAsArray(int iRowIndex) {
			string[] a_sReturn;

				//#### If the passed iRowIndex is valid (signaling .ValiateRowIndex not to raise any errors)
			if (g_oData.ValiateRowIndex(g_cClassName + "RowAsArray", iRowIndex, false)) {
					//#### .DeepCopy the referenced .Row's .Columns into our a_sReturn value
				a_sReturn = Platform.Specific.DeepCopy(g_oData.Rows[iRowIndex].Columns);
			}
				//#### Else the passed iRowIndex was invalid, so set our a_sReturn value to an empty array
			else {
				a_sReturn = new string[0];
			}

				//#### Return the above determined a_sReturn value to the caller
			return a_sReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves a structure that represents the referenced column.
		/// </summary>
		/// <param name="sColumnName">String representing the desired column name.</param>
		/// <returns>String array where each index represents the string data for each row of the passed <paramref>sColumnName</paramref>, or null if it was not defined.</returns>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public string[] Column(string sColumnName) {
				//#### Pass the call off to our sibling implementation (determining the passed sColumnName's .ColumnIndex as we go)
			return Column(ColumnIndex(sColumnName));
		}

		///############################################################
		/// <summary>
		/// Retrieves a structure that represents the referenced column.
		/// </summary>
		/// <param name="iColumnIndex">0-based integer representing the desired column index.</param>
		/// <returns>String array where each index represents the string data for each row of the passed <paramref>iColumnIndex</paramref>, or null if it was not defined.</returns>
		///############################################################
		/// <LastUpdated>February 19, 2010</LastUpdated>
		public string[] Column(int iColumnIndex) {
			string[] a_sReturn;
			int iRowCount = RowCount;
			int i;

				//#### If the passed iColumnIndex is valid (signaling .ValidateColumnIndex to not raise any errors)
			if (g_oData.ValidateColumnIndex(g_cClassName + "Column", iColumnIndex, false)) {
					//#### Dimension the a_sReturn value to fit one .Column entry for each .Row
				a_sReturn = new string[iRowCount];

					//#### Traverse the .Rows, copying each .Column value into our a_sReturn value as we go
				for (i = 0; i < iRowCount; i++) {
					a_sReturn[i] = g_oData.Rows[i].Columns[iColumnIndex];
				}
			}
				//#### Else the passed iRowIndex was invalid, so set our a_sReturn value to an empty array
			else {
				a_sReturn = new string[0];
			}

				//#### Return the above determined a_sReturn value to the caller
			return a_sReturn;
		}

		///############################################################
		/// <summary>
		/// Appends the passed row onto this instance.
		/// </summary>
		/// <param name="a_sRow">Array of strings representing the string data to insert as the new row.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sRow</paramref> does not have the proper number of columns.</exception>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void InsertRow(string[] a_sRow) {
				//#### Call .ManageRows to actually insert the passed a_sRow into the g_oData, telling it to insert it at the bottom
			ManageRows("InsertRow", true, RowCount, a_sRow);
		}

		///############################################################
		/// <summary>
		/// Inserts the passed row into this instance at the passed destination index.
		/// </summary>
		/// <param name="a_sRow">Array of strings representing the string data to insert as the new row.</param>
		/// <param name="iDestinationRowIndex">0-based integer representing the desired destination row index.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sRow</paramref> does not have the proper number of columns.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iDestinationRowIndex</paramref> is outside the proper range (0 to <c>RowCount</c>).</exception>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void InsertRow(string[] a_sRow, int iDestinationRowIndex) {
				//#### Call .ManageRows to actually insert the passed a_sRow into the g_oData
			ManageRows("InsertRow", true, iDestinationRowIndex, a_sRow);
		}

		///############################################################
		/// <summary>
		/// Appends the passed row onto this instance.
		/// </summary>
		/// <param name="h_sRow">Hashtable of strings representing the string data to insert as the new row.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sRow</paramref> does not have the proper number of columns.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sRow</paramref> contains a column name not defined within this instance.</exception>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void InsertRow(Hashtable h_sRow) {
				//#### Pass the call off to our sibling implementation with the 1-based RowCount (because it's a new trailing entry) as the iDestinationRowIndex to do the actual work
			InsertRow(h_sRow, RowCount);
		}

		///############################################################
		/// <summary>
		/// Inserts the passed row into this instance at the passed destination index.
		/// </summary>
		/// <param name="h_sRow">Hashtable of strings representing the string data to insert as the new row.</param>
		/// <param name="iDestinationRowIndex">0-based integer representing the desired destination row index.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sRow</paramref> does not have the proper number of columns.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sRow</paramref> contains a column name not defined within this instance.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iDestinationRowIndex</paramref> is outside the proper range (0 to <c>RowCount</c>).</exception>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void InsertRow(Hashtable h_sRow, int iDestinationRowIndex) {
			string[] a_sHashKeys;
			string[] a_sNewRow;
			int iColumnCount = ColumnCount;
			int iColumnIndex;
			int i;

				//#### If the passed h_sRow has the proper number of columns
				//####     NOTE: This is also checked for within .ManageRows, but thanks to the .ColumnIndex calls below we need to set iColumnCount = .ColumnCount
			if (h_sRow != null && h_sRow.Count == iColumnCount) {
					//#### Dimension a_sNewRow and a_sHashKeys then copy the a_sHashKeys from the passed h_sRow
				a_sNewRow = new string[iColumnCount];
				a_sHashKeys = new string[iColumnCount];
				h_sRow.Keys.CopyTo(a_sHashKeys, 0);

					//#### Traverse the a_sHashKeys, populating the a_sNewRow with the data from h_sRow as we go
					//####     NOTE: We are copying the passed h_sRow into the local a_sNewRow, setting each of the h_sRow's keys into the correct index within the a_sNewRow
				for (i = 0; i < iColumnCount; i++) {
						//#### Determine the iColumnIndex for the current a_sHashKey
					iColumnIndex = ColumnIndex(a_sHashKeys[i]);

						//#### If the current a_sHashKey was found
					if (iColumnIndex != -1) {
						a_sNewRow[iColumnIndex] = h_sRow[a_sHashKeys[i]].ToString();
					}
						//#### Else the current a_sHashKey was not found above, so raise the error
					else {
						Internationalization.RaiseDefaultError(g_cClassName + "InsertRow", Internationalization.enumInternationalizationValues.cnDeveloperMessages_MultiArray_ColumnNameNotFound, a_sHashKeys[i], "");
					}
				}

					//#### Call .ManageRows to actually insert the above filled a_sNewRow into the g_oData
				ManageRows("InsertRow", true, iDestinationRowIndex, a_sNewRow);
			}
				//#### Else the passed h_sRow does not have the proper number of columns, so raise the error
			else {
			    Internationalization.RaiseDefaultError(g_cClassName + "InsertRow", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_InvalidColumnCount, ColumnCount.ToString(), "");
			}
		}

		///############################################################
		/// <summary>
		/// Appends a blank column onto this instance.
		/// </summary>
		/// <param name="sColumnName">String representing the desired column name.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sColumnName</paramref> is a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sColumnName</paramref> already exists within this instance.</exception>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void InsertColumn(string sColumnName) {
			string[] a_sColumn = new string[RowCount];

				//#### Pass the call off to .ManageColumns, signaling it to insert the new a_sColumn at the right of the existing .Columns
			ManageColumns("InsertColumn", true, ColumnCount, a_sColumn, sColumnName);
		}

		///############################################################
		/// <summary>
		/// Appends the passed column onto this instance.
		/// </summary>
		/// <param name="sColumnName">String representing the desired column name.</param>
		/// <param name="a_sColumn">String array representing the string data to insert as the new column.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sColumnName</paramref> is a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sColumnName</paramref> already exists within this instance.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sColumn</paramref> does not have the proper number of rows.</exception>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void InsertColumn(string sColumnName, string[] a_sColumn) {
				//#### Pass the call off to .ManageColumns, signaling it to insert the a_sNewColumn at the right of the existing .Columns
			ManageColumns("InsertColumn", true, ColumnCount, a_sColumn, sColumnName);
		}

		///############################################################
		/// <summary>
		/// Inserts the passed column into this instance at the passed destination index.
		/// </summary>
		/// <param name="a_sColumn">String array representing the string data to insert as the new column.</param>
		/// <param name="sColumnName">String representing the desired column name.</param>
		/// <param name="iDestinationColumnIndex">0-based integer representing the desired destination column index.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sColumnName</paramref> is a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sColumnName</paramref> already exists within this instance.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sColumn</paramref> does not have the proper number of rows.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iDestinationColumnIndex</paramref> is outside the proper range (0 to <c>ColumnCount</c>).</exception>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void InsertColumn(string sColumnName, string[] a_sColumn, int iDestinationColumnIndex) {
				//#### Pass the call off to .ManageColumns, signaling it to insert the a_sNewColumn at the passed iDestinationColumnIndex
			ManageColumns("InsertColumn", true, iDestinationColumnIndex, a_sColumn, sColumnName);
		}

		///############################################################
		/// <summary>
		/// Removes the referenced row from this instance.
		/// </summary>
		/// <param name="iRowIndex">0-based integer representing the desired row index.</param>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void RemoveRow(int iRowIndex) {
				//#### Pass the call off to .ManageRows, signaling it to remove the iRowIndex
			ManageRows("RemoveRow", false, iRowIndex, null);
		}

		///############################################################
		/// <summary>
		/// Removes the referenced column from this instance.
		/// </summary>
		/// <param name="sColumnName">String representing the desired column name.</param>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void RemoveColumn(string sColumnName) {
				//#### Pass the call off to .ManageRows, signaling it to remove the iRowIndex
			ManageColumns("RemoveColumn", false, ColumnIndex(sColumnName), null, "");
		}

		///############################################################
		/// <summary>
		/// Removes the referenced column from this instance.
		/// </summary>
		/// <param name="iColumnIndex">0-based integer representing the desired column index.</param>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public void RemoveColumn(int iColumnIndex) {
				//#### Pass the call off to .ManageRows, signaling it to remove the iRowIndex
			ManageColumns("RemoveColumn", false, iColumnIndex, null, "");
		}

		///############################################################
		/// <summary>
		/// Renames the referenced column within this instance.
		/// </summary>
		/// <param name="sColumnName">String representing the current column name.</param>
		/// <param name="sNewColumnName">String representing the desired new column name.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sNewColumnName</paramref> is a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sNewColumnName</paramref> already exists within this instance.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sColumnName</paramref> does not exist within this instance.</exception>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		public void RenameColumn(string sColumnName, string sNewColumnName) {
				//#### If the passed sColumnName and sNewColumnName do not match
				//####     NOTE: Although it's somewhat stupid to "rename" a column to the same name (and could be rightly seen as a "duplicate" name an error out), we explicitly ignore it
			if (sColumnName.ToLower() != sNewColumnName.ToLower()) {
					//#### Pass the call off to our sibling implementation
				RenameColumn(ColumnIndex(sColumnName), sNewColumnName);
			}
		}

		///############################################################
		/// <summary>
		/// Renames the referenced column within this instance.
		/// </summary>
		/// <param name="iColumnIndex">0-based integer representing the desired column index.</param>
		/// <param name="sNewColumnName">String representing the desired new column name.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sNewColumnName</paramref> is a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sNewColumnName</paramref> already exists within this instance.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>iColumnIndex</paramref> is outside the proper range (0 to <c>(ColumnCount - 1)</c>).</exception>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		public void RenameColumn(int iColumnIndex, string sNewColumnName) {
				//#### If the passed iColumnIndex is valid (signaling .ValidateColumnIndex to raise the error if the iColumnIndex is invalid)
			if (g_oData.ValidateColumnIndex(g_cClassName + "RenameColumn", iColumnIndex, true)) {
					//####     NOTE: We do not .ToLower() the sNewColumnName here as we want to preserve the casing (as some systems/objects care about the casing), so we need to inline .ToLower() the .ColumnNames as we use them internally (which is just within .ColumnIndex)
				sNewColumnName = Cn.Data.Tools.MakeString(sNewColumnName, "");

					//#### If the passed sNewColumnName is a null-string, raise the error
				if (sNewColumnName.Length == 0) {
					Internationalization.RaiseDefaultError(g_cClassName + "RenameColumn", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "sNewColumnName", "");
				}
					//#### Else if the passed sNewColumnName already exists within this instance
				else if (ColumnIndex(sNewColumnName) != -1) {
						//#### If the user is NOT trying to "rename" the iColumnIndex to its current name, raise the error
						//####     NOTE: Although it's somewhat stupid to "rename" a column to the same name (and could be rightly seen as a "duplicate" name an error out), we explicitly ignore it
					if (iColumnIndex != ColumnIndex(sNewColumnName)) {
						Internationalization.RaiseDefaultError(g_cClassName + "RenameColumn", Internationalization.enumInternationalizationValues.cnDeveloperMessages_MultiArray_DuplicateColumnName, sNewColumnName, "sNewColumnName");
					}
				}
					//#### Else the passed sNewColumnName seems valid, so set the sNewColumnName into g_oData's .ColumnNames
				else {
					g_oData.ColumnNames[iColumnIndex] = sNewColumnName;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Converts the data stored within this instance into its equivalent string representation.
		/// </summary>
		/// <returns>String representing this instance of a MultiArray class.</returns>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public override string ToString() {
		    string[] a_sCurrentRow;
		    string sReturn = "";
		    int iColumnCount = ColumnCount;
		    int iRowCount = RowCount;
		    int i;
		    int j;

				//#### If we have columns to traverse
				//####     NOTE: This should never fail, as sInitialColumnNames are required at init and .Reset and you cannot .Remove the last column
			if (iColumnCount > 0) {
		            //#### Init the sReturn value with the metadata column info
		        sReturn = string.Join(Settings.SecondaryDelimiter, ColumnNames);

                    //#### If we have .Rows to traverse
                if (iRowCount > 0) {
		                //#### Traverse the .Rows
		            for (i = 0; i < iRowCount; i++) {
		                    //#### Collect the current .Row into a_sCurrentRow
//! There is some weird bug here or at the .DelimiterDecoder that in some rare cases results in doubly encoded strings (so ":" = "%58" = "%3758", as the "%" is getting encoded twice)
// Believe that the error was found, as previously only a reference was collected here (instead of a .DeepCopy) so the class data was being re-encoded and re-re-encoded
						a_sCurrentRow = Platform.Specific.DeepCopy(g_oData.Rows[i].Columns);

		                    //#### Traverse the a_sCurrentRow, .DelimiterEncoder'ing each value
		                for (j = 0; j < iColumnCount; j++) {
		                    a_sCurrentRow[j] = Settings.DelimiterEncoder(a_sCurrentRow[j]);
		                }

		                    //#### Append the a_sCurrentRow information onto the sReturn value
		                sReturn += Settings.PrimaryDelimiter + string.Join(Settings.SecondaryDelimiter, a_sCurrentRow);
		            }
                }
		    }

		        //#### Return the above determined sReturn value to the caller
		    return sReturn;
		}

		///############################################################
		/// <summary>
		/// Converts the data stored within this instance into SQL statement(s).
		/// </summary>
		/// <param name="sTableName">String representing the destination table name.</param>
		/// <param name="sIDColumn">String representing the name of the primary key column (this column will not be included in the generated SQL statements).<para/> NOTE: Pass in a null-string if there is no ID column.</param>
		/// <param name="eStatementType">Enumeration representing the desired SQL statement type.</param>
		/// <returns>String array where each index represents a single SQL statment for each row within this instance.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed non-null-string <paramref>sIDColumn</paramref> does not exist within this instance.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eStatementType</paramref> requests an UPDATE statement and the passed <paramref>sIDColumn</paramref> does not exist within this instance.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sTableName</paramref> is a null-string.</exception>
		///############################################################
		/// <LastUpdated>February 19, 2010</LastUpdated>
		public string[] GenerateSQLStatements(string sTableName, string sIDColumn, enumStatementTypes eStatementType) {
		    ColumnDescription[] a_oColumns;
		    string[] a_sCurrentRow;
		    string[] a_sReturn = new string[0];
		    int iColumnCount = ColumnCount;
		    int iRowCount = RowCount;
		    int i;
		    int j;

                //#### Borrow the use of j to determine the .ColumnIndex of the passed sIDColumn and borrow a_sCurrentRow to store the .ColumnNames
            j = ColumnIndex(sIDColumn);
            a_sCurrentRow = g_oData.ColumnNames;

                //#### If the passed sIDColumn was not found above (and is not a null-string), raise the error
            if (j == -1 && Cn.Data.Tools.MakeString(sIDColumn, "").Trim().Length > 0) {
                Internationalization.RaiseDefaultError(g_cClassName + "SQL", Internationalization.enumInternationalizationValues.cnDeveloperMessages_MultiArray_ColumnNameNotFound, "sIDColumn", "");
            }
				//#### Else if the passed sIDColumn was not found above and this is an .cnUpdate request, raise the error
			else if (j == -1 && eStatementType == enumStatementTypes.cnUpdate) {
                Internationalization.RaiseDefaultError(g_cClassName + "SQL", Internationalization.enumInternationalizationValues.cnDeveloperMessages_MultiArray_IDColumnRequired, "sIDColumn", "");
			}
				//#### Else if the passed sTableName is a null-string, raise the error
			else if (Cn.Data.Tools.MakeString(sTableName, "").Trim().Length == 0) {
                Internationalization.RaiseDefaultError(g_cClassName + "SQL", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "sTableName", "");
			}
                //#### Else the passed data seems valid
            else {
                    //#### Dimension the a_oColumns and the a_sReturn value as required
                a_oColumns = new ColumnDescription[iColumnCount];
                a_sReturn = new string[iRowCount];

                    //#### Traverse the .ColumnNames within the borrowed a_sCurrentRow
                for (i = 0; i < iColumnCount; i++) {
						//#### If we are at the above determined ID column (calculating it by comparing i to the .ColumnIndex in the borrowed in j), set the appropriate data into the new SaveType
					if (i == j) {
						a_oColumns[i] = new ColumnDescription(a_sCurrentRow[i], "", true, enumValueOperators.cnWhereEqualTo);
					}
						//#### Else this is .cnInsertIfPresent, so set the appropriate data into the new SaveType 
					else {
						a_oColumns[i] = new ColumnDescription(a_sCurrentRow[i], "", true, enumValueOperators.cnInsertIfPresent);
					}
                }

                    //#### Traverse the .Rows
                for (i = 0; i < iRowCount; i++) {
		                //#### Collect the a_sCurrentRow
		            a_sCurrentRow = g_oData.Rows[i].Columns;

                        //#### Traverse the a_sCurrentRow, populating the a_oColumns' .Values as we go
                    for (j = 0; j < iColumnCount; j++) {
                        a_oColumns[j].Value = a_sCurrentRow[j];
                    }

                        //#### Determine the eStatementType and process accordingly
                    switch (eStatementType) {
                        case enumStatementTypes.cnInsert: {
                            a_sReturn[i] = Statements.Insert(sTableName, a_oColumns);
                            break;
                        }
                        default: { //enumStatementTypes.cnUpdate
                            a_sReturn[i] = Statements.Update(sTableName, a_oColumns);
                            break;
                        }
                    }
                }
            }

		        //#### Return the above determined a_sReturn value to the caller
		    return a_sReturn;
		}


		//##########################################################################################
		//# Public Static Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Determines which column values differ between the provided records.
		/// </summary>
		/// <remarks>
		/// NOTE: All of the column names present within <paramref>h_sCheckRow</paramref> must be present within <paramref>h_sOriginalRow</paramref>, else an error will be thrown. Any additional columns defined within <paramref>h_sOriginalRow</paramref> will be ignored.
		/// <para/>NOTE: A null array is returned if there were no column value differences between the provided records.
		/// </remarks>
		/// <param name="h_sCheckRow">Hashtable that represents the record to check.</param>
		/// <param name="h_sOriginalRow">Hashtable that represents the original record.</param>
		/// <returns>Array of strings where each index represents a column name who's value differed between the provided <paramref>h_sCheckRow</paramref> and <paramref>h_sOriginalRow</paramref>.</returns>
		/// <exception cref="Cn.CnException">Thrown when a null <paramref>h_sCheckRow</paramref> is passed.</exception>
		/// <exception cref="Cn.CnException">Thrown when a null <paramref>h_sOriginalRow</paramref> is passed.</exception>
		/// <exception cref="Cn.CnException">Thrown when a key (column name) present within <paramref>h_sCheckRow</paramref> is not present within <paramref>h_sOriginalRow</paramref>.</exception>
		///############################################################
		/// <LastUpdated>February 19, 2010</LastUpdated>
		public static string[] UpdatedColumns(Hashtable h_sCheckRow, Hashtable h_sOriginalRow) {
			string[] a_sReturn = new string[0];
			string[] a_sKeys;
			int iReturnIndex = 0;
			int i;

				//#### If the passed h_sCheckRow and h_sOriginalRow are not null
			if (h_sCheckRow != null && h_sOriginalRow != null) {
					//#### Dimension the a_sReturn and collect the a_sKeys from the h_sCheckRow
				a_sReturn = new string[h_sCheckRow.Keys.Count];
				a_sKeys = new string[a_sReturn.Length];
				h_sCheckRow.Keys.CopyTo(a_sKeys, 0);

					//#### Traverse the a_sKeys
				for (i = 0; i < a_sKeys.Length; i++) {
						//#### If the current a_sKey is present within the h_sOriginalRow
					if (h_sOriginalRow.Contains(a_sKeys[i])) {
							//#### If the values do not match between the h_sCheckRow and the h_sOriginalRow
						if (h_sCheckRow[a_sKeys[i]].ToString() != h_sOriginalRow[a_sKeys[i]].ToString()) {
								//#### Add the current a_sKey into the sReturn value, then inc iReturnIndex
							a_sReturn[iReturnIndex] = a_sKeys[i];
							iReturnIndex++;
						}
					}
						//#### Else the current a_sKey is not within the h_sOriginalRow
					else {
							//#### Reset the iReturnIndex to 0, raise the error and fall from the loop
							//####     NOTE: We really don't need to reset the iReturnIndex, nor fall from the loop are necessary, but are done below for completeness
						iReturnIndex = 0;
						Internationalization.RaiseDefaultError(g_cClassName + "GetUpdatedColumns", Internationalization.enumInternationalizationValues.cnDeveloperMessages_MultiArray_ColumnNameNotFound, a_sKeys[i], "");
					  //break;
					}
				}

					//#### If updated values were found within the h_sCheckRow above
				if (iReturnIndex > 0) {
						//#### If the iReturnIndex is less then the a_sKeys.Length (meaning less then a full a_sReturn value was populated above)
						//####     NOTE: iReturnIndex was incremented to 1 past the logical ubound, making it logicially 1-based, hence <a_sKeys.Length
					if (iReturnIndex < a_sKeys.Length) {
							//#### Borrow the use of a_sKeys to store the current a_sReturn value
						a_sKeys = a_sReturn;

							//#### Redimension the a_sReturn to the iReturnIndex (which was incremented to 1 past the logical ubound, making it logicially 1-based)
						a_sReturn = new string[iReturnIndex];

							//#### Traverse the filled section of the a_sReturn value (as stored within the borrowed a_sKeys)
						for (i = 0; i < iReturnIndex; i++) {
								//#### Copy the current a_sReturn value from the borrowed a_sKeys back into the redimensioned a_sReturn value
							a_sReturn[i] = a_sKeys[i];
						}
					}
				}
					//#### Else all the values matched between the h_sCheckRow and h_sOriginalRow
				else {
						//#### Reset the a_sReturn value to an empty array (as no updated columns were found)
					a_sReturn = new string[0];
				}
			}
				//#### Else one (or both) of the passed rows were null
			else {
					//#### If it was h_sCheckRow that was null (or if they were both null require h_sCheckRow first), raise the error
				if (h_sCheckRow == null) {
					Internationalization.RaiseDefaultError(g_cClassName + "GetUpdatedColumns", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "h_sCheckRow", "");
				}
					//#### Else it was h_sOriginalRow that was null, so raise the error
				else {
					Internationalization.RaiseDefaultError(g_cClassName + "GetUpdatedColumns", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "h_sOriginalRow", "");
				}
			}

				//#### Return the above determined a_sReturn value to the caller
			return a_sReturn;
		}


		//##########################################################################################
		//# Private Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Manages the <c>Rows</c> for this instance.
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="bInsert">Boolean value signaling if this is an insert request.<para/></param>
		/// <param name="iRowIndex">0-based integer representing the desired row index.</param>
		/// <param name="a_sRow">Array of strings representing the string data to insert as the new row.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sRow</paramref> does not have the proper number of columns.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>bInsert</paramref> is true and the passed <paramref>iRowIndex</paramref> is outside the proper range (0 to (<c>RowCount</c> - 1)).</exception>
		///############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		private void ManageRows(string sFunction, bool bInsert, int iRowIndex, string[] a_sRow) {
			DataContainer.structRow[] a_oCurrentRows;
			DataContainer.structRow[] a_oNewRows = null;
			int iRowCount = RowCount;
			int i;

				//#### If the passed (0-based) iRowIndex is valid (while allowing bInsert's to reference a new 0-based upper index)
				//####     NOTE: .ValiateRowIndex raises the error if the iRowIndex is invalid and we are bInsert'ing
			if ((bInsert && iRowIndex == iRowCount) || g_oData.ValiateRowIndex(g_cClassName + sFunction, iRowIndex, bInsert)) {
					//#### Collect the a_oCurrentRows
				a_oCurrentRows = g_oData.Rows;

					//#### If we are supposed to bInsert the passed a_sRow
				if (bInsert) {
						//#### If the passed a_sRow is of the proper .Length (with 1 entry for each .Column)
					if (a_sRow != null && a_sRow.Length == ColumnCount) {
							//#### Dimension a_oNewRows to allow for the new .Row (hence the +1)
						a_oNewRows = new DataContainer.structRow[iRowCount + 1];

							//#### Insert the passed a_sRow into the a_oNewRow at the passed iRowIndex
						a_oNewRows[iRowIndex].Columns = a_sRow;

							//#### Traverse the upper section of a_oCurrentRows
						for (i = iRowIndex; i < iRowCount; i++) {
								//#### Copy the data form the a_oCurrentRow into the a_oNewRow (accounting for the added .Row as we copy, hence the +1)
							a_oNewRows[i + 1] = a_oCurrentRows[i];
						}
					}
						//#### Else the passed a_sRow was invalid, so raise the error
					else {
						Internationalization.RaiseDefaultError(g_cClassName + "InsertRow", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_InvalidColumnCount, ColumnCount.ToString(), "");
					}
				}
					//#### Else we are supposed to remove the .Row at the passed iRowIndex
				else {
						//#### Dimension a_oNewRows to account for the removed .Row (hence the -1)
					a_oNewRows = new DataContainer.structRow[iRowCount - 1];

						//#### Traverse the upper section of a_oCurrentRows
					for (i = (iRowIndex + 1); i < iRowCount; i++) {
							//#### Copy the data form the a_oCurrentRow into the a_oNewRow (accounting for the removed .Row as we copy, hence the -1)
						a_oNewRows[i - 1] = a_oCurrentRows[i];
					}
				}

					//#### Traverse the lower section of .Rows (hence <iRowIndex)
				for (i = 0; i < iRowIndex; i++) {
						//#### Copy the data form the a_oCurrentRow into the a_oNewRow
					a_oNewRows[i] = a_oCurrentRows[i];
				}

					//#### Replace the a_oCurrentRows with the newly populated a_oNewRows
				g_oData.Rows = a_oNewRows;
			}
		}

		///############################################################
		/// <summary>
		/// Manages the <c>Columns</c> for this instance.
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="bInsert">Boolean value signaling if this is an insert request.<para/></param>
		/// <param name="iColumnIndex">0-based integer representing the desired column index.</param>
		/// <param name="a_sColumn">String array representing the string data to insert as the new column.</param>
		/// <param name="sColumnName">String representing the desired column name.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sColumnName</paramref> is a null-string.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sColumnName</paramref> already exists within this instance.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_sColumn</paramref> does not have the proper number of rows.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>bInsert</paramref> is true and the passed <paramref>iColumnIndex</paramref> is outside the proper range (0 to (<c>ColumnCount</c> - 1)).</exception>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		private void ManageColumns(string sFunction, bool bInsert, int iColumnIndex, string[] a_sColumn, string sColumnName) {
			string[] a_sCurrentColumns;
			string[] a_sNewColumns;
			int iColumnCount = ColumnCount;
			int iRowCount = RowCount;
			int i;
			int j;

				//#### .MakeString the passed sColumnName
				//####     NOTE: We do not .ToLower() the sColumnName here as we want to preserve the casing (as some systems/objects care about the casing), so we need to inline .ToLower() the .ColumnNames as we use them internally (which is just within .ColumnIndex)
			sColumnName = Cn.Data.Tools.MakeString(sColumnName, "");

				//#### If the passed (0-based) iColumnIndex is valid (while allowing bInsert's to reference a new 0-based upper index)
				//####     NOTE: .ValidateColumnIndex raises the error if the iColumnIndex is invalid
			if ((bInsert && iColumnIndex == iColumnCount) || g_oData.ValidateColumnIndex(sFunction, iColumnIndex, bInsert)) {
					//#### If we are not bInsert'ing OR
					//####     The passed a_sColumn is of the proper .Length (with 1 entry for each .Row) and the passed sColumnName does not already .Exist
				if (! bInsert ||
					(
						a_sColumn != null && a_sColumn.Length == iRowCount &&
						sColumnName.Length > 0 && ! Exists(sColumnName)
					)
				) {
						//#### If we are supposed to bInsert the passed sColumnName
					if (bInsert) {
							//#### Borrow the use of a_sNewColumns to store the new .ColumnNames, allowing for the new sColumnName (hence the +1) 
						a_sNewColumns = new string[iColumnCount + 1];

							//#### Insert the new sColumnName into the a_sNewColumns at the passed iColumnIndex
							//####     NOTE: We allow for a iColumnIndex (which is 0-based) equal to iColumnCount (which is 1-based) here thanks to the dimension of a_sNewColumns as [iColumnCount + 1] above
						a_sNewColumns[iColumnIndex] = sColumnName;

							//#### Traverse the upper section of the .ColumnNames
						for (i = iColumnIndex; i < iColumnCount; i++) {
							a_sNewColumns[i + 1] = g_oData.ColumnNames[i];
						}
					}
						//#### Else we are supposed to remove the .Column at the passed iColumnIndex
					else {
							//#### Setup the a_sNewColumns for this loop, accounting for the removed .Column (hence the -1)
						a_sNewColumns = new string[iColumnCount - 1];

							//#### Traverse the upper section of .Columns we are supposed to keep
						for (i = (iColumnIndex + 1); i < iColumnCount; i++) {
								//#### Copy the data form the g_oData.ColumnNames into the a_sNewColumns (accounting for the removed column as we copy, hence the -1)
							a_sNewColumns[i - 1] = g_oData.ColumnNames[i];
						}
					}

						//#### Traverse the lower section of the .ColumnNames
					for (i = 0; i < iColumnIndex; i++) {
						a_sNewColumns[i] = g_oData.ColumnNames[i];
					}

						//#### Reset the .ColumnNames with the above filled a_sNewColumns
					g_oData.ColumnNames = a_sNewColumns;

					//#####
					//#####

						//#### Traverse the .Rows
					for (i = 0; i < iRowCount; i++) {
							//#### Collect the a_sCurrentColumns
						a_sCurrentColumns = g_oData.Rows[i].Columns;

							//#### If we are supposed to bInsert the passed a_sColumn
						if (bInsert) {
								//#### Setup the a_sNewColumns for this loop, allowing for the new .Column (hence the +1)
							a_sNewColumns = new string[iColumnCount + 1];

								//#### Insert the new a_sColumn's data into the a_sNewColumns at the passed iColumnIndex
								//####     NOTE: We allow for a iColumnIndex (which is 0-based) equal to iColumnCount (which is 1-based) here thanks to the dimension of a_sNewColumns as [iColumnCount + 1] above
							a_sNewColumns[iColumnIndex] = Cn.Data.Tools.MakeString(a_sColumn[i], "");

								//#### Traverse the upper section of .Columns
							for (j = iColumnIndex; j < iColumnCount; j++) {
									//#### Copy the data form the a_sCurrentColumns into the a_sNewColumns (accounting for the added column as we copy, hence the +1)
								a_sNewColumns[j + 1] = a_sCurrentColumns[j];
							}
						}
							//#### Else we are supposed to remove the .Column at the passed iColumnIndex
						else {
								//#### Setup the a_sNewColumns for this loop, accounting for the removed .Column (hence the -1)
							a_sNewColumns = new string[iColumnCount - 1];

								//#### Traverse the upper section of .Columns we are supposed to keep
							for (j = (iColumnIndex + 1); j < iColumnCount; j++) {
									//#### Copy the data form the a_sCurrentColumns into the a_sNewColumns (accounting for the removed column as we copy, hence the -1)
								a_sNewColumns[j - 1] = a_sCurrentColumns[j];
							}
						}

							//#### Traverse the lower section of .Columns
						for (j = 0; j < iColumnIndex; j++) {
								//#### Copy the data form the a_sCurrentColumns into the a_sNewColumns
							a_sNewColumns[j] = a_sCurrentColumns[j];
						}

							//#### Replace the a_sCurrentColumns with the newly populated a_sNewColumns for the current .Row
						g_oData.Rows[i].Columns = a_sNewColumns;
					}
				}
					//#### Else the passed a_sColumn or sColumnName was invalid
				else {
						//#### If the caller didn't pass in a sColumnName, raise the error
					if (sColumnName.Length == 0) {
						Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "sColumnName", "");
					}
						//#### Else if it was the sColumnName already .Exists, raise the error
					if (Exists(sColumnName)) {
						Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_MultiArray_DuplicateColumnName, sColumnName, "sColumnName");
					}
						//#### Else it was the a_sColumn that was invalid, so raise the error
					else { 
						Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_InvalidColumnCount, "a_sColumn", RowCount.ToString());
					}
				}
			}
		}

		///############################################################
		/// <summary>
		/// Retrieves or commits the value for the referenced data.
		/// </summary>
		/// <param name="iRowIndex">0-based integer representing the desired row index.</param>
		/// <param name="iColumnIndex">0-based integer representing the desired column index.</param>
		/// <param name="bUpdate">Boolean value signaling if this is an update request.<para/></param>
		/// <param name="sNewValue">String representing the new value for the referenced index.</param>
		/// <returns>String representing the value of the referenced data.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>bUpdate</paramref> is true and the passed <paramref>iRowIndex</paramref> is outside the proper range (0 to (<c>RowCount</c> - 1)).</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>bUpdate</paramref> is true and the passed <paramref>iColumnIndex</paramref> is outside the proper range (0 to (<c>ColumnCount</c> - 1)).</exception>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		private string DoValue(int iRowIndex, int iColumnIndex, bool bUpdate, string sNewValue) {
			string sReturn = "";

				//#### If the passed iRowIndex and iColumnIndex is valid (signaling .ValiateRowIndex and .ValidateColumnIndex to raise errors if this is an bUpdate call)
			if (g_oData.ValiateRowIndex(g_cClassName + "Value", iRowIndex, bUpdate) && g_oData.ValidateColumnIndex(g_cClassName + "Value", iColumnIndex, bUpdate)) {
					//#### If we are supposed to bUpdate the value
				if (bUpdate) {
						//#### Set the sNewValue into the iRowIndex/iColumnIndex
					g_oData.Rows[iRowIndex].Columns[iColumnIndex] = Cn.Data.Tools.MakeString(sNewValue, "");
				}
					//#### Else we're supposed to return the current value to the caller
				else {
						//#### Reset our sReturn value to the data at the iRowIndex/iColumnIndex
					sReturn = g_oData.Rows[iRowIndex].Columns[iColumnIndex];
				}
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

	#endregion


		///########################################################################################################################
		/// <summary>
		/// Internally utilized class structure that represents the data for the each instance.
		/// </summary>
		/// <remarks>
		/// NOTE: This class is only used internally by MultiArray.
		/// </remarks>
		///########################################################################################################################
		/// <LastFullCodeReview>June 4, 2007</LastFullCodeReview>
		private class DataContainer {
				//#### Define the required public variables
				/// <summary>Gets/sets the column names (along with their column indexes) for this instance.</summary>
			public string[] ColumnNames;
				/// <summary>Gets/sets the rows for this instance.</summary>
			public structRow[] Rows;

				//#### Define the required public structs
				/// <summary>Structure that represents a single <c>Row</c>.</summary>
			public struct structRow {
				public string[] Columns;
			}

			///############################################################
			/// <summary>
			/// Determines if the passed index is within the bounds of the <c>Row</c>s.
			/// </summary>
			/// <param name="sFunction">String representing the calling function's name.</param>
			/// <param name="iRowIndex">0-based integer representing the desired row index.</param>
			/// <param name="bRaiseErrors">Boolean value indicating if errors are to be raised if the referenced index is invalid.</param>
			/// <returns>Boolean value signaling if the referenced index is valid.</returns>
			///############################################################
			/// <LastUpdated>January 8, 2010</LastUpdated>
			public bool ValiateRowIndex(string sFunction, int iRowIndex, bool bRaiseErrors) {
					//#### Set our bReturn value based on if we have .Rows and if the passed iRowIndex falls within the proper range
//!				bool bReturn = ((iRowIndex == 0) || (Rows != null && iRowIndex > -1 && iRowIndex < Rows.Length));
				bool bReturn = (Rows != null && iRowIndex > -1 && iRowIndex < Rows.Length);

					//#### If the passed iColumnIndex is invalid and we are supposed to bRaiseErrors, raise the error
				if (! bReturn && bRaiseErrors) {
					Internationalization.RaiseDefaultError(sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_IndexOutOfRange, iRowIndex.ToString(), "0-RowCount");
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}

			///############################################################
			/// <summary>
			/// Determines if the passed index is within the bounds of the <c>Columns</c>s.
			/// </summary>
			/// <param name="sFunction">String representing the calling function's name.</param>
			/// <param name="iColumnIndex">0-based integer representing the desired column index.</param>
			/// <param name="bRaiseErrors">Boolean value indicating if errors are to be raised if the referenced index is invalid.</param>
			/// <returns>Boolean value signaling if the referenced index is valid.</returns>
			///############################################################
			/// <LastUpdated>January 21, 2010</LastUpdated>
			public bool ValidateColumnIndex(string sFunction, int iColumnIndex, bool bRaiseErrors) {
					//#### Set our bReturn value based on if we have .ColumnNames and if the passed iColumnIndex falls within the proper range
				bool bReturn = ((iColumnIndex == 0) || (ColumnNames != null && iColumnIndex > -1 && iColumnIndex < ColumnNames.Length));

					//#### If the passed iColumnIndex is invalid and we are supposed to bRaiseErrors, raise the error
				if (! bReturn && bRaiseErrors) {
					Internationalization.RaiseDefaultError(sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_IndexOutOfRange, iColumnIndex.ToString(), "0-ColumnCount");
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}

		} //# private class DataContainer

	} //# class MultiArray

} //# namespace Cn
