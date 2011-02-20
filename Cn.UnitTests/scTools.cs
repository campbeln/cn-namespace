/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Cn.UnitTests {
	class Tools {

		public static T Binary_Serialize_Deserialize<T>(T oObject) {
			BinaryFormatter oBinaryFormatter = new BinaryFormatter();
			MemoryStream oMemoryStream = new MemoryStream();
			T oReturn; // = oObject;

				//#### If the passed oObject is non-null
			if (oObject != null) {
					//#### .Serialize the passed oObject into the oBinaryFormatter, then reset the oMemoryStream to the .Begin'ing
				oBinaryFormatter.Serialize(oMemoryStream, oObject);
				oMemoryStream.Seek(0, SeekOrigin.Begin);

					//#### .Deserialize the sSerializedXML back into a <T>, returning the result to the caller
				oReturn = (T)oBinaryFormatter.Deserialize(oMemoryStream);
			}
				//#### Else the passed oObject was null, so throw the error
			else {
				throw new ArgumentNullException("oObject");
			}

				//#### Return the above determined oReturn value to the caller
			return oReturn;
		}

	}
}
