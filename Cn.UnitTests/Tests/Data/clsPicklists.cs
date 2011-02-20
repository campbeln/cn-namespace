/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Cn.Collections;
using Cn.Configuration;
using Cn.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cn.UnitTests {
	
}

namespace Cn.UnitTests.Tests.Data {

	///########################################################################################################################
	/// <summary>
	/// Unit Test collection for the Picklists
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	[TestClass]
	public class Picklists_Test {
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
		/// Gets a valid 4 column row as an Array of Strings.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		private static string[] GetColumnNames() {
			return Cn.Data.Picklists.GetData.RequiredColumns;
		}

		///############################################################
		/// <summary>
		/// Gets a valid Picklist containing 2 picklists (plus the _PicklistMetaData).
		/// </summary>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		private static string GetMultiArrayString() {
			string sPrimaryDelimiter = Settings.PrimaryDelimiter;
			string sSecondaryDelimiter = Settings.SecondaryDelimiter;
			string sData = "1,0,0,_PicklistMetaData,Picklist0,true" + sPrimaryDelimiter +
				"2,0,1,Titles,Picklist1,true" + sPrimaryDelimiter +
				"3,0,2,Boolean,Picklist2,false" + sPrimaryDelimiter +
				"4,1,0,Mr,Mister,true" + sPrimaryDelimiter +
				"5,1,1,Mrs,Misses,true" + sPrimaryDelimiter +
				"6,1,2,Ms,Miss,false" + sPrimaryDelimiter +
				"7,1,3,Prof,Professor,true" + sPrimaryDelimiter +
				"8,2,0,1,True,false" + sPrimaryDelimiter +
				"9,2,1,0,False,true";

