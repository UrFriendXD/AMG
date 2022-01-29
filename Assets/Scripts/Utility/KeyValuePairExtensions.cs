﻿using System.Collections.Generic;

namespace Utility
{
    public static class KeyValuePairExtensions
    {
        public static void Deconstruct<TKey,TValue>(
            this KeyValuePair<TKey,TValue> entry,
            out TKey key,
            out TValue value)
        {
            key = entry.Key;
            value = entry.Value;
        }
    }
}