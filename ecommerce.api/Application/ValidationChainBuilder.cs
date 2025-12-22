namespace ECommerce.API.Application
{
    // 验证链构建器
    public class ValidationChainBuilder<T>
    {
        private IValidationHandler<T> _firstHandler;
        private IValidationHandler<T> _lastHandler;

        public ValidationChainBuilder<T> AddHandler(IValidationHandler<T> handler)
        {
            if (_firstHandler == null)
            {
                _firstHandler = handler;
                _lastHandler = handler;
            }
            else
            {
                // 通过反射或构造函数注入建立链
                _lastHandler = handler;
            }
            return this;
        }

        public IValidationHandler<T> Build()
        {
            return _firstHandler;
        }
    }
}