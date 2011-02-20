/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Reflection;
using System.Web.UI;								//# Required to access ControlCollection class
using System;
using Cn.Web.Inputs;


namespace Cn.Data {

	///########################################################################################################################
	/// <summary>
	/// Collection of static functions to map data between objects utilizing Reflection (hence the play on words).
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
	  //private const string g_cClassName = "Cn.Data.Mirror.";
//!		private const BindingFlags g_cBINDING_FLAGS = BindingFlags.Default; // (BindingFlags.Public | BindingFlags.Instance);



/*
struct structObjectPath {
	public string[] Keys = null;
	public string Name = "";
}

struct structWhereClause {
	public string TableName = "";
	public string ColumnName = "";
	public string Value = "";
}

// sPath = "parent.child[1].grandchild.greatgrandchild[7,3,name]"
public static structObjectPath[] ParseObjectPath(string sPath) {
	structObjectPath[] a_oReturn = new structObjectPath[0];
	string[] a_sPath;
	string sCurrentPath;
	int iCloseParenIndex;
	int iOpenParenIndex;
	int i;

		//#### If the passed sPath is valid
	if (! string.IsNullOrEmpty(sPath)) {
			//#### .Split the passed sPath, then reset our a_oReturn value to the dimensions of the .Split a_sPath
		a_sPath = sPath.Split('.');
		a_oReturn = new structObjectPath[a_sPath.Length];

			//#### Traverse the a_sPath, parsing it into our a_oReturn value as we go
		for (i = 0; i < a_sPath.Length; i++) {
				//#### Set the sCurrentPath, iOpenParenIndex and iCloseParenIndex for this loop
			sCurrentPath = a_sPath[i];
			iOpenParenIndex = sCurrentPath.IndexOf('[');
			iCloseParenIndex = sCurrentPath.IndexOf(']');

				//#### If there is a valid .Keys iOpenParenIndex/iCloseParenIndex pair, collect the .Keys and peal off the .Name from the current a_sPath
			if (iOpenParenIndex > 0 && iCloseParenIndex > 0 && iOpenParenIndex < iCloseParenIndex) {
				a_oReturn[i].Keys = sCurrentPath.Substring(iOpenParenIndex + 1, iCloseParenIndex - iOpenParenIndex).Split(',');
				a_oReturn[i].Name = sCurrentPath.Substring(0, iOpenParenIndex - 1);
			}
				//#### Else there is no .Key, so simply set the .Name from the current a_sPath
			else {
				a_oReturn[i].Name = sCurrentPath;
			}
		}
	}

		//#### Return the above determined a_oReturn value to the caller
	return a_oReturn;
}

public static object GetObject(object oRootObject, string sObjectPath) {
		//#### .ParseObjectPath while passing the call off to our sibling implementation, returning it's result as our own
	return GetObject(oRootObject, ParseObjectPath(sObjectPath));
}

public static object GetObject(object oRootObject, structObjectPath[] a_oObjectPath) {
	PropertyInfo oProperty;
	object oReturn = null;
	int i;

		//#### If a valid a_oObjectPath was passed
	if (a_oObjectPath != null && a_oObjectPath.Length > 0) {
			//#### Setup our oReturn value in prep for the loop below
		oReturn = oRootObject;

			//#### Traverse the passed a_oObjectPath
		for (i = 0; i < a_oObjectPath.Length; i++) {
				//#### If the current oReturn value is non-null
			if (oReturn != null) {
					//#### Collect the current .Name from our current oReturn value
				oProperty = oReturn.GetType().GetProperty(a_oObjectPath[i].Name);

					//#### If the oProperty exists and we .CanRead it, reset our oReturn value to the current .Name
				if (oProperty != null && oProperty.CanRead) {
					oReturn = oProperty.GetValue(oReturn, a_oObjectPath[i].Keys);
				}
			}
				//#### Else the passed oRootObject was null or the last a_oObjectPath element was, so fall from the loop
				//####     NOTE: There is no need to reset our oReturn value to null below, as it is already null =)
			else {
				break;
			}
		}
	}

		//#### Return the above determined oReturn value to the caller
	return oReturn;
}

public static structObjectPath[] TranslateToPath(string sTableName, object oBusinessObject, structWhereClause[] a_oWhereClause) {
	structObjectPath[] a_oReturn = new structObjectPath[0];
	string[] a_sTablePath = TablePath(sTableName);
	int iTablePathLength = a_sTablePath.Length;
	int i;

//System.Collections.Generic.List<string> l_oList = new System.Collections.Generic.List<string>[1];
//l_oList[1] = "";


		//#### If we have to traverse down the table structure
	if (iTablePathLength > 0) {
			//#### Reset our a_oReturn value to the proper dimension
		a_oReturn = new structObjectPath[iTablePathLength];

			//#### If we have a a_oWhereClause to consider
		if (a_oWhereClause != null && a_oWhereClause.Length > 0) {
			string[] a_sKeys;
			string sCurrentTableName;
			int j;

				//#### Traverse the a_sTablePath
			for (i = 0; i < iTablePathLength; i++) {
					//#### Collect the sCurrentTableName and set the a_oReturn value's current .Name for this loop
					//####     NOTE: There is no need to .ToLower the sCurrentTableName below because we know all indexes returned from .TablePath are already .ToLower'd
				sCurrentTableName = a_sTablePath[i];
				a_oReturn[i].Name = sCurrentTableName;

					//#### Traverse the a_oWhereClause
				for (j = 0; j < a_oWhereClause.Length; j++) {
						//#### If the current a_oWhereClause is for the sCurrentTableName
					if (Data.Tools.MakeString(a_oWhereClause[j].TableName, "").ToLower() == sCurrentTableName) {
							//#### Dimension the a_sKeys to include any previosu .Keys +1 to accomidate the new index
//						a_sKeys = new string[(a_oReturn[i].Keys == null ? 1 : a_oReturn[i].Keys.Length)];
						a_sKeys = new string[1];

							//#### Process the current a_oWhereClause, setting the data into the a_sKeys
//! The majic need to happen here to populate the index(es)/a_sKeys using the passed oBusinessObject

							//#### Set the above-collected a_sKeys into our  .Keys
						a_oReturn[i].Keys = a_sKeys;
					}
				}
			}
		}
			//#### Else there is no a_oWhereClause
		else {
				//#### Traverse the a_sTablePath, copying the object names into our a_oReturn value's .Names
			for (i = 0; i < iTablePathLength; i++) {
				a_oReturn[i].Name = a_sTablePath[i];
			}
		}
	}

		//#### Return the above determined a_oReturn value to the caller
	return a_oReturn;
}

//# NOTE: this would be located within the BusinessObject
public static string[] TablePath(string sTableName) {
	string[] a_sReturn;

		//#### .ToLower the passed sTableName (ensuring it's a string as we go)
	sTableName = Data.Tools.MakeString(sTableName, "").ToLower();

		//#### Determine the passed .ToLower'd sTableName and set our a_sReturn value accordingly
	switch (sTableName) {
		case "saap_int_child_support": {
			a_sReturn = new string[] { "saap_int_child", "saap_int_child_support" };
			break;
		}
		case "saap_int_child":
		case "saap_int_accom_prov":
		case "saap_int_assist_reason":
		case "saap_int_support_ass": {
			a_sReturn = new string[] { sTableName };
			break;
		}
		case "saap_int_client":
		default: {
			a_sReturn = new string[0];
			break;
		}
	}

		//#### Return the above determined a_sReturn value to the caller
	return a_sReturn;
}
*/


