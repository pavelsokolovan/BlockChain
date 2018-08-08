using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain
{
    public class Key
    {
        public RSAParameters Value { get; private set; }
        public User Owner { get; private set; }

        public Key(RSAParameters value, User owner)
        {
            Value = value;
            Owner = owner;
        }

        public override bool Equals(object obj)
        {
            var key = obj as Key;
            return key != null &&
                   EqualityComparer<RSAParameters>.Default.Equals(Value, key.Value);
        }

        public override string ToString()
        {
            return Owner.ToString() + " key";
        }
    }
}
