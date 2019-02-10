using Extension.String;
using System;
using System.Collections.Generic;

namespace Aibe {
  //Used primarily for checker helper and logging
  public static class LCZ { //can be extended as wanted
    //Error
    public static string E_ColumnUnknown = "The column name [{0}] is unknown"; //used
    public static string E_ContainsPotentiallyDangerousElements = "Column [{0}] contains potentially dangerous value [{1}]";
    public static string E_DoesNotMatchWithPattern = "The input string [{0}] for [{1}] does not match with the required pattern"; //used
    public static string E_FieldIsRequired = "Field [{0}] is required"; //used
    public static string E_MaxLengthViolated = "The input string [{0}] for [{1}] is too long. " + 
      "The maximum length for [{1}] is {2} character(s)"; //used
    public static string E_MinLengthViolated = "The input string [{0}] for [{1}] is too short. " +
      "The minimum length for [{1}] is {2} character(s)"; //used
    public static string E_MaxValueViolated = "The input value [{0}] for field [{1}] is greater than the maximum limit [{2}]"; //used
    public static string E_MinValueViolated = "The input value [{0}] for field [{1}] is smaller than the minimum limit [{2}]"; //used
    public static string E_PatternExample = "Example(s) of correct pattern: {0}"; //used
    public static string E_SQLInjection = "Column: {0}" + Environment.NewLine + 
      "DataType: {1}" + Environment.NewLine + "Value: {2}"; //used
    public static string E_WrongFormat = "The input for field [{0}] is in a non-acceptable data format. " +
      "The data type is [{1}] but the value given is [{2}]. " +
      "Please correct your input data format"; //used
    //-----------------below is not used by the Aibe itself
    public static string E_AlreadyExist = "[{0}] already exist";
    public static string E_CannotBeEditedOrDeleted = "[{0}] cannot be edited or deleted";
    public static string E_FailToDoActionOnItem = "Failed to [{0}] [{1}]";
    public static string E_FailToDoActionOnItemIn = "Failed to [{0}] [{1}] in [{2}]";
    public static string E_FieldsCannotBeBothEmpty = "Fields [{0}] and [{1}] cannot be both empty";
    public static string E_FolderPathNotInitialized = "[{0}] folder path is not initialized";
    public static string E_InvalidOrEmptyParameter = "Invalid/Empty parameters for [{0}]"; //0: tableName
    public static string E_NoRecordFoundIn = "No record found in [{0}]";
    public static string E_NoRecordFoundInTable = "No record found in Table [{0}]";
    public static string E_NoRecordFoundInTableForCid = "No record found in Table [{0}] with [" + DH.Cid + "] = {1}";
    public static string E_TableActionUnsuccessful = "Execution of table action [{0}] is unsuccessful!";

    //Non-formatted error
    public static string NFE_MetaInfoCannotBeNull = "MetaInfo cannot be null";
    public static string NFE_PasswordDoesNotMatch = "Password does not match";
    //-----------------below is not used by the Aibe itself
    public static string NFE_BadComparison = "Bad comparison";
    public static string NFE_FullyAutomaticGroup = "This group's making is fully automated, no edit is allowed";
    public static string NFE_GeneralError = "An error has occurred";
    public static string NFE_GeneralErrorMessage = "There was an error occurred while we were processing your request";
    public static string NFE_IdNotFound = "Id not found";
    public static string NFE_InputCannotBeEmpty = "Input cannot be empty";
    public static string NFE_InvalidColumnName = "Invalid column name";
    public static string NFE_InvalidLoginAttempt = "Invalid login attempt";
    public static string NFE_InvalidKeywordOrComponent = "Invalid keyword/component";
    public static string NFE_InvalidName = "Invalid name";
    public static string NFE_MatchingTheAppliedFilter = "matching the applied filter(s)"; //error because it is used together with E_NoRecordFoundInTable
    public static string NFE_NoDataTableFound = "No data table found"; //Happen in index, when the data table is not found
    public static string NFE_NoRecordFound = "No record found"; //when detail is loaded/filtered but cannot be found
    public static string NFE_UserCannotBeAuthenticated = "User cannot be authenticated";
    public static string NFE_UserClaimNotFound = "User claim not found";
    public static string NFE_UserCreationFailed = "User not created";
    public static string NFE_UserIdentityMustBeFilled = "User identity must be filled";
    public static string NFE_UserIdNotFound = "User Id not found";
    public static string NFE_UserNotFound = "User not found";
    public static string NFE_UserUpdateFailed = "User not updated";
    public static string NFE_UserRemovalFailed = "User not removed";
    public static string NFE_RoleAdditionFailed = "Role not added";
    public static string NFE_RoleCreationFailed = "Role not created";
    public static string NFE_RoleIdNotFound = "Role Id not found";
    public static string NFE_RoleNotFound = "Role not found";
    public static string NFE_RoleUpdateFailed = "Role not updated";
    public static string NFE_RoleRemovalFailed = "Role not removed";
    public static string NFE_TeamCreationFailed = "Team not created";
    public static string NFE_TeamUpdateFailed = "Team not updated";
    public static string NFE_TeamRemovalFailed = "Team not removed";
    public static string NFE_TeamNotFound = "Team not found";

