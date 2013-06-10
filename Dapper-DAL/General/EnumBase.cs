using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper_DAL.General.Interfaces;

namespace Dapper_DAL.General
{
    public abstract class EnumBase<TEntity, TValue> : IEnumBase<EnumBase<TEntity, TValue>, TValue> where TEntity : EnumBase<TEntity, TValue>
    {
        #region Instance code
        public TValue Value { get; private set; }
        public CommandType? CmdType { get; private set; }
        public string Name { get; private set; }

        protected EnumBase(string Name, TValue EnumValue, CommandType? cmdType = null)
        {
            Value = EnumValue;
            this.Name = Name;
            CmdType = cmdType;
            mapping.Add(Name, this);
        }

        public override string ToString() { return Name; }
        #endregion

        #region Static tools
        static private readonly Dictionary<string, EnumBase<TEntity, TValue>> mapping;
        static EnumBase() { mapping = new Dictionary<string, EnumBase<TEntity, TValue>>(); }
        protected static TEntity Parse(string name)
        {
            EnumBase<TEntity, TValue> result;
            if (mapping.TryGetValue(name, out result))
            {
                return (TEntity)result;
            }

            throw new InvalidCastException();
        }
        // This is protected to force the child class to expose it's own static
        // method.
        // By recreating this static method at the derived class, static
        // initialization will be explicit, promising the mapping dictionary
        // will never be empty when this method is called.
        protected static IEnumerable<TEntity> All { get { return mapping.Values.AsEnumerable().Cast<TEntity>(); } }
        #endregion

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}