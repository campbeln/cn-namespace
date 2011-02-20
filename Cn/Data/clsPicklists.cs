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
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Data {

    ///########################################################################################################################
    /// <summary>
	/// Represents a collection of picklists.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>May 29, 2007</LastFullCodeReview>
	public class Picklists {
            //#### Declare the required private variables
        private MultiArray g_oData;
        private static PicklistCollectionHelper g_oGetData;
		private bool g_bStrictDecodes;

            //#### Declare the required private eNums
		private enum enumProcessPicklistModes : int {
			cnGetItems,
			cnDecoder,
			cnEncoder,
			cnExists
		}

            //#### Declare the required private constants
		private const string g_cClassName = "Cn.Data.Picklists.";
		private const string g_cColumnNames = "ID,PicklistID,DisplayOrder,Data,Description,IsActive";	//# NOTE: The ID column is utilized internally by Data.Management.Picklists, so it's required
		private const string g_sBaseSQL = "SELECT " + g_cColumnNames + " FROM $TableName ORDER BY PicklistID, DisplayOrder";
		private const string g_sDefaultTableName = "cnPicklists";
		private const string g_cColumnAssoPicklistName = "_PicklistColumnAssociations";
		private const string g_cMetaDataPicklistName = "_PicklistMetaData";


        //##########################################################################################
        //# Class Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
		/// Initializes the class.
        /// </summary>
        /// <param name="oPicklistData"><c>MultiArray</c> representing the entire, self-referencing picklists structure.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklistData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklistData</paramref> does not contain the required column names as defined by <c>GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklistData</paramref> contains one or more <c>Rows</c> with a non-numeric value in 'PicklistID'.</exception>
        ///############################################################
		/// <LastUpdated>May 10, 2007</LastUpdated>
		public Picklists(MultiArray oPicklistData) {
				//#### Pass the call off to .DoReset
			DoReset("[Constructor]", oPicklistData);
		}

        ///############################################################
        /// <summary>
		/// Resets the class to its initilized state.
        /// </summary>
        /// <param name="oPicklistData"><c>MultiArray</c> representing the entire, self-referencing picklists structure.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklistData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklistData</paramref> does not contain the required column names as defined by <c>GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklistData</paramref> contains one or more <c>Rows</c> with a non-numeric value in 'PicklistID'.</exception>
        ///############################################################
		/// <LastUpdated>May 10, 2007</LastUpdated>
		public void Reset(MultiArray oPicklistData) {
				//#### Pass the call off to .DoReset
			DoReset("Reset", oPicklistData);
		}

        ///############################################################
        /// <summary>
		/// Resets the class to its initilized state while loading the provided picklist data into this instance.
        /// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
        /// <param name="oPicklistData"><c>MultiArray</c> representing the entire, self-referencing picklists structure.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklistData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklistData</paramref> does not contain the required column names as defined by <c>GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklistData</paramref> contains one or more <c>Rows</c> with a non-numeric value in 'PicklistID'.</exception>
        ///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		private void DoReset(string sFunction, MultiArray oPicklistData) {
			string sCurrentValue;
			int i;
			bool bErrorOccured = false;

				//#### (Re)Init the global variables
			g_bStrictDecodes = false;

                //#### If the passed oPicklistData contains no .Rows (or is null), raise the error
			if (oPicklistData == null || oPicklistData.RowCount == 0) {
			    Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_NoDataToLoad, "oPicklistData", "");
			}
                //#### Else we've got some data to process
			else {
                    //#### If the passed oPicklistData contains the .RequiredColumns
				if (oPicklistData.Exists(GetData.RequiredColumns)) {
						//#### Traverse the rows within the passed oPicklistData
					for (i = 0; i < oPicklistData.RowCount; i++) {
							//#### Collect the sCurrentValue for this loop
						sCurrentValue = oPicklistData.Value(i, "PicklistID");

                            //#### If the current PicklistID is non-null-string and non-numeric, set bErrorOccured to true, raise the error and exit the loop
						if (! Cn.Data.Tools.IsNumber(sCurrentValue)) {
							bErrorOccured = true;
			                Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_Picklists_InvalidPicklistID, "", "");
						  //break;
						}
					}
				}
                    //#### Else the passed oPicklistData did not contain the required columns, so set bErrorOccured to true and raise the error
                else {
					bErrorOccured = true;
			        Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MissingRequiredColumns, "oPicklistData", "GetData.SQL");
				}

                    //#### If no bError(s)Occured above
                    //####     NOTE: This is kinda stupid as the .RaiseDefaultError calls above should stop execution, but just in case it doesn't...
			    if (! bErrorOccured) {
                        //#### Do a deep copy of the data within the passed oPicklistData into the global g_oData
				    g_oData = oPicklistData.Data;
			    }
			}
		}


		//##########################################################################################
		//# Public Read-Write Properties
		//##########################################################################################
        ///############################################################
        /// <summary>
        /// Gets/sets a boolean value indicating if this instance utilizes strict decodes.
        /// </summary>
		/// <remarks>
		/// When "strict decodes" is enabled, only values present within the referenced picklist are returned from <c>Decode</c> calls.
		/// <para/>In "strict decodes" mode: If the passed value to decode is not within the referenced picklist, a null-string is returned.
		/// <para/>In non-"strict decodes" mode: If the passed value to decode is not within the referenced picklist, the passed value is returned.
		/// </remarks>
		/// <value>Boolean value signaling if this instance utilizes strict decodes by default.<para/>Returns true if this instance utilizes strict decodes by default, or false if it does not.</value>
        ///############################################################
		/// <LastUpdated>August 25, 2005</LastUpdated>
		public bool StrictDecodes {
			get { return g_bStrictDecodes; }
			set { g_bStrictDecodes = value; }
		}


        //##########################################################################################
        //# Public Read-Only Properties
        //##########################################################################################
        ///############################################################
        /// <summary>
		/// Assists in the collection of the underlying structure which defines this instance.
        /// </summary>
		/// <value>PicklistCollectionHelper instance configured to collect the underlying structure which defines this instance.</value>
        ///############################################################
		/// <LastUpdated>January 14, 2010</LastUpdated>
		public static PicklistCollectionHelper GetData {
			get {
					//#### If g_oGetData hasn't be setup, do so now
				if (g_oGetData == null) {
					g_oGetData = new PicklistCollectionHelper(g_cColumnNames.Split(','), g_sBaseSQL, g_sDefaultTableName);
				}

				return g_oGetData;
			}
		}

        ///############################################################
        /// <summary>
        /// Gets a deep copy of the underlying structure which defines this instance.
        /// </summary>
		/// <value>Deep copy of the MultiArray which defines the picklists of this instance.</value>
        ///############################################################
		/// <LastUpdated>December 18, 2009</LastUpdated>
		public MultiArray Data {
			get {
			    MultiArray oReturn = null;

					//#### If the global g_oData is not null
				if (g_oData != null) {
						//#### Make a deep copy of the g_oData it into our oReturn value
					oReturn = g_oData.Data;
				}

                    //#### Return the above determined oReturn value to the caller
				return oReturn;
			}
		}

		///############################################################
		/// <summary>
		/// Gets the name of the column associations picklist, which defines the default column name to picklist relationships.
		/// </summary>
		/// <value>String representing the name of the column associations picklist.</value>
		///############################################################
		/// <LastUpdated>May 25, 2007</LastUpdated>
		public static string ColumnAssociationsPicklistName {
			get { return g_cColumnAssoPicklistName; }
		}

		///############################################################
		/// <summary>
		/// Gets the name of the meta data picklist, which defines all other picklists within the collection.
		/// </summary>
		/// <value>String representing the name of the meta data picklist.</value>
		///############################################################
		/// <LastUpdated>May 25, 2007</LastUpdated>
		public static string MetaDataPicklistName {
			get { return g_cMetaDataPicklistName; }
		}


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
        ///############################################################
		/// <summary>
		/// Determines if this instance contains the referenced picklist.
		/// </summary>
		/// <param name="sPicklistName">String representing the picklist name to locate.</param>
		/// <returns>Boolean value signaling the existance of the passed <paramref>sPicklistName</paramref>.<para/>Returns true if the passed <paramref>sPicklistName</paramref> is defined within this instance, or false if it is not present.</returns>
        ///############################################################
		/// <LastUpdated>September 12, 2005</LastUpdated>
		public bool Exists(string sPicklistName) {
			int iJunk = 0;

                //#### Return based on if the passed sPicklistName is found within g_oData
			return (GetPicklistID(sPicklistName, ref iJunk) != -1);
		}

        ///############################################################
		/// <summary>
		/// Determines if this instance contains the referenced picklist/decode value pair.
		/// </summary>
		/// <param name="sPicklistName">String representing the picklist name to locate.</param>
		/// <param name="sDecodeValue">String representing the value to locate.</param>
		/// <returns>Boolean value signaling the existance of the passed <paramref>sPicklistName</paramref>/<paramref>sDecodeValue</paramref> pair.<para/>Returns true if the <paramref>sPicklistName</paramref>/<paramref>sDecodeValue</paramref> pair is defined within this instance, or false if it is not present.</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public bool Exists(string sPicklistName, string sDecodeValue) {
			string[] a_sDecodeValues = new string[1];
			string[] a_sJunk = null;
			MultiArray oJunk = null;
			int iStartingIndex = -1;
			int iEndingIndex = -1;

				//#### Place the passed sDecodeValue into the local a_sDecodeValues
			a_sDecodeValues[0] = sDecodeValue;

				//#### Determine the iStartingIndex and iEndingIndex for the passed sPicklistName
				//####     NOTE: We do not test the return value of .GetLogicalPicklist as the oReturn value is not properly set until after .ProcessPicklist
			GetLogicalPicklist(sPicklistName, ref iStartingIndex, ref iEndingIndex);

				//#### Pass the call off to .ProcessPicklist (collecting it's bReturn value as our own)
			return ProcessPicklist(g_oData, a_sDecodeValues, g_bStrictDecodes, iStartingIndex, iEndingIndex, enumProcessPicklistModes.cnExists, ref a_sJunk, ref oJunk);
		}

        ///############################################################
		/// <summary>
		/// Determines if this instance contains all of the passed decode values within the referenced picklist.
		/// </summary>
		/// <param name="sPicklistName">String representing the picklist name to locate.</param>
		/// <param name="a_sDecodeValues">String array representing the values to decode.</param>
		/// <returns>Boolean value signaling if all of the passed <paramref>a_sDecodeValues</paramref> are present within the <paramref>sPicklistName</paramref>.<para/>Returns true if all of the <paramref>a_sDecodeValues</paramref> were present within the <paramref>sPicklistName</paramref>, or false if one or more <paramref>a_sDecodeValues</paramref> were not present.</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public bool Exists(string sPicklistName, string[] a_sDecodeValues) {
			string[] a_sJunk = null;
			MultiArray oJunk = null;
			int iStartingIndex = -1;
			int iEndingIndex = -1;

				//#### Determine the iStartingIndex and iEndingIndex for the passed sPicklistName
				//####     NOTE: We do not test the return value of .GetLogicalPicklist as the oReturn value is not properly set until after .ProcessPicklist
			GetLogicalPicklist(sPicklistName, ref iStartingIndex, ref iEndingIndex);

				//#### Pass the call off to .ProcessPicklist (collecting it's bReturn value as our own)
			return ProcessPicklist(g_oData, a_sDecodeValues, g_bStrictDecodes, iStartingIndex, iEndingIndex, enumProcessPicklistModes.cnExists, ref a_sJunk, ref oJunk);
		}

        ///############################################################
		/// <summary>
		/// Determines if the passed structure contains the referenced decode value.
		/// </summary>
		/// <param name="oPicklist">MultiArray representing the single picklist to search.</param>
		/// <param name="sDecodeValue">String representing the value to decode.</param>
        /// <param name="bStrictDecodes">Boolean value signaling if the decode process is strict.</param>
		/// <returns>Boolean value signaling the existance of the passed <paramref>sDecodeValue</paramref> within the passed <paramref>oPicklist</paramref>.<para/>Returns true if <paramref>sDecodeValue</paramref> is defined within <paramref>oPicklist</paramref>, or false if it is not present.</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public static bool Exists(MultiArray oPicklist, string sDecodeValue, bool bStrictDecodes) {
			string[] a_sDecodeValues = new string[1];
			string[] a_sJunk = null;
			MultiArray oJunk = null;
			int iEndingIndex = (oPicklist == null ? -1 : oPicklist.RowCount - 1);

				//#### Place the passed sDecodeValue into the local a_sDecodeValues
			a_sDecodeValues[0] = sDecodeValue;

				//#### Pass the call off to .ProcessPicklist (returning it's bReturn value as our own)
				//####     NOTE: We do not test the validity of the passed oPicklist as the oReturn value is not properly set until after .ProcessPicklist
			return ProcessPicklist(oPicklist, a_sDecodeValues, bStrictDecodes, 0, iEndingIndex, enumProcessPicklistModes.cnExists, ref a_sJunk, ref oJunk);
		}

        ///############################################################
		/// <summary>
		/// Determines if the passed structure contains all of the passed decode values within the referenced picklist.
		/// </summary>
		/// <param name="oPicklist">MultiArray representing the single picklist to search.</param>
		/// <param name="a_sDecodeValues">String array representing the values to decode.</param>
        /// <param name="bStrictDecodes">Boolean value signaling if the decode process is strict.</param>
		/// <returns>Boolean value signaling if all of the passed <paramref>a_sDecodeValues</paramref> are present within the <paramref>sPicklistName</paramref>.<para/>Returns true if all of the <paramref>a_sDecodeValues</paramref> were present within the <paramref>sPicklistName</paramref>, or false if one or more <paramref>a_sDecodeValues</paramref> were not present.</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public static bool Exists(MultiArray oPicklist, string[] a_sDecodeValues, bool bStrictDecodes) {
			string[] a_sJunk = null;
			MultiArray oJunk = null;
			int iEndingIndex = (oPicklist == null ? -1 : oPicklist.RowCount - 1);

				//#### Pass the call off to .ProcessPicklist (returning it's bReturn value as our own)
				//####     NOTE: We do not test the validity of the passed oPicklist as the oReturn value is not properly set until after .ProcessPicklist
			return ProcessPicklist(oPicklist, a_sDecodeValues, bStrictDecodes, 0, iEndingIndex, enumProcessPicklistModes.cnExists, ref a_sJunk, ref oJunk);
		}

        ///############################################################
        /// <summary>
        /// Encodes a specific value utilizing the referenced picklist.
        /// </summary>
		/// <param name="sPicklistName">String representing the picklist name to query.</param>
		/// <param name="sEncodeValue">String representing the value to encode.</param>
        /// <returns>String value containing the encoded <paramref>sEncodeValue</paramref> as defined within the passed <paramref>sPicklistName</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>April 16, 2010</LastUpdated>
		public string Encoder(string sPicklistName, string sEncodeValue) {
			string[] a_sEncodeValues = new string[1];
			string[] a_sReturn = null;
			MultiArray oJunk = null;
			int iStartingIndex = -1;
			int iEndingIndex = -1;

				//#### Place the passed sEncodeValue into the local a_sEncodeValues
			a_sEncodeValues[0] = sEncodeValue;

				//#### Determine the iStartingIndex and iEndingIndex for the passed sPicklistName
				//####     NOTE: We do not test the return value of .GetLogicalPicklist as the oReturn value is not properly set until after .ProcessPicklist
			GetLogicalPicklist(sPicklistName, ref iStartingIndex, ref iEndingIndex);

				//#### Pass the call off to .ProcessPicklist (where it updates our a_sReturn value byref)
			ProcessPicklist(g_oData, a_sEncodeValues, g_bStrictDecodes, iStartingIndex, iEndingIndex, enumProcessPicklistModes.cnEncoder, ref a_sReturn, ref oJunk);

				//#### Return the above determined a_sReturn[0] value to the caller (MakeString'd s we go)
			return Cn.Data.Tools.MakeString(a_sReturn[0], "");
		}

        ///############################################################
        /// <summary>
        /// Encodes the provided values utilizing the referenced picklist.
        /// </summary>
		/// <param name="sPicklistName">String representing the picklist name to query.</param>
		/// <param name="a_sEncodeValues">String array representing the values to encode.</param>
        /// <returns>String array containing the encoded <paramref>a_sEncodeValues</paramref> as defined within the passed <paramref>sPicklistName</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>April 16, 2010</LastUpdated>
		public string[] Encoder(string sPicklistName, string[] a_sEncodeValues) {
			string[] a_sReturn = null;
			MultiArray oJunk = null;
			int iStartingIndex = -1;
			int iEndingIndex = -1;

				//#### Determine the iStartingIndex and iEndingIndex for the passed sPicklistName
				//####     NOTE: We do not test the return value of .GetLogicalPicklist as the oReturn value is not properly set until after .ProcessPicklist
			GetLogicalPicklist(sPicklistName, ref iStartingIndex, ref iEndingIndex);

				//#### Pass the call off to .ProcessPicklist (where it updates our a_sReturn value byref)
			ProcessPicklist(g_oData, a_sEncodeValues, g_bStrictDecodes, iStartingIndex, iEndingIndex, enumProcessPicklistModes.cnEncoder, ref a_sReturn, ref oJunk);

				//#### Return the above determined a_sReturn value to the caller
			return a_sReturn;
		}

        ///############################################################
        /// <summary>
        /// Encodes a specific value utilizing the passed structure (optionally utilizing a strict encode).
        /// </summary>
		/// <param name="oPicklist">MultiArray representing the single picklist to search.</param>
		/// <param name="sEncodeValue">String representing the value to encode.</param>
        /// <param name="bStrictEncodes">Boolean value signaling if the encode process is strict.</param>
        /// <returns>String value containing the encoded <paramref>sEncodeValue</paramref> as defined within the passed <paramref>sPicklistName</paramref> (optionally utilizing a strict encode).</returns>
        ///############################################################
		/// <LastUpdated>April 16, 2010</LastUpdated>
		public static string Encoder(MultiArray oPicklist, string sEncodeValue, bool bStrictEncodes) {
			string[] a_sEncodeValues = new string[1];
			string[] a_sReturn = null;
			MultiArray oJunk = null;
			int iEndingIndex = (oPicklist == null ? -1 : oPicklist.RowCount - 1);

				//#### Place the passed sEncodeValue into the local a_sEncodeValues
			a_sEncodeValues[0] = sEncodeValue;

				//#### Pass the call off to .ProcessPicklist (where it updates our a_sReturn value byref)
			ProcessPicklist(oPicklist, a_sEncodeValues, bStrictEncodes, 0, iEndingIndex, enumProcessPicklistModes.cnEncoder, ref a_sReturn, ref oJunk);

				//#### Return the above determined a_sReturn[0] value to the caller (MakeString'd s we go)
			return Cn.Data.Tools.MakeString(a_sReturn[0], "");
		}

        ///############################################################
        /// <summary>
        /// Encodes the provided values utilizing the passed structure (optionally utilizing strict encodes).
        /// </summary>
		/// <param name="oPicklist">MultiArray representing the single picklist to search.</param>
		/// <param name="a_sEncodeValues">String array representing the values to encode.</param>
        /// <param name="bStrictEncodes">Boolean value signaling if the encode process is strict.</param>
        /// <returns>String array containing the encoded <paramref>a_sEncodeValues</paramref> as defined within the passed <paramref>oPicklist</paramref> (optionally utilizing strict encodes).</returns>
        ///############################################################
		/// <LastUpdated>April 16, 2010</LastUpdated>
		public static string[] Encoder(MultiArray oPicklist, string[] a_sEncodeValues, bool bStrictEncodes) {
			string[] a_sReturn = null;
			MultiArray oJunk = null;
			int iEndingIndex = (oPicklist == null ? -1 : oPicklist.RowCount - 1);

				//#### Pass the call off to .ProcessPicklist (where it updates our a_sReturn value byref)
				//####     NOTE: We do not test the validity of the passed oPicklist as the oReturn value is not properly set until after .ProcessPicklist
			ProcessPicklist(oPicklist, a_sEncodeValues, bStrictEncodes, 0, iEndingIndex, enumProcessPicklistModes.cnEncoder, ref a_sReturn, ref oJunk);

				//#### Return the above determined a_sReturn value to the caller
			return a_sReturn;
		}

        ///############################################################
        /// <summary>
        /// Decodes a specific value utilizing the referenced picklist.
        /// </summary>
		/// <param name="sPicklistName">String representing the picklist name to query.</param>
		/// <param name="sDecodeValue">String representing the value to decode.</param>
        /// <returns>String value containing the decoded <paramref>sDecodeValue</paramref> as defined within the passed <paramref>sPicklistName</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public string Decoder(string sPicklistName, string sDecodeValue) {
			string[] a_sDecodeValues = new string[1];
			string[] a_sReturn = null;
			MultiArray oJunk = null;
			int iStartingIndex = -1;
			int iEndingIndex = -1;

				//#### Place the passed sDecodeValue into the local a_sDecodeValues
			a_sDecodeValues[0] = sDecodeValue;

				//#### Determine the iStartingIndex and iEndingIndex for the passed sPicklistName
				//####     NOTE: We do not test the return value of .GetLogicalPicklist as the oReturn value is not properly set until after .ProcessPicklist
			GetLogicalPicklist(sPicklistName, ref iStartingIndex, ref iEndingIndex);

				//#### Pass the call off to .ProcessPicklist (where it updates our a_sReturn value byref)
			ProcessPicklist(g_oData, a_sDecodeValues, g_bStrictDecodes, iStartingIndex, iEndingIndex, enumProcessPicklistModes.cnDecoder, ref a_sReturn, ref oJunk);

				//#### Return the above determined a_sReturn[0] value to the caller (MakeString'd s we go)
			return Cn.Data.Tools.MakeString(a_sReturn[0], "");
		}

        ///############################################################
        /// <summary>
        /// Decodes the provided values utilizing the referenced picklist.
        /// </summary>
		/// <param name="sPicklistName">String representing the picklist name to query.</param>
		/// <param name="a_sDecodeValues">String array representing the values to decode.</param>
        /// <returns>String array containing the decoded <paramref>a_sDecodeValues</paramref> as defined within the passed <paramref>sPicklistName</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public string[] Decoder(string sPicklistName, string[] a_sDecodeValues) {
			string[] a_sReturn = null;
			MultiArray oJunk = null;
			int iStartingIndex = -1;
			int iEndingIndex = -1;

				//#### Determine the iStartingIndex and iEndingIndex for the passed sPicklistName
				//####     NOTE: We do not test the return value of .GetLogicalPicklist as the oReturn value is not properly set until after .ProcessPicklist
			GetLogicalPicklist(sPicklistName, ref iStartingIndex, ref iEndingIndex);

				//#### Pass the call off to .ProcessPicklist (where it updates our a_sReturn value byref)
			ProcessPicklist(g_oData, a_sDecodeValues, g_bStrictDecodes, iStartingIndex, iEndingIndex, enumProcessPicklistModes.cnDecoder, ref a_sReturn, ref oJunk);

				//#### Return the above determined a_sReturn value to the caller
			return a_sReturn;
		}

        ///############################################################
        /// <summary>
        /// Decodes a specific value utilizing the passed structure (optionally utilizing a strict decode).
        /// </summary>
		/// <param name="oPicklist">MultiArray representing the single picklist to search.</param>
		/// <param name="sDecodeValue">String representing the value to decode.</param>
        /// <param name="bStrictDecodes">Boolean value signaling if the decode process is strict.</param>
        /// <returns>String value containing the decoded <paramref>sDecodeValue</paramref> as defined within the passed <paramref>sPicklistName</paramref> (optionally utilizing a strict decode).</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public static string Decoder(MultiArray oPicklist, string sDecodeValue, bool bStrictDecodes) {
			string[] a_sDecodeValues = new string[1];
			string[] a_sReturn = null;
			MultiArray oJunk = null;
			int iEndingIndex = (oPicklist == null ? -1 : oPicklist.RowCount - 1);

				//#### Place the passed sDecodeValue into the local a_sDecodeValues
			a_sDecodeValues[0] = sDecodeValue;

				//#### Pass the call off to .ProcessPicklist (where it updates our a_sReturn value byref)
			ProcessPicklist(oPicklist, a_sDecodeValues, bStrictDecodes, 0, iEndingIndex, enumProcessPicklistModes.cnDecoder, ref a_sReturn, ref oJunk);

				//#### Return the above determined a_sReturn[0] value to the caller (MakeString'd s we go)
			return Cn.Data.Tools.MakeString(a_sReturn[0], "");
		}

        ///############################################################
        /// <summary>
        /// Decodes the provided values utilizing the passed structure (optionally utilizing strict decodes).
        /// </summary>
		/// <param name="oPicklist">MultiArray representing the single picklist to search.</param>
		/// <param name="a_sDecodeValues">String array representing the values to decode.</param>
        /// <param name="bStrictDecodes">Boolean value signaling if the decode process is strict.</param>
        /// <returns>String array containing the decoded <paramref>a_sDecodeValues</paramref> as defined within the passed <paramref>oPicklist</paramref> (optionally utilizing strict decodes).</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public static string[] Decoder(MultiArray oPicklist, string[] a_sDecodeValues, bool bStrictDecodes) {
			string[] a_sReturn = null;
			MultiArray oJunk = null;
			int iEndingIndex = (oPicklist == null ? -1 : oPicklist.RowCount - 1);

				//#### Pass the call off to .ProcessPicklist (where it updates our a_sReturn value byref)
				//####     NOTE: We do not test the validity of the passed oPicklist as the oReturn value is not properly set until after .ProcessPicklist
			ProcessPicklist(oPicklist, a_sDecodeValues, bStrictDecodes, 0, iEndingIndex, enumProcessPicklistModes.cnDecoder, ref a_sReturn, ref oJunk);

				//#### Return the above determined a_sReturn value to the caller
			return a_sReturn;
		}

        ///############################################################
        /// <summary>
		/// Retrieves a structure containing the single entry for a specific decode value within the referenced picklist.
        /// </summary>
		/// <param name="sPicklistName">String representing the picklist name to locate.</param>
		/// <param name="sDecodeValue">String representing the value to locate.</param>
		/// <returns>MultiArray containing the single entry for the passed <paramref>sDecodeValue</paramref> within the passed <paramref>sPicklistName</paramref> defined within this instance, or null if it was not defined.</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public MultiArray Items(string sPicklistName, string sDecodeValue) {
			string[] a_sDecodeValues = new string[1];
			string[] a_sJunk = null;
			MultiArray oReturn = null;
			int iStartingIndex = -1;
			int iEndingIndex = -1;

				//#### Place the passed sDecodeValue into the local a_sDecodeValues
			a_sDecodeValues[0] = sDecodeValue;

				//#### Determine the iStartingIndex and iEndingIndex for the passed sPicklistName
				//####     NOTE: We do not test the return value of .GetLogicalPicklist as the oReturn value is not properly set until after .ProcessPicklist
			GetLogicalPicklist(sPicklistName, ref iStartingIndex, ref iEndingIndex);

				//#### Pass the call off to .ProcessPicklist (where it updates our oReturn value byref)
			ProcessPicklist(g_oData, a_sDecodeValues, g_bStrictDecodes, iStartingIndex, iEndingIndex, enumProcessPicklistModes.cnGetItems, ref a_sJunk, ref oReturn);

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

        ///############################################################
        /// <summary>
		/// Retrieves a structure containing the entries for the provided decode values utilizing the referenced picklist.
        /// </summary>
		/// <param name="sPicklistName">String representing the picklist name to locate.</param>
		/// <param name="a_sDecodeValues">String array representing the values to locate.</param>
		/// <returns>MultiArray containing the entries for the passed <paramref>a_sDecodeValues</paramref> as defined within the passed <paramref>sPicklistName</paramref> defined within this instance, or null if it was not defined.</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public MultiArray Items(string sPicklistName, string[] a_sDecodeValues) {
			string[] a_sJunk = null;
			MultiArray oReturn = null;
			int iStartingIndex = -1;
			int iEndingIndex = -1;

				//#### Determine the iStartingIndex and iEndingIndex for the passed sPicklistName
				//####     NOTE: We do not test the return value of .GetLogicalPicklist as the oReturn value is not properly set until after .ProcessPicklist
			GetLogicalPicklist(sPicklistName, ref iStartingIndex, ref iEndingIndex);

				//#### Pass the call off to .ProcessPicklist (where it updates our oReturn value byref)
			ProcessPicklist(g_oData, a_sDecodeValues, g_bStrictDecodes, iStartingIndex, iEndingIndex, enumProcessPicklistModes.cnGetItems, ref a_sJunk, ref oReturn);

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

        ///############################################################
        /// <summary>
		/// Retrieves a structure containing the single entry for a specific decode value within the passed structure.
        /// </summary>
		/// <param name="oPicklist">MultiArray representing the single picklist to search.</param>
		/// <param name="sDecodeValue">String representing the value to locate.</param>
        /// <param name="bStrictDecodes">Boolean value signaling if the decode process is strict.</param>
		/// <returns>MultiArray containing the single entry for the passed <paramref>sDecodeValue</paramref> within the passed <paramref>oPicklist</paramref>, or null if it was not defined.</returns>
        ///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		public static MultiArray Items(MultiArray oPicklist, string sDecodeValue, bool bStrictDecodes) {
			string[] a_sDecodeValues = new string[1];
			string[] a_sJunk = null;
			MultiArray oReturn = null;
			int iEndingIndex = (oPicklist == null ? -1 : oPicklist.RowCount - 1);

				//#### Place the passed sDecodeValue into the local a_sDecodeValues
			a_sDecodeValues[0] = sDecodeValue;

				//#### Pass the call off to .ProcessPicklist (where it updates our oReturn value byref)
				//####     NOTE: We do not test the validity of the passed oPicklist as the oReturn value is not properly set until after .ProcessPicklist
			ProcessPicklist(oPicklist, a_sDecodeValues, bStrictDecodes, 0, iEndingIndex, enumProcessPicklistModes.cnGetItems, ref a_sJunk, ref oReturn);

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

        ///############################################################
        /// <summary>
		/// Retrieves a structure containing the entries for the provided decode values utilizing the passed structure.
        /// </summary>
		/// <param name="oPicklist">MultiArray representing the single picklist to search.</param>
		/// <param name="a_sDecodeValues">String array representing the values to locate.</param>
        /// <param name="bStrictDecodes">Boolean value signaling if the decode process is strict.</param>
		/// <returns>MultiArray containing the entries for the passed <paramref>a_sDecodeValues</paramref> as defined within the passed <paramref>oPicklist</paramref>, or null if it was not defined.</returns>
        ///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		public static MultiArray Items(MultiArray oPicklist, string[] a_sDecodeValues, bool bStrictDecodes) {
			string[] a_sJunk = null;
			MultiArray oReturn = null;
			int iEndingIndex = (oPicklist == null ? -1 : oPicklist.RowCount - 1);

				//#### Pass the call off to .ProcessPicklist (where it updates our oReturn value byref)
				//####     NOTE: We do not test the validity of the passed oPicklist as the oReturn value is not properly set until after .ProcessPicklist
			ProcessPicklist(oPicklist, a_sDecodeValues, bStrictDecodes, 0, iEndingIndex, enumProcessPicklistModes.cnGetItems, ref a_sJunk, ref oReturn);

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

        ///############################################################
		/// <summary>
		/// Retrieves a structure containing the entries for the referenced picklist.
		/// </summary>
		/// <param name="sPicklistName">String representing the picklist name to locate.</param>
		/// <returns>MultiArray containing all the entries for the passed <paramref>sPicklistName</paramref> defined within this instance, or null if it was not defined.</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		public MultiArray Picklist(string sPicklistName) {
			MultiArray oReturn;
			int iStartingIndex = -1;
			int iEndingIndex = -1;
			int i;

				//#### Init the oReturn value
			oReturn = new MultiArray(g_oData.ColumnNames);

				//#### Determine the iStartingIndex and iEndingIndex for the passed sPicklistName
				//####     NOTE: We do not test the return value of .GetLogicalPicklist as the oReturn value is not properly set until after .ProcessPicklist
			GetLogicalPicklist(sPicklistName, ref iStartingIndex, ref iEndingIndex);

				//#### If the sPicklistName was found above
			if (iStartingIndex != -1 && iStartingIndex < iEndingIndex) {
					//#### Traverse the specified section of the passed g_oData, .Insert(ing each)Row as we go
				for (i = iStartingIndex; i <= iEndingIndex; i++) {
					oReturn.InsertRow(g_oData.Row(i));
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

        ///############################################################
		/// <summary>
		/// Retrieves a structure containing the passed collections of data and description values in a single picklist-like form.
		/// </summary>
		/// <param name="a_sDataValues">String array representing the encoded values.</param>
		/// <param name="a_sDescriptionValues">String array representing the decoded value equivalents.</param>
		/// <returns>
		/// MultiArray containing the passed <paramref>a_sDataValues</paramref>/<paramref>a_sDescriptionValues</paramref> paired entries in a virtual picklist.
		/// <para/>NOTE: The <paramref>a_sDataValues</paramref>/<paramref>a_sDescriptionValues</paramref> are paired off based on their related array indexes (i.e. <paramref>a_sDataValues[2]</paramref>paramref> is paired up with <paramref>a_sDescriptionValues[2]</paramref>, etc.). 
		/// <para/>NOTE: The values for the 'ID', 'PicklistID', and 'DisplayOrder' columns are filled with junk data (hence being returned in a 'single picklist-like form'). The returned structure is in no way related to any loaded picklist collection.
		/// </returns>
		/// <exception cref="Cn.CnException">Thrown when <paramref>a_sDataValues</paramref> is null or empty.</exception>
		/// <exception cref="Cn.CnException">Thrown when <paramref>a_sDescriptionValues</paramref> is null or empty.</exception>
		/// <exception cref="Cn.CnException">Thrown when the dimensions of <paramref>a_sDataValues</paramref> and <paramref>a_sDescriptionValues</paramref> do not match.</exception>
        ///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		public static MultiArray Picklist(string[] a_sDataValues, string[] a_sDescriptionValues) {
			MultiArray oReturn = null;
			Hashtable h_sRow = new Hashtable();
			int i;

				//#### If the passed a_sDataValues is null or empty, raise the error
			if (a_sDataValues == null || a_sDataValues.Length == 0) {
				Internationalization.RaiseDefaultError(g_cClassName + "Picklist", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "a_sDataValues", "");
			}
				//#### Else if the passed a_sDescriptionValues is null or empty, raise the error
			else if (a_sDescriptionValues == null || a_sDescriptionValues.Length == 0) {
				Internationalization.RaiseDefaultError(g_cClassName + "Picklist", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "a_sDescriptionValues", "");
			}
				//#### Else if the passed a_sDataValues or a_sDescriptionValues are not the same dimensions, raise the error
			else if (a_sDataValues.Length != a_sDescriptionValues.Length) {
				Internationalization.RaiseDefaultError(g_cClassName + "Picklist", Internationalization.enumInternationalizationValues.cnDeveloperMessages_Picklists_DataDescriptionDimensions, "", "");
			}
				//#### Else the passed a_sDataValues/a_sDescriptionValues are valid
			else {
					//#### Reset the oReturn value
				oReturn = new MultiArray(GetData.RequiredColumns);

					//#### Fill the h_sRow's ID, PicklistID and DisplayOrder column's with junk data
					//####     NOTE: We can get away with filling the PicklistID and DisplayOrder column's with junk data because those columns are not utilized within the passed oReturn .Decoder function (actually, the DisplayOrder column is never directly used by the Picklists class at all).
				h_sRow["ID"] = "1";
				h_sRow["PicklistID"] = "1";
				h_sRow["DisplayOrder"] = "1";
				h_sRow["IsActive"] = "1";

					//#### Traverse the passed a_sDataValues/a_sDescriptionValues
				for (i = 0; i < a_sDataValues.Length; i++) {
						//#### Repopulate the Data and Description columns, then Insert(the modified)Row into the oReturn
					h_sRow["Data"] = a_sDataValues[i];
					h_sRow["Description"] = a_sDescriptionValues[i];
					oReturn.InsertRow(h_sRow);
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}


        //#######################################################################################################
        //# Private Functions
        //#######################################################################################################
        ///############################################################
        /// <summary>
        /// Returns the PicklistID of the referenced picklist from within the g_oData MultiArray
        /// </summary>
		/// <param name="sPicklistName">String representing the picklist name to locate.</param>
        /// <param name="i">Reference to an integer which is returned pre-incremented to the MultiArray 0-based Row index to begin any subsequent searches.</param>
        /// <returns>Integer representing the picklist id of the passed <paramref>sPicklistName</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		private int GetPicklistID(string sPicklistName, ref int i) {
			string sCurrentData;
			int iReturn = -1;
			int iLen;
			bool bCurrentIsActive;

				//#### Ensure the passed sPicklistName is a string, then determine it's .Length
			sPicklistName = Cn.Data.Tools.MakeString(sPicklistName, "");
			iLen = sPicklistName.Length;

				//#### Traverse the .Rows of g_oData
			for (i = 0; i < g_oData.RowCount; i++) {
					//#### If we are still within the PicklistID #0 system list
				if (Cn.Data.Tools.MakeInteger(g_oData.Value(i, "PicklistID"), -1) == 0) {
						//#### Set the sCurrentData and bCurrentIsActive
					sCurrentData = g_oData.Value(i, "Data");
					bCurrentIsActive = Cn.Data.Tools.MakeBoolean(g_oData.Value(i, "IsActive"), true);

						//#### If the sCurrentData matches the passed sPicklistName (checking their .Lengths first as it is a faster comparison)
					if (iLen == sCurrentData.Length) {
							//#### If we are in g_bStrictDecodes mode, compare sPicklistName and sCurrentData as is (and the bCurrentIsActive is true), else .ToLower before comparing
						if ((g_bStrictDecodes && bCurrentIsActive && sPicklistName == sCurrentData) ||
							(! g_bStrictDecodes && sPicklistName.ToLower() == sCurrentData.ToLower())
						) {
								//#### Set our iReturn value to the current DisplayOrder field (which represents its child picklist's PicklistID), inc the ByRef i to the next .Row within the g_oData for the caller and exit the loop
							iReturn = Cn.Data.Tools.MakeInteger(g_oData.Value(i, "DisplayOrder"), -1);
							i++;
							break;
						}
					}
				}
					//#### Else we ran out of picklist definitions, so reset i and exit the loop
				else {
					i = 0;
					break;
				}
			}

				//#### If the found iPicklistID is 0, reset the preincremented i to 0 (so that the entire metadata picklist is checked by the caller)
			if (iReturn == 0) {
				i = 0;
			}

			    //#### Return the above determined iReturn value to the caller
			return iReturn;
		}

		///############################################################
		/// <summary>
		/// Determines the starting index and count of picklist items of the referenced picklist within this instance's data.
		/// </summary>
		/// <param name="sPicklistName">String representing the picklist name to locate.</param>
		/// <param name="iStartingIndex">0-based integer representing the picklist items starting index (updated by reference).</param>
		/// <param name="iEndingIndex">0-based integer representing the picklist items ending index (updated by reference).</param>
		///############################################################
		/// <LastUpdated>February 3, 2010</LastUpdated>
		private void GetLogicalPicklist(string sPicklistName, ref int iStartingIndex, ref int iEndingIndex) {
			int iCurrentPicklistID;
			int iPicklistID;
			int i = 0;
			bool bFound = false;

				//#### Collect the iPicklistID (as well as the i iterator so we don't have to re-search the picklist metadata)
			iPicklistID = GetPicklistID(sPicklistName, ref i);

				//#### Init the byref values for iStartingIndex and iEndingIndex
			iStartingIndex = -1;
			iEndingIndex = -1;

				//#### If the iPicklistID was found
			if (iPicklistID != -1) {
					//#### Traverse the .Rows of g_oData (starting at the above preincremented i)
				for (/*i = i*/; i < g_oData.RowCount; i++) {
						//#### Determine the iCurrentPicklistID
					iCurrentPicklistID = Cn.Data.Tools.MakeInteger(g_oData.Value(i, "PicklistID"), -1);

						//#### If the iCurrentPicklistID is less then the iPicklistID we're looking for, do nothing
					if (iCurrentPicklistID < iPicklistID) {
					}
						//#### Else if this is the iPicklistID we're looking for
					else if (iCurrentPicklistID == iPicklistID) {
							//#### If this is the first time we've found the iPicklistID, set the byref iStartingIndex to i
						if (! bFound) {
							iStartingIndex = i;
						}
						
							//#### Reset the byref iEndingIndex to the current value of i and reset bFound to true
						iEndingIndex = i;
						bFound = true;
					}
						//#### Else we've passed by the iPicklistID we were looking for, so exit the loop
					else {
						break;
					}
				}
			}
		}

        ///############################################################
        /// <summary>
        /// Processes the referenced picklist based on the provided mode.
        /// </summary>
		/// <param name="oPicklist">MultiArray representing the picklist data to search.</param>
		/// <param name="a_sDecodeValues">String array representing the values to decode.</param>
        /// <param name="bStrictDecodes">Boolean value signaling if the decode process is strict.</param>
		/// <param name="iStartingIndex">0-based integer representing the picklist items starting index.</param>
		/// <param name="iEndingIndex">0-based integer representing the picklist items ending index.</param>
		/// <param name="eMode">Enumeration representing the mode of processing to undertake against the provided picklist items.</param>
		/// <param name="a_sDecodedValues">String array representing the decoded values (updated by reference).</param>
		/// <param name="oGetItems">MultiArray representing the initilized picklist items collection (updated by reference).</param>
		/// <returns>Boolean value indicating if all of the passed <paramref>a_sDecodeValues</paramref> were found within the referenced picklist items.</returns>
        ///############################################################
		/// <LastUpdated>April 19, 2010</LastUpdated>
		private static bool ProcessPicklist(MultiArray oPicklist, string[] a_sDecodeValues, bool bStrictDecodes, int iStartingIndex, int iEndingIndex, enumProcessPicklistModes eMode, ref string[] a_sDecodedValues, ref MultiArray oGetItems) {
			string sCurrentValue;
			string sCurrentData;
			int iDecodeValuesLen;
			int iCurrentValueLen;
			int i;
			int j;
			bool bCurrentIsActive;
			bool bFound;
			bool bReturn;
			bool bEncode = (eMode == enumProcessPicklistModes.cnEncoder);

				//#### If the passed a_sDecodeValues or oPicklist is empty, or if the indexes are invalid
			if (a_sDecodeValues == null || a_sDecodeValues.Length == 0 ||
				oPicklist == null || oPicklist.RowCount == 0 ||
				iStartingIndex == -1 || iStartingIndex > iEndingIndex
			) {
					//#### Set the bReturn value to false
				bReturn = false;

					//#### Determine the eMode, resetting the byref values accordingly
				switch (eMode) {
					case enumProcessPicklistModes.cnGetItems: {
							//#### If the passed oPicklist is null, reset the byref oGetItems to the default .RequiredColumns
						if (oPicklist == null) {
							oGetItems = new MultiArray(GetData.RequiredColumns);
						}
							//#### Else reset the byref oGetItems to the passed oPicklist's .ColumnNames (which may include more the just the standard .RequiredColumns)
						else {
							oGetItems = new MultiArray(oPicklist.ColumnNames);
						}
						break;
					}

					case enumProcessPicklistModes.cnEncoder:
					case enumProcessPicklistModes.cnDecoder: {
							//#### Reset the a_sDecodedValues to the .Length of the passed a_sDecodeValues (if any)
						a_sDecodedValues = new string[ (a_sDecodeValues == null ? 0 : a_sDecodeValues.Length) ];

							//#### If we to enforce bStrictDecodes
						if (bStrictDecodes) {
								//#### Traverse the a_sDecodedValues, resetting each index to a null-string
							for (i = 0; i < a_sDecodedValues.Length; i++) {
								a_sDecodedValues[i] = "";
							}
						}
							//#### Else if a_sDecodeValues is valid, .DeepCopy the a_sDecodeValues into the byref a_sDecodedValues
						else if (a_sDecodeValues != null) {
							a_sDecodedValues = Platform.Specific.DeepCopy(a_sDecodeValues);
						}
						break;
					}
				}
			}
				//#### Else we have a_sDecodeValues and a oPicklist to process
			else {
					//#### Default the bReturn value to true (as any missed values reset it to false below) and determine the iDecodeValuesLen
				bReturn = true;
				iDecodeValuesLen = a_sDecodeValues.Length;

					//#### Determine the eMode and process accordingly
				switch (eMode) {
					case enumProcessPicklistModes.cnGetItems: {
							//#### Init the byref oGetItems MultiArray
						oGetItems = new MultiArray(oPicklist.ColumnNames);
						break;
					}

					case enumProcessPicklistModes.cnEncoder:
					case enumProcessPicklistModes.cnDecoder: {
							//#### Dimension the byref a_sDecodedValues array based on the iDecodeValuesLen
						a_sDecodedValues = new string[iDecodeValuesLen];
						break;
					}
				}

					//#### Traverse the a_sDecodeValues
				for (i = 0; i < iDecodeValuesLen; i++) {
						//#### Set the sCurrentValue, it's iCurrentValueLen and reset bFound for this (outer) loop
					sCurrentValue = Cn.Data.Tools.MakeString(a_sDecodeValues[i], "");
					iCurrentValueLen = sCurrentValue.Length;
					bFound = false;

						//#### Traverse the specified section of the passed oPicklist
					for (j = iStartingIndex; j <= iEndingIndex; j++) {
							//#### Set the sCurrentData and bCurrentIsActive for this (inner) loop
						sCurrentData = (bEncode ? oPicklist.Value(j, "Description") : oPicklist.Value(j, "Data"));
						bCurrentIsActive = Cn.Data.Tools.MakeBoolean(oPicklist.Value(j, "IsActive"), true);

							//#### If the sCurrentValue matches the current sCurrentData (checking their .Lengths first as that is a far faster comparison)
						if (iCurrentValueLen == sCurrentData.Length) {
								//#### If we are in g_bStrictDecodes mode, compare sCurrentValue and sCurrentData as is (and the bCurrentIsActive is true), else .ToLower before comparing
							if ((bStrictDecodes && bCurrentIsActive && sCurrentValue == sCurrentData) ||
								(! bStrictDecodes && sCurrentValue.ToLower() == sCurrentData.ToLower())
							) {
									//#### Determine the eMode and process accordingly
								switch (eMode) {
									case enumProcessPicklistModes.cnGetItems: {
										oGetItems.InsertRow(oPicklist.Row(j));
										break;
									}
									case enumProcessPicklistModes.cnDecoder: {
										a_sDecodedValues[i] = oPicklist.Value(j, "Description");
										break;
									}
									case enumProcessPicklistModes.cnEncoder: {
										a_sDecodedValues[i] = oPicklist.Value(j, "Data");
										break;
									}
								}

									//#### Flip bFound to true and exit the (inner) loop
								bFound = true;
								break;
							}
						}
					}

						//#### If the sCurrentValue was not bFound above
					if (! bFound) {
							//#### Reset the bReturn value to false as the current a_sDecodeValue was not found above
						bReturn = false;

							//#### If we are in .cnDecoder eMode
						if (eMode == enumProcessPicklistModes.cnDecoder) {
								//#### If this is a bStrictDecodes, fill the current index with a null-string
							if (bStrictDecodes) {
								a_sDecodedValues[i] = "";
							}
								//#### Else we are not supposed to do bStrictDecodes, so copy the sCurrentValue into the byref a_sDecodedValues
							else {
								a_sDecodedValues[i] = Cn.Data.Tools.MakeString(sCurrentValue, "");
							}
						}
					}
				}
			}

				//#### Return the above determined bReturn value to the caller (if any)
			return bReturn;
		}


		///########################################################################################################################
		/// <summary>
		/// Assists in the collection of <c>Picklist</c> data.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview>May 29, 2007</LastFullCodeReview>
		public class PicklistCollectionHelper : CollectionHelper {

				//#### Declare the required public structs
//! should this be a class, so the required fields are enforced?!
				/// <summary>Structure representing the table (or stored procedure) information for where to collect the picklist definitions.</summary>
			public struct structPicklistTable {
					/// <summary>Gets/sets a string value representing the name of this picklist.</summary>
				public string PicklistName;
					/// <summary>Gets/sets a string value representing the table (or stored procedure) name where the picklist definition is represented.</summary>
				public string TableName;
					/// <summary>Gets/sets a string value representing the column name where the value that is saved by refering tables is represented (generally the primary key column).</summary>
				public string DataColumnName;
					/// <summary>Gets/sets a string value representing the column name where the value that is displayed is represented.</summary>
				public string DescriptionColumnName;
					/// <summary>Gets/sets a string value representing the column name where the value indicating if this is an active entry is represented.</summary>
				public string IsActiveColumnName;
					/// <summary>Gets/sets a string value representing the SQL Order By clause to be used when collecting the picklist definition.</summary>
				public string OrderByClause;
					/// <summary>Gets/sets a boolean value signaling if the <c>TableName</c> field is actually the name of a stored procedure.</summary>
				public bool TableNameIsStoredProcedure;
			}


			//##########################################################################################
			//# Class Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			///############################################################
			/// <LastUpdated>December 24, 2009</LastUpdated>
			public PicklistCollectionHelper(string[] a_sRequiredColumns, string sBaseSQL, string sDefaultTableName) : base(a_sRequiredColumns, sBaseSQL, sDefaultTableName) {
			}


			//##########################################################################################
			//# Public Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Retrieves the data for the parent type based on the provided information and data source.
			/// </summary>
			/// <param name="oDBMS"><c>DBMS</c> instance representing an active connection to the related data source.</param>
			/// <param name="a_oPicklistTables">Array of <c>structPicklistTables</c> representing the table (or stored procedure) information for where to collect the picklist definitions.</param>
			/// <returns><c>MultiArray</c> instance based on the provided information and data source.</returns>
			///############################################################
			/// <LastUpdated>December 24, 2009</LastUpdated>
			public MultiArray Data(DBMS oDBMS, structPicklistTable[] a_oPicklistTables) {
				MultiArray[] a_oResults;
				SQL.OrderByClause oOrderByClause = new SQL.OrderByClause();
				MultiArray oPicklists = null;
				Hashtable h_sResultsRow;
				Hashtable h_sRow = new Hashtable();
				string[] a_sSQL;
				int iLen;
				int i;
				int j;

					//#### If we have some a_oPicklistTables to traverse
				if (a_oPicklistTables != null && a_oPicklistTables.Length > 0) {
						//#### Borrow the use of a_sSQL to collect the stock-standard Picklists.GetColumnNames, then init our local oPicklists accordingly
					a_sSQL = RequiredColumns;
					oPicklists = new MultiArray(a_sSQL);

						//#### Traverse the borrowed a_sSQL (which is holding the Picklists.GetColumnNames), .Add'ing in each row into our h_sRow as we go (resetting IsActive to "TRUE" when we're done)
					for (i = 0; i < a_sSQL.Length; i++) {
						h_sRow.Add(a_sSQL[i], "0");
					}
					h_sRow["IsActive"] = "TRUE";

						//#### Determine the iLen of the passed a_oPicklistTables and (re)dimension the a_sSQL accordingly
					iLen = a_oPicklistTables.Length;
					a_sSQL = new string[iLen];

						//#### Traverse the passed a_oPicklistTables
					for (i = 0; i < iLen; i++) {
							//#### Define and insert the metadata picklist entry for this index, setting the DisplayOrder (or child PicklistID in the metadata picklist's case) to i+1 and the Data (or PicklistName) to the .TableName
						h_sRow["DisplayOrder"] = (i + 1);
						h_sRow["Data"] = a_oPicklistTables[i].PicklistName;
						oPicklists.InsertRow(h_sRow);

							//#### If the .TableNameIs(a)StoredProcedure name instead, set this index's a_sSQL statement accordingly
						if (Tools.MakeBoolean(a_oPicklistTables[i].TableNameIsStoredProcedure, false)) {
							a_sSQL[i] = a_oPicklistTables[i].TableName + "; ";
						}
							//#### Else the .TableNameIs(not a)StoredProcedure name, so set this index's a_sSQL statement accordingly
						else {
								//#### .Load this index's .OrderByClause into our oOrderByClause
//! this may be useful in both cases?
							oOrderByClause.Load(a_oPicklistTables[i].OrderByClause);

								//#### If this index doesn't have a .IsActiveColumnName specified, reset it to 'TRUE' so it is properly collected below
							if (a_oPicklistTables[i].IsActiveColumnName.Length == 0) {
								a_oPicklistTables[i].IsActiveColumnName = "'TRUE'";
							}

								//#### Build the a_sSQL statement for the current index
							a_sSQL[i] = "SELECT " +
									"'" + (i + 1) + "' AS PicklistID, " +
									a_oPicklistTables[i].DataColumnName + " AS Data, " +
									a_oPicklistTables[i].DescriptionColumnName + " AS Description, " +
									a_oPicklistTables[i].IsActiveColumnName + " AS IsActive " +
								"FROM " + a_oPicklistTables[i].TableName + " " +
								oOrderByClause.ToString(true, false) +
							"; ";
						}
					}

						//#### Collect our a_oResults based on the above generated a_sSQL
						//####     NOTE: This is not as efficient as utilizing the native DataSet-ish objects, but this approach allows us to not have to modify this code from platform to platform (or version to version)
					a_oResults = oDBMS.GetMultiArrays(string.Join("", a_sSQL));

						//#### If the a_oResults were successfully collected above
					if (a_oResults != null) {
							//#### Reset the h_sRow's DisplayOrder to "0" in prep for the loop below
						h_sRow["DisplayOrder"] = "0";

							//#### Traverse the a_oResults
						for (i = 0; i < a_oResults.Length; i++) {
								//#### Traverse the current a_oResults' .Rows
							for (j = 0; j < a_oResults[i].RowCount; i++) {
									//#### Reset the h_sResultsRow for this index
								h_sResultsRow = a_oResults[i].Row(j);

									//#### Define and insert the metadata picklist entry for this oDataRow
								h_sRow["PicklistID"] = h_sResultsRow["PicklistID"];
							  //h_sRow["DisplayOrder"] = "0";
								h_sRow["Data"] = h_sResultsRow["Data"];
								h_sRow["Description"] = h_sResultsRow["Description"];
								h_sRow["IsActive"] = h_sResultsRow["IsActive"];
								oPicklists.InsertRow(h_sRow);
							}
						}
					}
				}

					//#### Return the above determined oPicklists value to the caller
				return oPicklists;
			}


		} //# public class PicklistCollectionHelper


	} //# class Picklists


} //# namespace Database