    //Formatted Message ---all is unused by Aibe
    public static string M_CryptoSerializeAllSuccess = "You have successfully crypto-serialize all ({0}) meta table entries!";
    public static string M_DecryptoSerializeAllSuccess = "You have successfully decrypto-serialize all ({0}) meta table {1} file(s)!";
    public static string M_FilterFor = "Filter for [{0}]"; //{0} = TableDisplayName
    public static string M_ItemIsUpdated = "The item with [Id = {0}] is successfully updated";
    public static string M_ItemIsNotUpdated = "The item with [Id = {0}] is not updated";
    public static string M_ItemIsDeleted = "The item with [Id = {0}] is successfully deleted";
    public static string M_ItemIsNotDeleted = "The item with [Id = {0}] is not deleted";
    public static string M_ListOf = "List of [{0}]";
    public static string M_MetaItemsAreUpdated = "You have successfully updated all ({0}) meta table entries!";
    public static string M_MetaItemIsCryptoSerialized = "The meta item with [Id = {0}] is successfully crypto-serialized!";
    public static string M_TableActionSuccess = "Table action [{0}] is successfully run!";
    public static string M_DefaultActionNotImplemented = "Default action [{0}] is not implemented for [{1}]";

    //Message ---all is unused by Aibe
    public static string NFM_AddPhoneSuccess = "Your phone number was added";
    public static string NFM_ApplyFilterButtonHelp = "Use this button to apply the filter form";
    public static string NFM_ChangeDisplayName = "Change Display Name"; //used in the View Title and Button when changing display name only
    public static string NFM_ChangeDisplayNameForm = "Change Display Name Form"; //used in the View Sub-title when changing display name only
    public static string NFM_ChangeDisplayNameSuccess = "Your display name has been changed";
    public static string NFM_ChangePassword = "Change Password"; //used in the View Title and Button when changing password only
    public static string NFM_ChangePasswordForm = "Change Password Form"; //used in the View Sub-title when changing password only
    public static string NFM_ChangePasswordSuccess = "Your password has been changed";
    public static string NFM_ChangeYourAccountSettings = "Change your account settings"; //used only in the Account Management page
    public static string NFM_CloseFilterButtonHelp = "Use this button to close the filter form";
    public static string NFM_DeleteGroupConfirmation = "Are you sure you want to delete this group?";
    public static string NFM_FootNotesRequired = "Notes: * required field, ** at least one must be filled";
    public static string NFM_ManageYourAccount = "Manage Your Account"; //Used as title message when mouse is placed over our display name in the login portion of the header
    public static string NFM_NewItemIsCreated = "The new item is successfully created";
    public static string NFM_NewItemIsNotCreated = "The new item is not created";
    public static string NFM_RegisterAsNewUser = "Register as a new user";
    public static string NFM_RemovePhoneSuccess = "Your phone number was removed";
    public static string NFM_SeeOnDetails = "See on Details";
    public static string NFM_SetPasswordSuccess = "Your password has been set";
    public static string NFM_SetTwoFactorSuccess = "Your two-factor authentication provider has been set";

    //Specific when error occurs but really is a message... ---all is unused by Aibe
    public static string NFM_ActionNotReady = "Action not ready";
    public static string NFM_ActionNotReadyMessage = "Default action not ready, please check again next time!";
    public static string NFM_ActionNotFound = "Action not found";
    public static string NFM_ActionNotFoundMessage = "Please ensure that you have input the right action name";
    public static string NFM_InsufficientAccessRight = "Insufficient Access Right";
    public static string NFM_InsufficientAccessRightActionMessage = "You do not have sufficient right to do this action";
    public static string NFM_InsufficientAccessRightPageMessage = "You do not have sufficient right to open this page";
    public static string NFM_LoginWithDifferentUserAccount = "Login with different user account";
    public static string NFM_LockedOut = "Locked Out";
    public static string NFM_LockedOutMessage = "This account has been locked out, please try again later";
    public static string NFM_TableDescriptionNotFound = "Table Description Not Found";
    public static string NFM_TableDescriptionNotFoundMessage = "Please ensure that you have input the right table name";
    public static string NFM_TableNotReady = "Table Not Ready";
    public static string NFM_TableNotReadyMessage = "Table data not ready, please check again next time!";

