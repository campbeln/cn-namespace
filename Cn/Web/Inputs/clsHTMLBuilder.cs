/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/


using System;                                       //# Required to access the Date/Decimal/Double/Int32 datatypes
using System.Text;									//# Required to access the StringBuilder class
using System.Collections;					        //# Required to access the Hashtable class
using Cn.Collections;                               //# Required to access the MultiArray class
using Cn.Configuration;								//# Required to access the Internationalization class


namespace Cn.Web.Inputs {

    ///########################################################################################################################
    /// <summary>
	/// Defines the requirements for a Input Builder class.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>November 13, 2009</LastFullCodeReview>
	public interface IInputBuilder {
		//string FormatDateTime(string sDateToFormat, ref string sInputSpecificFormat, Inputs.enumInputTypes eInputType, Settings.Current oSettings);
		//string FormatForForm(string sValue);
		//string EscapeCharacters(string sValue, string sCharacter);

		//string Build(eInputType, )

		string TextBox(string sInputName, string sAttributes, string sInitialValue, int iMaxLength);
		string TextBox(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, Settings.Current oSettings);
		string Password(string sInputName, string sAttributes, string sInitialValue, int iMaxLength);
		string Password(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, Settings.Current oSettings);
		string TextArea(string sInputName, string sAttributes, string sInitialValue, int iMaxLength);
		string TextArea(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, Settings.Current oSettings);
		string HTMLEditor(string sInputName, string sAttributes, string sInitialValue, int iMaxLength);
		string HTMLEditor(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, Settings.Current oSettings);
		string Hidden(string sInputName, string sAttributes, string sInitialValue);
		string Hidden(string sInputName, string sAttributes, string sInitialValue, Settings.Current oSettings);
		string Hidden(string sInputName, string sAttributes, string sInitialValue, string sLabelText, string sLabelAttributes);
		string Hidden(string sInputName, string sAttributes, string sInitialValue, string sLabelText, string sLabelAttributes, Settings.Current oSettings);
		string File(string sInputName, string sAttributes);
		string File(string sInputName, string sAttributes, Settings.Current oSettings);
		string Option(string sInputName, string sAttributes, string sInitialValue, string sContainerAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData);
		string Option(string sInputName, string sAttributes, string sInitialValue, string sContainerAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings);
		string OptionListBox(string sInputName, string sAttributes, string sInitialValue, string sListBoxAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData);
		string OptionListBox(string sInputName, string sAttributes, string sInitialValue, string sListBoxAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings);
		string Select(string sInputName, string sAttributes, string sInitialValue, bool bAddLeadingBlankOption, MultiArray oPicklist);
		string Select(string sInputName, string sAttributes, string sInitialValue, bool bAddLeadingBlankOption, MultiArray oPicklist, Settings.Current oSettings);
		string ComboBox(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, string sSelectAttributes, MultiArray oPicklist);
		string ComboBox(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, string sSelectAttributes, MultiArray oPicklist, Settings.Current oSettings);
		string MultiSelect(string sInputName, string sAttributes, string[] a_sInitialValues, bool bAddLeadingBlankOption, MultiArray oPicklist, Hashtable h_sAdditionalData);
		string MultiSelect(string sInputName, string sAttributes, string[] a_sInitialValues, bool bAddLeadingBlankOption, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings);
		string CheckBoxes(string sInputName, string sAttributes, string[] a_sInitialValues, string sContainerAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData);
		string CheckBoxes(string sInputName, string sAttributes, string[] a_sInitialValues, string sContainerAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings);
		string CheckedListBox(string sInputName, string sAttributes, string[] a_sInitialValues, string sListBoxAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData);
		string CheckedListBox(string sInputName, string sAttributes, string[] a_sInitialValues, string sListBoxAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings);

		string CheckBox(string sInputName, string sAttributes, ref string r_sInitialValue);
		string CheckBox(string sInputName, string sAttributes, ref string r_sInitialValue, Settings.Current oSettings);
		string Date(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData);
		string Date(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData, Settings.Current oSettings);
		string Time(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData);
		string Time(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData, Settings.Current oSettings);
		string DateTime(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData);
		string DateTime(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData, Settings.Current oSettings);
	} //# public interface IInputBuilder


    ///########################################################################################################################
    /// <summary>
	/// Implements the IInputBuilder interface to define the default Renderer HTML input builder functions.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>July 29, 2005</LastFullCodeReview>
	public class HTMLBuilder : IInputBuilder {
			//#### Declare the required private constants
			//####     CSSClasses: Defines the CSS class name for the Special List Box control.
		private const string g_cDefaultCSSClass_SpecialListBox = "cnSpecialListBox";

			//#### Declare the required private constants
		private const string g_cClassName = "Cn.Web.Inputs.HTMLBuilder.";
		private const string g_cREADONLYIDSUFFIX = "_ReadOnly";


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Initializes the class.
		/// </summary>
		///############################################################
		/// <LastUpdated>November 13, 2009</LastUpdated>
	  //public DefaultBuilder() {}


		//##########################################################################################
		//# Input Writing-related Functions
		//##########################################################################################
		#region Input Writing-related Functions
		///############################################################
		/// <summary>
		/// Renders an XHTML-based text form element.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="iMaxLength">Integer representing the maximum allowable character length of the input.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string TextBox(string sInputName, string sAttributes, string sInitialValue, int iMaxLength) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return TextBox(sInputName, sAttributes, sInitialValue, iMaxLength, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based text form element.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="iMaxLength">Integer representing the maximum allowable character length of the input.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string TextBox(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, Settings.Current oSettings) {
			string sReturn;

				//#### If we're in .IsReadOnly mode, just sReturn the passed sInitialValue
			if (oSettings.IsReadOnly) {
				sReturn = sInitialValue + oSettings.CRLF;
			}
				//#### Else we need to sReturn the textbox input
			else {
					//#### If a valid iMaxLength was passed (and a definition is not already present within the passed sAttributes), append the maxlength definition onto the passed sAttributes
				if (iMaxLength > 0 && sAttributes.IndexOf("maxlength=") == -1) {
					sAttributes += " maxlength='" + iMaxLength + "'";
				}

					//#### sReturn the form field (Formating(the sInitialValue)For(the)Form as we go)
				sReturn = "<input type='text' name='" + sInputName + "' id='" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sInitialValue) + "' " + sAttributes + " />" + oSettings.CRLF;
			}

				//#### Return the sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based password form element.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="iMaxLength">Integer representing the maximum allowable character length of the input.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string Password(string sInputName, string sAttributes, string sInitialValue, int iMaxLength) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return Password(sInputName, sAttributes, sInitialValue, iMaxLength, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based password form element.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="iMaxLength">Integer representing the maximum allowable character length of the input.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string Password(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, Settings.Current oSettings) {
			string sReturn;

