/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;										//# Required to access Enum.
using System.Collections.Generic;
using System.Data;
using System.IO;									//# Required to access File.
using System.Collections;					        //# Required to access the Hashtable class
using System.Configuration;
using System.Reflection;

//# Required to access ConfigurationSettings class.


namespace Cn.Platform {

    ///########################################################################################################################
    /// <summary>
	/// Language/enviroment specific helper methods (dotNET, PHP, Java, etc).
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>May 10, 2007</LastFullCodeReview>
	public class Specific {

        //#######################################################################################################
        //# Public Functions
        //#######################################################################################################
        ///###############################################################
		/// <summary>
		/// Abstraction layer dotNet's data retrieval from the application settings section of the configuration file.
		/// </summary>
		/// <param name="sName">String representing the name of the application settings entry to retrieve.</param>
		/// <returns>String representing the value for the referenced name from the application settings section of the configuration file.</returns>
        ///###############################################################
		/// <LastUpdated>May 28, 2007</LastUpdated>
		public static string AppSettings(string sName) {
				//#### Depricated dotNet 1.x function
			return ConfigurationSettings.AppSettings[sName];

				//#### dotNet 2.x function
			//!return System.Configuration.?
		}

/*        ///###############################################################
		/// <summary>
		/// Convert a List to a DataTable.
		/// </summary>
		/// <remarks>
		/// Based on MIT-licensed code presented at http://www.chinhdo.com/20090402/convert-list-to-datatable/ as "ToDataTable"
		/// <para/>Code modifications made by Nick Campbell.
		/// <para/>Source code provided on this web site (chinhdo.com) is under the MIT license.
		/// <para/>Copyright © 2010 Chinh Do
		/// <para/>Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
		/// <para/>The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
		/// <para/>THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
		/// <para/>(As per http://www.chinhdo.com/20080825/transactional-file-manager/)
		/// </remarks>
		/// <typeparam name="T">Type representing the type to convert.</typeparam>
		/// <param name="l_oItems">List of requested Type representing the values to convert.</param>
		/// <returns></returns>
        ///###############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static DataTable ToDataTable<T>(List<T> l_oItems) {
			DataTable oReturn = new DataTable(typeof(T).Name);
			PropertyInfo[] a_oProperties;
			object[] a_oValues;
			int i;

				//#### Collect the a_oProperties for the passed T
			a_oProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

				//#### Traverse each oProperty, .Add'ing each .Name/.BaseType into our oReturn value
				//####     NOTE: The call to .BaseType is required as DataTables/DataSets do not support nullable types, so it's non-nullable counterpart Type is required in the .Column definition
			foreach(PropertyInfo oProperty in a_oProperties) {
				oReturn.Columns.Add(oProperty.Name, BaseType(oProperty.PropertyType));
			}

				//#### Traverse the l_oItems
			foreach (T oItem in l_oItems) {
					//#### Collect the a_oValues for this loop
			    a_oValues = new object[a_oProperties.Length];

					//#### Traverse the a_oProperties, populating each a_oValues as we go
				for (i = 0; i < a_oProperties.Length; i++) {
					a_oValues[i] = a_oProperties[i].GetValue(oItem, null);
				}

					//#### .Add the .Row that represents the current a_oValues into our oReturn value
				oReturn.Rows.Add(a_oValues);
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

        ///###############################################################
		/// <summary>
		/// Returns the underlying/base type of nullable types.
		/// </summary>
		/// <remarks>
		/// Based on MIT-licensed code presented at http://www.chinhdo.com/20090402/convert-list-to-datatable/ as "GetCoreType"
		/// <para/>Code modifications made by Nick Campbell.
		/// <para/>Source code provided on this web site (chinhdo.com) is under the MIT license.
		/// <para/>Copyright © 2010 Chinh Do
		/// <para/>Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
		/// <para/>The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
		/// <para/>THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
		/// <para/>(As per http://www.chinhdo.com/20080825/transactional-file-manager/)
		/// </remarks>
		/// <param name="oType">Type representing the type to query.</param>
		/// <returns>Type representing the underlying/base type.</returns>
        ///###############################################################
		/// <LastUpdated>February 15, 2010</LastUpdated>
		public static Type BaseType(Type oType) {
				//#### If the passed oType is valid, .IsValueType and is logicially nullable, .Get(its)UnderlyingType
			if (oType != null && oType.IsValueType &&
				oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(Nullable<>)
			) {
				return Nullable.GetUnderlyingType(oType);
			}
				//#### Else the passed oType was null or was not logicially nullable, so simply return the passed oType
			else {
				return oType;
			}
		}
*/

