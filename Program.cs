﻿using System.Text;

string s = "hello world";
byte[] ascii = Encoding.UTF8.GetBytes(s);

string result = string.Empty;

//1 преобразование вводной строки в двоичный код
foreach (byte bb in ascii)
    result += Get01Code(bb);

//добавление 1
result += "1";

//заполнение 0 до 448 символов
int additionalZeros = 448 - result.Length;
for (int i = 0; i < additionalZeros; i++)
    result += "0";

//добавление в конец и заполнение 0 до 512 символов
string bigEnd = "1011000";

additionalZeros = 512 - (result.Length + bigEnd.Length);
for (int i = 0; i < additionalZeros; i++)
    result += "0";

//result с 512 символов
result += bigEnd;


//результат из 16ти 32-битных слов
string result32=string.Empty;
for (int i=0; i<result.Length; i++)
{
    if (i % 32 == 0)
        result32 += " ";

    result32 += result[i];
}

for (int i = 16; i <= 65; i++)
{
    result32 += " ";

    for (int j = 0; j < 32; j++)
        result32 += "0";
}

List<string> result32array = [string.Empty];

int resultArrayIter = 0;
foreach (var cc in result32.ToCharArray())
{
    if (cc != ' ')
        result32array[resultArrayIter] += cc;
    else
    {
        resultArrayIter++;
        result32array.Add(string.Empty);
    }
}

result32array.Remove(string.Empty);
result32array.RemoveAt(65);
result32array.RemoveAt(64);

result32array = Convert32WordArray(result32array);

Console.WriteLine();
foreach (var str in result32array)
    Console.WriteLine(str);

///////////////////////////////////////////////////////all good
//// значения хеша (8)
var h0 = 0x6a09e667;
var h1 = 0xbb67ae85;
var h2 = 0x3c6ef372;
var h3 = 0xa54ff53a;
var h4 = 0x510e527f;
var h5 = 0x9b05688c;
var h6 = 0x1f83d9ab;
var h7 = 0x5be0cd19;

////округленные конгстанты (64)
//string consts = "0x428a2f98 0x71374491 0xb5c0fbcf 0xe9b5dba5 0x3956c25b 0x59f111f1 0x923f82a4 0xab1c5ed5\r\n0xd807aa98 0x12835b01 0x243185be 0x550c7dc3 0x72be5d74 0x80deb1fe 0x9bdc06a7 0xc19bf174\r\n0xe49b69c1 0xefbe4786 0x0fc19dc6 0x240ca1cc 0x2de92c6f 0x4a7484aa 0x5cb0a9dc 0x76f988da\r\n0x983e5152 0xa831c66d 0xb00327c8 0xbf597fc7 0xc6e00bf3 0xd5a79147 0x06ca6351 0x14292967\r\n0x27b70a85 0x2e1b2138 0x4d2c6dfc 0x53380d13 0x650a7354 0x766a0abb 0x81c2c92e 0x92722c85\r\n0xa2bfe8a1 0xa81a664b 0xc24b8b70 0xc76c51a3 0xd192e819 0xd6990624 0xf40e3585 0x106aa070\r\n0x19a4c116 0x1e376c08 0x2748774c 0x34b0bcb5 0x391c0cb3 0x4ed8aa4a 0x5b9cca4f 0x682e6ff3\r\n0x748f82ee 0x78a5636f 0x84c87814 0x8cc70208 0x90befffa 0xa4506ceb 0xbef9a3f7 0xc67178f2";
var a = h0;
var b = h1 ;
var c = h2 ;
var d = h3 ;
var e = h4 ;
var f = h5 ;
var g = h6;
var h = h7;

Mutate(a, b, c, d, e, f, g, h);



string Get01Code(int b)
{
    string result = string.Empty;

    int i = b;

    do
    {
        result += i % 2;
        i = i / 2;
    }
    while (i >= 2);
    result += i.ToString();

    var resultChar = result.ToCharArray().Reverse();
    result = string.Empty;

    foreach (char c in resultChar)
        result += c;
    
    for (int a = result.Length; a<8; a++)
        result = 0 + result;

    return result;
}

