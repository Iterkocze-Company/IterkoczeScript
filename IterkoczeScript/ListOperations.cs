namespace IterkoczeScript
{
    public static class ListOperations {

        public static int Invoke(List<object?> List, object? val, string operation) {
            switch (operation) {
                case "Add":
                    List.Add(val);
                    break;
                case "Remove":
                    List.Remove(val);
                    break;
                case "IndexOf":
                    return List.IndexOf(val);
            }
            return 0;
        }
    }
}
