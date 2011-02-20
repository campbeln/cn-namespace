/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Data.SQL {

	    //#### Declare the required public eNums
	#region eNums
		/// <summary>Column value operators.</summary>
    public enum enumValueOperators : int {
			/// <summary>Designates that this column is not to be used in the statement at all.</summary>
		cnIgnore = -1,
			/// <summary>If a value for the column is present, it is inserted/updated. If no value is present, this column is not to be used in the statement at all.<para/>NOTE: This column is never used within the WHERE clause.</summary>
		cnInsertIfPresent = 0,
			/// <summary>Insert/update the column. If no value is present, insert a null-string.<para/>NOTE: This column is never used within the WHERE clause.</summary>
		cnInsertNullString = 1,
			/// <summary>Insert/update the column. If no value is present, insert null.<para/>NOTE: This column is never used within the WHERE clause.</summary>
		cnInsertNull = 2,

			/// <summary>Equal to the supplied value.</summary>
		cnWhereEqualTo = 10,
			/// <summary>Not equal to the supplied value.</summary>
		cnWhereNotEqualTo = 11,

			/// <summary>Greater then the supplied value.</summary>
		cnWhereGreaterThen = 20,
			/// <summary>Greater then or equal to the supplied value.</summary>
		cnWhereGreaterThenOrEqualTo = 21,

			/// <summary>Less then to the supplied value.</summary>
		cnWhereLessThen = 30,
			/// <summary>Less then or equal to the supplied value.</summary>
		cnWhereLessThenOrEqualTo = 31,

			/// <summary>Column is NULL.</summary>
		cnWhereIsNull = 40,
			/// <summary>Column is not NULL.</summary>
		cnWhereIsNotNull = 41,

			/// <summary>Column is LIKE the supplied value.</summary>
		cnWhereLike = 50,
			/// <summary>Column is not LIKE the supplied value.</summary>
		cnWhereNotLike = 51
    }
    #endregion


    ///########################################################################################################################
    /// <summary>
	/// General DataSource-related helper methods.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>July 29, 2005</LastFullCodeReview>
	public class Statements {
			//#### Declare the required private, developer modifiable constants
			//####     DisallowedUserStrings: Defines the disallowed strings that by definition cannot be present at all within a user provided string.
			//####     DisallowedUserWords: Defines the disallowed words (surrounded by whitepsace) that by definition cannot be present at all within a user provided string.
		private static string[] a_sDisallowedUserStrings = { ";", "--", "xp_" };
		private static string[] a_sDisallowedUserWords = { "select", "insert", "update", "delete", "from", "where", "order by", "and", "or" };

		    //#### Declare the required private constants
		private const string g_cClassName = "Cn.Data.SQL.Statements.";


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Determines if the provided data contains macilicious SQL statements.
        /// </summary>
        /// <param name="sData">String representing the data to check.</param>
		/// <value>Boolean value signaling the if the provided <paramref>sData</paramref> contains macilicious SQL statements.<para/>Returns true if the <paramref>sData</paramref> is safe, or false if it contains macilicious SQL statements.</value>
        ///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
	    public static bool IsUserDataSafe(string sData) {
		    int i;
		    bool bReturn = true;

                //#### Traverse the a_sDisallowedUserStrings
		    for (i = 0; i < a_sDisallowedUserStrings.Length; i++) {
                    //#### If the current LCase'd a_sDisallowedUserStrings is found within sData, set the return value to false and exit the loop
			    if (sData.IndexOf(a_sDisallowedUserStrings[i].ToLower(), 0) > -1) {
				    bReturn = false;
				    break;
			    }
		    }

                //#### If no a_sDisallowedUserStrings were located above
		    if (bReturn) {
                    //#### Normalize() sData, surrounding it with leading and trailing spaces for the comparisons below
			    sData = " " + Data.Tools.Normalize(sData) + " ";

                    //#### Traverse the a_sDisallowedUserWords
			    for (i = 0; i < a_sDisallowedUserWords.Length; i++) {
                        //#### If the current LCase'd a_sDisallowedUserWords (surrounded by spaces) is found within sData, set the return value to false and exit the loop
				    if (sData.IndexOf(" " + a_sDisallowedUserWords[i].ToLower() + " ", 0) > -1) {
					    bReturn = false;
					    break;
				    }
			    }
		    }

			    //#### Return the above determined bReturn value to the caller
		    return bReturn;
	    }

        ///############################################################
		/// <summary>
		/// Formats the provided value for insertion within an SQL string, including escaping SQL control strings and optionally escaping SQL wildcard characters.
		/// </summary>
		/// <param name="sValue">String representing the value to format.</param>
        /// <param name="bPadSQLWildcards">Boolean value signaling if SQL wildcard characters also need to be escaped.</param>
        /// <returns>String representing the formatted data.</returns>
        ///############################################################
		/// <LastUpdated>January 12, 2006</LastUpdated>
	    public static string FormatForSQL(string sValue, bool bPadSQLWildcards) {
                //#### Default the sReturn value to the passed sValue
		    string sReturn = sValue;

                //#### If we are to bPad(the)SQLWildcards
		    if (bPadSQLWildcards) {
                    //#### bPad(any)SQLWildcard characters in sValue into their escaped forms
			    sReturn = sReturn.Replace("^", "^^");
			    sReturn = sReturn.Replace("_", "^_");
			    sReturn = sReturn.Replace("%", "^%");
		    }

                //#### Pad the standard SQL-ish characters
		    sReturn = sReturn.Replace("'", "''");
		    sReturn = sReturn.Replace("--", "- -");
		    sReturn = sReturn.Replace("xp_", "xp _");

			    //#### Return the above determined sReturn value to the caller
		    return sReturn;
	    }

/*	    '###############################################################
	    '# Removes all _DisallowedUserStrings and _DisallowedUserWords (as defined in RendererCore.custom.asp) with "", returning the resulting string
	    '###############################################################
	    '# Last Updated: June 23, 2004
	    Public Function CleanSQL(ByVal sValue)
	        Dim i

	            '#### Default the return value to the normalized sValue
	        CleanSQL = Normalize(sValue)

	            '#### Traverse aRD_DisallowedUserStrings, replacing each _DisallowedUserString as we go
	        For i = 0 To UBound(aRD_DisallowedUserStrings)
	            CleanSQL = Replace(CleanSQL, aRD_DisallowedUserStrings(i), "", 1, -1, vbTextCompare)
	        Next

	            '#### Traverse aRD_DisallowedUserWords, replacing each _DisallowedUserWord as we go
	        For i = 0 To UBound(aRD_DisallowedUserWords)
	            CleanSQL = Replace(CleanSQL, " " & aRD_DisallowedUserWords(i) & " ", " ", 1, -1, vbTextCompare)
	        Next
	    End Function
*/

        //##########################################################################################
        //# Public Build*Statement-related Functions
        //##########################################################################################
        ///############################################################
	    /// <summary>
	    /// Creates a SQL INSERT statement based on the provided data.
	    /// </summary>
		/// <param name="sTableName">String representing the table name to target.</param>
	    /// <param name="a_oColumns">ColumnDescription array where each index represents a single column to be represented within the statement.</param>
	    /// <returns>String value containing the requested SQL INSERT statement.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sTableName</paramref> contains macilicious SQL statements.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> is null or contains no columns.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> contains one or more <c>ColumnName</c>'s containing macilicious SQL statements.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> contains no columns defined as insertable/updateable.</exception>
        ///############################################################
		/// <LastUpdated>March 1, 2006</LastUpdated>
		public static string Insert(string sTableName, ColumnDescription[] a_oColumns) {
			string sInsertValues = "";
			string sReturn = "";
			int i;
			bool bColumnsPresent = false;

				//#### If the caller passed in a_oColumns to traverse
			if (a_oColumns != null && a_oColumns.Length > 0) {
					//#### If the passed sTableName is safe
				if (IsUserDataSafe(sTableName)) {
						//#### Traverse the passed a_oColumns
					for (i = 0; i < a_oColumns.Length; i++) {
							//#### If the current .ColumnName is safe
						if (IsUserDataSafe(a_oColumns[i].ColumnName)) {
								//#### As long as this is an .IsInsertOrUpdateColumn
							if (a_oColumns[i].IsInsertOrUpdateColumn) {
									//#### Flip the value of bColumnsPresent to true
								bColumnsPresent = true;

									//#### If the .Value is a null-string
								if (a_oColumns[i].Value.Length == 0) {
										//#### Determine the current a_oColumns' .Operator and process accordingly
									switch (a_oColumns[i].Operator) {
										case enumValueOperators.cnInsertNull: {
												//#### Append the .ColumnName and the NULL .Value onto the sReturn value
											sReturn += "," + a_oColumns[i].ColumnName;
											sInsertValues += ",NULL";
											break;
										}
										case enumValueOperators.cnInsertNullString: {
												//#### Append the .ColumnName and the null-string .Value onto the sReturn value
											sReturn += "," + a_oColumns[i].ColumnName;
											sInsertValues += ",''";
											break;
										}
									}
								}
									//#### Else there is a .Value to insert
								else {
										//#### If we are supposed to .Quote(the)Value (i.e. - this is a string-based type)
									if (a_oColumns[i].Quote) {
											//#### Append the .ColumnName onto the sReturn value and the FormatForSQL'd .Value onto sInsertValues
										sReturn += "," + a_oColumns[i].ColumnName;
										sInsertValues += ",'" + FormatForSQL(a_oColumns[i].Value, false) + "'";
									}
										//#### Else this must be a numeric (or otherwise non-quotable) value
									else {
											//#### Append the .ColumnName onto the sReturn value and the FormatForSQL'd .Value onto sInsertValues
											//####     NOTE: We do a FormatForSQL below in order to avoid security issues. If this is indeed a numeric (or otherwise non-quotable) value, then .Value will not be changed by FormatForSQL. If it is a malicious attack, it will fail (as will the statement probably).
										sReturn += "," + a_oColumns[i].ColumnName;
										sInsertValues += "," + FormatForSQL(a_oColumns[i].Value, false);
									}
								}
							}
						}
							//#### Else the current .ColumnName seems to contain macilious code, so raise the error
						else {
							Internationalization.RaiseDefaultError(g_cClassName + "Insert", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "", "");
						}
					}

						//#### If insertable/updateable bColumns(were)Present
					if (bColumnsPresent) {
    						//#### Coalesce the above created sInsertValues back into the return value (while removing their leading ","s)
						sReturn = "INSERT INTO " + sTableName + " (" + sReturn.Substring(1) + ") VALUES (" + sInsertValues.Substring(1) + ")";
					}
						//#### Else no insertable/updateable bColumns(were)Present, so raise the error
					else {
						Internationalization.RaiseDefaultError(g_cClassName + "Insert", Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbTools_InsertUpdateColumnsRequired, "a_oColumns", "");
					}
				}
					//#### Else the passed sTableName seems to contain macilious code, so raise the error
				else {
					Internationalization.RaiseDefaultError(g_cClassName + "Insert", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "", "");
				}
			}
				//#### Else the passed a_oColumns is empty, so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + "Insert", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "a_oColumns", "");
			}

			    //#### Return the above determiend sReturn value to the caller
			return sReturn;
		}

        ///############################################################
	    /// <summary>
	    /// Creates a SQL UPDATE statement based on the provided data.
	    /// </summary>
		/// <param name="sTableName">String representing the table name to target.</param>
	    /// <param name="a_oColumns">ColumnDescription array where each index represents a single column to be represented within the statement.</param>
	    /// <returns>String value containing the requested SQL UPDATE statement.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sTableName</paramref> contains macilicious SQL statements.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> is null or contains no columns.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> contains one or more <c>ColumnName</c>'s containing macilicious SQL statements.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> contains no columns defining the WHERE clause (columns defined as non-insertable/updateable).</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> contains no columns defined as insertable/updateable.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> contains one or more unreconized <c>Operator</c>s.</exception>
        ///############################################################
		/// <LastUpdated>July 10, 2006</LastUpdated>
		public static string Update(string sTableName, ColumnDescription[] a_oColumns) {
            string sWhereClause = "";
			string sOperator;
		    string sReturn = "";
			int i;

				//#### If the caller passed in a_oColumns to traverse
			if (a_oColumns != null && a_oColumns.Length > 0) {
					//#### If the passed sTableName is safe
				if (IsUserDataSafe(sTableName)) {
						//#### Traverse the passed a_oColumns
					for (i = 0; i < a_oColumns.Length; i++) {
							//#### If the current .ColumnName is safe
						if (IsUserDataSafe(a_oColumns[i].ColumnName)) {
								//#### If this is an .IsInsertOrUpdateColumn
							if (a_oColumns[i].IsInsertOrUpdateColumn) {
									//#### If the .Value is a null-string
								if (a_oColumns[i].Value.Length == 0) {
										//#### Determine the current a_oColumns' .Operator and process accordingly
									switch (a_oColumns[i].Operator) {
										case enumValueOperators.cnInsertNull: {
												//#### Append the .ColumnName and the NULL .Value onto the sReturn value
											sReturn += "," + a_oColumns[i].ColumnName + "=NULL";
											break;
										}
										case enumValueOperators.cnInsertNullString: {
												//#### Append the .ColumnName and the null-string .Value onto the sReturn value
											sReturn += "," + a_oColumns[i].ColumnName + "=''";
											break;
										}
									}
								}
									//#### Else there is a .Value to insert
								else {
										//#### If we are supposed to .Quote(the)Value (i.e. - this is a string-based type)
									if (a_oColumns[i].Quote) {
											//#### Append the .ColumnName and the FormatForSQL'd .Value onto the sReturn value
										sReturn += "," + a_oColumns[i].ColumnName + "='" + FormatForSQL(a_oColumns[i].Value, false) + "'";
									}
										//#### Else this must be a numeric (or otherwise non-quotable) value
									else {
											//#### Append the .ColumnName and the .Value onto the sReturn value
											//####     NOTE: We do a FormatForSQL below in order to avoid security issues. If this is indeed a numeric (or otherwise non-quotable) value, then .Value will not be changed by FormatForSQL. If it is a malicious attack, it will fail (as will the statement probably).
										sReturn += "," + a_oColumns[i].ColumnName + "=" + FormatForSQL(a_oColumns[i].Value, false);
									}
								}
							}
								//#### Else if this is a WHERE clause column of some sort
							else if (a_oColumns[i].IsWhereClauseColumn) {
									//#### Determine the sOperator for the current a_oColumn
								sOperator = TranslateOperator("Update", a_oColumns[i].Operator);

									//#### If we are supposed to .Quote(the)Value (i.e. - this is a string-based type)
								if (a_oColumns[i].Quote) {
										//#### Append the .ColumnName and the PadSQL'd .Value onto the sWhereClause
									sWhereClause += " AND " + a_oColumns[i].ColumnName + sOperator + "'" + FormatForSQL(a_oColumns[i].Value, false) + "'";
								}
									//#### Else this must be a numeric (or otherwise non-quotable) value
								else {
										//#### If the .Value .Is(a)Numeric
										//####     NOTE: Since .IsNumeric traverses the sValue one character at a time (and therefore does not convert it into a numeric type), there is no issue with overflow errors
									if (Data.Tools.IsNumeric(a_oColumns[i].Value)) {
											//#### Append the .ColumnName and the .Value onto the sWhereClause
											//####     NOTE: We do a PadSQL below in order to avoid security issues. If this is indeed a numeric value, then .Value will not be changed by PadSQL. If it is a malicious attack, it will fail (as will the statement probably).
										sWhereClause += " AND " + a_oColumns[i].ColumnName + sOperator + FormatForSQL(a_oColumns[i].Value, false);
									}
								}
							}
						}
							//#### Else the current .ColumnName seems to contain macilious code, so raise the error
						else {
							Internationalization.RaiseDefaultError(g_cClassName + "Update", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "", "");
						}
					}

						//#### If the sWhereClause was never set above, raise the error
					if (sWhereClause.Length == 0) {
						Internationalization.RaiseDefaultError(g_cClassName + "Update", Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbTools_WhereClauseRequired, "a_oColumns", "");
					}
						//#### Else if insertable/updatable columns were present
					else if (sReturn.Length > 0) {
							//#### Coalesce the above created sReturn value and the determined sWhereClause (while removing its leading " AND ", hence begining at 5)
						sReturn = "UPDATE " + sTableName + " SET " + sReturn.Substring(1) + " WHERE " + sWhereClause.Substring(5);
					}
						//#### Else no insertable/updateable bColumns(were)Present, so raise the error
					else {
						Internationalization.RaiseDefaultError(g_cClassName + "Update", Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbTools_InsertUpdateColumnsRequired, "a_oColumns", "");
					}
				}
					//#### Else the passed sTableName seems to contain macilious code, so raise the error
				else {
					Internationalization.RaiseDefaultError(g_cClassName + "Update", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "", "");
				}
			}
				//#### Else the passed a_oColumns is empty, so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + "Update", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "a_oColumns", "");
			}

			    //#### Return the above determiend sReturn value to the caller
			return sReturn;
		}

        ///############################################################
	    /// <summary>
	    /// Creates a SQL DELETE statement based on the provided data.
	    /// </summary>
		/// <param name="sTableName">String representing the table name to target.</param>
	    /// <param name="a_oColumns">ColumnDescription array where each index represents a single column to be represented within the statement.</param>
	    /// <returns>String value containing the requested SQL DELETE statement.</returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>sTableName</paramref> contains macilicious SQL statements.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> is null or contains no columns.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> contains one or more <c>ColumnName</c>s containing macilicious SQL statements.</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> contains no columns defining the WHERE clause (columns defined as non-insertable/updateable).</exception>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>a_oColumns</paramref> contains one or more unreconized <c>Operator</c>s.</exception>
        ///############################################################
		/// <LastUpdated>February 15, 2006</LastUpdated>
		public static string Delete(string sTableName, ColumnDescription[] a_oColumns) {
			string sOperator;
			string sReturn = "";
			int i;

				//#### If the caller passed in a_oColumns to traverse
			if (a_oColumns != null && a_oColumns.Length > 0) {
					//#### If the passed sTableName is safe
				if (IsUserDataSafe(sTableName)) {
						//#### Traverse the passed a_oColumns
					for (i = 0; i < a_oColumns.Length; i++) {
							//#### If the current .ColumnName is safe
						if (IsUserDataSafe(a_oColumns[i].ColumnName)) {
								//#### If this is a .IsWhereClauseColumn
							if (a_oColumns[i].IsWhereClauseColumn) {
									//#### Determine the sOperator for the current a_oColumn
								sOperator = TranslateOperator("Delete", a_oColumns[i].Operator);

									//#### If we are supposed to .Quote(the)Value (i.e. - this is a string-based type)
								if (a_oColumns[i].Quote) {
										//#### Append the .ColumnName and the PadSQL'd .Value onto the sReturn value
									sReturn += " AND " + a_oColumns[i].ColumnName + sOperator + "'" + FormatForSQL(a_oColumns[i].Value, false) + "'";
								}
									//#### Else this must be a numeric (or otherwise non-quotable) value
								else {
										//#### If the .Value .Is(a)Numeric
										//####     NOTE: Since .IsNumeric traverses the sValue one character at a time (and therefore does not convert it into a numeric type), there is no issue with overflow errors
									if (Data.Tools.IsNumeric(a_oColumns[i].Value)) {
											//#### Append the .ColumnName and the .Value onto the sReturn value
											//####     NOTE: We do a PadSQL below in order to avoid security issues. If this is indeed a numeric value, then .Value will not be changed by PadSQL. If it is a malicious attack, it will fail (as will the statement probably).
										sReturn += " AND " + a_oColumns[i].ColumnName + sOperator + FormatForSQL(a_oColumns[i].Value, false);
									}
								}
							}
						}
							//#### Else the current .ColumnName seems to contain macilious code, so raise the error
						else {
							Internationalization.RaiseDefaultError(g_cClassName + "Delete", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "", "");
						}
					}

						//#### If the sReturn was never set above, raise the error
					if (sReturn.Length == 0) {
						Internationalization.RaiseDefaultError(g_cClassName + "Delete", Internationalization.enumInternationalizationValues.cnDeveloperMessages_DbTools_WhereClauseRequired, "a_oColumns", "");
					}
						//#### Else we have all the data we need to build the statement
					else {
							//#### Coalesce the above created sReturn (while removing its leading " AND ", hence 5), storing the return value back into the sReturn value
						sReturn = "DELETE FROM " + sTableName + " WHERE " + sReturn.Substring(5);
					}
				}
					//#### Else the passed sTableName seems to contain macilious code, so raise the error
				else {
					Internationalization.RaiseDefaultError(g_cClassName + "Delete", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_MaliciousSQLFound, "", "");
				}
			}
				//#### Else the passed a_oColumns is empty, so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + "Delete", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_ValueRequired, "a_oColumns", "");
			}

			    //#### Return the above determiend sReturn value to the caller
			return sReturn;
		}


        //#######################################################################################################
        //# Private Functions
        //#######################################################################################################
        ///############################################################
	    /// <summary>
	    /// Translates the provided comparison operator into the actual operator.
	    /// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
	    /// <param name="eValueOperator">Enumeration representing the comparison operator.</param>
	    /// <returns></returns>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>eValueOperator</paramref> is unreconized or defines a insertable/updateable column.</exception>
        ///############################################################
		/// <LastUpdated>September 13, 2005</LastUpdated>
	    private static string TranslateOperator(string sFunction, enumValueOperators eValueOperator) {
			string sReturn = "";

				//#### Determine the passed eValueOperator then set the sReturn value accordingly
			switch (eValueOperator) {
				case enumValueOperators.cnWhereEqualTo: {
					sReturn = "=";
					break;
				}
				case enumValueOperators.cnWhereGreaterThen: {
					sReturn = ">";
					break;
				}
				case enumValueOperators.cnWhereGreaterThenOrEqualTo: {
					sReturn = ">=";
					break;
				}
				case enumValueOperators.cnWhereIsNotNull: {
					sReturn = "IS NOT NULL";
					break;
				}
				case enumValueOperators.cnWhereIsNull: {
					sReturn = "IS NULL";
					break;
				}
				case enumValueOperators.cnWhereLessThen: {
					sReturn = "<";
					break;
				}
				case enumValueOperators.cnWhereLessThenOrEqualTo: {
					sReturn = "<=";
					break;
				}
				case enumValueOperators.cnWhereLike: {
					sReturn = "LIKE";
					break;
				}
				case enumValueOperators.cnWhereNotEqualTo: {
					sReturn = "!=";
					break;
				}
				case enumValueOperators.cnWhereNotLike: {
					sReturn = "NOT LIKE";
					break;
				}

//				case enumValueOperators.cnIgnore:
//				case enumValueOperators.cnInsertIfPresent:
//				case enumValueOperators.cnInsertNull:
//				case enumValueOperators.cnInsertNullString:
				default: {
					Internationalization.RaiseDefaultError(g_cClassName + sFunction, Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eValueOperator", eValueOperator.ToString());
					break;
				}
			}

			    //#### Return the above determiend sReturn value to the caller
			return sReturn;
	    }

	} //# class Statements


    ///########################################################################################################################
    /// <summary>
	/// Represents a column as an object.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>September 12, 2005</LastFullCodeReview>
	public class ColumnDescription {
            //#### Declare the required private variables
		private string g_sColumnName;
		private string g_sValue;
		private enumValueOperators g_eOperator;
		private bool g_bQuote;


        //##########################################################################################
        //# Class Functions
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
		public ColumnDescription() {
                //#### Call .Reset to init the class vars
			Reset();
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
		public void Reset() {
                //#### Call .Reset to init the class vars
			Reset("", "", true, enumValueOperators.cnIgnore);
		}

		///############################################################
        /// <summary>
		/// Resets the class to its initilized state based on the provided data.
        /// </summary>
        /// <param name="oInputData">InputData representing the column's metadata.</param>
		///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
		public ColumnDescription(Web.Inputs.InputData oInputData) {
                //#### Call .Reset to init the class vars
			Reset(oInputData);
		}

		///############################################################
        /// <summary>
		/// Initializes the class based on the provided data.
        /// </summary>
        /// <param name="oInputData">InputData representing the column's metadata.</param>
		///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
		public void Reset(Web.Inputs.InputData oInputData) {
                //#### Set the local variables to the passed data
			g_sColumnName = oInputData.ColumnName;
			g_sValue = oInputData.Value;

				//#### Determine the oInputData's .SaveType, translating it into the proper enumValueOperators
			switch (oInputData.SaveType) {
				case Web.Inputs.enumSaveTypes.cnID: {
					g_eOperator = enumValueOperators.cnWhereEqualTo;
					break;
				}
				case Web.Inputs.enumSaveTypes.cnIgnore: {
					g_eOperator = enumValueOperators.cnIgnore;
					break;
				}
				case Web.Inputs.enumSaveTypes.cnInsertNull: {
					g_eOperator = enumValueOperators.cnInsertNull;
					break;
				}
				case Web.Inputs.enumSaveTypes.cnInsertNullString: {
					g_eOperator = enumValueOperators.cnInsertNullString;
					break;
				}
				default: {
					g_eOperator = enumValueOperators.cnInsertIfPresent;
					break;
				}
			}

                //#### If the .ValueType is of a .cnVerbatum, set g_bQuote to false
			if (oInputData.ValueType == MetaData.enumValueTypes.cnVerbatum) {
				g_bQuote = false;
			}
                //#### Else if the .DataType is numeric, .Make(the g_sValue a)Number and set g_bQuote to false
			else if (oInputData.DataType == MetaData.enumDataTypes.cnInteger ||
			    oInputData.DataType == MetaData.enumDataTypes.cnFloat ||
			    oInputData.DataType == MetaData.enumDataTypes.cnCurrency
			) {
//! what about (1.0) negetive numbers?
				g_sValue = Data.Tools.MakeNumeric(g_sValue, "0");
				g_bQuote = false;
			}
                //#### Else the .DataType is non-numeric and non-verbatum, so set g_bQuote to true
			else {
				g_bQuote = true;
			}
		}

		///############################################################
		/// <summary>
		/// Initializes the class based on the provided data.
		/// </summary>
		/// <param name="sColumnName">String representing the column name.</param>
		/// <param name="sValue">String representing the column value.</param>
        /// <param name="bQuoteValue">Boolean value signaling if the column value needs to be surrounded by quotes.</param>
		/// <param name="eValueOperator">Enumeration representing the comparison operator, which also signals if the column is insertable/updateable.</param>
		///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
		public ColumnDescription(string sColumnName, string sValue, bool bQuoteValue, enumValueOperators eValueOperator) {
                //#### Call .Reset to init the class vars
			Reset(sColumnName, sValue, bQuoteValue, eValueOperator);
		}

		///############################################################
		/// <summary>
		/// Resets the class to its initilized state based on the provided data.
		/// </summary>
		/// <param name="sColumnName">String representing the column name.</param>
		/// <param name="sValue">String representing the column value.</param>
        /// <param name="bQuoteValue">Boolean value signaling if the column value needs to be surrounded by quotes.</param>
		/// <param name="eValueOperator">Enumeration representing the comparison operator, which also signals if the column is insertable/updateable.</param>
		///############################################################
		/// <LastUpdated>January 11, 2010</LastUpdated>
		public void Reset(string sColumnName, string sValue, bool bQuoteValue, enumValueOperators eValueOperator) {
                //#### Set the global variables to the passed data
			g_sColumnName = Data.Tools.MakeString(sColumnName, "");
			g_sValue = Data.Tools.MakeString(sValue, "");
			g_eOperator = eValueOperator;
			g_bQuote = bQuoteValue;
		}


		//##########################################################################################
		//# Public Read-Write Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets the column name.
		/// </summary>
		/// <value>String representing the column name.</value>
		///############################################################
		/// <LastUpdated>February 15, 2006</LastUpdated>
		public string ColumnName {
			get { return g_sColumnName; }
			set { g_sColumnName = Data.Tools.MakeString(value, ""); }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the column value.
		/// </summary>
		/// <value>String representing the column value.</value>
		///############################################################
		/// <LastUpdated>February 15, 2006</LastUpdated>
		public string Value {
			get { return g_sValue; }
			set { g_sValue = Data.Tools.MakeString(value, ""); }
		}

		///############################################################
		/// <summary>
		/// Gets/sets the comparison operator, which also signals if the column is insertable/updateable.
		/// </summary>
		/// <value>Enumeration representing the column's comprison operator, or signaling that the column is insertable/updateable.</value>
		///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public enumValueOperators Operator {
			get { return g_eOperator; }
			set { g_eOperator = value; }
		}

		///############################################################
		/// <summary>
		/// Gets/sets a value indicating if column value needs to be surrounded by quotes.
		/// </summary>
		/// <value>Boolean value signaling if the value needs to be surrounded by quotes.<para/>Returns true if the column value needs to be surrounded by quotes, or false if it does not.</value>
		///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public bool Quote {
			get { return g_bQuote; }
			set { g_bQuote = value; }
		}


		//##########################################################################################
		//# Public Read-Only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets a value indicating if the column is insertable/updateable.
		/// </summary>
		/// <value>Boolean value signaling if the column is insertable/updateable or part of the WHERE clause.<para/>Returns true if the column is insertable/updateable, or false if the column is to be used in the WHERE clause only.</value>
		///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public bool IsInsertOrUpdateColumn {
			get {
					//#### Determine the g_eOperator, returning accordingly
				switch (g_eOperator) {
					case enumValueOperators.cnInsertIfPresent:
					case enumValueOperators.cnInsertNull:
					case enumValueOperators.cnInsertNullString: {
						return true;
					  //break;
					}
					default: {
						return false;
					  //break;
					}
				}
			}
		}

		///############################################################
		/// <summary>
		/// Gets a value indicating if the column is to be used in the WHERE clause.
		/// </summary>
		/// <value>Boolean value signaling if the column is to be used in the WHERE clause or not.<para/>Returns true if the column is to be used in the WHERE clause, or false if the column is not.</value>
		///############################################################
		/// <LastUpdated>January 4, 2010</LastUpdated>
		public bool IsWhereClauseColumn {
			get {
				return (g_eOperator != enumValueOperators.cnIgnore && ! IsInsertOrUpdateColumn);
			}
		}

	} //# class ColumnDescription


} //# namespace Cn.Data.SQL
