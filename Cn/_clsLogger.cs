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
using System.Collections.Generic;
using System.Text;


namespace Cn {
/*
    ///########################################################################################################################
    /// <summary>
	/// 
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>March 22, 2007</LastFullCodeReview>
#region Logger
	class Logger {
			//#### Define the required private/protected variables
private string g_oFileHandle;
	    private string g_sFilePath;
	    private bool g_bEnabled;


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class based on the provided column names.
		/// </summary>
		/// <seealso cref="Cn.Logger.Reset"/>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		public Logger() {
				//#### Call the private .DoReset to init the class vars
//!			DoReset("[Constructor]", false, ?);
		}

		///############################################################
		/// <summary>
		/// Initializes the class based on the provided column names.
		/// </summary>
		/// <seealso cref="Cn.Logger.Reset"/>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		public Logger(bool bEnableLogging) {
				//#### Call the private .DoReset to init the class vars
//!			DoReset("[Constructor]", bEnableLogging, ?);
		}

		///############################################################
		/// <summary>
		/// Initializes the class based on the provided column names.
		/// </summary>
		/// <seealso cref="Cn.Logger.Reset"/>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		public Logger(bool bEnableLogging, string sFilePath) {
				//#### Call the private .DoReset to init the class vars
//!			DoReset("[Constructor]", bEnableLogging, sFilePath);
		}

		///############################################################
		/// <summary>
		/// Cleans up the class before terminiation.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		~Logger() {
				//#### Ensure that the g_oFileHandle has been closed before we terminate
			CloseLogFile();
		}


		//##########################################################################################
		//# Public Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Gets/sets 
		/// </summary>
		/// <value></value>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		public bool Enabled {
			get {
				return g_bEnabled;
			}
			set {
				g_bEnabled = value;
			}
		}


		//##########################################################################################
		//# Public Read-only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Get the path to the log file that is referenced by this instance.
		/// </summary>
		/// <value>String representing the path to the log file that is referenced by this instance.</value>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		public string FilePath {
			get {
				return g_sFilePath;
			}
		}


		//##########################################################################################
		//# Public Write-only Properties
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Logs the provided string into the log file referenced by this instance.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		public string Log {
			set {
					//#### If we are supposed to be logging to the g_sFilePath
				if (g_bEnabled) {
						//#### If the g_sFilePath has not yet been opened
					if (g_oFileHandle == null) {
//!						g_oFileHandle = something
					}

						//#### 
//!					Debug.Print(value);

						//#### 
//!					g_oFileHandle.WriteLine(value);
				}
			}
		}


		//##########################################################################################
		//# Public Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		public void Reset() {
				//#### Call the private .DoReset to reset the class vars
//!			DoReset("Reset", false, ?);
		}

		///############################################################
		/// <summary>
		/// Resets the class based on the provided data.
		/// </summary>
		/// <seealso cref="Cn.Logger.Reset"/>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		public void Reset(bool bEnableLogging) {
				//#### Call the private .DoReset to reset the class vars
//!			DoReset("Reset", bEnableLogging, ?);
		}

		///############################################################
		/// <summary>
		/// Resets the class based on the provided data.
		/// </summary>
		/// <seealso cref="Cn.Logger.Reset"/>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		public void Reset(bool bEnableLogging, string sFilePath) {
				//#### Call the private .DoReset to reset the class vars
//!			DoReset("Reset", bEnableLogging, sFilePath);
		}

		///############################################################
		/// <summary>
		/// Closes the log file referenced by this instance.
		/// </summary>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		public void CloseLogFile() {
				//#### If we have an open g_oFileHandle, close it
			if (g_oFileHandle != null) {
//!				g_oFileHandle.Close();
			}
		}


		//##########################################################################################
		//# Private Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Resets the class to its initilized state.
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="bEnableLogging"></param>
		/// <param name="sFilePath"></param>
		///############################################################
		/// <LastUpdated>March 22, 2007</LastUpdated>
		private void DoReset(string sFunction, bool bEnableLogging, string sFilePath) {
				//#### Ensure that the g_oFileHandle has been closed before we reset
			CloseLogFile();
		}

	}
#endregion
*/
} //# namespace Cn
