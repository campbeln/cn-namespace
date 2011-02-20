/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using Cn.Collections;
using Cn.Configuration;


namespace Cn.UnitTests.Tests.Collections {

	///########################################################################################################################
	/// <summary>
	/// Unit Test collection for the MultiArray
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	[TestClass]
	public class MultiArray_Tests {
			//#### Define the required private variables
		private TestContext testContextInstance;	


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets or sets the test context which provides information about and functionality for the current test run.
		/// </summary>
		///############################################################
		public TestContext TestContext {
			get {
				return testContextInstance;
			}
			set {
				testContextInstance = value;
			}
		}


		//##########################################################################################
		//# Private Static Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets a valid 4 column row as a Hashtable.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		private static Hashtable GetRowAsHash(string sRowIdentifier) {
			Hashtable h_sReturn = new Hashtable();
			string[] a_sColumnNames = GetColumnNames();

				//#### If the passed sRowIdentifier was blank, default it to a forth column
			if (string.IsNullOrEmpty(sRowIdentifier)) {
				sRowIdentifier = "d";
			}

				//#### Ensure the passed sRowIdentifier is a lower-cased single character
			sRowIdentifier = sRowIdentifier.Substring(0, 1).ToLower();

				//#### Populate our h_sReturn value with the a_sColumnNames
			for (int i = 0; i < a_sColumnNames.Length; i++) {
				h_sReturn[a_sColumnNames[i]] = "Value" + (i + 1) + "." + sRowIdentifier;
			}

				//#### Return the above determined h_sReturn value to the caller
			return h_sReturn;
		}

