/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;										//# Required to access the Exception class
using System.Data;									//# Required to access the DataTable class
using System.Data.Odbc;								//# Required to access the ODBC connection classes
using System.Data.OleDb;							//# Required to access the OLEDb connection classes
using System.Data.SqlClient;						//# Required to access the SQL*Server connection classes
using System.Data.OracleClient;						//# Required to access the Oracle connection classes
using System.Collections;					        //# Required to access the Hashtable class
using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Data {

    ///########################################################################################################################
    /// <summary>
	/// Enviroment specific DataSource-related helper methods (dotNET, PHP, Java, etc).
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	public class DBMS {
	#region DBMS
			//#### Declare the required private variables
		private object g_oConnection;
		private string g_sConnectionString;
		private enumConnectionType g_eConnectionType;
		private bool g_bIsConnected;

			//#### Declare the required enums
		#region eNums
			/// <summary>Database server connection types.</summary>
		public enum enumConnectionType {
				/// <summary>Connection to SQL*Server.</summary>
			cnSQLServer = 1,
				/// <summary>Connection to an ODBC compliant server.</summary>
			cnODBC = 2,
				/// <summary>Connection to an OLEDb compliant server.</summary>
			cnOLEDb = 3,
				/// <summary>Connection to Oracle.</summary>
			cnOracle = 4
		};
		#endregion

            //#### Declare the required private constants
	  //private const string g_cClassName = "Cn.Data.DBMS.";


        //##########################################################################################
        //# Class Functions
        //##########################################################################################
        ///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		/// <param name="eConnectionType">Enumeration representing the target database server's connection type.</param>
		/// <param name="sConnectionString">String representing the DSN connection string to login to the target database server.</param>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		public DBMS(enumConnectionType eConnectionType, string sConnectionString) {
                //#### Call .Reset to init the class vars
			Reset(eConnectionType, sConnectionString);
		}

        ///############################################################
		/// <summary>
		/// Disposes of the class's persistent objects.
		/// </summary>
        ///############################################################
		/// <LastUpdated>May 4, 2007</LastUpdated>
		~DBMS() {
//! is the try/catch required here?
				//#### Surround the .CloseConnection call with a try/catch to ignore any errors on class destruction
			try {
					//#### Close our current connection (if any)
				CloseConnection();
			}
			catch {}
		}

        ///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="eConnectionType">Enumeration representing the target database server's connection type.</param>
		/// <param name="sConnectionString">String representing the DSN connection string to login to the target database server.</param>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		public void Reset(enumConnectionType eConnectionType, string sConnectionString) {
				//#### Close our current connection (if any)
			CloseConnection();

                //#### (Re)Init the local variables
            g_oConnection = null;
            g_sConnectionString = sConnectionString;
			g_eConnectionType = eConnectionType;
			g_bIsConnected = false;
		}


        //##########################################################################################
        //# Public Read-Only Properties
        //##########################################################################################
        ///############################################################
		/// <summary>
		/// Gets a value indicating the target database server's connection type.
		/// </summary>
		/// <value>Enumeration indicating target database server's connection type.</value>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		public enumConnectionType ConnectionType {
			get { return g_eConnectionType; }
		}

        ///############################################################
		/// <summary>
        /// Gets a value indicating if this instance is currently connected to the target database server.
		/// </summary>
		/// <value>Boolean value indicating if this instance is currently connected to the target database server.</value>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		public bool IsConnected {
			get { return g_bIsConnected; }
		}


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
        ///############################################################
		/// <summary>
		/// Closes the active database server connection (if any).
		/// </summary>
        ///############################################################
		/// <LastUpdated>July 13, 2006</LastUpdated>
		public void CloseConnection() {
				//#### If we are currently connected
			if (g_bIsConnected) {
					//#### Determine the g_eConnectionType and process accordingly
				switch (g_eConnectionType) {
						//#### If we supposed to close an .cnSQLServer connection
					case enumConnectionType.cnSQLServer: {
						((SqlConnection)g_oConnection).Close();
						break;
					}

						//#### If we supposed to close an .cnOLEDb connection
					case enumConnectionType.cnOLEDb: {
						((OleDbConnection)g_oConnection).Close();
						break;
					}

						//#### If we supposed to close an .cnODBC connection
					case enumConnectionType.cnODBC: {
						((OdbcConnection)g_oConnection).Close();
						break;
					}

						//#### If we supposed to close an .cnOracle connection
					case enumConnectionType.cnOracle: {
							// NOTE: With some versions of .Net this .Close call errors, so surround it in a try/catch block to ignore any such errors
							//#### SEE: http://support.microsoft.com/default.aspx?scid=kb;en-us;330126&Product=NETFrame
						try {
							((OracleConnection)g_oConnection).Close();							
						}
						catch {}
						break;
					}
				}

					//#### Reset g_oConnection to null and flip g_bIsConnected
				g_oConnection = null;
				g_bIsConnected = false;
			}
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of results based on the provided SQL query.
		/// </summary>
		/// <param name="sSQL">String representing the SQL query to execute.</param>
		/// <returns>Set of results containing the data returned from the provided <paramref>sSQL</paramref> query.</returns>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		public DataSet GetResults(string sSQL) {
				//#### Pass the call off to .DoGetResults, passing in null for the hStoredProcedureParams (as this is not a stored procedure call)
			return DoGetResults(sSQL, null);
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of results based on the referenced stored procedure.
		/// </summary>
		/// <param name="sStoredProcedureName">String representing the names of the stored procedure to execute.</param>
		/// <param name="hStoredProcedureParams">Hashtable representing the stored procedure's parameters.</param>
		/// <returns>Set of results containing the data returned from the provided <paramref>sStoredProcedureName</paramref> query.</returns>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		public DataSet GetResults(string sStoredProcedureName, Hashtable hStoredProcedureParams) {
				//#### Pass the call off to .DoGetResults
			return DoGetResults(sStoredProcedureName, hStoredProcedureParams);
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of results based on the provided SQL query.
		/// </summary>
		/// <remarks>
		/// Returns an object that is accessable like this: 'oResults[iRowNumber]["ColumnName"]'.
		/// </remarks>
		/// <param name="sSQL">String representing the SQL query to execute.</param>
		/// <returns>Array of Hashtables containing Hashtables with the data returned from the provided <paramref>sSQL</paramref> query.</returns>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		public Hashtable[] GetHashArray(string sSQL) {
				//#### Pass the call off to .DoGetHashArray, passing in null for the hStoredProcedureParams (as this is not a stored procedure call)
			return DoGetHashArray(sSQL, null);
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of results based on the referenced stored procedure.
		/// </summary>
		/// <remarks>
		/// Returns an object that is accessable like this: 'oResults[iRowNumber]["ColumnName"]'.
		/// </remarks>
		/// <param name="sStoredProcedureName">String representing the names of the stored procedure to execute.</param>
		/// <param name="hStoredProcedureParams">Hashtable representing the stored procedure's parameters.</param>
		/// <returns>Array of Hashtables containing Hashtables with the data returned from the provided <paramref>sSQL</paramref> query.</returns>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		public Hashtable[] GetHashArray(string sStoredProcedureName, Hashtable hStoredProcedureParams) {
				//#### Pass the call off to .DoGetHashArray
			return DoGetHashArray(sStoredProcedureName, hStoredProcedureParams);
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of multiple results based on the provided SQL query.
		/// </summary>
		/// <remarks>
		/// This function returns the first set of results in the passed <paramref>sSQL</paramref> query as a MultiArray. This is useful for <c>Renderer</c> functions such as <see cref="Cn.Data.Picklists.Exists">Cn.Data.Picklists.Exists</see>.
		/// </remarks>
		/// <param name="sSQL">String representing the SQL query to execute.</param>
		/// <returns>Array of MultiArrays containing the multiple results from the passed <paramref>sSQL</paramref> query.</returns>
        ///############################################################
		/// <LastUpdated>May 29, 2007</LastUpdated>
		public MultiArray[] GetMultiArrays(string sSQL) {
			MultiArray[] a_oReturn = null;
			DataSet oDataSet;
			int iTableCount;
			int i;

				//#### Collect the oDataSet from .DoGetResults, passing in null for the hStoredProcedureParams (as this is not a stored procedure call)
			oDataSet = DoGetResults(sSQL, null);

				//#### If we were able to successfully collect the oDataSet
			if (oDataSet != null && oDataSet.Tables.Count > 0) {
					//#### Determine the iTableCount and dimension the a_oReturn value accordingly
				iTableCount = oDataSet.Tables.Count;
				a_oReturn = new MultiArray[iTableCount];

					//#### Traverse the oDataSet, converting each .Table as we go
				for (i = 0; i < iTableCount; i++) {
					a_oReturn[i] = Tools.ToMultiArray(oDataSet.Tables[i]);
				}
			}

				//#### Return the above determined a_oReturn value to the caller
			return a_oReturn;
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of results based on the provided SQL query.
		/// </summary>
		/// <remarks>
		/// This function returns the first set of results in the passed <paramref>sSQL</paramref> query as a MultiArray. This is useful for <c>Renderer</c> functions such as <see cref="Cn.Data.Picklists.Exists">Cn.Data.Picklists.Exists</see>.
		/// </remarks>
		/// <param name="sSQL">String representing the SQL query to execute.</param>
		/// <returns>MultiArray containing the results from the passed <paramref>sSQL</paramref> query.</returns>
        ///############################################################
		/// <LastUpdated>January 31, 2006</LastUpdated>
		public MultiArray GetMultiArray(string sSQL) {
			DataSet oDataSet;

				//#### Collect the oDataSet from .DoGetResults, passing in null for the hStoredProcedureParams (as this is not a stored procedure call)
			oDataSet = DoGetResults(sSQL, null);

				//#### Pass the above collected oDataSet's first .Table into .ToMultiArray, returning its result as our own
			return Tools.ToMultiArray(oDataSet.Tables[0]);
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of results based on the provided SQL query.
		/// </summary>
		/// <remarks>
		/// This function returns the first set of results in the passed <paramref>sSQL</paramref> query as a MultiArray. This is useful for <c>Renderer</c> functions such as <see cref="Cn.Data.Picklists.Exists">Cn.Data.Picklists.Exists</see>.
		/// </remarks>
		/// <param name="sStoredProcedureName">String representing the names of the stored procedure to execute.</param>
		/// <param name="hStoredProcedureParams">Hashtable representing the stored procedure's parameters.</param>
		/// <returns>MultiArray containing the results from the passed <paramref>sSQL</paramref> query.</returns>
        ///############################################################
		/// <LastUpdated>January 31, 2006</LastUpdated>
		public MultiArray GetMultiArray(string sStoredProcedureName, Hashtable hStoredProcedureParams) {
			DataSet oDataSet;

				//#### Collect the oDataSet from .DoGetResults
			oDataSet = DoGetResults(sStoredProcedureName, hStoredProcedureParams);

				//#### Pass the above collected oDataSet's first .Table into .ToMultiArray, returning its result as our own
			return Tools.ToMultiArray(oDataSet.Tables[0]);
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of results based on the provided SQL query.
		/// </summary>
		/// <remarks>
		/// This function returns the first column of the first set of results in the passed <paramref>sSQL</paramref> query as a string array. This is useful for <c>Renderer</c> functions such as <see cref='Cn.Data.Pagination.Load'>Cn.Data.Pagination.Load</see> and <see cref="Cn.Data.Pagination.SetCollectedIDs">Cn.Data.PaginationTable.SetCollectedIDs</see>.
		/// </remarks>
		/// <param name="sSQL">String representing the SQL query to execute.</param>
		/// <returns>String array where each element represents a value of each row's first column.</returns>
        ///############################################################
		/// <LastUpdated>January 31, 2006</LastUpdated>
		public string[] GetColumnArray(string sSQL) {
			DataSet oDataSet;

				//#### Collect the oDataSet from .DoGetResults, passing in null for the hStoredProcedureParams (as this is not a stored procedure call)
			oDataSet = DoGetResults(sSQL, null);

				//#### Pass the above collected oDataSet's first .Table into .ToColumnArray, returning its result as our own
			return Tools.ToColumnArray(oDataSet.Tables[0], 0);
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of results based on the provided SQL query.
		/// </summary>
		/// <remarks>
		/// This function returns the first column of the first set of results in the passed <paramref>sSQL</paramref> query as a string array. This is useful for <c>Renderer</c> functions such as <see cref='Cn.Data.Pagination.Load'>Cn.Data.Pagination.Load</see> and <see cref="Cn.Data.Pagination.SetCollectedIDs">Cn.Data.PaginationTable.SetCollectedIDs</see>.
		/// </remarks>
		/// <param name="sStoredProcedureName">String representing the names of the stored procedure to execute.</param>
		/// <param name="hStoredProcedureParams">Hashtable representing the stored procedure's parameters.</param>
		/// <returns>String array where each element represents a value of each row's first column.</returns>
        ///############################################################
		/// <LastUpdated>January 31, 2006</LastUpdated>
		public string[] GetColumnArray(string sStoredProcedureName, Hashtable hStoredProcedureParams) {
			DataSet oDataSet;

				//#### Collect the oDataSet from .DoGetResults
			oDataSet = DoGetResults(sStoredProcedureName, hStoredProcedureParams);

				//#### Pass the above collected oDataSet's first .Table into .ToColumnArray, returning its result as our own
			return Tools.ToColumnArray(oDataSet.Tables[0], 0);
		}

        ///############################################################
		/// <summary>
		/// Executes the provided SQL query.
		/// </summary>
		/// <param name="sSQL">String representing the SQL query to execute.</param>
		/// <param name="bUseTransactions">Boolean value indicating if transactions are to be utilized.</param>
		/// <returns>Integer representing the number of rows affected.</returns>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		public int ExecuteSQL(string sSQL, bool bUseTransactions) {
				//#### Pass the call off to .DoExecuteSQL, passing in null for the hStoredProcedureParams (as this is not a stored procedure call)
			return DoExecuteSQL(sSQL, null, bUseTransactions);
		}

        ///############################################################
		/// <summary>
		/// Executes the referenced stored procedure.
		/// </summary>
		/// <param name="sStoredProcedureName">String representing the names of the stored procedure to execute.</param>
		/// <param name="hStoredProcedureParams">Hashtable representing the stored procedure's parameters.</param>
		/// <param name="bUseTransactions">Boolean value indicating if transactions are to be utilized.</param>
		/// <returns>Integer representing the number of rows affected.</returns>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		public int ExecuteSQL(string sStoredProcedureName, Hashtable hStoredProcedureParams, bool bUseTransactions) {
				//#### Pass the call off to .DoExecuteSQL
			return DoExecuteSQL(sStoredProcedureName, hStoredProcedureParams, bUseTransactions);
		}


    	//##########################################################################################
        //# Private Functions
        //##########################################################################################
        ///############################################################
		/// <summary>
		/// Opens the active database server connection.
		/// </summary>
        ///############################################################
		/// <LastUpdated>November 24, 2005</LastUpdated>
		private void OpenConnection() {
				//#### Close our current connection (if any)
			CloseConnection();

				//#### Determine the g_eConnectionType and process accordingly
			switch (g_eConnectionType) {
					//#### If we supposed to establish an .cnSQLServer connection
				case enumConnectionType.cnSQLServer: {
					SqlConnection oSQLServer = new SqlConnection();

						//#### Set the .ConnectionString and .Open the connection
					oSQLServer.ConnectionString = g_sConnectionString;
					oSQLServer.Open();

						//#### Store a pointer to the connection into g_oConnection
					g_oConnection = oSQLServer;
					break;
				}				

					//#### If we supposed to establish an .cnOLEDb connection
				case enumConnectionType.cnOLEDb: {
					OleDbConnection oOLEDb = new OleDbConnection();

						//#### Set the .ConnectionString and .Open the connection
					oOLEDb.ConnectionString = g_sConnectionString;
					oOLEDb.Open();

						//#### Store a pointer to the connection into g_oConnection
					g_oConnection = oOLEDb;
					break;
				}

					//#### If we supposed to establish an .cnODBC connection
				case enumConnectionType.cnODBC: {
					OdbcConnection oODBC = new OdbcConnection();

						//#### Set the .ConnectionString and .Open the connection
					oODBC.ConnectionString = g_sConnectionString;
					oODBC.Open();

						//#### Store a pointer to the connection into g_oConnection
					g_oConnection = oODBC;
					break;
				}

					//#### If we supposed to establish an .cnOracle connection
				case enumConnectionType.cnOracle: {
					OracleConnection oOracle = new OracleConnection();

						//#### Set the .ConnectionString and .Open the connection
					oOracle.ConnectionString = g_sConnectionString;
					oOracle.Open();

						//#### Store a pointer to the connection into g_oConnection
					g_oConnection = oOracle;
					break;
				}
			}

				//#### Flip g_bIsConnected
			g_bIsConnected = true;
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of results based on the provided SQL query/stored procedure.
		/// </summary>
		/// <param name="sSQL">String representing the SQL query to execute.</param>
		/// <param name="hStoredProcedureParams">Hashtable representing the stored procedure's parameters.</param>
		/// <returns>Set of results containing the data returned from the provided <paramref>sSQL</paramref> query/stored procedure.</returns>
        ///############################################################
		/// <LastUpdated>December 8, 2005</LastUpdated>
		private DataSet DoGetResults(string sSQL, Hashtable hStoredProcedureParams) {
			DataSet oReturn = new DataSet();

				//#### If we are not currently connected, .Open(a)Connection now
			if (! g_bIsConnected) {
				OpenConnection();
			}

				//#### Determine the g_eConnectionType and process accordingly
			switch(g_eConnectionType) {
					//#### If we supposed to use an .cnSQLServer connection
				case enumConnectionType.cnSQLServer: {
					SqlDataAdapter oDataAdaptor;
					SqlCommand oCommand = new SqlCommand(sSQL, (SqlConnection)g_oConnection);

						//#### If the caller specified a stored procedure
					if (hStoredProcedureParams != null) {
							//#### Set the .CommandType
						oCommand.CommandType = CommandType.StoredProcedure;

							//#### Traverse the hStoredProcedureParams, .Add'ing in one oParam at a time to the oCommand
						foreach (DictionaryEntry oParam in hStoredProcedureParams) {
                            oCommand.Parameters.AddWithValue(oParam.Key.ToString(), oParam.Value);
						}
					}

						//#### Init the oDataAdaptor, then fill the oReturn value
					oDataAdaptor = new SqlDataAdapter(oCommand);
					//oDataAdaptor.MissingSchemaAction = MissingSchemaAction.AddWithKey;	//# This is required to include all length information in the XML schema.
					oDataAdaptor.Fill(oReturn);
					break;
				}

					//#### If we supposed to use an .cnOLEDb connection
				case enumConnectionType.cnOLEDb: {
					OleDbDataAdapter oDataAdaptor;
					OleDbCommand oCommand = new OleDbCommand(sSQL, (OleDbConnection)g_oConnection);

						//#### If the caller specified a stored procedure
					if (hStoredProcedureParams != null) {
							//#### Set the .CommandType
						oCommand.CommandType = CommandType.StoredProcedure;

							//#### Traverse the hStoredProcedureParams, .Add'ing in one oParam at a time to the oCommand
						foreach (DictionaryEntry oParam in hStoredProcedureParams) {
                            oCommand.Parameters.AddWithValue(oParam.Key.ToString(), oParam.Value);
						}
					}

						//#### Init the oDataAdaptor, then fill the oReturn value
					oDataAdaptor = new OleDbDataAdapter(oCommand);
					//oDataAdaptor.MissingSchemaAction = MissingSchemaAction.AddWithKey;	//# This is required to include all length information in the XML schema.
					oDataAdaptor.Fill(oReturn);
					break;
				}

					//#### If we supposed to use an .cnODBC connection
				case enumConnectionType.cnODBC: {
					OdbcDataAdapter oDataAdaptor;
					OdbcCommand oCommand = new OdbcCommand(sSQL, (OdbcConnection)g_oConnection);

						//#### If the caller specified a stored procedure
					if (hStoredProcedureParams != null) {
							//#### Set the .CommandType
						oCommand.CommandType = CommandType.StoredProcedure;

							//#### Traverse the hStoredProcedureParams, .Add'ing in one oParam at a time to the oCommand
						foreach (DictionaryEntry oParam in hStoredProcedureParams) {
                            oCommand.Parameters.AddWithValue(oParam.Key.ToString(), oParam.Value);
						}
					}

						//#### Init the oDataAdaptor, then fill the oReturn value
					oDataAdaptor = new OdbcDataAdapter(oCommand);
					//oDataAdaptor.MissingSchemaAction = MissingSchemaAction.AddWithKey;	//# This is required to include all length information in the XML schema.
					oDataAdaptor.Fill(oReturn);
					break;
				}

					//#### If we supposed to use an .cnOracle connection
				case enumConnectionType.cnOracle: {
					OracleDataAdapter oDataAdaptor;
					OracleCommand oCommand = new OracleCommand(sSQL, (OracleConnection)g_oConnection);

						//#### If the caller specified a stored procedure
					if (hStoredProcedureParams != null) {
							//#### Set the .CommandType
						oCommand.CommandType = CommandType.StoredProcedure;

							//#### Traverse the hStoredProcedureParams, .Add'ing in one oParam at a time to the oCommand
						foreach (DictionaryEntry oParam in hStoredProcedureParams) {
                            oCommand.Parameters.AddWithValue(oParam.Key.ToString(), oParam.Value);
						}
					}

						//#### Init the oDataAdaptor, then fill the oReturn value
					oDataAdaptor = new OracleDataAdapter(oCommand);
					oDataAdaptor.Fill (oReturn);
					break;
				}
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

        ///############################################################
		/// <summary>
		/// Retrieves a set of results based on the provided SQL query/stored procedure.
		/// </summary>
		/// <remarks>
		/// Returns an object that is accessable like this: 'oResults[iRowNumber]["ColumnName"]'.
		/// </remarks>
		/// <param name="sSQL">String representing the SQL query to execute.</param>
		/// <param name="hStoredProcedureParams">Hashtable representing the stored procedure's parameters.</param>
		/// <returns>Array of Hashtables containing Hashtables with the data returned from the provided <paramref>sSQL</paramref> query.</returns>
        ///############################################################
		/// <LastUpdated>December 7, 2005</LastUpdated>
		private Hashtable[] DoGetHashArray(string sSQL, Hashtable hStoredProcedureParams) {
			Hashtable[] hah_sReturn = null;
			System.Data.DataTable oTable;
			DataSet oDataSet;
			string[] a_sColumnNames;
			int iColumnCount;
			int iRowCount;
			int i;
			int j;

				//#### Pass the call off to .DoGetResults, collecting its returned oDataSet
			oDataSet = DoGetResults(sSQL, hStoredProcedureParams);

				//#### If there is a 0th .Table in the oDataSet
			if (oDataSet != null && oDataSet.Tables.Count > 0) {
					//#### Collect the 0th oTable, then its iRowCount and iColumnCount
				oTable = oDataSet.Tables[0];
				iRowCount = oTable.Rows.Count;
				iColumnCount = oTable.Columns.Count;

					//#### If there are .Rows and .Columns in the oTable
				if (iRowCount > 0 && iColumnCount > 0) {
						//#### Dimension the hah_sReturn value based on the iRowCount and a_sColumnNames based on the iColumnCount
					hah_sReturn = new Hashtable[iRowCount];
					a_sColumnNames = new string[iColumnCount];

						//#### Traverse the .Columns, populating the a_sColumnNames as we go
					for (i = 0; i < iColumnCount; i++) {
						a_sColumnNames[i] = oTable.Columns[i].ColumnName;
					}

						//#### Traverse the oTable's .Rows
					for (i = 0; i < iRowCount; i++) {
							//#### Init the current .Row's Hashtable
						hah_sReturn[i] = new Hashtable();

							//#### Traverse the current .Rows' a_sColumnNames
						for (j = 0; j < iColumnCount; j++) {
								//#### .Add the current .Column into the hah_sReturn value
							hah_sReturn[i].Add(a_sColumnNames[j], oTable.Rows[i][j]);
						}
					}
				}
			}

				//#### Return the above determined hah_sReturn value to the caller
			return hah_sReturn;
		}

        ///############################################################
		/// <summary>
		/// Executes the provided SQL query/stored procedure.
		/// </summary>
		/// <param name="sSQL">String representing the SQL query to execute.</param>
		/// <param name="hStoredProcedureParams">Hashtable representing the stored procedure's parameters.</param>
		/// <param name="bUseTransactions">Boolean value indicating if transactions are to be utilized.</param>
		/// <returns>Integer representing the number of rows affected.</returns>
        ///############################################################
		/// <LastUpdated>July 13, 2006</LastUpdated>
		private int DoExecuteSQL(string sSQL, Hashtable hStoredProcedureParams, bool bUseTransactions) {
			int iReturn = -1;

				//#### If we are not currently connected, .Open(a)Connection now
			if (! g_bIsConnected) {
				OpenConnection();
			}

				//#### Determine the g_eConnectionType and process accordingly
			switch (g_eConnectionType) {
					//#### If we supposed to use an .cnSQLServer connection
				case enumConnectionType.cnSQLServer: {
					SqlTransaction oTransaction = null;
					SqlCommand oCommand;

						//#### Setup the error handling
					try {
							//#### If we are supposed to bUseTransactions
						if (bUseTransactions) {
								//#### Init the oTransaction and oCommand objects
							oTransaction = ((SqlConnection)g_oConnection).BeginTransaction();
							oCommand = new SqlCommand(sSQL, (SqlConnection)g_oConnection, oTransaction);
						}
							//#### Else this is not a transaction
						else {
								//#### Init the oCommand object
							oCommand = new SqlCommand(sSQL, (SqlConnection)g_oConnection);
						}

							//#### If the caller specified a stored procedure
						if (hStoredProcedureParams != null) {
								//#### Set the .CommandType
							oCommand.CommandType = CommandType.StoredProcedure;

								//#### Traverse the hStoredProcedureParams, .Add'ing in one oParam at a time to the oCommand
							foreach (DictionaryEntry oParam in hStoredProcedureParams) {
                                oCommand.Parameters.AddWithValue(oParam.Key.ToString(), oParam.Value);
							}
						}

							//#### .Execute(the)NonQuery, setting our iReturn value to its
						iReturn = oCommand.ExecuteNonQuery();

							//#### If we are supposed to bUseTransactions, .Commit the oTransaction
						if (bUseTransactions) {
							oTransaction.Commit();
						}
					}
					catch (Exception) {
							//#### If we are supposed to bUseTransactions, .Rollback the oTransaction
						if (bUseTransactions) {
							oTransaction.Rollback();
						}

							//#### Throw the error back up to the caller
						throw;
					}
					break;
				}

					//#### If we supposed to use an .cnOLEDb connection
				case enumConnectionType.cnOLEDb: {
					OleDbTransaction oTransaction = null;
					OleDbCommand oCommand;

						//#### Setup the error handling
					try {
							//#### If we are supposed to bUseTransactions
						if (bUseTransactions) {
								//#### Init the oTransaction and oCommand objects
							oTransaction = ((OleDbConnection)g_oConnection).BeginTransaction();
							oCommand = new OleDbCommand(sSQL, (OleDbConnection)g_oConnection, oTransaction);
						}
							//#### Else this is not a transaction
						else {
								//#### Init the oCommand object
							oCommand = new OleDbCommand(sSQL, (OleDbConnection)g_oConnection);
						}

							//#### If the caller specified a stored procedure
						if (hStoredProcedureParams != null) {
								//#### Set the .CommandType
							oCommand.CommandType = CommandType.StoredProcedure;

								//#### Traverse the hStoredProcedureParams, .Add'ing in one oParam at a time to the oCommand
							foreach (DictionaryEntry oParam in hStoredProcedureParams) {
                                oCommand.Parameters.AddWithValue(oParam.Key.ToString(), oParam.Value);
							}
						}

							//#### .Execute(the)NonQuery, setting our iReturn value to its
						iReturn = oCommand.ExecuteNonQuery();

							//#### If we are supposed to bUseTransactions, .Commit the oTransaction
						if (bUseTransactions) {
							oTransaction.Commit();
						}
					}
					catch (Exception) {
							//#### If we are supposed to bUseTransactions, .Rollback the oTransaction
						if (bUseTransactions) {
							oTransaction.Rollback();
						}

							//#### Throw the error back up to the caller
						throw;
					}
					break;
				}

					//#### If we supposed to use an .cnODBC connection
				case enumConnectionType.cnODBC: {
					OdbcTransaction oTransaction = null;
					OdbcCommand oCommand;

						//#### Setup the error handling
					try {
							//#### If we are supposed to bUseTransactions
						if (bUseTransactions) {
								//#### Init the oTransaction and oCommand objects
							oTransaction = ((OdbcConnection)g_oConnection).BeginTransaction();
							oCommand = new OdbcCommand(sSQL, (OdbcConnection)g_oConnection, oTransaction);
						}
							//#### Else this is not a transaction
						else {
								//#### Init the oCommand object
							oCommand = new OdbcCommand(sSQL, (OdbcConnection)g_oConnection);
						}

							//#### If the caller specified a stored procedure
						if (hStoredProcedureParams != null) {
								//#### Set the .CommandType
							oCommand.CommandType = CommandType.StoredProcedure;

								//#### Traverse the hStoredProcedureParams, .Add'ing in one oParam at a time to the oCommand
							foreach (DictionaryEntry oParam in hStoredProcedureParams) {
								oCommand.Parameters.AddWithValue(oParam.Key.ToString(), oParam.Value);
							}
						}

							//#### .Execute(the)NonQuery, setting our iReturn value to its
						iReturn = oCommand.ExecuteNonQuery();

							//#### If we are supposed to bUseTransactions, .Commit the oTransaction
						if (bUseTransactions) {
							oTransaction.Commit();
						}
					}
					catch (Exception) {
							//#### If we are supposed to bUseTransactions, .Rollback the oTransaction
						if (bUseTransactions) {
							oTransaction.Rollback();
						}

							//#### Throw the error back up to the caller
						throw;
					}
					break;
				}

					//#### If we supposed to use an .cnODBC connection
				case enumConnectionType.cnOracle: {
					OracleTransaction oTransaction = null;
					OracleCommand oCommand;

					//#### Setup the error handling
					try {
							//#### If we are supposed to bUseTransactions
						if (bUseTransactions) {
								//#### Init the oTransaction and oCommand objects
							oTransaction = ((OracleConnection)g_oConnection).BeginTransaction();
							oCommand = new OracleCommand(sSQL, (OracleConnection)g_oConnection, oTransaction);
						}
							//#### Else this is not a transaction
						else {
								//#### Init the oCommand object
							oCommand = new OracleCommand(sSQL, (OracleConnection)g_oConnection);
						}

							//#### If the caller specified a stored procedure
						if (hStoredProcedureParams != null) {
								//#### Set the .CommandType
							oCommand.CommandType = CommandType.StoredProcedure;

								//#### Traverse the hStoredProcedureParams, .Add'ing in one oParam at a time to the oCommand
							foreach (DictionaryEntry oParam in hStoredProcedureParams) {
                                oCommand.Parameters.AddWithValue(oParam.Key.ToString(), oParam.Value);
							}
						}

							//#### .Execute(the)NonQuery, setting our iReturn value to its
						iReturn = oCommand.ExecuteNonQuery();

							//#### If we are supposed to bUseTransactions, .Commit the oTransaction
						if (bUseTransactions) {
							oTransaction.Commit();
						}
					}
					catch (Exception) {
							//#### If we are supposed to bUseTransactions, .Rollback the oTransaction
						if (bUseTransactions) {
							oTransaction.Rollback();
						}

							//#### Throw the error back up to the caller
						throw;
					}
					break;
				}
			}

				//#### Return the above determined iReturn value to the caller
			return iReturn;
		}

	#endregion

		///########################################################################################################################
		/// <summary>
		/// Utility to collect values from either the provided data source or the user input based on the current context.
		/// </summary>
		/// <remarks>
		/// This class is provided as a utility to collect the current value of a column for a Renderer.Form. If it is the initial load of the Renderer.Form, the value is sourced from the provided DataTable object. If it is a subsequent page load (i.e. the user submitted an incomplete form), the value is sourced from the Request.Form object.
		/// </remarks>
		///########################################################################################################################
		/// <LastFullCodeReview>February 20, 2006</LastFullCodeReview>
		#region GetValue
		public class GetValue {
				//#### Declare the required private variables
			private Web.Renderer.Form g_oForm;
			private DataSet g_oData;

				//#### Declare the required private constants
			private const string g_cClassName = "Cn.Data.DBMS.GetValue.";


			//##########################################################################################
			//# Class Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			/// <param name="oForm">Form object reference to the related Renderer.Form object.</param>
			/// <param name="oResults">Object reference representing the related "set of results" instance.</param>
			/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oForm</paramref> is null.</exception>
			///############################################################
			/// <LastUpdated>February 20, 2006</LastUpdated>
			public GetValue(Web.Renderer.Form oForm, DataSet oResults) {
					//#### Call .DoReset to init the class vars
				DoReset("[Constructor]", oForm, oResults);
			}

			///############################################################
			/// <summary>
			/// Initializes the class.
			/// </summary>
			/// <param name="oForm">Form object reference to the related Renderer.Form object.</param>
			/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oForm</paramref> is null.</exception>
			///############################################################
			/// <LastUpdated>March 2, 2006</LastUpdated>
			public GetValue(Web.Renderer.Form oForm) {
					//#### Call .DoReset to init the class vars
				DoReset("[Constructor]", oForm, null);
			}

			///############################################################
			/// <summary>
			/// Resets the class to its initilized state.
			/// </summary>
			/// <param name="oForm">Form object reference to the related Renderer.Form object.</param>
			/// <param name="oResults">Object reference representing the related "set of results" instance.</param>
			/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oForm</paramref> is null.</exception>
			///############################################################
			/// <LastUpdated>May 21, 2007</LastUpdated>
			public void Reset(Web.Renderer.Form oForm, DataSet oResults) {
					//#### Call .DoReset to init the class vars
				DoReset("Reset", oForm, oResults);
			}

			///############################################################
			/// <summary>
			/// Resets the class to its initilized state.
			/// </summary>
			/// <param name="oForm">Form object reference to the related Renderer.Form object.</param>
			/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oForm</paramref> is null.</exception>
			///############################################################
			/// <LastUpdated>May 21, 2007</LastUpdated>
			public void Reset(Web.Renderer.Form oForm) {
					//#### Call .DoReset to init the class vars
				DoReset("Reset", oForm, null);
			}


			//##########################################################################################
			//# Public Read-Write Properties
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Gets/sets the "set of results" represented by this instance.
			/// </summary>
			/// <value>DataSet representing the "set of results" represented by this instance.</value>
			///############################################################
			/// <LastUpdated>March 2, 2006</LastUpdated>
			public DataSet Data {
				get {
					return g_oData;
				}
				set {
					g_oData = value;
				}
			}


			//##########################################################################################
			//# Public Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Retrieves the requested data value for the referenced column name.
			/// </summary>
			/// <param name="sColumnName">String representing the column name to locate.</param>
	        /// <returns>String representing the requested data value.</returns>
			///############################################################
			/// <LastUpdated>May 29, 2007</LastUpdated>
			public string Value(string sColumnName) {
				string sReturn;
				int iCurrentRecordIndex = g_oForm.TableRecordIndex;
				int iCurrentTableIndex = g_oForm.TableIndex;

					//#### If we can safely index into the g_oResults
				if (! g_oForm.IsPostBack && ! g_oForm.IsNewRecord &&
					iCurrentTableIndex >= 0 && iCurrentRecordIndex >= 0 &&
					g_oData != null &&
					g_oData.Tables.Count > iCurrentTableIndex &&
					g_oData.Tables[iCurrentTableIndex] != null &&
					g_oData.Tables[iCurrentTableIndex].Rows.Count > iCurrentRecordIndex
				) {
						//#### Set the sReturn value to the passed sColumnName at the .TableRecordIndex
					sReturn = Cn.Data.Tools.MakeString(g_oData.Tables[iCurrentTableIndex].Rows[iCurrentRecordIndex][sColumnName], "");
				}
					//#### Else we need to retrieve the passed sColumnName from the submitted .Form
				else {
						//#### Set the sReturn value to the passed sColumnName from the submitted .Form
					sReturn = g_oForm.Settings.Request.Form[g_oForm.InputCollection.InputName(sColumnName, g_oForm.RecordCount)];
				}

					//#### sReturn the above determined sReturn value to the caller (ensuring it's a string as we go)
				return Cn.Data.Tools.MakeString(sReturn, "");
			}


			//##########################################################################################
			//# Private Functions
			//##########################################################################################
			///############################################################
			/// <summary>
			/// Resets the class to its initilized state.
			/// </summary>
			/// <param name="sFunction">String representing the calling function's name.</param>
			/// <param name="oForm">Form object reference to the related Renderer.Form object.</param>
			/// <param name="oResults">Object reference representing the related "set of results" instance.</param>
			/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oForm</paramref> is null.</exception>
			///############################################################
			/// <LastUpdated>May 29, 2007</LastUpdated>
			private void DoReset(string sFunction, Web.Renderer.Form oForm, DataSet oResults) {
					//#### If the provided oForm is valid
				if (oForm != null) {
						//#### (Re)Init the local variables
					g_oForm = oForm;
					g_oData = oResults;
				}
					//#### Else the caller passed in a null oForm, so raise the error
				else {
					Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "oForm", "");
				}
			}

		} //# class GetValue
		#endregion

	} //# class DBMS

} //# namespace Cn.Data