    	public static string[] FilePaths(string sPath, string sFileMask, bool bIncludeSubDirectories) {
	string[] a_sReturn = null;
	DirectoryInfo oRootDir;
	FileInfo[] oFiles;
	int i = 0;

		//#### If the passed sPath .Exists
	if (Directory.Exists(sPath)) {
			//#### Grab the oRootDir for the passed sPath
		oRootDir = new DirectoryInfo(sPath);
//		DirectoryInfo[] oDirs = oRootDir.GetDirectories("*", SearchOption.AllDirectories);

			//#### If we are supposed to bIncludeSubDirectories, collect the oFiles from .AllDirectories
		if (bIncludeSubDirectories) {
			oFiles = oRootDir.GetFiles(sFileMask, SearchOption.AllDirectories);
		}
			//#### Else we're not supposed to bIncludeSubDirectories, so only collect the oFiles for the .TopDirectoryOnly
		else {
			oFiles = oRootDir.GetFiles(sFileMask, SearchOption.TopDirectoryOnly);
		}

			//#### If we found some oFiles above
		if (oFiles.Length > 0) {
				//#### Dimension our a_sReturn value to fit all of the oFiles names
			a_sReturn = new string[oFiles.Length];

				//#### Store each oFile.FullName into the a_sReturn value (post incrementing i as we go)
			foreach (FileInfo oFile in oFiles) {
				a_sReturn[i++] = oFile.FullName;
			}
		}
	}

		//#### Return the above determined a_sReturn value to the caller
	return a_sReturn;
}