        //##########################################################################################
        //# GetProperty-related Functions
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the value of the referenced property from the provided object.
		/// </summary>
		/// <param name="oObject">Object to be used as the data source.</param>
		/// <param name="sPropertyName">String representing the name of the requested property.</param>
		/// <returns>Object representing the value of the property.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static object GetPropertyValue(object oObject, string sPropertyName) {
//			return oObject.GetType().InvokeMember(sPropertyName, BindingFlags.GetProperty, null, oObject, null);
			object oReturn;

				//#### Collect our oReturn value (if any) via our sibling implementation
			GetPropertyValue(oObject, sPropertyName, out oReturn);

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Gets the referenced property within the provided object.
		/// </summary>
		/// <param name="oObject">Object to be used as the data source.</param>
		/// <param name="a_sPath">Array of Strings representing the path to the requested property.</param>
		/// <returns>Object representing the value of the property.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static object GetPropertyValue(object oObject, string[] a_sPath) {
			object oPropertyObject;
			string sPropertyName;
			object oReturn = null;

				//#### If we can .Locate(the)PropertyObject
			if (LocatePropertyObject(oObject, a_sPath, out oPropertyObject, out sPropertyName)) {
					//#### Collect our oReturn value (if any) via our sibling implementation
				oReturn = GetPropertyValue(oPropertyObject, sPropertyName);
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		///############################################################
		/// <summary>
		/// Gets the value of the referenced property from the provided object.
		/// </summary>
		/// <typeparam name="T">Type of the property value to return.</typeparam>
		/// <param name="oObject">Object to be used as the data source.</param>
		/// <param name="sPropertyName">String representing the name of the requested property.</param>
		/// <returns>Type representing the value of the property.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static T GetPropertyValueAs<T>(object oObject, string sPropertyName) {
//			return (T)(oObject.GetType().InvokeMember(sPropertyName, BindingFlags.GetProperty, null, oObject, null));
			T tReturn;

				//#### Collect our tReturn value (if any) via our sibling implementation
			GetPropertyValueAs(oObject, sPropertyName, out tReturn);

				//#### Return the above determined tReturn value to the caller
			return tReturn;
		}

		///############################################################
		/// <summary>
		/// Gets the referenced property within the provided object.
		/// </summary>
		/// <typeparam name="T">Type of the property value to return.</typeparam>
		/// <param name="oObject">Object to be used as the data source.</param>
		/// <param name="a_sPath">Array of Strings representing the path to the requested property.</param>
		/// <returns>Object representing the value of the property.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static T GetPropertyValueAs<T>(object oObject, string[] a_sPath) {
			object oPropertyObject;
			string sPropertyName;
			T tReturn = default(T);

				//#### If we can .Locate(the)PropertyObject
			if (LocatePropertyObject(oObject, a_sPath, out oPropertyObject, out sPropertyName)) {
					//#### Collect our tReturn value (if any) via our sibling implementation
				GetPropertyValueAs(oPropertyObject, sPropertyName, out tReturn);
			}

