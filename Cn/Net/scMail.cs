/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Net.Mail;								//# Required to access MailAttachment, MailMessage, SmtpMail


namespace Cn.Net {

    ///########################################################################################################################
    /// <summary>
	/// Sends email via the provided SMTP server details.
    /// </summary>
    ///########################################################################################################################
	/// <LastFullCodeReview>November 17, 2009</LastFullCodeReview>
	public class Mail {
            //#### Declare the required private constants
	  //private const string g_cClassName = "Cn.Net.Mail.";


        //##########################################################################################
        //# Public Static Functions
        //##########################################################################################
        ///###############################################################
		/// <summary>
		/// Sends an email.
		/// </summary>
		/// <param name="sSMTPServer">String representing the SMTP Mail Server to utilize to send the email.</param>
		/// <param name="sTo">String representing a semicolon-delimited list of recipient email addresses.</param>
		/// <param name="sFrom">String representing the sender's email address.</param>
		/// <param name="sSubject">String representing the email's subject.</param>
		/// <param name="sBody">String representing the email's body.</param>
		/// <param name="bBodyIsHTML">Boolean value indicating if the email's body is to be sent as HTML formatted.</param>
        ///###############################################################
		/// <LastUpdated>May 3, 2007</LastUpdated>
		public static void Send(string sSMTPServer, string sTo, string sFrom, string sSubject, string sBody, bool bBodyIsHTML) {
			string[] a_sJunk = null;

				//#### Pass the call off to our sibling implementation to do the actual work
			Send(sSMTPServer, sTo, sFrom, sSubject, sBody, bBodyIsHTML, a_sJunk, "", "");
		}

        ///###############################################################
		/// <summary>
		/// Sends an email.
		/// </summary>
		/// <param name="sSMTPServer">String representing the SMTP Mail Server to utilize to send the email.</param>
		/// <param name="sTo">String representing a semicolon-delimited list of recipient email addresses.</param>
		/// <param name="sFrom">String representing the sender's email address.</param>
		/// <param name="sSubject">String representing the email's subject.</param>
		/// <param name="sBody">String representing the email's body.</param>
		/// <param name="bBodyIsHTML">Boolean value indicating if the email's body is to be sent as HTML formatted.</param>
		/// <param name="sAttachmentPath">String representing the file path to the email's attachment.</param>
        ///###############################################################
		/// <LastUpdated>May 3, 2007</LastUpdated>
		public static void Send(string sSMTPServer, string sTo, string sFrom, string sSubject, string sBody, bool bBodyIsHTML, string sAttachmentPath) {
			string[] a_sAttachmentPaths = new string[1];

				//#### Put the passed sAttachmentPath into the a_sAttachmentPaths (as an array is expected)
			a_sAttachmentPaths[0] = sAttachmentPath;

				//#### Pass the call off to our sibling implementation to do the actual work
			Send(sSMTPServer, sTo, sFrom, sSubject, sBody, bBodyIsHTML, a_sAttachmentPaths, "", "");
		}

        ///###############################################################
		/// <summary>
		/// Sends an email.
		/// </summary>
		/// <param name="sSMTPServer">String representing the SMTP Mail Server to utilize to send the email.</param>
		/// <param name="sTo">String representing a semicolon-delimited list of recipient email addresses.</param>
		/// <param name="sFrom">String representing the sender's email address.</param>
		/// <param name="sSubject">String representing the email's subject.</param>
		/// <param name="sBody">String representing the email's body.</param>
		/// <param name="bBodyIsHTML">Boolean value indicating if the email's body is to be sent as HTML formatted.</param>
		/// <param name="a_sAttachmentPaths">String array representing the file paths to the email's attachments.</param>
		/// <param name="sCC">String representing a semicolon-delimited list of email addresses to recieve a "carbon copy" (CC).</param>
		/// <param name="sBCC">String representing a semicolon-delimited list of email addresses to recieve a "blind carbon copy" (BCC).</param>
        ///###############################################################
		/// <LastUpdated>November 17, 2009</LastUpdated>
		public static void Send(string sSMTPServer, string sTo, string sFrom, string sSubject, string sBody, bool bBodyIsHTML, string[] a_sAttachmentPaths, string sCC, string sBCC) {
			Attachment oAttachment;
			MailMessage oMessage;
			SmtpClient oSMTPClient = new SmtpClient(sSMTPServer);
			int i;

				//#### Setup the base oMessage and its additional properties to the passed info
			oMessage = new MailMessage(sFrom, sTo, sSubject, sBody);
			if (sCC.Length > 0) { oMessage.CC.Add(sCC); }
			if (sBCC.Length > 0) { oMessage.Bcc.Add(sBCC); }
			oMessage.IsBodyHtml = bBodyIsHTML;

				//#### If the caller passed in some a_sAttachmentPaths
			if (a_sAttachmentPaths != null && a_sAttachmentPaths.Length > 0) {
					//#### Traverse the a_sAttachmentPaths
				for (i = 0; i < a_sAttachmentPaths.Length; i++) {
						//#### If the current a_sAttachmentPath .Is(not)NullOrEmpty, attach it to the oMessage
					if (! string.IsNullOrEmpty(a_sAttachmentPaths[i])) {
						oAttachment = new Attachment(a_sAttachmentPaths[i]);
						oMessage.Attachments.Add(oAttachment);
					}
				}
			}

				//#### .Send the above created oMessage
			oSMTPClient.Send(oMessage);
		}

	} //# class Mail

} //# namespace Cn.Net
