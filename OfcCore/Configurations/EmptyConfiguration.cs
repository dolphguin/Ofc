﻿namespace OfcCore.Configurations
{
    using JetBrains.Annotations;

    public class EmptyConfiguration : IConfiguaration
    {
        private static IConfiguaration instance;

        public static IConfiguaration Instance => instance ?? (instance = new EmptyConfiguration());


        private EmptyConfiguration()
        {
        }


        public object Get(string name) => null;

        public bool Has(string name) => false;

        public bool Remove(string name) => false;

        public void Set(string name, [CanBeNull] object value)
        {
        }

        public object this[string name]
        {
            get { return null; }
            set { }
        }
    }
}