namespace Hedgey.Extensions;

static public class Converter{
 static public string ToBase64(ulong id)
  {
    var bytes = BitConverter.GetBytes(id);

    if (!BitConverter.IsLittleEndian)
      Array.Reverse(bytes);

    string block = ToBase64(bytes);
    return block;
  }
 public static string ToBase64(byte[] bytes)
  {
    const string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

    int length = bytes.Length;
    int padding = length % 3;
    if(padding!=0)
      padding = padding ^ 3;
    int stringLength = length / 3 * 4;
    if (padding != 0)
      stringLength += 4;

    char[] result = new char[stringLength];

    int resultIndex = 0;
    int index = 0;

    while (index < length - 2)
    {
      byte b1 = bytes[index++];
      byte b2 = bytes[index++];
      byte b3 = bytes[index++];

      result[resultIndex++] = Base64Chars[(b1 >> 2) & 0x3F];
      result[resultIndex++] = Base64Chars[((b1 << 4) & 0x30) | ((b2 >> 4) & 0x0F)];
      result[resultIndex++] = Base64Chars[((b2 << 2) & 0x3C) | ((b3 >> 6) & 0x03)];
      result[resultIndex++] = Base64Chars[b3 & 0x3F];
    }

    if (index == length) return new string(result);

    byte b4 = bytes[index++];

    result[resultIndex++] = Base64Chars[(b4 >> 2) & 0x3F];

    if (index == length)
    {
      result[resultIndex++] = Base64Chars[(b4 << 4) & 0x30];
      result[resultIndex++] = '=';
    }
    else
    {
      byte b5 = bytes[index++];
      result[resultIndex++] = Base64Chars[((b4 << 4) & 0x30) | ((b5 >> 4) & 0x0F)];
      result[resultIndex++] = Base64Chars[(b5 << 2) & 0x3C];
    }

    result[resultIndex++] = '=';

    return new string(result);
  }

  static public long FromBase64(string hash)
  {
    hash = hash.Replace('_', '/').Replace('-', '+');
    switch (hash.Length % 4)
    {
      case 2: hash += "=="; break;
      case 3: hash += "="; break;
    }

    var bytes = Convert.FromBase64String(hash);
    var number = BitConverter.ToInt64(bytes, 0);
    return number;
    }
}