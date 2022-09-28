using Konstrukt.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Site.ValueMappers;
public class EnumDropdownValueMapper<TEnumType> : KonstruktValueMapper
    where TEnumType : struct, Enum
{
    public override object ModelToEditor(object input)
    {
        var bob = (TEnumType)Enum.Parse(typeof(TEnumType), input.ToString());
        return bob;
    }

    public override object EditorToModel(object input)
    {
        return input.ToString();
    }
}
