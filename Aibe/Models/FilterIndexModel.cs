using Extension.Database.SqlServer;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Aibe.Models {
  public class FilterIndexModel : BaseFilterIndexModel {
    public List<string> IdentifierColumns { get; set; } = new List<string>();
    public DataTable ForeignIdentifiersData { get; protected set; } //v1.4.1.0 additional items just to distinguish between the data to be displayed and the data keys used for identifiers
    public FilterIndexModel(MetaInfo meta, int? page, Dictionary<string, string> dictCollections) :
      base (meta, page, dictCollections) {
      IdentifierColumns = meta.GroupByColumns;
      if (meta.IsGroupTable) { //there should be a group table query here...
        QueryScript.Append(" GROUP BY ");
        QueryScript.Append(string.Join(", ", meta.GroupByColumns));
      }
    }

    public override void CompleteModelAndData(bool isGrouping, bool loadAllData = false) {
      using (SqlConnection conn = new SqlConnection(DH.DataDBConnectionString)) {
        conn.Open();
        InternalProcessCompleteModelAndData(conn, CopiesPars, isGrouping, loadAllData);
        string completeScript = string.Concat(SelectScript, " ", Meta.IsGroupTable ?  
          Meta.AggregationStatement.AggregationQueryScript : "*", " ", QueryScript); //form the complete script here
        Data = SQLServerHandler.GetDataTable(conn, completeScript, Pars);
        string completeIdentifierScript = string.Concat(SelectScript, " ", Meta.IsGroupTable ?
          Meta.AggregationStatement.IdentifierAggregationQueryScript : "*", " ", QueryScript); //form the complete script here
        List<SqlParameter> foreignInfoPars = new List<SqlParameter>();
        if (Pars != null) //fixed in Aibe v1.4.2.0 so that Pars will not be used twice for actual data and foreign info alike
          foreach (var par in Pars)
            foreignInfoPars.Add(new SqlParameter(par.ParameterName, par.Value));
        ForeignIdentifiersData = SQLServerHandler.GetDataTable(conn, completeIdentifierScript, foreignInfoPars);
        conn.Close();
      }
    }
  }
}