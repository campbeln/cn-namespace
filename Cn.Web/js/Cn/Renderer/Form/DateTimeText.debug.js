//# © 2004-2006 Nick Campbell, All Rights Reserved. This code is hereby licensed under the terms of the LGPL (http://www.gnu.org/copyleft/lesser.html).

//# <DebugCode>
	//#### If all of the required Cn objects do not exist, popup the related error message
if (typeof(Cn) == 'undefined' || ! Cn.Renderer || ! Cn.Renderer.Form || ! Cn.Renderer.Form.DateTime) {
	//alert("Cn.Renderer.Form.DateTime.Text: [DEVELOPER] 'Cn/Renderer/Form/DateTime.js' must be included before referencing this code.");
}
//# </DebugCode>


//########################################################################################################################
//# Supplemental Errors class - Errors.Messages
//# 
//#     NOTE: The "WeekOfYearCalculation", "SystemName" and "LanguageCode" are included on the querystring to this script.
//#     Always Included By: Cn/Renderer/Form/DateTime.js
//########################################################################################################################
//# Last Code Review: April 18, 2006
Cn.Renderer.Form.DateTime.Text = function() {
	return {
		PreviousYear:'Previous Year',
		PreviousMonth:'Previous Month',
		Today:'Today',
		NextMonth:'Next Month',
		NextYear:'Next Year',
		Close:'Close',
		Clear:'Clear',
		Now:'Now',
		AM:'am',
		PM:'pm',
		Delimiter:':',

		DaySuffixes:['st','nd','rd','th','th','th','th','th','th','th','th','th','th','th','th','th','th','th','th','th','st','nd','rd','th','th','th','th','th','th','th','st'],
		aDays:['Sun','Mon','Tue','Wed','Thu','Fri','Sat'],
		cDays:['Su','Mo','Tu','We','Th','Fr','Sa'],
		Days:['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'],
		aMonths:['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'],
		Months:['January','Feburary','March','April','May','June','July','August','September','October','November','December']
	};

} (); //# Cn.Renderer.Form.DateTime.Text


	//#### Now that the class is defined, .Initilize ourselves
Cn._.dt.Initilize();
