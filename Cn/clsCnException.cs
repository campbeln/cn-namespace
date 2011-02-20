/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;										//# Required to access Exception.
using System.Web;                                   //# Required to access Request, Response, Application, etc.
using System.Text;									//# Required to access StringBuilder.
using System.Reflection;							//# Required to access MemberInfo.
using System.Diagnostics;							//# Required to access StackFrame.
using System.Collections;				            //# Required to access the HashTable class
using System.Collections.Specialized;				//# Required to access NameValueCollection.
using System.Runtime.Serialization;					//# Required to access ISerializable


namespace Cn {

    ///########################################################################################################################
    /// <summary>
	/// Generic custom exception utilized by the Cn namespace.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>February 3, 2006</LastFullCodeReview>
	[Serializable()]
	public class CnException : Exception, ISerializable {
			//#### Define the required private variables
		private string g_sStackTrace;
		private int g_iErrorCode;


		//##########################################################################################
		//# Class Functions
		//##########################################################################################
        ///############################################################
        /// <summary>
		/// Initializes the class.
        /// </summary>
        ///############################################################
		/// <LastUpdated>November 13, 2009</LastUpdated>
		public CnException(string sSource, string sMessage, int iErrorCode) : base(sMessage) {
				//#### Set the base's .Source to the passed sSource (as well as setting the passed sMessage via the base's constructor)
			base.Source = sSource;
			g_sStackTrace = base.StackTrace;
			g_iErrorCode = iErrorCode;
		}
		

		//##########################################################################################
		//# ISerializable Required Functions
		//##########################################################################################
        ///############################################################
        /// <summary>
		/// Initializes the class based on the provided SerializationInfo.
        /// </summary>
		/// <param name="info">Standard SerializationInfo object.</param>
		/// <param name="ctxt">Standard StreamingContext object.</param>
        ///############################################################
		/// <LastUpdated>December 21, 2009</LastUpdated>
		public CnException(SerializationInfo info, StreamingContext ctxt) : base((string)info.GetValue("Message", typeof(string))) {
			base.Source = Cn.Data.Tools.MakeString(info.GetValue("Source", typeof(string)), "");
			g_iErrorCode = Cn.Data.Tools.MakeInteger(info.GetValue("ErrorCode", typeof(int)), -1);
			g_sStackTrace = Cn.Data.Tools.MakeString(info.GetValue("StackTrace", typeof(string)), "");
		}

        ///############################################################
        /// <summary>
		/// Stores the state of the class into the provided SerializationInfo.
        /// </summary>
		/// <param name="info">Standard SerializationInfo object.</param>
		/// <param name="ctxt">Standard StreamingContext object.</param>
        ///############################################################
		/// <LastUpdated>November 13, 2009</LastUpdated>
		public new void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
			info.AddValue("Source", base.Source);
			info.AddValue("Message", base.Message);
			info.AddValue("ErrorCode", g_iErrorCode);
			info.AddValue("StackTrace", g_sStackTrace);
		}


        //##########################################################################################
        //# Public Read-Only Properties
        //##########################################################################################
        ///############################################################
        /// <summary>
        /// Gets a string representation of the frames on the call stack at the time the current exception was thrown.
        /// </summary>
		/// <value>String representing the stack trace.</value>
        ///############################################################
		/// <LastUpdated>November 13, 2009</LastUpdated>
        public override string StackTrace {
			get { return g_sStackTrace; }
        }

        ///############################################################
        /// <summary>
        /// Gets the error code.
        /// </summary>
		/// <value>Integer representing the error code.</value>
        ///############################################################
		/// <LastUpdated>June 5, 2007</LastUpdated>
        public int ErrorCode {
			get { return g_iErrorCode; }
        }


        //##########################################################################################
        //# Public Functions
        //##########################################################################################
        ///############################################################
        /// <summary>
		/// Converts this instance into its formatted string representation.
        /// </summary>
		/// <returns>String representing the formatted <c>Exception</c>.</returns>
        ///############################################################
		/// <LastUpdated>February 3, 2006</LastUpdated>
		public override string ToString() {
			return ToString(this);
		}


