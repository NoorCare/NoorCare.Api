﻿namespace NoorCare
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
