using System.Collections.Generic;

namespace Lx.Project.Unicache
{
    public interface IUniCache
    {
        int DefaultExpiresSeconds { get; }
        void Exit();
        bool Del(string key);
        bool DelAll(IEnumerable<string> keys);
        bool RemoveByPattern(string pattern);

        bool SetAll<T>(IDictionary<string, T> values, int expiresSeconds);
        bool Set<T>(string key, T value, int expiresSeconds);
        bool SetWithDefaultExpiresSeconds<T>(string key, T value);

        bool Get<T>(string key, out bool hasValue, out T value);
        T Get<T>(string key);
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);
    }

}

