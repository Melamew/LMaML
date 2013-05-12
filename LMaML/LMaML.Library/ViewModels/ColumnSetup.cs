using System;
using LMaML.Infrastructure.Util;
using iLynx.Common;

namespace LMaML.Library.ViewModels
{
    /// <summary>
    /// ColumnSetup
    /// </summary>
    public class ColumnSetup : IColumnSetup
    {
        public string Name { get; private set; }
        public Guid Id { get; private set; }

        public ColumnSetup(string name, Guid id)
        {
            name.GuardString("name");

            Name = name;
            Id = id;
        }
    }
}