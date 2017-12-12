using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSConsole
{
    class Result
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int NameLength { get; set; }
        public int ValueLength { get; set; }

        public Result()
        {

        }

        public Result(string PropertyName, string PropertyValue)
        {
            Name = PropertyName;
            Value = PropertyValue;
            NameLength = PropertyName.Length;
            ValueLength = PropertyValue.Length;
        }
    }
}
