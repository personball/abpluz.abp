﻿using System.Collections.Generic;
using System.Text;
namespace System
{
    public static class AbpluzStringExtensions
    {
        public static string ReplaceAsSpan(this string str, Dictionary<string, string> map)
        {
            ReadOnlySpan<char> content = str.AsSpan();
            StringBuilder builder = new StringBuilder();
            var keyPosMap = new Dictionary<string, List<int>>();
            var posSortedMapKey = new SortedDictionary<int, string>();

            foreach (var key in map.Keys)
            {
                int accPos = 0;
                int keyLen = key.Length;
                ReadOnlySpan<char> keySpan = key.AsSpan();
                ReadOnlySpan<char> contentTmp = content;

                while (true)
                {
                    int startPos = contentTmp.IndexOf(keySpan);
                    if (startPos == -1)
                    {
                        break;
                    }

                    if (keyPosMap.ContainsKey(key))
                    {
                        keyPosMap[key].Add(accPos + startPos);// 相对于原始字串的index
                    }
                    else
                    {
                        keyPosMap.Add(key, new List<int> { accPos + startPos });
                    }

                    if (!posSortedMapKey.ContainsKey(accPos + startPos))
                    {
                        posSortedMapKey.Add(accPos + startPos, key);
                    }

                    contentTmp = contentTmp.Slice(startPos + keyLen);
                    accPos += startPos + keyLen;
                }
            }

            // slice and merge
            var start = 0;
            foreach (var pos in posSortedMapKey.Keys)
            {
                var key = posSortedMapKey[pos];
                var value = map[key];
                builder.Append(content.Slice(start, pos - start));
                start = pos + key.Length;
                builder.Append(value);
            }

            // tail
            if (start < content.Length)
            {
                builder.Append(content.Slice(start, content.Length - start));
            }

            return builder.ToString();
        }
    }
}
