namespace Hedgey.Extensions;

static public class Converter
{
  const string Base64Chars = "-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";
  static public string ToBase64URL(ulong id)
  {
    var bytes = BitConverter.GetBytes(id);

    if (!BitConverter.IsLittleEndian)
      Array.Reverse(bytes);
    return ToBase64URLHM(bytes);
  }
  /// <summary>
  /// Classic converter byte to base 64 url string
  /// </summary>
  /// <param name="bytes"></param>
  /// <returns></returns>
  public static string ToBase64URL(byte[] bytes)
  {
    //Each 3 bytes translates into 4 letters from the list
    int length = bytes.Length;
    int padding = length % 3;
    if (padding != 0)
      padding = padding ^ 3;
    int stringLength = (length / 3) << 2;
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

      //0x3F 1111 1100
      result[resultIndex++] = Base64Chars[(b1 >> 2) & 0x3F];
      //0x30 0000 1100 //0x0F 1111 0000
      result[resultIndex++] = Base64Chars[((b1 << 4) & 0x30) | ((b2 >> 4) & 0x0F)];
      //0x3C 0011 1100
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
  public static string ToBase64URLHM(byte[] bytes)
  {
    //Each 3 bytes translates into 4 letters from the list
    int length = bytes.Length;
    int padding = length % 3;
    if (padding != 0)
      padding = padding ^ 3;
    int stringLength = (length / 3) << 2;
    if (padding != 0)
      stringLength += 4;

    char[] result = new char[stringLength];

    int resultIndex = 0;
    int index = 0;

    // TimeStamp(42 bits)							            Sequence(12) Machine ID(10)
    // 10000010111010011110100 1010100010000000 00 0000000000000 1111111111
    // 											                          v            v
    // 10100110 10010010 01110100 00000000 00000000 00010010 10000000 00000000
    // 													                             ^7

    while (index < length - 2)
    {
      byte b1 = bytes[index++];
      byte b2 = bytes[index++];
      byte b3 = bytes[index++];

      result[resultIndex++] = Base64Chars[b1 & 0x3F]; //0x3F 1111 1100
      result[resultIndex++] = Base64Chars[(b1 >> 6) | ((b2 << 2) & 0x3C)]; //0x3C 0011 1100
      result[resultIndex++] = Base64Chars[(b2 >> 4) | ((b3 << 4) & 0x30)]; //0x30 0000 1100
      result[resultIndex++] = Base64Chars[(b3 >> 2) & 0x3F]; //0x0F 1111 0000
    }

    if (index == length) return new string(result);

    byte b4 = bytes[index++];
    result[resultIndex++] = Base64Chars[b4 & 0x3F]; //0x3F 1111 1100

    if (index == length)
    {
      result[resultIndex++] = Base64Chars[b4 >> 6];
      result[resultIndex++] = '=';
    }
    else
    {
      byte b5 = bytes[index++];
      result[resultIndex++] = Base64Chars[(b4 >> 6) | ((b5 << 2) & 0x3C)]; //0x3C 0011 1100
      result[resultIndex++] = Base64Chars[b5 >> 4];
    }
    result[resultIndex++] = '=';

    return new string(result);
  }
  static public long FromBase64URLHMToLong(string hash)
  {
    var bytes = FromBase64URLHM(hash);
    return BitConverter.ToInt64(bytes, 0);
  }

  static public unsafe byte[] FromBase64URLHM(string hash)
  {
    const uint intTab = '\t';   //9u
    const uint intNLn = '\n';   //10u
    const uint intCRt = '\r';   //13u
    const uint intDash = '-';   //45u  Base64 -> 0
    const uint int0 = '0' - 1u; //47u //[0 - 9] : [48 - 57]  Base64 -> [1 - 10]  diff = 47
    const uint intEq = '=';     //61u
    const uint intA = 'A' - 1u; //64u  //[A - Z] : [65 - 90]  Base64 -> [11 - 36] diff = 54
    const uint intSub = '_';    //95u  Base64 -> 37
    const uint inta = 'a' - 1u; //96u  //[a - z] : [97 - 122] Base64 -> [38 - 63] diff = 59

    int bytesLength = hash.Length - (hash.Length >> 2); // = 3/4 * length
    int switcher = 0;
    unchecked
    {
      byte* bytePtr = stackalloc byte[bytesLength];
      byte* byteBeginPtr = bytePtr;
      fixed (char* charPtr = hash)
      {

        char* endPtr = charPtr + hash.Length;
        for (char* ptr = charPtr; ptr != endPtr; ++ptr)
        // for (int id = 0; id != hash.Length; ++id)
        {
          // char c = hash[id];
          uint currCode = *ptr;

          // Determine current char code:
          //intDash has code 000000. So we don't need to write anything. Just move pointers to next bits
          if (currCode == intDash)
          {
            if (switcher != 0)
              ++bytePtr;

            ++switcher;
            switcher %= 4;
          }
          else
          {
            if (currCode > inta)
              currCode -= 59u;
            else if (currCode == intSub)
              currCode = 37u;
            else if (currCode > intA)
              currCode -= 54u;
            else if (currCode > int0)
              currCode -= 47u;

            else if (currCode == intTab
                  || currCode == intNLn
                  || currCode == intCRt)
            {
              ++ptr;
              continue;
            }
            else if (currCode == intEq)
            {
              break;
            }
            else
              throw new FormatException($"Symbol \"{*ptr}\"[code:{currCode}] doesn't belong to Base64URL");

            //Distribute 4 letters by 6 bits into 3 bytes one  by one
            //[000000] [00 0000] [0000 00] [000000]
            // 1           2           3
            if (switcher == 0)
            {
              *bytePtr |= (byte)currCode;
              ++switcher;
            }
            else if (switcher == 1)
            {
              *bytePtr |= (byte)(currCode << 6);
              ++bytePtr;
              *bytePtr |= (byte)(currCode >> 2);
              ++switcher;
            }
            else if (switcher == 2)
            {
              *bytePtr |= (byte)(currCode << 4);
              ++bytePtr;
              *bytePtr |= (byte)(currCode >> 4);
              ++switcher;
            }
            else
            {
              *bytePtr |= (byte)(currCode << 2);
              ++bytePtr;
              switcher = 0;
            }
          }
        }
      }

      byte[] array = new byte[bytePtr - byteBeginPtr];
      int id = 0;
      while (byteBeginPtr != bytePtr)
      {
        array[id++] = *byteBeginPtr;
        ++byteBeginPtr;
      }
      return array;
    }
  }
}