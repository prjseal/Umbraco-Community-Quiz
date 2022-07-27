using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quiz.Site.Services
{
    public interface IDataTypeValueService
    {
        IEnumerable<SelectListItem> GetItemsFromValueListDataType(string dataTypeName, string[] selectedValues);
    }
}
