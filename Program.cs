using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Input: ");
        var input = Console.ReadLine();
        Console.WriteLine();
        if (input == null) return;

        // Should only use alphabets and numbers.
        foreach (var ch in input)
            if ((ch < 'A' || ch > 'Z') && (ch < 'a' || ch > 'z') && (ch < '0' || ch > '9'))
                return;

        // Should end with '$' (dollar sign).
        if (string.IsNullOrEmpty(input) || input[input.Length - 1] != '$') input += '$';

        // Bucket sort by the two-char prefix of every substring.
        var types = new bool[input.Length];
        var bucket = new Dictionary<char, List<int>[]>();
        for (var i = 0; i < input.Length; i++)
        {
            var key = input[i];
            if (!bucket.ContainsKey(key))
                bucket.Add(key, new[] { new List<int>(), new List<int>() });
            bucket[key][(types[i] = key <= (input + '$')[i + 1]) ? 1 : 0].Add(i);
        }

        // Put Type B strings into the result array.
        var bucketHead = new Dictionary<char, int>();
        var result = new int[input.Length];
        {
            var i = 0;
            foreach (var key in bucket.Keys.OrderBy(x => x))
            {
                var substrings = bucket[key];

                // Make a placeholder for Type A substrings.
                switch (substrings[0].Count)
                {
                    case 0:
                        // Type A substring does not exist.
                        break;

                    case 1:
                        // Only one exists, so put it into the beginning of
                        // the bucket with respect to the key.
                        types[result[i++] = substrings[0][0]] = true;
                        break;

                    default:
                        // Make placeholders for the Type A substrings for now.
                        bucketHead.Add(key, i);
                        i += substrings[0].Count;
                        break;
                }

                // Sort and copy Type B substrings into the result array.
                foreach (var index in substrings[1].OrderBy(x => input.Substring(x)))
                    result[i++] = index;
            }
        }

        // Then put Type A substrings into the result array.
        for (var i = 0; i < result.Length; i++)
        {
            var assign = result[i] - 1;
            if (!(assign < 0 || types[assign]))
                result[bucketHead[input[assign]]++] = assign;
        }

        // Print the result.
        Console.WriteLine("Index  Suffix");
        Console.WriteLine(new string('=', 50));
        for (var i = 0; i < result.Length; i++)
            Console.WriteLine("{0}{1}  {2}",
                new string(' ', 5 - result[i].ToString().Length), result[i],
                input.Substring(result[i]));

        // Validation.
        var item = new int[input.Length];
        for (var i = 0; i < input.Length; i++) item[i] = i;
        item = item.OrderBy(x => input.Substring(x)).ToArray();
        for (var i = 0; i < result.Length; i++)
            if (item[i] != result[i])
            {
                Console.WriteLine("Error");
                return;
            }
    }
}