        //##########################################################################################
        //# Public Static Error Reporting-Related Functions
        //##########################################################################################
		///###############################################################
		/// <summary>
		/// Determines if the referenced exception is a "\get_aspx_ver.aspx"-related error.
		/// </summary>
		/// <remarks>
		/// "\get_aspx_ver.aspx"-related errors can be ignored because we get them when this file is missing from our systems every time we startup Visual Studio.
		/// </remarks>
		/// <param name="oException">Object representing the exception to query.</param>
		/// <returns>Boolean value signaling if this is a "\get_aspx_ver.aspx"-related error.<para/>Returns true if it is a "\get_aspx_ver.aspx"-related error, and false if it is not.</returns>
		///###############################################################
		/// <LastUpdated>May 28, 2007</LastUpdated>
		public static bool IsGetASPXVersionError(Exception oException) {
				//#### Return based on if this is a "\get_aspx_ver.aspx"-related error (by seeing if the .Message ends in "\get_aspx_ver.aspx")
				//####     NOTE: "\get_aspx_ver.aspx"-related errors are ignored because we get them when this file is missing from our systems every time we startup Visual Studio
			return (oException.Message.IndexOf(@"\get_aspx_ver.aspx") == (oException.Message.Length - 18));
		}

        ///###############################################################
        /// <summary>
        /// Handles the provided exception, emailing the templated error report while showing the user the templated error page.
        /// </summary>
        /// <remarks>
        /// The following variables can be used within the referenced templates:
        /// <para/>* $SystemName - This variable will be replaced with the passed <paramref>sSystemName</paramref> within the referenced templates.
        /// <para/>* $ExceptionMessage - This variable will be replaced with the passed <paramref>oException</paramref>'s <c>Message</c> property value within the referenced templates.
        /// <para/>* $Exception - This variable will be replaced with the <see cref='ToString(System.Exception)'>ExceptionToString</see> version of the passed <paramref>oException</paramref> within the referenced templates.
        /// <para/>* $CurrentURL - This variable will be replaced with the current URL (i.e. - not including the script name) within the referenced templates.
        /// </remarks>
        /// <param name="oException">Object representing the exception to handle.</param>
        /// <param name="sSystemName">String representing the system this instance is related to.</param>
        /// <param name="sPathToErrorPageTemplate">String representing the file path to the error page template.</param>
		/// <param name="sSMTPServer">String representing the SMTP Mail Server to utilize to send the email.</param>
		/// <param name="sToEMailAddress">String representing a semicolon-delimited list of recipient email addresses.</param>
        /// <param name="sPathToEMailTemplate">String representing the file path to the EMail template.</param>
		/// <param name="bBodyIsHTML">Boolean value indicating if the email's body is to be sent as HTML formatted.</param>
        ///###############################################################
		/// <LastUpdated>June 18, 2010</LastUpdated>
        public static void ReportError(Exception oException, string sSystemName, string sPathToErrorPageTemplate, string sSMTPServer, string sToEMailAddress, string sPathToEMailTemplate, bool bBodyIsHTML) {
			HttpResponse Response = HttpContext.Current.Response;
			string sExceptionMessage = oException.Message;
			string sException = ToString(oException);
			string sCurrentURL = Web.Tools.CurrentURL();

				//#### Reset the values of the passed paths to the contents of the referenced oFiles
			sPathToEMailTemplate = Cn.Data.Tools.MakeString(Platform.Specific.ReadFromFile(sPathToEMailTemplate), "Unable to load Error EMail template!");
			sPathToErrorPageTemplate = Cn.Data.Tools.MakeString(Platform.Specific.ReadFromFile(sPathToErrorPageTemplate), "Unable to load Error Page template!");

				//#### Replace the variables within the templates with their values
			sPathToEMailTemplate = sPathToEMailTemplate.Replace("$SystemName", sSystemName);
			sPathToEMailTemplate = sPathToEMailTemplate.Replace("$ExceptionMessage", sExceptionMessage);
			sPathToEMailTemplate = sPathToEMailTemplate.Replace("$Exception", sException);
			sPathToEMailTemplate = sPathToEMailTemplate.Replace("$CurrentURL", sCurrentURL);
			sPathToErrorPageTemplate = sPathToErrorPageTemplate.Replace("$SystemName", sSystemName);
			sPathToErrorPageTemplate = sPathToErrorPageTemplate.Replace("$ExceptionMessage", sExceptionMessage);
			sPathToErrorPageTemplate = sPathToErrorPageTemplate.Replace("$Exception", sException);
			sPathToErrorPageTemplate = sPathToErrorPageTemplate.Replace("$CurrentURL", sCurrentURL);

				//#### .Send the EMail to the passed sToEMailAddress
//			Net.Mail.Send(sSMTPServer, sToEMailAddress, HttpContext.Current.User.Identity.Name,
//				"\"" + sSystemName + "\" encountered an error!", sPathToEMailTemplate, bBodyIsHTML
//			);
			Net.Mail.Send(sSMTPServer, sToEMailAddress, sToEMailAddress,
				"\"" + sSystemName + "\" encountered an error!", sPathToEMailTemplate, bBodyIsHTML
			);

				//#### Clear the Response buffer
			Response.Clear();
			Response.ClearContent();

				//#### Render the error page template for the user
			Response.Write(sPathToErrorPageTemplate);

				//#### .Flush the Response buffer to the user, .Clear(the)Error and .End the Response
			Response.Flush();
			HttpContext.Current.Server.ClearError();
			Response.End();
        }

