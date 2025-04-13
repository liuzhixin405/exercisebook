using spot.Application.DTOs;

namespace spot.Application.Interfaces
{
    public interface ITranslator
    {
        string this[string name]
        {
            get;
        }

        string GetString(string name);
        string GetString(TranslatorMessageDto input);
    }
}