    //Questions ---all is unused by Aibe
    public static string NFQ_CryptoSerialize = "Are you sure to serialize and to encrypt this data?";
    public static string NFQ_DeleteConfirmation = "Are you sure to delete this?";
    public static string NFQ_DeleteRecord = "Are you sure you want to delete this record?";
    public static string NFQ_ForgotYourPassword = "Forgot your password?";

    //Items (basically, non-spaced multiple (>1) words) ---ALL IS USED by Aibe
    public static string I_AuthenticateUser = "AuthenticateUser"; //for logging info only
    public static string I_EditName = "EditName"; //for logging info only
    public static string I_GetUser = "GetUser"; //for logging info only
    public static string I_MetaInfo = "MetaInfo"; //for null exception, should actually be remained as MetaInfo unless name changed
    public static string I_MetaItemApplyAllUpdatesActionName = "ApplyAllUpdates";
    public static string I_MetaItemApplyUpdatesActionName = "ApplyUpdates";
    public static string I_MetaItemCryptoSerializeActionName = "CryptoSerialize";
    public static string I_MetaItemCryptoSerializeAllActionName = "CryptoSerializeAll";
    public static string I_MetaItemDecryptoSerializeAllActionName = "DecryptoSerializeAll";
    public static string I_SetPassword = "SetPassword"; //for logging only, when UserMap password is changed
    public static string I_UnknownColumn = "UnknownColumn"; //for error dictionary generated by CheckModelValidity, when column is unknown
    public static string I_UserMap = "UserMap"; //for logging, all used in the UserHelper

    //Table column names
    public static string T_RoleNameColumnName = "Name";
    public static string T_TeamNameColumnName = "Team";
    public static string T_UserIdColumnName = "Id";
    public static string T_UserNameColumnName = "User Name";
    public static string T_UserFullNameColumnName = "Full Name";
    public static string T_UserDisplayNameColumnName = "Display Name";
    public static string T_UserEmailColumnName = "Email";
    public static string T_UserTeamColumnName = "Team";
    public static string T_UserWorkingRoleColumnName = "Working Role";
    public static string T_UserAdminRoleColumnName = "Admin Role";
    public static string T_UserRegistrationDateColumnName = "Registration Date";
    public static string T_UserLastLoginColumnName = "Last Login";