        ///###############################################################
		/// <summary>
		/// Converts the passed exception into its formatted string representation.
		/// </summary>
		/// <param name="oException">Exception you wish to convert into a formatted string.</param>
		/// <returns>String representing the formatted <paramref>oException</paramref>.</returns>
        ///###############################################################
		/// <LastUpdated>February 3, 2006</LastUpdated>
		public static string ToString(Exception oException) {
			ParameterInfo[] a_oMethodParams;
			Hashtable h_sAssemblyInfo = GetAssemblyInfo();
			StringBuilder o_sReturn = new StringBuilder("");
			StackTrace oStackTrace = new StackTrace(oException);
			StackFrame oStackFrame;
			MemberInfo oMethodInfo;
			int i;
			int j;

				//#### If the passed oException has an .InnerException
			if (oException.InnerException != null) {
					//#### Render the innder exceptions header, then recurse to render the .InnerException
				o_sReturn.Append("###############################################################\n");
				o_sReturn.Append("# Inner Exception #############################################\n");
				o_sReturn.Append("###############################################################\n\n");
				o_sReturn.Append(ToString(oException.InnerException) + "\n\n\n");

					//#### Now render the header outer exception
				o_sReturn.Append("###############################################################\n");
				o_sReturn.Append("# Outer Exception #############################################\n");
				o_sReturn.Append("###############################################################\n\n");
			}

			//##########
			//##########

				//#### Render the exception information header
			o_sReturn.Append("# Exception Information\n");
			o_sReturn.Append("########################################\n");

				//#### Render the current date/time
			o_sReturn.Append("Exception Occured:           " + DateTime.Now + "\n");

				//#### Render the current URL
			o_sReturn.Append("Exception's Source URL:      " + Web.Tools.CurrentURL() + "\n");

				//#### Safely render the oException's type
			o_sReturn.Append("Exception's Type:            ");			
			try {
				o_sReturn.Append(oException.GetType().FullName + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

				//#### Safely render the oException's .Source
			o_sReturn.Append("Exception's Source:          ");			
			try {
				o_sReturn.Append(oException.Source + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

				//#### Safely render the oException's .TargetSite
			o_sReturn.Append("Exception's TargetSite:      ");			
			try {
				o_sReturn.Append(oException.TargetSite.Name + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

				//#### Safely render the oException's .Message
			o_sReturn.Append("Exception's Message:         ");			
			try {
				o_sReturn.Append(oException.Message + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

				//#### Safely render the oException's .Message
			o_sReturn.Append("Exception's StackTrace:\n\n");
			try {
					//#### Traverse the oStackTrace's Frames, rendering each as we go
				for (i = 0; i < oStackTrace.FrameCount; i++) {
						//#### Collect the oStackFrame, oMethodInfo and a_oMethodParams for this loop
					oStackFrame = oStackTrace.GetFrame(i);
					oMethodInfo = oStackFrame.GetMethod();
					a_oMethodParams = oStackFrame.GetMethod().GetParameters();

						//#### Render the path to the method
					o_sReturn.Append("     " + oMethodInfo.DeclaringType.Namespace + "." + oMethodInfo.DeclaringType.Name + "." + oMethodInfo.Name + "(");

						//#### If we have a_oMethodParams to render
					if (a_oMethodParams != null && a_oMethodParams.Length > 0) {
							//#### Traverse the a_oMethodParams (not including the last a_oMethodParam), rendering each as we go with a trailing ", "
						for (j = 0; j < a_oMethodParams.Length - 1; j++) {
							o_sReturn.Append(a_oMethodParams[j].ParameterType.Name + " " + a_oMethodParams[j].Name + ", ");
						}

							//#### Render the last a_oMethodParam (sans the trailing ", ")
						o_sReturn.Append(a_oMethodParams[j].ParameterType.Name + " " + a_oMethodParams[j].Name);
					}

						//#### Render the closing paren
					o_sReturn.Append(")\n\n");
				}
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

			//##########
			//##########

				//#### Render the enviroment information header
			o_sReturn.Append("\n# Enviroment Information\n");
			o_sReturn.Append("########################################\n");

				//#### Safely render the .MachineName
			o_sReturn.Append("Server's MachineName:        ");
			try {
				o_sReturn.Append(Environment.MachineName + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

				//#### Render the runtime version
			o_sReturn.Append("dotNET Runtime Version:      " + Environment.Version + "\n");

				//#### Safely render the application's domain
			o_sReturn.Append("Application's Domain:        ");
			try {
				o_sReturn.Append(AppDomain.CurrentDomain.FriendlyName + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

				//#### Safely render the username who started the current thread
			o_sReturn.Append("Thread's Username:           ");
			try {
				o_sReturn.Append(Environment.UserName + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

				//#### Safely render the impersonated username
			o_sReturn.Append("Impersonated Username:       ");
			try {
				o_sReturn.Append(System.Security.Principal.WindowsIdentity.GetCurrent().Name + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

				//#### Safely render the authenticated username
			o_sReturn.Append("Authenticated Username:      ");
			try {
				o_sReturn.Append(HttpContext.Current.User.Identity.Name + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

			//##########
			//##########

				//#### Render the remote information (inc. its header)
			o_sReturn.Append("\n\n# Remote Information\n");
			o_sReturn.Append("########################################\n");
			o_sReturn.Append("Remote Username:             " + HttpContext.Current.Request.ServerVariables["REMOTE_USER"] + "\n");
			o_sReturn.Append("Remote Address:              " + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "\n");
			o_sReturn.Append("Remote Host:                 " + HttpContext.Current.Request.ServerVariables["REMOTE_HOST"] + "\n");

			//##########
			//##########

				//#### Render the assembly information header
			o_sReturn.Append("\n\n# Assembly Information\n");
			o_sReturn.Append("########################################\n");

				//#### Safely render the assembly's codebase
			o_sReturn.Append("Assembly Codebase:           ");			
			try {
				o_sReturn.Append(h_sAssemblyInfo["CodeBase"] + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

				//#### Safely render the assembly's full name
			o_sReturn.Append("Assembly Full Name:          ");			
			try {
				o_sReturn.Append(h_sAssemblyInfo["FullName"] + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}
			
				//#### Safely render the assembly's version
			o_sReturn.Append("Assembly Version:            ");
			try {
				o_sReturn.Append(h_sAssemblyInfo["Version"] + "\n");
			}
			catch (Exception ex) {
				o_sReturn.Append("[Error:[" + ex.Message + "]]\n");
			}

			//##########
			//##########

				//#### Render the variable collections header
			o_sReturn.Append("\n\n# Variable Collections\n");
			o_sReturn.Append("########################################\n");

				//#### Render the QueryString, Form, Cookies and ServerVariables collections
			o_sReturn.Append("QueryString Collection:\n");
			o_sReturn.Append(ToString(HttpContext.Current.Request.QueryString));
			o_sReturn.Append("\nForm Collection:\n");
			o_sReturn.Append(ToString(HttpContext.Current.Request.Form));
			o_sReturn.Append("\nCookies:\n");
			o_sReturn.Append(ToString(HttpContext.Current.Request.Cookies));
			o_sReturn.Append("\nServerVariables Collection:\n");
			o_sReturn.Append(ToString(HttpContext.Current.Request.ServerVariables));

			//##########
			//##########

				//#### Return the above determined o_sReturn.ToString'd value to the caller
			return o_sReturn.ToString();
		}

        ///###############################################################
		/// <summary>
		/// Converts the passed collection into its formatted string representation.
		/// </summary>
		/// <param name="oCollection">NameValueCollection you wish to convert into a formatted string.</param>
		/// <returns>String representing the formatted <paramref>oCollection</paramref>.</returns>
        ///###############################################################
		/// <LastUpdated>February 11, 2010</LastUpdated>
		public static string ToString(NameValueCollection oCollection) {
			StringBuilder o_sReturn = new StringBuilder("");

				//#### If the caller passed in a valid oCollection
			if (oCollection != null && oCollection.HasKeys()) {
					//#### Traverse the oCollection one sKey at a time, rendering each sKey as we go
				foreach (string sKey in oCollection.AllKeys) {
					o_sReturn.Append("	" + Cn.Data.Tools.RPad(sKey, " ", 30) + oCollection[sKey] + "\n");
				}
			}

				//#### Return the above determined o_sReturn.ToString'd value to the caller
			return o_sReturn.ToString();
		}

        ///###############################################################
		/// <summary>
		/// Converts the current HTTP context's Cookies into their formatted string representation.
		/// </summary>
		/// <returns>String representing the formatted <c>Cookies</c>.</returns>
        ///###############################################################
		/// <LastUpdated>February 11, 2010</LastUpdated>
		public static string ToString(HttpCookieCollection oCookieCollection) {
			StringBuilder o_sReturn = new StringBuilder("");

				//#### If we have .Cookies to process
			if (oCookieCollection != null && oCookieCollection.Count > 0) {
					//#### Traverse the .Cookies one sCookieName at a time, rendering each sCookieName as we go
				foreach (string sCookieName in oCookieCollection.AllKeys) {
						//#### If the sCookieName is null, use the hardcoded "[NULL]" for the cookie .Value
					if (oCookieCollection[sCookieName] == null) {
						o_sReturn.Append("	" + Cn.Data.Tools.RPad(sCookieName, " ", 30) + "[NULL]\n"
						);
					}
						//#### Else there is a valid object at sCookieName, so use it's .Value
					else {
						o_sReturn.Append("	" + Cn.Data.Tools.RPad(sCookieName, " ", 30) + oCookieCollection[sCookieName].Value + "\n"
						);
					}
				}
			}

				//#### Return the above determined o_sReturn.ToString'd value to the caller
			return o_sReturn.ToString();
		}


        //##########################################################################################
        //# Private Static Functions
        //##########################################################################################
        ///###############################################################
		/// <summary>
		/// Retrieves the running system's assembly information.
		/// </summary>
		/// <returns>Hashtable where each index represents a key/value pair of the running systems assembly information.</returns>
        ///###############################################################
		/// <LastUpdated>March 5, 2007</LastUpdated>
		private static Hashtable GetAssemblyInfo() {
			Hashtable h_sReturn = new Hashtable();
			Assembly oAssembly = Assembly.GetEntryAssembly();
			
				//#### If the oAssembly was not successfully collected via .GetEntryAssembly above, reset it to .GetCallingAssembly
			if (oAssembly == null) {
				oAssembly = Assembly.GetCallingAssembly();
			}

				//#### If the oAssembly is still null, reset our h_sReturn value to null
			if (oAssembly == null) {
				h_sReturn = null;
			}
				//#### Else we have a valid oAssembly
			else {
/*				object[] a_oAttributes;
				string sName;
				string sValue;

					//#### Collect the a_oAttributes from the oAssembly
				a_oAttributes = oAssembly.GetCustomAttributes(false);

					//#### Traverse the a_oAttributes, one oAttribute at a time
				foreach (Object oAttribute in a_oAttributes) {
						//#### Collect the sName and reset the sValue for this loop
					sName = oAttribute.GetType().ToString();
					sValue = "";

						//#### Determine the value of sName and process accordingly
					switch (sName) {
						case "System.Reflection.AssemblyTrademarkAttribute": {
							sName = "Trademark";
							sValue = ((AssemblyTrademarkAttribute)oAttribute).Trademark.ToString();
							break;
						}
						case "System.Reflection.AssemblyProductAttribute": {
							sName = "Product";
							sValue = ((AssemblyProductAttribute)oAttribute).Product.ToString();
							break;
						}
						case "System.Reflection.AssemblyCopyrightAttribute": {
							sName = "Copyright";
							sValue = ((AssemblyCopyrightAttribute)oAttribute).Copyright.ToString();
							break;
						}
						case "System.Reflection.AssemblyCompanyAttribute": {
							sName = "Company";
							sValue = ((AssemblyCompanyAttribute)oAttribute).Company.ToString();
							break;
						}
						case "System.Reflection.AssemblyTitleAttribute": {
							sName = "Title";
							sValue = ((AssemblyTitleAttribute)oAttribute).Title.ToString();
							break;
						}
						case "System.Reflection.AssemblyDescriptionAttribute": {
							sName = "Description";
							sValue = ((AssemblyDescriptionAttribute)oAttribute).Description.ToString();
							break;
						}
						default: {
						  //sName = sName;
							sValue = "";
							break;
						}
					}

						//#### If a sValue was set above, .Add it into the h_sReturn value
					if (sValue.Length > 0) {
						h_sReturn.Add(sName, sValue);
					}
				}
*/
					//#### .Add in the additional assembly info
				h_sReturn.Add("CodeBase", oAssembly.CodeBase.Replace("file:///", ""));
				h_sReturn.Add("Version", oAssembly.GetName().Version.ToString());
				h_sReturn.Add("FullName", oAssembly.FullName);
			}

				//#### Return the above determined h_sReturn value to the caller
			return h_sReturn;
		}

	} //# class CnException

} //# namespace Cn