		///############################################################
		/// <summary>
		/// Gets a valid 4th 4 column row as an Array of Strings.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		private static string[] GetRowAsArray() {
			return GetRowAsArray("d");
		}

		///############################################################
		/// <summary>
		/// Gets a valid 4 column row as an Array of Strings.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		private static string[] GetRowAsArray(string sRowIdentifier) {
				//#### If the passed sRowIdentifier was blank, default it to a forth column
			if (string.IsNullOrEmpty(sRowIdentifier)) {
				sRowIdentifier = "d";
			}

				//#### Ensure the passed sRowIdentifier is a lower-cased single character
			sRowIdentifier = sRowIdentifier.Substring(0, 1).ToLower();

				//#### Calculate the Row, then return the .Split version to the caller
			return (
				"Value1." + sRowIdentifier + "," +
					"Value2." + sRowIdentifier + "," +
					"Value3." + sRowIdentifier + "," +
					"Value4." + sRowIdentifier
				).Split(',');
		}

		///############################################################
		/// <summary>
		/// Gets a valid 3 value column as an Array of Strings.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		private static string[] GetColumn() {
			return "Value5.a,Value5.b,Value5.c".Split(',');
		}

		///############################################################
		/// <summary>
		/// Gets a valid 4 column row as an Array of Strings.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		private static string[] GetColumnNames() {
			return "Column1,Column2,Column3,Column4".Split(',');
		}

		///############################################################
		/// <summary>
		/// Gets a valid 4 column, 3 row string representation of MultiArray.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		private static string GetMultiArrayString() {
			string sPrimaryDelimiter = Settings.PrimaryDelimiter;
			string sSecondaryDelimiter = Settings.SecondaryDelimiter;

			return string.Join(sSecondaryDelimiter, GetColumnNames()) + sPrimaryDelimiter +
				string.Join(sSecondaryDelimiter, GetRowAsArray("a")) + sPrimaryDelimiter +
				string.Join(sSecondaryDelimiter, GetRowAsArray("b")) + sPrimaryDelimiter +
				string.Join(sSecondaryDelimiter, GetRowAsArray("c"))
			;
		}


		//##########################################################################################
		//# Test Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Test the Constructor(a_sInitialColumnNames)/Reset(a_sInitialColumnNames) pair
		/// </summary>
		/// <remarks>
		/// void Reset(
		///		string[] a_sInitialColumnNames	Null, ValidSingleColumn, ValidMultipleColumns, *DuplicateColumnName, 
		///										*DuplicateColumnNames, *DuplicateColumnNamesiNCORRECTcASE, *BlankColumnName
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void ConstructorReset_InitialColumnNames() {
			MultiArray oTest;
			string[] a_sColumnNames;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "[Constructor] Null; ";
			a_sColumnNames = null;
			oTest = new MultiArray(a_sColumnNames);

			Assert.AreEqual(0, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			sTestMessage = "Reset Null; ";
			a_sColumnNames = null;
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Reset(a_sColumnNames);

			Assert.AreEqual(0, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			//##########
			sTestMessage = "[Constructor] ValidSingleColumn; ";
			a_sColumnNames = new string[] { "Column1" };
			oTest = new MultiArray(a_sColumnNames);

			Assert.AreEqual(1, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			sTestMessage = "Reset ValidSingleColumn; ";
			a_sColumnNames = new string[] { "Column2" };
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Reset(a_sColumnNames);

			Assert.AreEqual(1, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			//##########
			sTestMessage = "[Constructor] ValidMultipleColumns; ";
			oTest = new MultiArray(GetColumnNames());

			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			sTestMessage = "Reset ValidMultipleColumns; ";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Reset(GetColumnNames());

			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			//##########
			sTestMessage = "[Constructor] *DuplicateColumnName; ";
			a_sColumnNames = GetColumnNames();
			a_sColumnNames[1] = "Column1";

			try {
				oTest = new MultiArray(a_sColumnNames);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *DuplicateColumnName; ";
			a_sColumnNames = GetColumnNames();
			a_sColumnNames[3] = "Column3";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Reset(a_sColumnNames);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *DuplicateColumnNames; ";
			a_sColumnNames = GetColumnNames();
			a_sColumnNames[1] = "Column1";
			a_sColumnNames[2] = "Column4";

			try {
				oTest = new MultiArray(a_sColumnNames);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *DuplicateColumnNames; ";
			a_sColumnNames = GetColumnNames();
			a_sColumnNames[0] = "Column3";
			a_sColumnNames[3] = "Column2";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Reset(a_sColumnNames);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *DuplicateColumnNamesiNCORRECTcASE; ";
			a_sColumnNames = GetColumnNames();
			a_sColumnNames[1] = "COLUMN1";
			a_sColumnNames[2] = "column4";

			try {
				oTest = new MultiArray(a_sColumnNames);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *DuplicateColumnNamesiNCORRECTcASE; ";
			a_sColumnNames = GetColumnNames();
			a_sColumnNames[0] = "ColuMN3";
			a_sColumnNames[3] = "CoLUMn2";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Reset(a_sColumnNames);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *BlankColumnName; ";
			a_sColumnNames = GetColumnNames();
			a_sColumnNames[1] = "";

			try {
				oTest = new MultiArray(a_sColumnNames);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *BlankColumnName; ";
			a_sColumnNames = GetColumnNames();
			a_sColumnNames[3] = "";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Reset(a_sColumnNames);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}
		}

		///############################################################
		/// <summary>
		/// Test the Constructor(sMultiArrayString)/Reset(sMultiArrayString) pair
		/// </summary>
		/// <remarks>
		/// public MultiArray(
		///		string  sMultiArrayString	Valid, Null-String, SingleColumn, ColumnNamesOnly, *MissingColumn, *MissingsColumn,
		///									*DuplicateColumnNames, *DuplicateColumnNamesiNCORRECTcASE, *BlankColumnName
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void ConstructorReset_MultiArrayString() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "[Constructor] Valid; ";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(3, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			sTestMessage = "Reset Valid; ";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Reset(GetMultiArrayString());

			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(3, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			//##########
			sTestMessage = "[Constructor] Null-String; ";
			oTest = new MultiArray("");

			Assert.AreEqual(0, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			sTestMessage = "Reset Null-String; ";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Reset("");

			Assert.AreEqual(0, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			//##########
			sTestMessage = "[Constructor] SingleColumn; ";
			oTest = new MultiArray("Column0");

			Assert.AreEqual(1, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			sTestMessage = "Reset SingleColumn; ";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Reset("Column0");

			Assert.AreEqual(1, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			//##########
			sTestMessage = "[Constructor] ColumnNamesOnly; ";
			oTest = new MultiArray(string.Join(Settings.SecondaryDelimiter, GetColumnNames()));

			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			sTestMessage = "Reset ColumnNamesOnly; ";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Reset(string.Join(Settings.SecondaryDelimiter, GetColumnNames()));

			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			//##########
			sTestMessage = "[Constructor] *MissingColumn; ";

			try {
				oTest = new MultiArray(GetMultiArrayString()
					.Replace("Value2.c" + Settings.SecondaryDelimiter, "")
				);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *MissingColumn; ";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Reset(GetMultiArrayString()
					.Replace("Value1.a" + Settings.SecondaryDelimiter, "")
				);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *MissingColumns; ";

			try {
				oTest = new MultiArray(GetMultiArrayString()
					.Replace("Value3.b" + Settings.SecondaryDelimiter, "")
					.Replace("Value4.a" + Settings.SecondaryDelimiter, "")
				);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *MissingColumns; ";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Reset(GetMultiArrayString()
					.Replace("Value2.c" + Settings.SecondaryDelimiter, "")
					.Replace(Settings.SecondaryDelimiter + "Value1.d", "")
				);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *DuplicateColumnNames; ";

			try {
				oTest = new MultiArray(GetMultiArrayString()
					.Replace("Column1" + Settings.SecondaryDelimiter, "Column3" + Settings.SecondaryDelimiter)
				);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *DuplicateColumnNames; ";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Reset(GetMultiArrayString()
					.Replace(Settings.SecondaryDelimiter + "Column4", "Column1" + Settings.SecondaryDelimiter)
				);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *DuplicateColumnNamesiNCORRECTcASE; ";

			try {
				oTest = new MultiArray(GetMultiArrayString()
					.Replace("Column1" + Settings.SecondaryDelimiter, "column3" + Settings.SecondaryDelimiter)
				);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *DuplicateColumnNamesiNCORRECTcASE; ";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Reset(GetMultiArrayString()
					.Replace(Settings.SecondaryDelimiter + "Column4", "COLUMN1" + Settings.SecondaryDelimiter)
				);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *BlankColumnName; ";

			try {
				oTest = new MultiArray(GetMultiArrayString()
					.Replace("Column2" + Settings.SecondaryDelimiter, "" + Settings.SecondaryDelimiter)
				);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *BlankColumnName; ";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Reset(GetMultiArrayString()
					.Replace("Column1" + Settings.SecondaryDelimiter, "" + Settings.SecondaryDelimiter)
				);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}
		}

		///############################################################
		/// <summary>
		/// Test the Constructor()/Reset() pair
		/// </summary>
		/// <remarks>
		/// public MultiArray()				NoParameters
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		[TestMethod]
		public void ConstructorReset() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "[Constructor] NoParameters; ";
			oTest = new MultiArray();

			Assert.AreEqual(0, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			//##########
			sTestMessage = "Reset NoParameters; ";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Reset();

			Assert.AreEqual(0, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");
		}

		///############################################################
		/// <summary>
		/// Test the Constructor(info, ctxt)/GetObjectData(info, ctxt) pair [ISerializable]
		/// </summary>
		/// <remarks>
		/// public MultiArray(				NoParameters, MultiArrayString, InitalColumnNames
		///		SerializationInfo info,		n/a	(dotNet framework provided object)
		///		StreamingContext ctxt		n/a	(dotNet framework provided object)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		[TestMethod]
		public void Constructor_ISerializable() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "[Constructor] NoParameters; ";
			oTest = Tools.Binary_Serialize_Deserialize(new MultiArray());

			Assert.AreEqual(0, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			//##########
			sTestMessage = "[Constructor] MultiArrayString; ";
			oTest = Tools.Binary_Serialize_Deserialize(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(3, oTest.RowCount, sTestMessage + "RowCount");

			//##########
			//##########
			sTestMessage = "[Constructor] InitalColumnNames; ";
			oTest = Tools.Binary_Serialize_Deserialize(new MultiArray(GetColumnNames()));

			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage + "ColumnCount");
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "RowCount");
		}

		///############################################################
		/// <summary>
		/// Test Data
		/// </summary>
		/// <remarks>
		/// MultiArray Data					ExpectedValue, EnsureDeepCopy
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void Data() {
			MultiArray oTest;
			MultiArray oData;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue; ";
			oTest = new MultiArray(GetMultiArrayString());
			oData = oTest.Data;

			Assert.AreEqual(oTest.ColumnCount, oData.ColumnCount, sTestMessage);
			Assert.AreEqual(oTest.RowCount, oData.RowCount, sTestMessage);
			Assert.AreEqual(oTest.ToString(), oData.ToString(), sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue EnsureDeepCopy; ";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreNotSame(oTest, oData, sTestMessage);

			oData.RemoveColumn(0);
			Assert.AreNotEqual(oTest.ColumnCount, oData.ColumnCount, sTestMessage);

			oData.RemoveRow(0);
			Assert.AreNotEqual(oTest.RowCount, oData.RowCount, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test RowCount
		/// </summary>
		/// <remarks>
		/// int RowCount					ExpectedValue
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		[TestMethod]
		public void RowCount() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue; ";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(3, oTest.RowCount, sTestMessage + "3 Rows");

			oTest.InsertRow(GetRowAsArray());
			Assert.AreEqual(4, oTest.RowCount, sTestMessage + "4 Rows");

			oTest.RemoveRow(0);
			Assert.AreEqual(3, oTest.RowCount, sTestMessage + "3 Rows (#2)");

			oTest.RemoveRow(0);
			Assert.AreEqual(2, oTest.RowCount, sTestMessage + "2 Rows");

			oTest.RemoveRow(0);
			Assert.AreEqual(1, oTest.RowCount, sTestMessage + "1 Row");

			oTest.RemoveRow(0);
			Assert.AreEqual(0, oTest.RowCount, sTestMessage + "0 Rows");

			oTest.InsertRow(GetRowAsArray());
			Assert.AreEqual(1, oTest.RowCount, sTestMessage + "1 Row (#2)");
		}

		///############################################################
		/// <summary>
		/// Test ColumnCount
		/// </summary>
		/// <remarks>
		/// int ColumnCount					ExpectedValue
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		[TestMethod]
		public void ColumnCount() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue; ";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage + "4 Columns");

			oTest.InsertColumn("Column5", GetColumn());
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage + "5 Columns");

			oTest.RemoveColumn(0);
			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage + "4 Columns (#2)");

			oTest.RemoveColumn(0);
			Assert.AreEqual(3, oTest.ColumnCount, sTestMessage + "3 Columns");

			oTest.RemoveColumn(0);
			Assert.AreEqual(2, oTest.ColumnCount, sTestMessage + "2 Columns");

			oTest.RemoveColumn(0);
			Assert.AreEqual(1, oTest.ColumnCount, sTestMessage + "1 Column");

			oTest.RemoveColumn(0);
			Assert.AreEqual(0, oTest.ColumnCount, sTestMessage + "0 Columns");

			oTest.InsertColumn("Column6", GetColumn());
			Assert.AreEqual(1, oTest.ColumnCount, sTestMessage + "1 Column (#2)");
		}

		///############################################################
		/// <summary>
		/// Test ColumnNames
		/// </summary>
		/// <remarks>
		/// string[] ColumnNames			ExpectedLength, EnsureDeepCopy
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		[TestMethod]
		public void ColumnNames() {
			MultiArray oTest;
			string[] a_sColumnNames;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedLength";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = GetColumnNames();

			Assert.AreEqual(a_sColumnNames[0], oTest.ColumnName(0), sTestMessage);
			Assert.AreEqual(a_sColumnNames[1], oTest.ColumnName(1), sTestMessage);
			Assert.AreEqual(a_sColumnNames[2], oTest.ColumnName(2), sTestMessage);
			Assert.AreEqual(a_sColumnNames[3], oTest.ColumnName(3), sTestMessage);
			Assert.AreEqual(a_sColumnNames.Length, oTest.ColumnNames.Length, sTestMessage);
			
			oTest.RemoveColumn(0);
			Assert.AreEqual(a_sColumnNames.Length - 1, oTest.ColumnNames.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue EnsureDeepCopy";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = oTest.ColumnNames;

			a_sColumnNames[0] = "NewColumn1";
			Assert.AreNotEqual(a_sColumnNames[0], oTest.ColumnName(0), sTestMessage);
			oTest.RenameColumn("Column3", "NewColumn3");
			Assert.AreNotEqual(a_sColumnNames[2], oTest.ColumnName(2), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Value(iRowIndex, sColumnName)
		/// </summary>
		/// <remarks>
		/// string Value(					n/a (primitive/simple non-nullable type, value is tested below)
		///		int iRowIndex,				LBound, UBound, WithinBounds, OutOfBounds(+/-)
		///		string sColumnName			Reconized, iNCORRECTcASE, Unreconized, Null-String
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void Value_RowIndexColumnName() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "iRowIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Value1.a", oTest.Value(0, "Column1"), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Value4.c", oTest.Value(oTest.RowCount - 1, "Column4"), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Value2.b", oTest.Value(1, "Column2"), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("", oTest.Value(oTest.RowCount, "Column2"), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("", oTest.Value(-1, "Column3"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Reconized";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Value2.b", oTest.Value(1, "Column2"), sTestMessage);
			Assert.AreEqual("Value1.c", oTest.Value(2, "Column1"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName iNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Value2.c", oTest.Value(2, "column2"), sTestMessage);
			Assert.AreEqual("Value1.a", oTest.Value(0, "column1"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Unreconized";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("", oTest.Value(1, "NotColumn2"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("", oTest.Value(0, ""), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Value(iRowIndex, sColumnName, sNewValue)
		/// </summary>
		/// <remarks>
		/// void Value(
		///		int iRowIndex,				LBound, UBound, WithinBounds, *OutOfBounds(+/-)
		///		string sColumnName,			Reconized, iNCORRECTcASE, *Unreconized, *Null-String
		///		string sNewValue			Non-Null-String, Null-String, Null
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void Value_RowIndexColumnNameNewValue() {
			MultiArray oTest;
			string[] a_sNullValue = new string[1];
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "iRowIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(0, "Column1", "new1");

			Assert.AreEqual("new1", oTest.Value(0, "Column1"), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(oTest.RowCount - 1, "Column4", "new2");

			Assert.AreEqual("new2", oTest.Value(oTest.RowCount - 1, "Column4"), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(1, "Column2", "new3");

			Assert.AreEqual("new3", oTest.Value(1, "Column2"), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex *OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Value(oTest.RowCount, "Column2", "new4");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "iRowIndex *OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Value(-1, "Column3", "new5");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName Reconized";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(1, "Column4", "new6");
			oTest.Value(1, "Column3", "new7");

			Assert.AreEqual("new6", oTest.Value(1, "Column4"), sTestMessage);
			Assert.AreEqual("new7", oTest.Value(1, "Column3"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName iNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(1, "Column1", "new8");
			oTest.Value(1, "Column2", "new9");

			Assert.AreEqual("new8", oTest.Value(1, "cOLumn1"), sTestMessage);
			Assert.AreEqual("new9", oTest.Value(1, "colUMN2"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName *Unreconized";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Value(1, "NotColumn1", "new10");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Value(1, "", "new11");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sNewValue Non-Null-String";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(2, "Column3", "new12");

			Assert.AreEqual("new12", oTest.Value(2, "Column3"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sNewValue Null-String";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(1, "Column4", "");

			Assert.AreEqual("", oTest.Value(1, "Column4"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sNewValue Null";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(2, "Column2", a_sNullValue[0]);

			Assert.AreEqual("", oTest.Value(2, "Column2"), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Value(iRowIndex, iColumnIndex)
		/// </summary>
		/// <remarks>
		/// string Value(					n/a (primitive/simple non-nullable type, value is tested below)
		///		int iRowIndex,				LBound, UBound, WithinBounds, OutOfBounds(+/-)
		///		int iColumnIndex			LBound, UBound, WithinBounds, OutOfBounds(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		[TestMethod]
		public void Value_RowIndexColumnIndex() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "iRowIndex LBound, iColumnIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Value1.a", oTest.Value(0, 0), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex UBound, iColumnIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Value4.c", oTest.Value(oTest.RowCount - 1, oTest.ColumnCount - 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds, iColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Value2.b", oTest.Value(1, 1), sTestMessage);
			Assert.AreEqual("Value3.b", oTest.Value(1, 2), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds +1, iColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("", oTest.Value(oTest.RowCount, 2), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds -1, iColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("", oTest.Value(-1, 2), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds, iColumnIndex OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("", oTest.Value(1, oTest.ColumnCount), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds, iColumnIndex OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("", oTest.Value(1, -1), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Value(iRowIndex, iColumnIndex, sNewValue)
		/// </summary>
		/// <remarks>
		/// void Value(
		///		int iRowIndex,				LBound, UBound, WithinBounds, *OutOfBounds(+/-)
		///		int iColumnIndex,			LBound, UBound, WithinBounds, *OutOfBounds(+/-)
		///		string sNewValue			Non-Null-String, Null-String, Null
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void Value_RowIndexColumnIndexNewValue() {
			MultiArray oTest;
			string[] a_sNullValue = new string[1];
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "iRowIndex LBound, iColumnIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(0, 0, "new1");

			Assert.AreEqual("new1", oTest.Value(0, 0), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex UBound, iColumnIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(oTest.RowCount - 1, oTest.ColumnCount - 1, "new2");

			Assert.AreEqual("new2", oTest.Value(oTest.RowCount - 1, oTest.ColumnCount - 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds, iColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(1, 1, "new3");
			oTest.Value(1, 2, "new4");

			Assert.AreEqual("new3", oTest.Value(1, 1), sTestMessage);
			Assert.AreEqual("new4", oTest.Value(1, 2), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds +1, iColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Value(oTest.RowCount, 2, "new5");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds -1, iColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Value(-1, 2, "new6");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds, iColumnIndex OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Value(1, oTest.ColumnCount, "new7");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds, iColumnIndex OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.Value(1, -1, "new8");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sNewValue Non-Null-String";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(2, 2, "new9");

			Assert.AreEqual("new9", oTest.Value(2, 2), sTestMessage);

			//##########
			//##########
			sTestMessage = "sNewValue Null-String";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(2, 1, "");

			Assert.AreEqual("", oTest.Value(2, 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "sNewValue Null";
			oTest = new MultiArray(GetMultiArrayString());
			oTest.Value(2, 1, a_sNullValue[0]);

			Assert.AreEqual("", oTest.Value(2, 1), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Exists(sColumnName)
		/// </summary>
		/// <remarks>
		/// bool Exists(					n/a (primitive/simple non-nullable type, value is tested below)
		///		string sColumnName			Reconized, iNCORRECTcASE, Unreconized, Null-String
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void Exists_ColumnName() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sColumnName Reconized";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(true, oTest.Exists("Column1"), sTestMessage);
			Assert.AreEqual(true, oTest.Exists("Column4"), sTestMessage);
			Assert.AreEqual(true, oTest.Exists("Column2"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName iNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(true, oTest.Exists("COLumn1"), sTestMessage);
			Assert.AreEqual(true, oTest.Exists("COLUMN4"), sTestMessage);
			Assert.AreEqual(true, oTest.Exists("column2"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Unreconized";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(false, oTest.Exists("NotAColumn"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(false, oTest.Exists(""), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Exists(a_sColumnNames)
		/// </summary>
		/// <remarks>
		/// bool Exists(					n/a (primitive/simple non-nullable type, value is tested below)
		///		string[] a_sColumnNames		LBound, UBound, WithinBounds, MultiValidValues, AllValidValues, InvalidValue,
		///									MultiInvalidValues, MixedValidInvalidColumns, Null-String, MixedNullStringValidValues, Null
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 22, 2010</LastUpdated>
		[TestMethod]
		public void Exists_ColumnNames() {
			MultiArray oTest;
			string[] a_sColumnNames;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "a_sColumnNames LBound";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = new string[1]{ "Column1" };

			Assert.AreEqual(true, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames UBound";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = new string[1]{ "Column4" };

			Assert.AreEqual(true, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = new string[1]{ "Column4" };

			Assert.AreEqual(true, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames MultiValidValues";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = new string[3]{ "Column1", "Column4", "Column2" };

			Assert.AreEqual(true, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames AllValidValues";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = GetColumnNames();

			Assert.AreEqual(true, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames InvalidValue";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = new string[1]{ "NotColumn1" };

			Assert.AreEqual(false, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames MultiInvalidValues";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = new string[2]{ "NotColumn1", "NotColumn2" };

			Assert.AreEqual(false, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames MixedValidInvalidColumns";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = new string[4]{ "Column1", "NotColumn1", "NotColumn2", "Column3" };

			Assert.AreEqual(false, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames MixedValidInvalidColumns";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = new string[4]{ "Column1", "NotColumn1", "NotColumn2", "Column3" };

			Assert.AreEqual(false, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames Null-String";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = new string[1]{ "" };

			Assert.AreEqual(false, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames MixedNullStringValidValues";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = new string[5]{ "Column1", "", "Column4", "Column3", "NotColumn1" };

			Assert.AreEqual(false, oTest.Exists(a_sColumnNames), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumnNames Null";
			oTest = new MultiArray(GetMultiArrayString());
			a_sColumnNames = null;

			Assert.AreEqual(false, oTest.Exists(a_sColumnNames), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test the ColumnIndex function
		/// </summary>
		/// <remarks>
		/// int ColumnIndex(				n/a (primitive/simple non-nullable type, value is tested below)
		///		string sColumnName			Reconized, iNCORRECTcASE, Unreconized, Null-String
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void ColumnIndex() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sColumnName Reconized";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(0, oTest.ColumnIndex("Column1"), sTestMessage);
			Assert.AreEqual(3, oTest.ColumnIndex("Column4"), sTestMessage);
			Assert.AreEqual(1, oTest.ColumnIndex("Column2"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName iNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(0, oTest.ColumnIndex("ColUMN1"), sTestMessage);
			Assert.AreEqual(3, oTest.ColumnIndex("column4"), sTestMessage);
			Assert.AreEqual(1, oTest.ColumnIndex("COLUMN2"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Unreconized";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(-1, oTest.ColumnIndex("NotAColumn"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(-1, oTest.ColumnIndex(""), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test ColumnName(iColumnIndex)
		/// </summary>
		/// <remarks>
		/// string ColumnName(				n/a (primitive/simple non-nullable type, value is tested below)
		///		int iColumnIndex			LBound, UBound, WithinBounds, OutOfBounds(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 22, 2010</LastUpdated>
		[TestMethod]
		public void ColumnName() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "iColumnIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Column1", oTest.ColumnName(0), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Column4", oTest.ColumnName(oTest.ColumnCount - 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("Column2", oTest.ColumnName(1), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("", oTest.ColumnName(-1), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual("", oTest.ColumnName(oTest.ColumnCount), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Row(iRowIndex)
		/// </summary>
		/// <remarks>
		/// Hashtable Row(					ExpectedLength, EnsureDeepCopy
		///		int iRowIndex				LBound, UBound, WithinBounds, OutOfBounds(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		[TestMethod]
		public void Row() {
			MultiArray oTest;
			Hashtable h_sReturnedRow;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedLength";
			oTest = new MultiArray(GetMultiArrayString());
			h_sReturnedRow = oTest.Row(1);

			Assert.AreEqual(oTest.ColumnCount, h_sReturnedRow.Count, sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue EnsureDeepCopy";
			oTest = new MultiArray(GetMultiArrayString());
			h_sReturnedRow = oTest.Row(0);

			h_sReturnedRow["Column1"] = "new1";
			Assert.AreNotEqual(h_sReturnedRow["Column1"], oTest.Value(0, 0), sTestMessage);
			oTest.Value(0, "Column3", "new2");
			Assert.AreNotEqual(h_sReturnedRow["Column3"], oTest.Value(0, 2), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());
			h_sReturnedRow = oTest.Row(0);

			Assert.AreEqual("Value1.a", h_sReturnedRow["Column1"], sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());
			h_sReturnedRow = oTest.Row(oTest.RowCount - 1);

			Assert.AreEqual("Value4.c", h_sReturnedRow["Column4"], sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());
			h_sReturnedRow = oTest.Row(1);

			Assert.AreEqual("Value1.b", h_sReturnedRow["Column1"], sTestMessage);
			Assert.AreEqual("Value2.b", h_sReturnedRow["Column2"], sTestMessage);
			Assert.AreEqual("Value3.b", h_sReturnedRow["Column3"], sTestMessage);
			Assert.AreEqual("Value4.b", h_sReturnedRow["Column4"], sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());
			h_sReturnedRow = oTest.Row(-1);

			Assert.AreEqual(0, h_sReturnedRow.Count, sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());
			h_sReturnedRow = oTest.Row(oTest.RowCount);

			Assert.AreEqual(0, h_sReturnedRow.Count, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test RowAsArray(iRowIndex)
		/// </summary>
		/// <remarks>
		/// string[] RowAsArray(			ExpectedLength, EnsureDeepCopy
		///		int iRowIndex				LBound, UBound, WithinBounds, OutOfBounds(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		[TestMethod]
		public void RowAsArray() {
			MultiArray oTest;
			string[] a_sReturnedRow;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedLength";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedRow = oTest.RowAsArray(1);

			Assert.AreEqual(oTest.ColumnCount, a_sReturnedRow.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue EnsureDeepCopy";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedRow = oTest.RowAsArray(0);

			a_sReturnedRow[0] = "new1";
			Assert.AreNotEqual(a_sReturnedRow[0], oTest.Value(0, 0), sTestMessage);
			oTest.Value(0, "Column3", "new2");
			Assert.AreNotEqual(a_sReturnedRow[2], oTest.Value(0, 2), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedRow = oTest.RowAsArray(0);

			Assert.AreEqual("Value1.a", a_sReturnedRow[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedRow = oTest.RowAsArray(oTest.RowCount - 1);

			Assert.AreEqual("Value4.c", a_sReturnedRow[oTest.ColumnCount - 1], sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedRow = oTest.RowAsArray(1);

			Assert.AreEqual("Value1.b", a_sReturnedRow[0], sTestMessage);
			Assert.AreEqual("Value2.b", a_sReturnedRow[1], sTestMessage);
			Assert.AreEqual("Value3.b", a_sReturnedRow[2], sTestMessage);
			Assert.AreEqual("Value4.b", a_sReturnedRow[3], sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedRow = oTest.RowAsArray(-1);

			Assert.AreEqual(0, a_sReturnedRow.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedRow = oTest.RowAsArray(oTest.RowCount);

			Assert.AreEqual(0, a_sReturnedRow.Length, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Column(sColumnName)
		/// </summary>
		/// <remarks>
		/// string[] Column(				ExpectedLength, EnsureDeepCopy
		///		string sColumnName			Reconized, iNCORRECTcASE, Unreconized, Null-String
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void Column_ColumnName() {
			MultiArray oTest;
			string[] a_sReturnedColumn;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedLength";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column("Column2");

			Assert.AreEqual(oTest.RowCount, a_sReturnedColumn.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue EnsureDeepCopy";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column("Column3");

			a_sReturnedColumn[1] = "new1";
			Assert.AreNotEqual(a_sReturnedColumn[0], oTest.Value(1, "Column3"), sTestMessage);
			oTest.Value(2, "Column3", "new2");
			Assert.AreNotEqual(a_sReturnedColumn[2], oTest.Value(2, "Column3"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Reconized";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column("Column1");

			Assert.AreEqual("Value1.a", a_sReturnedColumn[0], sTestMessage);
			Assert.AreEqual("Value1.b", a_sReturnedColumn[1], sTestMessage);
			Assert.AreEqual("Value1.c", a_sReturnedColumn[2], sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName iNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column("COLUMN1");

			Assert.AreEqual("Value1.a", a_sReturnedColumn[0], sTestMessage);
			Assert.AreEqual("Value1.b", a_sReturnedColumn[1], sTestMessage);
			Assert.AreEqual("Value1.c", a_sReturnedColumn[2], sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Unreconized";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column("UnreconizedColumn");

			Assert.AreEqual(0, a_sReturnedColumn.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Null-String";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column("");

			Assert.AreEqual(0, a_sReturnedColumn.Length, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Column(iColumnIndex)
		/// </summary>
		/// <remarks>
		/// string[] Column(				ExpectedLength, EnsureDeepCopy
		///		int iColumnIndex			LBound, UBound, WithinBounds, OutOfBounds(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 23, 2010</LastUpdated>
		[TestMethod]
		public void Column_ColumnIndex() {
			MultiArray oTest;
			string[] a_sReturnedColumn;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedLength";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column(1);

			Assert.AreEqual(oTest.RowCount, a_sReturnedColumn.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue EnsureDeepCopy";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column("Column4");

			a_sReturnedColumn[2] = "new1";
			Assert.AreNotEqual(a_sReturnedColumn[2], oTest.Value(2, "Column4"), sTestMessage);
			oTest.Value(1, "Column4", "new2");
			Assert.AreNotEqual(a_sReturnedColumn[1], oTest.Value(1, "Column4"), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column(0);

			Assert.AreEqual("Value1.a", a_sReturnedColumn[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column(oTest.ColumnCount - 1);

			Assert.AreEqual("Value4.c", a_sReturnedColumn[oTest.RowCount - 1], sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column(1);

			Assert.AreEqual("Value2.a", a_sReturnedColumn[0], sTestMessage);
			Assert.AreEqual("Value2.b", a_sReturnedColumn[1], sTestMessage);
			Assert.AreEqual("Value2.c", a_sReturnedColumn[2], sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column(oTest.ColumnCount);

			Assert.AreEqual(0, a_sReturnedColumn.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sReturnedColumn = oTest.Column(-1);

			Assert.AreEqual(0, a_sReturnedColumn.Length, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test InsertRow(a_sRow)
		/// </summary>
		/// <remarks>
		/// void InsertRow(
		///		string[] a_sRow				Valid, *Null, *IncorrectColumnCount(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 22, 2010</LastUpdated>
		[TestMethod]
		public void InsertRow_RowArray() {
			MultiArray oTest;
			string[] a_sRow;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "a_sRow Valid";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = GetRowAsArray();

			oTest.InsertRow(a_sRow);
			Assert.AreEqual(4, oTest.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray()), string.Join(",", oTest.RowAsArray(oTest.RowCount - 1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sRow *Null";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = null;

			try {
				oTest.InsertRow(a_sRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sRow *IncorrectColumnCount -1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = Cn.Data.Tools.Pop(GetRowAsArray());

			try {
				oTest.InsertRow(a_sRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sRow *IncorrectColumnCount +1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = Cn.Data.Tools.Push(GetRowAsArray(), "Value5.d");

			try {
				oTest.InsertRow(a_sRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}
		}

		///############################################################
		/// <summary>
		/// Test InsertRow(a_sRow, iDestinationRowIndex)
		/// </summary>
		/// <remarks>
		/// void InsertRow(
		///		string[] a_sRow,			Valid, *Null, *IncorrectColumnCount(+/-)
		///		int iDestinationRowIndex	LBound, UBound, WithinBounds, *OutOfBounds(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 22, 2010</LastUpdated>
		[TestMethod]
		public void InsertRow_RowArrayDestinationRowIndex() {
			MultiArray oTest;
			string[] a_sRow;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "a_sRow Valid";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = GetRowAsArray();

			oTest.InsertRow(a_sRow, 1);
			Assert.AreEqual(4, oTest.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray()), string.Join(",", oTest.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sRow *Null";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = null;

			try {
				oTest.InsertRow(a_sRow, 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sRow *IncorrectColumnCount -1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = Cn.Data.Tools.Pop(GetRowAsArray());

			try {
				oTest.InsertRow(a_sRow, 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sRow *IncorrectColumnCount +1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = Cn.Data.Tools.Push(GetRowAsArray(), "Value5.d");

			try {
				oTest.InsertRow(a_sRow, 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "iDestinationRowIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = GetRowAsArray();

			oTest.InsertRow(a_sRow, 0);
			Assert.AreEqual(4, oTest.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray()), string.Join(",", oTest.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "iDestinationRowIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = GetRowAsArray();

			oTest.InsertRow(a_sRow, oTest.RowCount);
			Assert.AreEqual(4, oTest.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray()), string.Join(",", oTest.RowAsArray(oTest.RowCount - 1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "iDestinationRowIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = GetRowAsArray();

			oTest.InsertRow(a_sRow, 1);
			Assert.AreEqual(4, oTest.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray()), string.Join(",", oTest.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "iDestinationRowIndex *OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = GetRowAsArray();

			try {
				oTest.InsertRow(a_sRow, -1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "iDestinationRowIndex *OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sRow = GetRowAsArray();

			try {
				oTest.InsertRow(a_sRow, oTest.RowCount + 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}
		}

		///############################################################
		/// <summary>
		/// Test InsertRow(h_sRow)
		/// </summary>
		/// <remarks>
		/// void InsertRow(
		///		Hashtable h_sRow			Valid, *Null, *IncorrectColumnCount(+/-), *UnreconizedColumn
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 22, 2010</LastUpdated>
		[TestMethod]
		public void InsertRow_RowHash() {
			MultiArray oTest;
			Hashtable h_sRow;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "h_sRow Valid";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");

			oTest.InsertRow(h_sRow);
			Assert.AreEqual(4, oTest.RowCount, sTestMessage);
			Assert.AreEqual("Value1.d", oTest.Row(oTest.RowCount - 1)["Column1"], sTestMessage);

			//##########
			//##########
			sTestMessage = "h_sRow *Null";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = null;

			try {
				oTest.InsertRow(h_sRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "h_sRow *IncorrectColumnCount +1";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");
			h_sRow["Column5"] = "NewValue";

			try {
				oTest.InsertRow(h_sRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "h_sRow *IncorrectColumnCount -1";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");
			h_sRow.Remove("Column1");

			try {
				oTest.InsertRow(h_sRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "h_sRow *UnreconizedColumn";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = oTest.Row(0);
			h_sRow.Remove("Column1");
			h_sRow["Column5"] = "NewValue";

			try {
				oTest.InsertRow(h_sRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}
		}

		///############################################################
		/// <summary>
		/// Test InsertRow(h_sRow, iDestinationRowIndex)
		/// </summary>
		/// <remarks>
		/// void InsertRow(
		///		Hashtable h_sRow,			Valid, *Null, *IncorrectColumnCount(+/-), *UnreconizedColumn
		///		int iDestinationRowIndex	LBound, UBound, WithinBounds, *OutOfBounds(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 22, 2010</LastUpdated>
		[TestMethod]
		public void InsertRow_RowHashDestinationRowIndex() {
			MultiArray oTest;
			Hashtable h_sRow;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "h_sRow Valid";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");

			oTest.InsertRow(h_sRow, 1);
			Assert.AreEqual(4, oTest.RowCount, sTestMessage);
			Assert.AreEqual("Value1.d", oTest.Row(1)["Column1"], sTestMessage);

			//##########
			//##########
			sTestMessage = "h_sRow *Null";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = null;

			try {
				oTest.InsertRow(h_sRow, 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "h_sRow *IncorrectColumnCount +1";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");
			h_sRow["Column5"] = "Value5.d";

			try {
				oTest.InsertRow(h_sRow, 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "h_sRow *IncorrectColumnCount -1";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");
			h_sRow.Remove("Column1");

			try {
				oTest.InsertRow(h_sRow, 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "h_sRow *UnreconizedColumn";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");
			h_sRow.Remove("Column1");
			h_sRow["Column5"] = "Value5.d";

			try {
				oTest.InsertRow(h_sRow, 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "iDestinationRowIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");

			oTest.InsertRow(h_sRow, 0);
			Assert.AreEqual(4, oTest.RowCount, sTestMessage);
			Assert.AreEqual("Value1.d", oTest.Row(0)["Column1"], sTestMessage);

			//##########
			//##########
			sTestMessage = "iDestinationRowIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");

			oTest.InsertRow(h_sRow, oTest.RowCount);
			Assert.AreEqual(4, oTest.RowCount, sTestMessage);
			Assert.AreEqual("Value1.d", oTest.Row(oTest.RowCount - 1)["Column1"], sTestMessage);

			//##########
			//##########
			sTestMessage = "iDestinationRowIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");

			oTest.InsertRow(h_sRow, 1);
			Assert.AreEqual(4, oTest.RowCount, sTestMessage);
			Assert.AreEqual("Value3.d", oTest.Row(1)["Column3"], sTestMessage);

			//##########
			//##########
			sTestMessage = "iDestinationRowIndex *OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");

			try {
				oTest.InsertRow(h_sRow, oTest.RowCount + 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "iDestinationRowIndex *OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());
			h_sRow = GetRowAsHash("d");

			try {
				oTest.InsertRow(h_sRow, -1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}
		}

		///############################################################
		/// <summary>
		/// Test InsertColumn(sColumnName)
		/// </summary>
		/// <remarks>
		/// void InsertColumn(
		///		string sColumnName			Valid, *Duplicate, *DupicateiNCORRECTcASE, *Null-String
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void InsertColumn_ColumnName() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sColumnName Valid";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.InsertColumn("Column5");
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("", oTest.Value(0, oTest.ColumnCount - 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName *Duplicate";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.InsertColumn("Column1");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *DupicateiNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.InsertColumn("column1");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.InsertColumn("");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}
		}

		///############################################################
		/// <summary>
		/// Test InsertColumn(sColumnName, a_sColumn)
		/// </summary>
		/// <remarks>
		/// void InsertColumn(
		///		string sColumnName,			Valid, *Duplicate, *DupicateiNCORRECTcASE, *Null-String
		///		string[] a_sColumn			Valid, ValidWithNulls, *MissingRow, *MissingRows, *TooManyRows, EnsureDeepCopy
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void InsertColumn_ColumnNameColumnArray() {
			MultiArray oTest;
			string[] a_sNewColumn;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sColumnName Valid";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new1", "new2", "new3" };

			oTest.InsertColumn("Column5", a_sNewColumn);
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("new1", oTest.Value(0, oTest.ColumnCount - 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName *Duplicate";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new4", "new5", "new6" };

			try {
				oTest.InsertColumn("Column1", a_sNewColumn);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *DupicateiNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new7", "new8", "new9" };

			try {
				oTest.InsertColumn("CoLUmn1", a_sNewColumn);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *Null-String";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new10", "new11", "new12" };

			try {
				oTest.InsertColumn("", a_sNewColumn);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sColumn Valid";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new13", "new14", "new15" };

			oTest.InsertColumn("Column5", a_sNewColumn);
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("new14", oTest.Value(1, oTest.ColumnCount - 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumn ValidWithNulls";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[3];

			oTest.InsertColumn("Column5", a_sNewColumn);
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("", oTest.Value(0, oTest.ColumnCount - 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumn *MissingRow";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new16", "new17" };

			try {
				oTest.InsertColumn("Column5", a_sNewColumn);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sColumn *MissingRows";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new18" };

			try {
				oTest.InsertColumn("Column5", a_sNewColumn);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sColumn *TooManyRows";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new19", "new20", "new21", "new22" };

			try {
				oTest.InsertColumn("Column5", a_sNewColumn);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sColumn EnsureDeepCopy";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new23", "new24", "new25" };

			oTest.InsertColumn("Column5", a_sNewColumn, 0);
			a_sNewColumn[2] = "newer25";
			Assert.AreNotEqual(a_sNewColumn[2], oTest.Value(0, 2), sTestMessage);
			oTest.Value(0, 0, "newer23");
			Assert.AreNotEqual(a_sNewColumn[0], oTest.Value(0, 0), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test InsertColumn(sColumnName, a_sColumn, iDestinationColumnIndex)
		/// </summary>
		/// <remarks>
		/// void InsertColumn(
		///		string sColumnName,			Valid, *Duplicate, *DupicateiNCORRECTcASE, *Null-String
		///		string[] a_sColumn,			Valid, ValidWithNulls, *MissingRow, *MissingRows, *TooManyRows, EnsureDeepCopy
		///		int iDestinationColumnIndex	LBound, UBound, WithinBounds, *OutOfBounds(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void InsertColumn_ColumnNameColumnArrayDestinationColumnIndex() {
			MultiArray oTest;
			string[] a_sNewColumn;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sColumnName Valid";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new1", "new2", "new3" };

			oTest.InsertColumn("Column5", a_sNewColumn, 1);
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("new2", oTest.Value(1, 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName *Duplicate";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new4", "new5", "new6" };

			try {
				oTest.InsertColumn("Column1", a_sNewColumn, 2);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *DupicateiNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new7", "new8", "new9" };

			try {
				oTest.InsertColumn("Column1", a_sNewColumn, 2);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *Null-String";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new10", "new11", "new12" };

			try {
				oTest.InsertColumn("", a_sNewColumn, 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sColumn Valid";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new13", "new14", "new15" };

			oTest.InsertColumn("Column5", a_sNewColumn, 2);
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("new15", oTest.Value(2, 2), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumn ValidWithNulls";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[3];

			oTest.InsertColumn("Column5", a_sNewColumn, 1);
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("", oTest.Value(0, 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sColumn *MissingRow";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new16", "new17" };

			try {
				oTest.InsertColumn("Column5", a_sNewColumn, 2);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sColumn *MissingRows";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new18" };

			try {
				oTest.InsertColumn("Column5", a_sNewColumn, 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sColumn *TooManyRows";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new19", "new20", "new21", "new22" };

			try {
				oTest.InsertColumn("Column5", a_sNewColumn, 2);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sColumn EnsureDeepCopy";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new23", "new24", "new25" };

			oTest.InsertColumn("Column5", a_sNewColumn, 0);
			a_sNewColumn[2] = "newer25";
			Assert.AreNotEqual(a_sNewColumn[2], oTest.Value(0, 2), sTestMessage);
			oTest.Value(0, 0, "newer23");
			Assert.AreNotEqual(a_sNewColumn[0], oTest.Value(0, 0), sTestMessage);

			//##########
			//##########
			sTestMessage = "iDestinationColumnIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new26", "new27", "new28" };

			oTest.InsertColumn("Column5", a_sNewColumn, 0);
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("new26", oTest.Value(0, 0), sTestMessage);

			//##########
			//##########
			sTestMessage = "iDestinationColumnIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new29", "new30", "new31" };

			oTest.InsertColumn("Column5", a_sNewColumn, oTest.ColumnCount);
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("new31", oTest.Value(oTest.RowCount - 1, oTest.ColumnCount - 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "iDestinationColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new32", "new33", "new34" };

			oTest.InsertColumn("Column5", a_sNewColumn, 2);
			Assert.AreEqual(5, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("new33", oTest.Value(1, 2), sTestMessage);

			//##########
			//##########
			sTestMessage = "iDestinationColumnIndex *OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new35", "new36", "new37" };

			try {
				oTest.InsertColumn("Column5", a_sNewColumn, -1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "iDestinationColumnIndex *OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());
			a_sNewColumn = new string[] { "new38", "new39", "new40" };

			try {
				oTest.InsertColumn("Column5", a_sNewColumn, oTest.ColumnCount + 1);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}
		}

		///############################################################
		/// <summary>
		/// Test RemoveRow(iRowIndex)
		/// </summary>
		/// <remarks>
		/// void RemoveRow(
		///		int iRowIndex				LBound, UBound, WithinBounds, OutOfBounds(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void RemoveRow() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "iRowIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveRow(0);
			Assert.AreEqual(2, oTest.RowCount, sTestMessage);
			Assert.AreNotEqual("Value1.a", oTest.Value(0, 0), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveRow(oTest.RowCount - 1);
			Assert.AreEqual(2, oTest.RowCount, sTestMessage);
			Assert.AreNotEqual("Value1.c", oTest.Value(oTest.RowCount - 1, 0), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveRow(1);
			Assert.AreEqual(2, oTest.RowCount, sTestMessage);
			Assert.AreNotEqual("Value1.b", oTest.Value(1, 0), sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveRow(-1);
			Assert.AreEqual(3, oTest.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "iRowIndex OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveRow(oTest.RowCount + 1);
			Assert.AreEqual(3, oTest.RowCount, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test RemoveColumn(sColumnName)
		/// </summary>
		/// <remarks>
		/// void RemoveColumn(
		///		string sColumnName			Reconized, iNCORRECTcASE, Unreconized, Null-String
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void RemoveColumn_ColumnName() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sColumnName Reconized";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveColumn("Column1");
			Assert.AreEqual(3, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("Value2.a", oTest.Value(0, 0), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName iNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveColumn("COLumn3");
			Assert.AreEqual(3, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("Value4.a", oTest.Value(0, 2), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Unreconized";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveColumn("NotColumn1");
			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveColumn("");
			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test RemoveColumn(iColumnIndex)
		/// </summary>
		/// <remarks>
		/// void RemoveColumn(
		///		int iColumnIndex			LBound, UBound, WithinBounds, OutOfBounds(+/-)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void RemoveColumn_ColumnIndex() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "iColumnIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveColumn(0);
			Assert.AreEqual(3, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("Value2.a", oTest.Value(0, 0), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveColumn(oTest.ColumnCount - 1);
			Assert.AreEqual(3, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("Value3.a", oTest.Value(0, oTest.ColumnCount - 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveColumn(1);
			Assert.AreEqual(3, oTest.ColumnCount, sTestMessage);
			Assert.AreEqual("Value3.a", oTest.Value(0, 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveColumn(-1);
			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RemoveColumn(oTest.ColumnCount);
			Assert.AreEqual(4, oTest.ColumnCount, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test RenameColumn(sColumnName, sNewColumnName)
		/// </summary>
		/// <remarks>
		/// void RenameColumn(
		///		string sColumnName,			Reconized, iNCORRECTcASE, *Unreconized, *Null-String
		///		string sNewColumnName		Valid, *Duplicate, *DupicateiNCORRECTcASE, *Null-String, RenameToSame
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void RenameColumn_ColumnNameNewColumnName() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sColumnName Reconized";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RenameColumn("Column1", "NewColumn1");
			Assert.AreEqual("NewColumn1", oTest.ColumnName(0), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName iNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RenameColumn("ColUMn2", "NewColumn2");
			Assert.AreEqual("NewColumn2", oTest.ColumnName(1), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName *Unreconized";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.RenameColumn("NotColumn1", "NewNotColumn1");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.RenameColumn("", "NewColumn");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sNewColumnName Valid";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RenameColumn("Column4", "NewColumn4");
			Assert.AreEqual("NewColumn4", oTest.ColumnName(3), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName *Duplicate";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.RenameColumn("Column2", "Column3");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *DupicateiNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.RenameColumn("Column3", "COLUMN2");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.RenameColumn("Column3", "");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sNewColumnName RenameToSame";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RenameColumn("Column3", "Column3");
			Assert.AreEqual("Column3", oTest.ColumnName(2), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test RenameColumn(iColumnIndex, sNewColumnName)
		/// </summary>
		/// <remarks>
		/// void RenameColumn(
		///		int iColumnIndex,			LBound, UBound, WithinBounds, *OutOfBounds(+/-)
		///		string sNewColumnName		Valid, *Duplicate, *DupicateiNCORRECTcASE, *Null-String, ?RenameToSame
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void RenameColumn_ColumnIndexNewColumnName() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "iColumnIndex LBound";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RenameColumn(0, "NewColumn1");
			Assert.AreEqual("NewColumn1", oTest.ColumnName(0), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex UBound";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RenameColumn(oTest.ColumnCount - 1, "NewColumn4");
			Assert.AreEqual("NewColumn4", oTest.ColumnName(oTest.ColumnCount - 1), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex WithinBounds";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RenameColumn(2, "NewColumn3");
			Assert.AreEqual("NewColumn3", oTest.ColumnName(2), sTestMessage);

			//##########
			//##########
			sTestMessage = "iColumnIndex *OutOfBounds -1";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.RenameColumn(-1, "NewColumn");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "iColumnIndex *OutOfBounds +1";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.RenameColumn(oTest.ColumnCount, "NewColumn");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sNewColumnName Valid";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RenameColumn(1, "NewColumn2");
			Assert.AreEqual("NewColumn2", oTest.ColumnName(1), sTestMessage);

			//##########
			//##########
			sTestMessage = "sColumnName *Duplicate";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.RenameColumn(1, "Column3");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *DupicateiNCORRECTcASE";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.RenameColumn(2, "COLUMN2");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sColumnName *Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				oTest.RenameColumn(2, "");
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sNewColumnName RenameToSame";
			oTest = new MultiArray(GetMultiArrayString());

			oTest.RenameColumn(2, "Column3");
			Assert.AreEqual("Column3", oTest.ColumnName(2), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test ToString()
		/// </summary>
		/// <remarks>
		/// string ToString()				ExpectedValueColumnsOnly, ExpectedValueFullStructure, ExpectedValueEmpty
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void ToString_() {
			MultiArray oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValueColumnsOnly";
			oTest = new MultiArray(GetColumnNames());

			Assert.AreEqual(string.Join(Settings.SecondaryDelimiter, GetColumnNames()), oTest.ToString(), sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValueFullStructure";
			oTest = new MultiArray(GetMultiArrayString());

			Assert.AreEqual(GetMultiArrayString(), oTest.ToString(), sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValueEmpty";
			oTest = new MultiArray("");

			Assert.AreEqual("", oTest.ToString(), sTestMessage);

			oTest = new MultiArray();

			Assert.AreEqual("", oTest.ToString(), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test GenerateSQLStatements(sTableName, sIDColumn, eStatementType)
		/// </summary>
		/// <remarks>
		/// string[] GenerateSQLStatements(			n/a (ExpectedLength is tested below)
		///		string sTableName,					Non-Null-String, *Null-String
		///		string sIDColumn,					Reconized, *Unreconized, ~Null-String
		///		enumStatementTypes eStatementType	EnumValues
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void GenerateSQLStatements() {
			MultiArray oTest;
			string[] a_sSQLStatements;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sTableName Non-Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			a_sSQLStatements = oTest.GenerateSQLStatements("TableName", "column3", MultiArray.enumStatementTypes.cnInsert);
			Assert.AreNotEqual(0, a_sSQLStatements.Length, sTestMessage);
			Assert.AreEqual(oTest.RowCount, a_sSQLStatements.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "sTableName *Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				a_sSQLStatements = oTest.GenerateSQLStatements("", "Column1", MultiArray.enumStatementTypes.cnUpdate);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sIDColumn Reconized";
			oTest = new MultiArray(GetMultiArrayString());

			a_sSQLStatements = oTest.GenerateSQLStatements("TableName", "COLUMN4", MultiArray.enumStatementTypes.cnInsert);
			Assert.AreNotEqual(0, a_sSQLStatements.Length, sTestMessage);
			Assert.AreEqual(oTest.RowCount, a_sSQLStatements.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "sIDColumn *Unreconized";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				a_sSQLStatements = oTest.GenerateSQLStatements("TableName", "NotColumn1", MultiArray.enumStatementTypes.cnUpdate);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "sIDColumn ~Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			a_sSQLStatements = oTest.GenerateSQLStatements("TableName", "", MultiArray.enumStatementTypes.cnInsert);
			Assert.AreNotEqual(0, a_sSQLStatements.Length, sTestMessage);
			Assert.AreEqual(oTest.RowCount, a_sSQLStatements.Length, sTestMessage);

			//##########
			sTestMessage = "sIDColumn ~Null-String";
			oTest = new MultiArray(GetMultiArrayString());

			try {
				a_sSQLStatements = oTest.GenerateSQLStatements("TableName", "", MultiArray.enumStatementTypes.cnUpdate);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "eStatementType EnumValues; ";
			oTest = new MultiArray(GetMultiArrayString());

			a_sSQLStatements = oTest.GenerateSQLStatements("TableName", "CoLUmn3", MultiArray.enumStatementTypes.cnInsert);
			Assert.AreNotEqual(0, a_sSQLStatements.Length, sTestMessage + "Insert");
			Assert.AreEqual(oTest.RowCount, a_sSQLStatements.Length, sTestMessage + "Insert");

			//##########
			sTestMessage = "eStatementType EnumValues; ";
			oTest = new MultiArray(GetMultiArrayString());

			a_sSQLStatements = oTest.GenerateSQLStatements("TableName", "ColUmN1", MultiArray.enumStatementTypes.cnUpdate);
			Assert.AreNotEqual(0, a_sSQLStatements.Length, sTestMessage + "Update");
			Assert.AreEqual(oTest.RowCount, a_sSQLStatements.Length, sTestMessage + "Update");
		}

		///############################################################
		/// <summary>
		/// Test UpdatedColumns(h_sCheckRow, h_sOriginalRow)
		/// </summary>
		/// <remarks>
		/// string[] UpdatedColumns(		ExpectedValue
		///		Hashtable h_sCheckRow,		Valid, Empty, *Null, *ExtraColumn, MissingColumn, MissingColumns
		///		Hashtable h_sOriginalRow	Valid, ~Empty, *Null, ExtraColumn, *MissingColumn, *MissingColumns
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void UpdatedColumns() {
			Hashtable h_sOriginalRow;
			Hashtable h_sCheckRow;
			string[] a_sUpdatedColumns;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue; ";
			h_sOriginalRow = GetRowAsHash("a");
			h_sCheckRow = GetRowAsHash("a");
			h_sCheckRow["Column2"] = "new1";

			a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
			Assert.AreNotEqual(0, a_sUpdatedColumns.Length, sTestMessage + "ChangedValue");
			Assert.AreEqual(1, a_sUpdatedColumns.Length, sTestMessage + "ChangedValue");
			Assert.AreEqual("Column2", a_sUpdatedColumns[0], sTestMessage + "ChangedValue");

			//##########
			sTestMessage = "ReturnValue ExpectedValue; ";
			h_sOriginalRow = GetRowAsHash("a");
			h_sCheckRow = GetRowAsHash("a");
			h_sCheckRow["Column1"] = "new2";
			h_sCheckRow["Column4"] = "new3";

			a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
			Assert.AreNotEqual(0, a_sUpdatedColumns.Length, sTestMessage + "ChangedValues");
			Assert.AreEqual(2, a_sUpdatedColumns.Length, sTestMessage + "ChangedValues");
			Assert.AreEqual("Column1", a_sUpdatedColumns[1], sTestMessage + "ChangedValues");
			Assert.AreEqual("Column4", a_sUpdatedColumns[0], sTestMessage + "ChangedValues");

			//##########
			//##########
			sTestMessage = "h_sCheckRow Valid";
			h_sOriginalRow = GetRowAsHash("a");
			h_sCheckRow = GetRowAsHash("a");

			a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
			Assert.AreEqual(0, a_sUpdatedColumns.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "h_sCheckRow Empty";
			h_sOriginalRow = GetRowAsHash("a");
			h_sCheckRow = new Hashtable();

			a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
			Assert.AreEqual(0, a_sUpdatedColumns.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "h_sCheckRow *Null";
			h_sOriginalRow = GetRowAsHash("a");
			h_sCheckRow = null;

			try {
				a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "h_sCheckRow *ExtraColumn";
			h_sOriginalRow = GetRowAsHash("a");
			h_sCheckRow = GetRowAsHash("a");
			h_sCheckRow["Column5"] = "Value5.a";

			try {
				a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "h_sCheckRow MissingColumn";
			h_sOriginalRow = GetRowAsHash("a");
			h_sCheckRow = GetRowAsHash("a");
			h_sCheckRow.Remove("Column4");

			a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
			Assert.AreEqual(0, a_sUpdatedColumns.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "h_sCheckRow MissingColumns";
			h_sOriginalRow = GetRowAsHash("a");
			h_sCheckRow = GetRowAsHash("a");
			h_sCheckRow.Remove("Column1");
			h_sCheckRow.Remove("Column3");

			a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
			Assert.AreEqual(0, a_sUpdatedColumns.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "h_sOriginalRow Valid";
			h_sOriginalRow = GetRowAsHash("a");
			h_sCheckRow = GetRowAsHash("a");

			a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
			Assert.AreEqual(0, a_sUpdatedColumns.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "h_sOriginalRow ~Empty";
			h_sOriginalRow = new Hashtable();
			h_sCheckRow = GetRowAsHash("a");

			try {
				a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "h_sOriginalRow ~Empty";
			h_sOriginalRow = new Hashtable();
			h_sCheckRow = new Hashtable();

			a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
			Assert.AreEqual(0, a_sUpdatedColumns.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "h_sOriginalRow *Null";
			h_sOriginalRow = null;
			h_sCheckRow = GetRowAsHash("a");

			try {
				a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "h_sOriginalRow ExtraColumn";
			h_sOriginalRow = GetRowAsHash("a");
			h_sOriginalRow["Column5"] = "Value5.a";
			h_sCheckRow = GetRowAsHash("a");

			a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
			Assert.AreEqual(0, a_sUpdatedColumns.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "h_sOriginalRow *MissingColumn";
			h_sOriginalRow = GetRowAsHash("a");
			h_sOriginalRow.Remove("Column3");
			h_sCheckRow = GetRowAsHash("a");

			try {
				a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "h_sOriginalRow *MissingColumns";
			h_sOriginalRow = GetRowAsHash("a");
			h_sOriginalRow.Remove("Column1");
			h_sOriginalRow.Remove("Column4");
			h_sCheckRow = GetRowAsHash("a");

			try {
				a_sUpdatedColumns = MultiArray.UpdatedColumns(h_sCheckRow, h_sOriginalRow);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}
		}


	} //# public class MultiArray_Tests


} //# namespace Cn_UnitTests.Collections
