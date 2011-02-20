--##################################################
--# Internationalization: cnInternationalization
--##################################################
CREATE TABLE [dbo].[cnInternationalization](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PicklistID] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[Data] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL,
	[Description] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL,
	[IsActive] [bit] NULL CONSTRAINT [DF_cnInternationalization_IsActive]  DEFAULT ((0)),
 CONSTRAINT [PK_cnInternationalization] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','0','_PicklistMetaData','Describes the picklists housed within this table. The "DisplayOrder" column is logicially inner-joined to the "PicklistID" column.');
INSERT INTO cnInternationalization (picklistid,displayorder,data) VALUES ('0','1','GeneralSettings');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','2','LanguageCodes','ISO 639: 2-letter codes');
INSERT INTO cnInternationalization (picklistid,displayorder,data) VALUES ('0','100','enLocalization');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','101','enBoolean','Defines the default boolean value description picklist.');
INSERT INTO cnInternationalization (picklistid,displayorder,data) VALUES ('0','110','enDate_MonthNames');
INSERT INTO cnInternationalization (picklistid,displayorder,data) VALUES ('0','111','enDate_AbbreviatedMonthNames');
INSERT INTO cnInternationalization (picklistid,displayorder,data) VALUES ('0','112','enDate_MonthDaySuffix');
INSERT INTO cnInternationalization (picklistid,displayorder,data) VALUES ('0','113','enDate_WeekDayNames');
INSERT INTO cnInternationalization (picklistid,displayorder,data) VALUES ('0','114','enDate_AbbreviatedWeekDayNames');
INSERT INTO cnInternationalization (picklistid,displayorder,data) VALUES ('0','115','enDate_Meridiem');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','116','enDate_CalendarWeekDayNames','Weekday names displayed in the DHTML calendar''s headers. It''s a good idea to keep these entries to 1 or 2 characters, else the day columns may be of differing widths.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','120','enRendererSearchForm_Boolean','Defines the comparison drop-down values for RendererSearchForm criteria against a boolean column.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','121','enRendererSearchForm_Numeric','Defines the comparison drop-down values for RendererSearchForm criteria against a numeric column.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','122','enRendererSearchForm_DateTime','Defines the comparison drop-down values for RendererSearchForm criteria against a date/time column.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','123','enRendererSearchForm_Char','Defines the comparison drop-down values for RendererSearchForm criteria against a character column.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','124','enRendererSearchForm_LongChar','Defines the comparison drop-down values for RendererSearchForm criteria against a "long character" column. A "long character" column is one that cannot utilize the "equals" compairson operator in a Where Clause.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','125','enRendererSearchForm_SingleValuePicklist','Defines the comparison drop-down values for RendererSearchForm criteria against a column described as a "single-value picklist".');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','126','enRendererSearchForm_MultiValuePicklist','Defines the comparison drop-down values for RendererSearchForm criteria against a column described as a "multi-value picklist".');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','127','enRendererSearchForm_MultiValueSearchInSingleValue','Defines the comparison drop-down values for RendererSearchForm criteria against a column described as a "multi-value search in a single value".');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','128','enRendererSearchForm_SingleValueSearchInMultiValue','Defines the comparison drop-down values for RendererSearchForm criteria against a column described as a "single-value search in a common delimited multi-value".');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','129','enRendererSearchForm_IsNullStringIsNull','Defines the is null-string/is null conditionally included drop-down value for RendererSearchForm criteria against all column types.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','130','enDateMath_Frequency','Holiday date frequencies utilized by DateMath to determine all the holidays for a given year.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','131','enDateMath_DefinitionType','Holiday date definition types utilized by DateMath to determine all the holidays for a given year.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','132','enDateMath_Calculations','Internal DateMath calculation constants utilized by "Calculated" DefinitionType''s to determine nondeterministic holidays. NOTE: All additionally overloaded, developer-defined calculations must utilize a ''Data'' value >= 1000.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','140','enPicklistManagement','Defines the strings seen by the user within the Management.Picklist class.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','10100','enEndUserMessages','Defines the default datatype-related error messages displayed by the system.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('0','10101','enDeveloperMessages','Defines the developer-centric error messages. These are errors that are "thrown" or "raised" by Renderer.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('1','0','DefaultLanguageCode','en');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('2','0','en','English (American)');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('100','1','CurrencySymbol','$');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('100','2','CurrencyMask_Positive','$ #,##0.00');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('100','3','CurrencyMask_Negetive','$ -#,##0.00');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('100','4','CurrencyMask_Zero','$ 0.00');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('100','150','Date_DateTimeFormat','$DD $MMMM $YYYY $hh:$mm:$ss');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('100','151','Date_TimeFormat','$hh:$mm:$ss');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('100','152','Date_DateFormat','$DD $MMMM $YYYY');
INSERT INTO cnInternationalization (picklistid,displayorder,data) VALUES ('100','153','Date_WeekOfYearCalculationEnum');
INSERT INTO cnInternationalization (picklistid,displayorder) VALUES ('101','-1');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('101','0','1','Yes');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('101','1','0','No');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','0','1','January');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','1','2','February');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','2','3','March');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','3','4','April');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','4','5','May');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','5','6','June');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','6','7','July');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','7','8','August');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','8','9','September');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','9','10','October');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','10','11','November');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('110','11','12','December');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','0','1','Jan');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','1','2','Feb');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','2','3','Mar');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','3','4','Apr');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','4','5','May');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','5','6','Jun');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','6','7','Jul');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','7','8','Aug');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','8','9','Sep');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','9','10','Oct');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','10','11','Nov');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('111','11','12','Dec');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','0','1','st');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','1','2','nd');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','2','3','rd');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','3','4','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','4','5','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','5','6','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','6','7','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','7','8','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','8','9','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','9','10','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','10','11','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','11','12','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','12','13','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','13','14','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','14','15','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','15','16','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','16','17','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','17','18','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','18','19','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','19','20','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','20','21','st');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','21','22','nd');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','22','23','rd');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','23','24','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','24','25','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','25','26','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','26','27','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','27','28','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','28','29','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','29','30','th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('112','30','31','st');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('113','0','1','Sunday');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('113','1','2','Monday');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('113','2','3','Tuesday');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('113','3','4','Wednesday');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('113','4','5','Thursday');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('113','5','6','Friday');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('113','6','7','Saturday');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('114','0','1','Sun');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('114','1','2','Mon');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('114','2','3','Tue');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('114','3','4','Wed');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('114','4','5','Thu');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('114','5','6','Fri');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('114','6','7','Sat');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('115','0','am','am');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('115','1','pm','pm');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('116','0','1','Su');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('116','1','2','Mo');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('116','2','3','Tu');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('116','3','4','We');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('116','4','5','Th');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('116','5','6','Fr');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('116','6','7','Sa');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('120','0','100','And');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('120','1','101','Or');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('120','2','102','And Not');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('121','0','102','Equal To');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('121','1','103','Not Equal To');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('121','2','101','Less Then');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('121','3','100','Greater Then');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('122','0','102','On');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('122','1','103','Not On');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('122','2','101','Before');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('122','3','100','After');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('123','0','204','Equal To');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('123','1','206','Not Equal To');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('123','2','200','Begins With');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('123','3','201','Ends With');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('123','4','202','Contains');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('123','5','203','Does Not Contain');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('123','6','205','Wildcard Search');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('124','0','200','Begins With');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('124','1','201','Ends With');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('124','2','202','Contains');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('124','3','203','Does Not Contain');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('124','4','205','Wildcard Search');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('125','0','102','Is');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('125','1','103','Is Not');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('126','0','301','Contains Any');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('126','1','300','Contains All');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('126','2','302','Contains None');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('127','0','301','Contains Any');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('127','1','302','Contains None');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('128','0','102','Contains');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('128','1','103','Does Not Contain');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('129','0','Boolean','Is Unknown');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('129','1','Numeric','Is Unknown');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('129','2','DateTime','Is Not Set');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('129','3','Char','Is Blank');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('129','4','LongChar','Is Blank');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('129','5','SingleValuePicklist','Is Blank');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('129','6','MultiValuePicklist','Is Blank');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('129','7','MultiValueSearchInSingleValuePicklist','Is Blank');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('129','8','SingleValueSearchInMultiValuePicklist','Is Blank');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('130','0','0','Weekly');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('130','1','1','Fortnightly');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('130','2','2','Monthly');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('130','3','3','Quarterly');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('130','4','4','Tri-Annually');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('130','5','5','Semi-Annually');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('130','6','6','Annually');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('131','0','0','Static');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('131','1','1','Nth Week Day In Month');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('131','2','2','Calculated');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('132','0','0','Good Friday');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('132','1','1','Easter');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('132','2','2','Easter Monday');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('132','1000','1000','Christmas');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('132','1001','1001','BoxingDay');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('132','1002','1002','NewYearsDay');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('132','1003','1003','AustraliaDay');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('132','1004','1004','AddlXMas');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('132','1005','1005','QBDayWA');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('132','1006','1006','ANZACDay');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','0','UniqueIDRequired','You must define a unique Picklist ID for the new picklist. ''$sExtraInfo1'' is already in use.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','1','UniqueNameRequired','You must define a unique Picklist Name for the new picklist. ''$sExtraInfo1'' is already in use.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','2','ViewPicklist','View Picklist');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','3','PicklistWasUpdated','Picklist Was Updated!');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','4','UpdatePicklist','Update Picklist');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','5','Type','Type');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','6','Description','Description');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','7','Name','Name');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','8','PicklistNameIsCurrentlyEmpty','''$sExtraInfo1'' is currently empty.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','9','DisplayOrder','Display Order');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','10','Delete','Delete');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','11','DisplayedValue','Displayed Value');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','12','StoredValue','Stored Value');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','13','PicklistDescription','Picklist Description');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','14','PicklistName','Picklist Name');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','15','PicklistID','Picklist ID');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','16','New','New');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','17','ViewInternationalization','View Internationalization');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','18','TableColumnName','Table.Column Name');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','19','TableColumnNameFormat','You must define a Table name and a Column name seperated by a period (i.e. "TableName.ColumnName").');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','20','TableColumnNamePicklist','You must define an existing Picklist name.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('140','21','IsActive','Active');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1000','Alert','Unable to submit the form!\n\nPlease review the highlighted form fields and try again.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1001','NoError','This value is valid. No validation errors occured.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1002','IncorrectLength','This value is too long.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1003','ValueIsRequired','A value is required for this input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1004','Custom','A custom data validation error occured for this value.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1005','UnknownOrUnsupportedType','DEVELOPER: This column type is unknown or is unsupported at this time.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1006','IncorrectDataType_Boolean','This must be a recognized boolean value (i.e. - ''true'', ''false'', ''yes'', ''no'' or a number).');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1007','IncorrectDataType_Integer','This must be an integer value (i.e. - a whole number).');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1008','IncorrectDataType_Float','This value must be a numeric value.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1009','IncorrectDataType_Currency','This value must be a numeric value.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1010','IncorrectDataType_DateTime','This must be a valid date/time.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1011','IncorrectDataType_GUID','This must be a valid GUID.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1012','IncorrectDataType_Other','This value must be a string value.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1013','IncorrectDataType_NotWithinPicklist','This value is not from within the provided list.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1014','UnknownErrorCode','An unknown data validation error occured for this input (error code ''$iErrorCode'').');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1015','MissingInput','DEVELOPER: Required input fields missing from the form! The following required input field is missing from the form: ');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1016','MultipleValuesSubmittedForSingleValue','DEVELOPER: Multiple values submitted for an input defined as a single value input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1017','ComboBox_OrSelect','Or Select');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1018','DateTime_PreviousYear','Previous Year');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1019','DateTime_PreviousMonth','Previous Month');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1020','DateTime_Today','Today');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1021','DateTime_NextMonth','Next Month');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1022','DateTime_NextYear','Next Year');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1023','DateTime_Close','Close');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1024','DateTime_Now','Now');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1025','DateTime_AM','am');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1026','DateTime_PM','pm');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1027','DateTime_Delimiter',':');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10100','1028','DateTime_Clear','Clear');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1100','General_UnknownError','An unknown error occured (don''t you just hate this error message)!');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1101','General_DeveloperDefined','$sExtraInfo1');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1102','General_RendererIsOpen','The parent Renderer class is currently in use by another ''RendererList'', ''RendererForm'', or ''RendererReport'' object. This action cannot be taken while a render is occurring.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1103','General_MaliciousSQLFound','Malicious SQL code has been detected within the passed ''$sExtraInfo1''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1104','General_MissingRequiredColumns','The passed ''$sExtraInfo1'' does not contain the required columns. Please use ''$sExtraInfo2'' to retrieve the data in the required columns.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1105','General_InvalidColumnCount','The passed ''$sExtraInfo1'' does not contain the correct number of columns ($sExtraInfo2).');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1106','General_NoDataToLoad','The passed ''$sExtraInfo1'' contains no data to load.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1107','General_DataNotLoaded','No $sExtraInfo1 data to query. Please load $sExtraInfo1 data via ''Load'' and try again.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1108','General_PositiveIntegerRequired','The ''$sExtraInfo1'' must be an integer greater then or equal to $sExtraInfo2.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1109','General_UnknownValue','The passed ''$sExtraInfo1'' value ''$sExtraInfo2'' is invalid or unreconized!');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1110','General_IndexOutOfRange','The passed ''$sExtraInfo1'' referes to an index that is outside of the acceptable range ($sExtraInfo2).');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1111','General_ValueRequired','A value is required for the passed ''$sExtraInfo1''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1200','FormRenderer_UnknownInputAlias','There is no input alias definition for the alias ''$sExtraInfo1''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1201','FormRenderer_DuplicateInputAlias','An alias by the name of ''$sExtraInfo1'' has already been defined. Please specify another alias for ''$sExtraInfo2''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1202','FormRenderer_InsertNullSaveType','You cannot define ''$sExtraInfo1'' with both a ''SaveType'' of ''InsertNull'' and ''IsNullable'' of ''false''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1203','FormRenderer_UnknownPicklistName','The ''PicklistName'' value ''$sExtraInfo1'' defined within the passed ''h_oAdditionalData'' does not exist.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1204','FormRenderer_PicklistNameNotDefined','A ''PicklistName'' value must be defined for ''$sExtraInfo1'' within the passed ''h_oAdditionalData'' if ''eExtendedDataType'' is defined as a ''PicklistType''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1205','FormRenderer_CallMultiRenderInput','You must call the multiple ''a_sDefaultValues'' version of ''RenderInput'' for the ''MultiSelectInput'' or ''CheckboxesInput'' input types.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1206','FormRenderer_CallSingleRenderInput','You must call the single ''sDefaultValue'' version of ''RenderInput'' for input types other then ''MultiSelectInput'' or ''CheckboxesInput''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1207','FormRenderer_UnreconizedFormInputType_Hidden','** Not Currently Used **');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1208','FormRenderer_UnreconizedFormInputType_Picklist','Invalid or unsupported ''eFormInputType'' for a picklist input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1209','FormRenderer_UnreconizedFormInputType_Boolean','Invalid or unsupported ''eFormInputType'' for a boolean input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1210','FormRenderer_UnreconizedFormInputType_Integer','Invalid or unsupported ''eFormInputType'' for an integer input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1211','FormRenderer_UnreconizedFormInputType_Float','Invalid or unsupported ''eFormInputType'' for a float input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1212','FormRenderer_UnreconizedFormInputType_Currency','Invalid or unsupported ''eFormInputType'' for a currency input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1213','FormRenderer_UnreconizedFormInputType_DateTime','Invalid or unsupported ''eFormInputType'' for a date/time input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1214','FormRenderer_UnreconizedFormInputType_Binary','Invalid or unsupported ''eFormInputType'' for a binary-based input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1215','FormRenderer_UnreconizedFormInputType_GUID','Invalid or unsupported ''eFormInputType'' for a GUID input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1216','FormRenderer_UnreconizedFormInputType_Other','Invalid or unsupported ''eFormInputType'' for a string-based input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1217','FormRenderer_UnsupportedDataType','The ''DataType'' of ''$sExtraInfo1'' is not currently supported.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1218','FormRenderer_SQLNotGenerated','No SQL statements were auto-generated! You will need to generate your own SQL statements.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1219','FormRenderer_UpdateMissingID','A valid SQL update statement cannot be auto-generated for ''$sExtraInfo1'' because no ID column was defined.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1220','FormRenderer_PicklistIsEmpty','**UPDATE** You must pass in a valid picklist within the ''a_sPicklist'' argument.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1221','FormRenderer_PicklistNotDefined','You must set the Form.Picklist with your system''s picklist.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1222','FormRenderer_Picklist_IsAdHoc','The input alias ''$sExtraInfo1'' was defined as a external picklist. You cannot utilize ''$sExtraInfo2'' for a picklist defined as being external .');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1223','FormRenderer_InputAliasWithPrimaryDelimiter','The input alias ''$sExtraInfo1'' contains the PrimaryDelimiter ''$sExtraInfo2''. Input alias'' that are a part of auto-generated SQL statements cannot include the PrimaryDelimiter.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1300','ReportRenderer_InvalidBodyReturn','The object returned by RendererReport.Body is not of a reconized type. Only a ''RendererList'' or ''RendererForm'' object may be returned by RendererReport.Body.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1400','InputTools_NoInputAliases','No ''InputAliases'' to have been defined. You must define at least one input via ''DefineInput'' before you can attempt to collect the ''InputAliases''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1500','RendererSearchForm_MaxResults','You must specify a positive number for the ''MaxResults''. If you want to allow an unlimited number of results, set ''MaxResults'' to ''0''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1501','RendererSearchForm_UniqueUserID','You must specify a unique ''UserID'' before you are allowed to insert data into the search tables.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1502','RendererSearchForm_MissingIDColumn','You must specify an ''IDColumn'' before you are allowed to insert data into the search tables.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1503','RendererSearchForm_NullCookieMonster','A ''CookieMonster'' object reference must be set before calling ''$sExtraInfo1''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1504','RendererSearchForm_GetSQL_SingleArgument','Inputs must be defined and values must be set for the following properties before calling the single argument version of GetSQL: UserID, IDColumn, SearchesTable, SearchResultsTable');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1505','RendererSearchForm_GetSQL_NullIDColumnOrTables','The passed ''a_sSearchesRecord'' contains a null-string for the ''IDColumn'' and/or ''Tables'' columns. Both the ''IDColumn'' and ''Tables'' columns must contain string data (i.e. - not null and not '''').');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1506','RendererSearchForm_GetSQL_NotASingleRecord','The passed ''a_sSearchesRecord'' must contain a single data record.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1507','RendererSearchForm_InvalidCompairsonType','The ''eCompairsonType'' specified for ''$sExtraInfo1'' is invalid for its ''DataType''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1508','RendererSearchForm_UnknownCustomInputAlias','There is no custom input alias definition for the alias ''$sExtraInfo1''. NOTE: ''$sExtraInfo1'' must be defined as a *CUSTOM* input alias.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1509','RendererSearchForm_InsertCustomInputClauseMissingToken','The custom input alias identifier for the input alias ''$sExtraInfo1'' is missing from the passed sSQL.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1510','RendererSearchForm_InsertCustomInputClauseSecondCall','The custom input alias identifier for the input alias ''$sExtraInfo1'' has already been replaced.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1511','RendererSearchForm_DuplicateInputOrderAlias','You have already defined ''$sExtraInfo1'' within the input alias ordering. Each searchable input alias can only be represented only once within the ordering.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1512','RendererSearchForm_InputAliasMissingFromOrdering','The input alias ''$sExtraInfo1'' was not found within the input alias ordering. Each searchable input alias must be represented once within the ordering.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1513','RendererSearchForm_InputOrderingIncomplete','The input alias ordering is incomplete. Each searchable input alias must be represented once within the ordering.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1514','RendererSearchForm_NoSearchableInputs','You must define at least one searchable input if you choose not to overload the ValidateRecord function.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1515','RendererSearchForm_NoOrderedInputAliases','You must define the input alias ordering if you choose not to overload the ValidateRecord function. Each searchable input alias must be represented once within the ordering.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1516','RendererSearchForm_UnknownNonCustomInputAlias','There is no input alias definition for the alias ''$sExtraInfo1''. NOTE: ''$sExtraInfo1'' must be defined as a *NON-CUSTOM* input alias.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1517','RendererSearchForm_DateTimeFormatMissing','You must pass in a hash array specifying at least a ''DateTime_Format'' for ''$sExtraInfo1''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1600','Renderer_InvalidDecoderTruncateString','**UPDATE** You must pass in an array with two or three elements.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1601','Renderer_USSHashMissingFormName','You must pass in a hash array specifying at least a ''FormName'' (reconized keys: ''UserSelectedStack'', ''FormName'', ''TableName'', and ''IDColumn'').');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1602','Renderer_RenderedJavaScript','Some JavaScript files have been previously rendered, therefore cannot render all JavaScript files.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1700','UserSelectedStack_NotInitialized','You must call ''Initialize'' with a valid ''rPageResults'' before you are allowed to render a User Selected Stack input.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1801','DateMath_UnreconizedCalculateDayFunction','A ''Calculated'' date must have a reconized ''CalculateDayFunction''. The ''CalculateDayFunction'' value ''$sExtraInfo1'' is unreconized.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1802','DateMath_InvalidHolidayMonth','A(n) ''$sExtraInfo1'' date must have a valid value set for ''HolidayMonth''. The ''HolidayMonth'' value ''$sExtraInfo2'' is invalid.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1803','DateMath_InvalidMonthDay','A(n) ''$sExtraInfo1'' date must have a valid value set for ''MonthDay''. The ''MonthDay'' value ''$sExtraInfo2'' is invalid.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1804','DateMath_InvalidWeekInMonth','A(n) ''$sExtraInfo1'' date must have a numeric, non-zero value set for ''WeekInMonth''. The ''WeekInMonth'' value ''$sExtraInfo2'' is invalid.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1805','DateMath_InvalidWeekDay','A(n) ''$sExtraInfo1'' date must have a valid value set for ''WeekDay''. The ''WeekDay'' value ''$sExtraInfo2'' is invalid.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1806','DateMath_InvalidCalculatedFrequency','A ''Calculated'' date''s ''Frequency'' must be set to ''Annually'', the current ''Frequency'' value ''$sExtraInfo1'' is invalid.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1807','DateMath_CheckConfigurationFile','One or more of the required DateMath-related settings is missing - Date_MonthNames, Date_AbbreviatedMonthNames, Date_WeekDayNames, Date_AbbreviatedWeekDayNames, Date_EnglishNumberSuffix, Date_Meridian');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1900','CookieMonster_InvalidKeysValues','The passed ''sKeysValues'' ''$sExtraInfo1'' is malformed. Encountered a key/value definition without an ''=''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1901','CookieMonster_InvalidKeysValuesDuplicateKey','The passed ''sKeysValues'' ''$sExtraInfo1'' is malformed. Encountered a duplicate key definition for the key ''$sExtraInfo2''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1902','CookieMonster_KeyValueArrayBounds','The passed ''a_sKeysValues'' must be a multidimensional array with one or more elements in it''s first dimension, and exactly two elements in it''s second dimension.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1903','CookieMonster_InvalidKeyName','The passed ''sKey'' ''$sExtraInfo1'' is an invalid key name.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','1904','CookieMonster_ValueTooLong','There is too much data stored within the CookieMonster, cannot proceed without risking an HTTP request error. The CookieMonster contains ''$sExtraInfo1'' characters of data, which is above the max of ''$sExtraInfo2'' characters.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2001','DbMetaData_InvalidTable','The passed ''a_sTable'' does not contain any table information to query.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2002','DbMetaData_InvalidIsNullable','The passed ''$sExtraInfo1'' contains one or more rows with a non-boolean value for ''Is_Nullable''. The ''Is_Nullable'' column must contain a boolean value (i.e. - ''True'', ''False'', ''Yes'', ''No'' or a numeric value).');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2003','DbMetaData_BlankTableOrColumnName','The passed ''$sExtraInfo1'' contains one or more rows with a blank value for ''Table_Name'' and/or ''Column_Name''. The ''Table_Name'' and ''Column_Name'' columns must contain a value (i.e. - not null and not '''').');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2004','DbMetaData_InvalidDataType','The passed ''$sExtraInfo1'' contains one or more rows with a blank value for ''Data_Type''. The ''Data_Type'' column must contain a value (i.e. - not null and not '''').');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2005','DbMetaData_InvalidTableName','No metadata information could be found for a table named ''$sExtraInfo1''. Please ensure that all of the database table definitions you require have been properly loaded into ''DataSource.MetaData''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2006','DbMetaData_InvalidTableColumnName','No metadata information could be found for a column named ''$sExtraInfo1''. Please ensure that all of the database table definitions you require have been properly loaded into ''DataSource.MetaData''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2007','DbMetaData_InvalidMinMamimumNumericValue','The passed ''$sExtraInfo1'' contains one or more rows with a non-blank/non-numeric value for ''MinimumNumericValue'' and/or ''MamimumNumericValue''. The  ''MinimumNumericValue'' and ''MamimumNumericValue'' columns must contain either a blank (null-string) or a numeric value.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2008','DbMetaData_MissingMinMamimumNumericValue','The passed ''$sExtraInfo1'' contains one or more rows with a non-matching value pair for ''MinimumNumericValue'' and ''MamimumNumericValue''. If you choose to define your own ''MinimumNumericValue'' / ''MamimumNumericValue'' pair, you must define both values.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2009','DbMetaData_InvalidParentChildRelationship','The defined ''$sExtraInfo1'' or one of its decendants contain a refence back to this instance. This would result in an infinate loop and is therefore not allowed. Please review your ''$sExtraInfo1'' definitions.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2100','Picklists_InvalidPicklistID','The passed ''a_sPicklistData'' contains one or more rows with a non-numeric value for ''PicklistID''. The ''PicklistID'' column must contain numeric data (i.e. - not null).');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2101','Picklists_DataDescriptionDimensions','You must pass valid arrays containing the same number of elements (i.e. - their bounds must match).');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2200','MultiArray_DuplicateColumnName','A column name by the name of ''$sExtraInfo1'' has already been defined. Please specify another column name for ''$sExtraInfo2''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2201','MultiArray_ColumnNameNotFound','There is no column name definition for the column ''$sExtraInfo1''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2202','MultiArray_NoRows','The MultiArray is currently empty.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2203','MultiArray_LastColumn','The MultiArray must have at least one column defined. You cannot remove the last column from a MultiArray.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2204','MultiArray_IDColumnRequired','You must define a present IDColumn within the passed ''$sExtraInfo1''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2300','DbTools_WhereClauseRequired','You must define at least one non-insertable/updateable column within the passed ''$sExtraInfo1''.');
INSERT INTO cnInternationalization (picklistid,displayorder,data,description) VALUES ('10101','2301','DbTools_InsertUpdateColumnsRequired','You must define at least one insertable/updateable column within the passed ''$sExtraInfo1''.')


--##################################################
--# Picklists: cnPicklists
--##################################################
CREATE TABLE [dbo].[cnPicklists](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PicklistID] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[Data] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL,
	[Description] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL,
	[IsActive] [bit] NULL CONSTRAINT [DF_cnPicklists_IsActive]  DEFAULT ((0)),
 CONSTRAINT [PK_cnPicklists] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO cnPicklists (picklistid,displayorder,data,isactive) VALUES ('0','0','_PicklistMetaData','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('0','1','Titles','Person Formal Titles (Mr, Mrs, etc.)','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('0','2','YesNo','True/False to Yes/No decoding','True');
INSERT INTO cnPicklists (picklistid,displayorder,data) VALUES ('0','5','_PicklistColumnAssociations');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('0','10','Formats','CD, CD3, 12", etc.');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('0','11','MediaTypes','Media Types (i.e. - Official, Bootleg, etc.)');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('0','12','Countries','US, UK, Australia, Japan, etc.');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('0','13','Labels','Grand Royal, Capitol, Def Jam, Ratcage, etc.');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('0','14','FormatDescriptions','Red Vinyl, Blue Vinyl, etc');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('1','0','0','Mr','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('1','1','1','Mrs','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('1','2','2','Prof','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('1','3','3','Grand Imperial','False');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('2','0','True','Yes','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('2','1','False','No','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('5','0','PeopleDemo.Title','Titles','True');
INSERT INTO cnPicklists (picklistid,displayorder,isactive) VALUES ('10','-1','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('10','0','0','CD');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('10','1','1','CD3 (3 inch single)');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('10','2','2','12"');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('10','3','3','10"');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('10','4','4','7"');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('10','5','5','Cassette');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('10','6','6','MiniDisk');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('10','7','7','SACD');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('10','8','8','HDCD');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('10','9','9','DAT');
INSERT INTO cnPicklists (picklistid,displayorder,isactive) VALUES ('11','-1','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('11','0','0','Official Release','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('11','1','1','Official Promo Release','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('11','3','3','Unofficial Promo Release','True');
INSERT INTO cnPicklists (picklistid,displayorder,data,description,isactive) VALUES ('11','4','4','Bootleg','True');
INSERT INTO cnPicklists (picklistid,displayorder) VALUES ('12','-1');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('12','0','0','US');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('12','1','1','Australia');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('12','2','2','Japan');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('12','3','3','UK');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('12','4','4','Holland');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('12','5','5','Germany');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('12','6','6','Mexico');
INSERT INTO cnPicklists (picklistid,displayorder) VALUES ('13','-1');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('13','0','0','Def Jam');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('13','1','1','Grand Royal');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('13','2','2','Capitol');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('13','3','3','n/a');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('13','4','4','Ratcage');
INSERT INTO cnPicklists (picklistid,displayorder) VALUES ('14','-1');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','0','0','Red Vinyl');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','1','1','Red Vinyl (Clear)');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','2','2','Blue Vinyl');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','3','3','Blue Vinyl (Clear)');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','4','4','White Vinyl');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','5','5','Clear Vinyl');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','6','6','Etched Vinyl');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','7','7','Shaped Vinyl');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','8','8','Shaped CD');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','9','9','Blue Vinyl (Marbled)');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','10','10','Shaped Vinyl (Uncut)');
INSERT INTO cnPicklists (picklistid,displayorder,data,description) VALUES ('14','11','11','Shaped CD (Uncut)')


--##################################################
--# HolidayCalculations: cnHolidayCalculations
--##################################################
CREATE TABLE [dbo].[cnHolidayCalculations](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[DefinitionType] [int] NOT NULL,
	[Frequency] [int] NOT NULL,
	[Country] [nvarchar](2) COLLATE Latin1_General_CI_AS NULL,
	[Region] [nvarchar](10) COLLATE Latin1_General_CI_AS NULL,
	[HolidayMonth] [int] NULL,
	[MonthDay] [int] NULL,
	[WeekDay] [int] NULL,
	[WeekInMonth] [int] NULL,
	[EffectiveYear] [int] NULL,
	[CalculatedDayFunction] [int] NULL,
 CONSTRAINT [PK_cnHolidayCalculations] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,holidaymonth,monthday,calculateddayfunction) VALUES ('Christmas','2','6','12','25','1000');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,holidaymonth,monthday,calculateddayfunction) VALUES ('Boxing Day','2','6','12','26','1001');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,holidaymonth,monthday,calculateddayfunction) VALUES ('New Years Day','2','6','1','1','1002');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Melbourne Cup','1','6','VIC','11','3','1');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,holidaymonth,monthday,calculateddayfunction) VALUES ('Australia Day','2','6','1','26','1003');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,holidaymonth,calculateddayfunction) VALUES ('Addl XMas Day','2','6','12','1004');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,calculateddayfunction) VALUES ('Easter Monday','2','6','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,calculateddayfunction) VALUES ('Good Friday','2','6','0');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,holidaymonth,monthday) VALUES ('New Year''s Day','0','6','1','1');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Queen''s Birthday','1','6','ACT','6','2','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Queen''s Birthday','1','6','NSW','6','2','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Queen''s Birthday','1','6','VIC','6','2','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Queen''s Birthday','1','6','TAS','6','2','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Queen''s Birthday','1','6','NT','6','2','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Queen''s Birthday','1','6','SA','6','2','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Queen''s Birthday','1','6','QLD','6','2','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,weekday,calculateddayfunction) VALUES ('Queen''s Birthday','2','6','WA','2','1005');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Canberra Day','1','6','ACT','3','2','3');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Royal Hobart Regatta','1','6','TAS','2','2','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Eight Hours Day','1','6','TAS','3','2','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Labour Day','1','6','VIC','3','2','2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Labour Day','1','6','WA','3','2','1');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Labour Day','1','6','QLD','5','2','1');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('May Day','1','6','NT','5','2','1');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Adelaide Cup Carnival and Volunteers Day','1','6','SA','5','2','3');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Foundation Day','1','6','WA','6','2','1');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Darwin Show Day','1','6','NT','7','6','4');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Picnic Day','1','6','NT','8','2','1');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Royal National Show','1','6','QLD','8','4','-3');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Labour Day','1','6','ACT','10','2','1');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Labour Day','1','6','NSW','10','2','1');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Labour Day','1','6','SA','10','2','1');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,weekday,weekinmonth) VALUES ('Royal Hobart Show','1','6','TAS','10','5','-2');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,monthday,calculateddayfunction) VALUES ('ANZAC Day','2','6','ACT','4','25','1006');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,monthday,calculateddayfunction) VALUES ('ANZAC Day','2','6','NSW','4','25','1006');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,monthday,calculateddayfunction) VALUES ('ANZAC Day','2','6','WA','4','25','1006');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,monthday,calculateddayfunction) VALUES ('ANZAC Day','2','6','NT','4','25','1006');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,monthday,calculateddayfunction) VALUES ('ANZAC Day','2','6','QLD','4','25','1006');
INSERT INTO cnHolidayCalculations (description,definitiontype,frequency,region,holidaymonth,monthday,calculateddayfunction) VALUES ('ANZAC Day','2','6','SA','4','25','1006')
