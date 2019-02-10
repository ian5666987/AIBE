using Extension.Database.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Aibe.Models {
  public class FilterGroupDetailsModel : BaseFilterIndexModel {
    public List<KeyValuePair<string, object>> Identifiers { get; set; } = new List<KeyValuePair<string, object>>();

    public FilterGroupDetailsModel(MetaInfo meta, int? page, List<KeyValuePair<string, object>> identifiers, 
      Dictionary<string, string> dictCollections) : base(meta, page, dictCollections) {
      //Group by parts
      if (identifiers != null) {
        int identifierIndex = 0;
        if (Pars == null)
          Pars = new List<SqlParameter>();
        if (CopiesPars == null)
          CopiesPars = new List<SqlParameter>();
        foreach (var identifier in identifiers) {
          if (identifier.Key == null) //identifier key cannot be NULL
            continue;
          QueryScript.Append(_hasAutoWhere ? " AND " : " WHERE ");
          QueryScript.Append(identifier.Key);
          QueryScript.Append("=");
          string parName = "@identifierPar" + identifierIndex;
          QueryScript.Append(parName);
          SqlParameter par = new SqlParameter(parName, identifier.Value ?? DBNull.Value);
          SqlParameter copyPar = new SqlParameter(parName, identifier.Value ?? DBNull.Value);
          Pars.Add(par);
          CopiesPars.Add(copyPar);
          ++identifierIndex;
          _hasAutoWhere = true;
        }
        Identifiers = identifiers;        
      }
    }

    //Now the completion on this GroupDetails is including IdentifierPars
    public override void CompleteModelAndData(bool isGrouping, bool loadAllData = false) { //loadAllData is added to give a way to do so when needed
      using (SqlConnection conn = new SqlConnection(DH.DataDBConnectionString)) {
        conn.Open();
        InternalProcessCompleteModelAndData(conn, CopiesPars, isGrouping, loadAllData);
        string completeScript = string.Concat(SelectScript, " * ", QueryScript); //form the complete script here
        Data = SQLServerHandler.GetDataTable(conn, completeScript, Pars); //new DataTable();
        conn.Close();
      }
    }
  }
}