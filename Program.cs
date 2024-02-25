using System.Collections;
using System.Collections.Specialized;
using System.Text;

string s = "hello world";
byte[] ascii = Encoding.UTF8.GetBytes(s);

string result = string.Empty;

//1 преобразование вводной строки в двоичный код
foreach (byte b in ascii)
    result += Get01Code(b);

//добавление 1
result += "1";

//заполнение 0 до 448 символов
int additionalZeros = 448 - result.Length;
for (int i = 0; i < additionalZeros; i++)
    result += "0";

//добавление 88 в конец и заполнение 0 до 512 символов
string bigEnd = "1011000";

additionalZeros = 512 - (result.Length + bigEnd.Length);
for (int i = 0; i < additionalZeros; i++)
    result += "0";

//result с 512 символов
result += bigEnd;

// значения хеша (8)
var h0 = 0x6a09e667;
var h1 = 0xbb67ae85;
var h2 = 0x3c6ef372;
var h3 = 0xa54ff53a;
var h4 = 0x510e527f;
var h5 = 0x9b05688c;
var h6 = 0x1f83d9ab;
var h7 = 0x5be0cd19;

//округленные конгстанты (64)
string consts = "0x428a2f98 0x71374491 0xb5c0fbcf 0xe9b5dba5 0x3956c25b 0x59f111f1 0x923f82a4 0xab1c5ed5\r\n0xd807aa98 0x12835b01 0x243185be 0x550c7dc3 0x72be5d74 0x80deb1fe 0x9bdc06a7 0xc19bf174\r\n0xe49b69c1 0xefbe4786 0x0fc19dc6 0x240ca1cc 0x2de92c6f 0x4a7484aa 0x5cb0a9dc 0x76f988da\r\n0x983e5152 0xa831c66d 0xb00327c8 0xbf597fc7 0xc6e00bf3 0xd5a79147 0x06ca6351 0x14292967\r\n0x27b70a85 0x2e1b2138 0x4d2c6dfc 0x53380d13 0x650a7354 0x766a0abb 0x81c2c92e 0x92722c85\r\n0xa2bfe8a1 0xa81a664b 0xc24b8b70 0xc76c51a3 0xd192e819 0xd6990624 0xf40e3585 0x106aa070\r\n0x19a4c116 0x1e376c08 0x2748774c 0x34b0bcb5 0x391c0cb3 0x4ed8aa4a 0x5b9cca4f 0x682e6ff3\r\n0x748f82ee 0x78a5636f 0x84c87814 0x8cc70208 0x90befffa 0xa4506ceb 0xbef9a3f7 0xc67178f2";

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
foreach (var c in result32.ToCharArray())
{
    if (c != ' ')
        result32array[resultArrayIter] += c;
    else
    {
        resultArrayIter++;
        result32array.Add(string.Empty);
    }
}

result32array.Remove(string.Empty);
result32array.Remove(result32array.Last());
result32array.Remove(result32array.Last());

result32array = Convert32WordArray(result32array);

foreach (var str in result32array)
    Console.WriteLine(str);

//Console.WriteLine(result32array.Count());


string Get01Code(byte b)
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

List<string> Convert32WordArray(List<string> words)
{
    for (int i = 16; i < 64; i++)
    {
        string s0 = string.Empty;

        var w1 = RightRotate(7, words[i - 15]);
        var w2 = RightRotate(18, words[i - 15]);
        var w3 = RightShift(3, words[i - 15]);

        //s0=w1 xor w2 xor w3
        s0 = ConvertXOR(w1, w2, w3);


        string s1 = string.Empty;

        var w10 = RightRotate(17, words[i - 2]);
        var w20 = RightRotate(19, words[i - 2]);
        var w30 = RightShift(10, words[i - 2]);

        //s0=w1 xor w2 xor w3
        s1 = ConvertXOR(w10, w20, w30);


        words[i] = CountWord(words[i-16], s0, words[i-7], w1);
    }

    return words;
}

string RightRotate(int rotations, string str)
{
    var w1 = str.ToCharArray();

    for (int a = 0; a < rotations; a++)
    {
        var last = w1.Last();
        for (int b = 0; b < 32; b++)
        {
            if (b != 31)
                w1[b + 1] = w1[b];
        }
        w1[0] = last;
    }
    
    return str;
}

string RightShift(int shifts, string str)
{
    var w1 = str.ToCharArray();

    for (int a = 0; a < shifts; a++)
    {
        var last = w1.Last();
        for (int b = 0; b < 32; b++)
        {
            if (b != 31)
                w1[b + 1] = w1[b];
        }
        w1[0] = '0';
    }

    return str;
}

string ConvertXOR (string w1, string w2, string w3)
{
    var str = new char[32];
    var c1 = w1.ToCharArray();
    var c2 = w2.ToCharArray();
    var c3 = w3.ToCharArray();

    for (int i = 0; i < 32; i++)
    {
        if (c1[i]==0)
        {
            if (c2[i] == 0)
            {
                if (c3[i] == 0)
                    str[i] = '0';
                else
                    str[i] = '1';
            }
            else
            {
                if (c3[i] == 0)
                    str[i] = '1';
                else
                    str[i] = '0';
            }
        }
        else
        {
            if (c2[i] == 0)
            {
                if (c3[i] == 0)
                    str[i] = '1';
                else
                    str[i] = '0';
            }
            else
            {
                if (c3[i] == 0)
                    str[i] = '0';
                else
                    str[i] = '1';
            }
        }
    }

    return str.ToString();
}

string CountWord(string w1, string s0, string w2, string s1)
{

}
