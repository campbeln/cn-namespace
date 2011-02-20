/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections.Generic;
using System.Data;
using Cn.Collections;
using Cn.Web.Inputs;


namespace Cn.Web.Controls {

	///########################################################################################################################
	/// <summary>
	/// Collection of static functions to map data between objects.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	public class Mirror {
            //#### Declare the required public eNums
        #region eNums
		private enum enumDirection {
			PopulateBusinessObject = 0,
			PopulateContols = 1
		}
		#endregion

            //#### Declare the required private constants
	  //private const string g_cClassName = "Cn.Web.Controls.Mirror.";


        //##########################################################################################
        //# Map-related Functions
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Maps data between the provided objects.
		/// </summary>
		/// <param name="oFrom">Object to be used as the data source.</param>
		/// <param name="oTo">Object to be used as the data destination.</param>
		/// <param name="iRowIndex">Integer representing the row index to use within the data source.</param>
		/// <param name="bStrict">Boolean value indicating if we are to ensure the table names match (case insensitive).</param>
		/// <returns>Boolean value representing if all of the readable properties within the provided <paramref name="oFrom"/> were successfully mapped into <paramref name="oTo"/>.</returns>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static bool Map(DataTable oFrom, IList<Controls.Input> oTo, int iRowIndex, bool bStrict) {
				//#### Pass the call into .DoMap, signaling that it is to .Populate(the)Controls
			return DoMap(oFrom, oTo, iRowIndex, bStrict, enumDirection.PopulateContols);
		}

		///############################################################
		/// <summary>
		/// Maps data between the provided objects.
		/// </summary>
		/// <param name="oFrom">Object to be used as the data source.</param>
		/// <param name="oTo">Object to be used as the data destination.</param>
		/// <param name="iRowIndex">Integer representing the row index to use within the data source.</param>
		/// <param name="bStrict">Boolean value indicating if we are to ensure the table names match (case insensitive).</param>
		/// <returns>Boolean value representing if all of the readable properties within the provided <paramref name="oFrom"/> were successfully mapped into <paramref name="oTo"/>.</returns>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static bool Map(IList<Controls.Input> oFrom, DataTable oTo, int iRowIndex, bool bStrict) {
				//#### Pass the call into .DoMap, signaling that it is to .Populate(the)Controls
			return DoMap(oTo, oFrom, iRowIndex, bStrict, enumDirection.PopulateBusinessObject);
		}

			///############################################################
			/// <summary>
			/// Maps data between the provided objects.
			/// </summary>
			/// <param name="oBusinessObject">Object to be used as a data source/destination.</param>
			/// <param name="l_oInputCollection">IInputCollection object to be used as a data source/destination.</param>
			/// <param name="iRowIndex">Integer representing the row index to use within the data source.</param>
			/// <param name="bStrict">Boolean value indicating if we are to ensure the table names match (case insensitive).</param>
			/// <returns>Boolean value representing if all of the readable properties within the provided oFrom object were successfully mapped into oTo object.</returns>
			///############################################################
			/// <LastUpdated>February 15, 2010</LastUpdated>
			private static bool DoMap(DataTable oBusinessObject, IList<Controls.Input> l_oInputCollection, int iRowIndex, bool bStrict, enumDirection eDirection) {
				Controls.Input oCurrentInput;
				InputData oInputData;
				string sTableName;
				int i;
				bool bReturn = false;

					//#### If we have valid objects to traverse (as well as a valid iRowIndex within the oBusinessObject)
				if (oBusinessObject != null && oBusinessObject.Rows.Count > iRowIndex && iRowIndex >= 0 &&
					l_oInputCollection != null && l_oInputCollection.Count > 0
				) {
						//#### Collect the .ToLower'd sTableName and re-default our bReturn value to true
					sTableName = oBusinessObject.TableName.ToLower();
					bReturn = true;

						//#### Traverse the l_oInputCollection
					for (i = 0; i < l_oInputCollection.Count; i++) {
							//#### Collect the oCurrentInput and oInputData for this loop
						oCurrentInput = l_oInputCollection[i];
						oInputData = oCurrentInput.ControlManager;

							//#### If the oCurrentInput was found, it's .InitialValueIs(not an)Expression, the .ColumnName exists within the oDataSet and we're not in bStrict mode or if the .TableNames match, copy the .ColumnName's value across
							//####     NOTE: We do not test for the .TableName below because we allow the developer to bypass the test by using a DataTable. If the .TableName is to be tested, a DataSet should be set into the oDataSource
						if (oCurrentInput != null && ! oCurrentInput.InitialValueIsExpression &&
							oBusinessObject.Columns.Contains(oInputData.ColumnName) &&
							(! bStrict || sTableName == oInputData.TableName.ToLower())
						) {
								//#### Determine the eDirection, copying the .ColumnName's value accordingly
							switch (eDirection) {
								case enumDirection.PopulateContols: {
									oCurrentInput.InitialValue = oBusinessObject.Rows[iRowIndex][oInputData.ColumnName].ToString();
									break;
								}
								case enumDirection.PopulateBusinessObject: {
//! typing issues here?
									oBusinessObject.Rows[iRowIndex][oInputData.ColumnName] = oCurrentInput.Value;
									break;
								}
							}
						}
							//#### Else the oCurrentInput was not found within the oDataSet, so flip our bReturn value to false
						else {
							bReturn = false;
						}
					}
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}

		///############################################################
		/// <summary>
		/// Maps data between the provided objects.
		/// </summary>
		/// <param name="oFrom">Object to be used as the data source.</param>
		/// <param name="oTo">Object to be used as the data destination.</param>
		/// <param name="iRowIndex">Integer representing the row index to use within the data source.</param>
		/// <returns>Boolean value representing if all of the readable properties within the provided <paramref name="oFrom"/> were successfully mapped into <paramref name="oTo"/>.</returns>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static bool Map(MultiArray oFrom, IList<Controls.Input> oTo, int iRowIndex) {
				//#### Pass the call into .DoMap, signaling that it is to .Populate(the)Controls
			return DoMap(oFrom, oTo, iRowIndex, enumDirection.PopulateContols);
		}

		///############################################################
		/// <summary>
		/// Maps data between the provided objects.
		/// </summary>
		/// <param name="oFrom">Object to be used as the data source.</param>
		/// <param name="oTo">Object to be used as the data destination.</param>
		/// <param name="iRowIndex">Integer representing the row index to use within the data source.</param>
		/// <returns>Boolean value representing if all of the readable properties within the provided <paramref name="oFrom"/> were successfully mapped into <paramref name="oTo"/>.</returns>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static bool Map(IList<Controls.Input> oFrom, MultiArray oTo, int iRowIndex) {
				//#### Pass the call into .DoMap, signaling that it is to .Populate(the)Controls
			return DoMap(oTo, oFrom, iRowIndex, enumDirection.PopulateBusinessObject);
		}

			///############################################################
			/// <summary>
			/// Maps data between the provided objects.
			/// </summary>
			/// <param name="oBusinessObject">Object to be used as a data source/destination.</param>
			/// <param name="l_oInputCollection">IInputCollection object to be used as a data source/destination.</param>
			/// <param name="iRowIndex">Integer representing the row index to use within the data source.</param>
			/// <returns>Boolean value representing if all of the readable properties within the provided oFrom object were successfully mapped into oTo object.</returns>
			///############################################################
			/// <LastUpdated>February 15, 2010</LastUpdated>
			private static bool DoMap(MultiArray oBusinessObject, IList<Controls.Input> l_oInputCollection, int iRowIndex, enumDirection eDirection) {
				Controls.Input oCurrentInput;
				InputData oInputData;
				int i;
				bool bReturn = false;

					//#### If we have valid objects to traverse (as well as a valid iRowIndex within the oDataTable)
				if (oBusinessObject != null && oBusinessObject.RowCount > iRowIndex && iRowIndex >= 0 &&
					l_oInputCollection != null && l_oInputCollection.Count > 0
				) {
						//#### Re-default our bReturn value to true
					bReturn = true;

						//#### Traverse the l_oInputCollection
					for (i = 0; i < l_oInputCollection.Count; i++) {
							//#### Collect the oCurrentInput and oInputData for this loop
						oCurrentInput = l_oInputCollection[i];
						oInputData = oCurrentInput.ControlManager;

							//#### If the oCurrentInput was found, it's .InitialValueIs(not an)Expression and the .ColumnName .Exists within the oBusinessObject
							//####     NOTE: We do not test for the .TableName below because we allow the developer to bypass the test by using a DataTable. If the .TableName is to be tested, a DataSet should be set into the oDataSource
						if (oCurrentInput != null && ! oCurrentInput.InitialValueIsExpression &&
							oBusinessObject.Exists(oInputData.ColumnName)
						) {
								//#### Determine the eDirection, copying the .ColumnName's value accordingly
							switch (eDirection) {
								case enumDirection.PopulateContols: {
									oCurrentInput.InitialValue = oBusinessObject.Value(iRowIndex, oInputData.ColumnName);
									break;
								}
								case enumDirection.PopulateBusinessObject: {
									oBusinessObject.Value(iRowIndex, oInputData.ColumnName, oCurrentInput.Value);
									break;
								}
							}
						}
							//#### Else the oCurrentInput was not found within the oBusinessObject, so flip our bReturn value to false
						else {
							bReturn = false;
						}
					}
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}


	} //# public class Mirror

 
} //# namespace Cn.Web.Controls
