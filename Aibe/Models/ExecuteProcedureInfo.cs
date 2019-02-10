using Aibe.Models.Core;
using Extension.Models;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Aibe.Models {
  public class ExecuteProcedureInfo {
    public ActionProcedureFullInfo ProcedureFull { get; set; }
    public Dictionary<string, object> Parameters { get; set; }

    public BaseScriptModel GetExecuteProcedureSQLScript() {
      if (ProcedureFull == null)
        return null;
      List<SqlParameter> pars = new List<SqlParameter>();
      string procedureScript = ProcedureFull.Procedure.ProcedureName;

      //The procedure has parameters
      if (ProcedureFull.Procedure.ProcedureParameters != null && ProcedureFull.Procedure.ProcedureParameters.Count > 0)
        foreach (var pPar in ProcedureFull.Procedure.ProcedureParameters) {
          SqlParameter par;
          if (Parameters != null && Parameters.Any(x => x.Key.EqualsIgnoreCase(pPar.ParameterValueString))) { //parameters is found in the list
            KeyValuePair<string, object> kvp = Parameters.FirstOrDefault(x => x.Key.EqualsIgnoreCase(pPar.ParameterValueString));
            par = new SqlParameter(pPar.ParameterName, kvp.Value); //use the pPar key but replaces the pPar.Value with kvp.Value
          } else if (pPar.ParameterValueString != null && pPar.ParameterValueString.StartsWith("@@")) { //parameter but not found
            par = new SqlParameter(pPar.ParameterName, DBNull.Value);
          } else
            par = new SqlParameter(pPar.ParameterName, pPar.GetDBParameter());
          pars.Add(par);
        }

      BaseScriptModel model = new BaseScriptModel(procedureScript, pars);
      return model;
    }
  }
}
