/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Data {

    ///########################################################################################################################
    /// <summary>
	/// Represents a datasource's metadata description (names, data types, etc.).
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>September 12, 2005</LastFullCodeReview>
	public class MetaData {
		#region MetaData
            //#### Declare the required private variables
        private MultiArray g_oData;
        private Picklists g_oPicklists;
        private static MetaDataCollectionHelper g_oGetData;

            //#### Declare the required public eNums
        #region eNums
			/// <summary>Generalized metadata types.</summary>
		public enum enumDataTypes : int {           //# NOTE: Any changes/additions made here must also be made to rfDataValidation.js's DataTypes_*
				/// <summary>Unknown data type.</summary>
			cnUnknown = 0,
				/// <summary>Boolean data type.</summary>
				/// <remarks>
				/// NOTE: <c>Input</c>s defined as <c>cnBoolean</c>s always return values based on <see cref="Cn.Data.Tools.MakeBooleanInteger">MakeBooleanInteger</see>, where zero equates to false and non-zero equates to true.
				/// </remarks>
			cnBoolean = 1,
				/// <summary>Integer (whole number) data type.</summary>
			cnInteger = 2,
				/// <summary>Floating point data type.</summary>
			cnFloat = 4,
				/// <summary>Floating point currency data type.</summary>
			cnCurrency = 8,
				/// <summary>"Short" character data type.<para/>"Short" character types are defined as character types where the data source allows "equal to" and "not equal to" comparisons on the columns.</summary>
			cnChar = 16,
				/// <summary>"Long" character data type.<para/>"Long" character types are defined as character types where the data source does not allow "equal to" and "not equal to" comparisons on the columns.</summary>
			cnLongChar = 32,
				/// <summary>Date/time data type.</summary>
			cnDateTime = 64,
				/// <summary>Binary (file-based) data type.</summary>
			cnBinary = 128,
				/// <summary>GUID (global unique identifier) data type.</summary>
			cnGUID = 256,

            //cn? = 512,
            //cn? = 1024,
            //cn? = 2048,
            //cn? = 4096,
            //cn? = 8192,
            //cn? = 16384,

				/// <summary>Currently unsupported data type.</summary>
			cnUnsupported = 32768
		}
			/// <summary>Tracked metadata descriptions.</summary>
			/// <seealso cref="Cn.Data.MetaData.enumDataTypes"/>
		public enum enumMetaDataTypes : int {
				/// <summary>Table name.</summary>
			cnTable_Name = 0,
				/// <summary>Column name.</summary>
			cnColumn_Name = 1,
				/// <summary>Default column value.</summary>
			cnColumn_Default = 2,
				/// <summary>Column can contain NULL.</summary>
			cnIs_Nullable = 3,
				/// <summary>Generalized column data type as defined within enumDataTypes.</summary>
			cnData_Type = 4,
				/// <summary>Maximum number of characters allowed.</summary>
			cnCharacter_Maximum_Length = 5,
				/// <summary>Maximum number of significant digits allowed.</summary>
			cnNumeric_Precision = 6,
					/// <summary>Numeric scale (number of digits the decimal point has been shifted).</summary>
			cnNumeric_Scale = 7,

				/// <summary>String representing the minimum allowed numeric value.</summary>
			cnMinimumNumericValue = 8,
				/// <summary>String representing the maximum allowed numeric value.</summary>
			cnMamimumNumericValue = 9,
				/// <summary>String representing the name of the <c>Picklist</c> related to the column.</summary>
			cnRelatedPicklist = 10
		}
			/// <summary>Value types.</summary>
		public enum enumValueTypes : int {
				/// <summary>Single value.</summary>
			cnSingleValue = 0,
				/// <summary>Verbatum type.<para/>Value is not automaticially validated for range, length or data type.</summary>
			cnVerbatum = 1,

				/// <summary>Single value from picklist.<para/>Value refers to a single related enumerated value.</summary>
			cnSingleValueFromPicklist = 2,
				/// <summary>Multiple values from picklist.<para/>Common delimited value refers to one or more related enumerated values.</summary>
			cnMultiValuesFromPicklist = 4

				//#### NOTE: Values 1024, 2048, 4096, 8192, 16384, 32768, etc are reserved for use in enums that leverage this enum
			//cn? = 8,
			//cn? = 16,
			//cn? = 32,
			//cn? = 64,
			//cn? = 128,
			//cn? = 256,
			//cn? = 512
		}
			/// <summary>Form input errors.</summary>
		public enum enumValueErrorTypes : int {			//# NOTE: Any changes/additions made here must also be made to Validation.js's Errors_*
				/// <summary>No errors have occured.</summary>
			cnNoError = 0,
				/// <summary>Submitted value is too long.</summary>
			cnIncorrectLength = 1,
				/// <summary>Submitted value is of an incorrect type.</summary>
			cnIncorrectDataType = 2,
				/// <summary>A value is required for this input.</summary>
			cnValueIsRequired = 3,
				/// <summary>Submitted value is not within the picklist.</summary>
			cnNotWithinPicklist = 4,
				/// <summary>Input data type is of an unknown or unsupported type (so Renderer cannot validate its value).</summary>
			cnUnknownOrUnsupportedType = 5,
				/// <summary>Custom error (developer should define their own error message).</summary>
			cnCustom = 6,
				/// <summary>A defined input is missing from the HTML form.</summary>
			cnMissingInput = 7,
				/// <summary>Multiple values were submitted for an input defined as a single value.</summary>
			cnMultipleValuesSubmittedForSingleValue = 8
		}
		#endregion

			//#### Declare the required private structs
		private struct structMinMaxDataType {
			public string MinimumValue;
			public string MaximumValue;
			public enumDataTypes DataType;
		}

            //#### Declare the required private constants
		private const string g_cClassName = "Cn.Data.MetaData.";


        //##########################################################################################
        //# Class Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
		/// Initializes the class.
        /// </summary>
        /// <param name="oDataSourceMetaData">MultiArray representing the entire metadata structure.</param>
		/// <param name="eDataSource">Enumeration representing the source the <paramref>oDataSourceMetaData</paramref> was generated from.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eDataSource</paramref> is unreconized.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> does not contain the required column names as defined by <c>GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a non-boolean value in 'Is_Nullable'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a null-string value in 'Table_Name' or 'Column_Name'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a non-numeric, non null-string value in 'MinimumNumericValue' or 'MamimumNumericValue'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a mismatched pair of MinimumNumericValue/MamimumNumericValue (a pair must both be numeric or both be null-strings).</exception>
        ///############################################################
		/// <LastUpdated>May 9, 2007</LastUpdated>
		public MetaData(MultiArray oDataSourceMetaData, enumDataSource eDataSource) {
				//#### Pass the call off to .DoReset
			DoReset("[Constructor]", oDataSourceMetaData, eDataSource);
		}

        ///############################################################
        /// <summary>
		/// Resets the class to its initilized state.
        /// </summary>
        /// <param name="oDataSourceMetaData">MultiArray representing the entire metadata structure.</param>
		/// <param name="eDataSource">Enumeration representing the source the <paramref>oDataSourceMetaData</paramref> was generated from.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eDataSource</paramref> is unreconized.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> does not contain the required column names as defined by <c>GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a non-boolean value in 'Is_Nullable'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a null-string value in 'Table_Name' or 'Column_Name'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a non-numeric, non null-string value in 'MinimumNumericValue' or 'MamimumNumericValue'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a mismatched pair of MinimumNumericValue/MamimumNumericValue (a pair must both be numeric or both be null-strings).</exception>
        ///############################################################
		/// <LastUpdated>May 9, 2007</LastUpdated>
		public void Reset(MultiArray oDataSourceMetaData, enumDataSource eDataSource) {
				//#### Pass the call off to .DoReset
			DoReset("Reset", oDataSourceMetaData, eDataSource);
		}

        ///############################################################
        /// <summary>
        /// Resets the class to its initilized state while loading the provided metadata into this instance.
        /// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
        /// <param name="oDataSourceMetaData">MultiArray representing the entire metadata structure.</param>
		/// <param name="eDataSource">Enumeration representing the source the <paramref>oDataSourceMetaData</paramref> was generated from.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eDataSource</paramref> is unreconized.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> does not contain the required column names as defined by <c>GetColumnNames</c>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a non-boolean value in 'Is_Nullable'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a null-string value in 'Table_Name' or 'Column_Name'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a non-numeric, non null-string value in 'MinimumNumericValue' or 'MamimumNumericValue'.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oDataSourceMetaData</paramref> contains one or more <c>Rows</c> with a mismatched pair of MinimumNumericValue/MamimumNumericValue (a pair must both be numeric or both be null-strings).</exception>
        ///############################################################
		/// <LastUpdated>January 25, 2010</LastUpdated>
		private void DoReset(string sFunction, MultiArray oDataSourceMetaData, enumDataSource eDataSource) {
			structMinMaxDataType oMinMaxDataType;
			string sMinimumNumericValue;
			string sMamimumNumericValue;
			string sDataType;
			int iRowCount = -1;
			int i;
			bool bErrorOccured = false;

				//#### Reset the value of g_oPicklists
			g_oPicklists = null;

                //#### If the passed oDataSourceMetaData has no .Rows (or is null), raise the error
			if (oDataSourceMetaData == null || oDataSourceMetaData.RowCount == 0) {
				Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_NoDataToLoad, "oDataSourceMetaData", "");
			}
                //#### Else we've got some data to process
			else {
                    //#### If the passed oDataSourceMetaData contains the required columns
				if (oDataSourceMetaData.Exists(GetData.RequiredColumns)) {
                        //#### Do a deep copy of the data within the passed oDataSourceMetaData into the global g_oData, then determine its iRowCount
					g_oData = oDataSourceMetaData.Data;
					iRowCount = g_oData.RowCount;

						//#### If any/all of the .OptionalColummns do not .Exists
					if (! g_oData.Exists(GetData.OptionalColummns)) {
							//#### Traverse the .OptionalColummns, .Insert('ing each)Column that does not .Exists
						for (i = 0; i < GetData.OptionalColummns.Length; i++) {
							if (! g_oData.Exists(GetData.OptionalColummns[i])) {
								g_oData.InsertColumn(GetData.OptionalColummns[i]);
							}
						}
					}

                        //#### Traverse each .Row within the g_oData
					for (i = 0; i < iRowCount; i++) {
                            //#### Ensure that the string types are properly .Trim'd and set the values of sMinimumNumericValue, sMamimumNumericValue and sDataType
						g_oData.Value(i, "Table_Name", g_oData.Value(i, "Table_Name").Trim());
						g_oData.Value(i, "Column_Name", g_oData.Value(i, "Column_Name").Trim());
						g_oData.Value(i, "Data_Type", g_oData.Value(i, "Data_Type").Trim());
						g_oData.Value(i, "RelatedPicklist", g_oData.Value(i, "RelatedPicklist").Trim());
						sMinimumNumericValue = g_oData.Value(i, "MinimumNumericValue");
						sMamimumNumericValue = g_oData.Value(i, "MamimumNumericValue");
						sDataType = g_oData.Value(i, "Data_Type");

                            //#### If the current Is_Nullable is non-boolean, raise the error, set bErrorOccured to true and exit the loop (even though the raised error should stop execution)
						if (! Cn.Data.Tools.IsBoolean(g_oData.Value(i, "Is_Nullable"))) {
							Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbMetaData_InvalidIsNullable, "oDataSourceMetaData", "");
							bErrorOccured = true;
							break;
						}
                            //#### Else if the current Table_Name or Column_Name is blank, raise the error, set bErrorOccured to true and exit the loop (even though the raised error should stop execution)
						else if (g_oData.Value(i, "Table_Name").Length == 0 || g_oData.Value(i, "Column_Name").Length == 0) {
							Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbMetaData_BlankTableOrColumnName, "oDataSourceMetaData", "");
							bErrorOccured = true;
							break;
						}
                            //#### Else if the current MinimumNumericValue/MamimumNumericValue isn't a null-string and .Is(n't)Numeric, raise the error, set bErrorOccured to true and exit the loop (even though the raised error should stop execution)
						else if (
						    (sMinimumNumericValue.Length > 0 && ! Cn.Data.Tools.IsNumber(sMinimumNumericValue)) ||
						    (sMamimumNumericValue.Length > 0 && ! Cn.Data.Tools.IsNumber(sMamimumNumericValue))
						) {
							Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbMetaData_InvalidMinMamimumNumericValue, "oDataSourceMetaData", "");
							bErrorOccured = true;
							break;
						}
						    //#### Else if the caller defined only one of the MinimumNumericValue/MinimumNumericValues, raise the error, set bErrorOccured to true and exit the loop (even though the raised error should stop execution)
						else if (
						    (sMinimumNumericValue.Length == 0 && sMamimumNumericValue.Length > 0) ||
						    (sMinimumNumericValue.Length > 0 && sMamimumNumericValue.Length == 0)
						) {
							Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbMetaData_MissingMinMamimumNumericValue, "oDataSourceMetaData", "");
							bErrorOccured = true;
							break;
						}
                            //#### Else if the current Data_Type is blank, raise the error, set bErrorOccured to true and exit the loop (even though the raised error should stop execution)
						else if (sDataType.Length == 0) {
							Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbMetaData_InvalidDataType, "oDataSourceMetaData", "");
							bErrorOccured = true;
							break;
						}
                            //#### Else this entry seems valid
                        else {
								//#### Collect the oMinMaxDataType for this entry, then set its Data_Type to the casted int and .ToString'd .DataType
								//####     NOTE: We need to cast the .DataType as an int first else it does not come out as a numeric value
							oMinMaxDataType = DetermineMinMaxDataType(sDataType, g_oData.Value(i, "Numeric_Precision"), g_oData.Value(i, "Numeric_Scale"), eDataSource);
					        g_oData.Value(i, "Data_Type", ((int)oMinMaxDataType.DataType).ToString());

								//#### If the developer hasn't set the MinimumNumericValue/MamimumNumericValue for the current entry
							if (sMinimumNumericValue.Length == 0 || sMamimumNumericValue.Length == 0) {
									//#### Reset MinimumNumericValue/MamimumNumericValue's to the values within oMinMaxDataType
								g_oData.Value(i, "MinimumNumericValue", oMinMaxDataType.MinimumValue);
								g_oData.Value(i, "MamimumNumericValue", oMinMaxDataType.MaximumValue);
							}
                        }
					}
				}
                    //#### Else the passed oDataSourceMetaData did not contain the required columns, so raise the error and set bErrorOccured to true (even though the raised error should stop execution)
				else {
					Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MissingRequiredColumns, "oDataSourceMetaData", "");
					bErrorOccured = true;
				}

                    //#### If no bError(s)Occured above
			    if (! bErrorOccured) {
                        //#### Traverse the g_oData's .Rows
				    for (i = 0; i < iRowCount; i++) {
                            //#### Determine the eDataSource and do the required additional processing accordingly
                        switch (eDataSource) {
                                //#### If this is a .cnSQLServer-based data source
                            case enumDataSource.cnSQLServer: {
                                    //#### Borrow the use of sDataType to store the Column_Default
						        sDataType = g_oData.Value(i, "Column_Default");

                                    //#### If here is a Column_Default (in the borrowed sDataType) to check
                                if (sDataType.Length > 0) {
                                        //#### If the Column_Default is surrounded by parens (which all Column_Default's under .cnSQLServer are), remove them
						            if (sDataType.Substring(0, 1) == "(" && sDataType.Substring(sDataType.Length - 1, 1) == ")") {
							            sDataType = sDataType.Substring(1, sDataType.Length - 2);
						            }

                                        //#### If the Column_Default is surrounded by single-quotes (which all string-based g_cColumn_Default's under .cnSQLServer are), remove them
						            if (sDataType.Substring(0, 1) == "'" && sDataType.Substring(sDataType.Length - 1, 1) == "'") {
							            sDataType = sDataType.Substring(1, sDataType.Length - 2);
						            }

                                        //#### Reset the g_cColumn_Default with the above modified sDataType
						            g_oData.Value(i, "Column_Default", sDataType);
						        }
                                break;
                            }
                        }
				    }
			    }
					//#### Else bError(s)Occured, so reset the g_oData to null
					//####     NOTE: We should never get here, as if bError(s)Occured above we should have errored out
				else {
					g_oData = null;
				}
			}
		}


        //##########################################################################################
        //# Public Read-Only Properties
        //##########################################################################################
        ///############################################################
        /// <summary>
		/// Assists in the collection of the underlying structure which defines this instance.
        /// </summary>
		/// <value>MetaDataCollectionHelper instance configured to collect the underlying structure which defines this instance.</value>
        ///############################################################
		/// <LastUpdated>January 14, 2010</LastUpdated>
		public static MetaDataCollectionHelper GetData {
			get {
					//#### If g_oGetData hasn't be setup, do so now
				if (g_oGetData == null) {
					g_oGetData = new MetaDataCollectionHelper();
				}

				return g_oGetData;
			}
		}

        ///############################################################
        /// <summary>
        /// Gets a deep copy of the underlying structure which defines this instance.
        /// </summary>
		/// <value>Deep copy of the MultiArray which defines the metadata of this instance.</value>
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


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
        ///############################################################
		/// <summary>
		/// Determines if this instance contains the referenced table.
		/// </summary>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <returns>Boolean value signaling the existance of the passed <paramref>sTableName</paramref>.<para/>Returns true if the passed <paramref>sTableName</paramref> is present within this instance, or false if it is not present.</returns>
        ///############################################################
		/// <LastUpdated>May 9, 2007</LastUpdated>
		public bool Exists(string sTableName) {
			string sCurrentTableName;
			int iLen;
			int i;
			bool bReturn = false;

				//#### Lower case the passed sTableName and determine its iLen
			sTableName = sTableName.ToLower();
			iLen = sTableName.Length;

				//#### Traverse the rows of the g_oData
			for (i = 0; i < g_oData.RowCount; i++) {
					//#### Determine the sCurrentTableName
				sCurrentTableName = g_oData.Value(i, "Table_Name");

					//#### If the sCurrentTableName matches the passed sTableName (checking their Lens first as that is a faster comparison)
				if (sCurrentTableName.Length == iLen && sCurrentTableName.ToLower() == sTableName) {
						//#### Flip the return value to true and exit the loop
					bReturn = true;
					break;
				}
			}

                //#### Return the above determined bReturn value to the caller
            return bReturn;
		}

        ///############################################################
		/// <summary>
		/// Determines if this instance contains the referenced table/column name pair.
		/// </summary>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <param name="sColumnNames">String representing the comma delimited column names to locate.</param>
		/// <returns>Boolean value signaling the existance of the passed <paramref>sTableName</paramref>/<paramref>sColumnNames</paramref> pair.<para/>Returns true if the <paramref>sTableName</paramref>/<paramref>sColumnNames</paramref> pair is defined within this instance, or false if it is not present.</returns>
        ///############################################################
		/// <LastUpdated>May 28, 2007</LastUpdated>
		public bool Exists(string sTableName, string sColumnNames) {
			int iJunk = 0;

                //#### Collect the oTable from the global (application) variable
			MultiArray oTable = SearchMetaData(g_oData, sTableName, "", true, ref iJunk);

                //#### Pass the call off to FindPresentColumns (letting it know to return a MakeBoolean-able value), returning its MakeBoolean'd as our own
			return Cn.Data.Tools.MakeBoolean(FindPresentColumns(oTable, sTableName, sColumnNames, false), false);
		}

        ///############################################################
		/// <summary>
		/// Determines if the passed structure contains the referenced column names.
		/// </summary>
		/// <param name="oTable">MultiArray representing the single table to search.</param>
		/// <param name="sColumnNames">String representing the comma delimited column names to locate.</param>
		/// <returns>Boolean value signaling the existance of the passed <paramref>sColumnName</paramref> within the passed <paramref>oTable</paramref>.<para/>Returns true if all the <paramref>sColumnNames</paramref> are defined within <paramref>oTable</paramref>, or false if they are not all present.</returns>
        ///############################################################
		/// <LastUpdated>September 12, 2005</LastUpdated>
		public static bool Exists(MultiArray oTable, string sColumnNames) {
			bool bReturn = false;

                //#### If the passed oTable contains table information
			if (oTable != null && oTable.RowCount > 0) {
					//#### Pass the call off to FindPresentColumns (letting it know to return a MakeBoolean'able value), setting our bReturn value to its MakeBoolean'd return value
				bReturn = Cn.Data.Tools.MakeBoolean(FindPresentColumns(oTable, oTable.Value(0, "Table_Name"), sColumnNames, false), false);
			}

                //#### Return the above determined bReturn value to the caller
            return bReturn;
		}

        ///############################################################
        /// <summary>
        /// Gets the requested metadata value for the referenced table/column name pair.
        /// </summary>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <param name="sColumnName">String representing the column name to locate.</param>
        /// <param name="eMetaDataValue">Enumeration representing the required metadata value.</param>
        /// <returns>String representing the requested metadata value.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eMetaDataValue</paramref> is unreconized.</exception>
        ///############################################################
		/// <LastUpdated>May 28, 2007</LastUpdated>
        public string Value(string sTableName, string sColumnName, enumMetaDataTypes eMetaDataValue) {
				//#### Pass the call off to .DoValue, signaling that the .Value is to be returned (so we can return it to our caller)
			return DoValue("Value", sTableName, sColumnName, eMetaDataValue, false, "");
        }

        ///############################################################
		/// <summary>
        /// Sets the requested metadata value for the referenced table/column name pair with the new value.
		/// </summary>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <param name="sColumnName">String representing the column name to locate.</param>
        /// <param name="eMetaDataValue">Enumeration representing the required metadata value.</param>
		/// <param name="sNewValue">String representing the new value for the referenced index.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eMetaDataValue</paramref> is unreconized.</exception>
        ///############################################################
		/// <LastUpdated>May 28, 2007</LastUpdated>
        public void Value(string sTableName, string sColumnName, enumMetaDataTypes eMetaDataValue, string sNewValue) {
				//#### Pass the call off to .DoValue, signaling that the sNewValue is to be set
			DoValue("Value", sTableName, sColumnName, eMetaDataValue, true, sNewValue);
        }

		///############################################################
		/// <summary>
		/// Determines if the provided values are valid for the referenced table/column name pair.
		/// </summary>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <param name="sColumnName">String representing the column name to locate.</param>
		/// <param name="sValue">String representing a value to validate.</param>
		/// <returns>Boolean value indicating if the provided values are valid for the referenced table/column name pair.</returns>
		///############################################################
		/// <LastUpdated>December 18, 2009</LastUpdated>
		public bool Validate(string sTableName, string sColumnName, string sValue) {
			string[] a_sValues = new string[] { sValue };

				//#### Pass the call off to .DoValidate
			return DoValidate(Column(sTableName, sColumnName), a_sValues, g_oPicklists);
		}

		///############################################################
		/// <summary>
		/// Determines if the provided values are valid for the referenced table/column name pair.
		/// </summary>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <param name="sColumnName">String representing the column name to locate.</param>
		/// <param name="a_sValues">String array where each index represents a value to validate.</param>
		/// <returns>Boolean value indicating if the provided values are valid for the referenced table/column name pair.</returns>
		///############################################################
		/// <LastUpdated>December 18, 2009</LastUpdated>
		public bool Validate(string sTableName, string sColumnName, string[] a_sValues) {
				//#### Pass the call off to .DoValidate
			return DoValidate(Column(sTableName, sColumnName), a_sValues, g_oPicklists);
		}

		///############################################################
		/// <summary>
		/// Determines if the provided values are valid for the referenced table/column name pair.
		/// </summary>
		/// <param name="oTable">MultiArray representing the single table to search.</param>
		/// <param name="sColumnName">String representing the column name to locate.</param>
		/// <param name="sValue">String representing a value to validate.</param>
		/// <param name="oPicklists">Picklists instance representing the picklists related to this instance.</param>
		/// <returns>Boolean value indicating if the provided values are valid for the referenced table/column name pair.</returns>
		///############################################################
		/// <LastUpdated>December 18, 2009</LastUpdated>
		public static bool Validate(MultiArray oTable, string sColumnName, string sValue, Picklists oPicklists) {
			string[] a_sValues = new string[] { sValue };

				//#### Pass the call off to .DoValidate
			return DoValidate(Column(oTable, sColumnName), a_sValues, oPicklists);
		}

		///############################################################
		/// <summary>
		/// Determines if the provided values are valid for the referenced table/column name pair.
		/// </summary>
		/// <param name="oTable">MultiArray representing the single table to search.</param>
		/// <param name="sColumnName">String representing the column name to locate.</param>
		/// <param name="a_sValues">String array where each index represents a value to validate.</param>
		/// <param name="oPicklists">Picklists instance representing the picklists related to this instance.</param>
		/// <returns>Boolean value indicating if the provided values are valid for the referenced table/column name pair.</returns>
		///############################################################
		/// <LastUpdated>December 18, 2009</LastUpdated>
		public static bool Validate(MultiArray oTable, string sColumnName, string[] a_sValues, Picklists oPicklists) {
				//#### Pass the call off to .DoValidate
			return DoValidate(Column(oTable, sColumnName), a_sValues, oPicklists);
		}

		///############################################################
		/// <summary>
		/// Sets the picklist names related to table/column name pairs within this instance as defined by the referenced <c>Picklists</c>.
		/// </summary>
		/// <param name="oPicklists">Picklists instance representing the picklists related to this instance.</param>
		///############################################################
		/// <LastUpdated>December 17, 2009</LastUpdated>
		public void SetRelatedPicklists(Picklists oPicklists) {
			MultiArray oRelatedPicklist;
			string[] a_sTableColumnName;
			int i;

				//#### If the caller passed in a valid oPicklists
			if (oPicklists != null) {
					//#### Set the passed oPicklists into g_oPicklists, then collect the oRelatedPicklist
				g_oPicklists = oPicklists;
				oRelatedPicklist = oPicklists.Picklist(Picklists.ColumnAssociationsPicklistName);

					//#### If oRelatedPicklist was successfully collected above
				if (oRelatedPicklist != null && oRelatedPicklist.RowCount > 0) {
						//#### Traverse the oRelatedPicklists .Rows
					for (i = 0; i < oRelatedPicklist.RowCount; i++) {
							//#### Determine the a_sTableColumnName for this loop
						a_sTableColumnName = oRelatedPicklist.Value(i, "Data").Split('.');

							//#### If the a_sTableColumnName has 2 entries and the a_sTableColumnName .Exists
						if (a_sTableColumnName.Length == 2 && Exists(a_sTableColumnName[0], a_sTableColumnName[1])) {
								//#### Set the .Value for the .cnRelatedPicklist for the defined a_sTableColumnName
							DoValue("SetRelatedPicklists", a_sTableColumnName[0], a_sTableColumnName[1], enumMetaDataTypes.cnRelatedPicklist, true, oRelatedPicklist.Value(i, "Description"));
						}
					}
				}
			}	
		}

        ///############################################################
		/// <summary>
		/// Retrieves a structure containing the entries for the referenced table.
		/// </summary>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <returns>MultiArray containing all the entries for the passed <paramref>sTableName</paramref> defined within this instance, or null if it was not defined.</returns>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public MultiArray Table(string sTableName) {
			int iJunk = 0;

                //#### Pass the call off to SearchMetaData (letting it know to return the entire table), returning it's return value as our own
			return SearchMetaData(g_oData, sTableName, "", true, ref iJunk);
		}

        ///############################################################
        /// <summary>
		/// Retrieves a structure containing the single entry for a specific column name within the referenced table.
        /// </summary>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <param name="sColumnName">String representing the column name to locate.</param>
		/// <returns>MultiArray containing the single entry for the passed <paramref>sColumnName</paramref> within the passed <paramref>sTableName</paramref> defined within this instance, or null if it was not defined.</returns>
        ///############################################################
		/// <LastUpdated>February 1, 2010</LastUpdated>
		public MultiArray Column(string sTableName, string sColumnName) {
			MultiArray oReturn = null;
			int iRowIndex = -1;

                //#### Pass the call off to .SearchMetaData (letting it know to return only the iRowIndex for the passed sColumnName)
			SearchMetaData(g_oData, sTableName, sColumnName, false, ref iRowIndex);

				//#### If the iRowIndex was successfully found
			if (iRowIndex != -1) {
					//#### Reinit our oReturn value then insert the iRowIndex into our oReturn value
				oReturn = new MultiArray(g_oData.ColumnNames);
				oReturn.InsertRow(g_oData.Row(iRowIndex));
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

        ///############################################################
        /// <summary>
		/// Retrieves a structure containing the single entry for a specific column name within the referenced table.
        /// </summary>
		/// <param name="oTable">MultiArray representing the single table to search.</param>
		/// <param name="sColumnName">String representing the column name to locate.</param>
		/// <returns>MultiArray containing the single entry for the passed <paramref>sColumnName</paramref> within the passed <paramref>oTable</paramref>, or null if it was not defined.</returns>
        ///############################################################
		/// <LastUpdated>February 1, 2010</LastUpdated>
		public static MultiArray Column(MultiArray oTable, string sColumnName) {
			MultiArray oReturn = null;
			int iRowIndex = -1;

                //#### If the passed oTable contains table information
			if (oTable != null && oTable.RowCount > 0) {
					//#### Pass the call off to .SearchMetaData (letting it know to return only the iRowIndex for the passed sColumnName)
				SearchMetaData(oTable, oTable.Value(0, "Table_Name"), sColumnName, false, ref iRowIndex);

					//#### If the iRowIndex was successfully found
				if (iRowIndex != -1) {
						//#### Reinit our oReturn value then insert the iRowIndex into our oReturn value
					oReturn = new MultiArray(oTable.ColumnNames);
					oReturn.InsertRow(oTable.Row(iRowIndex));
				}
			}

                //#### Return the above determined oReturn value to the caller
            return oReturn;
		}

        ///############################################################
		/// <summary>
		/// Retrieves the column names present within the referenced table.
		/// </summary>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <param name="sColumnNames">String representing the comma delimited column names to locate.</param>
        /// <returns>String value containing a comma delimited list of <paramref>sColumnNames</paramref> present within the passed <paramref>sTableName</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public string ColumnsInTable(string sTableName, string sColumnNames) {
			MultiArray oTable;
			int iJunk = 0;

                //#### Collect the passed sTableName into the local oTable
			oTable = SearchMetaData(g_oData, sTableName, "", true, ref iJunk);

                //#### Pass the call off to .FindPresentColumns (letting it know to return the present sColumnNames), returning it's return value as our own
			return FindPresentColumns(oTable, sTableName, sColumnNames, true);
		}

        ///############################################################
		/// <summary>
		/// Retrieves the column names present within the referenced table.
		/// </summary>
		/// <param name="oTable">MultiArray representing the single table to search.</param>
		/// <param name="sColumnNames">String representing the comma delimited column names to locate.</param>
        /// <returns>String value containing a comma delimited list of <paramref>sColumnNames</paramref> present within the passed <paramref>oTable</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public static string ColumnsInTable(MultiArray oTable, string sColumnNames) {
			string sReturn = "";

                //#### If the passed oTable contains table information
			if (oTable != null && oTable.RowCount > 0) {
					//#### Pass the call off to FindPresentColumns (letting it know to return the present sColumnNames), setting our sReturn value to its return value
				sReturn = FindPresentColumns(oTable, oTable.Value(0, "Table_Name"), sColumnNames, true);
			}

                //#### Return the above determined sReturn value to the caller
            return sReturn;
		}


        //#######################################################################################################
        //# Private Functions
        //#######################################################################################################
        ///############################################################
        /// <summary>
        /// Get/sets the requested meta data value for the referenced table/column name pair.
        /// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <param name="sColumnName">String representing the column name to locate.</param>
        /// <param name="eMetaDataValue">Enumeration representing the required metadata value.</param>
		/// <param name="bSetNewValue">Boolean value signaling if a new value is to be set.</param>
		/// <param name="sNewValue">String representing the new value for the referenced index.</param>
        /// <returns>String representing the requested metadata value.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eMetaDataValue</paramref> is unreconized.</exception>
        ///############################################################
		/// <LastUpdated>May 28, 2007</LastUpdated>
        private string DoValue(string sFunction, string sTableName, string sColumnName, enumMetaDataTypes eMetaDataValue, bool bSetNewValue, string sNewValue) {
			string sReturn = "";
			int iRowIndex = 0;

                //#### Call .SearchMetaData (letting it know to return only the iRowIndex for the passed sColumnName, ignoring the return value)
			SearchMetaData(g_oData, sTableName, sColumnName, false, ref iRowIndex);

				//#### If the sTableName/sColumnName was found
			if (iRowIndex != -1) {
					//#### Determine the passed eMetaDataValue and reset the sReturn value accordingly
				switch (eMetaDataValue) {
					case enumMetaDataTypes.cnCharacter_Maximum_Length: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "Character_Maximum_Length", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "Character_Maximum_Length");
						}
						break;
					}
					case enumMetaDataTypes.cnColumn_Default: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "Column_Default", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "Column_Default");
						}
						break;
					}
					case enumMetaDataTypes.cnColumn_Name: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "Column_Name", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "Column_Name");
						}
						break;
					}
					case enumMetaDataTypes.cnData_Type: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "Data_Type", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "Data_Type");
						}
						break;
					}
					case enumMetaDataTypes.cnIs_Nullable: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "Is_Nullable", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "Is_Nullable");
						}
						break;
					}
					case enumMetaDataTypes.cnMamimumNumericValue: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "MamimumNumericValue", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "MamimumNumericValue");
						}
						break;
					}
					case enumMetaDataTypes.cnMinimumNumericValue: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "MinimumNumericValue", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "MinimumNumericValue");
						}
						break;
					}
					case enumMetaDataTypes.cnNumeric_Precision: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "Numeric_Precision", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "Numeric_Precision");
						}
						break;
					}
					case enumMetaDataTypes.cnNumeric_Scale: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "Numeric_Scale", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "Numeric_Scale");
						}
						break;
					}
					case enumMetaDataTypes.cnRelatedPicklist: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "RelatedPicklist", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "RelatedPicklist");
						}
						break;
					}
					case enumMetaDataTypes.cnTable_Name: {
							//#### If we are supposed to bSet(the)NewValue, do so now
						if (bSetNewValue) {
							g_oData.Value(iRowIndex, "Table_Name", sNewValue);
						}
							//#### Else we just need to retrieve the value
						else {
							sReturn = g_oData.Value(iRowIndex, "Table_Name");
						}
						break;
					}
					default: {
						Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eMetaDataValue", eMetaDataValue.ToString());
						break;
					}
				}
			}

                //#### Return the above determined sReturn value to the caller
            return sReturn;
        }

		///############################################################
		/// <summary>
		/// Gets a value indicating if the provided values are valid for the referenced column definition.
		/// </summary>
		/// <param name="oColumn">MultiArray representing the single column to use as the datatype definition.</param>
		/// <param name="a_sValues">String array where each index represents a value to validate.</param>
		/// <param name="oPicklists">Picklists instance representing the picklists related to this instance.</param>
		/// <returns>Boolean value indicating if the provided values are valid for the referenced column defintion.</returns>
		///############################################################
		/// <LastUpdated>January 8, 2010</LastUpdated>
		private static bool DoValidate(MultiArray oColumn, string[] a_sValues, Picklists oPicklists) {
			enumValueErrorTypes eErrorType = enumValueErrorTypes.cnUnknownOrUnsupportedType;

				//#### If the passed sTableName.sColumnName pair exists
			if (oColumn != null) {
					//#### Pull the datasource-related variables from the oColumn values
				eErrorType = Values.Validate(
					a_sValues,
					Tools.MakeEnum(oColumn.Value(0, "Data_Type"), enumDataTypes.cnUnknown),
					enumValueTypes.cnSingleValue,							//# eValueType is used by Renderer.Form, so it is always .cnSingleValue here
					Cn.Data.Tools.MakeBoolean(oColumn.Value(0, "Is_Nullable"), false),
					Cn.Data.Tools.MakeInteger(oColumn.Value(0, "Character_Maximum_Length"), 0),
					oColumn.Value(0, "MinimumNumericValue"),
					oColumn.Value(0, "MamimumNumericValue"),
					Cn.Data.Tools.MakeInteger(oColumn.Value(0, "Numeric_Precision"), 0),
					Cn.Data.Tools.MakeInteger(oColumn.Value(0, "Numeric_Scale"), 0),
					false,													//# bIsNewRecord is used by Renderer.Form, so it is always false here
					oPicklists,
					oColumn.Value(0, "RelatedPicklist"),
					false													//# bPicklistIsAdHoc is used by Renderer.Form, so it is always false here
				);
			}

				//#### Return based on the above determined eErrorType
			return (eErrorType == enumValueErrorTypes.cnNoError);
		}

        ///############################################################
        /// <summary>
        /// Searches the passed MultiArray for the referenced table/column name pair.
        /// </summary>
		/// <param name="oMetaData">MultiArray representing the metadata to search.</param>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <param name="sColumnName">String representing the column name to locate.</param>
        /// <param name="bReturnTable">Boolean value signaling if we are to return a single column entry or an entire table.</param>
		/// <param name="iRowIndex">0-based by reference integer representing the desired row index.</param>
		/// <returns>MultiArray containing the entries for the passed <paramref>sTableName</paramref> or <paramref>sTableName</paramref>/<paramref>sColumnName</paramref> pair (as defined by <paramref>bReturnTable</paramref>) as defined within the passed <paramref>oMetaData</paramref>.</returns>
        ///############################################################
		/// <LastUpdated>January 28, 2010</LastUpdated>
		private static MultiArray SearchMetaData(MultiArray oMetaData, string sTableName, string sColumnName, bool bReturnTable, ref int iRowIndex) {
			MultiArray oReturn = null;
			string sCurrentTableName;
			int iTableNameLen;
			int i;

				//#### If the passed oMetaData and sTableName are not null
			if (oMetaData != null && sTableName != null) {
					//#### Lower case the passed sTableName and determine its .Length
					//####     NOTE: Since RDBMSs are case insensitive, the passed sTableName (and later sColumnName) is lowercased. 'Course not all described DataSources are from an RDBMS, but since it represents the majority its rules stick.
				sTableName = sTableName.ToLower();
				iTableNameLen = sTableName.Length;

					//#### If we are to return an entire table
				if (bReturnTable) {
					bool bFound = false;

						//#### Init our oReturn value
					oReturn = new MultiArray(oMetaData.ColumnNames);

						//#### Traverse the .Rows of the passed oMetaData
					for (i = 0; i < oMetaData.RowCount; i++) {
							//#### Determine the sCurrentTableName
						sCurrentTableName = oMetaData.Value(i, "Table_Name");

							//#### If the sCurrentTableName matches the passed sTableName (checking their Lens first as that is a faster comparison)
						if (sCurrentTableName.Length == iTableNameLen && sCurrentTableName.ToLower() == sTableName) {
								//#### Copy the .Row into the oReturn value and make sure bFound is set to true
							oReturn.InsertRow(oMetaData.Row(i));
							bFound = true;
						}
							//#### Else if we have bFound the passed sTableName (yet are no longer within its bounds)
						else if (bFound) {
								//#### Exit the for loop as there is no further info for the passed sTableName
							break;
						}
					}
				}
					//#### Else we are to return the iRowIndex of a single table/column, so if the passed sColumnName is not null
				else if (sColumnName != null) {
					string sCurrentColumnName;
					int iColumnNameLen;

						//#### Since we are not returning a table, set our oReturn value to null and reset the byref iRowIndex to -1 (indicating that no .Row was found... yet)
				  //oReturn = null;
					iRowIndex = -1;

						//#### Lower case the passed sColumnName and determine its .Length
					sColumnName = sColumnName.ToLower();
					iColumnNameLen = sColumnName.Length;

						//#### Traverse the .Rows of the passed oMetaData
					for (i = 0; i < oMetaData.RowCount; i++) {
							//#### Determine the sCurrentTableName and sCurrentColumnName
						sCurrentTableName = oMetaData.Value(i, "Table_Name");
						sCurrentColumnName = oMetaData.Value(i, "Column_Name");

							//#### If the sCurrentTableName/sCurrentColumnName matches the passed sTableName/sColumnName (checking their .Lenghts first as that is a faster comparison)
						if (sCurrentTableName.Length == iTableNameLen && sCurrentColumnName.Length == iColumnNameLen &&
							sCurrentTableName.ToLower() == sTableName && sCurrentColumnName.ToLower() == sColumnName
						) {
								//#### Reset the byref iRowIndex to the value of i and exit the loop
							iRowIndex = i;
							break;
						}
					}
				}
			}

                //#### Return the above deterined oReturn value to the caller
            return oReturn;
		}

        ///############################################################
        /// <summary>
        /// Determines which of the referenced column names are present within the referenced table/structure. 
        /// </summary>
        /// <param name="oMetaData">MultiArray representing the metadata to search</param>
		/// <param name="sTableName">String representing the table name to locate.</param>
		/// <param name="sColumnNames">String representing the column name to locate.</param>
        /// <param name="bReturnPresentColumns"></param>
        /// <returns>String optionally containing the <paramref>sColumnNames</paramref> present within the passed <paramref>oMetaData</paramref> or a MakeBoolean-able value representing the presence of all of the passed <paramref>sColumnNames</paramref> (as defined by <paramref>bReturnPresentColumns</paramref>).</returns>
        ///############################################################
		/// <LastUpdated>September 12, 2005</LastUpdated>
		private static string FindPresentColumns(MultiArray oMetaData, string sTableName, string sColumnNames, bool bReturnPresentColumns) {
			string[] a_sColumns;
			string sCurrentColumnName;
			string sPassedColumnName;
			string sReturn = "";
			int iPassedColumnNameLen;
			int iRowCount;
			int i;
			int j;
			bool bFound;

                //#### If the passed oMetaData contains the table metadata
			if (oMetaData != null && oMetaData.RowCount > 0) {
				    //#### Lower case the passed sTableName
				sTableName = sTableName.ToLower();

                    //#### .Split the passed sColumnNames apart (lower caseing it as we go) then determine the iRowCount
				a_sColumns = sColumnNames.ToLower().Split(',');
				iRowCount = oMetaData.RowCount;

                    //#### Traverse the a_sColumns
				for (i = 0; i < a_sColumns.Length; i++) {
                        //#### Reset the value of sPassedColumnName for this loop
                        //####     NOTE: sPassedColumnName is not lower cased because the passed sColumnNames was before it was .Split into a_sColumns
					sPassedColumnName = a_sColumns[i].Trim();

                        //#### If the current sPassedColumnName contains a sTableName (or alias) prefix
                    if (sPassedColumnName.IndexOf(".") > -1) {
                            //#### If the sTableName prefix is on the head of the current sPassedColumnName
                        if (sPassedColumnName.IndexOf(sTableName + ".") == 0) {
                                //#### Peal off the sTableName prefix from the current sPassedColumnName
                            sPassedColumnName = sPassedColumnName.Substring(sPassedColumnName.IndexOf(".") + 1);
                        }
                            //#### Else the prefix on the current sPassedColumnName is not for the passed sTableName (or is an alias which we cannot hope to understand since we do not have the table list)
                        else {
                                //#### Reset the current sPassedColumnName to a null-string so it is passed over below (as it is not a part of the passed sTableName)
                            sPassedColumnName = "";
                        }
                    }

                        //#### Determine sPassedColumnName's .Length and reset the value of bFound
					iPassedColumnNameLen = sPassedColumnName.Length;
					bFound = false;

                        //#### If the sPassedColumnName is holding a value
					if (iPassedColumnNameLen > 0) {
                            //#### Traverse the .Rows of the passed oMetaData
						for (j = 0; j < iRowCount; j++) {
                                //#### Reset the value of sCurrentColumnName for this loop
							sCurrentColumnName = oMetaData.Value(j, "Column_Name");

                                //#### If sPassedColumnName matches the sCurrentColumnName (checking their .Lengths first as that is a faster comparison)
							if (iPassedColumnNameLen == sCurrentColumnName.Length && sPassedColumnName == sCurrentColumnName.ToLower()) {
                                    //#### If we're supposed to bReturn(the)PresentColumns, append sCurrentColumnName onto the return value
								if (bReturnPresentColumns) {
									sReturn += sCurrentColumnName + ",";
								}

                                    //#### Flip bFound and exit the inner for loop as we have found what we were looking for
								bFound = true;
								break;
							}
						}
					}

                        //#### If we're not supposed to bReturn(the)PresentColumns and the sPassedColumnName was not bFound above
					if (! bReturnPresentColumns && ! bFound) {
                            //#### Reset the return value to the MakeBoolean'able "false" and exit the outer for loop
						sReturn = "false";
						break;
					}
				}

                    //#### Borrow the use of i to determine the .Length of the sReturn value
				i = sReturn.Length;

                    //#### If we're supposed to bReturn(the)PresentColumns
				if (bReturnPresentColumns) {
                        //#### If there are columns in the sReturn value, remove the trailing comma
					if (i > 0) {
						sReturn = sReturn.Substring(0, i - 1);
					}
				}
                    //#### Else we're supposed to return a MakeBoolean'able value
				else {
                        //#### If the return value was never set to "false" above, all of the passed sColumnNames must have been found successfully, so set the return value to the MakeBoolean'able "true"
					if (i == 0) {
						sReturn = "true";
					}
				}
			}
				//#### Else if we were not supposed to bReturnPresentColumns (meaning we're supposed to return a MakeBoolean-able value)
			else if (! bReturnPresentColumns) {
					//#### Reset the sReturn value to "false", as no columns were found
				sReturn = "false";
			}

			    //#### Return the above determined sReturn value to the caller
			return sReturn;
		}

        ///############################################################
		/// <summary>
		/// Determines the generalized data type, minimum and maximum numeric values for the referenced database data type.
		/// </summary>
		/// <remarks>
		/// NOTE: Code snipit to test numeric column width's: "select [test1] = convert(decimal(18,0),999999999999999999+0)", "...+1)", etc.
		/// </remarks>
		/// <param name="sDataType">String representing the columns data type.</param>
		/// <param name="sNumericPrecision">String representing the columns numeric precision.</param>
		/// <param name="sNumericScale">String representing the columns numeric scale.</param>
		/// <param name="eDataSource">Enumeration representing the source the <paramref>oDataSourceMetaData</paramref> was generated from.</param>
		/// <returns>structMinMaxDataType containing the generalized data type, minimum and maximum numeric values.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eDataSource</paramref> is unreconized.</exception>
        ///############################################################
		/// <LastUpdated>September 15, 2005</LastUpdated>
		private static structMinMaxDataType DetermineMinMaxDataType(string sDataType, string sNumericPrecision, string sNumericScale, enumDataSource eDataSource) {
		    structMinMaxDataType oReturn;

		        //#### Lower case the passed sDataType and default the oReturn value's .MinimumValue/.MaximumValue to null-strings (so that if they are not set below, Renderer.Form can use the defined NumericPrecision instead)
		    sDataType = sDataType.ToLower();
		    oReturn.MinimumValue = "";
			oReturn.MaximumValue = "";

                //#### Determine the passed eDataSource and process accordingly
                //####     NOTE: These min/max values were verified within Enterprise manager utilizing SQL and the CONVERT function (i.e. - "select [test1] = convert(decimal(18,0),999999999999999999+0)")
            switch (eDataSource) {
                case enumDataSource.cnSQLServer: {
                        //#### Determine the passed sDataType and process accordingly
                    switch (sDataType) {
                            //#### If this is a boolean field
                        case "bit": {
							oReturn.MinimumValue = "0";
							oReturn.MaximumValue = "1";
					        oReturn.DataType = enumDataTypes.cnBoolean;
                            break;
                        }
                            //#### If this is a integer field
                        case "bigint": {
							oReturn.MinimumValue = "-9223372036854775808";
							oReturn.MaximumValue = "9223372036854775807";
					        oReturn.DataType = enumDataTypes.cnInteger;
                            break;
                        }
                        case "int": {
							oReturn.MinimumValue = "-2147483648";
							oReturn.MaximumValue = "2147483647";
					        oReturn.DataType = enumDataTypes.cnInteger;
                            break;
                        }
                        case "smallint": {
							oReturn.MinimumValue = "-32768";
							oReturn.MaximumValue = "32767";
					        oReturn.DataType = enumDataTypes.cnInteger;
                            break;
                        }
                        case "tinyint": {
							oReturn.MinimumValue = "0";
							oReturn.MaximumValue = "255";
					        oReturn.DataType = enumDataTypes.cnInteger;
                            break;
                        }

                            //#### If this is a float field
                        case "float": {
							oReturn.MinimumValue = "-99999999999999999999999999999999999999";
							oReturn.MaximumValue = "99999999999999999999999999999999999999";
					        oReturn.DataType = enumDataTypes.cnFloat;
                            break;
                        }
                        case "numeric": {	//# This sDataType is variable
							int iNumericPrecision = Cn.Data.Tools.MakeInteger(sNumericPrecision, 0);
							int iNumericScale = Cn.Data.Tools.MakeInteger(sNumericScale, 0);

								//#### If this is the default sDataType definition, set the .MinimumValue/.MaximumValue accordingly
								//####     NOTE: This test is mostly done to ease in code maintenance, as we can see the true min/max values for the default sDataType definition below
							if (iNumericPrecision == 18 && iNumericScale == 0) {
								oReturn.MinimumValue = "-999999999999999999";
								oReturn.MaximumValue = "999999999999999999";
							}
								//#### Else we need to properly calculate the .MinimumValue/.MaximumValue
							else {
									//#### Init the value of .MaximumValue to the digits to the left of the decimal point
								oReturn.MaximumValue = Cn.Data.Tools.LPad("", "9", iNumericPrecision - iNumericScale);

									//#### Append the digits to the right of the decimal point (along with the decimal point itself) onto the .MaximumValue
								oReturn.MaximumValue = oReturn.MaximumValue + "." + Cn.Data.Tools.LPad("", "9", iNumericScale);

									//#### Set the .MinimumValue to the negetive equivalent of the above calculated .MaximumValue
								oReturn.MinimumValue = "-" + oReturn.MaximumValue;
							}

								//#### Set the oReturn.DataType value to a .cnFloat
					        oReturn.DataType = enumDataTypes.cnFloat;
                            break;
                        }
                        case "real": {
							oReturn.MinimumValue = "-99999999999999999999999999999999999999";
							oReturn.MaximumValue = "99999999999999999999999999999999999999";
					        oReturn.DataType = enumDataTypes.cnFloat;
                            break;
                        }

                            //#### If this is a currency/decimal field
                        case "decimal": {	//# This sDataType is variable
							int iNumericPrecision = Cn.Data.Tools.MakeInteger(sNumericPrecision, 0);
							int iNumericScale = Cn.Data.Tools.MakeInteger(sNumericScale, 0);

								//#### If this is the default sDataType definition, set the .MinimumValue/.MaximumValue accordingly
								//####     NOTE: This test is mostly done to ease code maintenance, as we can see the true min/max values for the default sDataType definition below
							if (iNumericPrecision == 18 && iNumericScale == 0) {
								oReturn.MinimumValue = "-999999999999999999";
								oReturn.MaximumValue = "999999999999999999";
							}
								//#### Else we need to properly calculate the .MinimumValue/.MaximumValue
							else {
									//#### Init the value of .MaximumValue to the digits to the left of the decimal point
								oReturn.MaximumValue = Cn.Data.Tools.LPad("", "9", iNumericPrecision - iNumericScale);

									//#### Append the digits to the right of the decimal point (along with the decimal point itself) onto the .MaximumValue
								oReturn.MaximumValue = oReturn.MaximumValue + "." + Cn.Data.Tools.LPad("", "9", iNumericScale);

									//#### Set the .MinimumValue to the negetive equivalent of the above calculated .MaximumValue
								oReturn.MinimumValue = "-" + oReturn.MaximumValue;
							}

								//#### Set the oReturn.DataType value to a .cnCurrency
					        oReturn.DataType = enumDataTypes.cnCurrency;
                            break;
						}
                        case "money": {
							oReturn.MinimumValue = "-922337203685477.5808";
							oReturn.MaximumValue = "922337203685477.5807";
					        oReturn.DataType = enumDataTypes.cnCurrency;
                            break;
						}
                        case "smallmoney": {
							oReturn.MinimumValue = "-214748.3648";
							oReturn.MaximumValue = "214748.3647";
					        oReturn.DataType = enumDataTypes.cnCurrency;
                            break;
                        }

                            //#### If this is a character field
                        case "char":
                        case "nchar":
                        case "nvarchar":
                        case "varchar": {
    					    oReturn.DataType = enumDataTypes.cnChar;
                            break;
                        }
                            //#### If this is a long character field
                            //####     NOTE: A "long character" field is defined as a character field that cannot be compaired with an equal sign in an SQL statement
                        case "ntext":
                        case "text": {
					        oReturn.DataType = enumDataTypes.cnLongChar;
                            break;
                        }
                            //#### If this is a date/time field
                        case "datetime":
                        case "smalldatetime": {
					        oReturn.DataType = enumDataTypes.cnDateTime;
                            break;
                        }
                            //#### If this is a GUID field
                        case "uniqueidentifier": {
					        oReturn.DataType = enumDataTypes.cnGUID;
                            break;
                        }
                            //#### If this is a binary field
                        case "binary":
                        case "varbinary":
                        case "image": {
					        oReturn.DataType = enumDataTypes.cnBinary;
                            break;
                        }
                            //#### Types that are unsupported
                            //####     NOTE: SQL*Server sql_variant stores values of various SQL Server-supported data types, except text, ntext, image, timestamp, and sql_variant. sql_variant may be used in columns, parameters, variables, and return values of user-defined functions. sql_variant allows these database objects to support values of other data types.
                            //####     NOTE: SQL*Server Timestamps are internally managed, non-updatable column types so no user input is allowed (hence its unsupported)
                        case "sql_variant":
                        case "timestamp": {
					        oReturn.DataType = enumDataTypes.cnUnsupported;
                            break;
                        }
                            //#### Else this in an unknown field type, so error out
                        default: {
					        oReturn.DataType = enumDataTypes.cnUnknown;
                            break;
                        }
                    }
                    break;
                }
                case enumDataSource.cnOracle: {
                        //#### Determine the passed sDataType and process accordingly
                    switch (sDataType) {
                            //#### If this is a boolean field
//                        case "bit": {
//					        oReturn.DataType = enumDataTypes.BooleanType;
//                            break;
//                        }
                            //#### If this is a integer field
                        case "int16":
                        case "int32":
                        case "int64":
                        case "byte": {
					        oReturn.DataType = enumDataTypes.cnInteger;
                            break;
                        }
                            //#### If this is a currency/decimal field
                        case "decimal": {
					        oReturn.DataType = enumDataTypes.cnCurrency;
                            break;
                        }


                            //#### If this is a float field
                        case "number":
                        case "float": {
					        oReturn.DataType = enumDataTypes.cnFloat;
                            break;
                        }
                            //#### If this is a character field
                        case "char":
                        case "nchar":
                        case "varchar2":
                        case "nvarchar2": {
					        oReturn.DataType = enumDataTypes.cnChar;
                            break;
                        }
                            //#### If this is a long character field
                            //####     NOTE: A "long character" field is defined as a character field that cannot be compaired with an equal sign in an SQL statement
//! check Oracle's limitations with equal comparisons
                        case "clob":
                        case "nclob":
                        case "long":
                        case "xmltype": {
    					    oReturn.DataType = enumDataTypes.cnLongChar;
                            break;
                        }
                            //#### If this is a date/time field
//! Check the timestamp* types - are they updatable?
                        case "date":
                        case "timestamp":
                        case "timestampltz":
                        case "timestamptz": {
					        oReturn.DataType = enumDataTypes.cnDateTime;
                            break;
                        }
                            //#### If this is a GUID field
//                        case "guid": {
//					        oReturn.DataType = enumDataTypes.GUIDType;
//                            break;
//                        }
                            //#### If this is a binary field
                        case "bfile":
                        case "blob":
                        case "longraw":
                        case "raw": {
					        oReturn.DataType = enumDataTypes.cnBinary;
                            break;
                        }
                            //#### Types yet to be implemented
//!
                        case "intervalds":
                        case "intervalym": {
					        oReturn.DataType = enumDataTypes.cnUnsupported;
                            break;
                        }
                            //#### Else this in an unknown field type, so error out
                        default: {
					        oReturn.DataType = enumDataTypes.cnUnknown;
                            break;
                        }
                    }
                    break;
                }
                case enumDataSource.cnSharePointPseudobase: {
                        //#### Determine the passed sDataType and process accordingly
                    switch (sDataType) {
                            //#### If this is a boolean field
                        case "boolean": {
					        oReturn.DataType = enumDataTypes.cnBoolean;
                            break;
                        }
                            //#### If this is a integer field
                        case "counter":
                        case "integer": {
					        oReturn.DataType = enumDataTypes.cnInteger;
                            break;
                        }
                            //#### If this is a float field
                        case "number": {
					        oReturn.DataType = enumDataTypes.cnFloat;
                            break;
                        }
                            //#### If this is a currency/decimal field
                        case "currency": {
					        oReturn.DataType = enumDataTypes.cnCurrency;
                            break;
                        }
                            //#### If this is a character field
                        case "calculated":
                        case "choice":
                        case "multichoice":
                        case "error":
                        case "text":
                        case "url":
                        case "user":
                        case "modstat":
                        case "lookup": {
					        oReturn.DataType = enumDataTypes.cnChar;
                            break;
                        }
                            //#### If this is a long character field
                            //####     NOTE: A "long character" field is defined as a character field that cannot be compaired with an equal sign in an SQL statement
                        case "note": {
					        oReturn.DataType = enumDataTypes.cnLongChar;
                            break;
                        }
                            //#### If this is a date/time field
                        case "datetime": {
					        oReturn.DataType = enumDataTypes.cnDateTime;
                            break;
                        }
                            //#### If this is a GUID field
                        case "guid": {
					        oReturn.DataType = enumDataTypes.cnGUID;
                            break;
                        }
                            //#### If this is a binary field
//                        case "": {
//					        oReturn.DataType = enumDataTypes.cnBinary;
//                            break;
//                        }
                            //#### If this is a unsupported field
//                        case "": {
//					        oReturn.DataType = enumDataTypes.cnUnsupported;
//                            break;
//                        }
                            //#### Else this in an unknown field type, so error out
                        default: {
					        oReturn.DataType = enumDataTypes.cnUnknown;
                            break;
                        }
                    }
                    break;
                }
                    //#### Else the passed eDataSource is unreconized, so raise the error
                default: {
                    Internationalization.RaiseDefaultError(g_cClassName + "", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eDataSource", Cn.Data.Tools.MakeString(eDataSource, ""));
                    oReturn.DataType = enumDataTypes.cnUnknown;
                    break;
                }
            }

                //#### Return the above determined oReturn value to the caller
            return oReturn;
		}
		#endregion


		///########################################################################################################################
		/// <summary>
		/// A collection of functions to determine the validity of values based on their related datatype descriptions.
		/// </summary>
		///########################################################################################################################
		public class Values {

			///############################################################
			/// <summary>
			/// Determines if all of the provided values are valid based on the provided datatype description. 
			/// </summary>
			/// <param name="a_sValues">String array where each index represents a value to validate.</param>
			/// <param name="eDataType">Enumerated value representing the column's datatype.</param>
			/// <param name="eValueType">Enumerated value representing the column's stored value type.</param>
			/// <param name="bIsNullable">Boolean value indicating if the column is permitted to hold a null value.</param>
			/// <param name="iMaximumCharacterLength">Integer representing the column's maximum character length.</param>
			/// <param name="sMinimumNumericValue">String representing the column's minimum numeric value.</param>
			/// <param name="sMaximumNumericValue">String representing the column's maximum numeric value.</param>
			/// <param name="iNumericPrecision">Integer representing the column's numeric precision.</param>
			/// <param name="iNumericScale">Integer representing the column's numeric scale.</param>
			/// <param name="bIsIDColumnForNewRecord">Boolean value indicating if this is an ID column and a new record (used by Renderer.Form).</param>
			/// <param name="oPicklists">Picklists instance representing the picklists related to this instance.</param>
			/// <param name="sPicklistName">String representing the picklist name.</param>
			/// <param name="bPicklistIsAdHoc">Boolean value representing if the picklist associated with the input will be defined at runtime by the developer (used by Renderer.Form).</param>
			/// <returns>Enumeration representing the error message for the first invalid value within the provided values.</returns>
			///############################################################
			/// <LastUpdated>March 26, 2010</LastUpdated>
			public static enumValueErrorTypes Validate(string[] a_sValues, enumDataTypes eDataType, enumValueTypes eValueType, bool bIsNullable, int iMaximumCharacterLength, string sMinimumNumericValue, string sMaximumNumericValue, int iNumericPrecision, int iNumericScale, bool bIsIDColumnForNewRecord, Picklists oPicklists, string sPicklistName, bool bPicklistIsAdHoc) {
				int iValueLength = -1;
				int iValueCount = -1;
				int i;
				bool bIsNumericType = false;
				enumValueErrorTypes eErrorType = enumValueErrorTypes.cnNoError;

					//#### If we have a_sValues to process
				if (a_sValues != null && a_sValues.Length > 0) {
						//#### Determine the iValueLength of all the a_sValues
					iValueLength = string.Join("", a_sValues).Length;
				}

					//#### If a_sValues are present
					//####     NOTE: Multiple a_sValues are checked below in case the request is comming from a RendererSearchForm (as the user could be looking for one of a number of values within a column that holds a single value)
				if (iValueLength > 0) {
						//#### Determine the a_sValues' iValueCount
					iValueCount = a_sValues.Length;

						//#### If there is more then a single iValueCount and this is not a "multi-single" or "multi-multi" input, set the error
						//####     NOTE: "multi-single" = .cnMultiValueSearchInSingleValuePicklistExType, "multi-multi" = .cnMultiValuePicklistExType but since .cnSingleValueSearchInMultiValuePicklistExType includes .cnMultiValuePicklistExType, we exclude .cnSingleValuePicklistExType below.
					if (iValueCount > 1 && eValueType != enumValueTypes.cnMultiValuesFromPicklist) {
						eErrorType = enumValueErrorTypes.cnMultipleValuesSubmittedForSingleValue;
					}
						//#### Else the iValueCount and the presence of .cnMultiValuePicklistExType agree
					else {
							//#### If the a_sValues are supposed to be a .cnVerbatum
						if (eValueType == enumValueTypes.cnVerbatum) {
								//#### Skip the additional checks, making the assumption that the developer will validate the a_sValues themselves
								//####     NOTE: .Required is valided below for .cnVerbatum's as the a_sValues failed the iValueLength test above
							//# Do nothing
						}
							//#### Else we need to check the eDataType
						else {
								//#### Determine the eDataType and process accordingly
							switch (eDataType) {
									//#### If the a_sValues are supposed to be a .cnBoolean
								case enumDataTypes.cnBoolean: {
										//#### Traverse the a_sValues
									for (i = 0; i < iValueCount; i++) {
											//#### If the current a_sValue .Is(not a)Boolean, set the eErrorType and fall from the loop
										if (! Cn.Data.Tools.IsBoolean(a_sValues[i])) {
											eErrorType = enumValueErrorTypes.cnIncorrectDataType;
											break;
										}
											//#### Else force the current a_sValue into a boolean, assigning it back into the current index
											//####     NOTE: Can't do this because we skip the remaining values on an error! Besides, reconized boolean values are processed within .Value(s) setter
									  //else {
									  //	a_sValues[i] = Cn.Data.Tools.MakeBooleanInteger(a_sValues[i], false).ToString();
									  //}
									}
									break;
								}

									//#### If the a_sValues are supposed to be an .cnInteger
								case enumDataTypes.cnInteger: {
										//#### Reset the bIsNumericType to true (so the additional tests are run below)
									bIsNumericType = true;

										//#### Traverse the a_sValues
									for (i = 0; i < iValueCount; i++) {
											//#### If the current a_sValue .Is(not an)Integer
											//####     NOTE: Since all .cnIntegerTypes are "small", use of .IsInteger should not cause any overflow errors
										if (! Cn.Data.Tools.IsInteger(a_sValues[i])) {
												//#### Set the eErrorType and fall from the loop
											eErrorType = enumValueErrorTypes.cnIncorrectDataType;
											break;
										}
											//#### Else force the current a_sValue into an integer, assigning it back into the current index
											//####     NOTE: Can't do this because we skip the remaining values on an error! Besides, it is numeric so there should be no need to change the developer set value(s)
									  //else {
									  //	a_sValues[i] = Cn.Data.Tools.MakeInteger(a_sValues[i], 0).ToString();
									  //}
									}
									break;
								}

									//#### If the a_sValues are supposed to be a .cnFloat
								case enumDataTypes.cnFloat: {
										//#### Reset the bIsNumericType to true (so the additional tests are run below)
									bIsNumericType = true;

										//#### Traverse the a_sValues
									for (i = 0; i < iValueCount; i++) {
											//#### If the current a_sValue .Is(not a)Number
											//####     NOTE: Since .IsNumeric traverses the sValue one character at a time (and therefore does not convert it into a numeric type), there is no issue with overflow errors
										if (! Cn.Data.Tools.IsNumeric(a_sValues[i])) {
												//#### Set the eErrorType and fall from the loop
											eErrorType = enumValueErrorTypes.cnIncorrectDataType;
											break;
										}
											//#### Else force the current a_sValue into a double, assigning it back into the current index
											//####     NOTE: Can't do this because we skip the remaining values on error! Besides, it is numeric so there should be no need to change the developer set value(s)
											//####     NOTE: REALLY can't do this because the size of .cnFloat can be larger then that of a Double (think Oracle and .NumericScale)
									  //else {
									  //	a_sValues[i] = Cn.Data.Tools.MakeDouble(a_sValues[i], 0).ToString();
									  //}
									}
									break;
								}

									//#### If the a_sValues are supposed to be a .cnCurrency
								case enumDataTypes.cnCurrency: {
										//#### Reset the bIsNumericType to true (so the additional tests are run below)
									bIsNumericType = true;

										//#### Traverse the a_sValues
									for (i = 0; i < iValueCount; i++) {
											//#### If the current a_sValue .Is(not)Numeric
											//####     NOTE: Since all .cnCurrencyTypes are "small", use of .IsNumeric should not cause any overflow errors
//####     NOTE: We no longer need to remove the .CurrencySymbol from each value because that is done within .Value(s) setter
										if (! Cn.Data.Tools.IsNumber(a_sValues[i])) {
												//#### Set the eErrorType and fall from the loop
											eErrorType = enumValueErrorTypes.cnIncorrectDataType;
											break;
										}
											//#### Else force the current a_sValue into a double, assigning it back into the current index
											//####     NOTE: Can't do this because we skip the remaining values on error! Besides, it is numeric so there should be no need to change the developer set value(s)
									  //else {
									  //	a_sValues[i] = Cn.Data.Tools.MakeDouble(a_sValues[i], 0).ToString();
									  //}
									}
									break;
								}

									//#### If the a_sValues are supposed to be a .cnDateTime
								case enumDataTypes.cnDateTime: {
										//#### Traverse the a_sValues
									for (i = 0; i < iValueCount; i++) {
											//#### If the current a_sValue .Is(not a)Date, set the eErrorType and fall from the loop
										if (! Cn.Data.Tools.IsDate(a_sValues[i])) {
											eErrorType = enumValueErrorTypes.cnIncorrectDataType;
											break;
										}
									}
									break;
								}

									//#### If the a_sValues are supposed to be a .cnGUID
								case enumDataTypes.cnGUID: {
										//#### Traverse the a_sValues
									for (i = 0; i < iValueCount; i++) {
											//#### If the current a_sValue .Is(not a)GUID, set the eErrorType and fall from the loop
										if (! Cn.Data.Tools.IsGUID(a_sValues[i])) {
											eErrorType = enumValueErrorTypes.cnIncorrectDataType;
											break;
										}
									}
									break;
								}

									//#### If the a_sValues are a .cnUnknown or .cnUnsupported, set the eErrorType
								case enumDataTypes.cnUnknown:
								case enumDataTypes.cnUnsupported: {
									eErrorType = enumValueErrorTypes.cnUnknownOrUnsupportedType;
									break;
								}

									//#### Else the eTypeDescription is textual or otherwise
								default: {
										//#### If iValueLength is longer then the iMaximumCharacterLength, set the eErrorType
										//####     NOTE: Multi-value string types are handeled differently as Multi-value inputs are assumed to be inserted into a single string-based common-delimited column (and have to therefore have to fit into that column's length as a fully formed common-delimited string). If a multi-value input is being handeled differently (such as being searched against from a RendererSearchForm), it's up to the developer to validate its .Values as necessary.
									if (iValueLength > iMaximumCharacterLength) {
										eErrorType = enumValueErrorTypes.cnIncorrectLength;
									}
										//#### Else if sValue is supposed to be from a .cnSingleValueFromPicklist or a .cnMultiValuesFromPicklist
										//####     NOTE: WriteComboBoxInputs will fail this test if the user does not enter a value form the list, so it is up to the developer to "unset" these simple errors within their own ValidateRecord functions (as there is no way for us to tell at this point if they rendered a .cnComboBox)
										//####     NOTE: Both .cnSingleValueFromPicklist and .cnMultiValuesFromPicklist are checked below because .cnSingleValueFromPicklist need to access the single user entered value via oInputData.Values because .Value retruns the .MultiValueString version (which is surrounded by .PrimaryDelimiters).
									else if (eValueType == enumValueTypes.cnSingleValueFromPicklist ||
										eValueType == enumValueTypes.cnMultiValuesFromPicklist
									) {
											//#### If all of the a_sValues are not present within the sPicklistName, set the eErrorType
										if (! bPicklistIsAdHoc && oPicklists != null && ! oPicklists.Exists(sPicklistName, a_sValues)) {
											eErrorType = enumValueErrorTypes.cnNotWithinPicklist;
										}
									}
									break;
								}
							}
						}
					}
				}
					//#### Else no a_sValues were passed, so if this input bIs(not)Nullable
				else if (! bIsNullable) {
						//#### As long as this is not a bIsIDColumnForNewRecord, set the eErrorType (as the .cnValueIsRequired)
					if (! bIsIDColumnForNewRecord) {
						eErrorType = enumValueErrorTypes.cnValueIsRequired;
					}
				}

					//#### If the oInputData bIs(a)NumericType and no eErrorType were set above, run the additional numeric sDataType-related tests
				if (bIsNumericType && eErrorType == enumValueErrorTypes.cnNoError) {
						//#### If the sMinimumNumericValue or sMaximumNumericValue have not been set, default to testing the value based on the iNumericPrecision
						//####     NOTE: This is done as a failsafe measure, as well as allowing partial functionality of datasources that have not yet been fully defined within Data.MetaData
						//####     NOTE: This is a "better then nothing" test, as testing the sValue against the iNumericPrecision will not guarentee that the value "fits" into the column
					if (sMinimumNumericValue.Length == 0 || sMaximumNumericValue.Length == 0) {
							//#### Traverse the a_sValues
						for (i = 0; i < iValueCount; i++) {
								//#### If the iNumericPrecision of the current a_sValue is too long, set the eErrorType and fall from the loop
							if (Cn.Data.Tools.NumericPrecision(a_sValues[i]) > iNumericPrecision) {
								eErrorType = enumValueErrorTypes.cnIncorrectLength;
								break;
							}
						}
					}
						//#### Else the sMinimumNumericValue and sMaximumNumericValue have been set
					else {
							//#### Traverse the a_sValues
						for (i = 0; i < iValueCount; i++) {
								//#### If the current a_sValue is outside of the sMinimumNumericValue/sMaximumNumericValue range, set the eErrorType and fall from the loop
							if (! Cn.Data.Tools.IsNumericInRange(a_sValues[i], sMinimumNumericValue, sMaximumNumericValue)) {
								eErrorType = enumValueErrorTypes.cnIncorrectLength;
								break;
							}
						}
					}
				}

					//#### Return the above detemined eErrorType
				return eErrorType;
			}


		} //# public class Values


		///########################################################################################################################
		/// <summary>
		/// Assists in the collection of <c>MetaData</c> data.
		/// </summary>
		///########################################################################################################################
		/// <LastFullCodeReview></LastFullCodeReview>
		public class MetaDataCollectionHelper : CollectionHelper {
				//#### Declare the required private constants
		  //private const string g_cClassName = "Cn.Data.MetaData.MetaDataCollectionHelper";
			private const string g_cColumnNames = "Table_Name,Column_Name,Column_Default,Is_Nullable,Data_Type,Character_Maximum_Length,Numeric_Precision,Numeric_Scale";
			private const string g_cOptionalColummnNames = "MinimumNumericValue,MamimumNumericValue,RelatedPicklist";


			//##########################################################################################
			//# Class Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			///############################################################
			/// <LastUpdated>January 25, 2009</LastUpdated>
			public MetaDataCollectionHelper() : base((g_cColumnNames).Split(','), "", "") {
			}


			//#######################################################################################################
			//# Public Read-Only Properties
			//#######################################################################################################
			///############################################################
			/// <summary>
			/// Retrieves the optionally definable column names present within a MultiArray structure that defines the parent type.
			/// </summary>
			/// <returns>String array where each index represents a MultiArray column name optionally definable by the parent type.</returns>
			///############################################################
			/// <LastUpdated>January 25, 2009</LastUpdated>
			public string[] OptionalColummns {
				get {
					return g_cOptionalColummnNames.Split(',');
				}
			}


			//##########################################################################################
			//# Public Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Retrieves a properly formatted SQL query to collect the data from a data source.
			/// </summary>
			/// <remarks>
			/// For SQL*Server, the <paramref>sTableName</paramref> is as follows - "[DatabaseName].INFORMATION_SCHEMA.COLUMNS".
			/// </remarks>
			/// <param name="sTableName">String representing the table name containing the data.</param>
			/// <param name="eDataSource">Enumeration representing the source the <paramref>oDataSourceMetaData</paramref> was generated from.</param>
			/// <returns>String containing a SQL query referencing the passed <paramref>sTableName</paramref> to collect the data for the parent type.</returns>
			///############################################################
			/// <LastUpdated>January 25, 2009</LastUpdated>
			public string SQLStatement(string sTableName, enumDataSource eDataSource) {
				string sReturn;

					//#### Determine the value of the passed eDataSource and process accordingly
				switch (eDataSource) {
						//#### If this is a .cnOracle request
					case enumDataSource.cnOracle: {
						sReturn = "select TABLE_NAME Table_Name,COLUMN_NAME Column_Name,DATA_DEFAULT Column_Default,DECODE(NULLABLE, 'Y', 'Yes', 'No') Is_Nullable,DATA_TYPE Data_Type,DATA_LENGTH Character_Maximum_Length,DATA_PRECISION Numeric_Precision,DATA_SCALE Numeric_Scale,LOW_VALUE MinimumNumericValue, HIGH_VALUE MamimumNumericValue FROM " + sTableName + " ORDER BY Table_Name, COLUMN_ID";
						break;
					}
						//#### Else this is a standard SQL92 Information Schema implimentation
					default: {
							//#### Congeal the g_cColumnNames, g_cOptionalColummnNames (as null-string SELECTs) and the passed sTableName into the required SELECT statement
						sReturn = "SELECT " + g_cColumnNames + ",'' AS " + g_cOptionalColummnNames.Replace(",", ",'' AS ") + " FROM " + sTableName + " ORDER BY Table_Name, Ordinal_Position";
						break;
					}
				}

					//#### Return the above determined sReturn value to the caller
				return sReturn;
			}
			//############################################################
			//# Hide the underlying functions from our base class (as a redefinition with different parameters is defined above)
			//############################################################
			private new string SQLStatement() { return null; }
			private new string SQLStatement(string sTableName) { return null; }

			///############################################################
			/// <summary>
			/// Retrieves the data for the parent type based on the provided information and data source.
			/// </summary>
			/// <remarks>
			/// For SQL*Server, the <paramref>sTableName</paramref> is as follows - "[DatabaseName].INFORMATION_SCHEMA.COLUMNS".
			/// </remarks>
			/// <param name="oDBMS"><c>DBMS</c> instance representing an active connection to the related data source.</param>
			/// <param name="sTableName">String representing the table name containing the data.</param>
			/// <param name="eDataSource">Enumeration representing the data source type.</param>
			/// <returns><c>MultiArray</c> instance based on the provided information and data source.</returns>
			///############################################################
			/// <LastUpdated>January 25, 2009</LastUpdated>
			public MultiArray Data(DBMS oDBMS, string sTableName, enumDataSource eDataSource) {
					//#### Return a MultiArray of the passed sTableName (utilizing our own .SQL)
				return oDBMS.GetMultiArray(SQLStatement(sTableName, eDataSource));
			}
			//############################################################
			//# Hide the underlying functions from our base class (as a redefinition with different parameters is defined above)
			//############################################################
			private new MultiArray Data(DBMS oDBMS) { return null; }
			private new MultiArray Data(DBMS oDBMS, string sTableName) { return null; }


/*
			public MultiArray Data(DataSet oDataSet) {
				oDataSet.Tables[0].Columns[0].DataType
			}
*/


		} //# public class MetaDataCollectionHelper


	} //# class MetaData


} //# namespace Cn.Database
