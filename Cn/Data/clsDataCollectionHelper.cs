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


namespace Cn.Data {

	///########################################################################################################################
	/// <summary>
	/// Assists in the collection of <c>MultiArray</c>-based data that requires a paticular structure.
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview>December 24, 2009</LastFullCodeReview>
	public class CollectionHelper {
		//#### Declare the required private variables
        private string[] ga_sRequiredColumns;
        private string g_sDefaultTableName;
        private string g_sBaseSQL;

            //#### Declare the required private constants
	  //private const string g_cClassName = "Cn.Data.CollectionHelper.";


        //##########################################################################################
        //# Class Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
		/// Initializes the class.
        /// </summary>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public CollectionHelper(string[] a_sRequiredColumns, string sBaseSQL, string sDefaultTableName) {
				//#### Init the global vars
			ga_sRequiredColumns = a_sRequiredColumns;
			g_sBaseSQL = sBaseSQL;
			g_sDefaultTableName = sDefaultTableName;
		}


        //#######################################################################################################
        //# Public Read-Only Properties
        //#######################################################################################################
        ///############################################################
		/// <summary>
		/// Retrieves the column names required to be present within a MultiArray structure that defines the parent type.
		/// </summary>
		/// <returns>String array where each index represents a MultiArray column name required by the parent type.</returns>
        ///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public virtual string[] RequiredColumns {
			get {
				return ga_sRequiredColumns;
			}
		}

		///############################################################
		/// <summary>
		/// Gets the unmodified base SQL.
		/// </summary>
		/// <value>String representing the unmodified base SQL.</value>
		///############################################################
		/// <LastUpdated>January 14, 2010</LastUpdated>
    	internal string BaseSQLStatement {
    		get { return g_sBaseSQL; }
    	}


        //#######################################################################################################
        //# Public Functions
        //#######################################################################################################
        ///############################################################
        /// <summary>
        /// Retrieves a properly formatted SQL query to collect the data from a data source.
        /// </summary>
        /// <returns>String containing a SQL query referencing the passed <paramref>sTableName</paramref> to collect the data for the parent type.</returns>
        ///############################################################
		/// <LastUpdated>January 14, 2010</LastUpdated>
		public virtual string SQLStatement() {
				//#### Pass the call off to our sibling implementation, while passing in the g_sDefaultTableName
			return SQLStatement(g_sDefaultTableName);
		}

        ///############################################################
        /// <summary>
        /// Retrieves a properly formatted SQL query to collect the data from a data source.
        /// </summary>
        /// <param name="sTableName">String representing the table name containing the data.</param>
        /// <returns>String containing a SQL query referencing the passed <paramref>sTableName</paramref> to collect the data for the parent type.</returns>
        ///############################################################
		/// <LastUpdated>January 14, 2010</LastUpdated>
		public virtual string SQLStatement(string sTableName) {
				//#### .Replace the $TableName token with the passed sTableName and return it to the caller
			return g_sBaseSQL.Replace("$TableName", sTableName);
		}

		///############################################################
		/// <summary>
		/// Retrieves the data for the parent type based on the provided information and data source.
		/// </summary>
		/// <param name="oDBMS"><c>DBMS</c> instance representing an active connection to the related data source.</param>
		/// <returns><c>MultiArray</c> instance based on the provided information and data source.</returns>
		///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public virtual MultiArray Data(DBMS oDBMS) {
				//#### Pass the call off to our sibling implementation, while passing in the g_sDefaultTableName
			return Data(oDBMS, g_sDefaultTableName);
		}

		///############################################################
		/// <summary>
		/// Retrieves the data for the parent type based on the provided information and data source.
		/// </summary>
		/// <param name="oDBMS"><c>DBMS</c> instance representing an active connection to the related data source.</param>
        /// <param name="sTableName">String representing the table name containing the data.</param>
		/// <returns><c>MultiArray</c> instance based on the provided information and data source.</returns>
		///############################################################
		/// <LastUpdated>December 24, 2009</LastUpdated>
		public virtual MultiArray Data(DBMS oDBMS, string sTableName) {
				//#### Return a MultiArray of the passed sTableName (utilizing our own .SQL)
			return oDBMS.GetMultiArray(SQLStatement(sTableName));
		}


	} //# public class DataCollectionHelper


} //# namespace Cn.Data
