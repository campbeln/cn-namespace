/*

///########################################################################################################################
/// <summary>
/// Simple class to demostrate an approach to Unit Testing
/// </summary>
///########################################################################################################################
public class StringArray {
	//#### Declare the required global variables
	private string[] ga_sData;

	///############################################################
	/// <summary>
	/// Class constructor
	/// </summary>
	///############################################################
	public StringArray(int iLength) {
		if (iLength > 0) {
			ga_sData = new string[iLength];
		}
		else {
			throw new Exception("Initial length must be a positive integer.");
		}
	}

	///############################################################
	/// <summary>
	/// Gets the Length of the StringArray
	/// </summary>
	///############################################################
	public int Length {
		get { return ga_sData.Length; }
	}

	///############################################################
	/// <summary>
	/// Gets the value at the referenced index
	/// </summary>
	///############################################################
	public string Get(int iIndex) {
		//#### Make note of the intentional bug injected into the if statement that allows
		//####     for the passed iIndex to be outside the bounds of ga_sData
	//! if (iIndex >= 0 && iIndex < ga_sData.Length) {
		if (iIndex >= 0 && iIndex <= ga_sData.Length) {
			return ga_sData[iIndex];
		}
		else {
			throw new Exception("Index must be within the bounds of the array.");
		}
	}

	///############################################################
	/// <summary>
	/// Sets the value at the referenced index
	/// </summary>
	///############################################################
	public void Set(int iIndex, string sValue) {
		if (iIndex >= 0 && iIndex < ga_sData.Length) {
			if (! string.IsNullOrEmpty(sValue)) {
				ga_sData[iIndex] = sValue;
			}
			else {
				throw new Exception("Value must be a valid string reference with a length greater then 0 characters.");
			}
		}
		else {
			throw new Exception("Index must be within the bounds of the array.");
		}
	}


	///########################################################################################################################
	/// <summary>
	/// Simple Exception class to demostrate an approach to Unit Testing
	/// </summary>
	///########################################################################################################################
	public class Exception : System.Exception {

		///############################################################
		/// <summary>
		/// Class constructor
		/// </summary>
		///############################################################
		public Exception(string message) : base(message) {
		}

	} //# public class Exception

} //# public class StringArray



// As defined within the Unit Testing Project...




///########################################################################################################################
/// <summary>
/// Unit Test collection for the StringArray
/// </summary>
///########################################################################################################################
[TestClass]
public class StringArray_Tests {
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
	//# Unit Tests
	//##########################################################################################
	///############################################################
	/// <summary>
	/// Test the Constructor(iLength)
	/// </summary>
	/// <remarks>
	/// StringArray(
	///		int iLength				Valid, Invalid
	///	)
	/// </remarks>
	///############################################################
	[TestMethod]
	public void Constructor() {
		StringArray oTest;
		string sTestMessage;

		//##########
		//##########
		sTestMessage = "[Constructor] Valid";

		oTest = new StringArray(1);
		Assert.AreEqual(1, oTest.Length, sTestMessage);

		oTest = new StringArray(10);
		Assert.AreEqual(10, oTest.Length, sTestMessage);

		oTest = new StringArray(10000);
		Assert.AreEqual(10000, oTest.Length, sTestMessage);

		//##########
		//##########
		sTestMessage = "[Constructor] Invalid";

		try {
			oTest = new StringArray(0);
			Assert.Fail(sTestMessage);
		}
		catch (StringArray.Exception oEx) {}

		try {
			oTest = new StringArray(-1);
			Assert.Fail(sTestMessage);
		}
		catch (StringArray.Exception oEx) {}
	}

	///############################################################
	/// <summary>
	/// Test the Length
	/// </summary>
	/// <remarks>
	/// int Length					ExpectedValue
	/// </remarks>
	///############################################################
	[TestMethod]
	public void Length() {
		StringArray oTest;
		string sTestMessage;

		//##########
		//##########
		sTestMessage = "ReturnValue ExpectedValue";

		oTest = new StringArray(7);
		Assert.AreEqual(7, oTest.Length, sTestMessage);

		oTest = new StringArray(22);
		Assert.AreEqual(22, oTest.Length, sTestMessage);

		oTest = new StringArray(1492);
		Assert.AreEqual(1492, oTest.Length, sTestMessage);
	}

	///############################################################
	/// <summary>
	/// Test the Get(iIndex)
	/// </summary>
	/// <remarks>
	/// string Get(					n/a (primitive/simple type, value is tested below)
	///		int iIndex				LBound, UBound, WithinBounds, OutOfBounds(+/-)
	///	)
	/// </remarks>
	///############################################################
	[TestMethod]
	public void Get() {
		StringArray oTest;
		string sTestMessage;
		string sValue;

		//##########
		//##########
		sTestMessage = "iIndex LBound";
		oTest = new StringArray(8);

		oTest.Set(0, "new1");
		Assert.AreEqual("new1", oTest.Get(0), sTestMessage);

		//##########
		//##########
		sTestMessage = "iIndex UBound";
		oTest = new StringArray(6);

		oTest.Set(5, "new2");
		Assert.AreEqual("new2", oTest.Get(5), sTestMessage);

		//##########
		//##########
		sTestMessage = "iIndex WithinBounds";
		oTest = new StringArray(4);

		oTest.Set(1, "new3");
		Assert.AreEqual("new3", oTest.Get(1), sTestMessage);

		oTest.Set(2, "new4");
		Assert.AreEqual("new4", oTest.Get(2), sTestMessage);

		//##########
		//##########
		sTestMessage = "iIndex OutOfBounds -1";
		oTest = new StringArray(6);

		try {
			sValue = oTest.Get(-1);
			Assert.Fail(sTestMessage);
		}
		catch (StringArray.Exception oEx) {}

		//##########
		//##########
		sTestMessage = "iIndex OutOfBounds +1";
		oTest = new StringArray(2);

		try {
			//#### NOTE: This test will fail because of the bug injected into the code
			//####     above, as the raised Exception will not be of type\
			//####     StringArray.Exception
			sValue = oTest.Get(oTest.Length);
			Assert.Fail(sTestMessage);
		}
		catch (StringArray.Exception oEx) {}
	}

	///############################################################
	/// <summary>
	/// Test the Set(iIndex, sValue)
	/// </summary>
	/// <remarks>
	/// string Set(					n/a (primitive/simple type, value is tested below)
	///		int iIndex,				LBound, UBound, WithinBounds, OutOfBounds(+/-)
	///		string sValue			Valid, Null-String, Null
	///	)
	/// </remarks>
	///############################################################
	[TestMethod]
	public void Set() {
		StringArray oTest;
		string[] a_sNull;
		string sTestMessage;

		//##########
		//##########
		sTestMessage = "iIndex LBound";
		oTest = new StringArray(8);

		oTest.Set(0, "new1");
		Assert.AreEqual("new1", oTest.Get(0), sTestMessage);

		//##########
		//##########
		sTestMessage = "iIndex UBound";
		oTest = new StringArray(6);

		oTest.Set(5, "new2");
		Assert.AreEqual("new2", oTest.Get(5), sTestMessage);

		//##########
		//##########
		sTestMessage = "iIndex WithinBounds";
		oTest = new StringArray(4);

		oTest.Set(1, "new3");
		Assert.AreEqual("new3", oTest.Get(1), sTestMessage);

		oTest.Set(2, "new4");
		Assert.AreEqual("new4", oTest.Get(2), sTestMessage);

		//##########
		//##########
		sTestMessage = "iIndex OutOfBounds -1";
		oTest = new StringArray(6);

		try {
			oTest.Set(-1, "new5");
			Assert.Fail(sTestMessage);
		}
		catch (StringArray.Exception oEx) {}

		//##########
		//##########
		sTestMessage = "iIndex OutOfBounds +1";
		oTest = new StringArray(2);

		try {
			oTest.Set(oTest.Length, "new6");
			Assert.Fail(sTestMessage);
		}
		catch (StringArray.Exception oEx) {}

		//##########
		//##########
		sTestMessage = "sValue Valid";
		oTest = new StringArray(5);

		oTest.Set(2, "new7");
		Assert.AreEqual("new7", oTest.Get(2), sTestMessage);

		//##########
		//##########
		sTestMessage = "sValue Null-String";
		oTest = new StringArray(22);

		try {
			oTest.Set(17, "");
			Assert.Fail(sTestMessage);
		}
		catch (StringArray.Exception oEx) {}

		//##########
		//##########
		sTestMessage = "sValue Null";
		oTest = new StringArray(14);
		a_sNull = new string[1];

		try {
			//#### In order to avoid the compiler error of an unitilized string being used,
			//####     we have to be sneaky by getting a null-string from an uninitilized
			//####     index from a string-array
			oTest.Set(11, a_sNull[0]);
			Assert.Fail(sTestMessage);
		}
		catch (StringArray.Exception oEx) {}
	}

} //# public class StringArray_Tests


*/


