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


namespace Cn.UnitTests.Tests.Data {

	///########################################################################################################################
	/// <summary>
	/// Unit Test collection for the Tools
	/// </summary>
	///########################################################################################################################
	/// <LastFullCodeReview></LastFullCodeReview>
	[TestClass]
	public class Tools_Tests {
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


		//##########################################################################################
		//# Test Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Test the Push(a_sArray, sValue)
		/// </summary>
		/// <remarks>
		/// string[] Push(					n/a (value is tested below)
		///		string[] a_sArray,			Null, Empty, SingleValue, MultiValue
		///		string sValue				Valid, Null-String, Null
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		[TestMethod]
		public void Push() {
			string[] a_sReturn;
			string[] a_sInput;
			string[] a_sNull;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "a_sArray Null";
			a_sInput = null;

			a_sReturn = Cn.Data.Tools.Push(a_sInput, "new1");
			Assert.AreEqual(1, a_sReturn.Length, sTestMessage);
			Assert.AreEqual("new1", a_sReturn[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sArray Empty";
			a_sInput = new string[0];

			a_sReturn = Cn.Data.Tools.Push(a_sInput, "new2");
			Assert.AreEqual(1, a_sReturn.Length, sTestMessage);
			Assert.AreEqual("new2", a_sReturn[0], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sArray SingleValue";
			a_sInput = new string[] { "value1" };

			a_sReturn = Cn.Data.Tools.Push(a_sInput, "new3");
			Assert.AreEqual(2, a_sReturn.Length, sTestMessage);
			Assert.AreEqual("new3", a_sReturn[1], sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sArray MultiValue";
			a_sInput = new string[] { "value1", "value2" };

			a_sReturn = Cn.Data.Tools.Push(a_sInput, "new4");
			Assert.AreEqual(3, a_sReturn.Length, sTestMessage);
			Assert.AreEqual("new4", a_sReturn[2], sTestMessage);

			//##########
			//##########
			sTestMessage = "sValue Valid";
			a_sInput = new string[] { "value1", "value2", "value3" };

			a_sReturn = Cn.Data.Tools.Push(a_sInput, "new5");
			Assert.AreEqual(4, a_sReturn.Length, sTestMessage);
			Assert.AreEqual("new5", a_sReturn[3], sTestMessage);

			//##########
			//##########
			sTestMessage = "sValue Null-String";
			a_sInput = new string[] { "value1", "value2", "value3", "value4" };

			a_sReturn = Cn.Data.Tools.Push(a_sInput, "");
			Assert.AreEqual(5, a_sReturn.Length, sTestMessage);
			Assert.AreEqual("", a_sReturn[4], sTestMessage);

			//##########
			//##########
			sTestMessage = "sValue Null-String";
			a_sInput = new string[] { "value1", "value2", "value3", "value4", "value5" };
			a_sNull = new string[1];

			a_sReturn = Cn.Data.Tools.Push(a_sInput, a_sNull[0]);
			Assert.AreEqual(6, a_sReturn.Length, sTestMessage);
			Assert.AreEqual(null, a_sReturn[5], sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test the Pop(a_sArray)
		/// </summary>
		/// <remarks>
		/// string[] Pop(					n/a (value is tested below)
		///		string[] a_sArray			Null, Empty, SingleValue, MultiValue
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		[TestMethod]
		public void Pop() {
			string[] a_sReturn;
			string[] a_sInput;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "a_sArray Null";
			a_sInput = null;

			a_sReturn = Cn.Data.Tools.Pop(a_sInput);
			Assert.AreEqual(0, a_sReturn.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sArray Empty";
			a_sInput = new string[0];

			a_sReturn = Cn.Data.Tools.Pop(a_sInput);
			Assert.AreEqual(0, a_sReturn.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sArray SingleValue";
			a_sInput = new string[] { "value1" };

			a_sReturn = Cn.Data.Tools.Pop(a_sInput);
			Assert.AreEqual(0, a_sReturn.Length, sTestMessage);

			//##########
			//##########
			sTestMessage = "a_sArray MultiValue";
			a_sInput = new string[] { "value1", "value2" };

			a_sReturn = Cn.Data.Tools.Pop(a_sInput);
			Assert.AreEqual(1, a_sReturn.Length, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test the LPad(oValue, sPadCharacter, iLength)
		/// </summary>
		/// <remarks>
		/// string[] LPad(					n/a (value is tested below)
		///		object oValue,				Null, Null-String, Object, String
		///		string sPadCharacter,		Null, Null-String, SingleChar, MultiChar
		///		int iLength					Negetive, Zero, Positive, LessThenValueLength
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		[TestMethod]
		public void LPad() {
			object oValue;
			string[] a_sNull;
			string sPadCharacter;
			string sValue;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "oValue Null";
			oValue = null;
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.LPad(oValue, sPadCharacter, 2);
			Assert.AreEqual(2, sValue.Length, sTestMessage);
			Assert.AreEqual("__", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "oValue Null-String";
			sValue = "";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.LPad(sValue, sPadCharacter, 2);
			Assert.AreEqual(2, sValue.Length, sTestMessage);
			Assert.AreEqual("__", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "oValue Object";
			oValue = new Cn.Collections.MultiArray("Column1,Column2".Split(','));
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.LPad(oValue, sPadCharacter, 17);
			Assert.AreEqual(17, sValue.Length, sTestMessage);
			Assert.AreEqual("__Column1" + Configuration.Settings.SecondaryDelimiter + "Column2", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "oValue String";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.LPad(sValue, sPadCharacter, 8);
			Assert.AreEqual(8, sValue.Length, sTestMessage);
			Assert.AreEqual("__123456", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPadCharacter Null";
			sValue = "123456";
			a_sNull = new string[1];

			sValue = Cn.Data.Tools.LPad(sValue, a_sNull[0], 8);
			Assert.AreEqual(8, sValue.Length, sTestMessage);
			Assert.AreEqual("  123456", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPadCharacter Null-String";
			sValue = "123456";
			sPadCharacter = "";

			sValue = Cn.Data.Tools.LPad(sValue, sPadCharacter, 8);
			Assert.AreEqual(8, sValue.Length, sTestMessage);
			Assert.AreEqual("  123456", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPadCharacter SingleChar";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.LPad(sValue, sPadCharacter, 8);
			Assert.AreEqual(8, sValue.Length, sTestMessage);
			Assert.AreEqual("__123456", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPadCharacter MultiChar";
			sValue = "123456";
			sPadCharacter = "abc";

			sValue = Cn.Data.Tools.LPad(sValue, sPadCharacter, 8);
			Assert.AreEqual(8, sValue.Length, sTestMessage);
			Assert.AreEqual("bc123456", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "iLength Negetive";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.LPad(sValue, sPadCharacter, -1);
			Assert.AreEqual(6, sValue.Length, sTestMessage);
			Assert.AreEqual("123456", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "iLength Zero";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.LPad(sValue, sPadCharacter, 0);
			Assert.AreEqual(6, sValue.Length, sTestMessage);
			Assert.AreEqual("123456", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "iLength Positive";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.LPad(sValue, sPadCharacter, 7);
			Assert.AreEqual(7, sValue.Length, sTestMessage);
			Assert.AreEqual("_123456", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "iLength LessThenValueLength";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.LPad(sValue, sPadCharacter, 1);
			Assert.AreEqual(6, sValue.Length, sTestMessage);
			Assert.AreEqual("123456", sValue, sTestMessage);
		}

		///############################################################
		/// <summary>
		/// Test the RPad(oValue, sPadCharacter, iLength)
		/// </summary>
		/// <remarks>
		/// string[] RPad(					n/a (value is tested below)
		///		object oValue,				Null, Null-String, Object, String
		///		string sPadCharacter,		Null, Null-String, SingleChar, MultiChar
		///		int iLength					Negetive, Zero, Positive, LessThenValueLength
		/// )
		/// </remarks>
		///############################################################
		/// <LastUpdated>February 5, 2010</LastUpdated>
		[TestMethod]
		public void RPad() {
			object oValue;
			string[] a_sNull;
			string sPadCharacter;
			string sValue;
			string sTestMessage;

			//##########
			//##########
			sTestMessage = "oValue Null";
			oValue = null;
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.RPad(oValue, sPadCharacter, 2);
			Assert.AreEqual(2, sValue.Length, sTestMessage);
			Assert.AreEqual("__", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "oValue Null-String";
			sValue = "";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.RPad(sValue, sPadCharacter, 2);
			Assert.AreEqual(2, sValue.Length, sTestMessage);
			Assert.AreEqual("__", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "oValue Object";
			oValue = new Cn.Collections.MultiArray("Column1,Column2".Split(','));
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.RPad(oValue, sPadCharacter, 17);
			Assert.AreEqual(17, sValue.Length, sTestMessage);
			Assert.AreEqual("Column1" + Configuration.Settings.SecondaryDelimiter + "Column2__", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "oValue String";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.RPad(sValue, sPadCharacter, 8);
			Assert.AreEqual(8, sValue.Length, sTestMessage);
			Assert.AreEqual("123456__", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPadCharacter Null";
			sValue = "123456";
			a_sNull = new string[1];

			sValue = Cn.Data.Tools.RPad(sValue, a_sNull[0], 8);
			Assert.AreEqual(8, sValue.Length, sTestMessage);
			Assert.AreEqual("123456  ", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPadCharacter Null-String";
			sValue = "123456";
			sPadCharacter = "";

			sValue = Cn.Data.Tools.RPad(sValue, sPadCharacter, 8);
			Assert.AreEqual(8, sValue.Length, sTestMessage);
			Assert.AreEqual("123456  ", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPadCharacter SingleChar";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.RPad(sValue, sPadCharacter, 8);
			Assert.AreEqual(8, sValue.Length, sTestMessage);
			Assert.AreEqual("123456__", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "sPadCharacter MultiChar";
			sValue = "123456";
			sPadCharacter = "abc";

			sValue = Cn.Data.Tools.RPad(sValue, sPadCharacter, 8);
			Assert.AreEqual(8, sValue.Length, sTestMessage);
			Assert.AreEqual("123456ab", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "iLength Negetive";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.RPad(sValue, sPadCharacter, -1);
			Assert.AreEqual(6, sValue.Length, sTestMessage);
			Assert.AreEqual("123456", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "iLength Zero";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.RPad(sValue, sPadCharacter, 0);
			Assert.AreEqual(6, sValue.Length, sTestMessage);
			Assert.AreEqual("123456", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "iLength Positive";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.RPad(sValue, sPadCharacter, 7);
			Assert.AreEqual(7, sValue.Length, sTestMessage);
			Assert.AreEqual("123456_", sValue, sTestMessage);

			//##########
			//##########
			sTestMessage = "iLength LessThenValueLength";
			sValue = "123456";
			sPadCharacter = "_";

			sValue = Cn.Data.Tools.RPad(sValue, sPadCharacter, 1);
			Assert.AreEqual(6, sValue.Length, sTestMessage);
			Assert.AreEqual("123456", sValue, sTestMessage);
		}


		public void Chr() {
			
		}
		


	} //# public class Tools_Tests

 
} //# namespace Cn.UnitTests.Tests.Data
