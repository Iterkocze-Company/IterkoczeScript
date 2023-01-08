namespace IterkoczeScript
{
    public static class ListOperations {
        public static void Add( List<object?> List, object? val) {
            List.Add(val);
        }
        public static void Remove(List<object?> List, object? val) {
            List.Remove(val);
        }
        public static int IndefOf(List<object?> List, object? val) {
            return List.IndexOf(val);
        }
    }
}