/*

'''########################################################################################################################
''' <summary>
''' Simple class to demostrate an approach to Unit Testing
''' </summary>
'''########################################################################################################################
Public Class StringArray
    '#### Declare the required global variables
    Private ga_sData As String()
    
    '''############################################################
    ''' <summary>
    ''' Class constructor
    ''' </summary>
    '''############################################################
    Public Sub New(ByVal iLength As Integer)
        If iLength > 0 Then
            ga_sData = New String(iLength - 1) {}
        Else
            Throw New Exception("Initial length must be a positive integer.")
        End If
    End Sub
    
    '''############################################################
    ''' <summary>
    ''' Gets the Length of the StringArray
    ''' </summary>
    '''############################################################
    Public ReadOnly Property Length() As Integer
        Get
            Return ga_sData.Length
        End Get
    End Property
    
    '''############################################################
    ''' <summary>
    ''' Gets the value at the referenced index
    ''' </summary>
    '''############################################################
    Public Function [Get](ByVal iIndex As Integer) As String
        '#### Make note of the intentional bug injected into the if statement that allows
        '####     for the passed iIndex to be outside the bounds of ga_sData
     '! if (iIndex >= 0 && iIndex < ga_sData.Length) {
        If iIndex >= 0 AndAlso iIndex <= ga_sData.Length Then
            Return ga_sData(iIndex)
        Else
            Throw New Exception("Index must be within the bounds of the array.")
        End If
    End Function
    
    '''############################################################
    ''' <summary>
    ''' Sets the value at the referenced index
    ''' </summary>
    '''############################################################
    Public Sub [Set](ByVal iIndex As Integer, ByVal sValue As String)
        If iIndex >= 0 AndAlso iIndex < ga_sData.Length Then
            If Not String.IsNullOrEmpty(sValue) Then
                ga_sData(iIndex) = sValue
            Else
                Throw New Exception("Value must be a valid string reference with a length greater then 0 characters.")
            End If
        Else
            Throw New Exception("Index must be within the bounds of the array.")
        End If
    End Sub
    
    
    '''########################################################################################################################
    ''' <summary>
    ''' Simple Exception class to demostrate an approach to Unit Testing
    ''' </summary>
    '''########################################################################################################################
    Public Class Exception
        Inherits System.Exception
        
        '''############################################################
        ''' <summary>
        ''' Class constructor
        ''' </summary>
        '''############################################################
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        
    End Class '# public class Exception


End Class '# public class StringArray







'''########################################################################################################################
''' <summary>
''' Unit Test collection for the StringArray
''' </summary>
'''########################################################################################################################
<TestClass()> _
Public Class StringArray_Tests
    '#### Define the required private variables
    Private testContextInstance As TestContext
    
    
    '##########################################################################################
    '# Public Properties
    '##########################################################################################
    '''############################################################
    ''' <summary>
    ''' Gets or sets the test context which provides information about and functionality for the current test run.
    ''' </summary>
    '''############################################################
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(ByVal value As TestContext)
            testContextInstance = value
        End Set
    End Property
    
    
    '##########################################################################################
    '# Unit Tests
    '##########################################################################################
    '''############################################################
    ''' <summary>
    ''' Test the Constructor(iLength)
    ''' </summary>
    ''' <remarks>
    ''' StringArray(
    '''		int iLength				Valid, Invalid
    '''	)
    ''' </remarks>
    '''############################################################
    <TestMethod()> _
    Public Sub Constructor()
        Dim oTest As StringArray
        Dim sTestMessage As String
        
        '##########
        '##########
        sTestMessage = "[Constructor] Valid"
        
        oTest = New StringArray(1)
        Assert.AreEqual(1, oTest.Length, sTestMessage)
        
        oTest = New StringArray(10)
        Assert.AreEqual(10, oTest.Length, sTestMessage)
        
        oTest = New StringArray(10000)
        Assert.AreEqual(10000, oTest.Length, sTestMessage)
        
        '##########
        '##########
        sTestMessage = "[Constructor] Invalid"
        
        Try
            oTest = New StringArray(0)
            Assert.Fail(sTestMessage)
        Catch oEx As StringArray.Exception
        End Try
        
        Try
            oTest = New StringArray(-1)
            Assert.Fail(sTestMessage)
        Catch oEx As StringArray.Exception
        End Try
    End Sub
    
    '''############################################################
    ''' <summary>
    ''' Test the Length
    ''' </summary>
    ''' <remarks>
    ''' int Length					ExpectedValue
    ''' </remarks>
    '''############################################################
    <TestMethod()> _
    Public Sub Length()
        Dim oTest As StringArray
        Dim sTestMessage As String
        
        '##########
        '##########
        sTestMessage = "ReturnValue ExpectedValue"
        
        oTest = New StringArray(7)
        Assert.AreEqual(7, oTest.Length, sTestMessage)
        
        oTest = New StringArray(22)
        Assert.AreEqual(22, oTest.Length, sTestMessage)
        
        oTest = New StringArray(1492)
        Assert.AreEqual(1492, oTest.Length, sTestMessage)
    End Sub
    
    '''############################################################
    ''' <summary>
    ''' Test the Get(iIndex)
    ''' </summary>
    ''' <remarks>
    ''' string Get(					n/a (primitive/simple type, value is tested below)
    '''		int iIndex				LBound, UBound, WithinBounds, OutOfBounds(+/-)
    '''	)
    ''' </remarks>
    '''############################################################
    <TestMethod()> _
    Public Sub [Get]()
        Dim oTest As StringArray
        Dim sTestMessage As String
        Dim sValue As String
        
        '##########
        '##########
        sTestMessage = "iIndex LBound"
        oTest = New StringArray(8)
        
        oTest.[Set](0, "new1")
        Assert.AreEqual("new1", oTest.[Get](0), sTestMessage)
        
        '##########
        '##########
        sTestMessage = "iIndex UBound"
        oTest = New StringArray(6)
        
        oTest.[Set](5, "new2")
        Assert.AreEqual("new2", oTest.[Get](5), sTestMessage)
        
        '##########
        '##########
        sTestMessage = "iIndex WithinBounds"
        oTest = New StringArray(4)
        
        oTest.[Set](1, "new3")
        Assert.AreEqual("new3", oTest.[Get](1), sTestMessage)
        
        oTest.[Set](2, "new4")
        Assert.AreEqual("new4", oTest.[Get](2), sTestMessage)
        
        '##########
        '##########
        sTestMessage = "iIndex OutOfBounds -1"
        oTest = New StringArray(6)
        
        Try
            sValue = oTest.[Get](-1)
            Assert.Fail(sTestMessage)
        Catch oEx As StringArray.Exception
        End Try
        
        '##########
        '##########
        sTestMessage = "iIndex OutOfBounds +1"
        oTest = New StringArray(2)
        
        Try
            '#### NOTE: This test will fail because of the bug injected into the code
            '####     above, as the raised Exception will not be of type\
            '####     StringArray.Exception
            sValue = oTest.[Get](oTest.Length)
            Assert.Fail(sTestMessage)
        Catch oEx As StringArray.Exception
        End Try
    End Sub
    
    '''############################################################
    ''' <summary>
    ''' Test the Set(iIndex, sValue)
    ''' </summary>
    ''' <remarks>
    ''' string Set(					n/a (primitive/simple type, value is tested below)
    '''		int iIndex,				LBound, UBound, WithinBounds, OutOfBounds(+/-)
    '''		string sValue			Valid, Null-String, Null
    '''	)
    ''' </remarks>
    '''############################################################
    <TestMethod()> _
    Public Sub [Set]()
        Dim oTest As StringArray
        Dim a_sNull As String()
        Dim sTestMessage As String
        
        '##########
        '##########
        sTestMessage = "iIndex LBound"
        oTest = New StringArray(8)
        
        oTest.[Set](0, "new1")
        Assert.AreEqual("new1", oTest.[Get](0), sTestMessage)
        
        '##########
        '##########
        sTestMessage = "iIndex UBound"
        oTest = New StringArray(6)
        
        oTest.[Set](5, "new2")
        Assert.AreEqual("new2", oTest.[Get](5), sTestMessage)
        
        '##########
        '##########
        sTestMessage = "iIndex WithinBounds"
        oTest = New StringArray(4)
        
        oTest.[Set](1, "new3")
        Assert.AreEqual("new3", oTest.[Get](1), sTestMessage)
        
        oTest.[Set](2, "new4")
        Assert.AreEqual("new4", oTest.[Get](2), sTestMessage)
        
        '##########
        '##########
        sTestMessage = "iIndex OutOfBounds -1"
        oTest = New StringArray(6)
        
        Try
            oTest.[Set](-1, "new5")
            Assert.Fail(sTestMessage)
        Catch oEx As StringArray.Exception
        End Try
        
        '##########
        '##########
        sTestMessage = "iIndex OutOfBounds +1"
        oTest = New StringArray(2)
        
        Try
            oTest.[Set](oTest.Length, "new6")
            Assert.Fail(sTestMessage)
        Catch oEx As StringArray.Exception
        End Try
        
        '##########
        '##########
        sTestMessage = "sValue Valid"
        oTest = New StringArray(5)
        
        oTest.[Set](2, "new7")
        Assert.AreEqual("new7", oTest.[Get](2), sTestMessage)
        
        '##########
        '##########
        sTestMessage = "sValue Null-String"
        oTest = New StringArray(22)
        
        Try
            oTest.[Set](17, "")
            Assert.Fail(sTestMessage)
        Catch oEx As StringArray.Exception
        End Try
        
        '##########
        '##########
        sTestMessage = "sValue Null"
        oTest = New StringArray(14)
        a_sNull = New String(0) {}
        
        Try
            '#### In order to avoid the compiler error of an unitilized string being used,
            '####     we have to be sneaky by getting a null-string from an uninitilized
            '####     index from a string-array
            oTest.[Set](11, a_sNull(0))
            Assert.Fail(sTestMessage)
        Catch oEx As StringArray.Exception
        End Try
    End Sub


End Class '# public class StringArray_Tests


*/