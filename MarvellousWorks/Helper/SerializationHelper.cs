using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public enum FormatterType
    {
        Soap,
        Binary
    }
    public static class SerializationHelper
    {
        private const FormatterType DefaultFormatterType = FormatterType.Binary;
        
        private static IFormatter GetFormatter(FormatterType formatterType)
        {
            switch (formatterType)
            {
                case FormatterType.Binary:return new BinaryFormatter();
               
            }
            throw new NotSupportedException();
        }
    }
}