string Get01CodeFor32bit(ulong b)
{
    string result = string.Empty;
    //Console.WriteLine($"get32bitfornum: {b}");
    ulong i = b;

    do
    {
        result += i % 2;
        i = i / 2;
    }
    while (i >= 2);
    result += i.ToString();
   // Console.WriteLine($"res: {result}, rescnt: {result.Count()}");

    var resultChar = result.ToCharArray().Reverse();
    result = string.Empty;

    foreach (char c in resultChar)
        result += c;

    //Console.WriteLine($"res: {result}, rescnt: {result.Count()}");

    if (result.Length > 32)
    {
        var cres = result.ToCharArray().Reverse().ToList();
        result = string.Empty;
        for (int ii = 0; ii < 32; ii++)
        {
            result += cres[ii];
        }

        var charRes = result.ToCharArray().Reverse();
        result = string.Empty;

        foreach (var c in charRes)
        {
            result += c;
        }
    }
    else
    {
        for (int a = result.Length; a < 32; a++)
            result = '0' + result;
    }
   // Console.WriteLine($"res: {result}, rescnt: {result.Count()}");


    return result;
}

List<string> Convert32WordArray(List<string> words)
{
    for (int i = 16; i < 64; i++)
    {
        //Console.WriteLine($"word {i}");
        var w1 = RightRotate(7, words[i - 15]);
        var w2 = RightRotate(18, words[i - 15]);
        var w3 = RightShift(3, words[i - 15]);
        //Console.WriteLine($"w1 {w1}");
       // Console.WriteLine($"w2 {w2}");
       // Console.WriteLine($"w3 {w3}");
        //s0=w1 xor w2 xor w3
        string s0 = ConvertXOR(w1, w2, w3);
       // Console.WriteLine($"s0 {s0}");

        var w10 = RightRotate(17, words[i - 2]);
        var w20 = RightRotate(19, words[i - 2]);
        var w30 = RightShift(10, words[i - 2]);
       //Console.WriteLine($"w10 {w10}");
       //Console.WriteLine($"w20 {w20}");
       //Console.WriteLine($"w30 {w30}");
        //s1=w10 xor w20 xor w30
        string s1 = ConvertXOR(w10, w20, w30);
        //Console.WriteLine($"s1 {s1}");

        words[i] = CountWord(words[i-16], s0, words[i-7], s1);
       // Console.WriteLine(words[i]);
      //  Console.WriteLine();
    }

    return words;
}

string RightRotate(int rotations, string str)
{
    //Console.WriteLine($"rotating {rotations}, before:{str}");

    var word1 = str.ToCharArray().ToList();
    //foreach ( var word in word1)
    //{
    //    Console.Write($"{word}");
    //}

    for (int a = 0; a < rotations; a++)
    {
       // Console.WriteLine($"rotation {a}");
        var wNew = str.ToCharArray().ToList();

        //foreach (var word in word1)
        //{
        //    Console.Write($"{word}");
        //}
        //Console.WriteLine();
        //foreach (var word in wNew)
        //{
        //    Console.Write($"{word}");
        //}
        //Console.WriteLine();

            for (int c=1;c<32;c++)
            {
                    wNew[c] = word1[c-1];


            //foreach (var word in word1)
            //{
            //    Console.Write($"{word}");
            //}
            //Console.WriteLine();
            //foreach (var word in wNew)
            //{
            //    Console.Write($"{word}");
            //}
            //Console.WriteLine();
        }
            wNew[0] = word1.Last();

        //foreach (var word in word1)
        //{
        //    Console.Write($"{word}");
        //}
        //Console.WriteLine();
        //foreach (var word in wNew)
        //{
        //    Console.Write($"{word}");
        //}
        //Console.WriteLine();

        word1 = wNew;
    }

    string res = string.Empty;
    foreach (char c in word1)
    {
        res += c;
    }
    //Console.WriteLine($"rotating {rotations}, after:{res}");
    return res;
}

