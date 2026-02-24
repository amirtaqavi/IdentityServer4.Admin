using Aerospike.Client;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories
{
    internal class BinList
    {
        private readonly List<Bin> _bins = new();

        private void Add(object value, Func<Bin> action)
        {
            if (value == null)
            {
                return;
            }
            _bins.Add(action.Invoke());
        }

        public void Add(string name, string[] values)
        {
            Add(values, () => new Bin(name, values));
        }

        public void Add(string name, byte[] values)
        {
            Add(values, () => new Bin(name, values));
        }

        public void Add(string name, List<byte[]> valuesList)
        {
            foreach (var values in valuesList)
            {
                Add(values, () => new Bin(name, values));
            }
        }

        public void Add(string name, string value)
        {
            Add(value, () => string.IsNullOrEmpty(value) ? new Bin(name, "NULL") : new Bin(name, value));
        }

        public void Add(string name, bool value)
        {
            Add(value, () => new Bin(name, value));
        }

        public void Add(string name, long value)
        {
            Add(value, () => new Bin(name, value));
        }

        public void Add(string name, long? value)
        {
            Add(value, () => value != null ? new Bin(name, (long)value) : new Bin(name, (long)-1));
        }

        public Bin[] ToArray()
        {
            return _bins.ToArray();
        }
    }
}