			return string.Join(sSecondaryDelimiter, GetColumnNames()) + sPrimaryDelimiter +
				sData.Replace(",", sSecondaryDelimiter)
			;
		}

		///############################################################
		/// <summary>
		/// Gets a valid Picklist Item.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 29, 2010</LastUpdated>
		private static string[] GetRowAsArray(int iIndex) {
			string[] a_sMultiArrayString = GetMultiArrayString().Split(Settings.PrimaryDelimiter.ToCharArray());

			return a_sMultiArrayString[iIndex].Split(Settings.SecondaryDelimiter.ToCharArray());
		}

		//##########################################################################################
		//# Test Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Test the Constructor(oPicklistData)/Reset(oPicklistData) pair
		/// </summary>
		/// <remarks>
		/// Picklists(
		///		MultiArray oPicklistData	Valid, *Null, *NoRows, *MissingRequiredColumn, *MissingRequiredColumns, *NonNumericPicklistID
		///									*NonNumericPicklistIDs, ExtraColumns
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 28, 2010</LastUpdated>
		[TestMethod]
		public void ConstructorReset_PicklistData() {
			Picklists oTest;
			MultiArray oMultiArray;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "[Constructor] Valid";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(6, oTest.Data.ColumnCount, sTestMessage);
			Assert.AreEqual(9, oTest.Data.RowCount, sTestMessage);

			//##########
			sTestMessage = "Reset Valid";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oTest.Reset(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(6, oTest.Data.ColumnCount, sTestMessage);
			Assert.AreEqual(9, oTest.Data.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "[Constructor] *Null";

			try {
				oTest = new Picklists(null);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			try {
				oTest.Reset(null);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *NoRows";

			try {
				oTest = new Picklists(new MultiArray(string.Join(Settings.SecondaryDelimiter, GetColumnNames())));
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *NoRows";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			try {
				oTest.Reset(new MultiArray(string.Join(Settings.SecondaryDelimiter, GetColumnNames())));
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *MissingRequiredColumn";
			oMultiArray = new MultiArray(GetMultiArrayString());
			oMultiArray.RemoveColumn("ID");

			try {
				oTest = new Picklists(oMultiArray);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *MissingRequiredColumn";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oMultiArray = new MultiArray(GetMultiArrayString());
			oMultiArray.RemoveColumn("ID");

			try {
				oTest.Reset(oMultiArray);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *MissingRequiredColumns";
			oMultiArray = new MultiArray(GetMultiArrayString());
			oMultiArray.RemoveColumn("PicklistID");
			oMultiArray.RemoveColumn("IsActive");

			try {
				oTest = new Picklists(oMultiArray);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *MissingRequiredColumns";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oMultiArray = new MultiArray(GetMultiArrayString());
			oMultiArray.RemoveColumn("PicklistID");
			oMultiArray.RemoveColumn("IsActive");

			try {
				oTest.Reset(oMultiArray);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *NonNumericPicklistID";
			oMultiArray = new MultiArray(GetMultiArrayString());
			oMultiArray.Value(2, "PicklistID", "NotANumber");

			try {
				oTest = new Picklists(oMultiArray);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *NonNumericPicklistID";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oMultiArray = new MultiArray(GetMultiArrayString());
			oMultiArray.Value(5, "PicklistID", "NotANumber");

			try {
				oTest.Reset(oMultiArray);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] *NonNumericPicklistIDs";
			oMultiArray = new MultiArray(GetMultiArrayString());
			oMultiArray.Value(0, "PicklistID", "NotANumber");
			oMultiArray.Value(7, "PicklistID", "");

			try {
				oTest = new Picklists(oMultiArray);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			sTestMessage = "Reset *NonNumericPicklistIDs";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oMultiArray = new MultiArray(GetMultiArrayString());
			oMultiArray.Value(1, "PicklistID", "-.");
			oMultiArray.Value(6, "PicklistID", ",");

			try {
				oTest.Reset(oMultiArray);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "[Constructor] ExtraColumns";
			oMultiArray = new MultiArray(GetMultiArrayString());
			oMultiArray.InsertColumn("MyAdditionalColumn1", new string[oMultiArray.RowCount], 0);
			oMultiArray.InsertColumn("MyAdditionalColumn2", new string[oMultiArray.RowCount], 5);
			oTest = new Picklists(oMultiArray);

			Assert.AreEqual(8, oTest.Data.ColumnCount, sTestMessage);
			Assert.AreEqual(9, oTest.Data.RowCount, sTestMessage);

			//##########
			sTestMessage = "Reset ExtraColumns";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oMultiArray = new MultiArray(GetMultiArrayString());
			oMultiArray.InsertColumn("MyAdditionalColumn3");
			oMultiArray.InsertColumn("MyAdditionalColumn4", new string[oMultiArray.RowCount], 1);

			oTest.Reset(oMultiArray);
			Assert.AreEqual(8, oTest.Data.ColumnCount, sTestMessage);
			Assert.AreEqual(9, oTest.Data.RowCount, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test StrictDecodes
		/// </summary>
		/// <remarks>
		/// bool StrictDecodes				ExpectedValue
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void StrictDecodes() {
			Picklists oTest;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue; ";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(false, oTest.StrictDecodes, sTestMessage + "DefaultValue");

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = true;
			Assert.AreEqual(true, oTest.StrictDecodes, sTestMessage + "ModifiedValue");
		}

		///############################################################
		/// <summary>
		/// Test GetData
		/// </summary>
		/// <remarks>
		/// PicklistCollectionHelper GetData	
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void GetData() {
//!
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
			Picklists oTest;
			MultiArray oData;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue; ";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oData = oTest.Data;

			Assert.AreEqual(oTest.Data.ColumnCount, oData.ColumnCount, sTestMessage);
			Assert.AreEqual(oTest.Data.RowCount, oData.RowCount, sTestMessage);
			Assert.AreEqual(oTest.Data.ToString(), oData.ToString(), sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue EnsureDeepCopy; ";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreNotSame(oTest, oData, sTestMessage);

			oData.RemoveColumn(0);
			Assert.AreNotEqual(oTest.Data.ColumnCount, oData.ColumnCount, sTestMessage);

			oData.RemoveRow(0);
			Assert.AreNotEqual(oTest.Data.RowCount, oData.RowCount, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test ColumnAssociationsPicklistName
		/// </summary>
		/// <remarks>
		/// string ColumnAssociationsPicklistName	ExpectedValue
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void ColumnAssociationsPicklistName() {
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue; ";

			Assert.AreEqual("_PicklistColumnAssociations", Picklists.ColumnAssociationsPicklistName, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test MetaDataPicklistName
		/// </summary>
		/// <remarks>
		/// string MetaDataPicklistName		ExpectedValue
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void MetaDataPicklistName() {
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue; ";

			Assert.AreEqual("_PicklistMetaData", Picklists.MetaDataPicklistName, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Exists(sPicklistName)
		/// </summary>
		/// <remarks>
		/// bool Exists(					n/a (primitive/simple non-nullable type, value is tested below)
		///		string sPicklistName		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, PicklistNotIsActive
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 27, 2010</LastUpdated>
		[TestMethod]
		public void Exists_PicklistName() {
			Picklists oTest;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sPicklistName Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(true, oTest.Exists("Titles"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			Assert.AreEqual(true, oTest.Exists("TITlEs"), sTestMessage);

			oTest.StrictDecodes = true;
			Assert.AreEqual(false, oTest.Exists("TITlEs"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(false, oTest.Exists("NotAPicklist"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(false, oTest.Exists(""), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];

			Assert.AreEqual(false, oTest.Exists(a_sNull[0]), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName PicklistNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(true, oTest.Exists("Boolean"), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Exists(sPicklistName, sDecodeValue)
		/// </summary>
		/// <remarks>
		/// bool Exists(					n/a (primitive/simple non-nullable type, value is tested below)
		///		string sPicklistName,		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, PicklistNotIsActive
		///		string sDecodeValue			Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, ItemNotIsActive
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 29, 2010</LastUpdated>
		[TestMethod]
		public void Exists_PicklistNameDecodeValue() {
			Picklists oTest;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sPicklistName Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(true, oTest.Exists("Titles", "Mrs"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

//!
			Assert.AreEqual(true, oTest.Exists("TiTLEs", "Mr"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(false, oTest.Exists("NotAPicklist", "0"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(false, oTest.Exists("", "Mrs"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];

			Assert.AreEqual(false, oTest.Exists(a_sNull[0], "Mr"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName PicklistNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(true, oTest.Exists("Boolean", "0"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(true, oTest.Exists("Titles", "Mr"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

//!
			Assert.AreEqual(true, oTest.Exists("Titles", "MRS"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(false, oTest.Exists("Titles", "NotATitle"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(false, oTest.Exists("Titles", ""), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];

			Assert.AreEqual(false, oTest.Exists("Titles", a_sNull[0]), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual(true, oTest.Exists("Titles", "Prof"), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Exists(sPicklistName, a_sDecodeValues)
		/// </summary>
		/// <remarks>
		/// bool Exists(					n/a (primitive/simple non-nullable type, value is tested below)
		///		string sPicklistName,		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, PicklistNotIsActive
		///		string[] a_sDecodeValues	LBound, UBound, WithinBounds, MultiValidValues, AllValidValues, InvalidValue,
		///									InvalidValues, MixedValidInvalidColumns, Null-String, MixedNullStringValidValues,
		///									Null, NullReference, ItemNotIsActive
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 29, 2010</LastUpdated>
		[TestMethod]
		public void Exists_PicklistNameDecodeValues() {
			Picklists oTest;
			string[] a_sValuesToDecode;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sPicklistName Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			Assert.AreEqual(true, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr", "Mrs" };

//!
//			Assert.AreEqual(true, oTest.Exists( "TiTLEs", a_sValuesToDecode ), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			Assert.AreEqual(false, oTest.Exists("NotAPicklist", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr", "Mrs" };

			Assert.AreEqual(false, oTest.Exists("", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };
			a_sNull = new string[1];

			Assert.AreEqual(false, oTest.Exists(a_sNull[0], a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName PicklistNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "0" };

			Assert.AreEqual(true, oTest.Exists("Boolean", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue LBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr" };

			Assert.AreEqual(true, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue UBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Prof" };

			Assert.AreEqual(true, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue WithinBounds";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs" };

			Assert.AreEqual(true, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue MultiValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			Assert.AreEqual(true, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue AllValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr", "Ms", "Prof", "Mrs" };

			Assert.AreEqual(true, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue InvalidValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "NotATitle" };

			Assert.AreEqual(false, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue InvalidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "AlsoNotATitle", "StillNotATitle" };

			Assert.AreEqual(false, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue MixedValidInvalidColumns";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "NotATitle", "Mr", "AnotherNotTitle" };

			Assert.AreEqual(false, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "" };

			Assert.AreEqual(false, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue MixedNullStringValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "", "Mr", "Prof" };

			Assert.AreEqual(false, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];
			a_sValuesToDecode = new string[] { a_sNull[0] };

			Assert.AreEqual(false, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue NullReference";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = null;

			Assert.AreEqual(false, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);

			//##########
			//########## 
			sTestMessage = "sDecodeValue ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] {"Mr", "Prof", "Mrs"};

			Assert.AreEqual(true, oTest.Exists("Titles", a_sValuesToDecode), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Exists(oPicklist, sDecodeValue, bStrictDecodes)
		/// </summary>
		/// <remarks>
		/// bool Exists(					n/a (primitive/simple non-nullable type, value is tested below)
		///		MultiArray oPicklist,		Valid, Empty, Null
		///		string sDecodeValue,		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, ItemNotIsActive
		///		bool bStrictDecodes			n/a (primitive/simple non-nullable type, value is tested above)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		[TestMethod]
		public void Exists_PicklistDecodeValue() {
			Picklists oTest;
			MultiArray oPicklist;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "oPicklist Valid";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual(true, Picklists.Exists(oPicklist, "Mrs", true), sTestMessage);
			Assert.AreEqual(true, Picklists.Exists(oPicklist, "Mrs", false), sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Empty";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			oPicklist.RemoveRow(3);
			oPicklist.RemoveRow(2);
			oPicklist.RemoveRow(1);
			oPicklist.RemoveRow(0);

			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, "Mr", true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, "Mr", false), sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Null";
			oPicklist = null;

			Assert.AreEqual(false, Picklists.Exists(oPicklist, "Mrs", true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, "Mrs", false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual(true, Picklists.Exists(oPicklist, "Mr", true), sTestMessage);
			Assert.AreEqual(true, Picklists.Exists(oPicklist, "Mr", false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual(false, Picklists.Exists(oPicklist, "MRS", true), sTestMessage);
			Assert.AreEqual(true, Picklists.Exists(oPicklist, "MRS", false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual(false, Picklists.Exists(oPicklist, "NotATitle", true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, "NotATitle", false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual(false, Picklists.Exists(oPicklist, "", true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, "", false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sNull = new string[1];

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sNull[0], true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sNull[0], false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual(false, Picklists.Exists(oPicklist, "Ms", true), sTestMessage);
			Assert.AreEqual(true, Picklists.Exists(oPicklist, "Ms", false), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Exists(oPicklist, a_sDecodeValues, bStrictDecodes)
		/// </summary>
		/// <remarks>
		/// bool Exists(					n/a (primitive/simple non-nullable type, value is tested below)
		///		MultiArray oPicklist,		Valid, Empty, Null
		///		string[] a_sDecodeValues	LBound, UBound, WithinBounds, MultiValidValues, AllValidValues, InvalidValue,
		///									InvalidValues, MixedValidInvalidColumns, Null-String, MixedNullStringValidValues,
		///									Null, NullReference, ItemNotIsActive, iNCORRECTcASE
		///		bool bStrictDecodes			n/a (primitive/simple non-nullable type, value is tested above)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		[TestMethod]
		public void Exists_PicklistDecodeValues() {
			Picklists oTest;
			MultiArray oPicklist;
			string[] a_sValuesToDecode;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "oPicklist Valid";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			Assert.AreEqual(true, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Empty";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			oPicklist.RemoveRow(3);
			oPicklist.RemoveRow(2);
			oPicklist.RemoveRow(1);
			oPicklist.RemoveRow(0);
			a_sValuesToDecode = new string[] { "Mr", "Mrs" };

			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Null";
			oPicklist = null;
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue LBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mr" };

			Assert.AreEqual(true, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue UBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Prof" };

			Assert.AreEqual(true, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue WithinBounds";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs" };

			Assert.AreEqual(true, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue MultiValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			Assert.AreEqual(true, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue AllValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mr", "Ms", "Prof", "Mrs" };

			Assert.AreEqual(true, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			sTestMessage = "sDecodeValue AllValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mr", "Prof", "Mrs" };

			Assert.AreEqual(true, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue InvalidValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "NotATitle" };

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue InvalidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "AlsoNotATitle", "StillNotATitle" };

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue MixedValidInvalidColumns";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs", "NotATitle", "Mr", "AnotherNotTitle" };

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "" };

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue MixedNullStringValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "", "Mr", "Prof" };

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sNull = new string[1];
			a_sValuesToDecode = new string[] { a_sNull[0] };

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue NullReference";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = null;

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			//########## 
			sTestMessage = "sDecodeValue ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] {"Mr", "Ms", "Prof"};

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(true, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);

			//##########
			//########## 
			sTestMessage = "sDecodeValue iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] {"mr", "MrS"};

			Assert.AreEqual(false, Picklists.Exists(oPicklist, a_sValuesToDecode, true), sTestMessage);
			Assert.AreEqual(true, Picklists.Exists(oPicklist, a_sValuesToDecode, false), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Decoder(sPicklistName, sDecodeValue)
		/// </summary>
		/// <remarks>
		/// string Decoder(					n/a (primitive/simple non-nullable type, value is tested below)
		///		string sPicklistName,		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, PicklistNotIsActive
		///		string sDecodeValue			Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, ItemNotIsActive
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 29, 2010</LastUpdated>
		[TestMethod]
		public void Decoder_PicklistNameDecodeValue() {
			Picklists oTest;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sPicklistName Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual("Misses", oTest.Decoder("Titles", "Mrs"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			Assert.AreEqual("Mister", oTest.Decoder("TiTLEs", "Mr"), sTestMessage);

			oTest.StrictDecodes = true;
			Assert.AreEqual("", oTest.Decoder("TiTLEs", "Mr"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			Assert.AreEqual("0", oTest.Decoder("NotAPicklist", "0"), sTestMessage);

			oTest.StrictDecodes = true;
			Assert.AreEqual("", oTest.Decoder("NotAPicklist", "0"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			Assert.AreEqual("Mrs", oTest.Decoder("", "Mrs"), sTestMessage);

			oTest.StrictDecodes = true;
			Assert.AreEqual("", oTest.Decoder("", "Mrs"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];

			oTest.StrictDecodes = false;
			Assert.AreEqual("Mr", oTest.Decoder(a_sNull[0], "Mr"), sTestMessage);

			oTest.StrictDecodes = true;
			Assert.AreEqual("", oTest.Decoder(a_sNull[0], "Mr"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName PicklistNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual("False", oTest.Decoder("Boolean", "0"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual("Mister", oTest.Decoder("Titles", "Mr"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			Assert.AreEqual("Misses", oTest.Decoder("Titles", "MRS"), sTestMessage);

			oTest.StrictDecodes = true;
			Assert.AreEqual("", oTest.Decoder("Titles", "MRS"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			Assert.AreEqual("NotATitle", oTest.Decoder("Titles", "NotATitle"), sTestMessage);

			oTest.StrictDecodes = true;
			Assert.AreEqual("", oTest.Decoder("Titles", "NotATitle"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual("", oTest.Decoder("Titles", ""), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];

			Assert.AreEqual("", oTest.Decoder("Titles", a_sNull[0]), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			Assert.AreEqual("Professor", oTest.Decoder("Titles", "Prof"), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Decoder(sPicklistName, a_sDecodeValues)
		/// </summary>
		/// <remarks>
		/// bool Decoder(					n/a (primitive/simple non-nullable type, value is tested below)
		///		string sPicklistName,		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, PicklistNotIsActive
		///		string[] a_sDecodeValues	LBound, UBound, WithinBounds, MultiValidValues, AllValidValues, InvalidValue,
		///									InvalidValues, MixedValidInvalidColumns, Null-String, MixedNullStringValidValues,
		///									Null, NullReference, ItemNotIsActive
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>January 29, 2010</LastUpdated>
		[TestMethod]
		public void Decoder_PicklistNameDecodeValues() {
			Picklists oTest;
			string[] a_sValuesToDecode;
			string[] a_sDecodedValues;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "sPicklistName Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("Misses", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mister", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr", "Mrs" };

			oTest.StrictDecodes = false;
			a_sDecodedValues = oTest.Decoder("TiTLEs", a_sValuesToDecode);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Misses", a_sDecodedValues[1], sTestMessage);

			oTest.StrictDecodes = true;
			a_sDecodedValues = oTest.Decoder("TiTLEs", a_sValuesToDecode);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			oTest.StrictDecodes = false;
			a_sDecodedValues = oTest.Decoder("NotAPicklist", a_sValuesToDecode);
			Assert.AreEqual("Mrs", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mr", a_sDecodedValues[1], sTestMessage);

			oTest.StrictDecodes = true;
			a_sDecodedValues = oTest.Decoder("NotAPicklist", a_sValuesToDecode);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr", "Mrs" };

			oTest.StrictDecodes = false;
			a_sDecodedValues = oTest.Decoder("", a_sValuesToDecode);
			Assert.AreEqual("Mr", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mrs", a_sDecodedValues[1], sTestMessage);

			oTest.StrictDecodes = true;
			a_sDecodedValues = oTest.Decoder("", a_sValuesToDecode);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };
			a_sNull = new string[1];

			oTest.StrictDecodes = false;
			a_sDecodedValues = oTest.Decoder(a_sNull[0], a_sValuesToDecode);
			Assert.AreEqual("Mrs", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mr", a_sDecodedValues[1], sTestMessage);

			oTest.StrictDecodes = true;
			a_sDecodedValues = oTest.Decoder(a_sNull[0], a_sValuesToDecode);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName PicklistNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "0" };

			oTest.StrictDecodes = false;
			a_sDecodedValues = oTest.Decoder("Boolean", a_sValuesToDecode);
			Assert.AreEqual("False", a_sDecodedValues[0], sTestMessage);

			oTest.StrictDecodes = true;
			a_sDecodedValues = oTest.Decoder("Boolean", a_sValuesToDecode);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue LBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr" };

			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue UBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Prof" };

			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("Professor", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue WithinBounds";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs" };

			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("Misses", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue MultiValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("Misses", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mister", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue AllValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr", "Ms", "Prof", "Mrs" };

			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Miss", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Professor", a_sDecodedValues[2], sTestMessage);
			Assert.AreEqual("Misses", a_sDecodedValues[3], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue InvalidValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "NotATitle" };

			oTest.StrictDecodes = false;
			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("NotATitle", a_sDecodedValues[0], sTestMessage);

			oTest.StrictDecodes = true;
			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue InvalidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "AlsoNotATitle", "StillNotATitle" };

			oTest.StrictDecodes = false;
			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("AlsoNotATitle", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("StillNotATitle", a_sDecodedValues[1], sTestMessage);

			oTest.StrictDecodes = true;
			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue MixedValidInvalidColumns";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "NotATitle", "Mr", "AnotherNotTitle" };

			oTest.StrictDecodes = false;
			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("Misses", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("NotATitle", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Mister", a_sDecodedValues[2], sTestMessage);
			Assert.AreEqual("AnotherNotTitle", a_sDecodedValues[3], sTestMessage);

			oTest.StrictDecodes = true;
			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("Misses", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Mister", a_sDecodedValues[2], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[3], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "" };

			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue MixedNullStringValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "", "Mr", "Prof" };

			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mister", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Professor", a_sDecodedValues[2], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];
			a_sValuesToDecode = new string[] { a_sNull[0] };

			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue NullReference";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = null;

			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual(0, a_sDecodedValues.Length, sTestMessage);

			//##########
			//########## 
			sTestMessage = "sDecodeValue ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] {"Mr", "Ms", "Mrs"};

			oTest.StrictDecodes = false;
			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Miss", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Misses", a_sDecodedValues[2], sTestMessage);

			oTest.StrictDecodes = true;
			a_sDecodedValues = oTest.Decoder("Titles", a_sValuesToDecode);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Misses", a_sDecodedValues[2], sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Decoder(oPicklist, sDecodeValue, bStrictDecodes)
		/// </summary>
		/// <remarks>
		/// string Decoder(					n/a (primitive/simple non-nullable type, value is tested below)
		///		MultiArray oPicklist,		Valid, Empty, Null
		///		string sDecodeValue,		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, ItemNotIsActive
		///		bool bStrictDecodes			n/a (primitive/simple non-nullable type, value is tested above)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		[TestMethod]
		public void Decoder_PicklistDecodeValue() {
			Picklists oTest;
			MultiArray oPicklist;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "oPicklist Valid";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual("Misses", Picklists.Decoder(oPicklist, "Mrs", false), sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Empty";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			oPicklist.RemoveRow(3);
			oPicklist.RemoveRow(2);
			oPicklist.RemoveRow(1);
			oPicklist.RemoveRow(0);

			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);
			Assert.AreEqual("Mr", Picklists.Decoder(oPicklist, "Mr", false), sTestMessage);
			Assert.AreEqual("", Picklists.Decoder(oPicklist, "Mr", true), sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Null";
			oPicklist = null;

			Assert.AreEqual("Mrs", Picklists.Decoder(oPicklist, "Mrs", false), sTestMessage);
			Assert.AreEqual("", Picklists.Decoder(oPicklist, "Mrs", true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual("Mister", Picklists.Decoder(oPicklist, "Mr", false), sTestMessage);
			Assert.AreEqual("Mister", Picklists.Decoder(oPicklist, "Mr", true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual("Misses", Picklists.Decoder(oPicklist, "MRS", false), sTestMessage);
			Assert.AreEqual("", Picklists.Decoder(oPicklist, "MRS", true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual("NotATitle", Picklists.Decoder(oPicklist, "NotATitle", false), sTestMessage);
			Assert.AreEqual("", Picklists.Decoder(oPicklist, "NotATitle", true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual("", Picklists.Decoder(oPicklist, "", false), sTestMessage);
			Assert.AreEqual("", Picklists.Decoder(oPicklist, "", true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sNull = new string[1];

			Assert.AreEqual("", Picklists.Decoder(oPicklist, a_sNull[0], false), sTestMessage);
			Assert.AreEqual("", Picklists.Decoder(oPicklist, a_sNull[0], true), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			Assert.AreEqual("Miss", Picklists.Decoder(oPicklist, "Ms", false), sTestMessage);
			Assert.AreEqual("", Picklists.Decoder(oPicklist, "Ms", true), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Decoder(oPicklist, a_sDecodeValues, bStrictDecodes)
		/// </summary>
		/// <remarks>
		/// bool Decoder(					n/a (primitive/simple non-nullable type, value is tested below)
		///		MultiArray oPicklist,		Valid, Empty, Null
		///		string[] a_sDecodeValues	LBound, UBound, WithinBounds, MultiValidValues, AllValidValues, InvalidValue,
		///									InvalidValues, MixedValidInvalidColumns, Null-String, MixedNullStringValidValues,
		///									Null, NullReference, ItemNotIsActive, iNCORRECTcASE
		///		bool bStrictDecodes			n/a (primitive/simple non-nullable type, value is tested above)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		[TestMethod]
		public void Decoder_PicklistDecodeValues() {
			Picklists oTest;
			MultiArray oPicklist;
			string[] a_sValuesToDecode;
			string[] a_sDecodedValues;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "oPicklist Valid";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Misses", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mister", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Empty";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			oPicklist.RemoveRow(3);
			oPicklist.RemoveRow(2);
			oPicklist.RemoveRow(1);
			oPicklist.RemoveRow(0);
			a_sValuesToDecode = new string[] { "Mr", "Mrs" };

			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Mr", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mrs", a_sDecodedValues[1], sTestMessage);

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Null";
			oPicklist = null;
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Mrs", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mr", a_sDecodedValues[1], sTestMessage);

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues LBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mr" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues UBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Prof" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Professor", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues WithinBounds";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Misses", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues MultiValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Misses", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mister", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues AllValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mr", "Ms", "Prof", "Mrs" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Miss", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Professor", a_sDecodedValues[2], sTestMessage);
			Assert.AreEqual("Misses", a_sDecodedValues[3], sTestMessage);

			//##########
			sTestMessage = "a_sDecodeValues AllValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mr", "Prof", "Mrs" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Professor", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Misses", a_sDecodedValues[2], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues InvalidValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "NotATitle" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("NotATitle", a_sDecodedValues[0], sTestMessage);

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues InvalidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "AlsoNotATitle", "StillNotATitle" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("AlsoNotATitle", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("StillNotATitle", a_sDecodedValues[1], sTestMessage);

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues MixedValidInvalidColumns";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs", "NotATitle", "Mr", "AnotherNotTitle" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Misses", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("NotATitle", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Mister", a_sDecodedValues[2], sTestMessage);
			Assert.AreEqual("AnotherNotTitle", a_sDecodedValues[3], sTestMessage);

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual("Misses", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Mister", a_sDecodedValues[2], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[3], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues MixedNullStringValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "", "Mr", "Prof" };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Mister", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Professor", a_sDecodedValues[2], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sNull = new string[1];
			a_sValuesToDecode = new string[] { a_sNull[0] };

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues NullReference";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = null;

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(0, a_sDecodedValues.Length, sTestMessage);

			//##########
			//########## 
			sTestMessage = "a_sDecodeValues ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] {"Mr", "Ms", "Prof"};

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Miss", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Professor", a_sDecodedValues[2], sTestMessage);

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Professor", a_sDecodedValues[2], sTestMessage);

			//##########
			//########## 
			sTestMessage = "a_sDecodeValues iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] {"mr", "Mrs", "pRoF"};

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual("Mister", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Misses", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("Professor", a_sDecodedValues[2], sTestMessage);

			a_sDecodedValues = Picklists.Decoder(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual("", a_sDecodedValues[0], sTestMessage);
			Assert.AreEqual("Misses", a_sDecodedValues[1], sTestMessage);
			Assert.AreEqual("", a_sDecodedValues[2], sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Items(sPicklistName, sDecodeValue)
		/// </summary>
		/// <remarks>
		/// MultiArray Items(				ExpectedValue, AdditionalColumns
		///		string sPicklistName,		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, PicklistNotIsActive
		///		string sDecodeValue			Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, ItemNotIsActive
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		[TestMethod]
		public void Items_PicklistNameDecodeValue() {
			Picklists oTest;
			MultiArray oItems;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oItems = oTest.Items("Titles", "Mr");
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue AdditionalColumns";
			oItems = new MultiArray(GetMultiArrayString());
			oItems.InsertColumn("NewColumn");
			oItems.Value(3, "NewColumn", "new1");
			oTest = new Picklists(oItems);
		  //oItems = null;

			oItems = oTest.Items("Titles", "Mr");
			Assert.AreEqual(7, oItems.ColumnCount, sTestMessage);
			Assert.AreEqual("new1", oItems.Value(0, "NewColumn"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oItems = oTest.Items("Titles", "Mrs");
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			oItems = oTest.Items("TiTLEs", "Mr");
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("TiTLEs", "Mr");
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oItems = oTest.Items("NotAPicklist", "Mr");
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oItems = oTest.Items("", "Mr");
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];

			oItems = oTest.Items(a_sNull[0], "Mr");
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName PicklistNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			oItems = oTest.Items("Boolean", "0");
			Assert.AreEqual(string.Join(",", GetRowAsArray(9)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("Boolean", "0");
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oItems = oTest.Items("Titles", "Mr");
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			oItems = oTest.Items("Titles", "MRS");
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("Titles", "MRS");
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oItems = oTest.Items("Titles", "NotATitle");
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oItems = oTest.Items("Titles", "");
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];

			oItems = oTest.Items("Titles", a_sNull[0]);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			oItems = oTest.Items("Titles", "Ms");
			Assert.AreEqual(string.Join(",", GetRowAsArray(6)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("Titles", "Ms");
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

/*
Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oTest.Items("Titles", "Mr").RowAsArray(0)), sTestMessage);
Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oTest.Items("Titles", "Mrs").RowAsArray(0)), sTestMessage);
*/
		}

		///############################################################
		/// <summary>
		/// Test Items(sPicklistName, a_sDecodeValues)
		/// </summary>
		/// <remarks>
		/// MultiArray Items(				ExpectedValue, AdditionalColumns
		///		string sPicklistName,		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, PicklistNotIsActive
		///		string[] a_sDecodeValues	LBound, UBound, WithinBounds, MultiValidValues, AllValidValues, InvalidValue,
		///									InvalidValues, MixedValidInvalidColumns, Null-String, MixedNullStringValidValues,
		///									Null, NullReference, ItemNotIsActive, iNCORRECTcASE
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		[TestMethod]
		public void Items_PicklistNameDecodeValues() {
			Picklists oTest;
			MultiArray oItems;
			string[] a_sValuesToDecode;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Titles", "Boolean" };

			oItems = oTest.Items(Picklists.MetaDataPicklistName, a_sValuesToDecode);
			Assert.AreEqual(2, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(3)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue AdditionalColumns";
			oItems = new MultiArray(GetMultiArrayString());
			oItems.InsertColumn("NewColumn");
			oItems.Value(3, "NewColumn", "new1");
			oTest = new Picklists(oItems);
		  //oItems = null;
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(2, oItems.RowCount, sTestMessage);
			Assert.AreEqual(7, oItems.ColumnCount, sTestMessage);
			Assert.AreEqual("new1", oItems.Value(1, "NewColumn"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr", "Mrs", "Prof" };

			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(3, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs" };

			oTest.StrictDecodes = false;
			oItems = oTest.Items("TiTLEs", a_sValuesToDecode);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("TiTLEs", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			oItems = oTest.Items("NotAPicklist", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr", "Mrs" };

			oItems = oTest.Items("", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			oItems = oTest.Items(a_sNull[0], a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName PicklistNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "0", "1" };

			oTest.StrictDecodes = false;
			oItems = oTest.Items("Boolean", a_sValuesToDecode);
			Assert.AreEqual(string.Join(",", GetRowAsArray(9)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(2, oItems.RowCount, sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("Boolean", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues LBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr" };

			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues UBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Prof" };

			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues WithinBounds";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs" };

			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues MultiValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(2, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues AllValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr", "Ms", "Prof", "Mrs" };

			oTest.StrictDecodes = false;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(4, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(6)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(2)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(3)), sTestMessage);

			//##########
			sTestMessage = "a_sDecodeValues AllValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mr", "Prof", "Mrs" };

			oTest.StrictDecodes = true;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(3, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(2)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues InvalidValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "NotATitle" };

			oTest.StrictDecodes = false;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues InvalidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "AlsoNotATitle", "StillNotATitle" };

			oTest.StrictDecodes = false;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues MixedValidInvalidColumns";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "Mrs", "NotATitle", "Mr", "AnotherNotTitle" };

			oTest.StrictDecodes = false;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(2, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(2, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "" };

			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues MixedNullStringValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] { "", "Mr", "Prof" };

			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];
			a_sValuesToDecode = new string[] { a_sNull[0] };

			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues NullReference";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = null;

			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//########## 
			sTestMessage = "a_sDecodeValues ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] {"Mr", "Ms", "Prof"};

			oTest.StrictDecodes = false;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(3, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(6)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(2)), sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(2, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//########## 
			sTestMessage = "a_sDecodeValues iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sValuesToDecode = new string[] {"mr", "Mrs", "pRoF"};

			oTest.StrictDecodes = false;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(3, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(2)), sTestMessage);

			oTest.StrictDecodes = true;
			oItems = oTest.Items("Titles", a_sValuesToDecode);
			Assert.AreEqual(1, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Items(oPicklist, sDecodeValue, bStrictDecodes)
		/// </summary>
		/// <remarks>
		/// MultiArray Items(				ExpectedValue, AdditionalColumns
		///		MultiArray oPicklist,		Valid, Empty, Null
		///		string sDecodeValue,		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, ItemNotIsActive
		///		bool bStrictDecodes			n/a (primitive/simple non-nullable type, value is tested above)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		[TestMethod]
		public void Items_PicklistDecodeValue() {
			Picklists oTest;
			MultiArray oPicklist;
			MultiArray oItems;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			oItems = Picklists.Items(oPicklist, "Mr", false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			oItems = Picklists.Items(oPicklist, "Mr", true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue AdditionalColumns";
			oItems = new MultiArray(GetMultiArrayString());
			oItems.InsertColumn("NewColumn");
			oItems.Value(3, "NewColumn", "new1");
			oTest = new Picklists(oItems);
			oPicklist = oTest.Picklist("Titles");
		  //oItems = null;

			oItems = Picklists.Items(oPicklist, "Mr", true);
			Assert.AreEqual(7, oItems.ColumnCount, sTestMessage);
			Assert.AreEqual("new1", oItems.Value(0, "NewColumn"), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)) + ",new1", string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Valid";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			oItems = Picklists.Items(oPicklist, "Mr", false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Empty";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			oPicklist.RemoveRow(3);
			oPicklist.RemoveRow(2);
			oPicklist.RemoveRow(1);
			oPicklist.RemoveRow(0);

			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, "Mr", false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, "Mr", true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Null";
			oPicklist = null;

			oItems = Picklists.Items(oPicklist, "Mrs", false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, "Mrs", true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			oItems = Picklists.Items(oPicklist, "Mr", false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			oItems = Picklists.Items(oPicklist, "Mr", true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			oItems = Picklists.Items(oPicklist, "MrS", false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			oItems = Picklists.Items(oPicklist, "mrs", true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			oItems = Picklists.Items(oPicklist, "NotATitle", false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, "NotATitle", true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			oItems = Picklists.Items(oPicklist, "", false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, "", true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sNull = new string[1];

			oItems = Picklists.Items(oPicklist, a_sNull[0], false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sNull[0], true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sDecodeValue ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");

			oItems = Picklists.Items(oPicklist, "Ms", false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(6)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			oItems = Picklists.Items(oPicklist, "Ms", true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Items(oPicklist, a_sDecodeValues, bStrictDecodes)
		/// </summary>
		/// <remarks>
		/// MultiArray Items(				ExpectedValue, AdditionalColumns
		///		MultiArray oPicklist,		Valid, Empty, Null
		///		string[] a_sDecodeValues	LBound, UBound, WithinBounds, MultiValidValues, AllValidValues, InvalidValue,
		///									InvalidValues, MixedValidInvalidColumns, Null-String, MixedNullStringValidValues,
		///									Null, NullReference, ItemNotIsActive, iNCORRECTcASE
		///		bool bStrictDecodes			n/a (primitive/simple non-nullable type, value is tested above)
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		[TestMethod]
		public void Items_PicklistDecodeValues() {
			Picklists oTest;
			MultiArray oPicklist;
			MultiArray oItems;
			string[] a_sValuesToDecode;
			string[] a_sDecodedValues;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(2, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(2, oItems.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue AdditionalColumns";
			oItems = new MultiArray(GetMultiArrayString());
			oItems.InsertColumn("NewColumn");
			oItems.Value(3, "NewColumn", "new1");
			oItems.Value(4, "NewColumn", "new2");
			oTest = new Picklists(oItems);
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mr", "Mrs" };
		  //oItems = null;

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(7, oItems.ColumnCount, sTestMessage);
			Assert.AreEqual("new1", oItems.Value(0, "NewColumn"), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)) + ",new1", string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)) + ",new2", string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Valid";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Empty";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			oPicklist.RemoveRow(3);
			oPicklist.RemoveRow(2);
			oPicklist.RemoveRow(1);
			oPicklist.RemoveRow(0);
			a_sValuesToDecode = new string[] { "Mr", "Mrs" };

			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "oPicklist Null";
			oPicklist = null;
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues LBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mr" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues UBound";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Prof" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues WithinBounds";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues MultiValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs", "Mr" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues AllValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mr", "Ms", "Prof", "Mrs" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(6)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(2)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(3)), sTestMessage);

			//##########
			sTestMessage = "a_sDecodeValues AllValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mr", "Prof", "Mrs" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(2)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues InvalidValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "NotATitle" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues InvalidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "AlsoNotATitle", "StillNotATitle" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues MixedValidInvalidColumns";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "Mrs", "NotATitle", "Mr", "AnotherNotTitle" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues MixedNullStringValidValues";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] { "", "Mr", "Prof" };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sNull = new string[1];
			a_sValuesToDecode = new string[] { a_sNull[0] };

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDecodeValues NullReference";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = null;

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(0, oItems.RowCount, sTestMessage);

			//##########
			//########## 
			sTestMessage = "a_sDecodeValues ItemNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] {"Mr", "Ms", "Prof"};

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(6)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(2)), sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);

			//##########
			//########## 
			sTestMessage = "a_sDecodeValues iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			oPicklist = oTest.Picklist("Titles");
			a_sValuesToDecode = new string[] {"mr", "Mrs", "pRoF"};

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, false);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(1)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oItems.RowAsArray(2)), sTestMessage);

			oItems = Picklists.Items(oPicklist, a_sValuesToDecode, true);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oItems.RowAsArray(0)), sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Picklist(sPicklistName)
		/// </summary>
		/// <remarks>
		/// MultiArray Picklist(			ExpectedValue, AdditionalColumns
		///		string sPicklistName		Reconized, iNCORRECTcASE, Unreconized, Null-String, Null, PicklistNotIsActive
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 2, 2010</LastUpdated>
		[TestMethod]
		public void Picklist_PicklistName() {
			Picklists oTest;
			MultiArray oPicklist;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue ExpectedValue";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oPicklist = oTest.Picklist("Titles");
			Assert.AreEqual(4, oPicklist.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oPicklist.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oPicklist.RowAsArray(1)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(6)), string.Join(",", oPicklist.RowAsArray(2)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oPicklist.RowAsArray(3)), sTestMessage);

			//##########
			//##########
			sTestMessage = "ReturnValue AdditionalColumns";
			oPicklist = new MultiArray(GetMultiArrayString());
			oPicklist.InsertColumn("NewColumn");
			oPicklist.Value(3, "NewColumn", "new1");
			oTest = new Picklists(oPicklist);
		  //oPicklist = null;

			oPicklist = oTest.Picklist("Titles");
			Assert.AreEqual(4, oPicklist.RowCount, sTestMessage);
			Assert.AreEqual("new1", oPicklist.Value(0, "NewColumn"), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Reconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oPicklist = oTest.Picklist("Titles");
			Assert.AreEqual(4, oPicklist.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(4)), string.Join(",", oPicklist.RowAsArray(0)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(5)), string.Join(",", oPicklist.RowAsArray(1)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(6)), string.Join(",", oPicklist.RowAsArray(2)), sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(7)), string.Join(",", oPicklist.RowAsArray(3)), sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName iNCORRECTcASE";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			oPicklist = oTest.Picklist("TiTLEs");
			Assert.AreEqual(4, oPicklist.RowCount, sTestMessage);

			oTest.StrictDecodes = true;
			oPicklist = oTest.Picklist("TiTLEs");
			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Unreconized";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oPicklist = oTest.Picklist("NotAPicklist");
			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null-String";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oPicklist = oTest.Picklist("");
			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName Null";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));
			a_sNull = new string[1];

			oPicklist = oTest.Picklist(a_sNull[0]);
			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPicklistName PicklistNotIsActive";
			oTest = new Picklists(new MultiArray(GetMultiArrayString()));

			oTest.StrictDecodes = false;
			oPicklist = oTest.Picklist("Boolean");
			Assert.AreEqual(2, oPicklist.RowCount, sTestMessage);
			Assert.AreEqual(string.Join(",", GetRowAsArray(8)), string.Join(",", oPicklist.RowAsArray(0)), sTestMessage);

			oTest.StrictDecodes = true;
			oPicklist = oTest.Picklist("Boolean");
			Assert.AreEqual(0, oPicklist.RowCount, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test Picklist(a_sDataValues, a_sDescriptionValues)
		/// </summary>
		/// <remarks>
		/// MultiArray Picklist(				DeepCopied
		///		string[] a_sDataValues,			Valid, *InvalidLength, *Null, *Empty
		///		string[] a_sDescriptionValues	Valid, *InvalidLength, *Null, *Empty
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 3, 2010</LastUpdated>
		[TestMethod]
		public void Picklist_DataValuesDescriptionValues() {
			MultiArray oPicklist;
			string[] a_sDescriptionValues;
			string[] a_sDataValues;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "ReturnValue DeepCopied";
			a_sDataValues = new string[] { "data1", "data2", "data3" };
			a_sDescriptionValues = new string[] { "description1", "description2", "description3" };

			oPicklist = Picklists.Picklist(a_sDataValues, a_sDescriptionValues);
			Assert.AreEqual(3, oPicklist.RowCount, sTestMessage);

			a_sDataValues[0] = "newdata1";
			Assert.AreEqual("data1", oPicklist.Value(0, "Data"), sTestMessage);

			a_sDescriptionValues[2] = "newdescription3";
			Assert.AreEqual("description3", oPicklist.Value(2, "Description"), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDataValues Valid";
			a_sDataValues = new string[] { "data1", "data2", "data3" };
			a_sDescriptionValues = new string[] { "description1", "description2", "description3" };

			oPicklist = Picklists.Picklist(a_sDataValues, a_sDescriptionValues);
			Assert.AreEqual(3, oPicklist.RowCount, sTestMessage);
			Assert.AreEqual("data1", oPicklist.Value(0, "Data"), sTestMessage);
			Assert.AreEqual("description3", oPicklist.Value(2, "Description"), sTestMessage);
			Assert.AreEqual("data2", oPicklist.Value(1, "Data"), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDataValues *InvalidLength";
			a_sDataValues = new string[] { "data1", "data2", "data3", "data4" };
			a_sDescriptionValues = new string[] { "description1", "description2", "description3" };

			try {
				oPicklist = Picklists.Picklist(a_sDataValues, a_sDescriptionValues);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sDataValues *Null";
			a_sDataValues = null;
			a_sDescriptionValues = new string[] { "description1", "description2", "description3" };

			try {
				oPicklist = Picklists.Picklist(a_sDataValues, a_sDescriptionValues);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sDataValues *Empty";
			a_sDataValues = new string[0];
			a_sDescriptionValues = new string[] { "description1", "description2", "description3" };

			try {
				oPicklist = Picklists.Picklist(a_sDataValues, a_sDescriptionValues);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sDescriptionValues Valid";
			a_sDataValues = new string[] { "data1" };
			a_sDescriptionValues = new string[] { "description1" };

			oPicklist = Picklists.Picklist(a_sDataValues, a_sDescriptionValues);
			Assert.AreEqual(1, oPicklist.RowCount, sTestMessage);
			Assert.AreEqual("data1", oPicklist.Value(0, "Data"), sTestMessage);
			Assert.AreEqual("description1", oPicklist.Value(0, "Description"), sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sDescriptionValues *InvalidLength";
			a_sDataValues = new string[] { "data1", "data2", "data3" };
			a_sDescriptionValues = new string[] { "description1" };

			try {
				oPicklist = Picklists.Picklist(a_sDataValues, a_sDescriptionValues);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sDescriptionValues *Null";
			a_sDataValues = new string[] { "data1", "data2", "data3" };
			a_sDescriptionValues = null;

			try {
				oPicklist = Picklists.Picklist(a_sDataValues, a_sDescriptionValues);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

			//##########
			//##########
			sTestMessage = "a_sDescriptionValues *Empty";
			a_sDataValues = new string[] { "data1", "data2", "data3" };
			a_sDescriptionValues = new string[0];

			try {
				oPicklist = Picklists.Picklist(a_sDataValues, a_sDescriptionValues);
				Assert.Fail(sTestMessage);
			}
			catch (CnException oEx) {}

		}


	} //# public class Picklists_Test


} //# namespace Cn.UnitTests.Tests.Data
