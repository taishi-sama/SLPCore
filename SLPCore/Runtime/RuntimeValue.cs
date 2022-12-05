using SLPCore.Types;

namespace SLPCore
{
    public struct RuntimeValue
    {
        public TypeSLP langType { get; set; }
        public object value;
        public RuntimeValue(TypeSLP langType, object value = null)
        {
            this.langType = langType;
            this.value = value;
        }
    }
    
}