				//#### Return the above determined tReturn value to the caller
			return tReturn;
		}

		///############################################################
		/// <summary>
		/// Safely gets the value of the referenced property from the provided object.
		/// </summary>
		/// <param name="oObject">Object to be used as the data source.</param>
		/// <param name="sPropertyName">String representing the name of the requested property.</param>
		/// <param name="oValue">Returned Object that will recieve the value of the property.</param>
		/// <returns>Boolean value indicating if we successfully collected the property's value.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool GetPropertyValue(object oObject, string sPropertyName, out object oValue) {
			PropertyInfo oProperty;
			bool bReturn = false;

				//#### Set the passed oValue to null
			oValue = null;

				//#### If the passed oObject is non-null
			if (oObject != null) {
					//#### Collect the sPropertyName
				oProperty = oObject.GetType().GetProperty(sPropertyName);

					//#### If the oProperty exists and we .CanRead it, reset the passed oValue to it's .GetValue and flip our bReturn value to true
				if (oProperty != null && oProperty.CanRead) {
					oValue = oProperty.GetValue(oObject, null);
					bReturn = true;
				}
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Gets the referenced property within the provided object.
		/// </summary>
		/// <param name="oObject">Object to be used as the data source.</param>
		/// <param name="a_sPath">Array of Strings representing the path to the requested property.</param>
		/// <param name="oValue">Returned Object that will recieve the value of the property.</param>
		/// <returns>Boolean value indicating if we successfully collected the property's value.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool GetPropertyValue(object oObject, string[] a_sPath, out object oValue) {
			object oPropertyObject;
			string sPropertyName;
			bool bReturn = false;

				//#### Set the passed oValue to null
			oValue = null;

				//#### If we can .Locate(the)PropertyObject
			if (LocatePropertyObject(oObject, a_sPath, out oPropertyObject, out sPropertyName)) {
					//#### Collect our oReturn value (if any) via our sibling implementation
				bReturn = GetPropertyValue(oPropertyObject, sPropertyName, out oValue);
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Safely gets the value of the referenced property from the provided object.
		/// </summary>
		/// <typeparam name="T">Type of the property value to return.</typeparam>
		/// <param name="oObject">Object to be used as the data source.</param>
		/// <param name="sPropertyName">String representing the name of the requested property.</param>
		/// <param name="tValue">Returned Type that will recieve the value of the property.</param>
		/// <returns>Boolean value indicating if we successfully collected the property's value.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool GetPropertyValueAs<T>(object oObject, string sPropertyName, out T tValue) {
			object oValue;
			bool bReturn;

				//#### Determine our bReturn value based on our sibling implementation and set the passed tValue to its type's default
				//####     NOTE: We cannot simply set tValue = null as it my be a primitive type, hence the need to use default()
			bReturn = GetPropertyValue(oObject, sPropertyName, out oValue);
			tValue = default(T);

				//#### If the sPropertyName exists and we have a valid oValue to recast as a T
			if (bReturn && oValue != null) {
					//#### If the sPropertyName's .PropertyType matches the passed tValue's T
					//####     NOTE: We know that "oProperty != null && oProperty.CanRead" because our sibling implementation returned true above, so there is no need to test it here
					//####     NOTE: Cannot seem to use either "Convert.ChangeType(...)" or "[...] as oType" due to weirdnesses with using T
				if (oObject.GetType().GetProperty(sPropertyName).PropertyType == typeof(T)) {
					tValue = (T)oValue;
				}
					//#### Else the T didn't match the sPropertyName's .PropertyType, so reset our bReturn value to false
				else {
					bReturn = false;
				}
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Gets the referenced property within the provided object.
		/// </summary>
		/// <typeparam name="T">Type of the property value to return.</typeparam>
		/// <param name="oObject">Object to be used as the data source.</param>
		/// <param name="a_sPath">Array of Strings representing the path to the requested property.</param>
		/// <param name="tValue">Returned Type that will recieve the value of the property.</param>
		/// <returns>Boolean value indicating if we successfully collected the property's value.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool GetPropertyValueAs<T>(object oObject, string[] a_sPath, out T tValue) {
			object oPropertyObject;
			string sPropertyName;
			bool bReturn = false;

				//#### Set the passed tValue to its type's default
			tValue = default(T);

				//#### If we can .Locate(the)PropertyObject
			if (LocatePropertyObject(oObject, a_sPath, out oPropertyObject, out sPropertyName)) {
					//#### Collect our oReturn value (if any) via our sibling implementation
				bReturn = GetPropertyValueAs(oPropertyObject, sPropertyName, out tValue);
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}


        //##########################################################################################
        //# SetProperty-related Functions
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Sets the referenced property within the provided object.
		/// </summary>
		/// <param name="oObject">Object where the property is to be set.</param>
		/// <param name="sPropertyName">String representing the name of the requested property.</param>
		/// <param name="oValue">Object representing the value to set within the property.</param>
		/// <returns>Boolean value indicating if we successfully set the property's value.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool SetPropertyValue(object oObject, string sPropertyName, object oValue) {
//			oObject.GetType().InvokeMember(sPropertyName, BindingFlags.SetProperty, null, oObject, new object[] { oValue });
			PropertyInfo oProperty;
			bool bReturn = false;

				//#### If the passed oObject is non-null
			if (oObject != null) {
					//#### Collect the sPropertyName
				oProperty = oObject.GetType().GetProperty(sPropertyName);

					//#### If the oProperty exists and we .CanWrite to it
				if (oProperty != null && oProperty.CanWrite) {
						//#### Attempt to .Set(the)Value within the oProperty (as the cast could fail), then set our bReturn value to true
						//####     NOTE: We don't check for (oProperty.PropertyType == tValue.GetType()) as tValue could be null, or an integer/float, or... and that value could be valid. So instead we try/catch any errors below.
					try {
						oProperty.SetValue(oObject, Convert.ChangeType(oValue, oProperty.PropertyType), null);
						bReturn = true;
					}
						//#### If the .SetValue failed, ensue our bReturn value is false
						//####     NOTE: We don't just look for InvalidCastException and FormatException's because we suppress all errors here and simpyl return false
					catch (Exception ex) {
						bReturn = false;
					}
				}
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Sets the referenced property within the provided object.
		/// </summary>
		/// <param name="oObject">Object where the property is to be set.</param>
		/// <param name="a_sPath">Array of Strings representing the path to the requested property.</param>
		/// <param name="oValue">Object representing the value to set within the property.</param>
		/// <returns>Boolean value indicating if we successfully set the property's value.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool SetPropertyValue(object oObject, string[] a_sPath, object oValue) {
			object oPropertyObject;
			string sPropertyName;
			bool bReturn = false;

				//#### If we can .Locate(the)PropertyObject
			if (LocatePropertyObject(oObject, a_sPath, out oPropertyObject, out sPropertyName)) {
					//#### Attempt to .Set(the)Property, resetting our bReturn value to the result
				bReturn = SetPropertyValue(oPropertyObject, sPropertyName, oValue);
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Safely sets the referenced property within the provided object.
		/// </summary>
		/// <remarks>
		/// NOTE: This function will raise errors if the passed <paramref name="oObject"/> is null, if the <paramref name="sPropertyName"/> is invalid or if the provided <typeparamref name="T"/> doesn't match the property's type.
		/// </remarks>
		/// <typeparam name="T">Type of the property value to set.</typeparam>
		/// <param name="oObject">Object to be used as the data source.</param>
		/// <param name="sPropertyName">String representing the name of the requested property.</param>
		/// <param name="tValue">Type that will recieve the value of the property.</param>
		/// <returns>Boolean value indicating if we successfully set the property's value.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool SetPropertyValueAs<T>(object oObject, string sPropertyName, T tValue) {
			PropertyInfo oProperty;
			bool bReturn = false;

				//#### If the passed oObject is non-null
			if (oObject != null) {
					//#### Collect the sPropertyName
				oProperty = oObject.GetType().GetProperty(sPropertyName);

					//#### If the oProperty exists, we .CanWrite to it and it's of the expected T type
				if (oProperty != null && oProperty.CanWrite && oProperty.PropertyType == typeof(T)) {
						//#### .Set(the)Value into the oProperty and flip our bReturn value to true
					oProperty.SetValue(oObject, tValue, null);
					bReturn = true;
				}
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Sets the referenced property within the provided object.
		/// </summary>
		/// <typeparam name="T">Type of the property value to return.</typeparam>
		/// <param name="oObject">Object where the property is to be set.</param>
		/// <param name="a_sPath">Array of Strings representing the path to the requested property.</param>
		/// <param name="tValue">Object representing the value to set within the property.</param>
		/// <returns>Boolean value indicating if we successfully set the property's value.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool SetPropertyValueAs<T>(object oObject, string[] a_sPath, T tValue) {
			object oPropertyObject;
			string sPropertyName;
			bool bReturn = false;

				//#### If we can .Locate(the)PropertyObject
			if (LocatePropertyObject(oObject, a_sPath, out oPropertyObject, out sPropertyName)) {
					//#### Attempt to .Set(the)PropertyAs, resetting our bReturn value to the result
				bReturn = SetPropertyValueAs(oPropertyObject, sPropertyName, tValue);
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}


        //##########################################################################################
        //# Map-related Functions
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Maps data between the provided objects.
		/// </summary>
		/// <param name="oFrom">Object to be used as the data source.</param>
		/// <param name="oTo">Object to be used as the data destination.</param>
		/// <returns>Boolean value representing if all of the readable properties within the provided <paramref name="oFrom"/> were successfully mapped into <paramref name="oTo"/>.</returns>
		///############################################################
		/// <LastUpdated>December 11, 2009</LastUpdated>
		public static bool Map(object oFrom, object oTo) {
			PropertyInfo[] a_oFromProperties;
			PropertyInfo[] a_oToProperties;
			Type oFromType;
			Type oToType;
			string sName;
			int i;
			bool bFound;
			bool bReturn = true;

				//#### Determine the oFromType/oToType of the passed objects
			oFromType = oFrom.GetType();
			oToType = oTo.GetType();

				//#### .Get(all the)Properties for the passed objects
			a_oFromProperties = oFromType.GetProperties();
			a_oToProperties = oToType.GetProperties();

				//#### If we have a_oToProperties to populate
			if (a_oToProperties != null && a_oToProperties.Length > 0) {
					//#### Traverse the a_oFromProperties
				foreach (PropertyInfo oFromProperty in a_oFromProperties) {
						//#### If we .CanRead the current oFromProperty (as there's no reason to bother processing it if we can't)
					if (oFromProperty.CanRead) {
							//#### Collect the sName for the current oFromProperty and reset bFound for this loop
						sName = oFromProperty.Name;
						bFound = false;

							//#### Traverse the a_oToProperties via thier indexes
						for (i = 0; i < a_oToProperties.Length; i++) {
								//#### If the current a_oToProperties hasn't been bFound before and it's .Name matches the sName
							if (a_oToProperties[i] != null && a_oToProperties[i].Name == sName) {
									//#### If we .CanWrite the current a_oToProperties
								if (a_oToProperties[i].CanWrite) {
										//#### Attempt to .Set(the)Value within the oFromProperty (as the cast could fail)
									try {
										oFromProperty.SetValue(oTo, Convert.ChangeType(a_oToProperties[i].GetValue(oFrom, null), a_oToProperties[i].PropertyType), null);
									}
										//#### If the cast failed, handle it by resetting our bReturn value to false (as the .SetValue failed) else let the Exception bubble up
									catch (InvalidCastException ex) {
										bReturn = false;
									}
									catch (FormatException ex) {
										bReturn = false;
									}
//!
									bFound = true;
								}

									//#### Reset the current a_oToProperties to null so it's not re-processed in subsequent loops and fall from the inner for loop
								a_oToProperties = null;
								break;
							}
						}

							//#### If the current oFromProperty was not bFound above, reset our bReturn value to false (as not all of the .CanRead properties from the oFrom object were set into oTo)
						if (! bFound) {
							bReturn = false;
						}
					}
				}
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Maps data between the provided objects of the same type.
		/// </summary>
		/// <remarks>
		/// This function always returns true. A boolean return value is provided only for consistency across the other <c>Map</c> functions.
		/// </remarks>
		/// <param name="oFrom">Class-based Object to be used as the data source.</param>
		/// <param name="oTo">Class-based Object to be used as the data destination.</param>
		/// <returns>Boolean value of true.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool MapAs<T>(T oFrom, T oTo) where T : class {
			PropertyInfo[] a_oProperties;
			bool bReturn = (oFrom != null && oTo != null);

				//#### If the passed oFrom and oTo are valid
			if (bReturn) {
					//#### .Get(all the)Properties for the passed objects
				a_oProperties = oFrom.GetType().GetProperties();

					//#### Traverse the a_oProperties
				foreach (PropertyInfo oProperty in a_oProperties) {
						//#### If we .CanRead and .CanWrite the current oProperty (as there's no reason to bother processing it if we can't)
					if (oProperty.CanRead && oProperty.CanWrite) {
							//#### .GetValue from the oFrom and .SetValue into the oTo objects
							//####     NOTE: We don't need to worry about type casting issues as both oTo and oFrom are of the same Type
try {
						oProperty.SetValue(oTo, Convert.ChangeType(oProperty.GetValue(oFrom, null), oProperty.PropertyType), null);
}
catch(System.InvalidCastException oEx) {
//! if objects implement interfaces then InvalidCastException's can occur if said objects don't implement IConvertable
}
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
		/// <param name="sFromPropertyName">String representing the name of the requested property within the data source.</param>
		/// <param name="oTo">Object to be used as the data destination.</param>
		/// <param name="sToPropertyName">String representing the name of the requested property within the data destination.</param>
		/// <returns>Boolean value representing if all of the readable properties within the provided <paramref name="oFrom"/> were successfully mapped into <paramref name="oTo"/>.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool Map(object oFrom, string sFromPropertyName, object oTo, string sToPropertyName) {
			object oValue;
			bool bReturn = false;
			
				//#### If we can successfully collect the oValue of the sFromPropertyName
			if (GetPropertyValue(oFrom, sFromPropertyName, out oValue)) {
					//#### Reset our bReturn value based on if we can successfully set the oValue into the oToPropertyObject's sToPropertyName
				bReturn = SetPropertyValue(oTo, sToPropertyName, oValue);
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Maps data between the provided objects.
		/// </summary>
		/// <param name="oFrom">Object to be used as the data source.</param>
		/// <param name="a_sFromPath">Array of Strings representing the path to the requested property within the data source.</param>
		/// <param name="oTo">Object to be used as the data destination.</param>
		/// <param name="a_sToPath">Array of Strings representing the path to the requested property within the data destination.</param>
		/// <returns>Boolean value representing if all of the readable properties within the provided <paramref name="oFrom"/> were successfully mapped into <paramref name="oTo"/>.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		public static bool Map(object oFrom, string[] a_sFromPath, object oTo, string[] a_sToPath) {
			object oFromPropertyObject;
			object oToPropertyObject;
			string sFromPropertyName;
			string sToPropertyName;
			bool bReturn = false;

				//#### If we can .Locate(the oFrom)PropertyObject
			if (LocatePropertyObject(oFrom, a_sFromPath, out oFromPropertyObject, out sFromPropertyName)) {
					//#### If we can .Locate(the oTo)PropertyObject
				if (LocatePropertyObject(oFrom, a_sFromPath, out oToPropertyObject, out sToPropertyName)) {
						//#### Reset our bReturn value based on if we can successfully .Map the properties via our sibling implementation
					bReturn = Map(oFromPropertyObject, sFromPropertyName, oToPropertyObject, sToPropertyName);
				}
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}

		///############################################################
		/// <summary>
		/// Maps data between the provided objects.
		/// </summary>
		/// <param name="oFrom">IInputCollection object to be used as the data source.</param>
		/// <param name="oTo">Object to be used as the data destination.</param>
		/// <returns>Boolean value representing if all of the properties within the provided <paramref name="oTo"/> object were successfully mapped.</returns>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static bool Map(IInputCollection oFrom, object oTo) {
				//#### Pass the call into .DoMap, signaling that it is to .Populate(the)BusinessObject
			return DoMap(oFrom, oTo, enumDirection.PopulateBusinessObject);
		}

		///############################################################
		/// <summary>
		/// Maps data between the provided objects.
		/// </summary>
		/// <param name="oFrom">Object to be used as the data source.</param>
		/// <param name="oTo">IInputCollection object to be used as the data destination.</param>
		/// <returns>Boolean value representing if all of the properties within the provided <paramref name="oTo"/> object were successfully mapped.</returns>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static bool Map(object oFrom, IInputCollection oTo) {
				//#### Pass the call into .DoMap, signaling that it is to .Populate(the)Controls
			return DoMap(oTo, oFrom, enumDirection.PopulateContols);
		}

			///############################################################
			/// <summary>
			/// Maps data between the provided objects.
			/// </summary>
			/// <param name="oInputCollection">IInputCollection object to be used as a data source/destination.</param>
			/// <param name="oBusinessObject">Object to be used as a data source/destination.</param>
			/// <param name="eDirection">Enumeration defining the direction of the mapping.</param>
			/// <returns>Boolean value representing if all of the properties within the provided <paramref name="oBusinessObject"/> were successfully mapped.</returns>
			///############################################################
			/// <LastUpdated>December 1, 2009</LastUpdated>
			private static bool DoMap(IInputCollection oInputCollection, object oBusinessObject, enumDirection eDirection) {
				PropertyInfo[] a_oProperties;
				Type oType;
				Web.Inputs.InputData oInput;
				string sName;
				bool bReturn = true;

					//#### Determine the oType of the passed oBusinessObject
				oType = oBusinessObject.GetType();

					//#### .Get(all the)Properties for the oBusinessObject
				a_oProperties = oType.GetProperties();

					//#### Traverse the a_oProperties
				foreach (PropertyInfo oProperty in a_oProperties) {
//! add .CanRead logic to other imps.
						//#### If we .CanRead the current oProperty (as there's no reason to bother processing it if we can't)
					if (oProperty.CanRead) {
							//#### Collect the sName for the current oProperty
						sName = oProperty.Name;
						
							//#### If the oInput does exist, collect it
						if (oInputCollection.Exists(sName)) {
							oInput = oInputCollection.Inputs(sName);

								//#### Determine the passed eDirection and process accordingly
							switch (eDirection) {
								case enumDirection.PopulateBusinessObject: {
										//#### If we .CanWrite to the oProperty
									if (oProperty.CanWrite) {
											//#### Attempt to .Set(the)Value within the oProperty (as the cast could fail)
										try {
											oProperty.SetValue(oBusinessObject, Convert.ChangeType(oInput.Value, oProperty.PropertyType), null);
										}
											//#### If the cast failed, handle it by resetting our bReturn value to false (as the .SetValue failed) else let the Exception bubble up
										catch (InvalidCastException ex) {
											bReturn = false;
										}
										catch (FormatException ex) {
											bReturn = false;
										}
									}
										//#### Else the oProperty isn't writable, so reset our bReturn value
									else {
										bReturn = false;
									}
									break;
								}

								default: { //case enumDirection.PopulateContols: {
										//#### If we .CanRead the oProperty, collect the sValue from the oProperty and set it into the oInput.Value
									if (oProperty.CanRead) {
										oInput.Value = Tools.MakeString(oProperty.GetValue(oBusinessObject, null), string.Empty);
									}
									break;
								}
							}
						}
							//#### Else the oInput does not exist, so since we have not found all of the oBusinessObjects a_oProperties, reset our bReturn value to false
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
		/// <param name="oFrom">IInputCollection object to be used as the data source.</param>
		/// <param name="oTo">Object to be used as the data destination.</param>
		/// <param name="a_sNamesToMap">Array of strings defining the properties/controls to map.</param>
		/// <returns>Boolean value representing if all of the provided <paramref name="a_sNamesToMap"/> were successfully mapped.</returns>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static bool Map(IInputCollection oFrom, object oTo, string[] a_sNamesToMap) {
				//#### Pass the call into .DoMap, signaling that it is to .Populate(the)BusinessObject
			return DoMap(oFrom, oTo, a_sNamesToMap, enumDirection.PopulateBusinessObject);
		}

		///############################################################
		/// <summary>
		/// Maps data between the provided objects.
		/// </summary>
		/// <param name="oFrom">Object to be used as the data source.</param>
		/// <param name="oTo">IInputCollection object to be used as the data destination.</param>
		/// <param name="a_sNamesToMap">Array of strings defining the properties/controls to map.</param>
		/// <returns>Boolean value representing if all of the provided <paramref name="a_sNamesToMap"/> were successfully mapped.</returns>
		///############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static bool Map(object oFrom, IInputCollection oTo, string[] a_sNamesToMap) {
				//#### Pass the call into .DoMap, signaling that it is to .Populate(the)Contols
			return DoMap(oTo, oFrom, a_sNamesToMap, enumDirection.PopulateContols);
		}

			///############################################################
			/// <summary>
			/// Maps data between the provided objects.
			/// </summary>
			/// <param name="oInputCollection">IInputCollection object to be used as a data source/destination.</param>
			/// <param name="oBusinessObject">Object to be used as a data source/destination.</param>
			/// <param name="a_sNamesToMap">Array of strings defining the properties/controls to map.</param>
			/// <param name="eDirection">Enumeration defining the direction of the mapping.</param>
			/// <returns>Boolean value representing if all of the provided <paramref name="a_sNamesToMap"/> were successfully mapped.</returns>
			///############################################################
			/// <LastUpdated>February 15, 2010</LastUpdated>
			private static bool DoMap(IInputCollection oInputCollection, object oBusinessObject, string[] a_sNamesToMap, enumDirection eDirection) {
				PropertyInfo oProperty;
				Type oType;
				Web.Inputs.InputData oInput;
				int iNamesCount;
				int i;
				bool bFound;
				bool bReturn = true;

//List<string> l_sNames;
//l_sNames.Remove

					//#### If we have a_sNames to traverse and valid objects to query
				if (a_sNamesToMap != null && a_sNamesToMap.Length > 0 &&
					oInputCollection != null && oBusinessObject != null
				) {
						//#### Collect the iNamesCount
					iNamesCount = a_sNamesToMap.Length;

						//#### Determine the oType of the passed oBusinessObject
					oType = oBusinessObject.GetType();

						//#### Traverse the passed a_sNamesToMap
					for (i = 0; i < iNamesCount; i++) {
							//#### Reset bFound for this loop, setting it to if the current a_sNamesToMap .Exists within the oInputCollection
						bFound = oInputCollection.Exists(a_sNamesToMap[i]);

							//#### If we bFound the current a_sNamesToMap above
						if (bFound) {
								//#### .Get(the)Property for the current a_sNamesToMap, then reset the current a_sNamesToMap to a null-string
							oProperty = oType.GetProperty(a_sNamesToMap[i]);
							oInput = oInputCollection.Inputs(a_sNamesToMap[i]);
//!
							a_sNamesToMap[i] = "";
							bFound = (oProperty != null);

								//#### If the oProperty was found above
							if (bFound) {
									//#### Determine the passed eDirection and process accordingly
								switch (eDirection) {
									case enumDirection.PopulateBusinessObject: {
										oProperty.SetValue(oBusinessObject, Convert.ChangeType(oInput.Value, oProperty.PropertyType), null);
										break;
									}

									default: { //case enumDirection.PopulateContols: {
											//#### If we .CanRead the oProperty, collect the sValue from the oProperty and set it into the oInput.Value
										if (oProperty.CanRead) {
											oInput.Value = Tools.MakeString(oProperty.GetValue(oBusinessObject, null), string.Empty);
										}
										break;
									}
								}
							}
						}

							//#### If the current a_sNamesToMap was not bFound above, reset our bReturn value to false
						if (! bFound) {
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
		/// <param name="oTo">ControlCollection object to be used as the data destination.</param>
		/// <param name="a_sNamesToMap">Array of strings defining the properties/controls to map.</param>
		/// <returns>Boolean value representing if all of the provided <paramref name="a_sNamesToMap"/> were successfully mapped.</returns>
		///############################################################
		/// <LastUpdated>November 24, 2009</LastUpdated>
		public static bool Map(ControlCollection oFrom, object oTo, string[] a_sNamesToMap) {
				//#### Pass the call into .DoMap, signaling that it is to .Populate(the)BusinessObject
			return DoMap(oFrom, oTo, a_sNamesToMap, enumDirection.PopulateBusinessObject);
		}

		///############################################################
		/// <summary>
		/// Maps data between the provided objects.
		/// </summary>
		/// <param name="oFrom">ControlCollection object to be used as the data source.</param>
		/// <param name="oTo">Object to be used as the data destination.</param>
		/// <param name="a_sNamesToMap">Array of strings defining the properties/controls to map.</param>
		/// <returns>Boolean value representing if all of the provided <paramref name="a_sNamesToMap"/> were successfully mapped.</returns>
		///############################################################
		/// <LastUpdated>November 24, 2009</LastUpdated>
		public static bool Map(object oFrom, ControlCollection oTo, string[] a_sNamesToMap) {
				//#### Pass the call into .DoMap, signaling that it is to .Populate(the)Contols
			return DoMap(oTo, oFrom, a_sNamesToMap, enumDirection.PopulateContols);
		}

			///############################################################
			/// <summary>
			/// Maps data between the provided objects.
			/// </summary>
			/// <param name="oControlCollection">ControlCollection object to be used as a data source/destination.</param>
			/// <param name="oBusinessObject">Object to be used as a data source/destination.</param>
			/// <param name="a_sNamesToMap">Array of strings defining the properties/controls to map.</param>
			/// <param name="eDirection">Enumeration defining the direction of the mapping.</param>
			/// <returns>Boolean value representing if all of the provided <paramref name="a_sNamesToMap"/> were successfully mapped.</returns>
			///############################################################
			/// <LastUpdated>December 11, 2009</LastUpdated>
			private static bool DoMap(ControlCollection oControlCollection, object oBusinessObject, string[] a_sNamesToMap, enumDirection eDirection) {
				PropertyInfo oProperty;
				Type oType;
				string sValue;
				int iNamesCount;
				int i;
				bool bFound;
				bool bReturn = true;

//List<string> l_sNames;
//l_sNames.Remove

					//#### If we have a_sNames to traverse
				if (a_sNamesToMap != null && a_sNamesToMap.Length > 0) {
						//#### Collect the iNamesCount
					iNamesCount = a_sNamesToMap.Length;

						//#### Determine the oType of the passed oBusinessObject
					oType = oBusinessObject.GetType();

						//#### Traverse each oControl within the passed oControlCollection
					foreach (Control oControl in oControlCollection) {
							//#### Traverse the passed a_sNamesToMap
						for (i = 0; i < iNamesCount; i++) {
								//#### Reset bFound for this loop
							bFound = false;

								//#### If the current a_sNamesToMap matches the current oControl's .ID
							if (a_sNamesToMap[i].Length > 0 && a_sNamesToMap[i] == oControl.ID) {
									//#### .Get(the)Property for the current a_sNamesToMap, then reset the current a_sNamesToMap to a null-string (so we don't look for it again)
								oProperty = oType.GetProperty(a_sNamesToMap[i]);
								a_sNamesToMap[i] = "";
								bFound = (oProperty != null);

									//#### If the oProperty was found above
								if (bFound) {
										//#### Determine the passed eDirection and process accordingly
									switch (eDirection) {
										case enumDirection.PopulateBusinessObject: {
												//#### Reset bFound based on if we can .Get(the)Value from the oControl
											bFound = Web.Controls.Tools.GetValue(oControl, out sValue);

												//#### If we successfully collected the sValue above, .Set(the)Value within the oBusinessObject
											if (bFound) {
												oProperty.SetValue(oBusinessObject, Convert.ChangeType(sValue, oProperty.PropertyType), null);
											}
											break;
										}

										default: { //case enumDirection.PopulateContols: {
												//#### If we .CanRead the oProperty
											if (oProperty.CanRead) {
													//#### Collect the sValue from the oProperty and reset bFound based on if the sValue is successfully set into the oControl
												sValue = Tools.MakeString(oProperty.GetValue(oBusinessObject, null), string.Empty);
												bFound = Web.Controls.Tools.SetValue(oControl, sValue);
											}
											break;
										}
									}
								}
							}

								//#### If the current oControl.HasControls, recurse to Map those as well, resetting our own bReturn based on the recursive call
							if (oControl.HasControls()) {
								bReturn = DoMap(oControl.Controls, oBusinessObject, a_sNamesToMap, eDirection);
							}

								//#### If the current a_sNamesToMap was not bFound above, reset our bReturn value to false
							if (! bFound) {
								bReturn = false;
							}
						}
					}
				}

					//#### Return the above determined bReturn value to the caller
				return bReturn;
			}


        //##########################################################################################
        //# Private Functions
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Sets the referenced property within the provided object.
		/// </summary>
		/// <param name="oObject">Object where the property is to be set.</param>
		/// <param name="a_sPath">Array of Strings representing the path to the requested property.</param>
		/// <param name="oPropertyObject">Returned Object representing the object containing the requested property.</param>
		/// <param name="sPropertyName">Returned String representing the name of the requested property.</param>
		/// <returns>Boolean value indicating if we successfully located the requested property.</returns>
		///############################################################
		/// <LastUpdated>February 12, 2010</LastUpdated>
		private static bool LocatePropertyObject(object oObject, string[] a_sPath, out object oPropertyObject, out string sPropertyName) {
			object oPreviousObject;
			object oCurrentObject;
			int iLength;
			int i;
			bool bReturn = true;

				//#### Default the out parameters
			oPropertyObject = null;
			sPropertyName = "";

				//#### If the passed a_sPath is holding valid values
			if (a_sPath != null && a_sPath.Length > 0) {
					//#### Set the oPreviousObject to the passed oObject and determine the a_sPath's .Length
				oPreviousObject = oObject;
				iLength = a_sPath.Length;

					//#### Traverse all but the last element of the a_sPath
				for (i = 0; i < (iLength - 1); i++) {
						//#### If we couldn't collect the oCurrentObject, flip our bReturn to false and fall from the loop
					if (! GetPropertyValue(oPreviousObject, a_sPath[i], out oCurrentObject)) {
						bReturn = false;
						break;
					}

						//#### Reset oPreviousObject to the oCurrentObject in prep for the next loop
					oPreviousObject = oCurrentObject;
				}

					//#### If the a_sPath was successfully traversed above (as if it wasn't, our bReutrn value was flipped)
				if (bReturn) {
						//#### Reset the out parameters to the located oPropertyObject and sPropertyName
					oPropertyObject = oPreviousObject;
					sPropertyName = a_sPath[iLength - 1];
				}
			}

				//#### Return the above determined bReturn value to the caller
			return bReturn;
		}


	} //# class Mirror


} //# namespace Cn.Data

