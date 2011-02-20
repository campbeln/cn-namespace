/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Text;                                  //# Required to access the StringBuilder class
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Web {

    ///########################################################################################################################
    /// <summary>
	/// General helper methods.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>July 29, 2005</LastFullCodeReview>
	public class JavaScript {
			//#### Declare the required public eNums
		#region eNums
			/// <summary>Renderer JavaScript functionality types.</summary>
		public enum enumJavaScriptFiles : int {
				/// <summary>Cn JavaScript namespace.</summary>
			cnCn = 1,
				/// <summary>Cn.Tools JavaScript class.</summary>
			cnCnTools = 2,

				/// <summary>Cn.Inputs & Cn.Inputs.Form JavaScript classes.</summary>
			cnCnInputs = 8,
				/// <summary>Cn.Inputs.Validation & Cn.Inputs.Errors JavaScript classes.</summary>
			cnCnInputsValidation = 16,
				/// <summary>Cn.Inputs.DateTime JavaScript class.</summary>
			cnCnInputsDateTime = 32,
				/// <summary>Cn.Inputs.ComboBox JavaScript class.</summary>
			cnCnInputsComboBox = 64,
				/// <summary>Cn.Renderer.Form.HTMLEditor JavaScript class.</summary>
			cnCnInputsHTMLEditor = 128,
				/// <summary>Cn.Renderer.Form.MaxLength JavaScript class.</summary>
			cnCnInputsMaxLength = 256,
				/// <summary>Cn.Web.Inputs.Radio JavaScript class.</summary>
			cnCnInputsRadio = 512,

				/// <summary>Cn.Renderer.Form JavaScript class.</summary>
			cnCnRendererForm = 1024,
				/// <summary>Cn.Renderer.ComplexSorter JavaScript class.</summary>
			cnCnRendererComplexSorter = 2048,
				/// <summary>Cn.Renderer.UserSelectedStack JavaScript class.</summary>
			cnCnRendererUserSelectedStack = 4096,

				/// <summary>YAHOO JavaScript namespace.</summary>
			cnYUI = 8192,
				/// <summary>YAHOO.widget.Calendar_Core JavaScript class.</summary>
			cnYUICalendar = 16384,
				/// <summary>YAHOO.widget.Event JavaScript class.</summary>
			cnYUIEvent = 32768,
				/// <summary>YAHOO.utils.Dom JavaScript class.</summary>
			cnYUIDOM = 65536,
//			cnYUIDateMath = 131072,

				/// <summary>FCKeditor Javascript functionality.</summary>
			cnYUIRichTextEditor = 262144,

				/// <summary>Renders all of the available JavaScript classes and namespaces.</summary>
			cnRenderAllJavaScript = 0,

				/// <summary>Disables all JavaScript rendering from <see cref='JavaScript'>Renderer.JavaScript</see>.</summary>
			cnDisableJavaScriptRendering = -1
		}
		#endregion

            //#### Declare the required private constants
	    private const string g_cClassName = "Cn.Web.JavaScript.";

			//#### Declare the required private, developer modifiable constants
			//####     RealtiveJavaScript: Defines the realtive path under the Cn.Web.Settings.Current.BaseDirectory to the Cn Namespace JavaScript oFiles.
			//####         NOTE: Always end these values with a trailing slash! Also since they are realtive paths under the .BaseDirectory, a leading slash is not recommended.
			//####     ScriptFileExtension: Defines the scripting file extension without a leading period (i.e.: "aspx", "php", etc).
			//####     BlockStart/BlockEnd: Defines the strings that start and end JavaScript code blocks.
		private const string g_cJavaScriptDebugDirectory = "js/Debug/";
		private const string g_cJavaScriptDirectory = "js/";
//!private const string g_cJavaScriptDirectory = "js/Debug/";
		private const string g_cServerSideScriptFileExtension = "aspx";
        private const string g_cBlockStart = "<script type='text/javascript'>/*<![CDATA[*/"; //"<script type='text/javascript'>/*<![CDATA[*/";
        private const string g_cBlockEnd = "//]]></script>"; //"//]]></script>";


        //##########################################################################################
        //# Public Static Read-Only Properties
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Gets the start of a JavaScript code block.
		/// </summary>
		///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
		public static string BlockStart {
			get { return g_cBlockStart; }
		}

		///############################################################
		/// <summary>
		/// Gets the end of a JavaScript code block.
		/// </summary>
		///############################################################
		/// <LastUpdated>May 15, 2007</LastUpdated>
		public static string BlockEnd {
			get { return g_cBlockEnd; }
		}

		///############################################################
		/// <summary>
		/// Gets the file extension of the defined server side scripting enviroment.
		/// </summary>
		///############################################################
		/// <LastUpdated>January 8, 2010</LastUpdated>
		public static string ServerSideScriptFileExtension {
			get { return g_cServerSideScriptFileExtension; }
		}


        //##########################################################################################
        //# Public Static Functions
        //##########################################################################################
		///############################################################
		/// <summary>
		/// Retrieves the defined UI directory from the webserver's root.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the defined UI directory from the webserver's root.</returns>
		///############################################################
		/// <LastUpdated>June 18, 2010</LastUpdated>
		public static string BaseDirectory(Settings.Current oSettings) {
				//#### Pass the call off to our sibling implementation, returning its result as our own
			return BaseDirectory(oSettings.Debug);
		}

		///############################################################
		/// <summary>
		/// Retrieves the defined UI directory from the webserver's root.
		/// </summary>
		/// <param name="bDebug">Boolean representing the debug status of the current enviroment.</param>
		/// <returns>String representing the defined UI directory from the webserver's root.</returns>
		///############################################################
		/// <LastUpdated>June 18, 2010</LastUpdated>
		public static string BaseDirectory(bool bDebug) {
			string sReturn = Settings.Value(Settings.enumSettingValues.cnUIDirectory);

				//#### If we are in bDebug mode, append the g_cJavaScriptDebugDirectory onto our sReturn value
			if (bDebug) {
				sReturn += g_cJavaScriptDebugDirectory;
			}
				//#### Else we are not in bDebug mode, so append the g_cJavaScriptDirectory onto our sReturn value
			else {
				sReturn += g_cJavaScriptDirectory;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Retrieves the defined UI URL.
		/// </summary>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the defined UI URL.</returns>
		///############################################################
		/// <LastUpdated>June 18, 2010</LastUpdated>
		public static string BaseURL(Settings.Current oSettings) {
				//#### Pass the call off to our sibling implementation, returning its result as our own
			return BaseURL(oSettings.Debug);
		}

		///############################################################
		/// <summary>
		/// Retrieves the defined UI URL.
		/// </summary>
		/// <param name="bDebug">Boolean representing the debug status of the current enviroment.</param>
		/// <returns>String representing the defined UI URL.</returns>
		///############################################################
		/// <LastUpdated>June 18, 2010</LastUpdated>
		public static string BaseURL(bool bDebug) {
			string sReturn = Settings.Value(Settings.enumSettingValues.cnUIDirectoryURL);

				//#### If we are in bDebug mode, append the g_cJavaScriptDebugDirectory onto our sReturn value
			if (bDebug) {
				sReturn += g_cJavaScriptDebugDirectory;
			}
				//#### Else we are not in bDebug mode, so append the g_cJavaScriptDirectory onto our sReturn value
			else {
				sReturn += g_cJavaScriptDirectory;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Transforms the passed array into the JavaScript inline definition equivlent.
		/// </summary>
		/// <param name="a_sArray">Array of strings to convert into a JavaScript array of strings.</param>
		/// <returns>String representing the JavaScript inline definition equivlent of the passed <paramref>a_sArray</paramref>.</returns>
		///############################################################
		/// <LastUpdated>August 7, 2007</LastUpdated>
		public static string ToArray(string[] a_sArray) {
			StringBuilder oReturn = new StringBuilder();
			string sReturn;
			int i;

				//#### If the passed a_sArray is valid
			if (a_sArray != null && a_sArray.Length > 0) {
					//#### Treverse all but the last element of the passed a_sArray, .Append'ing on each element (followed by the JS array delimiter and escaping) as we go
				for (i = 0; i < (a_sArray.Length - 1); i++) {
					oReturn.Append(a_sArray[i].Replace("'", "\\'") + "','");
				}

					//#### Pre/append the JavaScript-ish inline array code onto the sReturn value (while appending the final element within the a_sArray)
				sReturn = "['" + oReturn + a_sArray[i].Replace("'", "\\'") + "']";
			}
				//#### Else the passed a_sArray was empty, so set the sReturn value to "null"
			else {
				sReturn = "null";
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

        ///############################################################
        /// <summary>
        /// Renders the requested JavaScript file references.
        /// </summary>
        /// <remarks>
        /// NOTE: <c>cnCnRendererComplexSorter</c> is not yet implemented.
        /// </remarks>
        /// <param name="eJavaScriptFile">Enumeration representing the JavaScript/DHTML code to return.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
        /// <returns>String representing the required JavaScript file script block(s) for the referenced <paramref>eJavaScriptFile</paramref>.</returns>
        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eJavaScriptFile</paramref> is unreconized.</exception>
        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eJavaScriptFile</paramref> is <c>cnRenderAllJavaScript</c> and the referenced <paramref>oSettings.RenderedJavaScriptFiles</paramref> signanifies that some or all of the JavaScript file script blocks have been rendered previously.</exception>
        ///############################################################
        /// <LastUpdated>March 3, 2010</LastUpdated>
        public static string GenerateFileReferences(enumJavaScriptFiles eJavaScriptFile, Settings.Current oSettings) {
            string sPath = Settings.Value(Settings.enumSettingValues.cnUIDirectory);
            string sCRLF = oSettings.CRLF;
            string sReturn;
            int iRenderedJavaScriptFiles = oSettings.GeneratedJavaScriptFileReferences;

                //#### If we are in .Debug mode, append g_cJavaScriptDebugDirectory onto the sPath
            if (oSettings.Debug) {
                sPath += g_cJavaScriptDebugDirectory;
            }
				//#### Else we are not in .Debug mode, so append g_cJavaScriptDirectory onto the sPath
			else {
				sPath += g_cJavaScriptDirectory;
			}

				//#### Pass the call off to .DoRender to generate the required JavaScript file references, Response.Write'ing out the returned value
			sReturn = DoGetScriptReferences(eJavaScriptFile, ref iRenderedJavaScriptFiles, oSettings.EndUserMessagesLanguageCode, sPath, sCRLF, oSettings.Debug);

				//#### Reset oSettings's .RenderedJavaScriptFiles to the local iRenderedJavaScriptFiles
				//####     NOTE: This is required because you cannot pass a property byref (as is required by the .DoGetScriptReferences call above)
			oSettings.GeneratedJavaScriptFileReferences = iRenderedJavaScriptFiles;

				//#### Return the above determined value to the caller
			return sReturn;
		}

        ///############################################################
		/// <summary>
		/// Determines the referenced HTML Form's ID via JavaScript code.
		/// </summary>
		/// <remarks>
		/// NOTE: If the passed <paramref name="sFormIDNameOrIndex"/> is null or a null-string, the ID of the first HTML Form will be returned.
		/// </remarks>
		/// <param name="sFormIDNameOrIndex">String representing the HTML Form's ID, Name or Index.</param>
		/// <returns>String containing JavaScript code to determine the referenced HTML Form's ID.</returns>
        ///############################################################
        /// <LastUpdated>November 23, 2009</LastUpdated>
		public static string GetFormID(string sFormIDNameOrIndex) {
			string sReturn;
		
				//#### If the passed sFormIDNameOrIndex is null(-string) or numeric, force it into an integer
				//####     NOTE: A null(-string) sFormIDNameOrIndex is caught here and forced into a "0" so that by default the first form is utilized if no g_sFormID was specified
			if (string.IsNullOrEmpty(sFormIDNameOrIndex) || Data.Tools.IsNumber(sFormIDNameOrIndex)) {
				sReturn = Data.Tools.MakeInteger(sFormIDNameOrIndex, 0).ToString();
			}
				//#### Else sFormIDNameOrIndex is non-null and non-numeric, so .Escape(the)Characters and surround it with qoutes
			else {
				sReturn = "'" + Inputs.Tools.EscapeCharacters(sFormIDNameOrIndex, "'") + "'";
			}

				//#### Embed the above-determined sReturn value into the JavaScript code and return it to the caller
			return "document.forms[" + sReturn + "].id";
		}


        //##########################################################################################
        //# Private Static Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Returns the requested JavaScript file references.
        /// </summary>
        /// <remarks>
        /// NOTE: <c>cnCnRendererComplexSorter</c> is not yet implemented.
        /// </remarks>
        /// <param name="eJavaScriptFile">Enumeration representing the JavaScript/DHTML code to return.</param>
		/// <param name="iRenderedJavaScriptFiles">Reference to an integer representing a bitwise value indicating which JavaScript file references have been rendered previously.</param>
		/// <param name="sEndUserMessagesLanguageCode">String representing the end user's ISO639 2-letter language code.</param>
		/// <param name="sPath">String representing the path from the web oRootDir to the JavaScript oFiles.</param>
		/// <param name="sCRLF">String representing the line break character(s) to be included in the return value.</param>
        /// <returns>String representing the required JavaScript file script block(s) for the referenced <paramref>eJavaScriptFile</paramref>.</returns>
        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eJavaScriptFile</paramref> is unreconized.</exception>
        /// <exception cref="Cn.CnException">Thrown when the passed <paramref>eJavaScriptFile</paramref> is <c>cnRenderAllJavaScript</c> and the passed <paramref>iRenderedJavaScriptFiles</paramref> signanifies that some or all of the JavaScript file script blocks have been rendered previously.</exception>
        ///############################################################
        /// <LastUpdated>March 3, 2010</LastUpdated>
        private static string DoGetScriptReferences(enumJavaScriptFiles eJavaScriptFile, ref int iRenderedJavaScriptFiles, string sEndUserMessagesLanguageCode, string sPath, string sCRLF, bool bDebug) {
            string sReturn = "";

                //#### As long as the iRenderedJavaScriptFiles is not set to .cnDisableJavaScriptRendering and the passed eJavaScriptFile has not yet been rendered
            if (iRenderedJavaScriptFiles != (int)enumJavaScriptFiles.cnDisableJavaScriptRendering && (iRenderedJavaScriptFiles & (int)eJavaScriptFile) == 0) {
                    //#### Determine the passed eJavaScriptFile and process accordingly
                switch (eJavaScriptFile) {
                        //#### If this is a .cnCn eJavaScriptFile request, reset the sReturn value accordingly
                    case enumJavaScriptFiles.cnCn: {
                        sReturn = "<script type='text/javascript' src='" + sPath + "Cn/Cn.js'></script>" + sCRLF +
							"<script type='text/javascript' src='" + sPath + "Cn/Cn.js." + g_cServerSideScriptFileExtension + "'></script>" + sCRLF;
                        break;
                    }

                        //#### If this is a .cnCnTools eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnTools: {
                            //#### Recurse to collect the JavaScript for .cnCn and .cnYUIDOM (as they are required for this eJavaScriptFile) before appending the script tag
                            //####     NOTE: Most of the .cnCn* calls below rely on the call below in order to collect their own copy of .cnCn
                            //####     NOTE: Since .cnYUIDOM also requires .cnYUI, it is not included below (as the .cnYUIDOM call will pick it up)
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnCn, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnYUIDOM, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnYUIEvent, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "Cn/Data/Tools.js'></script>" + sCRLF +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Tools.js'></script>" + sCRLF;
                        break;
                    }

					//##########
					//##########

                        //#### If this is a .cnCnInputs eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnInputs: {
                            //#### Recurse to collect the JavaScript for .cnCn and .cnCnTools (as they are required for this eJavaScriptFile) before appending the script tag
                            //####     NOTE: Since .cnCnSettings also requires .cnCn, it is not included below (as the .cnCnTools call will pick it up)
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnCnTools, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Inputs/Inputs.js'></script>" + sCRLF;
                        break;
                    }

                        //#### If this is a .cnCnInputsValidation eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnInputsValidation: {
                            //#### Recurse to collect the JavaScript for .cnCn, .cnCnTools, .cnCnInputs and .cnYUIDOM (as they are required for this eJavaScriptFile) before appending the script tag
                            //####     NOTE: Since .cnCnSettings also requires .cnCn, it is not included below (as the .cnCnTools call will pick it up)
                            //####     NOTE: Since .cnYUIDOM also requires .cnYUI, it is not included below (as the .cnYUIDOM call will pick it up)
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnCn, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnCnTools, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnCnInputs, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnYUIDOM, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Inputs/Validation.js'></script>" + sCRLF +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Inputs/Validation.js." + g_cServerSideScriptFileExtension +
                                "?LanguageCode=" + sEndUserMessagesLanguageCode +
                            "'></script>" + sCRLF;
                        break;
                    }

                        //#### If this is a .cnCnInputsDateTime eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnInputsDateTime: {
                            //#### Recurse to collect the JavaScript for .cnCn, .cnCnTools and .cnYUICalendar (as they are required for this eJavaScriptFile) before appending the script tag
                            //####     NOTE: Since .cnCnSettings also requires .cnCn, it is not included below (as the .cnCnSettings call will pick it up)
                            //####     NOTE: Since .cnYUICalendar also requires .cnYUI, it is not included below (as the .cnYUICalendar call will pick it up)
                            //####     NOTE: SystemName and LanguageCode are passed in via the QueryString because they are used by the server-side script
                            //####     NOTE: WeekOfYearCalculation is passed in via the QueryString because it is defined programaticially by the Developer at runtime
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnCn, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnCnTools, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnYUICalendar, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnYUIEvent, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "Cn/Dates/Tools.js'></script>" + sCRLF +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Inputs/DateTime.js'></script>" + sCRLF +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Inputs/DateTime.js." + g_cServerSideScriptFileExtension +
                                "?LanguageCode=" + sEndUserMessagesLanguageCode +
                            "'></script>" + sCRLF +
                            "<script type='text/javascript' src='" + sPath + "Cn/Dates/Tools.js'></script>" + sCRLF;
                        break;
                    }

                        //#### If this is a .cnCnInputsComboBox eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnInputsComboBox: {
                            //#### Recurse to collect the JavaScript for .cnCn and .cnCnTools (as they are required for this eJavaScriptFile) before appending the script tag
                            //####     NOTE: Since .cnCnSettings also requires .cnCn, it is not included below (as the .cnCnSettings call will pick it up)
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnCn, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnCnTools, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Inputs/ComboBox.js'></script>" + sCRLF;
                        break;
                    }

//! neek
                        //#### If this is a .cnCnInputsHTMLEditor eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnInputsHTMLEditor: {
                            //#### Recurse to collect the JavaScript for .cnCn, .cnCnTools and .cnYUIRichTextEditor (as it is required for this eJavaScriptFile) before appending the script tag
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnCn, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnCnTools, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnYUIRichTextEditor, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Inputs/HTMLEditor.js'></script>" + sCRLF;
                        break;
                    }

//! neek
                        //#### If this is a .cnCnInputsMaxLength eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnInputsMaxLength: {
                            //#### Recurse to collect the JavaScript for .cnCnTools, .cnCnInputs, .cnCnInputsValidation and .cnYUIEvent (as it is required for this eJavaScriptFile) before appending the script tag
                            //####     NOTE: Since .cnCnTools also requires .cnCn, it is not included below (as the .cnCnTools call will pick it up)
                            //####     NOTE: Since .cnYUIEvent also requires .cnYUI, it is not included below (as the .cnYUIEvent call will pick it up)
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnCnTools, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnCnInputs, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnCnInputsValidation, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnYUIEvent, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Inputs/MaxLength.js'></script>" + sCRLF;
                        break;
                    }

                        //#### If this is a .cnCnInputsRadio eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnInputsRadio: {
                            //#### Recurse to collect the JavaScript for .cnCnTools and .cnYUIEvent (as it is required for this eJavaScriptFile) before appending the script tag
                            //####     NOTE: Since .cnCnTools also requires .cnCn, it is not included below (as the .cnCnTools call will pick it up)
                            //####     NOTE: Since .cnYUIEvent also requires .cnYUI, it is not included below (as the .cnYUIEvent call will pick it up)
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnCnTools, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnYUIEvent, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Inputs/Radio.js'></script>" + sCRLF;
                        break;
                    }

					//##########
					//##########

                        //#### If this is a .cnCnRendererForm eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnRendererForm: {
                            //#### Recurse to collect the JavaScript for .cnCn, .cnCnInputs and .cnCnInputsValidation (as they are required for this eJavaScriptFile) before appending the script tag
                            //####     NOTE: Since .cnCnSettings also requires .cnCn, it is not included below (as the .cnCnSettings call will pick it up)
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnCn, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnCnInputs, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnCnInputsValidation, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Renderer/Form.js'></script>" + sCRLF;
                        break;
                    }

                        //#### If this is a .cnCnRendererComplexSorter eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnRendererComplexSorter: {
//!						sReturn += "<script type='text/javascript' src='" + sBaseDirectory + "Cn/Web/Renderer/ComplexSorter.js'></script>" + sCRLF;
                        break;
                    }

                        //#### If this is a .cnCnRendererUserSelectedStack eJavaScriptFile request
                    case enumJavaScriptFiles.cnCnRendererUserSelectedStack: {
                            //#### Recurse to collect the JavaScript for .cnCn (as it is required for this eJavaScriptFile) before appending the script tag
                            //####     NOTE: Since .cnCnSettings also requires .cnCn, it is not included below (as the .cnCnSettings call will pick it up)
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnCn, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "Cn/Web/Renderer/UserSelectedStack.js'></script>"
                        ;
                        break;
                    }

					//##########
					//##########

                        //#### If this is a .cnYUIRichTextEditor eJavaScriptFile request
                    case enumJavaScriptFiles.cnYUIRichTextEditor: {
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnYUIEvent, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            DoGetScriptReferences(enumJavaScriptFiles.cnYUIDOM, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "yui/element.js'></script>" + sCRLF +
                            "<script type='text/javascript' src='" + sPath + "yui/container.js'></script>" + sCRLF +
                            "<script type='text/javascript' src='" + sPath + "yui/simpleeditor.js'></script>" + sCRLF +

                            "<script type='text/javascript' src='" + sPath + "yui/menu.js'></script>" + sCRLF +
							"<script type='text/javascript' src='" + sPath + "yui/button.js'></script>" + sCRLF +
                            "<script type='text/javascript' src='" + sPath + "yui/editor.js'></script>" + sCRLF +
                            "<link rel='stylesheet' type='text/css' href='" + sPath + "yui/editor-core.css' />"
                        ;
                        break;
                    }

                    //##########
                    //##########

                        //#### If this is a .cnYUI eJavaScriptFile request, reset the sReturn value accordingly
                    case enumJavaScriptFiles.cnYUI: {
                        sReturn = "<script type='text/javascript' src='" + sPath + "yui/yahoo.js'></script>" + sCRLF;
                        break;
                    }

                        //#### If this is a .cnYUICalendar eJavaScriptFile request
                    case enumJavaScriptFiles.cnYUICalendar: {
                            //#### Recurse to collect the JavaScript for .cnYUI and .cnYUIEvent (as they are required for this eJavaScriptFile) before appending the script tag
                            //####     NOTE: Since .cnYUIEvent also requires .cnYUI, it is not included below (as the .cnYUIEvent call will pick it up)
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnYUIEvent, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "yui/calendar.js'></script>" + sCRLF +
                            "<link rel='stylesheet' type='text/css' href='" + sPath + "yui/calendar.css' />"
                        ;
                        break;
                    }

                        //#### If this is a .cnYUIEvent eJavaScriptFile request
                    case enumJavaScriptFiles.cnYUIEvent: {
                            //#### Recurse to collect the JavaScript for .cnYUI (as it is required for this eJavaScriptFile) before appending the script tag
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnYUI, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "yui/event.js'></script>" + sCRLF;
                        break;
                    }

                        //#### If this is a .cnYUIDOM eJavaScriptFile request
                    case enumJavaScriptFiles.cnYUIDOM: {
                            //#### Recurse to collect the JavaScript for .cnYUI (as it is required for this eJavaScriptFile) before appending the script tag
                        sReturn = DoGetScriptReferences(enumJavaScriptFiles.cnYUI, ref iRenderedJavaScriptFiles, sEndUserMessagesLanguageCode, sPath, sCRLF, bDebug) +
                            "<script type='text/javascript' src='" + sPath + "yui/dom.js'></script>" + sCRLF;
                        break;
                    }

                    //##########
                    //##########

                        //#### If this is a .cnRenderAllJavaScript eJavaScriptFile request
                    case enumJavaScriptFiles.cnRenderAllJavaScript: {
                            //#### If no other JavaScript has been previously rendered
                        if (iRenderedJavaScriptFiles == 0) {
								//#### Reset the value of the sPath to ensure that we are not looking at the /Debug path
							sPath = Settings.Value(Settings.enumSettingValues.cnUIDirectory) + g_cJavaScriptDirectory;

                                //#### Set the sReturn value to the required all/ .js oFiles
                                //####     NOTE: Since there is no Debug/ version of the all/ .js oFiles, the value of sPath is not utilized below
                                //####     NOTE: SystemName and LanguageCode are passed in via the QueryString because they are used by the server-side script
//! neek - debug hard coded!!
//								"<script type='text/javascript' src='" + sPath + "JS.aspx?Mode=Cn&Debug=" + bDebug + "&LanguageCode=" + sEndUserMessagesLanguageCode + "'></script>" + sCRLF +

                            sReturn = "<script type='text/javascript' src='" + sPath + "JS.aspx?Mode=YUI&Debug=true'></script>" + sCRLF +
								"<script type='text/javascript' src='" + sPath + "JS.aspx?Mode=Cn&Debug=true&LanguageCode=" + sEndUserMessagesLanguageCode + "'></script>" + sCRLF +
								"<script type='text/javascript' src='" + sPath + "JS.aspx?Mode=CSS&Debug=true'></script>"
                            ;

                                //#### Set the .RenderedJavaScriptFiles to .cnDisableJavaScriptRendering (as no further JavaScript is to be rendered and .cnDisableJavaScriptRendering gives us this effect)
                            iRenderedJavaScriptFiles = (int)enumJavaScriptFiles.cnDisableJavaScriptRendering;
                        }
                            //#### Else some JavaScript oFiles have been previously rendered, so raise the error
                        else {
                            Internationalization.RaiseDefaultError(g_cClassName + "JavaScript", Internationalization.enumInternationalizationValues.cnDeveloperMessages_Renderer_RenderedJavaScript, "", "");
                        }
                        break;
                    }

                        //#### If this is a .cnDisableJavaScriptRendering eJavaScriptFile request
                    case enumJavaScriptFiles.cnDisableJavaScriptRendering: {
                        iRenderedJavaScriptFiles = (int)enumJavaScriptFiles.cnDisableJavaScriptRendering;
                        break;
                    }

                        //#### Else the eJavaScriptFile was unreconized, so raise the error
                    default: {
                        Internationalization.RaiseDefaultError(g_cClassName + "JavaScript", Internationalization.enumInternationalizationValues.cnDeveloperMessages_General_UnknownValue, "eJavaScriptFile", Data.Tools.MakeString(eJavaScriptFile, ""));
                        break;
                    }
                }

                    //#### If the .RenderedJavaScriptFiles is not set to .cnDisableJavaScriptRendering
                if (iRenderedJavaScriptFiles != (int)enumJavaScriptFiles.cnDisableJavaScriptRendering) {
                        //#### Flip the eJavaScriptFile bit within the .RenderedJavaScriptFiles
                    iRenderedJavaScriptFiles = (iRenderedJavaScriptFiles | (int)eJavaScriptFile);
                }
            }

                //#### Return the above determined sReturn value to the caller
            return sReturn;
        }

	} //# class JavaScript

} //# namespace Cn.Web
