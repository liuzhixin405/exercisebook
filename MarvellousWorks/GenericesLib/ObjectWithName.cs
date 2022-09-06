namespace GenericesLib
{
    public class ObjectWithName
    {
        private string name;
        public ObjectWithName(string name)
        {
            this.name = name;
        }
        public override string ToString()
        {
            return name;
        }
    }
}