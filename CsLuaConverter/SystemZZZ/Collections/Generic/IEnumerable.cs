﻿namespace SystemZZZ.Collections.Generic
{
    public interface IEnumerable<T> : IEnumerable
    {
        IEnumerator<T> GetEnumerator();
    }
}