string RightShift(int shifts, string str)
{
    var w1 = str.ToCharArray().ToList();
   // Console.WriteLine($"w1:{str}");

    for (int a = 0; a < shifts; a++)
    {
         //Console.WriteLine($"shift {a}");
        var wNew = str.ToCharArray().ToList();

        //foreach (var word in word1)
        //{
        //    Console.Write($"{word}");
        //}
        //Console.WriteLine();
        //foreach (var word in wNew)
        //{
        //    Console.Write($"{word}");
        //}
        //Console.WriteLine();

        for (int c = 1; c < 32; c++)
        {
            wNew[c] = w1[c - 1];


            //foreach (var word in word1)
            //{
            //    Console.Write($"{word}");
            //}
            //Console.WriteLine();
            //foreach (var word in wNew)
            //{
            //    Console.Write($"{word}");
            //}
            //Console.WriteLine();
        }
        wNew[0] = '0';

        //foreach (var word in word1)
        //{
        //    Console.Write($"{word}");
        //}
        //Console.WriteLine();
        //foreach (var word in wNew)
        //{
        //    Console.Write($"{word}");
        //}
        //Console.WriteLine();

        w1 = wNew;
    }

    string res = string.Empty;
    foreach (char c in w1)
    {
        res += c;
    }
    //Console.WriteLine(res); ;
    return res;
}

string ConvertXOR (string w1, string w2, string w3)
{
    var c1 = w1.ToCharArray().ToList();
    var c2 = w2.ToCharArray().ToList();
    var c3 = w3.ToCharArray().ToList();
   // Console.WriteLine($"XOR c1:{w1}");
   // Console.WriteLine($"XOR c2:{w2}");
   // Console.WriteLine($"XOR c3:{w3}");

    string res = string.Empty;
    //uint c1 = Word32ToInt(w1);
    //uint c2 = Word32ToInt(w2);
    //uint c3 = Word32ToInt(w2);
    //Console.WriteLine($"int c1:{c1}");
    //Console.WriteLine($"int c2:{c2}");
    //Console.WriteLine($"int c3:{c3}");

    //ulong summ = c1 + c2 + c3;                  //error
    //                                            //error
    //string res = Get01CodeFor32bit(summ);       //error
    
    for(int i=0;i<32;i++)
    {
        if (c1[i]== '0' &&c2[i] == '0'&& c3[i] == '0')
            res += '0';
        if (c1[i] == '0' && c2[i] == '0' && c3[i] == '1')
            res += '1';
        if (c1[i] == '0' && c2[i] == '1' && c3[i] == '0')
            res += '1';
        if (c1[i] == '1' && c2[i] == '0' && c3[i] == '0')
            res += '1';
        if (c1[i] == '0' && c2[i] == '1' && c3[i] == '1')
            res += '0';
        if (c1[i] == '1' && c2[i] == '0' && c3[i] == '1')
            res += '0';
        if (c1[i] == '1' && c2[i] == '1' && c3[i] == '0')
            res += '0';
        if (c1[i] == '1' && c2[i] == '1' && c3[i] == '1')
            res += '1';

    }

    //Console.WriteLine(res);
    return res;
}