				//#### If we're in .IsReadOnly mode, just sReturn the passed sInitialValue (which is stupid for a password field, but hey! passwords shouldn't have an sInitialValue)
			if (oSettings.IsReadOnly) {
				sReturn = sInitialValue + oSettings.CRLF;
			}
				//#### Else we need to sReturn the password input
			else {
					//#### If a valid iMaxLength was passed (and a definition is not already present within the passed sAttributes), append the maxlength definition onto the passed sAttributes
				if (iMaxLength > 0 && sAttributes.IndexOf("maxlength=") == -1) {
					sAttributes += " maxlength='" + iMaxLength + "'";
				}

					//#### sReturn the form field (Formating(the sInitialValue)For(the)Form as we go)
				sReturn = "<input type='password' name='" + sInputName + "' id='" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sInitialValue) + "' " + sAttributes + " />" + oSettings.CRLF;
			}

				//#### Return the sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based textarea form element.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="iMaxLength">Integer representing the maximum allowable character length of the input.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string TextArea(string sInputName, string sAttributes, string sInitialValue, int iMaxLength) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return TextArea(sInputName, sAttributes, sInitialValue, iMaxLength, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based textarea form element.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="iMaxLength">Integer representing the maximum allowable character length of the input.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string TextArea(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, Settings.Current oSettings) {
			string sReturn;

				//#### If we're in .IsReadOnly mode, just sReturn the passed sInitialValue (replacing any CrLf's with br's so the text is formatted as it would have been within the textarea)
			if (oSettings.IsReadOnly) {
				sReturn = sInitialValue.Replace("\n", "\n<br/>") + oSettings.CRLF;
			}
				//#### Else we need to sReturn the defined input
			else {
					//#### If a valid iMaxLength was passed (and a definition is not already present within the passed sAttributes), append the maxlength definition onto the passed sAttributes
				if (iMaxLength > 0 && sAttributes.IndexOf("maxlength=") == -1) {
					sAttributes += " maxlength='" + iMaxLength + "'";
				}

					//#### sReturn the form field based on the passed data
				sReturn = "<textarea name='" + sInputName + "' id='" + sInputName + "' " + sAttributes + ">" + sInitialValue + "</textarea>" + oSettings.CRLF;
			}

				//#### Return the sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based WYSIWYG HTML editor.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="iMaxLength">Integer representing the maximum allowable character length of the input.</param>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string HTMLEditor(string sInputName, string sAttributes, string sInitialValue, int iMaxLength) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return HTMLEditor(sInputName, sAttributes, sInitialValue, iMaxLength, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based WYSIWYG HTML editor.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="iMaxLength">Integer representing the maximum allowable character length of the input.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>February 19, 2010</LastUpdated>
		public string HTMLEditor(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, Settings.Current oSettings) {
			StringBuilder oReturn = new StringBuilder();

				//#### If we're in .IsReadOnly mode, just oReturn the passed sInitialValue
			if (oSettings.IsReadOnly) {
				oReturn.Append(sInitialValue + oSettings.CRLF);
			}
				//#### Else we need to output the defined input
			else {
//! neek
					//#### If a valid iMaxLength was passed (and a definition is not already present within the passed sAttributes), append the maxlength definition onto the passed sAttributes
				if (iMaxLength > 0 && sAttributes.IndexOf("maxlength=") == -1) {
					sAttributes += " maxlength='" + iMaxLength + "'";
				}

					//#### oReturn the form field based on the passed data
				oReturn.Append("<textarea name='" + sInputName + "' id='" + sInputName + "' " + sAttributes + ">" + sInitialValue + "</textarea>" + oSettings.CRLF);

					//#### Render the required JavaScript references to properly configure this editor
				oReturn.Append(JavaScript.GenerateFileReferences(JavaScript.enumJavaScriptFiles.cnCnInputsHTMLEditor, oSettings) + oSettings.CRLF +
                    JavaScript.BlockStart + oSettings.CRLF +
                        "Cn._.wih.Define('" + Inputs.Tools.EscapeCharacters(sInputName, "'") + "');" + oSettings.CRLF +
                    JavaScript.BlockEnd + oSettings.CRLF
				);
			}

				//#### Return the oReturn value to the caller
			return oReturn.ToString();
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based hidden form element.
		/// </summary>
		/// <remarks>
		/// NOTE: Nothing is rendered if the view is <c>ReadOnly</c>.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string Hidden(string sInputName, string sAttributes, string sInitialValue) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return Hidden(sInputName, sAttributes, sInitialValue, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based hidden form element.
		/// </summary>
		/// <remarks>
		/// NOTE: Nothing is rendered if the view is <c>ReadOnly</c>.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>November 6, 2009</LastUpdated>
		public string Hidden(string sInputName, string sAttributes, string sInitialValue, Settings.Current oSettings) {
			string sReturn = "";

				//#### If we're not in .IsReadOnly mode
			if (! oSettings.IsReadOnly) {
					//#### Output the hidden form field (Formating(the sInitialValue)For(the)Form as we go)
				sReturn = "<input type='hidden' name='" + sInputName + "' id='" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sInitialValue) + "' " + sAttributes + " />" + oSettings.CRLF;
			}

				//#### Return the sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based hidden form element and label.
		/// </summary>
		/// <remarks>
		/// Utilizing the text <paramref>sLabel</paramref> allows you to output a "read only input", or a hidden input with a visual marker.
		/// NOTE: The <paramref>sLabelAttributes</paramref> is applied to the implicit DIV that holds the <paramref>sLabel</paramref>.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="sLabelText">String representing the text to display within the label.</param>
		/// <param name="sLabelAttributes">String representing the additional HTML attributes to apply to the label.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string Hidden(string sInputName, string sAttributes, string sInitialValue, string sLabelText, string sLabelAttributes) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return Hidden(sInputName, sAttributes, sInitialValue, sLabelText, sLabelAttributes, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based hidden form element and label.
		/// </summary>
		/// <remarks>
		/// Utilizing the text <paramref>sLabel</paramref> allows you to output a "read only input", or a hidden input with a visual marker.
		/// NOTE: The <paramref>sLabelAttributes</paramref> is applied to the implicit DIV that holds the <paramref>sLabel</paramref>.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="sLabelText">String representing the text to display within the label.</param>
		/// <param name="sLabelAttributes">String representing the additional HTML attributes to apply to the label.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>December 7, 2009</LastUpdated>
		public string Hidden(string sInputName, string sAttributes, string sInitialValue, string sLabelText, string sLabelAttributes, Settings.Current oSettings) {
			StringBuilder oReturn = new StringBuilder();

				//#### oReturn the psuedo-label DIV
				//####     NOTE: We could/should return a "<label for='sInputName' ..." here, but since the input is hidden, this could cause some problems so we output a paeudo-label DIV instead. Also, we don't output any other <label...> tags, so outputting one here would be inconsistent.
			oReturn.Append("<div id='" + Settings.Value(Settings.enumSettingValues.cnDOMElementPrefix) + sInputName + g_cREADONLYIDSUFFIX + "' " + sLabelAttributes + ">" + sLabelText + "</div>" + oSettings.CRLF);

				//#### If we're not in .IsReadOnly mode, also output the hidden input
			if (! oSettings.IsReadOnly) {
					//#### oReturn the hidden form field (Formating(the sInitialValue)For(the)Form as we go)
				oReturn.Append("<input type='hidden' name='" + sInputName + "' id='" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sInitialValue) + "' " + sAttributes + " />" + oSettings.CRLF);
			}

				//#### Return the oReturn value to the caller
			return oReturn.ToString();
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based file form element.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string File(string sInputName, string sAttributes) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return File(sInputName, sAttributes, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based file form element.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string File(string sInputName, string sAttributes, Settings.Current oSettings) {
			string sReturn;

				//#### If we're in .IsReadOnly mode
			if (oSettings.IsReadOnly) {
					//#### sReturn the disabled form field based on the passed data
				sReturn = "<input type='file' name='" + sInputName + "' id='" + sInputName + "' disabled='disabled' " + sAttributes + " />" + oSettings.CRLF;
			}
				//#### Else we need to sReturn the file input
			else {
					//#### sReturn the form field based on the passed data
				sReturn = "<input type='file' name='" + sInputName + "' id='" + sInputName + "' " + sAttributes + " />" + oSettings.CRLF;
			}

				//#### Return the sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based option form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="sContainerAttributes">String representing the additional HTML attributes to apply to the containing DIV.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string Option(string sInputName, string sAttributes, string sInitialValue, string sContainerAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData) {
				//#### Pass the call off to DoCheckboxesInput, signaling that this is a not a option list box
			return DoOption("Option", sInputName, sAttributes, sInitialValue, sContainerAttributes, oPicklist, h_sAdditionalData, new Settings.Current(), false);
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based option form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="sContainerAttributes">String representing the additional HTML attributes to apply to the containing DIV.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string Option(string sInputName, string sAttributes, string sInitialValue, string sContainerAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings) {
				//#### Pass the call off to DoCheckboxesInput, signaling that this is a not a option list box
			return DoOption("Option", sInputName, sAttributes, sInitialValue, sContainerAttributes, oPicklist, h_sAdditionalData, oSettings, false);
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based option list box form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="sListBoxAttributes">String representing the additional HTML attributes to apply to the containing DIV.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string OptionListBox(string sInputName, string sAttributes, string sInitialValue, string sListBoxAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData) {
				//#### Pass the call off to DoCheckboxesInput, signaling that this is a a option list box
			return DoOption("OptionListBox", sInputName, sAttributes, sInitialValue, sListBoxAttributes, oPicklist, h_sAdditionalData, new Settings.Current(), true);
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based option list box form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="sListBoxAttributes">String representing the additional HTML attributes to apply to the containing DIV.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string OptionListBox(string sInputName, string sAttributes, string sInitialValue, string sListBoxAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings) {
				//#### Pass the call off to DoCheckboxesInput, signaling that this is a a option list box
			return DoOption("OptionListBox", sInputName, sAttributes, sInitialValue, sListBoxAttributes, oPicklist, h_sAdditionalData, oSettings, true);
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based select form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="bAddLeadingBlankOption">Boolean value indicating if a leading blank option should be added to the input.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string Select(string sInputName, string sAttributes, string sInitialValue, bool bAddLeadingBlankOption, MultiArray oPicklist) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return Select(sInputName, sAttributes, sInitialValue, bAddLeadingBlankOption, oPicklist, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based select form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="bAddLeadingBlankOption">Boolean value indicating if a leading blank option should be added to the input.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string Select(string sInputName, string sAttributes, string sInitialValue, bool bAddLeadingBlankOption, MultiArray oPicklist, Settings.Current oSettings) {
			StringBuilder oReturn = new StringBuilder();

				//#### If the passed oPicklist has .Rows (and is not null)
			if (oPicklist != null && oPicklist.RowCount > 0) {
					//#### If we're in .IsReadOnly mode
				if (oSettings.IsReadOnly) {
						//#### .Decode the passed sInitialValue from the passed oPicklist
					oReturn.Append(Cn.Data.Picklists.Decoder(oPicklist, sInitialValue, oSettings.StrictPicklistDecodes) + oSettings.CRLF);
				}
					//#### Else we need to output the select input
				else {
					string sCurrentData;
					int iRowCount;
					int iLen;
					int i;

						//#### Determine the iRowCount of the oPicklist and determine sInitialValue's .Length in prep of the loop below
					iRowCount = oPicklist.RowCount;
					iLen = Cn.Data.Tools.MakeString(sInitialValue, "").Length;

						//#### .Append the top of the select
					oReturn.Append("<select name='" + sInputName + "' id='" + sInputName + "' " + sAttributes + ">" + oSettings.CRLF);

						//#### If we are supposed to bAdd(in a)LeadingBlankOption, output the requested blank element and inc the iIDIndex
					if (bAddLeadingBlankOption) {
						oReturn.Append("<option value=''></option>" + oSettings.CRLF);
					}

						//#### Traverse the passed oPicklist
					for (i = 0; i < iRowCount; i++) {
							//#### Reset the sCurrentData for this loop
						sCurrentData = oPicklist.Value(i, "Data");

							//#### If the sCurrentData is equal to the passed sInitialValue (checking their .Lengths first as that is a far faster comparison)
							//####     NOTE: We do not look at IsActive here so that we allow records with non-IsActive entries to be edited without loosing those values
						if (iLen == sCurrentData.Length && sInitialValue == sCurrentData) {
								//#### .Append the SELECTED option, flip bFoundComboValue to true and exit the loop (Formating(the sInitialValue)For(the)Form as we go)
							oReturn.Append("<option value='" + Inputs.Tools.FormatForForm(sCurrentData) + "' SELECTED='True'>" + oPicklist.Value(i, "Description") + "</option>" + oSettings.CRLF);
							break;
						}
							//#### Else the sCurrentData is not equal to sInitialValue, so print out the option accordingly (Formating(the sInitialValue)For(the)Form as we go)
						else {
								//#### If the current .Value IsActive
							if (Data.Tools.MakeBoolean(oPicklist.Value(i, "IsActive"), true)) {
								oReturn.Append("<option value='" + Inputs.Tools.FormatForForm(sCurrentData) + "'>" + oPicklist.Value(i, "Description") + "</option>" + oSettings.CRLF);
							}
						}
					}

						//#### Traverse the remainder of oPicklist's .Rows, .Append'ing each remaining option as we go (Formating(the sInitialValue)For(the)Form as we go)
					for (i = (i + 1); i < iRowCount; i++) {
							//#### If the current .Value IsActive
						if (Data.Tools.MakeBoolean(oPicklist.Value(i, "IsActive"), true)) {
							oReturn.Append("<option value='" + Inputs.Tools.FormatForForm(oPicklist.Value(i, "Data")) + "'>" + oPicklist.Value(i, "Description") + "</option>" + oSettings.CRLF);
						}
					}

						//#### .Append the bottom of the select box
					oReturn.Append("</select> " + oSettings.CRLF);
				}
			}
				//#### Else the passed oPicklist is null, so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + "RenderSelectInput", Configuration.Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_PicklistIsEmpty, sInputName, "");
			}

				//#### Return the oReturn value to the caller
			return oReturn.ToString();
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based combobox form element based on the provided picklist.
		/// </summary>
		/// <remarks>
		/// NOTE: <c>WriteComboBoxInput</c> will fail the basic datatype validation done by <see cref='Cn.Web.Renderer.FormRenderer'>FormRenderer</see> if the user does not enter a value from the list. It is up to the developer to clear this error within their own <c>ValidateRecord</c> function.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the text input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="iMaxLength">Integer representing the maximum allowable character length of the input.<para/>A positive value indicates that a maxlength arrtibute is to be set. A non-positive value indicates that no maxlength is to be set.</param>
		/// <param name="sSelectAttributes">String representing the additional HTML attributes to apply to the select input.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string ComboBox(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, string sSelectAttributes, MultiArray oPicklist) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return ComboBox(sInputName, sAttributes, sInitialValue, iMaxLength, sSelectAttributes, oPicklist, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based combobox form element based on the provided picklist.
		/// </summary>
		/// <remarks>
		/// NOTE: <c>WriteComboBoxInput</c> will fail the basic datatype validation done by <see cref='Cn.Web.Renderer.FormRenderer'>FormRenderer</see> if the user does not enter a value from the list. It is up to the developer to clear this error within their own <c>ValidateRecord</c> function.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the text input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="iMaxLength">Integer representing the maximum allowable character length of the input.<para/>A positive value indicates that a maxlength arrtibute is to be set. A non-positive value indicates that no maxlength is to be set.</param>
		/// <param name="sSelectAttributes">String representing the additional HTML attributes to apply to the select input.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 3, 2007</LastUpdated>
		public string ComboBox(string sInputName, string sAttributes, string sInitialValue, int iMaxLength, string sSelectAttributes, MultiArray oPicklist, Settings.Current oSettings) {
			StringBuilder oReturn = new StringBuilder();

				//#### If the passed oPicklist has .Rows (and is not null)
			if (oPicklist != null && oPicklist.RowCount > 0) {
					//#### If we're in .IsReadOnly mode
				if (oSettings.IsReadOnly) {
						//#### .Decode the passed sInitialValue from the passed oPicklist (passing in false for the .StrictPicklistDecodes argument, as this is a combobox)
					oReturn.Append(Cn.Data.Picklists.Decoder(oPicklist, sInitialValue, false) + oSettings.CRLF);
				}
					//#### Else we need to output the select input
				else {
					string sDOMElementPrefix = Web.Settings.Value(Settings.enumSettingValues.cnDOMElementPrefix);
string sOptionAttributes = "";
					int i;
bool bOptionAttributesExist = oPicklist.Exists("OptionAttributes");

						//#### If a valid iMaxLength was passed (and a definition is not already present within the passed sAttributes), append the maxlength definition onto the passed sAttributes
					if (iMaxLength > 0 && sAttributes.IndexOf("maxlength=") == -1) {
						sAttributes += " maxlength='" + iMaxLength + "'";
					}

						//#### Write out the main text input of the combobox, then the required DIV and select
					oReturn.Append("<input type='text' name='" + sInputName + "' id='" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sInitialValue) + "' " + sAttributes + " />" + oSettings.CRLF);
					oReturn.Append("<div id='" + sDOMElementPrefix + sInputName + "_DIV' style='visibility: hidden; position: absolute; white-space: nowrap; display: inline;'>" + oSettings.CRLF);
					oReturn.Append(Web.Settings.Internationalization.Value(Internationalization.enumInternationalizationValues.cnEndUserMessages_ComboBox_OrSelect, oSettings.EndUserMessagesLanguageCode));
					oReturn.Append("<select name='" + sDOMElementPrefix + sInputName + "_Select' id='" + sDOMElementPrefix + sInputName + "_Select' " + sSelectAttributes + " >" + oSettings.CRLF);

						//#### Output the required leading 0th blank element
					oReturn.Append("<option value=''></option>" + oSettings.CRLF);

						//#### Traverse the passed oPicklist, outputting each option as we go
					for (i = 0; i < oPicklist.RowCount; i++) {
							//#### If the current .Value IsActive
						if (Data.Tools.MakeBoolean(oPicklist.Value(i, "IsActive"), true)) {
if (bOptionAttributesExist) {
	sOptionAttributes = " " + oPicklist.Value(i, "OptionAttributes");
}
//!							oReturn.Append("<option value='" + Inputs.Tools.FormatForForm(oPicklist.Value(i, "Data")) + "'>" + oPicklist.Value(i, "Description") + "</option>" + oSettings.CRLF);
							oReturn.Append("<option value='" + Inputs.Tools.FormatForForm(oPicklist.Value(i, "Data")) + sOptionAttributes + "'>" + oPicklist.Value(i, "Description") + "</option>" + oSettings.CRLF);
						}
					}

						//#### Output the bottom of the select and DIV along with the required JavaScript
					oReturn.Append("</select>" + oSettings.CRLF + "</div>" + oSettings.CRLF);
					oReturn.Append(JavaScript.GenerateFileReferences(JavaScript.enumJavaScriptFiles.cnCnInputsComboBox, oSettings) + oSettings.CRLF +
                        Web.JavaScript.BlockStart + oSettings.CRLF +
                            "Cn._.wic.Add('" + Inputs.Tools.EscapeCharacters(sInputName, "'") + "');" + oSettings.CRLF +
                        Web.JavaScript.BlockEnd + oSettings.CRLF
					);
				}
			}
				//#### Else the passed oPicklist is null, so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + "RenderComboBoxInput", Configuration.Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_PicklistIsEmpty, "", "");
			}

				//#### Return the oReturn value to the caller
			return oReturn.ToString();
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based multi-select form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="a_sInitialValues">String array where each element represents an initial value of the input.</param>
		/// <param name="bAddLeadingBlankOption">Boolean value indicating if a leading blank option should be added to the input.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 3, 2007</LastUpdated>
		public string MultiSelect(string sInputName, string sAttributes, string[] a_sInitialValues, bool bAddLeadingBlankOption, MultiArray oPicklist, Hashtable h_sAdditionalData) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return MultiSelect(sInputName, sAttributes, a_sInitialValues, bAddLeadingBlankOption, oPicklist, h_sAdditionalData, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based multi-select form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="a_sInitialValues">String array where each element represents an initial value of the input.</param>
		/// <param name="bAddLeadingBlankOption">Boolean value indicating if a leading blank option should be added to the input.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 3, 2007</LastUpdated>
		public string MultiSelect(string sInputName, string sAttributes, string[] a_sInitialValues, bool bAddLeadingBlankOption, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings) {
			StringBuilder oReturn = new StringBuilder();

				//#### If the passed oPicklist has .Rows (and is not null)
			if (oPicklist != null && oPicklist.RowCount > 0) {
					//#### If we're in .IsReadOnly mode
				if (oSettings.IsReadOnly) {
						//#### If the passed a_sInitialValues is not null
					if (a_sInitialValues != null) {
							//#### .Decode the passed a_sInitialValues from the passed oPicklist
						a_sInitialValues = Cn.Data.Picklists.Decoder(oPicklist, a_sInitialValues, oSettings.StrictPicklistDecodes);

							//#### If the passed h_sAdditionalData is Nothing, init it to a new Hashtable (so we don't raise any errors below)
						if (h_sAdditionalData == null) {
							h_sAdditionalData = new Hashtable();
						}

							//#### If we are supposed to Print(the)LabelFirst
						if (Cn.Data.Tools.MakeBoolean(h_sAdditionalData["MultiValue_PrintLabelFirst"], false)) {
								//#### Write out the .Joined a_sInitialValues with the defined MultiValue_LabelSpacer and MultiValue_Delimiter
							oReturn.Append(string.Join(Cn.Data.Tools.MakeString(h_sAdditionalData["MultiValue_LabelSpacer"], "") + Cn.Data.Tools.MakeString(h_sAdditionalData["MultiValue_Delimiter"], ""), a_sInitialValues) + oSettings.CRLF);
						}
							//#### Else we aren't supposed to Print(the)LabelFirst
						else {
								//#### Write out the .Joined a_sInitialValues with the defined MultiValue_Delimiter and MultiValue_LabelSpacer
							oReturn.Append(string.Join(Cn.Data.Tools.MakeString(h_sAdditionalData["MultiValue_Delimiter"], "") + Cn.Data.Tools.MakeString(h_sAdditionalData["MultiValue_LabelSpacer"], ""), a_sInitialValues) + oSettings.CRLF);
						}
					}
				}
					//#### Else we need to output the multi-select input
				else {
					string sCurrentData;
					int iInitialValuesLen = -1;
					int iLen;
					int i;
					int j;
					bool bFound;

						//#### Write out the top of the multi-select 
					oReturn.Append("<select name='" + sInputName + "' id='" + sInputName + "' MULTIPLE='True' " + sAttributes + "> " + oSettings.CRLF);

						//#### If we are supposed to bAdd(in a)LeadingBlankOption, output the requested blank element
					if (bAddLeadingBlankOption) {
						oReturn.Append("<option value=''></option>" + oSettings.CRLF);
					}

						//#### If the passed a_sInitialValues is not null, determine its .Length
					if (a_sInitialValues != null) {
						iInitialValuesLen = a_sInitialValues.Length;
					}

						//#### Traverse the passed oPicklist
					for (i = 0; i < oPicklist.RowCount; i++) {
							//#### Reset the sCurrentData, its iLen and bFound for this loop
						sCurrentData = oPicklist.Value(i, "Data");
						iLen = sCurrentData.Length;
						bFound = false;

							//#### Traverse the passed a_sInitialValues
						for (j = 0; j < iInitialValuesLen; j++) {
								//#### If the sCurrentData is equal to the current a_sDefaultValue (checking their .Lengths first as that is a far faster comparison)
								//####     NOTE: We do not look at IsActive here so that we allow records with non-IsActive entries to be edited without loosing those values
							if (a_sInitialValues[j].Length == iLen && a_sInitialValues[j] == sCurrentData) {
									//#### Write out the SELECTED option, flip bFound and exit the inner for loop (Formating(the sDefaultValue)For(the)Form as we go)
								oReturn.Append("<option value='" + Inputs.Tools.FormatForForm(sCurrentData) + "' SELECTED='True'>" + oPicklist.Value(i, "Description") + "</option>" + oSettings.CRLF);
								bFound = true;
								break;
							}
						}

							//#### If the sCurrentData was not bFound above, print out the non-SELECTED option (Formating(the sDefaultValue)For(the)Form as we go)
						if (! bFound) {
								//#### If the current .Value IsActive
							if (Data.Tools.MakeBoolean(oPicklist.Value(i, "IsActive"), true)) {
								oReturn.Append("<option value='" + Inputs.Tools.FormatForForm(sCurrentData) + "'>" + oPicklist.Value(i, "Description") + "</option>" + oSettings.CRLF);
							}
						}
					}

						//#### Output the bottom of the select box
					oReturn.Append("</select> " + oSettings.CRLF);
				}
			}
				//#### Else the passed oPicklist is null, so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + "RenderMultiSelectInput", Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_PicklistIsEmpty, "", "");
			}

				//#### Return the oReturn value to the caller
			return oReturn.ToString();
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based checkbox group form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="a_sInitialValues">String array where each element represents an initial value of the input.</param>
		/// <param name="sContainerAttributes">String representing the additional HTML attributes to apply to the containing DIV.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string CheckBoxes(string sInputName, string sAttributes, string[] a_sInitialValues, string sContainerAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData) {
				//#### Pass the call off to DoCheckboxesInput, signaling that this is not a checked list box
			return DoCheckBoxes("Checkboxes", sInputName, sAttributes, a_sInitialValues, sContainerAttributes, oPicklist, h_sAdditionalData, new Settings.Current(), false);
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based checkbox group form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="a_sInitialValues">String array where each element represents an initial value of the input.</param>
		/// <param name="sContainerAttributes">String representing the additional HTML attributes to apply to the containing DIV.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string CheckBoxes(string sInputName, string sAttributes, string[] a_sInitialValues, string sContainerAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings) {
				//#### Pass the call off to DoCheckboxesInput, signaling that this is not a checked list box
			return DoCheckBoxes("Checkboxes", sInputName, sAttributes, a_sInitialValues, sContainerAttributes, oPicklist, h_sAdditionalData, oSettings, false);
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based checked list box form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="a_sInitialValues">String array where each element represents an initial value of the input.</param>
		/// <param name="sListBoxAttributes">String representing the additional HTML attributes to apply to the containing DIV.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string CheckedListBox(string sInputName, string sAttributes, string[] a_sInitialValues, string sListBoxAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData) {
				//#### Pass the call off to DoCheckboxesInput, signaling that this is a checked list box
			return DoCheckBoxes("CheckedListBox", sInputName, sAttributes, a_sInitialValues, sListBoxAttributes, oPicklist, h_sAdditionalData, new Settings.Current(), true);
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based checked list box form element based on the provided picklist.
		/// </summary>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="a_sInitialValues">String array where each element represents an initial value of the input.</param>
		/// <param name="sListBoxAttributes">String representing the additional HTML attributes to apply to the containing DIV.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 2, 2007</LastUpdated>
		public string CheckedListBox(string sInputName, string sAttributes, string[] a_sInitialValues, string sListBoxAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings) {
				//#### Pass the call off to DoCheckboxesInput, signaling that this is a checked list box
			return DoCheckBoxes("CheckedListBox", sInputName, sAttributes, a_sInitialValues, sListBoxAttributes, oPicklist, h_sAdditionalData, oSettings, true);
		}


		//##########################################################################################
		//# Public Input Writing-Related Functions (with coerced sInitialValue's)
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Renders an XHTML-based checkbox form element based on the provided picklist.
		/// </summary>
		/// <remarks>
		/// The provided <paramref>sInitialValue</paramref> is coerced into a boolean value (as a checkbox is boolean by nature).  Because of this, the effective initial value of the input is returned so that end user data changes can be reconized despite this data transormation.
		/// <para/>NOTE: The returned coerced <paramref>sInitialValue</paramref> is "true" if the value was determined to be true, or a null-string if it was determined to be false. A null-string is returned here in place of "false" because a non-checked checkbox submits as a null-string.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="r_sInitialValue">By reference String representing the initial value of the input. This value is modified during processing to represent the coerced value, which in turn represents the effective initial value of the input.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 3, 2007</LastUpdated>
		public string CheckBox(string sInputName, string sAttributes, ref string r_sInitialValue) {
				//#### Pass the call off to our sibling implementation with the default value for the oSettings
			return CheckBox(sInputName, sAttributes, ref r_sInitialValue, new Settings.Current());
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML-based checkbox form element based on the provided picklist.
		/// </summary>
		/// <remarks>
		/// The provided <paramref>r_sInitialValue</paramref> is coerced into a boolean value (as a checkbox is boolean by nature).  Because of this, the effective initial value of the input is returned so that end user data changes can be reconized despite this data transormation.
		/// <para/>NOTE: The returned coerced <paramref>r_sInitialValue</paramref> is "true" if the value was determined to be true, or a null-string if it was determined to be false. A null-string is returned here in place of "false" because a non-checked checkbox submits as a null-string.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="r_sInitialValue">By reference String representing the initial value of the input. This value is modified during processing to represent the coerced value, which in turn represents the effective initial value of the input.</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>April 14, 2010</LastUpdated>
		public string CheckBox(string sInputName, string sAttributes, ref string r_sInitialValue, Settings.Current oSettings) {
			string sReturn;
			int iInitialValue = Cn.Data.Tools.MakeBooleanInteger(r_sInitialValue, false);

				//#### If the above coerced r_sInitialValue was true, reset the ByRef r_sInitialValue value accordingly
			if (iInitialValue == 1) {
				r_sInitialValue = "1";
			}
//!
				//#### Else the passed r_sInitialValue is logicially false, so reset it to a null string
				//####     NOTE: This is done because CheckBoxes are only submitted if they are checked. Otherwise they are not submitted, which is interpretered as a null-string during .IsPostBack so we we leave the r_sInitialValue as something other then a null-string then the .CalculateCurrentMD5* logic is not processed correctly, hence the need to null-string it out below
			else {
				r_sInitialValue = "";
			}

				//#### If we're in .IsReadOnly mode
			if (oSettings.IsReadOnly) {
				MultiArray oPicklist = Web.Settings.Internationalization.Values(Configuration.Internationalization.enumInternationalizationPicklists.cnBoolean, oSettings.EndUserMessagesLanguageCode);

					//#### Print out the iInitialValue (using .Internationalization's .cnBoolean picklist)
					//####     NOTE: The value of .StrictPicklistDecodes argument is not important as we utilize .MakeBooleanInteger to force the r_sInitialValue into a reconized state
				sReturn = Cn.Data.Picklists.Decoder(oPicklist, iInitialValue.ToString(), false) + oSettings.CRLF;
			}
				//#### Else we need to output the defined input
			else {
					//#### If the iInitialValue is true, write out a CHECKED checkbox
				if (iInitialValue == 1) {
					sReturn = "<input type='checkbox' name='" + sInputName + "' id='" + sInputName + "' value='1' checked='checked' " + sAttributes + " />" + oSettings.CRLF;
				}
					//#### Else the iInitialValue is false, so write out a non-CHECKED checkbox
				else {
					sReturn = "<input type='checkbox' name='" + sInputName + "' id='" + sInputName + "' value='1' " + sAttributes + " />" + oSettings.CRLF;
				}
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based date picker form element.
		/// </summary>
		/// <remarks>
		/// Since a date format can be defined within the <paramref>h_sAdditionalData</paramref>, the <paramref>r_sInitialValue</paramref> may be reformatted into a string representation of a date that differs from the provided value (though is logicially equivalent). Because of this, the effective initial value of the input is returned so that end user data changes can be reconized despite this data transormation.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="r_sInitialValue">By reference String representing the initial value of the input. This value is modified during processing to represent the coerced value, which in turn represents the effective initial value of the input.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 3, 2007</LastUpdated>
		public string Date(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData) {
				//#### Pass the call off to .DoDateTime with the default value for the oSettings, signaling that this is a .cnDate
			return DoDateTime(sInputName, sAttributes, ref r_sInitialValue, h_sAdditionalData, new Settings.Current(), enumInputTypes.cnDate);
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based date picker form element.
		/// </summary>
		/// <remarks>
		/// Since a date format can be defined within the <paramref>h_sAdditionalData</paramref>, the <paramref>r_sInitialValue</paramref> may be reformatted into a string representation of a date that differs from the provided value (though is logicially equivalent). Because of this, the effective initial value of the input is returned so that end user data changes can be reconized despite this data transormation.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="r_sInitialValue">By reference String representing the initial value of the input. This value is modified during processing to represent the coerced value, which in turn represents the effective initial value of the input.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 3, 2007</LastUpdated>
		public string Date(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData, Settings.Current oSettings) {
				//#### Pass the call off to .DoDateTime, signaling that this is a .cnDate
			return DoDateTime(sInputName, sAttributes, ref r_sInitialValue, h_sAdditionalData, oSettings, enumInputTypes.cnDate);
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based time picker form element.
		/// </summary>
		/// <remarks>
		/// Since a time format can be defined within the <paramref>h_sAdditionalData</paramref>, the <paramref>r_sInitialValue</paramref> may be reformatted into a string representation of a date that differs from the provided value (though is logicially equivalent). Because of this, the effective initial value of the input is returned so that end user data changes can be reconized despite this data transormation.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="r_sInitialValue">By reference String representing the initial value of the input. This value is modified during processing to represent the coerced value, which in turn represents the effective initial value of the input.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 3, 2007</LastUpdated>
		public string Time(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData) {
				//#### Pass the call off to .DoDateTime with the default value for the oSettings, signaling that this is a .cnTime
			return DoDateTime(sInputName, sAttributes, ref r_sInitialValue, h_sAdditionalData, new Settings.Current(), enumInputTypes.cnTime);
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based time picker form element.
		/// </summary>
		/// <remarks>
		/// Since a time format can be defined within the <paramref>h_sAdditionalData</paramref>, the <paramref>r_sInitialValue</paramref> may be reformatted into a string representation of a date that differs from the provided value (though is logicially equivalent). Because of this, the effective initial value of the input is returned so that end user data changes can be reconized despite this data transormation.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="r_sInitialValue">By reference String representing the initial value of the input. This value is modified during processing to represent the coerced value, which in turn represents the effective initial value of the input.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 3, 2007</LastUpdated>
		public string Time(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData, Settings.Current oSettings) {
				//#### Pass the call off to .DoDateTime, signaling that this is a .cnTime
			return DoDateTime(sInputName, sAttributes, ref r_sInitialValue, h_sAdditionalData, oSettings, enumInputTypes.cnTime);
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based date/time picker form element.
		/// </summary>
		/// <remarks>
		/// Since a date/time format can be defined within the <paramref>h_sAdditionalData</paramref>, the <paramref>r_sInitialValue</paramref> may be reformatted into a string representation of a date that differs from the provided value (though is logicially equivalent). Because of this, the effective initial value of the input is returned so that end user data changes can be reconized despite this data transormation.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="r_sInitialValue">By reference String representing the initial value of the input. This value is modified during processing to represent the coerced value, which in turn represents the effective initial value of the input.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 3, 2007</LastUpdated>
		public string DateTime(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData) {
				//#### Pass the call off to .DoDateTime with the default value for the oSettings, signaling that this is a .cnDateTime
			return DoDateTime(sInputName, sAttributes, ref r_sInitialValue, h_sAdditionalData, new Settings.Current(), enumInputTypes.cnDateTime);
		}

		///############################################################
		/// <summary>
		/// Renders a DHTML-based date/time picker form element.
		/// </summary>
		/// <remarks>
		/// Since a date/time format can be defined within the <paramref>h_sAdditionalData</paramref>, the <paramref>r_sInitialValue</paramref> may be reformatted into a string representation of a date that differs from the provided value (though is logicially equivalent). Because of this, the effective initial value of the input is returned so that end user data changes can be reconized despite this data transormation.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="r_sInitialValue">By reference String representing the initial value of the input. This value is modified during processing to represent the coerced value, which in turn represents the effective initial value of the input.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>August 3, 2007</LastUpdated>
		public string DateTime(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData, Settings.Current oSettings) {
				//#### Pass the call off to .DoDateTime, signaling that this is a .cnDateTime
			return DoDateTime(sInputName, sAttributes, ref r_sInitialValue, h_sAdditionalData, oSettings, enumInputTypes.cnDateTime);
		}


		//##########################################################################################
		//# Private Functions
		//##########################################################################################
		///############################################################
		/// <summary>
		/// Renders a DHTML-based date/time picker form element.
		/// </summary>
		/// <remarks>
		/// Since a date/time format can be defined within the <paramref>h_sAdditionalData</paramref>, the <paramref>r_sInitialValue</paramref> may be reformatted into a string representation of a date that differs from the provided value (though is logicially equivalent). Because of this, the effective initial value of the input is returned so that end user data changes can be reconized despite this data transormation.
		/// </remarks>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="r_sInitialValue">By reference String representing the initial value of the input. This value is modified during processing to represent the coerced value, which in turn represents the effective initial value of the input.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="oSettings">Cn.Web.Settings.Current instance representing the current enviroment.</param>
		/// <param name="eInputType">Enumeration representing the type of input to render.</param>
		/// <returns>String representing the DHTML control.</returns>
		///############################################################
		/// <LastUpdated>April 16, 2010</LastUpdated>
		private string DoDateTime(string sInputName, string sAttributes, ref string r_sInitialValue, Hashtable h_sAdditionalData, Settings.Current oSettings, enumInputTypes eInputType) {
			DateTime dInitialDate;
			string sDateTimeFormat;
//!			string sSerialDate;
			string sReturn = "";

				//#### If the passed h_sAdditionalData is null, init it to a new Hashtable (so we don't raise any errors below)
			if (h_sAdditionalData == null) {
				h_sAdditionalData = new Hashtable();
			}

				//#### Determine the dInitialDate and the sSerialDate of the passed r_sInitialValue
			dInitialDate = Cn.Data.Tools.MakeDate(r_sInitialValue, System.DateTime.Now);
//!			sSerialDate = dInitialDate.Year + " " + dInitialDate.Month + " " + dInitialDate.Day + " " + dInitialDate.Hour + " " + dInitialDate.Minute + " " + dInitialDate.Second;

				//#### Determine the sDateTimeFormat from h_sAdditionalData's DateTime_Format key, then reset the ByRef r_sInitialValue value to the appropriately Format(ed)DateTimeForInput from the passed r_sInitialValue (which updates sDateTimeFormat ByRef as necessary)
			sDateTimeFormat = Cn.Data.Tools.MakeString(h_sAdditionalData["DateTime_Format"], "");
			r_sInitialValue = Inputs.Tools.FormatDateTime(r_sInitialValue, ref sDateTimeFormat, eInputType, oSettings);

				//#### If we're in .IsReadOnly mode, print out our above formatted r_sInitialValue value
			if (oSettings.IsReadOnly) {
				sReturn = r_sInitialValue + oSettings.CRLF;
			}
				//#### Else if this is a .cnText request, pass the call off into .TextboxInput (with a iMaxLength of 0 so none is set)
			else if (eInputType == enumInputTypes.cnText) {
				TextBox(sInputName, sAttributes, r_sInitialValue, 0, oSettings);
			}
				//#### Else we need to output the defined date/time input
			else {
				string sPopUpButtonCode = "";

					//#### If we are supposed to _Show(the)PopUpButton, reset sPopUpButtonCode accordingly
				if (Cn.Data.Tools.MakeBoolean(h_sAdditionalData["DateTime_ShowPopUpButton"], true)) {
					sPopUpButtonCode = Cn.Data.Tools.MakeString(h_sAdditionalData["DateTime_PopupButtonCode"],
						"<img src='" + Web.Settings.Value(Settings.enumSettingValues.cnUIDirectory) + "img/show.gif' onClick=\"Cn._.wid.Show('" + sInputName + "', '" + sDateTimeFormat + "', " + (int)eInputType + ", " + Cn.Data.Tools.MakeInteger(h_sAdditionalData["DateTime_HourIncrement"], 1) + ", " + Cn.Data.Tools.MakeInteger(h_sAdditionalData["DateTime_MinuteIncrement"], 1) + ", " + Cn.Data.Tools.MakeInteger(h_sAdditionalData["DateTime_SecondIncrement"], 1) + ");\" />"
					);
				}

					//#### If we are supposed to _ShowOnClick, append the onClick code into the sAttributes
				if (Cn.Data.Tools.MakeBoolean(h_sAdditionalData["DateTime_ShowOnClick"], true)) {
//! make sure this.blur() is correct syntax
					sAttributes += " onClick=\"Cn._.wid.Show('" + sInputName + "', '" + sDateTimeFormat + "', " + (int)eInputType + ", " + Cn.Data.Tools.MakeInteger(h_sAdditionalData["DateTime_HourIncrement"], 1) + ", " + Cn.Data.Tools.MakeInteger(h_sAdditionalData["DateTime_MinuteIncrement"], 1) + ", " + Cn.Data.Tools.MakeInteger(h_sAdditionalData["DateTime_SecondIncrement"], 1) + "); this.blur();\"";
				}

					//#### If the developer has decided that the _UserMustUse(the)Control, append the related code the passed sAttributes
				if (Cn.Data.Tools.MakeBoolean(h_sAdditionalData["DateTime_UserMustUseControl"], false)) {
					sAttributes += " readonly='readonly'";
				}

					//#### Write out the date/time input field (Formating(the return value)For(the)Form as we go)
				sReturn = JavaScript.GenerateFileReferences(JavaScript.enumJavaScriptFiles.cnCnInputsDateTime, oSettings) + oSettings.CRLF +
					"<input type='text' name='" + sInputName + "' id='" + sInputName + "' value='" + Inputs.Tools.FormatForForm(r_sInitialValue) + "' " + sAttributes + " />" + oSettings.CRLF +
					sPopUpButtonCode
				;
			}

				//#### Return the above determined sReturn value to the caller
			return sReturn;
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML option form element based on the provided picklist.
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="sInitialValue">String representing the initial value of the input.</param>
		/// <param name="sContainerAttributes">String representing the additional HTML attributes to apply to the containing DIV.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="bIsSpecialListBox">Boolean value indicating if the input is to be printed as a DHTML option list box.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>September 7, 2007</LastUpdated>
		private string DoOption(string sFunction, string sInputName, string sAttributes, string sInitialValue, string sContainerAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings, bool bIsSpecialListBox) {
			StringBuilder oReturn = new StringBuilder();

				//#### If the passed oPicklist has .Rows (and is not null)
			if (oPicklist != null && oPicklist.RowCount > 0) {
					//#### Ensure the passed sInitialValue is a valid string
				sInitialValue = Cn.Data.Tools.MakeString(sInitialValue, "");

					//#### If we're in .IsReadOnly mode
				if (oSettings.IsReadOnly) {
						//#### .Decode the passed sInitialValue from the passed oPicklist
					oReturn.Append(Cn.Data.Picklists.Decoder(oPicklist, sInitialValue, oSettings.StrictPicklistDecodes) + oSettings.CRLF);
				}
					//#### Else we need to output the option input group
				else {
					string sSpecialListBox_LabelAttributes;
					string sCurrentData;
					string sLabelSpacer;
					string sDelimiter;
					string sInput = "";
					int iRowCount = oPicklist.RowCount;
					int iLen = sInitialValue.Length;
					int i;
					bool bPrintLabelFirst;
					bool bIsActive;
					bool bFound = false;

						//#### If the passed h_sAdditionalData is null, init it to a new Hashtable (so we don't raise any errors below)
						//####     NOTE: Renderer does this for us, but we are not guarenteed that a direct developer call to .RenderOptionInput will be non-null
					if (h_sAdditionalData == null) {
						h_sAdditionalData = new Hashtable();
					}

						//#### Collect the values from within the passed h_sAdditionalData into the local vars
					sSpecialListBox_LabelAttributes = Cn.Data.Tools.MakeString(h_sAdditionalData["SpecialListBox_LabelAttributes"], "");
					sLabelSpacer = Cn.Data.Tools.MakeString(h_sAdditionalData["MultiValue_LabelSpacer"], "");
					sDelimiter = Cn.Data.Tools.MakeString(h_sAdditionalData["MultiValue_Delimiter"], "");
					bPrintLabelFirst = Cn.Data.Tools.MakeBoolean(h_sAdditionalData["MultiValue_PrintLabelFirst"], false);

						//#### If this bIs(a)SpecialListBox
					if (bIsSpecialListBox) {
							//#### If we are supposed to _Include(the)HoverJavaScript
						if (Cn.Data.Tools.MakeBoolean(h_sAdditionalData["SpecialListBox_IncludeHoverJavaScript"], false)) {
								//#### Append the JavaScript conde onto the sSpecialListBox_LabelAttributes and .Write out the .cnCnTools JavaScript
							sSpecialListBox_LabelAttributes += " onMouseOver='Cn._.t.AddClass(this, \"hover\");' onMouseOut='Cn._.t.RemoveClass(this, \"hover\");'";
							oReturn.Append(JavaScript.GenerateFileReferences(JavaScript.enumJavaScriptFiles.cnCnTools, oSettings));
						}

							//#### Borrow the use of sInput to determine the sCSSClass_SpecialListBox
						sInput = Data.Tools.MakeString(Cn.Platform.Specific.AppSettings("CSSClass_SpecialListBox"), g_cDefaultCSSClass_SpecialListBox);

							//#### Write out the top of the containing div and the unordered list
							//####     NOTE: Since none of the inputs below are set with an id of sInputName, the div can correctly us it as it's id
						oReturn.Append("<div id='" + sInputName +
							"' class='" + Data.Tools.MakeString(h_sAdditionalData["SpecialListBox_CSSClass"], sInput) + "' " +
							sContainerAttributes + " >" +
							"<ul>"
						);

							//#### Reset the value of the above borrowed sInput to a null-string
						sInput = "";
					}
						//#### Else this bIs(not a)SpecialListBox 
					else {
							//#### Write out the containing div
							//####     NOTE: Since none of the inputs below are set with an id of sInputName, the div can correctly us it as it's id
						oReturn.Append("<div id='" + sInputName + "' " + sContainerAttributes + " >");
					}

						//#### Traverse the passed oPicklist
					for (i = 0; i < iRowCount; i++) {
							//#### Reset the sCurrentData and bIsActive for this loop
						sCurrentData = oPicklist.Value(i, "Data");
						bIsActive = Data.Tools.MakeBoolean(oPicklist.Value(i, "IsActive"), true);

							//#### If we've not yet bFound the sInitialValue and the sCurrentData is equal to the passed sInitialValue (checking their .Lengths first as that is a far faster comparison)
						if (! bFound && iLen == sCurrentData.Length && sInitialValue == sCurrentData) {
								//#### If this bIs(a)SpecialListBox, set the sInput to a CHECKED radio input with an id (Formating(the sInitialValue)For(the)Form as we go)
								//####     NOTE: The id's are required by the label tags so that when they are clicked on, the asso. input is fired
							if (bIsSpecialListBox) {
								sInput = "<input type='radio' name='" + sInputName + "' id='" + i + "_" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sCurrentData) + "' " + sAttributes + " CHECKED='True' />";
							}
								//#### Else this bIs(not a)SpecialListBox, so set the sInput to a CHECKED radio input without an id (Formating(the sInitialValue)For(the)Form as we go)
							else {
								sInput = "<input type='radio' name='" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sCurrentData) + "' " + sAttributes + " CHECKED='True' />";
							}

								//#### Ensure that bIsActive is true so that the label is printed below, and flip bFound to true
							bIsActive = true;
							bFound = true;
						}
							//#### Else if the current .Value bIsActive, set sInput to a non-CHECKED radio (.Formating(the sInitialValue)For(the)Form as we go)
						else if (bIsActive) {
								//#### If this bIs(a)SpecialListBox, set the sInput to a radio input with an id (Formating(the sInitialValue)For(the)Form as we go)
								//####     NOTE: The id's are required by the label tags so that when they are clicked on, the asso. input is fired
							if (bIsSpecialListBox) {
								sInput = "<input type='radio' name='" + sInputName + "' id='" + i + "_" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sCurrentData) + "' " + sAttributes + " />";
							}
								//#### Else this bIs(not a)SpecialListBox, so set the sInput to a radio input without an id (Formating(the sInitialValue)For(the)Form as we go)
							else {
								sInput = "<input type='radio' name='" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sCurrentData) + "' " + sAttributes + " />";
							}
						}

							//#### If the current .Value bIsActive (or matched the sInitialValue above)
						if (bIsActive) {
								//#### If we're supposed to bPrint(the)LabelFirst
							if (bPrintLabelFirst) {
									//#### If this bIs(a)SpecialListBox
									//####     NOTE: It really doesn't make sence to print the sInput after the Description, but the developer is allowed to do so if they want to...
								if (bIsSpecialListBox) {
										//#### Pre/append the required additional HTML around the sInput (figuring out if this is an even input and outputting the CSS if it is as we go)
									oReturn.Append("<li ");
									if ((i % 2) != 0) { oReturn.Append(" class='alt'"); }
									oReturn.Append("><label for='" + i + "_" + sInputName + "' " + sSpecialListBox_LabelAttributes + ">" +
										oPicklist.Value(i, "Description") + sLabelSpacer + sInput +
										"</label></li>" + oSettings.CRLF
									);
								}
									//#### Else this is a standard checkbox group, so print out the sInput accordingly
								else {
									oReturn.Append(oPicklist.Value(i, "Description") + sLabelSpacer + sInput + oSettings.CRLF);
								}
							}
								//#### Else we're supposed to print the label last
							else {
									//#### If this bIs(a)SpecialListBox
								if (bIsSpecialListBox) {
										//#### Pre/append the required additional HTML around the sInput (figuring out if this is an even input and outputting the CSS if it is as we go)
									oReturn.Append("<li ");
									if ((i % 2) != 0) { oReturn.Append(" class='alt'"); }
									oReturn.Append("><label for='" + i + "_" + sInputName + "' " + sSpecialListBox_LabelAttributes + ">" +
										sInput + sLabelSpacer + oPicklist.Value(i, "Description") +
										"</label></li>" + oSettings.CRLF
									);
								}
									//#### Else this is a standard checkbox group, so print out the sInput accordingly
								else {
									oReturn.Append(sInput + sLabelSpacer + oPicklist.Value(i, "Description") + oSettings.CRLF);
								}
							}

								//#### If we are still in the midst of the oPicklist, print out the sDelimiter
							if (i < (iRowCount - 1)) {
								oReturn.Append(sDelimiter);
							}
						}
					}

						//#### If this bIs(a)SpecialListBox, write out the bottom of the unordered list
					if (bIsSpecialListBox) {
						oReturn.Append("</ul>");
					}

						//#### Write out the bottom of the containing div
					oReturn.Append("</div>");
				}
			}
				//#### Else the passed oPicklist has no .Rows (or is null), so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + sFunction, Configuration.Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_PicklistIsEmpty, "", "");
			}

				//#### Return the oReturn value to the caller
			return oReturn.ToString();
		}

		///############################################################
		/// <summary>
		/// Renders an XHTML checkbox group or a DHTML checked list box form element based on the provided picklist.
		/// </summary>
		/// <param name="sFunction">String representing the calling function's name.</param>
		/// <param name="sInputName">String representing the name of the input.</param>
		/// <param name="sAttributes">String representing the additional HTML attributes to apply to the input.</param>
		/// <param name="a_sInitialValues">String array where each element represents an initial value of the input.</param>
		/// <param name="sContainerAttributes">String representing the additional HTML attributes to apply to the containing DIV.</param>
		/// <param name="oPicklist">MultiArray representing the single picklist to render.</param>
		/// <param name="h_sAdditionalData">Hashtable of strings representing the additionally definable properties of the input (see <see cref='Cn.Web.Renderer.Form.Add'>Renderer.Form.Add</see>'s remarks).</param>
		/// <param name="bIsSpecialListBox">Boolean value indicating if the input is to be printed as a DHTML checked list box.</param>
		/// <exception cref="Cn.CnException">Thrown when the passed <paramref>oPicklist</paramref> is null or contains no <c>Rows</c>.</exception>
		/// <returns>String representing the XHTML control.</returns>
		///############################################################
		/// <LastUpdated>September 7, 2007</LastUpdated>
		private string DoCheckBoxes(string sFunction, string sInputName, string sAttributes, string[] a_sInitialValues, string sContainerAttributes, MultiArray oPicklist, Hashtable h_sAdditionalData, Settings.Current oSettings, bool bIsSpecialListBox) {
			StringBuilder oReturn = new StringBuilder();
			string sSpecialListBox_LabelAttributes;
			string sLabelSpacer;
			string sDelimiter;
			bool bPrintLabelFirst;

				//#### If the passed oPicklist has .Rows (and is not null)
			if (oPicklist != null && oPicklist.RowCount > 0) {
					//#### If the passed h_sAdditionalData is null, init it to a new Hashtable (so we don't raise any errors below)
					//####     NOTE: Renderer does this for us, but we are not guarenteed that a direct developer call to .RenderOptionInput will be non-null
				if (h_sAdditionalData == null) {
					h_sAdditionalData = new Hashtable();
				}

					//#### Load sSpecialListBox_LabelAttributes, sDelimiter, sLabelSpacer and bPrintLabelFirst with the data within the passed h_sAdditionalData
				sSpecialListBox_LabelAttributes = Cn.Data.Tools.MakeString(h_sAdditionalData["SpecialListBox_LabelAttributes"], "");
				sLabelSpacer = Cn.Data.Tools.MakeString(h_sAdditionalData["MultiValue_LabelSpacer"], "");
				sDelimiter = Cn.Data.Tools.MakeString(h_sAdditionalData["MultiValue_Delimiter"], "");
				bPrintLabelFirst = Cn.Data.Tools.MakeBoolean(h_sAdditionalData["MultiValue_PrintLabelFirst"], false);

					//#### If we're in .IsReadOnly mode
				if (oSettings.IsReadOnly) {
						//#### If the passed a_sInitialValues is not null
					if (a_sInitialValues != null) {
							//#### .Decode the passed a_sInitialValues from the passed oPicklist
						a_sInitialValues = Cn.Data.Picklists.Decoder(oPicklist, a_sInitialValues, oSettings.StrictPicklistDecodes);

							//#### If we are supposed to bPrint(the)LabelFirst
						if (bPrintLabelFirst) {
								//#### Write out the .Joined a_sInitialValues with the defined sLabelSpacer and sDelimiter
							oReturn.Append(string.Join(sLabelSpacer + sDelimiter, a_sInitialValues) + oSettings.CRLF);
						}
							//#### Else we aren't supposed to bPrint(the)LabelFirst
						else {
								//#### Write out the .Joined a_sInitialValues with the defined sDelimiter and sLabelSpacer
							oReturn.Append(string.Join(sDelimiter + sLabelSpacer, a_sInitialValues) + oSettings.CRLF);
						}
					}
				}
					//#### Else we need to output the checkboxes input group
				else {
					string sCurrentData;
					string sInput = "";
					int iInitialValuesLen = -1;
					int iRowCount = oPicklist.RowCount;
					int iLen;
					int i;
					int j;
					bool bIsActive;
					bool bFound;

						//#### If the passed a_sInitialValues is not null, determine the iInitialValuesLen
					if (a_sInitialValues != null) {
						iInitialValuesLen = a_sInitialValues.Length;
					}

						//#### If this bIs(a)SpecialListBox
					if (bIsSpecialListBox) {
							//#### If we are supposed to _Include(the)HoverJavaScript
						if (Cn.Data.Tools.MakeBoolean(h_sAdditionalData["SpecialListBox_IncludeHoverJavaScript"], false)) {
								//#### Append the JavaScript conde onto the sSpecialListBox_LabelAttributes and .Write out the .cnCnTools JavaScript
							sSpecialListBox_LabelAttributes += " onMouseOver='Cn._.t.AddClass(this, \"hover\");' onMouseOut='Cn._.t.RemoveClass(this, \"hover\");'";
							oReturn.Append(JavaScript.GenerateFileReferences(JavaScript.enumJavaScriptFiles.cnCnTools, oSettings));
						}

							//#### Borrow the use of sInput to determine the sCSSClass_SpecialListBox
						sInput = Data.Tools.MakeString(Cn.Platform.Specific.AppSettings("CSSClass_SpecialListBox"), g_cDefaultCSSClass_SpecialListBox);

							//#### Write out the containing div and the top of the unordered list
							//####     NOTE: Since none of the inputs below are set with an id of sInputName, the div can correctly us it as it's id
						oReturn.Append("<div id='" + sInputName +
							"' class='" + Data.Tools.MakeString(h_sAdditionalData["SpecialListBox_CSSClass"], sInput) + "' " +
							sContainerAttributes + " >" +
							"<ul>"
						);

							//#### Reset the above borrowed sInput to a null-string
						sInput = "";
					}
						//#### Else this bIs(not a)SpecialListBox 
					else {
							//#### Write out the containing div
							//####     NOTE: Since none of the inputs below are set with an id of sInputName, the div can correctly us it as it's id
						oReturn.Append("<div id='" + sInputName + "' " + sContainerAttributes + " >");
					}

						//#### Traverse the oPicklist
					for (i = 0; i < iRowCount; i++) {
							//#### Reset the sCurrentData, it's iLen, bIsActive and bFound for this loop
						sCurrentData = oPicklist.Value(i, "Data");
						iLen = sCurrentData.Length;
						bIsActive = Data.Tools.MakeBoolean(oPicklist.Value(i, "IsActive"), true);
						bFound = false;

							//#### Traverse the passed a_sInitialValues
						for (j = 0; j < iInitialValuesLen; j++) {
								//#### If the sCurrentData is equal to the current a_sInitialValue (checking their Lens first as that is a far faster comparison)
							if (a_sInitialValues[j].Length == iLen && a_sInitialValues[j] == sCurrentData) {
									//#### If this bIs(a)SpecialListBox, set the sInput to a CHECKED checkbox input with an id (Formating(the sInitialValue)For(the)Form as we go)
									//####     NOTE: The id's are required by the label tags so that when they are clicked on, the asso. input is fired
								if (bIsSpecialListBox) {
									sInput = "<input type='checkbox' name='" + sInputName + "' id='" + i + "_" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sCurrentData) + "' " + sAttributes + " CHECKED='True' />";
								}
									//#### Else this bIs(not a)SpecialListBox, so set the sInput to a CHECKED checkbox input without an id (Formating(the sInitialValue)For(the)Form as we go)
								else {
									sInput = "<input type='checkbox' name='" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sCurrentData) + "' " + sAttributes + " CHECKED='True' />";
								}

									//#### Ensure that bIsActive is true so that the label is printed below, flip bFound to true and exit the inner for loop
								bIsActive = true;
								bFound = true;
								break;
							}
						}

							//#### If the current .Value bIsActive (or matched the sInitialValue above)
						if (bIsActive) {
								//#### If the sCurrentData was not bFound above
							if (! bFound) {
									//#### If this bIs(a)SpecialListBox, set the sInput to a checkbox input with an id (.Formating(the sInitialValue)For(the)Form as we go)
									//####     NOTE: The id's are required by the label tags so that when they are clicked on, the asso. input is fired
								if (bIsSpecialListBox) {
									sInput = "<input type='checkbox' name='" + sInputName + "' id='" + i + "_" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sCurrentData) + "' " + sAttributes + " />";
								}
									//#### Else this bIs(not a)SpecialListBox, so set the sInput to a checkbox input without an id (.Formating(the sInitialValue)For(the)Form as we go)
								else {
									sInput = "<input type='checkbox' name='" + sInputName + "' value='" + Inputs.Tools.FormatForForm(sCurrentData) + "' " + sAttributes + " />";
								}
							}

								//#### If we're supposed to bPrint(the)LabelFirst
							if (bPrintLabelFirst) {
									//#### If this bIs(a)SpecialListBox
									//####     NOTE: It really doesnt make since to print the sInput after the Description, but the developer is allowed to do so if they want to...
								if (bIsSpecialListBox) {
										//#### Pre/append the required additional HTML around the sInput (figuring out if this is an even input and outputting the CSS if it is as we go)
									oReturn.Append("<li ");
									if ((i % 2) != 0) { oReturn.Append(" class='alt'"); }
									oReturn.Append("><label for='" + i + "_" + sInputName + "' " + sSpecialListBox_LabelAttributes + ">" +
										oPicklist.Value(i, "Description") + sLabelSpacer + sInput +
										"</label></li>" + oSettings.CRLF
									);
								}
									//#### Else this is a standard checkbox group, so print out the sInput accordingly
								else {
									oReturn.Append(oPicklist.Value(i, "Description") + sLabelSpacer + sInput + oSettings.CRLF);
								}
							}
								//#### Else we're supposed to print the label last
							else {
									//#### If this bIs(a)SpecialListBox
								if (bIsSpecialListBox) {
										//#### Pre/append the required additional HTML around the sInput (figuring out if this is an even input and outputting the CSS if it is as we go)
									oReturn.Append("<li ");
									if ((i % 2) != 0) { oReturn.Append(" class='alt'"); }
									oReturn.Append("><label for='" + i + "_" + sInputName + "' " + sSpecialListBox_LabelAttributes + ">" +
										sInput + sLabelSpacer + oPicklist.Value(i, "Description") +
										"</label></li>" + oSettings.CRLF
									);
								}
									//#### Else this is a standard checkbox group, so print out the sInput accordingly
								else {
									oReturn.Append(sInput + sLabelSpacer + oPicklist.Value(i, "Description") + oSettings.CRLF);
								}
							}

								//#### If we are still in the midst of the oPicklist, print out the sDelimiter
							if (i < (iRowCount - 1)) {
								oReturn.Append(sDelimiter);
							}
						}
					}

						//#### If this bIs(a)SpecialListBox, write out the bottom of the unordered list
					if (bIsSpecialListBox) {
						oReturn.Append("</ul>");
					}

						//#### Write out the bottom of the containing div
					oReturn.Append("</div>");
				}
			}
				//#### Else the passed oPicklist is null (or had no .Rows), so raise the error
			else {
				Internationalization.RaiseDefaultError(g_cClassName + sFunction, Configuration.Internationalization.enumInternationalizationValues.cnDeveloperMessages_FormRenderer_PicklistIsEmpty, "", "");
			}

				//#### Return the oReturn value to the caller
			return oReturn.ToString();
		}
		#endregion

	} //# public class HTMLBuilder


} //# namespace Cn.Web.Inputs