    //Words
    public static string W_Aibe = "Aibe";
    public static string W_Auto = "Auto";
    public static string W_Create = "Create";
    public static string W_CreateGroup = "Create Group";
    public static string W_Delete = "Delete";
    public static string W_DeleteGroup = "Delete Group";
    public static string W_Details = "Details";
    public static string W_DownloadAttachments = "Download Attachments";
    public static string W_DropDownUpdate = "Dropdown Update";
    public static string W_Edit = "Edit";
    public static string W_EditGroup = "Edit Group";
    public static string W_Ending = "Ending";
    public static string W_FirstPage = "First Page";
    public static string W_GroupDetails = "Group Details";
    public static string W_Header = "Header";
    public static string W_LastPage = "Last Page";
    public static string W_Name = "Name";
    public static string W_NextPage = "Next Page";
    public static string W_Next10Pages = "Next 10 Pages";
    public static string W_Next100Pages = "Next 100 Pages";
    public static string W_NotFound = "Not found";
    public static string W_PreviousPage = "Previous Page";
    public static string W_Previous10Pages = "Previous 10 Pages";
    public static string W_Previous100Pages = "Previous 100 Pages";
    public static string W_SQLInjection = "SQL Injection";
    public static string W_Value = "Value";
    //-----------------below is not used by the Aibe itself
    public static string W_About = "About";
    public static string W_AccessLog = "Access Log";
    public static string W_Account = "Account";
    public static string W_AccountManagement = "Account Management";
    public static string W_Action = "Action";
    public static string W_ActionLog = "Action Log";
    public static string W_Actions = "Actions"; //for rightmost item on ListColumn
    public static string W_Add = "Add";
    public static string W_Admin = "Admin";
    public static string W_AdminPage = "Admin Page";
    public static string W_And = "And";
    public static string W_Apply = "Apply"; //used in "Apply" filter
    public static string W_BackToAdmin = "Back to Admin";
    public static string W_BackToGroupList = "Back to Group List";
    public static string W_BackToHome = "Back to Home";
    public static string W_BackToList = "Back to List";
    public static string W_BaseError = "Base Error"; //Only shown in error page, title of the error message
    public static string W_BoFalse = "False"; //To be displayed in boolean option
    public static string W_BoTrue = "True"; //To be displayed in boolean option
    public static string W_Browse = "Browse";
    public static string W_Button = "Button"; //currently not used
    public static string W_Cancel = "Cancel";
    public static string W_Change = "Change";
    public static string W_Close = "Close";
    public static string W_Column = "Column";
    public static string W_Configuration = "Configuration"; //used in the index home page, the first step, "Configuration"
    public static string W_Configurations = "Configurations";
    public static string W_Confirm = "Konfirmasi";
    public static string W_ConfirmPassword = "Confirm Password";
    public static string W_Contact = "Contact";
    public static string W_Copy = "Copy";
    public static string W_CreateNew = "Create New";
    public static string W_CreateLocalLogin = "Create Local Login"; //Originally in the "Set Password" page
    public static string W_CryptExtension = "cfile";
    public static string W_CryptoSerialize = "Cryto-Serialize";
    public static string W_Data = "Data";
    public static string W_DecryptoSerialize = "Decryto-Serialize";
    public static string W_Desc = "Desc"; //description can be used when making new item from description
    public static string W_DeveloperOptions = "Developer Options";
    public static string W_Download = "Download";
    public static string W_Email = "Email";
    public static string W_Error = "Error";
    public static string W_ErrorList = "Error List";
    public static string W_ErrorLog = "Error Log";
    public static string W_ErrorMessage = "Error Message";
    public static string W_ErrorModelCodeWord = "Code";
    public static string W_ErrorModelMessageWord = "Message";
    public static string W_ErrorModelExceptionWord = "Exception";
    public static string W_ErrorModelStackTraceWord = "Stack Trace";
    public static string W_Exit = "Exit";
    public static string W_ExportToCSV = "Export to CSV";
    public static string W_ExportAllToCSV = "Export All to CSV";
    public static string W_Failed = "Failed";
    public static string W_False = "False"; //Normal true and false
    public static string W_Filter = "Filter";
    public static string W_FilterElement = "Filter element";
    public static string W_FilterElements = "Filter elements";
    public static string W_Final = "Final";
    public static string W_First = "First";
    public static string W_From = "from";
    public static string W_FromTo = "from-to";
    public static string W_Form = "Form"; //may or may not be used
    public static string W_Global = "Global";
    public static string W_Hello = "Hello"; //for greeting the display name of the logged in account
    public static string W_History = "History";
    public static string W_HistoryLog = "History Log"; //used in the index home page, the fourth step, "History Logs"
    public static string W_HistoryLogs = "History Logs";
    public static string W_Home = "Home";
    public static string W_HomePage = "Home Page";
    public static string W_in = "in";
    public static string W_Index = "Index";
    public static string W_Item = "Item";
    public static string W_Last = "Last";
    public static string W_LcNo = "No"; //To be displayed in ListColumn yes and no
    public static string W_LcYes = "Yes"; //To be displayed in ListColumn yes and no
    public static string W_Load = "Load";
    public static string W_Log = "Log";
    public static string W_LogIn = "Log-In";
    public static string W_LogOff = "Log-Off";
    public static string W_LogOn = "Log-On";
    public static string W_LogOut = "Log-Out";
    public static string W_Logs = "Logs";
    public static string W_Meta = "Meta";
    public static string W_NA = "Not Available";
    public static string W_NoAttachment = "no attachment";
    public static string W_NoPicture = "no picture";
    public static string W_Next = "Next";
    public static string W_NextSymbol = ">";
    public static string W_NewPassword = "New Password";
    public static string W_No = "No"; //Normal Yes and No
    public static string W_of = "of";
    public static string W_OldPassword = "Old Password";
    public static string W_Open = "Open";
    public static string W_Option = "Option";
    public static string W_Or = "Or";
    public static string W_Page = "Page";
    public static string W_Password = "Password";
    public static string W_PoweredBy = "Powered by";
    public static string W_Previous = "Previous";
    public static string W_PreviousSymbol = "<";
    public static string W_Registration = "Registration"; //used in the index home page, the second step, "Registration"
    public static string W_Registrations = "Registrations";
    public static string W_Remove = "Remove";
    public static string W_Role = "Role";
    public static string W_Roles = "Roles";
    public static string W_Save = "Save";
    public static string W_Schedule = "Schedule"; //used in the index home page, the thirs step, "Schedule" or "Job Schedules"
    public static string W_Schedules = "Schedules";
    public static string W_See = "See";
    public static string W_Settings = "Settings";
    public static string W_SetPassword = "Set Password"; //Only used in the button for "Set Password" page...
    public static string W_ShortNo = "No"; //is used in left most column header of the table, "No" = "Number"
    public static string W_Success = "Success";
    public static string W_Successful = "Successful";
    public static string W_Table = "Table";
    public static string W_Team = "Team";
    public static string W_Teams = "Teams";
    public static string W_To = "to";
    public static string W_True = "True"; //Normal true and false
    public static string W_Type = "Type";
    public static string W_Unknown = "Unknown";
    public static string W_Update = "Update";
    public static string W_Upload = "Upload";
    public static string W_User = "User";
    public static string W_UserMap = "User Map";
    public static string W_UserName = "User Name";
    public static string W_Users = "Users";
    public static string W_Yes = "Yes"; //Normal Yes and No

