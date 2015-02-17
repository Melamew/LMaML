using System;

namespace LMaML.Infrastructure.Commands
{
    public class OrderByCommand<T, TKey> : BusMessage where TKey : IComparable
    {
        public Func<T, TKey> PropertyAccessor { get; set; }

        public OrderByCommand(Func<T, TKey> propertyAccessor)
        {
            PropertyAccessor = propertyAccessor;
        }
    }
}