        ///###############################################################
		/// <summary>
		/// Retrieves the contents of the referenced file.
		/// </summary>
		/// <param name="sPathToFile">String representing realtive or absolute path to the file to read.</param>
		/// <returns>String representing the contents of the referenced file.</returns>
        ///###############################################################
		/// <LastUpdated>February 6, 2006</LastUpdated>
		public static string ReadFromFile(string sPathToFile) {
			StreamReader oStreamReader;
			string sReturn;

				//#### Attempt to read the passed sPathToFile, setting the sReturn value to the contents of the file
			try {
				oStreamReader = File.OpenText(sPathToFile);
				sReturn = oStreamReader.ReadToEnd();
				oStreamReader.Close();
			}
				//#### Else an error has occured, so set the sReturn value to a null-string
			catch {
				sReturn = "";
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}


        //#######################################################################################################
        //# Public DeepCopy-related Functions
        //#######################################################################################################
        ///###############################################################
	    /// <summary>
		/// Retrieves a deep copy of the data stored within the passed structure.
	    /// </summary>
	    /// <remarks>
	    /// The returned hashtable contains a deep copy of only the primitive/value types contained within the passed <paramref>hHash</paramref>. Object/reference types are represented by a new reference (pointer) to the object, but still refer to the same instance refereced to within the passed <paramref>hHash</paramref>.
	    /// </remarks>
		/// <param name="hHash">Hashtable representing the data to copy.</param>
		/// <returns>Deep copy of the passed <paramref>hHash</paramref> hashtable.</returns>
        ///###############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
	    public static Hashtable DeepCopy(Hashtable hHash) {
		    Hashtable hReturn = null;
		    string[] a_sKeys = null;
		    int iCount;
		    int i;

				//#### If the caller passed in a valid hHash
			if (hHash != null) {
					//#### Determine the iCount and (re)init the hReturn value
				iCount = hHash.Count;
				hReturn = new Hashtable();
				
					//#### If the passed hHash has a_sKeys, dimension the a_sKeys accordingly then retrieve the a_sKeys from the hHash
				if (iCount > 0) {
					a_sKeys = new string[iCount];
					hHash.Keys.CopyTo(a_sKeys, 0);
				}

					//#### Traverse a_sKeys explicitly copying each entry into the hReturn value
					//####     NOTE: This copy only deep-copies primitive types. Any pointers/references are copied only as pointers/references to the same object in memory
				for (i = 0; i < iCount; i++) {
					hReturn.Add(a_sKeys[i], hHash[a_sKeys[i]]);
				}
			}

                //#### Return the above determined hReturn value to the caller
		    return hReturn;
	    }

        ///###############################################################
	    /// <summary>
		/// Retrieves a deep copy of the data stored within the passed structure.
	    /// </summary>
		/// <param name="a_sArray">Array of strings representing the data to copy.</param>
		/// <returns>Deep copy of the passed <paramref>a_sArray</paramref> array of strings.</returns>
        ///###############################################################
		/// <LastUpdated>June 4, 2007</LastUpdated>
		public static string[] DeepCopy(string[] a_sArray) {
			string[] a_sReturn = null;
			int iCount;
			int i;

				//#### If the caller passed in a valid a_sArray
			if (a_sArray != null) {
					//#### Determine the iCount and dimension the a_sReturn value accordingly
				iCount = a_sArray.Length;
				a_sReturn = new string[iCount];

					//#### Traverse the passed a_sArray, copying each index into the a_sReturn value
				for (i = 0; i < iCount; i++) {
					a_sReturn[i] = a_sArray[i];
				}
			}

				//#### Return the above determined a_sReturn value to the caller
			return a_sReturn;
		}

        ///###############################################################
	    /// <summary>
		/// Retrieves a deep copy of the data stored within the passed structure.
	    /// </summary>
		/// <remarks>
		/// Based on the unlicensed example presented by KDDatacore (http://www.datacore.co.za/FileCopy.aspx)
		/// </remarks>
		/// <param name="oStream">Stream representing the data to copy.</param>
		/// <returns>A MemoryStream object containing a deep copy of the passed <paramref>oStream</paramref>.</returns>
        ///###############################################################
		/// <LastUpdated>October 12, 2007</LastUpdated>
		public static MemoryStream DeepCopy(Stream oStream) {
			MemoryStream oReturn = new MemoryStream();

				//#### Pass the call off to our sibling implementation
			DeepCopy(oStream, oReturn);

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

		public static void DeepCopy(Stream oStream, Stream oDestination) {
				//#### 
			CopyStreamToStream(oStream, oDestination, 0, (int)oStream.Length, 0);

/*			Stream oReturn = new MemoryStream();
			BinaryReader oReader = new BinaryReader(oStream);
			BinaryWriter oWriter = new BinaryWriter(oReturn);
			byte[] a_byteBuffer;
			int iBufferLength = 32768;
			int iReadLength;

				//#### If the passed oStream .CanSeek, .Read the entire oStream.Length into the oWriter
			if (oStream.CanSeek) {
				oWriter.Write(oReader.ReadBytes((int)oStream.Length));
			}
				//#### Else the passed oStream .Can(not)Seek
			else {
					//#### Setup the a_byteBuffer to 32kb
				a_byteBuffer = new byte[iBufferLength];

					//#### Do the "half" of the "loop and a half", .Read'ing the first section of the oStream
				iReadLength = oStream.Read(a_byteBuffer, 0, iBufferLength);

					//#### While we still have a iReadLength in the a_byteBuffer
				while (iReadLength > 0) {
						//#### .Write the a_byteBuffer's iReadLength into the oWriter, then .Read the next 32kb section of the oStream
					oWriter.Write(a_byteBuffer, 0, iReadLength);
					iReadLength = oStream.Read(a_byteBuffer, 0, iBufferLength);
				}
			}

				//#### .Flush the buffers of both the oWriter and our own oReturn value
			oWriter.Flush();
			oReturn.Flush();

				//#### Return the above determined oReturn value to the caller
			return oReturn;
*/
		}


		private static void CopyStreamToStream(Stream p_fromStream, Stream p_toStream, int p_fromStreamStartOffset, int p_fromStreamCopyRange, int p_toStreamCopyToStartOffset) {
			byte[] buffer;
			if ((p_fromStreamCopyRange + p_fromStreamStartOffset) > p_fromStream.Length) {
				p_fromStreamCopyRange = (int)p_fromStream.Length - p_fromStreamStartOffset;
			}

			//Specify how many bytes we wish to copy at a time
			int bufferLength = GetOptimisedBlockSize(p_fromStreamCopyRange);

			if (bufferLength > p_fromStreamCopyRange) {
				 bufferLength = p_fromStreamCopyRange;
			}
			p_fromStream.Position = p_fromStreamStartOffset;
			p_toStream.Position = p_toStreamCopyToStartOffset;

			//The main copying loop
			for (int i = 0; i < p_fromStreamCopyRange; i += bufferLength) {
				if ((i + bufferLength) > p_fromStreamCopyRange) {
					 bufferLength = p_fromStreamCopyRange - i;
				}
				buffer = new byte[bufferLength];
				p_fromStream.Read(buffer,0,bufferLength);
				p_toStream.Write(buffer,0,bufferLength);
			}
		}


		/// Always return the nearest KB or MB rounded of value ten times less than the p_size parameter. 
		/// If the resultant value is larger than 64Mb then check that 64Mb is less than one tenth of the available memory, if 
		/// it is larger then get the closest rounded of value to one tenth of the available memory; 
		/// <param name="p_size"/>The size of the data that will be needed to be split up 
		 private static int GetOptimisedBlockSize(int p_size) {
			int blockSize = 1024;
			int Mb64 = (1024 * 1024 * 64);
			for (int i = 0; i < 50; i++) {
				if ((p_size/10) <= (blockSize) ) {
					 if (blockSize > Mb64) {
						System.Diagnostics.PerformanceCounter ramCounter; ramCounter = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
						int oneTenthRam = ((int)ramCounter.NextValue() / 10);
						if (oneTenthRam > 64) {
							return blockSize;
						}
						else {
							blockSize = Mb64;
							int oneTenthRamInBytes = (oneTenthRam * 1024 * 1024);
							for (int j = 10; j > 1 ; j++) {
							   if (blockSize <= oneTenthRamInBytes) {
								   return blockSize;
							   }
							   blockSize = (blockSize >> 1); //Half it with a shift
							}
							return (1024 * 1024);
						}
					}
					else {
						return blockSize;
					}
				}
				blockSize = blockSize << 1; //Double
			}

			return (1024 * 1024 * 64);
		}



	} //# public class Specific

} //# namespace Cn.Platform
