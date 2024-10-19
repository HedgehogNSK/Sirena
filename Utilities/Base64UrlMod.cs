namespace Hedgey.Utilities;

static public class Base64UrlMod
{
  public const string alphabet = "-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";
  static public unsafe byte[] GetBytes(string hash)
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
        {
          uint currCode = *ptr;

          // Determine current char code:
          //intDash has code 000000. So we don't need to write anything. Just move pointers to next bits
          if (currCode == intDash)
          {
            if (switcher != 0)
              ++bytePtr;

            ++switcher;
            switcher &= 3;
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
  public static string FromBytes(byte[] bytes)
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

    //Distribute 4 letters by 6 bits into 3 bytes one  by one
    //[000000] [00 0000] [0000 00] [000000]
    while (index < length - 2)
    {
      byte b1 = bytes[index++];
      byte b2 = bytes[index++];
      byte b3 = bytes[index++];

      result[resultIndex++] = alphabet[b1 & 0x3F]; //0x3F = 1111 1100 (Big Ending)
      result[resultIndex++] = alphabet[(b1 >> 6) | ((b2 << 2) & 0x3C)]; //0x3C 0011 1100
      result[resultIndex++] = alphabet[(b2 >> 4) | ((b3 << 4) & 0x30)]; //0x30 0000 1100
      result[resultIndex++] = alphabet[(b3 >> 2) & 0x3F]; //0x0F 1111 0000
    }

    if (index == length) return new string(result);

    byte b4 = bytes[index++];
    result[resultIndex++] = alphabet[b4 & 0x3F]; //0x3F 1111 1100

    if (index == length)
    {
      result[resultIndex++] = alphabet[b4 >> 6];
      result[resultIndex++] = '=';
    }
    else
    {
      byte b5 = bytes[index++];
      result[resultIndex++] = alphabet[(b4 >> 6) | ((b5 << 2) & 0x3C)]; //0x3C 0011 1100
      result[resultIndex++] = alphabet[b5 >> 4];
    }
    result[resultIndex++] = '=';

    return new string(result);
  }
  static public string FromLong(ulong id)
  {
    var bytes = BitConverter.GetBytes(id);

    if (!BitConverter.IsLittleEndian)
      Array.Reverse(bytes);
    return FromBytes(bytes);
  }
  static public long ToLong(string hash)
  {
    var bytes = GetBytes(hash);
    if (bytes.Length != 8)
      Array.Resize(ref bytes, 8);
    return BitConverter.ToInt64(bytes, 0);
  }
}