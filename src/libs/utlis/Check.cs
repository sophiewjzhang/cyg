﻿using System;

namespace utlis
{
    public class Check
    {
        public static void NotNull(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
        }
    }
}
