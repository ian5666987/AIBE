using Extension.Cryptography;

namespace Aibe.Helpers {
  public class CryptographyHelper {
    public static void Init(string keyName, string extension, string password) {
      switch (keyName) { //ASTrio is one of the predefined keyNames to replace the original value
        case "ASTrio":
          Cryptography.SetExtension(extension);
          Cryptography.SetPassword(password);
          Cryptography.SetAesKey(new byte[] {
            0x03, 0x12, 0x19, 0x65,
            0x25, 0x12, 0x00, 0x01,
            0x03, 0x05, 0x19, 0x87,
            0x01, 0x07, 0x20, 0x13
          });
          Cryptography.SetValidity();
          break;
      }
    }
  }
}
