using Extension.Values;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Aibe {
  public class DHObscured { //remove the "Obscured" part to make the project works
    //User related global items
    public const string DevFullName = "Ian Kamajaya";
    public const string DevDisplayName = "developer";
    public const string DevName = "ian";
    public const string DevEmail = "notrealemailaddress@fakedomain.com"; //change this with your actual email address
    public const string DevPass = "fakepassword"; //change this with your actual password
    public const string DevRole = "Developer";
    public const string MainAdminFullName = "Administrator";
    public const string MainAdminDisplayName = "admin";
    public const string MainAdminRole = "Main Administrator";
    public const string AdminRole = "Administrator";
    public const string UserRole = "User"; //this is the very basic of the user role, named user role
    public const string AnonymousRole = "Anonymous";
    public const string MobileAppRole = "MobileApp";
    public readonly static List<string> AdminRoles = new List<string> { DevRole, MainAdminRole, AdminRole };
    public readonly static List<string> MainAdminRoles = new List<string> { DevRole, MainAdminRole };
    public readonly static List<string> AllowedAdminRoles = new List<string> { AdminRole };
    public readonly static List<string> SpecialRoles = new List<string> { AnonymousRole, MobileAppRole };

    //Foldering
    public const string DefaultDownloadFolderName = "Downloads";
    public const string DefaultImageFolderName = "Images";
    public const string DefaultAttachmentFolderName = "Attachments";
    public const string DefaultSettingFolderName = "Settings";

    //Use to help link and actions
    public const string CreateActionName = "Create";
    public const string EditActionName = "Edit";
    public const string DeleteActionName = "Delete";
    public const string DetailsActionName = "Details";
    public const string DownloadAttachmentsActionName = "DownloadAttachments";
    public const string IndexActionName = "Index";
    public const string CreateGroupActionName = "CreateGroup";
    public const string EditGroupActionName = "EditGroup";
    public const string DeleteGroupActionName = "DeleteGroup";
    public const string GroupDetailsActionName = "GroupDetails";
    public const string AuthenticateActionName = "Authenticate";
    public const string CryptoSerializeAction = "CryptoSerialize";
    public const string CryptoSerializeAllAction = "CryptoSerializeAll";
    public const string DecryptoSerializeAction = "DecryptoSerialize";
    public const string DecryptoSerializeAllAction = "DecryptoSerializeAll";
    public readonly static List<string> BaseTriggerRowActions = new List<string> {
      CreateActionName, EditActionName, DeleteActionName,
    };
    public readonly static List<string> DefaultNonCreateRowActions = new List<string> {
      EditActionName, DeleteActionName, DetailsActionName, DownloadAttachmentsActionName,
    };
    public readonly static List<string> DefaultRowActions = new List<string> {
      CreateActionName, EditActionName, DeleteActionName, DetailsActionName, DownloadAttachmentsActionName,
    };
    public readonly static List<string> DefaultGroupByRowActions = new List<string> {
      CreateGroupActionName, EditGroupActionName, DeleteGroupActionName, GroupDetailsActionName,
    };
    public readonly static List<string> DefaultAllRowActions = new List<string> {
      CreateActionName, EditActionName, DeleteActionName, DetailsActionName, DownloadAttachmentsActionName,
      CreateGroupActionName, EditGroupActionName, DeleteGroupActionName, GroupDetailsActionName,
    };
    public readonly static List<string> DefaultGroupByNonCreateRowActions = new List<string> {
      EditGroupActionName, DeleteGroupActionName, GroupDetailsActionName,
    };
    public readonly static List<string> DefaultAllCreateRowActions = new List<string> {
      CreateActionName, CreateGroupActionName,
    };
    public readonly static List<string> DefaultAllNonCreateRowActions = new List<string> {
      EditActionName, DeleteActionName, DetailsActionName, DownloadAttachmentsActionName, EditGroupActionName, DeleteGroupActionName, GroupDetailsActionName,
    };
    public const string AddActionName = "Add"; //used in ListColumn button, for giving "real" action name whenever necessary
    public const string FilterActionName = "Filter";
    public const string RemoveActionName = "Remove";
    public const string CopyActionName = "Copy"; //used in ListColumn button, for giving "real" action name whenever necessary
    public const string TableActionNamePostfix = "TableActionName";
    public const string DefaultTableActionPrefix = "DefaultTableAction";
    public const string ExportToCSVTableActionName = "ExportToCSV";
    public const string ExportAllToCSVTableActionName = "ExportAllToCSV";
    public readonly static List<string> DefaultTableActions = new List<string> {
      ExportToCSVTableActionName, ExportAllToCSVTableActionName
    };
    public const string IndexPageName = "Index";
    public const string CreateEditFilterPageName = "CreateEditFilter";
    public const string DetailsPageName = "Details";
    public const string CsvPageName = "Csv";

    //Base data types
    public const string SharedPrefixDataType = V.SystemPrefix;
    public const string UnknownDataType = "Unknown";
    public const string StringDataType = V.StringDataType;
    public const string BooleanDataType = V.BooleanDataType;
    public const string CharDataType = V.CharDataType;
    public const string DateTimeDataType = V.DateTimeDataType;
    public const string ByteDataType = V.ByteDataType;
    public const string SByteDataType = V.SByteDataType;
    public const string Int16DataType = V.Int16DataType;
    public const string Int32DataType = V.Int32DataType;
    public const string Int64DataType = V.Int64DataType;
    public const string UInt16DataType = V.UInt16DataType;
    public const string UInt32DataType = V.UInt32DataType;
    public const string UInt64DataType = V.UInt64DataType;
    public const string SingleDataType = V.SingleDataType;
    public const string DoubleDataType = V.DoubleDataType;
    public const string DecimalDataType = V.DecimalDataType;
    public const string NullableIndicator = V.NullableIndicator;

    //ScriptConstructorColumn related
    public const string ScPictureLinksAttribute = "PictureLinks";
    public const string ScRefTableNameAttribute = "RefTableName";
    public readonly static List<string> ScAttributes = new List<string> { //list of allowed attributes
      ScPictureLinksAttribute, ScRefTableNameAttribute
    };
    public const string PP = "@@"; //parameter prefix = @@
    //The parameters must be in the form of @@parName then the next can be anything. i.e. @@parName) or @@parName other or @@parName, etc... but dot is linked
    public readonly static Regex ParRegex = new Regex(PP + @"([\p{L}_$][\p{L}\p{N}_$]*\.)*[\p{L}_$][\p{L}\p{N}_$]*", RegexOptions.Multiline);

    //TableType related
    public const string NormalTableType = "Normal";
    public const string GroupTableType = "Group";
    public readonly static List<string> TableTypes = new List<string> {
        NormalTableType, GroupTableType };
    public const string GroupByAutoDirectiveInt = "AUTO-INT";
    public const string GroupByAutoDirectiveDateTime = "AUTO-DATETIME";
    public readonly static List<string> GroupByAutoDirectives = new List<string> {
      GroupByAutoDirectiveInt, GroupByAutoDirectiveDateTime,
    };

    //DB related
    public const string MetaTableName = "MetaItem";
    public const string MetaTableNameColumnName = "TableName";
    public const string DataDBConnectionStringName = "CoreDataModel";
    public const string UserDBConnectionStringName = "DefaultConnection";
    public const string AccessLogTableName = "CoreAccessLog";
    public const string ActionLogTableName = "CoreActionLog";
    public const string ErrorLogTableName = "CoreErrorLog";
    public const string UserMapTableName = "CoreUserMap";
    public const string EmailInfoTableName = "CoreEmailInfo";
    public const string EmailTemplateTableName = "CoreEmailTemplate";
    public const string UserIdColumnName = "Id";
    public const string RoleIdColumnName = "Id";
    public const string TeamIdColumnName = "Id";
    public const string UserNameColumnName = "UserName";
    public const string UserFullNameColumnName = "FullName";
    public const string UserDisplayNameColumnName = "DisplayName";
    public const string UserEmailColumnName = "Email";
    public const string UserTeamColumnName = "Team";
    public const string UserWorkingRoleColumnName = "WorkingRole";
    public const string UserAdminRoleColumnName = "AdminRole";
    public const string UserRegistrationDateColumnName = "RegistrationDate";
    public const string UserLastLoginColumnName = "LastLogin";
    public const string RoleNameColumnName = "Name";
    public const string TeamNameColumnName = "Name";
    public const string UserRoleUserColumnName = "UserId";
    public const string UserRoleRoleColumnName = "RoleId";
    public const string AscOrderWord = "ASC";
    public const string DescOrderWord = "DESC";
    public const string NULL = "NULL";
    public const string AggrMax = "MAX";
    public const string AggrMin = "MIN";
    public const string AggrCount = "COUNT";
    public const string AggrSum = "SUM";
    public const string AggrAvg = "AVG";
    public const string DefaultHRTSColumnName = "HRTS";
    public const string AutoGeneratedHRTSWord = "AUTO-GENERATED";
    public const string Min = "Min";
    public const string Max = "Max";
    public const string DBProviderSQLServerName = "SQL Server";
    public const string DBProviderOracleName = "Oracle";
    public readonly static List<string> MinMax = new List<string> { //for number limit column only
      Min, Max
    };
    //valid aggregate names for comparison expression
    public readonly static List<string> AggregateNames = new List<string> {
        AggrMin, AggrMax, AggrCount, AggrSum, AggrAvg };
    public readonly static List<string> CoreTableNames = new List<string> {
      AccessLogTableName, ActionLogTableName, ErrorLogTableName, UserMapTableName, EmailInfoTableName, EmailTemplateTableName,
    };
    public readonly static List<string> UserRelatedDirectives = new List<string> {
      All,
    };
    public readonly static List<string> NonRecordedActions = new List<string> {
      IndexActionName, DetailsActionName
    };
    public readonly static List<string> AcceptablePageNames = new List<string> {
      IndexPageName, CreateEditFilterPageName, DetailsPageName, CsvPageName,
    };
    public readonly static List<string> ValidOrderDirections = new List<string>() { AscOrderWord, DescOrderWord };
    public static string DataDBConnectionString { get { return ConfigurationManager.ConnectionStrings[DataDBConnectionStringName].ConnectionString; } }
    public static string UserDBConnectionString { get { return ConfigurationManager.ConnectionStrings[UserDBConnectionStringName].ConnectionString; } }

    //For number and date time. Six codes are accepted: 
    //(1) GE = greater than or equal to, 
    //(2) G = greater than, 
    //(3) E = equal to, 
    //(4) L = less than, 
    //(5) LE = less than or equal to, 
    //(6) NE = not equal to. 
    public readonly static List<string> ValidComparatorSigns = new List<string>() {
      "GE", "G", "E", "L", "LE", "NE"
    };
    //For other data types, only code (3) E and code (6) NE are valid
    public readonly static List<string> ValidComparatorSignsForOthers = new List<string>() {
      "E", "NE"
    };

    //Parameterizations
    public const string SQLScriptDirectivePrefix = "SQL-SCRIPT:";
    public const string UserPrefix = "USER:";
    public const string ForeignInfoPrefix = "ForeignInfo";
    public const string ParameterUserPrefix = "User.";
    public const string ParameterDataPrefix = "Data.";
    public const string ParameterNow = "Now";
    public const string ParameterToday = "Today";
    public const string ParameterTableName = "TableName";
    public const string ParameterTableSource = "TableSource";
    public const string ParameterRowAction = "RowAction";

    //Emails
    public const string EmailMakerTemplateNameColumnName = "TemplateName";
    public const string EmailMakerParamatersColumnName = "EmailParameters";
    public const string EmailMakerIsSentColumnName = "IsSent";
    public const string SMTPSectionConf = "system.net/mailSettings/smtp";
    public const string SMTP_SERVER = "http://schemas.microsoft.com/cdo/configuration/smtpserver";
    public const string SMTP_SERVER_PORT = "http://schemas.microsoft.com/cdo/configuration/smtpserverport";
    public const string SEND_USING = "http://schemas.microsoft.com/cdo/configuration/sendusing";
    public const string SMTP_USE_SSL = "http://schemas.microsoft.com/cdo/configuration/smtpusessl";
    public const string SMTP_AUTHENTICATE = "http://schemas.microsoft.com/cdo/configuration/smtpauthenticate";
    public const string SEND_USERNAME = "http://schemas.microsoft.com/cdo/configuration/sendusername";
    public const string SEND_PASSWORD = "http://schemas.microsoft.com/cdo/configuration/sendpassword";

    //Meta Item Column Name
    public const string MICNTableName = "TableName";
    public const string MICNDisplayName = "DisplayName";
    public const string MICNTableSource = "TableSource";
    public const string MICNPrefilledColumns = "PrefilledColumns";
    public const string MICNItemsPerPage = "ItemsPerPage";
    public const string MICNOrderBy = "OrderBy";
    public const string MICNActionList = "ActionList";
    public const string MICNDefaultActionList = "DefaultActionList";
    public const string MICNTableActionList = "TableActionList";
    public const string MICNDefaultTableActionList = "DefaultTableActionList";
    public const string MICNTextFieldColumns = "TextFieldColumns";
    public const string MICNPictureColumns = "PictureColumns";
    public const string MICNIndexShownPictureColumns = "IndexShownPictureColumns";
    public const string MICNRequiredColumns = "RequiredColumns";
    public const string MICNNumberLimitColumns = "NumberLimitColumns";
    public const string MICNRegexCheckedColumns = "RegexCheckedColumns";
    public const string MICNRegexCheckedColumnExamples = "RegexCheckedColumnExamples";
    public const string MICNUserRelatedFilters = "UserRelatedFilters";
    public const string MICNDisableFilter = "DisableFilter";
    public const string MICNForcedFilterColumns = "ForcedFilterColumns";
    public const string MICNColumnExclusionList = "ColumnExclusionList";
    public const string MICNFilterExclusionList = "FilterExclusionList";
    public const string MICNDetailsExclusionList = "DetailsExclusionList";
    public const string MICNCreateEditExclusionList = "CreateEditExclusionList";
    public const string MICNCsvExclusionList = "CsvExclusionList";
    public const string MICNAccessExclusionList = "AccessExclusionList";
    public const string MICNColoringList = "ColoringList";
    public const string MICNFilterDropDownLists = "FilterDropDownLists";
    public const string MICNCreateEditDropDownLists = "CreateEditDropDownLists";
    public const string MICNPrefixesOfColumns = "PrefixesOfColumns";
    public const string MICNPostfixesOfColumns = "PostfixesOfColumns";
    public const string MICNListColumns = "ListColumns";
    public const string MICNTimeStampColumns = "TimeStampColumns";
    public const string MICNHistoryTable = "HistoryTable";
    public const string MICNHistoryTriggers = "HistoryTriggers";
    public const string MICNAutoGeneratedColumns = "AutoGeneratedColumns";
    public const string MICNColumnSequence = "ColumnSequence";
    public const string MICNColumnAliases = "ColumnAliases";
    public const string MICNEditShowOnlyColumns = "EditShowOnlyColumns";
    public const string MICNScriptConstructorColumns = "ScriptConstructorColumns";
    public const string MICNScriptColumns = "ScriptColumns";
    public const string MICNCustomDateTimeFormatColumns = "CustomDateTimeFormatColumns";
    public const string MICNEmailMakerTriggers = "EmailMakerTriggers";
    public const string MICNEmailMakers = "EmailMakers";
    public const string MICNNonPictureAttachmentColumns = "NonPictureAttachmentColumns";
    public const string MICNDownloadColumns = "DownloadColumns";
    public const string MICNPreActionTriggers = "PreActionTriggers";
    public const string MICNPreActionProcedures = "PreActionProcedures";
    public const string MICNPostActionTriggers = "PostActionTriggers";
    public const string MICNPostActionProcedures = "PostActionProcedures";
    public const string MICNTableType = "TableType";
    public const string MICNAggregationStatement = "AggregationStatement";
    public const string MICNForeignInfoColumns = "ForeignInfoColumns";
    public readonly static List<string> MICNGroupDropDownLists = new List<string> {
      MICNFilterDropDownLists, MICNCreateEditDropDownLists,
    };
    public readonly static List<string> MICNGroupTagged = new List<string> {
      MICNRegexCheckedColumns, MICNRegexCheckedColumnExamples,
    };
    public readonly static List<string> MICNGroupTVR = new List<string> {
      MICNCreateEditDropDownLists, MICNFilterDropDownLists, MICNListColumns,
    };
    public readonly static List<string> MICNGroupVBarBreak = new List<string> {
      MICNColoringList, MICNTimeStampColumns, MICNHistoryTriggers, MICNScriptConstructorColumns,
      MICNCustomDateTimeFormatColumns, MICNPreActionTriggers, MICNPostActionTriggers
    };
    public readonly static List<string> MICNGroupTriggered = new List<string> {
      MICNHistoryTriggers, MICNEmailMakerTriggers, MICNPreActionTriggers, MICNPostActionTriggers,
    };
    public readonly static List<string> MICNGroupActionTriggers = new List<string> {
      MICNPreActionTriggers, MICNPostActionTriggers,
    };
    public readonly static List<string> MICNGroupScripted = new List<string> {
      MICNOrderBy, MICNCreateEditDropDownLists, MICNFilterDropDownLists, MICNListColumns,
      MICNHistoryTriggers, MICNScriptConstructorColumns, MICNScriptColumns, MICNEmailMakerTriggers,
      MICNPreActionTriggers, //MICNPreActionProcedures,
      MICNPostActionTriggers, //MICNPostActionProcedures,
    };

    //Constants
    public const string DropDownTextDataType = "string";
    public const string DropDownNumberDataType = "number";
    public const string DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    public const string DefaultDateFormat = "yyyy-MM-dd";
    public const string DefaultTimeFormat = "HH:mm:ss"; //used in create and edit
    public const string DefaultTimeFormatWithoutSecond = "HH:mm"; //used in filter
    public const string Now = "NOW";
    public const string Self = "SELF";
    public const string Skip = "SKIP";
    public const string Ref = "reference";
    public const string Cid = "Cid"; //capital C
    public const string All = "All";
    public const string Any = "Any";
    public const string BvTrue = "True"; //used in boolean in create edit, as value, not as display
    public const string BvFalse = "False"; //display will be controlled in the LCZ
    public const string True = "True"; //used in KeyInfo.CreateQueryValueAsObject
    public const string False = "False";
    public readonly static List<string> TrueFalse = new List<string> { True, False };
    public readonly static List<string> BvTrueFalse = new List<string> { BvTrue, BvFalse };
    //public const string LcYes = "Yes"; //used in list column, as value, not as display, not need anymore, because for Lc, the value and display must be the same
    //public const string LcNo = "No";

    //Specials
    public const string RegexCheckedColumnTag = "reg";
    public const string RegexCheckedColumnExampleTag = "ex";
    public readonly static List<string> AllowedMetaTableTags = new List<string> {
      RegexCheckedColumnTag, RegexCheckedColumnExampleTag
    };
    public readonly static List<KeyValuePair<char, char>> DefaultScriptEnclosurePairs = new List<KeyValuePair<char, char>> {
      new KeyValuePair<char, char>('"', '"'), new KeyValuePair<char, char>('\'', '\''),
    };

    //Column Naming
    public const string TableNameParameterName = "commonDataTableName";
    public const string BaseAppendixName = "CommonData";
    public const string BaseFilterAppendixName = BaseAppendixName + "Filter";
    public const string BaseCreateEditAppendixName = BaseAppendixName + "CreateEdit";
    public const string CreateEditPictureLinkAppendixName = BaseCreateEditAppendixName + "PictureLink";
    public const string CreateEditTimeAppendixName = BaseCreateEditAppendixName + "Time";
    public const string CreateEditNonPictureAttachmentAppendixName = BaseCreateEditAppendixName + "AttachmentLink";
    public const string ListColumnAppendixName = BaseAppendixName + "ListColumn";
    public const string FromName = "From";
    public const string ToName = "To";
    public const string FilterTimeAppendixFrontName = BaseFilterAppendixName + "Time";
    public const string FilterDateAppendixFrontName = BaseFilterAppendixName + "Date";
    public const string FilterTableActionNameInput = BaseFilterAppendixName + TableActionNamePostfix;
    public readonly static List<string> ExemptedFilterFormCollection = new List<string>() {
      BaseFilterAppendixName + "Page",
      BaseFilterAppendixName + "No",
      BaseFilterAppendixName + "Msg",
      BaseFilterAppendixName + "UserName",
      BaseFilterAppendixName + "Type",
      BaseFilterAppendixName + TableActionNamePostfix,
    };
    public readonly static List<string> FilterTimeAppendixNames = new List<string> {
      FilterTimeAppendixFrontName + FromName,
      FilterTimeAppendixFrontName + ToName
    };
    public readonly static List<string> FilterDateAppendixNames = new List<string> {
      FilterDateAppendixFrontName + FromName,
      FilterDateAppendixFrontName + ToName
    };
    public readonly static List<string> NumberDataTypes = V.NumberDataTypes;
    public readonly static List<string> NullableIndicators = new List<string> {
      NullableIndicator, StringDataType
    };
    public readonly static List<string> EqualNotEqualOnlyDataTypes = new List<string> {
      StringDataType, CharDataType, BooleanDataType
    };

    public readonly static List<string> NumberTypeFilterColumns = new List<string> {
      BaseFilterAppendixName + Int16DataType + FromName,
      BaseFilterAppendixName + Int32DataType + FromName,
      BaseFilterAppendixName + Int64DataType + FromName,
      BaseFilterAppendixName + UInt16DataType + FromName,
      BaseFilterAppendixName + UInt32DataType + FromName,
      BaseFilterAppendixName + UInt64DataType + FromName,
      BaseFilterAppendixName + DecimalDataType + FromName,
      BaseFilterAppendixName + DoubleDataType + FromName,
      BaseFilterAppendixName + SingleDataType + FromName,
      BaseFilterAppendixName + ByteDataType + FromName,
      BaseFilterAppendixName + SByteDataType + FromName,

      BaseFilterAppendixName + Int16DataType + ToName,
      BaseFilterAppendixName + Int32DataType + ToName,
      BaseFilterAppendixName + Int64DataType + ToName,
      BaseFilterAppendixName + UInt16DataType + ToName,
      BaseFilterAppendixName + UInt32DataType + ToName,
      BaseFilterAppendixName + UInt64DataType + ToName,
      BaseFilterAppendixName + DecimalDataType + ToName,
      BaseFilterAppendixName + DoubleDataType + ToName,
      BaseFilterAppendixName + SingleDataType + ToName,
      BaseFilterAppendixName + ByteDataType + ToName,
      BaseFilterAppendixName + SByteDataType + ToName,
    };
    public readonly static List<string> BooleanTypeFilterColumns = new List<string>() {
      BaseFilterAppendixName + BooleanDataType };
    public readonly static List<string> CharTypeFilterColumns = new List<string>() {
      BaseFilterAppendixName + CharDataType };
  }
}