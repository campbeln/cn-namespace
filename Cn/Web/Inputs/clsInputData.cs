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
using Cn.Data;										//# Required to access the MetaData, Picklist class
using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Web.Inputs {


	///########################################################################################################################
	/// <summary>
	/// Represents an Input's related metadata.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>November 26, 2009</LastFullCodeReview>
	public class InputData {
			//#### Declare the required private variables
		private IInputCollection g_oParentCollection;
		private AdditionalData g_oAdditionalData = null;		//# We (stupidly/lazily) use this ==null as a flag to determine within DoReset if it was called by a "[Constructor]" or a "Reset"
		private Hashtable gh_oErrorInfo;
		private string[] ga_sValues;
		private string g_sTableName;
		private string g_sColumnName;
		private string g_sInputAlias;
		private string g_sDefaultValue;
		private string g_sMinimumNumericValue;
		private string g_sMaximumNumericValue;
		private int g_iMaximumCharacterLength;
		private int g_iNumericPrecision;
		private int g_iNumericScale;
		protected int g_iSourceRecordNumber;
		private enumSaveTypes g_eSaveType;
		private MetaData.enumDataTypes g_eDataType;
		private MetaData.enumValueTypes g_eValueType;
		private bool g_bIsAttachedToDataSource;
		private bool g_bValueWasSubmitted;
		private bool g_bValueIsFromForm;
		private bool g_bIsNullable;

			//#### Declare the required private constants
			//####     NOTE: The g_cDEFAULT_* definitions below are logicially invalid when possible, thereby forcing the developer to reset those which they need to use
		private const string g_cClassName = "Cn.Web.Inputs.InputData.";
		private const string g_cDEFAULT_MINIMUM_NUMERIC_VALUE = "";
		private const string g_cDEFAULT_MAXIMUM_NUMERIC_VALUE = "";
		private const int g_cDEFAULT_MAXIMUM_CHARACTER_LENGTH = -1;
		private const int g_cDEFAULT_NUMERIC_PRECISION = -1;
		private const int g_cDEFAULT_NUMERIC_SCALE = -1;
		private const MetaData.enumDataTypes g_cDEFAULT_DATA_TYPE = MetaData.enumDataTypes.cnUnknown;
		private const MetaData.enumValueTypes g_cDEFAULT_VALUE_TYPE = MetaData.enumValueTypes.cnSingleValue;
		private const enumSaveTypes g_cDEFAULT_SAVE_TYPE = enumSaveTypes.cnInsertIfPresent;
		private const bool g_cDEFAULT_IS_NULLABLE = true;


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="sTableName">String representing the column's source table name.</param>
		/// <param name="sColumnName">String representing the column name.</param>
		/// <param name="eSaveType">Enumeration representing the HTML input's form processing requirements.</param>
		/// <param name="eValueType">Enumerated value representing the column's stored value type.</param>
		/// <param name="oAdditionalData">AdditionalData representing the additionally definable properties of the input.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> while also defind as not IsNullable.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref name="oAdditionalData.AttachedToDataSource"/> is true and the <paramref>sTableName</paramref>/<paramref>sColumnName</paramref> pair does not exist.</exception>
		/// <seealso cref="Cn.Web.Inputs.InputData.Reset"/>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		public InputData(string sInputAlias, string sTableName, string sColumnName, enumSaveTypes eSaveType, MetaData.enumValueTypes eValueType, AdditionalData oAdditionalData) {
				//#### Pass the call off to our .Reset equivlent
			Reset(sInputAlias, sTableName, sColumnName, eSaveType, eValueType, oAdditionalData);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="sTableName">String representing the column's source table name.</param>
		/// <param name="sColumnName">String representing the column name.</param>
		/// <param name="eSaveType">Enumeration representing the HTML input's form processing requirements.</param>
		/// <param name="eValueType">Enumerated value representing the column's stored value type.</param>
		/// <param name="oAdditionalData">AdditionalDat representing the additionally definable properties of the input.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> while also defind as not IsNullable.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref name="oAdditionalData.AttachedToDataSource"/> is true and the <paramref>sTableName</paramref>/<paramref>sColumnName</paramref> pair does not exist.</exception>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		public void Reset(string sInputAlias, string sTableName, string sColumnName, enumSaveTypes eSaveType, MetaData.enumValueTypes eValueType, AdditionalData oAdditionalData) {
				//#### Pass the call off to .DoReset to set the global vars from the passed arguments and collected MetaData
			DoReset(
				sInputAlias,
				sTableName,
				sColumnName,
				eSaveType,
				g_cDEFAULT_DATA_TYPE,					//# eDataType: This value is retrieved from the MetaData
				eValueType,
				"",										//# sDefaultValue: This value is retrieved from the MetaData
				g_cDEFAULT_IS_NULLABLE,					//# bIsNullable: This value is retrieved from the MetaData
				g_cDEFAULT_MAXIMUM_CHARACTER_LENGTH,	//# iMaximumCharacterLength: This value is retrieved from the MetaData
				g_cDEFAULT_MINIMUM_NUMERIC_VALUE,		//# sMinimumNumericValue: This value is retrieved from the MetaData
				g_cDEFAULT_MAXIMUM_NUMERIC_VALUE,		//# sMaximumNumericValue: This value is retrieved from the MetaData
				g_cDEFAULT_NUMERIC_PRECISION,			//# iNumericPrecision: This value is retrieved from the MetaData
				g_cDEFAULT_NUMERIC_SCALE,				//# iNumericScale: This value is retrieved from the MetaData
				oAdditionalData,
				true									//# Pass in true as this is an bAttachedToDataSource input
			);
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> while also defind as not IsNullable.</exception>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		public InputData(string sInputAlias) {
				//#### Pass the call off to our .Reset equivlent
			Reset(sInputAlias);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> while also defind as not IsNullable.</exception>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		public void Reset(string sInputAlias) {
				//#### Pass the call off to .DoReset to set the global vars from the passed arguments and collected MetaData
				//####     NOTE: All of the unknown values are set to their CONSTANT values, which are logicially invalid, thus requiring the developer to update the ones they will utilize
			DoReset(
				sInputAlias, 
				"",
				"",
				g_cDEFAULT_SAVE_TYPE,
				g_cDEFAULT_DATA_TYPE,
				g_cDEFAULT_VALUE_TYPE,
				"",
				g_cDEFAULT_IS_NULLABLE,
				g_cDEFAULT_MAXIMUM_CHARACTER_LENGTH,
				g_cDEFAULT_MINIMUM_NUMERIC_VALUE,
				g_cDEFAULT_MAXIMUM_NUMERIC_VALUE,
				g_cDEFAULT_NUMERIC_PRECISION,
				g_cDEFAULT_NUMERIC_SCALE,
				new AdditionalData(),
				false
			);
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="eSaveType">Enumeration representing the HTML input's form processing requirements.</param>
		/// <param name="eDataType">Enumerated value representing the column's datatype.</param>
		/// <param name="eValueType">Enumerated value representing the column's stored value type.</param>
		/// <param name="sDefaultValue">String representing the column's default value.</param>
		/// <param name="bIsNullable">Boolean value indicating if the column is permitted to hold a null value.</param>
		/// <param name="iMaximumCharacterLength">Integer representing the column's maximum character length.</param>
		/// <param name="sMinimumNumericValue">String representing the column's minimum numeric value.</param>
		/// <param name="sMaximumNumericValue">String representing the column's maximum numeric value.</param>
		/// <param name="iNumericPrecision">Integer representing the column's numeric precision.</param>
		/// <param name="iNumericScale">Integer representing the column's numeric scale.</param>
		/// <param name="oAdditionalData">AdditionalData representing the additionally definable properties of the input.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> while also defind as not <paramref name="bIsNullable"/>.</exception>
		/// <seealso cref="Cn.Web.Inputs.InputData.Reset"/>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		public InputData(string sInputAlias, enumSaveTypes eSaveType, MetaData.enumDataTypes eDataType, MetaData.enumValueTypes eValueType, string sDefaultValue, bool bIsNullable, int iMaximumCharacterLength, string sMinimumNumericValue, string sMaximumNumericValue, int iNumericPrecision, int iNumericScale, AdditionalData oAdditionalData) {
				//#### Pass the call off to our .Reset equivlent
			Reset(sInputAlias, eSaveType, eDataType, eValueType, sDefaultValue, bIsNullable, iMaximumCharacterLength, sMinimumNumericValue, sMaximumNumericValue, iNumericPrecision, iNumericScale, oAdditionalData);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="eSaveType">Enumeration representing the HTML input's form processing requirements.</param>
		/// <param name="eDataType">Enumerated value representing the column's datatype.</param>
		/// <param name="eValueType">Enumerated value representing the column's stored value type.</param>
		/// <param name="sDefaultValue">String representing the column's default value.</param>
		/// <param name="bIsNullable">Boolean value indicating if the column is permitted to hold a null value.</param>
		/// <param name="iMaximumCharacterLength">Integer representing the column's maximum character length.</param>
		/// <param name="sMinimumNumericValue">String representing the column's minimum numeric value.</param>
		/// <param name="sMaximumNumericValue">String representing the column's maximum numeric value.</param>
		/// <param name="iNumericPrecision">Integer representing the column's numeric precision.</param>
		/// <param name="iNumericScale">Integer representing the column's numeric scale.</param>
		/// <param name="oAdditionalData">AdditionalData representing the additionally definable properties of the input.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> while also defind as not <paramref name="bIsNullable"/>.</exception>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		public void Reset(string sInputAlias, enumSaveTypes eSaveType, MetaData.enumDataTypes eDataType, MetaData.enumValueTypes eValueType, string sDefaultValue, bool bIsNullable, int iMaximumCharacterLength, string sMinimumNumericValue, string sMaximumNumericValue, int iNumericPrecision, int iNumericScale, AdditionalData oAdditionalData) {
				//#### Pass the call off to .DoReset to set the global vars from the passed arguments
			DoReset(
				sInputAlias,
				"",
				"",
				eSaveType,
				eDataType,
				eValueType,
				sDefaultValue,
				bIsNullable,
				iMaximumCharacterLength,
				sMinimumNumericValue,
				sMaximumNumericValue,
				iNumericPrecision,
				iNumericScale,
				oAdditionalData,
				false
			);
		}

		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="oInputData">Object representing the InputData instance to deeply copy into this instance.</param>
		/// <seealso cref="Cn.Web.Inputs.InputData.Reset"/>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		public InputData(InputData oInputData) {
				//#### Pass the call off to our .Reset equivlent
			Reset(oInputData);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="oInputData">Object representing the InputData instance to deeply copy into this instance.</param>
		///############################################################
		/// <LastUpdated>November 27, 2009</LastUpdated>
		public void Reset(InputData oInputData) {
				//#### Pass the call off to .DoReset to set the global vars from the passed oInputData
			DoReset(
				oInputData.InputAlias,
				oInputData.TableName,
				oInputData.ColumnName,
				oInputData.SaveType,
				oInputData.DataType,
				oInputData.ValueType,
				oInputData.DefaultValue,
				oInputData.IsNullable,
				oInputData.MaximumCharacterLength,
				oInputData.MinimumNumericValue,
				oInputData.MaximumNumericValue,
				oInputData.NumericPrecision,
				oInputData.NumericScale,
				oInputData.AdditionalData,
				oInputData.IsAttachedToDataSource
			);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="sInputAlias">String representing the HTML input's unique base name.</param>
		/// <param name="sTableName">String representing the column's source table name.</param>
		/// <param name="sColumnName">String representing the column name.</param>
		/// <param name="eSaveType">Enumeration representing the HTML input's form processing requirements.</param>
		/// <param name="eDataType">Enumerated value representing the column's datatype.</param>
		/// <param name="eValueType">Enumerated value representing the column's stored value type.</param>
		/// <param name="sDefaultValue">String representing the column's default value.</param>
		/// <param name="bIsNullable">Boolean value indicating if the column is permitted to hold a null value.</param>
		/// <param name="iMaximumCharacterLength">Integer representing the column's maximum character length.</param>
		/// <param name="sMinimumNumericValue">String representing the column's minimum numeric value.</param>
		/// <param name="sMaximumNumericValue">String representing the column's maximum numeric value.</param>
		/// <param name="iNumericPrecision">Integer representing the column's numeric precision.</param>
		/// <param name="iNumericScale">Integer representing the column's numeric scale.</param>
		/// <param name="oAdditionalData">AdditionalData representing the additionally definable properties of the input.</param>
		/// <param name="bAttachedToDataSource">Boolean value indicating whether the input represents a field from a <c>MetaData</c>-described datasource.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> while also defind as not <paramref name="bIsNullable"/>.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref name="bAttachedToDataSource"/> is true and the <paramref>sTableName</paramref>/<paramref>sColumnName</paramref> pair does not exist.</exception>
		///############################################################
		/// <LastUpdated>July 15, 2010</LastUpdated>
		private void DoReset(string sInputAlias, string sTableName, string sColumnName, enumSaveTypes eSaveType, MetaData.enumDataTypes eDataType, MetaData.enumValueTypes eValueType, string sDefaultValue, bool bIsNullable, int iMaximumCharacterLength, string sMinimumNumericValue, string sMaximumNumericValue, int iNumericPrecision, int iNumericScale, AdditionalData oAdditionalData, bool bAttachedToDataSource) {
			MultiArray oColumn;
			bool bErrorsOccured = false;

				//#### If this is not a bIsNullable column and the eSaveType is set to .cnInsertNull, raise the error (while determining the calling function via majic ;)
				//####     NOTE: bErrorsOccured doesn't need to be set below as we raise an error
			if (! bIsNullable && eSaveType == enumSaveTypes.cnInsertNull) {
				string sFunction = (g_oAdditionalData == null ? "[Constructor]" : "Reset");
				Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_InsertNullSaveType, sTableName + "." + sColumnName, "");
			  //bErrorsOccured = true;
			}
				//#### Else the passed eSaveType/bIsNullable combo is valid
			else {
					//#### If this input is bAttachedToDataSource
				if (bAttachedToDataSource) {
						//#### Collect the oColumn as the information for the passed sTableName.sColumnName
					oColumn = Settings.MetaData.Column(sTableName, sColumnName);

						//#### If the passed sTableName.sColumnName pair did not exist, raise the error (while determining the calling function via majic ;)
					if (oColumn == null) {
						string sFunction = (g_oAdditionalData == null ? "[Constructor]" : "Reset");
						Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbMetaData_InvalidTableColumnName, sTableName + "." + sColumnName, "");
						bErrorsOccured = true;
					}
						//#### Else the passed sTableName.sColumnName pair .Exists
					else {
							//#### Since this is an bAttachedToDataSource input, set g_sTableName and g_sColumnName
							//####     NOTE: Do not Lowercase the passed sTableName and sColumnName
							//####     NOTE: Due to casing issues between Hashtables' keys and data returned from .ColumnNames/et'la, the casing is preserved (besides, Data.MetaData does case insenstive comparisons internally anyway)
						g_sTableName = sTableName; //# .ToLower();
						g_sColumnName = sColumnName; //# .ToLower();

							//#### Setup the global datasource-related variables from the oColumn values
						g_eDataType = Data.Tools.MakeEnum(oColumn.Value(0, "Data_Type"), MetaData.enumDataTypes.cnUnknown);
						g_sDefaultValue = oColumn.Value(0, "Column_Default");
						g_bIsNullable = Data.Tools.MakeBoolean(oColumn.Value(0, "Is_Nullable"), false);
						g_iMaximumCharacterLength = Data.Tools.MakeInteger(oColumn.Value(0, "Character_Maximum_Length"), 0);
						g_sMinimumNumericValue = oColumn.Value(0, "MinimumNumericValue");
						g_sMaximumNumericValue = oColumn.Value(0, "MamimumNumericValue");
						g_iNumericPrecision = Data.Tools.MakeInteger(oColumn.Value(0, "Numeric_Precision"), 0);
						g_iNumericScale = Data.Tools.MakeInteger(oColumn.Value(0, "Numeric_Scale"), 0);
					}
				}
					//#### Else this is not an input bAttachedToDataSource
				else {
						//#### Since this is an un-bAttachedToDataSource input, set g_sTableName and g_sColumnName to null-strings
					g_sTableName = "";
					g_sColumnName = "";

						//#### Setup the global datasource-related variables from the passed values
					g_eDataType = eDataType;
					g_sDefaultValue = sDefaultValue;
					g_bIsNullable = bIsNullable;
					g_iMaximumCharacterLength = iMaximumCharacterLength;
					g_sMinimumNumericValue = sMinimumNumericValue;
					g_sMaximumNumericValue = sMaximumNumericValue;
					g_iNumericPrecision = iNumericPrecision;
					g_iNumericScale = iNumericScale;
				}

					//#### If no bErrors(have)Occured above
				if (! bErrorsOccured) {
						//#### Ensure the passed oAdditionalData is a valid reference
					if (oAdditionalData == null) {
						oAdditionalData = new AdditionalData();
					}

						//#### Set the global non-datasource-related variables from the passed data
					g_oAdditionalData = oAdditionalData;
					g_sInputAlias = sInputAlias;
					g_eSaveType = eSaveType;
					g_eValueType = eValueType;
					g_bIsAttachedToDataSource = bAttachedToDataSource;

						//#### .Get(the)PicklistName, defaulting to the .cnRelatedPicklist if the developer didn't supply one within oAdditionalData
					oAdditionalData.Picklist_Name = GetPicklistName(sTableName, sColumnName, eValueType, oAdditionalData);

						//#### (Re)Set the default values for the remaining global variables
					gh_oErrorInfo = new Hashtable();
					g_oParentCollection = null;
					ga_sValues = null;
					g_iSourceRecordNumber = -1;		//# NOTE: This is set to match the test value within InputCollection.Get() so that the .Values are updated on the first collection
					g_bValueWasSubmitted = false;
					g_bValueIsFromForm = false;
				}
			}
		}


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the parent <c>InputCollection</c> class related to this instance (if any).
		/// </summary>
		/// <value>InputCollection object that represents the instance's related <c>InputCollection</c> class (if any).</value>
		///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public IInputCollection Parent {
			get { return g_oParentCollection; }
			set { g_oParentCollection = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the collection of values for the input represented by this instance.
		/// </summary>
		/// <remarks>
		/// NOTE: <c>Input</c>s defined as <c>cnBoolean</c>s always return values based on <see cref="Cn.Data.Tools.MakeBooleanInteger">MakeBooleanInteger</see>, where zero equates to false and non-zero equates to true.
		/// </remarks>
		/// <value>String array where each element represents a single value of the collection of values for the input represented by this instance.</value>
		///############################################################
		/// <LastUpdated>May 4, 2010</LastUpdated>
		public string[] Values {
			get { return ga_sValues; }
			set {
				int iLen;
				int i;

					//#### If the passed value(s) is null (or is holding nothing), reset ga_sValues to null
				if (value == null || value.Length == 0 || (value.Length == 1 && value[0].Length == 0)) {
					ga_sValues = new string[0];
				}
					//#### Else the passed value has values to process
				else {
						//#### Determine the passed value's .Length, re-dimensioning ga_sValues to fit
					iLen = value.Length;
					ga_sValues = new string[iLen];

						//#### Determine the g_eDataType and process accordingly
						//####     NOTE: We need to do these value modifications here for these types as their values are not properly reconized until this processing is done
//! neek - this is being done at the wrong juncture, as this screwes up the HasRecordChanged logic
					switch (g_eDataType) {
						case MetaData.enumDataTypes.cnBoolean: {
								//#### Traverse the value
							for (i = 0; i < iLen; i++) {
									//#### If the current value .IsBoolean, coerce it into a .MakeBoolean
									//####     NOTE: Since we know the current ga_sValues is a boolean, we need not worry about the default value of false as it will never be used
									//####     NOTE: Non-boolean values are not processed here as they will be dealt with within .IsValid or elsewhere by the developer
								if (Data.Tools.IsBoolean(value[i])) {
									ga_sValues[i] = Data.Tools.MakeBoolean(value[i], false).ToString();
								}
									//#### Else the current value is not a reconized boolean value, so set it into our ga_sValues as is
									//####     NOTE: We don't simply "blank" these values as other "invalid" values are accepted until the checks within .IsValid, so if we did boolean types would "mysteriously" work differently.
								else {
									ga_sValues[i] = value[i];
								}
							}
							break;
						}
						case MetaData.enumDataTypes.cnCurrency: {
							string sCurrencySymbol = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnLocalization_CurrencySymbol, GetEndUserMessagesLanguageCode());

								//#### Traverse the value
							for (i = 0; i <iLen; i++) {
									//#### Remove the sCurrencySymbol for the current .EndUserMessagesLanguageCode from the current value
									//####     NOTE: This value modification is done due to the IsNumericType checks below (which doesn't like having a sCurrencySymbol on the front of numeric values, and nor do DBMS's)
								ga_sValues[i] = value[i].Replace(sCurrencySymbol, "").TrimStart(' ');
							}
							break;
						}
						default: {
								//#### Traverse the passed value, copying each index into ga_sValues as is
							for (i = 0; i < iLen; i++) {
								ga_sValues[i] = value[i];
							}
							break;
						}
					}
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets the value(s) of the input represented by this instance.
		/// </summary>
		/// <remarks>
		/// NOTE: If the input has multiple values, they will be returned as a <c>Cn.Tools.MultiValueString</c> for a <c>Get</c> call, and will be reset to the single provided string value for a <c>Set</c> call.
		/// </remarks>
		/// <value>String representing the value(s) of the input represented by this instance (serialized as a MultiValueString is multiple values exist).</value>
		///############################################################
		/// <LastUpdated>April 14, 2010</LastUpdated>
		public string Value {
			get {
				string sReturn;

					//#### If there is a null ga_sValues, return a null-string
				if (ga_sValues == null || ga_sValues.Length == 0) {
					sReturn = "";
				}
					//#### Else if there is only a single ga_sValue and this is not a .cnMultiValuesFromPicklist, return it to the caller
				else if (ga_sValues.Length == 1 && g_eValueType != MetaData.enumValueTypes.cnMultiValuesFromPicklist) {
					sReturn = ga_sValues[0];
				}
					//#### Else there is more then one value within ga_sValues, or this is a .cnMultiValuePicklistExType
				else {
						//#### Utilize .MultiValueString within Data.Tools to properly format our ga_sValues (including leading/trailing .PrimaryDelimiters)
					sReturn = Data.Tools.MultiValueString(ga_sValues);
				}

					//#### Return the above determined sReturn value to the caller
				return sReturn;
			}
			set {
				string[] a_sNewValues = new string[1];

					//#### Bundle the passed value into a_sNewValues and pass the call off to our sibling setter implementation of .Values
					//####     NOTE: We do no checking here as it's all done within .Values (not quite as efficient, but there's less duplicated code/logic)
				a_sNewValues[0] = value;
				Values = a_sNewValues;
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets the current error message for the input represented by this instance.
		/// </summary>
		/// <remarks>
		/// NOTE: You must set the <c>Errors</c> to <c>cnCustom</c> before you set the custom <c>ErrorMessage</c>.
		/// </remarks>
		/// <value>String representing the current error message for the input represented by this instance.</value>
		///############################################################
		/// <LastUpdated>March 18, 2010</LastUpdated>
		public string ErrorMessage {
			get {
				ErrorInfo oErrorInfo = gh_oErrorInfo[g_iSourceRecordNumber] as ErrorInfo;
				string sReturn;

					//#### If there is no oErrorInfo for our g_iSourceRecordNumber this it is implicitly a .cnNoError (and the developer hasn't set their own .Message), so set our sReturn value accordingly
				if (oErrorInfo == null) {
					sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_NoError, GetEndUserMessagesLanguageCode());
				}
					//#### Else we have an oErrorInfo for our g_iSourceRecordNumber
				else {
						//#### If the developer has not defined a .Message
					if (oErrorInfo.Message.Length == 0) {
							//#### Determine the .Type, setting our sReturn value accordingly
						switch (oErrorInfo.Type) {
							case MetaData.enumValueErrorTypes.cnNoError: {
								sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_NoError, GetEndUserMessagesLanguageCode());
								break;
							}
							case MetaData.enumValueErrorTypes.cnValueIsRequired: {
								sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_ValueIsRequired, GetEndUserMessagesLanguageCode());
								break;
							}
							case MetaData.enumValueErrorTypes.cnIncorrectLength: {
								sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectLength, GetEndUserMessagesLanguageCode());
								break;
							}
							case MetaData.enumValueErrorTypes.cnIncorrectDataType: {
									//#### Determine the g_eDataType, setting the sReturn value accordingly
								switch (g_eDataType) {
									case MetaData.enumDataTypes.cnBoolean: {
										sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Boolean, GetEndUserMessagesLanguageCode());
										break;
									}
									case MetaData.enumDataTypes.cnInteger: {
										sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Integer, GetEndUserMessagesLanguageCode());
										break;
									}
									case MetaData.enumDataTypes.cnFloat: {
										sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Float, GetEndUserMessagesLanguageCode());
										break;
									}
									case MetaData.enumDataTypes.cnCurrency: {
										sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Currency, GetEndUserMessagesLanguageCode());
										break;
									}
									case MetaData.enumDataTypes.cnDateTime: {
										sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_DateTime, GetEndUserMessagesLanguageCode());
										break;
									}
									case MetaData.enumDataTypes.cnGUID: {
										sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_GUID, GetEndUserMessagesLanguageCode());
										break;
									}
									default: { //# MetaData.enumDataTypes.BinaryType, MetaData.enumDataTypes.CharType, MetaData.enumDataTypes.LongCharType
										sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_Other, GetEndUserMessagesLanguageCode());
										break;
									}
								}
								break;
							}
							case MetaData.enumValueErrorTypes.cnNotWithinPicklist: {
								sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_IncorrectDataType_NotWithinPicklist, GetEndUserMessagesLanguageCode());
								break;
							}
							case MetaData.enumValueErrorTypes.cnUnknownOrUnsupportedType: {
								sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_UnknownOrUnsupportedType, GetEndUserMessagesLanguageCode());
								break;
							}
							case MetaData.enumValueErrorTypes.cnCustom: {
								sReturn = Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_Custom, GetEndUserMessagesLanguageCode());
								break;
							}

								//#### Else the .Type was unreconized
							default: {
								sReturn = Settings.Internationalization.ValueDecoder(Internationalization.enumInternationalizationValues.cnEndUserMessages_UnknownErrorCode, Data.Tools.MakeString(oErrorInfo.Type, ""), "", GetEndUserMessagesLanguageCode(), false);
								break;
							}
						}
					}
						//#### Else the developer has defined a .Message, so set our sReturn value accordingly
					else {
						sReturn = oErrorInfo.Message;
					}
				}

					//#### Return the above determined sReturn value to the caller
				return sReturn;
			}
			set {
				ErrorInfo oErrorInfo = gh_oErrorInfo[g_iSourceRecordNumber] as ErrorInfo;

					//#### If there is no oErrorInfo for our g_iSourceRecordNumber
				if (oErrorInfo == null) {
						//#### Create a new oErrorInfo with the passed value as the .Message and .cnNoError as the .Type, setting it back into the gh_oErrorInfo at our g_iSourceRecordNumber
					oErrorInfo = new ErrorInfo(MetaData.enumValueErrorTypes.cnNoError, value);
					gh_oErrorInfo[g_iSourceRecordNumber] = oErrorInfo;
				}
					//#### Else we have an oErrorInfo for our g_iSourceRecordNumber, so set it's .Message to the passed value
				else {
					oErrorInfo.Message = value;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's default value.
		/// </summary>
		/// <returns>String representing the input's default value.</returns>
		///############################################################
		/// <LastUpdated>November 12, 2009</LastUpdated>
		public string DefaultValue {
			get { return g_sDefaultValue; }
			set { g_sDefaultValue = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's minimum numeric value.
		/// </summary>
		/// <returns>String representing the input's minimum numeric value.</returns>
		///############################################################
		/// <LastUpdated>November 12, 2009</LastUpdated>
		public string MinimumNumericValue {
			get { return g_sMinimumNumericValue; }
			set { g_sMinimumNumericValue = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's maximum numeric value.
		/// </summary>
		/// <returns>String representing the input's maximum numeric value.</returns>
		///############################################################
		/// <LastUpdated>November 12, 2009</LastUpdated>
		public string MaximumNumericValue {
			get { return g_sMaximumNumericValue; }
			set { g_sMaximumNumericValue = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's maximum character length.
		/// </summary>
		/// <returns>Integer representing the input's maximum character length.</returns>
		///############################################################
		/// <LastUpdated>November 12, 2009</LastUpdated>
		public int MaximumCharacterLength {
			get { return g_iMaximumCharacterLength; }
			set { g_iMaximumCharacterLength = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's numeric precision.
		/// </summary>
		/// <returns>Integer representing the input's numeric precision.</returns>
		///############################################################
		/// <LastUpdated>November 12, 2009</LastUpdated>
		public int NumericPrecision {
			get { return g_iNumericPrecision; }
			set { g_iNumericPrecision = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's numeric scale.
		/// </summary>
		/// <returns>Integer representing the input's numeric scale.</returns>
		///############################################################
		/// <LastUpdated>November 12, 2009</LastUpdated>
		public int NumericScale {
			get { return g_iNumericScale; }
			set { g_iNumericScale = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's form processing requirements.
		/// </summary>
		/// <returns>Enumeration representing the input's form processing requirements.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> on a <paramref>sColumnName</paramref> also defind as not nullable.</exception>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public enumSaveTypes SaveType {
			get { return g_eSaveType; }
			set {
					//#### If the perposed values agree, reset the g_eSaveType to the passed value
				if (ValidateSaveTypeIsNullable("SaveType", g_bIsNullable, value)) {
					g_eSaveType = value;
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's data type.
		/// </summary>
		/// <returns>Enumeration representing the input's data type.</returns>
		///############################################################
		/// <LastUpdated>November 12, 2009</LastUpdated>
		public MetaData.enumDataTypes DataType {
			get { return g_eDataType; }
			set { g_eDataType = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value representing the input's stored value type.
		/// </summary>
		/// <returns>Enumeration representing the input's stored value type.</returns>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public MetaData.enumValueTypes ValueType {
			get { return g_eValueType; }
			set {
				g_eValueType = value;

					//#### .Get(the)PicklistName, defaulting to the .cnRelatedPicklist if the developer didn't supply one within oAdditionalData
				g_oAdditionalData.Picklist_Name = GetPicklistName(g_sTableName, g_sColumnName, g_eValueType, g_oAdditionalData);
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets the current error message enumeration for the input represented by this instance.
		/// </summary>
		/// <remarks>
		/// NOTE: You must set the <c>Errors</c> to <c>cnCustom</c> before you set a custom <c>ErrorMessage</c> here.
		/// </remarks>
		/// <value>Enumeration that represents the current error message for the input represented by this instance.</value>
		///############################################################
		/// <LastUpdated>March 18, 2010</LastUpdated>
		public MetaData.enumValueErrorTypes ErrorType {
			get {
					//#### Collect the oErrorInfo for our g_iSourceRecordNumber (if any), returning it's .Type or .cnNoError (depending on it's existance)
				ErrorInfo oErrorInfo = gh_oErrorInfo[g_iSourceRecordNumber] as ErrorInfo;
				return (oErrorInfo == null ? MetaData.enumValueErrorTypes.cnNoError : oErrorInfo.Type);
			}
			set {
					//#### If the passed value is a .cnNoError, .Remove the gh_oErrorInfo for our g_iSourceRecordNumber (if any)
				if (value == MetaData.enumValueErrorTypes.cnNoError) {
					gh_oErrorInfo.Remove(g_iSourceRecordNumber);
				}
					//#### Else an .ErrorType is being set, so create a new gh_oErrorInfo instance for the g_iSourceRecordNumber (resetting the .Message to a null-string as we go)
				else {
					gh_oErrorInfo[g_iSourceRecordNumber] = new ErrorInfo(value, "");
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating whether the value(s) of the input represented by this instance are from the user-submitted form.
		/// </summary>
		/// <remarks>
		/// NOTE: This property is initially loaded with a value representing if the value(s) were submitted by the user. If <c>Renderer</c> is currently processing the <c>Form</c>, this value is set to true if the <c>Value</c> is not equal to the <c>DefaultValue</c>. Otherwise this value is set to false.
		/// </remarks>
		/// <returns>Boolean value indicating whether the value(s) of the input represented by this instance are from the user-submitted form.</returns>
		///############################################################
		/// <LastUpdated>June 29, 2005</LastUpdated>
		public bool ValueIsFromForm {
			get { return g_bValueIsFromForm; }
			set { g_bValueIsFromForm = value; }
		}

public bool ValueWasSubmitted {
	get { return g_bValueWasSubmitted; }
	set { g_bValueWasSubmitted = value; }
}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if the column permits null values.
		/// </summary>
		/// <returns>Boolean value indicating if the column permits null values.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eSaveType</paramref> is set as <c>cnInsertNull</c> on a <paramref>sColumnName</paramref> also defind as not nullable.</exception>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public bool IsNullable {
			get { return g_bIsNullable; }
			set {
					//#### If the perposed values agree, reset g_bIsNullable to the passed value
				if (ValidateSaveTypeIsNullable("IsNullable", value, g_eSaveType)) {
					g_bIsNullable = value;
				}
			}
		}


		//##########################################################################################
		//# Public Read-Only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets a value representing the additionally defined properties of the input represented by this instance (see <c>Renderer.Form.Add</c>).
		/// </summary>
		/// <remarks>
		/// NOTE: This value is returned by reference, so the values can be modified.
		/// <para/>NOTE: Since the values are modifiable, the developer could reset the Picklist_Name to an unreconized name or a null-string, but we cannot do anything here so it has to be handeled by the control renderer.
		/// </remarks>
		/// <returns>AdditionalData that represents the additionally defined properties of the input represented by this instance (see <c>Renderer.Form.Add</c>).</returns>
		///############################################################
		/// <LastUpdated>November 14, 2009</LastUpdated>
		public AdditionalData AdditionalData {
			get { return g_oAdditionalData; }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the source record numbers with errors registered against them.
		/// </summary>
		/// <returns>Array of Integers representing the source record numbers with errors registered against them.</returns>
		///############################################################
		/// <LastUpdated>March 19, 2010</LastUpdated>
		public int[] ErroredRecordNumbers {
			get {
				int[] a_sReturn = new int[gh_oErrorInfo.Count];

				gh_oErrorInfo.Keys.CopyTo(a_sReturn, 0);

				return a_sReturn;
			}
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the HTML input's base name.
		/// </summary>
		/// <returns>String representing the HTML input's unique base name.</returns>
		///############################################################
		/// <LastUpdated>June 29, 2004</LastUpdated>
		public string InputAlias {
			get { return g_sInputAlias; }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the input's source table name.
		/// </summary>
		/// <returns>String representing the input's source table name.</returns>
		///############################################################
		/// <LastUpdated>June 29, 2004</LastUpdated>
		public string TableName {
			get { return g_sTableName; }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing the input's source column name.
		/// </summary>
		/// <returns>String representing the input's source column name.</returns>
		///############################################################
		/// <LastUpdated>June 29, 2004</LastUpdated>
		public string ColumnName {
			get { return g_sColumnName; }
		}

		///############################################################
		/// <summary>
		/// Gets a 1-based value representing the source record's number.
		/// </summary>
		/// <returns>1-based integer representing the source record's number.</returns>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		public int SourceRecordNumber {
			get { return g_iSourceRecordNumber; }
		}
			internal void SetSourceRecordNumber(int iSourceRecordNumber) {
				g_iSourceRecordNumber = iSourceRecordNumber;
			}

		///############################################################
		/// <summary>
		/// Gets a value indicating whether the input represents a field from a <c>MetaData</c>-described datasource.
		/// </summary>
		/// <returns>Boolean value indicating whether the input represents a field from a <c>MetaData</c>-described datasource.</returns>
		///############################################################
		/// <LastUpdated>November 12, 2009</LastUpdated>
		public bool IsAttachedToDataSource {
			get { return g_bIsAttachedToDataSource; }
		}

		///############################################################
		/// <summary>
		/// Gets a value representing if any of the source record numbers associated with this input have errors registered against them.
		/// </summary>
		/// <returns>Boolean value representing if any of the source record numbers associated with this input have errors registered against them.</returns>
		///############################################################
		/// <LastUpdated>March 19, 2010</LastUpdated>
		public bool ErrorsRegistered {
			get {
				return (gh_oErrorInfo.Count > 0);
			}
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating if this instance is a multi-value input.
		/// </summary>
		/// <returns>Boolean value if this instance is a multi-value input.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public bool IsMultiValue {
			get { return (g_eValueType == MetaData.enumValueTypes.cnMultiValuesFromPicklist); }
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating if this input is valid.
		/// </summary>
		/// <returns>Boolean value indicating if this input is valid.</returns>
		///############################################################
		/// <LastUpdated>March 26, 2010</LastUpdated>
		public bool IsValid {
			get {
				MetaData.enumValueErrorTypes eErrorType;
				bool bIsIDColumnForNewRecord;
				bool bLogicalIsNullable;
				bool bReturn = true;

					//#### Collect the .ErrorType from our property (which manages the g_oErrorInfo Hash for us)
				eErrorType = this.ErrorType;

					//#### If there is already an enumInputErrors registered against this input, set our bReturn value to false
				if (eErrorType != MetaData.enumValueErrorTypes.cnNoError) {
					bReturn = false;
				}
					//#### Else we need to check to see if we are valid
				else {
						//#### Set bIsIDColumnForNewRecord based on if this is an .cnID and a .IsNewRecord
//! not certian I like this logic
					bIsIDColumnForNewRecord = (
						g_eSaveType == enumSaveTypes.cnID &&
						HaveARelatedForm() && ((Renderer.Form.FormInputCollection)g_oParentCollection).Parent.IsNewRecord
					);

						//#### Determine the bLogicalIsNullable based on g_eSaveType or g_bIsNullable
						//####     NOTE: This is necessary as we do not pass in the g_eSaveType into .Validate so we determine the "logical" value of g_bIsNullable based on how we have been advised to actually save the value
					switch (g_eSaveType) {
						case enumSaveTypes.cnIgnore:
						case enumSaveTypes.cnInsertIfPresent:
						case enumSaveTypes.cnInsertNullString: {
							bLogicalIsNullable = true;
							break;
						}
						default: {
							bLogicalIsNullable = g_bIsNullable;
							break;
						}
					}

						//#### Pass the call off to .Values.Validate, collecting it's return value into our local eErrorType
					eErrorType = MetaData.Values.Validate(
						ga_sValues,
						g_eDataType,
						g_eValueType,
						bLogicalIsNullable,
						g_iMaximumCharacterLength,
						g_sMinimumNumericValue,
						g_sMaximumNumericValue,
						g_iNumericPrecision,
						g_iNumericScale,
						bIsIDColumnForNewRecord,
						(g_oParentCollection == null ? null : g_oParentCollection.Picklists),	//# If we have a g_oParentCollection, pass in it's .Picklists, else pass in a null
						g_oAdditionalData.Picklist_Name,
						g_oAdditionalData.Picklist_IsAdHoc
					);

						//#### If an error was identified within .Validate above, flip our bReturn value and set the eErrorType via our property (which manages the g_oErrorInfo Hash for us)
					if (eErrorType != MetaData.enumValueErrorTypes.cnNoError) {
						bReturn = false;
						this.ErrorType = eErrorType;
					}
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}
		}


		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Retrieves the logged error information for the referenced record number.
		/// </summary>
		/// <param name="iRecordNumber">Integer representing the referenced record number.</param>
		/// <returns>Object representing the logged error information for the referenced record number.</returns>
		///############################################################
		/// <LastUpdated>April 1, 2010</LastUpdated>
		public ErrorInfo Errors(int iRecordNumber) {
			ErrorInfo oErrorInfo = gh_oErrorInfo[iRecordNumber] as ErrorInfo;
			ErrorInfo oReturn;

				//#### If there was no oErrorInfo for the passed iRecordNumber, set our oReturn value to a logicially blank entry
			if (oErrorInfo == null) {
				oReturn = new ErrorInfo(
					MetaData.enumValueErrorTypes.cnNoError,
					Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_NoError, GetEndUserMessagesLanguageCode())
				);
			}
				//#### Else an oErrorInfo exists for the passed iRecordNumber, so copy it into our oReturn value
			else {
				oReturn = new ErrorInfo(
					oErrorInfo.Type,
					oErrorInfo.Message
				);
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Returns the snipit of JavaScript code to validate the input. 
		/// </summary>
		/// <remarks>
		/// This function allows the developer to easily implement their own custom version of <c>RenderValidationJS</c>.
		/// </remarks>
		/// <param name="sFormID">String representing the HTML input's parent Form ID.</param>
		/// <returns>String representing a snipit of JavaScript code to validate the input.</returns>
		///############################################################
		/// <LastUpdated>December 3, 2009</LastUpdated>
		public string ValidationJavaScript(string sFormID) {
			string sIsRequired;
			string sInput;
			string sReturn = "";
			
				//#### If the current g_eValueType is not a .Verbatum
				//####     NOTE: .VerbatumExType's are not checked on either the client or server side because the value is set verbatum (duh!). This ExType exists so that the developer can force seemingly non-valid data into the datatype (i.e. - "GETDATE()" into a date column).
			if (g_eValueType != MetaData.enumValueTypes.cnVerbatum) {
					//#### If this input g_bIs(not)Nullable, set sIsRequired to true
				if (! g_bIsNullable) {
					sIsRequired = "true";

						//#### If this is an ID and we .HaveARelatedForm, reset sIsRequired to determine if this .Is(a)NewRecord on a per-record basis so set sIsRequired as necessary
					if (g_eSaveType == enumSaveTypes.cnID && HaveARelatedForm()) {
						sIsRequired = "(Cn._.wrf.RecordMode(" + sFormID + ", iCurrentRecord) != Cn._.wrf.enumRecordTrackerModes.cnNew)";
					}
				}
					//#### Else this input is not required, so set sIsRequired to false
				else {
					sIsRequired = "false";
				}

					//#### If we .HaveARelatedForm, setup the sInput to collect a Renderer.Form-based input
				if (HaveARelatedForm()) {
					sInput = "Cn._.wrf.Input(" + sFormID + ", '" + Inputs.Tools.EscapeCharacters(g_sInputAlias, "'") + "', iCurrentRecord)";
				}
					//#### Else we do not .HaveARelatedForm, so setup sInput to collect the input via it's ID
				else {
					sInput = "Cn._.wi.Get('" + Inputs.Tools.EscapeCharacters(g_sInputAlias, "'") + "')";
				}

					//#### If this is a .cnMultiValuePicklistExType or .cnSingleValueFromPicklist g_eValueType, only test for them being required
					//####     NOTE: .cnSingleValuePicklistExType's can be tested below because they refer to a single value in a column, so there is no need to catch them here.
					//####     NOTE: .cnSingleValueSearchInMultiValuePicklistExType and .cnMultiValueSearchInSingleValuePicklistExType are implicitly included in the check below due to the nature of their values (which include .cnMultiValuePicklistExType)
					//####     NOTE: Complex .*PicklistExType checking is not done on the client side, as they are goverened by rendered lists of data. If a neferious end user has taken the time to modify said lists, they would also modify this test script making the test moot. .*PicklistExTypes' individually submitted values are checked on the server side for datatype and length.
				if (g_eValueType == MetaData.enumValueTypes.cnMultiValuesFromPicklist ||
					g_eValueType == MetaData.enumValueTypes.cnSingleValueFromPicklist
				) {
					sReturn = "Cn._.wiv.IsRequired(" + sInput + ", " + sIsRequired + ")";
				}
					//#### Else we need to check the g_eDataType
				else {
						//#### Determine the g_eDataType and set our sReturn value accordingly
					switch (g_eDataType) {
						case MetaData.enumDataTypes.cnBoolean: {
							sReturn = "Cn._.wiv.IsBoolean(" + sInput + ", " + sIsRequired + ")";
							break;
						}
						case MetaData.enumDataTypes.cnInteger: {
							sReturn = "Cn._.wiv.IsInteger(" + sInput + ", " + sIsRequired + ", " + g_iNumericPrecision + ", '" + g_sMinimumNumericValue + "', '" + g_sMaximumNumericValue + "')";
							break;
						}
						case MetaData.enumDataTypes.cnFloat: {
							sReturn = "Cn._.wiv.IsFloat(" + sInput + ", " + sIsRequired + ", " + g_iNumericPrecision + ", '" + g_sMinimumNumericValue + "', '" + g_sMaximumNumericValue + "')";
							break;
						}
						case MetaData.enumDataTypes.cnCurrency: {
							sReturn = "Cn._.wiv.IsCurrency(" + sInput + ", " + sIsRequired + ", " + g_iNumericPrecision + ", '" + g_sMinimumNumericValue + "', '" + g_sMaximumNumericValue + "')";
							break;
						}
						case MetaData.enumDataTypes.cnDateTime: {
//! make sure bool.ToString() returns "true" and "false"
							sReturn = "Cn._.wiv.IsDate(" + sInput + ", " + sIsRequired + ", " + g_oAdditionalData.DateTime_ValidateDataType.ToString().ToLower() + ")";
							break;
						}
						case MetaData.enumDataTypes.cnGUID: {
							sReturn = "Cn._.wiv.IsGUID(" + sInput + ", " + sIsRequired + ")";
							break;
						}
						case MetaData.enumDataTypes.cnUnknown:
						case MetaData.enumDataTypes.cnUnsupported: {
								//#### This is a .cnUnknown or .cnUnsupported g_iDataType, so raise the error
							Internationalization.RaiseDefaultError(g_cClassName + "GetValidationScript", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_UnsupportedDataType, g_sInputAlias, "");
							break;
						}
						case MetaData.enumDataTypes.cnBinary: {
								//#### This is a .cnBinary g_iDataType, so return a null-string as no checking is done on the client side
							sReturn = "";
							break;
						}

							//#### Else this is a string-based g_iDataType
						default: {
							sReturn = "Cn._.wiv.IsString(" + sInput + ", " + sIsRequired + ", " + g_iMaximumCharacterLength + ")";
							break;
						}
					}
				}
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Renders the error message associated with the <c>Form</c> input if an error is defined.
		/// </summary>
		/// <remarks>
		/// This function will render the error message associated with the <c>Form</c> input if an error has been defined. If no errors have been defined for the <c>Form</c> input, this function will return a null-string. This functionality allows you to avoid having to test each input for errors, and if an input has an error write out its <see cref="ErrorMessage" />. Simply call this function for all of your inputs if you have choosen not to use the popup style messages, and only those with errors will be rendered to the screen. The use of <paramref>sMessageHead</paramref> and <paramref>sMessageTail</paramref> allow you to further customize these error messages.
		/// </remarks>
		/// <param name="sMessageHead">String representing the data to precede the error message.</param>
		/// <param name="sMessageTail">String representing the data to follow the error message.</param>
		/// <returns>String representing the error message associated with the <c>Form</c> input surrounded by the passed <paramref name="sMessageHead"/> and <paramref name="sMessageTail"/>. A null-string is returned if the <c>Form</c> input is not in error.</returns>
		///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public string FormatErrorMessage(string sMessageHead, string sMessageTail) {
			string sReturn = "";

				//#### If our .Value .Is(not)Valid
			if (! IsValid) {
					//#### Reset our sReturn value to the ErrorMessage (calling our own property so that the g_sErrorMessage is properly calculated) surrounded by the passed sMessageHead/sMessageTail
				sReturn = sMessageHead + ErrorMessage + sMessageTail;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}


		//##########################################################################################
		//# Private Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Sets the PicklistName for set for picklist-based inputs.
		/// </summary>
		/// <remarks>
		/// NOTE: This function modifies the <paramref name="oAdditionalData"/> if the <c>cnRelatedPicklist</c> functionality is being utilized.
		/// </remarks>
		/// <param name="sTableName">String representing the column's source table name.</param>
		/// <param name="sColumnName">String representing the column name.</param>
		/// <param name="eValueType">Enumerated value representing the column's stored value type.</param>
		/// <param name="oAdditionalData">AdditionalData representing the additionally definable properties of the input.</param>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		private string GetPicklistName(string sTableName, string sColumnName, MetaData.enumValueTypes eValueType, AdditionalData oAdditionalData) {
			string sReturn = oAdditionalData.Picklist_Name;

				//#### If this g_sInputAlias g_bIsAttachedToDataSource and the passed eValueType specified a picklist type
				//####     NOTE: We need to ensure that this g_sInputAlias g_bIsAttachedToDataSource as we are called from multiple places
			if (g_bIsAttachedToDataSource && (
				eValueType == MetaData.enumValueTypes.cnSingleValueFromPicklist ||
				eValueType == MetaData.enumValueTypes.cnMultiValuesFromPicklist
			)) {
					//#### If this g_sInputAlias is not a .Picklist_IsAdHoc and a .Picklist_Name has not been set (which is currently stored in our sReturn value)
				if (! oAdditionalData.Picklist_IsAdHoc && sReturn.Length == 0) {
						//#### Collect the .cnRelatedPicklist from the .MetaData, resetting our sReturn value with the .cnRelatedPicklist
					sReturn = Settings.MetaData.Value(sTableName, sColumnName, MetaData.enumMetaDataTypes.cnRelatedPicklist);
				}

					//#### If the sPicklistName has not been defined, raise the error and flip our bReturn value
					//####     NOTE: The oAdditionalData.Picklist_Name is checked at .IsValid and at render, so we do not need to test it here
					//####     NOTE: Since the developer can reset/chane the value within the oAdditionalData without a proper hook for validating the .Picklist_Name, checking here only partially enforces the rule so it's better to do it at .IsValid and render
			  //if (sPicklistName.Length == 0) {
			  //	Internationalization.RaiseDefaultError(g_cClassName + sFunction, Cn.Configuration.Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_PicklistNameNotDefined, sInputAlias, "");
			  //	bReturn = false;
			  //}
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Determines if the passed IsNullable and SaveType agree (as some combonations are mutually exclusive)
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="bIsNullable">Boolean value indicating if the column is permitted to hold a null value.</param>
		/// <param name="eSaveType">Enumeration representing the HTML input's form processing requirements.</param>
		/// <returns>Boolean value indicating is the passed variables are in agreement.</returns>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		private bool ValidateSaveTypeIsNullable(string sFunction, bool bIsNullable, enumSaveTypes eSaveType) {
			bool bReturn = true;

				//#### If the column bIs(not)Nullable and the eSaveType is .cnInsertNull, reset our bReturn value to false and raise the error
			if (! bIsNullable && eSaveType == enumSaveTypes.cnInsertNull) {
				bReturn = false;
				Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_InsertNullSaveType, g_sTableName + "." + g_sColumnName, "");
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Safely retrieves the End User Messages Language Code.
		/// </summary>
		/// <returns>String representing the End User Messages Language Code.</returns>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		private string GetEndUserMessagesLanguageCode() {
				//#### If we have a g_oParentCollection, send in it's .Settings to determine the .EndUserMessagesLanguageCode
			if (g_oParentCollection != null) {
				return Settings.EndUserMessagesLanguageCode(g_oParentCollection.Settings);
			}
				//#### Else we don't have a g_oParentCollection, so send in null as the oSettings (which will get us the default .LanguageCode from the global Internationalization instance)
			else {
				return Settings.EndUserMessagesLanguageCode(null);
			}
		}

		///############################################################
		/// <summary>
		/// Determines if there is a Renderer.Form object that is assoicated with our Parent.
		/// </summary>
		/// <returns>Boolean value indicating if there is a Renderer.Form object that is assoicated with our Parent.</returns>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		private bool HaveARelatedForm() {
				//#### Return based on if we have a g_oParentCollection that is a Form.InputCollection
			return (g_oParentCollection != null && g_oParentCollection.GetType() == typeof(Renderer.Form.FormInputCollection));
		}


	} //# class InputData


	///########################################################################################################################
	/// <summary>
	/// Represents the error data for a specific Input record number.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	public class ErrorInfo {
		/// <summary>Gets the current error message for the input represented by this instance.</summary>
		public string Message { get; internal set; }
		/// <summary>Gets the current error message enumeration for the input represented by this instance.</summary>
		public MetaData.enumValueErrorTypes Type { get; internal set; }


		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="eType">Enumeration representing the current error message enumeration for the input represented by this instance</param>
		/// <param name="sMessage">String representing the current error message for the input represented by this instance.</param>
		///############################################################
		/// <LastUpdated>April 1, 2010</LastUpdated>
		public ErrorInfo(MetaData.enumValueErrorTypes eType, string sMessage) {
			Message = sMessage;
			Type = eType;
		}
	}

	///########################################################################################################################
	/// <summary>
	/// Represents the additional data for an Input.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	public class AdditionalData {
			//#### Declare the required private/protected variables
		private Hashtable gh_sData;


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>September 26, 2007</LastUpdated>
		public AdditionalData() {
				//#### Initilize the global objects
			gh_sData = new Hashtable();
		}
/*
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="h_sData">Hashtable containing the data to load into this instance</param>
		///############################################################
		/// <LastUpdated>November 15, 2009</LastUpdated>
		public AdditionalData(Hashtable h_sData) {
				//#### Initilize the global objects
			gh_sData = Platform.Specific.DeepCopy(h_sData);
		}
*/
		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		///############################################################
		/// <LastUpdated>September 26, 2007</LastUpdated>
		public void Reset() {
				//#### Reset the global variables
			gh_sData.Clear();
		}


		//##########################################################################################
		//# Read-Only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the Hashtable that represents this instance's underlying data.
		/// </summary>
		/// <value>Hashtable that represents this instance's underlying data.</value>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		public Hashtable Data {
			get { return Platform.Specific.DeepCopy(gh_sData); }
		}


		//##########################################################################################
		//# Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the value for the referenced key.
		/// </summary>
		/// <param name="sKey">String representing the key for the key/value pair to get/set.</param>
		/// <value>String representing the value for the referenced key.</value>
		///############################################################
		/// <LastUpdated>April 9, 2010</LastUpdated>
		public string this[string sKey] {
			get { return Cn.Data.Tools.MakeString(gh_sData[sKey], ""); }
			set { gh_sData[sKey] = Cn.Data.Tools.MakeString(value, ""); }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the name of the input's associated picklist.
		/// </summary>
		/// <remarks>
		/// A value is required for inputs with a <paramref>eSaveType</paramref> of <c>cnSingleValuePicklistExType</c> and/or <c>cnMultiValuePicklistExType</c> and not defined as <c>Picklist_IsAdHoc</c>.
		/// <para/>If a <c>Picklist_Name</c> is not defined and this is a datasource attached input, the <c>Picklists.ColumnAssociationsPicklistName</c> is queried for the passed <paramref>sTableName</paramref>/<paramref>sColumnName</paramref> pair.
		/// <para/>There is no default value for this key.
		/// </remarks>
		/// <value>String representing the name of the input's associated picklist.</value>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public string Picklist_Name {
			get { return Cn.Data.Tools.MakeString(gh_sData["Picklist_Name"], ""); }
			set { gh_sData["Picklist_Name"] = Cn.Data.Tools.MakeString(value, ""); }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value that specifies if the picklist associated with the input will be defined at runtime by the developer.
		/// </summary>
		/// <remarks>
		/// This value is revelent for inputs with a <paramref>eSaveType</paramref> of <c>cnSingleValuePicklistExType</c> and/or <c>cnMultiValuePicklistExType</c>.
		/// <para/>The default value for this key is 'false'.
		/// </remarks>
		/// <value>Boolean value representing if the picklist associated with the input will be defined at runtime by the developer.</value>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public bool Picklist_IsAdHoc {
			get { return Cn.Data.Tools.MakeBoolean(gh_sData["Picklist_IsAdHoc"], false); }
			set { gh_sData["Picklist_IsAdHoc"] = Cn.Data.Tools.MakeBoolean(value, false); }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value that specifies if the input is rendered by <see cref='InputCollection.Get'>Get</see> with an <c>eInputType</c> of <c>cnSelect</c>, <c>cnComboBox</c>, or <c>cnMultiSelect</c> should include a leading blank option.
		/// </summary>
		/// <remarks>
		/// This value is revelent for inputs with a <paramref>eSaveType</paramref> of <c>cnSingleValuePicklistExType</c> and/or <c>cnMultiValuePicklistExType</c>.
		/// <para/>The default value for this key is 'false'.
		/// </remarks>
		/// <value>Boolean value representing if the input is rendered by <see cref='InputCollection.Get'>Get</see> with an <c>eInputType</c> of <c>cnSelect</c>, <c>cnComboBox</c>, or <c>cnMultiSelect</c> should include a leading blank option.</value>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public bool Picklist_AddLeadingBlankOption {
			get { return Cn.Data.Tools.MakeBoolean(gh_sData["Picklist_AddLeadingBlankOption"], false); }
			set { gh_sData["Picklist_AddLeadingBlankOption"] = Cn.Data.Tools.MakeBoolean(value, false); }
		}


// <para/>Available <paramref>h_sAdditionalData</paramref> key definitions for inputs rendered as <c>cnOption</c>s, <c>cnCheckboxes</c>s, <c>cnCheckedListBox</c>s, <c>cnOptionListBox</c>s, or read-only <c>cnMultiSelect</c>s are:
// <para/>* <c>MultiValue_Delimiter</c> is a string that defines the multiple value delimiter for the input. The default value for this key is a null-string.
// <para/>* <c>MultiValue_InputAttributes</c> is a string that defines the HTML attribute code that will be included within all of the <c>Option</c> or <c>Checkbox</c> inputs. The default value for this key is a null-string.
// <para/>* <c>MultiValue_LabelSpacer</c> is a string that defines the multiple value label spacer (placed between the label and the value) for the input. The default value for this key is a null-string.
// <para/>* <c>MultiValue_PrintLabelFirst</c> is a string that is boolean in nature ('true' or 'false') that defines if the label will be printed before or after each value for the input. The default value for this key is 'false'.
		public string MultiValue_Delimiter {
			get { return Cn.Data.Tools.MakeString(gh_sData["MultiValue_Delimiter"], ""); } //# ", " as default?
			set { gh_sData["MultiValue_Delimiter"] = Cn.Data.Tools.MakeString(value, ""); }
		}

		public string MultiValue_InputAttributes {
			get { return Cn.Data.Tools.MakeString(gh_sData["MultiValue_InputAttributes"], ""); }
			set { gh_sData["MultiValue_InputAttributes"] = Cn.Data.Tools.MakeString(value, ""); }
		}

//! not used!?
		public string MultiValue_LabelSpacer {
			get { return Cn.Data.Tools.MakeString(gh_sData["MultiValue_LabelSpacer"], ""); }
			set { gh_sData["MultiValue_LabelSpacer"] = Cn.Data.Tools.MakeString(value, ""); }
		}

//! not used!?
		public bool MultiValue_PrintLabelFirst {
			get { return Cn.Data.Tools.MakeBoolean(gh_sData["MultiValue_PrintLabelFirst"], false); }
			set { gh_sData["MultiValue_PrintLabelFirst"] = Cn.Data.Tools.MakeBoolean(value, false); }
		}


// <para/>Available <paramref>h_sAdditionalData</paramref> key definitions for inputs rendered as <c>cnCheckedListBox</c>s or <c>cnOptionListBox</c>s include the above defined <c>MultiValue_*</c> keys as well as:
// <para/>* <c>SpecialListBox_IncludeHoverJavaScript</c> is a string that is boolean in nature ('true' or 'false') that defines if the CSS hover Javascript contained within 'SpecialListBox.js' will be included. The default value for this key is 'true'.
// <para/>* <c>SpecialListBox_LabelAttributes</c> is a string that defines the HTML attribute code that will be included within all of the <c>cnCheckedListBox</c>s or <c>cnOptionListBox</c>s label tags. If <c>SpecialListBox_IncludeHoverJavaScript</c> evaluated to 'true', the default value for this key defines the <c>onMouseOver</c> and <c>onMouseOut</c> attributes referencing the <c>CSSHover</c> JavaScript function to add CSS <c>:hover</c> functionality to browsers that do not natively support it (such as IE). If <c>SpecialListBox_IncludeHoverJavaScript</c> evaluated to 'false', the default value for this key is a null-string.
// <para/>* <c>SpecialListBox_CSSClass</c> is a string that defines the CSS class that will be applied to all of the <c>cnCheckedListBox</c>s or <c>cnOptionListBox</c>s.
//! not used!?
		public bool SpecialListBox_IncludeHoverJavaScript {
			get { return Cn.Data.Tools.MakeBoolean(gh_sData["SpecialListBox_IncludeHoverJavaScript"], true); }
			set { gh_sData["SpecialListBox_IncludeHoverJavaScript"] = Cn.Data.Tools.MakeBoolean(value, true); }
		}

//! not used!?
		public string SpecialListBox_LabelAttributes {
			get { return Cn.Data.Tools.MakeString(gh_sData["SpecialListBox_LabelAttributes"], ""); }
			set { gh_sData["SpecialListBox_LabelAttributes"] = Cn.Data.Tools.MakeString(value, ""); }
		}

//! not used!?
		public string SpecialListBox_CSSClass {
			get { return Cn.Data.Tools.MakeString(gh_sData["SpecialListBox_CSSClass"], ""); }
			set { gh_sData["SpecialListBox_CSSClass"] = Cn.Data.Tools.MakeString(value, ""); }
		}


// <para/>Available <paramref>h_sAdditionalData</paramref> key definitions for <c>cnDateTime</c> inputs are:
// <para/>* <c>DateTime_Format</c> is a string that defines the <c>Renderer</c> date/time format to apply to the input's value. This value is required by <c>RendererSearchForm</c> but is optional for <c>Renderer.Form</c> inputs. The default value is <c>cnDate_DateFormat</c> defined within the <c>Internationalization</c> data for <c>cnDate</c> rendered inputs. The default value is <c>cnDate_TimeFormat</c> defined within the <c>Internationalization</c> data for <c>cnTime</c> rendered inputs. The default value is <c>cnDate_DateTimeFormat</c> defined within the <c>Internationalization</c> data for <c>cnDateTime</c> rendered inputs.
// <para/>* <c>DateTime_ShowPopUpButton</c> is a string that is boolean in nature ('true' or 'false') that defines if the date/time selector's popup button is rendered. If this value evaluates to 'false' (signaling that the button is not supposed to be rendered), then the DHTML control's 'onClick' code is appended into the inputs attributes when the control is rendered (i.e. 'onClick="Cn._.wid.Show(...)"' is automagicially added into the attributes of the input). The default value for this key is 'true'.
// <para/>* <c>DateTime_PopupButtonCode</c> is a string that defines the HTML code utilized to render the date/time selector's popup button. The default value for this key is an HTML image tag referencing 'Renderer/pics/calendar.gif' with the required 'onclick' JavaScript code.
// <para/>* <c>DateTime_UserMustUseControl</c> is a string that is boolean in nature ('true' or 'false') that defines if the user is required to input the date/time via the DHTML control (i.e. if 'READONLY="true"' is automagicially added into the attributes of the input). The default value for this key is 'true'.
// <para/>* <c>DateTime_ValidateDataType</c> is a string that is boolean in nature ('true' or 'false') that defines if the client side JavaScript will validate the input as being of a known date format. This is useful if you are using a date format that is non-standard or otherwise unrecnoized by JavaScript's Date object. The default value for this key is 'true'.
// <para/>* <c>DateTime_HourIncrement</c> is a string that is numeric in nature that defines the increment value for the hours input. The default value for this key is '1'.
// <para/>* <c>DateTime_MinuteIncrement</c> is a string that is numeric in nature that defines the increment value for the minutes input. The default value for this key is '1'.
// <para/>* <c>DateTime_SecondIncrement</c> is a string that is numeric in nature that defines the increment value for the seconds input. The default value for this key is '1'.
		public string DateTime_Format {
			get { return Cn.Data.Tools.MakeString(gh_sData["DateTime_Format"], ""); }
			set { gh_sData["DateTime_Format"] = Cn.Data.Tools.MakeString(value, ""); }
		}

//! not used!?
		public bool DateTime_ShowPopUpButton {
			get { return Cn.Data.Tools.MakeBoolean(gh_sData["DateTime_ShowPopUpButton"], true); }
			set { gh_sData["DateTime_ShowPopUpButton"] = Cn.Data.Tools.MakeBoolean(value, true); }
		}

		public bool DateTime_ShowOnClick {
			get { return Cn.Data.Tools.MakeBoolean(gh_sData["DateTime_ShowOnClick"], true); }
			set { gh_sData["DateTime_ShowOnClick"] = Cn.Data.Tools.MakeBoolean(value, true); }
		}

//! not used!?
		public string DateTime_PopupButtonCode {
			get { return Cn.Data.Tools.MakeString(gh_sData["DateTime_PopupButtonCode"], ""); }
			set { gh_sData["DateTime_PopupButtonCode"] = Cn.Data.Tools.MakeString(value, ""); }
		}

//! not used!?
		public bool DateTime_UserMustUseControl {
			get { return Cn.Data.Tools.MakeBoolean(gh_sData["DateTime_UserMustUseControl"], false); }
			set { gh_sData["DateTime_UserMustUseControl"] = Cn.Data.Tools.MakeBoolean(value, false); }
		}

		public bool DateTime_ValidateDataType {
			get { return Cn.Data.Tools.MakeBoolean(gh_sData["DateTime_ValidateDataType"], true); }
			set { gh_sData["DateTime_ValidateDataType"] = Cn.Data.Tools.MakeBoolean(value, true); }
		}

//! not used!?
		public int DateTime_HourIncrement {
			get { return Cn.Data.Tools.MakeInteger(gh_sData["DateTime_HourIncrement"], 1); }
			set { gh_sData["DateTime_HourIncrement"] = Cn.Data.Tools.MakeInteger(value, 1); }
		}

//! not used!?
		public int DateTime_MinuteIncrement {
			get { return Cn.Data.Tools.MakeInteger(gh_sData["DateTime_MinuteIncrement"], 1); }
			set { gh_sData["DateTime_MinuteIncrement"] = Cn.Data.Tools.MakeInteger(value, 1); }
		}

//! not used!?
		public int DateTime_SecondIncrement {
			get { return Cn.Data.Tools.MakeInteger(gh_sData["DateTime_SecondIncrement"], 1); }
			set { gh_sData["DateTime_SecondIncrement"] = Cn.Data.Tools.MakeInteger(value, 1); }
		}


		///############################################################
		/// <summary>
		/// Gets/sets the HTML attribute code that will be included within the related HIDDEN element.
		/// </summary>
		/// <remarks>
		/// This value is revelent for <c>cnReadOnlyType</c> inputs.
		/// <para/>The default value for this key is '' (a null-string).
		/// </remarks>
		/// <value>String representing the HTML attribute code that will be included within the related HIDDEN element.</value>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public string ReadOnly_InputAttributes {
			get { return Cn.Data.Tools.MakeString(gh_sData["ReadOnly_InputAttributes"], ""); }
			set { gh_sData["ReadOnly_InputAttributes"] = Cn.Data.Tools.MakeString(value, ""); }
		}
//! can this and the next property be merged?


		///############################################################
		/// <summary>
		/// Gets/sets the HTML attribute code that will be included within the related SELECT element.
		/// </summary>
		/// <remarks>
		/// This value is revelent for <c>cnComboBoxType</c> inputs.
		/// <para/>The default value for this key is '' (a null-string).
		/// </remarks>
		/// <value>String representing the HTML attribute code that will be included within the related SELECT element.</value>
		///############################################################
		/// <LastUpdated>November 26, 2009</LastUpdated>
		public string ComboBox_SelectAttributes {
			get { return Cn.Data.Tools.MakeString(gh_sData["ComboBox_SelectAttributes"], ""); }
			set { gh_sData["ComboBox_SelectAttributes"] = Cn.Data.Tools.MakeString(value, ""); }
		}

	} //# public class AdditionalData

} //# namespace Cn.Web
