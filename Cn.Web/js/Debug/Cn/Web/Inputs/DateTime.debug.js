/*
Copyright © 2004-2010, Nicholas Campbell
All Rights Reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Nicholas Campbell nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined' || ! Cn.Inputs || ! Cn.Inputs.DateTime) {
	alert("Cn.Inputs.DateTime.Text: [DEVELOPER] 'Cn/Inputs/DateTime.js' must be included before referencing this code.");
}
//# </DebugCode>


//########################################################################################################################
//# Supplemental Errors class - Errors.Messages
//# 
//#     NOTE: The "WeekOfYearCalculation", "SystemName" and "LanguageCode" are included on the querystring to this script.
//#     Always Included By: Cn/Renderer/Form/DateTime.js
//########################################################################################################################
//# Last Code Review: Sepetember 7, 2007
Cn.Inputs.DateTime.Text = new function() {
	this.PreviousYear = 'Previous Year';
	this.PreviousMonth = 'Previous Month';
	this.Today = 'Today';
	this.NextMonth = 'Next Month';
	this.NextYear = 'Next Year';
	this.Close = 'Close';
	this.Clear = 'Clear';
	this.Now = 'Now';
	this.AM = 'am';
	this.PM = 'pm';
	this.Delimiter = ':';

	this.DaySuffixes = ['st','nd','rd','th','th','th','th','th','th','th','th','th','th','th','th','th','th','th','th','th','st','nd','rd','th','th','th','th','th','th','th','st'];
	this.aDays = ['Sun','Mon','Tue','Wed','Thu','Fri','Sat'];
	this.cDays = ['Su','Mo','Tu','We','Th','Fr','Sa'];
	this.Days = ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'];
	this.aMonths = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
	this.Months = ['January','Feburary','March','April','May','June','July','August','September','October','November','December'];

}; //# Cn.Inputs.DateTime.Text

	//#### Set the .WeekOfYearCalculation in .DateTime
	//####     NOTE: This is unnecessary as .WeekOfYearCalculation is init'd to .cnDefault by Cn._.wid
//Cn._.wid.WeekOfYearCalculation = Cn._.wid.enumWeekOfYearCalculations.cnDefault;
