using System.Text;

namespace Hedgey.Extensions;

static public class Bit{

  
  static public int ReverseBits(int n,int count){
    int start = 0;
    
    for(int i=0; i!=count; ++i)
    {
      start = (start << 1) | n & 1;
      n >>= 1;
    }
    return start;
  }  
  static public uint ReverseBits(uint n,int count){
    uint start = 0;
    
    for(int i=0; i!=count; ++i)
    {
      start = (start << 1) | n & 1;
      n >>= 1;
    }
    return start;
  }  
  static public long ReverseBits(long n,int count){
    long start = 0;
    
    for(int i=0; i!=count; ++i)
    {
      start = (start << 1) | n & 1;
      n >>= 1;
    }
    return start;
  }  
  static public ulong ReverseBits(ulong n,int count){
    ulong start = 0;
    
    for(int i=0; i!=count; ++i)
    {
      start = (start << 1) | n & 1;
      n >>= 1;
    }
    return start;
  }  
  static public uint ReverseBits(uint n)
  {
    n = (n << 16) | (n >> 16);
    n = ((n & 0xFF00FF00) >> 8) | ((n & 0x00FF00FF) << 8);
    n = ((n & 0xF0F0F0F0) >> 4) | ((n & 0x0F0F0F0F) << 4);
    n = ((n & 0xCCCCCCCC) >> 2) | ((n & 0x33333333) << 2);
    n = ((n & 0xAAAAAAAA) >> 1) | ((n & 0x55555555) << 1);
    return n;
  }
  static public ulong ReverseBits(ulong n)
  {
    n = (n << 32) | (n >> 32);
    n = ((n & 0xFFFF0000FFFF0000) >> 16) | ((n & 0x0000FFFF0000FFFF) << 16);
    n = ((n & 0xFF00FF00FF00FF00) >> 8) | ((n & 0x00FF00FF00FF00FF) << 8);
    n = ((n & 0xF0F0F0F0F0F0F0F0) >> 4) | ((n & 0x0F0F0F0F0F0F0F0F) << 4);
    n = ((n & 0xCCCCCCCCCCCCCCCC) >> 2) | ((n & 0x3333333333333333) << 2);
    n = ((n & 0xAAAAAAAAAAAAAAAA) >> 1) | ((n & 0x5555555555555555) << 1);
    return n;
  }
  static public string ToString(ulong n, bool bigEnding = true)
  {
    StringBuilder builder = new StringBuilder();
    int from = bigEnding ? 0 : 63;
    int to = bigEnding ? 64 : -1;
    Func<int, int> Next = bigEnding ? (i) => ++i : (i) => --i;
    for (int i = from; i != to; i = Next(i))
      builder.Append((n >> i) & 1);
    return builder.ToString();
  }
  static public string ToString(int n, bool bigEnding = true)
  {
    StringBuilder builder = new StringBuilder();
    int from = bigEnding ? 0 : 31;
    int to = bigEnding ? 32 : -1;
    Func<int, int> Next = bigEnding ? (i) => ++i : (i) => --i;
    for (int i = from; i != to; i = Next(i))
      builder.Append((n >> i) & 1);
    return builder.ToString();
  }
  static public void Write(ulong n)
  {
    for(int i =0; i!=64;++i)
      Console.Write((n >> i) & 1);
    Console.WriteLine();
  }
  static public void Write(int n)
  {
    for(int i =0; i!=32;++i)
      Console.Write((n >> i) & 1);
    Console.WriteLine();
  }
}
