namespace SignService.Extention
{
    public static class JsonExtention
    {
        public static string ToJson(this object str)
        {
            return System.Text.Json.JsonSerializer.Serialize(str);
        }
    }
}