string ConvertXOR2(string w1, string w2)
{
    var c1 = w1.ToCharArray().ToList();
    var c2 = w2.ToCharArray().ToList();
    // Console.WriteLine($"XOR c1:{w1}");
    // Console.WriteLine($"XOR c2:{w2}");
    // Console.WriteLine($"XOR c3:{w3}");

    string res = string.Empty;
    //uint c1 = Word32ToInt(w1);
    //uint c2 = Word32ToInt(w2);
    //uint c3 = Word32ToInt(w2);
    //Console.WriteLine($"int c1:{c1}");
    //Console.WriteLine($"int c2:{c2}");
    //Console.WriteLine($"int c3:{c3}");

    //ulong summ = c1 + c2 + c3;                  //error
    //                                            //error
    //string res = Get01CodeFor32bit(summ);       //error

    for (int i = 0; i < 32; i++)
    {
        if (c1[i] == '0' && c2[i] == '0' )
            res += '0';
        if (c1[i] == '0' && c2[i] == '1' )
            res += '1';
        if (c1[i] == '0' && c2[i] == '1' )
            res += '1';
        if (c1[i] == '1' && c2[i] == '1' )
            res += '0';
        
    }

    //Console.WriteLine(res);
    return res;
}
string ConvertAND2(string w1, string w2)
{
    var c1 = w1.ToCharArray().ToList();
    var c2 = w2.ToCharArray().ToList();
    // Console.WriteLine($"XOR c1:{w1}");
    // Console.WriteLine($"XOR c2:{w2}");
    // Console.WriteLine($"XOR c3:{w3}");

    string res = string.Empty;
    //uint c1 = Word32ToInt(w1);
    //uint c2 = Word32ToInt(w2);
    //uint c3 = Word32ToInt(w2);
    //Console.WriteLine($"int c1:{c1}");
    //Console.WriteLine($"int c2:{c2}");
    //Console.WriteLine($"int c3:{c3}");

    //ulong summ = c1 + c2 + c3;                  //error
    //                                            //error
    //string res = Get01CodeFor32bit(summ);       //error

    for (int i = 0; i < 32; i++)
    {
        if (c1[i] == '0' && c2[i] == '0')
            res += '0';
        if (c1[i] == '0' && c2[i] == '1')
            res += '0';
        if (c1[i] == '0' && c2[i] == '1')
            res += '0';
        if (c1[i] == '1' && c2[i] == '1')
            res += '1';

    }

    //Console.WriteLine(res);
    return res;
}
string CountWord(string w1, string s0, string w2, string s1)
{
   // Console.WriteLine("countWord");
   //
   // Console.WriteLine($"str w1 : {w1}");
   // Console.WriteLine($"str w2: {w2}");
   // Console.WriteLine($"str s0: {s0}");
   // Console.WriteLine($"str s1: {s1}");
    var c1 = Word32ToInt(w1);
    var c2 = Word32ToInt(w2);
    var c3 = Word32ToInt(s1);
    var c4 = Word32ToInt(s0);
  // Console.WriteLine($"int : {c1}");
  // Console.WriteLine($"int : {c2}");
  // Console.WriteLine($"int : {c3}");
  // Console.WriteLine($"int : {c4}");

    string intWord = Get01CodeFor32bit(c1 + c2 + c3 + c4);

   // Console.WriteLine($"sum : {intWord}");

    List<char> chars = [.. intWord];

    while (chars.Count > 32)
        chars.RemoveAt(0);

    string res= string.Empty;
    foreach (char c in chars)
    {
        res += c;
    }
        
    return res;
}

uint Word32ToInt(string word)
{
    double res = 0;

    var c = word.ToCharArray();
    for(int i = c.Count()-1; i >= 0; i--)
    {
        if (c[i] == '1')
            res += Math.Pow(2, 31-i);
    }

    return (uint)res;
}

void Mutate (int a,uint b,int c,uint d,int e,uint f ,int g,int h)
{
    string codeA = Get01CodeFor32bit((ulong)a);
    string codeB = Get01CodeFor32bit((ulong)b);
    string codeC = Get01CodeFor32bit((ulong)c);
    string codeD = Get01CodeFor32bit((ulong)d);
    string codeE = Get01CodeFor32bit((ulong)e);
    string codeF = Get01CodeFor32bit((ulong)f);
    string codeG = Get01CodeFor32bit((ulong)g);
    string codeH = Get01CodeFor32bit((ulong)h);

    var eROT6 = RightRotate(6, codeE);
    var eROT11 = RightRotate(11, codeE);
    var eROT25 = RightRotate(25, codeE);
    var s1 = ConvertXOR(eROT11, eROT25, eROT6);

    var eANDf = ConvertAND2(codeE, codeF);

    string not_e = string.Empty;
    foreach (var cha in codeE.ToCharArray().ToList())
    {
        if(cha == '0')
            not_e += "1";
        else
            not_e += "0";
    }
    
    var not_eANDg = ConvertAND2(not_e, codeG);
    var ch = ConvertXOR2(eANDf, not_eANDg);
}