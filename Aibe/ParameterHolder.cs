namespace Aibe {
  public class PH {
    public static string DBProvider = Aibe.DH.DBProviderSQLServerName;
    public static bool DBProviderIsSQLServer { get { return DBProvider == Aibe.DH.DBProviderSQLServerName; } }
    public static bool DBProviderIsOracle { get { return DBProvider == Aibe.DH.DBProviderOracleName; } }
  }
}