    //Descriptions ---all is not used by Aibe
    public static string D_About = "Your about description";
    public static string D_Contact = "Your contact description";
    public static string D_SetPassword = //I also do not know what SetPassword has such info in the beginning
      "You do not have a local username/password for this site. Add a local" +
      "account so you can log in without an external login.";

    public static string GetLocalizedDefaultActionName(string actionName) {
      if (actionName.EqualsIgnoreCase(DH.EditActionName))
        return W_Edit;
      if (actionName.EqualsIgnoreCase(DH.CreateActionName))
        return W_Create;
      if (actionName.EqualsIgnoreCase(DH.DeleteActionName))
        return W_Delete;
      if (actionName.EqualsIgnoreCase(DH.DetailsActionName))
        return W_Details;
      if (actionName.EqualsIgnoreCase(DH.FilterActionName))
        return W_Filter;
      if (actionName.EqualsIgnoreCase(DH.DownloadAttachmentsActionName))
        return W_DownloadAttachments;
      if (actionName.EqualsIgnoreCase(DH.CreateGroupActionName))
        return W_CreateGroup;
      if (actionName.EqualsIgnoreCase(DH.EditGroupActionName))
        return W_EditGroup;
      if (actionName.EqualsIgnoreCase(DH.DeleteGroupActionName))
        return W_DeleteGroup;
      if (actionName.EqualsIgnoreCase(DH.GroupDetailsActionName))
        return W_GroupDetails;
      return actionName; //localization fails
    }

    public static string GetLocalizedDefaultTableActionName(string actionName) {
      if (actionName.EqualsIgnoreCase(DH.ExportToCSVTableActionName))
        return W_ExportToCSV;
      if (actionName.EqualsIgnoreCase(DH.ExportAllToCSVTableActionName))
        return W_ExportAllToCSV;
      return actionName;
    }

    public static string GetLocalizedBooleanOption(bool val) {
      return val ? W_BoTrue : W_BoFalse;
    }

    public static string GetLocalizedBooleanOption(string valStr) {
      if (string.IsNullOrWhiteSpace(valStr))
        return string.Empty;
      bool val = !string.IsNullOrWhiteSpace(valStr) && valStr.EqualsIgnoreCase(DH.True);
      return val ? W_BoTrue : W_BoFalse;
    }

    public static bool IsLocalizedLcBooleanOption(string val) {
      return val.EqualsIgnoreCase(W_LcYes) || val.EqualsIgnoreCase(W_LcNo);
    }

    public static List<string> GetLocalizedLcBooleanOptions() {
      return new List<string> { string.Empty, W_LcYes, W_LcNo };
    }

    public static List<string> GetLocalizedBooleanOptions() {
      return new List<string> { string.Empty, W_BoTrue, W_BoFalse };
    }

    public static bool IsBooleanTrue(string localStr) {
      return !string.IsNullOrWhiteSpace(localStr) && W_BoTrue.EqualsIgnoreCase(localStr);
    }

    public static bool IsBooleanFalse(string localStr) {
      return !string.IsNullOrWhiteSpace(localStr) && W_BoFalse.EqualsIgnoreCase(localStr);
    }

    public static List<KeyValuePair<string, string>> GetLocalizedBooleanDropDownOptions() {
      return new List<KeyValuePair<string, string>> {
        new KeyValuePair<string, string>(null, null),
        new KeyValuePair<string, string>(W_BoTrue, DH.BvTrue),
        new KeyValuePair<string, string>(W_BoFalse, DH.BvFalse),
      };
    }
  }
}