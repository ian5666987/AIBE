using Aibe.Helpers;
using Aibe.Models.Core;
using Extension.Database.SqlServer;
using Extension.Extractor;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Aibe.Models {
  public class BaseFilterIndexModel {
    protected MetaInfo Meta { get; set; }
    protected int UsedPage { get; set; }
    protected DateTime Now { get; set; }
    protected List<SqlParameter> Pars { get; set; }
    protected List<SqlParameter> CopiesPars { get; set; }
    protected StringBuilder SelectScript { get; set; }
    public StringBuilder QueryScript { get; protected set; }
    public Dictionary<string, string> StringDictionary { get; protected set; }
    public int FilterNo { get; protected set; }
    public string FilterMsg { get; protected set; }
    public DataTable Data { get; protected set; }
    public NavDataModel NavData { get; protected set; }
    public DataTable Table { get; private set; }
    public bool HasTable { get { return Table != null; } }
    protected bool _hasAutoWhere; //to speed up the evaluation, gives internal flag
    public bool HasWhere { get { return _hasAutoWhere || FilterNo > 0 || QueryScript.ToString().ToUpper().Contains(" WHERE "); } }

    public BaseFilterIndexModel(MetaInfo meta, int? page, Dictionary<string, string> dictCollections) {
      Meta = meta;
      UsedPage = page.HasValue && page.Value > 0 ? page.Value : 1;

      //Script initialization
      SelectScript = new StringBuilder("SELECT");
      QueryScript = new StringBuilder(string.Concat("FROM [", meta.TableSource, "]")); //TableSource

      //Auto-filtering parts, parameterless, caused by prefilled columns
      if (meta.HasPrefilledColumn)
        foreach(PrefilledColumnInfo pcInfo in meta.PrefilledColumns) {
          QueryScript.Append(_hasAutoWhere ? " AND " : " WHERE ");
          QueryScript.Append(pcInfo.Name);
          QueryScript.Append("=");
          QueryScript.Append(pcInfo.RightSide); //use the original right-side, rather than the value, since the original right-side must already be defined correctly
          _hasAutoWhere = true; //to tell the subsequent lines that "WHERE" keyword already exists
        }

      //Filtering parts started
      if (dictCollections != null && dictCollections.Count > 0) {
        Now = DateTime.Now;
        Pars = new List<SqlParameter>();
        CopiesPars = new List<SqlParameter>(); //WARNING! if not copied, the complete script cannot be run as the Sql Pars have been used by this countScript
        StringDictionary = new Dictionary<string, string>();

        //Apply filter on query script here...
        FilterNo = AddFiltersOnScript(dictCollections);

        if (FilterNo > 0) {
          _hasAutoWhere = true;
          int count = 0;
          foreach (var filterMsg in StringDictionary) { 
            if (count > 0)
              FilterMsg += Environment.NewLine;
            FilterMsg += filterMsg.Key + ": " + filterMsg.Value; //TODO may not be a good idea to use the filter.Key directly, may instead use the translation
            count++;
          }
        }
      }
    }

    protected int AddFiltersOnScript(Dictionary<string, string> collections) {
      //Filters
      var filteredKeys = collections.Keys.Except(DH.ExemptedFilterFormCollection);
      int filterNo = 0;
      var completeKeyInfo = KeyInfoHelper.GetCompleteKeyInfo(Meta.TableSource, collections, filteredKeys, filterStyle: true);

      foreach (var validKeyInfo in completeKeyInfo.ValidKeys) {
        object value = validKeyInfo.ExtractValueAsObject(collections, refDtNow: Now, filterStyle: true);
        if (value != null) {
          object valueObject = validKeyInfo.CreateQueryValueAsObject(value);
          string querySubstring = validKeyInfo.CreateQuerySubstring(filterNo);
          AddQueryParameter(querySubstring, valueObject, validKeyInfo.Key, value.ToString(), ref filterNo);
          StringDictionary.Add(validKeyInfo.Key, value.ToString());
        }
      }

      foreach (var nullifiedKeyInfo in completeKeyInfo.NullifiedKeys) {
        //TODO as of now, no need to to anything, but might probably be needed for data type like boolean
      }

      return filterNo;
    }

    protected void AddQueryParameter(string querySubstring, object val, string key, string valueStr, ref int filterNo) {
      QueryScript.Append(HasWhere ? " AND " : " WHERE ");
      QueryScript.Append(querySubstring);
      string parName = string.Concat("@par" + filterNo);
      SqlParameter par = new SqlParameter(parName, val);
      SqlParameter copyPar = new SqlParameter(parName, val);
      Pars.Add(par);
      CopiesPars.Add(copyPar);
      filterNo++;
    }

    protected void InternalProcessCompleteModelAndData(SqlConnection conn, List<SqlParameter> copiesPars, 
      bool isGrouping, bool loadAllData = false) {
      //Finalize
      //Counting filtered/non-filtered results
      string countScript = string.Concat(SelectScript, " count(*) as CountNo ", QueryScript);
      DataTable countDataTable = SQLServerHandler.GetDataTable(conn, countScript, copiesPars);
      int queryCount = 0;
      if (countDataTable != null && countDataTable.Rows != null && countDataTable.Rows.Count > 0)
        queryCount = isGrouping ? countDataTable.Rows.Count : (int)countDataTable.Rows[0]["CountNo"];

      //Order by      
      bool hasOrderBy = false;
      bool hasOrderByCid = false;
      bool skipOrderByCid = false;
      if (Meta.OrderBys != null && Meta.OrderBys.Count > 0) {
        int orderCount = 0;
        OrderByInfo firstOrderByInfo = Meta.OrderBys[0];
        if (firstOrderByInfo.IsScript) {
          hasOrderBy = true; //always true, creates the script immediately, then do nothing else
          skipOrderByCid = true;
          QueryScript.Append(string.Concat(" ORDER BY (", firstOrderByInfo.Script, ")"));
        } else
          foreach (var orderBy in Meta.OrderBys) {
            QueryScript.Append(string.Concat(orderCount == 0 ? " ORDER BY " : ", ", "[", orderBy.Name, "] ",
              orderBy.GetOrderDirection())); //only the first one requires [order by] phrase
            hasOrderBy = true;
            hasOrderByCid = orderBy.Name.EqualsIgnoreCaseTrim(DH.Cid);
            orderCount++;
          }
      }

      if (!hasOrderBy) {//there must be at least one, as long as it is not a group by
        if (!isGrouping)
          QueryScript.Append(" ORDER BY [" + DH.Cid + "] ASC ");
      } else if (!hasOrderByCid && !skipOrderByCid) { //If it has order by but not having Cid, add the order by [Cid] at the very last
        QueryScript.Append(", [" + DH.Cid + "] ASC ");
      }

      //Preparing page navigation model
      NavData = new NavDataModel(UsedPage, Meta.ItemsPerPage, queryCount);
      if (!loadAllData) {  //only if we do not load all data that we have this query, otherwise, query everything!
                           //navData.ParentPage = Request.Url.AbsoluteUri.Split('?')[0]; //take everything before the first '?' //not used at all
        int skippedItemNo = (NavData.CurrentPage - 1) * Meta.ItemsPerPage; //current page in the Nav Data is controlled within the possible range
        if (skippedItemNo == 0) {
          SelectScript.Append(string.Concat(" TOP ", Meta.ItemsPerPage));
        } else {
          QueryScript.Append(string.Concat(" OFFSET (", skippedItemNo, ") ROWS FETCH NEXT (", Meta.ItemsPerPage, ") ROWS ONLY"));
        }
      }
    }

    public virtual void CompleteModelAndData(bool isGrouping, bool loadAllData = false) { //loadAllData is added to give a way to do so when needed
      using (SqlConnection conn = new SqlConnection(DH.DataDBConnectionString)) {
        conn.Open();
        InternalProcessCompleteModelAndData(conn, CopiesPars, isGrouping, loadAllData);
        string completeScript = string.Concat(SelectScript, " * ", QueryScript); //form the complete script here
        Data = SQLServerHandler.GetDataTable(conn, completeScript, Pars);
        conn.Close();
      }
    }

    public string GenerateCSVString(List<string> excludedColumns = null, string defaultDateTimeFormat = null, bool useAlias = true, bool orderData = true, bool camelBroken = true) {
      StringBuilder sb = new StringBuilder();
      if (Data == null)
        return sb.ToString();

      //remember that this should also apply column sequence, column aliases, column exclusion
      List<DataColumn> usedColumns = new List<DataColumn>();
      for (int i = 0; i < Data.Columns.Count; ++i) {
        DataColumn column = Data.Columns[i];
        if (excludedColumns == null || excludedColumns.Count <= 0 || 
          !excludedColumns.Any(x => x.EqualsIgnoreCase(column.ColumnName)))
          usedColumns.Add(column);
      }

      List<DataColumn> arrangedUsedColumns = orderData ? 
        Meta.GetColumnSequenceFor(usedColumns) : new List<DataColumn>(usedColumns);

      //Creates Header
      for (int i = 0; i < arrangedUsedColumns.Count; ++i) {
        DataColumn column = arrangedUsedColumns[i];
        string columnName = string.Empty;
        if (useAlias && Meta.ColumnAliases.Keys.Any(x => x.EqualsIgnoreCase(column.ColumnName))) {
          columnName = Meta.ColumnAliases.FirstOrDefault(x => x.Key.EqualsIgnoreCase(column.ColumnName)).Value;
        } else {
          columnName = camelBroken ? column.ColumnName.ToCamelBrokenString() : column.ColumnName;
        }
        sb.Append(columnName.AsCsvStringValue());
        if (i < arrangedUsedColumns.Count - 1)
          sb.Append(",");
      }
      if (sb.Length > 0)
        sb.AppendLine(); //next line

      //Creates items
      for (int i = 0; i < Data.Rows.Count; ++i) {
        DataRow row = Data.Rows[i];
        for (int j = 0; j < arrangedUsedColumns.Count; ++j) {
          DataColumn column = arrangedUsedColumns[j];
          object objVal = row[column];
          BaseSystemData data = BaseExtractor.Extract(objVal, column);
          string usedDateTimeFormat = defaultDateTimeFormat;
          if (data.IsDateTime && Meta.HasCustomDateTimeFormatFor(column.ColumnName, DH.CsvPageName))
            usedDateTimeFormat = Meta.GetCustomDateTimeFormatFor(column.ColumnName, DH.CsvPageName);
          string printedData = data.GetCsvValueString(usedDateTimeFormat);
          sb.Append(printedData);
          if (j < arrangedUsedColumns.Count - 1)
            sb.Append(",");
        }
        if (i < Data.Rows.Count - 1)
          sb.AppendLine();
      }

      return sb.ToString();
    }
  }
}