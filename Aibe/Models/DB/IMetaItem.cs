namespace Aibe.Models.DB {
  public interface IMetaItem {
    /// <summary>
    /// The [C]ommon [ID]
    /// </summary>
    int Cid { get; set; }

    /// <summary>
    /// The name of the table as given by developers. Table whose name is not written here cannot be accessed by the engine.
    /// If [TableSource] column is empty, the value in this columns will be used as the name of the table in the database.
    /// </summary>
    string TableName { get; set; }

    /// <summary>
    /// The display name of the table. If this field is empty, [TableName] will be used as display name.
    /// </summary>
    string DisplayName { get; set; }

    /// <summary>
    /// The name of the table as written in the database. This column is to be used together with [PrefilledColumns].
    /// </summary>
    string TableSource { get; set; }

    /// <summary>
    /// To prefill some columns with some static values. This column is to be used together with [TableSource]. It is useful to create sub-table from a table.
    /// Columns which are prefilled will be: 
    /// 1. Used as filters to display index items.
    /// 2. Excluded from index, filter, and all default row and table actions.
    /// 3. Filled with the static values on item creation.
    /// <para>Syntax: ColumnName1=ColumnValue1;ColumnName2=ColumnValue2;…;ColumnNameN=ColumnValueN</para>
    /// <para>A non-string [ColumnValue] (for example, number) is written as it is. 
    /// SQL Server Only: Note that boolean column is a 1-or-0 number column (not true-or-false valued) in SQL Server.
    /// A string [ColumnValue] is written in between two apostrophes.The apostrophe in the string value is to be replaced with double apostrophes.</para>
    /// <para>Example: BrandName='O''Neil';Price=50;Location='Singapore';IsNew=0;IsExported=1</para>
    /// </summary>
    string PrefilledColumns { get; set; }

    /// <summary>
    /// Number of items (rows) to be displayed per page for the named table
    /// </summary>
    short? ItemsPerPage { get; set; }

    /// <summary>
    /// Specifying how the table display would be ordered.
    /// <para>Syntax: ColumnName1=OrderDir1;ColumnName2=OrderDir2;ColumnName3;…;ColumnNameN</para>
    /// Or
    /// <para>SQL-SCRIPT:OrderByScript</para>
    /// There are two ways to specify the OrderBy column:
    /// <para>1. The first, and safer, way is by declaring the column name and the order direction one by one. OrderDir is either ASC or DESC. If OrderDir is not specified, default direction of the Database provider will be assumed</para>
    /// 2. The second way is by using SQL script. Using the second, unsafe, way, the value must be initialized with the prefix "SQL-SCRIPT:" then followed by the OrderByScript to be used. Unenclosed double dash (--) and semicolon (;) must not be present in the OrderByScript. When used, the OrderByScript will be enclosed by parentheses by the script generator: ORDER BY (OrderByScript).
    /// </summary>
    string OrderBy { get; set; }

    /// <summary>
    /// Row action list of the named table, applied to every row of the table entries, except for Create.
    /// Currently available default row actions: Create, Edit, Delete, Details.
    /// <para>Syntax: RowAction1=Role1,Role2,...;RowAction2=Role3,Role4,...;…;RowActionN</para>
    /// [Role] is used to specify user roles to whom this column criteria will be applied.
    /// [Developer] and [Main Administrator] roles are immune from [Role] restrictions.
    /// </summary>
    string ActionList { get; set; }

    /// <summary>
    /// List of row actions which use the engine's default actions.
    /// <para>Syntax: RowAction1;RowAction2;…;RowActionN</para>
    /// </summary>
    string DefaultActionList { get; set; }

    /// <summary>
    /// Action list of the named table, applied to the table as a whole. Currently available default table actions: ExportToCsv, ExportAllToCsv.
    /// <para>Syntax: TableAction1=Role1,Role2,...;TableAction2=Role3,Role4,…;…;TableActionN</para>
    /// Role is used to specify user roles to whom this column criteria will be applied.
    /// [Developer] and [Main Administrator] roles are immune from role restrictions
    /// </summary>
    string TableActionList { get; set; }

    /// <summary>
    /// List of table actions which use the engine's default actions.
    /// <para>Syntax: TableAction1;TableAction2;...;TableActionN</para>
    /// </summary>
    string DefaultTableActionList { get; set; }

    /// <summary>
    /// To create TextArea, instead of TextBox, for data input control. Applied only for string (nvarchar, char, varchar, etc) column.
    /// <para>Syntax: ColumnName1=RowSize1;ColumnName2;…;ColumnNameN=RowSizeN</para>
    /// [RowSize] is used to determine how many rows are set for the TextArea for this column.
    /// </summary>
    string TextFieldColumns { get; set; }

    /// <summary>To upload picture, instead of normal string, for data input control. Applied only for string (nvarchar, char, varchar, etc) column.
    /// Column containing picture cannot be used for filtering (as image cannot be filtered like words do).
    /// <para>Syntax: ColumnName1=PicWidth1,PicHeight1|IndexPicWidth1,IndexPicHeight1;
    /// ColumnName2=PicWidth2,PicHeight2|SKIP,IndexPicHeight2;
    /// ColumnName3=PicWidth3,PicHeight3|IndexPicWidth3;
    /// ColumnName4=PicWidth4,PicHeight4;
    /// ColumnName5=SKIP,PicHeight5;
    /// ColumnName6=PicWidth6;…;
    /// ColumnNameN
    /// </para>
    /// Specify [PicWidth] and [PicHeight] to control the desired picture's width. Default value is 100px for both.
    /// [PicWidth] and [PicHeight] values, if specified, must in in the form of positive integer number.
    /// If [PicWidth] is specified while [PicHeight] is not, the picture will apply the [PicWidth] and maintain the aspect ratio for [PicHeight] (not using default value 100px).
    /// <para>
    /// To specify [PicHeight] without [PicWidth], fill the [PicWidth] with keyword "SKIP". Example: PicColumnName=SKIP,80.
    /// Skipping to specify [PicWidth] will make the picture apply [PicHeight] first then maintain the aspect ratio for [PicWidth] (not using default value 100px).
    /// To skip [PicWidth] and use default value for [PicHeight], simply use keyword "SKIP" without specifying [PicHeight].
    /// Example: PicColumnName=SKIP.
    /// Only [PicWidth] can be skipped.
    /// </para>
    /// If both [PicWidth] and [PicHeight] are specified, then the picture image will be stretched according to the specified values.
    /// If both [PicWidth] and [PicHeight] are not specified, then the picture image will use default value for [PicWidth] (100px) and maintain the aspect ratio for [PicHeight] (not using default value 100px).
    /// <para>
    /// If [IndexPicWidth] and/or [IndexPicHeight] are not specified, whatever has been applied in [PicWidth] and [PicHeight] will be used in index page too.
    /// If [IndexPicWidth] and/or [IndexPicHeight] are specified, the index page will follow the [IndexPicWidth] and [IndexPicHeight] size following the same rules as the previously explained [PicWidth] and [PicHeight] (i.e. if width only is specified, then height maintain aspect ratio and vice versa. If both are specified, picture will be stretched. However, if none is specified, it will follow the size and rules in the [PicWidth] and [PicHeight]).
    /// </para>
    /// </summary>
    string PictureColumns { get; set; }

    /// <summary>
    /// To specify which picture-type column to show the picture, instead of the string name, in the index page.
    /// <para>Syntax: ColumnName1;ColumnName2;…;ColumnNameN</para>
    /// Depending on [PictureColumns]. A column found here not found in the [PictureColumns] would be neglected.
    /// Applied only for string (nvarchar, char, varchar, etc) column.
    /// </summary>
    string IndexShownPictureColumns { get; set; }

    /// <summary>
    /// List of required columns. By default, any NOT NULL column except column with string data type is already required.
    /// Use this only to add NOT NULL column with string data type.
    /// <para>Syntax: ColumnName1;ColumnName2;…;ColumnNameN</para>
    /// </summary>
    string RequiredColumns { get; set; }

    /// <summary>
    /// List of limits for columns with number (or int, double, smallint, tinyint, and all alike) data type.
    /// If not specified, the engine will not limit the value which can be input by the users.
    /// <para>Syntax: ColumnName1=min:No1|max:No2;ColumnName2=min:No3;ColumnName3=max:no4;…;ColumnNameN</para>
    /// </summary>
    string NumberLimitColumns { get; set; }

    /// <summary>
    /// List of columns to be checked with regular expression. Useful for column like email.
    /// If the column checked is of [ListColumn] type, the regular expression will be applied per item line, not the entire string
    /// <para>Syntax: &lt;reg&gt;ColumnName1=regex1&lt;/reg&gt;&lt;reg&gt;ColumnName2=regex2&lt;/reg&gt;…&lt;reg&gt;ColumnNameN=regexN&lt;/reg&gt;</para>
    /// Applied only for string (nvarchar, char, varchar, etc) column.
    /// </summary>
    string RegexCheckedColumns { get; set; }

    /// <summary>
    /// List of columns to give input-with-correct-pattern-examples to the user. Useful for column like email. 
    /// To be used together with [RegexCheckedColumns].
    /// <para>Syntax: &lt;ex&gt;ColumnName1=regexExample1&lt;/ex&gt;&lt;ex&gt;ColumnName2=regexExample2&lt;/ex&gt;…&lt;ex&gt;ColumnNameN=regexExampleN&lt;/ex&gt;</para>
    /// Applied only for string (nvarchar, char, varchar, etc) column.
    /// </summary>
    string RegexCheckedColumnExamples { get; set; }

    /// <summary>
    /// To filter the table result in the index list as specified by the relationship between the table column value and the User's property.
    /// Useful, for instance, to limit access User from Team "Philippines" so that they can only see data with "Philippines" entri in the specified [ColumnName].
    /// <para>Syntax: 
    /// ColumnName1|{LeftDir:ColumnValue11,ColumnValue12}=UserTableColumnName1|{RightDir:UserTableColumnValue11,UserTableColumnValue12};
    /// ColumnName2|{LeftDir:ColumnValue21,ColumnValue22}=UserTableColumnName2|{RightDir:UserTableColumnValue21,UserTableColumnValue22};…
    /// ColumnNameN|{LeftDir:ColumnValueN1,ColumnValueN2}=UserTableColumnNameN|{RightDir:UserTableColumnValueN1,UserTableColumnValueN2}    
    /// </para>
    /// [LeftDir] = left hand side directive. Valid directive values = All (data can be seen by User with any property).
    /// [RightDir] = right hand side directive. Valid directive values = All (User can see all data).
    /// [Developer] and [Main Administrator] roles are immune from directive restrictions.
    /// </summary>
    string UserRelatedFilters { get; set; }

    /// <summary>
    /// To enable or disable filtering on the table.
    /// <para>Syntax: 1 or 0 or NULL</para>
    /// 1 means that the named table has no filter, the others mean the table has filter.
    /// </summary>
    bool? DisableFilter { get; set; }

    /// <summary>
    /// To force the columns to appear in the filter even it does not appear in the index page.
    /// <para>Syntax: ColumnName1=Role1,Role2,...;ColumnName2=Role3,Role4,...;…;ColumnNameN</para>
    /// [Role] is used to specify user roles to whom this column criteria will be applied.
    /// [Developer] and [Main Administrator] roles are immune from role restrictions.
    /// </summary>
    string ForcedFilterColumns { get; set; }

    /// <summary>
    /// Columns in the table to be excluded from the users' eyes in the index page. 
    /// Anything excluded from column will also be excluded in filter (unless it is forced by [ForcedFilterColumns]), but not in delete or details.
    /// <para>Syntax: ColumnName1=Role1,Role2,...;ColumnName2=Role3,Role4,...;…;ColumnNameN</para>
    /// [Role] is used to specify user roles to whom this column criteria will be applied.
    /// [Developer] and [Main Administrator] roles are immune from role restrictions.
    /// </summary>
    string ColumnExclusionList { get; set; }

    /// <summary>
    /// Columns in the table to be excluded from filter use.
    /// <para>Syntax: ColumnName1=Role1,Role2,...;ColumnName2=Role3,Role4,...;…;ColumnNameN</para>
    /// [Role] is used to specify user roles to whom this column criteria will be applied.
    /// [Developer] and [Main Administrator] roles are immune from role restrictions.
    /// </summary>
    string FilterExclusionList { get; set; }

    /// <summary>
    /// Columns in the table to be excluded from the details and delete actions.
    /// <para>Syntax: ColumnName1=Role1,Role2,...;ColumnName2=Role3,Role4,...;…;ColumnNameN</para>
    /// [Role] is used to specify user roles to whom this column criteria will be applied.
    /// [Developer] and [Main Administrator] roles are immune from role restrictions.
    /// </summary>
    string DetailsExclusionList { get; set; }

    /// <summary>
    /// Columns in the table to be excluded from the create and edit actions. [Cid] will always be excluded in the create and edit actions.
    /// <para>Syntax: ColumnName1=Role1,Role2,...;ColumnName2=Role3,Role4,...;…;ColumnNameN</para>
    /// [Role] is used to specify user roles to whom this column criteria will be applied.
    /// [Developer] and [Main Administrator] roles are immune from role restrictions.
    /// </summary>
    string CreateEditExclusionList { get; set; }

    /// <summary>
    /// Columns in the table to be excluded from the export-to-CSV and export-all-to-CSV default table actions.
    /// <para>Syntax: ColumnName1=Role1,Role2,...;ColumnName2=Role3,Role4,...;…;ColumnNameN</para>
    /// [Role] is used to specify user roles to whom this column criteria will be applied.
    /// [Developer] and [Main Administrator] roles are immune from role restrictions.
    /// </summary>
    string CsvExclusionList { get; set; }

    /// <summary>
    /// To specify user roles denied to access the table's data. Use special keyword "Anonymous" to deny non-logged in user to access the table's data
    /// <para>Syntax: Role1;Role2;…;RoleN</para>
    /// <para>Example: Anonymous;User;Supervisor</para>
    /// <para>Means: Non-logged in user, user having 'User' role, and user having 'Supervisor' role is denied from accessing the table's data</para>
    /// </summary>
    string AccessExclusionList { get; set; }

    /// <summary>
    /// To set criteria for coloring cell which meet the [ColoringCondition].
    /// <para>Syntax: ColoringCondition1;ColoringCondition2;…;ColoringConditionN</para>
    /// The [ColoringCondition] has the following format:
    /// <para>Format: [ColumnName|CompCode|CompExp|HTML Color]</para>
    /// <para>ColumnName = The column to be checked for coloring condition</para>
    /// <para>CompCode = Comparison code. For number and date time. Six codes are accepted:
    /// (1) GE = greater than or equal to, (2) G = greater than, (3) E = equal to, (4) L = less than, (5) LE = less than or equal to, (6) NE = not equal to. For other data types, only code (3) E and code (6) NE are valid</para>
    /// <para>CompExp = Comparison expression. Must be explicitly stated for all data types other than number or date time.
    /// For number data type, it can either be explicitly stated or using aggregate value of a column with format of
    ///  aggregateName(RefTableName:RefTableColumn:OptionalSelf+-PosNum)
    /// Acceptable aggregateName are: MIN, MAX, COUNT, SUM, and AVG
    /// OptionalSelf can be put as self which indicates cid of itself with option (+) or (-) a positive number
    ///  self must be accompanied by its own table and its own column
    /// For date time data type, the format must be M/d/yyyy HH:mm:ss according to MSDN document or using keyword NOW 
    ///  or table value reference in the form of RefTableName:RefTableColumn:OptionalSelf+-PosNum</para>
    /// <para>HTML color = HTML color code in text. For example: red or blue</para>
    /// If multiple conditions are applied to a single column, the color of the first (left-most) condition to be met will be applied
    /// </summary>
    string ColoringList { get; set; }

    /// <summary>
    /// To specify the dropdown options for the specified column for filtering. 
    /// <para>Syntax: 
    /// ColumnName1=item1,item2,[RefTableName:RefColumnName],{ASC};
    /// ColumnName2=[RefTableName:RefColumnName],{DESC};
    /// ColumnName3=item1,item2,item3,item4;…
    /// </para>
    /// The dropdown items can either be explicitly specified or taken from a column in other table with format of [RefTableName:RefColumnName]. 
    /// The directive {ASC} or {DESC} can be used to order the dropdown options ascendingly or descendingly. 
    /// If the directive is not specified, the dropdown list will be unordered.
    /// </summary>
    string FilterDropDownLists { get; set; }

    /// <summary>
    /// To specify the dropdown options for the specified column for create and edit actions. 
    /// <para>Syntax:
    /// ColumnName1=item1,item2,[RefTableName:RefColumnName],{ASC};
    /// ColumnName2=[RefTableName:RefColumnName],{DESC};
    /// ColumnName3=[RefTableName:RefColumnName:RefAnotherColumnName=ThisOtherColumnName:AddWhereClause];
    /// …;
    /// ColumnNameN=item1,item2,item3,item4;…
    /// </para>
    /// The dropdown items can either be explicitly specified or taken from a column in other table with format of [RefTableName:RefColumnName]. 
    /// The directive {ASC} or {DESC} can be used to order the dropdown options ascendingly or descendingly. 
    /// If the directive is not specified, the dropdown list will be unordered.
    /// <para>Supports LIVE dropdown filters! Supports Additional Where Clause (AddWhereClause) placed as shown: {Main Select Clause} WHERE {Main Condition} AND {Additional Where Clause}</para>
    ///   !LIMITATION!: AddWhereClause must not contain any of these characters: comma (,), semicolon (;), colon (:), or double dash (--).
    ///   The (AddWhereClause) also supports parameters (to be replaced by LIVE value on create or edit) whenever and item is written with preceding @@ symbols.
    /// </summary>
    string CreateEditDropDownLists { get; set; }

    /// <summary>
    /// To prefill the specified column with a prefix value (for ease of filling up). Only for Create row action.
    /// <para>Syntax: ColumnName1=prefix1;ColumnName2=prefix2;…;ColumnNameN=prefixN</para>
    /// </summary>
    string PrefixesOfColumns { get; set; }

    /// <summary>
    /// To prefill the specified column with a postfix value (for ease of filling up). Only for Create row action.
    /// <para>Syntax: ColumnName1=postfix1;ColumnName2=postfix2;…;ColumnNameN=postfixN</para>
    /// </summary>
    string PostfixesOfColumns { get; set; }

    /// <summary>
    /// To specify columns made of list instead of single-valued.
    /// <para>See Excel guidelines for more details</para>
    /// </summary>
    string ListColumns { get; set; }

    /// <summary>
    /// To generate system timestamp on the specified columns. 
    /// Columns specified as timestamp will generate its own value as the system's current datetime value.
    /// Only applied for DateTime column type and only applicable for Create and Edit row actions.
    /// <para>Syntax: 
    /// ColumnName1=RowAction11|TimeShiftValue11|IsFixed,RowAction12|TimeShiftValue12;
    /// ColumnName2=RowAction21|TimeShiftValue21,RowAction22;
    /// ColumnName3=RowAction31,RowAction32;…;
    /// ColumnNameN=RowActionN
    /// </para>
    /// If RowAction is not specified, then the timestamp attribute is applied to all row actions (Create and Edit).
    /// Columns specified as timestamp will NOT be displayed (and thus, cannot be filled) when the user doing the specified RowAction.
    /// IsFixed can only receive true or false. By default, true is chosen. 
    /// A fixed timestamp column's value will not be changed when the specified row action is performed.
    /// TimeShiftValue is in the format of +IntergerNumber (like +7200) or -IntegerNumber (like -10800).
    /// TimeShiftValue is used to create a value shift in seconds from the current timestamp.
    /// </summary>
    string TimeStampColumns { get; set; }

    /// <summary>
    /// To specify table for historical record of this table. To be used together with HistoryTrigger column.
    /// <para>Syntax:
    /// HistoryTableName or 
    /// HistoryTableName=ColumnName1,ColumnName2,…,ColumnNameN or
    /// HistoryTableName=HTableHRTSColumnName:AUTO-GENERATED or
    /// HistoryTableName=ColumnName1:HTableColumnName1,HTableHRTSColumnName:AUTO-GENERATED,ColumnName3:HTableColumnName3,ColumnName4,…,ColumnNameN
    /// </para>
    /// The table for historical record (so-called H-Table) must always have one column for historical record time stamp (so called HRTS). 
    /// By default, the name of the column is HRTS.
    /// If the H-Table has different HRTS column name than the default name (HRTS), then HTableHRTSColumnName:AUTO-GENERATED must be written with HTableHRTSColumnName being the actual HRTS column name in the H-Table.    
    /// <para>
    /// To transfer all columns from original table (so called O-Table) to the H-Table, simply put the HistoryTableName
    /// To transfer only some columns to the historical table, specify the ColumnName(s) you want to transfer to the historical table.
    /// To transfer the column value from the O-Table to column in H-Table with different name, HTableColumnName must be specify.
    ///  Else, the engine will assume O-Table's column namd and H-Table's column name are the same.
    /// </para>
    /// Example 1: JobOrderHistory
    /// Means: data from all columns of this table will be transferred to JobOrderHistory which has default-named HRTS column.
    /// <para>
    /// Example 2: JobOrderHistory=CustomHRTS:AUTO-GENERATED
    /// Means: data from all columns of this table will be transferred to JobOrderHistory which has custom-named HRTS column CustomHRTS.
    /// </para>
    /// Example 3: JobOrderHistory=Personel,TaskCompleted,TaskWorth:Score,CustomHRTS:AUTO-GENERATED
    /// Means: 
    /// - transfer only Personel, TaskCompleted, and TaskWorth columns from this table to the H-Table.
    /// - Personel and TaskCompleted column values are to be transferred to the H-table columns with the same name.
    /// - TaskWorth column is to be transferred to H-Table column named Score.
    /// - H-Table HRTS column name is CustomHRTS, not the original name HRTS.
    /// </summary>
    string HistoryTable { get; set; }

    /// <summary>
    /// To be used to specify the triggering conditions to either move or copy this table's data to the specified historical table (H-Table). Note that multiple triggering conditions can be applied for the table. To be used together with HistoryTable column.
    /// 
    /// <para>Syntax: 
    /// TriggerConditionInSQLScript (TC-SQLS)
    /// </para>
    /// The TC-SQLS takes form of the typical SQL script with the following rules:
    /// 1. static value is written as it is: string written in between two apostrophes, numbers written as they are, apostrophe in string value is replaced with double apostrophes.
    /// 2. some symbols are forbidden: double dash (--), semicolon (;)
    /// <para>
    /// IsDataDeleted is either True or False. True value will make the original data deleted (moved to history table) when triggered. False value will make the original data remain (copied to history table) when triggered. The default value for IsDataDeleted is True.
    /// MustEditHaveChange is either True of False. True value will make the trigger applied in edit only when there is a change in the edit value. False value will make the trigger applied in the edit without any change in the edit value. The default value for MustEditHaveChange is True.
    /// </para>
    /// <para>Example: 
    /// True,False|Edit=(TaskCompleted=1 AND TaskStatus='Completed') OR (ScheduledTime > DeadlineTime); False|Create=((FinalScore > InitialScore + 50) AND PersonelName = 'O''Neil')
    /// </para>
    /// <para>
    /// Means:
    /// - On edit, the record of this table will be moved to the H-Table (and then deleted from this table upon successful transfer) on one of the following conditions:
    ///    1. TaskCompleted column value is 1 and TaskStatus column value is (literally) Completed
    ///    2. ScheduledTime column value is greater than the DeadlineTime column value
    /// - On create, the record of this table will be copied to the H-Table (not deleted from this table) on the following condition:
    ///    1. FinalScore column value is greater by more than 50 from InitialScore value and the PersonelName column value is O'Neil
    /// - Since MustEditHaveChange is set to False, therefore, on edit action, the history trigger will be applied even when there is no change in the edit value    
    /// </para>
    /// The TC-SQLS in HistoryTrigger is only checked after finishing default Action, Edit, or Delete row actions, depending on the RowAction specified. If no row action is specified, the trigger will be applied to all actions.
    /// TC-SQLS in HistoryTrigger is only checked against the current item (that is, the added, edited, or deleted item). Other items in the table which satisfy the TC-SQLS will not be affected.
    /// </summary>
    string HistoryTriggers { get; set; }

    /// <summary>
    /// To generate automatic id-like number increment for the specified columns.
    /// Only applied for number types (int, uint, short, ushort, decimal, float, etc) columns, and only applied to Create row action.
    /// <para>Syntax: 
    /// ColumnName1=OtherTableName11:OtherColumnName11,OtherTableName12:OtherColumnName12,…;
    /// ColumnName2=OtherTableName21:OtherColumnName22;…;
    /// ColumnNameN
    /// </para>
    /// If the auto-generated column is only self-referenced, use only the ColumnName format without OtherTableName:OtherColumnName.
    /// For shared id among multiple table columns, use ColumnName=OtherTableName1:OtherColumnName1,...,OtherTableNameN:OtherColumnNameN.
    /// </summary>
    string AutoGeneratedColumns { get; set; }

    /// <summary>
    /// To rearrange the column sequence displayed different than the sequence in the table. 
    /// The rearrangement is applied to all basic actions: Index, Filter, Create, Edit, Details, and Delete.
    /// <para> Syntax:
    /// ColumnName1;ColumnName2;…;ColumnNameN
    /// </para>
    /// If only some columns are rearranged, the rearranged columns will be put first and the remaining columns are arranged with their default arrangement.
    /// The default arrangement is the same arrangement as the table columns arrangement.
    /// </summary>
    string ColumnSequence { get; set; }

    /// <summary>
    /// To rearrange the column sequence displayed different than the sequence in the table. 
    /// The rearrangement is applied to all basic actions: Index, Filter, Create, Edit, Details, and Delete.
    /// <para> Syntax: 
    /// ColumnName1=ColumnName1Alias;ColumnName2=ColumnName2Alias;…;ColumnNameN=ColumnNameNAlias
    /// </para>
    /// If only some columns are rearranged, the rearranged columns will be put first and the remaining columns are arranged with their default arrangement.
    /// The default arrangement is the same arrangement as the table columns arrangement.
    /// </summary>
    string ColumnAliases { get; set; }

    /// <summary>
    /// To list the columns to be shown, but not edited, in Edit row action.
    /// <para> Syntax:
    /// ColumnName1;ColumnName2;…;ColumnNameN
    /// </para>
    /// </summary>
    string EditShowOnlyColumns { get; set; }

    /// <summary>
    /// !ATTENTION:The number of columns returned, its sequence, and its column names must be consistent with what is provided in ScriptColumns!
    /// <para>To provide constructor for script columns.</para>
    /// <para> Syntax:
    /// ColumnName1|AttrName1A:AttrVal1A|AttrName1B:AttrVal1B|…|AttrName1Z:AttrVal1Z=Script1;ColumnName2=Script2;…;ColumnNameN=ScriptN
    /// </para>
    /// [Script] part is simply SQL script.
    /// The [Script] part must not contain semicolon (;) symbol.
    /// [Script] part should not contain where clause.
    /// Skillfully use AS SQL keyword to display the returned table's column names differently than the table's column name.
    ///   Example: "select MyNumber as No, Cid as UniqueNo from MyTable" will show No and Unique No as table column names in the returned table.
    ///   You may also change the sequence of the table columns returned by using select statement just like in normal SQL script.
    ///   Example: "select MyNumber,Cid, SimpleName from MyTable" will have reversed result from "select SimpleName, Cid, MyNumber from MyTable".
    /// If the [Script] returns picture-type column, make sure that RefTableName attribute is provided and [Cid] column (of the referenced table) must be one of the returned columns.
    /// 
    /// <para>Allowed AttrName:</para>
    /// <para>- PictureLinks. To list the column names from table taken shown as picture rather than text. Must be present for picture column to be shown.
    ///     Syntax: PictureLinks:PicColumnName1#PicWidth1;PicColumnName2;...;PicColumnNameN
    ///     Example: PictureLinks:EmployeePhoto#300;EmployeeSignature;EmployeeThumbPrint#80
    ///     Means: there are three picture columns from the returned table, namely EmployeePhoto, EmployeeSignature, and EmployeeThumbPrint. EmployeePhoto should have picture with of 300px, EmployeeThumbPrint 80px, and EmployeeSignature uses default picture width (100px)</para>
    /// <para>- RefTableName. To indicate that the script takes data exclusively from single table. Must be present for picture column to be shown.
    ///     Syntax: RefTableName:MyRefTableName. 
    ///     Example: RefTableName:CustomerTable. 
    ///     Means: the data for this column is specifically taken from a single table (or view) called CustomerTable (not from complex SQL scripting)</para>
    /// </summary>
    string ScriptConstructorColumns { get; set; }

    /// <summary>
    /// !ATTENTION:The number of columns returned, its sequence, and its column names must be consistent with what is provided in ScriptConstructorColumns!
    /// <para>To provide data from other data table as the column data.</para>
    /// <para> Syntax:
    /// ColumnName1=Script1;ColumnName2=Script2;…;ColumnNameN=ScriptN
    /// </para>
    /// [Script] part is simply SQL script with flexible parameterized input(s) using symbol @@.
    /// The [Script] part must not contain semicolon (;) symbol.
    /// If where clause is used, it may use dynamically assigned values from the current table's column by using prefix @@ before the current table's column.
    ///   The parameterized value (using prefix @@) is not limited to where clause though, it can be in any part of the SQL [Script].
    /// <para>This column is read-only, show only the constructor in the Create row action, and cannot be shown in the index.
    /// Filter always exclude columns used as ScriptColumn.
    /// The resulting table returned by the script cannot contain column which is of ScriptColumn type (does not support multi-level ScriptColumn).
    /// In order to show picture column, RefTableName attribute must be provided in its ScriptConstructorColumn counterpart and [Cid] must be one of the columns which the table returned.
    /// </para>
    /// </summary>
    string ScriptColumns { get; set; }

    /// <summary>
    /// To specify date time format for specific columns. 
    /// <para> Syntax:
    /// ColumnName1=PageName11|DateTimeFormat11|PageName12|DateTimeFormat12;
    /// ColumnName2=PageName21|DateTimeFormat21;…;
    /// ColumnNameN1=PageNameN1|DateTimeFormatN1
    /// </para>
    /// [PageName] has four possible values:
    /// - Index: to specify date time format for Index page.
    /// - CreateEditFilter: to specify date time format for Create, Edit, and Filter pages. Due to website browser's constraint, not applicable for Aiwe.
    /// - Details: to specify date time format for Details and Delete pages.
    /// <para>
    /// Example: ScheduledTime=Index|dd-MM-yy HH:mm:ss|Details|dd-MMMM-yyyy HH:mm:ss;RunTime=CreateEditFilter|dd/MMM/yyyy
    /// </para>
    /// <para>
    /// Means:
    /// - The two DateTime columns, ScheduledTime and RunTime use customized DateTime formats. The rests of the DateTime columns of this table use default DateTime format.
    /// - ScheduledTime column use "dd-MM-yy HH:mm:ss" format for Index page and "dd-MMMM-yyyy HH:mm:ss" format for Details and Delete pages. It uses default DateTime format for other pages which are not specified (Create, Edit, and Filter)
    /// - RunTime column use "dd/MMM/yyyy" format for Create, Edit, and Filter pages. It uses default DateTime format for other pages which are not specified (Index, Details, and Delete)
    /// </para>
    /// </summary>
    string CustomDateTimeFormatColumns { get; set; }

    /// <summary>
    /// To be used to specify the triggering condition to create email based on TC-SQLS given. This column is to be used together with EmailMakers column. EmailMakerTrigger's TriggerName which does not come in pair with (or in other words, not duplicated in) EmailMaker's TriggerName is not valid. In addition, the CoreEmailInfo and CoreEmailTemplate tables must be present in the database.
    /// <para>
    /// The TC-SQLS in EmailTrigger is only checked after finishing default Action, Edit, or Delete row actions, depending on the RowAction specified. 
    /// If no row action is specified, the trigger will be applied to all actions.
    /// </para>
    /// TC-SQLS in EmailMakerTrigger is only checked against the current item (that is, the added, edited, or deleted item). 
    /// Other items in the table which satisfy the TC-SQLS will not be affected. 
    /// When EmailMakerTrigger is applied on Edit action, it will only take effect when there is a changed in the edited item.
    /// <para>See Excel guidelines for more details</para>
    /// </summary>
    string EmailMakerTriggers { get; set; }

    /// <summary>
    /// To be used to find email template and to get the email rules for the given trigger. This column is to be used together with EmailMakerTriggers column. EmailMaker's TriggerName which does not come in pair with (or in other words, not duplicated in) EmailMakerTrigger's TriggerName is not valid. In addition, the CoreEmailInfo and CoreEmailTemplate tables must be present in the database.
    /// <para>
    /// When an EmailMaker is triggered, a new row entry in the CoreEmailInfo will be created with the following information:
    /// 1. The email template will be taken from CoreEmailTemplate and be placed to the CoreEmailInfo
    /// 2. Info from the this table's row entry which caused the trigger will be put as parameters in the EmailParameters column of the CoreEmailInfo table
    /// 3. Info of the current user will be put as parameters in the EmailParameters column of the CoreEmailInfo table    
    /// </para>
    /// <para>See Excel guidelines for more details</para>
    ///</summary>
    string EmailMakers { get; set; }

    /// <summary>
    /// To upload attachment, instead of normal string, for data input control. Applied only for string (nvarchar, char, varchar, etc) column.
    /// Column containing attachment cannot be used for filtering and will not be shown in the index.
    /// <para> Syntax:
    /// ColumnName1=format1,format2,...;ColumnName2;…;ColumnNameN
    /// </para>
    /// Use formats to restrict the attachments to certain formats. Otherwise, the attachment can be of any format.
    /// </summary>
    string NonPictureAttachmentColumns { get; set; }

    /// <summary>
    /// To indicate list of attachment columns which can be downloaded.
    /// <para> Syntax:
    /// ColumnName1;ColumnName2;…;ColumnNameN
    /// </para>
    /// </summary>
    string DownloadColumns { get; set; }

    /// <summary>
    /// To be used to specify triggering conditions for (SQL) PreActionProcedures - that is, Stored Procedures to be run before a default row action is performed. 
    /// This column is to be used together with PreActionProcedures column. 
    /// PreActionTrigger's TriggerName which does not come in pair with (or in other words, not duplicated in) PreActionProcedure's TriggerName is not valid.     
    /// <para> Syntax:
    /// TriggerName1;
    /// TriggerName2=TriggerConditionInSQLScript (TC-SQLS)-2;
    /// TriggerName3|RowAction31,RowAction32,…=TC-SQLS-3;
    /// TriggerName4|RowAction41,RowAction42,…|MustEditHaveChange=TC-SQLS-4;…;
    /// TriggerNameN=TC-SQLS-N
    /// </para>
    /// Read "TC-SQLS" tab for more info about TC-SQLS. 
    /// The TC-SQLS in PreActionTrigger is checked before executing default Create, Edit, or Delete row actions, depending on the RowAction specified. 
    /// If no row action is specified, the trigger will be applied to all actions. 
    /// If TC-SQLS is not specified, then the trigger is applied to (always True for) all specified row actions 
    /// - the procedures with the same TriggerName will always be triggered for the applied for all applied row actions.   
    /// <para>
    /// MustEditHaveChange is either True of False. True value will make the trigger applied in edit only when there is a change in the edit value. 
    /// False value will make the trigger applied in the edit without any change in the edit value. The default value for MustEditHaveChange is True.
    /// </para>
    /// TC-SQLS in PreActionTrigger is only checked against the current item (that is, the added, edited, or deleted item). 
    /// Other items in the table which satisfy the TC-SQLS will not be affected. 
    /// <para> 
    /// Example (to be taken together with PreActionProcedures example): 
    /// PreActionCreateTrigger|Create=TaskStatus='Started';PreActionEditTrigger|Edit|False=TaskStatus=JobStatus;PreActionDeleteTrigger|Delete
    /// </para>
    /// Means:
    /// - There are three valid TriggerNames, namely "PreActionCreateTrigger", "PreActionEditTrigger", and "PreActionDeleteTrigger".
    /// - The "PreActionCreateTrigger" condition is TaskStatus column value is (literally) Started.
    /// - The "PreActionEditTrigger" condition is TaskStatus column value is the same as JobStatus column value.
    /// - The "PreActionDeleteTrigger" has no condition. 
    /// Thus, the respective procedure having TriggerName "PreActionDeleteTrigger" will ALWAYS be run before executing default delete row action.
    /// </summary>
    string PreActionTriggers { get; set; }

    /// <summary>
    /// To execute a Stored Procedure before a default row action is performed. This column is to be used together with PreActionTriggers column.
    /// <para> Syntax:
    /// TriggerName1=StoredProcedure1;
    /// TriggerName2=StoredProcedure2(@SpPar21=Par21,@SpPar22=Par22,…);
    /// TriggerName3=USER:StoredProcedure3;
    /// TriggerName4=USER:StoredProcedure4(@SpPar41=Par21,@SpPar42=Par42,…);…;
    /// TriggerNameN=StoredProcedureN
    /// </para>
    /// By default, the Stored Procedure is run from the Data database. 
    /// To run the Stored Procedure from the User database, the special prefix "USER:" must be used as a prefix before the Stored Procedure name.
    /// @SpPar(s) is(are) the Store Procedure parameter names, its name must match exactly with the input parameter names of the called Stored Procedure.
    /// <para>
    /// Parameters (Par) are to be written with using with initial @@ symbol. 
    /// Static values are written as the normal the SQL script (nvarchar is written between two aposthropes (i.e. 'static value'), numbers are written without aposthrope (i.e. 750).
    /// Only input parameters are supported for the parameters of the Stored Procedure.
    /// </para>
    /// The Stored Procedure for:
    /// - Create row action will be executed before the data is created.Parameters from Data will NOT be available.
    /// - Edit row action will be executed before the data is changed/updated.Parameters from Data will use the data before it is changed.
    /// - Delete row action will be executed before the data is deleted.Parameters from Data will use the data before it is deleted.
    /// <para> 
    /// Example (to be taken together with PreActionTriggers example):
    /// PreActionCreateTrigger=SpRecordStart;
    /// PreActionEditTrigger=SpRecordEdit(@UserId=@@User.UserName, @PurchasedItem=@@Data.ItemName, @Explanation='StaticParameter', @ItemCount=5);
    /// PreActionDeleteTrigger=SpRecordParameterless;
    /// PreActionInvalidTrigger=SpRecordNonExisting(@NonExistingPar=@@Data.NonExistingItem)
    /// </para>
    /// Means:
    /// - There are three valid TriggerNames, namely "PreActionCreateTrigger", "PreActionEditTrigger", and "PreActionDeleteTrigger". The "PreActionInvalidTrigger" is invalid since there is no trigger with the same name in PreActionTriggers.
    /// - The "PreActionCreateTrigger" runs SpRecordStart Stored Procedure (SP). SpRecordStart SP is a parameterless SP.
    /// - The "PreActionEditTrigger" runs SpRecordEdit SP. The SpRecordEdit SP has four parameters, namely @UserId, @PurchasedItem, @Explanation, @ItemCount. The four SP parameters are obtained in the following way:
    ///   (1) @UserId is obtained from @@Data.ItemName. @@Data.ItemName takes the current [data] value in the ItemName column. 
    ///   (2) @PurchasedItem is obtained from @@User.UserName. @@User.UserName takes the current [user] value in the UserName column.
    ///   (3) @Explanation takes the value of 'StaticParameter' which is a static string (nvarchar) parameter.
    ///   (4) @ItemCount takes the value of 5 which is a static number parameter.
    /// - The "PreActionDeleteTrigger" runs SpRecordParameterless SP. SpRecordParameterless SP is also a parameterless SP.
    /// </summary>
    string PreActionProcedures { get; set; }

    /// <summary>
    /// To be used to specify triggering conditions for (SQL) PostActionProcedures - that is, Stored Procedures to be run after a default row action is performed. 
    /// This column is to be used together with PostActionProcedures column. 
    /// PostActionTrigger's TriggerName which does not come in pair with (or in other words, not duplicated in) PostActionProcedure's TriggerName is not valid
    /// <para> Syntax:
    /// TriggerName1;
    /// TriggerName2=TriggerConditionInSQLScript (TC-SQLS)-2;
    /// TriggerName3|RowAction31,RowAction32,…=TC-SQLS-3;
    /// TriggerName4|RowAction41,RowAction42,…|MustEditHaveChange=TC-SQLS-4;…;
    /// TriggerNameN=TC-SQLS-N
    /// </para>
    /// Read "TC-SQLS" tab for more info about TC-SQLS. 
    /// The TC-SQLS in PostActionTrigger is only checked after finishing default Action, Edit, or Delete row actions, depending on the RowAction specified. 
    /// If no row action is specified, the trigger will be applied to all actions.
    /// If TC-SQLS is not specified, then the trigger is applied to (always True for) all specified row actions 
    /// - the procedures with the same TriggerName will always be triggered for the applied for all applied row actions.
    /// <para>
    /// MustEditHaveChange is either True of False. True value will make the trigger applied in edit only when there is a change in the edit value. 
    /// False value will make the trigger applied in the edit without any change in the edit value. The default value for MustEditHaveChange is True.
    /// </para>
    /// TC-SQLS in PostActionTrigger is only checked against the current item (that is, the added, edited, or deleted item). 
    /// Other items in the table which satisfy the TC-SQLS will not be affected. 
    /// <para> 
    /// Example (to be taken together with PostActionProcedures example): 
    /// PostActionCreateTrigger|Create=TaskStatus='Started';PostActionEditTrigger|Edit|False=TaskStatus=JobStatus;PostActionDeleteTrigger|Delete
    /// </para>
    /// Means:
    /// - There are three valid TriggerNames, namely "PostActionCreateTrigger", "PostActionEditTrigger", and "PostActionDeleteTrigger".
    /// - The "PostActionCreateTrigger" condition is TaskStatus column value is (literally) Started.
    /// - The "PostActionEditTrigger" condition is TaskStatus column value is the same as JobStatus column value.    
    /// - The "PostActionDeleteTrigger" has no condition. Thus, the respective procedure having TriggerName "PostActionDeleteTrigger" will ALWAYS be run after executing default delete row action.
    /// </summary>
    string PostActionTriggers { get; set; }

    /// <summary>
    /// To execute a Stored Procedure before a default row action is performed. 
    /// This column is to be used together with PostActionTriggers column.
    /// <para> Syntax:
    /// TriggerName1=StoredProcedure1;
    /// TriggerName2=StoredProcedure2(Par21,Par22,…);
    /// TriggerName3=USER:StoredProcedure3;
    /// TriggerName4=USER:StoredProcedure4(Par41,Par42,…);…;
    /// TriggerNameN=StoredProcedureN
    /// </para>
    /// By default, the Stored Procedure is run from the Data database. 
    /// To run the Stored Procedure from the User database, the special prefix "USER:" must be used as a prefix before the Stored Procedure name.
    /// @SpPar(s) is(are) the Store Procedure parameter names, its name must match exactly with the input parameter names of the called Stored Procedure.
    /// <para>
    /// Parameters (Par) are to be written with using with initial @@ symbol. 
    /// Static values are written as the normal the SQL script (nvarchar is written between two aposthropes (i.e. 'static value'), numbers are written without aposthrope (i.e. 750).
    /// Only input parameters are supported for the parameters of the Stored Procedure.
    /// </para>
    /// The Stored Procedure for:
    /// - Create row action will be executed after the data is created. Parameters from Data will be available.
    /// - Edit row action will be executed after the data is changed/updated. Parameters from Data will use the data after it is changed.
    /// - Delete row action will be executed after the data is deleted. Parameters from Data will NOT be available.
    /// <para>
    /// Example (to be taken together with PostActionTriggers example):
    /// PostActionCreateTrigger=SpRecordStart;
    /// PostActionEditTrigger=SpRecordEdit(@UserId=@@User.UserName, @PurchasedItem=@@Data.ItemName, @Explanation='StaticParameter', @ItemCount=5);
    /// PostActionDeleteTrigger=SpRecordParameterless;
    /// PostActionInvalidTrigger=SpRecordNonExisting(@NonExistingPar=@@Data.NonExistingItem)
    /// </para>
    /// Means:
    /// - There are three valid TriggerNames, namely "PostActionCreateTrigger", "PostActionEditTrigger", and "PostActionDeleteTrigger". The "PostActionInvalidTrigger" is invalid since there is no trigger with the same name in PostActionTriggers
    /// - The "PostActionCreateTrigger" runs SpRecordStart Stored Procedure (SP). SpRecordStart SP is a parameterless SP.
    /// - The "PostActionEditTrigger" runs SpRecordEdit SP. The SpRecordEdit SP has four parameters, namely @UserId, @PurchasedItem, @Explanation, @ItemCount. The four SP parameters are obtained in the following way:
    ///   (1) @UserId is obtained from @@Data.ItemName. @@Data.ItemName takes the current [data] value in the ItemName column. 
    ///   (2) @PurchasedItem is obtained from @@User.UserName. @@User.UserName takes the current [user] value in the UserName column.
    ///   (3) @Explanation takes the value of 'StaticParameter' which is a static string (nvarchar) parameter.
    ///   (4) @ItemCount takes the value of 5 which is a static number parameter.
    /// - The "PostActionDeleteTrigger" runs SpRecordParameterless SP. SpRecordParameterless SP is also a parameterless SP.
    /// </summary>
    string PostActionProcedures { get; set; }

    /// <summary>
    /// To determine the Table type for this Table. The TableType will make the table display and behave differently. There are two valid TableTypes:
    /// (1) Normal: No need to be written. The Normal TableType is the default Table type.
    /// (2) Group: To be used together with AggregationStatement column. To show aggregation of a group of items per single row in Index View, instead of single item per single row in Index View, based on AggregationStatement.
    /// </summary>
    string TableType { get; set; }

    /// <summary>
    /// Group Table Type only. To specify the aggregation statement used for generating Index View of the table.
    /// <para> Syntax:
    /// GroupByColumn11=AggregationQuerySQLScript (AQ-SQLS)-1 or
    /// GroupByColumn21,GroupByColumn22,…=AQ-SQLS-2 or
    /// GroupByColumn31:AutoDirective31,GroupByColumn32:AutoDirective32,…=AQ-SQLS-3 
    /// </para>
    /// Support additional CreateGroup, DeleteGroup, and GroupDetails row actions and Finish table action.
    /// - CreateGroup will require the user to put the new value for the non-AUTO Group By columns, and then to create multiple items at the same time.
    /// - DeleteGroup will allow the user to delete all items in the same group at the same time.
    /// - GroupDetails will show the user the Index View for all the items in the group, which can access all the normal Create, Edit, Delete, Details, and DownloadAttachments row actions, plus additional Finish table action to return to the page where the group details is called.    
    /// <para>
    /// The left side of the equal mark is to be populated with list of "Group By" columns as in the normal SQL. Each group by column can be given an AutoDirective to indicate that the group by column values are to be created automatically. There are two valid AutoDirective value:
    /// - AUTO-INT, used to create the automatically increased number for Group Table
    /// - AUTO-DATETIME, used to create the automatic date and time values for the Group Table
    /// </para>
    /// Group by columns can either be created manually, partially-automatically, or fully-automatically. 
    /// When created manually or partially-automatically, the GroupBy-Form to fill in the manual items shall be shown and be filled to continue 
    ///  to make item in the group one by one in the GroupDetails View. 
    /// When created automatically, the GroupBy-Form filling shall be skipped. 
    /// <para>
    /// The right side of the equal mark is to be written using Aggregation Query SQL Script (AQ-SQLS) which are normal SQL script used for 
    ///  querying using aggregation functions (such as SUM(), COUNT(), MAX(), MIN(), AVG(), and so on...) and the columns used in Group By portions, just like normal SQL script.
    /// </para>
    /// Example: ProductTypeId:AUTO-INT, ProductCode=ProductTypeId, ProductCode, sum(Cid) as SumCid, Count(ProductSeriesNo) as CountProductSeriesNo
    /// <para>
    /// Means:
    /// - This table is grouped by ProductTypeId and ProductCode.
    /// - The ProductTypeId value will be generated automatically to create the Group Table.
    /// - The ProductCode value must be inputed manually for the Group Table.
    /// - The Index View takes the ProductTypeId, ProductCode, Sum result of Cid column, and Count result of ProductSeriesNo.
    /// - The Sum(Cid) is given alias of SumCid in the view, while Count(ProductSeriesNo) is given alias CountProductSeriesNo.
    /// </para>
    /// </summary>
    string AggregationStatement { get; set; }

    /// <summary>
    /// To specify columns which represent foreign keys to other table's columns and to display (not to edit or to delete) the other table's data columns. 
    /// <para> Syntax:
    /// ColumnName1=RefTableName1:ForeignKeyColumnName1;
    /// ColumnName2=RefTableName2:ForeignKeyColumnName2:RefColumnName21,RefColumnName22,…,RefColumnName2N
    /// </para>
    /// If not specified, all other table's column data will be displayed except the foreign key column value itself. 
    /// To select some other table's data columns to be displayed, specify the RefColumnName(s) of the other table's columns.
    /// <para> Example:
    /// ProductId=TBL_PRODUCT_DESC:Id:ProductCode,ProductType,ProductDesc
    /// </para>
    /// Means:
    /// - The ProductId column is a foreign key to TBL_PRODUCT_DESC table, Id column.
    /// - Takes ProductCode, ProductType, and ProductDesc column values from TBL_PRODUCT_DESC to be displayed.
    /// </summary>
    string ForeignInfoColumns { get; set; }
  }
}
