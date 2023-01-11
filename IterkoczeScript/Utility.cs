namespace IterkoczeScript;
    public static class Utility {
        public static int GetRealArrayLenght(Array array) {
            int count = 0;
            foreach (var element in array) {
                if (element != null) {
                    count++;
                }
            }
            return count;
        }
    }
